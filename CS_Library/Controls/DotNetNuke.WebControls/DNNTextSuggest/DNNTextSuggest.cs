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
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
	[Designer(typeof(Design.WebControls.DNNTextSuggestDesigner)), ToolboxData("<{0}:DNNTextSuggest runat=server></{0}:DNNTextSuggest>")]
	public class DNNTextSuggest : TextBox, IPostBackEventHandler, IClientAPICallbackEventHandler
	{

		public enum eIDTokenChar
		{
			None,
			Brackets,
			Paranthesis
		}

		#region "Events / Delegates"
		public delegate void DNNTextSuggestEventHandler(object source, DNNTextSuggestEventArgs e);
		public delegate void DNNDNNNodeClickHandler(object source, DNNTextSuggestEventArgs e);

		public event DNNDNNNodeClickHandler NodeClick;
		public event DNNTextSuggestEventHandler PopulateOnDemand;
		#endregion

		#region "Member Variables"
		private DNNNodeCollection m_objNodes;
		#endregion

		#region "Constructors"
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	5/6/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public DNNTextSuggest()
		{
		}
		#endregion

		#region "Properties"
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Allows developer to force the rendering of the Menu in DownLevel mode
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/22/2005	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Behavior"), Description("Allows developer to force the rendering of the Menu in DownLevel mode"), DefaultValue(false)]
		public bool ForceDownLevel {
			get 
               {
                   if (ViewState["ForceDownLevel"] == null)
                   {
                       return false;
                   }
                   return (bool)ViewState["ForceDownLevel"]; 
               }
			set { ViewState["ForceDownLevel"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Returns whether the TextSuggest will render DownLevel or not
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/22/2005	Commented
		///		[Jon Henning]	3/9/2005	Requiring XML support on client for uplevel
		/// </history>
		/// -----------------------------------------------------------------------------
		[Browsable(false)]
		public bool IsDownLevel {
			get {
				if (ForceDownLevel || ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.DHTML) == false || ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.XML) == false)
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
		[Category("Paths"), Description("Location of ClientAPI js files"), DefaultValue("")]
		public string ClientAPIScriptPath {
			get 
               {
                   if (ClientAPI.ScriptPath == null)
                   {
                       return string.Empty;
                   }
                   return ClientAPI.ScriptPath; 
               }
			set { ClientAPI.ScriptPath = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Location of dnn.controls.DNNTextSuggest.js file
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// Since 1.1 this path will be the same as the ClientAPI path.
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/22/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Paths"), Description("Location of dnn.controls.DNNTextSuggest.js file"), DefaultValue("")]
		public string TextSuggestScriptPath {
			get {
				if (String.IsNullOrEmpty(ViewState["TextSuggestScriptPath"].ToString()))
				{
					return ClientAPIScriptPath;
				}
				else
				{
					return (string)ViewState["TextSuggestScriptPath"];
				}
			}
			set { ViewState["TextSuggestScriptPath"] = value; }
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
		[Category("Paths"), Description("Directory to find the images for the TextSuggest."), DefaultValue("images/")]
		public string SystemImagesPath {
			get {
				if (ViewState["SystemImagesPath"]== null || String.IsNullOrEmpty(ViewState["SystemImagesPath"].ToString()))
				{
					return "images/";
				}
				else
				{
					return (string)ViewState["SystemImagesPath"];
				}
			}
			set { ViewState["SystemImagesPath"] = value; }
		}


		/// -----------------------------------------------------------------------------
		/// <summary>
		/// This property really makes no sense for this control and should not be here, 
		/// but due to concerns for backwards compatibility I am keeping.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	5/6/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public DNNNodeCollection DNNNodes {
			get {
				if (m_objNodes == null)
				{
					m_objNodes = new DNNNodeCollection(this.ClientID);
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
		public DNNNodeCollection SelectedNodes {
			get {
				DNNNodeCollection objNodes = new DNNNodeCollection("");
				DNNNode objNode;
				if (Page.IsPostBack)
				{
					string[] aryEntries = this.Text.Split((char)this.Delimiter);
					string strText;
					string strKey;
					foreach (string strEntry in aryEntries) {
						//wish I knew regular expressions better...
						if (!String.IsNullOrEmpty(strEntry))
						{
							strText = strEntry;
							strKey = "";
							switch (this.IDToken) {
								case eIDTokenChar.None:
									break;
								case eIDTokenChar.Brackets:
									//text [key]
									int intTextEnd = strEntry.LastIndexOf(" [");
									int intKeyBegin = intTextEnd + " [".Length;
									int intKeyEnd = strEntry.LastIndexOf("]");
									if (intTextEnd > -1 && intKeyEnd > intKeyBegin)
									{
										strText = strEntry.Substring(0, intTextEnd);
										strKey = strEntry.Substring(intKeyBegin, intKeyEnd - intKeyBegin);
									}

									break;
								case eIDTokenChar.Paranthesis:
									break;
							}
							objNode = new DNNNode(strText);
							objNode.Key = strKey;
							objNodes.Add(objNode);
						}
					}
				}
				return objNodes;
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// If ClickAction for a node is set to navigate this is the target frame that 
		///	will do the navigating.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	5/6/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Behavior"), Description("If ClickAction for a node is set to navigate this is the target frame that will do the navigating."), DefaultValue("")]
		public string Target {
			get 
               {
                   if (ViewState["Target"] == null)
                   {
                       return string.Empty;
                   }
                   return ((string)ViewState["Target"]); 
               }
			set { ViewState["Target"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Default Classname for node.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	5/6/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Appearance"), Description("Default Classname for node."), DefaultValue("")]
		public string DefaultNodeCssClass {
			get { return this.CssClass; }
			set { this.CssClass = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Default Classname for child node.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	5/6/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Appearance"), Description("Default Classname for child node."), DefaultValue("")]
		public string DefaultChildNodeCssClass {
			get 
               {
                   if (ViewState["DefaultChildNodeCssClass"] == null)
                   {
                       return string.Empty;
                   }
                   return ((string)ViewState["DefaultChildNodeCssClass"]); 
               }
			set { ViewState["DefaultChildNodeCssClass"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Default Classname for node when hovered.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	5/6/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Appearance"), Description("Default Classname for node when hovered."), DefaultValue("")]
		public string DefaultNodeCssClassOver {
			get 
               {
                   if (ViewState["DefaultNodeCssClassOver"] == null)
                   {
                       return string.Empty;
                   }
                   return ((string)ViewState["DefaultNodeCssClassOver"]); 
               }
			set { ViewState["DefaultNodeCssClassOver"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Default Classname for node when selected.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	5/6/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Appearance"), Description("Default Classname for node when selected."), DefaultValue("")]
		public string DefaultNodeCssClassSelected {
			get 
               {
                   if (ViewState["DefaultNodeCssClassSelected"] == null)
                   {
                       return string.Empty;
                   }
                   return ((string)ViewState["DefaultNodeCssClassSelected"]); 
               }
			set { ViewState["DefaultNodeCssClassSelected"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Default Classname container holding all of suggestion nodes.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	5/6/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Appearance"), Description("Default Classname container holding all of suggestion nodes."), DefaultValue("")]
		public string TextSuggestCssClass {
			get 
               {
                   if (ViewState["TextSuggestCssClass"] == null)
                   {
                       return string.Empty;
                   }
                   return ((string)ViewState["TextSuggestCssClass"]); 
               }
			set { ViewState["TextSuggestCssClass"] = value; }
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
		[Category("Behavior"), Description("Allows you to have a common JS function be invoked for all nodes, unless a different JS function is provided on the node level."), DefaultValue("")]
		public string JSFunction {
			get 
               {
                   if (ViewState["JSFunction"] == null)
                   {
                       return string.Empty;
                   }
                   return ((string)ViewState["JSFunction"]); 
               }
			set { ViewState["JSFunction"] = value; }
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
		[Category("Behavior"), Description("If callbacks are supported/enabled, this javascript function will be invoked with each status change of the xmlhttp request."), DefaultValue("")]
		public string CallbackStatusFunction {
			get 
               {
                   if (ViewState["CallbackStatusFunction"] == null)
                   {
                       return string.Empty;
                   }
                   return ((string)ViewState["CallbackStatusFunction"]); 
               }
			set { ViewState["CallbackStatusFunction"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Specifies a delimiter to be used to allow for multiple entries to be added.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// This should be a safe character that is not going to be used as an entry itself.
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/24/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Behavior"), Description("Specifies a delimiter to be used to allow for multiple entries to be added."), DefaultValue("")]
		public char Delimiter {
			get 
               {
                   if (ViewState["Delimiter"] == null)
                   {
                       return new char();
                   }
                   return ((char)ViewState["Delimiter"]); 
               }
			set { ViewState["Delimiter"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Specifies a type of character to be used to surround/delimit the underlying 
		///	value/id of the selected item
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// An example of the text generated with this set to Brackets would be Smith [123]
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/24/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Behavior"), Description("Specifies a type of character to be used to surround/delimit the underlying value/id of the selected item."), DefaultValue(eIDTokenChar.None)]
		public eIDTokenChar IDToken {
			get
               {
                   if (ViewState["IDToken"] == null)
                   {
                       return eIDTokenChar.None;
                   }
                   return ((eIDTokenChar)ViewState["IDToken"]); 
               }
			set { ViewState["IDToken"] = (int)value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Number of characters typed before a lookup is performed
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/24/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Behavior"), Description("Minimum number of characters typed before a lookup will be invoked"), DefaultValue(1)]
		public int MinCharacterLookup {
			get {
				if (ViewState["MinCharacterLookup"] == null)
				{
					return 1;
				}
				else
				{
					return ((int)ViewState["MinCharacterLookup"]);
				}
			}
			set { ViewState["MinCharacterLookup"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Maximum number of rows to display.  
		/// </summary>
		/// <value></value>
		/// <remarks>
		///	This is important since it will allow the client side code to determine when
		///	a new lookup is needed.  As a developer you need to return MaxSuggestRows + 1
		///	results down to the client in order for it to determine when a lookup is required.
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/24/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Behavior"), Description("Maximum number of rows to display."), DefaultValue(10)]
		public int MaxSuggestRows {
			get {
				if (ViewState["MaxSuggestRows"] == null)
				{
					return 10;
				}
				else
				{
					return ((int)ViewState["MaxSuggestRows"]);
				}
			}
			set { ViewState["MaxSuggestRows"] = value; }
		}


		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Number of milliseconds to wait after keypress before a lookup takes place
		/// </summary>
		/// <value></value>
		/// <remarks>
		///	Defaults to half a second (500)
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/24/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Behavior"), Description("Number of milliseconds to wait after keypress before a lookup takes place."), DefaultValue(500)]
		public int LookupDelay {
			get {
				if (ViewState["LookupDelay"] == null)
				{
					return 500;
				}
				else
				{
					return ((int)ViewState["LookupDelay"]);
				}
			}
			set { ViewState["LookupDelay"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Number of milliseconds to wait after lostfocus before a menu is hidden
		/// </summary>
		/// <value></value>
		/// <remarks>
		///	Defaults to half a second (500)
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	9/29/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Behavior"), Description("Number of milliseconds to wait after focus is lost before a hiding the results."), DefaultValue(500)]
		public int LostFocusDelay {
			get {
				if (ViewState["LostFocusDelay"] == null)
				{
					return 500;
				}
				else
				{
					return ((int)ViewState["LostFocusDelay"]);
				}
			}
			set { ViewState["LostFocusDelay"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Determines if lookup uses a case sensitve match
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/24/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Behavior"), Description("Determines if lookup uses a case sensitve match."), DefaultValue(false)]
		public bool CaseSensitive {
			get {
				if (ViewState["CaseSensitive"] == null)
				{
					return false;
				}
				else
				{
					return ((bool)ViewState["CaseSensitive"]);
				}
			}
			set { ViewState["CaseSensitive"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// This property determines what type of callback is used
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// The Simple callback short-circuits at the Page Init event while the ProcessPage
		/// will run the page through the entire page's lifecycle.
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	9/14/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), DefaultValue(ClientAPICallBackResponse.CallBackTypeCode.Simple), Description("Image to display during a callback"), Category("Behavior")]
		public ClientAPICallBackResponse.CallBackTypeCode CallBackType {
			get {
				if (ViewState["CallBackTypeCode"] != null)
				{
					return (ClientAPICallBackResponse.CallBackTypeCode)ViewState["CallBackTypeCode"];
				}
				else
				{
					return ClientAPICallBackResponse.CallBackTypeCode.Simple;
				}
			}
			set { ViewState["CallBackTypeCode"] = value; }
		}

		#endregion

		#region "OverLoaded Methods"

		protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);
			//writer.AddAttribute(HtmlTextWriterAttribute.Class, Me.CssClass)

			writer.AddAttribute("sysimgpath", this.SystemImagesPath);
			if (!String.IsNullOrEmpty(this.Target)) writer.AddAttribute("target", this.Target); 

			//css attributes
			if (!String.IsNullOrEmpty(this.TextSuggestCssClass)) writer.AddAttribute("tscss", this.TextSuggestCssClass); 
			if (!String.IsNullOrEmpty(this.DefaultNodeCssClass)) writer.AddAttribute("css", this.DefaultNodeCssClass); 
			if (!String.IsNullOrEmpty(this.DefaultChildNodeCssClass)) writer.AddAttribute("csschild", this.DefaultChildNodeCssClass); 
			if (!String.IsNullOrEmpty(this.DefaultNodeCssClassOver)) writer.AddAttribute("csshover", this.DefaultNodeCssClassOver); 
			if (!String.IsNullOrEmpty(this.DefaultNodeCssClassSelected)) writer.AddAttribute("csssel", this.DefaultNodeCssClassSelected); 

			if (!String.IsNullOrEmpty(this.JSFunction)) writer.AddAttribute("js", this.JSFunction); 
			if (!String.IsNullOrEmpty(this.Delimiter.ToString())) writer.AddAttribute("del", this.Delimiter.ToString()); 
			switch (this.IDToken) {
				case eIDTokenChar.None:
					break;
				case eIDTokenChar.Brackets:
					writer.AddAttribute("idtok", "[~]");
					break;
				case eIDTokenChar.Paranthesis:
					writer.AddAttribute("idtok", "(~)");
					break;
			}

			if (this.MinCharacterLookup > 1) writer.AddAttribute("minchar", this.MinCharacterLookup.ToString()); 
			if (this.MaxSuggestRows != 10) writer.AddAttribute("maxrows", this.MaxSuggestRows.ToString()); 
			if (this.LookupDelay != 500) writer.AddAttribute("ludelay", this.LookupDelay.ToString()); 
			if (this.LostFocusDelay != 500) writer.AddAttribute("lfdelay", this.LostFocusDelay.ToString()); 

			if (this.CaseSensitive) writer.AddAttribute("casesens", "1"); 


			writer.AddAttribute("postback", ClientAPI.GetPostBackEventReference(this, "[TEXT]" + ClientAPI.COLUMN_DELIMITER + "Click"));

			if (ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.XMLHTTP))
			{
				writer.AddAttribute("callback", ClientAPI.GetCallbackEventReference(this, "this.getText()", "this.callBackSuccess", "this", "this.callBackFail", "this.callBackStatus", CallBackType));
			}

			if (!String.IsNullOrEmpty(this.CallbackStatusFunction))
			{
				writer.AddAttribute("callbackSF", this.CallbackStatusFunction);
			}

			if (!String.IsNullOrEmpty(this.JSFunction))
			{
				writer.AddAttribute("js", this.JSFunction);
			}
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
		/// 	[Jon Henning]	5/6/2005	Created
		///		[Jon Henning]	2/21/2006	Fixed arg to not pass Click text
		/// </history>
		/// -----------------------------------------------------------------------------
		public virtual void RaisePostBackEvent(string eventArgument)
		{
              string[] args = eventArgument.Split(new String[] { ClientAPI.COLUMN_DELIMITER },StringSplitOptions.None);

			if (args.Length > 1)
			{
				switch (args[1]) {
					case "Click":
						DNNTextSuggestEventArgs oArg = new DNNTextSuggestEventArgs(this.DNNNodes, args[0]);
						OnNodeClick(oArg);
						break;
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
		/// 	[Jon Henning]	10/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public virtual void OnNodeClick(DNNTextSuggestEventArgs e)
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
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			base.Render(writer);
			if (IsDownLevel == false)
			{
				ClientAPI.RegisterStartUpScript(Page, this.ClientID + "_startup", "<script>dnn.controls.initTextSuggest($('" + this.ClientID + "'));</script>");
			}
		}

		public string RaiseClientAPICallbackEvent(string eventArgument)
		{
			if (PopulateOnDemand != null) {
				PopulateOnDemand(this, new DNNTextSuggestEventArgs(this.DNNNodes, eventArgument));
			}
			return this.DNNNodes.ToXml();
		}

		#endregion

		#region "Methods"

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
				ClientAPI.RegisterClientReference(this.Page, ClientAPI.ClientNamespaceReferences.dnn_dom);
				ClientAPI.RegisterClientReference(this.Page, ClientAPI.ClientNamespaceReferences.dnn_xml);
				ClientAPI.RegisterClientReference(this.Page, ClientAPI.ClientNamespaceReferences.dnn_dom_positioning);
				ClientAPI.RegisterClientReference(this.Page, ClientAPI.ClientNamespaceReferences.dnn_xmlhttp);

				if (!ClientAPI.IsClientScriptBlockRegistered(this.Page, "dnn.controls.DNNTextSuggest.js"))
				{
					ClientAPI.RegisterClientScriptBlock(this.Page, "dnn.controls.dnnTextSuggest.js", "<script src=\"" + TextSuggestScriptPath + "dnn.controls.dnnTextSuggest.js\"></script>");
				}
				//ClientAPI.RegisterStartUpScript(Page, Me.ClientID & "_startup", "<script>dnn.controls.initTextSuggest($('" & Me.ClientID & "'));</script>")
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
		public DNNNode FindNode(string strID)
		{
			return this.DNNNodes.FindNode(strID);
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
		public DNNNode FindNodeByKey(string strKey)
		{
			return this.DNNNodes.FindNodeByKey(strKey);
		}

		public void LoadXml(string strXml)
		{
			m_objNodes = new DNNNodeCollection(strXml, "");
		}

		#endregion

	}
}
