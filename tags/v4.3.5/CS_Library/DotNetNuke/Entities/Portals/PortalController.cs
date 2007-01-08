#region DotNetNuke License

// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web;
using System.Xml;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Skins;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualBasic;
using FileInfo=DotNetNuke.Services.FileSystem.FileInfo;
using Globals=DotNetNuke.Common.Globals;
using TabInfo=DotNetNuke.Entities.Tabs.TabInfo;

namespace DotNetNuke.Entities.Portals
{
    public class PortalController
    {
        /// <summary>
        /// Creates a new portal.
        /// </summary>
        /// <param name="PortalName">Name of the portal to be created</param>
        /// <param name="FirstName">Portal Administrator's first name</param>
        /// <param name="LastName">Portal Administrator's last name</param>
        /// <param name="Username">Portal Administrator's username</param>
        /// <param name="Password">Portal Administrator's password</param>
        /// <param name="Email">Portal Administrator's email</param>
        /// <param name="Description">Description for the new portal</param>
        /// <param name="KeyWords">KeyWords for the new portal</param>
        /// <param name="TemplatePath">Path where the templates are stored</param>
        /// <param name="TemplateFile">Template file</param>
        /// <param name="PortalAlias">Portal Alias String</param>
        /// <param name="ServerPath">The Path to the root of the Application</param>
        /// <param name="ChildPath">The Path to the Child Portal Folder</param>
        /// <param name="IsChildPortal">True if this is a child portal</param>
        /// <returns>PortalId of the new portal if there are no errors, -1 otherwise.</returns>
        /// <remarks>
        /// After the selected portal template is parsed the admin template ("admin.template") will be
        /// also processed. The admin template should only contain the "Admin" menu since it's the same
        /// on all portals. The selected portal template can contain a <settings/> node to specify portal
        /// properties and a <roles/> node to define the roles that will be created on the portal by default.
        /// </remarks>
        public int CreatePortal( string PortalName, string FirstName, string LastName, string Username, string Password, string Email, string Description, string KeyWords, string TemplatePath, string TemplateFile, string HomeDirectory, string PortalAlias, string ServerPath, string ChildPath, bool IsChildPortal )
        {
            FolderController objFolderController = new FolderController();
            string strMessage = Null.NullString;
            int AdministratorId = Null.NullInteger;
            UserInfo objAdminUser = new UserInfo();

            //Attempt to create a new portal
            int intPortalId = CreatePortal( PortalName, HomeDirectory );

            if( intPortalId != -1 )
            {
                if( HomeDirectory == "" )
                {
                    HomeDirectory = "Portals/" + intPortalId;
                }
                string MappedHomeDirectory = objFolderController.GetMappedDirectory( Globals.ApplicationPath + "/" + HomeDirectory + "/" );

                strMessage += CreateProfileDefinitions( intPortalId, TemplatePath, TemplateFile );
                if( strMessage == Null.NullString )
                {
                    // add administrator
                    try
                    {
                        objAdminUser.PortalID = intPortalId;
                        objAdminUser.FirstName = FirstName;
                        objAdminUser.LastName = LastName;
                        objAdminUser.Username = Username;
                        objAdminUser.DisplayName = FirstName + " " + LastName;
                        objAdminUser.Membership.Password = Password;
                        objAdminUser.Email = Email;
                        objAdminUser.IsSuperUser = false;
                        objAdminUser.Membership.Approved = true;

                        objAdminUser.Profile.FirstName = FirstName;
                        objAdminUser.Profile.LastName = LastName;

                        UserCreateStatus createStatus = UserController.CreateUser( ref objAdminUser );

                        if( createStatus == UserCreateStatus.Success )
                        {
                            AdministratorId = objAdminUser.UserID;
                        }
                        else
                        {
                            strMessage += UserController.GetUserCreateStatus( createStatus );
                        }
                    }
                    catch( Exception exc )
                    {
                        strMessage += Localization.GetString( "CreateAdminUser.Error" ) + exc.Message + exc.StackTrace;
                    }
                }
                else
                {
                    throw ( new Exception( strMessage ) );
                }

                if( strMessage == "" && AdministratorId > 0 )
                {
                    try
                    {
                        // the upload directory may already exist if this is a new DB working with a previously installed application
                        if( Directory.Exists( MappedHomeDirectory ) )
                        {
                            Globals.DeleteFolderRecursive( MappedHomeDirectory );
                        }
                    }
                    catch( Exception exc )
                    {
                        strMessage += Localization.GetString( "DeleteUploadFolder.Error" ) + exc.Message + exc.StackTrace;
                    }

                    //Set up Child Portal
                    if( strMessage == Null.NullString )
                    {
                        try
                        {
                            if( IsChildPortal )
                            {
                                // create the subdirectory for the new portal
                                if( !Directory.Exists( ChildPath ) )
                                {
                                    Directory.CreateDirectory( ChildPath );
                                }

                                // create the subhost default.aspx file
                                if( !File.Exists( ChildPath + "\\" + Globals.glbDefaultPage ) )
                                {
                                    File.Copy( Globals.HostMapPath + "subhost.aspx", ChildPath + "\\" + Globals.glbDefaultPage );
                                }
                            }
                        }
                        catch( Exception exc )
                        {
                            strMessage += Localization.GetString( "ChildPortal.Error" ) + exc.Message + exc.StackTrace;
                        }
                    }
                    else
                    {
                        throw ( new Exception( strMessage ) );
                    }

                    if( strMessage == Null.NullString )
                    {
                        try
                        {
                            // create the upload directory for the new portal
                            Directory.CreateDirectory( MappedHomeDirectory );

                            // copy the default stylesheet to the upload directory
                            File.Copy( Globals.HostMapPath + "portal.css", MappedHomeDirectory + "portal.css" );

                            // process zip resource file if present
                            ProcessResourceFile( MappedHomeDirectory, TemplatePath + TemplateFile );
                        }
                        catch( Exception exc )
                        {
                            strMessage += Localization.GetString( "ChildPortal.Error" ) + exc.Message + exc.StackTrace;
                        }
                    }
                    else
                    {
                        throw ( new Exception( strMessage ) );
                    }

                    if( strMessage == Null.NullString )
                    {
                        // parse portal template
                        try
                        {
                            ParseTemplate( intPortalId, TemplatePath, TemplateFile, AdministratorId, PortalTemplateModuleAction.Replace, true );
                        }
                        catch( Exception exc )
                        {
                            strMessage += Localization.GetString( "PortalTemplate.Error" ) + exc.Message + exc.StackTrace;
                            ;
                        }

                        // parse admin template
                        try
                        {
                            ParseTemplate( intPortalId, TemplatePath, "admin.template", AdministratorId, PortalTemplateModuleAction.Replace, true );
                        }
                        catch( Exception exc )
                        {
                            strMessage += Localization.GetString( "AdminTemplate.Error" ) + exc.Message + exc.StackTrace;
                            ;
                        }
                    }
                    else
                    {
                        throw ( new Exception( strMessage ) );
                    }

                    if( strMessage == Null.NullString )
                    {
                        // update portal setup
                        PortalInfo objportal = GetPortal( intPortalId );

                        // update portal info
                        objportal.Description = Description;
                        objportal.KeyWords = KeyWords;
                        UpdatePortalInfo( objportal.PortalID, objportal.PortalName, objportal.LogoFile, objportal.FooterText, objportal.ExpiryDate, objportal.UserRegistration, objportal.BannerAdvertising, objportal.Currency, objportal.AdministratorId, objportal.HostFee, objportal.HostSpace, objportal.PaymentProcessor, objportal.ProcessorUserId, objportal.ProcessorPassword, objportal.Description, objportal.KeyWords, objportal.BackgroundFile, objportal.SiteLogHistory, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.UserTabId, objportal.DefaultLanguage, objportal.TimeZoneOffset, objportal.HomeDirectory );

                        //Update Administrators Locale/TimeZone
                        objAdminUser.Profile.PreferredLocale = objportal.DefaultLanguage;
                        objAdminUser.Profile.TimeZone = objportal.TimeZoneOffset;

                        //Save Admin User
                        UserController.UpdateUser( objportal.PortalID, objAdminUser );

                        //clear portal alias cache
                        DataCache.ClearHostCache( true );

                        // clear roles cache
                        DataCache.RemoveCache( "GetRoles" );

                        //Create Portal Alias
                        AddPortalAlias( intPortalId, PortalAlias );

                        // log installation event
                        try
                        {
                            LogInfo objEventLogInfo = new LogInfo();
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString();
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString();
                            objEventLogInfo.LogProperties.Add( new LogDetailInfo( "Install Portal:", PortalName ) );
                            objEventLogInfo.LogProperties.Add( new LogDetailInfo( "FirstName:", FirstName ) );
                            objEventLogInfo.LogProperties.Add( new LogDetailInfo( "LastName:", LastName ) );
                            objEventLogInfo.LogProperties.Add( new LogDetailInfo( "Username:", Username ) );
                            objEventLogInfo.LogProperties.Add( new LogDetailInfo( "Email:", Email ) );
                            objEventLogInfo.LogProperties.Add( new LogDetailInfo( "Description:", Description ) );
                            objEventLogInfo.LogProperties.Add( new LogDetailInfo( "Keywords:", KeyWords ) );
                            objEventLogInfo.LogProperties.Add( new LogDetailInfo( "TemplatePath:", TemplatePath ) );
                            objEventLogInfo.LogProperties.Add( new LogDetailInfo( "TemplateFile:", TemplateFile ) );
                            objEventLogInfo.LogProperties.Add( new LogDetailInfo( "HomeDirectory:", HomeDirectory ) );
                            objEventLogInfo.LogProperties.Add( new LogDetailInfo( "PortalAlias:", PortalAlias ) );
                            objEventLogInfo.LogProperties.Add( new LogDetailInfo( "ServerPath:", ServerPath ) );
                            objEventLogInfo.LogProperties.Add( new LogDetailInfo( "ChildPath:", ChildPath ) );
                            objEventLogInfo.LogProperties.Add( new LogDetailInfo( "IsChildPortal:", IsChildPortal.ToString() ) );
                            EventLogController objEventLog = new EventLogController();
                            objEventLog.AddLog( objEventLogInfo );
                        }
                        catch( Exception )
                        {
                            // error
                        }
                    }
                    else
                    {
                        throw ( new Exception( strMessage ) );
                    }
                }
                else // clean up
                {
                    DeletePortalInfo( intPortalId );
                    throw ( new Exception( strMessage ) );
                }
            }
            else
            {
                strMessage += Localization.GetString( "CreatePortal.Error" );
                throw ( new Exception( strMessage ) );
            }

            return intPortalId;
        }

