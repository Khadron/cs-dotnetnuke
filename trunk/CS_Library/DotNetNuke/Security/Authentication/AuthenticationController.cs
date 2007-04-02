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
using System.Web;
using System.Web.Security;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Log.EventLog;

namespace DotNetNuke.Security.Authentication
{
    public class AuthenticationController
    {
        private PortalSettings _portalSettings;
        private string mProcessLog = "";
        private string mProviderTypeName = "";

        public AuthenticationController()
        {
            Configuration configuration = Configuration.GetConfig();
            _portalSettings = PortalController.GetCurrentPortalSettings();
            mProviderTypeName = configuration.ProviderTypeName;
        }

        public Array AuthenticationTypes()
        {
            return AuthenticationProvider.Instance( mProviderTypeName ).GetAuthenticationTypes();
        }

        public Entities.Users.UserInfo GetDNNUser( int portalID, string loggedOnUserName )
        {
            Entities.Users.UserInfo objUser;

            //TODO: Check the new concept of 3.0 for user in multi portal
            // check if this user exists in database for any portal
            objUser = Entities.Users.UserController.GetUserByName( Null.NullInteger, loggedOnUserName, false );
            if( objUser != null )
            {
                // Check if user exists in this portal
                if( Entities.Users.UserController.GetUserByName( portalID, loggedOnUserName, false ) == null )
                {
                    // The user does not exist in this portal - add them
                    objUser.PortalID = portalID;
                    Entities.Users.UserController.CreateUser( ref objUser );
                }
                return objUser;
            }
            else
            {
                // the user does not exist
                return null;
            }
        }

        public static AuthenticationStatus GetStatus( int portalID )
        {
            string authCookies = Configuration.AUTHENTICATION_STATUS_KEY + "." + portalID;
            try
            {
                if( HttpContext.Current.Request.Cookies[authCookies] != null )
                {
                    // get Authentication from cookie
                    FormsAuthenticationTicket authenticationTicket = FormsAuthentication.Decrypt( HttpContext.Current.Request.Cookies[authCookies].Value );
                    return ( (AuthenticationStatus)( Enum.Parse( typeof( AuthenticationStatus ), authenticationTicket.UserData ) ) );
                }
                else
                {
                    return AuthenticationStatus.Undefined;
                }
            }
            catch( Exception )
            {
            }
            return AuthenticationStatus.Undefined;
        }

        public string NetworkStatus()
        {
            return AuthenticationProvider.Instance( mProviderTypeName ).GetNetworkStatus();
        }

        public UserInfo ProcessFormAuthentication( string loggedOnUserName, string loggedOnPassword ) //DotNetNuke.Entities.Users.UserInfo
        {
            Configuration configuration = Configuration.GetConfig();
            UserController objAuthUserController = new UserController();

            if( configuration.WindowsAuthentication )
            {
                UserInfo objAuthUser = objAuthUserController.GetUser( loggedOnUserName, loggedOnPassword );
                return objAuthUser;
            }
            return null;
            //Return -1
        }

        public void AuthenticationLogoff()
        {
            PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();
            string authCookies = Configuration.AUTHENTICATION_KEY + "_" + portalSettings.PortalId;

            // Log User Off from Cookie Authentication System
            FormsAuthentication.SignOut();
            if( GetStatus( portalSettings.PortalId ) == AuthenticationStatus.WinLogon )
            {
                SetStatus( portalSettings.PortalId, AuthenticationStatus.WinLogoff );
            }

            // expire cookies
            if( PortalSecurity.IsInRoles( portalSettings.AdministratorRoleId.ToString() ) && HttpContext.Current.Request.Cookies["_Tab_Admin_Content" + portalSettings.PortalId] != null )
            {
                HttpContext.Current.Response.Cookies["_Tab_Admin_Content" + portalSettings.PortalId].Value = null;
                HttpContext.Current.Response.Cookies["_Tab_Admin_Content" + portalSettings.PortalId].Path = "/";
                HttpContext.Current.Response.Cookies["_Tab_Admin_Content" + portalSettings.PortalId].Expires = DateTime.Now.AddYears( -30 );
            }

            HttpContext.Current.Response.Cookies["portalaliasid"].Value = null;
            HttpContext.Current.Response.Cookies["portalaliasid"].Path = "/";
            HttpContext.Current.Response.Cookies["portalaliasid"].Expires = DateTime.Now.AddYears( -30 );

            HttpContext.Current.Response.Cookies["portalroles"].Value = null;
            HttpContext.Current.Response.Cookies["portalroles"].Path = "/";
            HttpContext.Current.Response.Cookies["portalroles"].Expires = DateTime.Now.AddYears( -30 );

            // Redirect browser back to portal
            if( portalSettings.HomeTabId != -1 )
            {
                HttpContext.Current.Response.Redirect( Globals.NavigateURL( portalSettings.HomeTabId ), true );
            }
            else
            {
                //If (IsAdminTab(_portalSettings.ActiveTab.TabID, _portalSettings.ActiveTab.ParentId)) Then
                if( portalSettings.ActiveTab.IsAdminTab )
                {
                    HttpContext.Current.Response.Redirect( "~/" + Globals.glbDefaultPage, true );
                }
                else
                {
                    HttpContext.Current.Response.Redirect( Globals.NavigateURL(), true );
                }
            }
        }

