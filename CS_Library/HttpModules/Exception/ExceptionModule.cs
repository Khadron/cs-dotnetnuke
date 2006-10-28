using System;
using System.Web;
using DotNetNuke.Services.Log.EventLog;

namespace DotNetNuke.HttpModules
{
    public class ExceptionModule : IHttpModule
    {
        public string ModuleName
        {
            get
            {
                return "ExceptionModule";
            }
        }

        public void Init( HttpApplication application )
        {
            application.Error += new EventHandler( this.OnErrorRequest );
        }

        public void OnErrorRequest( object s, EventArgs e )
        {
            HttpContext Context = HttpContext.Current;
            HttpServerUtility Server = Context.Server;

            Exception lex = new Exception( "Unhandled Error: ", Server.GetLastError() );
            ExceptionLogController objExceptionLog = new ExceptionLogController();
            objExceptionLog.AddLog( lex );
        }

        public void Dispose()
        {
        }
    }
}