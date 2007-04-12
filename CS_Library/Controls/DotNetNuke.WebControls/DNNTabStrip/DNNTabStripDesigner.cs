//
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
//

using System;
using DotNetNuke.UI.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;

//since supporting 1.x of the framework, the panels are not going to be updateable.  Once we move firmly on the 2.0
//platform we will make updateable regions and provide a much richer design-time experience
//http://scottonwriting.net/sowblog/posts/1646.aspx 

namespace DotNetNuke.UI.Design.WebControls
{
	public class DNNTabStripDesigner : ControlDesigner
	{
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// This class allows us to render the design time mode with custom HTML
		/// </summary>
		/// <remarks>
		/// The reason for implementing this class is to allow a default tabstrip to be 
		/// shown at design-time, when no tabs are present in the html
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		public override string GetDesignTimeHtml()
		{
			string strText;
			DNNTabStrip objTabStrip = (DNNTabStrip)base.Component;
			TabStripTabCollection objTabs = objTabStrip.Tabs;
			bool blnFDL = objTabStrip.ForceDownLevel;
			//store original before setting to true
			objTabStrip.ForceDownLevel = true;
			//force downlevel to cause only one tab to render
			if (objTabs.Count == 0)
			{
				objTabs.Add(new DNNTab("Tab 1"));
				objTabs.Add(new DNNTab("Tab 2"));
				Panel objPanel = new Panel();
				objPanel.Width = new Unit("100px");
				objPanel.Height = new Unit("50px");
				((DNNTab)objTabs[0]).Controls.Add(objPanel);
				objTabStrip.SelectedIndex = 0;
				strText = base.GetDesignTimeHtml();
				objTabs.Clear();
			}
			else
			{
				strText = base.GetDesignTimeHtml();
			}
			objTabStrip.ForceDownLevel = blnFDL;
			//restore original value
			return strText;

		}

	}
}
