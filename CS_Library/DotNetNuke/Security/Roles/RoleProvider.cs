using System.Collections;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;

namespace DotNetNuke.Security.Roles
{
    public abstract class RoleProvider
    {
        // singleton reference to the instantiated object
        private static RoleProvider objProvider = null;

        // constructor
        static RoleProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (RoleProvider)Reflection.CreateObject( "roles" );
        }

        // return the provider
        public new static RoleProvider Instance()
        {
            return objProvider;
        }

        //Roles
        public abstract bool CreateRole( int portalId, ref RoleInfo role );
        public abstract void DeleteRole( int portalId, ref RoleInfo role );
        public abstract RoleInfo GetRole( int portalId, int roleId );
        public abstract RoleInfo GetRole( int portalId, string roleName );
        public abstract string[] GetRoleNames( int portalId );
        public abstract string[] GetRoleNames( int portalId, int userId );
        public abstract ArrayList GetRoles( int portalId );
        public abstract ArrayList GetRolesByGroup( int portalId, int roleGroupId );
        public abstract void UpdateRole( RoleInfo role );

        //Role Groups
        public abstract int CreateRoleGroup( RoleGroupInfo roleGroup );
        public abstract void DeleteRoleGroup( RoleGroupInfo roleGroup );
        public abstract RoleGroupInfo GetRoleGroup( int portalId, int roleGroupId );
        public abstract ArrayList GetRoleGroups( int portalId );
        public abstract void UpdateRoleGroup( RoleGroupInfo roleGroup );

        //User Roles
        public abstract bool AddUserToRole( int portalId, UserInfo user, UserRoleInfo userRole );
        public abstract UserRoleInfo GetUserRole( int PortalId, int UserId, int RoleId );
        public abstract ArrayList GetUserRoles( int PortalId, int UserId, bool includePrivate );
        public abstract ArrayList GetUserRoles( int PortalId, string Username, string Rolename );
        public abstract ArrayList GetUsersByRoleName( int portalId, string roleName );
        public abstract ArrayList GetUserRolesByRoleName( int portalId, string roleName );
        public abstract void RemoveUserFromRole( int portalId, UserInfo user, UserRoleInfo userRole );
        public abstract void UpdateUserRole( UserRoleInfo userRole );
    }
}