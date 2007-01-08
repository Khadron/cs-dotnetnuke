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
using System.Data;

namespace DotNetNuke.Security.Membership.Data
{
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Security.Membership
    /// Class:      DataProvider
    /// <summary>
    /// The DataProvider provides the abstract Data Access Layer for the project
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	03/28/2006	created
    /// </history>
    public abstract class DataProvider
    {
        // singleton reference to the instantiated object
        private static DataProvider objProvider = null;

        // constructor
        static DataProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (DataProvider)Framework.Reflection.CreateObject( "data", "DotNetNuke.Security.Membership.Data", "DotNetNuke.Provider.Membership" );
        }

        // return the provider
        public static DataProvider Instance()
        {
            return objProvider;
        }

        //Login/Security
        public abstract IDataReader UserLogin( string Username, string Password );
        public abstract IDataReader GetAuthRoles( int PortalId, int ModuleId );

        //Users
        public abstract int AddUser( int PortalID, string Username, string FirstName, string LastName, int AffiliateId, bool IsSuperUser, string Email, string DisplayName, bool UpdatePassword, bool IsApproved );
        public abstract void DeleteUser( int UserId );
        public abstract void DeleteUserPortal( int UserId, int PortalId );
        public abstract IDataReader GetAllUsers( int PortalID, int pageIndex, int pageSize );
        public abstract IDataReader GetUnAuthorizedUsers( int portalId );
        public abstract IDataReader GetUsers( int PortalId );
        public abstract IDataReader GetUser( int PortalId, int UserId );
        public abstract IDataReader GetUserByUsername( int PortalID, string Username );
        public abstract int GetUserCountByPortal( int portalId );
        public abstract IDataReader GetUsersByEmail( int PortalID, string Email, int pageIndex, int pageSize );
        public abstract IDataReader GetUsersByProfileProperty( int PortalID, string propertyName, string propertyValue, int pageIndex, int pageSize );
        public abstract IDataReader GetUsersByRolename( int PortalID, string Rolename );
        public abstract IDataReader GetUsersByUsername( int PortalID, string Username, int pageIndex, int pageSize );
        public abstract IDataReader GetSuperUsers();
        public abstract void UpdateUser( int UserId, int PortalID, string FirstName, string LastName, string Email, string DisplayName, bool UpdatePassword, bool IsApproved );

        //Roles
        public abstract IDataReader GetPortalRoles( int PortalId );
        public abstract IDataReader GetRoles();
        public abstract IDataReader GetRole( int RoleID, int PortalID );
        public abstract IDataReader GetRoleByName( int PortalId, string RoleName );
        public abstract int AddRole( int PortalId, int RoleGroupId, string RoleName, string Description, float ServiceFee, string BillingPeriod, string BillingFrequency, float TrialFee, int TrialPeriod, string TrialFrequency, bool IsPublic, bool AutoAssignment, string RSVPCode, string IconFile );
        public abstract void DeleteRole( int RoleId );
        public abstract void UpdateRole( int RoleId, int RoleGroupId, string Description, float ServiceFee, string BillingPeriod, string BillingFrequency, float TrialFee, int TrialPeriod, string TrialFrequency, bool IsPublic, bool AutoAssignment, string RSVPCode, string IconFile );
        public abstract IDataReader GetRolesByUser( int UserId, int PortalId );

        //RoleGroups
        public abstract int AddRoleGroup( int PortalId, string GroupName, string Description );
        public abstract void DeleteRoleGroup( int RoleGroupId );
        public abstract IDataReader GetRoleGroup( int portalId, int roleGroupId );
        public abstract IDataReader GetRoleGroups( int portalId );
        public abstract IDataReader GetRolesByGroup( int RoleGroupId, int PortalId );
        public abstract void UpdateRoleGroup( int RoleGroupId, string GroupName, string Description );

        //User Roles
        public abstract IDataReader GetUserRole( int PortalID, int UserId, int RoleId );
        public abstract IDataReader GetUserRoles( int PortalID, int UserId );
        public abstract IDataReader GetUserRolesByUsername( int PortalID, string Username, string Rolename );
        public abstract int AddUserRole( int PortalID, int UserId, int RoleId, DateTime EffectiveDate, DateTime ExpiryDate );
        public abstract void UpdateUserRole( int UserRoleId, DateTime EffectiveDate, DateTime ExpiryDate );
        public abstract void DeleteUserRole( int UserId, int RoleId );
        public abstract IDataReader GetServices( int PortalId, int UserId );

        //Profile
        public abstract IDataReader GetUserProfile( int UserId );
        public abstract void UpdateProfileProperty( int ProfileId, int UserId, int PropertyDefinitionID, string PropertyValue, int Visibility, DateTime LastUpdatedDate );

        // users online
        public abstract void UpdateUsersOnline( Hashtable UserList );
        public abstract void DeleteUsersOnline( int TimeWindow );
        public abstract IDataReader GetOnlineUser( int UserId );
        public abstract IDataReader GetOnlineUsers( int PortalId );
    }
}