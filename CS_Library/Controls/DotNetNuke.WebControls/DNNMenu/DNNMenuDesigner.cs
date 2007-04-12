//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2005
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
//
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Drawing;

using System;
using System.IO;
using System.Web;

using System.Windows.Forms.Design;
using System.Drawing.Design;

namespace DotNetNuke.UI.WebControls
{

	public class DNNMenuDesigner : System.Web.UI.Design.ControlDesigner
	{
		//--- This class allows us to render the design time mode with custom HTML ---'
		public override string GetDesignTimeHtml()
		{
			// Component is the instance of the component or control that
			// this designer object is associated with. This property is 
			// inherited from System.ComponentModel.ComponentDesigner.
			DNNMenu objMenu = (DNNMenu)Component;

			if (objMenu.ID.Length > 0)
			{
				StringWriter sw = new StringWriter();
				HtmlTextWriter tw = new HtmlTextWriter(sw);
				Label objText = new Label();

				objText.CssClass = objMenu.MenuBarCssClass + " " + objMenu.DefaultNodeCssClass;
				objText.Text = objMenu.ID;

				if (objMenu.Orientation == Orientation.Horizontal)
				{
					objText.Width = new Unit("100%");
				}
				//objText.Height = New Unit(objMenu)
				else
				{
					objText.Height = new Unit(500);
					//---not sure why 100% doesn't work here ---' 'Unit("100%")
					//objText.Width = Unit.Empty
				}
				objText.RenderControl(tw);
				return sw.ToString();
			}
			else
			{
				return null;
			}
		}

		public override bool AllowResize {
			get { return false; }
		}
	}
}
