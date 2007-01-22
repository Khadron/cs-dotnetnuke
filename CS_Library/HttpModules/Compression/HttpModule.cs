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
using System.IO;
using System.Web;

namespace DotNetNuke.HttpModules.Compression
{
    /// <summary>
    /// An HttpModule that hooks onto the Response.Filter property of the
    /// current request and tries to compress the output, based on what
    /// the browser supports
    /// </summary>
    /// <remarks>
    /// <p>This HttpModule uses classes that inherit from <see cref="CompressingFilter"/>.
    /// We already support gzip and deflate (aka zlib), if you'd like to add 
    /// support for compress (which uses LZW, which is licensed), add in another
    /// class that inherits from HttpFilter to do the work.</p>
    /// 
    /// <p>This module checks the Accept-Encoding HTTP header to determine if the
    /// client actually supports any notion of compression.  Currently, we support
    /// the deflate (zlib) and gzip compression schemes.  I chose to not implement
    /// compress because it uses lzw which requires a license from 
    /// Unisys.  For more information about the common compression types supported,
    /// see http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.11 for details.</p> 
    /// </remarks>
    /// <seealso cref="CompressingFilter"/>
    /// <seealso cref="Stream"/>
    public sealed class HttpModule : IHttpModule
    {
        private const string INSTALLED_KEY = "httpcompress.attemptedinstall";
        private static readonly object INSTALLED_TAG = new object();

        /// <summary>
        /// Init the handler and fulfill <see cref="IHttpModule"/>
        /// </summary>
        /// <remarks>
        /// This implementation hooks the ReleaseRequestState and PreSendRequestHeaders events to 
        /// figure out as late as possible if we should install the filter.  Previous versions did
        /// not do this as well.
        /// </remarks>
        /// <param name="context">The <see cref="HttpApplication"/> this handler is working for.</param>
        void IHttpModule.Init( HttpApplication context )
        {
            context.ReleaseRequestState += new EventHandler( this.CompressContent );
            context.PreSendRequestHeaders += new EventHandler( this.CompressContent );
        }

        /// <summary>
        /// Implementation of <see cref="IHttpModule"/>
        /// </summary>
        /// <remarks>
        /// Currently empty.  Nothing to really do, as I have no member variables.
        /// </remarks>
        void IHttpModule.Dispose()
        {
        }

        /// <summary>
        /// EventHandler that gets ahold of the current request context and attempts to compress the output.
        /// </summary>
        /// <param name="sender">The <see cref="HttpApplication"/> that is firing this event.</param>
        /// <param name="e">Arguments to the event</param>
        private void CompressContent( object sender, EventArgs e )
        {
            HttpApplication app = (HttpApplication)sender;

            if( ( app == null ) || ( app.Context == null ) || ( app.Context.Items == null ) )
            {
                return;
            }

            // only do this if we havn't already attempted an install.  This prevents PreSendRequestHeaders from
            // trying to add this item way to late.  We only want the first run through to do anything.
            // also, we use the context to store whether or not we've attempted an add, as it's thread-safe and
            // scoped to the request.  An instance of this module can service multiple requests at the same time,
            // so we cannot use a member variable.
            if( !app.Context.Items.Contains( INSTALLED_KEY ) )
            {
                // log the install attempt in the HttpContext
                // must do this first as several IF statements
                // below skip full processing of this method
                app.Context.Items.Add( INSTALLED_KEY, INSTALLED_TAG );

                // path comparison is based on filename and querystring parameters ( ie. default.aspx?tabid=## )
                string realPath = app.Request.Url.ToString();

                try
                {
                    realPath = realPath.Substring( realPath.LastIndexOf( "/" ) + 1 );
                }
                catch
                {
                    // there are no / characters present in the path
                }

                if( realPath.ToLower().Contains( "webresource.axd" ) )
                {
                    //No compression at all if this is a WebResource file
                    return;
                }

                // get the config settings
                Settings settings = Settings.GetSettings();
                if( settings == null )
                {
                    return;
                }

                bool compress = true;

                if( settings.PreferredAlgorithm == Algorithms.None || ( settings.PreferredAlgorithm == Algorithms.Deflate && settings.CompressionLevel == CompressionLevels.None ) )
                {
                    compress = false;

                    // Terminate processing if both compression and whitespace handling are disabled
                    if( !settings.Whitespace )
                    {
                        return;
                    }
                }

                // grab an array of algorithm;q=x, algorith;q=x style values
                string acceptedTypes = app.Request.Headers["Accept-Encoding"];
                if( settings.IsExcludedPath( realPath ) || settings.IsExcludedMimeType( app.Response.ContentType ) || acceptedTypes == null )
                {
                    // skip if the file path excludes compression
                    // skip if the MimeType excludes compression
                    // if we couldn't find the header, bail out
                    compress = false;
                }

                // fix to handle caching appropriately
                // see http://www.pocketsoap.com/weblog/2003/07/1330.html
                // Note, this header is added only when the request
                // has the possibility of being compressed...
                // i.e. it is not added when the request is excluded from
                // compression by CompressionLevel, Path, or MimeType
                app.Response.Cache.VaryByHeaders["Accept-Encoding"] = true;

                CompressingFilter filter = null;
                if( compress )
                {
                    // the actual types could be , delimited.  split 'em out.
                    string[] types = acceptedTypes.Split( ',' );

                    filter = GetFilterForScheme( types, app.Response.Filter, settings );
                }

                if( filter == null )
                {
                    if( settings.Whitespace )
                    {
                        app.Response.Filter = new WhitespaceFilter( app.Response.Filter, settings.Reg );
                    }
                }
                else
                {
                    if( settings.Whitespace )
                    {
                        app.Response.Filter = new WhitespaceFilter( filter, settings.Reg );
                    }
                    else
                    {
                        app.Response.Filter = filter;
                    }
                }
            }
        }

