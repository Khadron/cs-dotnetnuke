using System;
using DotNetNuke.Services.Scheduling;

namespace DotNetNuke.Services.Search
{
    /// <Summary>
    /// The SearchEngineScheduler implements a SchedulerClient for the Indexing of
    /// portal content.
    /// </Summary>
    public class SearchEngineScheduler : SchedulerClient
    {
        public SearchEngineScheduler( ScheduleHistoryItem objScheduleHistoryItem )
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }

        /// <Summary>DoWork runs the scheduled item</Summary>
        public override void DoWork()
        {
            try
            {
                SearchEngine se = new SearchEngine();
                se.IndexContent();
                ScheduleHistoryItem.Succeeded = true;
                ScheduleHistoryItem.AddLogNote( "Completed re-indexing content" );
            }
            catch( Exception ex )
            {
                ScheduleHistoryItem.Succeeded = false;
                ScheduleHistoryItem.AddLogNote( "EXCEPTION: " + ex.Message );
                Errored( ref ex );
                Exceptions.Exceptions.LogException( ex );
            }
        }
    }
}