using DotNetNuke.Framework;

namespace DotNetNuke.Services.Search
{
    public abstract class IndexingProvider
    {
        // singleton reference to the instantiated object
        private static IndexingProvider objProvider = null;

        // constructor
        static IndexingProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (IndexingProvider)Reflection.CreateObject("searchIndex");
        }

        // return the provider
        public new static IndexingProvider Instance()
        {
            return objProvider;
        }

        public abstract SearchItemInfoCollection GetSearchIndexItems(int PortalID);
    }
}