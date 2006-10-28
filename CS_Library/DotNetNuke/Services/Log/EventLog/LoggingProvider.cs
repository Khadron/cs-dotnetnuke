using System.Collections;
using DotNetNuke.Framework;

namespace DotNetNuke.Services.Log.EventLog
{
    public abstract class LoggingProvider
    {
        public enum ReturnType
        {
            LogInfoObjects,
            XML
        }

        // singleton reference to the instantiated object
        private static LoggingProvider objProvider = null;

        // constructor
        static LoggingProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (LoggingProvider)DotNetNuke.Framework.Reflection.CreateObject("logging");
        }

        // return the provider
        public new static LoggingProvider Instance()
        {
            return objProvider;
        }

        // methods to return functionality support indicators
        public abstract bool SupportsEmailNotification();
        public abstract bool SupportsInternalViewer();
        public abstract bool LoggingIsEnabled(string LogType, int PortalID);
        public abstract bool SupportsSendToCoreTeam();
        public abstract bool SupportsSendViaEmail();

        // method to add a log entry
        public abstract void AddLog(LogInfo LogInfo);
        public abstract void DeleteLog(LogInfo LogInfo);
        public abstract void ClearLog();
        public abstract void PurgeLogBuffer();
        public abstract void SendLogNotifications();

        // methods to get the log configuration info
        public abstract ArrayList GetLogTypeConfigInfo();
        public abstract LogTypeConfigInfo GetLogTypeConfigInfoByID(string ID);
        public abstract ArrayList GetLogTypeInfo();

        public abstract void AddLogTypeConfigInfo(string ID, bool LoggingIsActive, string LogTypeKey, string LogTypePortalID, string KeepMostRecent, string LogFileName, bool EmailNotificationIsActive, string Threshold, string NotificationThresholdTime, string NotificationThresholdTimeType, string MailFromAddress, string MailToAddress);
        public abstract void UpdateLogTypeConfigInfo(string ID, bool LoggingIsActive, string LogTypeKey, string LogTypePortalID, string KeepMostRecent, string LogFileName, bool EmailNotificationIsActive, string Threshold, string NotificationThresholdTime, string NotificationThresholdTimeType, string MailFromAddress, string MailToAddress);
        public abstract void DeleteLogTypeConfigInfo(string ID);

        public abstract void AddLogType(string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner);
        public abstract void UpdateLogType(string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner);
        public abstract void DeleteLogType(string LogTypeKey);

        // methods to get the log entries
        public abstract object GetSingleLog(LogInfo LogInfo, ReturnType objReturnType);
        public abstract LogInfoArray GetLog();
        public abstract LogInfoArray GetLog(string LogType);
        public abstract LogInfoArray GetLog(int PortalID);
        public abstract LogInfoArray GetLog(int PortalID, string LogType);

        public abstract LogInfoArray GetLog(int PageSize, int PageIndex, ref int TotalRecords);
        public abstract LogInfoArray GetLog(string LogType, int PageSize, int PageIndex, ref int TotalRecords);
        public abstract LogInfoArray GetLog(int PortalID, int PageSize, int PageIndex, ref int TotalRecords);
        public abstract LogInfoArray GetLog(int PortalID, string LogType, int PageSize, int PageIndex, ref int TotalRecords);
    }
}