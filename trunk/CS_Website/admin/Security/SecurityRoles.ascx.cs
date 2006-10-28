using System;
using System.Collections;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using Microsoft.VisualBasic;
using Calendar=DotNetNuke.Common.Utilities.Calendar;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Security
{
    /// <summary>
    /// The SecurityRoles PortalModuleBase is used to manage the users and roles they
    /// have
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class SecurityRoles : PortalModuleBase, IActionable
    {
        private PortalModuleBase _ParentModule;

        private int roleId = -1;
        private int userId = -1;

        /// <summary>
        /// Gets the Return Url for the page
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/14/2006  Created
        /// </history>
        protected string ReturnUrl
        {
            get
            {
                return Globals.NavigateURL( TabId );
            }
        }

        /// <summary>
        /// Gets the control should use a Combo Box or Text Box to display the users
        /// </summary>
        /// <history>
        /// 	[cnurse]	05/01/2006  Created
        /// </history>
        protected UsersControl UsersControl
        {
            get
            {
                object setting = UserModuleBase.GetSetting( PortalId, "Security_UsersControl" );
                return ( (UsersControl)setting );
            }
        }

        /// <summary>
        /// Gets and sets the ParentModule (if one exists)
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/10/2006  Created
        /// </history>
        public PortalModuleBase ParentModule
        {
            get
            {
                return _ParentModule;
            }
            set
            {
                _ParentModule = value;
            }
        }

        /// <summary>
        /// BindData loads the controls from the Database
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void BindData()
        {
            RoleController objRoles = new RoleController();

            // bind all portal roles to dropdownlist
            if( roleId == - 1 )
            {
                if( cboRoles.Items.Count == 0 )
                {
                    cboRoles.DataSource = objRoles.GetPortalRoles( PortalId );
                    cboRoles.DataBind();
                }
            }
            else
            {
                if( ! Page.IsPostBack )
                {
                    RoleInfo objRole = objRoles.GetRole( roleId, PortalId );
                    if( objRole != null )
                    {
                        cboRoles.Items.Add( new ListItem( objRole.RoleName, objRole.RoleID.ToString() ) );
                        cboRoles.Items[0].Selected = true;
                        lblTitle.Text = string.Format( Localization.GetString( "RoleTitle.Text", LocalResourceFile ), objRole.RoleName, objRole.RoleID.ToString() );
                    }
                    cmdAdd.Text = string.Format( Localization.GetString( "AddUser.Text", LocalResourceFile ), objRole.RoleName );
                    cboRoles.Visible = false;
                    plRoles.Visible = false;
                }
            }

            // bind all portal users to dropdownlist
            if( userId == - 1 )
            {
                if( UsersControl == UsersControl.Combo )
                {
                    if( cboUsers.Items.Count == 0 )
                    {
                        cboUsers.DataSource = UserController.GetUsers( PortalId, false );
                        cboUsers.DataBind();
                    }
                    txtUsers.Visible = false;
                    cboUsers.Visible = true;
                    cmdValidate.Visible = false;
                }
                else
                {
                    txtUsers.Visible = true;
                    cboUsers.Visible = false;
                    cmdValidate.Visible = true;
                }
            }
            else
            {
                UserInfo objUser = UserController.GetUser( PortalId, userId, false );
                if( objUser != null )
                {
                    txtUsers.Text = objUser.UserID.ToString();
                    lblTitle.Text = string.Format( Localization.GetString( "UserTitle.Text", LocalResourceFile ), objUser.Username, objUser.UserID.ToString() );
                }
                cmdAdd.Text = string.Format( Localization.GetString( "AddRole.Text", LocalResourceFile ), objUser.Profile.FullName );
                txtUsers.Visible = false;
                cboUsers.Visible = false;
                cmdValidate.Visible = false;
                plUsers.Visible = false;
            }

            chkNotify.Checked = true;
        }

        /// <summary>
        /// BindGrid loads the data grid from the Database
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void BindGrid()
        {
            RoleController objRoleController = new RoleController();

            if( roleId != - 1 )
            {
                string RoleName = objRoleController.GetRole( roleId, PortalId ).RoleName;
                grdUserRoles.DataKeyField = "UserId";
                grdUserRoles.Columns[2].Visible = false;
                grdUserRoles.DataSource = objRoleController.GetUserRolesByRoleName( PortalId, RoleName );
                grdUserRoles.DataBind();
            }
            if( userId != - 1 )
            {
                UserInfo objUserInfo = UserController.GetUser( PortalId, userId, false );
                grdUserRoles.DataKeyField = "RoleId";
                grdUserRoles.Columns[1].Visible = false;
                grdUserRoles.DataSource = objRoleController.GetUserRolesByUsername( PortalId, objUserInfo.Username, Null.NullString );
                grdUserRoles.DataBind();
            }
        }

        /// <summary>
        /// GetDates gets the expiry/effective Dates of a Users Role membership
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="userId">The Id of the User</param>
        /// <param name="roleId">The Id of the Role</param>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        ///     [cnurse]    01/20/2006  Added support for Effective Date
        /// </history>
        private void GetDates( int userId, int roleId )
        {
            string strExpiryDate = "";
            string strEffectiveDate = "";

            RoleController objRoles = new RoleController();
            UserRoleInfo objUserRole = objRoles.GetUserRole( PortalId, userId, roleId );
            if( objUserRole != null )
            {
                if( Null.IsNull( objUserRole.EffectiveDate ) == false )
                {
                    strEffectiveDate = objUserRole.EffectiveDate.ToShortDateString();
                }
                if( Null.IsNull( objUserRole.ExpiryDate ) == false )
                {
                    strExpiryDate = objUserRole.ExpiryDate.ToShortDateString();
                }
            }
            else // new role assignment
            {
                RoleInfo objRole = objRoles.GetRole( roleId, PortalId );

                if( objRole.BillingPeriod > 0 )
                {
                    switch( objRole.BillingFrequency )
                    {
                        case "D":

                            strExpiryDate = DateAndTime.DateAdd( DateInterval.Day, objRole.BillingPeriod, DateTime.Now ).ToShortDateString();
                            break;
                        case "W":

                            strExpiryDate = DateAndTime.DateAdd( DateInterval.Day, ( objRole.BillingPeriod*7 ), DateTime.Now ).ToShortDateString();
                            break;
                        case "M":

                            strExpiryDate = DateAndTime.DateAdd( DateInterval.Month, objRole.BillingPeriod, DateTime.Now ).ToShortDateString();
                            break;
                        case "Y":

                            strExpiryDate = DateAndTime.DateAdd( DateInterval.Year, objRole.BillingPeriod, DateTime.Now ).ToShortDateString();
                            break;
                    }
                }
            }

            txtEffectiveDate.Text = strEffectiveDate;
            txtExpiryDate.Text = strExpiryDate;
        }

        /// <summary>
        /// SendNotification sends an email notification to the user of the change in his/her role
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="userId">The Id of the User</param>
        /// <param name="roleId">The Id of the Role</param>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void SendNotification( int userId, int roleId, string Action )
        {
            UserInfo objUser = UserController.GetUser( PortalId, userId, false );

            RoleController objRoles = new RoleController();
            RoleInfo objRole = objRoles.GetRole( roleId, PortalId );

            ArrayList Custom = new ArrayList();
            Custom.Add( objRole.RoleName );
            Custom.Add( objRole.Description );

            switch( Action )
            {
                case "add":

                    UserRoleInfo objUserRole = objRoles.GetUserRole( PortalId, userId, roleId );
                    Custom.Add( objUserRole.EffectiveDate.ToString() );
                    Custom.Add( objUserRole.ExpiryDate.ToString() );
                    Mail.SendMail( PortalSettings.Email, objUser.Email, "", Localization.GetSystemMessage( objUser.Profile.PreferredLocale, PortalSettings, "EMAIL_ROLE_ASSIGNMENT_SUBJECT", objUser ), Localization.GetSystemMessage( objUser.Profile.PreferredLocale, PortalSettings, "EMAIL_ROLE_ASSIGNMENT_BODY", objUser, Localization.GlobalResourceFile, Custom ), "", "", "", "", "", "" );
                    break;
                case "remove":

                    Custom.Add( "" );
                    Mail.SendMail( PortalSettings.Email, objUser.Email, "", Localization.GetSystemMessage( objUser.Profile.PreferredLocale, PortalSettings, "EMAIL_ROLE_UNASSIGNMENT_SUBJECT", objUser ), Localization.GetSystemMessage( objUser.Profile.PreferredLocale, PortalSettings, "EMAIL_ROLE_UNASSIGNMENT_BODY", objUser, Localization.GlobalResourceFile, Custom ), "", "", "", "", "", "" );
                    break;
            }
        }

        /// <summary>
        /// DataBind binds the data to the controls
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/10/2006  Created
        /// </history>
        public override void DataBind()
        {
            UserInfo objUser = UserController.GetUser( PortalId, userId, false );

            if( ( objUser != null ) && objUser.IsSuperUser )
            {
                Response.Redirect( Globals.NavigateURL( "Access Denied" ), true );
            }

            base.DataBind();

            //this needs to execute always to the client script code is registred in InvokePopupCal
            cmdEffectiveCalendar.NavigateUrl = Calendar.InvokePopupCal( txtEffectiveDate );
            cmdExpiryCalendar.NavigateUrl = Calendar.InvokePopupCal( txtExpiryDate );

            string localizedCalendarText = Localization.GetString( "Calendar" );
            string calendarText = "<img src=\'" + ResolveUrl( "~/images/calendar.png" ) + "\' border=\'0\' alt=\'" + localizedCalendarText + "\'>&nbsp;" + localizedCalendarText;
            cmdExpiryCalendar.Text = calendarText;
            cmdEffectiveCalendar.Text = calendarText;

            //Localize Headers
            Localization.LocalizeDataGrid( ref grdUserRoles, this.LocalResourceFile );

            //Bind the role data to the datalist
            BindData();

            BindGrid();
        }

        /// <summary>
        /// FormatExpiryDate formats the expiry/effective date and filters out nulls
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="DateTime">The Date object to format</param>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatDate( DateTime DateTime )
        {
            if( ! Null.IsNull( DateTime ) )
            {
                return DateTime.ToShortDateString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// FormatExpiryDate formats the expiry/effective date and filters out nulls
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="DateTime">The Date object to format</param>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatUser( int UserID, string DisplayName )
        {
            return "<a href=\"" + Globals.LinkClick( "userid=" + UserID.ToString(), TabId, ModuleId ) + "\" class=\"CommandButton\">" + DisplayName + "</a>";
        }

        /// <summary>
        /// Page_Init runs when the control is initialised
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/10/2006  created
        /// </history>
        protected void Page_Init( Object sender, EventArgs e )
        {
            if( ( Request.QueryString["RoleId"] != null ) )
            {
                roleId = int.Parse( Request.QueryString["RoleId"] );
            }

            if( ( Request.QueryString["UserId"] != null ) )
            {
                userId = int.Parse( Request.QueryString["UserId"] );
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
        ///     [VMasanas]  9/28/2004   Changed redirect to Access Denied
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                // Verify that the current user has access to this page
                if( ParentModule == null )
                {
                    if( PortalSecurity.IsInRoles( PortalSettings.AdministratorRoleName ) == false )
                    {
                        Response.Redirect( Globals.NavigateURL( "Access Denied" ), true );
                    }

                    DataBind();
                }
            }
            catch( ThreadAbortException )
            {
                //Do nothing if ThreadAbort as this is caused by a redirect
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cboUsers_SelectedIndexChanged runs when the selected User is changed in the
        /// Users Drop-Down
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cboUsers_SelectedIndexChanged( object sender, EventArgs e )
        {
            if( ( cboUsers.SelectedItem != null ) && ( cboRoles.SelectedItem != null ) )
            {
                GetDates( int.Parse( cboUsers.SelectedItem.Value ), int.Parse( cboRoles.SelectedItem.Value ) );
            }
            BindGrid();
        }

        /// <summary>
        /// cmdValidate_Click executes when a user selects the Validate link for a username
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        protected void cmdValidate_Click( object sender, EventArgs e )
        {
            if( txtUsers.Text != "" )
            {
                // validate username
                UserInfo objUser = UserController.GetUserByName( PortalId, txtUsers.Text, false );
                if( objUser != null )
                {
                    GetDates( objUser.UserID, int.Parse( cboRoles.SelectedItem.Value ) );
                }
                else
                {
                    txtUsers.Text = "";
                }
            }
        }

        /// <summary>
        /// cboRoles_SelectedIndexChanged runs when the selected Role is changed in the
        /// Roles Drop-Down
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cboRoles_SelectedIndexChanged( object sender, EventArgs e )
        {
            GetDates( userId, int.Parse( cboRoles.SelectedItem.Value ) );
            BindGrid();
        }

        /// <summary>
        /// cmdAdd_Click runs when the Update Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdAdd_Click( Object sender, EventArgs e )
        {
            try
            {
                if( Page.IsValid )
                {
                    RoleController objRoleController = new RoleController();

                    UserInfo objUser;

                    //Get the User
                    if( userId != - 1 )
                    {
                        objUser = UserController.GetUser( PortalId, userId, false );
                    }
                    else if( UsersControl == UsersControl.TextBox && txtUsers.Text != "" )
                    {
                        objUser = UserController.GetUserByName( PortalId, txtUsers.Text, false );
                    }
                    else if( UsersControl == UsersControl.Combo && ( cboUsers.SelectedItem != null ) )
                    {
                        objUser = UserController.GetUser( PortalId, Convert.ToInt32( cboUsers.SelectedItem.Value ), false );
                    }
                    else
                    {
                        objUser = null;
                    }

                    if( ( cboRoles.SelectedItem != null ) && ( objUser != null ) )
                    {
                        // do not modify the portal Administrator account dates
                        if( objUser.UserID == PortalSettings.AdministratorId && cboRoles.SelectedItem.Value == PortalSettings.AdministratorRoleId.ToString() )
                        {
                            txtEffectiveDate.Text = "";
                            txtExpiryDate.Text = "";
                        }

                        DateTime datEffectiveDate;
                        if( txtEffectiveDate.Text != "" )
                        {
                            datEffectiveDate = DateTime.Parse( txtEffectiveDate.Text );
                        }
                        else
                        {
                            datEffectiveDate = Null.NullDate;
                        }
                        DateTime datExpiryDate;
                        if( txtExpiryDate.Text != "" )
                        {
                            datExpiryDate = DateTime.Parse( txtExpiryDate.Text );
                        }
                        else
                        {
                            datExpiryDate = Null.NullDate;
                        }
                        EventLogController objEventLog = new EventLogController();

                        // update assignment
                        objRoleController.AddUserRole( PortalId, objUser.UserID, Convert.ToInt32( cboRoles.SelectedItem.Value ), datEffectiveDate, datExpiryDate );
                        objEventLog.AddLog( "Role", cboRoles.SelectedItem.Text, PortalSettings, userId, EventLogController.EventLogType.USER_ROLE_CREATED );

                        // send notification
                        if( chkNotify.Checked )
                        {
                            SendNotification( objUser.UserID, Convert.ToInt32( cboRoles.SelectedItem.Value ), "add" );
                        }
                    }
                }

                BindGrid();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// grdUserRoles_Delete runs when one of the Delete Buttons in the UserRoles Grid
        /// is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public void grdUserRoles_Delete( object sender, DataGridCommandEventArgs e )
        {
            try
            {
                RoleController objUser = new RoleController();
                string strMessage = "";

                if( roleId != - 1 )
                {
                    if( objUser.DeleteUserRole( PortalId, int.Parse( Convert.ToString( grdUserRoles.DataKeys[e.Item.ItemIndex] ) ), roleId ) == false )
                    {
                        strMessage = Localization.GetString( "RoleRemoveError", this.LocalResourceFile );
                    }
                    else
                    {
                        if( chkNotify.Checked )
                        {
                            SendNotification( int.Parse( Convert.ToString( grdUserRoles.DataKeys[e.Item.ItemIndex] ) ), roleId, "remove" );
                        }
                    }
                }
                if( userId != - 1 )
                {
                    if( objUser.DeleteUserRole( PortalId, userId, int.Parse( Convert.ToString( grdUserRoles.DataKeys[e.Item.ItemIndex] ) ) ) == false )
                    {
                        strMessage = Localization.GetString( "RoleRemoveError", this.LocalResourceFile );
                    }
                    else
                    {
                        if( chkNotify.Checked )
                        {
                            SendNotification( userId, int.Parse( Convert.ToString( grdUserRoles.DataKeys[e.Item.ItemIndex] ) ), "remove" );
                        }
                    }
                }

                grdUserRoles.EditItemIndex = - 1;
                BindGrid();

                if( strMessage != "" )
                {
                    UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessage.ModuleMessageType.RedError );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// grdUserRoles_ItemCreated runs when an item in the UserRoles Grid is created
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void grdUserRoles_ItemCreated( object sender, DataGridItemEventArgs e )
        {
            try
            {
                Control cmdDeleteUserRole = e.Item.FindControl( "cmdDeleteUserRole" );

                if( cmdDeleteUserRole != null )
                {
                    ClientAPI.AddButtonConfirm( ( (ImageButton)cmdDeleteUserRole ), Localization.GetString( "DeleteItem" ) );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// Gets the ModuleActions for this ModuleControl
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	3/01/2006	created
        /// </history>
        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();

                actions.Add( GetNextActionID(), Localization.GetString( "Cancel.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "lt.gif", ReturnUrl, false, SecurityAccessLevel.Admin, true, false );
                return actions;
            }
        }
    }
}