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
using System.Web.Security;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.HttpModules
{
    public class DNNMembershipModule : IHttpModule
    {
        public string ModuleName
        {
            get
            {
                return "DNNMembershipModule";
            }
        }

        public void Init( HttpApplication application )
        {
            application.AuthenticateRequest += new EventHandler( this.OnAuthenticateRequest );
            application.EndRequest += new EventHandler( this.OnEndRequest );
        }

        public void OnAuthenticateRequest( object s, EventArgs e )
        {
            HttpContext Context = ( (HttpApplication)s ).Context;
            HttpRequest Request = Context.Request;
            HttpResponse Response = Context.Response;

            //First check if we are upgrading/installing
            if( Request.Url.LocalPath.EndsWith( "Install.aspx" ) )
            {
                return;
            }

            //exit if a request for a .net mapping that isn't a content page is made i.e. axd
            if (Request.Url.LocalPath.ToLower().EndsWith(".aspx") == false && Request.Url.LocalPath.ToLower().EndsWith(".asmx") == false)
            {
                return;
            }

            // Obtain PortalSettings from Current Context
            PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();

            if( Request.IsAuthenticated && portalSettings != null )
            {
                RoleController objRoleController = new RoleController();

                UserInfo objUser = UserController.GetCachedUser( portalSettings.PortalId, Context.User.Identity.Name );

                if( !Convert.ToBoolean( Request.Cookies["portalaliasid"] == null ) )
                {
                    FormsAuthenticationTicket PortalCookie = FormsAuthentication.Decrypt( Context.Request.Cookies["portalaliasid"].Value );
                    // check if user has switched portals
                    if( portalSettings.PortalAlias.PortalAliasID != int.Parse( PortalCookie.UserData ) )
                    {
                        // expire cookies if portal has changed
                        Response.Cookies["portalaliasid"].Value = null;
                        Response.Cookies["portalaliasid"].Path = "/";
                        Response.Cookies["portalaliasid"].Expires = DateTime.Now.AddYears( - 30 );

                        Response.Cookies["portalroles"].Value = null;
                        Response.Cookies["portalroles"].Path = "/";
                        Response.Cookies["portalroles"].Expires = DateTime.Now.AddYears( - 30 );
                    }
                }

                // authenticate user and set last login ( this is necessary for users who have a permanent Auth cookie set )
                if( objUser == null || objUser.Membership.LockedOut || objUser.Membership.Approved == false )
                {
                    PortalSecurity objPortalSecurity = new PortalSecurity();
                    objPortalSecurity.SignOut();
                    // Redirect browser back to home page
                    Response.Redirect( Request.RawUrl, true );
                    return;
                }
                else // valid Auth cookie
                {
                    // create cookies if they do not exist yet for this session.
                    if( Request.Cookies["portalroles"] == null )
                    {
                        // keep cookies in sync
                        DateTime CurrentDateTime = DateTime.Now;

                        // create a cookie authentication ticket ( version, user name, issue time, expires every hour, don't persist cookie, roles )
                        FormsAuthenticationTicket PortalTicket = new FormsAuthenticationTicket( 1, objUser.Username, CurrentDateTime, CurrentDateTime.AddHours( 1 ), false, portalSettings.PortalAlias.PortalAliasID.ToString() );
                        // encrypt the ticket
                        string strPortalAliasID = FormsAuthentication.Encrypt( PortalTicket );
                        // send portal cookie to client
                        Response.Cookies["portalaliasid"].Value = strPortalAliasID;
                        Response.Cookies["portalaliasid"].Path = "/";
                        Response.Cookies["portalaliasid"].Expires = CurrentDateTime.AddMinutes( 1 );

                        // get roles from UserRoles table
                        string[] arrPortalRoles = objRoleController.GetRolesByUser( objUser.UserID, portalSettings.PortalId );

                        // create a string to persist the roles
                        string strPortalRoles = String.Join(";", arrPortalRoles);

                        // create a cookie authentication ticket ( version, user name, issue time, expires every hour, don't persist cookie, roles )
                        FormsAuthenticationTicket rolesTicket = new FormsAuthenticationTicket( 1, objUser.Username, CurrentDateTime, CurrentDateTime.AddHours( 1 ), false, strPortalRoles );
                        // encrypt the ticket
                        string strRoles = FormsAuthentication.Encrypt( rolesTicket );
                        // send roles cookie to client
                        Response.Cookies["portalroles"].Value = strRoles;
                        Response.Cookies["portalroles"].Path = "/";
                        Response.Cookies["portalroles"].Expires = CurrentDateTime.AddMinutes( 1 );
                    }

                    if( Request.Cookies["portalroles"] != null )
                    {
                        // get roles from roles cookie
                        if( !String.IsNullOrEmpty( Request.Cookies["portalroles"].Value ))
                        {
                            FormsAuthenticationTicket RoleTicket = FormsAuthentication.Decrypt( Context.Request.Cookies["portalroles"].Value );

                            // convert the string representation of the role data into a string array
                            // and store it in the Roles Property of the User
                            objUser.Roles = RoleTicket.UserData.Split( ';' );
                        }
                        Context.Items.Add( "UserInfo", objUser );
                        Localization.SetLanguage( objUser.Profile.PreferredLocale );
                    }
                }
            }

            if( HttpContext.Current.Items["UserInfo"] == null )
            {
                Context.Items.Add( "UserInfo", new UserInfo() );
            }
        }

        public void OnEndRequest( object s, EventArgs e )
        {
            HttpContext Context = ( (HttpApplication)s ).Context;
            HttpResponse Response = Context.Response;
            //avoid adding to .net 2 as httpOnlyCookies default to true in 2.0
            if( Environment.Version.Major < 2 )
            {
                const string HTTPONLYSTRING = ";HttpOnly";
                foreach( string cookie in Response.Cookies )
                {
                    string path = Response.Cookies[cookie].Path;
                    if( path.EndsWith( HTTPONLYSTRING ) == false )
                    {
                        //append HttpOnly to cookie
                        Response.Cookies[cookie].Path += HTTPONLYSTRING;
                    }
                }
            }
        }

        public void Dispose()
        {
        }
    }
}