using System.Collections;

namespace DotNetNuke.Services.Scheduling
{
    /// <Summary>
    /// The ScheduleStatusSortRemainingTimeDescending Class is a custom IComparer Implementation
    /// used to sort the Schedule Items
    /// </Summary>
    public class ScheduleStatusSortRemainingTimeDescending : IComparer
    {
        public virtual int Compare( object x, object y )
        {
            return ( (ScheduleHistoryItem)x ).RemainingTime.CompareTo( ( (ScheduleHistoryItem)y ).RemainingTime );
        }
    }
}