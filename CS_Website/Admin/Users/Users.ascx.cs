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
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Profile;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Users
{
    /// <summary>
    /// The Users PortalModuleBase is used to manage the Registered Users of a portal
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    ///     [cnurse]    02/16/2006  Updated to reflect custom profile definitions
    /// </history>
    public partial class UserAccounts : PortalModuleBase, IActionable
    {
        private string _Filter = "";
        private int _CurrentPage = 1;
        private ArrayList _Users = new ArrayList();

        protected int TotalPages = - 1;
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

        /// <summary>
        /// Gets whether we are dealing with SuperUsers
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/02/2006  Created
        /// </history>
        protected bool IsSuperUser
        {
            get
            {
                if( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the Page Size for the Grid
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/02/2006  Created
        /// </history>
        protected int PageSize
        {
            get
            {
                object setting = UserModuleBase.GetSetting(UsersPortalId, "Records_PerPage");
                return System.Convert.ToInt32(setting);
            }
        }

        /// <summary>
        /// Gets a flag that determines whether to suppress the Pager (when not required)
        /// </summary>
        /// <history>
        /// 	[cnurse]	08/10/2006  Created
        /// </history>
        protected bool SuppressPager
        {
            get
            {
                object setting = UserModuleBase.GetSetting(UsersPortalId, "Display_SuppressPager");
                return System.Convert.ToBoolean(setting);
            }
        }

        /// <summary>
        /// Gets the Portal Id whose Users we are managing
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/02/2006  Created
        /// </history>
        protected int UsersPortalId
        {
            get
            {
                int intPortalId = PortalId;
                if( IsSuperUser )
                {
                    intPortalId = Null.NullInteger;
                }
                return intPortalId;
            }
        }

        protected ArrayList Users
        {
            get
            {
                return _Users;
            }
            set
            {
                _Users = value;
            }
        }

        private ListItem AddSearchItem(string name)
        {
            string propertyName = Null.NullString;
            if (Request.QueryString["filterProperty"] != null)
            {
                propertyName = Request.QueryString["filterProperty"];
            }

            string text = Localization.GetString(name, this.LocalResourceFile);
            if (text == "")
            {
                text = name;
            }
            ListItem li = new ListItem(text, name);
            if (name == propertyName)
            {
                li.Selected = true;
            }
            return li;
        }

        /// <summary>
        /// BindData gets the users from the Database and binds them to the DataGrid
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void BindData()
        {
            BindData( null, null );
        }

        /// <summary>
        /// BindData gets the users from the Database and binds them to the DataGrid
        /// </summary>
        /// <param name="SearchText">Text to Search</param>
        /// <param name="SearchField">Field to Search</param>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void BindData(string SearchText, string SearchField)
        {

            CreateLetterSearch();

            string strQuerystring = Null.NullString;

            // Get the list of registered users from the database
            if (SearchText == Localization.GetString("OnLine"))
            {
                Filter = "";
            }
            else if (SearchText == Localization.GetString("Unauthorized"))
            {
                Filter = "";
            }
            else
            {
                Filter = SearchText;
            }

            if (Filter != "")
            {
                strQuerystring += "filter=" + Filter;
            }

            if (Filter == "")
            {
                if (SearchText == Localization.GetString("Unauthorized"))
                {
                    Users = UserController.GetUnAuthorizedUsers(UsersPortalId, false);
                }
                else if (SearchText == Localization.GetString("OnLine"))
                {
                    Users = UserController.GetOnlineUsers(UsersPortalId);
                }
                // Hide pagingcontrol while diplaying UnAuthorized/Online members, since they are not used here
                ctlPagingControl.Visible = false;
            }
            else if (Filter == Localization.GetString("All"))
            {
                Users = UserController.GetUsers(UsersPortalId, false, CurrentPage - 1, PageSize, ref TotalRecords);
            }
            else if (Filter != "None")
            {
                switch (SearchField)
                {
                    case "Email":
                        Users = UserController.GetUsersByEmail(UsersPortalId, false, Filter + "%", CurrentPage - 1, PageSize, ref TotalRecords);
                        break;
                    case "Username":
                        Users = UserController.GetUsersByUserName(UsersPortalId, false, Filter + "%", CurrentPage - 1, PageSize, ref TotalRecords);
                        break;
                    default:
                        string propertyName = ddlSearchType.SelectedItem.Value;
                        Users = UserController.GetUsersByProfileProperty(UsersPortalId, false, propertyName, Filter + "%", CurrentPage - 1, PageSize, ref TotalRecords);
                        strQuerystring += "&filterProperty=" + propertyName;
                        break;
                }
            }
            if (SuppressPager & ctlPagingControl.Visible)
            {
                ctlPagingControl.Visible = (PageSize < TotalRecords);
            }

            grdUsers.DataSource = Users;
            grdUsers.DataBind();

            ctlPagingControl.TotalRecords = TotalRecords;
            ctlPagingControl.PageSize = PageSize;
            ctlPagingControl.CurrentPage = CurrentPage;

            ctlPagingControl.QuerystringParams = strQuerystring;
            ctlPagingControl.TabID = TabId;
        }

        /// <summary>
        /// Builds the letter filter
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void CreateLetterSearch()
        {
            string filters = Localization.GetString( "Filter.Text", this.LocalResourceFile );

            filters += "," + Localization.GetString( "All" );
            filters += "," + Localization.GetString( "OnLine" );
            filters += "," + Localization.GetString( "Unauthorized" );

            string[] strAlphabet = filters.Split( ',' );
            rptLetterSearch.DataSource = strAlphabet;
            rptLetterSearch.DataBind();
        }

        /// <summary>
        /// Deletes all unauthorized users
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/02/2006	Created
        /// </history>
        private void DeleteUnAuthorizedUsers()
        {
            try
            {
                UserController.DeleteUnauthorizedUsers( PortalId );

                BindData();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// DisplayAddress correctly formats an Address
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string DisplayAddress(object Unit, object Street, object City, object Region, object Country, object PostalCode)
{
            string _Address = Null.NullString;
            try
            {
                _Address = Globals.FormatAddress(Unit, Street, City, Region, Country, PostalCode);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return _Address;
        }

        /// <summary>
        /// DisplayEmail correctly formats an Email Address
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string DisplayEmail(string Email)
        {
            string _Email = Null.NullString;
            try
            {
                if (Email != null)
                {
                    _Email = HtmlUtils.FormatEmail(Email);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return _Email;
        }

        /// <summary>
        /// DisplayDate correctly formats the Date
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string DisplayDate(System.DateTime UserDate)
        {
            string _Date = Null.NullString;
            try
            {
                if (!(Null.IsNull(UserDate)))
                {
                    _Date = UserDate.ToString();
                }
                else
                {
                    _Date = "";
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return _Date;
        }

        /// <summary>
        /// FormatURL correctly formats the Url for the Edit User Link
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected string FormatURL(string strKeyName, string strKeyValue)
        {
            string _URL = Null.NullString;
            try
            {
                if (Filter != "")
                {
                    _URL = EditUrl(strKeyName, strKeyValue, "", "filter=" + Filter);
                }
                else
                {
                    _URL = EditUrl(strKeyName, strKeyValue);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return _URL;
        }

        /// <summary>
        /// FilterURL correctly formats the Url for filter by first letter and paging
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected string FilterURL(string Filter, string CurrentPage)
        {
            string _URL = Null.NullString;
            if (Filter != "")
            {
                if (CurrentPage != "")
                {
                    _URL = Common.Globals.NavigateURL(TabId, "", "filter=" + Filter, "currentpage=" + CurrentPage);
                }
                else
                {
                    _URL = Common.Globals.NavigateURL(TabId, "", "filter=" + Filter);
                }
            }
            else
            {
                if (CurrentPage != "")
                {
                    _URL = Common.Globals.NavigateURL(TabId, "", "currentpage=" + CurrentPage);
                }
                else
                {
                    _URL = Common.Globals.NavigateURL(TabId, "");
                }
            }
            return _URL;

        }

        /// <summary>
        /// Page_Init runs when the control is initialised
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Init( Object sender, EventArgs e )
        {
            foreach( DataGridColumn column in grdUsers.Columns )
            {
                bool isVisible;
                string header = column.HeaderText;
                if( header == "" || header.ToLower() == "username" )
                {
                    isVisible = true;
                }
                else
                {
                    string settingKey = "Column_" + header;                    
                    object setting = UserModuleBase.GetSetting(UsersPortalId, settingKey);
                    isVisible = Convert.ToBoolean( setting );
                }

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
                        string formatString = EditUrl( "UserId", "KEYFIELD", "Edit" );
                        formatString = formatString.Replace( "KEYFIELD", "{0}" );
                        imageColumn.NavigateURLFormatString = formatString;
                    }
                    //Manage Roles Column NavigateURLFormatString
                    if( imageColumn.CommandName == "UserRoles" )
                    {
                        if( IsHostMenu )
                        {
                            isVisible = false;
                        }
                        else
                        {
                            //The Friendly URL parser does not like non-alphanumeric characters
                            //so first create the format string with a dummy value and then
                            //replace the dummy value with the FormatString place holder
                            string formatString = Globals.NavigateURL( TabId, "User Roles", "UserId=KEYFIELD" );
                            formatString = formatString.Replace( "KEYFIELD", "{0}" );
                            imageColumn.NavigateURLFormatString = formatString;
                        }
                    }

                    //Localize Image Column Text
                    if( !String.IsNullOrEmpty( imageColumn.CommandName ))
                    {
                        imageColumn.Text = Localization.GetString( imageColumn.CommandName, this.LocalResourceFile );
                    }
                }

                column.Visible = isVisible;
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                //Add an Action Event Handler to the Skin
                AddActionHandler( new ActionEventHandler( ModuleAction_Click ) );

                if( Request.QueryString["CurrentPage"] != null )
                {
                    CurrentPage = Convert.ToInt32( Request.QueryString["CurrentPage"] );
                }

                if( Request.QueryString["filter"] != null )
                {
                    Filter = Request.QueryString["filter"];
                }

                if( Filter == "" )
                {
                    //Get Default View
                    object setting = UserModuleBase.GetSetting(UsersPortalId, "Display_Mode");
                    DisplayMode mode = (DisplayMode)Enum.Parse(typeof(DisplayMode), (string)setting);
                    switch( mode )
                    {
                        case DisplayMode.All:

                            Filter = Localization.GetString( "All" );
                            break;
                        case DisplayMode.FirstLetter:

                            Filter = Localization.GetString( "Filter.Text", this.LocalResourceFile ).Substring( 0, 1 );
                            break;
                        case DisplayMode.None:

                            Filter = "None";
                            break;
                    }
                }

                if (!Page.IsPostBack)
                {
                    //Load the Search Combo
                    ddlSearchType.Items.Add(AddSearchItem("Username"));
                    ddlSearchType.Items.Add(AddSearchItem("Email"));
                    ProfilePropertyDefinitionCollection profileProperties = ProfileController.GetPropertyDefinitionsByPortal(PortalId);
                    foreach (ProfilePropertyDefinition definition in profileProperties)
                    {
                        ddlSearchType.Items.Add(AddSearchItem(definition.PropertyName));
                    }

                    //Localize the Headers
                    Localization.LocalizeDataGrid(ref grdUsers, this.LocalResourceFile);
                    BindData(Filter, ddlSearchType.SelectedItem.Value);
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// ModuleAction_Click handles all ModuleAction events raised from the skin
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="sender"> The object that triggers the event</param>
        /// <param name="e">An ActionEventArgs object</param>
        /// <history>
        /// 	[cnurse]	03/02/2006	Created
        /// </history>
        private void ModuleAction_Click( object sender, ActionEventArgs e )
        {
            switch( e.Action.CommandArgument )
            {
                case "Delete":

                    DeleteUnAuthorizedUsers();
                    break;
            }
        }

        /// <summary>
        /// btnSearch_Click runs when the user searches for accounts by username or email
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[dancaron]	10/28/2004	Intial Version
        /// </history>
        protected void btnSearch_Click( Object sender, ImageClickEventArgs e )
        {
            CurrentPage = 1;
            BindData( txtSearch.Text, ddlSearchType.SelectedItem.Value );
        }

        protected void grdUsers_DeleteCommand( object source, DataGridCommandEventArgs e )
        {
            try
            {
                int userId = int.Parse( e.CommandArgument.ToString() );

                UserInfo user = UserController.GetUser( PortalId, userId, false );

                if( user != null )
                {
                    if( UserController.DeleteUser( ref user, true, false ) )
                    {
                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "UserDeleted", this.LocalResourceFile ), ModuleMessageType.GreenSuccess );
                    }
                    else
                    {
                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "UserDeleteError", this.LocalResourceFile ), ModuleMessageType.RedError );
                    }
                }

                BindData();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void grdUsers_ItemDataBound( object sender, DataGridItemEventArgs e )
        {
            DataGridItem item = e.Item;

            if( item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.SelectedItem )
            {
                Control imgColumnControl = item.Controls[1].Controls[0];
                if( imgColumnControl is ImageButton )
                {
                    ImageButton delImage = (ImageButton)imgColumnControl;
                    UserInfo user = (UserInfo)item.DataItem;

                    delImage.Visible = !( user.UserID == PortalSettings.AdministratorId );
                }

                imgColumnControl = item.Controls[3].FindControl( "imgOnline" );
                if( imgColumnControl is Image )
                {
                    Image userOnlineImage = (Image)imgColumnControl;
                    UserInfo user = (UserInfo)item.DataItem;

                    userOnlineImage.Visible = user.Membership.IsOnLine;
                }
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "add.gif", EditUrl(), false, SecurityAccessLevel.Admin, true, false );
                if( ! IsSuperUser )
                {
                    actions.Add( GetNextActionID(), Localization.GetString( "DeleteUnAuthorized.Action", LocalResourceFile ), ModuleActionType.AddContent, "Delete", "delete.gif", "", "confirm('" + ClientAPI.GetSafeJSString( Localization.GetString( "DeleteItems.Confirm" ) ) + "')", true, SecurityAccessLevel.Admin, true, false );
                }
                if( ProfileProviderConfig.CanEditProviderProperties )
                {
                    actions.Add( GetNextActionID(), Localization.GetString( "ManageProfile.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "icon_profile_16px.gif", EditUrl( "ManageProfile" ), false, SecurityAccessLevel.Admin, true, false );
                }
                actions.Add( GetNextActionID(), Localization.GetString( "UserSettings.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "settings.gif", EditUrl( "UserSettings" ), false, SecurityAccessLevel.Admin, true, false );
                return actions;
            }
        }
    }
}