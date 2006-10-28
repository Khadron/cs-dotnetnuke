using System;

namespace DotNetNuke.UI.WebControls
{
    [AttributeUsage( AttributeTargets.Property )]
    public sealed class ListAttribute : Attribute
    {
        private string _ListName;
        private string _ParentKey;
        private ListBoundField _TextField;
        private ListBoundField _ValueField;

        /// <Summary>Initializes a new instance of the ListAttribute class.</Summary>
        /// <Param name="listName">The name of the List to use for this property</Param>
        /// <Param name="parentKey">The key of the parent for this List</Param>
        public ListAttribute( string listName, string parentKey, ListBoundField valueField, ListBoundField textField )
        {
            this._ListName = listName;
            this._ParentKey = parentKey;
            this._TextField = textField;
            this._ValueField = valueField;
        }

        public string ListName
        {
            get
            {
                return this._ListName;
            }
        }

        public string ParentKey
        {
            get
            {
                return this._ParentKey;
            }
        }

        public ListBoundField TextField
        {
            get
            {
                return this._TextField;
            }
        }

        public ListBoundField ValueField
        {
            get
            {
                return this._ValueField;
            }
        }
    }
}