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
                object setting = UserModuleBase.GetSetting( PortalId, "Records_PerPage" );
                return Convert.ToInt32( setting );
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
        /// <remarks>
        /// </remarks>
        /// <param name="SearchText">Text to Search</param>
        /// <param name="SearchField">Field to Search</param>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void BindData( string SearchText, string SearchField )
        {
            CreateLetterSearch();

            // Get the list of registered users from the database
            if( SearchText == Localization.GetString( "OnLine" ) )
            {
                Filter = "";
            }
            else if( SearchText == Localization.GetString( "Unauthorized" ) )
            {
                Filter = "";
            }
            else
            {
                Filter = SearchText;
            }

            if( Filter == "" )
            {
                if( SearchText == Localization.GetString( "Unauthorized" ) )
                {
                    Users = UserController.GetUnAuthorizedUsers( UsersPortalId, false );
                }
                else if( SearchText == Localization.GetString( "OnLine" ) )
                {
                    Users = UserController.GetOnlineUsers( UsersPortalId );
                }
                // Hide pagingcontrol while diplaying UnAuthorized/Online members, since they are not used here
                ctlPagingControl.Visible = false;
            }
            else if( Filter == Localization.GetString( "All" ) )
            {
                Users = UserController.GetUsers( UsersPortalId, false, CurrentPage - 1, PageSize, ref TotalRecords );
            }
            else if( Filter != "None" )
            {
                switch( SearchField )
                {
                    case "email":

                        Users = UserController.GetUsersByEmail( UsersPortalId, false, Filter + "%", CurrentPage - 1, PageSize, ref TotalRecords );
                        break;
                    case "username":

                        Users = UserController.GetUsersByUserName( UsersPortalId, false, Filter + "%", CurrentPage - 1, PageSize, ref TotalRecords );
                        break;
                    default:

                        string propertyName = ddlSearchType.SelectedItem.Value;
                        Users = UserController.GetUsersByProfileProperty( UsersPortalId, false, propertyName, Filter + "%", CurrentPage - 1, PageSize, ref TotalRecords );
                        break;
                }
            }

            grdUsers.DataSource = Users;
            grdUsers.DataBind();

            ctlPagingControl.TotalRecords = TotalRecords;
            ctlPagingControl.PageSize = PageSize;
            ctlPagingControl.CurrentPage = CurrentPage;

            string strQuerystring;
            strQuerystring = "PageRecords=" + PageSize.ToString();
            if( Filter != "" )
            {
                strQuerystring += "&filter=" + Filter;
            }
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
        public string DisplayAddress( object Unit, object Street, object City, object Region, object Country, object PostalCode )
        {
            string returnValue = String.Empty;
            try
            {
                returnValue = Globals.FormatAddress( Unit, Street, City, Region, Country, PostalCode );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return returnValue;
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
        public string DisplayEmail( string Email )
        {
            try
            {
                string _Email = Null.NullString;
                if( Email != null )
                {
                    _Email = HtmlUtils.FormatEmail( Email );
                }
                return _Email;
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return String.Empty;
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
        public string DisplayDate( DateTime UserDate )
        {
            string returnValue = String.Empty;
            try
            {
                if( ! Null.IsNull( UserDate ) )
                {
                    returnValue = UserDate.ToString();
                }
                else
                {
                    returnValue = "";
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return returnValue;
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
        protected string FormatURL( string strKeyName, string strKeyValue )
        {
            string returnValue = String.Empty;
            try
            {
                if( Filter != "" )
                {
                    returnValue = EditUrl( strKeyName, strKeyValue, "", "filter=" + Filter );
                }
                else
                {
                    returnValue = EditUrl( strKeyName, strKeyValue );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return returnValue;
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
        protected string FilterURL( string filter, string currentPage )
        {
            if( filter != "" )
            {
                if( currentPage != "" )
                {
                    return Globals.NavigateURL( TabId, "", "filter=" + filter, "currentpage=" + currentPage, "PageRecords=" + PageSize );
                }
                else
                {
                    return Globals.NavigateURL( TabId, "", "filter=" + filter, "PageRecords=" + PageSize );
                }
            }
            else
            {
                if( currentPage != "" )
                {
                    return Globals.NavigateURL( TabId, "", "currentpage=" + currentPage, "PageRecords=" + PageSize );
                }
                else
                {
                    return Globals.NavigateURL( TabId, "", "PageRecords=" + PageSize );
                }
            }
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
                    object setting = UserModuleBase.GetSetting( PortalId, settingKey );
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
                    if( imageColumn.CommandName != "" )
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
                    object setting = UserModuleBase.GetSetting( PortalId, "Display_Mode" );
                    DisplayMode mode = (DisplayMode)setting;
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

                if( ! Page.IsPostBack )
                {
                    //Localize the Headers
                    Localization.LocalizeDataGrid( ref grdUsers, this.LocalResourceFile );
                    BindData( Filter, "username" );

                    //Load the Search Combo
                    ddlSearchType.Items.Add( new ListItem( Localization.GetString( "Username.Header", this.LocalResourceFile ), "username" ) );
                    ddlSearchType.Items.Add( new ListItem( Localization.GetString( "Email.Header", this.LocalResourceFile ), "email" ) );
                    ProfilePropertyDefinitionCollection profileProperties = ProfileController.GetPropertyDefinitionsByPortal( PortalId );
                    foreach( ProfilePropertyDefinition definition in profileProperties )
                    {
                        string text = Localization.GetString( definition.PropertyName, this.LocalResourceFile );
                        if( text == "" )
                        {
                            text = definition.PropertyName;
                        }
                        ddlSearchType.Items.Add( new ListItem( text, definition.PropertyName ) );
                    }
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
                DataGridItem item = e.Item;
                int userId = int.Parse( e.CommandArgument.ToString() );

                UserInfo user = UserController.GetUser( PortalId, userId, false );

                if( user != null )
                {
                    if( UserController.DeleteUser( ref user, true, false ) )
                    {
                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "UserDeleted", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.GreenSuccess );
                    }
                    else
                    {
                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "UserDeleteError", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.RedError );
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
                ModuleActionCollection Actions = new ModuleActionCollection();
                Actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "add.gif", EditUrl(), false, SecurityAccessLevel.Admin, true, false );
                if( ! IsSuperUser )
                {
                    Actions.Add( GetNextActionID(), Localization.GetString( "DeleteUnAuthorized.Action", LocalResourceFile ), ModuleActionType.AddContent, "Delete", "delete.gif", "", "confirm(\'" + ClientAPI.GetSafeJSString( Localization.GetString( "DeleteItems.Confirm" ) ) + "\')", true, SecurityAccessLevel.Admin, true, false );
                }
                if( ProfileProviderConfig.CanEditProviderProperties )
                {
                    Actions.Add( GetNextActionID(), Localization.GetString( "ManageProfile.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "icon_profile_16px.gif", EditUrl( "ManageProfile" ), false, SecurityAccessLevel.Admin, true, false );
                }
                Actions.Add( GetNextActionID(), Localization.GetString( "UserSettings.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "settings.gif", EditUrl( "UserSettings" ), false, SecurityAccessLevel.Admin, true, false );
                return Actions;
            }
        }
    }
}