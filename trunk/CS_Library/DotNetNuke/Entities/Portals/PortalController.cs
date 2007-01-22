#region DotNetNuke License

// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Xml;
using DotNetNuke.Common;
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
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Skins;
using ICSharpCode.SharpZipLib.Zip;
using FileInfo=DotNetNuke.Services.FileSystem.FileInfo;

namespace DotNetNuke.Entities.Portals
{
    public class PortalController
    {
        private static void AddPortalToDictionary( int portalId )
        {
            Dictionary<int, int> portalDic = GetPortalDictionary();
            portalDic[portalId] = portalId;
        }

        private static PortalInfo FillPortalInfo( IDataReader dr )
        {
            return FillPortalInfo( dr, true );
        }

        private static PortalInfo FillPortalInfo( IDataReader dr, bool CheckForOpenDataReader )
        {
            PortalInfo objPortalInfo = null;

            // read datareader
            bool canContinue = true;
            if( CheckForOpenDataReader )
            {
                canContinue = false;
                if( dr.Read() )
                {
                    canContinue = true;
                }
            }
            if( canContinue )
            {
                objPortalInfo = new PortalInfo();
                objPortalInfo.PortalID = Convert.ToInt32( Null.SetNull( dr["PortalID"], objPortalInfo.PortalID ) );
                objPortalInfo.PortalName = Convert.ToString( Null.SetNull( dr["PortalName"], objPortalInfo.PortalName ) );
                objPortalInfo.LogoFile = Convert.ToString( Null.SetNull( dr["LogoFile"], objPortalInfo.LogoFile ) );
                objPortalInfo.FooterText = Convert.ToString( Null.SetNull( dr["FooterText"], objPortalInfo.FooterText ) );
                objPortalInfo.ExpiryDate = Convert.ToDateTime( Null.SetNull( dr["ExpiryDate"], objPortalInfo.ExpiryDate ) );
                objPortalInfo.UserRegistration = Convert.ToInt32( Null.SetNull( dr["UserRegistration"], objPortalInfo.UserRegistration ) );
                objPortalInfo.BannerAdvertising = Convert.ToInt32( Null.SetNull( dr["BannerAdvertising"], objPortalInfo.BannerAdvertising ) );
                objPortalInfo.AdministratorId = Convert.ToInt32( Null.SetNull( dr["AdministratorID"], objPortalInfo.AdministratorId ) );
                objPortalInfo.Email = Convert.ToString( Null.SetNull( dr["Email"], objPortalInfo.Email ) );
                objPortalInfo.Currency = Convert.ToString( Null.SetNull( dr["Currency"], objPortalInfo.Currency ) );
                objPortalInfo.HostFee = Convert.ToInt32( Null.SetNull( dr["HostFee"], objPortalInfo.HostFee ) );
                objPortalInfo.HostSpace = Convert.ToInt32( Null.SetNull( dr["HostSpace"], objPortalInfo.HostSpace ) );
                objPortalInfo.PageQuota = Convert.ToInt32( Null.SetNull( dr["PageQuota"], objPortalInfo.PageQuota ) );
                objPortalInfo.UserQuota = Convert.ToInt32( Null.SetNull( dr["UserQuota"], objPortalInfo.UserQuota ) );
                objPortalInfo.AdministratorRoleId = Convert.ToInt32( Null.SetNull( dr["AdministratorRoleID"], objPortalInfo.AdministratorRoleId ) );
                objPortalInfo.RegisteredRoleId = Convert.ToInt32( Null.SetNull( dr["RegisteredRoleID"], objPortalInfo.RegisteredRoleId ) );
                objPortalInfo.Description = Convert.ToString( Null.SetNull( dr["Description"], objPortalInfo.Description ) );
                objPortalInfo.KeyWords = Convert.ToString( Null.SetNull( dr["KeyWords"], objPortalInfo.KeyWords ) );
                objPortalInfo.BackgroundFile = Convert.ToString( Null.SetNull( dr["BackGroundFile"], objPortalInfo.BackgroundFile ) );
                objPortalInfo.GUID = new Guid( Convert.ToString( Null.SetNull( dr["GUID"], objPortalInfo.GUID ) ) );
                objPortalInfo.PaymentProcessor = Convert.ToString( Null.SetNull( dr["PaymentProcessor"], objPortalInfo.PaymentProcessor ) );
                objPortalInfo.ProcessorUserId = Convert.ToString( Null.SetNull( dr["ProcessorUserId"], objPortalInfo.ProcessorUserId ) );
                objPortalInfo.ProcessorPassword = Convert.ToString( Null.SetNull( dr["ProcessorPassword"], objPortalInfo.ProcessorPassword ) );
                objPortalInfo.SiteLogHistory = Convert.ToInt32( Null.SetNull( dr["SiteLogHistory"], objPortalInfo.SiteLogHistory ) );
                objPortalInfo.SplashTabId = Convert.ToInt32( Null.SetNull( dr["SplashTabID"], objPortalInfo.SplashTabId ) );
                objPortalInfo.HomeTabId = Convert.ToInt32( Null.SetNull( dr["HomeTabID"], objPortalInfo.HomeTabId ) );
                objPortalInfo.LoginTabId = Convert.ToInt32( Null.SetNull( dr["LoginTabID"], objPortalInfo.LoginTabId ) );
                objPortalInfo.UserTabId = Convert.ToInt32( Null.SetNull( dr["UserTabID"], objPortalInfo.UserTabId ) );
                objPortalInfo.DefaultLanguage = Convert.ToString( Null.SetNull( dr["DefaultLanguage"], objPortalInfo.DefaultLanguage ) );
                objPortalInfo.TimeZoneOffset = Convert.ToInt32( Null.SetNull( dr["TimeZoneOffset"], objPortalInfo.TimeZoneOffset ) );
                objPortalInfo.AdminTabId = Convert.ToInt32( Null.SetNull( dr["AdminTabID"], objPortalInfo.AdminTabId ) );
                objPortalInfo.HomeDirectory = Convert.ToString( Null.SetNull( dr["HomeDirectory"], objPortalInfo.HomeDirectory ) );
                objPortalInfo.SuperTabId = Convert.ToInt32( Null.SetNull( dr["SuperTabId"], objPortalInfo.SuperTabId ) );
                objPortalInfo.AdministratorRoleName = Convert.ToString( Null.SetNull( dr["AdministratorRoleName"], objPortalInfo.AdministratorRoleName ) );
                objPortalInfo.RegisteredRoleName = Convert.ToString( Null.SetNull( dr["RegisteredRoleName"], objPortalInfo.RegisteredRoleName ) );

                objPortalInfo.Users = Null.NullInteger;
                objPortalInfo.Pages = Null.NullInteger;
            }

            return objPortalInfo;
        }

        private static ArrayList FillPortalInfoCollection( IDataReader dr )
        {
            int TotalRecords = 0;
            return FillPortalInfoCollection( dr, ref TotalRecords );
        }

        private static ArrayList FillPortalInfoCollection( IDataReader dr, ref int totalRecords )
        {
            ArrayList arr = new ArrayList();
            try
            {
                PortalInfo obj = null;
                while( dr.Read() )
                {
                    // fill business object
                    obj = FillPortalInfo( dr, false );
                    // add to collection
                    arr.Add( obj );
                }

                //Get the next result (containing the total)
                bool nextResult = dr.NextResult();

                if( nextResult )
                {
                    //Get the total no of records from the second result
                    totalRecords = Globals.GetTotalRecords(ref dr );
                }
            }
            catch( Exception exc )
            {
                Exceptions.LogException( exc );
            }
            finally
            {
                // close datareader
                if( dr != null )
                {
                    dr.Close();
                }
            }
            return arr;
        }

        public static void DeleteExpiredPortals( string serverPath )
        {
            foreach( PortalInfo portal in GetExpiredPortals() )
            {
                DeletePortal( portal, serverPath );
            }
        }

