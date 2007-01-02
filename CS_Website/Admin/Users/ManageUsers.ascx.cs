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
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Profile;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Vendors;
using DotNetNuke.UI.Skins.Controls;

namespace DotNetNuke.Modules.Admin.Users
{
    /// <summary>
    /// The ManageUsers UserModuleBase is used to manage Users
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    ///     [cnurse]    2/21/2005   Updated to use new User UserControl
    /// </history>
    public partial class ManageUsers : UserModuleBase, IActionable
    {
        /// <summary>
        /// Gets the Redirect URL (after successful registration)
        /// </summary>
        /// <history>
        /// 	[cnurse]	05/18/2006  Created
        /// </history>
        protected string RedirectURL
        {
            get
            {
                string redirectURL;
                object setting = GetSetting( PortalId, "Redirect_AfterRegistration" );

                if( Convert.ToInt32( setting ) == Null.NullInteger )
                {
                    // redirect to current page
                    redirectURL = Globals.NavigateURL();
                }
                else // redirect to after registration page
                {
                    redirectURL = Globals.NavigateURL( Convert.ToInt32( setting ) );
                }

                return redirectURL;
            }
        }

        /// <summary>
        /// Gets whether a profile is required in AddUser mode
        /// </summary>
        /// <history>
        /// 	[cnurse]	05/18/2006  Created
        /// </history>
        protected bool RequireProfile
        {
            get
            {
                object setting = GetSetting( PortalId, "Security_RequireValidProfile" );
                return Convert.ToBoolean( setting ) && IsRegister;
            }
        }

        /// <summary>
        /// Gets the Return Url for the page
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/09/2006  Created
        /// </history>
        protected string ReturnUrl
        {
            get
            {
                return Globals.NavigateURL( TabId, "", ( !String.IsNullOrEmpty(UserFilter) ? "filter=" + UserFilter : "" ).ToString() );
            }
        }

        /// <summary>
        /// Gets and sets the Filter to use
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/09/2006  Created
        /// </history>
        protected string UserFilter
        {
            get
            {
                return ( ( Request.QueryString["filter"] != "" ) ? ( Request.QueryString["filter"] ) : "" ).ToString();
            }
        }

        /// <summary>
        /// Gets and sets the current Page No
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/09/2006  Created
        /// </history>
        public int PageNo
        {
            get
            {
                int pageNo = 0;
                if( ViewState["PageNo"] != null )
                {
                    pageNo = Convert.ToInt32( ViewState["PageNo"] );
                }
                return pageNo;
            }
            set
            {
                ViewState["PageNo"] = value;
            }
        }

        /// <summary>
        /// AddLocalizedModuleMessage adds a localized module message
        /// </summary>
        /// <param name="message">The localized message</param>
        /// <param name="type">The type of message</param>
        /// <param name="display">A flag that determines whether the message should be displayed</param>
        /// <history>
        /// 	[cnurse]	03/13/2006
        /// </history>
        private void AddLocalizedModuleMessage( string message, ModuleMessageType type, bool display )
        {
            if( display )
            {
                UI.Skins.Skin.AddModuleMessage( this, message, type );
            }
        }

        /// <summary>
        /// AddModuleMessage adds a module message
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="type">The type of message</param>
        /// <param name="display">A flag that determines whether the message should be displayed</param>
        /// <history>
        /// 	[cnurse]	03/13/2006
        /// </history>
        private void AddModuleMessage( string message, ModuleMessageType type, bool display )
        {
            AddLocalizedModuleMessage( Localization.GetString( message, LocalResourceFile ), type, display );
        }

