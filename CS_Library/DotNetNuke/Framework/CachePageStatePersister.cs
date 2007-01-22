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
using System.Text;
using System.Web.Caching;
using System.Web.UI;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Framework
{
    /// <summary>
    /// CachePageStatePersister provides a cache based page state peristence mechanism
    /// </summary>
    /// <history>
    ///		[cnurse]	11/30/2006	documented
    /// </history>
    public class CachePageStatePersister : PageStatePersister
    {
        /// <summary>
        /// Creates the CachePageStatePersister
        /// </summary>
        /// <history>
        /// 	[cnurse]	    11/30/2006	Documented
        /// </history>
        public CachePageStatePersister( Page page ) : base( page )
        {
        }

        /// <summary>
        /// Loads the Page State from the Cache
        /// </summary>
        /// <history>
        /// 	[cnurse]	    11/30/2006	Documented
        /// </history>
        public override void Load()
        {
            // Get the cache key from the web form data
            string key = Page.Request.Params["__VIEWSTATE_CACHEKEY"] as string;

            //Abort if cache key is not available or valid
            if( string.IsNullOrEmpty( key ) | ! ( key.StartsWith( "VIEWSTATE_" ) ) )
            {
                throw new ApplicationException( "Missing vaild __VIEWSTATE_CACHEKEY" );
            }

            Pair state = DataCache.GetPersistentCacheItem( key, typeof( Pair ) ) as Pair;

            if( state != null )
            {
                //Set view state and control state
                ViewState = state.First;
                ControlState = state.Second;
            }
        }

        /// <summary>
        /// Saves the Page State to the Cache
        /// </summary>
        /// <history>
        /// 	[cnurse]	    11/30/2006	Documented
        /// </history>
        public override void Save()
        {
            //No processing needed if no states available
            if( ViewState == null & ControlState == null )
            {
                return;
            }

            StringBuilder key = new StringBuilder();

            //Generate a unique cache key
            key.Append( "VIEWSTATE_" );
            key.Append( ( ( Page.Session == null ) ? Guid.NewGuid().ToString() : Page.Session.SessionID ) );
            key.Append( "_" );
            key.Append( Page.Request.RawUrl );
            key.Append( "_" );
            key.Append( DateTime.Now.Ticks.ToString() );

            //Save view state and control state separately
            Pair state = new Pair( ViewState, ControlState );

            //Add view state and control state to cache
            DataCache.SetCache( key.ToString(), state, null, DateTime.Now.AddMinutes( 15 ), Cache.NoSlidingExpiration, true );

            //Register hidden field to store cache key in
            Page.ClientScript.RegisterHiddenField( "__VIEWSTATE_CACHEKEY", key.ToString() );
        }
    }
}