using System;
using System.Collections;
using System.Web.Caching;
using DotNetNuke.Framework;

namespace DotNetNuke.Services.Cache
{
    public abstract class CachingProvider
    {
        // singleton reference to the instantiated object
        private static CachingProvider objProvider = null;

        // constructor
        static CachingProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (CachingProvider)Reflection.CreateObjectNotCached( "caching" );
        }

        // return the provider
        public new static CachingProvider Instance()
        {
            return objProvider;
        }

        // methods to return functionality support indicators
        public abstract object Add( string Key, object Value, CacheDependency Dependencies, DateTime AbsoluteExpiration, TimeSpan SlidingExpiration, CacheItemPriority Priority, CacheItemRemovedCallback OnRemoveCallback );
        public abstract IDictionaryEnumerator GetEnumerator();
        public abstract object GetItem( string CacheKey );
        public abstract object GetPersistentCacheItem( string CacheKey, Type objType );
        public abstract void Insert( string CacheKey, object objObject, bool PersistAppRestart );
        public abstract void Insert( string CacheKey, object objObject, CacheDependency objDependency, bool PersistAppRestart );
        public abstract void Insert( string CacheKey, object objObject, CacheDependency objDependency, DateTime AbsoluteExpiration, TimeSpan SlidingExpiration, bool PersistAppRestart );
        public abstract void Insert( string CacheKey, object objObject, CacheDependency objDependency, DateTime AbsoluteExpiration, TimeSpan SlidingExpiration, CacheItemPriority Priority, CacheItemRemovedCallback OnRemoveCallback, bool PersistAppRestart );
        public abstract void Remove( string CacheKey );
        public abstract void RemovePersistentCacheItem( string CacheKey );
        public abstract string PurgeCache();
    }
}