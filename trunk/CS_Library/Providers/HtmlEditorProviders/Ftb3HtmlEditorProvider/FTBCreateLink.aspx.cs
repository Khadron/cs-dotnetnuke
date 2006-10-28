using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.UI.UserControls;
using Microsoft.VisualBasic;
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

        private void Page_Load( Object sender, EventArgs e )
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
            htmlhidden.Value = "http://" + Globals.GetDomainName( Request ) + Strings.Replace( _portalSettings.HomeDirectory, Request.ApplicationPath, "", 1, -1, 0 );
            phHidden.Controls.Add( htmlhidden );
        }
    }
}