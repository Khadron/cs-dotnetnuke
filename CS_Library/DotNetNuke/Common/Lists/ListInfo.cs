using System;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Common.Lists
{
    [Serializable()]
    public class ListInfo
    {
        private int mDefinitionID;
        private string mDisplayName;
        private bool mEnableSortOrder;
        private int mEntryCount;
        private bool mIsPopulated;
        private string mKey;
        private int mLevel;
        private string mName;
        private string mParent;
        private int mParentID;
        private string mParentKey;
        private string mParentList;

        public ListInfo( string Name )
        {
            this.mName = Null.NullString;
            this.mDisplayName = Null.NullString;
            this.mLevel = 0;
            this.mDefinitionID = 0;
            this.mKey = Null.NullString;
            this.mEntryCount = 0;
            this.mParentID = 0;
            this.mParentKey = Null.NullString;
            this.mParent = Null.NullString;
            this.mParentList = Null.NullString;
            this.mIsPopulated = false;
            this.mEnableSortOrder = false;
            this.mName = Name;
        }

        public ListInfo()
        {
            this.mName = Null.NullString;
            this.mDisplayName = Null.NullString;
            this.mLevel = 0;
            this.mDefinitionID = 0;
            this.mKey = Null.NullString;
            this.mEntryCount = 0;
            this.mParentID = 0;
            this.mParentKey = Null.NullString;
            this.mParent = Null.NullString;
            this.mParentList = Null.NullString;
            this.mIsPopulated = false;
            this.mEnableSortOrder = false;
        }

        public int DefinitionID
        {
            get
            {
                return this.mDefinitionID;
            }
            set
            {
                this.mDefinitionID = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.mDisplayName;
            }
            set
            {
                this.mDisplayName = value;
            }
        }

        public bool EnableSortOrder
        {
            get
            {
                return this.mEnableSortOrder;
            }
            set
            {
                this.mEnableSortOrder = value;
            }
        }

        public int EntryCount
        {
            get
            {
                return this.mEntryCount;
            }
            set
            {
                this.mEntryCount = value;
            }
        }

        public bool IsPopulated
        {
            get
            {
                return this.mIsPopulated;
            }
            set
            {
                this.mIsPopulated = value;
            }
        }

        public string Key
        {
            get
            {
                return this.mKey;
            }
            set
            {
                this.mKey = value;
            }
        }

        public int Level
        {
            get
            {
                return this.mLevel;
            }
            set
            {
                this.mLevel = value;
            }
        }

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

        public string Parent
        {
            get
            {
                return this.mParent;
            }
            set
            {
                this.mParent = value;
            }
        }

        public int ParentID
        {
            get
            {
                return this.mParentID;
            }
            set
            {
                this.mParentID = value;
            }
        }

        public string ParentKey
        {
            get
            {
                return this.mParentKey;
            }
            set
            {
                this.mParentKey = value;
            }
        }

        public string ParentList
        {
            get
            {
                return this.mParentList;
            }
            set
            {
                this.mParentList = value;
            }
        }

        public bool SystemList
        {
            get
            {
                return ( this.mDefinitionID == -1 );
            }
        }
    }
}