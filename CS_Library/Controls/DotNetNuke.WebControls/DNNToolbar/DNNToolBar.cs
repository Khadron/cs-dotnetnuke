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
using System.ComponentModel.Design;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DotNetNuke.UI.Utilities;
using System.Web.UI.HtmlControls;
using System.Collections;

namespace DotNetNuke.UI.WebControls
{
	[ProvideProperty("Toolbar", typeof(System.Web.UI.Control)), ParseChildren(true, "Buttons"), DefaultProperty("Buttons"), PersistChildren(false), Designer(typeof(DotNetNuke.UI.Design.WebControls.DNNToolBarDesigner))]
	public class DNNToolBar : Control, IExtenderProvider
	{
		//Implements IStateManager

		#region "Member Variables"
		private bool m_bMarked = false;
		private StateBag m_objState;
		private DNNToolBarButtonCollection m_objButtons;
		#endregion

		#region "Properties"

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Css class name to use for a tab
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	9/15/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Appearance")]
		public string CssClass {
			get { return (string)ViewState["CssClass"]; }
			set {
				ViewState["CssClass"] = value;
				this.NotifyDesigner();
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Css class name to use on all buttons
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	9/27/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Appearance")]
		public string DefaultButtonCssClass {
			get { return (string)ViewState["DefaultButtonCssClass"]; }
			set {
				ViewState["DefaultButtonCssClass"] = value;
				this.NotifyDesigner();
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Css class name to use on all buttons when hovered
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	9/27/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[Category("Appearance")]
		public string DefaultButtonHoverCssClass {
			get { return (string)ViewState["DefaultButtonHoverCssClass"]; }
			set {
				ViewState["DefaultButtonHoverCssClass"] = value;
				this.NotifyDesigner();
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Number of milliseconds to wait befor hiding sub-menu on mouse out
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	9/16/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[Description("Number of milliseconds to wait befor hiding sub-menu on mouse out"), Category("Behavior"), PersistenceMode(PersistenceMode.Attribute), DefaultValue(250)]
		public int MouseOutDelay {
			get {
				if (String.IsNullOrEmpty((string)ViewState["MouseOutDelay"]))
				{
					return 250;
				}
				else
				{
					return (int)ViewState["MouseOutDelay"];
				}
			}
			set {
				ViewState["MouseOutDelay"] = value;
				this.NotifyDesigner();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		public virtual DNNToolBarButtonCollection Buttons {
			get {
				if (m_objButtons == null)
				{
					m_objButtons = new DNNToolBarButtonCollection(this);
					//New Generic.List(Of Tab)
				}
				return (DNNToolBarButtonCollection)m_objButtons;
			}
		}

		[Description("Allows toolbar structure to be reused by others sharing the same id"), Category("Behavior"), PersistenceMode(PersistenceMode.Attribute), DefaultValue(false)]
		public bool ReuseToolBar {
			get {
				if (String.IsNullOrEmpty((string)ViewState["ReuseToolBar"]))
				{
					return false;
				}
				else
				{
					return (bool)ViewState["ReuseToolBar"];
				}
			}
			set {
				ViewState["ReuseToolBar"] = value;
				this.NotifyDesigner();
			}
		}

		[Browsable(false), Bindable(false)]
		public new bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}

		[Browsable(false)]
		internal Control AttachedControl {
			get { return FindAttachedControl(Page); }
		}

		#endregion

		#region "State Management"
		//		Protected ReadOnly Property ViewState() As StateBag
		//			Get
		//				If m_objState Is Nothing Then
		//					m_objState = New StateBag(True)
		//					If IsTrackingViewState Then
		//						CType(m_objState, IStateManager).TrackViewState()
		//					End If
		//				End If
		//				Return m_objState
		//			End Get
		//		End Property

		//		Protected ReadOnly Property IsTrackingViewState() As Boolean Implements System.Web.UI.IStateManager.IsTrackingViewState
		//			Get
		//				Return m_bMarked
		//			End Get
		//		End Property

		//		Protected Sub LoadViewState(ByVal state As Object) Implements System.Web.UI.IStateManager.LoadViewState
		//			If Not (state Is Nothing) Then
		//				CType(ViewState, IStateManager).LoadViewState(state)
		//			End If
		//		End Sub

		//		Protected Function SaveViewState() As Object Implements System.Web.UI.IStateManager.SaveViewState
		//			Dim oState As Object = Nothing
		//			If Not (m_objState Is Nothing) Then
		//				oState = CType(m_objState, IStateManager).SaveViewState()
		//			End If
		//			Return oState
		//		End Function

		//		Protected Sub TrackViewState() Implements System.Web.UI.IStateManager.TrackViewState
		//			m_bMarked = True
		//		End Sub
		#endregion

		#region "Old Extender Implementation"
		//		Private m_objTBC As DNNToolBarComponent
		//		Private m_strTarget As String

		//		<Browsable(False)> _
		//		Friend ReadOnly Property HostingControl() As DNNToolBarComponent
		//			Get
		//				If m_objTBC Is Nothing Then
		//					m_objTBC = CType(Me.Parent, DNNToolBarComponent)
		//				End If
		//				Return m_objTBC
		//			End Get
		//			'Set(ByVal value As DNNToolbarComponent)
		//			'	m_objTBC = value
		//			'End Set
		//		End Property

		//		<Browsable(False)> _
		//		 Public Property Target() As String
		//			Get
		//				Return m_strTarget
		//			End Get
		//			Set(ByVal value As String)
		//				m_strTarget = value
		//			End Set
		//		End Property

		//		Friend Sub NotifyDesigner()
		//			If Not Me.HostingControl Is Nothing Then
		//				HostingControl.NotifyDesigner()
		//			End If
		//		End Sub

		//		'Friend Sub SetTarget(ByVal objControl As Control)
		//		'	Me.Target = objControl.ID
		//		'	AddHandler objControl.PreRender, AddressOf DNNToolBar_PreRender
		//		'End Sub

		#endregion

		#region "Extender Implementation"
		//for more info see http://www.codeproject.com/aspnet/PropertyExtendersASP20.asp

		public bool CanExtend(object extendee)
		{
			if (extendee is IDNNToolBar)
               {
					if (((IDNNToolBar)extendee).ToolBarId == this.ID)
					{
						return true;
					}
			}
			return false;
		}

		public void SetToolbar()
		{
			bool x = false;
		}

		public DNNToolBar GetToolbar(Control objControl)
		{
			IDNNToolBar objITB = (IDNNToolBar)objControl;
			Control objTB = null;
			if (!String.IsNullOrEmpty(objITB.ToolBarId))
			{
				objTB = objControl.Page.FindControl(objITB.ToolBarId);
			}
			return (DNNToolBar)objTB;
		}

		internal void NotifyDesigner()
		{
			if (WebControls.IsDesignMode() && (base.Site != null))
			{
				IDesignerHost dh = (IDesignerHost)base.Site.Container;
				ControlDesigner designer = (ControlDesigner)dh.GetDesigner(this);
				PropertyDescriptor pd = TypeDescriptor.GetProperties(this)["Toolbar"];
				ComponentChangedEventArgs objArgs = new ComponentChangedEventArgs(this, pd, null, this);
				designer.OnComponentChanged(this, objArgs);
			}
		}

		#endregion


		#region "Overrides"

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			//do not render
		}
		#endregion

		#region "Events"

          protected override void OnPreRender(EventArgs e)
          {
              base.OnPreRender(e);        
		    if (this.Visible)
		    {
			    RegisterToolbarScript();
		    }
		}

		#endregion

		#region "Methods"

		public bool RegisterToolBar(Control objAssociatedControl, string strShowEventName, string strHideEventName, string strToolBarActionHandler)
		{
			return RegisterToolBar(objAssociatedControl, strShowEventName, strHideEventName, strToolBarActionHandler, ClientAPI.ScriptPath);
		}

		public bool RegisterToolBar(Control objAssociatedControl, string strShowEventName, string strHideEventName, string strToolBarActionHandler, string ToolBarScriptPath)
		{
			if (ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.DHTML))
			{
				DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(this.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn_dom);

				if (!ClientAPI.IsClientScriptBlockRegistered(this.Page, "dnn.controls.dnntoolbarstub.js"))
				{
					ClientAPI.RegisterClientScriptBlock(this.Page, "dnn.controls.dnntoolbarstub.js", "<script src=\"" + ToolBarScriptPath + "dnn.controls.dnntoolbarstub.js\"></script>");
				}

				string strJS = string.Format("__dnn_toolbarHandler('{0}','{1}','{2}',{3},'{4}','{5}')", this.UniqueID, objAssociatedControl.ClientID, this.NamingContainer.UniqueID, strToolBarActionHandler, strShowEventName, strHideEventName);
				if (objAssociatedControl is WebControl)
				{
					((WebControl)objAssociatedControl).Attributes.Add(strShowEventName, strJS);
				}
else if (objAssociatedControl is HtmlControl) {
					((HtmlControl)objAssociatedControl).Attributes.Add(strShowEventName, strJS);
				}
				return true;
			}
			return false;
		}

		internal string ToJSON()
		{
			Hashtable oHash = new Hashtable();

			if (!String.IsNullOrEmpty(this.CssClass))
			{
				oHash.Add("css", "'" + this.CssClass + "'");
			}
			if (!String.IsNullOrEmpty(this.DefaultButtonCssClass))
			{
				oHash.Add("cssb", "'" + this.DefaultButtonCssClass + "'");
			}
			if (!String.IsNullOrEmpty(this.DefaultButtonHoverCssClass))
			{
				oHash.Add("cssbh", "'" + this.DefaultButtonHoverCssClass + "'");
			}
			if (this.MouseOutDelay != 250)
			{
				oHash.Add("mod", this.MouseOutDelay);
			}
			if (this.Visible != null)
			{
				oHash.Add("vis", this.Visible);
			}

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("[");
			foreach (DNNToolBarButton objBtn in this.Buttons) {
				if (sb.Length > 1) sb.Append(","); 
				sb.Append(objBtn.ToJSON());
			}
			sb.Append("]");
			oHash.Add("btns", sb.ToString());

			sb = new System.Text.StringBuilder();
			sb.Append("{");
			foreach (string strKey in oHash.Keys) {
				if (sb.Length > 1) sb.Append(","); 
				sb.Append(strKey + ":" + (string)oHash[strKey]);
			}
			sb.Append("}");

			return sb.ToString();
		}

		private void RegisterToolbarScript()
		{
			string strId = this.UniqueID;
			if (this.ReuseToolBar)
			{
				strId = this.ID;
			}
			if (!ClientAPI.IsClientScriptBlockRegistered(this.Page, strId + "_toolbar"))
			{
				if (ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.DHTML))
				{
					ClientAPI.RegisterClientReference(this.Page, ClientAPI.ClientNamespaceReferences.dnn);
					ClientAPI.RegisterStartUpScript(this.Page, strId + "_toolbar", string.Format("<script>dnn.controls.toolbars['{0}']={1};</script>", strId, this.ToJSON()));
				}
			}
		}

		private Control FindAttachedControl(Control objControl)
		{
			if (objControl is IDNNToolBar)
			{
				if (((IDNNToolBar)objControl).ToolBarId == this.ID)
				{
					return objControl;
				}
			}
			Control c;
			foreach (Control objCtl in objControl.Controls) {
				Control t = FindAttachedControl(objCtl);
				if ((t != null))
				{
					return t;
				}
			}
			return null;
		}

		#endregion

	}
}
