using System;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using DotNetNuke.Common;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.SearchInput
{
    public partial class SearchInput : PortalModuleBase
    {
        private void ShowHideImages()
        {
            string ShowGoImage = Convert.ToString( Settings["ShowGoImage"] );
            string ShowSearchImage = Convert.ToString( Settings["ShowSearchImage"] );

            bool bShowGoImage = false;
            bool bShowSearchImage = false;

            if( ShowGoImage != null )
            {
                bShowGoImage = Convert.ToBoolean( ShowGoImage );
            }

            if( ShowSearchImage != null )
            {
                bShowSearchImage = Convert.ToBoolean( ShowSearchImage );
            }

            imgSearch.Visible = bShowSearchImage;
            plSearch.Visible = ! bShowSearchImage;
            imgGo.Visible = bShowGoImage;
            cmdGo.Visible = ! bShowGoImage;
        }

        private void SearchExecute()
        {
            int ResultsTabid;

            if( Settings["SearchResultsModule"] != null )
            {
                ResultsTabid = int.Parse( Convert.ToString( Settings["SearchResultsModule"] ) );
            }
            else
            {
                //Get Default Page
                ModuleController objModules = new ModuleController();
                ResultsTabid = objModules.GetModuleByDefinition( PortalSettings.PortalId, "Search Results" ).TabID;
            }
            if( HostSettings.GetHostSetting( "UseFriendlyUrls" ) == "Y" )
            {
                Response.Redirect( Globals.NavigateURL( ResultsTabid ) + "?Search=" + Server.UrlEncode( txtSearch.Text ) );
            }
            else
            {
                Response.Redirect( Globals.NavigateURL( ResultsTabid ) + "&Search=" + Server.UrlEncode( txtSearch.Text ) );
            }
        }

        protected void Page_Load( Object sender, EventArgs e )
        {
            string GoUrl = Localization.GetString( "imgGo.ImageUrl", this.LocalResourceFile );
            string SearchUrl = Localization.GetString( "imgSearch.ImageUrl", this.LocalResourceFile );

            if( GoUrl.StartsWith( "~" ) )
            {
                imgGo.ImageUrl = GoUrl;
            }
            else
            {
                imgGo.ImageUrl = Path.Combine( PortalSettings.HomeDirectory, GoUrl );
            }

            if( SearchUrl.StartsWith( "~" ) )
            {
                imgSearch.ImageUrl = SearchUrl;
            }
            else
            {
                imgSearch.ImageUrl = Path.Combine( PortalSettings.HomeDirectory, SearchUrl );
            }

            plSearch.HelpText = "";
            ShowHideImages();

            cmdGo.Text = Localization.GetString( "cmdGo.Text", this.LocalResourceFile );
        }

        private void imgGo_Click( Object sender, ImageClickEventArgs e )
        {
            SearchExecute();
        }

        protected void cmdGo_Click( object sender, EventArgs e )
        {
            SearchExecute();
        }

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }

        private void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
    }
}