        /// <summary>
        /// BindData binds the controls to the Data
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void BindData()
        {
            if( User != null )
            {
                //If trying to add a SuperUser - check that user is a SuperUser
                if( AddUser && IsHostMenu && ! this.UserInfo.IsSuperUser )
                {
                    AddModuleMessage( "SuperUser", ModuleMessageType.YellowWarning, true );
                    DisableForm();
                    return;
                }

                //Check if User is a member of the Current Portal
                if( User.PortalID != Null.NullInteger && User.PortalID != PortalId )
                {
                    AddModuleMessage( "InvalidUser", ModuleMessageType.YellowWarning, true );
                    DisableForm();
                    return;
                }

                //Check if User is a SuperUser and that the current User is a SuperUser
                if( User.IsSuperUser && ! this.UserInfo.IsSuperUser )
                {
                    AddModuleMessage( "SuperUser", ModuleMessageType.YellowWarning, true );
                    DisableForm();
                    return;
                }

                if( IsEdit )
                {
                    //Check if user has admin rights
                    if( ! IsAdmin )
                    {
                        AddModuleMessage( "NotAuthorized", ModuleMessageType.YellowWarning, true );
                        DisableForm();
                        return;
                    }
                }
                else
                {
                    if( ! IsUser )
                    {
                        if( Request.IsAuthenticated )
                        {
                            //Display current user's profile
                            if( PortalSettings.UserTabId != - 1 )
                            {
                                Response.Redirect( Globals.NavigateURL( PortalSettings.UserTabId, "", "UserID=" + UserInfo.UserID ), true );
                            }
                            else
                            {
                                Response.Redirect( Globals.NavigateURL( PortalSettings.ActiveTab.TabID, "Profile", "UserID=" + UserInfo.UserID ), true );
                            }
                        }
                        else
                        {
                            if( ( User.UserID > Null.NullInteger ) || ( ! IsRegister ) )
                            {
                                AddModuleMessage( "NotAuthorized", ModuleMessageType.YellowWarning, true );
                                DisableForm();
                                return;
                            }
                        }
                    }
                }

                if( AddUser )
                {
                    if( IsRegister )
                    {
                        BindRegister();
                    }
                    else
                    {
                        lblTitle.Text = Localization.GetString( "AddUser.Text", LocalResourceFile );
                    }
                }
                else
                {
                    if( IsRegister )
                    {
                        trTitle.Visible = false;
                    }
                    else
                    {
                        lblTitle.Text = string.Format( Localization.GetString( "UserTitle.Text", LocalResourceFile ), User.Username, User.UserID );
                    }
                }

                if( ! Page.IsPostBack )
                {
                    PageNo = 0;
                }

                ShowPanel();
            }
            else
            {
                AddModuleMessage( "NoUser", ModuleMessageType.YellowWarning, true );
                DisableForm();
            }
        }

        /// <summary>
        /// BindMembership binds the membership controls
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/13/2006
        /// </history>
        private void BindMembership()
        {
            ctlMembership.User = User;
            ctlMembership.DataBind();

            AddModuleMessage( "UserLockedOut", ModuleMessageType.YellowWarning, ctlMembership.UserMembership.LockedOut && ( ! Page.IsPostBack ) );
            imgLockedOut.Visible = ctlMembership.UserMembership.LockedOut;
            imgOnline.Visible = ctlMembership.UserMembership.IsOnLine;
        }

        /// <summary>
        /// BindRegister binds the register controls
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/20/2006
        /// </history>
        private void BindRegister()
        {
            // Verify that the current user has access to this page
            if( PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.NoRegistration && Request.IsAuthenticated == false )
            {
                Response.Redirect( Globals.NavigateURL( "Access Denied" ), true );
            }

            lblTitle.Text = Localization.GetString( "Register.Text", this.LocalResourceFile );

            lblUserHelp.Text = Localization.GetSystemMessage( PortalSettings, "MESSAGE_REGISTRATION_INSTRUCTIONS" );
            switch( PortalSettings.UserRegistration )
            {
                case (int)Globals.PortalRegistrationType.PrivateRegistration:

                    lblUserHelp.Text += Localization.GetString( "PrivateMembership", this.LocalResourceFile );
                    break;
                case (int)Globals.PortalRegistrationType.PublicRegistration:

                    lblUserHelp.Text += Localization.GetString( "PublicMembership", this.LocalResourceFile );
                    break;
                case (int)Globals.PortalRegistrationType.VerifiedRegistration:

                    lblUserHelp.Text += Localization.GetString( "VerifiedMembership", this.LocalResourceFile );
                    break;
            }
            lblUserHelp.Text += Localization.GetString( "Required", this.LocalResourceFile );
            trHelp.Visible = true;
        }

        /// <summary>
        /// BindUser binds the user controls
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/13/2006
        /// </history>
        private void BindUser()
        {
            if( AddUser )
            {
                ctlUser.ShowUpdate = false;
            }
            ctlUser.User = User;
            ctlUser.DataBind();

            //Bind the Membership
            if( AddUser || ( IsUser && ! IsAdmin ) )
            {
                ctlMembership.Visible = false;
            }
            else
            {
                BindMembership();
            }
        }

        /// <summary>
        /// DisableForm disbles the form (if the user is not authorised)
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/13/2006
        /// </history>
        private void DisableForm()
        {
            pnlTabs.Visible = false;
            pnlUser.Visible = false;
            pnlRoles.Visible = false;
            pnlPassword.Visible = false;
            pnlProfile.Visible = false;
        }

