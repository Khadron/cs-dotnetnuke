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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.Utilities;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Tabs
{
    /// <summary>
    /// The RecycleBin PortalModuleBase allows Tabs and Modules to be recovered or
    /// prmanentl deleted
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/15/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class RecycleBin : PortalModuleBase
    {
        //Tabs
        protected SectionHeadControl dshTabs;

        //Modules
        protected SectionHeadControl dshModules;

        //tasks

        /// <summary>
        /// Loads deleted tabs and modules into the lists
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	18/08/2004
        ///   [VMasanas]  20/08/2004  Update display information for deleted modules to:
        ///               ModuleFriendlyName: ModuleTitle - Tab: TabName
        /// </history>
        private void BindData()
        {
            int intTab;
            ArrayList arrDeletedTabs = new ArrayList();
            TabController objTabs = new TabController();
            TabInfo objTab;

            ArrayList arrTabs = objTabs.GetTabs( PortalId );
            for( intTab = 0; intTab <= arrTabs.Count - 1; intTab++ )
            {
                objTab = (TabInfo)arrTabs[intTab];
                if( objTab.IsDeleted == true )
                {
                    arrDeletedTabs.Add( objTab );
                }
            }

            ModuleController objModules = new ModuleController();
            ModuleInfo objModule;
            int intModule;
            ArrayList arrDeletedModules = new ArrayList();

            ArrayList arrModules = objModules.GetModules( PortalId );
            for( intModule = 0; intModule <= arrModules.Count - 1; intModule++ )
            {
                objModule = (ModuleInfo)arrModules[intModule];
                if( objModule.IsDeleted == true )
                {
                    if( objModule.ModuleTitle == "" )
                    {
                        objModule.ModuleTitle = objModule.FriendlyName;
                    }
                    arrDeletedModules.Add( objModule );
                }
            }

            lstTabs.DataSource = arrDeletedTabs;
            lstTabs.DataBind();

            lstModules.DataSource = arrDeletedModules;
            lstModules.DataBind();

            cboTab.DataSource = Globals.GetPortalTabs(PortalSettings.DesktopTabs, -1, false, true, false, false, true);
            cboTab.DataBind();
        }

        /// <summary>
        /// Deletes a module
        /// </summary>
        /// <param name="intModuleId">ModuleId of the module to be deleted</param>
        /// <remarks>
        /// Adds a log entry for the action to the EvenLog
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	18/08/2004	Created
        /// </history>
        private void DeleteModule( int intModuleId )
        {
            EventLogController objEventLog = new EventLogController();

            // delete module
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModule( intModuleId, Null.NullInteger );
            if( objModule != null )
            {
                objModules.DeleteModule( objModule.ModuleID );
                objEventLog.AddLog( objModule, PortalSettings, UserId, "", EventLogController.EventLogType.MODULE_DELETED );
            }
        }

        /// <summary>
        /// Deletes a tab
        /// </summary>
        /// <param name="intTabid">TabId of the tab to be deleted</param>
        /// <remarks>
        /// Adds a log entry for the action to the EventLog
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	18/08/2004	Created
        ///                 19/09/2004  Remove skin deassignment. BLL takes care of this.
        ///                 30/09/2004  Change logic so log is only added when tab is actually deleted
        ///                 28/02/2005  Remove modules when deleting pages
        /// </history>
        private void DeleteTab( int intTabid )
        {
            EventLogController objEventLog = new EventLogController();

            // delete tab
            TabController objTabs = new TabController();
            ModuleController objModules = new ModuleController();

            TabInfo objTab = objTabs.GetTab( intTabid );
            if( objTab != null )
            {
                //save tab modules before deleting page
                ArrayList arrTabModules = objModules.GetPortalTabModules( objTab.PortalID, objTab.TabID );

                // hard delete the tab
                objTabs.DeleteTab( objTab.TabID, objTab.PortalID );

                // check if it's deleted
                TabInfo objTabDeleted = objTabs.GetTab( intTabid );
                if( objTabDeleted == null )
                {
                    //delete modules that do not have other instances
                    foreach( ModuleInfo objmodule in arrTabModules )
                    {
                        // check if all modules instances have been deleted
                        ModuleInfo objDelModule = objModules.GetModule( objmodule.ModuleID, Null.NullInteger );
                        if( objDelModule == null || objDelModule.TabID == Null.NullInteger )
                        {
                            objModules.DeleteModule( objmodule.ModuleID );
                        }
                    }
                    objEventLog.AddLog( objTab, PortalSettings, UserId, "", EventLogController.EventLogType.TAB_DELETED );
                }
                else
                {
                    // should be a parent tab
                    UI.Skins.Skin.AddModuleMessage( this, string.Format( Localization.GetString( "ParentTab.ErrorMessage", this.LocalResourceFile ), objTab.TabName ), ModuleMessage.ModuleMessageType.YellowWarning );
                }
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	18/08/2004	Add confirmation for Empty Recycle Bin button
        /// 	[cnurse]	15/09/2004	Localized Confirm text
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            string ResourceFileRoot = this.TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/" + this.ID;
            // If this is the first visit to the page
            if( Page.IsPostBack == false )
            {
                ClientAPI.AddButtonConfirm( cmdDeleteTab, Localization.GetString( "DeleteTab", ResourceFileRoot ) );
                ClientAPI.AddButtonConfirm( cmdDeleteModule, Localization.GetString( "DeleteModule", ResourceFileRoot ) );
                ClientAPI.AddButtonConfirm( cmdEmpty, Localization.GetString( "DeleteAll", ResourceFileRoot ) );

                BindData();
            }
        }

        /// <summary>
        /// Restores selected tabs in the listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Adds a log entry for each restored tab to the EventLog
        /// Redirects to same page after restoring so the menu can be refreshed with restored tabs.
        /// This will not restore deleted modules for selected tabs, only the tabs are restored.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	18/08/2004	Added support for multiselect listbox
        ///                 30/09/2004  Child tabs cannot be restored until their parent is restored first.
        ///                             Change logic so log is only added when tab is actually restored
        /// </history>
        protected void cmdRestoreTab_Click( Object sender, ImageClickEventArgs e )
        {
            ListItem item;
            bool errors = false;

            foreach( ListItem tempLoopVar_item in lstTabs.Items )
            {
                item = tempLoopVar_item;
                if( item.Selected )
                {
                    EventLogController objEventLog = new EventLogController();
                    TabController objTabs = new TabController();

                    TabInfo objTab = objTabs.GetTab( int.Parse( item.Value ) );
                    if( objTab != null )
                    {
                        if( Null.IsNull( objTab.ParentId ) && lstTabs.Items.FindByValue( objTab.ParentId.ToString() ) != null )
                        {
                            UI.Skins.Skin.AddModuleMessage( this, string.Format( Localization.GetString( "ChildTab.ErrorMessage", this.LocalResourceFile ), objTab.TabName ), ModuleMessage.ModuleMessageType.YellowWarning );
                            errors = true;
                        }
                        else
                        {
                            objTab.IsDeleted = false;
                            objTabs.UpdateTab( objTab );
                            objEventLog.AddLog( objTab, PortalSettings, UserId, "", EventLogController.EventLogType.TAB_RESTORED );

                            ModuleController objmodules = new ModuleController();
                            ArrayList arrMods = objmodules.GetAllTabsModules( objTab.PortalID, true );

                            foreach( ModuleInfo objModule in arrMods )
                            {
                                objmodules.CopyModule( objModule.ModuleID, objModule.TabID, objTab.TabID, "", true );
                            }
                        }
                    }
                }
            }
            if( ! errors )
            {
                Response.Redirect( Globals.NavigateURL() );
            }
            else
            {
                BindData();
            }
        }

        /// <summary>
        /// Deletes selected tabs in the listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Parent tabs will not be deleted. To delete a parent tab all child tabs need to be deleted before.
        /// Reloads data to refresh deleted modules and tabs listboxes
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	18/08/2004	Added support for multiselect listbox
        /// </history>
        protected void cmdDeleteTab_Click( Object sender, ImageClickEventArgs e )
        {
            ListItem item;

            foreach( ListItem tempLoopVar_item in lstTabs.Items )
            {
                item = tempLoopVar_item;
                if( item.Selected )
                {
                    DeleteTab( int.Parse( item.Value ) );
                }
            }
            BindData();
        }

        /// <summary>
        /// Restores selected modules in the listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Adds a log entry for each restored module to the EventLog
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	18/08/2004	Added support for multiselect listbox
        /// </history>
        protected void cmdRestoreModule_Click( Object sender, ImageClickEventArgs e )
        {
            EventLogController objEventLog = new EventLogController();
            ModuleController objModules = new ModuleController();
            ListItem item;

            if( cboTab.SelectedItem != null )
            {
                foreach( ListItem tempLoopVar_item in lstModules.Items )
                {
                    item = tempLoopVar_item;
                    if( item.Selected )
                    {
                        ModuleInfo objModule = objModules.GetModule( int.Parse( item.Value ), Null.NullInteger );
                        if( objModule != null )
                        {
                            objModule.IsDeleted = false;
                            objModule.TabID = Null.NullInteger;
                            objModules.UpdateModule( objModule );

                            // set defaults
                            objModule.CacheTime = 0;
                            objModule.Alignment = "";
                            objModule.Color = "";
                            objModule.Border = "";
                            objModule.IconFile = "";
                            objModule.Visibility = VisibilityState.Maximized;
                            objModule.ContainerSrc = "";
                            objModule.DisplayTitle = true;
                            objModule.DisplayPrint = true;
                            objModule.DisplaySyndicate = false;
                            objModule.AllTabs = false;

                            // get default module settings
                            Hashtable settings = PortalSettings.GetSiteSettings( PortalId );
                            if( Convert.ToString( settings["defaultmoduleid"] ) != "" && Convert.ToString( settings["defaulttabid"] ) != "" )
                            {
                                ModuleInfo objDefaultModule = objModules.GetModule( int.Parse( Convert.ToString( settings["defaultmoduleid"] ) ), int.Parse( Convert.ToString( settings["defaulttabid"] ) ) );
                                if( objDefaultModule != null )
                                {
                                    objModule.CacheTime = objDefaultModule.CacheTime;
                                    objModule.Alignment = objDefaultModule.Alignment;
                                    objModule.Color = objDefaultModule.Color;
                                    objModule.Border = objDefaultModule.Border;
                                    objModule.IconFile = objDefaultModule.IconFile;
                                    objModule.Visibility = objDefaultModule.Visibility;
                                    objModule.ContainerSrc = objDefaultModule.ContainerSrc;
                                    objModule.DisplayTitle = objDefaultModule.DisplayTitle;
                                    objModule.DisplayPrint = objDefaultModule.DisplayPrint;
                                    objModule.DisplaySyndicate = objDefaultModule.DisplaySyndicate;
                                }
                            }

                            // add tab module
                            objModule.TabID = int.Parse( cboTab.SelectedItem.Value );
                            objModule.PaneName = Globals.glbDefaultPane;
                            objModule.ModuleOrder = - 1;
                            objModules.AddModule( objModule );

                            objEventLog.AddLog( objModule, PortalSettings, UserId, "", EventLogController.EventLogType.MODULE_RESTORED );
                        }
                    }
                }
                BindData();
            }
        }

        /// <summary>
        /// Deletes selected modules in the listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	18/08/2004	Added support for multiselect listbox
        /// </history>
        protected void cmdDeleteModule_Click( Object sender, ImageClickEventArgs e )
        {
            ListItem item;

            foreach( ListItem tempLoopVar_item in lstModules.Items )
            {
                item = tempLoopVar_item;
                if( item.Selected )
                {
                    DeleteModule( int.Parse( item.Value ) );
                }
            }
            BindData();
        }

        /// <summary>
        /// Permanently removes all deleted tabs and modules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Parent tabs will not be deleted. To delete a parent tab all child tabs need to be deleted before.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	18/08/2004	Created
        /// </history>
        protected void cmdEmpty_Click( Object sender, EventArgs e )
        {
            ListItem item;

            foreach( ListItem tempLoopVar_item in lstTabs.Items )
            {
                item = tempLoopVar_item;
                DeleteTab( int.Parse( item.Value ) );
            }
            foreach( ListItem tempLoopVar_item in lstModules.Items )
            {
                item = tempLoopVar_item;
                DeleteModule( int.Parse( item.Value ) );
            }
            BindData();
        }

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }

        private void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
    }
}