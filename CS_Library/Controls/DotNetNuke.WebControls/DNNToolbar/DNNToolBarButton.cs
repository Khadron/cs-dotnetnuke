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
using System.ComponentModel;
using System.Collections;

namespace DotNetNuke.UI.WebControls
{

	[ParseChildren(false), PersistChildren(true)]
	public class DNNToolBarButton : Control, ICustomTypeDescriptor
	{
		//		Implements IStateManager
		private bool m_blnMarked = false;
		private StateBag m_objState;
		private DNNToolBar m_objToolbar;

		internal string[] CommonActions = {"js", "navigate"};

		#region "Constructors"
		public DNNToolBarButton()
		{
		}
		#endregion

		#region "Properties"

		[Browsable(false)]
		private DNNToolBar Owner {
			get {
				if (m_objToolbar == null)
				{
					m_objToolbar = (DNNToolBar)this.Parent;
				}
				return m_objToolbar;
			}
		}

		//<DefaultValue("")> _
		public string Key {
			get {
				//if it is a common action then allow custom key, otherwise use action as key
				if (Array.IndexOf(CommonActions, this.ControlAction) > -1)
				{
					if (MyState("Key") == null)
					{
						return "";
					}
					else
					{
						return ((string)MyState("Key"));
					}
				}
				else
				{
					return this.ControlAction;
				}
			}
			set { MyState("Key", value); }
		}

		[DefaultValue(""), Category("Appearance")]
		public string CssClass {
			get {
				if (MyState("CssClass") == null)
				{
					return "";
				}
				else
				{
					return ((string)MyState("CssClass"));
				}
			}
			set { MyState("CssClass", value); }
		}

		[DefaultValue(""), Category("Appearance")]
		public string CssClassHover {
			get {
				if (MyState("CssClassHover") == null)
				{
					return "";
				}
				else
				{
					return ((string)MyState("CssClassHover"));
				}
			}
			set { MyState("CssClassHover", value); }
		}

		[DefaultValue(""), Category("Behavior"), TypeConverter(typeof(Design.WebControls.DNNToolBarButtonActionTypeConverter))]
		public string ControlAction {
			get {
				if (MyState("ControlAction") == null)
				{
					return "";
				}
				else
				{
					return ((string)MyState("ControlAction"));
				}
			}
			set {
				MyState("ControlAction", value);
				TypeDescriptor.Refresh(this);
				//make sure we refresh the property grid
			}
		}

		[DefaultValue(""), Category("Appearance")]
		public string Text {
			get {
				if (MyState("Text") == null)
				{
					return "";
				}
				else
				{
					return ((string)MyState("Text"));
				}
			}
			set { MyState("Text", value); }
		}

		[DefaultValue(""), Category("Appearance")]
		public string ToolTip {
			get {
				if (MyState("ToolTip") == null)
				{
					return "";
				}
				else
				{
					return ((string)MyState("ToolTip"));
				}
			}
			set { MyState("ToolTip", value); }
		}

		[DefaultValue(""), Category("Appearance")]
		public string ImageUrl {
			get {
				if (MyState("ImageUrl") == null)
				{
					return "";
				}
				else
				{
					return ((string)MyState("ImageUrl"));
				}
			}
			set { MyState("ImageUrl", value); }
		}

		[DefaultValue(""), Category("Behavior")]
		public string NavigateUrl {
			get {
				if (MyState("NavigateUrl") == null)
				{
					return "";
				}
				else
				{
					return ((string)MyState("NavigateUrl"));
				}
			}
			set { MyState("NavigateUrl", value); }
		}

		[DefaultValue(""), Category("Behavior")]
		public string JSFunction {
			get {
				if (MyState("JSFunction") == null)
				{
					return "";
				}
				else
				{
					return ((string)MyState("JSFunction"));
				}
			}
			set { MyState("JSFunction", value); }
		}

		private object MyState(string key) 
          {
	        return ViewState[key]; 
          }
			
          private void MyState(string key, object value)
          {
             ViewState[key] = value;
             NotifyDesigner();
          }

		#endregion

