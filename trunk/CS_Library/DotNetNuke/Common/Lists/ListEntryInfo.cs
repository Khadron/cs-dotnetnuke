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
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Common.Lists
{
    [Serializable()]
    public class ListEntryInfo
    {
        private int _DefinitionID;
        private string _Description;
        private string _DisplayName;
        private int _EntryID;
        private bool _HasChildren;
        private string _Key;
        private int _Level;
        private string _ListName;
        private int _ParentID;
        private string _ParentKey;
        private int _SortOrder;
        private string _Text;
        private string _Value;

        public ListEntryInfo()
        {
            this._Key = Null.NullString;
            this._ListName = Null.NullString;
            this._DisplayName = Null.NullString;
            this._Value = Null.NullString;
            this._Text = Null.NullString;
            this._Description = Null.NullString;
            this._ParentID = 0;
            this._Level = 0;
            this._SortOrder = 0;
            this._DefinitionID = 0;
            this._HasChildren = false;
            this._ParentKey = Null.NullString;
        }

        public int DefinitionID
        {
            get
            {
                return this._DefinitionID;
            }
            set
            {
                this._DefinitionID = value;
            }
        }

        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                this._Description = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return this._DisplayName;
            }
            set
            {
                this._DisplayName = value;
            }
        }

        public int EntryID
        {
            get
            {
                return this._EntryID;
            }
            set
            {
                this._EntryID = value;
            }
        }

        public bool HasChildren
        {
            get
            {
                return this._HasChildren;
            }
            set
            {
                this._HasChildren = value;
            }
        }

        public string Key
        {
            get
            {
                return this._Key;
            }
            set
            {
                this._Key = value;
            }
        }

        public int Level
        {
            get
            {
                return this._Level;
            }
            set
            {
                this._Level = value;
            }
        }

        public string ListName
        {
            get
            {
                return this._ListName;
            }
            set
            {
                this._ListName = value;
            }
        }

        public int ParentID
        {
            get
            {
                return this._ParentID;
            }
            set
            {
                this._ParentID = value;
            }
        }

        public string ParentKey
        {
            get
            {
                return this._ParentKey;
            }
            set
            {
                this._ParentKey = value;
            }
        }

        public int SortOrder
        {
            get
            {
                return this._SortOrder;
            }
            set
            {
                this._SortOrder = value;
            }
        }

        public string Text
        {
            get
            {
                return this._Text;
            }
            set
            {
                this._Text = value;
            }
        }

        public string Value
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
    }
}