        /// <summary>
        /// ShowPanel displays the correct "panel"
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/13/2006
        /// </history>
        private void ShowPanel()
        {
            bool showUser = PageNo == 0;
            bool showRoles = PageNo == 1;
            bool showPassword = PageNo == 2;
            bool showProfile = PageNo == 3;
            bool showServices = PageNo == 4;

            pnlRoles.Visible = showRoles;
            pnlPassword.Visible = showPassword;
            pnlServices.Visible = showServices;

            cmdUser.Enabled = ! showUser;

            if( AddUser )
            {
                pnlTabs.Visible = false;
                pnlUser.Visible = true;
                pnlRegister.Visible = true;

                BindUser();

                if( RequireProfile )
                {
                    pnlProfile.Visible = true;
                    if( AddUser )
                    {
                        ctlProfile.ShowUpdate = false;
                    }
                    ctlProfile.User = User;
                    ctlProfile.DataBind();
                }
            }
            else
            {
                pnlUser.Visible = showUser;
                pnlProfile.Visible = showProfile;

                if( ! IsAdmin && ! IsUser )
                {
                    cmdPassword.Visible = false;
                }
                else
                {
                    cmdPassword.Enabled = ! showPassword;
                }

                if( ! IsEdit || User.IsSuperUser )
                {
                    cmdRoles.Visible = false;
                }
                else
                {
                    cmdRoles.Enabled = ! showRoles;
                }

                cmdProfile.Enabled = ! showProfile;

                if( IsEdit || User.IsSuperUser )
                {
                    cmdServices.Visible = false;
                }
                else
                {
                    cmdServices.Enabled = ! showServices;
                }

                switch( PageNo )
                {
                    case 0:

                        BindUser();
                        break;
                    case 1:

                        ctlRoles.DataBind();
                        break;
                    case 2:

                        ctlPassword.User = User;
                        ctlPassword.DataBind();
                        break;
                    case 3:

                        ctlProfile.User = User;
                        ctlProfile.DataBind();
                        break;
                    case 4:

                        ctlServices.User = User;
                        ctlServices.DataBind();
                        break;
                }
            }
        }

