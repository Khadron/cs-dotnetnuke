using System.Collections;

namespace DotNetNuke.Security.Permissions
{
    public class CompareModulePermissions : IComparer
    {
        public virtual int Compare( object x, object y )
        {
            return ( (ModulePermissionInfo)x ).ModulePermissionID.CompareTo( ( (ModulePermissionInfo)y ).ModulePermissionID );
        }
    }
}