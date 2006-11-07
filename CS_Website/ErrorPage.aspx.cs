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