using DotNetNuke.Framework;

namespace DotNetNuke.Services.Search
{
    public abstract class SearchDataStoreProvider
    {
        // singleton reference to the instantiated object
        private static SearchDataStoreProvider objProvider = null;

        // constructor
        static SearchDataStoreProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (SearchDataStoreProvider)Reflection.CreateObject("searchDataStore");
        }

        // return the provider
        public new static SearchDataStoreProvider Instance()
        {
            return objProvider;
        }

        public abstract void StoreSearchItems(SearchItemInfoCollection SearchItems);
        public abstract SearchResultsInfoCollection GetSearchResults(int PortalID, string Criteria);
        public abstract SearchResultsInfoCollection GetSearchItems(int PortalID, int TabID, int ModuleID);
    }
}