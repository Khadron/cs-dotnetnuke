using System;

namespace DotNetNuke.Services.Scheduling
{
    public class PurgeScheduleHistory : SchedulerClient
    {
        public PurgeScheduleHistory( ScheduleHistoryItem objScheduleHistoryItem )
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }

        public override void DoWork()
        {
           try
            {
                //notification that the event is progressing
                this.Progressing(); //OPTIONAL

                SchedulingProvider.Instance().PurgeScheduleHistory();

                //update the result to success since no exception was thrown
                this.ScheduleHistoryItem.Succeeded = true;
                this.ScheduleHistoryItem.AddLogNote( "Schedule history purged." );
            }
            catch( Exception exc )
            {
                this.ScheduleHistoryItem.Succeeded = false;
                this.ScheduleHistoryItem.AddLogNote( "Schedule history purge failed." + exc.ToString() );
                this.ScheduleHistoryItem.Succeeded = false;

                //notification that we have errored
                this.Errored( ref exc );

                //log the exception
                DotNetNuke.Services.Exceptions.Exceptions.LogException( exc );
            }
        }
        
    }
}