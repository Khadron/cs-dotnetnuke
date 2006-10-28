namespace DotNetNuke.Modules.SearchInput
{
    public class SearchResultsModuleInfo
    {
        private string _searchTabName;
        private int _tabID;

        public string SearchTabName
        {
            get
            {
                return this._searchTabName;
            }
            set
            {
                this._searchTabName = value;
            }
        }

        public int TabID
        {
            get
            {
                return this._tabID;
            }
            set
            {
                this._tabID = value;
            }
        }
    }
}