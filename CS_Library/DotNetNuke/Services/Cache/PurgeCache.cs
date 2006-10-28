using System;
using DotNetNuke.Services.Scheduling;

namespace DotNetNuke.Services.Cache
{
    public class PurgeCache : SchedulerClient
    {
        public PurgeCache( ScheduleHistoryItem objScheduleHistoryItem )
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem; //REQUIRED
        }

        public override void DoWork()
        {
            try
            {
                string str = CachingProvider.Instance().PurgeCache();

                this.ScheduleHistoryItem.Succeeded = true; //REQUIRED
                this.ScheduleHistoryItem.AddLogNote( str );
            }
            catch( Exception exc ) //REQUIRED
            {
                this.ScheduleHistoryItem.Succeeded = false; //REQUIRED

                this.ScheduleHistoryItem.AddLogNote( string.Format( "Purging cache task failed.", exc.ToString() ) ); //OPTIONAL

                //notification that we have errored
                this.Errored( ref exc ); //REQUIRED

                //log the exception
                Exceptions.Exceptions.LogException( exc ); //OPTIONAL
            }
        }
    }
}