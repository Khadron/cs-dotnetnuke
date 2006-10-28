using System;
using System.Collections;
using DotNetNuke.Common;
using DotNetNuke.Services.Scheduling;

namespace DotNetNuke.Services.Cache.BroadcastPollingCachingProvider
{
    public class Poll : SchedulerClient
    {
        public Poll( ScheduleHistoryItem objScheduleHistoryItem )
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem; //REQUIRED
        }

        public override void DoWork()
        {
            try
            {
                Controller c = new Controller();
                ArrayList arr = c.GetBroadcasts( Globals.ServerName );
                int i;
                for( i = 0; i <= arr.Count - 1; i++ )
                {
                    BroadcastInfo objBroadcastInfo;
                    objBroadcastInfo = (BroadcastInfo)arr[i];
                    switch( objBroadcastInfo.BroadcastType )
                    {
                        case "RemoveCachedItem":

                            CachingProvider.Instance().Remove( objBroadcastInfo.BroadcastMessage );
                            break;
                    }
                    this.ScheduleHistoryItem.AddLogNote( "Broadcast Type: " + objBroadcastInfo.BroadcastType + ", Broadcast Message: " + objBroadcastInfo.BroadcastMessage + '\r' );
                }
                this.ScheduleHistoryItem.Succeeded = true; //REQUIRED
            }
            catch( Exception exc ) //REQUIRED
            {
                this.ScheduleHistoryItem.Succeeded = false; //REQUIRED

                this.ScheduleHistoryItem.AddLogNote( string.Format( "An error ocurred polling for server broadcasts.", exc.ToString() ) ); //OPTIONAL

                //notification that we have errored
                this.Errored( ref exc ); //REQUIRED

                //log the exception
                Exceptions.Exceptions.LogException( exc ); //OPTIONAL
            }
        }
    }
}