        public void AuthenticationLogon()
        {
            Configuration configuration = Configuration.GetConfig();

            UserController authUserController = new UserController();
            string authCookies = Configuration.AUTHENTICATION_KEY + "_" + _portalSettings.PortalId;
            string LoggedOnUserName = HttpContext.Current.Request.ServerVariables[Configuration.LOGON_USER_VARIABLE];
            // HACK : Modified to not error if object is null.
            //if( LoggedOnUserName.Length > 0 )
            if (!String.IsNullOrEmpty(LoggedOnUserName))
            {
                UserInfo authUser;

                int intUserId = 0;

                Entities.Users.UserInfo dnnUser = Entities.Users.UserController.GetUserByName( _portalSettings.PortalId, LoggedOnUserName, false );

                if( dnnUser != null )
                {
                    intUserId = dnnUser.UserID;

                    // Synchronize role membership if it's required in settings
                    if( configuration.SynchronizeRole )
                    {
                        authUser = authUserController.GetUser( LoggedOnUserName );

                        // user object might be in simple version in none active directory network
                        if( authUser.GUID.Length != 0 )
                        {
                            authUser.UserID = intUserId;
                            UserController.AddUserRoles( _portalSettings.PortalId, authUser );
                        }
                    }
                }
                else
                {
                    // User not exists in DNN database, obtain user info from provider to add new
                    authUser = authUserController.GetUser( LoggedOnUserName );
                    if( authUser != null )
                    {
                        authUserController.AddDNNUser( authUser );
                        intUserId = authUser.UserID;
                        SetStatus( _portalSettings.PortalId, AuthenticationStatus.WinLogon );
                    }
                }

                if( intUserId > 0 )
                {
                    FormsAuthentication.SetAuthCookie( Convert.ToString( LoggedOnUserName ), true );

                    //check if user has supplied custom value for expiration
                    int PersistentCookieTimeout = 0;
                    if (Config.GetSetting("PersistentCookieTimeout") != null)
                    {
                        PersistentCookieTimeout = int.Parse(Config.GetSetting("PersistentCookieTimeout"));
                        //only use if non-zero, otherwise leave as asp.net value
                        if (PersistentCookieTimeout != 0)
                        {
                            //locate and update cookie
                            string authCookie = FormsAuthentication.FormsCookieName;
                            foreach (string cookie in HttpContext.Current.Response.Cookies)
                            {
                                if (cookie.Equals(authCookie))
                                {
                                    HttpContext.Current.Response.Cookies[cookie].Expires = DateTime.Now.AddMinutes(PersistentCookieTimeout);
                                }
                            }
                        }
                    }

                    SetStatus( _portalSettings.PortalId, AuthenticationStatus.WinLogon );

                    // Get ipAddress for eventLog
                    string ipAddress = "";
                    if( HttpContext.Current.Request.UserHostAddress != null )
                    {
                        ipAddress = HttpContext.Current.Request.UserHostAddress;
                    }

                    EventLogController eventLog = new EventLogController();
                    LogInfo eventLogInfo = new LogInfo();
                    eventLogInfo.AddProperty( "IP", ipAddress );
                    eventLogInfo.LogPortalID = _portalSettings.PortalId;
                    eventLogInfo.LogPortalName = _portalSettings.PortalName;
                    eventLogInfo.LogUserID = intUserId;
                    eventLogInfo.LogUserName = LoggedOnUserName;
                    eventLogInfo.AddProperty( "WindowsAuthentication", "True" );
                    eventLogInfo.LogTypeKey = "LOGIN_SUCCESS";

                    eventLog.AddLog( eventLogInfo );
                }
            }
            else
            {
                // Not Windows Authentication
            }

            // params "logon=windows" does nothing, just to force page to be refreshed
            string strURL = Globals.NavigateURL( _portalSettings.ActiveTab.TabID, "", "logon=windows" );
            HttpContext.Current.Response.Redirect( strURL, true );
        }

        public static void SetStatus( int portalID, AuthenticationStatus status )
        {
            string authCookies = Configuration.AUTHENTICATION_STATUS_KEY + "." + portalID;
            HttpRequest Request = HttpContext.Current.Request;
            HttpResponse Response = HttpContext.Current.Response;

            FormsAuthenticationTicket AuthenticationTicket = new FormsAuthenticationTicket( 1, authCookies, DateTime.Now, DateTime.Now.AddHours( 1 ), false, status.ToString() );
            // encrypt the ticket
            string strAuthentication = FormsAuthentication.Encrypt( AuthenticationTicket );

            if( Request.Cookies[authCookies] != null )
            {
                // expire
                Request.Cookies[authCookies].Value = null;
                Request.Cookies[authCookies].Path = "/";
                Request.Cookies[authCookies].Expires = DateTime.Now.AddYears( -1 );
            }

            Response.Cookies[authCookies].Value = strAuthentication;
            Response.Cookies[authCookies].Path = "/";
            Response.Cookies[authCookies].Expires = DateTime.Now.AddHours( 1 );
        }
    }
}