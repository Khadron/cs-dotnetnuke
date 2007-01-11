#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
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
using System;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The DNNDataGridCheckChangedEventArgs class is a cusom EventArgs class for
    /// handling Event Args from the CheckChanged event in a CheckBox Column
    /// </Summary>
    public class PropertyEditorEventArgs : EventArgs
    {
        private string mName;
        private object mOldValue;
        private string mStringValue;
        private object mValue;

        /// <Summary>Constructs a new PropertyEditorEventArgs</Summary>
        /// <Param name="name">The name of the property</Param>
        public PropertyEditorEventArgs( string name ) : this( name, null, null )
        {
        }

        /// <Summary>Constructs a new PropertyEditorEventArgs</Summary>
        /// <Param name="name">The name of the property</Param>
        /// <Param name="newValue">The new value of the property</Param>
        /// <Param name="oldValue">The old value of the property</Param>
        public PropertyEditorEventArgs( string name, object newValue, object oldValue )
        {
            this.mName = name;
            this.mValue = newValue;
            this.mOldValue = oldValue;
        }

        /// <Summary>Gets and sets the Name of the Property being changed</Summary>
        public string Name
        {
            get
            {
                return this.mName;
            }
            set
            {
                this.mName = value;
            }
        }

        /// <Summary>Gets and sets the OldValue of the Property being changed</Summary>
        public object OldValue
        {
            get
            {
                return this.mOldValue;
            }
            set
            {
                this.mOldValue = value;
            }
        }

        /// <Summary>
        /// Gets and sets the String Value of the Property being changed
        /// </Summary>
        public string StringValue
        {
            get
            {
                return this.mStringValue;
            }
            set
            {
                this.mStringValue = value;
            }
        }

        /// <Summary>Gets and sets the Value of the Property being changed</Summary>
        public object Value
        {
            get
            {
                return this.mValue;
            }
            set
            {
                this.mValue = value;
            }
        }
    }
}