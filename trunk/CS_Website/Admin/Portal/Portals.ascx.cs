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
using System.Text;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Portals
{
    /// <summary>
    /// The Portals PortalModuleBase is used to manage the portlas.
    /// </summary>
    /// <history>
    /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class Portals : PortalModuleBase, IActionable
    {
        private string _Filter = "";
        private int _CurrentPage = 1;
        private ArrayList _Portals = new ArrayList();
        protected int TotalPages = -1;
        protected int TotalRecords;

        protected int CurrentPage
        {
            get
            {
                return _CurrentPage;
            }
            set
            {
                _CurrentPage = value;
            }
        }

        protected string Filter
        {
            get
            {
                return _Filter;
            }
            set
            {
                _Filter = value;
            }
        }

        protected ArrayList PortalList
        {
            get
            {
                return _Portals;
            }
            set
            {
                _Portals = value;
            }
        }

        /// <summary>
        /// Gets the Page Size for the Grid
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/02/2006  Created
        /// </history>
        protected static int PageSize
        {
            get
            {
                //Dim setting As Object = UserModuleBase.GetSetting(UsersPortalId, "Records_PerPage")
                //Return CType(setting, Integer)
                return 20;
            }
        }

        /// <summary>
        /// Gets a flag that determines whether to suppress the Pager (when not required)
        /// </summary>
        /// <history>
        /// 	[cnurse]	08/10/2006  Created
        /// </history>
        protected static bool SuppressPager
        {
            get
            {
                //Dim setting As Object = UserModuleBase.GetSetting(UsersPortalId, "Display_SuppressPager")
                //Return CType(setting, Boolean)
                return true;
            }
        }

        /// <summary>
        /// BindData fetches the data from the database and updates the controls
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void BindData()
        {
            CreateLetterSearch();
            string strQuerystring = Null.NullString;
            if( Filter != "" )
            {
                strQuerystring += "filter=" + Filter;
            }

            if( Filter == Localization.GetString( "Expired", LocalResourceFile ) )
            {
                PortalList = PortalController.GetExpiredPortals();
                ctlPagingControl.Visible = false;
            }
            else
            {
                PortalList = PortalController.GetPortalsByName( Filter + "%", CurrentPage - 1, PageSize, TotalRecords );
            }

            grdPortals.DataSource = PortalList;
            grdPortals.DataBind();

            ctlPagingControl.TotalRecords = TotalRecords;
            ctlPagingControl.PageSize = PageSize;
            ctlPagingControl.CurrentPage = CurrentPage;

            ctlPagingControl.QuerystringParams = strQuerystring;
            ctlPagingControl.TabID = TabId;

            if( SuppressPager & ctlPagingControl.Visible )
            {
                ctlPagingControl.Visible = ( PageSize < TotalRecords );
            }
        }

        /// <summary>
        /// Builds the letter filter
        /// </summary>
        /// <history>
        /// 	[cnurse]	11/17/2006	Created
        /// </history>
        private void CreateLetterSearch()
        {
            string filters = Localization.GetString( "Filter.Text", LocalResourceFile );

            filters += "," + Localization.GetString( "All" );
            filters += "," + Localization.GetString( "Expired", LocalResourceFile );

            string[] strAlphabet = filters.Split( ',' );
            rptLetterSearch.DataSource = strAlphabet;
            rptLetterSearch.DataBind();
        }

        /// <summary>
        /// Deletes all expired portals
        /// </summary>
        /// <history>
        /// 	[cnurse]	11/17/2006	Created
        /// </history>
        private void DeleteExpiredPortals()
        {
            try
            {
                PortalController.DeleteExpiredPortals( Globals.GetAbsoluteServerPath( Request ) );

                BindData();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// FilterURL correctly formats the Url for filter by first letter and paging
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected string FilterURL( string filter, string currentPage )
        {
            string _URL;
            if( filter != "" )
            {
                if( currentPage != "" )
                {
                    _URL = Globals.NavigateURL( TabId, "", "filter=" + filter, "currentpage=" + currentPage );
                }
                else
                {
                    _URL = Globals.NavigateURL( TabId, "", "filter=" + filter );
                }
            }
            else
            {
                if( currentPage != "" )
                {
                    _URL = Globals.NavigateURL( TabId, "", "currentpage=" + currentPage );
                }
                else
                {
                    _URL = Globals.NavigateURL( TabId, "" );
                }
            }
            return _URL;
        }

        /// <summary>
        /// FormatExpiryDate formats the expiry date and filter out null-dates
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatExpiryDate( DateTime DateTime )
        {
            string returnValue = String.Empty;
            try
            {
                if( ! Null.IsNull( DateTime ) )
                {
                    returnValue = DateTime.ToShortDateString();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return returnValue;
        }

        /// <summary>
        /// FormatExpiryDate formats the format name as an <a> tag
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatPortalAliases( int PortalID )
        {
            StringBuilder str = new StringBuilder();
            try
            {
                PortalAliasController objPortalAliasController = new PortalAliasController();
                ArrayList arr = objPortalAliasController.GetPortalAliasArrayByPortalID( PortalID );

                for( int i = 0; i < arr.Count; i++ )
                {
                    PortalAliasInfo objPortalAliasInfo = (PortalAliasInfo)arr[i];
                    str.Append( "<a href=\"" + Globals.AddHTTP( objPortalAliasInfo.HTTPAlias ) + "\">" + objPortalAliasInfo.HTTPAlias + "</a>" + "<BR>" );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return str.ToString();
        }

        protected void Page_Init( object sender, EventArgs e )
        {
            foreach( DataGridColumn column in grdPortals.Columns )
            {
                if( column.GetType() == typeof( ImageCommandColumn ) )
                {
                    //Manage Delete Confirm JS
                    ImageCommandColumn imageColumn = (ImageCommandColumn)column;
                    if( imageColumn.CommandName == "Delete" )
                    {
                        imageColumn.OnClickJS = Localization.GetString( "DeleteItem" );
                    }
                    //Manage Edit Column NavigateURLFormatString
                    if( imageColumn.CommandName == "Edit" )
                    {
                        //The Friendly URL parser does not like non-alphanumeric characters
                        //so first create the format string with a dummy value and then
                        //replace the dummy value with the FormatString place holder
                        TabController objTabs = new TabController();
                        TabInfo objTab = objTabs.GetTabByName( "Site Settings", PortalSettings.PortalId, PortalSettings.AdminTabId );
                        string formatString = Globals.NavigateURL( objTab.TabID, "", "pid=KEYFIELD" );
                        formatString = formatString.Replace( "KEYFIELD", "{0}" );
                        imageColumn.NavigateURLFormatString = formatString;
                    }
                    //Localize Image Column Text
                    if( imageColumn.CommandName != "" )
                    {
                        imageColumn.Text = Localization.GetString( imageColumn.CommandName, LocalResourceFile );
                    }
                }
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        ///     [VMasanas]  9/28/2004   Changed redirect to Access Denied
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                AddActionHandler( ModuleAction_Click );

                // Verify that the current user has access to access this page
                if( !UserInfo.IsSuperUser )
                {
                    Response.Redirect( Globals.NavigateURL( "Access Denied" ), true );
                }

                if( Request.QueryString["CurrentPage"] != null )
                {
                    CurrentPage = Convert.ToInt32( Request.QueryString["CurrentPage"] );
                }

                if( Request.QueryString["filter"] != null )
                {
                    Filter = Request.QueryString["filter"];
                }

                if( Filter == Localization.GetString( "All" ) )
                {
                    Filter = "";
                }

                if( !Page.IsPostBack )
                {
                    //Localize the Headers
                    Localization.LocalizeDataGrid( ref grdPortals, LocalResourceFile );
                    BindData();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ModuleAction_Click handles all ModuleAction events raised from the skin
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="sender"> The object that triggers the event</param>
        /// <param name="e">An ActionEventArgs object</param>
        /// <history>
        /// 	[cnurse]	11/17/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void ModuleAction_Click( object sender, ActionEventArgs e )
        {
            switch( e.Action.CommandArgument )
            {
                case "Delete":
                    DeleteExpiredPortals();
                    break;
            }
        }

        protected void grdPortals_DeleteCommand( object source, DataGridCommandEventArgs e )
        {
            try
            {
                PortalController objPortalController = new PortalController();
                PortalInfo portal = objPortalController.GetPortal( Int32.Parse( e.CommandArgument.ToString() ) );

                if( portal != null )
                {
                    string strMessage = PortalController.DeletePortal( portal, Globals.GetAbsoluteServerPath( Request ) );
                    if( string.IsNullOrEmpty( strMessage ) )
                    {
                        EventLogController objEventLog = new EventLogController();
                        objEventLog.AddLog( "PortalName", portal.PortalName, PortalSettings, UserId, EventLogController.EventLogType.PORTAL_DELETED );
                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "PortalDeleted", LocalResourceFile ), ModuleMessageType.GreenSuccess );
                    }
                    else
                    {
                        UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessageType.RedError );
                    }
                }

                BindData();
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
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl( "Signup" ), false, SecurityAccessLevel.Host, true, false );
                actions.Add( GetNextActionID(), Localization.GetString( "ExportTemplate.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "lt.gif", EditUrl( "Template" ), false, SecurityAccessLevel.Admin, true, false );
                actions.Add( GetNextActionID(), Localization.GetString( "DeleteExpired.Action", LocalResourceFile ), ModuleActionType.AddContent, "Delete", "delete.gif", "", "confirm('" + ClientAPI.GetSafeJSString( Localization.GetString( "DeleteItems.Confirm" ) ) + "')", true, SecurityAccessLevel.Admin, true, false );
                return actions;
            }
        }
    }
}