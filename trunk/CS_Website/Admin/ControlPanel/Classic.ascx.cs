#region DotNetNuke License
// DotNetNuke� - http://www.dotnetnuke.com
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
using System.Web.UI;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.ControlPanels
{
    /// <summary>
    /// The Classic ControlPanel provides athe Classic Page/Module manager
    /// </summary>
    /// <history>
    /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class Classic : ControlPanelBase
    {
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
                    //Localization
                    lblAdmin.Text = Localization.GetString("Admin", this.LocalResourceFile);
                    cmdAddTab.Text = Localization.GetString("AddTab", this.LocalResourceFile);
                    cmdAddTab.ToolTip = Localization.GetString("AddTab.ToolTip", this.LocalResourceFile);
                    cmdEditTab.Text = Localization.GetString("EditTab", this.LocalResourceFile);
                    cmdEditTab.ToolTip = Localization.GetString("EditTab.ToolTip", this.LocalResourceFile);
                    cmdCopyTab.Text = Localization.GetString("CopyTab", this.LocalResourceFile);
                    cmdCopyTab.ToolTip = Localization.GetString("CopyTab.ToolTip", this.LocalResourceFile);
                    cmdHelpShow.AlternateText = Localization.GetString("ShowHelp.AlternateText", this.LocalResourceFile);
                    cmdHelpHide.AlternateText = Localization.GetString("HideHelp.AlternateText", this.LocalResourceFile);
                    lblModule.Text = Localization.GetString("Module", this.LocalResourceFile);
                    lblPane.Text = Localization.GetString("Pane", this.LocalResourceFile);
                    lblAlign.Text = Localization.GetString("Align", this.LocalResourceFile);
                    cmdAdd.Text = Localization.GetString("AddModule", this.LocalResourceFile);
                    cmdAdd.ToolTip = Localization.GetString("AddModule.ToolTip", this.LocalResourceFile);
                    chkContent.Text = Localization.GetString("Content.Text", this.LocalResourceFile);
                    chkPreview.Text = Localization.GetString("Preview.Text", this.LocalResourceFile);
                    chkContent.ToolTip = Localization.GetString("Content.ToolTip", this.LocalResourceFile);
                    chkPreview.ToolTip = Localization.GetString("Preview.ToolTip", this.LocalResourceFile);

                    cmdAddTab.NavigateUrl = Globals.NavigateURL("Tab");
                    if (!PortalSettings.ActiveTab.IsAdminTab)
                    {
                        cmdEditTab.NavigateUrl = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=edit");
                        cmdCopyTab.NavigateUrl = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=copy");
                    }
                    else
                    {
                        cmdEditTab.Visible = false;
                        cmdCopyTab.Visible = false;
                    }

                    cmdHelpShow.ImageUrl = "~/images/help.gif";
                    cmdHelpShow.Visible = true;

                    cmdHelpHide.ImageUrl = "~/images/cancel.gif";
                    cmdHelpHide.Visible = false;

                    DesktopModuleController objDesktopModules = new DesktopModuleController();
                    cboDesktopModules.DataSource = objDesktopModules.GetDesktopModulesByPortal(PortalSettings.PortalId);
                    cboDesktopModules.DataBind();

                    
                    for (int intItem = 0; intItem < PortalSettings.ActiveTab.Panes.Count; intItem++)
                    {
                        cboPanes.Items.Add(Convert.ToString(PortalSettings.ActiveTab.Panes[intItem]));
                    }
                    cboPanes.Items.FindByValue(Globals.glbDefaultPane).Selected = true;

                    if (cboAlign.Items.Count > 0)
                    {
                        cboAlign.SelectedIndex = 0;
                    }

                    chkContent.Checked = ShowContent;
                    chkPreview.Checked = IsPreview;
                }

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// cmdAdd_Click runs when the Add Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        ///     [vmasanas]  01/07/2005  Modified to add view perm. to all roles with edit perm.
        /// </history>
        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ViewPermissionType permissionType = ViewPermissionType.View;
                int position = Null.NullInteger;

                // save to database
                AddNewModule("", Int32.Parse(cboDesktopModules.SelectedItem.Value), cboPanes.SelectedItem.Text, Null.NullInteger, ViewPermissionType.View, cboAlign.SelectedItem.Value);

                // Redirect to the same page to pick up changes
                Response.Redirect(Request.RawUrl, true);

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// chkContent_CheckedChanged runs when the Content check box is changed
        /// </summary>
        /// <history>
        /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void chkContent_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                SetContentMode(chkContent.Checked);

                Response.Redirect(Request.RawUrl, true);

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// chkPreview_CheckedChanged runs when the Preview check box is changed
        /// </summary>
        /// <history>
        /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void chkPreview_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                SetPreviewMode(chkPreview.Checked);

                Response.Redirect(Request.RawUrl, true);

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// cmdHelpHide_Click runs when the Hide Help Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdHelpHide_Click( object sender, ImageClickEventArgs e )
        {
            try
            {
                lblDescription.Text = "";

                cmdHelpShow.Visible = true;
                cmdHelpHide.Visible = false;
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdHelpShow_Click runs when the Hide Help Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdHelpShow_Click( object sender, ImageClickEventArgs e )
        {
            try
            {
                DesktopModuleController objDesktopModules = new DesktopModuleController();
                DesktopModuleInfo objDesktopModule;

                objDesktopModule = objDesktopModules.GetDesktopModule( int.Parse( cboDesktopModules.SelectedItem.Value ) );
                if( objDesktopModule != null )
                {
                    lblDescription.Text = "<br>" + objDesktopModule.Description + "<br>";
                }

                cmdHelpShow.Visible = false;
                cmdHelpHide.Visible = true;
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void Page_Init( Object sender, EventArgs e )
        {

            this.ID = "Classic.ascx";
        }
    }
}