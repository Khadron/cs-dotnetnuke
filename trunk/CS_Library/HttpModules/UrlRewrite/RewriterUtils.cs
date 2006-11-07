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
using System.Web;

namespace DotNetNuke.HttpModules
{
    public class RewriterUtils
    {
        internal static void RewriteUrl( HttpContext context, string sendToUrl )
        {
            string x = "";
            string y = "";

            RewriteUrl( context, sendToUrl, ref x, ref y );
        }

        internal static void RewriteUrl( HttpContext context, string sendToUrl, ref string sendToUrlLessQString, ref string filePath )
        {
            // first strip the querystring, if any
            string queryString = string.Empty;
            sendToUrlLessQString = sendToUrl;

            if( sendToUrl.IndexOf( "?" ) > 0 )
            {
                sendToUrlLessQString = sendToUrl.Substring( 0, sendToUrl.IndexOf( "?" ) );
                queryString = sendToUrl.Substring( sendToUrl.IndexOf( "?" ) + 1 );
            }

            // grab the file's physical path            
            filePath = context.Server.MapPath( sendToUrlLessQString );

            // rewrite the path..
            context.RewritePath( sendToUrlLessQString, string.Empty, queryString );

            // NOTE!  The above RewritePath() overload is only supported in the .NET Framework 1.1
            // If you are using .NET Framework 1.0, use the below form instead:
            // context.RewritePath(sendToUrl);
        }

        internal static string ResolveUrl( string appPath, string url )
        {
            // String is Empty, just return Url
            if( url.Length == 0 )
            {
                return url;
            }

            // String does not contain a ~, so just return Url
            if( url.StartsWith( "~" ) == false )
            {
                return url;
            }

            // There is just the ~ in the Url, return the appPath
            if( url.Length == 1 )
            {
                return appPath;
            }

            if( ( url.ToCharArray()[1] == '/' || url.ToCharArray()[1] == '\\' ) )
            {
                // Url looks like ~/ or ~\
                if( appPath.Length > 1 )
                {
                    return appPath + "/" + url.Substring( 2 );
                }
                else
                {
                    return "/" + url.Substring( 2 );
                }
            }
            else
            {
                // Url look like ~something
                if( appPath.Length > 1 )
                {
                    return appPath + "/" + url.Substring( 1 );
                }
                else
                {
                    return appPath + url.Substring( 1 );
                }
            }
        }
    }
}