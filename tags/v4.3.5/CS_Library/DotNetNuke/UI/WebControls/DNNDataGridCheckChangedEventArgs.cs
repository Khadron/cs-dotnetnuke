#region DotNetNuke License
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
#endregion
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The DNNDataGridCheckChangedEventArgs class is a cusom EventArgs class for
    /// handling Event Args from the CheckChanged event in a CheckBox Column
    /// </Summary>
    public class DNNDataGridCheckChangedEventArgs : DataGridItemEventArgs
    {
        private bool mChecked;
        private CheckBoxColumn mColumn;
        private string mField;
        private bool mIsAll;

        public DNNDataGridCheckChangedEventArgs( DataGridItem item, bool isChecked, string field, bool isAll ) : base( item )
        {
            this.mIsAll = false;
            this.mChecked = isChecked;
            this.mIsAll = isAll;
            this.mField = field;
        }

        public DNNDataGridCheckChangedEventArgs( DataGridItem item, bool isChecked, string field ) : this( item, isChecked, field, false )
        {
        }

        public bool Checked
        {
            get
            {
                return this.mChecked;
            }
            set
            {
                this.mChecked = value;
            }
        }

        public CheckBoxColumn Column
        {
            get
            {
                return this.mColumn;
            }
            set
            {
                this.mColumn = value;
            }
        }

        public string Field
        {
            get
            {
                return this.mField;
            }
            set
            {
                this.mField = value;
            }
        }

        public bool IsAll
        {
            get
            {
                return this.mIsAll;
            }
            set
            {
                this.mIsAll = value;
            }
        }
    }
}