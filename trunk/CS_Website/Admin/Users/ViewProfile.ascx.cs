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
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Modules.Admin.Users
{
    /// <summary>
    /// The ViewProfile UserModuleBase is used to view a Users Profile
    /// </summary>
    /// <history>
    /// 	[cnurse]	05/02/2006   Created
    /// </history>
    public partial class ViewProfile : UserModuleBase
    {
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
            // get userid from encrypted ticket
            UserId = Null.NullInteger;
            if( Context.Request.QueryString["userticket"] != null )
            {
                UserId = int.Parse( UrlUtils.DecryptParameter( Context.Request.QueryString["userticket"] ) );
            }

            ctlProfile.ID = "Profile";
            ctlProfile.UserId = UserId;
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
                //Before we bind the Profile to the editor we need to "update" the visibility data
                ProfilePropertyDefinitionCollection properties = ctlProfile.UserProfile.ProfileProperties;

                foreach( ProfilePropertyDefinition profProperty in properties )
                {
                    if( profProperty.Visible )
                    {
                        //Check Visibility
                        if( profProperty.Visibility == UserVisibilityMode.AdminOnly )
                        {
                            //Only Visible if Admin (or self)
                            profProperty.Visible = IsAdmin || IsUser;
                        }
                        else if( profProperty.Visibility == UserVisibilityMode.MembersOnly )
                        {
                            //Only Visible if Is a Member (ie Authenticated)
                            profProperty.Visible = Request.IsAuthenticated;
                        }
                    }
                }

                //Bind the profile information to the control
                ctlProfile.DataBind();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}