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
using System.Reflection;
using System.Threading;
using System.Web.Compilation;

namespace DotNetNuke.Services.Scheduling.DNNScheduling
{
    public class ProcessGroup
    {
        public delegate void CompletedEventHandler();

        public event CompletedEventHandler Completed
        {
            add
            {
                CompletedEvent = (CompletedEventHandler)Delegate.Combine( CompletedEvent, value );
            }
            remove
            {
                CompletedEvent = (CompletedEventHandler)Delegate.Remove( CompletedEvent, value );
            }
        }

        private CompletedEventHandler CompletedEvent;

        private static int numberOfProcesses = 0;

        ///''''''''''''''''''''''''''''''''''''''''''''''''
        //This class represents a process group for
        //our threads to run in.
        ///''''''''''''''''''''''''''''''''''''''''''''''''
        private static int numberOfProcessesInQueue = 0;

        private static int processesCompleted;
        private static int ticksElapsed;

        public static int GetProcessesCompleted
        {
            get
            {
                return processesCompleted;
            }
        }

        public static int GetProcessesInQueue
        {
            get
            {
                return numberOfProcessesInQueue;
            }
        }

        public static int GetTicksElapsed
        {
            get
            {
                return ticksElapsed;
            }
        }

        private SchedulerClient GetSchedulerClient( string strProcess, ScheduleHistoryItem objScheduleHistoryItem )
        {
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //This is a method to encapsulate returning
            //an object whose class inherits SchedulerClient.
            ///''''''''''''''''''''''''''''''''''''''''''''''''            
            Type t = BuildManager.GetType(strProcess, true, true);
            ScheduleHistoryItem[] param = new ScheduleHistoryItem[1];
            param[0] = objScheduleHistoryItem;

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //Get the constructor for the Class
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            Type[] types = new Type[1];
            types[0] = typeof( ScheduleHistoryItem );
            ConstructorInfo objConstructor;
            objConstructor = t.GetConstructor( types );

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            //Return an instance of the class as an object
            ///''''''''''''''''''''''''''''''''''''''''''''''''
            return ( (SchedulerClient)objConstructor.Invoke( param ) );
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''
        //Add a queue request to Threadpool with a
        //callback to RunPooledThread which calls Run()
        ///''''''''''''''''''''''''''''''''''''''''''''''''
        public void AddQueueUserWorkItem( ScheduleItem s )
        {
            numberOfProcessesInQueue++;
            numberOfProcesses++;
            ScheduleHistoryItem obj = new ScheduleHistoryItem();
            obj.TypeFullName = s.TypeFullName;
            obj.ScheduleID = s.ScheduleID;
            obj.TimeLapse = s.TimeLapse;
            obj.TimeLapseMeasurement = s.TimeLapseMeasurement;
            obj.RetryTimeLapse = s.RetryTimeLapse;
            obj.RetryTimeLapseMeasurement = s.RetryTimeLapseMeasurement;
            obj.ObjectDependencies = s.ObjectDependencies;
            obj.CatchUpEnabled = s.CatchUpEnabled;
            obj.Enabled = s.Enabled;
            obj.NextStart = s.NextStart;
            obj.ScheduleSource = s.ScheduleSource;
            obj.ThreadID = s.ThreadID;
            obj.ProcessGroup = s.ProcessGroup;
            obj.RetainHistoryNum = s.RetainHistoryNum;

            try
            {
                // Create a callback to subroutine RunPooledThread
                WaitCallback callback = new WaitCallback( RunPooledThread );
                // And put in a request to ThreadPool to run the process.
                ThreadPool.QueueUserWorkItem( callback, ( (object)obj ) );
                Thread.Sleep( 1000 );
            }
            catch( Exception exc )
            {
                Exceptions.Exceptions.ProcessSchedulerException( exc );
            }
        }

        public void Run( ScheduleHistoryItem objScheduleHistoryItem )
        {
            SchedulerClient Process = null;
            try
            {
                ///''''''''''''''''''''''''''''''''''''''''''''''''
                //This is called from RunPooledThread()
                ///''''''''''''''''''''''''''''''''''''''''''''''''
                ticksElapsed = Environment.TickCount - ticksElapsed;
                Process = GetSchedulerClient( objScheduleHistoryItem.TypeFullName, objScheduleHistoryItem );

                Process.ScheduleHistoryItem = objScheduleHistoryItem;

                ///''''''''''''''''''''''''''''''''''''''''''''''''
                //Set up the handlers for the CoreScheduler
                ///''''''''''''''''''''''''''''''''''''''''''''''''
                Process.ProcessStarted += new WorkStarted( Scheduler.CoreScheduler.WorkStarted );
                Process.ProcessProgressing += new WorkProgressing( Scheduler.CoreScheduler.WorkProgressing );
                Process.ProcessCompleted += new WorkCompleted( Scheduler.CoreScheduler.WorkCompleted );
                Process.ProcessErrored += new WorkErrored( Scheduler.CoreScheduler.WorkErrored );

                ///''''''''''''''''''''''''''''''''''''''''''''''''
                //This kicks off the DoWork method of the class
                //type specified in the configuration.
                ///''''''''''''''''''''''''''''''''''''''''''''''''
                Process.Started();
                try
                {
                    Process.ScheduleHistoryItem.Succeeded = false;
                    Process.DoWork();
                }
                catch( Exception exc )
                {
                    //in case the scheduler client
                    //didn't have proper exception handling
                    //make sure we fire the Errored event
                    if(Process != null)
                    {
                        Process.ScheduleHistoryItem.Succeeded = false;
                        Process.Errored( ref exc );
                    }
                }
                if( Process.ScheduleHistoryItem.Succeeded == true )
                {
                    Process.Completed();
                }
                ///''''''''''''''''''''''''''''''''''''''''''''''''
                //If all processes in this ProcessGroup have
                //completed, set the ticksElapsed and raise
                //the Completed event.
                //I don't think this is necessary with the
                //other events.  I'll leave it for now and
                //will probably take it out later.
                ///''''''''''''''''''''''''''''''''''''''''''''''''
                if( processesCompleted == numberOfProcesses )
                {
                    if( processesCompleted == numberOfProcesses )
                    {
                        ticksElapsed = Environment.TickCount - ticksElapsed;
                        if( CompletedEvent != null )
                        {
                            CompletedEvent();
                        }
                    }
                }
            }
            catch( Exception exc )
            {
                //in case the scheduler client
                //didn't have proper exception handling
                //make sure we fire the Errored event
                if (Process != null)
                {
                    Process.ScheduleHistoryItem.Succeeded = false;
                    Process.Errored( ref exc );
                }
            }
            finally
            {
                ///''''''''''''''''''''''''''''''''''''''''''''''''
                //Track how many processes have completed for
                //this instanciation of the ProcessGroup
                ///''''''''''''''''''''''''''''''''''''''''''''''''
                numberOfProcessesInQueue--;
                processesCompleted++;
            }
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''
        // This subroutine is callback for Threadpool.QueueWorkItem.  This is the necessary
        // subroutine signature for QueueWorkItem, and Run() is proper for creating a Thread
        // so the two subroutines cannot be combined, so instead just call Run from here.
        ///''''''''''''''''''''''''''''''''''''''''''''''''
        private void RunPooledThread( object objScheduleHistoryItem )
        {
            Run( (ScheduleHistoryItem)objScheduleHistoryItem );
        }

        public void RunSingleTask( ScheduleItem s )
        {
            numberOfProcessesInQueue++;
            numberOfProcesses++;
            ScheduleHistoryItem obj = new ScheduleHistoryItem();
            obj.TypeFullName = s.TypeFullName;
            obj.ScheduleID = s.ScheduleID;
            obj.TimeLapse = s.TimeLapse;
            obj.TimeLapseMeasurement = s.TimeLapseMeasurement;
            obj.RetryTimeLapse = s.RetryTimeLapse;
            obj.RetryTimeLapseMeasurement = s.RetryTimeLapseMeasurement;
            obj.ObjectDependencies = s.ObjectDependencies;
            obj.CatchUpEnabled = s.CatchUpEnabled;
            obj.Enabled = s.Enabled;
            obj.NextStart = s.NextStart;
            obj.ScheduleSource = s.ScheduleSource;
            obj.ThreadID = s.ThreadID;
            obj.ProcessGroup = s.ProcessGroup;

            try
            {
                Run( obj );
                Thread.Sleep( 1000 );
            }
            catch( Exception exc )
            {
                Exceptions.Exceptions.ProcessSchedulerException( exc );
            }
        }
    }
}