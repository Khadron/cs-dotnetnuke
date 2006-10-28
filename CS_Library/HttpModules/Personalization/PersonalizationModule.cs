using System;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Personalization;

namespace DotNetNuke.HttpModules
{
    public class PersonalizationModule : IHttpModule
    {
        public string ModuleName
        {
            get
            {
                return "PersonalizationModule";
            }
        }

        public void Init( HttpApplication application )
        {
            application.EndRequest += new System.EventHandler( this.OnEndRequest );
        }

        public void OnEndRequest( object s, EventArgs e )
        {
            HttpContext Context = ( (HttpApplication)s ).Context;
            HttpRequest Request = Context.Request;

            if( Request.IsAuthenticated == true )
            {
                // Obtain PortalSettings from Current Context
                PortalSettings _portalSettings = (PortalSettings)Context.Items["PortalSettings"];

                if( !( _portalSettings == null ) )
                {
                    // load the user info object
                    UserInfo userInfo = UserController.GetCurrentUserInfo();

                    if (userInfo.UserID != -1)
                    {
                        PersonalizationController objPersonalization = new PersonalizationController();
                        objPersonalization.SaveProfile(Context, userInfo.UserID, _portalSettings.PortalId);
                    }
                }
            }
        }

        public void Dispose()
        {
        }
    }
}