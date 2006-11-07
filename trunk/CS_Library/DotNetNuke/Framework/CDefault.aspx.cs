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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.Framework
{
    public class CDefault : PageBase
    {
        public string Comment = "";
        public string Description = "";
        public string KeyWords = "";
        public string Copyright = "";
        public string Generator = "";
        public string Author = "";
        public string Shadows = "";


        public void AddStyleSheet(string id, string href, bool isFirst)
        {
            //Find the placeholder control
            Control objCSS = this.FindControl("CSS");

            if (objCSS != null)
            {
                //First see if we have already added the <LINK> control
                Control objCtrl = Page.Header.FindControl(id);

                if (objCtrl == null)
                {
                    HtmlLink objLink = new HtmlLink();
                    objLink.ID = id;
                    objLink.Attributes["rel"] = "stylesheet";
                    objLink.Attributes["type"] = "text/css";
                    objLink.Href = href;

                    if (isFirst)
                    {
                        //Find the first HtmlLink
                        int iLink;
                        for (iLink = 0; iLink <= objCSS.Controls.Count - 1; iLink++)
                        {
                            if (objCSS.Controls[iLink] is HtmlLink)
                            {
                                break;
                            }
                        }
                        objCSS.Controls.AddAt(iLink, objLink);
                    }
                    else
                    {
                        objCSS.Controls.Add(objLink);
                    }
                }
            }
        }

        public void AddStyleSheet(string id, string href)
        {
            AddStyleSheet(id, href, false);
        }

        /// <summary>
        /// Allows the scroll position on the page to be moved to the top of the passed in control.
        /// </summary>
        /// <param name="objControl">Control to scroll to</param>
        /// <remarks>
        /// </remarks>
        public void ScrollToControl(Control objControl)
        {
            if (ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.Positioning))
            {
                ClientAPI.RegisterClientReference(this, ClientAPI.ClientNamespaceReferences.dnn_dom_positioning); //client side needs to calculate the top of a particluar control (elementTop)
                ClientAPI.RegisterClientVariable(this, "ScrollToControl", objControl.ClientID, true);
                DNNClientAPI.AddBodyOnloadEventHandler(Page, "__dnn_setScrollTop();");
            }
        }
    }
}