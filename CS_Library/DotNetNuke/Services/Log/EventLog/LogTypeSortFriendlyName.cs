using System.Collections;

namespace DotNetNuke.Services.Log.EventLog
{
    public class LogTypeSortFriendlyName : IComparer
    {
        public virtual int Compare( object x, object y )
        {
            return ( (LogTypeInfo)x ).LogTypeFriendlyName.CompareTo( ( (LogTypeInfo)y ).LogTypeFriendlyName );
        }
    }
}