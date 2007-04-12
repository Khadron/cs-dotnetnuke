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

using System.ComponentModel;
using System.Web.UI;
using DotNetNuke.UI.WebControls;
using System.Collections;

namespace DotNetNuke.UI.Design.WebControls
{
	public class DNNToolBarButtonActionTypeConverter : TypeConverter
	{
		//--- This class provides enumerators for the SolpartMenu.MenuEffects.ShadowDir property ---'
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList objCol = new ArrayList();
			foreach (string strAction in ((DNNToolBarButton)context.Instance).CommonActions) {
				objCol.Add(strAction);
			}
			if (SupportsActions(context))
			{
				IDNNToolBarSupportedActions oInstance = (IDNNToolBarSupportedActions)AttachedControl(context);
				foreach (string strAction in oInstance.Actions) {
					objCol.Add(strAction);
				}
			}
			return new TypeConverter.StandardValuesCollection(objCol);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return SupportsActions(context);
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		private Control m_objAttached;
		private Control AttachedControl(ITypeDescriptorContext context)
		{
			if (m_objAttached == null)
			{
				DNNToolBar objTB = (DNNToolBar)((DNNToolBarButton)context.Instance).Parent;
				m_objAttached = objTB.AttachedControl;
			}
			return m_objAttached;
		}

		private bool SupportsActions(ITypeDescriptorContext context)
		{
			return (AttachedControl(context) != null) && AttachedControl(context) is IDNNToolBarSupportedActions;
		}
	}
}
