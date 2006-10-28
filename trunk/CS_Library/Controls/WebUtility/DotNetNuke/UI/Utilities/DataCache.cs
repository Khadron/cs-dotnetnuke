using System;
using System.Web;
using System.Web.Caching;

namespace DotNetNuke.UI.Utilities
{
    public class DataCache
    {
        public DataCache()
        {
        }

        public static object GetCache( string CacheKey )
        {
            Cache cache1 = HttpRuntime.Cache;
            return cache1[CacheKey];
        }

        public static void RemoveCache( string CacheKey )
        {
            Cache cache1 = HttpRuntime.Cache;
            if( cache1[CacheKey] == null )
            {
                return;
            }
            object object1 = cache1.Remove( CacheKey );
        }

        public static void SetCache( string CacheKey, object objObject, DateTime AbsoluteExpiration )
        {
            Cache cache1 = HttpRuntime.Cache;
            cache1.Insert( CacheKey, objObject, ( (CacheDependency)null ), AbsoluteExpiration, Cache.NoSlidingExpiration );
        }

        public static void SetCache( string CacheKey, object objObject )
        {
            Cache cache1 = HttpRuntime.Cache;
            cache1.Insert( CacheKey, objObject );
        }

        public static void SetCache( string CacheKey, object objObject, int SlidingExpiration )
        {
            Cache cache1 = HttpRuntime.Cache;
            cache1.Insert( CacheKey, objObject, ( (CacheDependency)null ), Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds( ( (double)SlidingExpiration ) ) );
        }

        public static void SetCache( string CacheKey, object objObject, CacheDependency objDependency )
        {
            Cache cache1 = HttpRuntime.Cache;
            cache1.Insert( CacheKey, objObject, objDependency );
        }

        public static void SetCache( string CacheKey, object objObject, CacheDependency objDependency, DateTime AbsoluteExpiration, TimeSpan SlidingExpiration )
        {
            Cache cache1 = HttpRuntime.Cache;
            cache1.Insert( CacheKey, objObject, objDependency, AbsoluteExpiration, SlidingExpiration );
        }
    }
}