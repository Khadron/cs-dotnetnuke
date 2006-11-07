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
using System.Collections;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Membership;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.Modules.Admin.Users
{
    /// <summary>
    /// The UserSettings PortalModuleBase is used to manage User Settings for the portal
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	03/02/2006
    /// </history>
    public partial class UserSettings : PortalModuleBase
    {
        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/02/2006  Created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            //Bind User Controller to PropertySettings
            MembershipProviderConfig config = new MembershipProviderConfig();
            if( MembershipProviderConfig.CanEditProviderProperties )
            {
                ProviderSettings.EditMode = PropertyEditorMode.Edit;
            }
            else
            {
                ProviderSettings.EditMode = PropertyEditorMode.View;
            }
            ProviderSettings.LocalResourceFile = this.LocalResourceFile;
            ProviderSettings.DataSource = config;
            ProviderSettings.DataBind();

            if( UserInfo.IsSuperUser )
            {
                PasswordSettings.EditMode = PropertyEditorMode.Edit;
            }
            else
            {
                PasswordSettings.EditMode = PropertyEditorMode.View;
            }
            PasswordSettings.LocalResourceFile = this.LocalResourceFile;
            PasswordSettings.DataSource = new PasswordConfig();
            PasswordSettings.DataBind();

            //Create a hashtable for the custom editors being used, using the same keys
            //as in the settings hashtable
            Hashtable editors = new Hashtable();
            editors["Redirect_AfterLogin"] = EditorInfo.GetEditor( "Page" );
            editors["Redirect_AfterLogout"] = EditorInfo.GetEditor( "Page" );
            editors["Redirect_AfterRegistration"] = EditorInfo.GetEditor( "Page" );

            UserSettingsControl.LocalResourceFile = this.LocalResourceFile;
            UserSettingsControl.DataSource = UserModuleBase.GetSettings(new Hashtable(this.Settings));
            UserSettingsControl.CustomEditors = editors;
            UserSettingsControl.DataBind();
        }

        /// <summary>
        /// cmdCancel_Click runs when the Cancel Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/02/2006
        /// </history>
        internal void cmdCancel_Click( object sender, EventArgs e )
        {
            Response.Redirect( Globals.NavigateURL( TabId, "", ( ( Request.QueryString["filter"] != "" ) ? ( "filter=" + Request.QueryString["filter"] ) : "" ).ToString() ), true );
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/02/2006
        /// </history>
        protected void cmdUpdate_Click( object sender, EventArgs e )
        {
            UserModuleBase.UpdateSettings(this.PortalId, ((Hashtable)UserSettingsControl.DataSource));

            Response.Redirect( Globals.NavigateURL( TabId, "", ( ( Request.QueryString["filter"] != "" ) ? ( "filter=" + Request.QueryString["filter"] ) : "" ).ToString() ), true );
        }
    }
}