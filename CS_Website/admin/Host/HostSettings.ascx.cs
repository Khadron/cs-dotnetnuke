using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.UserControls;

namespace DotNetNuke.Modules.Admin.Host
{
    /// <summary>
    /// The HostSettingsModule PortalModuleBase is used to edit the host settings
    /// for the application.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class HostSettingsModule : PortalModuleBase
    {
        //Basic Settings Section

        //Configuration Section
        protected LabelControl plFrameowrk;

        //Host Section

        //Appearance Section

        //Payment Section

        //Advanced Settings Section

        //Proxy Section

        //SMTP Section

        //Other Section

        /// <summary>
        /// BindData fetches the data from the database and updates the controls
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void BindData()
        {
            lblVersion.Text = Globals.glbAppVersion;
            switch( Convert.ToString( Globals.HostSettings["CheckUpgrade"] ) )
            {
                case "":
                    chkUpgrade.Checked = true;
                    break;

                case "Y":

                    chkUpgrade.Checked = true;
                    break;
                case "N":

                    chkUpgrade.Checked = false;
                    break;
            }
            if( chkUpgrade.Checked )
            {
                hypUpgrade.ImageUrl = Globals.glbAppUrl + "/upgrade.aspx?version=" + lblVersion.Text.Replace( ".", "" );
                hypUpgrade.NavigateUrl = Globals.glbAppUrl + "/tabid/125/default.aspx";
            }
            else
            {
                hypUpgrade.Visible = false;
            }
            lblDataProvider.Text = ProviderConfiguration.GetProviderConfiguration( "data" ).DefaultProvider;
            lblFramework.Text = Environment.Version.ToString();
            lblIdentity.Text = WindowsIdentity.GetCurrent().Name;
            lblHostName.Text = Dns.GetHostName();

            PortalController objPortals = new PortalController();
            cboHostPortal.DataSource = objPortals.GetPortals();
            cboHostPortal.DataBind();
            if( Convert.ToString( Globals.HostSettings["HostPortalId"] ) != "" )
            {
                if( cboHostPortal.Items.FindByValue( Convert.ToString( Globals.HostSettings["HostPortalId"] ) ) != null )
                {
                    cboHostPortal.Items.FindByValue( Convert.ToString( Globals.HostSettings["HostPortalId"] ) ).Selected = true;
                }
            }
            txtHostTitle.Text = Convert.ToString( Globals.HostSettings["HostTitle"] );
            txtHostURL.Text = Convert.ToString( Globals.HostSettings["HostURL"] );
            txtHostEmail.Text = Convert.ToString( Globals.HostSettings["HostEmail"] );

            //SkinController objSkins = new SkinController();
            SkinInfo objSkin;

            ctlHostSkin.Width = "252px";
            ctlHostSkin.SkinRoot = SkinInfo.RootSkin;
            objSkin = SkinController.GetSkin( SkinInfo.RootSkin, Null.NullInteger, SkinType.Portal );
            if( objSkin != null )
            {
                if( Null.IsNull( objSkin.PortalId )  )
                {
                    ctlHostSkin.SkinSrc = objSkin.SkinSrc;
                }
            }
            ctlHostContainer.Width = "252px";
            ctlHostContainer.SkinRoot = SkinInfo.RootContainer;
            objSkin = SkinController.GetSkin( SkinInfo.RootContainer, Null.NullInteger, SkinType.Portal );
            if( objSkin != null )
            {
                if( Null.IsNull( objSkin.PortalId )  )
                {
                    ctlHostContainer.SkinSrc = objSkin.SkinSrc;
                }
            }

            ctlAdminSkin.Width = "252px";
            ctlAdminSkin.SkinRoot = SkinInfo.RootSkin;
            objSkin = SkinController.GetSkin( SkinInfo.RootSkin, Null.NullInteger, SkinType.Admin );
            if( objSkin != null )
            {
                if( Null.IsNull( objSkin.PortalId )  )
                {
                    ctlAdminSkin.SkinSrc = objSkin.SkinSrc;
                }
            }
            ctlAdminContainer.Width = "252px";
            ctlAdminContainer.SkinRoot = SkinInfo.RootContainer;
            objSkin = SkinController.GetSkin( SkinInfo.RootContainer, Null.NullInteger, SkinType.Admin );
            if( objSkin != null )
            {
                if( Null.IsNull( objSkin.PortalId )  )
                {
                    ctlAdminContainer.SkinSrc = objSkin.SkinSrc;
                }
            }
            ModuleControlController objModuleControls = new ModuleControlController();
            ArrayList arrModuleControls = objModuleControls.GetModuleControls( Null.NullInteger );
            ModuleControlInfo objModuleControl;
            int intModuleControl;
            for( intModuleControl = 0; intModuleControl <= arrModuleControls.Count - 1; intModuleControl++ )
            {
                objModuleControl = (ModuleControlInfo)arrModuleControls[intModuleControl];
                if( objModuleControl.ControlType == SecurityAccessLevel.ControlPanel )
                {
                    cboControlPanel.Items.Add( new ListItem( objModuleControl.ControlKey.Replace( "CONTROLPANEL:", "" ), objModuleControl.ControlSrc ) );
                }
            }
            if( Convert.ToString( Globals.HostSettings["ControlPanel"] ) != "" )
            {
                if( cboControlPanel.Items.FindByValue( Convert.ToString( Globals.HostSettings["ControlPanel"] ) ) != null )
                {
                    cboControlPanel.Items.FindByValue( Convert.ToString( Globals.HostSettings["ControlPanel"] ) ).Selected = true;
                }
            }
            else
            {
                if( cboControlPanel.Items.FindByValue( Globals.glbDefaultControlPanel ) != null )
                {
                    cboControlPanel.Items.FindByValue( Globals.glbDefaultControlPanel ).Selected = true;
                }
            }

            ListController ctlList = new ListController();
            ListEntryInfoCollection colProcessor = ctlList.GetListEntryInfoCollection( "Processor", "" );

            cboProcessor.DataSource = colProcessor;
            cboProcessor.DataBind();
            cboProcessor.Items.Insert( 0, new ListItem( "<" + Localization.GetString( "None_Specified" ) + ">", "" ) );

            if( cboProcessor.Items.FindByText( Globals.HostSettings["PaymentProcessor"].ToString() ) != null )
            {
                cboProcessor.Items.FindByText( Globals.HostSettings["PaymentProcessor"].ToString() ).Selected = true;
            }
            txtUserId.Text = Convert.ToString( Globals.HostSettings["ProcessorUserId"] );
            txtPassword.Attributes.Add( "value", Convert.ToString( Globals.HostSettings["ProcessorPassword"] ) );

            txtHostFee.Text = Convert.ToString( Globals.HostSettings["HostFee"] );

            ListEntryInfoCollection colCurrency = ctlList.GetListEntryInfoCollection( "Currency", "" );

            cboHostCurrency.DataSource = colCurrency;
            cboHostCurrency.DataBind();
            if( cboHostCurrency.Items.FindByValue( Convert.ToString( Globals.HostSettings["HostCurrency"] ) ) != null )
            {
                cboHostCurrency.Items.FindByValue( Globals.HostSettings["HostCurrency"].ToString() ).Selected = true;
            }
            else
            {
                cboHostCurrency.Items.FindByValue( "USD" ).Selected = true;
            }
            if( cboPerformance.Items.FindByValue( Convert.ToString( Globals.HostSettings["PerformanceSetting"] ) ) != null )
            {
                cboPerformance.Items.FindByValue( Globals.HostSettings["PerformanceSetting"].ToString() ).Selected = true;
            }
            else
            {
                cboPerformance.Items.FindByValue( "3" ).Selected = true;
            }
            if( cboCacheability.Items.FindByValue( Convert.ToString( Globals.HostSettings["AuthenticatedCacheability"] ) ) != null )
            {
                cboCacheability.Items.FindByValue( Globals.HostSettings["AuthenticatedCacheability"].ToString() ).Selected = true;
            }
            else
            {
                cboCacheability.Items.FindByValue( "4" ).Selected = true;
            }
            if( cboSchedulerMode.Items.FindByValue( Convert.ToString( Globals.HostSettings["SchedulerMode"] ) ) != null )
            {
                cboSchedulerMode.Items.FindByValue( Globals.HostSettings["SchedulerMode"].ToString() ).Selected = true;
            }
            else
            {
                cboSchedulerMode.Items.FindByValue( "1" ).Selected = true;
            }

            txtHostSpace.Text = Convert.ToString( Globals.HostSettings["HostSpace"] );
            if( Convert.ToString( Globals.HostSettings["SiteLogStorage"] ) == "" )
            {
                optSiteLogStorage.Items.FindByValue( "D" ).Selected = true;
            }
            else
            {
                optSiteLogStorage.Items.FindByValue( Convert.ToString( Globals.HostSettings["SiteLogStorage"] ) ).Selected = true;
            }
            if( Convert.ToString( Globals.HostSettings["SiteLogBuffer"] ) == "" )
            {
                txtSiteLogBuffer.Text = "1";
            }
            else
            {
                txtSiteLogBuffer.Text = Convert.ToString( Globals.HostSettings["SiteLogBuffer"] );
            }
            txtSiteLogHistory.Text = Convert.ToString( Globals.HostSettings["SiteLogHistory"] );

            if( Convert.ToString( Globals.HostSettings["ModuleCaching"] ) == "" )
            {
                cboCacheMethod.Items.FindByValue( "M" ).Selected = true;
            }
            else
            {
                cboCacheMethod.Items.FindByValue( Convert.ToString( Globals.HostSettings["ModuleCaching"] ) ).Selected = true;
            }

            txtDemoPeriod.Text = Convert.ToString( Globals.HostSettings["DemoPeriod"] );
            if( Convert.ToString( Globals.HostSettings["DemoSignup"] ) == "Y" )
            {
                chkDemoSignup.Checked = true;
            }
            else
            {
                chkDemoSignup.Checked = false;
            }
            if( Globals.GetHashValue( Globals.HostSettings["Copyright"], "Y" ) == "Y" )
            {
                chkCopyright.Checked = true;
            }
            else
            {
                chkCopyright.Checked = false;
            }
            if( Globals.HostSettings.ContainsKey( "DisableUsersOnline" ) )
            {
                if( Globals.HostSettings["DisableUsersOnline"].ToString() == "Y" )
                {
                    chkUsersOnline.Checked = true;
                }
                else
                {
                    chkUsersOnline.Checked = false;
                }
            }
            else
            {
                chkUsersOnline.Checked = false;
            }
            txtUsersOnlineTime.Text = Convert.ToString( Globals.HostSettings["UsersOnlineTime"] );
            txtAutoAccountUnlock.Text = Convert.ToString( Globals.HostSettings["AutoAccountUnlockDuration"] );
            txtProxyServer.Text = Convert.ToString( Globals.HostSettings["ProxyServer"] );
            txtProxyPort.Text = Convert.ToString( Globals.HostSettings["ProxyPort"] );
            txtProxyUsername.Text = Convert.ToString( Globals.HostSettings["ProxyUsername"] );
            txtProxyPassword.Attributes.Add( "value", Convert.ToString( Globals.HostSettings["ProxyPassword"] ) );
            txtSMTPServer.Text = Convert.ToString( Globals.HostSettings["SMTPServer"] );
            if( Convert.ToString( Globals.HostSettings["SMTPAuthentication"] ) != "" )
            {
                optSMTPAuthentication.Items.FindByValue( Globals.HostSettings["SMTPAuthentication"].ToString() ).Selected = true;
            }
            else
            {
                optSMTPAuthentication.Items.FindByValue( "0" ).Selected = true;
            }

            txtSMTPUsername.Text = Convert.ToString( Globals.HostSettings["SMTPUsername"] );
            txtSMTPPassword.Attributes.Add( "value", Convert.ToString( Globals.HostSettings["SMTPPassword"] ) );
            txtFileExtensions.Text = Convert.ToString( Globals.HostSettings["FileExtensions"] );

            if( Globals.HostSettings.ContainsKey( "UseCustomErrorMessages" ) )
            {
                if( Globals.HostSettings["UseCustomErrorMessages"].ToString() == "Y" )
                {
                    chkUseCustomErrorMessages.Checked = true;
                }
                else
                {
                    chkUseCustomErrorMessages.Checked = false;
                }
            }
            else
            {
                chkUseCustomErrorMessages.Checked = false;
            }

            if( Globals.HostSettings.ContainsKey( "UseFriendlyUrls" ) )
            {
                if( Globals.HostSettings["UseFriendlyUrls"].ToString() == "Y" )
                {
                    chkUseFriendlyUrls.Checked = true;
                }
                else
                {
                    chkUseFriendlyUrls.Checked = false;
                }
            }
            else
            {
                chkUseFriendlyUrls.Checked = false;
            }
            rowFriendlyUrls.Visible = chkUseFriendlyUrls.Checked;

            if( Globals.HostSettings.ContainsKey( "EventLogBuffer" ) )
            {
                if( Globals.HostSettings["EventLogBuffer"].ToString() == "Y" )
                {
                    chkLogBuffer.Checked = true;
                }
                else
                {
                    chkLogBuffer.Checked = false;
                }
            }
            else
            {
                chkLogBuffer.Checked = false;
            }

            if( Convert.ToString( Globals.HostSettings["SkinUpload"] ) != "" )
            {
                optSkinUpload.Items.FindByValue( Globals.HostSettings["SkinUpload"].ToString() ).Selected = true;
            }
            else
            {
                optSkinUpload.Items.FindByValue( "G" ).Selected = true;
            }

            txtHelpURL.Text = Convert.ToString( Globals.HostSettings["HelpURL"] );
            if( Globals.HostSettings.ContainsKey( "EnableModuleOnLineHelp" ) )
            {
                if( Globals.HostSettings["EnableModuleOnLineHelp"].ToString() == "Y" )
                {
                    chkEnableHelp.Checked = true;
                }
                else
                {
                    chkEnableHelp.Checked = false;
                }
            }
            else
            {
                chkEnableHelp.Checked = true;
            }

            ViewState["SelectedSchedulerMode"] = cboSchedulerMode.SelectedItem.Value;
            ViewState["SelectedLogBufferEnabled"] = chkLogBuffer.Checked;
            ViewState["SelectedUsersOnlineEnabled"] = chkUsersOnline.Checked;

            // Get the name of the data provider
            ProviderConfiguration objProviderConfiguration = ProviderConfiguration.GetProviderConfiguration( "data" );

            // get list of script files
            string strProviderPath = PortalSettings.GetProviderPath();
            ArrayList arrScriptFiles = new ArrayList();
            string strFile;
            string[] arrFiles = Directory.GetFiles( strProviderPath, "*." + objProviderConfiguration.DefaultProvider );
            foreach( string tempLoopVar_strFile in arrFiles )
            {
                strFile = tempLoopVar_strFile;
                arrScriptFiles.Add( Path.GetFileNameWithoutExtension( strFile ) );
            }
            arrScriptFiles.Sort();

            cboUpgrade.DataSource = arrScriptFiles;
            cboUpgrade.DataBind();

            ModuleInfo FileManagerModule = ( new ModuleController() ).GetModuleByDefinition( Null.NullInteger, "File Manager" );

            string[] @params = new string[3];

            @params[0] = "mid=" + FileManagerModule.ModuleID;
            @params[1] = "ftype=" + UploadType.Skin.ToString();
            @params[2] = "rtab=" + this.TabId;
            lnkUploadSkin.NavigateUrl = Globals.NavigateURL( FileManagerModule.TabID, "Edit", @params );

            @params[1] = "ftype=" + UploadType.Container.ToString();
            lnkUploadContainer.NavigateUrl = Globals.NavigateURL( FileManagerModule.TabID, "Edit", @params );
        }

        private bool SkinChanged( string SkinRoot, int portalId, SkinType SkinType, string PostedSkinSrc )
        {
            //SkinController objSkins = new SkinController();
            SkinInfo objSkinInfo;
            string strSkinSrc = Null.NullString;
            objSkinInfo = SkinController.GetSkin( SkinRoot, portalId, SkinType.Admin );
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

        protected void Page_Init( Object sender, EventArgs e )
        {
        }

        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        ///     [VMasanas]  9/28/2004   Changed redirect to Access Denied
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                // Verify that the current user has access to access this page
                if( ! UserInfo.IsSuperUser )
                {
                    Response.Redirect( Globals.NavigateURL( "Access Denied" ), true );
                }

                // If this is the first visit to the page, populate the site data
                if( Page.IsPostBack == false )
                {
                    BindData();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// chkUseFriendlyUrls_CheckedChanged runs when the use friendly urls checkbox's
        /// value is changed.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	07/06/2006 Created
        /// </history>
        protected void chkUseFriendlyUrls_CheckedChanged( object sender, EventArgs e )
        {
            rowFriendlyUrls.Visible = chkUseFriendlyUrls.Checked;
        }

        /// <summary>
        /// cmdEmail_Click runs when the test email button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdEmail_Click( object sender, EventArgs e )
        {
            try
            {
                if( txtHostEmail.Text != "" )
                {
                    string strMessage = Mail.SendMail( txtHostEmail.Text, txtHostEmail.Text, "", Localization.GetSystemMessage( PortalSettings, "EMAIL_SMTP_TEST_SUBJECT" ), "", "", "", txtSMTPServer.Text, optSMTPAuthentication.SelectedItem.Value, txtSMTPUsername.Text, txtSMTPPassword.Text );

                    if( strMessage != "" )
                    {
                        lblEmail.Text = "<br>" + strMessage;
                    }
                    else
                    {
                        lblEmail.Text = "<br>" + Localization.GetString( "EmailSentMessage", this.LocalResourceFile );
                    }
                }
                else
                {
                    lblEmail.Text = "<br>" + Localization.GetString( "SpecifyHostEmailMessage", this.LocalResourceFile );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdProcessor_Click runs when the processor Go button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
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
        /// cmdEmail_Click runs when the clear cache button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdCache_Click( object sender, EventArgs e )
        {
            // clear entire cache
            DataCache.ClearHostCache( true );

            Response.Redirect( Request.RawUrl, true );
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Upgrade button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdUpdate_Click( object sender, EventArgs e )
        {
            try
            {
                HostSettingsController objHostSettings = new HostSettingsController();

                objHostSettings.UpdateHostSetting( "CheckUpgrade", Convert.ToString( chkUpgrade.Checked ? "Y" : "N" ) );
                objHostSettings.UpdateHostSetting( "HostPortalId", cboHostPortal.SelectedItem.Value );
                objHostSettings.UpdateHostSetting( "HostTitle", txtHostTitle.Text );
                objHostSettings.UpdateHostSetting( "HostURL", txtHostURL.Text );
                objHostSettings.UpdateHostSetting( "HostEmail", txtHostEmail.Text );
                objHostSettings.UpdateHostSetting( "PaymentProcessor", cboProcessor.SelectedItem.Text );
                objHostSettings.UpdateHostSetting( "ProcessorUserId", txtUserId.Text, true );
                objHostSettings.UpdateHostSetting( "ProcessorPassword", txtPassword.Text, true );
                objHostSettings.UpdateHostSetting( "HostFee", txtHostFee.Text );
                objHostSettings.UpdateHostSetting( "HostCurrency", cboHostCurrency.SelectedItem.Value );
                objHostSettings.UpdateHostSetting( "HostSpace", txtHostSpace.Text );
                objHostSettings.UpdateHostSetting( "SiteLogStorage", optSiteLogStorage.SelectedItem.Value );
                objHostSettings.UpdateHostSetting( "SiteLogBuffer", txtSiteLogBuffer.Text );
                objHostSettings.UpdateHostSetting( "SiteLogHistory", txtSiteLogHistory.Text );
                objHostSettings.UpdateHostSetting( "DemoPeriod", txtDemoPeriod.Text );
                objHostSettings.UpdateHostSetting( "DemoSignup", Convert.ToString( chkDemoSignup.Checked ? "Y" : "N" ) );
                objHostSettings.UpdateHostSetting( "Copyright", Convert.ToString( chkCopyright.Checked ? "Y" : "N" ) );

                bool OriginalUsersOnline;
                OriginalUsersOnline = Convert.ToBoolean( ViewState["SelectedUsersOnlineEnabled"] );
                if( OriginalUsersOnline != chkUsersOnline.Checked )
                {
                    ScheduleItem objScheduleItem;
                    objScheduleItem = SchedulingProvider.Instance().GetSchedule( "DotNetNuke.Entities.Users.PurgeUsersOnline, DOTNETNUKE", Null.NullString );
                    if( objScheduleItem != null )
                    {
                        if( ! chkUsersOnline.Checked )
                        {
                            if( ! objScheduleItem.Enabled )
                            {
                                objScheduleItem.Enabled = true;
                                SchedulingProvider.Instance().UpdateSchedule( objScheduleItem );
                                SchedulerMode mode = (SchedulerMode)Enum.Parse( typeof( SchedulerMode ), cboSchedulerMode.SelectedItem.Value );
                                if (mode == SchedulerMode.TIMER_METHOD)
                                {
                                    SchedulingProvider.Instance().ReStart( "Host Settings" );
                                }
                            }
                        }
                        else
                        {
                            if( objScheduleItem.Enabled )
                            {
                                objScheduleItem.Enabled = false;
                                SchedulingProvider.Instance().UpdateSchedule( objScheduleItem );
                                SchedulerMode mode = (SchedulerMode)Enum.Parse(typeof(SchedulerMode), cboSchedulerMode.SelectedItem.Value);
                                if( mode == SchedulerMode.TIMER_METHOD )
                                {
                                    SchedulingProvider.Instance().ReStart( "Host Settings" );
                                }
                            }
                        }
                    }
                }
                objHostSettings.UpdateHostSetting( "DisableUsersOnline", Convert.ToString( chkUsersOnline.Checked ? "Y" : "N" ) );

                objHostSettings.UpdateHostSetting( "AutoAccountUnlockDuration", txtAutoAccountUnlock.Text );
                objHostSettings.UpdateHostSetting( "UsersOnlineTime", txtUsersOnlineTime.Text );
                objHostSettings.UpdateHostSetting( "ProxyServer", txtProxyServer.Text );
                objHostSettings.UpdateHostSetting( "ProxyPort", txtProxyPort.Text );
                objHostSettings.UpdateHostSetting( "ProxyUsername", txtProxyUsername.Text, true );
                objHostSettings.UpdateHostSetting( "ProxyPassword", txtProxyPassword.Text, true );
                objHostSettings.UpdateHostSetting( "SMTPServer", txtSMTPServer.Text );
                objHostSettings.UpdateHostSetting( "SMTPAuthentication", optSMTPAuthentication.SelectedItem.Value );
                objHostSettings.UpdateHostSetting( "SMTPUsername", txtSMTPUsername.Text, true );
                objHostSettings.UpdateHostSetting( "SMTPPassword", txtSMTPPassword.Text, true );
                objHostSettings.UpdateHostSetting( "FileExtensions", txtFileExtensions.Text );
                objHostSettings.UpdateHostSetting( "SkinUpload", optSkinUpload.SelectedItem.Value );
                objHostSettings.UpdateHostSetting( "PerformanceSetting", cboPerformance.SelectedItem.Value );
                objHostSettings.UpdateHostSetting( "AuthenticatedCacheability", cboCacheability.SelectedItem.Value );
                objHostSettings.UpdateHostSetting( "UseCustomErrorMessages", Convert.ToString( chkUseCustomErrorMessages.Checked ? "Y" : "N" ) );
                objHostSettings.UpdateHostSetting( "UseFriendlyUrls", Convert.ToString( chkUseFriendlyUrls.Checked ? "Y" : "N" ) );
                objHostSettings.UpdateHostSetting( "ControlPanel", cboControlPanel.SelectedItem.Value );
                objHostSettings.UpdateHostSetting( "SchedulerMode", cboSchedulerMode.SelectedItem.Value );
                objHostSettings.UpdateHostSetting( "ModuleCaching", cboCacheMethod.SelectedItem.Value );
                objHostSettings.UpdateHostSetting( "EnableModuleOnLineHelp", Convert.ToString( chkEnableHelp.Checked ? "Y" : "N" ) );
                objHostSettings.UpdateHostSetting( "HelpURL", txtHelpURL.Text );

                bool OriginalLogBuffer;
                OriginalLogBuffer = Convert.ToBoolean( ViewState["SelectedLogBufferEnabled"] );
                if( OriginalLogBuffer != chkLogBuffer.Checked )
                {
                    ScheduleItem objScheduleItem;
                    objScheduleItem = SchedulingProvider.Instance().GetSchedule( "DotNetNuke.Services.Log.EventLog.PurgeLogBuffer, DOTNETNUKE", Null.NullString );
                    if( objScheduleItem != null )
                    {
                        if( chkLogBuffer.Checked )
                        {
                            if( ! objScheduleItem.Enabled )
                            {
                                objScheduleItem.Enabled = true;
                                SchedulingProvider.Instance().UpdateSchedule( objScheduleItem );
                                SchedulerMode mode = (SchedulerMode)Enum.Parse(typeof(SchedulerMode), cboSchedulerMode.SelectedItem.Value);
                                if( mode == SchedulerMode.TIMER_METHOD )
                                {
                                    SchedulingProvider.Instance().ReStart( "Host Settings" );
                                }
                            }
                        }
                        else
                        {
                            if( objScheduleItem.Enabled )
                            {
                                objScheduleItem.Enabled = false;
                                SchedulingProvider.Instance().UpdateSchedule( objScheduleItem );
                                SchedulerMode mode = (SchedulerMode)Enum.Parse(typeof(SchedulerMode), cboSchedulerMode.SelectedItem.Value);
                                if( mode == SchedulerMode.TIMER_METHOD )
                                {
                                    SchedulingProvider.Instance().ReStart( "Host Settings" );
                                }
                            }
                        }
                    }
                }
                objHostSettings.UpdateHostSetting( "EventLogBuffer", Convert.ToString( chkLogBuffer.Checked ? "Y" : "N" ) );

                //SkinController objSkins = new SkinController();
                //bool blnAdminSkinChanged = SkinChanged( SkinInfo.RootSkin, Null.NullInteger, SkinType.Admin, ctlAdminSkin.SkinSrc ) || SkinChanged( SkinInfo.RootContainer, Null.NullInteger, SkinType.Admin, ctlAdminContainer.SkinSrc );
                SkinController.SetSkin( SkinInfo.RootSkin, Null.NullInteger, SkinType.Portal, ctlHostSkin.SkinSrc );
                SkinController.SetSkin( SkinInfo.RootContainer, Null.NullInteger, SkinType.Portal, ctlHostContainer.SkinSrc );
                SkinController.SetSkin( SkinInfo.RootSkin, Null.NullInteger, SkinType.Admin, ctlAdminSkin.SkinSrc );
                SkinController.SetSkin( SkinInfo.RootContainer, Null.NullInteger, SkinType.Admin, ctlAdminContainer.SkinSrc );

                // clear host settings cache
                DataCache.ClearHostCache( true );

                SchedulerMode OriginalSchedulerMode;
                OriginalSchedulerMode = (SchedulerMode)ViewState["SelectedSchedulerMode"];
                SchedulerMode smode = (SchedulerMode)Enum.Parse(typeof(SchedulerMode), cboSchedulerMode.SelectedItem.Value);
                
                if( smode == SchedulerMode.DISABLED )
                {
                    if( OriginalSchedulerMode != SchedulerMode.DISABLED )
                    {
                        SchedulingProvider.Instance().Halt( "Host Settings" );
                    }
                }
                else if( smode == SchedulerMode.TIMER_METHOD )
                {
                    if( OriginalSchedulerMode == SchedulerMode.DISABLED || OriginalSchedulerMode == SchedulerMode.REQUEST_METHOD )
                    {
                        Thread newThread = new Thread( new ThreadStart( SchedulingProvider.Instance().Start ) );
                        newThread.IsBackground = true;
                        newThread.Start();
                    }
                }
                else if( smode != SchedulerMode.TIMER_METHOD )
                {
                    if( OriginalSchedulerMode == SchedulerMode.TIMER_METHOD )
                    {
                        SchedulingProvider.Instance().Halt( "Host Settings" );
                    }
                }

                // this is needed in order to fully flush the cache after changing FriendlyURL
                Response.Redirect( Request.RawUrl, true );

                // Redirect to this site to refresh only if admin skin changed
                //If blnAdminSkinChanged Then Response.Redirect(Request.RawUrl, True)
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdUpgrade_Click runs when the Upgrade Log Go button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdUpgrade_Click( object sender, EventArgs e )
        {
            try
            {
                // get path to provider
                string strProviderPath = PortalSettings.GetProviderPath();

                if( File.Exists( strProviderPath + cboUpgrade.SelectedItem.Text + ".log" ) )
                {
                    StreamReader objStreamReader;
                    objStreamReader = File.OpenText( strProviderPath + cboUpgrade.SelectedItem.Text + ".log" );
                    lblUpgrade.Text = objStreamReader.ReadToEnd().Replace( "\n", "<br>" );
                    objStreamReader.Close();
                }
                else
                {
                    lblUpgrade.Text = Localization.GetString( "ScriptFailure", this.LocalResourceFile );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}