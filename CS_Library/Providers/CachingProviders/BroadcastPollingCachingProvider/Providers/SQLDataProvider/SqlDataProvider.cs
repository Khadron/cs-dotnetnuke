using System;
using System.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using Microsoft.ApplicationBlocks.Data;

namespace DotNetNuke.Services.Cache.BroadcastPollingCachingProvider.Data
{
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

        public override IDataReader GetCachedObject( string Key )
        {
            try
            {
                return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetCachedObject", Key );
            }
            catch( Exception )
            {
                return null;
            }
        }

        public override void AddCachedObject( string Key, string Value, string ServerName )
        {
            try
            {
                SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddCachedObject", Key, Value, ServerName );
            }
            catch( Exception )
            {
            }
        }

        public override void DeleteCachedObject( string Key )
        {
            try
            {
                SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteCachedObject", Key );
            }
            catch( Exception )
            {
            }
        }

        public override int AddBroadcast( string BroadcastType, string BroadcastMessage, string ServerName )
        {
            try
            {
                return Convert.ToInt32( SqlHelper.ExecuteScalar( ConnectionString, DatabaseOwner + ObjectQualifier + "AddBroadcast", BroadcastType, BroadcastMessage, ServerName ) );
            }
            catch( Exception )
            {
            }
            return 0;
        }

        public override IDataReader GetBroadcasts( string ServerName )
        {
            try
            {
                return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetBroadcasts", ServerName );
            }
            catch( Exception )
            {
                return null;
            }
        }
    }
}