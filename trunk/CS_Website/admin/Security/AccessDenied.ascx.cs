using System;
using System.Diagnostics;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

namespace DotNetNuke.Modules.Admin.Security
{
    public partial class AccessDeniedPage : PortalModuleBase
    {
        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
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
            if( Request.QueryString["message"] != "" )
            {
                UI.Skins.Skin.AddModuleMessage( this, HttpUtility.UrlDecode( Request.QueryString["message"] ), ModuleMessage.ModuleMessageType.YellowWarning );
            }
            else
            {
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "AccessDenied", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.YellowWarning );
            }
        }
    }
}