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
using System.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using Microsoft.ApplicationBlocks.Data;

namespace DotNetNuke.Services.Log.EventLog.DBLoggingProvider.Data
{
    public class OracleDataProvider : DataProvider
    {
        private const string ProviderType = "data";
        private ProviderConfiguration providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
        private string connectionString;
        private string providerPath;
        private string objectQualifier;
        private string databaseOwner;

        public OracleDataProvider()
        {
            // Read the configuration specific information for this provider
            Provider objProvider = (Provider)providerConfiguration.Providers[providerConfiguration.DefaultProvider];

            // Read the attributes for this provider
            //Get Connection string from web.config
            connectionString = Config.GetConnectionString();

            if( connectionString == "" )
            {
                // Use connection string specified in provider
                connectionString = objProvider.Attributes["connectionString"];
            }

            providerPath = objProvider.Attributes["providerPath"];

            objectQualifier = objProvider.Attributes["objectQualifier"];
            if( !String.IsNullOrEmpty(objectQualifier) && objectQualifier.EndsWith( "_" ) == false )
            {
                objectQualifier += "_";
            }

            databaseOwner = objProvider.Attributes["databaseOwner"];
            if( !String.IsNullOrEmpty(databaseOwner) && databaseOwner.EndsWith( "." ) == false )
            {
                databaseOwner += ".";
            }
        }

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
        }

        public string ProviderPath
        {
            get
            {
                return providerPath;
            }
        }

        public string ObjectQualifier
        {
            get
            {
                return objectQualifier;
            }
        }

        public string DatabaseOwner
        {
            get
            {
                return databaseOwner;
            }
        }

        private object GetNull( object Field )
        {
            return Null.GetNull( Field, DBNull.Value );
        }

        public override void AddLog( string LogGUID, string LogTypeKey, int LogUserID, string LogUserName, int LogPortalID, string LogPortalName, DateTime LogCreateDate, string LogServerName, string LogProperties, int LogConfigID )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddEventLog", LogGUID, LogTypeKey, GetNull( LogUserID ), GetNull( LogUserName ), GetNull( LogPortalID ), GetNull( LogPortalName ), LogCreateDate, LogServerName, LogProperties, LogConfigID );
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
            OracleHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddEventLogConfig", GetNull( LogTypeKey ), GetNull( PortalID ), LoggingIsActive, KeepMostRecent, EmailNotificationIsActive, GetNull( Threshold ), GetNull( NotificationThresholdTime ), GetNull( NotificationThresholdTimeType ), MailFromAddress, MailToAddress );
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
            OracleHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateEventLogConfig", ID, GetNull( LogTypeKey ), GetNull( PortalID ), LoggingIsActive, KeepMostRecent, EmailNotificationIsActive, GetNull( Threshold ), GetNull( NotificationThresholdTime ), GetNull( NotificationThresholdTimeType ), MailFromAddress, MailToAddress );
        }

        public override void ClearLog()
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteEventLog", DBNull.Value );
        }

        public override void DeleteLog( string LogGUID )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteEventLog", LogGUID );
        }

        public override void DeleteLogTypeConfigInfo( string ID )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteEventLogConfig", ID );
        }

        public override IDataReader GetLog()
        {
            return ( OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", DBNull.Value, DBNull.Value ) );
        }

        public override IDataReader GetSingleLog( string LogGUID )
        {
            return ( OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogByLogGUID", LogGUID ) );
        }

        public override IDataReader GetLog( int PortalID )
        {
            return ( OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", PortalID, DBNull.Value ) );
        }

        public override IDataReader GetLog( int PortalID, string LogType )
        {
            return ( OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", PortalID, LogType ) );
        }

        public override IDataReader GetLog( string LogType )
        {
            return ( OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", DBNull.Value, LogType ) );
        }

        public override IDataReader GetLog( int PageSize, int PageIndex )
        {
            return ( OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", DBNull.Value, DBNull.Value, PageSize, PageIndex ) );
        }

        public override IDataReader GetLog( int PortalID, int PageSize, int PageIndex )
        {
            return ( OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", PortalID, DBNull.Value, PageSize, PageIndex ) );
        }

        public override IDataReader GetLog( int PortalID, string LogType, int PageSize, int PageIndex )
        {
            return ( OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", PortalID, LogType, PageSize, PageIndex ) );
        }

        public override IDataReader GetLog( string LogType, int PageSize, int PageIndex )
        {
            return ( OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", DBNull.Value, LogType, PageSize, PageIndex ) );
        }

        public override IDataReader GetLogTypeConfigInfo()
        {
            return ( OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogConfig", DBNull.Value ) );
        }

        public override IDataReader GetLogTypeConfigInfoByID( int ID )
        {
            return ( OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogConfig", ID ) );
        }

        public override IDataReader GetLogTypeInfo()
        {
            return ( OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogType", null ) );
        }

        public override void PurgeLog()
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "PurgeEventLog", null );
        }

        public override void AddLogType( string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "AddEventLogType", LogTypeKey, LogTypeFriendlyName, LogTypeDescription, LogTypeOwner, LogTypeCSSClass );
        }

        public override void UpdateLogType( string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateEventLogType", LogTypeKey, LogTypeFriendlyName, LogTypeDescription, LogTypeOwner, LogTypeCSSClass );
        }

        public override void DeleteLogType( string LogTypeKey )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteEventLogType", LogTypeKey );
        }

        public override IDataReader GetEventLogPendingNotifConfig()
        {
            return OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogPendingNotifConfig", null );
        }

        public override IDataReader GetEventLogPendingNotif( int LogConfigID )
        {
            return OracleHelper.ExecuteReader( ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogPendingNotif", LogConfigID );
        }

        public override void UpdateEventLogPendingNotif( int LogConfigID )
        {
            OracleHelper.ExecuteNonQuery( ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateEventLogPendingNotif", LogConfigID );
        }
    }
}