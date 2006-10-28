using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Log.EventLog.DBLoggingProvider.Data;
using DotNetNuke.Services.Scheduling;

namespace DotNetNuke.Services.Log.EventLog.DBLoggingProvider
{
    public class DBLoggingProvider : LoggingProvider
    {
        private const int ReaderLockTimeout = 10000; //milliseconds
        private const int WriterLockTimeout = 10000; //milliseconds

        private static ReaderWriterLock lockNotif = new ReaderWriterLock();
        private static ReaderWriterLock lockPurgeLog = new ReaderWriterLock();
        public static ArrayList LogQueue = new ArrayList();

        private LogProperties Deserialize( string str )
        {
            Stream s;
            s = new MemoryStream( UnicodeEncoding.UTF8.GetBytes( Convert.ToString( str ) ) );
            //Dim l(0) As Type
            //l(0) = GetType(LogDetailInfo)

            XmlSerializer xser;

            //xser = New XmlSerializer(GetType(LogProperties), l)
            xser = new XmlSerializer( typeof( LogProperties ) );
            return ( (LogProperties)xser.Deserialize( s ) );
        }

        private LogInfo FillLogInfo( IDataReader dr )
        {
            LogInfo obj = new LogInfo();
            try
            {
                string LogGUID;
                LogGUID = Convert.ToString( dr["LogGUID"] );

                obj.LogCreateDate = Convert.ToDateTime( dr["LogCreateDate"] );
                obj.LogGUID = Convert.ToString( dr["LogGUID"] );
                if( dr["LogPortalID"] != DBNull.Value )
                {
                    obj.LogPortalID = Convert.ToInt32( dr["LogPortalID"] );
                }
                if( dr["LogPortalName"] != DBNull.Value )
                {
                    obj.LogPortalName = Convert.ToString( dr["LogPortalName"] );
                }
                if( dr["LogServerName"] != DBNull.Value )
                {
                    obj.LogServerName = Convert.ToString( dr["LogServerName"] );
                }
                if( dr["LogUserID"] != DBNull.Value )
                {
                    obj.LogUserID = Convert.ToInt32( dr["LogUserID"] );
                }
                obj.LogTypeKey = Convert.ToString( dr["LogTypeKey"] );
                obj.LogUserName = Convert.ToString( dr["LogUserName"] );
                obj.LogConfigID = Convert.ToString( dr["LogConfigID"] );
                obj.LogProperties = Deserialize( Convert.ToString( dr["LogProperties"] ) );
            }
            finally
            {
            }
            return obj;
        }

        private Hashtable FillLogTypeConfigInfoByKey( ArrayList arr )
        {
            Hashtable ht = new Hashtable();
            int i;
            for( i = 0; i <= arr.Count - 1; i++ )
            {
                LogTypeConfigInfo obj;
                obj = (LogTypeConfigInfo)arr[i];
                if( obj.LogTypeKey == "" )
                {
                    obj.LogTypeKey = "*";
                }
                if( obj.LogTypePortalID == "" )
                {
                    obj.LogTypePortalID = "*";
                }
                ht.Add( obj.LogTypeKey + "|" + obj.LogTypePortalID, obj );
            }
            DataCache.SetCache( "GetLogTypeConfigInfoByKey", ht );
            return ht;
        }

