#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
using System;
using System.Collections;
using System.Text;
using System.Threading;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Log.EventLog;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Services.Scheduling.DNNScheduling
{
    internal sealed class Scheduler
    {
        public class CoreScheduler
        {
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //This is the heart of the scheduler mechanism.
            //This class manages running new events according
            //to the schedule.
            //
            //This class can also react to the three
            //scheduler events (Started, Progressing and Completed)
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            private static bool ThreadPoolInitialized = false;

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //The MaxThreadCount establishes the maximum
            //threads you want running simultaneously
            //for spawning SchedulerClient processes
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            private static int MaxThreadCount;

            private static int ActiveThreadCount;

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //If KeepRunning gets switched to false,
            //the scheduler stops running.
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            private static bool ForceReloadSchedule = false;

            private static bool Debug = false;

            private static int NumberOfProcessGroups;
            private static ArrayList _ScheduleQueue;
            private static ArrayList _ScheduleInProgress;

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //The ScheduleQueue collection contains the current
            //queue of scheduler clients that need to run
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            private static ArrayList ScheduleQueue
            {
                get
                {
                    try
                    {
                        objQueueReadWriteLock.AcquireReaderLock( ReadTimeout );
                        try
                        {
                            // It is safe for this thread to read or write
                            // from the shared resource.
                            if( _ScheduleQueue == null )
                            {
                                _ScheduleQueue = new ArrayList();
                            }
                            Interlocked.Increment( ref Reads );
                        }
                        finally
                        {
                            // Ensure that the lock is released.
                            objQueueReadWriteLock.ReleaseReaderLock();
                        }
                    }
                    catch( ApplicationException ex )
                    {
                        // The writer lock request timed out.
                        Interlocked.Increment( ref ReadTimeout );
                        Exceptions.Exceptions.LogException( ex );
                    }
                    return _ScheduleQueue;
                }
                set
                {
                    try
                    {
                        objQueueReadWriteLock.AcquireWriterLock( WriteTimeout );
                        try
                        {
                            // It is safe for this thread to read or write
                            // from the shared resource.
                            DataCache.SetCache( "ScheduleQueue", value );
                            _ScheduleQueue = value;
                            Interlocked.Increment( ref Writes );
                        }
                        finally
                        {
                            // Ensure that the lock is released.
                            objQueueReadWriteLock.ReleaseWriterLock();
                        }
                    }
                    catch( ApplicationException ex )
                    {
                        // The writer lock request timed out.
                        Interlocked.Increment( ref WriterTimeouts );
                        Exceptions.Exceptions.LogException( ex );
                    }
                }
            }

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //The ScheduleInProgress collection contains the
            //collection of tasks that are currently in progress
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            private static ArrayList ScheduleInProgress
            {
                get
                {
                    try
                    {
                        objQueueReadWriteLock.AcquireReaderLock( ReadTimeout );
                        try
                        {
                            // It is safe for this thread to read or write
                            // from the shared resource.
                            if( _ScheduleInProgress == null )
                            {
                                _ScheduleInProgress = new ArrayList();
                            }
                            Interlocked.Increment( ref Reads );
                        }
                        finally
                        {
                            // Ensure that the lock is released.
                            objQueueReadWriteLock.ReleaseReaderLock();
                        }
                    }
                    catch( ApplicationException ex )
                    {
                        // The writer lock request timed out.
                        Interlocked.Increment( ref ReadTimeout );
                        Exceptions.Exceptions.LogException( ex );
                    }
                    return _ScheduleInProgress;
                }
                set
                {
                    try
                    {
                        objQueueReadWriteLock.AcquireWriterLock( WriteTimeout );
                        try
                        {
                            // It is safe for this thread to read or write
                            // from the shared resource.
                            DataCache.SetCache( "ScheduleQueue", value );
                            _ScheduleInProgress = value;
                            Interlocked.Increment( ref Writes );
                        }
                        finally
                        {
                            // Ensure that the lock is released.
                            objQueueReadWriteLock.ReleaseWriterLock();
                        }
                    }
                    catch( ApplicationException ex )
                    {
                        // The writer lock request timed out.
                        Interlocked.Increment( ref WriterTimeouts );
                        Exceptions.Exceptions.LogException( ex );
                    }
                }
            }

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //This is our array that holds the process group
            //where our threads will be kicked off.
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            private static ProcessGroup[] arrProcessGroup;

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //A ReaderWriterLock will protect our objects
            //in memory from being corrupted by simultaneous
            //thread operations.  This block of code below
            //establishes variables to help keep track
            //of the ReaderWriter locks.
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            private static ReaderWriterLock objInProgressReadWriteLock = new ReaderWriterLock();

            private static ReaderWriterLock objQueueReadWriteLock = new ReaderWriterLock();
            private static int ReaderTimeouts = 0;
            private static int WriterTimeouts = 0;
            private static int Reads = 0;
            private static int Writes = 0;
            private static int ReadTimeout = 45000; //wait 45 seconds
            private static int WriteTimeout = 45000; //wait 45 seconds
            private static ReaderWriterLock objStatusReadWriteLock = new ReaderWriterLock();
            private static ScheduleStatus Status;

            public static bool KeepThreadAlive = true;
            public static bool KeepRunning = true;

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //FreeThreads tracks how many threads we have
            //free to work with at any given time.
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            public static int FreeThreads
            {
                get
                {
                    return MaxThreadCount - ActiveThreadCount;
                }
            }

            public static int GetActiveThreadCount()
            {
                return ActiveThreadCount;
            }

            public static int GetFreeThreadCount()
            {
                return FreeThreads;
            }

            public static int GetMaxThreadCount()
            {
                return MaxThreadCount;
            }

            public CoreScheduler( int MaxThreads )
            {
                Status = ScheduleStatus.STOPPED;

                if( ! ThreadPoolInitialized )
                {
                    InitializeThreadPool( MaxThreads );
                }
            }

            public CoreScheduler( bool boolDebug, int MaxThreads )
            {
                Status = ScheduleStatus.STOPPED;

                Debug = boolDebug;
                if( ! ThreadPoolInitialized )
                {
                    InitializeThreadPool( MaxThreads );
                }
            }

            private void InitializeThreadPool( int MaxThreads )
            {
                if( MaxThreads == - 1 )
                {
                    MaxThreads = 1;
                }
                NumberOfProcessGroups = MaxThreads;
                MaxThreadCount = MaxThreads;
                
                for(int i = 0; i < NumberOfProcessGroups; i++ )
                {
                    arrProcessGroup = new ProcessGroup[i + 1];
                    
                    arrProcessGroup[i] = new ProcessGroup();
                }
                ThreadPoolInitialized = true;
            }

            public static void ReloadSchedule()
            {
                ForceReloadSchedule = true;
            }

            public static ScheduleStatus GetScheduleStatus()
            {
                ScheduleStatus s = ScheduleStatus.NOT_SET;
                try
                {
                    objStatusReadWriteLock.AcquireReaderLock( ReadTimeout );
                    try
                    {
                        // It is safe for this thread to read from
                        // the shared resource.
                        s = Status;
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        objStatusReadWriteLock.ReleaseReaderLock();
                    }
                }
                catch( ApplicationException )
                {
                    // The reader lock request timed out.
                    Interlocked.Increment( ref ReaderTimeouts );
                }
                return s;
            }

            public static void SetScheduleStatus( ScheduleStatus objScheduleStatus )
            {
                try
                {
                    objStatusReadWriteLock.AcquireWriterLock( WriteTimeout );
                    try
                    {
                        // It is safe for this thread to read or write
                        // from the shared resource.
                        Status = objScheduleStatus;
                        Interlocked.Increment( ref Writes );
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        objStatusReadWriteLock.ReleaseWriterLock();
                    }
                }
                catch( ApplicationException ex )
                {
                    // The writer lock request timed out.
                    Interlocked.Increment( ref WriterTimeouts );
                    Exceptions.Exceptions.LogException( ex );
                }
            }

            public static void Halt( string SourceOfHalt )
            {
                if( SchedulingProvider.SchedulerMode != SchedulerMode.REQUEST_METHOD )
                {
                    EventLogController controller = new EventLogController();
                    SetScheduleStatus( ScheduleStatus.SHUTTING_DOWN );
                    LogInfo logInfo = new LogInfo();
                    logInfo.AddProperty( "Initiator", SourceOfHalt );
                    logInfo.LogTypeKey = "SCHEDULER_SHUTTING_DOWN";
                    controller.AddLog( logInfo );

                    KeepRunning = false;

                    //wait for up to 120 seconds for thread
                    //to shut down
                    for( int i = 0; i <= 120; i++ )
                    {
                        if( GetScheduleStatus() == ScheduleStatus.STOPPED )
                        {
                            return;
                        }
                        Thread.Sleep( 1000 );
                    }
                }
                ActiveThreadCount = 0;
            }

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //This is a multi-thread safe method that adds
            //an item to the collection of schedule items in
            //progress.  It first obtains a write lock
            //on the ScheduleInProgress object.
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            private static void AddToScheduleInProgress( ScheduleHistoryItem objScheduleHistoryItem )
            {
                try
                {
                    objInProgressReadWriteLock.AcquireWriterLock( WriteTimeout );
                    try
                    {
                        // It is safe for this thread to read or write
                        // from the shared resource.
                        ScheduleInProgress.Add( objScheduleHistoryItem );//, objScheduleHistoryItem.ScheduleID.ToString(), null, null );
                        Interlocked.Increment( ref Writes );
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        objInProgressReadWriteLock.ReleaseWriterLock();
                    }
                }
                catch( ApplicationException ex )
                {
                    // The writer lock request timed out.
                    Interlocked.Increment( ref WriterTimeouts );
                    Exceptions.Exceptions.LogException( ex );
                }
            }

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //This is a multi-thread safe method that removes
            //an item from the collection of schedule items in
            //progress.  It first obtains a write lock
            //on the ScheduleInProgress object.
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            private static void RemoveFromScheduleInProgress( ScheduleItem objScheduleItem )
            {
                try
                {
                    objInProgressReadWriteLock.AcquireWriterLock( WriteTimeout );
                    try
                    {
                        // It is safe for this thread to read or write
                        // from the shared resource.
                        ScheduleInProgress.Remove( objScheduleItem.ScheduleID.ToString() );
                        Interlocked.Increment( ref Writes );
                    }
                    catch( ArgumentException )
                    {
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        objInProgressReadWriteLock.ReleaseWriterLock();
                    }
                }
                catch( ApplicationException ex )
                {
                    // The writer lock request timed out.
                    Interlocked.Increment( ref WriterTimeouts );
                    Exceptions.Exceptions.LogException( ex );
                }
            }

            private static bool ScheduleQueueContains( ScheduleItem objScheduleItem )
            {
                int schedulerCount = GetScheduleQueueCount();
                for( int i = 0; i < schedulerCount; i++ )
                {
                    ScheduleItem scheduleItem = ScheduleQueue[i] as ScheduleItem;
                    if (scheduleItem!=null && scheduleItem.ScheduleID == objScheduleItem.ScheduleID)
                    {
                        return true;
                    }
                }
                return false;
            }

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //This is a multi-thread safe method that adds
            //an item to the collection of schedule items in
            //queue.  It first obtains a write lock
            //on the ScheduleQueue object.
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            public static void AddToScheduleQueue( ScheduleHistoryItem objScheduleHistoryItem )
            {
                if( ! ScheduleQueueContains( objScheduleHistoryItem ) )
                {
                    try
                    {
                        objQueueReadWriteLock.AcquireWriterLock( WriteTimeout );
                        try
                        {
                            // It is safe for this thread to read or write
                            // from the shared resource.
                            ScheduleQueue.Add( objScheduleHistoryItem);//, objScheduleHistoryItem.ScheduleID.ToString(), null, null );
                            Interlocked.Increment( ref Writes );
                        }
                        finally
                        {
                            // Ensure that the lock is released.
                            objQueueReadWriteLock.ReleaseWriterLock();
                        }
                    }
                    catch( ApplicationException ex )
                    {
                        // The writer lock request timed out.
                        Interlocked.Increment( ref WriterTimeouts );
                        Exceptions.Exceptions.LogException( ex );
                    }
                }
            }

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //This is a multi-thread safe method that clears
            //the collection of schedule items in
            //queue.  It first obtains a write lock
            //on the ScheduleQueue object.
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            private static void ClearScheduleQueue()
            {
                try
                {
                    int queueCount = GetScheduleQueueCount();
                    if( queueCount == 0 )
                    {
                        return;
                    }
                    objQueueReadWriteLock.AcquireWriterLock( WriteTimeout );
                    try
                    {
                        for( int i = 0; i < queueCount; i++ )
                        {
                            ScheduleQueue.Remove( i );
                        }
                        Interlocked.Increment( ref Writes );
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        objQueueReadWriteLock.ReleaseWriterLock();
                    }
                }
                catch( ApplicationException ex )
                {
                    // The writer lock request timed out.
                    Interlocked.Increment( ref WriterTimeouts );
                    Exceptions.Exceptions.LogException( ex );
                }
            }

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //This is a multi-thread safe method that removes
            //an item from the collection of schedule items in
            //queue.  It first obtains a write lock
            //on the ScheduleQueue object.
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            public static void RemoveFromScheduleQueue( ScheduleItem objScheduleItem )
            {
                try
                {
                    objQueueReadWriteLock.AcquireWriterLock( WriteTimeout );
                    try
                    {
                        // It is safe for this thread to read or write
                        // from the shared resource.
                        ScheduleQueue.Remove( objScheduleItem.ScheduleID.ToString() );
                        Interlocked.Increment( ref Writes );
                    }
                    catch( ArgumentException )
                    {
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        objQueueReadWriteLock.ReleaseWriterLock();
                    }
                }
                catch( ApplicationException ex )
                {
                    // The writer lock request timed out.
                    Interlocked.Increment( ref WriterTimeouts );
                    Exceptions.Exceptions.LogException( ex );
                }
            }

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //This is a multi-thread safe method that returns
            //the number of items in the collection of
            //schedule items in queue.  It first obtains a
            //read lock on the ScheduleQueue object.
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            public static ArrayList GetScheduleQueue()
            {
                ArrayList c = null;
                try
                {
                    objQueueReadWriteLock.AcquireReaderLock( ReadTimeout );
                    try
                    {
                        // It is safe for this thread to read from
                        // the shared resource.
                        c = ScheduleQueue;
                        Interlocked.Increment( ref Reads );
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        objQueueReadWriteLock.ReleaseReaderLock();
                    }
                }
                catch( ApplicationException )
                {
                    // The reader lock request timed out.
                    Interlocked.Increment( ref ReaderTimeouts );
                }
                return c;
            }

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //This is a multi-thread safe method that returns
            //the number of items in the collection of
            //schedule items in progress.  It first obtains a
            //read lock on the ScheduleProgress object.
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            public static ArrayList GetScheduleInProgress()
            {
                ArrayList c = null;
                try
                {
                    objInProgressReadWriteLock.AcquireReaderLock( ReadTimeout );
                    try
                    {
                        // It is safe for this thread to read from
                        // the shared resource.
                        c = ScheduleInProgress;
                        Interlocked.Increment( ref Reads );
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        objInProgressReadWriteLock.ReleaseReaderLock();
                    }
                }
                catch( ApplicationException )
                {
                    // The reader lock request timed out.
                    Interlocked.Increment( ref ReaderTimeouts );
                }
                return c;
            }

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //This is a multi-thread safe method that returns
            //the number of items in the collection of
            //schedule items in queue.  It first obtains a
            //read lock on the ScheduleQueue object.
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            public static int GetScheduleQueueCount()
            {
                int intCount = 0;
                try
                {
                    objQueueReadWriteLock.AcquireReaderLock( ReadTimeout );
                    try
                    {
                        // It is safe for this thread to read from
                        // the shared resource.
                        intCount = ScheduleQueue.Count;
                        Interlocked.Increment( ref Reads );
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        objQueueReadWriteLock.ReleaseReaderLock();
                    }
                }
                catch( ApplicationException )
                {
                    // The reader lock request timed out.
                    Interlocked.Increment( ref ReaderTimeouts );
                }
                return intCount;
            }

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //This is a multi-thread safe method that returns
            //the number of items in the collection of
            //schedule items in progress.  It first obtains a
            //read lock on the ScheduleProgress object.
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            public static int GetScheduleInProgressCount()
            {
                int intCount = 0;
                try
                {
                    objInProgressReadWriteLock.AcquireReaderLock( ReadTimeout );
                    try
                    {
                        // It is safe for this thread to read from
                        // the shared resource.
                        intCount = ScheduleInProgress.Count;
                        Interlocked.Increment( ref Reads );
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        objInProgressReadWriteLock.ReleaseReaderLock();
                    }
                }
                catch( ApplicationException )
                {
                    // The reader lock request timed out.
                    Interlocked.Increment( ref ReaderTimeouts );
                }
                return intCount;
            }

            public static void WorkStarted( ref SchedulerClient objSchedulerClient )
            {
                bool ActiveThreadCountIncremented = false;
                try
                {
                    objSchedulerClient.ScheduleHistoryItem.ThreadID = Thread.CurrentThread.GetHashCode();

                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    //Put the object in the ScheduleInProgress collection
                    //and remove it from the ScheduleQueue
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    RemoveFromScheduleQueue( objSchedulerClient.ScheduleHistoryItem );
                    AddToScheduleInProgress( objSchedulerClient.ScheduleHistoryItem );

                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    //A SchedulerClient is notifying us that their
                    //process has started.  Increase our ActiveThreadCount
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    Interlocked.Increment( ref ActiveThreadCount );
                    ActiveThreadCountIncremented = true;

                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    //Update the schedule item
                    //object property to note the start time.
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    objSchedulerClient.ScheduleHistoryItem.StartDate = DateTime.Now;
                    AddScheduleHistory( objSchedulerClient.ScheduleHistoryItem );

                    if( objSchedulerClient.ScheduleHistoryItem.RetainHistoryNum > 0 )
                    {
                        ///''''''''''''''''''''''''''''''''''''''''''''''''
                        //Write out the log entry for this event
                        ///''''''''''''''''''''''''''''''''''''''''''''''''
                        EventLogController objEventLog = new EventLogController();
                        LogInfo objEventLogInfo = new LogInfo();
                        objEventLogInfo.AddProperty( "THREAD ID", Thread.CurrentThread.GetHashCode().ToString() );
                        objEventLogInfo.AddProperty( "TYPE", objSchedulerClient.GetType().FullName );
                        objEventLogInfo.AddProperty( "SOURCE", objSchedulerClient.ScheduleHistoryItem.ScheduleSource.ToString() );
                        objEventLogInfo.AddProperty( "ACTIVE THREADS", ActiveThreadCount.ToString() );
                        objEventLogInfo.AddProperty( "FREE THREADS", FreeThreads.ToString() );
                        objEventLogInfo.AddProperty( "READER TIMEOUTS", ReaderTimeouts.ToString() );
                        objEventLogInfo.AddProperty( "WRITER TIMEOUTS", WriterTimeouts.ToString() );
                        objEventLogInfo.AddProperty( "IN PROGRESS", GetScheduleInProgressCount().ToString() );
                        objEventLogInfo.AddProperty( "IN QUEUE", GetScheduleQueueCount().ToString() );
                        objEventLogInfo.LogTypeKey = "SCHEDULER_EVENT_STARTED";
                        objEventLog.AddLog( objEventLogInfo );
                    }
                }
                catch( Exception exc )
                {
                    //Decrement the ActiveThreadCount because
                    //otherwise the number of active threads
                    //will appear to be climbing when in fact
                    //no tasks are being executed.
                    if( ActiveThreadCountIncremented )
                    {
                        Interlocked.Decrement( ref ActiveThreadCount );
                    }
                    Exceptions.Exceptions.ProcessSchedulerException( exc );
                }
            }

            public static void WorkProgressing( ref SchedulerClient objSchedulerClient )
            {
                try
                {
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    //A SchedulerClient is notifying us that their
                    //process is in progress.  Informational only.
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    if( objSchedulerClient.ScheduleHistoryItem.RetainHistoryNum > 0 )
                    {
                        ///''''''''''''''''''''''''''''''''''''''''''''''''
                        //Write out the log entry for this event
                        ///''''''''''''''''''''''''''''''''''''''''''''''''
                        EventLogController objEventLog = new EventLogController();
                        LogInfo objEventLogInfo = new LogInfo();
                        objEventLogInfo.AddProperty( "THREAD ID", Thread.CurrentThread.GetHashCode().ToString() );
                        objEventLogInfo.AddProperty( "TYPE", objSchedulerClient.GetType().FullName );
                        objEventLogInfo.AddProperty( "SOURCE", objSchedulerClient.ScheduleHistoryItem.ScheduleSource.ToString() );
                        objEventLogInfo.AddProperty( "ACTIVE THREADS", ActiveThreadCount.ToString() );
                        objEventLogInfo.AddProperty( "FREE THREADS", FreeThreads.ToString() );
                        objEventLogInfo.AddProperty( "READER TIMEOUTS", ReaderTimeouts.ToString() );
                        objEventLogInfo.AddProperty( "WRITER TIMEOUTS", WriterTimeouts.ToString() );
                        objEventLogInfo.AddProperty( "IN PROGRESS", GetScheduleInProgressCount().ToString() );
                        objEventLogInfo.AddProperty( "IN QUEUE", GetScheduleQueueCount().ToString() );
                        objEventLogInfo.LogTypeKey = "SCHEDULER_EVENT_PROGRESSING";
                        objEventLog.AddLog( objEventLogInfo );
                    }
                }
                catch( Exception exc )
                {
                    Exceptions.Exceptions.ProcessSchedulerException( exc );
                }
            }

            public static void WorkCompleted( ref SchedulerClient objSchedulerClient )
            {
                try
                {
                    ScheduleHistoryItem objScheduleHistoryItem;
                    objScheduleHistoryItem = objSchedulerClient.ScheduleHistoryItem;

                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    //Remove the object in the ScheduleInProgress collection
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    RemoveFromScheduleInProgress( objScheduleHistoryItem );

                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    //A SchedulerClient is notifying us that their
                    //process has completed.  Decrease our ActiveThreadCount
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    Interlocked.Decrement( ref ActiveThreadCount );

                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    //Update the schedule item object property
                    //to note the end time and next start
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    objScheduleHistoryItem.EndDate = DateTime.Now;

                    if( objScheduleHistoryItem.ScheduleSource == ScheduleSource.STARTED_FROM_EVENT )
                    {
                        objScheduleHistoryItem.NextStart = Null.NullDate;
                    }
                    else
                    {
                        if( objScheduleHistoryItem.CatchUpEnabled )
                        {
                            switch( objScheduleHistoryItem.TimeLapseMeasurement )
                            {
                                case "s":

                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.NextStart.AddSeconds( objScheduleHistoryItem.TimeLapse );
                                    break;
                                case "m":

                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.NextStart.AddMinutes( objScheduleHistoryItem.TimeLapse );
                                    break;
                                case "h":

                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.NextStart.AddHours( objScheduleHistoryItem.TimeLapse );
                                    break;
                                case "d":

                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.NextStart.AddDays( objScheduleHistoryItem.TimeLapse );
                                    break;
                            }
                        }
                        else
                        {
                            switch( objScheduleHistoryItem.TimeLapseMeasurement )
                            {
                                case "s":

                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddSeconds( objScheduleHistoryItem.TimeLapse );
                                    break;
                                case "m":

                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddMinutes( objScheduleHistoryItem.TimeLapse );
                                    break;
                                case "h":

                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddHours( objScheduleHistoryItem.TimeLapse );
                                    break;
                                case "d":

                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddDays( objScheduleHistoryItem.TimeLapse );
                                    break;
                            }
                        }
                    }

                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    //Update the ScheduleHistory in the database
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    UpdateScheduleHistory( objScheduleHistoryItem );
                    LogInfo objEventLogInfo = new LogInfo();

                    if( objScheduleHistoryItem.NextStart != Null.NullDate )
                    {
                        ///''''''''''''''''''''''''''''''''''''''''''''''''
                        //Put the object back into the ScheduleQueue
                        //collection with the new NextStart date.
                        ///''''''''''''''''''''''''''''''''''''''''''''''''
                        objScheduleHistoryItem.StartDate = Null.NullDate;
                        objScheduleHistoryItem.EndDate = Null.NullDate;
                        objScheduleHistoryItem.LogNotes = "";
                        objScheduleHistoryItem.ProcessGroup = - 1;
                        AddToScheduleQueue( objScheduleHistoryItem );
                    }

                    if( objSchedulerClient.ScheduleHistoryItem.RetainHistoryNum > 0 )
                    {
                        ///''''''''''''''''''''''''''''''''''''''''''''''''
                        //Write out the log entry for this event
                        ///''''''''''''''''''''''''''''''''''''''''''''''''
                        EventLogController objEventLog = new EventLogController();

                        objEventLogInfo.AddProperty( "TYPE", objSchedulerClient.GetType().FullName );
                        objEventLogInfo.AddProperty( "THREAD ID", Thread.CurrentThread.GetHashCode().ToString() );
                        objEventLogInfo.AddProperty( "NEXT START", Convert.ToString( objScheduleHistoryItem.NextStart ) );
                        objEventLogInfo.AddProperty( "SOURCE", objSchedulerClient.ScheduleHistoryItem.ScheduleSource.ToString() );
                        objEventLogInfo.AddProperty( "ACTIVE THREADS", ActiveThreadCount.ToString() );
                        objEventLogInfo.AddProperty( "FREE THREADS", FreeThreads.ToString() );
                        objEventLogInfo.AddProperty( "READER TIMEOUTS", ReaderTimeouts.ToString() );
                        objEventLogInfo.AddProperty( "WRITER TIMEOUTS", WriterTimeouts.ToString() );
                        objEventLogInfo.AddProperty( "IN PROGRESS", GetScheduleInProgressCount().ToString() );
                        objEventLogInfo.AddProperty( "IN QUEUE", GetScheduleQueueCount().ToString() );
                        objEventLogInfo.LogTypeKey = "SCHEDULER_EVENT_COMPLETED";
                        objEventLog.AddLog( objEventLogInfo );
                    }
                }
                catch( Exception exc )
                {
                    Exceptions.Exceptions.ProcessSchedulerException( exc );
                }
            }

            public static void WorkErrored( ref SchedulerClient objSchedulerClient, ref Exception objException )
            {
                try
                {
                    ScheduleHistoryItem objScheduleHistoryItem;
                    objScheduleHistoryItem = objSchedulerClient.ScheduleHistoryItem;
                    ///'''''''''''''''''''''''''''''''''''''''''''''''
                    //Remove the object in the ScheduleInProgress collection
                    ///'''''''''''''''''''''''''''''''''''''''''''''''
                    RemoveFromScheduleInProgress( objScheduleHistoryItem );

                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    //A SchedulerClient is notifying us that their
                    //process has errored.  Decrease our ActiveThreadCount
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    Interlocked.Decrement( ref ActiveThreadCount );

                    Exceptions.Exceptions.ProcessSchedulerException( objException );

                    ///'''''''''''''''''''''''''''''''''''''''''''''''
                    //Update the schedule item object property
                    //to note the end time and next start
                    ///'''''''''''''''''''''''''''''''''''''''''''''''
                    objScheduleHistoryItem.EndDate = DateTime.Now;
                    if( objScheduleHistoryItem.ScheduleSource == ScheduleSource.STARTED_FROM_EVENT )
                    {
                        objScheduleHistoryItem.NextStart = Null.NullDate;
                    }
                    else if( objScheduleHistoryItem.RetryTimeLapse != Null.NullInteger )
                    {
                        switch( objScheduleHistoryItem.RetryTimeLapseMeasurement )
                        {
                            case "s":

                                objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddSeconds( objScheduleHistoryItem.RetryTimeLapse );
                                break;
                            case "m":

                                objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddMinutes( objScheduleHistoryItem.RetryTimeLapse );
                                break;
                            case "h":

                                objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddHours( objScheduleHistoryItem.RetryTimeLapse );
                                break;
                            case "d":

                                objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddDays( objScheduleHistoryItem.RetryTimeLapse );
                                break;
                        }
                    }
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    //Update the ScheduleHistory in the database
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    UpdateScheduleHistory( objScheduleHistoryItem );

                    if( objScheduleHistoryItem.NextStart != Null.NullDate && objScheduleHistoryItem.RetryTimeLapse != Null.NullInteger )
                    {
                        ///''''''''''''''''''''''''''''''''''''''''''''''''
                        //Put the object back into the ScheduleQueue
                        //collection with the new NextStart date.
                        ///''''''''''''''''''''''''''''''''''''''''''''''''
                        objScheduleHistoryItem.StartDate = Null.NullDate;
                        objScheduleHistoryItem.EndDate = Null.NullDate;
                        objScheduleHistoryItem.LogNotes = "";
                        objScheduleHistoryItem.ProcessGroup = - 1;
                        AddToScheduleQueue( objScheduleHistoryItem );
                    }

                    if( objSchedulerClient.ScheduleHistoryItem.RetainHistoryNum > 0 )
                    {
                        ///''''''''''''''''''''''''''''''''''''''''''''''''
                        //Write out the log entry for this event
                        ///''''''''''''''''''''''''''''''''''''''''''''''''
                        EventLogController objEventLog = new EventLogController();
                        LogInfo objEventLogInfo = new LogInfo();
                        objEventLogInfo.AddProperty( "THREAD ID", Thread.CurrentThread.GetHashCode().ToString() );
                        objEventLogInfo.AddProperty( "TYPE", objSchedulerClient.GetType().FullName );
                        if( objException != null )
                        {
                            objEventLogInfo.AddProperty( "EXCEPTION", objException.Message );
                        }
                        objEventLogInfo.AddProperty( "RESCHEDULED FOR", Convert.ToString( objScheduleHistoryItem.NextStart ) );
                        objEventLogInfo.AddProperty( "SOURCE", objSchedulerClient.ScheduleHistoryItem.ScheduleSource.ToString() );
                        objEventLogInfo.AddProperty( "ACTIVE THREADS", ActiveThreadCount.ToString() );
                        objEventLogInfo.AddProperty( "FREE THREADS", FreeThreads.ToString() );
                        objEventLogInfo.AddProperty( "READER TIMEOUTS", ReaderTimeouts.ToString() );
                        objEventLogInfo.AddProperty( "WRITER TIMEOUTS", WriterTimeouts.ToString() );
                        objEventLogInfo.AddProperty( "IN PROGRESS", GetScheduleInProgressCount().ToString() );
                        objEventLogInfo.AddProperty( "IN QUEUE", GetScheduleQueueCount().ToString() );
                        objEventLogInfo.LogTypeKey = "SCHEDULER_EVENT_FAILURE";
                        objEventLog.AddLog( objEventLogInfo );
                    }
                }
                catch( Exception exc )
                {
                    Exceptions.Exceptions.ProcessSchedulerException( exc );
                }
            }

            public static void PurgeScheduleHistory()
            {
                SchedulingController objSchedulingController = new SchedulingController();
                objSchedulingController.PurgeScheduleHistory();
            }

            public static void RunEventSchedule( EventName EventName )
            {
                try
                {
                    EventLogController objEventLog = new EventLogController();
                    LogInfo objEventLogInfo = new LogInfo();
                    objEventLogInfo.AddProperty( "EVENT", EventName.ToString() );
                    objEventLogInfo.LogTypeKey = "SCHEDULE_FIRED_FROM_EVENT";
                    objEventLog.AddLog( objEventLogInfo );

                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    //We allow for three threads to run simultaneously.
                    //As long as we have an open thread, continue.
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    //Load the queue to determine which schedule
                    //items need to be run.
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    LoadQueueFromEvent( EventName );

                    while( GetScheduleQueueCount() > 0 )
                    {
                        SetScheduleStatus( ScheduleStatus.RUNNING_EVENT_SCHEDULE );

                        ///''''''''''''''''''''''''''''''''''''''''''''''''
                        //Fire off the events that need running.
                        ///''''''''''''''''''''''''''''''''''''''''''''''''

                        try
                        {
                            objQueueReadWriteLock.AcquireReaderLock( ReadTimeout );
                            try
                            {
                                // It is safe for this thread to read from
                                // the shared resource.
                                if( GetScheduleQueueCount() > 0 )
                                {
                                    //FireEvents(False)
                                    FireEvents( true );
                                }
                                Interlocked.Increment( ref Reads );
                            }
                            finally
                            {
                                // Ensure that the lock is released.
                                objQueueReadWriteLock.ReleaseReaderLock();
                            }
                        }
                        catch( ApplicationException )
                        {
                            // The reader lock request timed out.
                            Interlocked.Increment( ref ReaderTimeouts );
                        }

                        if( WriterTimeouts > 20 || ReaderTimeouts > 20 )
                        {
                            ///''''''''''''''''''''''''''''''''''''''''''''''''
                            //Wait for 10 minutes so we don't fill up the logs
                            ///''''''''''''''''''''''''''''''''''''''''''''''''
                            Thread.Sleep( 600000 ); //sleep for 10 seconds
                        }
                        else
                        {
                            ///''''''''''''''''''''''''''''''''''''''''''''''''
                            //Wait for 10 seconds to avoid cpu overutilization
                            ///''''''''''''''''''''''''''''''''''''''''''''''''
                            Thread.Sleep( 10000 ); //sleep for 10 seconds
                        }

                        if( GetScheduleQueueCount() == 0 )
                        {
                            return;
                        }
                    }
                }
                catch( Exception exc )
                {
                    Exceptions.Exceptions.ProcessSchedulerException( exc );
                }
            }

            public static void Start()
            {
                try
                {
                    ActiveThreadCount = 0;

                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    //This is where the action begins.
                    //Loop until KeepRunning = false
                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                    if( SchedulingProvider.SchedulerMode != SchedulerMode.REQUEST_METHOD || Debug )
                    {
                        EventLogController objEventLog = new EventLogController();
                        LogInfo objEventLogInfo = new LogInfo();
                        objEventLogInfo.LogTypeKey = "SCHEDULER_STARTED";
                        objEventLog.AddLog( objEventLogInfo );
                    }
                    while( KeepRunning )
                    {
                        try
                        {
                            if( SchedulingProvider.SchedulerMode == SchedulerMode.TIMER_METHOD )
                            {
                                SetScheduleStatus( ScheduleStatus.RUNNING_TIMER_SCHEDULE );
                            }
                            else
                            {
                                SetScheduleStatus( ScheduleStatus.RUNNING_REQUEST_SCHEDULE );
                            }
                            ///''''''''''''''''''''''''''''''''''''''''''''''''
                            //Load the queue to determine which schedule
                            //items need to be run.
                            ///''''''''''''''''''''''''''''''''''''''''''''''''

                            LoadQueueFromTimer();

                            ///''''''''''''''''''''''''''''''''''''''''''''''''
                            //Keep track of when the queue was last refreshed
                            //so we can perform a refresh periodically
                            ///''''''''''''''''''''''''''''''''''''''''''''''''
                            DateTime LastQueueRefresh = DateTime.Now;

                            bool RefreshQueueSchedule = false;

                            ///''''''''''''''''''''''''''''''''''''''''''''''''
                            //We allow for [MaxThreadCount] threads to run
                            //simultaneously.  As long as we have an open thread
                            //and we don't have to refresh the queue, continue
                            //to loop.
                            ///''''''''''''''''''''''''''''''''''''''''''''''''
                            while( FreeThreads > 0 && RefreshQueueSchedule == false && KeepRunning && ForceReloadSchedule == false )
                            {
                                ///''''''''''''''''''''''''''''''''''''''''''''''''
                                //Fire off the events that need running.
                                ///''''''''''''''''''''''''''''''''''''''''''''''''

                                try
                                {
                                    if( SchedulingProvider.SchedulerMode == SchedulerMode.TIMER_METHOD )
                                    {
                                        SetScheduleStatus( ScheduleStatus.RUNNING_TIMER_SCHEDULE );
                                    }
                                    else
                                    {
                                        SetScheduleStatus( ScheduleStatus.RUNNING_REQUEST_SCHEDULE );
                                    }
                                    objQueueReadWriteLock.AcquireReaderLock( ReadTimeout );
                                    try
                                    {
                                        // It is safe for this thread to read from
                                        // the shared resource.
                                        if( GetScheduleQueueCount() > 0 )
                                        {
                                            FireEvents( true );
                                        }
                                        if( KeepThreadAlive == false )
                                        {
                                            return;
                                        }
                                        Interlocked.Increment( ref Reads );
                                    }
                                    finally
                                    {
                                        // Ensure that the lock is released.
                                        objQueueReadWriteLock.ReleaseReaderLock();
                                    }
                                }
                                catch( ApplicationException )
                                {
                                    // The reader lock request timed out.
                                    Interlocked.Increment( ref ReaderTimeouts );
                                }

                                if( WriterTimeouts > 20 || ReaderTimeouts > 20 )
                                {
                                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                                    //Some kind of deadlock on a resource.
                                    //Wait for 10 minutes so we don't fill up the logs
                                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                                    if( KeepRunning )
                                    {
                                        Thread.Sleep( 600000 ); //sleep for 10 minutes
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                                    //Wait for 10 seconds to avoid cpu overutilization
                                    ///''''''''''''''''''''''''''''''''''''''''''''''''
                                    if( KeepRunning )
                                    {
                                        Thread.Sleep( 10000 ); //sleep for 10 seconds
                                    }
                                    else
                                    {
                                        return;
                                    }

                                    //Refresh queue from database every 10 minutes
                                    //if there are no items currently in progress
                                    if( ( LastQueueRefresh.AddMinutes( 10 ) <= DateTime.Now || ForceReloadSchedule ) && FreeThreads == MaxThreadCount )
                                    {
                                        RefreshQueueSchedule = true;
                                        break;
                                    }
                                }
                            }
                            ///''''''''''''''''''''''''''''''''''''''''''''''''
                            //There are no available threads, all threads are being
                            //used.  Wait 10 seconds until one is available
                            ///''''''''''''''''''''''''''''''''''''''''''''''''
                            if( KeepRunning )
                            {
                                if( RefreshQueueSchedule == false )
                                {
                                    SetScheduleStatus( ScheduleStatus.WAITING_FOR_OPEN_THREAD );
                                    Thread.Sleep( 10000 ); //sleep for 10 seconds
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        catch( Exception exc )
                        {
                            Exceptions.Exceptions.ProcessSchedulerException( exc );
                            ///''''''''''''''''''''''''''''''''''''''''''''''''
                            //sleep for 10 minutes
                            ///''''''''''''''''''''''''''''''''''''''''''''''''
                            Thread.Sleep( 600000 );
                        }
                    }
                }
                finally
                {
                    if( SchedulingProvider.SchedulerMode == SchedulerMode.TIMER_METHOD || SchedulingProvider.SchedulerMode == SchedulerMode.DISABLED )
                    {
                        SetScheduleStatus( ScheduleStatus.STOPPED );
                    }
                    else
                    {
                        SetScheduleStatus( ScheduleStatus.WAITING_FOR_REQUEST );
                    }
                    if( SchedulingProvider.SchedulerMode != SchedulerMode.REQUEST_METHOD || Debug )
                    {
                        EventLogController objEventLog = new EventLogController();
                        LogInfo objEventLogInfo = new LogInfo();
                        objEventLogInfo.LogTypeKey = "SCHEDULER_STOPPED";
                        objEventLog.AddLog( objEventLogInfo );
                    }
                }
            }

            public static void FireEvents( bool Asynchronous )
            {
                ///''''''''''''''''''''''''''''''''''''''''''''''''
                //This method uses a thread pool to
                //call the SchedulerClient methods that need
                //to be called.
                ///''''''''''''''''''''''''''''''''''''''''''''''''

                ///''''''''''''''''''''''''''''''''''''''''''''''''
                //For each item in the queue that there
                //is an open thread for, set the object
                //in the array to a new ProcessGroup object.
                //Pass in the ScheduleItem to the ProcessGroup
                //so the ProcessGroup can pass it around for
                //logging and notifications.
                ///''''''''''''''''''''''''''''''''''''''''''''''''
                int intScheduleQueueCount = GetScheduleQueueCount();
                int numToRun = intScheduleQueueCount;
                int numRun = 0;
                //If numToRun > FreeThreads Then
                //	numToRun = FreeThreads
                //End If

                for( int i = 0; i < intScheduleQueueCount; i++ )
                {
                    if( KeepRunning == false )
                    {
                        return;
                    }

                    if(i > ScheduleQueue.Count)
                    {
                        continue;
                    }
                    int ProcessGroup = GetProcessGroup();
                    
                    ScheduleItem objScheduleItem = (ScheduleItem)ScheduleQueue[i];

                    if( objScheduleItem.NextStart <= DateTime.Now && objScheduleItem.Enabled && ! IsInProgress( objScheduleItem ) && ! HasDependenciesConflict( objScheduleItem ) && numRun < numToRun )
                    {
                        objScheduleItem.ProcessGroup = ProcessGroup;
                        if( SchedulingProvider.SchedulerMode == SchedulerMode.TIMER_METHOD )
                        {
                            objScheduleItem.ScheduleSource = ScheduleSource.STARTED_FROM_TIMER;
                        }
                        else if( SchedulingProvider.SchedulerMode == SchedulerMode.REQUEST_METHOD )
                        {
                            objScheduleItem.ScheduleSource = ScheduleSource.STARTED_FROM_BEGIN_REQUEST;
                        }
                        if( Asynchronous )
                        {
                            arrProcessGroup[ProcessGroup].AddQueueUserWorkItem( objScheduleItem );
                        }
                        else
                        {
                            arrProcessGroup[ProcessGroup].RunSingleTask( objScheduleItem );
                        }
                        if( Debug )
                        {
                            EventLogController objEventLog = new EventLogController();
                            LogInfo objEventLogInfo = new LogInfo();
                            objEventLogInfo.AddProperty( "EVENT ADDED TO PROCESS GROUP " + objScheduleItem.ProcessGroup, objScheduleItem.TypeFullName );
                            objEventLogInfo.AddProperty( "SCHEDULE ID", objScheduleItem.ScheduleID.ToString() );
                            objEventLogInfo.LogTypeKey = "DEBUG";
                            objEventLog.AddLog( objEventLogInfo );
                        }
                        numRun++;
                    }
                    else
                    {
                        if( Debug )
                        {
                            bool appended = false;
                            StringBuilder strDebug = new StringBuilder( "Task not run because " );
                            if( objScheduleItem.NextStart > DateTime.Now )
                            {
                                strDebug.Append( " task is scheduled for " + objScheduleItem.NextStart );
                                appended = true;
                            }
                            //If Not (objScheduleItem.NextStart <> Null.NullDate And objScheduleItem.ScheduleSource <> ScheduleSource.STARTED_FROM_EVENT) Then
                            //    If appended Then strDebug.Append(" and")
                            //    strDebug.Append(" task's NextStart <> NullDate and it's wasn't started from an EVENT")
                            //    appended = True
                            //End If
                            if( ! objScheduleItem.Enabled )
                            {
                                if( appended )
                                {
                                    strDebug.Append( " and" );
                                }
                                strDebug.Append( " task is not enabled" );
                                appended = true;
                            }
                            if( IsInProgress( objScheduleItem ) )
                            {
                                if( appended )
                                {
                                    strDebug.Append( " and" );
                                }
                                strDebug.Append( " task is already in progress" );
                                appended = true;
                            }
                            if( HasDependenciesConflict( objScheduleItem ) )
                            {
                                if( appended )
                                {
                                    strDebug.Append( " and" );
                                }
                                strDebug.Append( " task has conflicting dependency" );
                                appended = true;
                            }
                            EventLogController objEventLog = new EventLogController();
                            LogInfo objEventLogInfo = new LogInfo();
                            objEventLogInfo.AddProperty( "EVENT NOT RUN REASON", strDebug.ToString() );
                            objEventLogInfo.AddProperty( "SCHEDULE ID", objScheduleItem.ScheduleID.ToString() );
                            objEventLogInfo.AddProperty( "TYPE FULL NAME", objScheduleItem.TypeFullName );
                            objEventLogInfo.LogTypeKey = "DEBUG";
                            objEventLog.AddLog( objEventLogInfo );
                        }
                    }
                }
            }

            public static void LoadQueueFromTimer()
            {
                ForceReloadSchedule = false;

                SchedulingController s = new SchedulingController();
                ArrayList a = s.GetSchedule( Globals.ServerName );

                for( int i = 0; i < a.Count; i++ )
                {
                    ScheduleHistoryItem objScheduleHistoryItem;
                    objScheduleHistoryItem = (ScheduleHistoryItem)( a[i] );

                    if( ! IsInQueue( objScheduleHistoryItem ) && objScheduleHistoryItem.TimeLapse != Null.NullInteger && objScheduleHistoryItem.TimeLapseMeasurement != Null.NullString && objScheduleHistoryItem.Enabled )
                    {
                        if( SchedulingProvider.SchedulerMode == SchedulerMode.TIMER_METHOD )
                        {
                            objScheduleHistoryItem.ScheduleSource = ScheduleSource.STARTED_FROM_TIMER;
                        }
                        else if( SchedulingProvider.SchedulerMode == SchedulerMode.REQUEST_METHOD )
                        {
                            objScheduleHistoryItem.ScheduleSource = ScheduleSource.STARTED_FROM_BEGIN_REQUEST;
                        }
                        AddToScheduleQueue( objScheduleHistoryItem );
                    }
                }
            }

            public static void LoadQueueFromEvent( EventName EventName )
            {
                SchedulingController s = new SchedulingController();
                ArrayList a = s.GetScheduleByEvent( EventName.ToString(), Globals.ServerName );

                for( int i = 0; i < a.Count; i++ )
                {
                    ScheduleHistoryItem scheduleItem = (ScheduleHistoryItem)( a[i] );

                    if( ! IsInQueue( scheduleItem ) && ! IsInProgress( scheduleItem ) && ! HasDependenciesConflict( scheduleItem ) && scheduleItem.Enabled )
                    {
                        scheduleItem.ScheduleSource = ScheduleSource.STARTED_FROM_EVENT;
                        AddToScheduleQueue( scheduleItem );
                    }
                }
            }

            private static int GetProcessGroup()
            {
                //return a random process group
                Random r = new Random();
                return r.Next( 0, NumberOfProcessGroups - 1 );
            }

            internal static bool IsInQueue( ScheduleItem objScheduleItem )
            {
                bool objReturn = false;
                try
                {
                    objQueueReadWriteLock.AcquireReaderLock( ReadTimeout );
                    try
                    {
                        // It is safe for this thread to read from
                        // the shared resource.
                        int schedulerCount = GetScheduleQueueCount();
                        if( schedulerCount > 0 )
                        {
                            for( int i = 0; i < schedulerCount; i++ )
                            {
                                ScheduleItem obj = ScheduleQueue[i] as ScheduleItem;
                                if( obj!=null && obj.ScheduleID == objScheduleItem.ScheduleID )
                                {
                                    objReturn = true;
                                }
                            }
                        }
                        Interlocked.Increment( ref Reads );
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        objQueueReadWriteLock.ReleaseReaderLock();
                    }
                }
                catch( ApplicationException )
                {
                    // The reader lock request timed out.
                    Interlocked.Increment( ref ReaderTimeouts );
                }
                return objReturn;
            }

            private static bool IsInProgress( ScheduleItem objScheduleItem )
            {
                
                bool objReturn = false;
                try
                {
                    objInProgressReadWriteLock.AcquireReaderLock( ReadTimeout );
                    try
                    {
                        // It is safe for this thread to read from
                        // the shared resource.
                        int inProcCount = ScheduleInProgress.Count;
                        if( inProcCount > 0 )
                        {
                            for( int i = 0; i < inProcCount; i++ )
                            {
                                ScheduleItem obj = ScheduleInProgress[i] as ScheduleItem;
                                if( obj!= null && obj.ScheduleID == objScheduleItem.ScheduleID )
                                {
                                    objReturn = true;
                                }
                            }
                        }
                        Interlocked.Increment( ref Reads );
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        objInProgressReadWriteLock.ReleaseReaderLock();
                    }
                }
                catch( ApplicationException )
                {
                    // The reader lock request timed out.
                    Interlocked.Increment( ref ReaderTimeouts );
                }
                return objReturn;
            }

            public static bool HasDependenciesConflict( ScheduleItem objScheduleItem )
            {
                
                bool objReturn = false;
                try
                {
                    objInProgressReadWriteLock.AcquireReaderLock( ReadTimeout );
                    try
                    {
                        // It is safe for this thread to read from
                        // the shared resource.
                        if( ScheduleInProgress != null && objScheduleItem.ObjectDependencies.Length > 0 )
                        {
                            int inProgressCount = ScheduleInProgress.Count;
                            for( int i = 0; i < inProgressCount; i++ )
                            {
                                ScheduleItem obj = ScheduleInProgress[i] as ScheduleItem;
                                if( obj!=null &&  obj.ObjectDependencies.Length > 0 )
                                {
                                    if( obj.HasObjectDependencies( objScheduleItem.ObjectDependencies ) )
                                    {
                                        objReturn = true;
                                    }
                                }
                            }
                        }

                        Interlocked.Increment( ref Reads );
                    }
                    finally
                    {
                        // Ensure that the lock is released.
                        objInProgressReadWriteLock.ReleaseReaderLock();
                    }
                }
                catch( ApplicationException )
                {
                    // The reader lock request timed out.
                    Interlocked.Increment( ref ReaderTimeouts );
                }

                return objReturn;
            }

            public static ScheduleHistoryItem AddScheduleHistory( ScheduleHistoryItem objScheduleHistoryItem )
            {
                try
                {
                    SchedulingController controller = new SchedulingController();
                    int historyID = controller.AddScheduleHistory( objScheduleHistoryItem );

                    objScheduleHistoryItem.ScheduleHistoryID = historyID;
                }
                catch( Exception exc )
                {
                    Exceptions.Exceptions.ProcessSchedulerException( exc );
                }
                return objScheduleHistoryItem;
            }

            public static void UpdateScheduleHistory( ScheduleHistoryItem objScheduleHistoryItem )
            {
                try
                {
                    SchedulingController controller = new SchedulingController();
                    controller.UpdateScheduleHistory( objScheduleHistoryItem );
                }
                catch( Exception exc )
                {
                    Exceptions.Exceptions.ProcessSchedulerException( exc );
                }
            }

           
        }
    }
}