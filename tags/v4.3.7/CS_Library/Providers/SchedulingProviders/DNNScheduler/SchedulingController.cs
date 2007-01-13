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
using System.Collections;
using System.Data;
using DotNetNuke.Common.Utilities;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Services.Scheduling.DNNScheduling
{
    public class SchedulingController
    {
        public ArrayList GetSchedule()
        {
            ArrayList arrSource = CBO.FillCollection( DataProvider.Instance().GetSchedule(), typeof( ScheduleHistoryItem ) );
            ArrayList arrDest = new ArrayList();
            int i;
            for( i = 0; i <= arrSource.Count - 1; i++ )
            {
                ScheduleItem objScheduleItem;
                objScheduleItem = (ScheduleItem)arrSource[i];
                arrDest.Add( objScheduleItem );
            }
            return arrDest;
        }

        public ArrayList GetSchedule( string Server )
        {
            ArrayList arrSource = CBO.FillCollection( DataProvider.Instance().GetSchedule( Server ), typeof( ScheduleHistoryItem ) );
            ArrayList arrDest = new ArrayList();
            int i;
            for( i = 0; i <= arrSource.Count - 1; i++ )
            {
                ScheduleItem objScheduleItem;
                objScheduleItem = (ScheduleItem)arrSource[i];
                arrDest.Add( objScheduleItem );
            }
            return arrDest;
        }

        public ScheduleItem GetSchedule( string TypeFullName, string Server )
        {
            ScheduleItem objScheduleItem = (ScheduleItem)CBO.FillObject( DataProvider.Instance().GetSchedule( TypeFullName, Server ), typeof( ScheduleItem ) );
            return objScheduleItem;
        }

        public ScheduleItem GetSchedule( int ScheduleID )
        {
            ScheduleItem objScheduleItem = (ScheduleItem)CBO.FillObject( DataProvider.Instance().GetSchedule( ScheduleID ), typeof( ScheduleItem ) );
            return objScheduleItem;
        }

        public ScheduleItem GetNextScheduledTask( string Server )
        {
            ScheduleItem objScheduleItem = (ScheduleItem)CBO.FillObject( DataProvider.Instance().GetNextScheduledTask( Server ), typeof( ScheduleItem ) );
            return objScheduleItem;
        }

        public ArrayList GetScheduleByEvent( string EventName, string Server )
        {
            ArrayList arrSource = CBO.FillCollection( DataProvider.Instance().GetScheduleByEvent( EventName, Server ), typeof( ScheduleHistoryItem ) );
            ArrayList arrDest = new ArrayList();
            int i;
            for( i = 0; i <= arrSource.Count - 1; i++ )
            {
                ScheduleItem objScheduleItem;
                objScheduleItem = (ScheduleItem)arrSource[i];
                arrDest.Add( objScheduleItem );
            }
            return arrDest;
        }

        public ArrayList GetScheduleHistory( int ScheduleID )
        {
            return CBO.FillCollection( DataProvider.Instance().GetScheduleHistory( ScheduleID ), typeof( ScheduleHistoryItem ) );
        }

        public ArrayList GetScheduleQueue()
        {
            return Scheduler.CoreScheduler.GetScheduleQueue();
        }

        public ArrayList GetScheduleProcessing()
        {
            return Scheduler.CoreScheduler.GetScheduleInProgress();
        }

        public ScheduleStatus GetScheduleStatus()
        {
            return Scheduler.CoreScheduler.GetScheduleStatus();
        }

        public int GetFreeThreadCount()
        {
            return Scheduler.CoreScheduler.GetFreeThreadCount();
        }

        public int GetActiveThreadCount()
        {
            return Scheduler.CoreScheduler.GetActiveThreadCount();
        }

        public int GetMaxThreadCount()
        {
            return Scheduler.CoreScheduler.GetMaxThreadCount();
        }

        public void ReloadSchedule()
        {
            Scheduler.CoreScheduler.ReloadSchedule();
        }

        public int AddSchedule( string TypeFullName, int TimeLapse, string TimeLapseMeasurement, int RetryTimeLapse, string RetryTimeLapseMeasurement, int RetainHistoryNum, string AttachToEvent, bool CatchUpEnabled, bool Enabled, string ObjectDependencies, string Servers )
        {
            return DataProvider.Instance().AddSchedule( TypeFullName, TimeLapse, TimeLapseMeasurement, RetryTimeLapse, RetryTimeLapseMeasurement, RetainHistoryNum, AttachToEvent, CatchUpEnabled, Enabled, ObjectDependencies, Servers );
        }

        public void UpdateSchedule( int ScheduleID, string TypeFullName, int TimeLapse, string TimeLapseMeasurement, int RetryTimeLapse, string RetryTimeLapseMeasurement, int RetainHistoryNum, string AttachToEvent, bool CatchUpEnabled, bool Enabled, string ObjectDependencies, string Servers )
        {
            DataProvider.Instance().UpdateSchedule( ScheduleID, TypeFullName, TimeLapse, TimeLapseMeasurement, RetryTimeLapse, RetryTimeLapseMeasurement, RetainHistoryNum, AttachToEvent, CatchUpEnabled, Enabled, ObjectDependencies, Servers );
        }

        public void DeleteSchedule( int ScheduleID )
        {
            DataProvider.Instance().DeleteSchedule( ScheduleID );
        }

        public int AddScheduleHistory( ScheduleHistoryItem objScheduleHistoryItem )
        {
            return DataProvider.Instance().AddScheduleHistory( objScheduleHistoryItem.ScheduleID, objScheduleHistoryItem.StartDate, Globals.ServerName );
        }

        public void UpdateScheduleHistory( ScheduleHistoryItem objScheduleHistoryItem )
        {
            DataProvider.Instance().UpdateScheduleHistory( objScheduleHistoryItem.ScheduleHistoryID, objScheduleHistoryItem.EndDate, objScheduleHistoryItem.Succeeded, objScheduleHistoryItem.LogNotes, objScheduleHistoryItem.NextStart );
        }

        public Hashtable GetScheduleItemSettings( int ScheduleID )
        {
            Hashtable h = new Hashtable();
            IDataReader r = DataProvider.Instance().GetScheduleItemSettings( ScheduleID );
            while( r.Read() )
            {
                h.Add( r["SettingName"], r["SettingValue"] );
            }

            // close datareader
            if( r != null )
            {
                r.Close();
            }

            return h;
        }

        public void AddScheduleItemSetting( int ScheduleID, string Name, string Value )
        {
            DataProvider.Instance().AddScheduleItemSetting( ScheduleID, Name, Value );
        }

        public void PurgeScheduleHistory()
        {
            DataProvider.Instance().PurgeScheduleHistory();
        }
    }
}