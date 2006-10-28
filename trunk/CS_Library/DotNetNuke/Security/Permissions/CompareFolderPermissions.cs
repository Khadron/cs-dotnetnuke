using System.Collections;

namespace DotNetNuke.Security.Permissions
{
    public class CompareFolderPermissions : IComparer
    {
        public virtual int Compare( object x, object y )
        {
            return ( (FolderPermissionInfo)x ).FolderPermissionID.CompareTo( ( (FolderPermissionInfo)y ).FolderPermissionID );
        }
    }
}