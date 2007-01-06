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
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;

namespace DotNetNuke.Security.Authentication
{
    public abstract class AuthenticationProvider
    {
        // singleton reference to the instantiated object
        private static AuthenticationProvider objProvider = null;

        static AuthenticationProvider()
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            //Dim _config As Authentication.Configuration = Authentication.Configuration.GetConfig(_portalSettings.PortalId)
            Configuration _config = Configuration.GetConfig();
            string strKey = "AuthenticationProvider" + _portalSettings.PortalId.ToString();

            objProvider = (AuthenticationProvider)Reflection.CreateObject(_config.ProviderTypeName, strKey);
        }

        public new static AuthenticationProvider Instance(string AuthenticationTypeName)
        {
            //CreateProvider()
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            string strKey = "AuthenticationProvider" + _portalSettings.PortalId.ToString();
            objProvider = (AuthenticationProvider)Reflection.CreateObject(AuthenticationTypeName, strKey);
            return objProvider;
        }

        public abstract UserInfo GetUser(string LoggedOnUserName, string LoggedOnPassword);
        public abstract UserInfo GetUser(string LoggedOnUserName);
        public abstract ArrayList GetGroups();
        public abstract Array GetAuthenticationTypes();
        public abstract bool IsAuthenticationMember(GroupInfo AuthenticationGroup, UserInfo AuthenticationUser);
        public abstract string GetNetworkStatus();
    }
}