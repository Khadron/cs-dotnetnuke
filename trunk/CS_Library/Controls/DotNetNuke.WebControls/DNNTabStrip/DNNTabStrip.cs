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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.UI.Design.WebControls;
using DotNetNuke.UI;
using DotNetNuke.UI.Utilities;
using System.Collections;

namespace DotNetNuke.UI.WebControls
{

	//
	//ParseChildren(GetType(Tabs), ChildrenAsProperties:=False, DefaultProperty:="Tabs")

	//To enable parsing of the collection items within a control's tags, the control adds the ParseChildren(true, "Tabs") attribute 
	//to the control. The first argument (true) of ParseChildrenAttribute specifies that the page parser should interpret nested content 
	//within the control's tags as properties and not as child controls. The second argument ("Tabs") provides the name of the inner 
	//default property. When you specify the second argument, content within the control's tags must correspond to the default inner 
	//property (Tab objects) and to nothing else. 
	//http://msdn2.microsoft.com/en-us/library/9txe1d4x.aspx
	//

	[Designer(typeof(DNNTabStripDesigner)), ParseChildren(true, "Tabs"), DefaultProperty("Tabs"), ToolboxData("<{0}:DNNTabStrip runat=server><{0}:DNNTab id=\"Tab1\" Text=\"Tab 1\" runat=\"server\">Tab 1 Content</{0}:DNNTab><{0}:DNNTab id=\"Tab2\" Text=\"Tab 2\" runat=\"server\">Tab 2 Content</{0}:DNNTab></{0}:DNNTabStrip>")]
	public class DNNTabStrip : WebControl, DotNetNuke.UI.Utilities.IClientAPICallbackEventHandler, IPostBackEventHandler, IPostBackDataHandler
	{

		const string TAB_CONTENT_DELIMITER = "~`~`~`~`~`~`";

		public enum eTabRenderMode
		{
			All,
			PostBack,
			CallBack
		}
		#region "Constructors"
		public DNNTabStrip() : base(HtmlTextWriterTag.Div)
		{
		}
		#endregion

		#region "Member Variables"
			//Generic.List(Of Tab)
		private TabStripTabCollection m_objTabs;
		private Hashtable m_objPostedTabData = new Hashtable();
		private DNNTabLabel m_objDefaultDNNTabLabel = new DNNTabLabel();
		private const int TAB_RENDERED = 1;
		private const int TAB_SELECTED = 2;
		private const int TAB_ENABLED = 4;
		#endregion

