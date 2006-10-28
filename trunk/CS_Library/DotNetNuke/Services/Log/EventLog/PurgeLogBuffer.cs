using System;
using DotNetNuke.Services.Scheduling;

namespace DotNetNuke.Services.Log.EventLog
{
    public class PurgeLogBuffer : SchedulerClient
    {
        public PurgeLogBuffer(ScheduleHistoryItem objScheduleHistoryItem)
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }

        public override void DoWork()
        {
            try
            {
                //notification that the event is progressing
                this.Progressing(); //OPTIONAL
                LoggingProvider.Instance().PurgeLogBuffer();
                this.ScheduleHistoryItem.Succeeded = true; //REQUIRED
                this.ScheduleHistoryItem.AddLogNote("Purged log entries successfully"); //OPTIONAL
            }
            catch (Exception exc) //REQUIRED
            {
                this.ScheduleHistoryItem.Succeeded = false; //REQUIRED
                this.ScheduleHistoryItem.AddLogNote("EXCEPTION: " + exc.ToString()); //OPTIONAL
                this.Errored(ref exc); //REQUIRED
                //log the exception
                Exceptions.Exceptions.LogException(exc); //OPTIONAL
            }
        }
    }
}