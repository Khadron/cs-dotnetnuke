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
    public class SqlDataProvider : DataProvider
    {
        private const string ProviderType = "data";
        private ProviderConfiguration _providerConfiguration;
        private string _connectionString;
        private string _providerPath;
        private string _objectQualifier;
        private string _databaseOwner;

        public SqlDataProvider()
        {
            // Read the configuration specific information for this provider
            Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            // Read the attributes for this provider
            //Get Connection string from web.config
            _connectionString = Config.GetConnectionString();

            if( _connectionString == "" )
            {
                // Use connection string specified in provider
                _connectionString = objProvider.Attributes["connectionString"];
            }

            _providerPath = objProvider.Attributes["providerPath"];

            _objectQualifier = objProvider.Attributes["objectQualifier"];
            if( _objectQualifier != "" && _objectQualifier.EndsWith( "_" ) == false )
            {
                _objectQualifier += "_";
            }

            _databaseOwner = objProvider.Attributes["databaseOwner"];
            if( _databaseOwner != "" && _databaseOwner.EndsWith( "." ) == false )
            {
                _databaseOwner += ".";
            }
        }

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        public string ProviderPath
        {
            get
            {
                return _providerPath;
            }
        }

        public string ObjectQualifier
        {
            get
            {
                return _objectQualifier;
            }
        }

        public string DatabaseOwner
        {
            get
            {
                return _databaseOwner;
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
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "UserLogin" ), Username, Password ) );
        }

        public override IDataReader GetAuthRoles( int PortalId, int ModuleId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetAuthRoles" ), PortalId, ModuleId ) );
        }

        //Users
        public override int AddUser( int PortalID, string Username, string FirstName, string LastName, int AffiliateId, bool IsSuperUser, string Email, string DisplayName, bool UpdatePassword, bool IsApproved )
        {
            try
            {
                return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddUser", PortalID, Username, FirstName, LastName, GetNull( AffiliateId ), IsSuperUser, Email, DisplayName, UpdatePassword, IsApproved ) );
            }
            catch // duplicate
            {
                return - 1;
            }
        }

        public override void DeleteUserPortal( int UserId, int PortalId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "DeleteUserPortal" ), UserId, PortalId );
        }

        public override void DeleteUser( int UserId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "DeleteUser" ), UserId );
        }

        public override void UpdateUser( int UserId, int PortalID, string FirstName, string LastName, string Email, string DisplayName, bool UpdatePassword, bool IsApproved )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "UpdateUser" ), UserId, PortalID, FirstName, LastName, Email, DisplayName, UpdatePassword, IsApproved );
        }

        public override IDataReader GetAllUsers( int PortalID, int pageIndex, int pageSize )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetAllUsers" ), GetNull( PortalID ), pageIndex, pageSize ) );
        }

        public override IDataReader GetUnAuthorizedUsers( int portalId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUnAuthorizedUsers" ), GetNull( portalId ) ) );
        }

        public override IDataReader GetUser( int PortalId, int UserId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUser" ), PortalId, UserId ) );
        }

        public override IDataReader GetUserByUsername( int PortalId, string Username )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUserByUsername" ), GetNull( PortalId ), Username ) );
        }

        public override int GetUserCountByPortal( int portalId )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, GetFullyQualifiedName( "GetUserCountByPortal" ), portalId ) );
        }

        public override IDataReader GetUsersByEmail( int PortalID, string Email, int pageIndex, int pageSize )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUsersByEmail" ), GetNull( PortalID ), Email, pageIndex, pageSize ) );
        }

        public override IDataReader GetUsersByProfileProperty( int PortalID, string propertyName, string propertyValue, int pageIndex, int pageSize )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUsersByProfileProperty" ), GetNull( PortalID ), propertyName, propertyValue, pageIndex, pageSize ) );
        }

        public override IDataReader GetUsersByRolename( int PortalID, string Rolename )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUsersByRolename" ), GetNull( PortalID ), Rolename ) );
        }

        public override IDataReader GetUsersByUsername( int PortalID, string Username, int pageIndex, int pageSize )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUsersByUsername" ), GetNull( PortalID ), Username, pageIndex, pageSize ) );
        }

        public override IDataReader GetSuperUsers()
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetSuperUsers" ), null ) );
        }

        //Roles
        public override IDataReader GetRolesByUser( int UserId, int PortalId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetRolesByUser" ), UserId, PortalId ) );
        }

        public override IDataReader GetPortalRoles( int PortalId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetPortalRoles" ), PortalId ) );
        }

        public override IDataReader GetRoles()
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetRoles" ), null ) );
        }

        public override IDataReader GetRole( int RoleId, int PortalId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetRole" ), RoleId, PortalId ) );
        }

        public override IDataReader GetRoleByName( int PortalId, string RoleName )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetRoleByName" ), PortalId, RoleName ) );
        }

        public override IDataReader GetRolesByGroup( int RoleGroupId, int PortalId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetRolesByGroup" ), GetNull( RoleGroupId ), PortalId ) );
        }

        public override int AddRole( int PortalId, int RoleGroupId, string RoleName, string Description, float ServiceFee, string BillingPeriod, string BillingFrequency, float TrialFee, int TrialPeriod, string TrialFrequency, bool IsPublic, bool AutoAssignment, string RSVPCode, string IconFile )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, GetFullyQualifiedName( "AddRole" ), PortalId, GetNull( RoleGroupId ), RoleName, Description, ServiceFee, BillingPeriod, GetNull( BillingFrequency ), TrialFee, TrialPeriod, GetNull( TrialFrequency ), IsPublic, AutoAssignment, RSVPCode, IconFile ) );
        }

        public override void DeleteRole( int RoleId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "DeleteRole" ), RoleId );
        }

        public override void UpdateRole( int RoleId, int RoleGroupId, string Description, float ServiceFee, string BillingPeriod, string BillingFrequency, float TrialFee, int TrialPeriod, string TrialFrequency, bool IsPublic, bool AutoAssignment, string RSVPCode, string IconFile )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "UpdateRole" ), RoleId, GetNull( RoleGroupId ), Description, ServiceFee, BillingPeriod, GetNull( BillingFrequency ), TrialFee, TrialPeriod, GetNull( TrialFrequency ), IsPublic, AutoAssignment, RSVPCode, IconFile );
        }

        //Role Groups
        public override int AddRoleGroup( int PortalId, string GroupName, string Description )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, GetFullyQualifiedName( "AddRoleGroup" ), PortalId, GroupName, Description ) );
        }

        public override void DeleteRoleGroup( int RoleGroupId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "DeleteRoleGroup" ), RoleGroupId );
        }

        public override IDataReader GetRoleGroup( int portalId, int roleGroupId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetRoleGroup" ), portalId, roleGroupId ) );
        }

        public override IDataReader GetRoleGroups( int portalId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetRoleGroups" ), portalId ) );
        }

        public override void UpdateRoleGroup( int RoleGroupId, string GroupName, string Description )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "UpdateRoleGroup" ), RoleGroupId, GroupName, Description );
        }

        //User Roles
        public override IDataReader GetUserRole( int PortalID, int UserId, int RoleId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUserRole" ), PortalID, UserId, RoleId ) );
        }

        public override IDataReader GetUserRoles( int PortalID, int UserId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUserRoles" ), PortalID, UserId ) );
        }

        public override IDataReader GetUserRolesByUsername( int PortalID, string Username, string Rolename )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUserRolesByUsername" ), PortalID, GetNull( Username ), GetNull( Rolename ) ) );
        }

        public override int AddUserRole( int PortalId, int UserId, int RoleId, DateTime EffectiveDate, DateTime ExpiryDate )
        {
            return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, GetFullyQualifiedName( "AddUserRole" ), PortalId, UserId, RoleId, GetNull( EffectiveDate ), GetNull( ExpiryDate ) ) );
        }

        public override void UpdateUserRole( int UserRoleId, DateTime EffectiveDate, DateTime ExpiryDate )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "UpdateUserRole" ), UserRoleId, GetNull( EffectiveDate ), GetNull( ExpiryDate ) );
        }

        public override void DeleteUserRole( int UserId, int RoleId )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "DeleteUserRole" ), UserId, RoleId );
        }

        public override IDataReader GetServices( int PortalId, int UserId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetServices" ), PortalId, GetNull( UserId ) ) );
        }

        public override IDataReader GetUsers( int PortalId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUsers" ), GetNull( PortalId ) ) );
        }

        //Profile
        public override IDataReader GetUserProfile( int UserId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, GetFullyQualifiedName( "GetUserProfile" ), UserId ) );
        }

        public override void UpdateProfileProperty( int ProfileId, int UserId, int PropertyDefinitionID, string PropertyValue, int Visibility, DateTime LastUpdatedDate )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, GetFullyQualifiedName( "UpdateUserProfileProperty" ), GetNull( ProfileId ), UserId, PropertyDefinitionID, PropertyValue, Visibility, LastUpdatedDate );
        }

        // users online
        public override void DeleteUsersOnline( int TimeWindow )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteUsersOnline", TimeWindow );
        }

        public override IDataReader GetOnlineUser( int UserId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetOnlineUser", UserId ) );
        }

        public override IDataReader GetOnlineUsers( int PortalId )
        {
            return ( (IDataReader)SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetOnlineUsers", PortalId ) );
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
                    SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateAnonymousUser", user.UserID, user.PortalID, user.TabID, user.LastActiveDate );
                }
                else if( UserList[key] is OnlineUserInfo )
                {
                    OnlineUserInfo user = (OnlineUserInfo)UserList[key];
                    SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateOnlineUser", user.UserID, user.PortalID, user.TabID, user.LastActiveDate );
                }
            }
        }
    }
}