        /// <summary>
        /// Creates a new portal based on the portal template provided.
        /// </summary>
        /// <param name="portalName">Name of the portal to be created</param>
        /// <returns>PortalId of the new portal if there are no errors, -1 otherwise.</returns>
        /// <param name="homeDirectory"></param>
        private int CreatePortal( string portalName, string homeDirectory )
        {
            // add portal
            int PortalId = -1;
            try
            {
                // Use host settings as default values for these parameters
                // This can be overwritten on the portal template
                DateTime datExpiryDate;
                if( Convert.ToString( Globals.HostSettings["DemoPeriod"] ) != "" )
                {
                    datExpiryDate = Convert.ToDateTime( Globals.GetMediumDate( DateAndTime.DateAdd( DateInterval.Day, int.Parse( Convert.ToString( Globals.HostSettings["DemoPeriod"] ) ), DateTime.Now ).ToString() ) );
                }
                else
                {
                    datExpiryDate = Null.NullDate;
                }

                double dblHostFee = 0;
                if( Convert.ToString( Globals.HostSettings["HostFee"] ) != "" )
                {
                    dblHostFee = Convert.ToDouble( Globals.HostSettings["HostFee"] );
                }

                double dblHostSpace = 0;
                if( Convert.ToString( Globals.HostSettings["HostSpace"] ) != "" )
                {
                    dblHostSpace = Convert.ToDouble( Globals.HostSettings["HostSpace"] );
                }

                int intSiteLogHistory = -1;
                if( Convert.ToString( Globals.HostSettings["SiteLogHistory"] ) != "" )
                {
                    intSiteLogHistory = Convert.ToInt32( Globals.HostSettings["SiteLogHistory"] );
                }

                string strCurrency = Convert.ToString( Globals.HostSettings["HostCurrency"] );
                if( strCurrency == "" )
                {
                    strCurrency = "USD";
                }
                PortalId = DataProvider.Instance().CreatePortal( portalName, strCurrency, datExpiryDate, dblHostFee, dblHostSpace, intSiteLogHistory, homeDirectory );
            }
            catch
            {
                // error creating portal
            }

            return PortalId;
        }

        private string CreateProfileDefinitions( int portalId, string templatePath, string templateFile )
        {
            string strMessage = Null.NullString;
            try
            {
                // add profile definitions
                XmlDocument xmlDoc = new XmlDocument();
                XmlNode node;
                // open the XML template file
                try
                {
                    xmlDoc.Load( templatePath + templateFile );
                }
                catch
                {
                    // error
                }

                // parse profile definitions if available
                node = xmlDoc.SelectSingleNode( "//portal/profiledefinitions" );
                if( node != null )
                {
                    ParseProfileDefinitions( node, portalId );
                }
                else // template does not contain profile definitions ( ie. was created prior to DNN 3.3.0 )
                {
                    ProfileController.AddDefaultDefinitions( portalId );
                }
            }
            catch( Exception )
            {
                strMessage = Localization.GetString( "CreateProfileDefinitions.Error" );
            }

            return strMessage;
        }

        private int CreateRole( int PortalId, string roleName, string description, float serviceFee, int billingPeriod, string billingFrequency, float trialFee, int trialPeriod, string trialFrequency, bool isPublic, bool isAuto )
        {
            RoleController objRoleController = new RoleController();
            int RoleId;

            //First check if the role exists
            RoleInfo objRoleInfo = objRoleController.GetRoleByName( PortalId, roleName );

            if( objRoleInfo == null )
            {
                objRoleInfo = new RoleInfo();
                objRoleInfo.PortalID = PortalId;
                objRoleInfo.RoleName = roleName;
                objRoleInfo.RoleGroupID = Null.NullInteger;
                objRoleInfo.Description = description;
                objRoleInfo.ServiceFee = Convert.ToSingle( serviceFee < 0 ? 0 : serviceFee );
                objRoleInfo.BillingPeriod = billingPeriod;
                objRoleInfo.BillingFrequency = billingFrequency;
                objRoleInfo.TrialFee = Convert.ToSingle( trialFee < 0 ? 0 : trialFee );
                objRoleInfo.TrialPeriod = trialPeriod;
                objRoleInfo.TrialFrequency = trialFrequency;
                objRoleInfo.IsPublic = isPublic;
                objRoleInfo.AutoAssignment = isAuto;
                RoleId = objRoleController.AddRole( objRoleInfo );
            }
            else
            {
                RoleId = objRoleInfo.RoleID;
            }

            return RoleId;
        }

        public static PortalSettings GetCurrentPortalSettings()
        {
            return ( (PortalSettings)HttpContext.Current.Items["PortalSettings"] );
        }

        /// <summary>
        /// Gets information of a portal
        /// </summary>
        /// <param name="PortalId">Id of the portal</param>
        /// <returns>PortalInfo object with portal definition</returns>
        /// <remarks>
        /// </remarks>
        public PortalInfo GetPortal( int PortalId )
        {
            return ( (PortalInfo)CBO.FillObject( DataProvider.Instance().GetPortal( PortalId ), typeof( PortalInfo ) ) );
        }

        /// <summary>
        /// Gets information from all portals
        /// </summary>
        /// <returns>ArrayList of PortalInfo objects</returns>
        /// <remarks>
        /// </remarks>
        public ArrayList GetPortals()
        {
            return CBO.FillCollection( DataProvider.Instance().GetPortals(), typeof( PortalInfo ) );
        }

        /// <summary>
        /// Gets the space used by a portal in bytes
        /// </summary>
        /// <param name="portalId">Id of the portal</param>
        ///  <returns>Space used in bytes</returns>
        /// <remarks>
        /// If PortalId is -1 or not  present (defaults to -1) the host space (\Portals\_default\) will be returned.
        /// </remarks>
        [Obsolete( "This function has been replaced by GetPortalSpaceUsedBytes" )]
        public int GetPortalSpaceUsed( int portalId )
        {
            int size;
            try
            {
                size = Convert.ToInt32( GetPortalSpaceUsedBytes( portalId ) );
            }
            catch( Exception )
            {
                size = int.MaxValue;
            }

            return size;
        }

        /// <summary>
        /// Gets the space used at the host level
        /// </summary>
        /// <returns>Space used in bytes</returns>
        /// <remarks>
        /// </remarks>
        public long GetPortalSpaceUsedBytes()
        {
            return GetPortalSpaceUsedBytes( -1 );
        }

        /// <summary>
        /// Gets the space used by a portal in bytes
        /// </summary>
        /// <param name="portalId">Id of the portal</param>
        /// <returns>Space used in bytes</returns>
        /// <remarks>
        /// </remarks>
        public long GetPortalSpaceUsedBytes( int portalId )
        {
            long size = 0;

            IDataReader dr = DataProvider.Instance().GetPortalSpaceUsed( portalId );
            if( dr.Read() )
            {
                if( !Information.IsDBNull( dr["SpaceUsed"] ) )
                {
                    size = Convert.ToInt64( dr["SpaceUsed"] );
                }
            }
            dr.Close();

            return size;
        }

