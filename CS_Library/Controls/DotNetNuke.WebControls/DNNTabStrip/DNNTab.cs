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
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.UI.WebControls
{

	[ParseChildren(false), PersistChildren(true), TypeConverter(typeof(ExpandableObjectConverter))]
	public class DNNTab : WebControl, INamingContainer
	{

		public enum eTabCallbackPostMode
		{
			None,
			DNNVariable,
			TabStrip,
			Form
		}


		#region "Public Events"
		public event SetupDefaultsEventHandler SetupDefaults;
		public delegate void SetupDefaultsEventHandler();
		public event PreLoadPostDataEventHandler PreLoadPostData;
		public delegate void PreLoadPostDataEventHandler();
		#endregion

		#region "Member Variables"

		private DNNTabStrip m_oParentTabStrip;
		private DNNTabLabel m_oDNNTabLabel = new DNNTabLabel();
		private bool m_blnIsPostBack;
		private bool m_blnIsCallBack;
		#endregion

		#region "Constructors"
		public DNNTab() : base(HtmlTextWriterTag.Div)
		{
		}

		public DNNTab(string LabelText) : this()
		{
			this.Label.Text = LabelText;
		}
		#endregion

		#region "Properties"

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// This property allows the developer to determine whether or not the controls
		/// contained within the tab are part of the postback.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// The controls within a tab that is rendered on demand will need to use this
		/// property instead of the Page's IsPostback method.
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsPostBack {
			get { return m_blnIsPostBack; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// This property allows the developer to determine whether or not the controls
		/// contained within the tab are part of the callback.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// The controls within a tab that is rendered on demand will possibly use this
		/// property for defaulting data.
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsCallBack {
			get { return m_blnIsCallBack; }
		}


		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Access to the control containing this tab is necessary in order to pull
		/// properties pertaining to the tab.  For example, css.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected DNNTabStrip ParentControl {
			get {
				if (m_oParentTabStrip == null && this.Parent is DNNTabStrip)
				{
					m_oParentTabStrip = (DNNTabStrip)this.Parent;
				}
				return m_oParentTabStrip;
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Allows developer to determine if this tab is currently selected or not
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool IsSelected {
			get {
				if (ParentControl == null)
				{
					return true;
				}
else if ((ParentControl.SelectedTab != null) && this.ID == ParentControl.SelectedTab.ID && (this.ID != null)) {
					return true;
				}
				return false;
			}
		}

		[Category("Style"), Description("The style to be applied to Label"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public DNNTabLabel Label {
			//HtmlControls.HtmlGenericControl
			get { return m_oDNNTabLabel; }
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
		[DefaultValue(DNNTabStrip.eTabRenderMode.CallBack), Category("Behavior")]
		public DNNTabStrip.eTabRenderMode TabRenderMode {
			get {
				if (ViewState["TabRenderMode"] == null)
				{
					if (this.ParentControl == null)
					{
						return DNNTabStrip.eTabRenderMode.CallBack;
					}
					else
					{
						return this.ParentControl.TabRenderMode;
					}
				}
				else
				{
					return ((DNNTabStrip.eTabRenderMode)ViewState["TabRenderMode"]);
				}
			}
			set { ViewState["TabRenderMode"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// If tab is rendered through callback, this property determines what data is available
		/// to the callback handler
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// None - Only the minimal information to perform a callback is passed.   
		/// DNNVariable - Only the variables set through the ClientAPI are posted
		/// Tab - All form data from the rendered tab's conrols is posted
		/// Form - The entire form's data is posted
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/23/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[DefaultValue(eTabCallbackPostMode.None), Category("Behavior")]
		public eTabCallbackPostMode TabCallbackPostMode {
			get {
				if (ViewState["TabCallbackPostMode"] == null)
				{
					return eTabCallbackPostMode.None;
				}
				else
				{
					return ((eTabCallbackPostMode)ViewState["TabCallbackPostMode"]);
				}
			}
			set { ViewState["TabCallbackPostMode"] = value; }
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
		[Bindable(true), DefaultValue(ClientAPICallBackResponse.CallBackTypeCode.Simple), Description("Image to display during a callback")]
		public ClientAPICallBackResponse.CallBackTypeCode CallBackType {
			get {
				if (String.IsNullOrEmpty(ViewState["CallBackTypeCode"].ToString()))
				{
					return (ClientAPICallBackResponse.CallBackTypeCode)ViewState["CallBackTypeCode"];
				}
else if ((ParentControl != null)) {
					return ParentControl.CallBackType;
				}
				else
				{
					return ClientAPICallBackResponse.CallBackTypeCode.Simple;
				}
			}
			set { ViewState["CallBackTypeCode"] = value; }
		}

		#endregion

		#region "Overrides"

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Adds HTML attributes and styles that need to be rendered to the specified System.Web.UI.HtmlTextWriter. 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// The attributes handled by this method include:  display, label id, tabid, and class (css)
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
		{
			if (this.IsSelected == false)
			{
				this.Style.Add("display", "none");
			}
			base.AddAttributesToRender(writer);
			writer.AddAttribute("label", this.ClientID + "_l");
			writer.AddAttribute("tid", this.ID);

			if (!String.IsNullOrEmpty(this.ParentControl.DefaultContainerCssClass) && String.IsNullOrEmpty(this.CssClass))
			{
				writer.AddAttribute("class", this.ParentControl.DefaultContainerCssClass);
			}

		}

		//Necessary???
		//Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
		//	MyBase.RenderContents(writer)
		//End Sub
		#endregion

		#region "Internal (Friend) Methods"
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Allows tab to have its postback property set
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// Assigned based off of posted data during tabstrip load
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		internal void SetTabState(bool IsPostBack)
		{
			m_blnIsPostBack = IsPostBack;
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Allows tab to be assigned a parent.  
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// Needed for callback scenerios where the tab is passed as a parameter
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		internal void SetParent(DNNTabStrip Parent)
		{
			m_oParentTabStrip = Parent;
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Need a way to raise the event 
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// Couldn't think of a better name
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		internal void RaiseSetupDefaultsEvent(bool IsCallBack)
		{
			m_blnIsCallBack = IsCallBack;
			if (SetupDefaults != null) {
				SetupDefaults();
			}
		}

		internal void RaisePreLoadPostData()
		{
			if (PreLoadPostData != null) {
				PreLoadPostData();
			}
		}


		/// -----------------------------------------------------------------------------
		/// <summary>
		/// There are 2 parts to render for each tab.  The Label and the panel with the contents.
		/// The contents are rendered through the normal rendering methods, however, an explicit call
		/// is made to render the label since we may render the label but not the contents
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		internal void RenderLabel(HtmlTextWriter writer)
		{
			bool blnHasParent = (this.ParentControl != null);

			Label oLabel = new Label();
			oLabel.Controls.Clear();
			oLabel.ID = this.ClientID + "_l";
			//m_oLabel.ApplyStyle(Me.LabelStyle)
			//Me.LabelStyle.AddAttributesToRender(writer, m_oLabel)

			string sCss;
			if (this.IsSelected)
			{
				sCss = this.Label.CssClassSelected;
				if (String.IsNullOrEmpty(sCss) && blnHasParent) sCss = this.ParentControl.DefaultLabel.CssClassSelected; 
				//if not set default to strip's selected class
			}
else if (this.Enabled == false) {
				sCss = this.Label.CssClassDisabled;
			}
			else
			{
				sCss = this.Label.CssClass;
			}
			if (blnHasParent)
			{
				if (this.Enabled)
				{
					if (String.IsNullOrEmpty(sCss)) sCss = this.ParentControl.CssClass; 
					//if not set default to tabs normal class
					if (String.IsNullOrEmpty(sCss)) sCss = this.ParentControl.DefaultLabel.CssClass; 
					//if not set default to strip's default label class
				}
				else
				{
					if (String.IsNullOrEmpty(sCss)) sCss = this.ParentControl.DefaultLabel.CssClassDisabled; 
					//if not set default to strip's default label class
				}
			}
			if (!String.IsNullOrEmpty(sCss)) oLabel.CssClass = sCss; 

			oLabel.Attributes.Add("tid", this.ID);
			//Me.Label.WriteAttributes(oLabel)
			if (!String.IsNullOrEmpty(this.Label.CssClass)) oLabel.Attributes.Add("css", this.Label.CssClass); 
			if (!String.IsNullOrEmpty(this.Label.CssClassSelected)) oLabel.Attributes.Add("csssel", this.Label.CssClassSelected); 
			if (!String.IsNullOrEmpty(this.Label.CssClassHover)) oLabel.Attributes.Add("csshover", this.Label.CssClassHover); 
			if (!String.IsNullOrEmpty(this.Label.CssClassDisabled)) oLabel.Attributes.Add("cssdisabled", this.Label.CssClassDisabled); 

			if (this.Enabled == false) oLabel.Attributes.Add("enabled", "0"); 
			if (this.Visible == false) oLabel.Style.Add("display", "none");

               if (WebControls.IsDesignMode())
			{
				//messes up design time view when locating form, so bypass in designer
				if (this.ParentControl.IsDownLevel == false)
				{
					if (this.TabRenderMode == DNNTabStrip.eTabRenderMode.CallBack)
					{
						switch (this.TabCallbackPostMode) {
							case eTabCallbackPostMode.DNNVariable:
								oLabel.Attributes.Add("postmode", DotNetNuke.UI.Utilities.ClientAPI.DNNVARIABLE_CONTROLID);
								break;
							case eTabCallbackPostMode.Form:
								Control oCtl = this.ParentControl;
								while (!(oCtl is System.Web.UI.HtmlControls.HtmlForm)) {
									oCtl = oCtl.Parent;
									if (oCtl == null)
									{
										break; // TODO: might not be correct. Was : Exit While
									}
								}

								if ((oCtl != null))
								{
									oLabel.Attributes.Add("postmode", oCtl.ClientID);
								}
								else
								{
									throw new Exception("Could not find form control");
								}

								break;

							case eTabCallbackPostMode.TabStrip:
								oLabel.Attributes.Add("postmode", this.ParentControl.ClientID);
								break;
						}
					}

					if (this.CallBackType != ParentControl.CallBackType)
					{
						oLabel.Attributes.Add("cbtype", (this.CallBackType).ToString());
					}
				}
				else
				{
					if (this.ParentControl.IsDownLevel)
					{
						oLabel.Attributes.Add("onclick", ClientAPI.GetPostBackClientHyperlink(this.ParentControl, this.ID + ClientAPI.COLUMN_DELIMITER + "OnDemand"));
					}
				}
			}

			if (!String.IsNullOrEmpty(this.Label.ImageUrl) || (blnHasParent && (!String.IsNullOrEmpty(this.ParentControl.DefaultLabel.ImageUrl))))
			{
				Image oImg = new Image();
				oImg.ID = this.ClientID + "_i";
				oImg.ImageUrl = this.Label.ImageUrl;
				if (String.IsNullOrEmpty(oImg.ImageUrl)) oImg.ImageUrl = this.ParentControl.DefaultLabel.ImageUrl; 
				oLabel.Controls.Add(oImg);
			}
			if (blnHasParent && !String.IsNullOrEmpty(this.ParentControl.WorkImage))
			{
				Image oImg = new Image();
				oImg.ID = this.ClientID + "_w";
				oImg.ImageUrl = this.ParentControl.WorkImage;
				oImg.Style.Add("display", "none");
				oLabel.Controls.Add(oImg);
			}

			oLabel.Controls.Add(new LiteralControl(this.Label.Text));
			oLabel.RenderControl(writer);

		}

		#endregion

		#region "State Management"
		protected override void LoadViewState(object savedState)
		{
			if (savedState == null)
			{
				// Always invoke LoadViewState on the base class, even if there is no saved state, because the base class might have implemented some logic that needs to be executed even if there is no state to restore.
				base.LoadViewState(savedState);
				return;
			}
			else
			{
				object[] oState = (object[])savedState;
				if (oState.Length != 2)
				{
					throw new ArgumentException("Invalid View State");
				}
                    if ((oState[0] != null)) base.LoadViewState(oState[0]); 
				if ((oState[1] != null)) ((IStateManager)Label).LoadViewState(oState[1]); 
			}

		}

		protected override object SaveViewState()
		{
			object[] oState = new object[2];
			oState[0] = base.SaveViewState();
			if ((m_oDNNTabLabel != null))
			{
				oState[1] = ((IStateManager)Label).SaveViewState();
			}
			else
			{
				oState[1] = null;
			}

			for (int i = 0; i <= oState.Length - 1; i++) {
				if ((oState[i] != null))
				{
					return oState;
				}
			}

			return null;
			//If there is no saved state it is more performant to return nothing than to return an array of nothing values.
		}

		protected override void TrackViewState()
		{
			base.TrackViewState();

			if ((Label != null))
			{
				((IStateManager)Label).TrackViewState();

			}
		}
		#endregion

	}
}
