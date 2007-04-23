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
using System.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.Providers;
using Microsoft.ApplicationBlocks.Data;

namespace DotNetNuke.Security.Membership.Data
{
    /// <summary>
    /// The SqlDataProvider provides a concrete SQL Server implementation of the
    /// Data Access Layer for the project
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	03/28/2006	created
    /// </history>
    public class OracleDataProvider : DataProvider
    {
        private const string ProviderType = "data";
        private ProviderConfiguration providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
        private string connectionString;
        private string providerPath;
        private string objectQualifier;
        private string databaseOwner;

        public OracleDataProvider()
        {
            // Read the configuration specific information for this provider
            Provider objProvider = (Provider)providerConfiguration.Providers[providerConfiguration.DefaultProvider];

            // Read the attributes for this provider
            //Get Connection string from web.config
            connectionString = Config.GetConnectionString();

            if( connectionString == "" )
            {
                // Use connection string specified in provider
                connectionString = objProvider.Attributes["connectionString"];
            }

            providerPath = objProvider.Attributes["providerPath"];

            objectQualifier = objProvider.Attributes["objectQualifier"];
            if( !String.IsNullOrEmpty(objectQualifier) && objectQualifier.EndsWith( "_" ) == false )
            {
                objectQualifier += "_";
            }

            databaseOwner = objProvider.Attributes["databaseOwner"];
            if( !String.IsNullOrEmpty(databaseOwner) && databaseOwner.EndsWith( "." ) == false )
            {
                databaseOwner += ".";
            }
        }

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
        }

        public string ProviderPath
        {
            get
            {
                return providerPath;
            }
        }

        public string ObjectQualifier
        {
            get
            {
                return objectQualifier;
            }
        }

        public string DatabaseOwner
        {
            get
            {
                return databaseOwner;
            }
        }

        private object GetNull( object Field )
        {
            return Null.GetNull( Field, DBNull.Value );
        }

        private string GetFullyQualifiedName( string name )
        {
            return DatabaseOwner + ObjectQualifier + name;
        }

