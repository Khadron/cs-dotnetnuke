using System.Collections;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Services.Search
{
    /// <Summary>
    /// The SearchEngine  manages the Indexing of the Portal content
    /// </Summary>
    public class SearchEngine
    {
        /// <Summary>
        /// GetContent gets all the content and passes it to the Indexer
        /// </Summary>
        /// <Param name="Indexer">
        /// The Index Provider that will index the content of the portal
        /// </Param>
        protected SearchItemInfoCollection GetContent( IndexingProvider Indexer )
        {
            SearchItemInfoCollection SearchItems = new SearchItemInfoCollection();
            PortalController objPortals = new PortalController();
            PortalInfo objPortal;

            ArrayList arrPortals = objPortals.GetPortals();
            int intPortal;
            for (intPortal = 0; intPortal <= arrPortals.Count - 1; intPortal++)
            {
                objPortal = (PortalInfo)arrPortals[intPortal];

                SearchItems.AddRange(Indexer.GetSearchIndexItems(objPortal.PortalID));
            }
            return SearchItems;
        }

        /// <Summary>
        /// GetContent gets the Portal's content and passes it to the Indexer
        /// </Summary>
        /// <Param name="PortalID">The Id of the Portal</Param>
        /// <Param name="Indexer">
        /// The Index Provider that will index the content of the portal
        /// </Param>
        protected SearchItemInfoCollection GetContent( int PortalID, IndexingProvider Indexer )
        {
            SearchItemInfoCollection SearchItems = new SearchItemInfoCollection();

            SearchItems.AddRange(Indexer.GetSearchIndexItems(PortalID));

            return SearchItems;
        }

        /// <Summary>IndexContent indexes all Portal content</Summary>
        public void IndexContent()
        {
            IndexingProvider indexer = IndexingProvider.Instance();
            SearchDataStoreProvider.Instance().StoreSearchItems( this.GetContent( indexer ) );
        }

        /// <Summary>IndexContent indexes the Portal's content</Summary>
        /// <Param name="PortalID">The Id of the Portal</Param>
        public void IndexContent( int PortalID )
        {
            IndexingProvider indexer = IndexingProvider.Instance();
            SearchDataStoreProvider.Instance().StoreSearchItems( this.GetContent( PortalID, indexer ) );
        }
    }
}