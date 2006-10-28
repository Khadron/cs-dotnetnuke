using System;
using System.Diagnostics;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.UI.Skins.Controls
{
    /// <summary></summary>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class Search : SkinObjectBase
    {
        // private members
        private string _submit;
        private string _cssClass;

        private const string MyFileName = "Search.ascx";

        // protected controls

        public string Submit
        {
            get
            {
                return _submit;
            }
            set
            {
                _submit = value;
            }
        }

        public string CssClass
        {
            get
            {
                return _cssClass;
            }
            set
            {
                _cssClass = value;
            }
        }

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

        //*******************************************************
        //
        // The Page_Load server event handler on this page is used
        // to populate the role information for the page
        //
        //*******************************************************

        protected void Page_Load( Object sender, EventArgs e )
        {
            if( ! Page.IsPostBack )
            {
                ClientAPI.RegisterKeyCapture( this.txtSearch, this.cmdSearch, '\r' );

                if( Request.QueryString["Search"] != null )
                {
                    txtSearch.Text = Server.HtmlEncode( Request.QueryString["Search"].ToString() );
                }

                if( Submit != "" )
                {
                    if( Submit.IndexOf( "src=" ) != - 1 )
                    {
                        Submit = Submit.Replace( "src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath );
                    }
                }
                else
                {
                    Submit = Localization.GetString( "Search", Localization.GetResourceFile( this, MyFileName ) );
                }
                cmdSearch.Text = Submit;

                if( CssClass != "" )
                {
                    cmdSearch.CssClass = CssClass;
                }
            }
        }

        protected void cmdSearch_Click( object sender, EventArgs e )
        {
            if( txtSearch.Text != "" )
            {
                ModuleController objModules = new ModuleController();
                int searchTabId = objModules.GetModuleByDefinition( PortalSettings.PortalId, "Search Results" ).TabID;
                if( HostSettings.GetHostSetting( "UseFriendlyUrls" ) == "Y" )
                {
                    Response.Redirect( Globals.NavigateURL( searchTabId ) + "?Search=" + Server.UrlEncode( txtSearch.Text ) );
                }
                else
                {
                    Response.Redirect( Globals.NavigateURL( searchTabId ) + "&Search=" + Server.UrlEncode( txtSearch.Text ) );
                }
            }
        }
    }
}