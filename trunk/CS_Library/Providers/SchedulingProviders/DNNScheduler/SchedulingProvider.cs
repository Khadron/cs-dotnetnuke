using System;
using System.Collections;
using System.Threading;
using DotNetNuke.Common.Utilities;
using Microsoft.VisualBasic;

namespace DotNetNuke.Services.Scheduling.DNNScheduling
{
    public class DNNScheduler : SchedulingProvider
    {
        private const string ProviderType = "schedulingprovider";

        public override string GetProviderPath()
        {
            return ProviderPath;
        }

        public override void Start()
        {
            if( Enabled )
            {
                Scheduler.CoreScheduler s = new Scheduler.CoreScheduler( Debug, MaxThreads );
                Scheduler.CoreScheduler.KeepRunning = true;
                Scheduler.CoreScheduler.KeepThreadAlive = true;
                Scheduler.CoreScheduler.Start();
            }
        }

        public override void ExecuteTasks()
        {
            if( Enabled )
            {
                Scheduler.CoreScheduler s = new Scheduler.CoreScheduler( Debug, MaxThreads );
                Scheduler.CoreScheduler.KeepRunning = true;
                Scheduler.CoreScheduler.KeepThreadAlive = false;
                Scheduler.CoreScheduler.Start();
            }
        }

        public override void ReStart( string SourceOfRestart )
        {
            Halt( SourceOfRestart );
            StartAndWaitForResponse();
        }

        public override void StartAndWaitForResponse()
        {
            if( Enabled )
            {
                Thread newThread = new Thread( new ThreadStart( Start ) );
                newThread.IsBackground = true;
                newThread.Start();

                //wait for up to 30 seconds for thread
                //to start up
                int i;
                for( i = 0; i <= 30; i++ )
                {
                    if( GetScheduleStatus() != ScheduleStatus.STOPPED )
                    {
                        return;
                    }
                    Thread.Sleep( 1000 );
                }
            }
        }

        public override void Halt( string SourceOfHalt )
        {
            Scheduler.CoreScheduler s = new Scheduler.CoreScheduler( Debug, MaxThreads );
            Scheduler.CoreScheduler.Halt( SourceOfHalt );
            Scheduler.CoreScheduler.KeepRunning = false;
        }

        public override void PurgeScheduleHistory()
        {
            Scheduler.CoreScheduler s = new Scheduler.CoreScheduler( MaxThreads );
            Scheduler.CoreScheduler.PurgeScheduleHistory();
        }

        public override void RunEventSchedule( EventName objEventName )
        {
            if( Enabled )
            {
                Scheduler.CoreScheduler s = new Scheduler.CoreScheduler( Debug, MaxThreads );
                Scheduler.CoreScheduler.RunEventSchedule( objEventName );
            }
        }

        public override ArrayList GetSchedule()
        {
            SchedulingController s = new SchedulingController();
            return s.GetSchedule();
        }

        public override ArrayList GetSchedule( string Server )
        {
            SchedulingController s = new SchedulingController();
            return s.GetSchedule( Server );
        }

        public override ScheduleItem GetSchedule( int ScheduleID )
        {
            SchedulingController s = new SchedulingController();
            return s.GetSchedule( ScheduleID );
        }

        public override ScheduleItem GetSchedule( string TypeFullName, string Server )
        {
            SchedulingController s = new SchedulingController();
            return s.GetSchedule( TypeFullName, Server );
        }

        public override ScheduleItem GetNextScheduledTask( string Server )
        {
            SchedulingController s = new SchedulingController();
            return s.GetNextScheduledTask( Server );
        }

        public override ArrayList GetScheduleHistory( int ScheduleID )
        {
            SchedulingController s = new SchedulingController();
            return s.GetScheduleHistory( ScheduleID );
        }

        public override ArrayList GetScheduleQueue()
        {
            SchedulingController s = new SchedulingController();
            return s.GetScheduleQueue();
        }

        public override ArrayList GetScheduleProcessing()
        {
            SchedulingController s = new SchedulingController();
            return s.GetScheduleProcessing();
        }

        public override ScheduleStatus GetScheduleStatus()
        {
            SchedulingController s = new SchedulingController();
            return s.GetScheduleStatus();
        }

