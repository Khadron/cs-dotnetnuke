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
using System.Web;
using System.Web.Caching;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;

namespace DotNetNuke.Services.Cache.BroadcastPollingCachingProvider
{
    public class BPCachingProvider : CachingProvider
    {
        public BPCachingProvider()
        {
            _providerConfiguration = ProviderConfiguration.GetProviderConfiguration( ProviderType );
        }

        private const string ProviderType = "caching";
        private ProviderConfiguration _providerConfiguration;

        private static System.Web.Caching.Cache _objCache;

        private static System.Web.Caching.Cache objCache
        {
            get
            {
                //create singleton of the cache object
                if( _objCache == null )
                {
                    _objCache = HttpRuntime.Cache;
                }
                return _objCache;
            }
        }

        public override object Add( string Key, object Value, CacheDependency Dependencies, DateTime AbsoluteExpiration, TimeSpan SlidingExpiration, CacheItemPriority Priority, CacheItemRemovedCallback OnRemoveCallback )
        {
            return objCache.Add( Key, Value, Dependencies, AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback );
        }

        public override IDictionaryEnumerator GetEnumerator()
        {
            return objCache.GetEnumerator();
        }

        public override object GetItem( string CacheKey )
        {
            object obj = objCache[CacheKey];
            if( obj != null )
            {
                return objCache[CacheKey];
            }
            return obj;
        }

        public override object GetPersistentCacheItem( string CacheKey, Type objType )
        {
            object obj = objCache[CacheKey];
            if( obj != null )
            {
                return obj;
            }
            else if( DataCache.CachePersistenceEnabled )
            {
                Controller c = new Controller();

                obj = c.GetCachedObject( CacheKey, objType );
                if( obj != null )
                {
                    Insert( CacheKey, obj, true );
                }
            }
            return obj;
        }

        public override void Insert( string CacheKey, object objObject, bool PersistAppRestart )
        {
            if( PersistAppRestart )
            {
                //remove the cache key which
                //will remove the serialized
                //file before creating a new one
                Remove( CacheKey );
            }
            Controller c = new Controller();
            if( PersistAppRestart && DataCache.CachePersistenceEnabled )
            {
                c.AddCachedObject( CacheKey, objObject, Globals.ServerName );
                c.AddBroadcast( "RemoveCachedItem", CacheKey, Globals.ServerName );
            }
            else if( Globals.WebFarmEnabled )
            {
                c.AddBroadcast( "RemoveCachedItem", CacheKey, Globals.ServerName );
            }

            objCache.Insert( CacheKey, objObject );
        }

        public override void Insert( string CacheKey, object objObject, CacheDependency objDependency, bool PersistAppRestart )
        {
            if( PersistAppRestart )
            {
                //remove the cache key which
                //will remove the serialized
                //file before creating a new one
                Remove( CacheKey );
            }

            Controller c = new Controller();
            if( PersistAppRestart && DataCache.CachePersistenceEnabled )
            {
                c.AddCachedObject( CacheKey, objObject, Globals.ServerName );
                c.AddBroadcast( "RemoveCachedItem", CacheKey, Globals.ServerName );
            }
            else if( Globals.WebFarmEnabled )
            {
                c.AddBroadcast( "RemoveCachedItem", CacheKey, Globals.ServerName );
            }

            objCache.Insert( CacheKey, objObject, objDependency );
        }

        public override void Insert( string CacheKey, object objObject, CacheDependency objDependency, DateTime AbsoluteExpiration, TimeSpan SlidingExpiration, bool PersistAppRestart )
        {
            if( PersistAppRestart )
            {
                //remove the cache key which
                //will remove the serialized
                //file before creating a new one
                Remove( CacheKey );
            }

            Controller c = new Controller();
            if( PersistAppRestart && DataCache.CachePersistenceEnabled )
            {
                c.AddCachedObject( CacheKey, objObject, Globals.ServerName );
                c.AddBroadcast( "RemoveCachedItem", CacheKey, Globals.ServerName );
            }
            else if( Globals.WebFarmEnabled )
            {
                c.AddBroadcast( "RemoveCachedItem", CacheKey, Globals.ServerName );
            }

            objCache.Insert( CacheKey, objObject, objDependency, AbsoluteExpiration, SlidingExpiration );
        }

        public override void Insert( string CacheKey, object objObject, CacheDependency objDependency, DateTime AbsoluteExpiration, TimeSpan SlidingExpiration, CacheItemPriority Priority, CacheItemRemovedCallback OnRemoveCallback, bool PersistAppRestart )
        {
            if( PersistAppRestart )
            {
                //remove the cache key which
                //will remove the serialized
                //file before creating a new one
                Remove( CacheKey );
            }

            Controller c = new Controller();
            if( PersistAppRestart && DataCache.CachePersistenceEnabled )
            {
                c.AddCachedObject( CacheKey, objObject, Globals.ServerName );
                c.AddBroadcast( "RemoveCachedItem", CacheKey, Globals.ServerName );
            }
            else if( Globals.WebFarmEnabled )
            {
                c.AddBroadcast( "RemoveCachedItem", CacheKey, Globals.ServerName );
            }

            objCache.Insert( CacheKey, objObject, objDependency, AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback );
        }

        public override void Remove( string CacheKey )
        {
            if( objCache[CacheKey] != null )
            {
                Controller c = new Controller();
                objCache.Remove( CacheKey );
                if( Globals.WebFarmEnabled )
                {
                    c.AddBroadcast( "RemoveCachedItem", CacheKey, Globals.ServerName );
                }
            }
        }

        public override void RemovePersistentCacheItem( string CacheKey )
        {
            if( objCache[CacheKey] != null )
            {
                objCache.Remove( CacheKey );
                if( DataCache.CachePersistenceEnabled == true )
                {
                    Controller c = new Controller();
                    c.DeleteCachedObject( CacheKey );
                    c.AddBroadcast( "RemoveCachedItem", CacheKey, Globals.ServerName );
                }
            }
        }

        public override string PurgeCache()
        {
            return "The Broadcast/Polling-Based Caching Provider does not require the PurgeCache feature.";
        }
    }
}