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