        /// <summary>
        /// Verifies if there's enough space to upload a new file on the given portal
        /// </summary>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="fileSizeBytes">Size of the file being uploaded</param>
        /// <returns>True if there's enough space available to upload the file</returns>
        public bool HasSpaceAvailable( int portalId, long fileSizeBytes )
        {
            int hostSpace;

            if( portalId == -1 )
            {
                hostSpace = 0;
            }
            else
            {
                PortalSettings ps = GetCurrentPortalSettings();
                if( ps != null && ps.PortalId == portalId )
                {
                    hostSpace = ps.HostSpace;
                }
                else
                {
                    PortalInfo portal = GetPortal( portalId );
                    hostSpace = portal.HostSpace;
                }
            }

            return ( ( ( ( GetPortalSpaceUsedBytes( portalId ) + fileSizeBytes )/Math.Pow( 1024, 2 ) ) <= hostSpace ) | hostSpace == 0 );
        }

        private string ImportFile( int portalId, string url )
        {
            string strUrl = url;

            if( Globals.GetURLType( url ) == TabType.File )
            {
                FileController objFileController = new FileController();
                int fileId = objFileController.ConvertFilePathToFileId( url, portalId );
                if( fileId >= 0 )
                {
                    strUrl = "FileID=" + fileId;
                }
            }

            return strUrl;
        }

        /// <summary>
        /// Processes a node for a Role and creates a new Role based on the information gathered from the node
        /// </summary>
        /// <param name="nodeRole">Template file node for the role</param>
        /// <param name="portalId">PortalId of the new portal</param>
        /// <returns>RoleId of the created role</returns>
        private int ParseRole( XmlNode nodeRole, int portalId )
        {
            string RoleName = XmlUtils.GetNodeValue( nodeRole, "rolename", "" );
            string Description = XmlUtils.GetNodeValue( nodeRole, "description", "" );
            float ServiceFee = XmlUtils.GetNodeValueSingle( nodeRole, "servicefee", 0 );
            int BillingPeriod = XmlUtils.GetNodeValueInt( nodeRole, "billingperiod", 0 );
            string BillingFrequency = XmlUtils.GetNodeValue( nodeRole, "billingfrequency", "M" );
            float TrialFee = XmlUtils.GetNodeValueSingle( nodeRole, "trialfee", 0 );
            int TrialPeriod = XmlUtils.GetNodeValueInt( nodeRole, "trialperiod", 0 );
            string TrialFrequency = XmlUtils.GetNodeValue( nodeRole, "trialfrequency", "N" );
            bool IsPublic = XmlUtils.GetNodeValueBoolean( nodeRole, "ispublic", false );
            bool AutoAssignment = XmlUtils.GetNodeValueBoolean( nodeRole, "autoassignment", false );

            //Call Create Role
            return CreateRole( portalId, RoleName, Description, ServiceFee, BillingPeriod, BillingFrequency, TrialFee, TrialPeriod, TrialFrequency, IsPublic, AutoAssignment );
        }

