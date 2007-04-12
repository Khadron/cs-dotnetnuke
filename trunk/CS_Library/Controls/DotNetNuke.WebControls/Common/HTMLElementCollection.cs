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
using System.Web.UI;

namespace DotNetNuke.UI.WebControls
{
	public class HTMLElementCollection : CollectionBase
	{

		public HTMLElementCollection() : base()
		{
		}

		public HTMLElement this[int index] {
			get { return (HTMLElement)this.List[index]; }
			set { this.List[index] = value; }
		}

		public int Add(HTMLElement value)
		{
			int index = this.List.Add(value);
			return index;
		}

		public void AddRange(HTMLElement[] value)
		{
			for (int i = 0; i <= value.Length - 1; i++) {
				Add(value[i]);
			}
		}

		public int IndexOf(HTMLElement value)
		{
			return this.List.IndexOf(value);
		}

		public bool Contains(HTMLElement value)
		{
              return this.List.Contains(value);
		}

		public void Insert(int index, HTMLElement value)
		{
			List.Insert(index, value);
		}

		public void Remove(HTMLElement value)
		{
			List.Remove(value);
		}
		//Remove

		public void CopyTo(HTMLElement[] array, int index)
		{
			List.CopyTo(array, index);
		}

		public override string ToString()
		{
              System.Text.StringBuilder objSB = new System.Text.StringBuilder();
			foreach (HTMLElement objElement in List) {
				objSB.Append(objElement.Raw);
			}
			return objSB.ToString();
		}

		public string ToJSON()
		{
			return ToJSON("");
		}

		public string ToJSON(string KeyAttribute)
		{
			string strKey;
               System.Text.StringBuilder objSB = new System.Text.StringBuilder();
			int iNotFoundCount = 0;
			objSB.Append("{");
			foreach (HTMLElement objElement in List) {
				if (objSB.Length > 1) objSB.Append(","); 
				if (objElement.Attributes.Contains(KeyAttribute))
				{
					strKey = (string)objElement.Attributes[KeyAttribute];
				}
				else
				{
					iNotFoundCount += 1;
					strKey = "__" + iNotFoundCount.ToString();
				}
                    objSB.Append(strKey + ":" + objElement.ToJSON());
			}
			objSB.Append("}");

			return objSB.ToString();
		}

	}
}