        //Security
        public override IDataReader UserLogin( string Username, string Password )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "UserLogin" ), Username, Password );
        }

        public override IDataReader GetAuthRoles( int PortalId, int ModuleId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetAuthRoles" ), PortalId, ModuleId );
        }

        //Users
        public override int AddUser( int PortalID, string Username, string FirstName, string LastName, int AffiliateId, bool IsSuperUser, string Email, string DisplayName, bool UpdatePassword, bool IsApproved )
        {
            try
            {
                return Convert.ToInt32( OracleHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddUser", PortalID, Username, FirstName, LastName, GetNull( AffiliateId ), IsSuperUser, Email, DisplayName, UpdatePassword, IsApproved ) );
            }
            catch // duplicate
            {
                return - 1;
            }
        }

        public override void DeleteUserPortal( int UserId, int PortalId )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "DeleteUserPortal" ), UserId, PortalId );
        }

        public override void DeleteUser( int UserId )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "DeleteUser" ), UserId );
        }

        public override void UpdateUser( int UserId, int PortalID, string FirstName, string LastName, string Email, string DisplayName, bool UpdatePassword, bool IsApproved )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "UpdateUser" ), UserId, PortalID, FirstName, LastName, Email, DisplayName, UpdatePassword, IsApproved );
        }

        public override IDataReader GetAllUsers( int PortalID, int pageIndex, int pageSize )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetAllUsers" ), GetNull( PortalID ), pageIndex, pageSize );
        }

        public override IDataReader GetUnAuthorizedUsers( int portalId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUnAuthorizedUsers" ), GetNull( portalId ) );
        }

        public override IDataReader GetUser( int PortalId, int UserId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUser" ), PortalId, UserId );
        }

        public override IDataReader GetUserByUsername( int PortalId, string Username )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUserByUsername" ), GetNull( PortalId ), Username );
        }

        public override int GetUserCountByPortal( int portalId )
        {
            return Convert.ToInt32( OracleHelper.ExecuteScalar( ConnectionString, GetFullyQualifiedName( "GetUserCountByPortal" ), portalId ) );
        }

        public override IDataReader GetUsersByEmail( int PortalID, string Email, int pageIndex, int pageSize )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUsersByEmail" ), GetNull( PortalID ), Email, pageIndex, pageSize );
        }

        public override IDataReader GetUsersByProfileProperty( int PortalID, string propertyName, string propertyValue, int pageIndex, int pageSize )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUsersByProfileProperty" ), GetNull( PortalID ), propertyName, propertyValue, pageIndex, pageSize );
        }

        public override IDataReader GetUsersByRolename( int PortalID, string Rolename )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUsersByRolename" ), GetNull( PortalID ), Rolename );
        }

        public override IDataReader GetUsersByUsername( int PortalID, string Username, int pageIndex, int pageSize )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUsersByUsername" ), GetNull( PortalID ), Username, pageIndex, pageSize );
        }

        public override IDataReader GetSuperUsers()
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetSuperUsers" ), null );
        }

        //Roles
        public override IDataReader GetRolesByUser( int UserId, int PortalId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetRolesByUser" ), UserId, PortalId );
        }

        public override IDataReader GetPortalRoles( int PortalId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetPortalRoles" ), PortalId );
        }

        public override IDataReader GetRoles()
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetRoles" ), null );
        }

        public override IDataReader GetRole( int RoleId, int PortalId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetRole" ), RoleId, PortalId );
        }

        public override IDataReader GetRoleByName( int PortalId, string RoleName )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetRoleByName" ), PortalId, RoleName );
        }

        public override IDataReader GetRolesByGroup( int RoleGroupId, int PortalId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetRolesByGroup" ), GetNull( RoleGroupId ), PortalId );
        }

        public override int AddRole( int PortalId, int RoleGroupId, string RoleName, string Description, float ServiceFee, string BillingPeriod, string BillingFrequency, float TrialFee, int TrialPeriod, string TrialFrequency, bool IsPublic, bool AutoAssignment, string RSVPCode, string IconFile )
        {
            return Convert.ToInt32( OracleHelper.ExecuteScalar( ConnectionString, GetFullyQualifiedName( "AddRole" ), PortalId, GetNull( RoleGroupId ), RoleName, Description, ServiceFee, BillingPeriod, GetNull( BillingFrequency ), TrialFee, TrialPeriod, GetNull( TrialFrequency ), IsPublic, AutoAssignment, RSVPCode, IconFile ) );
        }

        public override void DeleteRole( int RoleId )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "DeleteRole" ), RoleId );
        }

        public override void UpdateRole( int RoleId, int RoleGroupId, string Description, float ServiceFee, string BillingPeriod, string BillingFrequency, float TrialFee, int TrialPeriod, string TrialFrequency, bool IsPublic, bool AutoAssignment, string RSVPCode, string IconFile )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "UpdateRole" ), RoleId, GetNull( RoleGroupId ), Description, ServiceFee, BillingPeriod, GetNull( BillingFrequency ), TrialFee, TrialPeriod, GetNull( TrialFrequency ), IsPublic, AutoAssignment, RSVPCode, IconFile );
        }

        //Role Groups
        public override int AddRoleGroup( int PortalId, string GroupName, string Description )
        {
            return Convert.ToInt32( OracleHelper.ExecuteScalar( ConnectionString, GetFullyQualifiedName( "AddRoleGroup" ), PortalId, GroupName, Description ) );
        }

        public override void DeleteRoleGroup( int RoleGroupId )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "DeleteRoleGroup" ), RoleGroupId );
        }

        public override IDataReader GetRoleGroup( int portalId, int roleGroupId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetRoleGroup" ), portalId, roleGroupId );
        }

        public override IDataReader GetRoleGroups( int portalId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetRoleGroups" ), portalId );
        }

        public override void UpdateRoleGroup( int RoleGroupId, string GroupName, string Description )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "UpdateRoleGroup" ), RoleGroupId, GroupName, Description );
        }

        //User Roles
        public override IDataReader GetUserRole( int PortalID, int UserId, int RoleId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUserRole" ), PortalID, UserId, RoleId );
        }

        public override IDataReader GetUserRoles( int PortalID, int UserId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUserRoles" ), PortalID, UserId );
        }

        public override IDataReader GetUserRolesByUsername( int PortalID, string Username, string Rolename )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUserRolesByUsername" ), PortalID, GetNull( Username ), GetNull( Rolename ) );
        }

        public override int AddUserRole( int PortalId, int UserId, int RoleId, DateTime EffectiveDate, DateTime ExpiryDate )
        {
            return Convert.ToInt32( OracleHelper.ExecuteScalar( ConnectionString, GetFullyQualifiedName( "AddUserRole" ), PortalId, UserId, RoleId, GetNull( EffectiveDate ), GetNull( ExpiryDate ) ) );
        }

        public override void UpdateUserRole( int UserRoleId, DateTime EffectiveDate, DateTime ExpiryDate )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "UpdateUserRole" ), UserRoleId, GetNull( EffectiveDate ), GetNull( ExpiryDate ) );
        }

        public override void DeleteUserRole( int UserId, int RoleId )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "DeleteUserRole" ), UserId, RoleId );
        }

        public override IDataReader GetServices( int PortalId, int UserId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetServices" ), PortalId, GetNull( UserId ) );
        }

        public override IDataReader GetUsers( int PortalId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUsers" ), GetNull( PortalId ) );
        }

        //Profile
        public override IDataReader GetUserProfile( int UserId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUserProfile" ), UserId );
        }

        public override void UpdateProfileProperty( int ProfileId, int UserId, int PropertyDefinitionID, string PropertyValue, int Visibility, DateTime LastUpdatedDate )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "UpdateUserProfileProperty" ), GetNull( ProfileId ), UserId, PropertyDefinitionID, PropertyValue, Visibility, LastUpdatedDate );
        }

        // users online
        public override void DeleteUsersOnline( int TimeWindow )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteUsersOnline", TimeWindow );
        }

        public override IDataReader GetOnlineUser( int UserId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetOnlineUser", UserId );
        }

        public override IDataReader GetOnlineUsers( int PortalId )
        {
            return OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetOnlineUsers", PortalId );
        }

        public override void UpdateUsersOnline( Hashtable UserList )
        {
            if( UserList.Count == 0 )
            {
                //No users to process, quit method
                return;
            }
            foreach( string key in UserList.Keys )
            {
                if( UserList[key] is AnonymousUserInfo )
                {
                    AnonymousUserInfo user = (AnonymousUserInfo)UserList[key];
                    OracleHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateAnonymousUser", user.UserID, user.PortalID, user.TabID, user.LastActiveDate );
                }
                else if( UserList[key] is OnlineUserInfo )
                {
                    OnlineUserInfo user = (OnlineUserInfo)UserList[key];
                    OracleHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateOnlineUser", user.UserID, user.PortalID, user.TabID, user.LastActiveDate );
                }
            }
        }        
    }
}