        public static string DeletePortal( PortalInfo portal, string serverPath )
        {
            string strPortalName = null;
            string strMessage = string.Empty;

            // check if this is the last portal
            int portalCount = DataProvider.Instance().GetPortalCount();

            if( portalCount > 1 )
            {
                if( portal != null )
                {
                    // delete custom resource files
                    Globals.DeleteFilesRecursive( serverPath, ".Portal-" + portal.PortalID.ToString() + ".resx" );

                    //If child portal delete child folder
                    PortalAliasController objPortalAliasController = new PortalAliasController();
                    ArrayList arr = objPortalAliasController.GetPortalAliasArrayByPortalID( portal.PortalID );
                    PortalAliasInfo objPortalAliasInfo = (PortalAliasInfo)( arr[0] );
                    strPortalName = Globals.GetPortalDomainName( objPortalAliasInfo.HTTPAlias, null, true );
                    if( Convert.ToBoolean( ( objPortalAliasInfo.HTTPAlias.IndexOf( "/", 0 ) + 1 ) ) )
                    {
                        strPortalName = objPortalAliasInfo.HTTPAlias.Substring( ( objPortalAliasInfo.HTTPAlias.LastIndexOf( "/" ) + 1 ) );
                    }
                    if( strPortalName != "" && Directory.Exists( serverPath + strPortalName ) )
                    {
                        Globals.DeleteFolderRecursive( serverPath + strPortalName );
                    }

                    // delete upload directory
                    Globals.DeleteFolderRecursive( serverPath + "Portals\\" + portal.PortalID.ToString() );
                    string HomeDirectory = portal.HomeDirectoryMapPath;
                    if( Directory.Exists( HomeDirectory ) )
                    {
                        Globals.DeleteFolderRecursive( HomeDirectory );
                    }

                    // remove database references
                    PortalController objPortalController = new PortalController();
                    objPortalController.DeletePortalInfo( portal.PortalID );
                }
            }
            else
            {
                strMessage = Localization.GetString( "LastPortal" );
            }

            return strMessage;
        }

        public static Dictionary<int, int> GetPortalDictionary()
        {
            string portalsKey = DataCache.PortalDictionaryCacheKey;            
            Dictionary<int, int> portalDic = DataCache.GetPersistentCacheItem(portalsKey, typeof(Dictionary<int, int>)) as Dictionary<int, int>;
            if( portalDic == null )
            {
                portalDic = new Dictionary<int, int>();
            }

            //Save dictionary in Cache
            DataCache.SetCache( portalsKey, portalDic, true );

            //Return Dictionary
            return portalDic;
        }

        /// <summary>
        /// GetPortalsByName gets all the portals whose name matches a provided filter expression
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="nameToMatch">The email address to use to find a match.</param>
        /// <param name="pageIndex">The page of records to return.</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        /// <returns>An ArrayList of PortalInfo objects.</returns>
        /// <history>
        ///     [cnurse]	11/17/2006	created
        /// </history>
        public static ArrayList GetPortalsByName( string nameToMatch, int pageIndex, int pageSize, int totalRecords )
        {
            if( pageIndex == -1 )
            {
                pageIndex = 0;
                pageSize = int.MaxValue;
            }

            return FillPortalInfoCollection( DataProvider.Instance().GetPortalsByName( nameToMatch, pageIndex, pageSize ), ref totalRecords );
        }

        public static ArrayList GetExpiredPortals()
        {
            return FillPortalInfoCollection( DataProvider.Instance().GetExpiredPortals() );
        }

        private string CreateProfileDefinitions( int PortalId, string TemplatePath, string TemplateFile )
        {
            string strMessage = Null.NullString;
            try
            {
                // add profile definitions
                XmlDocument xmlDoc = new XmlDocument();
                XmlNode node = null;
                // open the XML template file
                try
                {
                    xmlDoc.Load( TemplatePath + TemplateFile );
                }
                catch
                {
                    // error
                }

                // parse profile definitions if available
                node = xmlDoc.SelectSingleNode( "//portal/profiledefinitions" );
                if( node != null )
                {
                    ParseProfileDefinitions( node, PortalId );
                }
                else // template does not contain profile definitions ( ie. was created prior to DNN 3.3.0 )
                {
                    ProfileController.AddDefaultDefinitions( PortalId );
                }
            }
            catch( Exception ex )
            {
                strMessage = Localization.GetString( "CreateProfileDefinitions.Error" );
            }

            return strMessage;
        }

