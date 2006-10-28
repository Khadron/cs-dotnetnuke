using System;
using System.Diagnostics;
using System.Web.UI;
using DotNetNuke.Security.Authentication;

namespace DotNetNuke.Modules.Authentication
{
    public partial class WindowsSignin : Page
    {
        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }

        //NOTE: The following placeholder declaration is required by the Web Form Designer.
        //Do not delete or move it.
        private Object designerPlaceholderDeclaration;

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();

            if( Request.ServerVariables[ "LOGON_USER" ].Length > 0 )
            {
                AuthenticationController objAuthentication = new AuthenticationController();
                objAuthentication.AuthenticationLogon();
            }
        }

        protected void Page_Load( Object sender, EventArgs e )
        {
            //Put user code to initialize the page here
        }
    }
}