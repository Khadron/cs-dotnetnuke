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

namespace DotNetNuke.Services.Log.EventLog.DBLoggingProvider.Data
{
    public abstract class DataProvider
    {
        // provider constants - eliminates need for Reflection later
        private const string ProviderType = "data"; // maps to <sectionGroup> in web.config
        private const string ProviderNamespace = "DotNetNuke.Services.Log.EventLog.DBLoggingProvider.Data"; // project namespace
        private const string ProviderAssemblyName = "DotNetNuke.Provider.DBLoggingProvider"; // project assemblyname

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
            objProvider = (DataProvider)Framework.Reflection.CreateObject( ProviderType, ProviderNamespace, ProviderAssemblyName );
        }

        // return the provider
        public static DataProvider Instance()
        {
            return objProvider;
        }

        //---------------------------------------------------------------------
        // TODO Declare DAL methods. Should be implemented in each DAL DataProvider
        // Use CodeSmith templates to generate this code
        //---------------------------------------------------------------------

        public abstract void AddLog( string LogGUID, string LogTypeKey, int LogUserID, string LogUserName, int LogPortalID, string LogPortalName, DateTime LogCreateDate, string LogServerName, string LogProperties, int LogConfigID );
        public abstract void DeleteLog( string LogGUID );
        public abstract void PurgeLog();
        public abstract void ClearLog();

        public abstract void AddLogTypeConfigInfo( bool LoggingIsActive, string LogTypeKey, string LogTypePortalID, int KeepMostRecent, bool EmailNotificationIsActive, int Threshold, int NotificationThresholdTime, int NotificationThresholdTimeType, string MailFromAddress, string MailToAddress );
        public abstract void DeleteLogTypeConfigInfo( string ID );
        public abstract IDataReader GetLogTypeConfigInfo();
        public abstract IDataReader GetLogTypeConfigInfoByID( int ID );
        public abstract void UpdateLogTypeConfigInfo( string ID, bool LoggingIsActive, string LogTypeKey, string LogTypePortalID, int KeepMostRecent, string LogFileName, bool EmailNotificationIsActive, int Threshold, int NotificationThresholdTime, int NotificationThresholdTimeType, string MailFromAddress, string MailToAddress );

        public abstract void AddLogType( string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner );
        public abstract void UpdateLogType( string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner );
        public abstract void DeleteLogType( string LogTypeKey );
        public abstract IDataReader GetLogTypeInfo();

        public abstract IDataReader GetLog();
        public abstract IDataReader GetLog( int PortalID );
        public abstract IDataReader GetLog( int PortalID, string LogType );
        public abstract IDataReader GetLog( string LogType );

        public abstract IDataReader GetLog( int PageSize, int PageIndex );
        public abstract IDataReader GetLog( int PortalID, int PageSize, int PageIndex );
        public abstract IDataReader GetLog( int PortalID, string LogType, int PageSize, int PageIndex );
        public abstract IDataReader GetLog( string LogType, int PageSize, int PageIndex );

        public abstract IDataReader GetSingleLog( string LogGUID );
        public abstract IDataReader GetEventLogPendingNotifConfig();
        public abstract IDataReader GetEventLogPendingNotif( int LogConfigID );
        public abstract void UpdateEventLogPendingNotif( int LogConfigID );
    }
}