        /// <summary>
        /// Creates a new portal based on the portal template provided.
        /// </summary>
        /// <param name="PortalName">Name of the portal to be created</param>
        /// <returns>PortalId of the new portal if there are no errors, -1 otherwise.</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	03/09/2004	Modified to support new template format.
        ///                             Portal template file should be processed before admin.template
        ///     [cnurse]    01/11/2005  Template parsing moved to CreatePortal
        ///     [cnurse]    05/10/2006  Removed unneccessary use of Administrator properties
        /// </history>
        private int CreatePortal( string PortalName, string HomeDirectory )
        {
            // add portal
            int PortalId = -1;
            try
            {
                // Use host settings as default values for these parameters
                // This can be overwritten on the portal template
                DateTime datExpiryDate = DateTime.MinValue;
                if( Convert.ToString( Globals.HostSettings["DemoPeriod"] ) != "" )
                {
                    datExpiryDate = Convert.ToDateTime( Globals.GetMediumDate( DateTime.Now.AddDays( Int32.Parse( Convert.ToString( Globals.HostSettings[ "DemoPeriod" ] ) ) ).ToString() ) );
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

                int intPageQuota = 0;
                if( Convert.ToString( Globals.HostSettings["PageQuota"] ) != "" )
                {
                    intPageQuota = Convert.ToInt32( Globals.HostSettings["PageQuota"] );
                }

                int intUserQuota = 0;
                if( Convert.ToString( Globals.HostSettings["UserQuota"] ) != "" )
                {
                    intUserQuota = Convert.ToInt32( Globals.HostSettings["UserQuota"] );
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
                PortalId = DataProvider.Instance().CreatePortal( PortalName, strCurrency, datExpiryDate, dblHostFee, dblHostSpace, intPageQuota, intUserQuota, intSiteLogHistory, HomeDirectory );
            }
            catch
            {
                // error creating portal
            }

            return PortalId;
        }

        private int CreateRole( int PortalId, string roleName, string description, float serviceFee, int billingPeriod, string billingFrequency, float trialFee, int trialPeriod, string trialFrequency, bool isPublic, bool isAuto )
        {
            RoleInfo objRoleInfo = new RoleInfo();
            RoleController objRoleController = new RoleController();
            int RoleId = 0;

            //First check if the role exists
            objRoleInfo = objRoleController.GetRoleByName( PortalId, roleName );

            if( objRoleInfo == null )
            {
                objRoleInfo = new RoleInfo();
                objRoleInfo.PortalID = PortalId;
                objRoleInfo.RoleName = roleName;
                objRoleInfo.RoleGroupID = Null.NullInteger;
                objRoleInfo.Description = description;
                objRoleInfo.ServiceFee = Convert.ToSingle( ( ( serviceFee < 0 ) ? 0 : serviceFee ) );
                objRoleInfo.BillingPeriod = billingPeriod;
                objRoleInfo.BillingFrequency = billingFrequency;
                objRoleInfo.TrialFee = Convert.ToSingle( ( ( trialFee < 0 ) ? 0 : trialFee ) );
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

        private string ImportFile( int PortalId, string url )
        {
            string strUrl = url;

            if( Globals.GetURLType( url ) == TabType.File )
            {
                FileController objFileController = new FileController();
                int fileId = objFileController.ConvertFilePathToFileId( url, PortalId );
                if( fileId >= 0 )
                {
                    strUrl = "FileID=" + fileId.ToString();
                }
            }

            return strUrl;
        }

        /// <summary>
        /// Processes all Files from the template
        /// </summary>
        /// <param name="nodeFiles">Template file node for the Files</param>
        /// <param name="PortalId">PortalId of the new portal</param>
        /// <history>
        /// 	[cnurse]	11/09/2004	Created
        /// </history>
        private void ParseFiles( XmlNodeList nodeFiles, int PortalId, FolderInfo objFolder )
        {
            int FileId = 0;
            FileController objController = new FileController();
            FileInfo objInfo = null;
            string fileName = null;

            foreach( XmlNode node in nodeFiles )
            {
                fileName = XmlUtils.GetNodeValue( node, "filename", "" );

                //First check if the file exists
                objInfo = objController.GetFile( fileName, PortalId, objFolder.FolderID );

                if( objInfo == null )
                {
                    objInfo = new FileInfo();
                    objInfo.PortalId = PortalId;
                    objInfo.FileName = fileName;
                    objInfo.Extension = XmlUtils.GetNodeValue( node, "extension", "" );
                    objInfo.Size = XmlUtils.GetNodeValueInt( node, "size", 0 );
                    objInfo.Width = XmlUtils.GetNodeValueInt( node, "width", 0 );
                    objInfo.Height = XmlUtils.GetNodeValueInt( node, "height", 0 );
                    objInfo.ContentType = XmlUtils.GetNodeValue( node, "contenttype", "" );
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
        /// Processes all Folders from the template
        /// </summary>
        /// <param name="nodeFolders">Template file node for the Folders</param>
        /// <param name="PortalId">PortalId of the new portal</param>
        /// <history>
        /// 	[cnurse]	11/09/2004	Created
        /// </history>
        private void ParseFolders( XmlNode nodeFolders, int PortalId )
        {
            int FolderId = 0;
            FolderController objController = new FolderController();
            FolderInfo objInfo = null;
            string folderPath = null;
            int storageLocation = 0;
            bool isProtected = false;

            foreach( XmlNode node in nodeFolders.SelectNodes( "//folder" ) )
            {
                folderPath = XmlUtils.GetNodeValue( node, "folderpath", "" );

                //First check if the folder exists
                objInfo = objController.GetFolder( PortalId, folderPath );

                if( objInfo == null )
                {
                    isProtected = FileSystemUtils.DefaultProtectedFolders( folderPath );
                    if( isProtected == true )
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
                    FolderId = objController.AddFolder( PortalId, folderPath, storageLocation, isProtected, false );
                    objInfo = objController.GetFolder( PortalId, folderPath );
                }
                else
                {
                    //Get Id from Folder
                    FolderId = objInfo.FolderID;
                }

                XmlNodeList nodeFolderPermissions = node.SelectNodes( "folderpermissions/permission" );
                ParseFolderPermissions( nodeFolderPermissions, PortalId, FolderId, folderPath );

                XmlNodeList nodeFiles = node.SelectNodes( "files/file" );
                if( folderPath != "" )
                {
                    folderPath += "/";
                }
                ParseFiles( nodeFiles, PortalId, objInfo );
            }
        }

        /// <summary>
        /// Parses folder permissions
        /// </summary>
        /// <param name="nodeFolderPermissions">Node for folder permissions</param>
        /// <param name="PortalID">PortalId of new portal</param>
        /// <param name="FolderId">FolderId of folder being processed</param>
        /// <history>
        /// 	[cnurse]	11/09/2004	Created
        /// </history>
        private void ParseFolderPermissions( XmlNodeList nodeFolderPermissions, int PortalID, int FolderId, string folderPath )
        {
            FolderPermissionCollection objFolderPermissions = new FolderPermissionCollection();
            PermissionController objPermissionController = new PermissionController();
            PermissionInfo objPermission = null;
            FolderPermissionController objFolderPermissionController = new FolderPermissionController();
            RoleController objRoleController = new RoleController();
            RoleInfo objRole = null;
            int RoleID = 0;
            int PermissionID = 0;
            string PermissionKey = null;
            string PermissionCode = null;
            string RoleName = null;
            bool AllowAccess = false;
            ArrayList arrPermissions = null;
            int i = 0;

            foreach( XmlNode xmlFolderPermission in nodeFolderPermissions )
            {
                PermissionKey = XmlUtils.GetNodeValue( xmlFolderPermission, "permissionkey", "" );
                PermissionCode = XmlUtils.GetNodeValue( xmlFolderPermission, "permissioncode", "" );
                RoleName = XmlUtils.GetNodeValue( xmlFolderPermission, "rolename", "" );
                AllowAccess = XmlUtils.GetNodeValueBoolean( xmlFolderPermission, "allowaccess", false );
                arrPermissions = objPermissionController.GetPermissionByCodeAndKey( PermissionCode, PermissionKey );

                for( i = 0; i < arrPermissions.Count; i++ )
                {
                    objPermission = (PermissionInfo)( arrPermissions[i] );
                    PermissionID = objPermission.PermissionID;
                }
                RoleID = int.MinValue;
                if( RoleName == Globals.glbRoleAllUsersName )
                {
                    RoleID = Convert.ToInt32( Globals.glbRoleAllUsers );
                }
                else if( RoleName == Globals.glbRoleUnauthUserName )
                {
                    RoleID = Convert.ToInt32( Globals.glbRoleUnauthUser );
                }
                else
                {
                    objRole = objRoleController.GetRoleByName( PortalID, RoleName );
                    if( objRole != null )
                    {
                        RoleID = objRole.RoleID;
                    }
                }

                // if role was found add, otherwise ignore
                if( RoleID != int.MinValue )
                {
                    if( AllowAccess )
                    {
                        FileSystemUtils.SetFolderPermission( PortalID, FolderId, PermissionID, RoleID, folderPath );
                    }
                }
            }
        }

        private void ParseModulePermissions( XmlNodeList nodeModulePermissions, int PortalId, int ModuleID )
        {
            RoleController objRoleController = new RoleController();
            RoleInfo objRole = null;
            ModulePermissionCollection objModulePermissions = new ModulePermissionCollection();
            ModulePermissionController objModulePermissionController = new ModulePermissionController();
            PermissionController objPermissionController = new PermissionController();
            PermissionInfo objPermission = null;
            ModulePermissionCollection objModulePermissionCollection = new ModulePermissionCollection();
            int PermissionID = 0;
            ArrayList arrPermissions = null;
            int i = 0;
            string PermissionKey = null;
            string PermissionCode = null;
            string RoleName = null;
            int RoleID = 0;
            bool AllowAccess = false;

            foreach( XmlNode node in nodeModulePermissions )
            {
                PermissionKey = XmlUtils.GetNodeValue( node, "permissionkey", "" );
                PermissionCode = XmlUtils.GetNodeValue( node, "permissioncode", "" );
                RoleName = XmlUtils.GetNodeValue( node, "rolename", "" );
                AllowAccess = XmlUtils.GetNodeValueBoolean( node, "allowaccess", false );

                RoleID = int.MinValue;
                if( RoleName == Globals.glbRoleAllUsersName )
                {
                    RoleID = Convert.ToInt32( Globals.glbRoleAllUsers );
                }
                else if( RoleName == Globals.glbRoleUnauthUserName )
                {
                    RoleID = Convert.ToInt32( Globals.glbRoleUnauthUser );
                }
                else
                {
                    objRole = objRoleController.GetRoleByName( PortalId, RoleName );
                    if( objRole != null )
                    {
                        RoleID = objRole.RoleID;
                    }
                }
                if( RoleID != int.MinValue )
                {
                    PermissionID = -1;
                    arrPermissions = objPermissionController.GetPermissionByCodeAndKey( PermissionCode, PermissionKey );

                    for( i = 0; i < arrPermissions.Count; i++ )
                    {
                        objPermission = (PermissionInfo)( arrPermissions[i] );
                        PermissionID = objPermission.PermissionID;
                    }

                    // if role was found add, otherwise ignore
                    if( PermissionID != -1 )
                    {
                        ModulePermissionInfo objModulePermission = new ModulePermissionInfo();
                        objModulePermission.ModuleID = ModuleID;
                        objModulePermission.PermissionID = PermissionID;
                        objModulePermission.RoleID = RoleID;
                        objModulePermission.AllowAccess = Convert.ToBoolean( XmlUtils.GetNodeValue( node, "allowaccess", "false" ) );
                        objModulePermissionController.AddModulePermission( objModulePermission );
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
        /// <history>
        /// 	[VMasanas]	27/08/2004	Created
        /// 	[VMasanas]	15/10/2004	Modified for new skin structure
        ///     [cnurse]    11/21/2004  Modified to use GetNodeValueDate for ExpiryDate
        ///     [VMasanas]  02/21/2005  Modified to not overwrite ExpiryDate if not present
        /// </history>
        private void ParsePortalSettings( XmlNode nodeSettings, int PortalId )
        {
            PortalInfo objPortal = null;
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
            if( XmlUtils.GetNodeValue( nodeSettings, "pagequota", "" ) != "" )
            {
                objPortal.PageQuota = XmlUtils.GetNodeValueInt( nodeSettings, "pagequota", 0 );
            }
            if( XmlUtils.GetNodeValue( nodeSettings, "userquota", "" ) != "" )
            {
                objPortal.UserQuota = XmlUtils.GetNodeValueInt( nodeSettings, "userquota", 0 );
            }
            objPortal.BackgroundFile = XmlUtils.GetNodeValue( nodeSettings, "backgroundfile", "" );
            objPortal.PaymentProcessor = XmlUtils.GetNodeValue( nodeSettings, "paymentprocessor", "" );
            if( XmlUtils.GetNodeValue( nodeSettings, "siteloghistory", "" ) != "" )
            {
                objPortal.SiteLogHistory = XmlUtils.GetNodeValueInt( nodeSettings, "siteloghistory", 0 );
            }
            objPortal.DefaultLanguage = XmlUtils.GetNodeValue( nodeSettings, "defaultlanguage", "en-US" );
            objPortal.TimeZoneOffset = XmlUtils.GetNodeValueInt( nodeSettings, "timezoneoffset", -8 );

            UpdatePortalInfo( objPortal.PortalID, objPortal.PortalName, objPortal.LogoFile, objPortal.FooterText, objPortal.ExpiryDate, objPortal.UserRegistration, objPortal.BannerAdvertising, objPortal.Currency, objPortal.AdministratorId, objPortal.HostFee, objPortal.HostSpace, objPortal.PageQuota, objPortal.UserQuota, objPortal.PaymentProcessor, objPortal.ProcessorUserId, objPortal.ProcessorPassword, objPortal.Description, objPortal.KeyWords, objPortal.BackgroundFile, objPortal.SiteLogHistory, objPortal.SplashTabId, objPortal.HomeTabId, objPortal.LoginTabId, objPortal.UserTabId, objPortal.DefaultLanguage, objPortal.TimeZoneOffset, objPortal.HomeDirectory );

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
        /// <param name="PortalId">PortalId of the new portal</param>
        private void ParseProfileDefinitions( XmlNode nodeProfileDefinitions, int PortalId )
        {
            ListController objListController = new ListController();
            ListEntryInfoCollection colDataTypes = objListController.GetListEntryInfoCollection( "DataType" );

            int OrderCounter = -1;

            ProfilePropertyDefinition objProfileDefinition = null;

            foreach( XmlNode node in nodeProfileDefinitions.SelectNodes( "//profiledefinition" ) )
            {
                OrderCounter += 2;

                ListEntryInfo typeInfo = colDataTypes.Item( "DataType." + XmlUtils.GetNodeValue( node, "datatype", "" ) );
                if( typeInfo == null )
                {
                    typeInfo = colDataTypes.Item("DataType.Unknown");
                }

                objProfileDefinition = new ProfilePropertyDefinition();
                objProfileDefinition.DataType = typeInfo.EntryID;
                objProfileDefinition.DefaultValue = "";
                objProfileDefinition.ModuleDefId = Null.NullInteger;
                objProfileDefinition.PortalId = PortalId;
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
        /// Processes a node for a Role and creates a new Role based on the information gathered from the node
        /// </summary>
        /// <param name="nodeRole">Template file node for the role</param>
        /// <param name="portalid">PortalId of the new portal</param>
        /// <returns>RoleId of the created role</returns>
        /// <history>
        /// 	[VMasanas]	26/08/2004	Created
        ///		[cnurse]	15/10/2004	Modified to allow for merging template
        ///								with existing pages
        ///     [cnurse]    12/07/2004  modified to use new CreateRole method, which takes care of
        ///                             checking if role exists
        /// </history>
        private int ParseRole( XmlNode nodeRole, int PortalId )
        {
            string RoleName = XmlUtils.GetNodeValue( nodeRole, "rolename", "" );
            string Description = XmlUtils.GetNodeValue( nodeRole, "description", "" );
            float ServiceFee = (float)( XmlUtils.GetNodeValueSingle( nodeRole, "servicefee", 0 ) );
            int BillingPeriod = XmlUtils.GetNodeValueInt( nodeRole, "billingperiod", 0 );
            string BillingFrequency = XmlUtils.GetNodeValue( nodeRole, "billingfrequency", "M" );
            float TrialFee = (float)( XmlUtils.GetNodeValueSingle( nodeRole, "trialfee", 0 ) );
            int TrialPeriod = XmlUtils.GetNodeValueInt( nodeRole, "trialperiod", 0 );
            string TrialFrequency = XmlUtils.GetNodeValue( nodeRole, "trialfrequency", "N" );
            bool IsPublic = XmlUtils.GetNodeValueBoolean( nodeRole, "ispublic", false );
            bool AutoAssignment = XmlUtils.GetNodeValueBoolean( nodeRole, "autoassignment", false );

            //Call Create Role
            return CreateRole( PortalId, RoleName, Description, ServiceFee, BillingPeriod, BillingFrequency, TrialFee, TrialPeriod, TrialFrequency, IsPublic, AutoAssignment );
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
        /// <history>
        /// 	[VMasanas]	26/08/2004	Created
        /// </history>
        private void ParseRoles( XmlNode nodeRoles, int PortalId, int AdministratorId, ref int AdministratorRole, ref int RegisteredRole, ref int SubscriberRole )
        {
            bool foundAdminRole = false;
            bool foundRegisteredRole = false;
            bool foundSubscriberRole = false;
            int RoleId = 0;

            foreach( XmlNode node in nodeRoles.SelectNodes( "//role" ) )
            {
                RoleId = ParseRole( node, PortalId );

                // check if this is the admin role (only first found is selected as admin)
                if( !foundAdminRole && XmlUtils.GetNodeValue( node, "roletype", "" ) == "adminrole" )
                {
                    foundAdminRole = true;
                    AdministratorRole = RoleId;
                }

                // check if this is the registered role (only first found is selected as registered)
                if( !foundRegisteredRole && XmlUtils.GetNodeValue( node, "roletype", "" ) == "registeredrole" )
                {
                    foundRegisteredRole = true;
                    RegisteredRole = RoleId;
                }

                // check if this is the subscriber role (only first found is selected as subscriber)
                if( !foundSubscriberRole && XmlUtils.GetNodeValue( node, "roletype", "" ) == "subscriberrole" )
                {
                    foundSubscriberRole = true;
                    SubscriberRole = RoleId;
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
        /// <history>
        /// 	[VMasanas]	26/08/2004	Removed code to allow multiple tabs with same name.
        /// 	[VMasanas]	15/10/2004	Modified for new skin structure
        ///		[cnurse]	15/10/2004	Modified to allow for merging template
        ///								with existing pages
        /// </history>
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
            string tabname = null;
            if( !IsNewPortal )
            {
                Hashtable hTabNames = new Hashtable();
                TabController objTabs = new TabController();
                foreach( KeyValuePair<int, TabInfo> tabPair in objTabs.GetTabsByPortal( PortalId ) )
                {
                    TabInfo objTab = tabPair.Value;

                    if( !objTab.IsDeleted & !objTab.IsAdminTab )
                    {
                        tabname = objTab.TabName;
                        if( !( Null.IsNull( objTab.ParentId ) ) )
                        {
                            tabname = Convert.ToString( hTabNames[objTab.ParentId] ) + "/" + objTab.TabName;
                        }
                        hTabNames.Add( objTab.TabID, tabname );
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
                hTabNames = null;
            }

            foreach( XmlNode nodeTab in nodeTabs.SelectNodes( "//tab" ) )
            {
                ParseTab( nodeTab, PortalId, IsAdminTemplate, mergeTabs, ref hModules, ref hTabs, IsNewPortal );
            }
        }

        /// <summary>
        /// Processes a single tab from the template
        /// </summary>
        /// <param name="nodeTab">Template file node for the tabs</param>
        /// <param name="PortalId">PortalId of the new portal</param>
        /// <param name="IsAdminTemplate">True when processing admin template, false when processing portal template</param>
        /// <param name="mergeTabs">Flag to determine whether Module content is merged.</param>
        /// <param name="hModules">Used to control if modules are true modules or instances</param>
        /// <param name="hTabs">Supporting object to build the tab hierarchy</param>
        /// <param name="IsNewPortal">Flag to determine is the template is applied to an existing portal or a new one.</param>
        /// <remarks>
        /// When a special tab is found (HomeTab, UserTab, LoginTab, AdminTab) portal information will be updated.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	26/08/2004	Removed code to allow multiple tabs with same name.
        /// 	[VMasanas]	15/10/2004	Modified for new skin structure
        ///		[cnurse]	15/10/2004	Modified to allow for merging template
        ///								with existing pages
        ///     [cnurse]    11/21/2204  modified to use GetNodeValueDate for Start and End Dates
        /// </history>
        private int ParseTab( XmlNode nodeTab, int PortalId, bool IsAdminTemplate, PortalTemplateModuleAction mergeTabs, ref Hashtable hModules, ref Hashtable hTabs, bool IsNewPortal )
        {
            TabInfo objTab = null;
            TabController objTabs = new TabController();
            string strName = XmlUtils.GetNodeValue( nodeTab, "name", "" );
            int intTabId = 0;
            PortalInfo objportal = null;
            bool tabExists = true;
            string tabName = null;

            objportal = GetPortal( PortalId );

            if( strName != "" )
            {
                if( !IsNewPortal ) // running from wizard: try to find the tab by path
                {
                    string parenttabname = "";

                    if( XmlUtils.GetNodeValue( nodeTab, "parent", "" ) != "" )
                    {
                        parenttabname = XmlUtils.GetNodeValue( nodeTab, "parent", "" ) + "/";
                    }
                    if( !( hTabs[parenttabname + strName] == null ) )
                    {
                        objTab = objTabs.GetTab( Convert.ToInt32( hTabs[parenttabname + strName] ), PortalId, false );
                    }
                }

                if( objTab == null | IsNewPortal )
                {
                    tabExists = false;

                    objTab = new TabInfo();
                    intTabId = Null.NullInteger;

                    objTab.TabID = intTabId;
                    objTab.PortalID = PortalId;
                    objTab.TabName = XmlUtils.GetNodeValue( nodeTab, "name", "" );
                    objTab.Title = XmlUtils.GetNodeValue( nodeTab, "title", "" );
                    objTab.Description = XmlUtils.GetNodeValue( nodeTab, "description", "" );
                    objTab.KeyWords = XmlUtils.GetNodeValue( nodeTab, "keywords", "" );
                    objTab.IsVisible = XmlUtils.GetNodeValueBoolean( nodeTab, "visible", true );
                    objTab.DisableLink = XmlUtils.GetNodeValueBoolean( nodeTab, "disabled", false );
                    objTab.IconFile = ImportFile( PortalId, XmlUtils.GetNodeValue( nodeTab, "iconfile", "" ) );
                    objTab.Url = XmlUtils.GetNodeValue( nodeTab, "url", "" );
                    objTab.StartDate = XmlUtils.GetNodeValueDate( nodeTab, "startdate", Null.NullDate );
                    objTab.EndDate = XmlUtils.GetNodeValueDate( nodeTab, "enddate", Null.NullDate );
                    objTab.RefreshInterval = XmlUtils.GetNodeValueInt( nodeTab, "refreshinterval", Null.NullInteger );
                    objTab.PageHeadText = XmlUtils.GetNodeValue( nodeTab, "pageheadtext", Null.NullString );

                    XmlNodeList nodeTabPermissions = nodeTab.SelectNodes( "tabpermissions/permission" );
                    objTab.TabPermissions = ParseTabPermissions( nodeTabPermissions, objportal, objTab.TabID, IsAdminTemplate );

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
                    tabName = objTab.TabName;

                    if( XmlUtils.GetNodeValue( nodeTab, "parent", "" ) != "" )
                    {
                        if( !( hTabs[XmlUtils.GetNodeValue( nodeTab, "parent", "" )] == null ) )
                        {
                            // parent node specifies the path (tab1/tab2/tab3), use saved tabid
                            objTab.ParentId = Convert.ToInt32( hTabs[XmlUtils.GetNodeValue( nodeTab, "parent", "" )] );
                            tabName = XmlUtils.GetNodeValue( nodeTab, "parent", "" ) + "/" + objTab.TabName;
                        }
                        else
                        {
                            // Parent node doesn't spcecify the path, search by name.
                            // Possible incoherence if tabname not unique
                            TabInfo objParent = objTabs.GetTabByName( XmlUtils.GetNodeValue( nodeTab, "parent", "" ), PortalId );
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

                if( IsAdminTemplate )
                {
                    // when processing the admin template we should identify the Admin tab
                    if( objTab.TabName == "Admin" )
                    {
                        objportal.AdminTabId = intTabId;
                        DataProvider.Instance().UpdatePortalSetup( PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.UserTabId, objportal.AdminTabId );
                    }
                }
                else
                {
                    // when processing the portal template we can find: hometab, usertab, logintab
                    switch( XmlUtils.GetNodeValue( nodeTab, "tabtype", "" ) )
                    {
                        case "splashtab":
                            objportal.SplashTabId = intTabId;
                            DataProvider.Instance().UpdatePortalSetup( PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.UserTabId, objportal.AdminTabId );
                            break;
                        case "hometab":
                            objportal.HomeTabId = intTabId;
                            DataProvider.Instance().UpdatePortalSetup( PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.UserTabId, objportal.AdminTabId );
                            break;
                        case "logintab":
                            objportal.LoginTabId = intTabId;
                            DataProvider.Instance().UpdatePortalSetup( PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.UserTabId, objportal.AdminTabId );
                            break;
                        case "usertab":
                            objportal.UserTabId = intTabId;
                            DataProvider.Instance().UpdatePortalSetup( PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.UserTabId, objportal.AdminTabId );
                            break;
                    }
                }

                //If tab does not already exist parse modules
                if( !tabExists )
                {
                    if( nodeTab.SelectSingleNode( "panes" ) != null )
                    {
                        ParsePanes( nodeTab.SelectSingleNode( "panes" ), PortalId, intTabId, mergeTabs, hModules );
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
        /// <param name="IsAdminTemplate">Flag to indicate if we are parsing admin template</param>
        /// <history>
        /// 	[Vicenç]	15/10/2004	Created
        /// </history>
        private TabPermissionCollection ParseTabPermissions( XmlNodeList nodeTabPermissions, PortalInfo objPortal, int TabId, bool IsAdminTemplate )
        {
            TabPermissionCollection objTabPermissions = new TabPermissionCollection();
            PermissionController objPermissionController = new PermissionController();
            PermissionInfo objPermission = null;
            TabPermissionInfo objTabPermission = null;
            RoleController objRoleController = new RoleController();
            RoleInfo objRole = null;
            int RoleID = 0;
            int PermissionID = 0;
            string PermissionKey = null;
            string PermissionCode = null;
            string RoleName = null;
            bool AllowAccess = false;
            ArrayList arrPermissions = null;
            int i = 0;

            foreach( XmlNode xmlTabPermission in nodeTabPermissions )
            {
                PermissionKey = XmlUtils.GetNodeValue( xmlTabPermission, "permissionkey", "" );
                PermissionCode = XmlUtils.GetNodeValue( xmlTabPermission, "permissioncode", "" );
                RoleName = XmlUtils.GetNodeValue( xmlTabPermission, "rolename", "" );
                AllowAccess = XmlUtils.GetNodeValueBoolean( xmlTabPermission, "allowaccess", false );
                arrPermissions = objPermissionController.GetPermissionByCodeAndKey( PermissionCode, PermissionKey );

                for( i = 0; i < arrPermissions.Count; i++ )
                {
                    objPermission = (PermissionInfo)( arrPermissions[i] );
                    PermissionID = objPermission.PermissionID;
                }
                RoleID = int.MinValue;
                if( RoleName == Globals.glbRoleAllUsersName )
                {
                    RoleID = Convert.ToInt32( Globals.glbRoleAllUsers );
                }
                else if( RoleName == Globals.glbRoleUnauthUserName )
                {
                    RoleID = Convert.ToInt32( Globals.glbRoleUnauthUser );
                }
                else
                {
                    objRole = objRoleController.GetRoleByName( objPortal.PortalID, RoleName );
                    if( objRole != null )
                    {
                        RoleID = objRole.RoleID;
                    }
                    else
                    {
                        // if parsing admin.template and role administrators redefined, use portal.administratorroleid
                        if( IsAdminTemplate && RoleName.ToLower() == "administrators" )
                        {
                            RoleID = objPortal.AdministratorRoleId;
                        }
                    }
                }

                // if role was found add, otherwise ignore
                if( RoleID != int.MinValue )
                {
                    objTabPermission = new TabPermissionInfo();
                    objTabPermission.TabID = TabId;
                    objTabPermission.PermissionID = PermissionID;
                    objTabPermission.RoleID = RoleID;
                    objTabPermission.AllowAccess = AllowAccess;
                    objTabPermissions.Add( objTabPermission );
                }
            }
            return objTabPermissions;
        }

        private void UpdatePortalSetup( int PortalId, int AdministratorId, int AdministratorRoleId, int RegisteredRoleId, int SplashTabId, int HomeTabId, int LoginTabId, int UserTabId, int AdminTabId )
        {
            DataProvider.Instance().UpdatePortalSetup( PortalId, AdministratorId, AdministratorRoleId, RegisteredRoleId, SplashTabId, HomeTabId, LoginTabId, UserTabId, AdminTabId );
            DataCache.ClearPortalCache( PortalId, true );
        }

        /// <summary>
        /// Creates a new portal alias
        /// </summary>
        /// <param name="PortalId">Id of the portal</param>
        /// <param name="PortalAlias">Portal Alias to be created</param>
        /// <history>
        ///     [cnurse]    01/11/2005  created
        /// </history>
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
        /// <history>
        /// 	[cnurse]	11/08/2004	created (most of this code was moved from SignUp.ascx.vb)
        /// </history>
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
                    HomeDirectory = "Portals/" + intPortalId.ToString();
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
                    catch( Exception Exc )
                    {
                        strMessage += Localization.GetString( "CreateAdminUser.Error" ) + Exc.Message + Exc.StackTrace;
                    }
                }
                else
                {
                    throw new Exception( strMessage );
                }

                if( strMessage == "" & AdministratorId > 0 )
                {
                    try
                    {
                        // the upload directory may already exist if this is a new DB working with a previously installed application
                        if( Directory.Exists( MappedHomeDirectory ) )
                        {
                            Globals.DeleteFolderRecursive( MappedHomeDirectory );
                        }
                    }
                    catch( Exception Exc )
                    {
                        strMessage += Localization.GetString( "DeleteUploadFolder.Error" ) + Exc.Message + Exc.StackTrace;
                    }

                    //Set up Child Portal
                    if( strMessage == Null.NullString )
                    {
                        try
                        {
                            if( IsChildPortal )
                            {
                                // create the subdirectory for the new portal
                                if( !( Directory.Exists( ChildPath ) ) )
                                {
                                    Directory.CreateDirectory( ChildPath );
                                }

                                // create the subhost default.aspx file
                                if( !( File.Exists( ChildPath + "\\" + Globals.glbDefaultPage ) ) )
                                {
                                    File.Copy( Globals.HostMapPath + "subhost.aspx", ChildPath + "\\" + Globals.glbDefaultPage );
                                }
                            }
                        }
                        catch( Exception Exc )
                        {
                            strMessage += Localization.GetString( "ChildPortal.Error" ) + Exc.Message + Exc.StackTrace;
                        }
                    }
                    else
                    {
                        throw new Exception( strMessage );
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
                        catch( Exception Exc )
                        {
                            strMessage += Localization.GetString( "ChildPortal.Error" ) + Exc.Message + Exc.StackTrace;
                        }
                    }
                    else
                    {
                        throw new Exception( strMessage );
                    }

                    if( strMessage == Null.NullString )
                    {
                        // parse portal template
                        try
                        {
                            ParseTemplate( intPortalId, TemplatePath, TemplateFile, AdministratorId, PortalTemplateModuleAction.Replace, true );
                        }
                        catch( Exception Exc )
                        {
                            strMessage += Localization.GetString( "PortalTemplate.Error" ) + Exc.Message + Exc.StackTrace;
                        }

                        // parse admin template
                        try
                        {
                            ParseTemplate( intPortalId, TemplatePath, "admin.template", AdministratorId, PortalTemplateModuleAction.Replace, true );
                        }
                        catch( Exception Exc )
                        {
                            strMessage += Localization.GetString( "AdminTemplate.Error" ) + Exc.Message + Exc.StackTrace;
                        }
                    }
                    else
                    {
                        throw new Exception( strMessage );
                    }

                    if( strMessage == Null.NullString )
                    {
                        // update portal setup
                        PortalInfo objportal = GetPortal( intPortalId );

                        // update portal info
                        objportal.Description = Description;
                        objportal.KeyWords = KeyWords;
                        UpdatePortalInfo( objportal.PortalID, objportal.PortalName, objportal.LogoFile, objportal.FooterText, objportal.ExpiryDate, objportal.UserRegistration, objportal.BannerAdvertising, objportal.Currency, objportal.AdministratorId, objportal.HostFee, objportal.HostSpace, objportal.PageQuota, objportal.UserQuota, objportal.PaymentProcessor, objportal.ProcessorUserId, objportal.ProcessorPassword, objportal.Description, objportal.KeyWords, objportal.BackgroundFile, objportal.SiteLogHistory, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.UserTabId, objportal.DefaultLanguage, objportal.TimeZoneOffset, objportal.HomeDirectory );

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
                        catch( Exception ex )
                        {
                            // error
                        }
                    }
                    else
                    {
                        throw new Exception( strMessage );
                    }
                }
                else // clean up
                {
                    DeletePortalInfo( intPortalId );
                    intPortalId = -1;
                    throw new Exception( strMessage );
                }
            }
            else
            {
                strMessage += Localization.GetString( "CreatePortal.Error" );
                throw new Exception( strMessage );
            }

            return intPortalId;
        }

        /// <summary>
        /// Deletes a portal permanently
        /// </summary>
        /// <param name="PortalId">PortalId of the portal to be deleted</param>
        /// <history>
        /// 	[VMasanas]	03/09/2004	Created
        /// 	[VMasanas]	26/10/2004	Remove dependent data (skins, modules)
        ///     [cnurse]    24/11/2006  Removal of Modules moved to sproc
        /// </history>
        public void DeletePortalInfo( int PortalId )
        {
            // remove skin assignments
            SkinController.SetSkin( SkinInfo.RootSkin, PortalId, SkinType.Portal, "" );
            SkinController.SetSkin( SkinInfo.RootContainer, PortalId, SkinType.Portal, "" );
            SkinController.SetSkin( SkinInfo.RootSkin, PortalId, SkinType.Admin, "" );
            SkinController.SetSkin( SkinInfo.RootContainer, PortalId, SkinType.Admin, "" );

            //delete portal users
            UserController.DeleteUsers( PortalId, false, true );

            //delete portal
            DataProvider.Instance().DeletePortalInfo( PortalId );

            // clear portal alias cache and entire portal
            DataCache.ClearHostCache( true );
        }

        public static PortalSettings GetCurrentPortalSettings()
        {
            return (PortalSettings)( HttpContext.Current.Items["PortalSettings"] );
        }

        /// <summary>
        /// Gets information of a portal
        /// </summary>
        /// <param name="PortalId">Id of the portal</param>
        /// <returns>PortalInfo object with portal definition</returns>
        public PortalInfo GetPortal( int PortalId )
        {
            string key = string.Format( DataCache.PortalCacheKey, PortalId );

            //First Check the Portal Cache
            PortalInfo portal = DataCache.GetPersistentCacheItem( key, typeof( PortalInfo ) ) as PortalInfo;

            if( portal == null )
            {
                //portal caching settings
                Int32 timeOut = DataCache.PortalCacheTimeOut*Convert.ToInt32( Globals.PerformanceSetting );

                //Get tabs form Database
                IDataReader dr = DataProvider.Instance().GetPortal( PortalId );
                try
                {
                    portal = FillPortalInfo( dr );
                }
                finally
                {
                    if( dr != null )
                    {
                        dr.Close();
                    }
                }

                //Cache tabs
                if( timeOut > 0 )
                {
                    DataCache.SetCache( key, portal, TimeSpan.FromMinutes( timeOut ), true );
                }
            }

            //Make sure that portal is in the Portal Dictionary
            AddPortalToDictionary( PortalId );

            return portal;
        }

        /// <summary>
        /// Gets information from all portals
        /// </summary>
        /// <returns>ArrayList of PortalInfo objects</returns>
        public ArrayList GetPortals()
        {
            return FillPortalInfoCollection( DataProvider.Instance().GetPortals() );
        }

        /// <summary>
        /// Gets the space used at the host level
        /// </summary>
        /// <returns>Space used in bytes</returns>
        /// <history>
        /// 	[VMasanas]	19/04/2006	Created
        /// </history>
        public long GetPortalSpaceUsedBytes()
        {
            return GetPortalSpaceUsedBytes( -1 );
        }

        /// <summary>
        /// Gets the space used by a portal in bytes
        /// </summary>
        /// <param name="PortalId">Id of the portal</param>
        /// <returns>Space used in bytes</returns>
        /// <history>
        /// 	[VMasanas]	07/04/2006	Created
        ///     [VMasanas]  11/05/2006  Use file size stored on the db. This is necessary
        ///         to take into account the new secure file storage options
        /// </history>
        public long GetPortalSpaceUsedBytes( int portalId )
        {
            long size = 0;

            IDataReader dr = DataProvider.Instance().GetPortalSpaceUsed( portalId );
            if( dr.Read() )
            {
                if( !( Convert.IsDBNull( dr["SpaceUsed"] ) ) )
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
        /// <param name="PortalId">Id of the portal</param>
        /// <param name="fileSizeBytes">Size of the file being uploaded</param>
        /// <returns>True if there's enough space available to upload the file</returns>
        /// <history>
        /// 	[VMasanas]	19/04/2006	Created
        /// </history>
        public bool HasSpaceAvailable( int portalId, long fileSizeBytes )
        {
            int hostSpace = 0;

            if( portalId == -1 )
            {
                hostSpace = 0;
            }
            else
            {
                PortalSettings ps = GetCurrentPortalSettings();
                if( ps != null & ps.PortalId == portalId )
                {
                    hostSpace = ps.HostSpace;
                }
                else
                {
                    PortalInfo portal = GetPortal( portalId );
                    hostSpace = portal.HostSpace;
                }
            }

            return ( ( ( GetPortalSpaceUsedBytes( portalId ) + fileSizeBytes )/( Math.Pow( 1024, 2 ) ) ) <= hostSpace ) | hostSpace == 0;
        }

        /// <summary>
        /// Processes all panes and modules in the template file
        /// </summary>
        /// <param name="nodePanes">Template file node for the panes is current tab</param>
        /// <param name="PortalId">PortalId of the new portal</param>
        /// <param name="TabId">Tab being processed</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	03/09/2004	Created
        /// 	[VMasanas]	15/10/2004	Modified for new skin structure
        ///		[cnurse]	15/10/2004	Modified to allow for merging template
        ///								with existing pages
        /// </history>
        public void ParsePanes( XmlNode nodePanes, int PortalId, int TabId, PortalTemplateModuleAction mergeTabs, Hashtable hModules )
        {
            XmlNode nodePane = null;
            XmlNode nodeModule = null;
            ModuleDefinitionController objModuleDefinitions = new ModuleDefinitionController();
            ModuleDefinitionInfo objModuleDefinition = null;
            ModuleController objModules = new ModuleController();
            Dictionary<int, ModuleInfo> dicModules = objModules.GetTabModules( TabId );
            ModuleInfo objModule = null;
            int intModuleId = 0;
            string modTitle = null;
            bool moduleFound = false;

            PortalInfo objportal = null;
            objportal = GetPortal( PortalId );

            //If Mode is Replace remove all the modules already on this Tab
            if( mergeTabs == PortalTemplateModuleAction.Replace )
            {
                foreach( KeyValuePair<int, ModuleInfo> kvp in dicModules )
                {
                    objModule = kvp.Value;
                    objModules.DeleteTabModule( TabId, objModule.ModuleID );
                }
            }

            // iterate through the panes
            foreach( XmlNode nodePaneWithinLoop in nodePanes.ChildNodes )
            {
                nodePane = nodePaneWithinLoop;

                // iterate through the modules
                if( nodePaneWithinLoop.SelectSingleNode( "modules" ) != null )
                {
                    foreach( XmlNode nodeModuleWithinLoop in nodePaneWithinLoop.SelectSingleNode( "modules" ) )
                    {
                        nodeModule = nodeModuleWithinLoop;
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
                        DesktopModuleInfo objDesktopModule = Globals.GetDesktopModuleByName( XmlUtils.GetNodeValue( nodeModule, "definition", "" ) );
                        if( objDesktopModule != null )
                        {
                            //Get the moduleDefinition from the <moduledefinition> element
                            string friendlyName = XmlUtils.GetNodeValue( nodeModule, "moduledefinition", "" );

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
                                moduleFound = false;
                                modTitle = XmlUtils.GetNodeValue( nodeModuleWithinLoop, "title", "" );
                                if( mergeTabs == PortalTemplateModuleAction.Merge )
                                {
                                    foreach( KeyValuePair<int, ModuleInfo> kvp in dicModules )
                                    {
                                        objModule = kvp.Value;
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
                                    objModule.PaneName = XmlUtils.GetNodeValue( nodePaneWithinLoop, "name", "" );
                                    objModule.ModuleDefID = objModuleDefinition.ModuleDefID;
                                    objModule.CacheTime = XmlUtils.GetNodeValueInt( nodeModuleWithinLoop, "cachetime", 0 );
                                    objModule.Alignment = XmlUtils.GetNodeValue( nodeModuleWithinLoop, "alignment", "" );
                                    objModule.IconFile = ImportFile( PortalId, XmlUtils.GetNodeValue( nodeModuleWithinLoop, "iconfile", "" ) );
                                    objModule.AllTabs = XmlUtils.GetNodeValueBoolean( nodeModuleWithinLoop, "alltabs", false );
                                    switch( XmlUtils.GetNodeValue( nodeModuleWithinLoop, "visibility", "" ) )
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
                                    objModule.Color = XmlUtils.GetNodeValue( nodeModuleWithinLoop, "color", "" );
                                    objModule.Border = XmlUtils.GetNodeValue( nodeModuleWithinLoop, "border", "" );
                                    objModule.Header = XmlUtils.GetNodeValue( nodeModuleWithinLoop, "header", "" );
                                    objModule.Footer = XmlUtils.GetNodeValue( nodeModuleWithinLoop, "footer", "" );
                                    objModule.InheritViewPermissions = XmlUtils.GetNodeValueBoolean( nodeModuleWithinLoop, "inheritviewpermissions", false );
                                    objModule.ModulePermissions = new ModulePermissionCollection();

                                    objModule.StartDate = XmlUtils.GetNodeValueDate( nodeModuleWithinLoop, "startdate", Null.NullDate );
                                    objModule.EndDate = XmlUtils.GetNodeValueDate( nodeModuleWithinLoop, "enddate", Null.NullDate );

                                    if( XmlUtils.GetNodeValue( nodeModuleWithinLoop, "containersrc", "" ) != "" )
                                    {
                                        objModule.ContainerSrc = XmlUtils.GetNodeValue( nodeModuleWithinLoop, "containersrc", "" );
                                    }
                                    objModule.DisplayTitle = XmlUtils.GetNodeValueBoolean( nodeModuleWithinLoop, "displaytitle", true );
                                    objModule.DisplayPrint = XmlUtils.GetNodeValueBoolean( nodeModuleWithinLoop, "displayprint", true );
                                    objModule.DisplaySyndicate = XmlUtils.GetNodeValueBoolean( nodeModuleWithinLoop, "displaysyndicate", false );

                                    if( !IsInstance )
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

                                    if( XmlUtils.GetNodeValue( nodeModuleWithinLoop, "content", "" ) != "" & !IsInstance )
                                    {
                                        objModule = objModules.GetModule( intModuleId, TabId, true );
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
                                    if( !IsInstance )
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
        /// <history>
        /// 	[VMasanas]	27/08/2004	Created
        /// </history>
        public void ParseTemplate( int PortalId, string TemplatePath, string TemplateFile, int AdministratorId, PortalTemplateModuleAction mergeTabs, bool IsNewPortal )
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode node = null;
            int AdministratorRoleId = -1;
            int RegisteredRoleId = -1;
            int SubscriberRoleId = -1;
            RoleController objrole = new RoleController();
            bool isAdminTemplate = false;

            isAdminTemplate = ( TemplateFile == "admin.template" );

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
                    AdministratorRoleId = CreateRole( PortalId, "Administrators", "Portal Administrators", 0F, 0, "M", 0F, 0, "N", false, false );
                }
                if( RegisteredRoleId == -1 )
                {
                    RegisteredRoleId = CreateRole( PortalId, "Registered Users", "Registered Users", 0F, 0, "M", 0F, 0, "N", false, true );
                }
                if( SubscriberRoleId == -1 )
                {
                    SubscriberRoleId = CreateRole( PortalId, "Subscribers", "A public role for portal subscriptions", 0F, 0, "M", 0F, 0, "N", true, true );
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
                if( node != null & IsNewPortal )
                {
                    ParsePortalSettings( node, PortalId );
                }

                // update portal setup
                PortalInfo objportal = null;
                objportal = GetPortal( PortalId );
                UpdatePortalSetup( PortalId, AdministratorId, AdministratorRoleId, RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.UserTabId, objportal.AdminTabId );

                //Remove Exising Tabs if doing a "Replace"
                if( mergeTabs == PortalTemplateModuleAction.Replace )
                {
                    TabController objTabs = new TabController();
                    TabInfo objTab = null;
                    foreach( KeyValuePair<int, TabInfo> tabPair in objTabs.GetTabsByPortal( PortalId ) )
                    {
                        objTab = tabPair.Value;
                        if( !objTab.IsAdminTab )
                        {
                            //soft delete Tab
                            objTab.TabName = objTab.TabName + "_old";
                            objTab.IsDeleted = true;
                            objTabs.UpdateTab( objTab );
                            //Delete all Modules
                            ModuleController objModules = new ModuleController();
                            ModuleInfo objModule = null;
                            foreach( KeyValuePair<int, ModuleInfo> modulePair in objModules.GetTabModules( objTab.TabID ) )
                            {
                                objModule = modulePair.Value;
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
        /// <history>
        /// 	[VMasanas]	10/09/2004	Created
        ///     [cnurse]    11/08/2004  Moved from SignUp to PortalController
        ///     [cnurse]    03/04/2005  made Public
        ///     [cnurse]    05/20/2005  moved most of processing to new method in FileSystemUtils
        /// </history>
        public void ProcessResourceFile( string portalPath, string TemplateFile )
        {
            ZipInputStream objZipInputStream = null;
            try
            {
                objZipInputStream = new ZipInputStream( new FileStream( TemplateFile + ".resources", FileMode.Open, FileAccess.Read ) );
                FileSystemUtils.UnzipResources( objZipInputStream, portalPath );
            }
            catch( Exception exc )
            {
                // error opening file
            }
        }

        public void UpdatePortalExpiry( int PortalId )
        {
            DateTime ExpiryDate = DateTime.MinValue;

            IDataReader dr = DataProvider.Instance().GetPortal( PortalId );
            if( dr.Read() )
            {
                if( Convert.IsDBNull( dr["ExpiryDate"] ) )
                {
                    ExpiryDate = Convert.ToDateTime( dr["ExpiryDate"] );
                }
                else
                {
                    ExpiryDate = DateTime.Now;
                }

                DataProvider.Instance().UpdatePortalInfo( PortalId, Convert.ToString( dr["PortalName"] ), Convert.ToString( dr["LogoFile"] ), Convert.ToString( dr["FooterText"] ), ExpiryDate.AddMonths( 1 ), Convert.ToInt32( dr["UserRegistration"] ), Convert.ToInt32( dr["BannerAdvertising"] ), Convert.ToString( dr["Currency"] ), Convert.ToInt32( dr["AdministratorId"] ), Convert.ToDouble( dr["HostFee"] ), Convert.ToDouble( dr["HostSpace"] ), Convert.ToInt32( dr["PageQuota"] ), Convert.ToInt32( dr["UserQuota"] ), Convert.ToString( dr["PaymentProcessor"] ), Convert.ToString( dr["ProcessorUserId"] ), Convert.ToString( dr["ProcessorPassword"] ), Convert.ToString( dr["Description"] ), Convert.ToString( dr["KeyWords"] ), Convert.ToString( dr["BackgroundFile"] ), Convert.ToInt32( dr["SiteLogHistory"] ), Convert.ToInt32( dr["SplashTabId"] ), Convert.ToInt32( dr["HomeTabId"] ), Convert.ToInt32( dr["LoginTabId"] ), Convert.ToInt32( dr["UserTabId"] ), Convert.ToString( dr["DefaultLanguage"] ), Convert.ToInt32( dr["TimeZoneOffset"] ), Convert.ToString( dr["HomeDirectory"] ) );
            }
            dr.Close();
        }

        /// <summary>
        /// Updates basic portal information
        /// </summary>
        /// <param name="Portal"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/13/2004	created
        /// </history>
        public void UpdatePortalInfo( PortalInfo Portal )
        {
            UpdatePortalInfo( Portal.PortalID, Portal.PortalName, Portal.LogoFile, Portal.FooterText, Portal.ExpiryDate, Portal.UserRegistration, Portal.BannerAdvertising, Portal.Currency, Portal.AdministratorId, Portal.HostFee, Portal.HostSpace, Portal.PageQuota, Portal.UserQuota, Portal.PaymentProcessor, Portal.ProcessorUserId, Portal.ProcessorPassword, Portal.Description, Portal.KeyWords, Portal.BackgroundFile, Portal.SiteLogHistory, Portal.SplashTabId, Portal.HomeTabId, Portal.LoginTabId, Portal.UserTabId, Portal.DefaultLanguage, Portal.TimeZoneOffset, Portal.HomeDirectory );
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
        /// <param name="HomeTabId"></param>
        /// <param name="LoginTabId"></param>
        /// <param name="UserTabId"></param>
        /// <param name="DefaultLanguage"></param>
        /// <param name="TimeZoneOffset"></param>
        public void UpdatePortalInfo( int PortalId, string PortalName, string LogoFile, string FooterText, DateTime ExpiryDate, int UserRegistration, int BannerAdvertising, string Currency, int AdministratorId, double HostFee, double HostSpace, int PageQuota, int UserQuota, string PaymentProcessor, string ProcessorUserId, string ProcessorPassword, string Description, string KeyWords, string BackgroundFile, int SiteLogHistory, int SplashTabId, int HomeTabId, int LoginTabId, int UserTabId, string DefaultLanguage, int TimeZoneOffset, string HomeDirectory )
        {
            DataProvider.Instance().UpdatePortalInfo( PortalId, PortalName, LogoFile, FooterText, ExpiryDate, UserRegistration, BannerAdvertising, Currency, AdministratorId, HostFee, HostSpace, PageQuota, UserQuota, PaymentProcessor, ProcessorUserId, ProcessorPassword, Description, KeyWords, BackgroundFile, SiteLogHistory, SplashTabId, HomeTabId, LoginTabId, UserTabId, DefaultLanguage, TimeZoneOffset, HomeDirectory );

            // clear portal settings
            DataCache.ClearPortalCache( PortalId, true );
        }

        /// <summary>
        /// Gets the space used by a portal in bytes
        /// </summary>
        /// <param name="PortalId">Id of the portal</param>
        ///  <returns>Space used in bytes</returns>
        /// <remarks>
        /// If PortalId is -1 or not  present (defaults to -1) the host space (\Portals\_default\) will be returned.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	27/08/2004	Changed to return the real space used by a portal looking at the files on disc.
        /// 	[VMasanas]	07/04/2006	Marked as obsolete (dnn 3.3). Will return Interger.Max if size > 2 GB (int limit)
        /// </history>
        public int GetPortalSpaceUsed()
        {
            return GetPortalSpaceUsed( -1 );
        }

        [Obsolete( "This function has been replaced by GetPortalSpaceUsedBytes" )]
        public int GetPortalSpaceUsed( int portalId )
        {
            int size = 0;
            try
            {
                size = Convert.ToInt32( GetPortalSpaceUsedBytes( portalId ) );
            }
            catch( Exception ex )
            {
                size = int.MaxValue;
            }

            return size;
        }
    }
}