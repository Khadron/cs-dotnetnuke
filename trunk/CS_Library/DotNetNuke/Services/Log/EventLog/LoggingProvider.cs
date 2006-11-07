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