#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
using System.Data;
using DotNetNuke.Framework;
//using Exceptions;

namespace DotNetNuke.Services.Scheduling.DNNScheduling
{
    public abstract class DataProvider
    {
        // provider constants - eliminates need for Reflection later
        private const string ProviderType = "data"; // maps to <sectionGroup> in web.config
        private const string ProviderNamespace = "DotNetNuke.Services.Scheduling.DNNScheduling"; // project namespace
        private const string ProviderAssemblyName = "DotNetNuke.DNNScheduler"; // project assemblyname

        // singleton reference to the instantiated object
        private static DataProvider objProvider = null;

        // constructor
        static DataProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (DataProvider)Reflection.CreateObject( ProviderType, ProviderNamespace, ProviderAssemblyName );
        }

        // return the provider
        public new static DataProvider Instance()
        {
            return objProvider;
        }

        // all core methods defined below

        public abstract IDataReader GetSchedule();
        public abstract IDataReader GetSchedule( string Server );
        public abstract IDataReader GetSchedule( int ScheduleID );
        public abstract IDataReader GetSchedule( string TypeFullName, string Server );
        public abstract IDataReader GetNextScheduledTask( string Server );

        public abstract IDataReader GetScheduleByEvent( string EventName, string Server );
        public abstract IDataReader GetScheduleHistory( int ScheduleID );

        public abstract int AddSchedule( string TypeFullName, int TimeLapse, string TimeLapseMeasurement, int RetryTimeLapse, string RetryTimeLapseMeasurement, int RetainHistoryNum, string AttachToEvent, bool CatchUpEnabled, bool Enabled, string ObjectDependencies, string Servers );
        public abstract void UpdateSchedule( int ScheduleID, string TypeFullName, int TimeLapse, string TimeLapseMeasurement, int RetryTimeLapse, string RetryTimeLapseMeasurement, int RetainHistoryNum, string AttachToEvent, bool CatchUpEnabled, bool Enabled, string ObjectDependencies, string Servers );
        public abstract void DeleteSchedule( int ScheduleID );
        public abstract IDataReader GetScheduleItemSettings( int ScheduleID );
        public abstract void AddScheduleItemSetting( int ScheduleID, string Name, string Value );
        public abstract int AddScheduleHistory( int ScheduleID, DateTime StartDate, string Server );
        public abstract void UpdateScheduleHistory( int ScheduleHistoryID, DateTime EndDate, bool Succeeded, string LogNotes, DateTime NextStart );
        public abstract void PurgeScheduleHistory();
    }
}