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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DotNetNuke.UI.WebControls
{

	public class MenuNode : DNNNode
	{

		private MenuNodeCollection m_objNodes;
		private DNNMenu m_objDNNMenu;

		public MenuNode() : base()
		{
		}

		public MenuNode(string strText) : base(strText)
		{
		}

		internal MenuNode(System.Xml.XmlNode objXmlNode, Control ctlOwner) : base(objXmlNode)
		{
			m_objDNNMenu = (DNNMenu)ctlOwner;
		}

		internal MenuNode(Control ctlOwner) : base()
		{
			m_objDNNMenu = (DNNMenu)ctlOwner;
		}


		[Browsable(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public MenuNodeCollection MenuNodes {
			get {
				if (m_objNodes == null)
				{
					m_objNodes = new MenuNodeCollection(this.XmlNode, this.DNNMenu);
				}
				return m_objNodes;
			}
		}

		[Browsable(false)]
		public MenuNode Parent {
			get { return (MenuNode)this.ParentNode; }
		}

		[Browsable(false)]
		public DNNMenu DNNMenu {
			get { return (DNNMenu)m_objDNNMenu; }
		}

		[Bindable(true), DefaultValue(""), PersistenceMode(PersistenceMode.Attribute)]
		public string CssClassOver {
			get { return this.CSSClassHover; }
			set { this.CSSClassHover = value; }
		}

		[Bindable(true), DefaultValue(-1), PersistenceMode(PersistenceMode.Attribute)]
		public int ImageIndex {
			get {
				if (!String.IsNullOrEmpty(this.CustomAttribute("iIdx")))
				{
					return Convert.ToInt32(this.CustomAttribute("iIdx"));
				}
				else
				{
					return -1;
				}
			}
			set { this.CustomAttribute("iIdx", value.ToString()); }
		}

		[Bindable(true), DefaultValue(-1), PersistenceMode(PersistenceMode.Attribute)]
		public int UrlIndex {
			get {
				if (!String.IsNullOrEmpty(this.CustomAttribute("uIdx")))
				{
					return Convert.ToInt32(this.CustomAttribute("uIdx"));
				}
				else
				{
					return -1;
				}
			}
               set { this.CustomAttribute("uIdx", value.ToString()); }
		}


		//Public Property PopulateOnDemand() As Boolean
		//	Get
		//		Return CBool(Me.CustomAttribute("pond", False))
		//	End Get
		//	Set(ByVal Value As Boolean)
		//		Me.SetCustomAttribute("pond", Value)
		//	End Set
		//End Property

		public string LeftHTML {
			get { return this.CustomAttribute("lhtml", ""); }
			set { this.SetCustomAttribute("lhtml", value); }
		}

		public string RightHTML {
			get { return this.CustomAttribute("rhtml", ""); }
			set { this.SetCustomAttribute("rhtml", value); }
		}

		public new MenuNode ParentNode {
			get {
				if ((this.XmlNode.ParentNode != null) && this.XmlNode.ParentNode.NodeType != System.Xml.XmlNodeType.Document) return new MenuNode(this.XmlNode.ParentNode, m_objDNNMenu); 				else return null; 
			}
		}

		private IMenuNodeWriter NodeWriter {
			get {
				if (m_objDNNMenu.IsDownLevel)
				{
					return new MenuNodeWriter(m_objDNNMenu.IsCrawler, m_objDNNMenu.Orientation);
				}
				else
				{
					return null;
					// New MenuNodeUpLevelWriter
				}
			}
		}

		public void Click()
		{
			this.Selected = !this.Selected;
			if (DNNMenu.IsDownLevel)
			{
				foreach (MenuNode objNode in DNNMenu.SelectedMenuNodes) {
					if (objNode.Level > this.Level) objNode.Selected = false; 
				}
			}
			DNNMenu.OnNodeClick(new DNNMenuNodeClickEventArgs(this));
		}

		public virtual void Render(HtmlTextWriter writer)
		{
			NodeWriter.RenderNode(writer, this);
		}

		internal void SetDNNMenu(DNNMenu objMenu)
		{
			m_objDNNMenu = objMenu;
		}

	}
}
