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