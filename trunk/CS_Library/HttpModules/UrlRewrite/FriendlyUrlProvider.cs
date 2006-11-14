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
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework.Providers;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;
using TabInfo=DotNetNuke.Entities.Tabs.TabInfo;

namespace DotNetNuke.Services.Url.FriendlyUrl
{
    public class DNNFriendlyUrlProvider : FriendlyUrlProvider
    {
        private const string ProviderType = "friendlyUrl";
        private const string RegexMatchExpression = "[^a-zA-Z0-9 ]";

        private ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
        private bool _includePageName;
        private string _regexMatch;

        public DNNFriendlyUrlProvider()
        {
            // Read the configuration specific information for this provider
            Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            // Read the attributes for this provider

            if( Convert.ToString( objProvider.Attributes["includePageName"] ) != "" )
            {
                _includePageName = bool.Parse( objProvider.Attributes["includePageName"] );
            }
            else
            {
                _includePageName = true;
            }

            if( Convert.ToString( objProvider.Attributes["includePageName"] ) != "" )
            {
                _includePageName = bool.Parse( objProvider.Attributes["includePageName"] );
            }
            else
            {
                _includePageName = true;
            }

            if( Convert.ToString( objProvider.Attributes["regexMatch"] ) != "" )
            {
                _regexMatch = objProvider.Attributes["regexMatch"];
            }
            else
            {
                _regexMatch = RegexMatchExpression;
            }
        }

        public bool IncludePageName
        {
            get
            {
                return _includePageName;
            }
        }

        public string RegexMatch
        {
            get
            {
                return _regexMatch;
            }
        }

        public override string FriendlyUrl( TabInfo tab, string path )
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            return FriendlyUrl( tab, path, Globals.glbDefaultPage, _portalSettings );
        }

