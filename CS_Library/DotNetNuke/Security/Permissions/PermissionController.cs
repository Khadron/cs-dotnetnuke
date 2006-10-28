using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Security.Permissions
{
    public class PermissionController
    {
        public int AddPermission( PermissionInfo objPermission )
        {
            return DataProvider.Instance().AddPermission( objPermission.PermissionCode, objPermission.ModuleDefID, objPermission.PermissionKey, objPermission.PermissionName );
        }

        public PermissionInfo GetPermission( int permissionID )
        {
            return ( (PermissionInfo)CBO.FillObject( DataProvider.Instance().GetPermission( permissionID ), typeof( PermissionInfo ) ) );
        }

        public ArrayList GetPermissionByCodeAndKey( string PermissionCode, string PermissionKey )
        {
            return CBO.FillCollection( DataProvider.Instance().GetPermissionByCodeAndKey( PermissionCode, PermissionKey ), typeof( PermissionInfo ) );
        }

        public ArrayList GetPermissionsByFolder( int PortalID, string Folder )
        {
            return CBO.FillCollection( DataProvider.Instance().GetPermissionsByFolderPath( PortalID, Folder ), typeof( PermissionInfo ) );
        }

        public ArrayList GetPermissionsByModuleID( int ModuleID )
        {
            return CBO.FillCollection( DataProvider.Instance().GetPermissionsByModuleID( ModuleID ), typeof( PermissionInfo ) );
        }

        public ArrayList GetPermissionsByTabID( int TabID )
        {
            return CBO.FillCollection( DataProvider.Instance().GetPermissionsByTabID( TabID ), typeof( PermissionInfo ) );
        }

        public void DeletePermission( int permissionID )
        {
            DataProvider.Instance().DeletePermission( permissionID );
        }

        public void UpdatePermission( PermissionInfo objPermission )
        {
            DataProvider.Instance().UpdatePermission( objPermission.PermissionID, objPermission.PermissionCode, objPermission.ModuleDefID, objPermission.PermissionKey, objPermission.PermissionName );
        }
    }
}