        /// <summary>
        /// Processes a single tab from the template
        /// </summary>
        /// <param name="nodeTab">Template file node for the tabs</param>
        /// <param name="portalId">PortalId of the new portal</param>
        /// <param name="isAdminTemplate">True when processing admin template, false when processing portal template</param>
        /// <param name="mergeTabs">Flag to determine whether Module content is merged.</param>
        /// <param name="hModules">Used to control if modules are true modules or instances</param>
        /// <param name="hTabs">Supporting object to build the tab hierarchy</param>
        /// <param name="isNewPortal">Flag to determine is the template is applied to an existing portal or a new one.</param>
        /// <remarks>
        /// When a special tab is found (HomeTab, UserTab, LoginTab, AdminTab) portal information will be updated.
        /// </remarks>
        private int ParseTab( XmlNode nodeTab, int portalId, bool isAdminTemplate, PortalTemplateModuleAction mergeTabs, ref Hashtable hModules, ref Hashtable hTabs, bool isNewPortal )
        {
            TabInfo objTab = null;
            TabController objTabs = new TabController();
            string strName = XmlUtils.GetNodeValue( nodeTab, "name", "" );
            int intTabId = 0;
            PortalInfo objportal;
            bool tabExists = true;

            objportal = GetPortal( portalId );

            if( !String.IsNullOrEmpty( strName ) )
            {
                if( !isNewPortal ) // running from wizard: try to find the tab by path
                {
                    string parenttabname = "";

                    if( XmlUtils.GetNodeValue( nodeTab, "parent", "" ) != "" )
                    {
                        parenttabname = XmlUtils.GetNodeValue( nodeTab, "parent", "" ) + "/";
                    }
                    if( hTabs[parenttabname + strName] != null )
                    {
                        objTab = objTabs.GetTab( Convert.ToInt32( hTabs[parenttabname + strName] ) );
                    }
                }

                if( objTab == null || isNewPortal )
                {
                    tabExists = false;

                    objTab = new TabInfo();
                    intTabId = Null.NullInteger;

                    objTab.TabID = intTabId;
                    objTab.PortalID = portalId;
                    objTab.TabName = XmlUtils.GetNodeValue( nodeTab, "name", "" );
                    objTab.Title = XmlUtils.GetNodeValue( nodeTab, "title", "" );
                    objTab.Description = XmlUtils.GetNodeValue( nodeTab, "description", "" );
                    objTab.KeyWords = XmlUtils.GetNodeValue( nodeTab, "keywords", "" );
                    objTab.IsVisible = XmlUtils.GetNodeValueBoolean( nodeTab, "visible", true );
                    objTab.DisableLink = XmlUtils.GetNodeValueBoolean( nodeTab, "disabled", false );
                    objTab.IconFile = ImportFile( portalId, XmlUtils.GetNodeValue( nodeTab, "iconfile", "" ) );
                    objTab.Url = XmlUtils.GetNodeValue( nodeTab, "url", "" );
                    objTab.StartDate = XmlUtils.GetNodeValueDate( nodeTab, "startdate", Null.NullDate );
                    objTab.EndDate = XmlUtils.GetNodeValueDate( nodeTab, "enddate", Null.NullDate );
                    objTab.RefreshInterval = XmlUtils.GetNodeValueInt( nodeTab, "refreshinterval", Null.NullInteger );
                    objTab.PageHeadText = XmlUtils.GetNodeValue( nodeTab, "pageheadtext", Null.NullString );

                    XmlNodeList nodeTabPermissions = nodeTab.SelectNodes( "tabpermissions/permission" );
                    objTab.TabPermissions = ParseTabPermissions( nodeTabPermissions, objportal, objTab.TabID, isAdminTemplate );

                    // set tab skin and container
                    if( XmlUtils.GetNodeValue( nodeTab, "skinsrc", "" ) != "" )
                    {
                        objTab.SkinSrc = XmlUtils.GetNodeValue( nodeTab, "skinsrc", "" );
                    }
                    if( XmlUtils.GetNodeValue( nodeTab, "containersrc", "" ) != "" )
                    {
                        objTab.ContainerSrc = XmlUtils.GetNodeValue( nodeTab, "containersrc", "" );
                    }

                    // process of parent tab
                    objTab.ParentId = Null.NullInteger;
                    string tabName = objTab.TabName;

                    if( XmlUtils.GetNodeValue( nodeTab, "parent", "" ) != "" )
                    {
                        if( hTabs[XmlUtils.GetNodeValue( nodeTab, "parent", "" )] != null )
                        {
                            // parent node specifies the path (tab1/tab2/tab3), use saved tabid
                            objTab.ParentId = Convert.ToInt32( hTabs[XmlUtils.GetNodeValue( nodeTab, "parent", "" )] );
                            tabName = XmlUtils.GetNodeValue( nodeTab, "parent", "" ) + "/" + objTab.TabName;
                        }
                        else
                        {
                            // Parent node doesn't spcecify the path, search by name.
                            // Possible incoherence if tabname not unique
                            TabInfo objParent = objTabs.GetTabByName( XmlUtils.GetNodeValue( nodeTab, "parent", "" ), portalId );
                            if( objParent != null )
                            {
                                objTab.ParentId = objParent.TabID;
                                tabName = objParent.TabName + "/" + objTab.TabName;
                            }
                            else
                            {
                                // parent tab not found!
                                objTab.ParentId = Null.NullInteger;
                                tabName = objTab.TabName;
                            }
                        }
                    }
                    // create tab
                    intTabId = objTabs.AddTab( objTab );
                    // extra check for duplicate tabs in same level
                    if( hTabs[tabName] == null )
                    {
                        hTabs.Add( tabName, intTabId );
                    }
                }

                if( isAdminTemplate )
                {
                    // when processing the admin template we should identify the Admin tab
                    if( objTab.TabName == "Admin" )
                    {
                        objportal.AdminTabId = intTabId;
                        DataProvider.Instance().UpdatePortalSetup( portalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.UserTabId, objportal.AdminTabId );
                    }
                }
                else
                {
                    // when processing the portal template we can find: hometab, usertab, logintab
                    switch( XmlUtils.GetNodeValue( nodeTab, "tabtype", "" ) )
                    {
                        case "splashtab":

                            objportal.SplashTabId = intTabId;
                            DataProvider.Instance().UpdatePortalSetup( portalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.UserTabId, objportal.AdminTabId );
                            break;
                        case "hometab":

                            objportal.HomeTabId = intTabId;
                            DataProvider.Instance().UpdatePortalSetup( portalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.UserTabId, objportal.AdminTabId );
                            break;
                        case "logintab":

                            objportal.LoginTabId = intTabId;
                            DataProvider.Instance().UpdatePortalSetup( portalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.UserTabId, objportal.AdminTabId );
                            break;
                        case "usertab":

                            objportal.UserTabId = intTabId;
                            DataProvider.Instance().UpdatePortalSetup( portalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.UserTabId, objportal.AdminTabId );
                            break;
                    }
                }

                //If tab does not already exist parse modules
                if( !tabExists )
                {
                    if( nodeTab.SelectSingleNode( "panes" ) != null )
                    {
                        ParsePanes( nodeTab.SelectSingleNode( "panes" ), portalId, intTabId, mergeTabs, hModules );
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Parses tab permissions
        /// </summary>
        /// <param name="nodeTabPermissions">Node for tab permissions</param>
        /// <param name="objPortal">Portal object of new portal</param>
        /// <param name="TabId">TabId of tab being processed</param>
        /// <param name="isAdminTemplate">Flag to indicate if we are parsing admin template</param>
        private TabPermissionCollection ParseTabPermissions( XmlNodeList nodeTabPermissions, PortalInfo objPortal, int TabId, bool isAdminTemplate )
        {
            TabPermissionCollection objTabPermissions = new TabPermissionCollection();
            PermissionController objPermissionController = new PermissionController();
            RoleController objRoleController = new RoleController();
            int permissionID = 0;

            foreach (XmlNode xmlTabPermission in nodeTabPermissions)
            {
                string permissionKey = XmlUtils.GetNodeValue( xmlTabPermission, "permissionkey", "" );
                string permissionCode = XmlUtils.GetNodeValue( xmlTabPermission, "permissioncode", "" );
                string roleName = XmlUtils.GetNodeValue( xmlTabPermission, "rolename", "" );
                bool allowAccess = XmlUtils.GetNodeValueBoolean( xmlTabPermission, "allowaccess", false );
                ArrayList arrPermissions = objPermissionController.GetPermissionByCodeAndKey( permissionCode, permissionKey );

                for( int i = 0; i < arrPermissions.Count; i++ )
                {
                    PermissionInfo objPermission = (PermissionInfo)arrPermissions[i];
                    permissionID = objPermission.PermissionID;
                }
                int roleId = int.MinValue;
                switch( roleName )
                {
                    case Globals.glbRoleAllUsersName:

                        roleId = Convert.ToInt32( Globals.glbRoleAllUsers );
                        break;
                    case Globals.glbRoleUnauthUserName:

                        roleId = Convert.ToInt32( Globals.glbRoleUnauthUser );
                        break;
                    default:

                        RoleInfo objRole = objRoleController.GetRoleByName( objPortal.PortalID, roleName );
                        if( objRole != null )
                        {
                            roleId = objRole.RoleID;
                        }
                        else
                        {
                            // if parsing admin.template and role administrators redefined, use portal.administratorroleid
                            if( isAdminTemplate && roleName.ToLower() == "administrators" )
                            {
                                roleId = objPortal.AdministratorRoleId;
                            }
                        }
                        break;
                }

                // if role was found add, otherwise ignore
                if( roleId != int.MinValue )
                {
                    TabPermissionInfo objTabPermission = new TabPermissionInfo();
                    objTabPermission.TabID = TabId;
                    objTabPermission.PermissionID = permissionID;
                    objTabPermission.RoleID = roleId;
                    objTabPermission.AllowAccess = allowAccess;
                    objTabPermissions.Add( objTabPermission );
                }
            }
            return objTabPermissions;
        }

        /// <summary>
        /// Creates a new portal alias
        /// </summary>
        /// <param name="PortalId">Id of the portal</param>
        /// <param name="PortalAlias">Portal Alias to be created</param>
        /// <remarks>
        /// </remarks>
        public void AddPortalAlias( int PortalId, string PortalAlias )
        {
            PortalAliasController objPortalAliasController = new PortalAliasController();

            //Check if the Alias exists
            PortalAliasInfo objPortalAliasInfo = objPortalAliasController.GetPortalAlias( PortalAlias, PortalId );

            //If alias does not exist add new
            if( objPortalAliasInfo == null )
            {
                objPortalAliasInfo = new PortalAliasInfo();
                objPortalAliasInfo.PortalID = PortalId;
                objPortalAliasInfo.HTTPAlias = PortalAlias;
                objPortalAliasController.AddPortalAlias( objPortalAliasInfo );
            }
        }

        /// <summary>
        /// Deletes a portal permanently
        /// </summary>
        /// <param name="PortalId">PortalId of the portal to be deleted</param>
        /// <remarks>
        /// </remarks>
        public void DeletePortalInfo( int PortalId )
        {
            // remove skin assignments
            SkinController.SetSkin( SkinInfo.RootSkin, PortalId, SkinType.Portal, "" );
            SkinController.SetSkin( SkinInfo.RootContainer, PortalId, SkinType.Portal, "" );
            SkinController.SetSkin( SkinInfo.RootSkin, PortalId, SkinType.Admin, "" );
            SkinController.SetSkin( SkinInfo.RootContainer, PortalId, SkinType.Admin, "" );

            // remove portal modules
            ModuleController objModules = new ModuleController();
            foreach( ModuleInfo objModule in objModules.GetModules( PortalId ) )
            {
                objModules.DeleteModule( objModule.ModuleID );
            }

            //delete portal users
            UserController.DeleteUsers( PortalId, false, true );

            //delete portal
            DataProvider.Instance().DeletePortalInfo( PortalId );

            // clear portal alias cache and entire portal
            DataCache.ClearHostCache( true );
        }

        /// <summary>
        /// Processes all Files from the template
        /// </summary>
        /// <param name="nodeFiles">Template file node for the Files</param>
        /// <param name="PortalId">PortalId of the new portal</param>
        /// <param name="objFolder"></param>
        private void ParseFiles( XmlNodeList nodeFiles, int PortalId, FolderInfo objFolder )
        {
            int FileId = 0;
            FileController objController = new FileController();

            foreach( XmlNode node in nodeFiles )
            {
                string fileName = XmlUtils.GetNodeValue( node, "filename", String.Empty );

                //First check if the file exists
                FileInfo objInfo = objController.GetFile( fileName, PortalId, objFolder.FolderID );

                if( objInfo == null )
                {
                    objInfo = new FileInfo();
                    objInfo.PortalId = PortalId;
                    objInfo.FileName = fileName;
                    objInfo.Extension = XmlUtils.GetNodeValue( node, "extension", String.Empty );
                    objInfo.Size = XmlUtils.GetNodeValueInt( node, "size", 0 );
                    objInfo.Width = XmlUtils.GetNodeValueInt( node, "width", 0 );
                    objInfo.Height = XmlUtils.GetNodeValueInt( node, "height", 0 );
                    objInfo.ContentType = XmlUtils.GetNodeValue( node, "contenttype", String.Empty );
                    objInfo.FolderId = objFolder.FolderID;
                    objInfo.Folder = objFolder.FolderPath;

                    //Save new File 
                    FileId = objController.AddFile( objInfo );
                }
                else
                {
                    //Get Id from File
                    FileId = objInfo.FileId;
                }
            }
        }

        /// <summary>
        /// Parses folder permissions
        /// </summary>
        /// <param name="nodeFolderPermissions">Node for folder permissions</param>
        /// <param name="PortalID">PortalId of new portal</param>
        /// <param name="FolderId">FolderId of folder being processed</param>
        /// <param name="folderPath"></param>
        private void ParseFolderPermissions( XmlNodeList nodeFolderPermissions, int PortalID, int FolderId, string folderPath )
        {
            PermissionController objPermissionController = new PermissionController();
            RoleController objRoleController = new RoleController();
            int permissionId = 0;

            foreach( XmlNode xmlFolderPermission in nodeFolderPermissions )
            {
                string permissionKey = XmlUtils.GetNodeValue( xmlFolderPermission, "permissionkey", "" );
                string permissionCode = XmlUtils.GetNodeValue( xmlFolderPermission, "permissioncode", "" );
                string roleName = XmlUtils.GetNodeValue( xmlFolderPermission, "rolename", "" );
                bool allowAccess = XmlUtils.GetNodeValueBoolean( xmlFolderPermission, "allowaccess", false );
                ArrayList arrPermissions = objPermissionController.GetPermissionByCodeAndKey( permissionCode, permissionKey );

                for( int i = 0; i < arrPermissions.Count; i++ )
                {
                    PermissionInfo objPermission = (PermissionInfo)arrPermissions[i];
                    permissionId = objPermission.PermissionID;
                }
                int roleID = int.MinValue;
                switch( roleName )
                {
                    case Globals.glbRoleAllUsersName:

                        roleID = Convert.ToInt32( Globals.glbRoleAllUsers );
                        break;
                    case Globals.glbRoleUnauthUserName:

                        roleID = Convert.ToInt32( Globals.glbRoleUnauthUser );
                        break;
                    default:

                        RoleInfo objRole = objRoleController.GetRoleByName( PortalID, roleName );
                        if( objRole != null )
                        {
                            roleID = objRole.RoleID;
                        }
                        break;
                }

                // if role was found add, otherwise ignore
                if( roleID != int.MinValue )
                {
                    if( allowAccess )
                    {
                        FileSystemUtils.SetFolderPermission( PortalID, FolderId, permissionId, roleID, folderPath );
                    }
                }
            }
        }

        /// <summary>
        /// Processes all Folders from the template
        /// </summary>
        /// <param name="nodeFolders">Template file node for the Folders</param>
        /// <param name="PortalId">PortalId of the new portal</param>
        private void ParseFolders( XmlNode nodeFolders, int PortalId )
        {
            FolderController objController = new FolderController();

            foreach( XmlNode node in nodeFolders.SelectNodes( "//folder" ) )
            {
                string folderPath = XmlUtils.GetNodeValue( node, "folderpath", String.Empty );

                //First check if the folder exists
                FolderInfo objInfo = objController.GetFolder( PortalId, folderPath );

                int folderId;
                if( objInfo == null )
                {
                    bool isProtected = FileSystemUtils.DefaultProtectedFolders( folderPath );
                    int storageLocation;
                    if( isProtected )
                    {
                        // protected folders must use insecure storage
                        storageLocation = (int)FolderController.StorageLocationTypes.InsecureFileSystem;
                    }
                    else
                    {
                        storageLocation = Convert.ToInt32( XmlUtils.GetNodeValue( node, "storagelocation", "0" ) );
                        isProtected = Convert.ToBoolean( XmlUtils.GetNodeValue( node, "isprotected", "0" ) );
                    }
                    //Save new folder 
                    folderId = objController.AddFolder( PortalId, folderPath, storageLocation, isProtected, false );
                    objInfo = objController.GetFolder( PortalId, folderPath );
                }
                else
                {
                    //Get Id from Folder
                    folderId = objInfo.FolderID;
                }

                XmlNodeList nodeFolderPermissions = node.SelectNodes( "folderpermissions/permission" );
                ParseFolderPermissions( nodeFolderPermissions, PortalId, folderId, folderPath );

                XmlNodeList nodeFiles = node.SelectNodes( "files/file" );
                if( folderPath != "" )
                {
                    folderPath += "/";
                }
                ParseFiles( nodeFiles, PortalId, objInfo );
            }
        }

        private void ParseModulePermissions( XmlNodeList nodeModulePermissions, int PortalId, int ModuleID )
        {
            RoleController objRoleController = new RoleController();
            ModulePermissionController objModulePermissionController = new ModulePermissionController();
            PermissionController objPermissionController = new PermissionController();

            foreach( XmlNode node in nodeModulePermissions )
            {
                string permissionKey = XmlUtils.GetNodeValue( node, "permissionkey", "" );
                string permissionCode = XmlUtils.GetNodeValue( node, "permissioncode", "" );
                string roleName = XmlUtils.GetNodeValue( node, "rolename", "" );
                bool allowAccess = XmlUtils.GetNodeValueBoolean( node, "allowaccess", false );

                int roleID = int.MinValue;
                switch( roleName )
                {
                    case Globals.glbRoleAllUsersName:

                        roleID = Convert.ToInt32( Globals.glbRoleAllUsers );
                        break;
                    case Globals.glbRoleUnauthUserName:

                        roleID = Convert.ToInt32( Globals.glbRoleUnauthUser );
                        break;
                    default:

                        RoleInfo objRole = objRoleController.GetRoleByName( PortalId, roleName );
                        if( objRole != null )
                        {
                            roleID = objRole.RoleID;
                        }
                        break;
                }
                if( roleID != int.MinValue )
                {
                    int permissionID = -1;
                    ArrayList arrPermissions = objPermissionController.GetPermissionByCodeAndKey( permissionCode, permissionKey );

                    for( int i = 0; i < arrPermissions.Count; i++ )
                    {
                        PermissionInfo objPermission = (PermissionInfo)arrPermissions[i];
                        permissionID = objPermission.PermissionID;
                    }

                    // if role was found add, otherwise ignore
                    if( permissionID != -1 )
                    {
                        ModulePermissionInfo objModulePermission = new ModulePermissionInfo();
                        objModulePermission.ModuleID = ModuleID;
                        objModulePermission.PermissionID = permissionID;
                        objModulePermission.RoleID = roleID;
                        objModulePermission.AllowAccess = Convert.ToBoolean( XmlUtils.GetNodeValue( node, "allowaccess", "" ) );
                        objModulePermissionController.AddModulePermission( objModulePermission );
                    }
                }
            }
        }

        /// <summary>
        /// Processes all panes and modules in the template file
        /// </summary>
        /// <param name="nodePanes">Template file node for the panes is current tab</param>
        /// <param name="PortalId">PortalId of the new portal</param>
        /// <param name="TabId">Tab being processed</param>
        /// <remarks>
        /// </remarks>
        /// <param name="mergeTabs"></param>
        /// <param name="hModules"></param>
        public void ParsePanes( XmlNode nodePanes, int PortalId, int TabId, PortalTemplateModuleAction mergeTabs, Hashtable hModules )
        {
            //Dim objDesktopModules As New DesktopModuleController
            ModuleDefinitionController objModuleDefinitions = new ModuleDefinitionController();
            ModuleController objModules = new ModuleController();
            ArrayList arrModules = objModules.GetPortalTabModules( PortalId, TabId );
            ModuleInfo objModule;

            int iModule;
            string modTitle;

            PortalInfo objportal;
            objportal = GetPortal( PortalId );

            //If Mode is Replace remove all the modules already on this Tab
            if( mergeTabs == PortalTemplateModuleAction.Replace )
            {
                for( iModule = 0; iModule < arrModules.Count; iModule++ )
                {
                    objModule = (ModuleInfo)( arrModules[iModule] );
                    objModules.DeleteTabModule( TabId, objModule.ModuleID );
                }
            }

            // iterate through the panes
            foreach( XmlNode nodePane in nodePanes.ChildNodes )
            {
                // iterate through the modules
                if( nodePane.SelectSingleNode( "modules" ) != null )
                {
                    foreach( XmlNode nodeModule in nodePane.SelectSingleNode( "modules" ) )
                    {
                        // will be instance or module?
                        int templateModuleID = XmlUtils.GetNodeValueInt( nodeModule, "moduleID", 0 );
                        bool IsInstance = false;
                        if( templateModuleID > 0 )
                        {
                            if( hModules[templateModuleID] != null )
                            {
                                // this module has already been processed -> process as instance
                                IsInstance = true;
                            }
                        }

                        // Templates prior to v4.3.5 only have the <definition> node to define the Module Type
                        // This <definition> node was populated with the DesktopModuleInfo.ModuleName property
                        // Thus there is no mechanism to determine to which module definition the module belongs.
                        //
                        // Template from v4.3.5 on also have the <moduledefinition> element that is populated
                        // with the ModuleDefinitionInfo.FriendlyName.  Therefore the module Instance identifies
                        // which Module Definition it belongs to.

                        //Get the DesktopModule defined by the <definition> element
                        DesktopModuleInfo objDesktopModule = Globals.GetDesktopModuleByName( XmlUtils.GetNodeValue( nodeModule, "definition", String.Empty ) );
                        if( objDesktopModule != null )
                        {
                            //Get the moduleDefinition from the <moduledefinition> element
                            string friendlyName = XmlUtils.GetNodeValue( nodeModule, "moduledefinition", String.Empty );

                            ModuleDefinitionInfo objModuleDefinition;
                            if( string.IsNullOrEmpty( friendlyName ) )
                            {
                                //Module is pre 4.3.5 so get the first Module Definition (at least it won't throw an error then)
                                ArrayList arrModuleDefinitions = objModuleDefinitions.GetModuleDefinitions( objDesktopModule.DesktopModuleID );
                                objModuleDefinition = (ModuleDefinitionInfo)( arrModuleDefinitions[0] );
                            }
                            else
                            {
                                //Module is 4.3.5 or later so get the Module Defeinition by its friendly name
                                objModuleDefinition = objModuleDefinitions.GetModuleDefinitionByName( objDesktopModule.DesktopModuleID, friendlyName );
                            }

                            if( objModuleDefinition != null )
                            {
                                //If Mode is Merge Check if Module exists
                                bool moduleFound = false;
                                modTitle = XmlUtils.GetNodeValue( nodeModule, "title", String.Empty );
                                if( mergeTabs == PortalTemplateModuleAction.Merge )
                                {
                                    for( iModule = 0; iModule < arrModules.Count; iModule++ )
                                    {
                                        objModule = (ModuleInfo)( arrModules[iModule] );
                                        if( modTitle == objModule.ModuleTitle )
                                        {
                                            moduleFound = true;
                                            break;
                                        }
                                    }
                                }

                                if( moduleFound == false )
                                {
                                    //Create New Module
                                    objModule = new ModuleInfo();
                                    objModule.PortalID = PortalId;
                                    objModule.TabID = TabId;
                                    objModule.ModuleOrder = -1;
                                    objModule.ModuleTitle = modTitle;
                                    objModule.PaneName = XmlUtils.GetNodeValue( nodePane, "name", String.Empty );
                                    objModule.ModuleDefID = objModuleDefinition.ModuleDefID;
                                    objModule.CacheTime = XmlUtils.GetNodeValueInt( nodeModule, "cachetime", 0 );
                                    objModule.Alignment = XmlUtils.GetNodeValue( nodeModule, "alignment", String.Empty );
                                    objModule.IconFile = ImportFile( PortalId, XmlUtils.GetNodeValue( nodeModule, "iconfile", String.Empty ) );
                                    objModule.AllTabs = XmlUtils.GetNodeValueBoolean( nodeModule, "alltabs", false );
                                    switch( XmlUtils.GetNodeValue( nodeModule, "visibility", String.Empty ) )
                                    {
                                        case "Maximized":
                                            objModule.Visibility = VisibilityState.Maximized;
                                            break;
                                        case "Minimized":
                                            objModule.Visibility = VisibilityState.Minimized;
                                            break;
                                        case "None":
                                            objModule.Visibility = VisibilityState.None;
                                            break;
                                    }
                                    objModule.Color = XmlUtils.GetNodeValue( nodeModule, "color", "" );
                                    objModule.Border = XmlUtils.GetNodeValue( nodeModule, "border", "" );
                                    objModule.Header = XmlUtils.GetNodeValue( nodeModule, "header", "" );
                                    objModule.Footer = XmlUtils.GetNodeValue( nodeModule, "footer", "" );
                                    objModule.InheritViewPermissions = XmlUtils.GetNodeValueBoolean( nodeModule, "inheritviewpermissions", false );
                                    objModule.ModulePermissions = new ModulePermissionCollection();

                                    objModule.StartDate = XmlUtils.GetNodeValueDate( nodeModule, "startdate", Null.NullDate );
                                    objModule.EndDate = XmlUtils.GetNodeValueDate( nodeModule, "enddate", Null.NullDate );

                                    if( XmlUtils.GetNodeValue( nodeModule, "containersrc", "" ) != "" )
                                    {
                                        objModule.ContainerSrc = XmlUtils.GetNodeValue( nodeModule, "containersrc", "" );
                                    }
                                    objModule.DisplayTitle = XmlUtils.GetNodeValueBoolean( nodeModule, "displaytitle", true );
                                    objModule.DisplayPrint = XmlUtils.GetNodeValueBoolean( nodeModule, "displayprint", true );
                                    objModule.DisplaySyndicate = XmlUtils.GetNodeValueBoolean( nodeModule, "displaysyndicate", false );

                                    int intModuleId;
                                    if( ! IsInstance )
                                    {
                                        //Add new module
                                        intModuleId = objModules.AddModule( objModule );
                                        if( templateModuleID > 0 )
                                        {
                                            hModules.Add( templateModuleID, intModuleId );
                                        }
                                    }
                                    else
                                    {
                                        //Add instance
                                        objModule.ModuleID = Convert.ToInt32( hModules[templateModuleID] );
                                        intModuleId = objModules.AddModule( objModule );
                                    }

                                    if( XmlUtils.GetNodeValue( nodeModule, "content", String.Empty ) != "" & !IsInstance )
                                    {
                                        objModule = objModules.GetModule( intModuleId, TabId );
                                        string strVersion = nodeModule.SelectSingleNode( "content" ).Attributes["version"].Value;
                                        string strType = nodeModule.SelectSingleNode( "content" ).Attributes["type"].Value;
                                        string strcontent = nodeModule.SelectSingleNode( "content" ).InnerXml;
                                        strcontent = strcontent.Substring( 9, strcontent.Length - 12 );
                                        strcontent = HttpContext.Current.Server.HtmlDecode( strcontent );

                                        if( objModule.BusinessControllerClass != "" & objModule.IsPortable )
                                        {
                                            try
                                            {
                                                object objObject = Reflection.CreateObject( objModule.BusinessControllerClass, objModule.BusinessControllerClass );
                                                if( objObject is IPortable )
                                                {
                                                    ( (IPortable)objObject ).ImportModule( objModule.ModuleID, strcontent, strVersion, objportal.AdministratorId );
                                                }
                                            }
                                            catch
                                            {
                                                //ignore errors
                                            }
                                        }
                                    }

                                    // Process permissions only once
                                    if( ! IsInstance )
                                    {
                                        XmlNodeList nodeModulePermissions = nodeModule.SelectNodes( "modulepermissions/permission" );
                                        ParseModulePermissions( nodeModulePermissions, PortalId, intModuleId );
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Processes the settings node
        /// </summary>
        /// <param name="nodeSettings">Template file node for the settings</param>
        /// <param name="PortalId">PortalId of the new portal</param>
        /// <remarks>
        /// </remarks>
        private void ParsePortalSettings( XmlNode nodeSettings, int PortalId )
        {
            PortalInfo objPortal;
            objPortal = GetPortal( PortalId );

            objPortal.LogoFile = ImportFile( PortalId, XmlUtils.GetNodeValue( nodeSettings, "logofile", "" ) );
            objPortal.FooterText = XmlUtils.GetNodeValue( nodeSettings, "footertext", "" );
            if( nodeSettings.SelectSingleNode( "expirydate" ) != null )
            {
                objPortal.ExpiryDate = XmlUtils.GetNodeValueDate( nodeSettings, "expirydate", Null.NullDate );
            }
            objPortal.UserRegistration = XmlUtils.GetNodeValueInt( nodeSettings, "userregistration", 0 );
            objPortal.BannerAdvertising = XmlUtils.GetNodeValueInt( nodeSettings, "banneradvertising", 0 );
            if( XmlUtils.GetNodeValue( nodeSettings, "currency", "" ) != "" )
            {
                objPortal.Currency = XmlUtils.GetNodeValue( nodeSettings, "currency", "" );
            }
            if( XmlUtils.GetNodeValue( nodeSettings, "hostfee", "" ) != "" )
            {
                objPortal.HostFee = XmlUtils.GetNodeValueSingle( nodeSettings, "hostfee", 0 );
            }
            if( XmlUtils.GetNodeValue( nodeSettings, "hostspace", "" ) != "" )
            {
                objPortal.HostSpace = XmlUtils.GetNodeValueInt( nodeSettings, "hostspace", 0 );
            }
            objPortal.BackgroundFile = XmlUtils.GetNodeValue( nodeSettings, "backgroundfile", "" );
            objPortal.PaymentProcessor = XmlUtils.GetNodeValue( nodeSettings, "paymentprocessor", "" );
            if( XmlUtils.GetNodeValue( nodeSettings, "siteloghistory", "" ) != "" )
            {
                objPortal.SiteLogHistory = XmlUtils.GetNodeValueInt( nodeSettings, "siteloghistory", 0 );
            }
            objPortal.DefaultLanguage = XmlUtils.GetNodeValue( nodeSettings, "defaultlanguage", "en-US" );
            objPortal.TimeZoneOffset = XmlUtils.GetNodeValueInt( nodeSettings, "timezoneoffset", -8 );

            UpdatePortalInfo( objPortal.PortalID, objPortal.PortalName, objPortal.LogoFile, objPortal.FooterText, objPortal.ExpiryDate, objPortal.UserRegistration, objPortal.BannerAdvertising, objPortal.Currency, objPortal.AdministratorId, objPortal.HostFee, objPortal.HostSpace, objPortal.PaymentProcessor, objPortal.ProcessorUserId, objPortal.ProcessorPassword, objPortal.Description, objPortal.KeyWords, objPortal.BackgroundFile, objPortal.SiteLogHistory, objPortal.SplashTabId, objPortal.HomeTabId, objPortal.LoginTabId, objPortal.UserTabId, objPortal.DefaultLanguage, objPortal.TimeZoneOffset, objPortal.HomeDirectory );

            // set portal skins and containers
            if( XmlUtils.GetNodeValue( nodeSettings, "skinsrc", "" ) != "" )
            {
                SkinController.SetSkin( SkinInfo.RootSkin, PortalId, SkinType.Portal, XmlUtils.GetNodeValue( nodeSettings, "skinsrc", "" ) );
            }
            if( XmlUtils.GetNodeValue( nodeSettings, "skinsrcadmin", "" ) != "" )
            {
                SkinController.SetSkin( SkinInfo.RootSkin, PortalId, SkinType.Admin, XmlUtils.GetNodeValue( nodeSettings, "skinsrcadmin", "" ) );
            }
            if( XmlUtils.GetNodeValue( nodeSettings, "containersrc", "" ) != "" )
            {
                SkinController.SetSkin( SkinInfo.RootContainer, PortalId, SkinType.Portal, XmlUtils.GetNodeValue( nodeSettings, "containersrc", "" ) );
            }
            if( XmlUtils.GetNodeValue( nodeSettings, "containersrcadmin", "" ) != "" )
            {
                SkinController.SetSkin( SkinInfo.RootContainer, PortalId, SkinType.Admin, XmlUtils.GetNodeValue( nodeSettings, "containersrcadmin", "" ) );
            }
        }

        /// <summary>
        /// Processes all Profile Definitions from the template
        /// </summary>
        /// <param name="nodeProfileDefinitions">Template file node for the Profile Definitions</param>
        /// <param name="portalId">PortalId of the new portal</param>
        /// <remarks>
        /// </remarks>
        private void ParseProfileDefinitions( XmlNode nodeProfileDefinitions, int portalId )
        {
            ListController objListController = new ListController();
            ListEntryInfoCollection colDataTypes = objListController.GetListEntryInfoCollection( "DataType" );

            int OrderCounter = -1;

            foreach( XmlNode node in nodeProfileDefinitions.SelectNodes( "//profiledefinition" ) )
            {
                OrderCounter += 2;

                ListEntryInfo typeInfo = colDataTypes.Item( "DataType." + XmlUtils.GetNodeValue( node, "datatype", "" ) );
                if( typeInfo == null )
                {
                    typeInfo = colDataTypes.Item( "DataType.Unknown" );
                }

                ProfilePropertyDefinition objProfileDefinition = new ProfilePropertyDefinition();
                objProfileDefinition.DataType = typeInfo.EntryID;
                objProfileDefinition.DefaultValue = "";
                objProfileDefinition.ModuleDefId = Null.NullInteger;
                objProfileDefinition.PortalId = portalId;
                objProfileDefinition.PropertyCategory = XmlUtils.GetNodeValue( node, "propertycategory", "" );
                objProfileDefinition.PropertyName = XmlUtils.GetNodeValue( node, "propertyname", "" );
                objProfileDefinition.Required = false;
                objProfileDefinition.Visible = true;
                objProfileDefinition.ViewOrder = OrderCounter;
                objProfileDefinition.Length = XmlUtils.GetNodeValueInt( node, "length", 0 );

                ProfileController.AddPropertyDefinition( objProfileDefinition );
            }
        }

        /// <summary>
        /// Processes all Roles from the template
        /// </summary>
        /// <param name="nodeRoles">Template file node for the Roles</param>
        /// <param name="PortalId">PortalId of the new portal</param>
        /// <param name="AdministratorId">New portal's Administrator</param>
        /// <param name="AdministratorRole">Used to return to caller the id of the Administrators Role if found</param>
        /// <param name="RegisteredRole">Used to return to caller the id of the Registered Users Role if found</param>
        /// <param name="SubscriberRole">Used to return to caller the id of the Subscribers Role if found</param>
        /// <remarks>
        /// There must be one role for the Administrators function. Only the first node with this mark will be used as the Administrators Role.
        /// There must be one role for the Registered Users function. Only the first node with this mark will be used as the Registered Users Role.
        /// There must be one role for the Subscribers function. Only the first node with this mark will be used as the Subscribers Role.
        /// If these two minimum roles are not found on the template they will be created with default values.
        ///
        /// The administrator user will be added to the following roles: Administrators, Registered Users and any role specified with autoassignment=true
        /// </remarks>
        private void ParseRoles( XmlNode nodeRoles, int PortalId, int AdministratorId, ref int AdministratorRole, ref int RegisteredRole, ref int SubscriberRole )
        {
            bool foundAdminRole = false;
            bool foundRegisteredRole = false;
            bool foundSubscriberRole = false;

            foreach( XmlNode node in nodeRoles.SelectNodes( "//role" ) )
            {
                int roleId = ParseRole( node, PortalId );

                // check if this is the admin role (only first found is selected as admin)
                if( !foundAdminRole && XmlUtils.GetNodeValue( node, "roletype", "" ) == "adminrole" )
                {
                    foundAdminRole = true;
                    AdministratorRole = roleId;
                }

                // check if this is the registered role (only first found is selected as registered)
                if( !foundRegisteredRole && XmlUtils.GetNodeValue( node, "roletype", "" ) == "registeredrole" )
                {
                    foundRegisteredRole = true;
                    RegisteredRole = roleId;
                }

                // check if this is the subscriber role (only first found is selected as subscriber)
                if( !foundSubscriberRole && XmlUtils.GetNodeValue( node, "roletype", "" ) == "subscriberrole" )
                {
                    foundSubscriberRole = true;
                    SubscriberRole = roleId;
                }
            }
        }

        /// <summary>
        /// Processes all tabs from the template
        /// </summary>
        /// <param name="nodeTabs">Template file node for the tabs</param>
        /// <param name="PortalId">PortalId of the new portal</param>
        /// <param name="IsAdminTemplate">True when processing admin template, false when processing portal template</param>
        /// <param name="mergeTabs">Flag to determine whether Module content is merged.</param>
        /// <param name="IsNewPortal">Flag to determine is the template is applied to an existing portal or a new one.</param>
        /// <remarks>
        /// When a special tab is found (HomeTab, UserTab, LoginTab, AdminTab) portal information will be updated.
        /// </remarks>
        private void ParseTabs( XmlNode nodeTabs, int PortalId, bool IsAdminTemplate, PortalTemplateModuleAction mergeTabs, bool IsNewPortal )
        {
            //used to control if modules are true modules or instances
            //will hold module ID from template / new module ID so new instances can reference right moduleid
            //only first one from the template will be create as a true module, 
            //others will be moduleinstances (tabmodules)
            Hashtable hModules = new Hashtable();
            Hashtable hTabs = new Hashtable();

            //if running from wizard we need to pre populate htabs with existing tabs so ParseTab 
            //can find all existing ones
            if( !IsNewPortal )
            {
                Hashtable hTabNames = new Hashtable();
                TabController objTabs = new TabController();
                ArrayList arrTabs = objTabs.GetTabs( PortalId );
                foreach( TabInfo objtab in arrTabs )
                {
                    if( !objtab.IsDeleted & !objtab.IsAdminTab )
                    {
                        string tabname = objtab.TabName;
                        if( !( Null.IsNull( objtab.ParentId ) ) )
                        {
                            tabname = Convert.ToString( hTabNames[objtab.ParentId] ) + "/" + objtab.TabName;
                        }
                        hTabNames.Add( objtab.TabID, tabname );
                    }
                }
                //when parsing tabs we will need tabid given tabname
                foreach( int i in hTabNames.Keys )
                {
                    if( hTabs[hTabNames[i]] == null )
                    {
                        hTabs.Add( hTabNames[i], i );
                    }
                }
            }

            foreach( XmlNode nodeTab in nodeTabs.SelectNodes( "//tab" ) )
            {
                ParseTab( nodeTab, PortalId, IsAdminTemplate, mergeTabs, ref hModules, ref hTabs, IsNewPortal );
            }
        }

        /// <summary>
        /// Processess a template file for the new portal. This method will be called twice: for the portal template and for the admin template
        /// </summary>
        /// <param name="PortalId">PortalId of the new portal</param>
        /// <param name="TemplatePath">Path for the folder where templates are stored</param>
        /// <param name="TemplateFile">Template file to process</param>
        /// <param name="AdministratorId">UserId for the portal administrator. This is used to assign roles to this user</param>
        /// <param name="mergeTabs">Flag to determine whether Module content is merged.</param>
        /// <param name="IsNewPortal">Flag to determine is the template is applied to an existing portal or a new one.</param>
        /// <remarks>
        /// The roles and settings nodes will only be processed on the portal template file.
        /// </remarks>
        public void ParseTemplate( int PortalId, string TemplatePath, string TemplateFile, int AdministratorId, PortalTemplateModuleAction mergeTabs, bool IsNewPortal )
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode node;
            int AdministratorRoleId = -1;
            int RegisteredRoleId = -1;
            int SubscriberRoleId = -1;
            RoleController objrole = new RoleController();
            bool isAdminTemplate;

            isAdminTemplate = TemplateFile == "admin.template";

            // open the XML file
            try
            {
                xmlDoc.Load( TemplatePath + TemplateFile );
            }
            catch // error
            {
                //
            }

            // settings, roles, folders and files can only be specified in portal templates, will be ignored on the admin template
            if( !isAdminTemplate )
            {
                // parse roles if available
                node = xmlDoc.SelectSingleNode( "//portal/roles" );
                if( node != null )
                {
                    ParseRoles( node, PortalId, AdministratorId, ref AdministratorRoleId, ref RegisteredRoleId, ref SubscriberRoleId );
                }

                // create required roles if not already created
                if( AdministratorRoleId == -1 )
                {
                    AdministratorRoleId = CreateRole( PortalId, "Administrators", "Portal Administrators", 0, 0, "M", 0, 0, "N", false, false );
                }
                if( RegisteredRoleId == -1 )
                {
                    RegisteredRoleId = CreateRole( PortalId, "Registered Users", "Registered Users", 0, 0, "M", 0, 0, "N", false, true );
                }
                if( SubscriberRoleId == -1 )
                {
                    SubscriberRoleId = CreateRole( PortalId, "Subscribers", "A public role for portal subscriptions", 0, 0, "M", 0, 0, "N", true, true );
                }

                objrole.AddUserRole( PortalId, AdministratorId, AdministratorRoleId, Null.NullDate, Null.NullDate );
                objrole.AddUserRole( PortalId, AdministratorId, RegisteredRoleId, Null.NullDate, Null.NullDate );
                objrole.AddUserRole( PortalId, AdministratorId, SubscriberRoleId, Null.NullDate, Null.NullDate );

                // parse portal folders
                node = xmlDoc.SelectSingleNode( "//portal/folders" );
                if( node != null )
                {
                    ParseFolders( node, PortalId );
                }
                // force creation of root folder if not present on template
                FolderController objController = new FolderController();
                if( objController.GetFolder( PortalId, "" ) == null )
                {
                    int folderid = objController.AddFolder( PortalId, "", (int)FolderController.StorageLocationTypes.InsecureFileSystem, true, false );
                    PermissionController objPermissionController = new PermissionController();
                    ArrayList arr = objPermissionController.GetPermissionByCodeAndKey( "SYSTEM_FOLDER", "" );
                    foreach( PermissionInfo objpermission in arr )
                    {
                        FileSystemUtils.SetFolderPermission( PortalId, folderid, objpermission.PermissionID, AdministratorRoleId, "" );
                        if( objpermission.PermissionKey == "READ" )
                        {
                            // add READ permissions to the All Users Role
                            FileSystemUtils.SetFolderPermission( PortalId, folderid, objpermission.PermissionID, int.Parse( Globals.glbRoleAllUsers ), "" );
                        }
                    }
                }

                // parse portal settings if available only for new portals
                node = xmlDoc.SelectSingleNode( "//portal/settings" );
                if( node != null && IsNewPortal )
                {
                    ParsePortalSettings( node, PortalId );
                }

                // update portal setup
                PortalInfo objportal;
                objportal = GetPortal( PortalId );
                DataProvider.Instance().UpdatePortalSetup( PortalId, AdministratorId, AdministratorRoleId, RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.UserTabId, objportal.AdminTabId );

                //Remove Exising Tabs if doing a "Replace"
                if( mergeTabs == PortalTemplateModuleAction.Replace )
                {
                    TabController objTabs = new TabController();
                    ArrayList arrTabs = objTabs.GetTabs( PortalId );
                    foreach( TabInfo tempLoopVar_objTab in arrTabs )
                    {
                        TabInfo objTab = tempLoopVar_objTab;
                        if( !objTab.IsAdminTab )
                        {
                            //soft delete Tab
                            objTab.TabName = objTab.TabName + "_old";
                            objTab.IsDeleted = true;
                            objTabs.UpdateTab( objTab );
                            //Delete all Modules
                            ModuleController objModules = new ModuleController();
                            ArrayList arrModules = objModules.GetPortalTabModules( objTab.PortalID, objTab.TabID );
                            foreach( ModuleInfo objModule in arrModules )
                            {
                                objModules.DeleteTabModule( objModule.TabID, objModule.ModuleID );
                            }
                        }
                    }
                }
            }

            // parse portal tabs
            node = xmlDoc.SelectSingleNode( "//portal/tabs" );
            if( node != null )
            {
                ParseTabs( node, PortalId, isAdminTemplate, mergeTabs, IsNewPortal );
            }
        }

        /// <summary>
        /// Processes the resource file for the template file selected
        /// </summary>
        /// <param name="portalPath">New portal's folder</param>
        /// <param name="TemplateFile">Selected template file</param>
        /// <remarks>
        /// The resource file is a zip file with the same name as the selected template file and with
        /// an extension of .resources (to unable this file being downloaded).
        /// For example: for template file "portal.template" a resource file "portal.template.resources" can be defined.
        /// </remarks>
        public void ProcessResourceFile( string portalPath, string TemplateFile )
        {
            try
            {
                ZipInputStream objZipInputStream = new ZipInputStream( new FileStream( TemplateFile + ".resources", FileMode.Open, FileAccess.Read ) );
                FileSystemUtils.UnzipResources( objZipInputStream, portalPath );
            }
            catch( Exception )
            {
                // error opening file
            }
        }

        public void UpdatePortalExpiry( int PortalId )
        {
            IDataReader dr = DataProvider.Instance().GetPortal( PortalId );
            if( dr.Read() )
            {
                DateTime ExpiryDate;
                if( Information.IsDBNull( dr["ExpiryDate"] ) )
                {
                    ExpiryDate = Convert.ToDateTime( dr["ExpiryDate"] );
                }
                else
                {
                    ExpiryDate = DateTime.Now;
                }

                DataProvider.Instance().UpdatePortalInfo( PortalId, Convert.ToString( dr["PortalName"] ), Convert.ToString( dr["LogoFile"] ), Convert.ToString( dr["FooterText"] ), DateAndTime.DateAdd( DateInterval.Month, 1, ExpiryDate ), Convert.ToInt32( dr["UserRegistration"] ), Convert.ToInt32( dr["BannerAdvertising"] ), Convert.ToString( dr["Currency"] ), Convert.ToInt32( dr["AdministratorId"] ), Convert.ToDouble( dr["HostFee"] ), Convert.ToDouble( dr["HostSpace"] ), Convert.ToString( dr["PaymentProcessor"] ), Convert.ToString( dr["ProcessorUserId"] ), Convert.ToString( dr["ProcessorPassword"] ), Convert.ToString( dr["Description"] ), Convert.ToString( dr["KeyWords"] ), Convert.ToString( dr["BackgroundFile"] ), Convert.ToInt32( dr["SiteLogHistory"] ), Convert.ToInt32( dr["SplashTabId"] ), Convert.ToInt32( dr["HomeTabId"] ), Convert.ToInt32( dr["LoginTabId"] ), Convert.ToInt32( dr["UserTabId"] ), Convert.ToString( dr["DefaultLanguage"] ), Convert.ToInt32( dr["TimeZoneOffset"] ), Convert.ToString( dr["HomeDirectory"] ) );
            }
            dr.Close();
        }

        /// <summary>
        /// Updates basic portal information
        /// </summary>
        /// <param name="Portal"></param>
        /// <remarks>
        /// </remarks>
        public void UpdatePortalInfo( PortalInfo Portal )
        {
            UpdatePortalInfo( Portal.PortalID, Portal.PortalName, Portal.LogoFile, Portal.FooterText, Portal.ExpiryDate, Portal.UserRegistration, Portal.BannerAdvertising, Portal.Currency, Portal.AdministratorId, Portal.HostFee, Portal.HostSpace, Portal.PaymentProcessor, Portal.ProcessorUserId, Portal.ProcessorPassword, Portal.Description, Portal.KeyWords, Portal.BackgroundFile, Portal.SiteLogHistory, Portal.SplashTabId, Portal.HomeTabId, Portal.LoginTabId, Portal.UserTabId, Portal.DefaultLanguage, Portal.TimeZoneOffset, Portal.HomeDirectory );
        }

        /// <summary>
        /// Updates basic portal information
        /// </summary>
        /// <param name="PortalId"></param>
        /// <param name="PortalName"></param>
        /// <param name="LogoFile"></param>
        /// <param name="FooterText"></param>
        /// <param name="ExpiryDate"></param>
        /// <param name="UserRegistration"></param>
        /// <param name="BannerAdvertising"></param>
        /// <param name="Currency"></param>
        /// <param name="AdministratorId"></param>
        /// <param name="HostFee"></param>
        /// <param name="HostSpace"></param>
        /// <param name="PaymentProcessor"></param>
        /// <param name="ProcessorUserId"></param>
        /// <param name="ProcessorPassword"></param>
        /// <param name="Description"></param>
        /// <param name="KeyWords"></param>
        /// <param name="BackgroundFile"></param>
        /// <param name="SiteLogHistory"></param>
        /// <param name="SplashTabId"></param>
        /// <param name="HomeTabId"></param>
        /// <param name="LoginTabId"></param>
        /// <param name="UserTabId"></param>
        /// <param name="DefaultLanguage"></param>
        /// <param name="TimeZoneOffset"></param>
        /// <remarks>
        /// </remarks>
        public void UpdatePortalInfo( int PortalId, string PortalName, string LogoFile, string FooterText, DateTime ExpiryDate, int UserRegistration, int BannerAdvertising, string Currency, int AdministratorId, double HostFee, double HostSpace, string PaymentProcessor, string ProcessorUserId, string ProcessorPassword, string Description, string KeyWords, string BackgroundFile, int SiteLogHistory, int SplashTabId, int HomeTabId, int LoginTabId, int UserTabId, string DefaultLanguage, int TimeZoneOffset, string HomeDirectory )
        {
            DataProvider.Instance().UpdatePortalInfo( PortalId, PortalName, LogoFile, FooterText, ExpiryDate, UserRegistration, BannerAdvertising, Currency, AdministratorId, HostFee, HostSpace, PaymentProcessor, ProcessorUserId, ProcessorPassword, Description, KeyWords, BackgroundFile, SiteLogHistory, SplashTabId, HomeTabId, LoginTabId, UserTabId, DefaultLanguage, TimeZoneOffset, HomeDirectory );

            // clear portal settings
            DataCache.ClearPortalCache( PortalId, true );
        }
    }
}