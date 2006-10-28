using System.Collections;

namespace DotNetNuke.Security.Roles
{
    /// <Summary>
    /// The RoleComparer class provides an Implementation of IComparer for
    /// RoleInfo objects
    /// </Summary>
    public class RoleComparer : IComparer
    {
        /// <Summary>
        /// Compares two RoleInfo objects by performing a comparison of their rolenames
        /// </Summary>
        /// <Param name="x">One of the items to compare</Param>
        /// <Param name="y">One of the items to compare</Param>
        /// <Returns>
        /// An Integer that determines whether x is greater, smaller or equal to y
        /// </Returns>
        public virtual int Compare( object x, object y )
        {
            return new CaseInsensitiveComparer().Compare( ( (RoleInfo)x ).RoleName, ( (RoleInfo)y ).RoleName );
        }
    }
}