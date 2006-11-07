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
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.HttpModules.Config;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.HttpModules
{
    public class UrlRewriteModule : IHttpModule
    {
        public string ModuleName
        {
            get
            {
                return "UrlRewriteModule";
            }
        }

        public void Init( HttpApplication application )
        {
            application.BeginRequest += new EventHandler( this.OnBeginRequest );
        }

        public void OnBeginRequest( object s, EventArgs e )
        {
            HttpApplication app = (HttpApplication)s;
            HttpServerUtility Server = app.Server;
            HttpRequest Request = app.Request;
            HttpResponse Response = app.Response;
            string requestedPath = app.Request.Url.AbsoluteUri;

            // URL validation
            // check for ".." escape characters commonly used by hackers to traverse the folder tree on the server
            // the application should always use the exact relative location of the resource it is requesting
            string strURL = Request.Url.AbsolutePath;
            string strDoubleDecodeURL = Server.UrlDecode( Server.UrlDecode( Request.RawUrl ) );
            if( strURL.IndexOf( ".." ) != - 1 || strDoubleDecodeURL.IndexOf( ".." ) != - 1 )
            {
                throw ( new HttpException( 404, "Not Found" ) );
            }

            //fix for ASP.NET canonicalization issues http://support.microsoft.com/?kbid=887459
            if( Request.Path.IndexOf( '\u005C' ) >= 0 || Path.GetFullPath( Request.PhysicalPath ) != Request.PhysicalPath )
            {
                throw ( new HttpException( 404, "Not Found" ) );
            }

            //check if we are upgrading/installing
            if( Request.Url.LocalPath.ToLower().EndsWith( "install.aspx" ) )
            {
                return;
            }

            // save original url in context
            app.Context.Items.Add( "UrlRewrite:OriginalUrl", app.Request.Url.AbsoluteUri );

            // Friendly URLs are exposed externally using the following format
            // http://www.domain.com/tabid/###/mid/###/ctl/xxx/default.aspx
            // and processed internally using the following format
            // http://www.domain.com/default.aspx?tabid=###&mid=###&ctl=xxx
            // The system for accomplishing this is based on an extensible Regex rules definition stored in /SiteUrls.config
            string sendTo = "";

            // save and remove the querystring as it gets added back on later
            // path parameter specifications will take precedence over querystring parameters
            string strQueryString = "";
            if( app.Request.Url.Query != "" )
            {
                strQueryString = Request.QueryString.ToString();
                requestedPath = requestedPath.Replace( app.Request.Url.Query, "" );
            }

            // get url rewriting rules
            RewriterRuleCollection rules = RewriterConfiguration.GetConfig().Rules;

            // iterate through list of rules
            int intMatch = - 1;
            for( int intRule = 0; intRule <= rules.Count - 1; intRule++ )
            {
                // check for the existence of the LookFor value
                string strLookFor = "^" + RewriterUtils.ResolveUrl( app.Context.Request.ApplicationPath, rules[intRule].LookFor ) + "$";
                Regex objLookFor = new Regex( strLookFor, RegexOptions.IgnoreCase );
                // if there is a match
                if( objLookFor.IsMatch( requestedPath ) )
                {
                    // create a new URL using the SendTo regex value
                    sendTo = RewriterUtils.ResolveUrl( app.Context.Request.ApplicationPath, objLookFor.Replace( requestedPath, rules[intRule].SendTo ) );
                    // obtain the RegEx match group which contains the parameters
                    Match objMatch = objLookFor.Match( requestedPath );
                    string strParameters = objMatch.Groups[2].Value;
                    // process the parameters
                    if( strParameters.Trim( null ).Length > 0 )
                    {
                        // split the value into an array based on "/" ( ie. /tabid/##/ )
                        strParameters = strParameters.Replace( "\\", "/" );
                        string[] arrParameters = strParameters.Split( '/' );
                        string strParameterDelimiter;
                        string strParameterName;
                        string strParameterValue;
                        // icreate a well formed querystring based on the array of parameters
                        for( int intParameter = 1; intParameter <= arrParameters.Length - 1; intParameter++ )
                        {
                            // ignore the page name
                            if( arrParameters[intParameter].ToLower().IndexOf( ".aspx" ) == - 1 )
                            {
                                // get parameter name
                                strParameterName = arrParameters[intParameter].Trim( null );
                                if( strParameterName.Length > 0 )
                                {
                                    // add parameter to SendTo if it does not exist already
                                    if( sendTo.ToLower().IndexOf( "?" + strParameterName.ToLower() ) == - 1 && sendTo.ToLower().IndexOf( "&" + strParameterName.ToLower() ) == - 1 )
                                    {
                                        // get parameter delimiter
                                        if( sendTo.IndexOf( "?" ) != - 1 )
                                        {
                                            strParameterDelimiter = "&";
                                        }
                                        else
                                        {
                                            strParameterDelimiter = "?";
                                        }
                                        sendTo = sendTo + strParameterDelimiter + strParameterName;
                                        // get parameter value
                                        strParameterValue = "";
                                        if( intParameter < ( arrParameters.Length - 1 ) )
                                        {
                                            intParameter++;
                                            if( arrParameters[intParameter].Trim() != "" )
                                            {
                                                strParameterValue = arrParameters[intParameter].Trim( null );
                                            }
                                        }
                                        // add the parameter value
                                        if( strParameterValue.Length > 0 )
                                        {
                                            sendTo = sendTo + "=" + strParameterValue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    intMatch = intRule;
                    break; // exit as soon as it processes the first match
                }
            }

            // add querystring parameters back to SendTo
            if( strQueryString != "" )
            {
                string[] arrParameters = strQueryString.Split( '&' );
                string strParameterName;
                // iterate through the array of parameters
                for( int intParameter = 0; intParameter <= arrParameters.Length - 1; intParameter++ )
                {
                    // get parameter name
                    strParameterName = arrParameters[intParameter];
                    if( strParameterName.IndexOf( "=" ) != - 1 )
                    {
                        strParameterName = strParameterName.Substring( 0, strParameterName.IndexOf( "=" ) );
                    }
                    // check if parameter already exists
                    if( sendTo.ToLower().IndexOf( "?" + strParameterName.ToLower() ) == - 1 && sendTo.ToLower().IndexOf( "&" + strParameterName.ToLower() ) == - 1 )
                    {
                        // add parameter to SendTo value
                        if( sendTo.IndexOf( "?" ) != - 1 )
                        {
                            sendTo = sendTo + "&" + arrParameters[intParameter];
                        }
                        else
                        {
                            sendTo = sendTo + "?" + arrParameters[intParameter];
                        }
                    }
                }
            }

            // if a match was found to the urlrewrite rules
            if( intMatch != - 1 )
            {
                if( rules[intMatch].SendTo.StartsWith( "~" ) )
                {
                    // rewrite the URL for internal processing
                    RewriterUtils.RewriteUrl( app.Context, sendTo );
                }
                else
                {
                    // it is not possible to rewrite the domain portion of the URL so redirect to the new URL
                    Response.Redirect( sendTo, true );
                }
            }

            // *Note: from this point on we are dealing with a "standard" querystring ( ie. http://www.domain.com/default.aspx?tabid=## )

            int TabId = - 1;
            int PortalId = - 1;
            string DomainName = null;
            string PortalAlias = null;
            PortalAliasInfo objPortalAliasInfo;

            // get TabId from querystring ( this is mandatory for maintaining portal context for child portals )
            try
            {
                if( !( Request.QueryString["tabid"] == null ) )
                {
                    TabId = int.Parse( Request.QueryString["tabid"] );
                }
                // get PortalId from querystring ( this is used for host menu options as well as child portal navigation )
                if( !( Request.QueryString["portalid"] == null ) )
                {
                    PortalId = int.Parse( Request.QueryString["portalid"] );
                }
            }
            catch( Exception )
            {
                //The tabId or PortalId are incorrectly formatted (potential DOS)
                throw ( new HttpException( 404, "Not Found" ) );
            }

            // alias parameter can be used to switch portals
            if( !( Request.QueryString["alias"] == null ) )
            {
                // check if the alias is valid
                if( PortalSettings.GetPortalAliasInfo( Request.QueryString["alias"] ) != null )
                {
                    // check if the domain name contains the alias
                    if( Strings.InStr( 1, Request.QueryString["alias"], DomainName, CompareMethod.Text ) == 0 )
                    {
                        // redirect to the url defined in the alias
                        Response.Redirect( Globals.GetPortalDomainName( Request.QueryString["alias"], Request, true ) );
                    }
                    else // the alias is the same as the current domain
                    {
                        PortalAlias = Request.QueryString["alias"];
                    }
                }
            }

            // parse the Request URL into a Domain Name token
            DomainName = Globals.GetDomainName( Request );

            // PortalId identifies a portal when set
            if( PortalAlias == null )
            {
                if( PortalId != - 1 )
                {
                    PortalAlias = PortalSettings.GetPortalByID( PortalId, DomainName );
                }
            }

            // TabId uniquely identifies a Portal
            if( PortalAlias == null )
            {
                if( TabId != - 1 )
                {
                    // get the alias from the tabid, but only if it is for a tab in that domain
                    PortalAlias = PortalSettings.GetPortalByTab( TabId, DomainName );
                    if( PortalAlias == null || PortalAlias == "" )
                    {
                        //if the TabId is not for the correct domain
                        //see if the correct domain can be found and redirect it
                        objPortalAliasInfo = PortalSettings.GetPortalAliasInfo( DomainName );
                        if( objPortalAliasInfo != null )
                        {
                            if( app.Request.Url.AbsoluteUri.ToLower().StartsWith( "https://" ) )
                            {
                                strURL = "https://" + objPortalAliasInfo.HTTPAlias.Replace( "*.", "" );
                            }
                            else
                            {
                                strURL = "http://" + objPortalAliasInfo.HTTPAlias.Replace( "*.", "" );
                            }
                            if( strURL.ToLower().IndexOf( DomainName.ToLower() ) == - 1 )
                            {
                                strURL += app.Request.Url.PathAndQuery;
                            }
                            Response.Redirect( strURL, true );
                        }
                    }
                }
            }

            // else use the domain name
            if( PortalAlias == null || PortalAlias == "" )
            {
                PortalAlias = DomainName;
            }
            //using the DomainName above will find that alias that is the domainname portion of the Url
            //ie. dotnetnuke.com will be found even if zzz.dotnetnuke.com was entered on the Url
            objPortalAliasInfo = PortalSettings.GetPortalAliasInfo( PortalAlias );
            if( objPortalAliasInfo != null )
            {
                PortalId = objPortalAliasInfo.PortalID;
            }

            // if the portalid is not known
            if( PortalId == - 1 )
            {
                if( !Request.Url.LocalPath.ToLower().EndsWith( Globals.glbDefaultPage.ToLower() ) )
                {
                    // allows requests for aspx pages in custom folder locations to be processed
                    return;
                }
                else
                {
                    //the domain name was not found so try using the host portal's first alias
                    if( Convert.ToString( Globals.HostSettings["HostPortalId"] ) != "" )
                    {
                        PortalId = Convert.ToInt32( Globals.HostSettings["HostPortalId"] );
                        // use the host portal
                        PortalAliasController objPortalAliasController = new PortalAliasController();
                        ArrayList arrPortalAliases;
                        arrPortalAliases = objPortalAliasController.GetPortalAliasArrayByPortalID( int.Parse( Convert.ToString( Globals.HostSettings["HostPortalId"] ) ) );
                        if( arrPortalAliases.Count > 0 )
                        {
                            //Get the first Alias
                            objPortalAliasInfo = (PortalAliasInfo)arrPortalAliases[0];
                            if( app.Request.Url.AbsoluteUri.ToLower().StartsWith( "https://" ) )
                            {
                                strURL = "https://" + objPortalAliasInfo.HTTPAlias.Replace( "*.", "" );
                            }
                            else
                            {
                                strURL = "http://" + objPortalAliasInfo.HTTPAlias.Replace( "*.", "" );
                            }
                            if( TabId != - 1 )
                            {
                                strURL += app.Request.Url.Query;
                            }
                            Response.Redirect( strURL, true );
                        }
                    }
                }
            }

            if( PortalId != - 1 )
            {
                // load the PortalSettings into current context
                PortalSettings _portalSettings = new PortalSettings( TabId, objPortalAliasInfo );
                app.Context.Items.Add( "PortalSettings", _portalSettings );
            }
            else
            {
                // alias does not exist in database
                // and all attempts to find another have failed
                //this should only happen if the HostPortal does not have any aliases
                StreamReader objStreamReader;
                objStreamReader = File.OpenText( Server.MapPath( "~/404.htm" ) );
                string strHTML = objStreamReader.ReadToEnd();
                objStreamReader.Close();
                strHTML = strHTML.Replace( "[DOMAINNAME]", DomainName );
                Response.Write( strHTML );
                Response.End();
            }
        }

        public void Dispose()
        {
        }
    }
}