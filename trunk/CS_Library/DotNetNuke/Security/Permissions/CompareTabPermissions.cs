using System.Collections;

namespace DotNetNuke.Security.Permissions
{
    public class CompareTabPermissions : IComparer
    {
        public virtual int Compare( object x, object y )
        {
            return ( (TabPermissionInfo)x ).TabPermissionID.CompareTo( ( (TabPermissionInfo)y ).TabPermissionID );
        }
    }
}