        public override int AddSchedule( ScheduleItem objScheduleItem )
        {
            Scheduler.CoreScheduler.RemoveFromScheduleQueue( objScheduleItem );

            SchedulingController s = new SchedulingController();
            int i = s.AddSchedule( objScheduleItem.TypeFullName, objScheduleItem.TimeLapse, objScheduleItem.TimeLapseMeasurement, objScheduleItem.RetryTimeLapse, objScheduleItem.RetryTimeLapseMeasurement, objScheduleItem.RetainHistoryNum, objScheduleItem.AttachToEvent, objScheduleItem.CatchUpEnabled, objScheduleItem.Enabled, objScheduleItem.ObjectDependencies, objScheduleItem.Servers );

            ScheduleHistoryItem objScheduleHistoryItem = new ScheduleHistoryItem( objScheduleItem );
            objScheduleHistoryItem.NextStart = DateTime.Now;
            objScheduleHistoryItem.ScheduleID = i;

            if( objScheduleHistoryItem.TimeLapse != Null.NullInteger && objScheduleHistoryItem.TimeLapseMeasurement != Null.NullString && objScheduleHistoryItem.Enabled )
            {
                objScheduleHistoryItem.ScheduleSource = ScheduleSource.STARTED_FROM_SCHEDULE_CHANGE;
                Scheduler.CoreScheduler.AddToScheduleQueue( objScheduleHistoryItem );
            }
            DataCache.RemoveCache( "ScheduleLastPolled" );
            return i;
        }

        public override void UpdateSchedule( ScheduleItem objScheduleItem )
        {
            Scheduler.CoreScheduler.RemoveFromScheduleQueue( objScheduleItem );

            SchedulingController s = new SchedulingController();
            s.UpdateSchedule( objScheduleItem.ScheduleID, objScheduleItem.TypeFullName, objScheduleItem.TimeLapse, objScheduleItem.TimeLapseMeasurement, objScheduleItem.RetryTimeLapse, objScheduleItem.RetryTimeLapseMeasurement, objScheduleItem.RetainHistoryNum, objScheduleItem.AttachToEvent, objScheduleItem.CatchUpEnabled, objScheduleItem.Enabled, objScheduleItem.ObjectDependencies, objScheduleItem.Servers );

            ScheduleHistoryItem objScheduleHistoryItem = new ScheduleHistoryItem( objScheduleItem );

            if( objScheduleHistoryItem.TimeLapse != Null.NullInteger && objScheduleHistoryItem.TimeLapseMeasurement != Null.NullString && objScheduleHistoryItem.Enabled )
            {
                objScheduleHistoryItem.ScheduleSource = ScheduleSource.STARTED_FROM_SCHEDULE_CHANGE;
                Scheduler.CoreScheduler.AddToScheduleQueue( objScheduleHistoryItem );
            }
            DataCache.RemoveCache( "ScheduleLastPolled" );
        }

        public override void DeleteSchedule( ScheduleItem objScheduleItem )
        {
            SchedulingController s = new SchedulingController();
            s.DeleteSchedule( objScheduleItem.ScheduleID );
            Scheduler.CoreScheduler.RemoveFromScheduleQueue( objScheduleItem );
            DataCache.RemoveCache( "ScheduleLastPolled" );
        }

        public override Hashtable GetScheduleItemSettings( int ScheduleID )
        {
            SchedulingController s = new SchedulingController();
            return s.GetScheduleItemSettings( ScheduleID );
        }

        public override void AddScheduleItemSetting( int ScheduleID, string Name, string Value )
        {
            SchedulingController s = new SchedulingController();
            s.AddScheduleItemSetting( ScheduleID, Name, Value );
        }

        public override int GetFreeThreadCount()
        {
            SchedulingController s = new SchedulingController();
            return s.GetFreeThreadCount();
        }

        public override int GetActiveThreadCount()
        {
            SchedulingController s = new SchedulingController();
            return s.GetActiveThreadCount();
        }

        public override int GetMaxThreadCount()
        {
            SchedulingController s = new SchedulingController();
            return s.GetMaxThreadCount();
        }
    }
}