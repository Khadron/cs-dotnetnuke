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
                ProfileUpdatedEvent += value;
            }
            remove
            {
                ProfileUpdatedEvent -= value;
            }
        }

        private EventHandler ProfileUpdateCompletedEvent;

        public event EventHandler ProfileUpdateCompleted
        {
            add
            {
                ProfileUpdateCompletedEvent += value;
            }
            remove
            {
                ProfileUpdateCompletedEvent -= value;
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
                bool isValid = false;

                if( ProfileProperties.IsValid || IsAdmin )
                {
                    isValid = true;
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
                UserProfile profile = null;
                if( User != null )
                {
                    profile = User.Profile;
                }
                return profile;
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
                lblTitle.Text = string.Format( Localization.GetString( "ProfileTitle.Text", LocalResourceFile ), User.Username, User.UserID );
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
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            ProfileProperties.LocalResourceFile = this.LocalResourceFile;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            this.cmdUpdate.Click +=new EventHandler(cmdUpdate_Click);
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update Button is clicked
        /// </summary>
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