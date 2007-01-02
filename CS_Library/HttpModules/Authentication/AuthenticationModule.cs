using System;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Authentication;

namespace DotNetNuke.HttpModules
{
    public class AuthenticationModule : IHttpModule
    {
        public string ModuleName
        {
            get
            {
                return "AuthenticationModule";
            }
        }

        public void Init( HttpApplication application )
        {
            //Call this method to make sure that portalsettings is stored in Context
            PortalSettings portalSettings = Globals.GetPortalSettings();
            AuthenticationController.SetStatus(portalSettings.PortalId, AuthenticationStatus.Undefined);
            application.AuthenticateRequest += new EventHandler(this.OnAuthenticateRequest);
        }

        public void OnAuthenticateRequest( object s, EventArgs e )
        {
            PortalSettings portalSettings = Globals.GetPortalSettings();
            Configuration config = Configuration.GetConfig();

            if( config.WindowsAuthentication )
            {
                HttpRequest Request = HttpContext.Current.Request;
                HttpResponse Response = HttpContext.Current.Response;

                bool blnWinLogon = Request.RawUrl.ToLower().IndexOf( ( Configuration.AUTHENTICATION_LOGON_PAGE ).ToLower() ) > - 1;
                bool blnWinLogoff = ( AuthenticationController.GetStatus( portalSettings.PortalId ) == AuthenticationStatus.WinLogon ) && ( Request.RawUrl.ToLower().IndexOf( ( Configuration.AUTHENTICATION_LOGOFF_PAGE ).ToLower() ) > - 1 );

                if( AuthenticationController.GetStatus( portalSettings.PortalId ) == AuthenticationStatus.Undefined ) //OrElse (blnWinLogon) Then
                {
                    AuthenticationController.SetStatus( portalSettings.PortalId, AuthenticationStatus.WinProcess );
                    string url;
                    if( Request.ApplicationPath == "/" )
                    {
                        url = "/Admin/Security/WindowsSignin.aspx?tabid=" + portalSettings.ActiveTab.TabID;
                    }
                    else
                    {
                        url = Request.ApplicationPath + "/Admin/Security/WindowsSignin.aspx?tabid=" + portalSettings.ActiveTab.TabID;
                    }
                    Response.Redirect( url );
                }
                else if( (  AuthenticationController.GetStatus( portalSettings.PortalId ) != AuthenticationStatus.WinLogoff ) && blnWinLogoff )
                {
                    AuthenticationController objAuthentication = new AuthenticationController();
                    objAuthentication.AuthenticationLogoff();
                }
                else if( ( AuthenticationController.GetStatus( portalSettings.PortalId ) == AuthenticationStatus.WinLogoff ) && blnWinLogon ) // has been logoff before
                {
                    AuthenticationController.SetStatus( portalSettings.PortalId, AuthenticationStatus.Undefined );
                    Response.Redirect( Request.RawUrl );
                }
            }
        }

        public void Dispose()
        {
            // Should check to see why this routine is never called
            //AuthenticationController.SetStatus(AuthenticationStatus.Undefined)
        }
    }
}