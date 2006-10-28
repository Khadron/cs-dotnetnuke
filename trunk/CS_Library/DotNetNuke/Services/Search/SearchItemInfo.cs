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