        public override string FriendlyUrl( TabInfo tab, string path, string pageName )
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            return FriendlyUrl( tab, path, pageName, _portalSettings );
        }

        public override string FriendlyUrl( TabInfo tab, string path, string pageName, PortalSettings settings )
        {
            string friendlyPath = path;
            string matchString = "";

            //Call GetFriendlyAlias to get the Alias part of the url
            friendlyPath = GetFriendlyAlias( path, settings.PortalAlias.HTTPAlias );

            //Call GetFriendlyQueryString to get the QueryString part of the url
            friendlyPath = GetFriendlyQueryString( tab, friendlyPath, settings, pageName );

            return friendlyPath;
        }

        public override string FriendlyUrl( TabInfo tab, string path, string pageName, string portalAlias )
        {
            string friendlyPath = path;
            string matchString = "";

            //Call GetFriendlyAlias to get the Alias part of the url
            friendlyPath = GetFriendlyAlias( path, portalAlias );

            //Call GetFriendlyQueryString to get the QueryString part of the url
            friendlyPath = GetFriendlyQueryString( tab, friendlyPath, pageName );

            return friendlyPath;
        }

        /// <summary>
        /// AddPage adds the page to the friendly url
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="path">The path to format.</param>
        /// <param name="pageName">The page name.</param>
        /// <returns>The formatted url</returns>
        /// <history>
        ///		[cnurse]	12/16/2004	created
        /// </history>
        private string AddPage( string path, string pageName )
        {
            string friendlyPath = path;

            if( friendlyPath.EndsWith( "/" ) )
            {
                friendlyPath = friendlyPath + pageName;
            }
            else
            {
                friendlyPath = friendlyPath + "/" + pageName;
            }

            return friendlyPath;
        }

        /// <summary>
        /// GetFriendlyAlias gets the Alias root of the friendly url
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="path">The path to format.</param>
        /// <param name="portalAlias">The portal alias of the site.</param>
        /// <returns>The formatted url</returns>
        /// <history>
        ///		[cnurse]	12/16/2004	created
        /// </history>
        private string GetFriendlyAlias( string path, string portalAlias )
        {
            string friendlyPath = path;
            string matchString = "";

            if( !( portalAlias == Null.NullString ) )
            {
                if( !( HttpContext.Current.Items["UrlRewrite:OriginalUrl"] == null ) )
                {
                    string originalUrl = HttpContext.Current.Items["UrlRewrite:OriginalUrl"].ToString();

                    //For Each entry As String In arrAlias
                    Match portalMatch = Regex.Match( originalUrl, "^" + Globals.AddHTTP( portalAlias ), RegexOptions.IgnoreCase );
                    if( !( portalMatch == Match.Empty ) )
                    {
                        matchString = Globals.AddHTTP( portalAlias );
                    }

                    if( matchString == "" )
                    {
                        //Manage the special case where original url contains the alias as
                        //http://www.domain.com/Default.aspx?alias=www.domain.com/child"
                        portalMatch = Regex.Match( originalUrl, "^?alias=" + portalAlias, RegexOptions.IgnoreCase );
                        if( !( portalMatch == Match.Empty ) )
                        {
                            matchString = Globals.AddHTTP( portalAlias );
                        }
                    }
                }
            }

            if( matchString != "" )
            {
                if( path.IndexOf( "~" ) != - 1 )
                {
                    friendlyPath = friendlyPath.Replace( "~", matchString );
                }
                else
                {
                    friendlyPath = matchString + friendlyPath;
                }
            }
            else
            {
                friendlyPath = Globals.ResolveUrl( friendlyPath );
            }

            return friendlyPath;
        }

        /// <summary>
        /// GetFriendlyQueryString gets the Querystring part of the friendly url
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="path">The path to format.</param>
        /// <param name="settings">The Portal Settings for the site.</param>
        /// <returns>The formatted url</returns>
        /// <history>
        ///		[cnurse]	12/16/2004	created
        ///		[smcculloch]10/10/2005	Regex update for rewritten characters
        /// </history>
        private string GetFriendlyQueryString( TabInfo tab, string path, PortalSettings settings, string pageName )
        {
            string friendlyPath = path;
            Match queryStringMatch = Regex.Match( friendlyPath, "(.[^\\\\?]*)\\\\?(.*)", RegexOptions.IgnoreCase );
            string queryStringSpecialChars = "";

            if( !( queryStringMatch == Match.Empty ) )
            {
                friendlyPath = queryStringMatch.Groups[1].Value;
                friendlyPath = Regex.Replace( friendlyPath, Globals.glbDefaultPage, "", RegexOptions.IgnoreCase );

                string queryString = queryStringMatch.Groups[2].Value.Replace( "&amp;", "&" );
                if( queryString.StartsWith( "?" ) )
                {
                    queryString = queryString.TrimStart( Convert.ToChar( "?" ) );
                }

                string[] nameValuePairs = queryString.Split( Convert.ToChar( "&" ) );
                for( int i = 0; i <= nameValuePairs.Length - 1; i++ )
                {
                    string pathToAppend = "";
                    string[] pair = nameValuePairs[i].Split( Convert.ToChar( "=" ) );

                    if( friendlyPath.EndsWith( "/" ) )
                    {
                        pathToAppend = pathToAppend + pair[0];
                    }
                    else
                    {
                        pathToAppend = pathToAppend + "/" + pair[0];
                    }

                    if( pair.Length > 1 )
                    {
                        if( pair[1].Length > 0 )
                        {
                            if( Regex.IsMatch( pair[1], _regexMatch ) == false )
                            {
                                // Contains Non-AlphaNumeric Characters

                                if( pair[0].ToLower() == "tabid" )
                                {
                                    if( Information.IsNumeric( pair[1] ) )
                                    {
                                        if( !( tab == null ) )
                                        {
                                            int tabId = Convert.ToInt32( pair[1] );
                                            if( tab.TabID == tabId )
                                            {
                                                if( ( tab.TabPath != Null.NullString ) && IncludePageName )
                                                {
                                                    pathToAppend = tab.TabPath.Replace( "//", "/" ).TrimStart( '/' ) + "/" + pathToAppend;
                                                }
                                            }
                                        }
                                    }
                                }

                                pathToAppend = pathToAppend + "/" + pair[1].Replace( " ", HttpUtility.UrlEncode( ( '\u0020' ).ToString() ) );
                            }
                            else
                            {
                                // Rewrite into URL, contains only alphanumeric and the % or space
                                if( queryStringSpecialChars.Length == 0 )
                                {
                                    queryStringSpecialChars = pair[0] + "=" + pair[1];
                                }
                                else
                                {
                                    queryStringSpecialChars = queryStringSpecialChars + "&" + pair[0] + "=" + pair[1];
                                }

                                pathToAppend = "";
                            }
                        }
                        else
                        {
                            pathToAppend = pathToAppend + "/" + HttpUtility.UrlEncode( ( '\u0020' ).ToString() );
                        }
                    }

                    friendlyPath = friendlyPath + pathToAppend;
                }
            }

            if( queryStringSpecialChars.Length > 0 )
            {
                return AddPage( friendlyPath, pageName ) + "?" + queryStringSpecialChars;
            }
            else
            {
                return AddPage( friendlyPath, pageName );
            }
        }

        /// <summary>
        /// GetFriendlyQueryString gets the Querystring part of the friendly url
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="tab">The tab whose url is being formatted.</param>
        /// <param name="path">The path to format.</param>
        /// <returns>The formatted url</returns>
        /// <history>
        ///		[cnurse]	12/16/2004	created
        ///		[smcculloch]10/10/2005	Regex update for rewritten characters
        /// </history>
        private string GetFriendlyQueryString( TabInfo tab, string path, string pageName )
        {
            string friendlyPath = path;
            Match queryStringMatch = Regex.Match( friendlyPath, "(.[^\\\\?]*)\\\\?(.*)", RegexOptions.IgnoreCase );
            string queryStringSpecialChars = "";

            if( !( queryStringMatch == Match.Empty ) )
            {
                friendlyPath = queryStringMatch.Groups[1].Value;
                friendlyPath = Regex.Replace( friendlyPath, Globals.glbDefaultPage, "", RegexOptions.IgnoreCase );

                string queryString = queryStringMatch.Groups[2].Value.Replace( "&amp;", "&" );
                if( queryString.StartsWith( "?" ) )
                {
                    queryString = queryString.TrimStart( Convert.ToChar( "?" ) );
                }

                string[] nameValuePairs = queryString.Split( Convert.ToChar( "&" ) );
                for( int i = 0; i <= nameValuePairs.Length - 1; i++ )
                {
                    string pathToAppend = "";
                    string[] pair = nameValuePairs[i].Split( Convert.ToChar( "=" ) );

                    //Add name part of name/value pair
                    if( friendlyPath.EndsWith( "/" ) )
                    {
                        pathToAppend = pathToAppend + pair[0];
                    }
                    else
                    {
                        pathToAppend = pathToAppend + "/" + pair[0];
                    }

                    if( pair.Length > 1 )
                    {
                        if( pair[1].Length > 0 )
                        {
                            if( Regex.IsMatch( pair[1], _regexMatch ) == false )
                            {
                                // Contains Non-AlphaNumeric Characters
                                if( pair[0].ToLower() == "tabid" )
                                {
                                    if( Information.IsNumeric( pair[1] ) )
                                    {
                                        if( !( tab == null ) )
                                        {
                                            int tabId = Convert.ToInt32( pair[1] );
                                            if( tab.TabID == tabId )
                                            {
                                                if( ( tab.TabPath != Null.NullString ) && IncludePageName )
                                                {
                                                    pathToAppend = tab.TabPath.Replace( "//", "/" ).TrimStart( '/' ) + "/" + pathToAppend;
                                                }
                                            }
                                        }
                                    }
                                }

                                pathToAppend = pathToAppend + "/" + pair[1].Replace( " ", HttpUtility.UrlEncode( ( '\u0020' ).ToString() ) );
                            }
                            else
                            {
                                // Rewrite into URL, contains only alphanumeric and the % or space
                                if( queryStringSpecialChars.Length == 0 )
                                {
                                    queryStringSpecialChars = pair[0] + "=" + pair[1];
                                }
                                else
                                {
                                    queryStringSpecialChars = queryStringSpecialChars + "&" + pair[0] + "=" + pair[1];
                                }

                                pathToAppend = "";
                            }
                        }
                        else
                        {
                            pathToAppend = pathToAppend + "/" + HttpUtility.UrlEncode( ( '\u0020' ).ToString() );
                        }
                    }

                    friendlyPath = friendlyPath + pathToAppend;
                }
            }

            if( queryStringSpecialChars.Length > 0 )
            {
                return AddPage( friendlyPath, pageName ) + "?" + queryStringSpecialChars;
            }
            else
            {
                return AddPage( friendlyPath, pageName );
            }
        }
    }
}