		#region "Public Methods"
		public string ToJSON()
		{
			Hashtable oHash = new Hashtable();
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if (!String.IsNullOrEmpty(this.CssClass))
			{
				oHash.Add("css", SafeJSONString(this.CssClass));
			}
			if (!String.IsNullOrEmpty(this.CssClassHover))
			{
				oHash.Add("cssh", SafeJSONString(this.CssClassHover));
			}
			if (!String.IsNullOrEmpty(this.ImageUrl))
			{
				oHash.Add("img", SafeJSONString(this.ImageUrl));
			}

			if (!String.IsNullOrEmpty(this.Key))
			{
				oHash.Add("key", SafeJSONString(this.Key));
			}
			if (!String.IsNullOrEmpty(this.JSFunction) && this.ControlAction == "js")
			{
				oHash.Add("js", SafeJSONString(this.JSFunction));
			}
			if (!String.IsNullOrEmpty(this.NavigateUrl) && this.ControlAction == "navigate")
			{
				oHash.Add("url", SafeJSONString(this.NavigateUrl));
			}
			if (!String.IsNullOrEmpty(this.ControlAction))
			{
				oHash.Add("ca", SafeJSONString(this.ControlAction));
			}
			if (!String.IsNullOrEmpty(this.Text))
			{
				oHash.Add("txt", SafeJSONString(this.Text));
			}
			if (!String.IsNullOrEmpty(this.ToolTip))
			{
				oHash.Add("alt", SafeJSONString(this.ToolTip));
			}
			if (this.Visible == false)
			{
				oHash.Add("vis", this.Visible);
			}
			sb.Append("{");
			foreach (string strKey in oHash.Keys) {
				if (sb.Length > 1) sb.Append(","); 
				sb.Append(strKey + ":" + (string)oHash[strKey]);
			}
			sb.Append("}");
			return sb.ToString();
		}

		private string SafeJSONString(string strString)
		{
			return "'" + strString.Replace("'", "\\'") + "'";
		}
		#endregion

		#region "Private Methods"
		private void NotifyDesigner()
		{
			if ((Owner != null))
			{
				Owner.NotifyDesigner();
			}
		}
		#endregion

		#region "Overrides"
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			//No output to render
		}
		#endregion

		#region "State Management"
		//		Public ReadOnly Property IsTrackingViewState() As Boolean Implements IStateManager.IsTrackingViewState
		//			Get
		//				Return m_blnMarked
		//			End Get
		//		End Property

		//		Public Sub TrackViewState() Implements IStateManager.TrackViewState
		//			m_blnMarked = True
		//		End Sub

		//		Public Function SaveViewState() As Object Implements IStateManager.SaveViewState
		//			' save m_objState state
		//			Dim objState As Object = Nothing
		//			If Not (m_objState Is Nothing) Then
		//				objState = CType(m_objState, IStateManager).SaveViewState()
		//			End If
		//			Return objState
		//		End Function

		//		Public Sub LoadViewState(ByVal state As Object) Implements IStateManager.LoadViewState
		//			If Not (state Is Nothing) Then
		//				CType(ViewState, IStateManager).LoadViewState(state)
		//			End If
		//		End Sub

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

		//		Friend Sub SetDirty()
		//			If Not m_objState Is Nothing Then
		//				Dim key As String
		//				For Each key In m_objState.Keys
		//					m_objState.SetItemDirty(key, True)
		//				Next
		//			End If
		//		End Sub
		#endregion

		#region "ICustomTypeDescriptor Implementation"

		public System.ComponentModel.AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this.GetType());
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this.GetType());
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this.GetType());
		}

		public System.ComponentModel.TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this.GetType());
		}

		public System.ComponentModel.EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this.GetType());
		}

		public System.ComponentModel.PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this.GetType());
		}

		public object GetEditor(System.Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this.GetType(), editorBaseType);
		}

		public System.ComponentModel.EventDescriptorCollection GetEvents(System.Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this.GetType(), attributes);
		}

		public System.ComponentModel.EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this.GetType());
		}

		public System.ComponentModel.PropertyDescriptorCollection GetProperties(System.Attribute[] attributes)
		{
			PropertyDescriptorCollection objNewCol = new PropertyDescriptorCollection(null);
			PropertyDescriptorCollection objExistingCol;

			objExistingCol = TypeDescriptor.GetProperties(this.GetType(), attributes);
			foreach (PropertyDescriptor pd in objExistingCol) {
				switch (pd.Name) {
					case "NavigateUrl":
						if (this.ControlAction == "navigate")
						{
							pd.GetValue(this);
							objNewCol.Add(pd);
						}

						break;
					case "JSFunction":
						if (this.ControlAction == "js")
						{
							pd.GetValue(this);
							objNewCol.Add(pd);
						}

						break;
					case "Key":
						if (Array.IndexOf(CommonActions, this.ControlAction) > -1)
						{
							pd.GetValue(this);
							objNewCol.Add(pd);
						}

						break;
					default:
						pd.GetValue(this);
						objNewCol.Add(pd);
						break;
				}
			}
			return objNewCol;

		}

		private void HandlePropertyFilters(PropertyDescriptor pd, PropertyDescriptorCollection x)
		{
			pd.GetValue(this);
			x.Add(pd);
		}

		public System.ComponentModel.PropertyDescriptorCollection GetProperties()
		{
			return TypeDescriptor.GetProperties(this.GetType());
		}

		public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

	}
}
