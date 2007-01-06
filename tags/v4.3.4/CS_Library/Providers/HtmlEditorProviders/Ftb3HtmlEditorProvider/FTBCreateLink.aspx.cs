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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.UI.UserControls;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.HtmlEditor
{
    /// <summary>
    /// The FTBCreateLink Class provides the DNN Link control for the FTB Provider
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[jobrien]	20051101  created
    /// </history>
    public class FTBCreateLink : PageBase
    {
        protected UrlControl ctlURL;
        protected PlaceHolder phHidden;

        protected void Page_Load( Object sender, EventArgs e )
        {
            // set page title
            string strTitle = PortalSettings.PortalName + " > Insert Link";

            // show copyright credits?
            if( Globals.GetHashValue( Globals.HostSettings["Copyright"], "Y" ) == "Y" )
            {
                strTitle += " ( DNN " + PortalSettings.Version + " )";
            }
            Title = strTitle;

            HtmlInputHidden htmlhidden = new HtmlInputHidden();
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            htmlhidden.ID = "TargetFreeTextBox";
            htmlhidden.Value = Request.Params["ftb"];
            phHidden.Controls.Add( htmlhidden );

            htmlhidden = new HtmlInputHidden();
            htmlhidden.ID = "DNNDomainNameTabid";
            htmlhidden.Value = "http://" + Globals.GetDomainName( Request ) + "/" + Globals.glbDefaultPage + "?tabid=";
            phHidden.Controls.Add( htmlhidden );

            htmlhidden = new HtmlInputHidden();
            htmlhidden.ID = "DNNDomainNameFilePath";
            htmlhidden.Value = "http://" + Globals.GetDomainName( Request ) + _portalSettings.HomeDirectory.Replace( Request.ApplicationPath, "" );
            phHidden.Controls.Add( htmlhidden );
        }
    }
}