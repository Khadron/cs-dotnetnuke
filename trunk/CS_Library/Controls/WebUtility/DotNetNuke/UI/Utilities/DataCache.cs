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