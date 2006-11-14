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
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

namespace DotNetNuke.Modules.Admin.Tabs
{
    /// <summary>
    /// The Tabs PortalModuleBase is used to manage the Tabs/Pages for a
    /// portal.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/9/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class Tabs : PortalModuleBase, IActionable
    {
        //Pages Area

        protected ArrayList arrPortalTabs;

        /// <summary>
        /// EditTab redirects to the Edit Tab Page for the currently selected tab/page
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/9/2004	Created
        /// </history>
        private void EditTab()
        {
            // Redirect to edit page of currently selected tab
            if( lstTabs.SelectedIndex != - 1 )
            {
                // Redirect to module settings page
                TabInfo objTab = (TabInfo)arrPortalTabs[lstTabs.SelectedIndex];

                Response.Redirect( Globals.NavigateURL( objTab.TabID, "Tab", "action=edit", "returntabid=" + TabId.ToString() ), true );
            }
        }

        /// <summary>
        /// ViewTab redirects to the Tab/Page for the currently selected tab/page
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/9/2004	Created
        /// </history>
        private void ViewTab()
        {
            if( lstTabs.SelectedIndex != - 1 )
            {
                TabController objTabs = new TabController();
                TabInfo objTab = objTabs.GetTab( ( (TabInfo)arrPortalTabs[lstTabs.SelectedIndex] ).TabID );
                if( objTab != null )
                {
                    if( objTab.DisableLink )
                    {
                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "ViewTabMessage", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.YellowWarning );
                    }
                    else
                    {
                        Response.Redirect( Globals.NavigateURL( objTab.TabID ), true );
                    }
                }
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/9/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                arrPortalTabs = Globals.GetPortalTabs(PortalSettings.DesktopTabs, false, true, false, true);

                // If this is the first visit to the page, bind the tab data to the page listbox
                if( Page.IsPostBack == false )
                {
                    lstTabs.DataSource = arrPortalTabs;
                    lstTabs.DataBind();

                    // select the tab ( if specified )
                    if( Request.QueryString["selecttabid"] != null )
                    {
                        if( lstTabs.Items.FindByValue( Request.QueryString["selecttabid"] ) != null )
                        {
                            lstTabs.Items.FindByValue( Request.QueryString["selecttabid"] ).Selected = true;
                        }
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// RightLeft_Click runs when either the cmdLeft or cmdRight buttons is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/9/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void RightLeft_Click( object sender, ImageClickEventArgs e )
        {
            try
            {
                if( lstTabs.SelectedIndex != - 1 )
                {
                    TabInfo objTab = (TabInfo)arrPortalTabs[lstTabs.SelectedIndex];

                    TabController objTabs = new TabController();

                    switch( ( (ImageButton)sender ).CommandName )
                    {
                        case "left":

                            objTabs.UpdatePortalTabOrder( PortalId, objTab.TabID, objTab.ParentId, - 1, 0, true, false );
                            break;
                        case "right":

                            objTabs.UpdatePortalTabOrder( PortalId, objTab.TabID, objTab.ParentId, 1, 0, true, false );
                            break;
                    }

                    // Redirect to this site to refresh
                    Response.Redirect( Globals.NavigateURL( TabId, "", "selecttabid", objTab.TabID.ToString() ), true );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// UpDown_Click runs when either the cmdUp or cmdDown buttons is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/9/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void UpDown_Click( object sender, ImageClickEventArgs e )
        {
            try
            {
                if( lstTabs.SelectedIndex != - 1 )
                {
                    TabInfo objTab = (TabInfo)arrPortalTabs[lstTabs.SelectedIndex];
                    TabController objTabs = new TabController();

                    switch( ( (ImageButton)sender ).CommandName )
                    {
                        case "up":

                            objTabs.UpdatePortalTabOrder( PortalId, objTab.TabID, objTab.ParentId, 0, - 1, true, false );
                            break;
                        case "down":

                            objTabs.UpdatePortalTabOrder( PortalId, objTab.TabID, objTab.ParentId, 0, 1, true, false );
                            break;
                    }

                    // Redirect to this site to refresh
                    Response.Redirect( Globals.NavigateURL( TabId, "", "selecttabid", objTab.TabID.ToString() ), true );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// EditBtn_Click runs when the cmdEdit button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/9/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void EditBtn_Click( Object sender, ImageClickEventArgs e )
        {
            try
            {
                EditTab();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// ViewBtn_Click runs when the cmdView button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/9/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdView_Click( Object sender, ImageClickEventArgs e )
        {
            try
            {
                ViewTab();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl( "returntabid", TabId.ToString() ), false, SecurityAccessLevel.Admin, true, false );
                return actions;
            }
        }


    }
}