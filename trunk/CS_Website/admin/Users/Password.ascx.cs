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
                PasswordUpdatedEvent = (PasswordUpdatedEventHandler)Delegate.Combine( PasswordUpdatedEvent, value );
            }
            remove
            {
                PasswordUpdatedEvent = (PasswordUpdatedEventHandler)Delegate.Remove( PasswordUpdatedEvent, value );
            }
        }

        private PasswordUpdatedEventHandler PasswordQuestionAnswerUpdatedEvent;

        public event PasswordUpdatedEventHandler PasswordQuestionAnswerUpdated
        {
            add
            {
                PasswordQuestionAnswerUpdatedEvent = (PasswordUpdatedEventHandler)Delegate.Combine( PasswordQuestionAnswerUpdatedEvent, value );
            }
            remove
            {
                PasswordQuestionAnswerUpdatedEvent = (PasswordUpdatedEventHandler)Delegate.Remove( PasswordQuestionAnswerUpdatedEvent, value );
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
                UserMembership _Membership = null;
                if( User != null )
                {
                    _Membership = User.Membership;
                }
                return _Membership;
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
                lblTitle.Text = string.Format( Localization.GetString( "PasswordTitle.Text", LocalResourceFile ), User.Username, User.UserID.ToString() );
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
                    lblResetHelp.Visible = false;
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
        /// Page_Init runs when the control is initialised
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/03/2006  created
        /// </history>
        protected void Page_Init( Object sender, EventArgs e )
        {
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/03/2006  created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
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

            //4. Check New Password is ddifferent
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