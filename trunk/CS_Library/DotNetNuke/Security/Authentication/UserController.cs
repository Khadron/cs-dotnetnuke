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