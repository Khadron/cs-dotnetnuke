using System;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
//using Common;
//using Utilities;
//using Portals;
//using Security;

namespace DotNetNuke.HtmlEditor
{
    /// <summary>
    /// The FTBInsertSmiley Class inserts a Smiley
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	01/27/2006  created
    /// </history>
    public class FTBInsertSmiley : PageBase
    {
        protected PlaceHolder phHidden;
        protected DataList lstSmileys;

        private void Page_Load( Object sender, EventArgs e )
        {
            // set page title
            string strTitle = PortalSettings.PortalName + " > Insert Smiley";
            string strSmileyPath;
            string strSmileyMapPath;

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

            //Get Host level Smileys
            strSmileyPath = Globals.HostPath + "Smileys/";
            strSmileyMapPath = Globals.HostMapPath + "Smileys\\";

            DirectoryInfo folder = new DirectoryInfo( strSmileyMapPath );
            FileInfo[] arrFiles = folder.GetFiles();
            lstSmileys.DataSource = arrFiles;
            lstSmileys.DataBind();
        }

        public string FormatUrl( string Url )
        {
            return Globals.HostPath + "Smileys/" + Url;
        }
    }
}