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

namespace DotNetNuke.UI.WebControls
{
	public class DNNTabLabel : IStateManager
	{

		#region "Member Variables"
		private bool m_bMarked = false;
		private StateBag m_oState;
		#endregion

		#region "Properties"

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Text to display on the Tab
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// NotifyParentPropertyAttribute - (with the constructor argument equal to true) applied to the Text property causes the editor to serialize changes in these properties into their parent property, an instance of the Tab class.
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		[NotifyParentProperty(true), Localizable(true), DefaultValue(""), Category("Appearance")]
		public virtual string Text {
			get { return (string)ViewState["Text"]; }
			set { ViewState["Text"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Css class name to use for a selected tab
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		public string CssClassSelected {
			get { return (string)ViewState["CssClassSelected"]; }
			set { ViewState["CssClassSelected"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Css class name to use for a hovered tab
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		public string CssClassHover {
			get { return (string)ViewState["CssClassHover"]; }
			set { ViewState["CssClassHover"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Css class name to use for a tab
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		public string CssClass {
			get { return (string)ViewState["CssClass"]; }
			set { ViewState["CssClass"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Css class name to use for a tab
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		public string CssClassDisabled {
			get { return (string)ViewState["CssClassDisabled"]; }
			set { ViewState["CssClassDisabled"] = value; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Url of image to display next to text of tab
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	8/7/2006	Commented
		/// </history>
		/// -----------------------------------------------------------------------------
		public string ImageUrl {
			get { return (string)ViewState["ImageUrl"]; }
			set { ViewState["ImageUrl"] = value; }
		}
		#endregion

		#region "State Management"
		public StateBag ViewState {
			get {
				if (m_oState == null)
				{
					m_oState = new StateBag(true);
					if (IsTrackingViewState)
					{
						((IStateManager)m_oState).TrackViewState();
					}
				}
				return m_oState;
			}
		}

		public bool IsTrackingViewState {
			get { return m_bMarked; }
		}

		public void LoadViewState(object state)
		{
			if ((state != null))
			{
				((IStateManager)ViewState).LoadViewState(state);
			}
		}

		public object SaveViewState()
		{
			object oState = null;
			if ((m_oState != null))
			{
				oState = ((IStateManager)m_oState).SaveViewState();
			}
			return oState;
		}

		public void TrackViewState()
		{
			m_bMarked = true;
		}
		#endregion

	}
}
