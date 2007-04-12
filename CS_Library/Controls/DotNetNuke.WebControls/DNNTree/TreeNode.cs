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

	public class TreeNode : DNNNode, IStateManager
	{
		static internal string _separator = ":";
		static internal readonly string _checkboxIDSufix = "checkbox";

		private TreeNodeCollection m_objNodes;
          private DNNTree m_objDNNTree;

		public TreeNode() : base()
		{
		}

		public TreeNode(string strText) : base(strText)
		{
		}

		internal TreeNode(System.Xml.XmlNode objXmlNode, Control ctlOwner) : base(objXmlNode)
		{
			m_objDNNTree = (DNNTree)ctlOwner;
		}

		internal TreeNode(Control ctlOwner) : base()
		{
			m_objDNNTree = (DNNTree)ctlOwner;
		}

		//This is here for backwards compatibility
		public new string CssClass {
			get { return base.CSSClass; }
			set { base.CSSClass = value; }
		}

		//This is here for backwards compatibility
		public new string NavigateUrl {
			get { return base.NavigateURL; }
			set { base.NavigateURL = value; }
		}

		[Browsable(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public TreeNodeCollection TreeNodes {
			get {
				if (m_objNodes == null)
				{
					m_objNodes = new TreeNodeCollection(this.XmlNode, this.DNNTree);
				}
				return m_objNodes;
			}
		}

		[Browsable(false)]
		public TreeNode Parent {
			get { return (TreeNode)this.ParentNode; }
		}

		[Browsable(false)]
		public DNNTree DNNTree {
			get { return (DNNTree)m_objDNNTree; }
		}

		[DefaultValue(false), Browsable(false)]
		public bool IsExpanded {
			get {
				string _expanded;
				if (DNNTree.IsDownLevel == false)
				{
					string sExpanded = DotNetNuke.UI.Utilities.ClientAPI.GetClientVariable(m_objDNNTree.Page, m_objDNNTree.ClientID + "_" + this.ClientID + ":expanded");
					if (!String.IsNullOrEmpty(sExpanded))
					{
						_expanded = sExpanded;
					}
					else
					{
						_expanded = this.CustomAttribute("expanded", "false");
					}
				}
				else
				{
					_expanded = this.CustomAttribute("expanded", "false");
				}
				return Convert.ToBoolean(_expanded);
			}
		}


		//<Bindable(True), DefaultValue(False), PersistenceMode(PersistenceMode.Attribute)> _
		//  Public Property CheckBox() As Boolean
		//	Get
		//		Dim _checkBox As Object = Me.CustomAttribute("checkBox", 0)
		//		Return CType(_checkBox, Boolean)
		//	End Get
		//	Set(ByVal Value As Boolean)
		//		Me.CustomAttribute("checkBox", 0) = Value
		//	End Set
		//End Property

		[Bindable(true), DefaultValue(""), PersistenceMode(PersistenceMode.Attribute)]
		public string CssClassOver {
			get { return this.CSSClassHover; }
			set { this.CSSClassHover = value; }
		}

		[Bindable(true), DefaultValue(-1), PersistenceMode(PersistenceMode.Attribute)]
		public int ImageIndex {
			get {
				if (!String.IsNullOrEmpty(this.CustomAttribute("imgIdx")))
				{
					return Convert.ToInt32(this.CustomAttribute("imgIdx"));
				}
				else
				{
					if (this.DNNTree.ImageList.Count > 0)
					{
						return 0;
						//BACKWARDS COMPATIBILITY!!!! SHOULD BE -1
					}
					else
					{
						return -1;
					}
				}
			}
			set { this.CustomAttribute("imgIdx", value.ToString()); }
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


		public new TreeNode ParentNode {
			get {
				if ((this.XmlNode.ParentNode != null) && this.XmlNode.ParentNode.NodeType != System.Xml.XmlNodeType.Document) return new TreeNode(this.XmlNode.ParentNode, m_objDNNTree); 				else return null; 
			}
		}

		public void MakeNodeVisible()
		{
			if ((this.Parent != null))
			{
				this.Parent.Expand();
				this.Parent.MakeNodeVisible();
			}
		}

		private ITreeNodeWriter NodeWriter {
			get {
				if (m_objDNNTree.IsDownLevel)
				{
					return new TreeNodeWriter();
				}
				else
				{
					return null;
					// New TreeNodeUpLevelWriter
				}
			}
		}

		public void Expand()
		{
			if (HasNodes)
			{
				this.SetCustomAttribute("expanded", "true");
				DNNTree.OnExpand(new DNNTreeEventArgs(this));
			}
		}

		public void Collapse()
		{
			if (HasNodes)
			{
				this.SetCustomAttribute("expanded", "false");
				DNNTree.OnCollapse(new DNNTreeEventArgs(this));
			}
		}

		public void Click()
		{
			this.Selected = true;
			DNNTree.OnNodeClick(new DNNTreeNodeClickEventArgs(this));
		}

		public virtual void Render(HtmlTextWriter writer)
		{
			NodeWriter.RenderNode(writer, this);
		}

		internal void SetDNNTree(DNNTree objTree)
		{
			m_objDNNTree = objTree;
		}

		//BACKWARDS COMPATIBILITY ONLY
		#region "IStateManager Interface"
		public bool IsTrackingViewState {
			get { return false; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public void LoadViewState(object state)
		{
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public object SaveViewState()
		{
              return null;
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public void TrackViewState()
		{
		}
		#endregion

	}
}
