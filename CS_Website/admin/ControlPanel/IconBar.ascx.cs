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
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Utilities;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.UI.ControlPanels
{
    /// <summary>
    /// The IconBar ControlPanel provides an icon bar based Page/Module manager
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class IconBar : ControlPanelBase
    {
        //Protected lblAddModule As System.Web.UI.WebControls.Label

        protected CheckBox chkVisible;

        private enum ViewPermissionType
        {
            View = 0,
            Edit = 1
        }

        private string BuildURL( int PortalID, string FriendlyName )
        {
            string strURL = "~/" + Globals.glbDefaultPage;

            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModuleByDefinition( PortalID, FriendlyName );
            if( objModule != null )
            {
                strURL = Globals.NavigateURL( objModule.TabID );
            }

            return strURL;
        }

        private void BindData()
        {
            switch( optModuleType.SelectedItem.Value )
            {
                case "0": // new module

                    cboTabs.Visible = false;
                    cboModules.Visible = false;
                    cboDesktopModules.Visible = true;
                    txtTitle.Visible = true;
                    lblModule.Text = Localization.GetString( "Module", this.LocalResourceFile );
                    lblTitle.Text = Localization.GetString( "Title", this.LocalResourceFile );
                    cboPermission.Enabled = true;

                    DesktopModuleController objDesktopModules = new DesktopModuleController();
                    cboDesktopModules.DataSource = objDesktopModules.GetDesktopModulesByPortal( PortalSettings.PortalId );
                    cboDesktopModules.DataBind();
                    cboDesktopModules.Items.Insert( 0, new ListItem( "<" + Localization.GetString( "SelectModule", this.LocalResourceFile ) + ">", "-1" ) );
                    break;
                case "1": // existing module

                    cboTabs.Visible = true;
                    cboModules.Visible = true;
                    cboDesktopModules.Visible = false;
                    txtTitle.Visible = false;
                    lblModule.Text = Localization.GetString( "Tab", this.LocalResourceFile );
                    lblTitle.Text = Localization.GetString( "Module", this.LocalResourceFile );
                    cboPermission.Enabled = false;

                    ArrayList arrTabs = new ArrayList();

                    TabInfo objTab;
                    ArrayList arrPortalTabs = Globals.GetPortalTabs( PortalSettings.DesktopTabs, true, true );
                    foreach( TabInfo tempLoopVar_objTab in arrPortalTabs )
                    {
                        objTab = tempLoopVar_objTab;
                        if( objTab.TabID == - 1 )
                        {
                            // <none specified>
                            objTab.TabName = "<" + Localization.GetString( "SelectPage", this.LocalResourceFile ) + ">";
                            arrTabs.Add( objTab );
                        }
                        else
                        {
                            if( objTab.TabID != PortalSettings.ActiveTab.TabID )
                            {
                                if( PortalSecurity.IsInRoles( objTab.AuthorizedRoles ) )
                                {
                                    arrTabs.Add( objTab );
                                }
                            }
                        }
                    }

                    cboTabs.DataSource = arrTabs;
                    cboTabs.DataBind();
                    break;
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ! Page.IsPostBack )
                {
                    // localization
                    lblPageFunctions.Text = Localization.GetString( "PageFunctions", this.LocalResourceFile );
                    optModuleType.Items.FindByValue( "0" ).Selected = true;
                    lblCommonTasks.Text = Localization.GetString( "CommonTasks", this.LocalResourceFile );
                    imgAddTabIcon.AlternateText = Localization.GetString( "AddTab.AlternateText", this.LocalResourceFile );
                    cmdAddTab.Text = Localization.GetString( "AddTab", this.LocalResourceFile );
                    imgEditTabIcon.AlternateText = Localization.GetString( "EditTab.AlternateText", this.LocalResourceFile );
                    cmdEditTab.Text = Localization.GetString( "EditTab", this.LocalResourceFile );
                    imgDeleteTabIcon.AlternateText = Localization.GetString( "DeleteTab.AlternateText", this.LocalResourceFile );
                    cmdDeleteTab.Text = Localization.GetString( "DeleteTab", this.LocalResourceFile );
                    imgCopyTabIcon.AlternateText = Localization.GetString( "CopyTab.AlternateText", this.LocalResourceFile );
                    cmdCopyTab.Text = Localization.GetString( "CopyTab", this.LocalResourceFile );
                    imgPreviewTabIcon.AlternateText = Localization.GetString( "PreviewTab.AlternateText", this.LocalResourceFile );
                    cmdPreviewTab.Text = Localization.GetString( "PreviewTab", this.LocalResourceFile );
                    if( Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId.ToString()] != null )
                    {
                        HttpCookie objPreview;
                        objPreview = Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId.ToString()];
                        if( objPreview.Value == "True" )
                        {
                            imgPreviewTabIcon.ImageUrl = "~/Admin/ControlPanel/images/iconbar_previewtab_on.gif";
                        }
                    }
                    lblModule.Text = Localization.GetString( "Module", this.LocalResourceFile );
                    lblPane.Text = Localization.GetString( "Pane", this.LocalResourceFile );
                    lblTitle.Text = Localization.GetString( "Title", this.LocalResourceFile );
                    lblAlign.Text = Localization.GetString( "Align", this.LocalResourceFile );
                    imgAddModuleIcon.AlternateText = Localization.GetString( "AddModule.AlternateText", this.LocalResourceFile );
                    cmdAddModule.Text = Localization.GetString( "AddModule", this.LocalResourceFile );
                    imgWizardIcon.AlternateText = Localization.GetString( "Wizard.AlternateText", this.LocalResourceFile );
                    cmdWizard.Text = Localization.GetString( "Wizard", this.LocalResourceFile );
                    imgSiteIcon.AlternateText = Localization.GetString( "Site.AlternateText", this.LocalResourceFile );
                    cmdSite.Text = Localization.GetString( "Site", this.LocalResourceFile );
                    imgUsersIcon.AlternateText = Localization.GetString( "Users.AlternateText", this.LocalResourceFile );
                    cmdUsers.Text = Localization.GetString( "Users", this.LocalResourceFile );
                    imgFilesIcon.AlternateText = Localization.GetString( "Files.AlternateText", this.LocalResourceFile );
                    cmdFiles.Text = Localization.GetString( "Files", this.LocalResourceFile );
                    imgHelpIcon.AlternateText = Localization.GetString( "Help.AlternateText", this.LocalResourceFile );
                    cmdHelp.Text = Localization.GetString( "Help", this.LocalResourceFile );

                    if( PortalSettings.ActiveTab.IsAdminTab )
                    {
                        imgEditTabIcon.ImageUrl = "~/Admin/ControlPanel/images/iconbar_edittab_bw.gif";
                        cmdEditTab.Enabled = false;
                        cmdEditTabIcon.Enabled = false;
                        imgDeleteTabIcon.ImageUrl = "~/Admin/ControlPanel/images/iconbar_deletetab_bw.gif";
                        cmdDeleteTab.Enabled = false;
                        cmdDeleteTabIcon.Enabled = false;
                        imgCopyTabIcon.ImageUrl = "~/Admin/ControlPanel/images/iconbar_copytab_bw.gif";
                        cmdCopyTab.Enabled = false;
                        cmdCopyTabIcon.Enabled = false;
                    }
                    else
                    {
                        ClientAPI.AddButtonConfirm( cmdDeleteTab, Localization.GetString( "DeleteTabConfirm", this.LocalResourceFile ) );
                        ClientAPI.AddButtonConfirm( cmdDeleteTabIcon, Localization.GetString( "DeleteTabConfirm", this.LocalResourceFile ) );
                    }

                    if( Globals.IsAdminControl() )
                    {
                        cmdAddModule.Enabled = false;
                        imgAddModuleIcon.ImageUrl = "~/Admin/ControlPanel/images/iconbar_addmodule_bw.gif";
                        cmdAddModuleIcon.Enabled = false;
                    }

                    if( PortalSecurity.IsInRole( PortalSettings.AdministratorRoleName ) == false )
                    {
                        imgWizardIcon.ImageUrl = "~/Admin/ControlPanel/images/iconbar_wizard_bw.gif";
                        cmdWizard.Enabled = false;
                        cmdWizardIcon.Enabled = false;
                        imgSiteIcon.ImageUrl = "~/Admin/ControlPanel/images/iconbar_site_bw.gif";
                        cmdSite.Enabled = false;
                        cmdSiteIcon.Enabled = false;
                        imgUsersIcon.ImageUrl = "~/Admin/ControlPanel/images/iconbar_users_bw.gif";
                        cmdUsers.Enabled = false;
                        cmdUsersIcon.Enabled = false;
                        imgFilesIcon.ImageUrl = "~/Admin/ControlPanel/images/iconbar_files_bw.gif";
                        cmdFiles.Enabled = false;
                        cmdFilesIcon.Enabled = false;
                    }

                    BindData();

                    if( PortalSettings.ActiveTab.IsAdminTab == false && Globals.IsAdminControl() == false )
                    {
                        int intItem;
                        for( intItem = 0; intItem <= PortalSettings.ActiveTab.Panes.Count - 1; intItem++ )
                        {
                            cboPanes.Items.Add( Convert.ToString( PortalSettings.ActiveTab.Panes[intItem] ) );
                        }
                    }
                    else
                    {
                        cboPanes.Items.Add( Globals.glbDefaultPane );
                    }
                    if( cboPanes.Items.FindByValue( Globals.glbDefaultPane ) != null )
                    {
                        cboPanes.Items.FindByValue( Globals.glbDefaultPane ).Selected = true;
                    }

                    if( cboPermission.Items.Count > 0 )
                    {
                        cboPermission.SelectedIndex = 0; // view
                    }

                    if( cboAlign.Items.Count > 0 )
                    {
                        cboAlign.SelectedIndex = 0; // left
                    }

                    if( cboPosition.Items.Count > 0 )
                    {
                        cboPosition.SelectedIndex = 1; // bottom
                    }

                    if( Convert.ToString( PortalSettings.HostSettings["HelpURL"] ) != "" )
                    {
                        cmdHelp.NavigateUrl = Globals.FormatHelpUrl( Convert.ToString( PortalSettings.HostSettings["HelpURL"] ), PortalSettings, "" );
                        cmdHelpIcon.NavigateUrl = cmdHelp.NavigateUrl;
                        cmdHelp.Enabled = true;
                        cmdHelpIcon.Enabled = true;
                    }
                    else
                    {
                        cmdHelp.Enabled = false;
                        cmdHelpIcon.Enabled = false;
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// PageFunctions_Click runs when any button in the Page toolbar is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void PageFunctions_Click( object sender, EventArgs e )
        {
            try
            {
                string URL = Request.RawUrl;

                switch( ( (LinkButton)sender ).ID )
                {
                    case "cmdAddTab":
                        URL = Globals.NavigateURL( "Tab" );
                        break;

                    case "cmdAddTabIcon":

                        URL = Globals.NavigateURL( "Tab" );
                        break;
                    case "cmdEditTab":
                        URL = Globals.NavigateURL( PortalSettings.ActiveTab.TabID, "Tab", "action=edit" );
                        break;

                    case "cmdEditTabIcon":

                        URL = Globals.NavigateURL( PortalSettings.ActiveTab.TabID, "Tab", "action=edit" );
                        break;
                    case "cmdDeleteTab":
                        URL = Globals.NavigateURL( PortalSettings.ActiveTab.TabID, "Tab", "action=delete" );
                        break;

                    case "cmdDeleteTabIcon":

                        URL = Globals.NavigateURL( PortalSettings.ActiveTab.TabID, "Tab", "action=delete" );
                        break;
                    case "cmdCopyTab":
                        URL = Globals.NavigateURL( PortalSettings.ActiveTab.TabID, "Tab", "action=copy" );
                        break;

                    case "cmdCopyTabIcon":

                        URL = Globals.NavigateURL( PortalSettings.ActiveTab.TabID, "Tab", "action=copy" );
                        break;
                    case "cmdPreviewTab":
                        HttpCookie objPreview;

                        if( Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId.ToString()] == null )
                        {
                            objPreview = new HttpCookie( "_Tab_Admin_Preview" + PortalSettings.PortalId.ToString() );
                            objPreview.Value = "False";
                            Response.AppendCookie( objPreview );
                        }

                        objPreview = Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId.ToString()];
                        if( objPreview.Value == "True" )
                        {
                            objPreview.Value = "False";
                        }
                        else
                        {
                            objPreview.Value = "True";
                        }
                        Response.SetCookie( objPreview );
                        break;

                    case "cmdPreviewTabIcon":

                        if( Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId.ToString()] == null )
                        {
                            objPreview = new HttpCookie( "_Tab_Admin_Preview" + PortalSettings.PortalId.ToString() );
                            objPreview.Value = "False";
                            Response.AppendCookie( objPreview );
                        }

                        objPreview = Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId.ToString()];
                        if( objPreview.Value == "True" )
                        {
                            objPreview.Value = "False";
                        }
                        else
                        {
                            objPreview.Value = "True";
                        }
                        Response.SetCookie( objPreview );
                        break;
                }

                Response.Redirect( URL, true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// CommonTasks_Click runs when any button in the Common Tasks toolbar is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void CommonTasks_Click( object sender, EventArgs e )
        {
            try
            {
                string URL = Request.RawUrl;

                switch( ( (LinkButton)sender ).ID )
                {
                    case "cmdWizard":
                        URL = BuildURL( PortalSettings.PortalId, "Site Wizard" );
                        break;

                    case "cmdWizardIcon":

                        URL = BuildURL( PortalSettings.PortalId, "Site Wizard" );
                        break;
                    case "cmdSite":
                        URL = BuildURL( PortalSettings.PortalId, "Site Settings" );
                        break;

                    case "cmdSiteIcon":

                        URL = BuildURL( PortalSettings.PortalId, "Site Settings" );
                        break;
                    case "cmdUsers":
                        URL = BuildURL( PortalSettings.PortalId, "User Accounts" );
                        break;

                    case "cmdUsersIcon":

                        URL = BuildURL( PortalSettings.PortalId, "User Accounts" );
                        break;
                    case "cmdFiles":
                        URL = BuildURL( PortalSettings.PortalId, "File Manager" );
                        break;

                    case "cmdFilesIcon":

                        URL = BuildURL( PortalSettings.PortalId, "File Manager" );
                        break;
                }

                Response.Redirect( URL, true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// AddModule_Click runs when the Add Module Icon or text button is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        ///     [vmasanas]  01/07/2005  Modified to add view perm. to all roles with edit perm.
        /// </history>
        protected void AddModule_Click( object sender, EventArgs e )
        {
            try
            {
                TabPermissionCollection objTabPermissions;
                objTabPermissions = PortalSettings.ActiveTab.TabPermissions;
                PermissionController objPermissionController = new PermissionController();
                EventLogController objEventLog = new EventLogController();
                RoleController objRoles = new RoleController();
                RoleInfo objRole;

                int UserId = - 1;
                if( Request.IsAuthenticated )
                {
                    UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                    UserId = objUserInfo.UserID;
                }

                switch( optModuleType.SelectedItem.Value )
                {
                    case "0": // new module

                        if( cboDesktopModules.SelectedIndex > 0 )
                        {
                            try
                            {
                                DesktopModuleController objDesktopModules = new DesktopModuleController();
                                ArrayList arrDM = objDesktopModules.GetDesktopModulesByPortal( PortalSettings.PortalId );
                                int intloop;
                                bool isSelectable = false;
                                for( intloop = 0; intloop <= arrDM.Count - 1; intloop++ )
                                {
                                    if( ( (DesktopModuleInfo)arrDM[intloop] ).DesktopModuleID == int.Parse( cboDesktopModules.SelectedItem.Value ) )
                                    {
                                        isSelectable = true;
                                        break;
                                    }
                                }
                                if( isSelectable == false )
                                {
                                    throw new Exception();
                                }
                            }
                            catch( Exception )
                            {
                                throw new Exception();
                            }

                            ModuleDefinitionController objModuleDefinitions = new ModuleDefinitionController();
                            ModuleDefinitionInfo objModuleDefinition;

                            int intIndex;
                            ArrayList arrModuleDefinitions = objModuleDefinitions.GetModuleDefinitions( int.Parse( cboDesktopModules.SelectedItem.Value ) );
                            for( intIndex = 0; intIndex <= arrModuleDefinitions.Count - 1; intIndex++ )
                            {
                                objModuleDefinition = (ModuleDefinitionInfo)arrModuleDefinitions[intIndex];

                                ModuleInfo objModule = new ModuleInfo();
                                objModule.Initialize( PortalSettings.PortalId );

                                objModule.PortalID = PortalSettings.PortalId;
                                objModule.TabID = PortalSettings.ActiveTab.TabID;
                                if( cboPosition.SelectedItem != null )
                                {
                                    objModule.ModuleOrder = int.Parse( cboPosition.SelectedItem.Value );
                                }
                                else
                                {
                                    objModule.ModuleOrder = - 1; // bottom
                                }
                                if( txtTitle.Text != "" )
                                {
                                    objModule.ModuleTitle = txtTitle.Text;
                                }
                                else
                                {
                                    objModule.ModuleTitle = objModuleDefinition.FriendlyName;
                                }
                                objModule.PaneName = cboPanes.SelectedItem.Text;
                                objModule.ModuleDefID = objModuleDefinition.ModuleDefID;
                                objModule.CacheTime = objModuleDefinition.DefaultCacheTime;

                                // initialize module permissions
                                ModulePermissionCollection objModulePermissions = new ModulePermissionCollection();
                                objModule.ModulePermissions = objModulePermissions;
                                objModule.InheritViewPermissions = false;

                                // get the default module view permissions
                                ArrayList arrSystemModuleViewPermissions = objPermissionController.GetPermissionByCodeAndKey( "SYSTEM_MODULE_DEFINITION", "VIEW" );

                                // get the permissions from the page
                                TabPermissionInfo objTabPermission;
                                foreach( TabPermissionInfo tempLoopVar_objTabPermission in objTabPermissions )
                                {
                                    objTabPermission = tempLoopVar_objTabPermission;
                                    // get the system module permissions for the permissionkey
                                    ArrayList arrSystemModulePermissions = objPermissionController.GetPermissionByCodeAndKey( "SYSTEM_MODULE_DEFINITION", objTabPermission.PermissionKey );
                                    int j;
                                    // loop through the system module permissions
                                    for( j = 0; j <= arrSystemModulePermissions.Count - 1; j++ )
                                    {
                                        // create the module permission
                                        ModulePermissionInfo objModulePermission = new ModulePermissionInfo();
                                        PermissionInfo objSystemModulePermission;
                                        objSystemModulePermission = (PermissionInfo)arrSystemModulePermissions[j];
                                        objModulePermission.ModuleID = objModule.ModuleID;
                                        objModulePermission.PermissionID = objSystemModulePermission.PermissionID;
                                        objModulePermission.RoleID = objTabPermission.RoleID;
                                        objModulePermission.PermissionKey = objSystemModulePermission.PermissionKey;
                                        objModulePermission.AllowAccess = false;

                                        // allow access to the permission if the role is in the list of administrator roles for the page
                                        objRole = objRoles.GetRole( objModulePermission.RoleID, PortalSettings.PortalId );
                                        if( objRole != null )
                                        {
                                            if( PortalSettings.ActiveTab.AdministratorRoles.IndexOf( objRole.RoleName ) != - 1 )
                                            {
                                                objModulePermission.AllowAccess = true;
                                            }
                                        }

                                        // add the permission to the collection
                                        if( ! objModulePermissions.Contains( objModulePermission ) && objModulePermission.AllowAccess )
                                        {
                                            objModulePermissions.Add( objModulePermission );
                                        }

                                        // ensure that every EDIT permission which allows access also provides VIEW permission
                                        if( objModulePermission.PermissionKey == "EDIT" && objModulePermission.AllowAccess )
                                        {
                                            ModulePermissionInfo objModuleViewperm = new ModulePermissionInfo();
                                            objModuleViewperm.ModuleID = objModulePermission.ModuleID;
                                            objModuleViewperm.PermissionID = ( (PermissionInfo)arrSystemModuleViewPermissions[0] ).PermissionID;
                                            objModuleViewperm.RoleID = objModulePermission.RoleID;
                                            objModuleViewperm.PermissionKey = "VIEW";
                                            objModuleViewperm.AllowAccess = true;
                                            if( ! objModulePermissions.Contains( objModuleViewperm ) )
                                            {
                                                objModulePermissions.Add( objModuleViewperm );
                                            }
                                        }
                                    }
                                }

                                if( cboPermission.SelectedItem != null )
                                {
                                    switch( int.Parse( cboPermission.SelectedItem.Value ) )
                                    {
                                        case (int)ViewPermissionType.View:

                                            objModule.InheritViewPermissions = true;
                                            break;
                                        case (int)ViewPermissionType.Edit:

                                            objModule.ModulePermissions = objModulePermissions;
                                            break;
                                    }
                                }
                                else
                                {
                                    objModule.InheritViewPermissions = true;
                                }

                                objModule.AllTabs = false;
                                objModule.Alignment = cboAlign.SelectedItem.Value;

                                ModuleController objModules = new ModuleController();
                                objModules.AddModule( objModule );
                                objEventLog.AddLog( objModule, PortalSettings, UserId, "", EventLogController.EventLogType.MODULE_CREATED );
                            }

                            // Redirect to the same page to pick up changes
                            Response.Redirect( Request.RawUrl, true );
                        }
                        break;
                    case "1": // existing module

                        if( cboModules.SelectedItem != null )
                        {
                            ModuleController objModules = new ModuleController();
                            ModuleInfo objModule = objModules.GetModule( int.Parse( cboModules.SelectedItem.Value ), int.Parse( cboTabs.SelectedItem.Value ) );
                            if( objModule != null )
                            {
                                objModule.TabID = PortalSettings.ActiveTab.TabID;
                                if( cboPosition.SelectedItem != null )
                                {
                                    objModule.ModuleOrder = int.Parse( cboPosition.SelectedItem.Value );
                                }
                                else
                                {
                                    objModule.ModuleOrder = - 1; // bottom
                                }
                                objModule.PaneName = cboPanes.SelectedItem.Text;
                                objModule.Alignment = cboAlign.SelectedItem.Value;
                                objModules.AddModule( objModule );
                                objEventLog.AddLog( objModule, PortalSettings, UserId, "", EventLogController.EventLogType.MODULE_CREATED );
                            }

                            // Redirect to the same page to pick up changes
                            Response.Redirect( Request.RawUrl, true );
                        }
                        break;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void optModuleType_SelectedIndexChanged( object sender, EventArgs e )
        {
            BindData();
        }

        protected void cboTabs_SelectedIndexChanged( object sender, EventArgs e )
        {
            ModuleController objModules = new ModuleController();
            ArrayList arrModules = new ArrayList();

            ModuleInfo objModule;
            ArrayList arrPortalModules = objModules.GetPortalTabModules( PortalSettings.PortalId, int.Parse( cboTabs.SelectedItem.Value ) );
            foreach( ModuleInfo tempLoopVar_objModule in arrPortalModules )
            {
                objModule = tempLoopVar_objModule;
                if( PortalSecurity.IsInRoles( objModule.AuthorizedEditRoles ) && objModule.IsDeleted == false )
                {
                    arrModules.Add( objModule );
                }
            }

            cboModules.DataSource = arrModules;
            cboModules.DataBind();
        }

        protected void Page_Init( Object sender, EventArgs e )
        {


            this.ID = "IconBar.ascx";
        }
    }
}