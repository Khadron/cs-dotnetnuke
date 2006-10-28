using System.Data;
using DotNetNuke.Framework;

namespace DotNetNuke.Services.Cache.BroadcastPollingCachingProvider.Data
{
    public abstract class DataProvider
    {
        // provider constants - eliminates need for Reflection later
        private const string ProviderType = "data"; // maps to <sectionGroup> in web.config
        private const string ProviderNamespace = "DotNetNuke.Services.Cache.BroadcastPollingCachingProvider.Data"; // project namespace
        private const string ProviderAssemblyName = "DotNetNuke.Caching.BroadcastPollingCachingProvider"; // project assemblyname

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
            objProvider = (DataProvider)Reflection.CreateObject( ProviderType, ProviderNamespace, ProviderAssemblyName );
        }

        // return the provider
        public new static DataProvider Instance()
        {
            return objProvider;
        }

        public abstract IDataReader GetCachedObject( string Key );
        public abstract void AddCachedObject( string Key, string Value, string ServerName );
        public abstract void DeleteCachedObject( string Key );

        public abstract int AddBroadcast( string BroadcastType, string BroadcastMessage, string ServerName );
        public abstract IDataReader GetBroadcasts( string ServerName );
    }
}