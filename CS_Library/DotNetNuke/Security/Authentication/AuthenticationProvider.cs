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