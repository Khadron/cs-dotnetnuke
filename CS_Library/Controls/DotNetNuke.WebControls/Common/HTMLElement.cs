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
using System.Xml;
using System.Collections;

namespace DotNetNuke.UI.WebControls
{
	public class HTMLElement
	{

		private string m_strRaw;
		private string m_strText;
		private string m_strTagName;
		private Hashtable m_objAttributes = new Hashtable();

		#region "Constructors"
		public HTMLElement(string TagName)
		{
			m_strTagName = TagName;
		}
		#endregion

		public string Raw {
			get { return m_strRaw; }
			set { m_strRaw = value; }
		}

		public string Text {
			get { return m_strText; }
			set { m_strText = value; }
		}

		public string TagName {
			get { return m_strTagName; }
			set { m_strTagName = value; }
		}

		public Hashtable Attributes {
			get { return m_objAttributes; }
		}

		public string ToJSON()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (string strAttr in this.Attributes.Keys) {
				if (sb.Length == 0)
				{
					sb.Append("{");
				}
				else
				{
					sb.Append(",");
				}
				sb.Append(string.Format("{0}:{1}", strAttr, SafeJSONString((string)this.Attributes[strAttr])));
			}
			if (!String.IsNullOrEmpty(this.Text))
			{
				if (sb.Length == 0)
				{
					sb.Append("{");
				}
				else
				{
					sb.Append(",");
				}
				sb.Append(string.Format("{0}:{1}", "__text", SafeJSONString(this.Text)));
			}
			sb.Append("}");
			return sb.ToString();
		}

		private string SafeJSONString(string strString)
		{
			//TODO: Move this to Utility. ClientAPI!
			return "'" + ((strString.Replace(((char)13).ToString(), "\\r")).Replace(((char)10).ToString(), "\\n")).Replace("'", "\\'") + "'";
		}

	}
}
