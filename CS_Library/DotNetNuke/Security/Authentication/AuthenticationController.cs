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
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Membership;
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
            Configuration _config = Configuration.GetConfig();
            _portalSettings = PortalController.GetCurrentPortalSettings();
            mProviderTypeName = _config.ProviderTypeName;
        }

        public Array AuthenticationTypes()
        {
            return AuthenticationProvider.Instance( mProviderTypeName ).GetAuthenticationTypes();
        }

        public Entities.Users.UserInfo GetDNNUser( int PortalID, string LoggedOnUserName )
        {
            Entities.Users.UserInfo objUser;

            //TODO: Check the new concept of 3.0 for user in multi portal
            // check if this user exists in database for any portal
            objUser = Entities.Users.UserController.GetUserByName( Null.NullInteger, LoggedOnUserName, false );
            if( objUser != null )
            {
                // Check if user exists in this portal
                if( Entities.Users.UserController.GetUserByName( PortalID, LoggedOnUserName, false ) == null )
                {
                    // The user does not exist in this portal - add them
                    objUser.PortalID = PortalID;
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

        public static AuthenticationStatus GetStatus( int PortalID )
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            string authCookies = Configuration.AUTHENTICATION_STATUS_KEY + "." + PortalID.ToString();
            try
            {
                if( HttpContext.Current.Request.Cookies[authCookies] != null )
                {
                    // get Authentication from cookie
                    FormsAuthenticationTicket AuthenticationTicket = FormsAuthentication.Decrypt( HttpContext.Current.Request.Cookies[authCookies].Value );
                    return ( (AuthenticationStatus)( @Enum.Parse( typeof( AuthenticationStatus ), AuthenticationTicket.UserData ) ) );
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

        public UserInfo ProcessFormAuthentication( string LoggedOnUserName, string LoggedOnPassword ) //DotNetNuke.Entities.Users.UserInfo
        {
            //Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            //Dim _config As Authentication.Configuration = Authentication.Configuration.GetConfig(_portalSettings.PortalId)
            Configuration _config = Configuration.GetConfig();
            UserController objAuthUserController = new UserController();
            Entities.Users.UserController objUsers = new Entities.Users.UserController();

            if( _config.WindowsAuthentication )
            {
                UserInfo objAuthUser = objAuthUserController.GetUser( LoggedOnUserName, LoggedOnPassword );
                return objAuthUser;
            }
            return null;
            //Return -1
        }

        public void AuthenticationLogoff()
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            string authCookies = Configuration.AUTHENTICATION_KEY + "_" + _portalSettings.PortalId.ToString();

            // Log User Off from Cookie Authentication System
            FormsAuthentication.SignOut();
            if( GetStatus( _portalSettings.PortalId ) == AuthenticationStatus.WinLogon )
            {
                SetStatus( _portalSettings.PortalId, AuthenticationStatus.WinLogoff );
            }

            // expire cookies
            if( PortalSecurity.IsInRoles( _portalSettings.AdministratorRoleId.ToString() ) && HttpContext.Current.Request.Cookies["_Tab_Admin_Content" + _portalSettings.PortalId.ToString()] != null )
            {
                HttpContext.Current.Response.Cookies["_Tab_Admin_Content" + _portalSettings.PortalId.ToString()].Value = null;
                HttpContext.Current.Response.Cookies["_Tab_Admin_Content" + _portalSettings.PortalId.ToString()].Path = "/";
                HttpContext.Current.Response.Cookies["_Tab_Admin_Content" + _portalSettings.PortalId.ToString()].Expires = DateTime.Now.AddYears( -30 );
            }

            HttpContext.Current.Response.Cookies["portalaliasid"].Value = null;
            HttpContext.Current.Response.Cookies["portalaliasid"].Path = "/";
            HttpContext.Current.Response.Cookies["portalaliasid"].Expires = DateTime.Now.AddYears( -30 );

            HttpContext.Current.Response.Cookies["portalroles"].Value = null;
            HttpContext.Current.Response.Cookies["portalroles"].Path = "/";
            HttpContext.Current.Response.Cookies["portalroles"].Expires = DateTime.Now.AddYears( -30 );

            // Redirect browser back to portal
            if( _portalSettings.HomeTabId != -1 )
            {
                HttpContext.Current.Response.Redirect( Globals.NavigateURL( _portalSettings.HomeTabId ), true );
            }
            else
            {
                //If (IsAdminTab(_portalSettings.ActiveTab.TabID, _portalSettings.ActiveTab.ParentId)) Then
                if( _portalSettings.ActiveTab.IsAdminTab )
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
            Configuration _config = Configuration.GetConfig();

            UserController objAuthUserController = new UserController();
            string authCookies = Configuration.AUTHENTICATION_KEY + "_" + _portalSettings.PortalId.ToString();
            string LoggedOnUserName = HttpContext.Current.Request.ServerVariables[Configuration.LOGON_USER_VARIABLE];

            if( LoggedOnUserName.Length > 0 )
            {
                Entities.Users.UserInfo objDNNUser;
                UserInfo objAuthUser;

                int intUserId = 0;

                objDNNUser = Entities.Users.UserController.GetUserByName( _portalSettings.PortalId, LoggedOnUserName, false );

                if( objDNNUser != null )
                {
                    intUserId = objDNNUser.UserID;

                    // Synchronize role membership if it's required in settings
                    if( _config.SynchronizeRole )
                    {
                        objAuthUser = objAuthUserController.GetUser( LoggedOnUserName );

                        // user object might be in simple version in none active directory network
                        if( objAuthUser.GUID.Length != 0 )
                        {
                            objAuthUser.UserID = intUserId;
                            UserController.AddUserRoles( _portalSettings.PortalId, objAuthUser );
                        }
                    }
                }
                else
                {
                    // User not exists in DNN database, obtain user info from provider to add new
                    objAuthUser = objAuthUserController.GetUser( LoggedOnUserName );
                    objDNNUser = (Entities.Users.UserInfo)objAuthUser;
                    if( objAuthUser != null )
                    {
                        UserCreateStatus createStatus = objAuthUserController.AddDNNUser( objAuthUser );
                        intUserId = objAuthUser.UserID;
                        SetStatus( _portalSettings.PortalId, AuthenticationStatus.WinLogon );
                    }
                }

                if( intUserId > 0 )
                {
                    FormsAuthentication.SetAuthCookie( Convert.ToString( LoggedOnUserName ), true );
                    SetStatus( _portalSettings.PortalId, AuthenticationStatus.WinLogon );

                    // Get ipAddress for eventLog
                    string ipAddress = "";
                    if( HttpContext.Current.Request.UserHostAddress != null )
                    {
                        ipAddress = HttpContext.Current.Request.UserHostAddress;
                    }

                    EventLogController objEventLog = new EventLogController();
                    LogInfo objEventLogInfo = new LogInfo();
                    objEventLogInfo.AddProperty( "IP", ipAddress );
                    objEventLogInfo.LogPortalID = _portalSettings.PortalId;
                    objEventLogInfo.LogPortalName = _portalSettings.PortalName;
                    objEventLogInfo.LogUserID = intUserId;
                    objEventLogInfo.LogUserName = LoggedOnUserName;
                    objEventLogInfo.AddProperty( "WindowsAuthentication", "True" );
                    objEventLogInfo.LogTypeKey = "LOGIN_SUCCESS";

                    objEventLog.AddLog( objEventLogInfo );
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

        public static void SetStatus( int PortalID, AuthenticationStatus Status )
        {
            string authCookies = Configuration.AUTHENTICATION_STATUS_KEY + "." + PortalID.ToString();
            HttpRequest Request = HttpContext.Current.Request;
            HttpResponse Response = HttpContext.Current.Response;

            FormsAuthenticationTicket AuthenticationTicket = new FormsAuthenticationTicket( 1, authCookies, DateTime.Now, DateTime.Now.AddHours( 1 ), false, Status.ToString() );
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