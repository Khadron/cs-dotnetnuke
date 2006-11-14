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

        protected void imgGo_Click( Object sender, ImageClickEventArgs e )
        {
            SearchExecute();
        }

        protected void cmdGo_Click( object sender, EventArgs e )
        {
            SearchExecute();
        }


    }
}