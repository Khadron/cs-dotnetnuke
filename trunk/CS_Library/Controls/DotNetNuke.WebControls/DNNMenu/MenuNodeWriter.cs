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

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using DotNetNuke.UI.Utilities;
namespace DotNetNuke.UI.WebControls
{

	internal class MenuNodeWriter : IMenuNodeWriter
	{
		private MenuNode m_objNode;
		private bool m_blnForceFullMenu;
		private Orientation m_eOrientation;

		public bool ForceFullMenu {
			get { return m_blnForceFullMenu; }
			set { m_blnForceFullMenu = value; }
		}
		public Orientation Orientation {
			get { return m_eOrientation; }
			set { m_eOrientation = value; }
		}

		public MenuNodeWriter()
		{

		}

		public MenuNodeWriter(bool blnForceFullMenu)
		{
			this.ForceFullMenu = blnForceFullMenu;
		}

		public MenuNodeWriter(bool blnForceFullMenu, Orientation eOrientation)
		{
			this.ForceFullMenu = blnForceFullMenu;
			this.Orientation = eOrientation;
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="Node"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jhenning]	9/21/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public void RenderNode(HtmlTextWriter writer, MenuNode Node)
		{
			m_objNode = Node;
			Render(writer);
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jhenning]	9/21/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected void Render(HtmlTextWriter writer)
		{
			MenuNode objLastNode = null;
			string strParentID = "";
			if (m_objNode.DNNMenu.SelectedMenuNodes.Count > 0)
			{
				objLastNode = (MenuNode)m_objNode.DNNMenu.SelectedMenuNodes[m_objNode.DNNMenu.SelectedMenuNodes.Count];
			}

			if ((m_objNode.Parent != null)) strParentID = m_objNode.Parent.ID; 

			if (objLastNode == null || m_objNode.Selected || objLastNode.ID == m_objNode.ID || objLastNode.ID == strParentID || this.ForceFullMenu)
			{
				RenderContents(writer);
			}

			if (m_objNode.HasNodes && (m_objNode.Selected || this.ForceFullMenu))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "Child");
				writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
				if (this.ForceFullMenu || this.Orientation == Orientation.Vertical)
				{
					writer.RenderBeginTag(HtmlTextWriterTag.Div);
				}
				else
				{
					writer.RenderBeginTag(HtmlTextWriterTag.Span);
				}
				RenderChildren(writer);
				writer.RenderEndTag();
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jhenning]	9/21/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected void RenderContents(HtmlTextWriter writer)
		{

			RenderOpenTag(writer);

			RenderNodeIcon(writer);

			RenderNodeText(writer);

			RenderNodeArrow(writer);

			writer.RenderEndTag();

		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jhenning]	9/21/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected void RenderOpenTag(HtmlTextWriter writer)
		{
			string NodeClass = "Node";

			//writer.AddAttribute(HtmlTextWriterAttribute.Class, GetNodeCss(m_objNode))
			writer.AddAttribute(HtmlTextWriterAttribute.Name, m_objNode.ID);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, m_objNode.ID);
			if (this.ForceFullMenu || this.Orientation == Orientation.Vertical)
			{
				writer.AddStyleAttribute("padding-left", (m_objNode.Level * 20).ToString() + "px");
				//WARNING:  HARDCODE
				writer.RenderBeginTag(HtmlTextWriterTag.Div);
			}
			else
			{
				writer.RenderBeginTag(HtmlTextWriterTag.Span);
			}

		}


		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jhenning]	9/21/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected void RenderNodeIcon(HtmlTextWriter writer)
		{
			Label oSpan = new Label();
			if (!String.IsNullOrEmpty(m_objNode.CSSIcon))
			{
				oSpan.CssClass = m_objNode.CSSIcon;
			}
else if (!String.IsNullOrEmpty(m_objNode.DNNMenu.DefaultIconCssClass)) {
				oSpan.CssClass = m_objNode.DNNMenu.DefaultIconCssClass;
			}
			oSpan.RenderBeginTag(writer);

			if (m_objNode.ImageIndex > -1)
			{
				NodeImage m_objNodeImage = m_objNode.DNNMenu.ImageList[m_objNode.ImageIndex];
				if ((m_objNodeImage != null))
				{
					Image objImage = new Image();
					objImage.ImageUrl = m_objNodeImage.ImageUrl;
					objImage.RenderControl(writer);
					writer.Write("&nbsp;");
				}
			}
			oSpan.RenderEndTag(writer);
		}

		protected void RenderNodeArrow(HtmlTextWriter writer)
		{
			if (m_objNode.HasNodes)
			{
				Label oSpan = new Label();
				oSpan.RenderBeginTag(writer);

				if (m_objNode.Level == 0)
				{
					if (!String.IsNullOrEmpty(m_objNode.DNNMenu.RootArrowImage))
					{
						Image objImage = new Image();
						objImage.ImageUrl = m_objNode.DNNMenu.RootArrowImage;
						objImage.RenderControl(writer);
					}
				}
				else
				{
					if (!String.IsNullOrEmpty(m_objNode.DNNMenu.ChildArrowImage))
					{
						Image objImage = new Image();
						objImage.ImageUrl = m_objNode.DNNMenu.ChildArrowImage;
						objImage.RenderControl(writer);
					}
				}
				oSpan.RenderEndTag(writer);
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jhenning]	9/21/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected void RenderNodeText(HtmlTextWriter writer)
		{
			HyperLink _link = new HyperLink();
			string strJS = "";
			bool blnNormalLink = false;
			_link.Text = m_objNode.Text;

			if (!String.IsNullOrEmpty(m_objNode.JSFunction))
			{
				if (m_objNode.JSFunction.EndsWith(";") == false) strJS += m_objNode.JSFunction + ";";
			}
else if (!String.IsNullOrEmpty(m_objNode.DNNMenu.JSFunction)) {
				if (m_objNode.DNNMenu.JSFunction.EndsWith(";") == false) strJS += m_objNode.DNNMenu.JSFunction + ";";
			}

			if (m_objNode.Enabled)
			{
				switch (m_objNode.ClickAction) {
					case eClickAction.PostBack:
					case eClickAction.Expand:
					case eClickAction.None:
						//none included since downlevel
						if (!String.IsNullOrEmpty(strJS)) strJS = "if (eval(\"" + strJS.Replace("\"", "\"\"") + "\") != false) "; 
						strJS += m_objNode.DNNMenu.Page.GetPostBackEventReference(m_objNode.DNNMenu, m_objNode.ID + ClientAPI.COLUMN_DELIMITER + "Click");
						break;
					case eClickAction.Navigate:
						if (!String.IsNullOrEmpty(strJS)) strJS = "if (eval(\"" + strJS.Replace("\"", "\"\"") + "\") != false) "; 
						if (!String.IsNullOrEmpty(m_objNode.DNNMenu.Target))
						{
                                  strJS += "window.frames." + m_objNode.DNNMenu.Target + ".location.href='" + m_objNode.NavigateURL + "'; void(0);";
                                  //FOR SOME REASON THIS DOESNT WORK UNLESS WE HAVE CODE AFTER THE SETTING OF THE HREF...
						}
						else
						{
							if (String.IsNullOrEmpty(strJS))
							{
								blnNormalLink = true;
							}
							else
							{
                                       strJS += "window.location.href='" + m_objNode.NavigateURL + "';"
;
							}
						}

						break;
				}
				if (blnNormalLink)
				{
					_link.NavigateUrl = m_objNode.NavigateURL;
				}
				else
				{
					_link.NavigateUrl = "javascript:" + strJS;
				}
			}

			if (!String.IsNullOrEmpty(m_objNode.ToolTip))
			{
				_link.ToolTip = m_objNode.ToolTip;
			}

			string sCSS = GetNodeCss(m_objNode);
			if (!String.IsNullOrEmpty(sCSS)) _link.CssClass = sCSS; 
			_link.RenderControl(writer);
		}

		private string GetNodeCss(MenuNode oNode)
		{
			string sNodeCss = oNode.DNNMenu.CssClass;

			if (oNode.Level > 0) sNodeCss = oNode.DNNMenu.DefaultChildNodeCssClass; 
			if (!String.IsNullOrEmpty(oNode.CSSClass)) sNodeCss = oNode.CSSClass; 

			if (oNode.Selected)
			{
				if (!String.IsNullOrEmpty(oNode.CSSClassSelected))
				{
                        sNodeCss += " " + oNode.CSSClassSelected;
				}
				else
				{
                        sNodeCss += " " + oNode.DNNMenu.DefaultNodeCssClassSelected;
				}
			}

			return sNodeCss;
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jhenning]	9/21/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected void RenderChildren(HtmlTextWriter writer)
		{
			foreach (MenuNode _elem in m_objNode.MenuNodes) {
				_elem.Render(writer);
			}
		}

	}
}
