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
        protected override void RenderAttributes(HtmlTextWriter writer)
        {
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            base.RenderAttributes(htmlWriter);
            string html = stringWriter.ToString();

            // Locate and replace action attribute
            int startPoint = html.IndexOf("action=\"");
            if (startPoint >= 0) //does action exist?
            {
                int endPoint = html.IndexOf("\"", startPoint + 8) + 1;
                html = html.Remove(startPoint, endPoint - startPoint);
                PortalSecurity objSecurity = new PortalSecurity();
                html = html.Insert(startPoint, "action=\"" + objSecurity.InputFilter(HttpContext.Current.Request.RawUrl, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup) + "\"");
            }

            //' Locate and replace id attribute
            if (base.ID != null)
            {
                startPoint = html.IndexOf("id=\"");
                if (startPoint >= 0) //does id exist?
                {
                    int EndPoint = html.IndexOf("\"", startPoint + 4) + 1;
                    html = html.Remove(startPoint, EndPoint - startPoint);
                    html = html.Insert(startPoint, "id=\"" + base.ClientID + "\"");
                }
            }

            writer.Write(html);
        }
    }
}
