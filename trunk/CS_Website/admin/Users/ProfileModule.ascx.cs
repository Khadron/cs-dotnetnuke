using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.Modules.Admin.Users
{
    /// <summary>
    /// The Profile UserModuleBase is used to register Users
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	03/02/2006
    /// </history>
    public partial class ProfileModule : UserModuleBase
    {
        private EventHandler ProfileUpdatedEvent;

        public event EventHandler ProfileUpdated
        {
            add
            {
                ProfileUpdatedEvent = (EventHandler)Delegate.Combine( ProfileUpdatedEvent, value );
            }
            remove
            {
                ProfileUpdatedEvent = (EventHandler)Delegate.Remove( ProfileUpdatedEvent, value );
            }
        }

        private EventHandler ProfileUpdateCompletedEvent;

        public event EventHandler ProfileUpdateCompleted
        {
            add
            {
                ProfileUpdateCompletedEvent = (EventHandler)Delegate.Combine( ProfileUpdateCompletedEvent, value );
            }
            remove
            {
                ProfileUpdateCompletedEvent = (EventHandler)Delegate.Remove( ProfileUpdateCompletedEvent, value );
            }
        }

        /// <summary>
        /// Gets and sets the EditorMode
        /// </summary>
        /// <history>
        /// 	[cnurse]	05/02/2006  Created
        /// </history>
        public PropertyEditorMode EditorMode
        {
            get
            {
                return ProfileProperties.EditMode;
            }
            set
            {
                ProfileProperties.EditMode = value;
            }
        }

        /// <summary>
        /// Gets whether the User is valid
        /// </summary>
        /// <history>
        /// 	[cnurse]	05/18/2006  Created
        /// </history>
        public bool IsValid
        {
            get
            {
                bool _IsValid = false;

                if( ProfileProperties.IsValid || IsAdmin )
                {
                    _IsValid = true;
                }

                return _IsValid;
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
                return cmdUpdate.Visible;
            }
            set
            {
                cmdUpdate.Visible = value;
            }
        }

        /// <summary>
        /// Gets the UserProfile associated with this control
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/02/2006  Created
        /// </history>
        public UserProfile UserProfile
        {
            get
            {
                UserProfile _Profile = null;
                if( User != null )
                {
                    _Profile = User.Profile;
                }
                return _Profile;
            }
        }

        /// <summary>
        /// Raises the OnProfileUpdateCompleted Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/13/2006  Created
        /// </history>
        public void OnProfileUpdateCompleted( EventArgs e )
        {
            if( ProfileUpdateCompletedEvent != null )
            {
                ProfileUpdateCompletedEvent( this, e );
            }
        }

        /// <summary>
        /// Raises the ProfileUpdated Event
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/16/2006  Created
        /// </history>
        public void OnProfileUpdated( EventArgs e )
        {
            if( ProfileUpdatedEvent != null )
            {
                ProfileUpdatedEvent( this, e );
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
            if( IsAdmin )
            {
                lblTitle.Text = string.Format( Localization.GetString( "ProfileTitle.Text", LocalResourceFile ), User.Username, User.UserID.ToString() );
            }
            else
            {
                trTitle.Visible = false;
            }

            //Before we bind the Profile to the editor we need to "update" the visible data
            ProfilePropertyDefinitionCollection properties = UserProfile.ProfileProperties;

            foreach( ProfilePropertyDefinition profProperty in properties )
            {
                if( IsAdmin )
                {
                    profProperty.Visible = true;
                }
            }

            ProfileProperties.DataSource = UserProfile.ProfileProperties;
            ProfileProperties.DataBind();
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
            ProfileProperties.LocalResourceFile = this.LocalResourceFile;
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        protected void cmdUpdate_Click( object sender, EventArgs e )
        {
            if( IsValid )
            {
                ProfilePropertyDefinitionCollection properties = (ProfilePropertyDefinitionCollection)ProfileProperties.DataSource;

                //Update User's profile
                User = ProfileController.UpdateUserProfile( User, properties );

                OnProfileUpdated( EventArgs.Empty );
                OnProfileUpdateCompleted( EventArgs.Empty );
            }
        }
    }
}