#region DotNetNuke License
// DotNetNuke� - http://www.dotnetnuke.com
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
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Utilities;
using Calendar=DotNetNuke.Common.Utilities.Calendar;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Modules
{
    /// <summary>
    /// The ModuleSettingsPage PortalModuleBase is used to edit the settings for a
    /// module.
    /// </summary>
    /// <history>
    /// 	[cnurse]	10/18/2004	documented
    /// 	[cnurse]	10/19/2004	modified to support custm module specific settings
    /// </history>
    public partial class ModuleSettingsPage : PortalModuleBase
    {
        protected ModuleSettingsBase ctlSpecific;

        private int moduleId = - 1;
        private int tabModuleId = -1;

        /// <summary>
        /// BindData loads the settings from the Database
        /// </summary>
        /// <history>
        /// 	[cnurse]	10/18/2004	documented
        /// </history>
        private void BindData()
        {
            // declare roles
            ArrayList arrAvailableAuthViewRoles = new ArrayList();
            ArrayList arrAvailableAuthEditRoles = new ArrayList();

            // add an entry of All Users for the View roles
            arrAvailableAuthViewRoles.Add( new ListItem( "All Users", Globals.glbRoleAllUsers ) );
            // add an entry of Unauthenticated Users for the View roles
            arrAvailableAuthViewRoles.Add( new ListItem( "Unauthenticated Users", Globals.glbRoleUnauthUser ) );
            // add an entry of All Users for the Edit roles
            arrAvailableAuthEditRoles.Add( new ListItem( "All Users", Globals.glbRoleAllUsers ) );

            // process portal roles
            RoleController objRoles = new RoleController();
            RoleInfo objRole;
            ArrayList arrRoleInfo = objRoles.GetPortalRoles( PortalId );
            foreach( RoleInfo tempLoopVar_objRole in arrRoleInfo )
            {
                objRole = tempLoopVar_objRole;
                arrAvailableAuthViewRoles.Add( new ListItem( objRole.RoleName, objRole.RoleID.ToString() ) );
            }
            foreach( RoleInfo tempLoopVar_objRole in arrRoleInfo )
            {
                objRole = tempLoopVar_objRole;
                arrAvailableAuthEditRoles.Add( new ListItem( objRole.RoleName, objRole.RoleID.ToString() ) );
            }

            // get module
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModule( moduleId, TabId, false );
            if( objModule != null )
            {
                // configure grid
                DesktopModuleController objDeskMod = new DesktopModuleController();
                DesktopModuleInfo desktopModule = objDeskMod.GetDesktopModule( objModule.DesktopModuleID );
                dgPermissions.ResourceFile = Globals.ApplicationPath + "/DesktopModules/" + desktopModule.FolderName + "/" + Localization.LocalResourceDirectory + "/" + Localization.LocalSharedResourceFile;

                chkInheritPermissions.Checked = objModule.InheritViewPermissions;
                dgPermissions.InheritViewPermissionsFromTab = objModule.InheritViewPermissions;

                txtTitle.Text = objModule.ModuleTitle;
                ctlIcon.Url = objModule.IconFile;

                if( cboTab.Items.FindByValue( objModule.TabID.ToString() ) != null )
                {
                    cboTab.Items.FindByValue( objModule.TabID.ToString() ).Selected = true;
                }

                chkAllTabs.Checked = objModule.AllTabs;
                cboVisibility.SelectedIndex = (int)objModule.Visibility;

                ModuleDefinitionController objModuleDefController = new ModuleDefinitionController();
                ModuleDefinitionInfo objModuleDef = objModuleDefController.GetModuleDefinition( objModule.ModuleDefID );
                if( objModuleDef.DefaultCacheTime == Null.NullInteger )
                {
                    rowCache.Visible = false;
                }
                else
                {
                    txtCacheTime.Text = objModule.CacheTime.ToString();
                }

                txtCacheTime.Text = objModule.CacheTime.ToString();

                // populate view roles
                ArrayList arrAssignedAuthViewRoles = new ArrayList();
                Array arrAuthViewRoles = objModule.AuthorizedViewRoles.Split(new char[] { ';' });
                foreach( string strRole in arrAuthViewRoles )
                {
                    if( !String.IsNullOrEmpty( strRole ) )
                    {
                        foreach( ListItem objListItem in arrAvailableAuthViewRoles )
                        {                            
                            if( objListItem.Value == strRole )
                            {
                                arrAssignedAuthViewRoles.Add( objListItem );
                                arrAvailableAuthViewRoles.Remove( objListItem );
                                break;
                            }
                        }
                    }
                }

                // populate edit roles
                ArrayList arrAssignedAuthEditRoles = new ArrayList();
                Array arrAuthEditRoles = objModule.AuthorizedEditRoles.Split(new char[] { ';' });
                foreach( string strRole in arrAuthEditRoles )
                {
                    
                    if( !String.IsNullOrEmpty( strRole ) )
                    {
                        foreach( ListItem objListItem in arrAvailableAuthEditRoles )
                        {                            
                            if( objListItem.Value == strRole )
                            {
                                arrAssignedAuthEditRoles.Add( objListItem );
                                arrAvailableAuthEditRoles.Remove( objListItem );
                                break;
                            }
                        }
                    }
                }

                if( objModule.Alignment == "" )
                {
                    objModule.Alignment = "left";
                }
                cboAlign.Items.FindByValue( objModule.Alignment ).Selected = true;
                cboTab.Items.FindByValue( Convert.ToString( TabId ) ).Selected = true;
                txtColor.Text = objModule.Color;
                txtBorder.Text = objModule.Border;

                txtHeader.Text = objModule.Header;
                txtFooter.Text = objModule.Footer;

                if( ! Null.IsNull( objModule.StartDate ) )
                {
                    txtStartDate.Text = objModule.StartDate.ToShortDateString();
                }
                if( ! Null.IsNull( objModule.EndDate ) )
                {
                    txtEndDate.Text = objModule.EndDate.ToShortDateString();
                }

                ctlModuleContainer.Width = "250px";
                ctlModuleContainer.SkinRoot = SkinInfo.RootContainer;
                ctlModuleContainer.SkinSrc = objModule.ContainerSrc;

                chkDisplayTitle.Checked = objModule.DisplayTitle;
                chkDisplayPrint.Checked = objModule.DisplayPrint;
                chkDisplaySyndicate.Checked = objModule.DisplaySyndicate;
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <history>
        /// 	[cnurse]	10/18/2004	documented
        /// 	[cnurse]	10/19/2004	modified to support custm module specific settings
        ///     [vmasanas]  11/28/2004  modified to support modules in admin tabs
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                // Verify that the current user has access to edit this module
                if( PortalSecurity.IsInRoles( PortalSettings.AdministratorRoleName ) == false && PortalSecurity.IsInRoles( PortalSettings.ActiveTab.AdministratorRoles.ToString() ) == false )
                {
                    Response.Redirect( Globals.NavigateURL( "Access Denied" ), true );
                }

                //this needs to execute always to the client script code is registred in InvokePopupCal
                cmdStartCalendar.NavigateUrl = Calendar.InvokePopupCal( txtStartDate );
                cmdEndCalendar.NavigateUrl = Calendar.InvokePopupCal( txtEndDate );

                if( Page.IsPostBack == false )
                {
                    ctlIcon.FileFilter = Globals.glbImageFileTypes;
                    
                    dgPermissions.TabId = PortalSettings.ActiveTab.TabID;
                    dgPermissions.ModuleID = moduleId;

                    ClientAPI.AddButtonConfirm( cmdDelete, Localization.GetString( "DeleteItem" ) );

                    cboTab.DataSource = Globals.GetPortalTabs( PortalSettings.DesktopTabs, -1, false, true, false, false, true );
                    cboTab.DataBind();
                    //if is and admin or host tab, then add current tab
                    if( PortalSettings.ActiveTab.ParentId == PortalSettings.AdminTabId || PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId )
                    {
                        cboTab.Items.Insert( 0, new ListItem( PortalSettings.ActiveTab.TabName, PortalSettings.ActiveTab.TabID.ToString() ) );
                    }

                    // tab administrators can only manage their own tab
                    if( PortalSecurity.IsInRoles( PortalSettings.AdministratorRoleName ) == false )
                    {
                        chkAllTabs.Enabled = false;
                        chkDefault.Enabled = false;
                        chkAllModules.Enabled = false;
                        cboTab.Enabled = false;
                    }

                    if( moduleId != - 1 )
                    {
                        BindData();
                    }
                    else
                    {
                        cboVisibility.SelectedIndex = 0; // maximized
                        chkAllTabs.Checked = false;
                        cmdDelete.Visible = false;
                    }

                    //Set visibility of Specific Settings
                    if( ctlSpecific != null )
                    {
                        //Get the module settings from the PortalSettings and pass the
                        //two settings hashtables to the sub control to process
                        ctlSpecific.LoadSettings();
                        dshSpecific.Visible = true;
                        tblSpecific.Visible = true;
                    }
                    else
                    {
                        dshSpecific.Visible = false;
                        tblSpecific.Visible = false;
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// chkInheritPermissions_CheckedChanged runs when the Inherit View Permissions
        ///	check box is changed
        /// </summary>
        /// <history>
        /// 	[cnurse]	10/18/2004	documented
        /// </history>
        protected void chkInheritPermissions_CheckedChanged( Object sender, EventArgs e )
        {
            if( chkInheritPermissions.Checked )
            {
                dgPermissions.InheritViewPermissionsFromTab = true;
            }
            else
            {
                dgPermissions.InheritViewPermissionsFromTab = false;
            }
        }

        /// <summary>
        /// cmdCancel_Click runs when the Cancel LinkButton is clicked.  It returns the user
        /// to the referring page
        /// </summary>
        /// <history>
        /// 	[cnurse]	10/18/2004	documented
        /// </history>
        protected void cmdCancel_Click( Object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Globals.NavigateURL(), true );
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
        /// 	[cnurse]	10/18/2004	documented
        /// </history>
        protected void cmdDelete_Click( Object sender, EventArgs e )
        {
            try
            {
                ModuleController objModules = new ModuleController();

                objModules.DeleteTabModule( TabId, moduleId );

                Response.Redirect( Globals.NavigateURL(), true );
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
        /// 	[cnurse]	10/18/2004	documented
        /// 	[cnurse]	10/19/2004	modified to support custm module specific settings
        /// </history>
        protected void cmdUpdate_Click( object Sender, EventArgs e )
        {
            try
            {
                if( Page.IsValid )
                {
                    ModuleController objModules = new ModuleController();
                    bool AllTabsChanged = false;
                   
                    // tab administrators can only manage their own tab
                    if( PortalSecurity.IsInRoles( PortalSettings.AdministratorRoleName ) == false )
                    {
                        chkAllTabs.Enabled = false;
                        chkDefault.Enabled = false;
                        chkAllModules.Enabled = false;
                        cboTab.Enabled = false;
                    }

                    // update module
                    ModuleInfo objModule = objModules.GetModule( moduleId, TabId, false );

                    objModule.ModuleID = moduleId;
                    objModule.ModuleTitle = txtTitle.Text;
                    objModule.Alignment = cboAlign.SelectedItem.Value;
                    objModule.Color = txtColor.Text;
                    objModule.Border = txtBorder.Text;
                    objModule.IconFile = ctlIcon.Url;
                    if( !String.IsNullOrEmpty( txtCacheTime.Text ) )
                    {
                        objModule.CacheTime = int.Parse( txtCacheTime.Text );
                    }
                    else
                    {
                        objModule.CacheTime = 0;
                    }
                    objModule.TabID = TabId;
                    if( objModule.AllTabs != chkAllTabs.Checked )
                    {
                        AllTabsChanged = true;
                    }
                    objModule.AllTabs = chkAllTabs.Checked;
                    switch( int.Parse( cboVisibility.SelectedItem.Value ) )
                    {
                        case 0:

                            objModule.Visibility = VisibilityState.Maximized;
                            break;
                        case 1:

                            objModule.Visibility = VisibilityState.Minimized;
                            break;
                        case 2:

                            objModule.Visibility = VisibilityState.None;
                            break;
                    }
                    objModule.IsDeleted = false;
                    objModule.Header = txtHeader.Text;
                    objModule.Footer = txtFooter.Text;
                    if( !String.IsNullOrEmpty( txtStartDate.Text ) )
                    {
                        objModule.StartDate = Convert.ToDateTime( txtStartDate.Text );
                    }
                    else
                    {
                        objModule.StartDate = Null.NullDate;
                    }
                    if( !String.IsNullOrEmpty( txtEndDate.Text ) )
                    {
                        objModule.EndDate = Convert.ToDateTime( txtEndDate.Text );
                    }
                    else
                    {
                        objModule.EndDate = Null.NullDate;
                    }
                    objModule.ContainerSrc = ctlModuleContainer.SkinSrc;
                    objModule.ModulePermissions = dgPermissions.Permissions;
                    objModule.InheritViewPermissions = chkInheritPermissions.Checked;
                    objModule.DisplayTitle = chkDisplayTitle.Checked;
                    objModule.DisplayPrint = chkDisplayPrint.Checked;
                    objModule.DisplaySyndicate = chkDisplaySyndicate.Checked;
                    objModule.IsDefaultModule = chkDefault.Checked;
                    objModule.AllModules = chkAllModules.Checked;
                    objModules.UpdateModule( objModule );

                    //Update Custom Settings
                    if( ctlSpecific != null )
                    {
                        ctlSpecific.UpdateSettings();
                    }

                    //These Module Copy/Move statements must be
                    //at the end of the Update as the Controller code assumes all the
                    //Updates to the Module have been carried out.

                    //Check if the Module is to be Moved to a new Tab
                    if( ! chkAllTabs.Checked )
                    {
                        int newTabId = int.Parse( cboTab.SelectedItem.Value );
                        if( TabId != newTabId )
                        {
                            objModules.MoveModule( moduleId, TabId, newTabId, "" );
                        }
                    }

                    //'Check if Module is to be Added/Removed from all Tabs
                    if( AllTabsChanged )
                    {
                        ArrayList arrTabs = Globals.GetPortalTabs( PortalSettings.DesktopTabs, false, true );
                        if( chkAllTabs.Checked )
                        {
                            objModules.CopyModule( moduleId, TabId, arrTabs, true );
                        }
                        else
                        {
                            objModules.DeleteAllModules( moduleId, TabId, arrTabs, false, false );
                        }
                    }

                    // Navigate back to admin page
                    Response.Redirect( Globals.NavigateURL(), true );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            ModuleController objModules = new ModuleController();
            ModuleControlController objModuleControlController = new ModuleControlController();

            // get ModuleId
            if( ( Request.QueryString["ModuleId"] != null ) )
            {
                moduleId = int.Parse( Request.QueryString["ModuleId"] );
            }

            // get module
            ModuleInfo objModule = objModules.GetModule( moduleId, TabId, false );
            if( objModule != null )
            {
                tabModuleId = objModule.TabModuleID;

                //get Settings Control(s)
                ArrayList arrModuleControls = objModuleControlController.GetModuleControlsByKey( "Settings", objModule.ModuleDefID );

                if( arrModuleControls.Count > 0 )
                {
                    ModuleControlInfo objModuleControlInfo = (ModuleControlInfo)arrModuleControls[0];
                    string src = "~/" + objModuleControlInfo.ControlSrc;
                    ctlSpecific = (ModuleSettingsBase)LoadControl( src );
                    ctlSpecific.ID = src.Substring( src.LastIndexOf( "/" ) + 1 );
                    ctlSpecific.ModuleId = moduleId;
                    ctlSpecific.TabModuleId = tabModuleId;
                    dshSpecific.Text = Localization.LocalizeControlTitle( objModuleControlInfo.ControlTitle, objModuleControlInfo.ControlSrc, "settings" );
                    pnlSpecific.Controls.Add( ctlSpecific );

                    if( Localization.GetString( ModuleActionType.HelpText, ctlSpecific.LocalResourceFile ) != "" )
                    {
                        rowspecifichelp.Visible = true;
                        imgSpecificHelp.AlternateText = Localization.GetString( ModuleActionType.ModuleHelp, Localization.GlobalResourceFile );
                        lnkSpecificHelp.Text = Localization.GetString( ModuleActionType.ModuleHelp, Localization.GlobalResourceFile );
                        lnkSpecificHelp.NavigateUrl = Globals.NavigateURL( TabId, "Help", "ctlid=" + objModuleControlInfo.ModuleControlID, "moduleid=" + moduleId );
                    }
                    else
                    {
                        rowspecifichelp.Visible = false;
                    }
                }
            }
        }
    }
}