		#region "Properties"

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
		/// 	[Jon Henning]	8/7/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), PersistenceMode(PersistenceMode.Attribute), Category("Behavior")]
		public string CallbackStatusFunction {
			get { return ((string)ViewState["CallbackStatusFunction"]); }
			set { ViewState["CallbackStatusFunction"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// This javascript function will be invoked whenever the user clicks on a tab
		/// other than the currently selected one
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// To cancel a tab from changing return false from this function
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), PersistenceMode(PersistenceMode.Attribute), Category("Behavior")]
		public string TabClickFunction {
			get { return ((string)ViewState["TabClickFunction"]); }
			set { ViewState["TabClickFunction"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// This javascript function will be invoked whenever the selectedIndex changes
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), PersistenceMode(PersistenceMode.Attribute), Category("Behavior")]
		public string SelectedIndexChangedFunction {
			get { return ((string)ViewState["SelectedIndexChangedFunction"]); }
			set { ViewState["SelectedIndexChangedFunction"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Location of ClientAPI js files 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string ClientAPIScriptPath {
			get { return DotNetNuke.UI.Utilities.ClientAPI.ScriptPath; }
			set { DotNetNuke.UI.Utilities.ClientAPI.ScriptPath = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Location of dnn.controls.dnntabstrip.js file
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string TabStripScriptPath {
			get {
				if (String.IsNullOrEmpty(ViewState["TabScriptPath"].ToString()))
				{
					return ClientAPIScriptPath;
				}
				else
				{
					return (string)ViewState["TabScriptPath"];
				}
			}
			set { ViewState["TabScriptPath"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Allows developer to force the rendering of the Menu in DownLevel mode
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[DefaultValue(false), Category("Behavior")]
		public bool ForceDownLevel {
			get { return (bool)ViewState["ForceDownLevel"]; }
			set { ViewState["ForceDownLevel"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Allows developer to force the rendering of the control as it is shown to a crawler
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[DefaultValue(false), Category("Behavior")]
		public bool IsCrawler {
			get {
				if (String.IsNullOrEmpty(ViewState["IsCrawler"].ToString()) && (System.Web.HttpContext.Current != null))
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
		/// Returns whether the TabStrip will render DownLevel or not
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public bool IsDownLevel {
			get {
				if (ForceDownLevel || IsCrawler || DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.DHTML) == false)
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
		/// Assigns how the tabs will be rendered.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// All - All tabs along with the html of the controls are sent down to the client.   
		/// When user changes the tab dhtml is used to display
		/// Postback - Only the selected tab and the html of the controls within it are sent 
		/// to the client.  When the user clicks another tab a postback occurs causing a new tab 
		/// to display.
		/// Callback - Only the selected tab and the html of the controls within it are sent 
		/// to the client.  When the user clicks another tab a callback occurs to retrieve the html
		/// for the new tab.
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[DefaultValue(eTabRenderMode.CallBack), Category("Behavior")]
		public eTabRenderMode TabRenderMode {
			get {
				if (ViewState["TabRenderMode"] == null)
				{
					return eTabRenderMode.CallBack;
				}
				else
				{
					return ((eTabRenderMode)ViewState["TabRenderMode"]);
				}
				//Else
				//Return False
				//End If
			}
			set { ViewState["TabRenderMode"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Returns the index of the tab that is currently selected
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		private int m_iSelectedIndex = -1;
		[DefaultValue(0), RefreshProperties(RefreshProperties.All)]
		public int SelectedIndex {
			get {
				if (m_iSelectedIndex != -1)
				{
					return m_iSelectedIndex;
				}
else if (String.IsNullOrEmpty(ViewState["SelectedIndex"].ToString())) {
					return 0;
				}
				else
				{
					return (int)ViewState["SelectedIndex"];
				}
			}
			set {
				if ((m_objTabs != null) && value > this.Tabs.Count - 1)
				{
					throw new Exception("Invalid SelectedIndex");
				}
				else
				{
					m_iSelectedIndex = value;
					ViewState["SelectedIndex"] = value;
				}
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Returns the tab that is currently selected
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public DNNTab SelectedTab {
			get {
				if (this.Tabs.Count > SelectedIndex)
				{
					return (DNNTab)Tabs[SelectedIndex];
				}
				return null;
			}
			set {
				for (int i = 0; i <= Tabs.Count - 1; i++) {
					if (((DNNTab)Tabs[i]).ID == value.ID)
					{
						this.SelectedIndex = i;
						break; // TODO: might not be correct. Was : Exit For
					}
				}
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// This property Exposes the tabstrip's tab collection
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// DesignerSerializationVisibilityAttribute - Setting the Content parameter specifies that a visual designer should serialize the contents of the property. In the example, the property contains Tab objects.
		/// PersistenceModeAttribute - Passing the InnerDefaultProperty parameter specifies that a visual designer should persist the property to which the attribute is applied as an inner default property. This means that a visual designer persists the property within the control's tags. The attribute can be applied to only one property, because only one property can be persisted within the control's tags. The property value is not wrapped in a special tag.
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerDefaultProperty), MergableProperty(false)]
		public virtual TabStripTabCollection Tabs {
			get {
				if (m_objTabs == null)
				{
					m_objTabs = new TabStripTabCollection(this);
					//New Generic.List(Of Tab)
				}
				return (TabStripTabCollection)m_objTabs;
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Default properties to use for a tab's label
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// This is more efficient to use (less payload) than specifying the same properties
		/// on each individual tab
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Appearance"), Description("The style to be applied to Label"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public DNNTabLabel DefaultLabel {
			get { return m_objDefaultDNNTabLabel; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Default css class name to use for a tab's container
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// This is more efficient to use (less payload) than specifying the same css class
		/// name on each individual tab
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Appearance")]
		public string DefaultContainerCssClass {
			get {
				if (ViewState["DefaultContainerCssClass"] == null)
				{
					return "";
				}
				else
				{
					return ((string)ViewState["DefaultContainerCssClass"]);
				}
			}
			set { ViewState["DefaultContainerCssClass"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// css class name to use for all tab's container
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	1/15/2007	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Appearance")]
		public string CssTabContainer {
			get {
				if (ViewState["CssTabContainer"] == null)
				{
					return "";
				}
				else
				{
					return ((string)ViewState["CssTabContainer"]);
				}
			}
			set { ViewState["CssTabContainer"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// css class name to use for element holding all content
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	1/15/2007	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Appearance")]
		public string CssContentContainer {
			get {
				if (ViewState["CssContentContainer"] == null)
				{
					return "";
				}
				else
				{
					return ((string)ViewState["CssContentContainer"]);
				}
			}
			set { ViewState["CssContentContainer"] = value; }
		}


		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Image to display during a callback
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/9/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), DefaultValue(""), Description("Image to display during a callback")]
		public string WorkImage {
			get { return (string)ViewState["WorkImage"]; }
			set { ViewState["WorkImage"] = value; }
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
		/// 	[Jon Henning]	8/9/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Bindable(true), DefaultValue(ClientAPICallBackResponse.CallBackTypeCode.Simple), Description("Image to display during a callback"), Category("Behavior")]
		public ClientAPICallBackResponse.CallBackTypeCode CallBackType {
			get {
				if (!String.IsNullOrEmpty(ViewState["CallBackTypeCode"].ToString()))
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

		[Bindable(true), DefaultValue(DotNetNuke.UI.WebControls.Alignment.Top), PersistenceMode(PersistenceMode.Attribute)]
		public DotNetNuke.UI.WebControls.Alignment TabAlignment {
			get {
				if (!String.IsNullOrEmpty(ViewState["TabAlignment"].ToString()))
				{
					return ((Alignment)ViewState["TabAlignment"]);
					//?
				}
				else
				{
					return Alignment.Top;
				}
			}
			set { ViewState["TabAlignment"] = value; }
          }
        
		#endregion

		#region "Event Handlers"

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// The init handler is responsible to register the dnnvariable control.
		/// We are also loading the posted values in this method
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		private void DNNTabStrip_Init(object sender, System.EventArgs e)
		{
			if (DotNetNuke.UI.Utilities.ClientAPI.NeedsDNNVariable(this))
			{
				//This is to allow us to add a control to our parent's control collection...  kindof a hack
				this.Page.Load += ParentOnLoad;
				//Else
				//	LoadPostedVars()
			}
			LoadPostedVars();
			RaisePreLoadPostDataEvents();
		}

		private void RaisePreLoadPostDataEvents()
		{
			foreach (DNNTab objTab in this.Tabs) {
				if (objTab.IsPostBack)
				{
					objTab.RaisePreLoadPostData();
				}
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// The prerender event handler is responsible for registering the required scripts along with 
		/// setting up the postback logic
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/8/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
          /// 
          protected override void OnPreRender(EventArgs e)
          {
              base.OnPreRender(e);
		    RegisterClientScript();
		    Page.RegisterRequiresPostBack(this);

		    if (DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.DHTML))
		    {
			    DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(this.Page, ClientAPI.ClientNamespaceReferences.dnn);
		    }
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			base.Render(writer);
			if (IsDownLevel == false)
			{
				ClientAPI.RegisterStartUpScript(Page, this.ClientID + "_startup", "<script>dnn.controls.initTabStrip($('" + this.ClientID + "'));</script>");
			}
		}

		#endregion

		#region "Overrides"

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// This method is overriden to make sure the custom attributes for our control
		/// get written
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/9/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);
			//Me.DefaultLabel.WriteAttributes(Me)
			if (!String.IsNullOrEmpty(this.DefaultLabel.CssClass)) writer.AddAttribute("css", this.DefaultLabel.CssClass); 
			if (!String.IsNullOrEmpty(this.DefaultLabel.CssClassSelected)) writer.AddAttribute("csssel", this.DefaultLabel.CssClassSelected); 
			if (!String.IsNullOrEmpty(this.DefaultLabel.CssClassHover)) writer.AddAttribute("csshover", this.DefaultLabel.CssClassHover); 
			if (!String.IsNullOrEmpty(this.DefaultLabel.CssClassDisabled)) writer.AddAttribute("cssdisabled", this.DefaultLabel.CssClassDisabled); 
			if (!String.IsNullOrEmpty(this.WorkImage)) writer.AddAttribute("workImage", this.WorkImage); 

			writer.AddAttribute("tabs", GetTabIds());
			//If PopulateNodesFromClient Then
			if (this.IsDownLevel == false)
			{
				switch (TabRenderMode) {
					case eTabRenderMode.CallBack:
						writer.AddAttribute("cbtype", ((int)CallBackType).ToString());
						if (DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.XMLHTTP))
						{
							string strCallback = DotNetNuke.UI.Utilities.ClientAPI.GetCallbackEventReference(this, "'[TABID]'", "this.callBackSuccess", "this", "this.callBackFail", "this.callBackStatus", "[POST]", ClientAPICallBackResponse.CallBackTypeCode.Simple);

							//TODO:  Enhance the callback to accept a string for last param to avoid this hack
							//minor hack to allow substitution of callbacktypes on tab level
							//dnn.xmlhttp.doCallBack('MyDNNTabStrip MyDNNTabStrip','[TABID]',this.callBackSuccess,this,this.callBackFail,this.callBackStatus,null,'[POST]',0);
							//dnn.xmlhttp.doCallBack('MyDNNTabStrip MyDNNTabStrip','[TABID]',this.callBackSuccess,this,this.callBackFail,this.callBackStatus,null,'[POST]',[CBTYPE]);
							int intPos = strCallback.IndexOf("'[POST]'") + "'[POST]'".Length + 1;
							strCallback = strCallback.Substring(0, intPos) + "'[CBTYPE]'" + strCallback.Substring(intPos + 1);
							writer.AddAttribute("callback", strCallback);
						}
else if (this.IsDownLevel) {
							writer.AddAttribute("callback", ClientAPI.GetPostBackClientHyperlink(this, "[TABID]" + ClientAPI.COLUMN_DELIMITER + "OnDemand"));
						}

						if (!String.IsNullOrEmpty(CallbackStatusFunction))
						{
							writer.AddAttribute("callbackSF", CallbackStatusFunction);
						}

						break;
					case eTabRenderMode.PostBack:
						writer.AddAttribute("callback", ClientAPI.GetPostBackClientHyperlink(this, "[TABID]" + ClientAPI.COLUMN_DELIMITER + "OnDemand"));
						break;
					case eTabRenderMode.All:
						break;

				}

				if (!String.IsNullOrEmpty(TabClickFunction))
				{
					writer.AddAttribute("tabClickF", TabClickFunction);
				}

				if (!String.IsNullOrEmpty(SelectedIndexChangedFunction))
				{
					writer.AddAttribute("selIdxF", SelectedIndexChangedFunction);
				}

				writer.AddAttribute("trm", ((int)this.TabRenderMode).ToString());
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// The overridden sub is responsible for rendering our tab's labels along with
		/// the tabs content panels that either are selected, previously rendered, or are 
		/// supposed to be rendered based off of TabRenderMode
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/9/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected override void RenderContents(HtmlTextWriter writer)
		{
			//MyBase.RenderContents(writer)

			switch (this.TabAlignment) {
				case Alignment.Left:
				case Alignment.Top:
					RenderTabs(writer);
					break;
			}

			writer.AddAttribute("id", this.ClientID + "_c");
			if (!String.IsNullOrEmpty(this.CssContentContainer))
			{
				writer.AddAttribute("class", this.CssContentContainer);
			}

			//If ClientAPI.IsInCallback(Me.Page) Then  'new callback logic to delimit portions we want
			//    writer.Write(TAB_CONTENT_DELIMITER)
			//End If
			writer.RenderBeginTag("div ");
			//<--- TRICK ASP.NET to not send down TABLE for FireFox/Mozilla
			foreach (DNNTab objTab in this.Tabs) {
				if (objTab.IsPostBack || objTab.IsSelected || objTab.TabRenderMode == eTabRenderMode.All)
				{
					objTab.RaiseSetupDefaultsEvent(false);
					if (objTab.Visible == false)
					{
						objTab.Visible = true;
						objTab.Style.Add("display", "none");
					}
                         else if (objTab.IsSelected) 
                         {
						if ((objTab.Style["display"] != null))
						{
							objTab.Style.Remove("display");
						}
					}
					objTab.RenderControl(writer);
				}
			}
			writer.RenderEndTag();
			//If ClientAPI.IsInCallback(Me.Page) Then 'new callback logic to delimit portions we want
			//    writer.Write(TAB_CONTENT_DELIMITER)
			//End If

			switch (this.TabAlignment) {
				case Alignment.Bottom:
				case Alignment.Right:
					RenderTabs(writer);
					break;
			}

		}

		private void RenderTabs(HtmlTextWriter writer)
		{
			//writer.AddStyleAttribute("float", "left")
			writer.AddAttribute("id", this.ClientID + "_lc");
			if (!String.IsNullOrEmpty(this.CssTabContainer))
			{
				writer.AddAttribute("class", this.CssTabContainer);
			}
			writer.RenderBeginTag("div ");

			foreach (DNNTab objTab in this.Tabs) {
				objTab.RenderLabel(writer);
			}
			writer.RenderEndTag();
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Make sure controls added between opening and closing tags are of type DNNTab
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/9/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected override void AddParsedSubObject(object obj)
		{
			if (obj is DNNTab)
			{
				base.AddParsedSubObject(obj);
			}
		}
		#endregion

		#region "Public Methods"

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// This funciton is called during a callback.  It is responsible for returning the
		/// html of the passed in tab
		/// </summary>
		/// <param name="eventArgument"></param>
		/// <returns></returns>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/9/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string RaiseClientAPICallbackEvent(string eventArgument)
		{
			DNNTab objTab = this.Tabs.FindTab(eventArgument);
			if ((objTab != null))
			{
				return GetTabContents(objTab);
			}
               return String.Empty;
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Handles tab's postback
		/// </summary>
		/// <param name="eventArgument"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/9/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public virtual void RaisePostBackEvent(string eventArgument)
		{
			string[] args = eventArgument.Split(new string[]{ClientAPI.COLUMN_DELIMITER}, StringSplitOptions.None);

			DNNTab objTab = Tabs.FindTab(args[0]);
			if ((objTab != null))
			{
				if (args.Length > 1)
				{
					switch (args[1]) {
						case "OnDemand":
							this.SelectedTab = objTab;
							break;
					}
				}
				else
				{
					//assume an event with no specific argument to be a ondemand
					this.SelectedTab = objTab;
				}
			}
		}

		public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
		{
              return false;
		}

		public void RaisePostDataChangedEvent()
		{

		}

		#endregion

		#region "Private Methods"
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// This method is responsible for obtaining the HTML of a given tab.
		/// </summary>
		/// <param name="objTab"></param>
		/// <returns></returns>
		/// <remarks>
		/// The original technique used here is the same as the one found on this site.
		/// http://aspnet.4guysfromrolla.com/articles/102203-1.2.aspx
		/// 
		/// EventValidation must be disabled in order to work (.NET 2.0 specific)
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/9/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		private string GetTabContents(DNNTab objTab)
		{
			objTab.RaiseSetupDefaultsEvent(true);

			System.Text.StringBuilder objSB = new System.Text.StringBuilder();
			System.IO.StringWriter objSW = new System.IO.StringWriter(objSB);
			HtmlTextWriter objHW = new HtmlTextWriter(objSW);
			System.Web.UI.HtmlControls.HtmlForm objForm;
			Page objPage;

			objPage = this.Page;
			objForm = DNNTabStrip.FindForm(this);

			if (objForm == null)
			{
				objForm = new System.Web.UI.HtmlControls.HtmlForm();
				objPage.Controls.Add(objForm);
			}
			else
			{
				objForm.Controls.Clear();
			}

			objTab.ID = objTab.UniqueID;
			objForm.Controls.Add(new LiteralControl(TAB_CONTENT_DELIMITER));
			objForm.Controls.Add(objTab);
			objForm.Controls.Add(new LiteralControl(TAB_CONTENT_DELIMITER));
			//Me.SelectedTab = objTab    'JH 1/29/07
			objTab.SetParent(this);
			objTab.Enabled = true;

			//Experimental rendering using new page object
			//instead of clearing the form's controls.  Unfortunately, there is
			//no way without reflection invoking a private member to associate 
			//the httpcontext object to a new page object.
			//initial idea behind came from http://aspnet.4guysfromrolla.com/articles/102203-1.2.aspx

			//objPage = New Page
			//If System.Environment.Version.Major >= 2 Then
			//    'EventValidation must be disabled in order to work...  since we are not .NET 2.0 specific yet we cannot do this here, so it needs to be done in web.config
			//    objPage.GetType.InvokeMember("EnableEventValidation", Reflection.BindingFlags.SetProperty, Nothing, objPage, New Object() {False})
			//End If

			//objPage.EnableViewState = False
			//objForm = New HtmlControls.HtmlForm
			//Dim objValidators As Collection = New Collection

			//For Each objValidator As IValidator In Me.Page.Validators
			//    objValidators.Add(objValidator)
			//Next

			//objPage.DesignerInitialize()    'This causes the Page class's protected InitRecursive() method to be called, which preps a number of properties needed for rendering.
			//objPage.GetType.InvokeMember("SetIntrinsics", Reflection.BindingFlags.InvokeMethod Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance, Nothing, objPage, New Object() {HttpContext.Current})
			//objPage.Controls.Add(objForm)
			//objTab.ID = objTab.UniqueID
			//objForm.Controls.Add(New LiteralControl(TAB_CONTENT_DELIMITER))
			//objForm.Controls.Add(objTab)
			//objForm.Controls.Add(New LiteralControl(TAB_CONTENT_DELIMITER))
			//Me.SelectedTab = objTab
			//objTab.SetParent(Me)
			//objTab.Enabled = True

			//Dim oCtl As Control
			//For Each objValidator As Control In objValidators
			//    'Me.Page.Validators.Remove(objValidator)
			//    'CType(objValidator, Control).Page = objPage
			//    'objPage.Validators.Add(objValidator)
			//    objTab.Controls.Add(objValidator)
			//Next

			objForm.RenderControl(objHW);

               string strReturn = objSB.ToString().Split(new String[]{TAB_CONTENT_DELIMITER}, StringSplitOptions.None)[1];
			//parse out only the contents of the tab's panel by using a delimiter
			HTMLElementCollection objScripts = this.GetScripts(objSB.ToString());
			//GetFormHTML())
			if (objScripts.Count > 0)
			{
                   strReturn += ClientAPI.COLUMN_DELIMITER + objScripts.ToJSON();
			}
			return strReturn;
			//Me.Tabs.FindTab(objTab.ID).RenderControl(objHW)
			//Return objSB.ToString()

		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Loads state of tabs through the ClientAPIs variables set
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/9/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		private void LoadPostedVars()
		{
              string[] aryPairs = DotNetNuke.UI.Utilities.ClientAPI.GetClientVariable(this.Page, this.ClientID + "_tabs").Split(new String[] { "," }, StringSplitOptions.None);
			string[] aryValues;
			foreach (string sPair in aryPairs) {
                   aryValues = sPair.Split(new String[] { "=" }, StringSplitOptions.None);
				if (aryValues.Length == 2)
				{
					m_objPostedTabData.Add(aryValues[0], aryValues[1]);
				}
			}
			AssignSelectedIndex();
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Assigns the selected index for the currently selected tab
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/9/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		private void AssignSelectedIndex()
		{
			for (int i = 0; i <= this.Tabs.Count - 1; i++) {
				if (m_objPostedTabData.ContainsKey(((DNNTab)this.Tabs[i]).ID))
				{
					if (((int)m_objPostedTabData[((DNNTab)this.Tabs[i]).ID] & TAB_SELECTED) != 0)
					{
						this.SelectedIndex = i;
						//Exit For
					}
					if (((int)m_objPostedTabData[((DNNTab)this.Tabs[i]).ID] & TAB_RENDERED) != 0)
					{
						((DNNTab)this.Tabs[i]).SetTabState(true);
					}
					if (((int)m_objPostedTabData[((DNNTab)this.Tabs[i]).ID] & TAB_ENABLED) != 0)
					{
                             ((DNNTab)this.Tabs[i]).Enabled = true;
					}
					else
					{
                             ((DNNTab)this.Tabs[i]).Enabled = false;
					}
				}
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Obtains a list of client-side tabids
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/9/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		private string GetTabIds()
		{
			string sRet = "";
			foreach (DNNTab objTab in this.Tabs) {
                   sRet += objTab.ClientID + ",";
			}
			if (!String.IsNullOrEmpty(sRet))
			{
				return sRet.Substring(0, sRet.Length - 1);
			}
			return "";
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// This routine is necessary in some cases to register the DNNVariable hidden input
		/// </summary>
		/// <param name="Sender"></param>
		/// <param name="e"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/9/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		private void ParentOnLoad(object Sender, System.EventArgs e)
		{
			DotNetNuke.UI.Utilities.ClientAPI.RegisterDNNVariableControl(this);
			//LoadPostedVars()
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Registers client-side script for the control
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/9/2006	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		private void RegisterClientScript()
		{
			if (IsDownLevel == false)
			{
				DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(this.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn_dom);
				DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(this.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn_xml);
				bool blnCallbackExists = this.TabRenderMode == eTabRenderMode.CallBack;
				foreach (DNNTab objTab in this.Tabs) {
					if (objTab.TabRenderMode == eTabRenderMode.CallBack) blnCallbackExists = true; 
					if (blnCallbackExists) break; // TODO: might not be correct. Was : Exit For
 
				}
				if (blnCallbackExists && DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.XMLHTTP))
				{
					DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(this.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn_xmlhttp);
				}
				if (!ClientAPI.IsClientScriptBlockRegistered(this.Page, "dnn.controls.dnntabstrip.js"))
				{
					ClientAPI.RegisterClientScriptBlock(this.Page, "dnn.controls.dnntabstrip.js", "<script src=\"" + TabStripScriptPath + "dnn.controls.dnntabstrip.js\"></script>");
				}
				//ClientAPI.RegisterStartUpScript(Page, Me.ClientID & "_startup", "<script>dnn.controls.initTabStrip($('" & Me.ClientID & "'));</script>")			 'wrong place
			}
		}
		#endregion

		private HTMLElementCollection GetScripts(string HTML)
		{
			string strExpression;
			//= "(?i:(?:<(?<element>script[^/ >]*)(?:\s(?!/)+(?:(?<attr>[^=]+)=(?:""|')(?<attrv>[^""\']+)(" & "?:""|')))*)(?:[^/]*/>|[^/]{0}>(?<text>[\s\S]*)(?<close></\k<element>>+)))"
			//(?i:
			//	(?<element>(?:<script
			//		(?:\s*
			//		(?:
			//			(?<attr>[^=>]*?)
			//			=(?:"|')
			//			(?<attrv>[^"|']*?)
			//			(?:"|')
			//		))*
			//        )
			//	(
			//(?(?=\s*?/>)\s*?/>
			//|
			//                (?:\s*?>
			//	(?:[\s\r\n]*?<!--)?(?<text>[\s\S]*?)
			//                </script>))
			//	))
			//)
			strExpression = "(?i:" + "\t(?<element>(?:<script" + "\t\t(?:\\s*" + "\t\t(?:" + "\t\t\t(?<attr>[^=>]*?)" + "\t\t\t=(?:\"|')" + "\t\t\t(?<attrv>[^\"|']*?)" + "\t\t\t(?:\"|')" + "\t\t))*" + "        )" + "\t(" + "(?(?=\\s*?/>)\\s*?/>" + "|" + "                (?:\\s*?>" + "\t(?:[\\s\\r\\n]*?<!--)?(?<text>[\\s\\S]*?)" + "                </script>))" + "\t)" + "))";

			System.Text.RegularExpressions.Regex oRE = new System.Text.RegularExpressions.Regex(strExpression, System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);

			string strGroup;
			HTMLElementCollection oCol = new HTMLElementCollection();
			HTMLElement objElement;
			ArrayList objAttr = new ArrayList();
			//Todo: the association between attr and attrv is a minor hack here... think of something better!
			int intAttr;
			foreach (System.Text.RegularExpressions.Match oMatch in oRE.Matches(HTML)) {
				objElement = null;

				for (int iGroup = 0; iGroup <= oMatch.Groups.Count - 1; iGroup++) {
					strGroup = oRE.GroupNameFromNumber(iGroup);
					if (strGroup == "attr") objAttr = new ArrayList(); 
					intAttr = 1;
					foreach (System.Text.RegularExpressions.Capture oCapture in oMatch.Groups[iGroup].Captures) {
						switch (strGroup) {
							case "element":
								objElement = new HTMLElement(oCapture.Value);
								objElement.Raw = oMatch.Value;
								break;
							case "attr":
								objAttr.Add(oCapture.Value);
								break;
							case "attrv":
								if ((string)objAttr[intAttr] == "src")
								{
									//need to replace &amp; with &  (webresource.axd for IE6)
									objElement.Attributes.Add(objAttr[intAttr], System.Web.HttpUtility.HtmlDecode(oCapture.Value));
								}
								else
								{
									objElement.Attributes.Add(objAttr[intAttr], oCapture.Value);
								}

								intAttr += 1;
								break;
							case "text":
								objElement.Text = oCapture.Value;
								break;
						}

					}
				}
				if ((objElement != null))
				{
					oCol.Add(objElement);
				}
			}
			return oCol;
		}

         private string GetFormHTML()
         {
             System.Text.StringBuilder objSB = new System.Text.StringBuilder();
             System.IO.StringWriter objSW = new System.IO.StringWriter(objSB);
             HtmlTextWriter objHW = new HtmlTextWriter(objSW);
             string sFormHTML = "";

             //todo:  regexp to match scripts, regexp to match __dnnvariable
             Control oCtl = FindForm(this);
             if ((oCtl != null))
             {
                 oCtl.Controls.Clear();
                 oCtl.RenderControl(objHW);
                 sFormHTML = objSB.ToString();
             }
             return sFormHTML;
         }

		//TODO: make public in capi!
		private static System.Web.UI.HtmlControls.HtmlForm FindForm(Control oCtl)
		{
			while (!(oCtl is System.Web.UI.HtmlControls.HtmlForm)) {
				if (oCtl == null || oCtl is Page) return null; 
				oCtl = oCtl.Parent;
			}
			return (System.Web.UI.HtmlControls.HtmlForm)oCtl;
		}

	}
}
