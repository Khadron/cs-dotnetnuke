using System.Collections;

namespace DotNetNuke.Services.Scheduling
{
    /// <Summary>
    /// The ScheduleHistorySortStartDate Class is a custom IComparer Implementation
    /// used to sort the Schedule Items
    /// </Summary>
    public class ScheduleHistorySortStartDate : IComparer
    {
        public virtual int Compare( object x, object y )
        {
            return ( (ScheduleHistoryItem)y ).StartDate.CompareTo( ( (ScheduleHistoryItem)x ).StartDate );
        }
    }
}