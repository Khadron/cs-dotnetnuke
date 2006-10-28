using System;
using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework;
using DotNetNuke.Framework.Providers;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Services.Scheduling
{
    public abstract class SchedulingProvider
    {
        private static bool _Debug;
        private static int _MaxThreads;
        private ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration( "scheduling" );
        private string _providerPath;
        public EventName EventName;

        // singleton reference to the instantiated object
        private static SchedulingProvider objProvider = null;

        public static bool Debug
        {
            get
            {
                return _Debug;
            }
        }

        public static bool Enabled
        {
            get
            {
                if( SchedulerMode != SchedulerMode.DISABLED )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static int MaxThreads
        {
            get
            {
                return _MaxThreads;
            }
        }

        public string ProviderPath
        {
            get
            {
                return _providerPath;
            }
        }

        public static bool ReadyForPoll
        {
            get
            {
                if( DataCache.GetCache( "ScheduleLastPolled" ) == null )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static DateTime ScheduleLastPolled
        {
            get
            {
                if( DataCache.GetCache( "ScheduleLastPolled" ) != null )
                {
                    return Convert.ToDateTime( DataCache.GetCache( "ScheduleLastPolled" ) );
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            set
            {
                DateTime ns;
                ScheduleItem s;
                s = Instance().GetNextScheduledTask( Globals.ServerName );
                if( s != null )
                {
                    DateTime NextStart = s.NextStart;
                    if( NextStart >= DateTime.Now )
                    {
                        ns = NextStart;
                    }
                    else
                    {
                        ns = DateTime.Now.AddMinutes( 1 );
                    }
                }
                else
                {
                    ns = DateTime.Now.AddMinutes( 1 );
                }
                DataCache.SetCache( "ScheduleLastPolled", value, ns );
            }
        }

        public static SchedulerMode SchedulerMode
        {
            get
            {
                string s = Convert.ToString( Globals.HostSettings["SchedulerMode"] );
                if( s.Length == 0 )
                {
                    return SchedulerMode.TIMER_METHOD;
                }
                else
                {
                    return ( (SchedulerMode)Globals.HostSettings["SchedulerMode"] );
                }
            }
        }

        public SchedulingProvider()
        {
            Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            _providerPath = objProvider.Attributes["providerPath"];

            if( objProvider.Attributes["debug"] != null )
            {
                _Debug = Convert.ToBoolean( objProvider.Attributes["debug"] );
            }
            if( objProvider.Attributes["maxThreads"] != null )
            {
                _MaxThreads = Convert.ToInt32( objProvider.Attributes["maxThreads"] );
            }
            else
            {
                _MaxThreads = 1;
            }
        }

        // constructor
        static SchedulingProvider()
        {
            CreateProvider();
        }
        public abstract int AddSchedule( ScheduleItem objScheduleItem );
        public abstract int GetActiveThreadCount();
        public abstract int GetFreeThreadCount();
        public abstract int GetMaxThreadCount();
        public abstract ScheduleItem GetNextScheduledTask( string Server );

        public abstract string GetProviderPath();
        public abstract ArrayList GetSchedule();
        public abstract ArrayList GetSchedule( string Server );
        public abstract ScheduleItem GetSchedule( int ScheduleID );
        public abstract ScheduleItem GetSchedule( string TypeFullName, string Server );
        public abstract ArrayList GetScheduleHistory( int ScheduleID );
        public abstract Hashtable GetScheduleItemSettings( int ScheduleID );
        public abstract ArrayList GetScheduleProcessing();
        public abstract ArrayList GetScheduleQueue();
        public abstract ScheduleStatus GetScheduleStatus();

        // return the provider
        public new static SchedulingProvider Instance()
        {
            return objProvider;
        }
        public abstract void AddScheduleItemSetting( int ScheduleID, string Name, string Value );

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (SchedulingProvider)Reflection.CreateObject( "scheduling" );
        }
        public abstract void DeleteSchedule( ScheduleItem objScheduleItem );
        public abstract void ExecuteTasks();
        public abstract void Halt( string SourceOfHalt );
        public abstract void PurgeScheduleHistory();
        public abstract void ReStart( string SourceOfRestart );
        public abstract void RunEventSchedule( EventName objEventName );

        public abstract void Start();
        public abstract void StartAndWaitForResponse();
        public abstract void UpdateSchedule( ScheduleItem objScheduleItem );
    }
}