using System;
using System.Web;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.HttpModules
{
    public class UsersOnlineModule : IHttpModule
    {
        public string ModuleName
        {
            get
            {
                return "UsersOnlineModule";
            }
        }

        public void Init( HttpApplication application )
        {
            application.AuthorizeRequest += new EventHandler( this.OnAuthorizeRequest );
        }

        public void OnAuthorizeRequest( object s, EventArgs e )
        {
            //First check if we are upgrading/installing
            HttpApplication app = (HttpApplication)s;
            HttpRequest Request = app.Request;
            if( Request.Url.LocalPath.EndsWith( "Install.aspx" ) )
            {
                return;
            }

            // Create a Users Online Controller
            UserOnlineController objUserOnlineController = new UserOnlineController();

            // Is Users Online Enabled?
            if( objUserOnlineController.IsEnabled() )
            {
                // Track the current user
                objUserOnlineController.TrackUsers();
            }
        }

        public void Dispose()
        {
        }
    }
}