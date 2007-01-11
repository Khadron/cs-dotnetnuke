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
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;

namespace DotNetNuke.Services.Cache.FileBasedCachingProvider
{
    public class FBCachingProvider : CachingProvider
    {
        public FBCachingProvider()
        {
            _providerConfiguration = ProviderConfiguration.GetProviderConfiguration( ProviderType );
        }

        private const string ProviderType = "caching";
        private ProviderConfiguration _providerConfiguration;

        internal static string CachingDirectory = Globals.HostMapPath + "Cache\\";
        internal const string CacheFileExtension = ".resources";
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
                return objCache[CacheKey];
            }
            else if( DataCache.CachePersistenceEnabled )
            {
                Stream objStream;
                string f = GetFileName( CacheKey );
                if( File.Exists( f ) )
                {
                    objStream = File.OpenRead( f );

                    XmlSerializer serializer = new XmlSerializer( objType );
                    TextReader tr = new StreamReader( objStream );
                    obj = serializer.Deserialize( tr );
                    tr.Close();
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

            string f = GetFileName( CacheKey );
            CacheDependency d = new CacheDependency( f );

            if( PersistAppRestart && DataCache.CachePersistenceEnabled )
            {
                CreateCacheFile( f, objObject );
            }
            else if( Globals.WebFarmEnabled )
            {
                CreateCacheFile( f );
            }
            else
            {
                d = null;
            }

            objCache.Insert( CacheKey, objObject, d );
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

            string[] f = new string[1];
            f[0] = GetFileName( CacheKey );

            CacheDependency d = new CacheDependency( f, null, objDependency );
            if( PersistAppRestart && DataCache.CachePersistenceEnabled )
            {
                CreateCacheFile( f[0], objObject );
            }
            else if( Globals.WebFarmEnabled )
            {
                CreateCacheFile( f[0] );
            }
            else
            {
                d = objDependency;
            }

            objCache.Insert( CacheKey, objObject, d );
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

            string[] f = new string[1];
            f[0] = GetFileName( CacheKey );

            CacheDependency d = new CacheDependency( f, null, objDependency );
            if( PersistAppRestart && DataCache.CachePersistenceEnabled )
            {
                CreateCacheFile( f[0], objObject );
            }
            else if( Globals.WebFarmEnabled )
            {
                CreateCacheFile( f[0] );
            }
            else
            {
                d = objDependency;
            }

            objCache.Insert( CacheKey, objObject, d, AbsoluteExpiration, SlidingExpiration );
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

            string[] f = new string[1];
            f[0] = GetFileName( CacheKey );

            CacheDependency d = new CacheDependency( f, null, objDependency );
            if( PersistAppRestart && DataCache.CachePersistenceEnabled )
            {
                CreateCacheFile( f[0], objObject );
            }
            else if( Globals.WebFarmEnabled )
            {
                CreateCacheFile( f[0] );
            }
            else
            {
                d = objDependency;
            }

            objCache.Insert( CacheKey, objObject, d, AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback );
        }

        public override void Remove( string CacheKey )
        {
            if( objCache[CacheKey] != null )
            {
                objCache.Remove( CacheKey );
                string f = GetFileName( CacheKey );
                if( Globals.WebFarmEnabled )
                {
                    DeleteCacheFile( f );
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
                    string f = GetFileName( CacheKey );
                    DeleteCacheFile( f );
                }
            }
        }

        public override string PurgeCache()
        {
            string[] f;
            f = Directory.GetFiles( CachingDirectory );
            int TotalFiles = f.Length;
            int PurgedFiles;
            PurgedFiles = PurgeCacheFiles( f );
            return string.Format( "Purged " + PurgedFiles.ToString() + " of " + TotalFiles.ToString() + " cache synchronization files.", null );
        }

        private int PurgeCacheFiles( string[] strFiles )
        {
            int PurgedFiles = 0;
            int i;
            for( i = 0; i <= strFiles.Length - 1; i++ )
            {
                DateTime dtLastWrite;
                dtLastWrite = File.GetLastWriteTime( strFiles[i] );
                if( dtLastWrite < DateTime.Now.Subtract( new TimeSpan( 2, 0, 0 ) ) )
                {
                    string strFileName;
                    strFileName = strFiles[i].Substring( strFiles[i].LastIndexOf( "\\" ) );
                    string strCacheKey;
                    strCacheKey = strFileName.Substring( 0, strFileName.Length - CacheFileExtension.Length );
                    if( DataCache.GetCache( strCacheKey ) == null )
                    {
                        File.Delete( strFiles[i] );
                        PurgedFiles++;
                    }
                }
            }
            return PurgedFiles;
        }

        private static string GetFileName( string FileName )
        {
            byte[] FileNameBytes = ASCIIEncoding.ASCII.GetBytes( FileName );
            string FinalFileName = Convert.ToBase64String( FileNameBytes );
            if( FinalFileName.IndexOf( "/" ) > - 1 )
            {
                FinalFileName = FinalFileName.Replace( "/", "-" );
            }
            return CachingDirectory + FinalFileName + CacheFileExtension;
        }

        private static void CreateCacheFile( string FileName )
        {
            StreamWriter s;
            if( ! File.Exists( FileName ) )
            {
                s = File.CreateText( FileName );
                if( s != null )
                {
                    s.Close();
                }
            }
        }

        private static void CreateCacheFile( string FileName, object ObjectToPersist )
        {
            string str = XmlUtils.Serialize( ObjectToPersist );

            StreamWriter s;
            if( ! File.Exists( FileName ) )
            {
                s = File.CreateText( FileName );
                s.Write( str );
                if( s != null )
                {
                    s.Close();
                }
            }
        }

        private static void DeleteCacheFile( string FileName )
        {
            if( File.Exists( FileName ) )
            {
                File.Delete( FileName );
            }
        }
    }
}