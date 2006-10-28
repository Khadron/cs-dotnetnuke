using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Modules.Admin.Users
{
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Modules.Admin.Users
    /// Class:      Membership
    /// <summary>
    /// The Membership UserModuleBase is used to manage the membership aspects of a
    /// User
    /// </summary>
    /// <history>
    /// 	[cnurse]	03/01/2006  Created
    /// </history>
    public partial class Membership : UserModuleBase
    {
        private EventHandler MembershipAuthorizedEvent;

        public event EventHandler MembershipAuthorized
        {
            add
            {
                MembershipAuthorizedEvent = (EventHandler)Delegate.Combine( MembershipAuthorizedEvent, value );
            }
            remove
            {
                MembershipAuthorizedEvent = (EventHandler)Delegate.Remove( MembershipAuthorizedEvent, value );
            }
        }

        private EventHandler MembershipUnAuthorizedEvent;

        public event EventHandler MembershipUnAuthorized
        {
            add
            {
                MembershipUnAuthorizedEvent = (EventHandler)Delegate.Combine( MembershipUnAuthorizedEvent, value );
            }
            remove
            {
                MembershipUnAuthorizedEvent = (EventHandler)Delegate.Remove( MembershipUnAuthorizedEvent, value );
            }
        }

        private EventHandler MembershipUnLockedEvent;

        public event EventHandler MembershipUnLocked
        {
            add
            {
                MembershipUnLockedEvent = (EventHandler)Delegate.Combine( MembershipUnLockedEvent, value );
            }
            remove
            {
                MembershipUnLockedEvent = (EventHandler)Delegate.Remove( MembershipUnLockedEvent, value );
            }
        }

        /// <summary>
        /// Gets the UserMembership associated with this control
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        public UserMembership UserMembership
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
        /// Raises the MembershipAuthorized Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        public void OnMembershipAuthorized( EventArgs e )
        {
            if( MembershipAuthorizedEvent != null )
            {
                MembershipAuthorizedEvent( this, e );
            }
        }

        /// <summary>
        /// Raises the MembershipUnAuthorized Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        public void OnMembershipUnAuthorized( EventArgs e )
        {
            if( MembershipUnAuthorizedEvent != null )
            {
                MembershipUnAuthorizedEvent( this, e );
            }
        }

        /// <summary>
        /// Raises the MembershipUnLocked Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        public void OnMembershipUnLocked( EventArgs e )
        {
            if( MembershipUnLockedEvent != null )
            {
                MembershipUnLockedEvent( this, e );
            }
        }

        /// <summary>
        /// DataBind binds the data to the controls
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        public override void DataBind()
        {
            //disable/enable buttons
            if( UserInfo.UserID == User.UserID || User.UserID == PortalSettings.AdministratorId )
            {
                cmdAuthorize.Visible = false;
                cmdUnAuthorize.Visible = false;
                cmdUnLock.Visible = false;
                cmdPassword.Visible = false;
            }
            else
            {
                cmdUnLock.Visible = UserMembership.LockedOut;
                cmdUnAuthorize.Visible = UserMembership.Approved;
                cmdAuthorize.Visible = ! UserMembership.Approved;
                cmdPassword.Visible = ! UserMembership.UpdatePassword;
            }

            MembershipEditor.DataSource = UserMembership;
            MembershipEditor.DataBind();
        }

        /// <summary>
        /// Page_Init runs when the control is initialised
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
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
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            MembershipEditor.LocalResourceFile = this.LocalResourceFile;
        }

        /// <summary>
        /// cmdAuthorize_Click runs when the Authorize User Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        protected void cmdAuthorize_Click( object sender, EventArgs e )
        {
            //Get the Membership Information from the property editors
            User.Membership = (UserMembership)MembershipEditor.DataSource;

            User.Membership.Approved = true;

            //Update User
            UserController.UpdateUser( PortalId, User );

            OnMembershipAuthorized( EventArgs.Empty );
        }

        /// <summary>
        /// cmdPassword_Click runs when the ChangePassword Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/15/2006  Created
        /// </history>
        protected void cmdPassword_Click( object sender, EventArgs e )
        {
            //Get the Membership Information from the property editors
            User.Membership = (UserMembership)MembershipEditor.DataSource;

            User.Membership.UpdatePassword = true;

            //Update User
            UserController.UpdateUser( PortalId, User );

            DataBind();
        }

        /// <summary>
        /// cmdUnAuthorize_Click runs when the UnAuthorize User Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        protected void cmdUnAuthorize_Click( object sender, EventArgs e )
        {
            //Get the Membership Information from the property editors
            User.Membership = (UserMembership)MembershipEditor.DataSource;

            User.Membership.Approved = false;

            //Update User
            UserController.UpdateUser( PortalId, User );

            OnMembershipUnAuthorized( EventArgs.Empty );
        }

        /// <summary>
        /// cmdUnlock_Click runs when the Unlock Account Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        protected void cmdUnLock_Click( Object sender, EventArgs e )
        {
            // update the user record in the database
            bool isUnLocked = UserController.UnLockUser( User );

            if( isUnLocked )
            {
                User.Membership.LockedOut = false;

                OnMembershipUnLocked( EventArgs.Empty );
            }
        }
    }
}