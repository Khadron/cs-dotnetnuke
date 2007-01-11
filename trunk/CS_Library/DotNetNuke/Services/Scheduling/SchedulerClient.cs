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
using System.Threading;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Services.Scheduling
{
    /// <summary>
    /// This class is inherited by any class that wants to run tasks in the scheduler.
    /// </summary>
    public abstract class SchedulerClient
    {
        private string _ProcessMethod;
        private ScheduleHistoryItem _ScheduleHistoryItem;
        private string _SchedulerEventGUID;
        private string _Status;
        private WorkCompleted ProcessCompletedEvent;
        private WorkErrored ProcessErroredEvent;
        private WorkProgressing ProcessProgressingEvent;
        private WorkStarted ProcessStartedEvent;

        public SchedulerClient()
        {
            //Assign the event a unique ID for tracking purposes.
            this._SchedulerEventGUID = Null.NullString;
            this._ProcessMethod = Null.NullString;
            this._Status = Null.NullString;
            this._ScheduleHistoryItem = new DotNetNuke.Services.Scheduling.ScheduleHistoryItem();
        }

        public string aProcessMethod
        {
            get
            {
                return this._ProcessMethod;
            }
            set
            {
                this._ProcessMethod = value;
            }
        }

        public ScheduleHistoryItem ScheduleHistoryItem
        {
            get
            {
                return this._ScheduleHistoryItem;
            }
            set
            {
                this._ScheduleHistoryItem = value;
            }
        }

        public string SchedulerEventGUID
        {
            get
            {
                return this._SchedulerEventGUID;
            }
            set
            {
                this._SchedulerEventGUID = value;
            }
        }

        public string Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                this._Status = value;
            }
        }

        public int ThreadID
        {
            get
            {
                return Thread.CurrentThread.ManagedThreadId;
            }
        }

        public event WorkCompleted ProcessCompleted
        {
            add
            {
                this.ProcessCompletedEvent = ( (WorkCompleted)Delegate.Combine( ( (Delegate)this.ProcessCompletedEvent ), ( (Delegate)value ) ) );
            }
            remove
            {
                this.ProcessCompletedEvent = ( (WorkCompleted)Delegate.Remove( ( (Delegate)this.ProcessCompletedEvent ), ( (Delegate)value ) ) );
            }
        }

        public event WorkErrored ProcessErrored
        {
            add
            {
                this.ProcessErroredEvent = ( (WorkErrored)Delegate.Combine( ( (Delegate)this.ProcessErroredEvent ), ( (Delegate)value ) ) );
            }
            remove
            {
                this.ProcessErroredEvent = ( (WorkErrored)Delegate.Remove( ( (Delegate)this.ProcessErroredEvent ), ( (Delegate)value ) ) );
            }
        }

        public event WorkProgressing ProcessProgressing
        {
            add
            {
                this.ProcessProgressingEvent = ( (WorkProgressing)Delegate.Combine( ( (Delegate)this.ProcessProgressingEvent ), ( (Delegate)value ) ) );
            }
            remove
            {
                this.ProcessProgressingEvent = ( (WorkProgressing)Delegate.Remove( ( (Delegate)this.ProcessProgressingEvent ), ( (Delegate)value ) ) );
            }
        }

        public event WorkStarted ProcessStarted
        {
            add
            {
                this.ProcessStartedEvent = ( (WorkStarted)Delegate.Combine( ( (Delegate)this.ProcessStartedEvent ), ( (Delegate)value ) ) );
            }
            remove
            {
                this.ProcessStartedEvent = ( (WorkStarted)Delegate.Remove( ( (Delegate)this.ProcessStartedEvent ), ( (Delegate)value ) ) );
            }
        }

        public void Completed()
        {
            WorkCompleted workCompleted = this.ProcessCompletedEvent;
            if( workCompleted == null )
            {
                return;
            }
            SchedulerClient schedulerClient = this;
            workCompleted( ref schedulerClient );
        }

        /// <summary>
        /// This is the sub that kicks off the actual work within the SchedulerClient's subclass
        /// </summary>
        public abstract void DoWork();

        public void Errored( ref Exception objException )
        {
            WorkErrored workErrored = this.ProcessErroredEvent;
            if( workErrored == null )
            {
                return;
            }
            SchedulerClient schedulerClient = this;
            workErrored( ref schedulerClient, ref objException );
        }

        public void Progressing()
        {
            WorkProgressing workProgressing = this.ProcessProgressingEvent;
            if( workProgressing == null )
            {
                return;
            }
            SchedulerClient schedulerClient = this;
            workProgressing( ref schedulerClient );
        }

        public void Started()
        {
            WorkStarted workStarted1 = this.ProcessStartedEvent;
            if( workStarted1 == null )
            {
                return;
            }
            SchedulerClient schedulerClient = this;
            workStarted1( ref schedulerClient );
        }
    }
}