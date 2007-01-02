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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Users
{
    /// <summary>
    /// The User UserModuleBase is used to manage the base parts of a User.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	03/01/2006  created
    /// </history>
    public partial class User : UserModuleBase
    {
        public delegate void UserCreatedEventHandler( object sender, UserCreatedEventArgs e );

        public delegate void UserDeletedEventHandler( object sender, UserDeletedEventArgs e );

        private UserCreatedEventHandler UserCreatedEvent;

        public event UserCreatedEventHandler UserCreated
        {
            add
            {
                UserCreatedEvent += value;
            }
            remove
            {
                UserCreatedEvent -= value;
            }
        }

        private UserCreatedEventHandler UserCreateCompletedEvent;

        public event UserCreatedEventHandler UserCreateCompleted
        {
            add
            {
                UserCreateCompletedEvent += value;
            }
            remove
            {
                UserCreateCompletedEvent -= value;
            }
        }

        private UserDeletedEventHandler UserDeletedEvent;

        public event UserDeletedEventHandler UserDeleted
        {
            add
            {
                UserDeletedEvent += value;
            }
            remove
            {
                UserDeletedEvent -= value;
            }
        }

        private EventHandler UserUpdatedEvent;

        public event EventHandler UserUpdated
        {
            add
            {
                UserUpdatedEvent += value;
            }
            remove
            {
                UserUpdatedEvent -= value;
            }
        }

        private EventHandler UserUpdateCompletedEvent;

        public event EventHandler UserUpdateCompleted
        {
            add
            {
                UserUpdateCompletedEvent += value;
            }
            remove
            {
                UserUpdateCompletedEvent -= value;
            }
        }

        /// <summary>
        /// Gets whether the Captcha control is used to validate the login
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        protected bool UseCaptcha
        {
            get
            {
                object setting = GetSetting( PortalId, "Security_CaptchaRegister" );
                return Convert.ToBoolean( setting ) && IsRegister;
            }
        }

        /// <summary>
        /// Gets whether the User is valid
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        public bool IsValid
        {
            get
            {
                bool isValid = true;

                //Check Captcha Control
                if( UseCaptcha )
                {
                    isValid = ctlCaptcha.IsValid;
                }

                //Check User Editor
                if( isValid )
                {
                    isValid = UserEditor.IsValid;
                }

                //Check Password is valid
                if( AddUser )
                {
                    UserCreateStatus createStatus = UserCreateStatus.AddUser;
                    if( ! chkRandom.Checked )
                    {
                        //1. Check Password and Confirm are the same
                        if( txtPassword.Text != txtConfirm.Text )
                        {
                            createStatus = UserCreateStatus.PasswordMismatch;
                        }
                        //2. Check Password is Valid
                        if( createStatus == UserCreateStatus.AddUser && ! UserController.ValidatePassword( txtPassword.Text ) )
                        {
                            createStatus = UserCreateStatus.InvalidPassword;
                        }
                        if( createStatus == UserCreateStatus.AddUser )
                        {
                            User.Membership.Password = txtPassword.Text;
                        }
                    }
                    else
                    {
                        //Generate a random password for the user
                        User.Membership.Password = UserController.GeneratePassword();
                    }

                    if( createStatus != UserCreateStatus.AddUser )
                    {
                        isValid = false;
                        valPassword.ErrorMessage = "<br/>" + UserController.GetUserCreateStatus( createStatus );
                        valPassword.IsValid = false;
                    }
                }

                return isValid;
            }
        }

        /// <summary>
        /// Gets and sets whether the Update button
        /// </summary>
        /// <history>
        /// 	[cnurse]	05/18/2006  Created
        /// </history>
        public bool ShowUpdate
        {
            get
            {
                return pnlUpdate.Visible;
            }
            set
            {
                pnlUpdate.Visible = value;
            }
        }

        /// <summary>
        /// Raises the UserCreateCompleted Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/13/2006  Created
        /// </history>
        public void OnUserCreateCompleted( UserCreatedEventArgs e )
        {
            if( UserCreateCompletedEvent != null )
            {
                UserCreateCompletedEvent( this, e );
            }
        }

        /// <summary>
        /// Raises the UserCreated Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        public void OnUserCreated( UserCreatedEventArgs e )
        {
            if( UserCreatedEvent != null )
            {
                UserCreatedEvent( this, e );
            }
        }

        /// <summary>
        /// Raises the UserDeleted Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        public void OnUserDeleted( UserDeletedEventArgs e )
        {
            if( UserDeletedEvent != null )
            {
                UserDeletedEvent( this, e );
            }
        }

        /// <summary>
        /// Raises the UserUpdated Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        public void OnUserUpdated( EventArgs e )
        {
            if( UserUpdatedEvent != null )
            {
                UserUpdatedEvent( this, e );
            }
        }

        /// <summary>
        /// Raises the UserUpdated Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        public void OnUserUpdateCompleted( EventArgs e )
        {
            if( UserUpdateCompletedEvent != null )
            {
                UserUpdateCompletedEvent( this, e );
            }
        }

        /// <summary>
        /// CreateUser creates a new user in the Database
        /// </summary>
        /// <history>
        /// 	[cnurse]	05/18/2006  Created
        /// </history>
        public void CreateUser()
        {
            if( IsRegister )
            {
                //Set the Approved status based on the Portal Settings
                if( PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.PublicRegistration )
                {
                    User.Membership.Approved = true;
                }
                else
                {
                    User.Membership.Approved = false;
                }
            }
            else
            {
                //Set the Approved status from the value in the Authorized checkbox
                User.Membership.Approved = chkAuthorize.Checked;
            }

            if( User.Profile.LastName == Null.NullString )
            {
                User.Profile.LastName = User.LastName;
            }
            if( User.Profile.FirstName == Null.NullString )
            {
                User.Profile.FirstName = User.FirstName;
            }
            UserInfo refUser = User;
            UserCreateStatus createStatus = UserController.CreateUser( ref refUser );

            UserCreatedEventArgs args;
            if( createStatus == UserCreateStatus.Success )
            {
                args = new UserCreatedEventArgs( User );
                args.Notify = chkNotify.Checked;
            }
            else // registration error
            {
                args = new UserCreatedEventArgs( null );
            }
            args.CreateStatus = createStatus;
            OnUserCreated( args );
            OnUserCreateCompleted( args );
        }

        /// <summary>
        /// DataBind binds the data to the controls
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        public override void DataBind()
        {
            if( UseCaptcha )
            {
                ctlCaptcha.ErrorMessage = Localization.GetString( "InvalidCaptcha", this.LocalResourceFile );
                ctlCaptcha.Text = Localization.GetString( "CaptchaText", this.LocalResourceFile );
            }

            if( Page.IsPostBack == false )
            {
                string confirmString = Localization.GetString( "DeleteItem" );
                if( IsUser )
                {
                    confirmString = Localization.GetString( "ConfirmUnRegister", this.LocalResourceFile );
                }
                ClientAPI.AddButtonConfirm( cmdDelete, confirmString );
                chkRandom.Checked = false;
            }

            if( AddUser )
            {
                cmdDelete.Visible = false;
            }

            if( IsUser )
            {
                cmdDelete.ResourceKey = "UnRegister";
            }
            else
            {
                cmdDelete.ResourceKey = "Delete";
            }

            if( AddUser )
            {
                if( IsRegister )
                {
                    tblAddUser.Visible = false;
                    trRandom.Visible = false;
                    lblPasswordHelp.Text = Localization.GetString( "PasswordHelpUser", this.LocalResourceFile );
                }
                else
                {
                    lblPasswordHelp.Text = Localization.GetString( "PasswordHelpAdmin", this.LocalResourceFile );
                }
                pnlAddUser.Visible = true;
                txtConfirm.Attributes.Add( "value", txtConfirm.Text );
                txtPassword.Attributes.Add( "value", txtPassword.Text );
            }

            UserEditor.DataSource = User;
            UserEditor.DataBind();

            trCaptcha.Visible = UseCaptcha;
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            UserEditor.LocalResourceFile = this.LocalResourceFile;
        }

        /// <summary>
        /// cmdDelete_Click runs when the delete Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        protected void cmdDelete_Click( Object sender, EventArgs e )
        {
            string name = User.Username;
            int id = UserId;

            UserInfo refUser = User;
            if( UserController.DeleteUser( ref refUser, true, false ) )
            {
                OnUserDeleted( new UserDeletedEventArgs( id, name ) );
            }
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        protected void cmdUpdate_Click( Object sender, EventArgs e )
        {
            if( AddUser )
            {
                if( IsValid )
                {
                    CreateUser();
                }
            }
            else
            {
                if( UserEditor.IsValid && UserEditor.IsDirty && ( User != null ) )
                {
                    UserController.UpdateUser( PortalId, User );
                    OnUserUpdated( EventArgs.Empty );
                    OnUserUpdateCompleted( EventArgs.Empty );
                }
            }
        }

        /// <summary>
        /// The UserCreatedEventArgs class provides a customised EventArgs class for
        /// the UserCreated Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/08/2006  created
        /// </history>
        public class UserCreatedEventArgs
        {
            public UserCreatedEventArgs()
            {
                _CreateStatus = UserCreateStatus.Success;
            }

            private UserInfo _NewUser;
            private UserCreateStatus _CreateStatus;
            private bool _Notify = false;

            /// <summary>
            /// Constructs a new UserCreatedEventArgs
            /// </summary>
            /// <param name="newUser">The newly Created User</param>
            /// <history>
            /// 	[cnurse]	03/08/2006  Created
            /// </history>
            public UserCreatedEventArgs( UserInfo newUser )
            {
                _CreateStatus = UserCreateStatus.Success;

                _NewUser = newUser;
            }

            /// <summary>
            /// Gets and sets the Create Status
            /// </summary>
            /// <history>
            /// 	[cnurse]	03/08/2006  Created
            /// </history>
            public UserCreateStatus CreateStatus
            {
                get
                {
                    return _CreateStatus;
                }
                set
                {
                    _CreateStatus = value;
                }
            }

            /// <summary>
            /// Gets and sets the New User
            /// </summary>
            /// <history>
            /// 	[cnurse]	03/08/2006  Created
            /// </history>
            public UserInfo NewUser
            {
                get
                {
                    return _NewUser;
                }
                set
                {
                    _NewUser = value;
                }
            }

            /// <summary>
            /// Gets and sets a flag whether to Notify the new User of the Creation
            /// </summary>
            /// <history>
            /// 	[cnurse]	03/08/2006  Created
            /// </history>
            public bool Notify
            {
                get
                {
                    return _Notify;
                }
                set
                {
                    _Notify = value;
                }
            }
        }

        /// <summary>
        /// The UserDeletedEventArgs class provides a customised EventArgs class for
        /// the UserDeleted Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/08/2006  created
        /// </history>
        public class UserDeletedEventArgs
        {
            private string _userName;
            private int _userId;

            /// <summary>
            /// Constructs a new UserDeletedEventArgs
            /// </summary>
            /// <param name="id">The Id of the deleted User</param>
            /// <param name="name">The user name of the deleted User</param>
            /// <history>
            /// 	[cnurse]	03/08/2006  Created
            /// </history>
            public UserDeletedEventArgs( int id, string name )
            {
                _userId = id;
                _userName = name;
            }

            /// <summary>
            /// Gets and sets the Id of the deleted User
            /// </summary>
            /// <history>
            /// 	[cnurse]	03/08/2006  Created
            /// </history>
            public int UserId
            {
                get
                {
                    return _userId;
                }
                set
                {
                    _userId = value;
                }
            }

            /// <summary>
            /// Gets and sets the username of the deleted User
            /// </summary>
            /// <history>
            /// 	[cnurse]	03/08/2006  Created
            /// </history>
            public string UserName
            {
                get
                {
                    return _userName;
                }
                set
                {
                    _userName = value;
                }
            }
        }
    }
}