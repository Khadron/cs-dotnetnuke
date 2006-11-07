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
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Personalization;

namespace DotNetNuke.HttpModules
{
    public class PersonalizationModule : IHttpModule
    {
        public string ModuleName
        {
            get
            {
                return "PersonalizationModule";
            }
        }

        public void Init( HttpApplication application )
        {
            application.EndRequest += new System.EventHandler( this.OnEndRequest );
        }

        public void OnEndRequest( object s, EventArgs e )
        {
            HttpContext Context = ( (HttpApplication)s ).Context;
            HttpRequest Request = Context.Request;

            if( Request.IsAuthenticated == true )
            {
                // Obtain PortalSettings from Current Context
                PortalSettings _portalSettings = (PortalSettings)Context.Items["PortalSettings"];

                if( !( _portalSettings == null ) )
                {
                    // load the user info object
                    UserInfo userInfo = UserController.GetCurrentUserInfo();

                    if (userInfo.UserID != -1)
                    {
                        PersonalizationController objPersonalization = new PersonalizationController();
                        objPersonalization.SaveProfile(Context, userInfo.UserID, _portalSettings.PortalId);
                    }
                }
            }
        }

        public void Dispose()
        {
        }
    }
}