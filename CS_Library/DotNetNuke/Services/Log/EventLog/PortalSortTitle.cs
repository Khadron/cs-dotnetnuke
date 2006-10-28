using System.Collections;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Services.Log.EventLog
{
    public class PortalSortTitle : IComparer
    {
        public virtual int Compare( object x, object y )
        {
            return ( (PortalInfo)x ).PortalName.CompareTo( ( (PortalInfo)y ).PortalName );
        }
    }
}