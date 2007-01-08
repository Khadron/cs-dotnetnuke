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
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.UI.ControlPanels
{
    /// <summary>
    /// The IconBar ControlPanel provides an icon bar based Page/Module manager
    /// </summary>
    /// <history>
    /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class IconBar : ControlPanelBase
    {
        private void BindData()
        {

            switch (optModuleType.SelectedItem.Value)
            {
                case "0": // new module
                    cboTabs.Visible = false;
                    cboModules.Visible = false;
                    cboDesktopModules.Visible = true;
                    txtTitle.Visible = true;
                    lblModule.Text = Localization.GetString("Module", this.LocalResourceFile);
                    lblTitle.Text = Localization.GetString("Title", this.LocalResourceFile);
                    cboPermission.Enabled = true;

                    DesktopModuleController objDesktopModules = new DesktopModuleController();
                    cboDesktopModules.DataSource = objDesktopModules.GetDesktopModulesByPortal(PortalSettings.PortalId);
                    cboDesktopModules.DataBind();
                    cboDesktopModules.Items.Insert(0, new ListItem("<" + Localization.GetString("SelectModule", this.LocalResourceFile) + ">", "-1"));
                    break;
                case "1": // existing module
                    cboTabs.Visible = true;
                    cboModules.Visible = true;
                    cboDesktopModules.Visible = false;
                    txtTitle.Visible = false;
                    lblModule.Text = Localization.GetString("Tab", this.LocalResourceFile);
                    lblTitle.Text = Localization.GetString("Module", this.LocalResourceFile);
                    cboPermission.Enabled = false;

                    ArrayList arrTabs = new ArrayList();

                    ArrayList arrPortalTabs = Globals.GetPortalTabs(PortalSettings.DesktopTabs, true, true);
                    foreach (TabInfo objTab in arrPortalTabs)
                    {
                        if (objTab.TabID == -1)
                        {
                            // <none specified>
                            objTab.TabName = "<" + Localization.GetString("SelectPage", this.LocalResourceFile) + ">";
                            arrTabs.Add(objTab);
                        }
                        else
                        {
                            if (objTab.TabID != PortalSettings.ActiveTab.TabID)
                            {
                                if (PortalSecurity.IsInRoles(objTab.AuthorizedRoles))
                                {
                                    arrTabs.Add(objTab);
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
        /// <history>
        /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!Page.IsPostBack)
                {

                    // localization
                    lblPageFunctions.Text = Localization.GetString("PageFunctions", this.LocalResourceFile);
                    optModuleType.Items.FindByValue("0").Selected = true;
                    lblCommonTasks.Text = Localization.GetString("CommonTasks", this.LocalResourceFile);
                    imgAddTabIcon.AlternateText = Localization.GetString("AddTab.AlternateText", this.LocalResourceFile);
                    cmdAddTab.Text = Localization.GetString("AddTab", this.LocalResourceFile);
                    imgEditTabIcon.AlternateText = Localization.GetString("EditTab.AlternateText", this.LocalResourceFile);
                    cmdEditTab.Text = Localization.GetString("EditTab", this.LocalResourceFile);
                    imgDeleteTabIcon.AlternateText = Localization.GetString("DeleteTab.AlternateText", this.LocalResourceFile);
                    cmdDeleteTab.Text = Localization.GetString("DeleteTab", this.LocalResourceFile);
                    imgCopyTabIcon.AlternateText = Localization.GetString("CopyTab.AlternateText", this.LocalResourceFile);
                    cmdCopyTab.Text = Localization.GetString("CopyTab", this.LocalResourceFile);
                    imgPreviewTabIcon.AlternateText = Localization.GetString("PreviewTab.AlternateText", this.LocalResourceFile);
                    cmdPreviewTab.Text = Localization.GetString("PreviewTab", this.LocalResourceFile);
                    if (IsPreview)
                    {
                        imgPreviewTabIcon.ImageUrl = "~/Admin/ControlPanel/images/iconbar_previewtab_on.gif";
                    }
                    lblModule.Text = Localization.GetString("Module", this.LocalResourceFile);
                    lblPane.Text = Localization.GetString("Pane", this.LocalResourceFile);
                    lblTitle.Text = Localization.GetString("Title", this.LocalResourceFile);
                    lblAlign.Text = Localization.GetString("Align", this.LocalResourceFile);
                    imgAddModuleIcon.AlternateText = Localization.GetString("AddModule.AlternateText", this.LocalResourceFile);
                    cmdAddModule.Text = Localization.GetString("AddModule", this.LocalResourceFile);
                    imgWizardIcon.AlternateText = Localization.GetString("Wizard.AlternateText", this.LocalResourceFile);
                    cmdWizard.Text = Localization.GetString("Wizard", this.LocalResourceFile);
                    imgSiteIcon.AlternateText = Localization.GetString("Site.AlternateText", this.LocalResourceFile);
                    cmdSite.Text = Localization.GetString("Site", this.LocalResourceFile);
                    imgUsersIcon.AlternateText = Localization.GetString("Users.AlternateText", this.LocalResourceFile);
                    cmdUsers.Text = Localization.GetString("Users", this.LocalResourceFile);
                    imgFilesIcon.AlternateText = Localization.GetString("Files.AlternateText", this.LocalResourceFile);
                    cmdFiles.Text = Localization.GetString("Files", this.LocalResourceFile);
                    imgHelpIcon.AlternateText = Localization.GetString("Help.AlternateText", this.LocalResourceFile);
                    cmdHelp.Text = Localization.GetString("Help", this.LocalResourceFile);

                    if (PortalSettings.ActiveTab.IsAdminTab)
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
                        ClientAPI.AddButtonConfirm(cmdDeleteTab, Localization.GetString("DeleteTabConfirm", this.LocalResourceFile));
                        ClientAPI.AddButtonConfirm(cmdDeleteTabIcon, Localization.GetString("DeleteTabConfirm", this.LocalResourceFile));
                    }

                    if (Globals.IsAdminControl())
                    {
                        cmdAddModule.Enabled = false;
                        imgAddModuleIcon.ImageUrl = "~/Admin/ControlPanel/images/iconbar_addmodule_bw.gif";
                        cmdAddModuleIcon.Enabled = false;
                    }

                    if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) == false)
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

                    if (PortalSettings.ActiveTab.IsAdminTab == false & Globals.IsAdminControl() == false)
                    {
                        for (int intItem = 0; intItem < PortalSettings.ActiveTab.Panes.Count; intItem++)
                        {
                            cboPanes.Items.Add(Convert.ToString(PortalSettings.ActiveTab.Panes[intItem]));
                        }
                    }
                    else
                    {
                        cboPanes.Items.Add(Globals.glbDefaultPane);
                    }
                    if (cboPanes.Items.FindByValue(Globals.glbDefaultPane) != null)
                    {
                        cboPanes.Items.FindByValue(Globals.glbDefaultPane).Selected = true;
                    }

                    if (cboPermission.Items.Count > 0)
                    {
                        cboPermission.SelectedIndex = 0; // view
                    }

                    if (cboAlign.Items.Count > 0)
                    {
                        cboAlign.SelectedIndex = 0; // left
                    }

                    if (cboPosition.Items.Count > 0)
                    {
                        cboPosition.SelectedIndex = 1; // bottom
                    }

                    if (Convert.ToString(PortalSettings.HostSettings["HelpURL"]) != "")
                    {
                        cmdHelp.NavigateUrl = Globals.FormatHelpUrl(Convert.ToString(PortalSettings.HostSettings["HelpURL"]), PortalSettings, "");
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
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// PageFunctions_Click runs when any button in the Page toolbar is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void PageFunctions_Click(object sender, EventArgs e)
        {
            try
            {
                string URL = Request.RawUrl;
                switch (((LinkButton)sender).ID)
                {
                    case "cmdAddTab":
                    case "cmdAddTabIcon":
                        URL = Globals.NavigateURL("Tab");
                        break;
                    case "cmdEditTab":
                    case "cmdEditTabIcon":
                        URL = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=edit");
                        break;
                    case "cmdDeleteTab":
                    case "cmdDeleteTabIcon":
                        URL = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=delete");
                        break;
                    case "cmdCopyTab":
                    case "cmdCopyTabIcon":
                        URL = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=copy");
                        break;
                    case "cmdPreviewTab":
                    case "cmdPreviewTabIcon":
                        SetPreviewMode(!IsPreview);
                        break;
                }

                Response.Redirect(URL, true);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
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
        /// <history>
        /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        ///     [vmasanas]  01/07/2005  Modified to add view perm. to all roles with edit perm.
        /// </history>
        protected void AddModule_Click(object sender, EventArgs e)
        {
            try
            {
                string title = txtTitle.Text;
                ViewPermissionType permissionType = ViewPermissionType.View;
                int position = Null.NullInteger;
                if (cboPosition.SelectedItem != null)
                {
                    position = int.Parse(cboPosition.SelectedItem.Value);
                }
                if (cboPermission.SelectedItem != null)
                {
                    permissionType = (ViewPermissionType)Enum.Parse(typeof(ViewPermissionType), cboPermission.SelectedItem.Value);
                }

                switch (optModuleType.SelectedItem.Value)
                {
                    case "0": // new module
                        if (cboDesktopModules.SelectedIndex > 0)
                        {
                            AddNewModule(title, int.Parse(cboDesktopModules.SelectedItem.Value), cboPanes.SelectedItem.Text, position, permissionType, cboAlign.SelectedItem.Value);

                            // Redirect to the same page to pick up changes
                            Response.Redirect(Request.RawUrl, true);
                        }
                        break;
                    case "1": // existing module
                        if (cboModules.SelectedItem != null)
                        {
                            AddExistingModule(int.Parse(cboModules.SelectedItem.Value), int.Parse(cboTabs.SelectedItem.Value), cboPanes.SelectedItem.Text, position, cboAlign.SelectedItem.Value);

                            // Redirect to the same page to pick up changes
                            Response.Redirect(Request.RawUrl, true);
                        }
                        break;
                }

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
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

            ArrayList arrPortalModules = objModules.GetPortalTabModules( PortalSettings.PortalId, int.Parse( cboTabs.SelectedItem.Value ) );
            foreach( ModuleInfo tempLoopVar_objModule in arrPortalModules )
            {
                ModuleInfo objModule = tempLoopVar_objModule;
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
            cmdAddTab.Click += new EventHandler(PageFunctions_Click);
            cmdAddTabIcon.Click += new EventHandler(PageFunctions_Click);
            cmdEditTab.Click += new EventHandler(PageFunctions_Click);
            cmdEditTabIcon.Click += new EventHandler(PageFunctions_Click);
            cmdDeleteTab.Click += new EventHandler(PageFunctions_Click);
            cmdDeleteTabIcon.Click += new EventHandler(PageFunctions_Click);
            cmdCopyTab.Click += new EventHandler(PageFunctions_Click);
            cmdCopyTabIcon.Click += new EventHandler(PageFunctions_Click);
            cmdPreviewTab.Click += new EventHandler(PageFunctions_Click);
            cmdPreviewTabIcon.Click += new EventHandler(PageFunctions_Click);

            cmdAddModule.Click += new EventHandler(AddModule_Click);
            cmdAddModuleIcon.Click += new EventHandler(AddModule_Click);

            this.ID = "IconBar.ascx";
        }
    }
}