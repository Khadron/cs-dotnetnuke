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