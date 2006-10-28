using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Modules.SearchInput
{
    public class SearchInputController
    {
        public ArrayList GetSearchResultModules( int PortalID )
        {
            return CBO.FillCollection( DataProvider.Instance().GetSearchResultModules( PortalID ), typeof( SearchResultsModuleInfo ) );
        }
    }
}