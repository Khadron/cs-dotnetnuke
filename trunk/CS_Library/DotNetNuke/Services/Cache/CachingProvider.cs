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