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
using System.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using Microsoft.ApplicationBlocks.Data;

namespace DotNetNuke.Services.Cache.BroadcastPollingCachingProvider.Data
{
    public class SqlDataProvider : DataProvider
    {
        private const string ProviderType = "data";
        private ProviderConfiguration providerConfiguration;
        private string connectionString;
        private string providerPath;
        private string objectQualifier;
        private string databaseOwner;

        public SqlDataProvider()
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