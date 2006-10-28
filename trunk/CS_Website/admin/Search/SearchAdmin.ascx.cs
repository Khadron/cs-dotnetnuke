using System;
using System.Collections;
using System.Diagnostics;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Search;

namespace DotNetNuke.Modules.Admin.Search
{
    public partial class SearchAdmin : PortalModuleBase
    {
        //tasks

        private Hashtable _settings;
        private Hashtable _defaultSettings;

        /// <summary>
        /// GetSetting gets a Search Setting from the Portal Modules Settings table (or
        /// from the Host Settings if not defined)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/16/2004	created
        /// </history>
        private string GetSetting( string txtName )
        {
            string settingValue = "";

            //Try Portal setting first
            if( _settings[txtName] == null == false )
            {
                settingValue = Convert.ToString( _settings[txtName] );
            }
            else
            {
                //Get Default setting
                if( _defaultSettings[txtName] == null == false )
                {
                    settingValue = Convert.ToString( _defaultSettings[txtName] );
                }
            }

            return settingValue;
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/16/2004 created
        ///     [cnurse]    01/10/2005 added UrlReferrer code so Cancel returns to previous page
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            if( ! Page.IsPostBack )
            {
                //Get Host Settings (used as default)
                _defaultSettings = Globals.HostSettings;

                //Get Search Settings (HostSettings if on Host Tab, Module Settings Otherwise)
//                ModuleController objModules = new ModuleController();
                if( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId )
                {
                    _settings = Globals.HostSettings;
                }
                else
                {
                    _settings = PortalSettings.GetModuleSettings( ModuleId );
                }

                txtMaxWordLength.Text = GetSetting( "MaxSearchWordLength" );
                txtMinWordLength.Text = GetSetting( "MinSearchWordLength" );
                if( GetSetting( "SearchIncludeCommon" ) == "Y" )
                {
                    chkIncludeCommon.Checked = true;
                }
                if( GetSetting( "SearchIncludeNumeric" ) == "Y" )
                {
                    chkIncludeNumeric.Checked = true;
                }

                // Store URL Referrer to return to portal
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

            if( Convert.ToString( ViewState["UrlReferrer"] ) == "" )
            {
                cmdCancel.Visible = false;
            }
            else
            {
                cmdCancel.Visible = true;
            }
        }

        /// <summary>
        /// cmdCancel_Click runs when the Cancel LinkButton is clicked.  It returns the user
        /// to the referring page
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/16/2004 created
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
        /// cmdReIndex_Click runs when the ReIndex LinkButton is clicked.  It re-indexes the
        /// site (or application if run on Host page)
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/16/2004 created
        /// </history>
        protected void cmdReIndex_Click( object sender, EventArgs e )
        {
            try
            {
                SearchEngine se = new SearchEngine();
                if( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId )
                {
                    se.IndexContent();
                }
                else
                {
                    se.IndexContent( PortalId );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update LinkButton is clicked.
        /// It saves the current Search Settings
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/9/2004	Modified
        /// </history>
        protected void cmdUpdate_Click( object sender, EventArgs e )
        {
            try
            {
                if( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId )
                {
                    HostSettingsController objHostSettings = new HostSettingsController();
                    objHostSettings.UpdateHostSetting( "MaxSearchWordLength", txtMaxWordLength.Text );
                    objHostSettings.UpdateHostSetting( "MinSearchWordLength", txtMinWordLength.Text );
                    objHostSettings.UpdateHostSetting( "SearchIncludeCommon", Convert.ToString( chkIncludeCommon.Checked ? "Y" : "N" ) );
                    objHostSettings.UpdateHostSetting( "SearchIncludeNumeric", Convert.ToString( chkIncludeNumeric.Checked ? "Y" : "N" ) );

                    // clear host settings cache
                    DataCache.ClearHostCache( false );
                }
                else
                {
                    ModuleController objModules = new ModuleController();
                    objModules.UpdateModuleSetting( ModuleId, "MaxSearchWordLength", txtMaxWordLength.Text );
                    objModules.UpdateModuleSetting( ModuleId, "MinSearchWordLength", txtMinWordLength.Text );
                    objModules.UpdateModuleSetting( ModuleId, "SearchIncludeCommon", Convert.ToString( chkIncludeCommon.Checked ? "Y" : "N" ) );
                    objModules.UpdateModuleSetting( ModuleId, "SearchIncludeNumeric", Convert.ToString( chkIncludeNumeric.Checked ? "Y" : "N" ) );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
    }
}