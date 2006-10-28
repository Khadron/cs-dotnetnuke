using System;
using System.Web;
using System.Web.Security;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using Microsoft.VisualBasic;

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

            //exit if a request for a .net mapping that isn't a content page is made i.e. axd, asmx
            if( Request.Url.LocalPath.ToLower().EndsWith( ".aspx" ) == false )
            {
                return;
            }

            // Obtain PortalSettings from Current Context
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            if( Request.IsAuthenticated == true && _portalSettings != null )
            {
                string[] arrPortalRoles;
                RoleController objRoleController = new RoleController();

                UserInfo objUser = UserController.GetCachedUser( _portalSettings.PortalId, Context.User.Identity.Name );

                if( !Convert.ToBoolean( Request.Cookies["portalaliasid"] == null ) )
                {
                    FormsAuthenticationTicket PortalCookie = FormsAuthentication.Decrypt( Context.Request.Cookies["portalaliasid"].Value );
                    // check if user has switched portals
                    if( _portalSettings.PortalAlias.PortalAliasID != int.Parse( PortalCookie.UserData ) )
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
                if( objUser == null || objUser.Membership.LockedOut == true || objUser.Membership.Approved == false )
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
                        FormsAuthenticationTicket PortalTicket = new FormsAuthenticationTicket( 1, objUser.Username, CurrentDateTime, CurrentDateTime.AddHours( 1 ), false, _portalSettings.PortalAlias.PortalAliasID.ToString() );
                        // encrypt the ticket
                        string strPortalAliasID = FormsAuthentication.Encrypt( PortalTicket );
                        // send portal cookie to client
                        Response.Cookies["portalaliasid"].Value = strPortalAliasID;
                        Response.Cookies["portalaliasid"].Path = "/";
                        Response.Cookies["portalaliasid"].Expires = CurrentDateTime.AddMinutes( 1 );

                        // get roles from UserRoles table
                        arrPortalRoles = objRoleController.GetRolesByUser( objUser.UserID, _portalSettings.PortalId );

                        // create a string to persist the roles
                        string strPortalRoles = Strings.Join( arrPortalRoles, new string( new char[] {';'} ) );

                        // create a cookie authentication ticket ( version, user name, issue time, expires every hour, don't persist cookie, roles )
                        FormsAuthenticationTicket RolesTicket = new FormsAuthenticationTicket( 1, objUser.Username, CurrentDateTime, CurrentDateTime.AddHours( 1 ), false, strPortalRoles );
                        // encrypt the ticket
                        string strRoles = FormsAuthentication.Encrypt( RolesTicket );
                        // send roles cookie to client
                        Response.Cookies["portalroles"].Value = strRoles;
                        Response.Cookies["portalroles"].Path = "/";
                        Response.Cookies["portalroles"].Expires = CurrentDateTime.AddMinutes( 1 );
                    }

                    if( !Convert.ToBoolean( Request.Cookies["portalroles"] == null ) )
                    {
                        // get roles from roles cookie
                        if( Request.Cookies["portalroles"].Value != "" )
                        {
                            FormsAuthenticationTicket RoleTicket = FormsAuthentication.Decrypt( Context.Request.Cookies["portalroles"].Value );

                            // convert the string representation of the role data into a string array
                            // and store it in the Roles Property of the User
                            objUser.Roles = RoleTicket.UserData.Split( ';' );
                        }
                        Context.Items.Add( "UserInfo", (object)objUser );
                        Localization.SetLanguage( objUser.Profile.PreferredLocale );
                    }
                }
            }

            if( ( (UserInfo)HttpContext.Current.Items["UserInfo"] ) == null )
            {
                Context.Items.Add( "UserInfo", (object)( new UserInfo() ) );
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