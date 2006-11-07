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
using System;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Services.Search
{
    /// <Summary>The SearchItemInfo represents a Search Item</Summary>
    public class SearchItemInfo
    {
        private int _Author;
        private string _Content;
        private string _Description;
        private string _GUID;
        private int _HitCount;
        private int _ImageFileId;
        private int _ModuleId;
        private DateTime _PubDate;
        private int _SearchItemId;
        private string _SearchKey;
        private string _Title;

        public SearchItemInfo()
        {
        }

        public SearchItemInfo( string Title, string Description, int Author, DateTime PubDate, int ModuleID, string SearchKey, string Content ) : this( Title, Description, Author, PubDate, ModuleID, SearchKey, Content, "", Null.NullInteger )
        {
        }

        public SearchItemInfo( string Title, string Description, int Author, DateTime PubDate, int ModuleID, string SearchKey, string Content, string Guid ) : this( Title, Description, Author, PubDate, ModuleID, SearchKey, Content, Guid, Null.NullInteger )
        {
        }

        public SearchItemInfo( string Title, string Description, int Author, DateTime PubDate, int ModuleID, string SearchKey, string Content, int Image ) : this( Title, Description, Author, PubDate, ModuleID, SearchKey, Content, "", Image )
        {
        }

        public SearchItemInfo( string Title, string Description, int Author, DateTime PubDate, int ModuleID, string SearchKey, string Content, string Guid, int Image )
        {
            this._Title = Title;
            this._Description = Description;
            this._Author = Author;
            this._PubDate = PubDate;
            this._ModuleId = ModuleID;
            this._SearchKey = SearchKey;
            this._Content = Content;
            this._GUID = Guid;
            this._ImageFileId = Image;
            this._HitCount = 0;
        }

        public int Author
        {
            get
            {
                return this._Author;
            }
            set
            {
                this._Author = value;
            }
        }

        public string Content
        {
            get
            {
                return this._Content;
            }
            set
            {
                this._Content = value;
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

        public string GUID
        {
            get
            {
                return this._GUID;
            }
            set
            {
                this._GUID = value;
            }
        }

        public int HitCount
        {
            get
            {
                return this._HitCount;
            }
            set
            {
                this._HitCount = value;
            }
        }

        public int ImageFileId
        {
            get
            {
                return this._ImageFileId;
            }
            set
            {
                this._ImageFileId = value;
            }
        }

        public int ModuleId
        {
            get
            {
                return this._ModuleId;
            }
            set
            {
                this._ModuleId = value;
            }
        }

        public DateTime PubDate
        {
            get
            {
                return this._PubDate;
            }
            set
            {
                this._PubDate = value;
            }
        }

        public int SearchItemId
        {
            get
            {
                return this._SearchItemId;
            }
            set
            {
                this._SearchItemId = value;
            }
        }

        public string SearchKey
        {
            get
            {
                return this._SearchKey;
            }
            set
            {
                this._SearchKey = value;
            }
        }

        public string Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }
    }
}