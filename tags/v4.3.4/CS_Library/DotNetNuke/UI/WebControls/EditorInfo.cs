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
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The EditorInfo class provides a helper class for the Property Editor
    /// </Summary>
    public class EditorInfo
    {
        private object[] _Attributes;
        private string _Category;
        private Style _ControlStyle;
        private PropertyEditorMode _EditMode;
        private string _Editor;
        private LabelMode _LabelMode;
        private string _Name;
        private bool _Required;
        private string _ResourceKey;
        private string _Type;
        private string _ValidationExpression;
        private object _Value;
        private UserVisibilityMode _Visibility;

        public object[] Attributes
        {
            get
            {
                return this._Attributes;
            }
            set
            {
                this._Attributes = value;
            }
        }

        public string Category
        {
            get
            {
                return this._Category;
            }
            set
            {
                this._Category = value;
            }
        }

        public Style ControlStyle
        {
            get
            {
                return this._ControlStyle;
            }
            set
            {
                this._ControlStyle = value;
            }
        }

        public PropertyEditorMode EditMode
        {
            get
            {
                return this._EditMode;
            }
            set
            {
                this._EditMode = value;
            }
        }

        public string Editor
        {
            get
            {
                return this._Editor;
            }
            set
            {
                this._Editor = value;
            }
        }

        public LabelMode LabelMode
        {
            get
            {
                return this._LabelMode;
            }
            set
            {
                this._LabelMode = value;
            }
        }

        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        public bool Required
        {
            get
            {
                return this._Required;
            }
            set
            {
                this._Required = value;
            }
        }

        public string ResourceKey
        {
            get
            {
                return this._ResourceKey;
            }
            set
            {
                this._ResourceKey = value;
            }
        }

        public string Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }

        public string ValidationExpression
        {
            get
            {
                return this._ValidationExpression;
            }
            set
            {
                this._ValidationExpression = value;
            }
        }

        public object Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
            }
        }

        public UserVisibilityMode Visibility
        {
            get
            {
                return this._Visibility;
            }
            set
            {
                this._Visibility = value;
            }
        }

        /// <Summary>
        /// GetEditor gets the appropriate Editor based on ID properties
        /// </Summary>
        public static string GetEditor( string editorValue )
        {
            string editor = "UseSystemType";

            ListController objListController = new ListController();
            ListEntryInfo definitionEntry = objListController.GetListEntryInfo("DataType", editorValue);

            if (definitionEntry != null)
            {
                editor = definitionEntry.Text;
            }

            return editor;
        }

        /// <Summary>
        /// GetEditor gets the appropriate Editor based on ID properties
        /// </Summary>
        /// <Param name="editorType">The Id of the Editor</Param>
        public static string GetEditor( int editorType )
        {
            string editor = "UseSystemType";

            ListController objListController = new ListController();
            ListEntryInfo definitionEntry = objListController.GetListEntryInfo(editorType);

            if ((definitionEntry != null) && (definitionEntry.ListName == "DataType"))
            {
                editor = definitionEntry.Text;
            }

            return editor;
            
            
        }
    }
}