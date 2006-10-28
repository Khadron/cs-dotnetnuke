using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DotNetNuke.Security;

namespace DotNetNuke.Common.Controls
{
    /// <summary>
    /// An ActionLess HtmlForm
    /// </summary>
    public class Form : HtmlForm
    {
        protected override void RenderAttributes( HtmlTextWriter writer )
        {
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter( stringWriter );
            base.RenderAttributes( htmlWriter );
            string html = stringWriter.ToString();

            // Locate and replace action attribute
            int StartPoint = html.IndexOf( "action=\"" );
            if( StartPoint >= 0 ) //does action exist?
            {
                int EndPoint = html.IndexOf( "\"", StartPoint + 8 ) + 1;
                html = html.Remove( StartPoint, EndPoint - StartPoint );
                PortalSecurity objSecurity = new PortalSecurity();
                html = html.Insert( StartPoint, "action=\"" + objSecurity.InputFilter( HttpContext.Current.Request.RawUrl, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets ) + "\"" );
            }

            //' Locate and replace id attribute
            if( !( base.ID == null ) )
            {
                StartPoint = html.IndexOf( "id=\"" );
                if( StartPoint >= 0 ) //does id exist?
                {
                    int EndPoint = html.IndexOf( "\"", StartPoint + 4 ) + 1;
                    html = html.Remove( StartPoint, EndPoint - StartPoint );
                    html = html.Insert( StartPoint, "id=\"" + base.ClientID + "\"" );
                }
            }

            writer.Write( html );
        }
    }
}
