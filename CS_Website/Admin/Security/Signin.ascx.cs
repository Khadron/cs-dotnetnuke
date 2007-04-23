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
using System.Web;
using System.Web.Security;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Framework;
using DotNetNuke.Modules.Admin.Users;
using DotNetNuke.Security.Authentication;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using Globals=DotNetNuke.Common.Globals;
using UserController=DotNetNuke.Entities.Users.UserController;
using UserInfo=DotNetNuke.Entities.Users.UserInfo;

namespace DotNetNuke.Modules.Admin.Security
{
    /// <summary>
    /// The Signin UserModuleBase is used to provide a login for a registered user
    /// portal.
    /// </summary>
    /// <history>
    /// 	[cnurse]	9/24/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class Signin : UserModuleBase
    {
        private string ipAddress;

        /// <summary>
        /// Gets the Redirect URL (after successful login)
        /// </summary>
        /// <history>
        /// 	[cnurse]	04/18/2006  Created
        /// </history>
        protected string RedirectURL
        {
            get
            {
                string _RedirectURL;

                if( Request.QueryString["returnurl"] != null )
                {
                    // return to the url passed to signin
                    _RedirectURL = HttpUtility.UrlDecode( Request.QueryString["returnurl"] );
                }
                else
                {
                    object setting = GetSetting( PortalId, "Redirect_AfterLogin" );

                    if( Convert.ToInt32( setting ) == Null.NullInteger )
                    {
                        if( PortalSettings.LoginTabId != - 1 && PortalSettings.HomeTabId != - 1 )
                        {
                            // redirect to portal home page specified
                            _RedirectURL = Globals.NavigateURL( PortalSettings.HomeTabId );
                        }
                        else
                        {
                            // redirect to current page
                            _RedirectURL = Globals.NavigateURL( this.TabId );
                        }
                    }
                    else // redirect to after login page
                    {
                        _RedirectURL = Globals.NavigateURL( Convert.ToInt32( setting ) );
                    }
                }

                return _RedirectURL;
            }
        }

        /// <summary>
        /// Gets whether a profile is required on login
        /// </summary>
        /// <history>
        /// 	[cnurse]	08/21/2006  Created
        /// </history>
        protected bool RequireValidProfile
        {
            get
            {
                object setting = GetSetting(PortalId, "Security_RequireValidProfileAtLogin");
                return Convert.ToBoolean(setting);
            }
        }

        /// <summary>
        /// Gets whether the Captcha control is used to validate the login
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/17/2006  Created
        /// </history>
        protected bool UseCaptcha
        {
            get
            {
                object setting = GetSetting( PortalId, "Security_CaptchaLogin" );
                return Convert.ToBoolean( setting );
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
                int _PageNo = 0;
                if( ViewState["PageNo"] != null )
                {
                    _PageNo = Convert.ToInt32( ViewState["PageNo"] );
                }
                return _PageNo;
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
        /// 	[cnurse]	03/14/2006
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
        /// 	[cnurse]	03/14/2006
        /// </history>
        private void AddModuleMessage( string message, ModuleMessageType type, bool display )
        {
            AddLocalizedModuleMessage( Localization.GetString( message, LocalResourceFile ), type, display );
        }

        /// <summary>
        /// ShowPanel controls what "panel" is to be displayed
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/21/2006
        /// </history>
        private void ShowPanel()
        {
            bool showLogin = PageNo == 0;
            bool showPassword = PageNo == 1;
            bool showProfile = PageNo == 2;

            pnlPassword.Visible = showPassword;
            pnlProfile.Visible = showProfile;
            pnlLogin.Visible = showLogin;

            switch( PageNo )
            {
                case 0:

                    if( PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.NoRegistration )
                    {
                        tdRegister.Visible = false;
                    }
                    txtPassword.Attributes.Add( "value", txtPassword.Text );
                    lblLogin.Text = Localization.GetSystemMessage( PortalSettings, "MESSAGE_LOGIN_INSTRUCTIONS" );
                    break;
                case 1:

                    ctlPassword.UserId = UserId;
                    ctlPassword.DataBind();
                    break;
                case 2:

                    ctlProfile.UserId = UserId;
                    ctlProfile.DataBind();
                    break;
            }

            trCaptcha1.Visible = UseCaptcha;
            trCaptcha2.Visible = UseCaptcha;

            if( UseCaptcha )
            {
                ctlCaptcha.ErrorMessage = Localization.GetString( "InvalidCaptcha", this.LocalResourceFile );
                ctlCaptcha.Text = Localization.GetString( "CaptchaText", this.LocalResourceFile );
            }
        }

        /// <summary>
        /// UserAuthorized runs when the user has been authorized by the data store
        /// </summary>
        /// <param name="objUser">The logged in User</param>
        /// <history>
        /// 	[cnurse]	03/15/2006
        /// </history>
        ///<param name="canProceed"></param>
        private void UserAuthorized( UserInfo objUser, bool canProceed )
        {
            bool updatePassword = false;
            bool updateProfile = false;
            string strMessage;

            UserId = objUser.UserID;

            //Set the Page Culture(Language) based on the Users Preferred Locale
            if( ( objUser.Profile != null ) && ( objUser.Profile.PreferredLocale != null ) )
            {
                Localization.SetLanguage( objUser.Profile.PreferredLocale );
            }
            else
            {
                Localization.SetLanguage( PortalSettings.DefaultLanguage );
            }

            //Check whether Password needs updating
            if( PasswordConfig.PasswordExpiry > 0 )
            {
                DateTime expiryDate = objUser.Membership.LastPasswordChangeDate.AddDays( PasswordConfig.PasswordExpiry );
                if( expiryDate < DateTime.Today )
                {
                    //Password Expired
                    strMessage = string.Format( Localization.GetString( "PasswordExpired", this.LocalResourceFile ), expiryDate.ToLongDateString(), null );
                    AddLocalizedModuleMessage( strMessage, ModuleMessageType.YellowWarning, true );
                    updatePassword = true;
                    pnlProceed.Visible = false;
                }
                if( ( ! updatePassword ) && expiryDate < DateTime.Today.AddDays( PasswordConfig.PasswordExpiryReminder ) )
                {
                    //Password update reminder
                    strMessage = string.Format( Localization.GetString( "PasswordExpiring", this.LocalResourceFile ), expiryDate.ToLongDateString(), null );
                    AddLocalizedModuleMessage( strMessage, ModuleMessageType.YellowWarning, true );
                    updatePassword = ! canProceed;
                    pnlProceed.Visible = true;
                }
            }
            if( ( ! updatePassword ) && objUser.Membership.UpdatePassword )
            {
                //Admin has forced password update
                AddModuleMessage( "PasswordUpdate", ModuleMessageType.YellowWarning, true );
                updatePassword = true;
            }

            //Check whether Profile needs updating
            if( ! updatePassword && this.RequireValidProfile )
            {
                //Admin has forced password update
                AddModuleMessage( "ProfileUpdate", ModuleMessageType.YellowWarning, true );
                updateProfile = ! ProfileController.ValidateProfile( PortalId, objUser.Profile );
            }

            if( updatePassword )
            {
                PageNo = 1;
            }
            else if( updateProfile )
            {
                //Admin has forced profile update
                AddModuleMessage( "ProfileUpdate", ModuleMessageType.YellowWarning, true );
                PageNo = 2;
            }
            else
            {
                //Complete Login
                UserController.UserLogin( PortalId, objUser, PortalSettings.PortalName, ipAddress, chkCookie.Checked );

                // redirect browser
                Response.Redirect( RedirectURL, true );
            }

            ShowPanel();
        }

        /// <summary>
        /// WindowsAuthorization checks whether the user credentials are valid
        /// Windows credentials
        /// </summary>
        /// <param name="loginStatus">The log in status</param>
        /// <history>
        /// 	[cnurse]	03/15/2006
        /// </history>
        private UserInfo WindowsAuthorization( UserLoginStatus loginStatus )
        {
            string strMessage = Null.NullString;

            UserInfo objUser = UserController.GetUserByName( PortalSettings.PortalId, txtUsername.Text, false );
            AuthenticationController objAuthentication = new AuthenticationController();
            DotNetNuke.Security.Authentication.UserInfo objAuthUser = objAuthentication.ProcessFormAuthentication(txtUsername.Text, txtPassword.Text);
            int _userID = - 1;

            if( ( objAuthUser != null ) && ( objUser == null ) )
            {
                // Add this user into DNN database for better performance on next logon
                UserCreateStatus createStatus;
                DotNetNuke.Security.Authentication.UserController objAuthUsers = new DotNetNuke.Security.Authentication.UserController();
                createStatus = objAuthUsers.AddDNNUser( objAuthUser );
                _userID = objAuthUser.UserID;

                // Windows/DNN password validation should be same, check this status here
                strMessage = UserController.GetUserCreateStatus( createStatus );
            }
            else if( ( objAuthUser != null ) && ( objUser != null ) )
            {
                // User might has been imported by Admin or automatically added with random password
                // update DNN password to match with authenticated password from AD
                if( objUser.Membership.Password != txtPassword.Text )
                {
                    UserController.ChangePassword( objUser, objUser.Membership.Password, txtPassword.Text );
                }
                _userID = objUser.UserID;
            }

            if( _userID > 0 )
            {
                // Authenticated with DNN
                objUser = UserController.ValidateUser( PortalId, txtUsername.Text, txtPassword.Text, "", PortalSettings.PortalName, ipAddress, ref loginStatus );
                if( loginStatus != UserLoginStatus.LOGIN_SUCCESS )
                {
                    strMessage = Localization.GetString( "LoginFailed", this.LocalResourceFile );
                }
            }
            else
            {
                objUser = null;
            }

            AddLocalizedModuleMessage( strMessage, ModuleMessageType.RedError, !String.IsNullOrEmpty(strMessage) );

            return objUser;
        }

        /// <summary>
        /// Page_Init runs when the control is initialised
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/8/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Init( Object sender, EventArgs e )
        {
            this.cmdProceed.Click+=new EventHandler(cmdProceed_Click);
            //Set the Password Control Properties
            ctlPassword.ID = "Password";

            //Set the Profile Control Properties
            ctlProfile.ID = "Profile";

            //Override the redirected page title
            DotNetNuke.Framework.CDefault myPage = null;
            myPage = (CDefault)this.Page;
            myPage.Title = Localization.GetString("ControlTitle_login", this.LocalResourceFile);


        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/8/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            // Verify if portal has a customized login page
            if( !Null.IsNull( PortalSettings.LoginTabId ) && Globals.IsAdminControl() )
            {
                // login page exists and trying to access this control directly with url param -> not allowed
                Response.Redirect( Globals.NavigateURL( PortalSettings.LoginTabId ) );
            }

            ClientAPI.RegisterKeyCapture( this.Parent, this.cmdLogin, '\r' );

            if( Request.UserHostAddress != null )
            {
                ipAddress = Request.UserHostAddress;
            }

            if( Page.IsPostBack == false )
            {
                try
                {
                    if( Request.QueryString["verificationcode"] != null )
                    {
                        if( PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.VerifiedRegistration )
                        {
                            //Display Verification Rows
                            rowVerification1.Visible = true;
                            rowVerification2.Visible = true;
                            txtVerification.Text = Request.QueryString["verificationcode"];
                        }
                    }

                    PageNo = 0;
                    if( Request.QueryString["username"] != null )
                    {
                        txtUsername.Text = Request.QueryString["username"];
                        Globals.SetFormFocus( txtPassword );
                    }
                    else
                    {
                        Globals.SetFormFocus( txtUsername );
                    }
                }
                catch
                {
                    //control not there or error setting focus
                }
            }

            ShowPanel();
        }

        /// <summary>
        /// cmdLogin_Click runs when the login button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/24/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        ///     [cnurse]    12/11/2005  Updated to reflect abstraction of Membership
        /// </history>
        protected void cmdLogin_Click( object sender, EventArgs e )
        {
            if( ( UseCaptcha && ctlCaptcha.IsValid ) || ( ! UseCaptcha ) )
            {
                //Try to validate user
                UserLoginStatus loginStatus = UserLoginStatus.LOGIN_FAILURE;
                UserInfo objUser = UserController.ValidateUser( PortalId, txtUsername.Text, txtPassword.Text, txtVerification.Text, PortalSettings.PortalName, ipAddress, ref loginStatus );

                if( objUser == null ) //Some kind of Login failure
                {
                    if( loginStatus == UserLoginStatus.LOGIN_FAILURE )
                    {
                        Hashtable settings = Entities.Portals.PortalSettings.GetSiteSettings(PortalId);
                        if (Convert.ToString(settings["WindowsAuthentication"]) == "True")
                        {
                            //Try Windows Authorization (user may be authorized in Windows, but not in DNN yet)
                            objUser = WindowsAuthorization(loginStatus);
                        }
                    }
                }

                if( objUser == null )
                {
                    switch( loginStatus )
                    {
                        case UserLoginStatus.LOGIN_USERNOTAPPROVED:

                            //Check if its the first time logging in to a verified site
                            if( PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.VerifiedRegistration )
                            {
                                if( ! rowVerification1.Visible )
                                {
                                    //Display Verification Rows so User can enter verification code
                                    rowVerification1.Visible = true;
                                    rowVerification2.Visible = true;
                                }
                                else
                                {
                                    if( !String.IsNullOrEmpty(txtVerification.Text) )
                                    {
                                        AddModuleMessage( "InvalidCode", ModuleMessageType.RedError, true );
                                    }
                                    else
                                    {
                                        AddModuleMessage( "EnterCode", ModuleMessageType.GreenSuccess, true );
                                    }
                                }
                            }
                            else
                            {
                                AddModuleMessage( "UserNotAuthorized", ModuleMessageType.RedError, true );
                            }
                            break;
                        case UserLoginStatus.LOGIN_USERLOCKEDOUT:

                            AddModuleMessage( "UserLockedOut", ModuleMessageType.RedError, true );
                            // notify administrator about account lockout ( possible hack attempt )
                            ArrayList Custom = new ArrayList();
                            Custom.Add( txtUsername.Text );
                            Mail.SendMail( PortalSettings.Email, PortalSettings.Email, "", Localization.GetSystemMessage( PortalSettings, "EMAIL_USER_LOCKOUT_SUBJECT", Localization.GlobalResourceFile, Custom ), Localization.GetSystemMessage( PortalSettings, "EMAIL_USER_LOCKOUT_BODY", Localization.GlobalResourceFile, Custom ), "", "", "", "", "", "" );
                            break;
                        case UserLoginStatus.LOGIN_FAILURE:

                            AddModuleMessage( "LoginFailed", ModuleMessageType.RedError, true );
                            break;
                    }
                }
                else //Login Success
                {
                    UserAuthorized( objUser, false );
                }
            }
        }

        /// <summary>
        /// cmdPassword_Click runs when the Password Reminder button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        protected void cmdPassword_Click( Object sender, EventArgs e )
        {
            switch (System.Web.Security.Membership.Provider.PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    {
                        Response.Redirect(Globals.NavigateURL("SendPassword"), true);
                        break;
                    }
                case MembershipPasswordFormat.Hashed:
                    {
                        Response.Redirect(Globals.NavigateURL("ResetPassword"), true);
                        break;
                    }
                case MembershipPasswordFormat.Encrypted:
                    {
                        Response.Redirect(Globals.NavigateURL("SendPassword"), true);
                        break;
                    }
            }
        }

        /// <summary>
        /// cmdProceed_Click runs when the Proceed Anyway button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	06/30/2006  Created
        /// </history>
        protected void cmdProceed_Click( object sender, EventArgs e )
        {
            UserInfo _User = ctlPassword.User;
            UserAuthorized( _User, true );
        }

        /// <summary>
        /// cmdRegister_Click runs when the register button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/24/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        ///     [cnurse]    12/11/2005  Updated to reflect abstraction of Membership
        /// </history>
        protected void cmdRegister_Click( object sender, EventArgs e )
        {
            if( PortalSettings.UserRegistration != (int)Globals.PortalRegistrationType.NoRegistration )
            {
                string registerUrl;
                if( PortalSettings.UserTabId != - 1 )
                {
                    // user defined tab
                    registerUrl = Globals.NavigateURL( PortalSettings.UserTabId );
                }
                else
                {
                    // admin tab
                    registerUrl = Globals.NavigateURL( "Register" );
                }
                Response.Redirect( registerUrl, true );
            }
        }

        /// <summary>
        /// PasswordUpdated runs when the password is updated
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/15/2006  Created
        /// </history>
        protected void PasswordUpdated( object sender, Password.PasswordUpdatedEventArgs e )
        {
            PasswordUpdateStatus status = e.UpdateStatus;

            if( status == PasswordUpdateStatus.Success )
            {
                AddModuleMessage( "PasswordChanged", ModuleMessageType.GreenSuccess, true );

                //Authorize User
                UserInfo _User = ctlPassword.User;
                _User.Membership.LastPasswordChangeDate = DateTime.Now;
                _User.Membership.UpdatePassword = false;
                UserAuthorized( _User, true );
            }
            else
            {
                AddModuleMessage( status.ToString(), ModuleMessageType.RedError, true );
            }
        }

        /// <summary>
        /// ProfileUpdated runs when the profile is updated
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/16/2006  Created
        /// </history>
        protected void ProfileUpdated( object sender, EventArgs e )
        {
            //Authorize User
            UserAuthorized( ctlProfile.User, true );
        }
    }
}