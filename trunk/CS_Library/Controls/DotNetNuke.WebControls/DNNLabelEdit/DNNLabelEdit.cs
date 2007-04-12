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
	public class DNNLabelEdit : Label, IPostBackEventHandler, DotNetNuke.UI.Utilities.IClientAPICallbackEventHandler, DotNetNuke.UI.WebControls.IDNNToolBar, DotNetNuke.UI.WebControls.IDNNToolBarSupportedActions
	{

		#region "Events / Delegates"
		public delegate void DNNLabelEditEventHandler(object source, DNNLabelEditEventArgs e);
		public event DNNLabelEditEventHandler UpdateLabel;
		#endregion

		#region "Member Variables"
		#endregion

		#region "Constructors"
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	10/10/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public DNNLabelEdit()
		{
		}
		#endregion

		#region "Properties"

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Returns whether the LabelEdit will save when contol looses focus
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/22/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Behavior"), DefaultValue(true)]
		public bool LostFocusSave {
			get {
				if (!String.IsNullOrEmpty(this.ToolBarId))
				{
					//if using toolbar do not cause save to occur on lostfocus... defeats purpose.
					return false;
				}
                    else if (ViewState["LostFocusSave"] == null)
{
					return true;
				}
				else
				{
					return (bool)ViewState["LostFocusSave"];
				}
			}
			set { ViewState["LostFocusSave"] = value; }
		}


		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Returns whether the LabelEdit will render DownLevel or not
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/22/2005	Commented
		///		[Jon Henning]	3/9/2005	Requiring XML support on client for uplevel
		/// </history>
		/// -----------------------------------------------------------------------------
		[Browsable(false), Category("Appearance")]
		public bool IsDownLevel {
			get {
				if (EditEnabled == false || DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.DHTML) == false || DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.XML) == false)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		[DefaultValue(true), PersistenceMode(PersistenceMode.Attribute), Category("Behavior")]
		public bool EditEnabled {
			get {
				if (ViewState["EditEnabled"] != null)
				{
					return (bool)ViewState["EditEnabled"];
				}
				else
				{
					return true;
				}
			}
			set { ViewState["EditEnabled"] = value; }
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
		[Category("Paths")]
		public string ClientAPIScriptPath {
			get { return DotNetNuke.UI.Utilities.ClientAPI.ScriptPath; }
// Commented out because it was breaking the build.. (SM 26 Oct)  
// uncommented out, don't understand how this could have broke build - JH 22 Feb 2005
			set { DotNetNuke.UI.Utilities.ClientAPI.ScriptPath = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Location of dnn.controls.DNNLabelEdit.js file
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// Since 1.1 this path will be the same as the ClientAPI path.
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	2/22/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Paths")]
		public string LabelEditScriptPath {
			get {
				if (ViewState["LabelEditScriptPath"] == null)
				{
					return ClientAPIScriptPath;
				}
				else
				{
					return ViewState["LabelEditScriptPath"].ToString();
				}
			}
			set { ViewState["LabelEditScriptPath"] = value; }
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
		[Description("Directory to find the images for the LabelEdit.  Need to have spacer.gif here!"), DefaultValue("images/"), Category("Paths")]
		public string SystemImagesPath {
			get 
               {
                   if (ViewState["SystemImagesPath"] == null)
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

		[Bindable(true), DefaultValue("onclick"), PersistenceMode(PersistenceMode.Attribute), Category("Behavior"), Description("Client-side event that will trigger an edit.  (onclick, ondblclick, none)")]
		public string EventName {
			get {
				if (ViewState["EventName"] == null)
				{
					return "onclick";
				}
				else
				{
					return ViewState["EventName"].ToString();
				}
			}
			set { ViewState["EventName"] = value; }
		}


		[DefaultValue(false), PersistenceMode(PersistenceMode.Attribute), Category("Behavior")]
		public bool MultiLine {
			get {
				if (ViewState["MultiLine"] == null)
				{
					return false;
				}
				else
				{
					return (bool)ViewState["MultiLine"];
				}
			}
			set { ViewState["MultiLine"] = value; }
		}

		[DefaultValue(false), PersistenceMode(PersistenceMode.Attribute), Category("Behavior")]
		public bool RichTextEnabled {
			get {
				if (ViewState["RichTextEnabled"] == null)
				{
					return false;
				}
				else
				{
					return (bool)ViewState["RichTextEnabled"];
				}
			}
			set { ViewState["RichTextEnabled"] = value; }
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
		[Bindable(true), PersistenceMode(PersistenceMode.Attribute), Category("Behavior")]
		public string CallbackStatusFunction {
			get { return ((string)ViewState["CallbackStatusFunction"]); }
			set { ViewState["CallbackStatusFunction"] = value; }
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

		[Category("Behavior")]
		public string ToolBarId {
			get 
               {
                   if (ViewState["ToolBarId"] == null)
                   {
                       return String.Empty;
                   }
                   else
                   {
                       return ViewState["ToolBarId"].ToString();
                   }
               }
			set 
               {
				ViewState["ToolBarId"] = value;
				TypeDescriptor.Refresh(this);
				//make sure we refresh the property grid
			}
		}

		[Browsable(false)]
		internal string[] ToolBarSupportedActions {
              get
              {
                  return this.Actions; 
              }                    
		}
          [Browsable(false)]
          public string[] Actions
          {
              get { return new string[] { "edit", "save", "cancel", "bold", "italic", "underline", "justifyleft", "justifycenter", "justifyright", "insertorderedlist", "insertunorderedlist", "outdent", "indent", "createlink" }; }
          }

		[DefaultValue("onmousemove"), Category("Behavior"), Description("Allows the client-side event that displays the toolbar to be configured (onmouseover, onclick)")]
		public string ShowToolBarEventName {
			get {
				if (ViewState["ShowToolBarEventName"] == null || String.IsNullOrEmpty(ViewState["ShowToolBarEventName"].ToString()))
				{
					return "onmousemove";
				}
				else
				{
					return ViewState["ShowToolBarEventName"].ToString();
				}
			}
			set { ViewState["ShowToolBarEventName"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// This javascript function will be invoked just before persisting the data
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
		public string BeforeSaveFunction {
			get 
               {
                   if (ViewState["BeforeSaveFunction"] == null)
                   {
                       return String.Empty;
                   }
                   else
                   {
                       return ViewState["BeforeSaveFunction"].ToString();
                   }                  
               }
			set { ViewState["BeforeSaveFunction"] = value; }
		}

		[Bindable(true), DefaultValue(""), PersistenceMode(PersistenceMode.Attribute), Category("Appearance")]
		public string LabelEditCssClass {
			get 
               {
                   if (ViewState["LabelEditCssClass"] == null)
                   {
                       return String.Empty;
                   }
                   else
                   {
                       return ViewState["LabelEditCssClass"].ToString();
                   }
               }
			set { ViewState["LabelEditCssClass"] = value; }
		}

		[Bindable(true), DefaultValue(""), PersistenceMode(PersistenceMode.Attribute), Category("Appearance")]
		public string WorkCssClass {
			get 
               {
                   if (ViewState["WorkCssClass"] == null)
                   {
                       return String.Empty;
                   }
                   else
                   {
                       return ViewState["WorkCssClass"].ToString();
                   }
               }
			set { ViewState["WorkCssClass"] = value; }
		}

		[DefaultValue(""), PersistenceMode(PersistenceMode.Attribute), Category("Appearance")]
		public string MouseOverCssClass {
			get 
               {
                   if (ViewState["MouseOverCssClass"] == null)
                   {
                       return String.Empty;
                   }
                   else
                   {
                       return ViewState["MouseOverCssClass"].ToString();
                   } 
               }
			set { ViewState["MouseOverCssClass"] = value; }
		}

		[DefaultValue(false), Category("Misc")]
		public bool RenderAsDiv {
			get 
               {
                   if (ViewState["RenderAsDiv"] == null)
                   {
                       return false;
                   }
                   else
                   {
                       return (bool)(ViewState["RenderAsDiv"]);
                   }
               }
			set { ViewState["RenderAsDiv"] = value; }
		}

		protected override string TagName {
			get {
				if (this.RenderAsDiv)
				{
					return "div";
				}
				else
				{
					return "span";
				}
			}
		}

		protected override HtmlTextWriterTag TagKey {
			get {
				if (this.RenderAsDiv)
				{
					return HtmlTextWriterTag.Div;
				}
				else
				{
					return HtmlTextWriterTag.Span;
				}
			}
		}

		#endregion

		#region "OverRidden Methods"

		protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);
			//writer.AddAttribute(HtmlTextWriterAttribute.Class, Me.CssClass)
			if (this.EditEnabled)
			{
				writer.AddAttribute("sysimgpath", this.SystemImagesPath);
				if (!String.IsNullOrEmpty(this.LabelEditCssClass))
				{
					writer.AddAttribute("cssEdit", this.LabelEditCssClass);
				}
				if (!String.IsNullOrEmpty(this.WorkCssClass))
				{
					writer.AddAttribute("cssWork", this.WorkCssClass);
				}

				if (!String.IsNullOrEmpty(this.MouseOverCssClass))
				{
					writer.AddAttribute("cssOver", this.MouseOverCssClass);
				}

				if (this.EventName != "onclick")
				{
					writer.AddAttribute("eventName", this.EventName);
				}

				if (this.MultiLine)
				{
					writer.AddAttribute("multiline", "1");
				}

				if (this.RichTextEnabled)
				{
					writer.AddAttribute("richtext", "1");
				}

				if (DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.XMLHTTP))
				{
					writer.AddAttribute("callback", DotNetNuke.UI.Utilities.ClientAPI.GetCallbackEventReference(this, "'[TEXT]'", "this.callBackSuccess", "this", "this.callBackFail", "this.callBackStatus", CallBackType));
				}

				if (!String.IsNullOrEmpty(this.CallbackStatusFunction))
				{
					writer.AddAttribute("callbackSF", this.CallbackStatusFunction);
				}

				if (!String.IsNullOrEmpty(this.BeforeSaveFunction))
				{
					writer.AddAttribute("beforeSaveF", BeforeSaveFunction);
				}

				if (!String.IsNullOrEmpty(this.ToolBarId))
				{
					writer.AddAttribute("tbId", ToolBarId);
					if (!String.IsNullOrEmpty(this.UniqueID.Replace(this.ID, "")))
					{
						//writer.AddAttribute("nsPrefix", Me.UniqueID.Replace(Me.ID, ""))
						writer.AddAttribute("nsPrefix", this.NamingContainer.UniqueID);
						//TEST ???
					}
				}
				if (this.ShowToolBarEventName != "onmousemove")
				{
					writer.AddAttribute("tbEvent", this.ShowToolBarEventName);
				}

				if (this.LostFocusSave == false)
				{
					writer.AddAttribute("blursave", "0");
				}
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	10/10/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected override object SaveViewState()
		{
			object _baseState = base.SaveViewState();
			object[] objState = new object[1];
			objState[0] = _baseState;
			return objState;
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	10/10/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected override void LoadViewState(object state)
		{
			if ((state != null))
			{
				object[] objState = (object[])state;
				if ((objState[0] != null))
				{
					base.LoadViewState(objState[0]);
				}
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	10/10/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected override void TrackViewState()
		{
			base.TrackViewState();
			//CType(Root, IStateManager).TrackViewState()
		}
		#endregion

		#region "Event Handlers"

		public virtual void RaisePostBackEvent(string eventArgument)
		{
			if (UpdateLabel != null) {
				UpdateLabel(this, new DNNLabelEditEventArgs(eventArgument));
			}
		}

		public string RaiseClientAPICallbackEvent(string eventArgument)
		{
			if (UpdateLabel != null) {
				UpdateLabel(this, new DNNLabelEditEventArgs(eventArgument));
			}
			return eventArgument;
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
			if (IsDownLevel == false && this.EditEnabled)
			{
				DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(this.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn_dom);
				DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(this.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn_xml);
				DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(this.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn_dom_positioning);
				DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(this.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn_xmlhttp);

				if (!ClientAPI.IsClientScriptBlockRegistered(this.Page, "dnn.controls.dnnlabeledit.js"))
				{
					ClientAPI.RegisterClientScriptBlock(this.Page, "dnn.controls.dnnlabeledit.js", "<script src=\"" + LabelEditScriptPath + "dnn.controls.dnnlabeledit.js\"></script>");
				}
				//ClientAPI.RegisterStartUpScript(Page, Me.ClientID & "_startup", "<script>dnn.controls.initLabelEdit(dnn.dom.getById('" & Me.ClientID & "'));</script>")				   'wrong place
			}
		}

		#endregion

          protected override void OnPreRender(EventArgs e)
          {
             base.OnPreRender(e);
             RegisterClientScript();             
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			base.Render(writer);
			if (IsDownLevel == false && this.EditEnabled)
			{
				ClientAPI.RegisterStartUpScript(Page, this.ClientID + "_startup", "<script>dnn.controls.initLabelEdit(dnn.dom.getById('" + this.ClientID + "'));</script>");
			}
		}

		//Public Overrides Sub RenderBeginTag(ByVal writer As System.Web.UI.HtmlTextWriter)
		//	MyBase.RenderBeginTag(writer)
		//	ClientAPI.RegisterStartUpScript(Page, Me.ClientID & "_startup", "<script>dnn.controls.initLabelEdit(dnn.dom.getById('" & Me.ClientID & "'));</script>")

		//End Sub
      }
}