        public override LogInfoArray GetLog( int PageSize, int PageIndex, ref int TotalRecords )
        {
            LogInfoArray objArr = new LogInfoArray();
            IDataReader dr = DataProvider.Instance().GetLog( PageSize, PageIndex );
            try
            {
                LogInfo objLogInfo;
                while( dr.Read() )
                {
                    objLogInfo = FillLogInfo( dr );
                    objArr.Add( objLogInfo );
                }
                dr.NextResult();
                while( dr.Read() )
                {
                    TotalRecords = Convert.ToInt32( dr["TotalRecords"] );
                }
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
            return objArr;
        }

        public override LogInfoArray GetLog( string LogType, int PageSize, int PageIndex, ref int TotalRecords )
        {
            LogInfoArray objArr = new LogInfoArray();
            IDataReader dr = DataProvider.Instance().GetLog( LogType, PageSize, PageIndex );
            try
            {
                LogInfo objLogInfo;
                while( dr.Read() )
                {
                    objLogInfo = FillLogInfo( dr );
                    objArr.Add( objLogInfo );
                }
                dr.NextResult();
                while( dr.Read() )
                {
                    TotalRecords = Convert.ToInt32( dr["TotalRecords"] );
                }
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
            return objArr;
        }

        public override LogInfoArray GetLog( int PortalID, int PageSize, int PageIndex, ref int TotalRecords )
        {
            if( PortalID == - 1 )
            {
                return GetLog();
            }
            LogInfoArray objArr = new LogInfoArray();
            IDataReader dr = DataProvider.Instance().GetLog( PortalID, PageSize, PageIndex );
            try
            {
                LogInfo objLogInfo;
                while( dr.Read() )
                {
                    objLogInfo = FillLogInfo( dr );
                    objArr.Add( objLogInfo );
                }
                dr.NextResult();
                while( dr.Read() )
                {
                    TotalRecords = Convert.ToInt32( dr["TotalRecords"] );
                }
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
            return objArr;
        }

        public override LogInfoArray GetLog( int PortalID, string LogType, int PageSize, int PageIndex, ref int TotalRecords )
        {
            if( PortalID == - 1 )
            {
                return GetLog( LogType );
            }
            LogInfoArray objArr = new LogInfoArray();
            IDataReader dr = DataProvider.Instance().GetLog( PortalID, LogType, PageSize, PageIndex );
            try
            {
                LogInfo objLogInfo;
                while( dr.Read() )
                {
                    objLogInfo = FillLogInfo( dr );
                    objArr.Add( objLogInfo );
                }
                dr.NextResult();
                while( dr.Read() )
                {
                    TotalRecords = Convert.ToInt32( dr["TotalRecords"] );
                }
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
            return objArr;
        }

        public override LogInfoArray GetLog()
        {
            LogInfoArray objArr = new LogInfoArray();
            IDataReader dr = DataProvider.Instance().GetLog();
            try
            {
                LogInfo objLogInfo;
                while( dr.Read() )
                {
                    objLogInfo = FillLogInfo( dr );
                    objArr.Add( objLogInfo );
                }
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
            return objArr;
        }

        public override LogInfoArray GetLog( string LogType )
        {
            LogInfoArray objArr = new LogInfoArray();
            IDataReader dr = DataProvider.Instance().GetLog( LogType );
            try
            {
                LogInfo objLogInfo;
                while( dr.Read() )
                {
                    objLogInfo = FillLogInfo( dr );
                    objArr.Add( objLogInfo );
                }
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
            return objArr;
        }

        public override LogInfoArray GetLog( int PortalID )
        {
            if( PortalID == - 1 )
            {
                return GetLog();
            }
            LogInfoArray objArr = new LogInfoArray();
            IDataReader dr = DataProvider.Instance().GetLog( PortalID );
            try
            {
                LogInfo objLogInfo;
                while( dr.Read() )
                {
                    objLogInfo = FillLogInfo( dr );
                    objArr.Add( objLogInfo );
                }
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
            return objArr;
        }

        public override LogInfoArray GetLog( int PortalID, string LogType )
        {
            if( PortalID == - 1 )
            {
                return GetLog( LogType );
            }
            LogInfoArray objArr = new LogInfoArray();
            IDataReader dr = DataProvider.Instance().GetLog( PortalID, LogType );
            try
            {
                LogInfo objLogInfo;
                while( dr.Read() )
                {
                    objLogInfo = FillLogInfo( dr );
                    objArr.Add( objLogInfo );
                }
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
            return objArr;
        }

        public override ArrayList GetLogTypeConfigInfo()
        {
            ArrayList arr;
            arr = (ArrayList)DataCache.GetCache( "GetLogTypeConfigInfo" );
            if( arr == null )
            {
                IDataReader dr = DataProvider.Instance().GetLogTypeConfigInfo();
                if( dr == null )
                {
                    return new ArrayList();
                }
                arr = CBO.FillCollection( dr, typeof( LogTypeConfigInfo ) );
                DataCache.SetCache( "GetLogTypeConfigInfo", arr );
                FillLogTypeConfigInfoByKey( arr );
            }
            return arr;
        }

        public override LogTypeConfigInfo GetLogTypeConfigInfoByID( string ID )
        {
            return ( (LogTypeConfigInfo)CBO.FillObject( DataProvider.Instance().GetLogTypeConfigInfoByID( Convert.ToInt32( ID ) ), typeof( LogTypeConfigInfo ) ) );
        }

        private LogTypeConfigInfo GetLogTypeConfigInfoByKey( string LogTypeKey, string LogTypePortalID )
        {
            Hashtable ht;
            ht = (Hashtable)DataCache.GetCache( "GetLogTypeConfigInfoByKey" );
            if( ht == null )
            {
                ht = FillLogTypeConfigInfoByKey( GetLogTypeConfigInfo() );
            }
            LogTypeConfigInfo objLogTypeConfigInfo;
            objLogTypeConfigInfo = (LogTypeConfigInfo)ht[LogTypeKey + "|" + LogTypePortalID];
            if( objLogTypeConfigInfo == null )
            {
                objLogTypeConfigInfo = (LogTypeConfigInfo)ht["*|" + LogTypePortalID];
                if( objLogTypeConfigInfo == null )
                {
                    objLogTypeConfigInfo = (LogTypeConfigInfo)ht[LogTypeKey + "|*"];
                    if( objLogTypeConfigInfo == null )
                    {
                        objLogTypeConfigInfo = (LogTypeConfigInfo)ht["*|*"];
                    }
                    else
                    {
                        return objLogTypeConfigInfo;
                    }
                }
                else
                {
                    return objLogTypeConfigInfo;
                }
            }
            else
            {
                return objLogTypeConfigInfo;
            }

            return objLogTypeConfigInfo;
        }

        public override ArrayList GetLogTypeInfo()
        {
            return CBO.FillCollection( DataProvider.Instance().GetLogTypeInfo(), typeof( LogTypeInfo ) );
        }

        public override object GetSingleLog( LogInfo LogInfo, ReturnType objReturnType )
        {
            IDataReader dr = DataProvider.Instance().GetSingleLog( LogInfo.LogGUID );
            LogInfo obj = null;
            try
            {
                if( dr != null )
                {
                    dr.Read();
                    obj = FillLogInfo( dr );
                }
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
            if( objReturnType == ReturnType.LogInfoObjects )
            {
                return obj;
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml( XmlUtils.Serialize( obj ) );
                return ( (XmlNode)xmlDoc.DocumentElement );
            }
        }

        public override bool LoggingIsEnabled( string LogType, int PortalID )
        {
            string ConfigPortalID = PortalID.ToString();
            if( PortalID == - 1 )
            {
                ConfigPortalID = "*";
            }
            LogTypeConfigInfo obj;
            obj = GetLogTypeConfigInfoByKey( LogType, ConfigPortalID );
            if( obj == null )
            {
                return false;
            }
            return obj.LoggingIsActive;
        }

        private string Serialize( object obj )
        {
            //Dim l(0) As Type
            //l(0) = GetType(LogDetailInfo)
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xser;
            StringWriter sw;

            //xser = New XmlSerializer(obj.GetType, l)
            xser = new XmlSerializer( obj.GetType() );
            sw = new StringWriter();
            xser.Serialize( sw, obj );
            xmlDoc.LoadXml( sw.GetStringBuilder().ToString() );
            XmlNode xmlDocEl = xmlDoc.DocumentElement;
            return xmlDocEl.OuterXml;
        }

        public override bool SupportsEmailNotification()
        {
            return true;
        }

        public override bool SupportsInternalViewer()
        {
            return true;
        }

        public override bool SupportsSendToCoreTeam()
        {
            return false;
        }

        public override bool SupportsSendViaEmail()
        {
            return true;
        }

        public override void AddLog( LogInfo objLogInfo )
        {
            string ConfigPortalID;
            if( objLogInfo.LogPortalID != Null.NullInteger )
            {
                ConfigPortalID = objLogInfo.LogPortalID.ToString();
            }
            else
            {
                ConfigPortalID = "*";
            }

            LogTypeConfigInfo objLogTypeConfigInfo;
            objLogTypeConfigInfo = GetLogTypeConfigInfoByKey( objLogInfo.LogTypeKey, ConfigPortalID );

            if( objLogTypeConfigInfo.LoggingIsActive == false )
            {
                return;
            }

            bool UseEventLogBuffer = true;
            if( Globals.HostSettings.ContainsKey( "EventLogBuffer" ) )
            {
                if( Convert.ToString( Globals.HostSettings["EventLogBuffer"] ) != "Y" )
                {
                    UseEventLogBuffer = false;
                }
            }
            else
            {
                UseEventLogBuffer = false;
            }

            objLogInfo.LogConfigID = objLogTypeConfigInfo.ID;

            LogQueueItem objLogQueueItem = new LogQueueItem();
            objLogQueueItem.LogInfo = objLogInfo;
            objLogQueueItem.LogTypeConfigInfo = objLogTypeConfigInfo;

            SchedulingProvider scheduler = SchedulingProvider.Instance();
            if( objLogInfo.BypassBuffering || SchedulingProvider.Enabled == false || scheduler.GetScheduleStatus() == ScheduleStatus.STOPPED || UseEventLogBuffer == false )
            {
                WriteLog( objLogQueueItem );
            }
            else
            {
                LogQueue.Add( objLogQueueItem );
            }
        }

        public override void AddLogType( string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner )
        {
            DataProvider.Instance().AddLogType( LogTypeKey, LogTypeFriendlyName, LogTypeDescription, LogTypeCSSClass, LogTypeOwner );
        }

        public override void AddLogTypeConfigInfo( string ID, bool LoggingIsActive, string LogTypeKey, string LogTypePortalID, string KeepMostRecent, string LogFileName, bool EmailNotificationIsActive, string Threshold, string ThresholdTime, string ThresholdTimeType, string MailFromAddress, string MailToAddress )
        {
            int intThreshold = - 1;
            int intThresholdTime = - 1;
            int intThresholdTimeType = - 1;
            int intKeepMostRecent = - 1;

            int result;

            if( Int32.TryParse( Threshold, out result ) )
            {
                intThreshold = result;
            }
            if( Int32.TryParse( ThresholdTime, out result ) )
            {
                intThresholdTime = result;
            }
            if( Int32.TryParse( ThresholdTimeType, out result ) )
            {
                intThresholdTimeType = result;
            }
            if( Int32.TryParse( KeepMostRecent, out result ) )
            {
                intKeepMostRecent = result;
            }

            DataProvider.Instance().AddLogTypeConfigInfo( LoggingIsActive, LogTypeKey, LogTypePortalID, intKeepMostRecent, EmailNotificationIsActive, intThreshold, intThresholdTime, intThresholdTimeType, MailFromAddress, MailToAddress );
            DataCache.RemoveCache( "GetLogTypeConfigInfo" );
            DataCache.RemoveCache( "GetLogTypeConfigInfoByKey" );
        }

        public override void ClearLog()
        {
            DataProvider.Instance().ClearLog();
        }

        public override void DeleteLog( LogInfo LogInfo )
        {
            DataProvider.Instance().DeleteLog( LogInfo.LogGUID );
        }

        public override void DeleteLogType( string LogTypeKey )
        {
            DataProvider.Instance().DeleteLogType( LogTypeKey );
        }

        public override void DeleteLogTypeConfigInfo( string ID )
        {
            DataProvider.Instance().DeleteLogTypeConfigInfo( ID );
            DataCache.RemoveCache( "GetLogTypeConfigInfo" );
            DataCache.RemoveCache( "GetLogTypeConfigInfoByKey" );
        }

        public override void PurgeLogBuffer()
        {
            try
            {
                lockPurgeLog.AcquireWriterLock( WriterLockTimeout );
                int i;
                ArrayList c = LogQueue;
                int j = c.Count;
                for( i = 1; i <= c.Count; i++ )
                {
                    //in case the log was removed
                    //by another thread simultaneously
                    if( c[j] != null )
                    {
                        LogQueueItem objLogQueueItem;
                        objLogQueueItem = (LogQueueItem)( c[j] );
                        WriteLog( objLogQueueItem );
                    }
                    //in case the log was removed
                    //by another thread simultaneously
                    if( c[j] != null )
                    {
                        c.Remove( j );
                    }

                    //use "j" instead of "i" so we
                    //can iterate in reverse, taking
                    //items out of the rear of the
                    //collection
                    j--;
                }

                DataProvider.Instance().PurgeLog();
            }
            finally
            {
                lockPurgeLog.ReleaseWriterLock();
            }
        }

        public override void SendLogNotifications()
        {
            ArrayList arrLogConfig;

            arrLogConfig = CBO.FillCollection( DataProvider.Instance().GetEventLogPendingNotifConfig(), typeof( LogTypeConfigInfo ) );

            int i;
            for( i = 0; i <= arrLogConfig.Count - 1; i++ )
            {
                LogTypeConfigInfo objLogConfig;
                objLogConfig = (LogTypeConfigInfo)arrLogConfig[i];

                IDataReader dr = DataProvider.Instance().GetEventLogPendingNotif( Convert.ToInt32( objLogConfig.ID ) );
                string strLog = "";
                try
                {
                    while( dr.Read() )
                    {
                        LogInfo objLogInfo = this.FillLogInfo( dr );
                        strLog += Serialize( objLogInfo ) + "\r\n" + "\r\n";
                    }
                }
                finally
                {
                    if( dr != null )
                    {
                        dr.Close();
                    }
                }
                dr = null;
                Mail.Mail.SendMail( objLogConfig.MailFromAddress, objLogConfig.MailToAddress, "", "Event Notification", strLog, "", "", "", "", "", "" );
                DataProvider.Instance().UpdateEventLogPendingNotif( Convert.ToInt32( objLogConfig.ID ) );
            }
        }

        public override void UpdateLogType( string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner )
        {
            DataProvider.Instance().UpdateLogType( LogTypeKey, LogTypeFriendlyName, LogTypeDescription, LogTypeCSSClass, LogTypeOwner );
        }

        public override void UpdateLogTypeConfigInfo( string ID, bool LoggingIsActive, string LogTypeKey, string LogTypePortalID, string KeepMostRecent, string LogFileName, bool EmailNotificationIsActive, string Threshold, string ThresholdTime, string ThresholdTimeType, string MailFromAddress, string MailToAddress )
        {
            int intThreshold = - 1;
            int intThresholdTime = - 1;
            int intThresholdTimeType = - 1;
            int intKeepMostRecent = - 1;

            int result;

            if( Int32.TryParse( Threshold, out result ) )
            {
                intThreshold = result;
            }
            if( Int32.TryParse( ThresholdTime, out result ) )
            {
                intThresholdTime = result;
            }
            if( Int32.TryParse( ThresholdTimeType, out result ) )
            {
                intThresholdTimeType = result;
            }
            if( Int32.TryParse( KeepMostRecent, out result ) )
            {
                intKeepMostRecent = result;
            }

            DataProvider.Instance().UpdateLogTypeConfigInfo( ID, LoggingIsActive, LogTypeKey, LogTypePortalID, intKeepMostRecent, LogFileName, EmailNotificationIsActive, intThreshold, intThresholdTime, intThresholdTimeType, MailFromAddress, MailToAddress );
            DataCache.RemoveCache( "GetLogTypeConfigInfo" );
            DataCache.RemoveCache( "GetLogTypeConfigInfoByKey" );
        }

        private void WriteLog( LogQueueItem objLogQueueItem )
        {
            LogTypeConfigInfo objLogTypeConfigInfo = null;
            try
            {
                objLogTypeConfigInfo = objLogQueueItem.LogTypeConfigInfo;
                if( objLogTypeConfigInfo != null )
                {
                    LogInfo objLogInfo;
                    objLogInfo = objLogQueueItem.LogInfo;
                    string LogProperties;
                    LogProperties = Serialize( objLogInfo.LogProperties );
                    DataProvider.Instance().AddLog( objLogInfo.LogGUID, objLogInfo.LogTypeKey, objLogInfo.LogUserID, objLogInfo.LogUserName, objLogInfo.LogPortalID, objLogInfo.LogPortalName, objLogInfo.LogCreateDate, objLogInfo.LogServerName, LogProperties, Convert.ToInt32( objLogInfo.LogConfigID ) );

                    if( objLogTypeConfigInfo.EmailNotificationIsActive == true )
                    {
                        try
                        {
                            lockNotif.AcquireWriterLock( ReaderLockTimeout );

                            if( objLogTypeConfigInfo.NotificationThreshold == 0 )
                            {
                                string str;
                                str = XmlUtils.Serialize( objLogQueueItem.LogInfo );
                                Mail.Mail.SendMail( objLogTypeConfigInfo.MailFromAddress, objLogTypeConfigInfo.MailToAddress, "", "Event Notification", str, "", "", "", "", "", "" );
                            }
                            else if( objLogTypeConfigInfo.LogTypeKey != "LOG_NOTIFICATION_FAILURE" )
                            {
                                //pending log notifications go here
                            }
                        }
                        finally
                        {
                            lockNotif.ReleaseWriterLock();
                        }
                    }
                }

                if( objLogTypeConfigInfo.EmailNotificationIsActive == true )
                {
                    if( objLogTypeConfigInfo.NotificationThreshold == 0 )
                    {
                        //SendNotification(objLogTypeConfigInfo.MailFromAddress, objLogTypeConfigInfo.MailToAddress, "", "Event Notification", xmlDoc.InnerXml)
                    }
                    else if( objLogTypeConfigInfo.LogTypeKey != "LOG_NOTIFICATION_FAILURE" )
                    {
                    }
                }
            }
            catch( Exception exc )
            {
                if( HttpContext.Current != null )
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader( response, "Unhandled Error" );

                    string strMessage = exc.Message;
                    HtmlUtils.WriteError( response, objLogTypeConfigInfo.LogFileNameWithPath, strMessage );

                    HtmlUtils.WriteFooter( response );
                    response.End();
                }
            }
        }
    }
}