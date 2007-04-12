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
using System.Web.UI;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Design;
using System;

namespace DotNetNuke.UI.WebControls
{
	public sealed class TabStripTabCollection : IList, ICollection, IEnumerable
	{

		private DNNTabStrip owner;

		// Properties
		public int Count {
			get {
				if (this.owner.HasControls())
				{
					return this.owner.Controls.Count;
				}
				return 0;
			}
		}

		public bool IsReadOnly {
			get { return false; }
		}


		public bool IsSynchronized {
			get { return false; }
		}


          //public DNNTab Item(int index) 
          //{
          //     return (DNNTab)this.owner.Controls[index];
          //}

		public object SyncRoot {
			get { return this; }
		}

		public bool IsFixedSize {
			get { return false; }
		}

		public object this[int index] {
			get { return this.owner.Controls[index]; }
			set {
				this.RemoveAt(index);
				this.AddAt(index, (DNNTab)value);
			}
		}

		// Methods
		internal TabStripTabCollection(DNNTabStrip owner)
		{
			this.owner = owner;
		}

		public int Add(DNNTab row)
		{
			this.AddAt(-1, row);
			return (this.owner.Controls.Count - 1);
		}

		public void AddAt(int index, DNNTab row)
		{
			this.owner.Controls.AddAt(index, row);
			if (String.IsNullOrEmpty(row.ID))
			{
				row.ID = this.owner.Controls.Count.ToString();
			}
			//JH - COMMENTED 2/29/07 - caused markup with selectedindex to not function
			//If Me.owner.Controls.Count = 1 Then
			//    Me.owner.SelectedIndex = 0    'assign default selected index
			//End If
		}

		public void AddRange(DNNTab[] rows)
		{
			if ((rows == null))
			{
				throw new ArgumentNullException("tabs");
			}

			foreach (DNNTab row1 in rows) {
				this.Add(row1);
			}
		}

		public void Clear()
		{
			if (this.owner.HasControls())
			{
				this.owner.Controls.Clear();
			}
		}

		public void CopyTo(Array array, int index)
		{
			IEnumerator enumerator1 = this.GetEnumerator();
			while (enumerator1.MoveNext()) {
				index += 1;
				array.SetValue(enumerator1.Current, index);
			}
		}

		public IEnumerator GetEnumerator()
		{
              return this.owner.Controls.GetEnumerator();
		}

		public int GetRowIndex(DNNTab row)
		{
			if (this.owner.HasControls())
			{
				return this.owner.Controls.IndexOf(row);
			}
			return -1;
		}

		public void Remove(DNNTab row)
		{
			this.owner.Controls.Remove(row);
		}


		public void RemoveAt(int index)
		{
			this.owner.Controls.RemoveAt(index);
		}

		public int Add(object o)
		{
			return this.Add((DNNTab)o);
		}

		public bool Contains(object o)
		{
			return this.owner.Controls.Contains((DNNTab)o);
		}

		public int IndexOf(object o)
		{
			return this.owner.Controls.IndexOf((DNNTab)o);
		}

		public void Insert(int index, object o)
		{
			this.AddAt(index, (DNNTab)o);
		}

		public void Remove(object o)
		{
			this.Remove((DNNTab)o);
		}

		public DNNTab FindTab(string strId)
		{
			foreach (DNNTab objTab in this) {
				if (objTab.ID == strId)
				{
					return objTab;
				}
			}
			return null;
		}

	}
}
