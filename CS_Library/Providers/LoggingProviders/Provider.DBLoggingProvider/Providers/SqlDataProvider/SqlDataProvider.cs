using System;
using System.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using Microsoft.ApplicationBlocks.Data;

namespace DotNetNuke.Services.Log.EventLog.DBLoggingProvider.Data
{
    public class SqlDataProvider : DataProvider
    {
        private const string ProviderType = "data";
        private ProviderConfiguration _providerConfiguration;
        private string _connectionString;
        private string _providerPath;
        private string _objectQualifier;
        private string _databaseOwner;

        public SqlDataProvider()
        {
            // Read the configuration specific information for this provider
            Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            // Read the attributes for this provider
            //Get Connection string from web.config
            _connectionString = Config.GetConnectionString();

            if( _connectionString == "" )
            {
                // Use connection string specified in provider
                _connectionString = objProvider.Attributes["connectionString"];
            }

            _providerPath = objProvider.Attributes["providerPath"];

            _objectQualifier = objProvider.Attributes["objectQualifier"];
            if( _objectQualifier != "" && _objectQualifier.EndsWith( "_" ) == false )
            {
                _objectQualifier += "_";
            }

            _databaseOwner = objProvider.Attributes["databaseOwner"];
            if( _databaseOwner != "" && _databaseOwner.EndsWith( "." ) == false )
            {
                _databaseOwner += ".";
            }
        }

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        public string ProviderPath
        {
            get
            {
                return _providerPath;
            }
        }

        public string ObjectQualifier
        {
            get
            {
                return _objectQualifier;
            }
        }

        public string DatabaseOwner
        {
            get
            {
                return _databaseOwner;
            }
        }

        private object GetNull( object Field )
        {
            return Null.GetNull( Field, DBNull.Value );
        }

        //---------------------------------------------------------------------
        // TODO Implement DAL methods.
        // Use CodeSmith templates to generate this code
        //---------------------------------------------------------------------

        public override void AddLog( string LogGUID, string LogTypeKey, int LogUserID, string LogUserName, int LogPortalID, string LogPortalName, DateTime LogCreateDate, string LogServerName, string LogProperties, int LogConfigID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddEventLog", LogGUID, LogTypeKey, GetNull( LogUserID ), GetNull( LogUserName ), GetNull( LogPortalID ), GetNull( LogPortalName ), LogCreateDate, LogServerName, LogProperties, LogConfigID );
        }

        public override void AddLogTypeConfigInfo( bool LoggingIsActive, string LogTypeKey, string LogTypePortalID, int KeepMostRecent, bool EmailNotificationIsActive, int Threshold, int NotificationThresholdTime, int NotificationThresholdTimeType, string MailFromAddress, string MailToAddress )
        {
            int PortalID;
            if( LogTypeKey == "*" )
            {
                LogTypeKey = "";
            }
            if( LogTypePortalID == "*" )
            {
                PortalID = - 1;
            }
            else
            {
                PortalID = Convert.ToInt32( LogTypePortalID );
            }
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddEventLogConfig", GetNull( LogTypeKey ), GetNull( PortalID ), LoggingIsActive, KeepMostRecent, EmailNotificationIsActive, GetNull( Threshold ), GetNull( NotificationThresholdTime ), GetNull( NotificationThresholdTimeType ), MailFromAddress, MailToAddress );
        }

        public override void UpdateLogTypeConfigInfo( string ID, bool LoggingIsActive, string LogTypeKey, string LogTypePortalID, int KeepMostRecent, string LogFileName, bool EmailNotificationIsActive, int Threshold, int NotificationThresholdTime, int NotificationThresholdTimeType, string MailFromAddress, string MailToAddress )
        {
            int PortalID;
            if( LogTypeKey == "*" )
            {
                LogTypeKey = "";
            }
            if( LogTypePortalID == "*" )
            {
                PortalID = - 1;
            }
            else
            {
                PortalID = Convert.ToInt32( LogTypePortalID );
            }
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateEventLogConfig", ID, GetNull( LogTypeKey ), GetNull( PortalID ), LoggingIsActive, KeepMostRecent, EmailNotificationIsActive, GetNull( Threshold ), GetNull( NotificationThresholdTime ), GetNull( NotificationThresholdTimeType ), MailFromAddress, MailToAddress );
        }

        public override void ClearLog()
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteEventLog", DBNull.Value );
        }

        public override void DeleteLog( string LogGUID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteEventLog", LogGUID );
        }

        public override void DeleteLogTypeConfigInfo( string ID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteEventLogConfig", ID );
        }

        public override IDataReader GetLog()
        {
            return ( SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", DBNull.Value, DBNull.Value ) );
        }

        public override IDataReader GetSingleLog( string LogGUID )
        {
            return ( SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogByLogGUID", LogGUID ) );
        }

        public override IDataReader GetLog( int PortalID )
        {
            return ( SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", PortalID, DBNull.Value ) );
        }

        public override IDataReader GetLog( int PortalID, string LogType )
        {
            return ( SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", PortalID, LogType ) );
        }

        public override IDataReader GetLog( string LogType )
        {
            return ( SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", DBNull.Value, LogType ) );
        }

        public override IDataReader GetLog( int PageSize, int PageIndex )
        {
            return ( SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", DBNull.Value, DBNull.Value, PageSize, PageIndex ) );
        }

        public override IDataReader GetLog( int PortalID, int PageSize, int PageIndex )
        {
            return ( SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", PortalID, DBNull.Value, PageSize, PageIndex ) );
        }

        public override IDataReader GetLog( int PortalID, string LogType, int PageSize, int PageIndex )
        {
            return ( SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", PortalID, LogType, PageSize, PageIndex ) );
        }

        public override IDataReader GetLog( string LogType, int PageSize, int PageIndex )
        {
            return ( SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", DBNull.Value, LogType, PageSize, PageIndex ) );
        }

        public override IDataReader GetLogTypeConfigInfo()
        {
            return ( SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogConfig", DBNull.Value ) );
        }

        public override IDataReader GetLogTypeConfigInfoByID( int ID )
        {
            return ( SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogConfig", ID ) );
        }

        public override IDataReader GetLogTypeInfo()
        {
            return ( SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogType", null ) );
        }

        public override void PurgeLog()
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "PurgeEventLog", null );
        }

        public override void AddLogType( string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddEventLogType", LogTypeKey, LogTypeFriendlyName, LogTypeDescription, LogTypeOwner, LogTypeCSSClass );
        }

        public override void UpdateLogType( string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateEventLogType", LogTypeKey, LogTypeFriendlyName, LogTypeDescription, LogTypeOwner, LogTypeCSSClass );
        }

        public override void DeleteLogType( string LogTypeKey )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteEventLogType", LogTypeKey );
        }

        public override IDataReader GetEventLogPendingNotifConfig()
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogPendingNotifConfig", null );
        }

        public override IDataReader GetEventLogPendingNotif( int LogConfigID )
        {
            return SqlHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogPendingNotif", LogConfigID );
        }

        public override void UpdateEventLogPendingNotif( int LogConfigID )
        {
            SqlHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateEventLogPendingNotif", LogConfigID );
        }
    }
}