        /// <summary>
        /// Page_Init runs when the control is initialised
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/01/2006
        /// </history>
        protected void Page_Init( Object sender, EventArgs e )
        {
            //Set the Membership Control Properties
            ctlMembership.ID = "Membership";
            ctlMembership.UserId = UserId;

            //Set the Membership Control Properties
            ctlUser.ID = "User";
            ctlUser.UserId = UserId;

            //Set the Roles Control Properties
            ctlRoles.ID = "SecurityRoles";
            ctlRoles.ParentModule = this;

            //Set the Password Control Properties
            ctlPassword.ID = "Password";
            ctlPassword.UserId = UserId;

            //Set the Profile Control Properties
            ctlProfile.ID = "Profile";
            ctlProfile.UserId = UserId;

            //Set the Services Control Properties
            ctlServices.ID = "MemberServices";
            ctlServices.UserId = UserId;
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/01/2006
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                //Add an Action Event Handler to the Skin
                AddActionHandler( new ActionEventHandler( ModuleAction_Click ) );

                //Bind the User information to the controls
                BindData();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdPassword_Click runs when the Manage Password button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/02/2006
        /// </history>
        protected void cmdPassword_Click( object sender, EventArgs e )
        {
            PageNo = 2;
            ShowPanel();
        }

        /// <summary>
        /// cmdProfile_Click runs when the Manage profile button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/02/2006
        /// </history>
        protected void cmdProfile_Click( object sender, EventArgs e )
        {
            PageNo = 3;
            ShowPanel();
        }

        /// <summary>
        /// cmdRegister_Click runs when the Register button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	05/18/2006
        /// </history>
        protected void cmdRegister_Click( object sender, EventArgs e )
        {
            if( ctlUser.IsValid && ( ( RequireProfile && ctlProfile.IsValid ) || ( ! RequireProfile ) ) )
            {
                //Call the Create User method of the User control so that it can create
                //the user and raise the appropriate event(s)
                ctlUser.CreateUser();
            }
        }

        /// <summary>
        /// cmdRoles_Click runs when the Manage roles button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/02/2006
        /// </history>
        protected void cmdRoles_Click( object sender, EventArgs e )
        {
            PageNo = 1;
            ShowPanel();
        }

        /// <summary>
        /// cmdServices_Click runs when the Manage Services button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/13/2006
        /// </history>
        protected void cmdServices_Click( object sender, EventArgs e )
        {
            PageNo = 4;
            ShowPanel();
        }

        /// <summary>
        /// cmdUser_Click runs when the Manage user credentials button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/02/2006
        /// </history>
        protected void cmdUser_Click( object sender, EventArgs e )
        {
            PageNo = 0;
            ShowPanel();
        }

        /// <summary>
        /// ModuleAction_Click handles all ModuleAction events raised from the skin
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="sender"> The object that triggers the event</param>
        /// <param name="e">An ActionEventArgs object</param>
        /// <history>
        /// 	[cnurse]	03/01/2006	Created
        /// </history>
        private void ModuleAction_Click( object sender, ActionEventArgs e )
        {
            switch( e.Action.CommandArgument )
            {
                case "ManageRoles":

                    pnlRoles.Visible = true;
                    pnlUser.Visible = false;
                    break;
                case "Cancel":

                    break;
                    //OnCancelAction()
                case "Delete":

                    break;
                    //OnDeleteAction()
                case "Edit":

                    break;
                    //OnEditAction()
                case "Save":

                    break;
                    //OnSaveAction()
                default:

                    break;
                    //OnModuleAction(e.Action.CommandArgument)
            }
        }

        /// <summary>
        /// MembershipAuthorized runs when the User has been unlocked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	3/01/2006	created
        /// </history>
        internal void MembershipAuthorized( object sender, EventArgs e )
        {
            try
            {
                AddModuleMessage( "UserAuthorized", ModuleMessageType.GreenSuccess, true );

                //Send Notification to User
                Mail.SendMail( User, MessageType.UserRegistrationPublic, PortalSettings );

                BindMembership();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// MembershipUnAuthorized runs when the User has been unlocked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	3/01/2006	created
        /// </history>
        protected void MembershipUnAuthorized( object sender, EventArgs e )
        {
            try
            {
                AddModuleMessage( "UserUnAuthorized", ModuleMessageType.GreenSuccess, true );

                BindMembership();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// MembershipUnLocked runs when the User has been unlocked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	3/01/2006	created
        /// </history>
        protected void MembershipUnLocked( object sender, EventArgs e )
        {
            try
            {
                AddModuleMessage( "UserUnLocked", ModuleMessageType.GreenSuccess, true );

                BindMembership();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// PasswordQuestionAnswerUpdated runs when the Password Q and A have been updated.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	3/09/2006	created
        /// </history>
        protected void PasswordQuestionAnswerUpdated( object sender, Password.PasswordUpdatedEventArgs e )
        {
            PasswordUpdateStatus status = e.UpdateStatus;

            if( status == PasswordUpdateStatus.Success )
            {
                AddModuleMessage( "PasswordQAChanged", ModuleMessageType.GreenSuccess, true );
            }
            else
            {
                AddModuleMessage( status.ToString(), ModuleMessageType.RedError, true );
            }
        }

        /// <summary>
        /// PasswordUpdated runs when the Password has been updated or reset
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	3/08/2006	created
        /// </history>
        protected void PasswordUpdated( object sender, Password.PasswordUpdatedEventArgs e )
        {
            PasswordUpdateStatus status = e.UpdateStatus;

            if( status == PasswordUpdateStatus.Success )
            {
                AddModuleMessage( "PasswordChanged", ModuleMessageType.GreenSuccess, true );

                //Send Notification to User
                Mail.SendMail( User, MessageType.PasswordReminder, PortalSettings );
            }
            else
            {
                AddModuleMessage( status.ToString(), ModuleMessageType.RedError, true );
            }
        }

        /// <summary>
        /// ProfileUpdateCompleted runs when the Profile has been updated
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	3/20/2006	created
        /// </history>
        protected void ProfileUpdateCompleted( object sender, EventArgs e )
        {
            if( IsUser )
            {
                //Notify the user that his/her profile was updated
                Mail.SendMail( User, MessageType.ProfileUpdated, PortalSettings );

                ProfilePropertyDefinition localeProperty = User.Profile.GetProperty( "PreferredLocale" );
                if( localeProperty.IsDirty )
                {
                    //store preferredlocale in cookie
                    Localization.SetLanguage( User.Profile.PreferredLocale );
                }
            }

            //Redirect to same page (this will update all controls for any changes to profile
            //and leave us at Page 0 (User Credentials)
            Response.Redirect( Request.RawUrl, true );
        }

        /// <summary>
        /// UserCreateCompleted runs when a new user has been Created
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	3/06/2006	created
        /// </history>
        protected void UserCreateCompleted( object sender, User.UserCreatedEventArgs e )
        {
            try
            {
                if( e.CreateStatus == UserCreateStatus.Success )
                {
                    UserInfo objUser = e.NewUser;

                    if( IsRegister )
                    {
                        // send notification to portal administrator of new user registration
                        Mail.SendMail( User, MessageType.UserRegistrationAdmin, PortalSettings );

                        // complete registration
                        string strMessage = "";
                        switch( PortalSettings.UserRegistration )
                        {
                            case (int)Globals.PortalRegistrationType.PrivateRegistration:

                                Mail.SendMail( User, MessageType.UserRegistrationPrivate, PortalSettings );

                                //show a message that a portal administrator has to verify the user credentials
                                strMessage = string.Format( Localization.GetString( "PrivateConfirmationMessage", this.LocalResourceFile ), User.Email );
                                break;

                            case (int)Globals.PortalRegistrationType.PublicRegistration:

                                Mail.SendMail( User, MessageType.UserRegistrationPublic, PortalSettings );

                                UserLoginStatus loginStatus = 0;
                                UserController.UserLogin( PortalSettings.PortalId, User.Username, User.Membership.Password, "", PortalSettings.PortalName, "", ref loginStatus, false );
                                break;
                            case (int)Globals.PortalRegistrationType.VerifiedRegistration:

                                Mail.SendMail( User, MessageType.UserRegistrationVerified, PortalSettings );

                                //show a message that an email has been send with the registration details
                                strMessage = string.Format( Localization.GetString( "VerifiedConfirmationMessage", this.LocalResourceFile ), User.Email );
                                break;
                        }

                        // affiliate
                        if( ! Null.IsNull( User.AffiliateID ) )
                        {
                            AffiliateController objAffiliates = new AffiliateController();
                            objAffiliates.UpdateAffiliateStats( User.AffiliateID, 0, 1 );
                        }

                        //store preferredlocale in cookie
                        Localization.SetLanguage( User.Profile.PreferredLocale );

                        AddLocalizedModuleMessage( strMessage, ModuleMessageType.GreenSuccess, ( strMessage.Length > 0 ) );
                    }
                    else
                    {
                        if( e.Notify )
                        {
                            //Send Notification to User
                            if( PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.VerifiedRegistration )
                            {
                                Mail.SendMail( User, MessageType.UserRegistrationVerified, PortalSettings );
                            }
                            else
                            {
                                Mail.SendMail( User, MessageType.UserRegistrationPublic, PortalSettings );
                            }
                        }
                    }

                    //Log Event to Event Log
                    EventLogController objEventLog = new EventLogController();
                    objEventLog.AddLog( User, PortalSettings, UserId, User.Username, EventLogController.EventLogType.USER_CREATED );

                    if( IsRegister )
                    {
                        Response.Redirect( RedirectURL, true );
                    }
                    else
                    {
                        Response.Redirect( ReturnUrl, true );
                    }
                }
                else
                {
                    AddLocalizedModuleMessage( UserController.GetUserCreateStatus( e.CreateStatus ), ModuleMessageType.RedError, true );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// UserDeleted runs when the User has been deleted
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	3/01/2006	created
        /// </history>
        protected void UserDeleted( object sender, User.UserDeletedEventArgs e )
        {
            try
            {
                EventLogController objEventLog = new EventLogController();
                objEventLog.AddLog( "Username", e.UserName, PortalSettings, e.UserId, EventLogController.EventLogType.USER_DELETED );

                Response.Redirect( Globals.NavigateURL( TabId, "", ( ( Request.QueryString["filter"] != "" ) ? ( "filter=" + Request.QueryString["filter"] ) : "" ).ToString() ), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// UserUpdateCompleted runs when a user has been updated
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	3/02/2006	created
        /// </history>
        protected void UserUpdateCompleted( object sender, EventArgs e )
        {
            //Redirect to same page (this will update all controls for any changes)
            Response.Redirect( Request.RawUrl, true );
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
                if( ( IsAdminTab || IsHostTab ) && ( this.ModuleId > - 1 ) )
                {
                    actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "add.gif", EditUrl(), false, SecurityAccessLevel.Admin, true, false );

                    if( ProfileProviderConfig.CanEditProviderProperties )
                    {
                        actions.Add( GetNextActionID(), Localization.GetString( "ManageProfile.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "icon_profile_16px.gif", EditUrl( "ManageProfile" ), false, SecurityAccessLevel.Admin, true, false );
                    }

                    actions.Add( GetNextActionID(), Localization.GetString( "Cancel.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "lt.gif", ReturnUrl, false, SecurityAccessLevel.Admin, true, false );
                }
                return actions;
            }
        }
    }
}