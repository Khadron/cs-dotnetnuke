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

namespace DotNetNuke.Services.Search
{
    /// <Summary>The SearchResultsInfo represents a Search Result Item</Summary>
    public class SearchResultsInfo
    {
        private string m_Author;
        private string m_AuthorName;
        private bool m_Delete;
        private string m_Description;
        private string m_Guid;
        private int m_Image;
        private int m_ModuleId;
        private int m_Occurrences;
        private DateTime m_PubDate;
        private int m_Relevance;
        private int m_SearchItemID;
        private string m_SearchKey;
        private int m_TabId;
        private string m_Title;
        private int m_PortalId;

        public SearchResultsInfo()
        {
            this.m_Delete = false;
        }

        public string Author
        {
            get
            {
                return this.m_Author;
            }
            set
            {
                this.m_Author = value;
            }
        }

        public string AuthorName
        {
            get
            {
                return this.m_AuthorName;
            }
            set
            {
                this.m_AuthorName = value;
            }
        }

        public bool Delete
        {
            get
            {
                return this.m_Delete;
            }
            set
            {
                this.m_Delete = value;
            }
        }

        public string Description
        {
            get
            {
                return this.m_Description;
            }
            set
            {
                this.m_Description = value;
            }
        }

        public string Guid
        {
            get
            {
                return this.m_Guid;
            }
            set
            {
                this.m_Guid = value;
            }
        }

        public int Image
        {
            get
            {
                return this.m_Image;
            }
            set
            {
                this.m_Image = value;
            }
        }

        public int ModuleId
        {
            get
            {
                return this.m_ModuleId;
            }
            set
            {
                this.m_ModuleId = value;
            }
        }

        public int Occurrences
        {
            get
            {
                return this.m_Occurrences;
            }
            set
            {
                this.m_Occurrences = value;
            }
        }

        public DateTime PubDate
        {
            get
            {
                return this.m_PubDate;
            }
            set
            {
                this.m_PubDate = value;
            }
        }

        public int Relevance
        {
            get
            {
                return this.m_Relevance;
            }
            set
            {
                this.m_Relevance = value;
            }
        }

        public int SearchItemID
        {
            get
            {
                return this.m_SearchItemID;
            }
            set
            {
                this.m_SearchItemID = value;
            }
        }

        public string SearchKey
        {
            get
            {
                return this.m_SearchKey;
            }
            set
            {
                this.m_SearchKey = value;
            }
        }

        public int TabId
        {
            get
            {
                return this.m_TabId;
            }
            set
            {
                this.m_TabId = value;
            }
        }

        public string Title
        {
            get
            {
                return this.m_Title;
            }
            set
            {
                this.m_Title = value;
            }
        }

        public int PortalId
        {
            get
            {
                return m_PortalId;
            }
            set
            {
                m_PortalId = value;
            }
        }
    }
}