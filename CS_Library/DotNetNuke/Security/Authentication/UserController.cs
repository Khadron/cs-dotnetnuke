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
using System.Collections;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Security.Authentication
{
    public class UserController
    {
        private string mProviderTypeName = "";

        public UserController()
        {
            Configuration _config = Configuration.GetConfig();
            mProviderTypeName = _config.ProviderTypeName;
        }

        public UserCreateStatus AddDNNUser(UserInfo AuthenticationUser)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            PortalSecurity objSecurity = new PortalSecurity();

            Entities.Users.UserController objDNNUsers = new Entities.Users.UserController();
            UserController objAuthUsers = new UserController();

            Entities.Users.UserInfo objDNNUser = (Entities.Users.UserInfo)AuthenticationUser;
            int AffiliateId = -1;

            if (HttpContext.Current.Request.Cookies["AffiliateId"] != null)
            {
                AffiliateId = int.Parse(HttpContext.Current.Request.Cookies["AffiliateId"].Value);
            }

            int UserID = -1;
            UserCreateStatus createStatus;
            createStatus = Entities.Users.UserController.CreateUser(ref objDNNUser);
            UserID = objDNNUser.UserID;

            if (AuthenticationUser.AuthenticationExists && UserID > -1)
            {
                AuthenticationUser.UserID = UserID;
                AddUserRoles(_portalSettings.PortalId, AuthenticationUser);
            }

            return createStatus;
        }

        public UserInfo GetUser(string LoggedOnUserName)
        {
            return AuthenticationProvider.Instance(mProviderTypeName).GetUser(LoggedOnUserName);
        }

        public UserInfo GetUser(string LoggedOnUserName, string LoggedOnPassword)
        {
            return AuthenticationProvider.Instance(mProviderTypeName).GetUser(LoggedOnUserName, LoggedOnPassword);
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// This routine is more accurated,
        /// Prevent user assign to admin role in case user logon as LOCAL\Administrator
        /// </remarks>
        public static void AddUserRoles(int PortalID, UserInfo AuthenticationUser)
        {
            GroupController objGroupController = new GroupController();
            ArrayList colGroup = objGroupController.GetGroups();
            RoleController objRoles = new RoleController();
            GroupInfo authenticationGroup;
            try
            {
                foreach( GroupInfo tempLoopVar_authenticationGroup in colGroup )
                {
                    authenticationGroup = tempLoopVar_authenticationGroup;
                    if( objGroupController.IsAuthenticationMember( authenticationGroup, AuthenticationUser ) )
                    {
                        objRoles.AddUserRole( PortalID, AuthenticationUser.UserID, authenticationGroup.RoleID, Null.NullDate, Null.NullDate );
                    }
                }
            }
            catch( Exception exc )
            {
                Exceptions.LogException( exc );
            }
        }
    }
}