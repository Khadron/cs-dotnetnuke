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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Admin.Users
{
    /// <summary>
    /// The Password UserModuleBase is used to manage Users Passwords
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	03/03/2006  created
    /// </history>
    public partial class Password : UserModuleBase
    {
        public delegate void PasswordUpdatedEventHandler( object sender, PasswordUpdatedEventArgs e );

        private PasswordUpdatedEventHandler PasswordUpdatedEvent;

        public event PasswordUpdatedEventHandler PasswordUpdated
        {
            add
            {
                PasswordUpdatedEvent += value;
            }
            remove
            {
                PasswordUpdatedEvent -= value;
            }
        }

        private PasswordUpdatedEventHandler PasswordQuestionAnswerUpdatedEvent;

        public event PasswordUpdatedEventHandler PasswordQuestionAnswerUpdated
        {
            add
            {
                PasswordQuestionAnswerUpdatedEvent += value;
            }
            remove
            {
                PasswordQuestionAnswerUpdatedEvent -= value;
            }
        }

        /// <summary>
        /// Gets the UserMembership associated with this control
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/03/2006  Created
        /// </history>
        public UserMembership Membership
        {
            get
            {
                UserMembership membership = null;
                if( User != null )
                {
                    membership = User.Membership;
                }
                return membership;
            }
        }

        /// <summary>
        /// Raises the PasswordUpdated Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/08/2006  Created
        /// </history>
        public void OnPasswordUpdated( PasswordUpdatedEventArgs e )
        {
            if( PasswordUpdatedEvent != null )
            {
                PasswordUpdatedEvent( this, e );
            }
        }

        /// <summary>
        /// Raises the PasswordQuestionAnswerUpdated Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/09/2006  Created
        /// </history>
        public void OnPasswordQuestionAnswerUpdated( PasswordUpdatedEventArgs e )
        {
            if( PasswordQuestionAnswerUpdatedEvent != null )
            {
                PasswordQuestionAnswerUpdatedEvent( this, e );
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            this.cmdReset.Click+=new EventHandler(cmdReset_Click);
            this.cmdUpdate.Click+=new EventHandler(cmdUpdate_Click);
            this.cmdUpdateQA.Click+=new EventHandler(cmdUpdateQA_Click);
        }

        /// <summary>
        /// DataBind binds the data to the controls
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/03/2006  Created
        /// </history>
        public override void DataBind()
        {
            if( IsAdmin )
            {
                lblTitle.Text = string.Format( Localization.GetString( "PasswordTitle.Text", LocalResourceFile ), User.Username, User.UserID );
            }
            else
            {
                trTitle.Visible = false;
            }

            lblLastChanged.Text = User.Membership.LastPasswordChangeDate.ToLongDateString();

            //Set Password Expiry Label
            if( PasswordConfig.PasswordExpiry > 0 )
            {
                lblExpires.Text = User.Membership.LastPasswordChangeDate.AddDays( PasswordConfig.PasswordExpiry ).ToLongDateString();
            }
            else
            {
                lblExpires.Text = Localization.GetString( "NoExpiry", this.LocalResourceFile );
            }

            //If Password retrieval is not supported then only the user can change
            //their password, an Admin must Reset
            if( ! MembershipProviderConfig.PasswordRetrievalEnabled && ! IsUser )
            {
                pnlChange.Visible = false;
            }
            else
            {
                pnlChange.Visible = true;

                //Set up Change Password
                if( IsAdmin && ! IsUser )
                {
                    lblChangeHelp.Text = Localization.GetString( "AdminChangeHelp", this.LocalResourceFile );
                    trOldPassword.Visible = false;
                }
                else
                {
                    lblChangeHelp.Text = Localization.GetString( "UserChangeHelp", this.LocalResourceFile );
                }
            }

            //If Password Reset is not enabled then only the Admin can reset the
            //Password, a User must Update
            if( ! MembershipProviderConfig.PasswordResetEnabled )
            {
                pnlReset.Visible = false;
            }
            else
            {
                pnlReset.Visible = true;

                //Set up Reset Password
                if( IsAdmin && ! IsUser )
                {
                    if (MembershipProviderConfig.RequiresQuestionAndAnswer)
                    {
                        pnlReset.Visible = false;
                    }
                    else
                    {
                        lblResetHelp.Visible = false;
                    }
                    trQuestion.Visible = false;
                    trAnswer.Visible = false;
                }
                else
                {
                    if( MembershipProviderConfig.RequiresQuestionAndAnswer && IsUser )
                    {
                        lblResetHelp.Text = Localization.GetString( "UserResetHelp", this.LocalResourceFile );
                        lblQuestion.Text = User.Membership.PasswordQuestion;
                        trQuestion.Visible = true;
                        trAnswer.Visible = true;
                    }
                    else
                    {
                        pnlReset.Visible = false;
                    }
                }
            }

            //Set up Edit Question and Answer area
            if( MembershipProviderConfig.RequiresQuestionAndAnswer && IsUser )
            {
                pnlQA.Visible = true;
            }
            else
            {
                pnlQA.Visible = false;
            }
        }

        /// <summary>
        /// cmdReset_Click runs when the Reset Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/03/2006  created
        /// </history>
        protected void cmdReset_Click( object sender, EventArgs e )
        {
            string answer = "";
            if( MembershipProviderConfig.RequiresQuestionAndAnswer && ! IsAdmin )
            {
                if( txtAnswer.Text == "" )
                {
                    OnPasswordUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.InvalidPasswordAnswer ) );
                    return;
                }
                answer = txtAnswer.Text;
            }

            try
            {
                UserController.ResetPassword( User, answer );
                OnPasswordUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.Success ) );
            }
            catch( ArgumentException )
            {
                OnPasswordUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.InvalidPasswordAnswer ) );
            }
            catch( Exception )
            {
                OnPasswordUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.PasswordResetFailed ) );
            }
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update  Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/03/2006  created
        /// </history>
        protected void cmdUpdate_Click( Object sender, EventArgs e )
        {
            //1. Check New Password and Confirm are the same
            if( txtNewPassword.Text != txtNewConfirm.Text )
            {
                OnPasswordUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.PasswordMismatch ) );
                return;
            }

            //2. Check New Password is Valid
            if( ! UserController.ValidatePassword( txtNewPassword.Text ) )
            {
                OnPasswordUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.PasswordInvalid ) );
                return;
            }

            //3. Check old Password is Provided
            if( ! IsAdmin && txtOldPassword.Text == "" )
            {
                OnPasswordUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.PasswordMissing ) );
                return;
            }

            //4. Check New Password is different
            if( ! IsAdmin && txtNewPassword.Text == txtOldPassword.Text )
            {
                OnPasswordUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.PasswordNotDifferent ) );
                return;
            }

            //Try and set password
            string oldPassword = Null.NullString;
            if( IsUser )
            {
                oldPassword = txtOldPassword.Text;
            }
            if( UserController.ChangePassword( User, oldPassword, txtNewPassword.Text ) )
            {
                //Success
                OnPasswordUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.Success ) );
            }
            else
            {
                //Fail
                OnPasswordUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.PasswordResetFailed ) );
            }
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update Question and Answer  Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/09/2006  created
        /// </history>
        protected void cmdUpdateQA_Click( object sender, EventArgs e )
        {
            if( txtQAPassword.Text == "" )
            {
                OnPasswordQuestionAnswerUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.PasswordInvalid ) );
                return;
            }

            if( txtEditQuestion.Text == "" )
            {
                OnPasswordQuestionAnswerUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.InvalidPasswordQuestion ) );
                return;
            }

            if( txtEditAnswer.Text == "" )
            {
                OnPasswordQuestionAnswerUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.InvalidPasswordAnswer ) );
                return;
            }

            //Try and set password Q and A
            UserInfo objUser = UserController.GetUser( PortalId, UserId, false );
            if( UserController.ChangePasswordQuestionAndAnswer( objUser, txtQAPassword.Text, txtEditQuestion.Text, txtEditAnswer.Text ) )
            {
                //Success
                OnPasswordQuestionAnswerUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.Success ) );
            }
            else
            {
                //Fail
                OnPasswordQuestionAnswerUpdated( new PasswordUpdatedEventArgs( PasswordUpdateStatus.PasswordResetFailed ) );
            }
        }

        /// <summary>
        /// The PasswordUpdatedEventArgs class provides a customised EventArgs class for
        /// the PasswordUpdated Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/08/2006  created
        /// </history>
        public class PasswordUpdatedEventArgs
        {
            private PasswordUpdateStatus _UpdateStatus;

            /// <summary>
            /// Constructs a new PasswordUpdatedEventArgs
            /// </summary>
            /// <param name="status">The Password Update Status</param>
            /// <history>
            /// 	[cnurse]	03/08/2006  Created
            /// </history>
            public PasswordUpdatedEventArgs( PasswordUpdateStatus status )
            {
                _UpdateStatus = status;
            }

            /// <summary>
            /// Gets and sets the Update Status
            /// </summary>
            /// <history>
            /// 	[cnurse]	03/08/2006  Created
            /// </history>
            public PasswordUpdateStatus UpdateStatus
            {
                get
                {
                    return _UpdateStatus;
                }
                set
                {
                    _UpdateStatus = value;
                }
            }
        }
    }
}