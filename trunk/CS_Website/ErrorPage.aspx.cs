using System;
using System.Web.UI;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Services.Exceptions
{
    /// <summary>
    /// Trapped errors are redirected to this universal error page, resulting in a
    /// graceful display.
    /// </summary>
    /// <remarks>
    /// 'get the last server error
    /// 'process this error using the Exception Management Application Block
    /// 'add to a placeholder and place on page
    /// 'catch direct access - No exception was found...you shouldn't end up here unless you go to this aspx page URL directly
    /// </remarks>
    /// <history>
    /// 	[sun1]	1/19/2004	Created
    /// </history>
    public partial class ErrorPage : Page
    {
        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }

        protected void Page_Load( Object sender, EventArgs e )
        {
            //get the last server error
            Exception exc = Server.GetLastError();
            try
            {
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

                PageLoadException lex = new PageLoadException( exc.Message.ToString(), exc );
                //process this error using the Exception Management Application Block
                Exceptions.LogException( lex );
                //add to a placeholder and place on page
                ErrorPlaceHolder.Controls.Add( new ErrorContainer( _portalSettings, "An error has occurred.", lex ).Container );
            }
            catch
            {
                //No exception was found...you shouldn't end up here
                // unless you go to this aspx page URL directly
                ErrorPlaceHolder.Controls.Add( new LiteralControl( "An unhandled error has occurred." ) );
            }
        }
    }
}