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
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.Utilities;
using Calendar=DotNetNuke.Common.Utilities.Calendar;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.PortalManagement
{
    /// <summary>
    /// The SiteSettings PortalModuleBase is used to edit the main settings for a
    /// portal.
    /// </summary>
    /// <history>
    /// 	[cnurse]	9/8/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class SiteSettings : PortalModuleBase
    {
        protected SectionHeadControl dshSecurity;

        protected SectionHeadControl dshOther;
        protected LabelControl plAdministratorId;

        protected Panel pnlStyleSheet;
        protected Label lblStyleSheet;

        protected HtmlTable tblHostingDetails;
        protected TextBox txtPortalAlias;

        private int intPortalId = - 1;

        /// <summary>
        /// LoadStyleSheet loads the stylesheet
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/8/2004	Created
        /// </history>
        private void LoadStyleSheet()
        {
            string strUploadDirectory = "";

            PortalController objPortalController = new PortalController();
            PortalInfo objPortal = objPortalController.GetPortal( intPortalId );
            if( objPortal != null )
            {
                strUploadDirectory = objPortal.HomeDirectoryMapPath;
            }

            // read CSS file
            if( File.Exists( strUploadDirectory + "portal.css" ) )
            {
                StreamReader objStreamReader;
                objStreamReader = File.OpenText( strUploadDirectory + "portal.css" );
                txtStyleSheet.Text = objStreamReader.ReadToEnd();
                objStreamReader.Close();
            }
        }

        /// <summary>
        /// SkinChanged changes the skin
        /// </summary>
        /// <history>
        /// 	[cnurse]	10/19/2004	documented
        /// </history>
        private static bool SkinChanged( string SkinRoot, int PortalId, SkinType SkinType, string PostedSkinSrc )
        {
            string strSkinSrc = "";
            SkinInfo objSkinInfo = SkinController.GetSkin( SkinRoot, PortalId, SkinType.Admin );
            if( objSkinInfo != null )
            {
                strSkinSrc = objSkinInfo.SkinSrc;
            }
            if( strSkinSrc == null )
            {
                strSkinSrc = "";
            }
            return strSkinSrc != PostedSkinSrc;
        }

        /// <summary>
        /// FormatCurrency formats the currency.
        /// control.
        /// </summary>
        /// <returns>A formatted string</returns>
        /// <history>
        /// 	[cnurse]	9/8/2004	Modified
        /// </history>
        public string FormatCurrency()
        {
            string retValue = "";
            try
            {
                retValue = Convert.ToString( Globals.HostSettings["HostCurrency"] ) + " / " + Localization.GetString( "Month" );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }

            return retValue;
        }

        /// <summary>
        /// FormatFee formats the fee.
        /// control.
        /// </summary>
        /// <returns>A formatted string</returns>
        /// <history>
        /// 	[cnurse]	9/8/2004	Modified
        /// </history>
        public string FormatFee( object objHostFee )
        {
            string retValue = "";

            try
            {
                //TODO - this needs to be localised
                if( objHostFee != DBNull.Value )
                {
                    retValue = String.Format( "#,##0.00", objHostFee );
                }
                else
                {
                    retValue = "0";
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }

            return retValue;
        }

        /// <summary>
        /// IsSubscribed determines whether the portal has subscribed to the premium
        /// control.
        /// </summary>
        /// <returns>True if Subscribed, False if not</returns>
        /// <history>
        /// 	[cnurse]	9/8/2004	Modified
        /// </history>
        public bool IsSubscribed( int PortalModuleDefinitionId )
        {
            try
            {
                return Null.IsNull( PortalModuleDefinitionId ) == false;
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return false;
        }

        /// <summary>
        /// IsSuperUser determines whether the cuurent user is a SuperUser
        /// control.
        /// </summary>
        /// <returns>True if SuperUser, False if not</returns>
        /// <history>
        /// 	[cnurse]	10/4/2004	Added
        /// </history>
        public bool IsSuperUser()
        {
            return UserInfo.IsSuperUser;
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/8/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ( Request.QueryString["pid"] != null ) && ( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId || UserInfo.IsSuperUser ) )
                {
                    intPortalId = int.Parse( Request.QueryString["pid"] );
                    ctlLogo.ShowUpLoad = false;
                    ctlBackground.ShowUpLoad = false;
                }
                else
                {
                    intPortalId = PortalId;
                    ctlLogo.ShowUpLoad = true;
                    ctlBackground.ShowUpLoad = true;
                }

                //this needs to execute always to the client script code is registred in InvokePopupCal
                cmdExpiryCalendar.NavigateUrl = Calendar.InvokePopupCal( txtExpiryDate );
                ClientAPI.AddButtonConfirm( cmdRestore, Localization.GetString( "RestoreCCSMessage", LocalResourceFile ) );

                // If this is the first visit to the page, populate the site data
                if( Page.IsPostBack == false )
                {
                    ClientAPI.AddButtonConfirm( cmdDelete, Localization.GetString( "DeleteMessage", LocalResourceFile ) );

                    PortalController objPortalController = new PortalController();
                    ListController ctlList = new ListController();
                    ListEntryInfoCollection colProcessor = ctlList.GetListEntryInfoCollection( "Processor" );

                    cboProcessor.DataSource = colProcessor;
                    cboProcessor.DataBind();
                    cboProcessor.Items.Insert( 0, new ListItem( "<" + Localization.GetString( "None_Specified" ) + ">", "" ) );

                    PortalInfo objPortal = objPortalController.GetPortal( intPortalId );

                    txtPortalName.Text = objPortal.PortalName;
                    ctlLogo.Url = objPortal.LogoFile;
                    ctlLogo.FileFilter = Globals.glbImageFileTypes;
                    txtDescription.Text = objPortal.Description;
                    txtKeyWords.Text = objPortal.KeyWords;
                    ctlBackground.Url = objPortal.BackgroundFile;
                    ctlBackground.FileFilter = Globals.glbImageFileTypes;
                    txtFooterText.Text = objPortal.FooterText;
                    optUserRegistration.SelectedIndex = objPortal.UserRegistration;
                    optBannerAdvertising.SelectedIndex = objPortal.BannerAdvertising;

                    cboSplashTabId.DataSource = Globals.GetPortalTabs( intPortalId, true, true, false, false, false );
                    cboSplashTabId.DataBind();
                    if( cboSplashTabId.Items.FindByValue( objPortal.SplashTabId.ToString() ) != null )
                    {
                        cboSplashTabId.Items.FindByValue( objPortal.SplashTabId.ToString() ).Selected = true;
                    }
                    cboHomeTabId.DataSource = Globals.GetPortalTabs( intPortalId, true, true, false, false, false );
                    cboHomeTabId.DataBind();
                    if( cboHomeTabId.Items.FindByValue( objPortal.HomeTabId.ToString() ) != null )
                    {
                        cboHomeTabId.Items.FindByValue( objPortal.HomeTabId.ToString() ).Selected = true;
                    }
                    cboLoginTabId.DataSource = Globals.GetPortalTabs( intPortalId, true, true, false, false, false );
                    cboLoginTabId.DataBind();
                    if( cboLoginTabId.Items.FindByValue( objPortal.LoginTabId.ToString() ) != null )
                    {
                        cboLoginTabId.Items.FindByValue( objPortal.LoginTabId.ToString() ).Selected = true;
                    }
                    cboUserTabId.DataSource = Globals.GetPortalTabs( intPortalId, true, true, false, false, false );
                    cboUserTabId.DataBind();
                    if( cboUserTabId.Items.FindByValue( objPortal.UserTabId.ToString() ) != null )
                    {
                        cboUserTabId.Items.FindByValue( objPortal.UserTabId.ToString() ).Selected = true;
                    }

                    ListEntryInfoCollection colList = ctlList.GetListEntryInfoCollection( "Currency" );

                    cboCurrency.DataSource = colList;
                    cboCurrency.DataBind();
                    if( Null.IsNull( objPortal.Currency ) || cboCurrency.Items.FindByValue( objPortal.Currency ) == null )
                    {
                        cboCurrency.Items.FindByValue( "USD" ).Selected = true;
                    }
                    else
                    {
                        cboCurrency.Items.FindByValue( objPortal.Currency ).Selected = true;
                    }
                    RoleController objRoleController = new RoleController();

                    ArrayList Arr = objRoleController.GetUserRolesByRoleName( intPortalId, objPortal.AdministratorRoleName );
                    int i;
                    for( i = 0; i <= Arr.Count - 1; i++ )
                    {
                        UserRoleInfo objUser = (UserRoleInfo)Arr[i];
                        cboAdministratorId.Items.Add( new ListItem( objUser.FullName, objUser.UserID.ToString() ) );
                    }
                    if( cboAdministratorId.Items.FindByValue( objPortal.AdministratorId.ToString() ) != null )
                    {
                        cboAdministratorId.Items.FindByValue( objPortal.AdministratorId.ToString() ).Selected = true;
                    }

                    if( ! Null.IsNull( objPortal.ExpiryDate ) )
                    {
                        txtExpiryDate.Text = objPortal.ExpiryDate.ToShortDateString();
                    }
                    txtHostFee.Text = objPortal.HostFee.ToString();
                    txtHostSpace.Text = objPortal.HostSpace.ToString();
                    txtPageQuota.Text = objPortal.PageQuota.ToString();
                    txtUserQuota.Text = objPortal.UserQuota.ToString();
                    if( objPortal.SiteLogHistory != 0 )
                    {
                        txtSiteLogHistory.Text = objPortal.SiteLogHistory.ToString();
                    }

                    DesktopModuleController objDesktopModules = new DesktopModuleController();
                    ArrayList arrDesktopModules = objDesktopModules.GetDesktopModules();

                    ArrayList arrPremiumModules = new ArrayList();                    
                    foreach( DesktopModuleInfo objDesktopModule in arrDesktopModules )
                    {                        
                        if( objDesktopModule.IsPremium )
                        {
                            arrPremiumModules.Add( objDesktopModule );
                        }
                    }

                    ArrayList arrPortalDesktopModules = objDesktopModules.GetPortalDesktopModules( intPortalId, Null.NullInteger );
                    foreach( PortalDesktopModuleInfo objPortalDesktopModule in arrPortalDesktopModules )
                    {
                        foreach( DesktopModuleInfo objDesktopModule in arrPremiumModules )
                        {                            
                            if( objDesktopModule.DesktopModuleID == objPortalDesktopModule.DesktopModuleID )
                            {
                                arrPremiumModules.Remove( objDesktopModule );
                                break;
                            }
                        }                                                
                    }

                    ctlDesktopModules.Available = arrPremiumModules;
                    ctlDesktopModules.Assigned = arrPortalDesktopModules;

                    if( !String.IsNullOrEmpty( objPortal.PaymentProcessor ) )
                    {
                        if( cboProcessor.Items.FindByText( objPortal.PaymentProcessor ) != null )
                        {
                            cboProcessor.Items.FindByText( objPortal.PaymentProcessor ).Selected = true;
                        }
                        else // default
                        {
                            if( cboProcessor.Items.FindByText( "PayPal" ) != null )
                            {
                                cboProcessor.Items.FindByText( "PayPal" ).Selected = true;
                            }
                        }
                    }
                    else
                    {
                        cboProcessor.Items.FindByValue( "" ).Selected = true;
                    }
                    txtUserId.Text = objPortal.ProcessorUserId;
                    txtPassword.Attributes.Add( "value", objPortal.ProcessorPassword );
                    txtHomeDirectory.Text = objPortal.HomeDirectory;

                    //Populate the default language combobox
                    Localization.LoadCultureDropDownList( cboDefaultLanguage, CultureDropDownTypes.NativeName, objPortal.DefaultLanguage );

                    //Populate the timezone combobox (look up timezone translations based on currently set culture)
                    Localization.LoadTimeZoneDropDownList( cboTimeZone, ( (PageBase)Page ).PageCulture.Name, Convert.ToString( objPortal.TimeZoneOffset ) );

                    SkinInfo objSkin;

                    ctlPortalSkin.Width = "275px";
                    ctlPortalSkin.SkinRoot = SkinInfo.RootSkin;
                    objSkin = SkinController.GetSkin( SkinInfo.RootSkin, PortalId, SkinType.Portal );
                    if( objSkin != null )
                    {
                        if( objSkin.PortalId == PortalId )
                        {
                            ctlPortalSkin.SkinSrc = objSkin.SkinSrc;
                        }
                    }
                    ctlPortalContainer.Width = "275px";
                    ctlPortalContainer.SkinRoot = SkinInfo.RootContainer;
                    objSkin = SkinController.GetSkin( SkinInfo.RootContainer, PortalId, SkinType.Portal );
                    if( objSkin != null )
                    {
                        if( objSkin.PortalId == PortalId )
                        {
                            ctlPortalContainer.SkinSrc = objSkin.SkinSrc;
                        }
                    }

                    ctlAdminSkin.Width = "275px";
                    ctlAdminSkin.SkinRoot = SkinInfo.RootSkin;
                    objSkin = SkinController.GetSkin( SkinInfo.RootSkin, PortalId, SkinType.Admin );
                    if( objSkin != null )
                    {
                        if( objSkin.PortalId == PortalId )
                        {
                            ctlAdminSkin.SkinSrc = objSkin.SkinSrc;
                        }
                    }
                    ctlAdminContainer.Width = "275px";
                    ctlAdminContainer.SkinRoot = SkinInfo.RootContainer;
                    objSkin = SkinController.GetSkin( SkinInfo.RootContainer, PortalId, SkinType.Admin );
                    if( objSkin != null )
                    {
                        if( objSkin.PortalId == PortalId )
                        {
                            ctlAdminContainer.SkinSrc = objSkin.SkinSrc;
                        }
                    }

                    LoadStyleSheet();

                    if( Convert.ToString( PortalSettings.HostSettings["SkinUpload"] ) == "G" && ! UserInfo.IsSuperUser )
                    {
                        lnkUploadSkin.Visible = false;
                        lnkUploadContainer.Visible = false;
                    }
                    else
                    {
                        ModuleInfo FileManagerModule = ( new ModuleController() ).GetModuleByDefinition( intPortalId, "File Manager" );
                        string[] parameters = new string[3];
                        parameters[0] = "mid=" + FileManagerModule.ModuleID;
                        parameters[1] = "ftype=" + UploadType.Skin;
                        parameters[2] = "rtab=" + TabId;
                        lnkUploadSkin.NavigateUrl = Globals.NavigateURL( FileManagerModule.TabID, "Edit", parameters );

                        parameters[1] = "ftype=" + UploadType.Container;
                        lnkUploadContainer.NavigateUrl = Globals.NavigateURL( FileManagerModule.TabID, "Edit", parameters );
                    }

                    if( Request.UrlReferrer != null )
                    {
                        if( Request.UrlReferrer.AbsoluteUri == Request.Url.AbsoluteUri )
                        {
                            ViewState["UrlReferrer"] = "";
                        }
                        else
                        {
                            ViewState["UrlReferrer"] = Convert.ToString( Request.UrlReferrer );
                        }
                    }
                    else
                    {
                        ViewState["UrlReferrer"] = "";
                    }
                }

                if( UserInfo.IsSuperUser )
                {
                    dshHost.Visible = true;
                    tblHost.Visible = true;
                    cmdDelete.Visible = true;
                    if( Convert.ToString( ViewState["UrlReferrer"] ) == "" )
                    {
                        cmdCancel.Visible = false;
                    }
                    else
                    {
                        cmdCancel.Visible = true;
                    }
                }
                else
                {
                    dshHost.Visible = false;
                    tblHost.Visible = false;
                    cmdDelete.Visible = false;
                    cmdCancel.Visible = false;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdCancel_Click runs when the Cancel LinkButton is clicked.  It returns the user
        /// to the referring page
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/9/2004	Modified
        /// </history>
        protected void cmdCancel_Click( object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Convert.ToString( ViewState["UrlReferrer"] ), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdDelete_Click runs when the Delete LinkButton is clicked.
        /// It deletes the current portal form the Database.  It can only run in Host
        /// (SuperUser) mode
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/9/2004	Modified
        ///     [VMasanas]  9/12/2004   Move skin deassignment to DeletePortalInfo.
        /// </history>
        protected void cmdDelete_Click( object sender, EventArgs e )
        {
            try
            {
                PortalController objPortalController = new PortalController();
                PortalInfo objPortalInfo = objPortalController.GetPortal( intPortalId );

                if( objPortalInfo != null )
                {
                    string strMessage = PortalController.DeletePortal( objPortalInfo, Globals.GetAbsoluteServerPath( Request ) );

                    if( string.IsNullOrEmpty( strMessage ) )
                    {
                        EventLogController objEventLog = new EventLogController();
                        objEventLog.AddLog( "PortalName", objPortalInfo.PortalName, PortalSettings, UserId, EventLogController.EventLogType.PORTAL_DELETED );

                        // Redirect to another site
                        if( intPortalId == PortalId )
                        {
                            if( PortalSettings.HostSettings["HostURL"].ToString() != "" )
                            {
                                Response.Redirect( Globals.AddHTTP( PortalSettings.HostSettings["HostURL"].ToString() ) );
                            }
                            else
                            {
                                Response.End();
                            }
                        }
                        else
                        {
                            Response.Redirect( Convert.ToString( ViewState["UrlReferrer"] ), true );
                        }
                    }
                    else
                    {
                        UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessageType.RedError );
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdGoogle_Click runs when the Submit to Google Linkbutton is clicked.
        /// It submits the site's Description, DomainName and Keywords to the Google Site.
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/9/2004	Modified
        /// </history>
        protected void cmdGoogle_Click( object sender, EventArgs e )
        {
            try
            {
                string strURL = "";
                string strComments = "";

                PortalController objPortalController = new PortalController();
                PortalInfo objPortal = objPortalController.GetPortal( intPortalId );
                if( objPortal != null )
                {
                    strComments += objPortal.PortalName;
                    if( !String.IsNullOrEmpty( objPortal.Description ) )
                    {
                        strComments += " " + objPortal.Description;
                    }
                    if( !String.IsNullOrEmpty( objPortal.KeyWords ) )
                    {
                        strComments += " " + objPortal.KeyWords;
                    }
                }

                strURL += "http://www.google.com/addurl?q=" + Globals.HTTPPOSTEncode( Globals.AddHTTP( Globals.GetDomainName( Request ) ) );
                strURL += "&dq=" + Globals.HTTPPOSTEncode( strComments );
                strURL += "&submit=Add+URL";

                UrlUtils.OpenNewWindow( strURL );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdProcessor_Click runs when the Processor Website Linkbutton is clicked. It
        /// redirects the user to the selected processor's website.
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/9/2004	Modified
        /// </history>
        protected void cmdProcessor_Click( object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Globals.AddHTTP( cboProcessor.SelectedItem.Value ), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdRestore_Click runs when the Restore Default Stylesheet Linkbutton is clicked.
        /// It reloads the default stylesheet (copies from _default Portal to current Portal)
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/9/2004	Modified
        /// </history>
        protected void cmdRestore_Click( object sender, EventArgs e )
        {
            try
            {
                PortalController objPortalController = new PortalController();
                PortalInfo objPortal = objPortalController.GetPortal( intPortalId );
                if( objPortal != null )
                {
                    if( File.Exists( objPortal.HomeDirectoryMapPath + "portal.css" ) )
                    {
                        // delete existing style sheet
                        File.Delete( objPortal.HomeDirectoryMapPath + "portal.css" );
                    }
                    // copy the default style sheet to the upload directory
                    File.Copy( Globals.HostMapPath + "portal.css", objPortal.HomeDirectoryMapPath + "portal.css" );
                }

                LoadStyleSheet();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdSave_Click runs when the Save Stylesheet Linkbutton is clicked.  It saves
        /// the edited Stylesheet
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/9/2004	Modified
        /// </history>
        protected void cmdSave_Click( object sender, EventArgs e )
        {
            try
            {
                string strUploadDirectory = "";

                PortalController objPortalController = new PortalController();
                PortalInfo objPortal = objPortalController.GetPortal( intPortalId );
                if( objPortal != null )
                {
                    strUploadDirectory = objPortal.HomeDirectoryMapPath;
                }

                // reset attributes
                if( File.Exists( strUploadDirectory + "portal.css" ) )
                {
                    File.SetAttributes( strUploadDirectory + "portal.css", FileAttributes.Normal );
                }

                // write CSS file
                StreamWriter objStream;
                objStream = File.CreateText( strUploadDirectory + "portal.css" );
                objStream.WriteLine( txtStyleSheet.Text );
                objStream.Close();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update LinkButton is clicked.
        /// It saves the current Site Settings
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/9/2004	Modified
        /// </history>
        protected void cmdUpdate_Click( object sender, EventArgs e )
        {
            try
            {
                string strLogo;
                string strBackground;

                strLogo = ctlLogo.Url;
                strBackground = ctlBackground.Url;

                double dblHostFee = 0;
                if( !String.IsNullOrEmpty( txtHostFee.Text ) )
                {
                    dblHostFee = double.Parse( txtHostFee.Text );
                }

                double dblHostSpace = 0;
                if( !String.IsNullOrEmpty( txtHostSpace.Text ) )
                {
                    dblHostSpace = double.Parse( txtHostSpace.Text );
                }

                int intPageQuota = 0;
                if( txtPageQuota.Text != "" )
                {
                    intPageQuota = int.Parse( txtPageQuota.Text );
                }

                double intUserQuota = 0;
                if( txtUserQuota.Text != "" )
                {
                    intUserQuota = int.Parse( txtUserQuota.Text );
                }

                int intSiteLogHistory = - 1;
                if( !String.IsNullOrEmpty( txtSiteLogHistory.Text ) )
                {
                    intSiteLogHistory = int.Parse( txtSiteLogHistory.Text );
                }

                DateTime datExpiryDate = Null.NullDate;
                if( !String.IsNullOrEmpty( txtExpiryDate.Text ) )
                {
                    datExpiryDate = Convert.ToDateTime( txtExpiryDate.Text );
                }

                int intSplashTabId = Null.NullInteger;
                if( cboSplashTabId.SelectedItem != null )
                {
                    intSplashTabId = int.Parse( cboSplashTabId.SelectedItem.Value );
                }

                int intHomeTabId = Null.NullInteger;
                if( cboHomeTabId.SelectedItem != null )
                {
                    intHomeTabId = int.Parse( cboHomeTabId.SelectedItem.Value );
                }

                int intLoginTabId = Null.NullInteger;
                if( cboLoginTabId.SelectedItem != null )
                {
                    intLoginTabId = int.Parse( cboLoginTabId.SelectedItem.Value );
                }

                int intUserTabId = Null.NullInteger;
                if( cboUserTabId.SelectedItem != null )
                {
                    intUserTabId = int.Parse( cboUserTabId.SelectedItem.Value );
                }

                if( txtPassword.Attributes["value"] != null )
                {
                    txtPassword.Attributes["value"] = txtPassword.Text;
                }

                // update Portal info in the database
                PortalController objPortalController = new PortalController();
                //check only relevant fields altered
                if( ! UserInfo.IsSuperUser )
                {
                    PortalInfo objPortal = objPortalController.GetPortal( intPortalId );
                    bool HostChanged = false;
                    if( dblHostFee != objPortal.HostFee )
                    {
                        HostChanged = true;
                    }
                    if( dblHostSpace != objPortal.HostSpace )
                    {
                        HostChanged = true;
                    }
                    if( intPageQuota != objPortal.PageQuota )
                    {
                        HostChanged = true;
                    }
                    if( intUserQuota != objPortal.UserQuota )
                    {
                        HostChanged = true;
                    }
                    if( intSiteLogHistory != objPortal.SiteLogHistory )
                    {
                        HostChanged = true;
                    }
                    if( datExpiryDate != objPortal.ExpiryDate )
                    {
                        HostChanged = true;
                    }
                    if( HostChanged )
                    {
                        throw new Exception();
                    }
                }

                objPortalController.UpdatePortalInfo( intPortalId, txtPortalName.Text, strLogo, txtFooterText.Text, datExpiryDate, optUserRegistration.SelectedIndex, optBannerAdvertising.SelectedIndex, cboCurrency.SelectedItem.Value, Convert.ToInt32( cboAdministratorId.SelectedItem.Value ), dblHostFee, dblHostSpace, intPageQuota, (int)intUserQuota, ( ( cboProcessor.SelectedValue == "" ) ? "" : cboProcessor.SelectedItem.Text ).ToString(), txtUserId.Text, txtPassword.Text, txtDescription.Text, txtKeyWords.Text, strBackground, intSiteLogHistory, intSplashTabId, intHomeTabId, intLoginTabId, intUserTabId, cboDefaultLanguage.SelectedValue, Convert.ToInt32( cboTimeZone.SelectedValue ), txtHomeDirectory.Text );
                bool blnAdminSkinChanged = SkinChanged( SkinInfo.RootSkin, PortalId, SkinType.Admin, ctlAdminSkin.SkinSrc ) || SkinChanged( SkinInfo.RootContainer, PortalId, SkinType.Admin, ctlAdminContainer.SkinSrc );

                //Dim objSkins As New UI.Skins.SkinController
                SkinController.SetSkin( SkinInfo.RootSkin, PortalId, SkinType.Portal, ctlPortalSkin.SkinSrc );
                SkinController.SetSkin( SkinInfo.RootContainer, PortalId, SkinType.Portal, ctlPortalContainer.SkinSrc );
                SkinController.SetSkin( SkinInfo.RootSkin, PortalId, SkinType.Admin, ctlAdminSkin.SkinSrc );
                SkinController.SetSkin( SkinInfo.RootContainer, PortalId, SkinType.Admin, ctlAdminContainer.SkinSrc );

                if( UserInfo.IsSuperUser )
                {
                    // delete old portal module assignments
                    DesktopModuleController objDesktopModules = new DesktopModuleController();
                    objDesktopModules.DeletePortalDesktopModules( intPortalId, Null.NullInteger );
                    // add new portal module assignments                    
                    foreach( ListItem objListItem in ctlDesktopModules.Assigned )
                    {
                        objDesktopModules.AddPortalDesktopModule( intPortalId, int.Parse( objListItem.Value ) );
                    }
                }

                // Redirect to this site to refresh only if admin skin changed
                if( blnAdminSkinChanged )
                {
                    Response.Redirect( Request.RawUrl, true );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}