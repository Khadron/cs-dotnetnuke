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
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Xml;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.UI.WebControls
{

	public class DNNTreeBuilder : ControlBuilder
	{

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tagName"></param>
		/// <param name="attribs"></param>
		/// <returns></returns>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public override Type GetChildControlType(string tagName, IDictionary attribs)
		{
			if (tagName.ToUpper().EndsWith("TreeNode"))
			{
				return typeof(TreeNode);
			}
			return null;
		}

	}

	[ControlBuilderAttribute(typeof(DNNTreeBuilder)), Designer(typeof(DotNetNuke.UI.Design.WebControls.DNNTreeDesigner)), DefaultProperty("Nodes"), ToolboxData("<{0}:DNNTree runat=server></{0}:DNNTree>")]
	public class DNNTree : WebControl, IPostBackEventHandler, IPostBackDataHandler, DotNetNuke.UI.Utilities.IClientAPICallbackEventHandler
	{

		#region "Events / Delegates"
		public delegate void DNNTreeEventHandler(object source, DNNTreeEventArgs e);
		public delegate void DNNTreeNodeClickHandler(object source, DNNTreeNodeClickEventArgs e);

		public event DNNTreeEventHandler Expand;
		public event DNNTreeEventHandler Collapse;
		public event DNNTreeNodeClickHandler NodeClick;
		public event DNNTreeEventHandler PopulateOnDemand;
		#endregion

		#region "Member Variables"
		private TreeNodeCollection m_objNodes;
		private NodeImageCollection m_objImages;
		#endregion

		#region "Constructors"
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
		public DNNTree()
		{
		}
		#endregion

		#region "Properties"
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Allows developer to force the rendering of the tree in DownLevel mode
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/22/2005	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		public bool ForceDownLevel {
			get 
               {
                   if (ViewState["ForceDownLevel"] == null)
                   {
                       return false;
                   }
                   else
                   {
                       return (bool)ViewState["ForceDownLevel"];
                   }
               }
			set { ViewState["ForceDownLevel"] = value; }
		}

		public bool IsCrawler {
			get {
				if (ViewState["IsCrawler"] == null)
				{
					return System.Web.HttpContext.Current.Request.Browser.Crawler;
				}
				else
				{
					return (bool)ViewState["IsCrawler"];
				}

			}
			set { ViewState["IsCrawler"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Returns whether the tree will render DownLevel or not
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/22/2005	Commented
		///		[Jon Henning]	3/9/2005	Requiring XML support on client for uplevel
		///		[Jon Henning]	11/28/2005	rendering downlevel if it is a crawler
		/// </history>
		/// -----------------------------------------------------------------------------
		[Browsable(false)]
		public bool IsDownLevel {
			get {
				if (ForceDownLevel || IsCrawler || DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.DHTML) == false || DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.XML) == false)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Location of ClientAPI js files 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/22/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string ClientAPIScriptPath {
			get { return DotNetNuke.UI.Utilities.ClientAPI.ScriptPath; }
// Commented out because it was breaking the build.. (SM 26 Oct)  
// uncommented out, don't understand how this could have broke build - JH 22 Feb 2005
			set { DotNetNuke.UI.Utilities.ClientAPI.ScriptPath = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Location of dnn.controls.dnntree.js file
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// Since 1.1 this path will be the same as the ClientAPI path.
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/22/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string TreeScriptPath {
			get {
				if (ViewState["TreeScriptPath"] == null || String.IsNullOrEmpty(ViewState["TreeScriptPath"].ToString()))
				{
					return ClientAPIScriptPath;
				}
				else
				{
					return ViewState["TreeScriptPath"].ToString();
				}
			}
			set { ViewState["TreeScriptPath"] = value; }
		}


		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Location of system images (i.e. spacer.gif)
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/22/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Description("Directory to find the images for the menu.  Need to have spacer.gif here!"), DefaultValue("images/")]
		public string SystemImagesPath {
			get {
				if (ViewState["SystemImagesPath"] == null || String.IsNullOrEmpty(ViewState["SystemImagesPath"].ToString()))
				{
					return "images/";
				}
				else
				{
					return ViewState["SystemImagesPath"].ToString();
				}
			}
			set { ViewState["SystemImagesPath"] = value; }
		}


		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Browsable(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public TreeNodeCollection TreeNodes {
			get {
				if (m_objNodes == null)
				{
					m_objNodes = new TreeNodeCollection(this.ClientID, this);
				}
				return m_objNodes;
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Holds collection of selected node objects
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	4/6/2005	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[Browsable(false)]
          public ArrayList SelectedTreeNodes
          {
			get { return this.TreeNodes.FindSelectedNodes(); }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), DefaultValue(10), PersistenceMode(PersistenceMode.Attribute)]
		public int IndentWidth {
			get 
               {
                   if (ViewState["IndentWidth"] == null)
                   {
                       return 10;
                   }
                   return (int)ViewState["IndentWidth"]; 
               }
			set { ViewState["IndentWidth"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), DefaultValue(""), PersistenceMode(PersistenceMode.Attribute)]
		public string CollapsedNodeImage {
			get 
               {
                   if (ViewState["CollapsedNodeImage"] == null || String.IsNullOrEmpty(ViewState["CollapsedNodeImage"].ToString()))
                   {
                       return String.Empty;
                   }
                   return ViewState["CollapsedNodeImage"].ToString(); 
               }
			set { ViewState["CollapsedNodeImage"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), DefaultValue(""), PersistenceMode(PersistenceMode.Attribute)]
		public string ExpandedNodeImage {
			get 
               {
                   if (ViewState["ExpandedNodeImage"] == null || String.IsNullOrEmpty(ViewState["ExpandedNodeImage"].ToString()))
                   {
                       return String.Empty;
                   }
                   return ViewState["ExpandedNodeImage"].ToString(); 
               }
			set { ViewState["ExpandedNodeImage"] = value; }
		}

		[Bindable(true), DefaultValue(""), PersistenceMode(PersistenceMode.Attribute)]
		public string WorkImage {
			get 
               {
                   if (ViewState["WorkImage"] == null || String.IsNullOrEmpty(ViewState["WorkImage"].ToString()))
                   {
                       return String.Empty;
                   }
                   return ViewState["WorkImage"].ToString(); 
               }
			set { ViewState["WorkImage"] = value; }
		}

		[Bindable(true), DefaultValue(5), PersistenceMode(PersistenceMode.Attribute), Category("Behavior")]
		public int AnimationFrames {
			get {
				if (ViewState["AnimationFrames"] != null)
				{
					return (int)ViewState["AnimationFrames"];
				}
				else
				{
					return 5;
				}
			}
			set { ViewState["AnimationFrames"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), DefaultValue(false), PersistenceMode(PersistenceMode.Attribute)]
		public bool CheckBoxes {
			get 
               {
                   if (ViewState["CheckBoxes"] == null)
                   {
                       return false;
                   }
                   return (bool)ViewState["CheckBoxes"]; 
               }
			set { ViewState["CheckBoxes"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), DefaultValue("")]
		public string Target {
			get 
               {
                   if (ViewState["Target"] == null || String.IsNullOrEmpty(ViewState["Target"].ToString()))
                   {
                       return String.Empty;
                   }
                   return ViewState["Target"].ToString(); 
               }
			set { ViewState["Target"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Browsable(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public NodeImageCollection ImageList {
			get {
				if (m_objImages == null)
				{
					m_objImages = new NodeImageCollection();
					if (IsTrackingViewState)
					{
						((IStateManager)m_objImages).TrackViewState();
					}
				}
				return m_objImages;
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), DefaultValue(""), PersistenceMode(PersistenceMode.Attribute)]
		public string DefaultNodeCssClass {
			get { return this.CssClass; }
			set { this.CssClass = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), DefaultValue(""), PersistenceMode(PersistenceMode.Attribute)]
		public string DefaultChildNodeCssClass {
			get 
               {
                   if (ViewState["DefaultChildNodeCssClass"] == null || String.IsNullOrEmpty(ViewState["DefaultChildNodeCssClass"].ToString()))
                   {
                       return String.Empty;
                   }
                   return ViewState["DefaultChildNodeCssClass"].ToString(); 
               }
			set { ViewState["DefaultChildNodeCssClass"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), DefaultValue(""), PersistenceMode(PersistenceMode.Attribute)]
		public string DefaultNodeCssClassOver {
			get 
               {
                   if (ViewState["DefaultNodeCssClassOver"] == null || String.IsNullOrEmpty(ViewState["DefaultNodeCssClassOver"].ToString()))
                   {
                       return String.Empty;
                   }
                   return ViewState["DefaultNodeCssClassOver"].ToString(); 
               }
			set { ViewState["DefaultNodeCssClassOver"] = value; }
		}

		[Bindable(true), DefaultValue(""), PersistenceMode(PersistenceMode.Attribute)]
		public string DefaultNodeCssClassSelected {
			get 
               {
                   if (ViewState["DefaultNodeCssClassSelected"] == null || String.IsNullOrEmpty(ViewState["DefaultNodeCssClassSelected"].ToString()))
                   {
                       return String.Empty;
                   }
                   return ViewState["DefaultNodeCssClassSelected"].ToString(); 
               }
			set { ViewState["DefaultNodeCssClassSelected"] = value; }
		}

		[Bindable(true), DefaultValue(""), PersistenceMode(PersistenceMode.Attribute)]
		public string DefaultIconCssClass {
			get
              {
                  if (ViewState["DefaultIconCssClass"] == null || String.IsNullOrEmpty(ViewState["DefaultIconCssClass"].ToString()))
                  {
                      return String.Empty;
                  } 
                  return ViewState["DefaultIconCssClass"].ToString();
              }
			set { ViewState["DefaultIconCssClass"] = value; }
		}

		[Bindable(true), DefaultValue(12), PersistenceMode(PersistenceMode.Attribute)]
		public int ExpandCollapseImageWidth {
			get {
				if (ViewState["ExpColImgWidth"] != null)
				{
					return (int)ViewState["ExpColImgWidth"];
				}
				else
				{
					return 12;
				}
			}
			set { ViewState["ExpColImgWidth"] = value; }
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
		private IDNNTreeWriter TreeWriter {
			get {
				if (this.IsDownLevel)
				{
					return new DNNTreeWriter();
				}
				else
				{
					return new DNNTreeUpLevelWriter();
				}
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Allows you to have a common JS function be invoked for all nodes, unless
		/// a different JS function is provided on the node level.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// If the client-side function returns false, the action associated to the node 
		/// selection will be canceled
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	4/6/2005	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		public string JSFunction {
			get 
               {
                   if (ViewState["JSFunction"] == null || String.IsNullOrEmpty(ViewState["JSFunction"].ToString()))
                   {
                       return String.Empty;
                   } 
                   return ViewState["JSFunction"].ToString();
               }
			set { ViewState["JSFunction"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Allows the nodes to be populated on the client 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	4/6/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(false), DefaultValue(true), PersistenceMode(PersistenceMode.Attribute)]
		public bool PopulateNodesFromClient {
			get {
				//If DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.XMLHTTP) Then
				if (ViewState["PopNodesFromClient"] == null)
				{
					return true;
				}
				else
				{
					return ((bool)ViewState["PopNodesFromClient"]);
				}
				//Else
				//Return False
				//End If
			}
			set { ViewState["PopNodesFromClient"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// If callbacks are supported/enabled, this javascript function will be invoked
		/// with each status change of the xmlhttp request.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// The Javascript transport does not raise any events.
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	4/6/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), PersistenceMode(PersistenceMode.Attribute)]
		public string CallbackStatusFunction {
			get 
               {
                   if (ViewState["CallbackStatusFunction"] == null || String.IsNullOrEmpty(ViewState["CallbackStatusFunction"].ToString()))
                   {
                       return String.Empty;
                   } 
                   return ViewState["CallbackStatusFunction"].ToString();
               }
			set { ViewState["CallbackStatusFunction"] = value; }
		}

		#endregion

		#region "OverLoaded Methods"

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Responsible for rendering the DNNTree and in an uplevel rendering of the tree, sending down the xml for the child nodes in a ClientAPI variable.
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		///		[Jon Henning 4/6/2005	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		protected override void Render(HtmlTextWriter writer)
		{
			//If Me.IsDownLevel = False Then DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Me.Page, Me.ClientID & "_xml", Me.TreeNodes.ToXml, True)
			TreeWriter.RenderTree(writer, this);
			foreach (Control oCtl in this.Controls) {
				oCtl.RenderControl(writer);
			}
			if (this.IsDownLevel == false)
			{
				ClientAPI.RegisterStartUpScript(Page, this.ClientID + "_startup", "<script>dnn.controls.initTree($('" + this.ClientID + "'));</script>");
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (DotNetNuke.UI.Utilities.ClientAPI.NeedsDNNVariable(this))
			{
				//This is to allow us to add a control to our parent's control collection...  kindof a hack
				this.Page.Load += ParentOnLoad;
			}
			else
			{
				LoadPostedXml();
			}
		}

		protected void ParentOnLoad(object Sender, System.EventArgs e)
		{
			DotNetNuke.UI.Utilities.ClientAPI.RegisterDNNVariableControl(this);
			LoadPostedXml();
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
		protected override object SaveViewState()
		{
			object _baseState = base.SaveViewState();
			object _imagesState = ((IStateManager)ImageList).SaveViewState();
			object[] _newState = new object[2];
			_newState[0] = _baseState;
			_newState[1] = _imagesState;
			return _newState;
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
		protected override void LoadViewState(object state)
		{
			if ((state != null))
			{
				object[] _newState = (object[])state;
				if ((_newState[0] != null))
				{
					base.LoadViewState(_newState[0]);
				}
				if ((_newState[1] != null))
				{
					((IStateManager)ImageList).LoadViewState(_newState[1]);
				}
			}
			if (IsDownLevel && !String.IsNullOrEmpty(ViewState["xml"].ToString())) m_objNodes = new TreeNodeCollection((string)ViewState["xml"], "", this); 
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
		protected override void TrackViewState()
		{
			base.TrackViewState();
			//CType(Root, IStateManager).TrackViewState()
		}
		#endregion

		#region "Event Handlers"
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventArgument"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public virtual void RaisePostBackEvent(string eventArgument)
		{
              string[] args = eventArgument.Split(new string[] { ClientAPI.COLUMN_DELIMITER }, StringSplitOptions.None);

			TreeNode Node = TreeNodes.FindNode(args[0]);

			if ((Node != null))
			{
				if (args.Length > 1)
				{
					switch (args[1]) {
						case "Click":
							if (this.CheckBoxes == false)
							{
								foreach (TreeNode objNode in TreeNodes.FindSelectedNodes()) {
									objNode.Selected = false;
								}
							}

							if (Node.ClickAction == eClickAction.Expand)
							{
								if (Node.DNNNodes.Count == 0 && this.PopulateNodesFromClient)
								{
									//Node.DNNNodes.Count = 0 NEW!!!
									if (PopulateOnDemand != null) {
										PopulateOnDemand(this, new DNNTreeEventArgs(Node));
									}
								}
								if (Node.IsExpanded)
								{
									Node.Collapse();
								}
								else
								{
									Node.Expand();
								}
							}

							Node.Click();
							break;
						case "Checked":
							Node.Selected = !Node.Selected;
							break;
						case "OnDemand":
							if (PopulateOnDemand != null) {
								PopulateOnDemand(this, new DNNTreeEventArgs(Node));
							}

							break;
					}
				}
				else
				{
					if (Node.IsExpanded)
					{
						Node.Collapse();
					}
					else
					{
						Node.Expand();
					}
					if (Node.DNNNodes.Count == 0 && this.PopulateNodesFromClient)
					{
						//Node.DNNNodes.Count = 0 NEW!!!
						if (PopulateOnDemand != null) {
							PopulateOnDemand(this, new DNNTreeEventArgs(Node));
						}
					}
				}
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public virtual void OnExpand(DNNTreeEventArgs e)
		{
			if (Expand != null) {
				Expand(this, e);
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public virtual void OnCollapse(DNNTreeEventArgs e)
		{
			if (Collapse != null) {
				Collapse(this, e);
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	10/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public virtual void OnNodeClick(DNNTreeNodeClickEventArgs e)
		{
			if (NodeClick != null) {
				NodeClick(this, e);
			}
		}

          protected override void OnPreRender(EventArgs e)
          {
               base.OnPreRender(e);
			RegisterClientScript();
			Page.RegisterRequiresPostBack(this);
			UpdateNodes(this.TreeNodes);
			//update all imageindex properties

			if (this.IsDownLevel == false)
			{
				DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(this.Page, this.ClientID + "_xml", this.TreeNodes.ToXml(), true);
			}
			else
			{
				ViewState["xml"] = this.TreeNodes.ToXml();
				if (DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.DHTML))
				{
					DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(this.Page, ClientAPI.ClientNamespaceReferences.dnn);
					if (this.SelectedTreeNodes.Count > 0)
					{
						DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(this.Page, this.ClientID + "_selNode", ((TreeNode)this.SelectedTreeNodes[1]).ToJSON(false), true);
					}
				}

			}

		}

		public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
		{
			//We need to process the individual checkboxes
			//If Me.CheckBoxes Then SelectNodes(Nothing, postCollection)
              return false;
		}

		public void RaisePostDataChangedEvent()
		{

		}

		public string RaiseClientAPICallbackEvent(string eventArgument)
		{
              string[] aryArgs = eventArgument.Split(new String[] { ClientAPI.COLUMN_DELIMITER }, StringSplitOptions.None);
			string strNodeXml = "<root>" + aryArgs[0] + "</root>";
			TreeNodeCollection objNodes = new TreeNodeCollection(strNodeXml, "", this);

			TreeNode objNode = objNodes[0];
			if ((objNode != null))
			{
				if (PopulateOnDemand != null) {
					PopulateOnDemand(this, new DNNTreeEventArgs(objNode));
				}
				TreeNode objTNode = this.FindNode(objNode.ID);
				//if whole tree was populated (i.e. LoadXml, then use the node from the tree

				if ((objTNode != null))
				{
					return objTNode.XmlNode.InnerXml;
					//objNode.ToXML
				}
				else
				{
					//if only passed in node object was updated then use that xml.
					return objNode.XmlNode.InnerXml;
					//objNode.ToXML
				}
			}
			else
			{
				return null;
			}
		}

		#endregion

		#region "Methods"
		//Private Sub SelectNodes(ByVal node As TreeNode, ByVal selectedNodes As System.Collections.Specialized.NameValueCollection)
		//	Dim objTreeNodes As TreeNodeCollection
		//	If Not node Is Nothing Then
		//		If selectedNodes(node.ID & ":checkbox") Is Nothing Then
		//			'We need to perform a check to see if data has changed
		//			node.Selected = False
		//		Else
		//			node.Selected = True
		//		End If
		//		objTreeNodes = node.TreeNodes
		//	Else
		//		objTreeNodes = Me.TreeNodes
		//	End If

		//	For Each childNode As TreeNode In objTreeNodes
		//		SelectNodes(childNode, selectedNodes)
		//	Next
		//End Sub

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Responsible for inputting the hidden field necessary for the ClientAPI to pass variables back in forth
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	4/6/2005	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[Obsolete("RegisterDNNVariableControl on control level is now obsolete.  Use RegisterDNNVariableControl.WebControls")]
		public void RegisterDNNVariableControl()
		{
			DotNetNuke.UI.Utilities.ClientAPI.RegisterDNNVariableControl(this);
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Determines if client supports an uplevel rendering, and if so it registers
		/// the appropriate scripts.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	4/6/2005	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		public void RegisterClientScript()
		{
			if (IsDownLevel == false)
			{
				DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(this.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn_dom);
				DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(this.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn_xml);
				if (this.PopulateNodesFromClient)
				{
					//AndAlso DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.XMLHTTP) Then
					DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(this.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn_xmlhttp);
				}
				if (!ClientAPI.IsClientScriptBlockRegistered(this.Page, "dnn.controls.dnntree.js"))
				{
					ClientAPI.RegisterClientScriptBlock(this.Page, "dnn.controls.dnntree.js", "<script src=\"" + TreeScriptPath + "dnn.controls.dnntree.js\"></script>");
				}
				//ClientAPI.RegisterStartUpScript(Page, Me.ClientID & "_startup", "<script>dnn.controls.initTree($('" & Me.ClientID & "'));</script>")				'wrong place
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Finds Node by passed in Key. 
		/// </summary>
		/// <param name="strID">ID of node</param>
		/// <returns></returns>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	11/17/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public TreeNode FindNode(string strID)
		{
			return this.TreeNodes.FindNode(strID);
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Finds Node by passed in Key. 
		/// </summary>
		/// <param name="strKey">Key of node</param>
		/// <returns></returns>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	11/17/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public TreeNode FindNodeByKey(string strKey)
		{
			return this.TreeNodes.FindNodeByKey(strKey);
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Finds Node by passed in ID and selects it.  Additionally it will expand 
		/// all parent nodes.
		/// </summary>
		/// <param name="strID">ID of node</param>
		/// <returns></returns>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	11/17/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public TreeNode SelectNode(string strID)
		{
			TreeNode objNode = null;

			if (this.TreeNodes.Count > 0)
			{
				objNode = this.FindNode(strID);
				if ((objNode != null))
				{
					objNode.Selected = true;
					TreeNode objParent;
					objParent = objNode.Parent;

					while ((objParent != null)) {
						objParent.Expand();
						objParent = objParent.Parent;
					}

				}
			}

			return objNode;
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Finds Node by passed in Key and selects it.  Additionally it will expand 
		/// all parent nodes.
		/// </summary>
		/// <param name="strKey">Key of node</param>
		/// <returns></returns>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	11/17/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public TreeNode SelectNodeByKey(string strKey)
		{
			TreeNode objNode = null;

			if (this.TreeNodes.Count > 0)
			{
				objNode = this.FindNodeByKey(strKey);
				if ((objNode != null))
				{
					objNode.Selected = true;
					TreeNode objParent;
					objParent = objNode;

					while ((objParent != null)) {
						objParent.Expand();
						objParent = objParent.Parent;
					}
				}
			}

			return objNode;
		}

		private void LoadPostedXml()
		{
			string strXML = "";
			if (this.IsDownLevel == false) strXML = DotNetNuke.UI.Utilities.ClientAPI.GetClientVariable(this.Page, this.ClientID + "_xml"); 
			if (!String.IsNullOrEmpty(strXML)) LoadXml(strXML); 
		}

		public void LoadXml(string strXml)
		{
			m_objNodes = new TreeNodeCollection(strXml, "", this);
		}

		private void UpdateNodes(TreeNodeCollection objNodes)
		{
			//todo: xpath lookup for img attribute so we don't waste time looping.
			foreach (TreeNode objNode in objNodes) {
				if (!String.IsNullOrEmpty(objNode.Image))
				{
					if (this.ImageList.Contains(objNode.Image) == false)
					{
						//THIS CHECK FOR CONTAINS DOES NOT WORK!!!
						objNode.ImageIndex = this.ImageList.Add(objNode.Image);
					}
					else
					{
						objNode.ImageIndex = this.ImageList.IndexOf(objNode.Image);
					}
					objNode.Image = null;
				}
				if (objNode.DNNNodes.Count > 0) UpdateNodes(objNode.TreeNodes); 
			}
		}

		#endregion

	}
}
