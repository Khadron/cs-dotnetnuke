using System.Security.Permissions;
using System.Web;
using System.Web.UI;

namespace DotNetNuke
{
    [AspNetHostingPermission( SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal )]
    public class NavDataSource : HierarchicalDataSourceControl
    {
        // Return a strongly typed view for the current data source control.
        private NavDataSourceView view;

        public NavDataSource()
        {
            this.view = null;
        }

        protected override HierarchicalDataSourceView GetHierarchicalView( string viewPath )
        {
            this.view = new NavDataSourceView( viewPath );
            return this.view;
        }
    }
}