        /// <summary>
        /// Get ahold of a <see cref="CompressingFilter"/> for the given encoding scheme.
        /// If no encoding scheme can be found, it returns null.
        /// </summary>
        /// <remarks>
        /// See http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.3 for details
        /// on how clients are supposed to construct the Accept-Encoding header.  This
        /// implementation follows those rules, though we allow the server to override
        /// the preference given to different supported algorithms.  I'm doing this as 
        /// I would rather give the server control over the algorithm decision than 
        /// the client.  If the clients send up * as an accepted encoding with highest
        /// quality, we use the preferred algorithm as specified in the config file.
        /// </remarks>
        public static CompressingFilter GetFilterForScheme( string[] schemes, Stream output, Settings prefs )
        {
            bool foundDeflate = false;
            bool foundGZip = false;
            bool foundStar = false;

            float deflateQuality = 0f;
            float gZipQuality = 0f;
            float starQuality = 0f;

            bool isAcceptableDeflate;
            bool isAcceptableGZip;
            bool isAcceptableStar;

            for( int i = 0; i < schemes.Length; i++ )
            {
                string acceptEncodingValue = schemes[i].Trim().ToLower();

                if( acceptEncodingValue.StartsWith( "deflate" ) )
                {
                    foundDeflate = true;

                    float newDeflateQuality = GetQuality( acceptEncodingValue );
                    if( deflateQuality < newDeflateQuality )
                    {
                        deflateQuality = newDeflateQuality;
                    }
                }

                else if( acceptEncodingValue.StartsWith( "gzip" ) || acceptEncodingValue.StartsWith( "x-gzip" ) )
                {
                    foundGZip = true;

                    float newGZipQuality = GetQuality( acceptEncodingValue );
                    if( gZipQuality < newGZipQuality )
                    {
                        gZipQuality = newGZipQuality;
                    }
                }

                else if( acceptEncodingValue.StartsWith( "*" ) )
                {
                    foundStar = true;

                    float newStarQuality = GetQuality( acceptEncodingValue );
                    if( starQuality < newStarQuality )
                    {
                        starQuality = newStarQuality;
                    }
                }
            }

            isAcceptableStar = foundStar && ( starQuality > 0 );
            isAcceptableDeflate = ( foundDeflate && ( deflateQuality > 0 ) ) || ( !foundDeflate && isAcceptableStar );
            isAcceptableGZip = ( foundGZip && ( gZipQuality > 0 ) ) || ( !foundGZip && isAcceptableStar );

            if( isAcceptableDeflate && !foundDeflate )
            {
                deflateQuality = starQuality;
            }

            if( isAcceptableGZip && !foundGZip )
            {
                gZipQuality = starQuality;
            }

            // do they support any of our compression methods?
            if( !( isAcceptableDeflate || isAcceptableGZip || isAcceptableStar ) )
            {
                return null;
            }

            // if deflate is better according to client
            if( isAcceptableDeflate && ( !isAcceptableGZip || ( deflateQuality > gZipQuality ) ) )
            {
                return new DeflateFilter( output, prefs.CompressionLevel );
            }

            // if gzip is better according to client
            if( isAcceptableGZip && ( !isAcceptableDeflate || ( deflateQuality < gZipQuality ) ) )
            {
                return new GZipFilter( output );
            }

            // if we're here, the client either didn't have a preference or they don't support compression
            if( isAcceptableDeflate && ( prefs.PreferredAlgorithm == Algorithms.Deflate || prefs.PreferredAlgorithm == Algorithms.Default ) )
            {
                return new DeflateFilter( output, prefs.CompressionLevel );
            }
            if( isAcceptableGZip && prefs.PreferredAlgorithm == Algorithms.GZip )
            {
                return new GZipFilter( output );
            }

            if( isAcceptableDeflate || isAcceptableStar )
            {
                return new DeflateFilter( output, prefs.CompressionLevel );
            }
            if( isAcceptableGZip )
            {
                return new GZipFilter( output );
            }

            // return null.  we couldn't find a filter.
            return null;
        }

        private static float GetQuality( string acceptEncodingValue )
        {
            int qParam = acceptEncodingValue.IndexOf( "q=" );

            if( qParam >= 0 )
            {
                float val = 0.0f;
                try
                {
                    val = float.Parse( acceptEncodingValue.Substring( qParam + 2, acceptEncodingValue.Length - ( qParam + 2 ) ) );
                }
                catch( FormatException )
                {
                }
                return val;
            }
            else
            {
                return 1;
            }
        }
    }
}