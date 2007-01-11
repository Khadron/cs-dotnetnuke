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
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Services.Scheduling;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Services.Log.EventLog
{
    public class LogQueueItem
    {
        private string _LogString;
        private LogTypeConfigInfo _LogTypeConfigInfo;

        public string LogString
        {
            get
            {
                return this._LogString;
            }
            set
            {
                this._LogString = value;
            }
        }

        public LogTypeConfigInfo LogTypeConfigInfo
        {
            get
            {
                return this._LogTypeConfigInfo;
            }
            set
            {
                this._LogTypeConfigInfo = value;
            }
        }
    }

    public class XMLLoggingProvider : LoggingProvider
    {
        private const string PendingNotificationsFile = "PendingLogNotifications.xml.resources";
        private const string ProviderType = "logging";
        private const int ReaderLockTimeout = 10000; //milliseconds
        private const int WriterLockTimeout = 10000; //milliseconds
        private ProviderConfiguration _providerConfiguration;

        private static ReaderWriterLock lockLog = new ReaderWriterLock();
        private static ReaderWriterLock lockNotif = new ReaderWriterLock();
        private static ReaderWriterLock lockPurgeLog = new ReaderWriterLock();
        public static Collection LogQueue = new Collection();
//        public static ArrayList LogQueue = new ArrayList();
        private static XmlDocument xmlConfigDoc;

        public XMLLoggingProvider()
        {
            _providerConfiguration = ProviderConfiguration.GetProviderConfiguration( ProviderType );

            if( xmlConfigDoc == null )
            {
                xmlConfigDoc = GetConfigDoc();
            }
        }

        private long DateToNum( DateTime dt )
        {
            long i;
            i = Convert.ToInt64( dt.Year )*10000000000000;
            i += Convert.ToInt64( dt.Month )*100000000000;
            i += Convert.ToInt64( dt.Day )*1000000000;
            i += Convert.ToInt64( dt.Hour )*10000000;
            i += Convert.ToInt64( dt.Minute )*100000;
            i += Convert.ToInt64( dt.Second )*1000;
            i += Convert.ToInt64( dt.Millisecond )*1;
            return i;
        }

        private XmlDocument GetConfigDoc()
        {
            XmlDocument xmlConfigDoc = (XmlDocument)DataCache.GetCache( "LoggingGetConfigDoc" );
            if( xmlConfigDoc == null )
            {
                string strConfigDoc = GetConfigDocFileName();
                xmlConfigDoc = new XmlDocument();
                if( File.Exists( strConfigDoc ) == false )
                {
                    string TemplateLogConfig = Globals.HostMapPath + "Logs\\LogConfig\\LogConfigTemplate.xml.resources";
                    File.Copy( TemplateLogConfig, strConfigDoc );
                    File.SetAttributes( strConfigDoc, FileAttributes.Normal );
                }

                int intAttempts = 0;
                //wait for up to 100 milliseconds for the file
                //to be unlocked if it is not available
                while( !( xmlConfigDoc.OuterXml != "" || intAttempts == 100 ) )
                {
                    intAttempts++;
                    try
                    {
                        xmlConfigDoc.Load( strConfigDoc );
                    }
                    catch( IOException )
                    {
                        Thread.Sleep( 1 );
                    }
                }

                string filePath = GetConfigDocFileName();
                if( File.Exists( filePath ) )
                {
                    DataCache.SetCache( "LoggingGetConfigDoc", xmlConfigDoc, new CacheDependency( filePath ) );
                }
            }
            return xmlConfigDoc;
        }

        private string GetConfigDocFileName()
        {
            //--------------------------------------------------------------
            // Read the configuration specific information for this provider
            //--------------------------------------------------------------
            Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];
            return GetFilePath( objProvider.Attributes["configfilename"], "LogConfig\\" );
        }

        private string GetConfigProviderPath()
        {
            //--------------------------------------------------------------
            // Read the configuration specific information for this provider
            //--------------------------------------------------------------
            Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];
            return HttpContext.Current.Server.MapPath( objProvider.Attributes["providerPath"] );
        }

        private static string GetFilePath( string strFileName )
        {
            return GetFilePath( strFileName, "" );
        }

        private static string GetFilePath( string strFileName, string strFolder )
        {
            //--------------------------------------------------------------
            //check to see if they entered a filename or an absolute file path
            //--------------------------------------------------------------
            if( strFileName.LastIndexOf( "\\" ) == - 1 && strFileName.LastIndexOf( "/" ) == - 1 )
            {
                //--------------------------------------------------------------
                //Config settings specified a filename only, with no absolute path
                //Use the /Portals/_default/Logs directory to store the log file
                //This allows user to specify alternative location for
                //log files to be stored, othre than the DNN directory.
                //--------------------------------------------------------------
                HttpContext objHttpContext = HttpContext.Current;
                if( !String.IsNullOrEmpty(strFolder) )
                {
                    strFileName = Globals.HostMapPath + "Logs\\" + strFolder + strFileName;
                }
                else
                {
                    strFileName = Globals.HostMapPath + "Logs\\" + strFileName;
                }
            }
            return strFileName;
        }

        //--------------------------------------------------------------
        //Method to get all log entries for all portals & log types
        //--------------------------------------------------------------
        public override LogInfoArray GetLog()
        {
            int records = 0;
            return GetLogFromXPath( "logs/log", "*", "*", int.MaxValue, 1, ref records );
        }

        //--------------------------------------------------------------
        //Method to get all log entries for specified portal & all log types
        //--------------------------------------------------------------
        public override LogInfoArray GetLog( int PortalID )
        {
            if( PortalID == - 1 )
            {
                return GetLog();
            }
            else
            {
                int records = 0;
                return GetLogFromXPath( "logs/log[@LogPortalID='" + PortalID.ToString() + "']", PortalID.ToString(), "*", int.MaxValue, 1, ref records );
            }
        }

        //--------------------------------------------------------------
        //Method to get all log entries for all portals & specified log type
        //--------------------------------------------------------------
        public override LogInfoArray GetLog( string LogType )
        {
            int records = 0;
            return GetLogFromXPath( "logs/log[@LogTypeKey='" + LogType + "']", "*", LogType, int.MaxValue, 1, ref records );
        }

        //--------------------------------------------------------------
        //Method to get all log entries for specified portal & log type
        //--------------------------------------------------------------
        public override LogInfoArray GetLog( int PortalID, string LogTypeKey )
        {
            if( PortalID == - 1 )
            {
                int records = 0;
                return GetLog( LogTypeKey, int.MaxValue, 1, ref records );
            }
            else
            {
                int records = 0;
                return GetLogFromXPath( "logs/log[@LogPortalID='" + PortalID.ToString() + "' and @LogTypeKey='" + LogTypeKey + "']", PortalID.ToString(), LogTypeKey, int.MaxValue, 1, ref records );
            }
        }

        public override LogInfoArray GetLog( int PageSize, int PageIndex, ref int TotalRecords )
        {
            return GetLogFromXPath( "logs/log", "*", "*", PageSize, PageIndex, ref TotalRecords );
        }

        //--------------------------------------------------------------
        //Method to get all log entries for specified portal & all log types
        //--------------------------------------------------------------
        public override LogInfoArray GetLog( int PortalID, int PageSize, int PageIndex, ref int TotalRecords )
        {
            if( PortalID == - 1 )
            {
                return GetLog();
            }
            else
            {
                return GetLogFromXPath( "logs/log[@LogPortalID='" + PortalID.ToString() + "']", PortalID.ToString(), "*", PageSize, PageIndex, ref TotalRecords );
            }
        }

        //--------------------------------------------------------------
        //Method to get all log entries for all portals & specified log type
        //--------------------------------------------------------------
        public override LogInfoArray GetLog( string LogType, int PageSize, int PageIndex, ref int TotalRecords )
        {
            return GetLogFromXPath( "logs/log[@LogTypeKey='" + LogType + "']", "*", LogType, PageSize, PageIndex, ref TotalRecords );
        }

        //--------------------------------------------------------------
        //Method to get all log entries for specified portal & log type
        //--------------------------------------------------------------
        public override LogInfoArray GetLog( int PortalID, string LogTypeKey, int PageSize, int PageIndex, ref int TotalRecords )
        {
            if( PortalID == - 1 )
            {
                return GetLog( LogTypeKey, PageSize, PageIndex, ref TotalRecords );
            }
            else
            {
                return GetLogFromXPath( "logs/log[@LogPortalID='" + PortalID.ToString() + "' and @LogTypeKey='" + LogTypeKey + "']", PortalID.ToString(), LogTypeKey, PageSize, PageIndex, ref TotalRecords );
            }
        }

        private string GetLogFileByLogFileID( XmlDocument xmlConfigDoc, string LogFileID )
        {
            XmlNode xmlLogFile;
            xmlLogFile = xmlConfigDoc.SelectSingleNode( "/LogConfig/LogTypeConfig[@LogFileID='" + LogFileID + "']/@FileName" );

            if( xmlLogFile != null )
            {
                return GetFilePath( xmlLogFile.InnerText );
            }

            return "";
        }

        private ArrayList GetLogFiles( XmlDocument xmlConfigDoc )
        {
            return GetLogFiles( xmlConfigDoc, "*", "*" );
        }

        private ArrayList GetLogFiles( XmlDocument xmlConfigDoc, string ConfigPortalID )
        {
            return GetLogFiles( xmlConfigDoc, ConfigPortalID, "*" );
        }

        private ArrayList GetLogFiles( XmlDocument xmlConfigDoc, string ConfigPortalID, string LogTypeKey )
        {
            Hashtable ht = new Hashtable();
            ArrayList arrFiles = new ArrayList();
            //--------------------------------------------------------------
            //First see if there is a log file specified
            //for this log type and this PortalID
            //--------------------------------------------------------------
            XmlNodeList xmlLogFiles = null;

            if( ConfigPortalID == "*" && LogTypeKey == "*" )
            {
                xmlLogFiles = xmlConfigDoc.SelectNodes( "/LogConfig/LogTypeConfig/@FileName" );
            }
            else if( ConfigPortalID != "*" && LogTypeKey == "*" )
            {
                xmlLogFiles = xmlConfigDoc.SelectNodes( "/LogConfig/LogTypeConfig[@LogTypePortalID='" + ConfigPortalID + "']/@FileName" );
            }
            else if( ConfigPortalID == "*" && LogTypeKey != "*" )
            {
                xmlLogFiles = xmlConfigDoc.SelectNodes( "/LogConfig/LogTypeConfig[@LogTypeKey='" + LogTypeKey + "']/@FileName" );
            }
            else if( ConfigPortalID != "*" && LogTypeKey != "*" )
            {
                xmlLogFiles = xmlConfigDoc.SelectNodes( "/LogConfig/LogTypeConfig[@LogTypePortalID='" + ConfigPortalID + "' and @LogTypeKey='" + LogTypeKey + "']/@FileName" );
            }

            if( xmlLogFiles != null )
            {
                XmlNode xmlLogFile;
                foreach( XmlNode tempLoopVar_xmlLogFile in xmlLogFiles )
                {
                    xmlLogFile = tempLoopVar_xmlLogFile;
                    if( xmlLogFile.InnerText != "" && ht[xmlLogFile.InnerText] == null )
                    {
                        //dedupe
                        arrFiles.Add( GetFilePath( xmlLogFile.InnerText ) );
                        ht.Add( xmlLogFile.InnerText, true );
                    }
                }
            }
            //If arrFiles.Count > 0 Then
            //	Return arrFiles
            //End If

            //--------------------------------------------------------------
            //This is a catch all...it gets the default log file name.
            //--------------------------------------------------------------
            XmlNode xmlDefaultLogFile;
            xmlDefaultLogFile = xmlConfigDoc.SelectSingleNode( "/LogConfig/LogTypeConfig[@LogTypeKey='*' and @LogTypePortalID='*']/@FileName" );
            if( xmlDefaultLogFile != null && ht[xmlDefaultLogFile.InnerText] == null )
            {
                //dedupe
                arrFiles.Add( GetFilePath( xmlDefaultLogFile.InnerText ) );
                ht.Add( xmlDefaultLogFile.InnerText, true );
            }
            return arrFiles;
        }

        private LogInfoArray GetLogFromXPath( string xpath, string PortalID, string LogType, int PageSize, int PageIndex, ref int TotalRecords )
        {
            XmlDocument xmlConfigDoc = GetConfigDoc();
            ArrayList arrLogFiles = GetLogFiles( xmlConfigDoc, PortalID, LogType );

            XmlDocument xmlLogFiles = new XmlDocument();
            xmlLogFiles.LoadXml( "<LogCollection></LogCollection>" );

            XmlElement xmlLogFilesDocEl;
            xmlLogFilesDocEl = xmlLogFiles.DocumentElement;

            ArrayList arrLogInfo = new ArrayList();

            int i;
            for( i = 0; i <= arrLogFiles.Count - 1; i++ )
            {
                bool FileIsCorrupt = false;
                bool FileExists = true;
                string LogFile;
                LogFile = Convert.ToString( arrLogFiles[i] );
                XmlDocument xmlLogFile = new XmlDocument();
                try
                {
                    lockLog.AcquireReaderLock( ReaderLockTimeout );
                    xmlLogFile.Load( LogFile );
                }
                catch( FileNotFoundException )
                {
                    FileExists = false;
                    //file doesn't exist
                }
                catch( XmlException )
                {
                    FileExists = false;
                    FileIsCorrupt = true;
                    //file is corrupt
                }
                finally
                {
                    lockLog.ReleaseReaderLock();
                }
                if( FileIsCorrupt )
                {
                    string s = "A log file is corrupt '" + LogFile + "'.";
                    if( Strings.InStr( LogFile, "Exceptions.xml.resources", 0 ) > 0 )
                    {
                        s += "  This could be due to an older exception log file being written to by the new logging provider.  Try removing 'Exceptions.xml.resources' from the logs directory to solve the problem.";
                    }
                    LogInfo objEventLogInfo = new LogInfo();
                    objEventLogInfo.AddProperty( "Note", s );
                    objEventLogInfo.BypassBuffering = true;
                    objEventLogInfo.LogTypeKey = "HOST_ALERT";
                    EventLogController objEventLog = new EventLogController();
                    objEventLog.AddLog( objEventLogInfo );
                }
                else if( FileExists )
                {
                    XmlNodeList xmlNodes;
                    xmlNodes = xmlLogFile.SelectNodes( xpath );

                    XmlElement xmlLogNodes;
                    xmlLogNodes = xmlLogFiles.CreateElement( "logs" );

                    XmlNode xmlNode;
                    foreach( XmlNode tempLoopVar_xmlNode in xmlNodes )
                    {
                        xmlNode = tempLoopVar_xmlNode;
                        xmlLogNodes.AppendChild( xmlLogFiles.ImportNode( xmlNode, true ) );
                    }

                    xmlLogFilesDocEl.AppendChild( xmlLogNodes );
                }
            }

            return GetLogInfoFromXML( xmlLogFiles, PageSize, PageIndex, ref TotalRecords );
        }

        private LogInfoArray GetLogInfoFromXML( XmlDocument xmlLogFiles, int PageSize, int PageIndex, ref int TotalRecords )
        {
            //Create the Stream to place the output.
            Stream str = new MemoryStream();
            StreamWriter xw = new StreamWriter( str, Encoding.UTF8 );

            //Transform the file.
            XmlUtils.XSLTransform( xmlLogFiles, ref xw, GetConfigProviderPath() + "log.xslt" );

            //flush and set the position to 0
            xw.Flush();
            str.Position = 0;

            StreamReader x = new StreamReader( str );
            xmlLogFiles.Load( x );
            x.Close();

            TotalRecords = xmlLogFiles.DocumentElement.SelectNodes( "logs/log" ).Count;
            int TotalPages;
            TotalPages = Convert.ToInt32( Math.Ceiling( Convert.ToDouble( TotalRecords/PageSize ) ) );
            int LowNum;
            int HighNum;
            LowNum = PageIndex*PageSize;
            HighNum = ( PageIndex*PageSize ) + PageSize;
            if( HighNum > TotalRecords )
            {
                HighNum = TotalRecords;
            }

            LogInfoArray arrLog = new LogInfoArray();
            XmlNode XmlNode;

            foreach( XmlNode tempLoopVar_XmlNode in xmlLogFiles.DocumentElement.SelectNodes( "logs/log[position()>=" + LowNum.ToString() + " and position()<" + HighNum.ToString() + "]" ) )
            {
                XmlNode = tempLoopVar_XmlNode;
                LogInfo Log = new LogInfo();
                LogProperties logProp = new LogProperties();
                if( XmlNode.Attributes["LogTypeKey"] != null )
                {
                    Log.LogTypeKey = Convert.ToString( XmlNode.Attributes["LogTypeKey"].Value );
                }
                if( XmlNode.Attributes["LogCreateDate"] != null )
                {
                    Log.LogCreateDate = Convert.ToDateTime( XmlNode.Attributes["LogCreateDate"].Value );
                }
                if( XmlNode.Attributes["LogCreateDateNum"] != null )
                {
                    Log.LogCreateDateNum = long.Parse( XmlNode.Attributes["LogCreateDateNum"].Value );
                }
                if( XmlNode.Attributes["LogGUID"] != null )
                {
                    Log.LogGUID = Convert.ToString( XmlNode.Attributes["LogGUID"].Value );
                }
                if( XmlNode.Attributes["LogUserID"] != null )
                {
                    Log.LogUserID = Convert.ToInt32( XmlNode.Attributes["LogUserID"].Value );
                }
                if( XmlNode.Attributes["LogUserName"] != null )
                {
                    Log.LogUserName = Convert.ToString( XmlNode.Attributes["LogUserName"].Value );
                }
                if( XmlNode.Attributes["LogPortalID"] != null )
                {
                    Log.LogPortalID = Convert.ToInt32( XmlNode.Attributes["LogPortalID"].Value );
                }
                if( XmlNode.Attributes["LogPortalName"] != null )
                {
                    Log.LogPortalName = Convert.ToString( XmlNode.Attributes["LogPortalName"].Value );
                }
                if( XmlNode.Attributes["LogFileID"] != null )
                {
                    Log.LogFileID = Convert.ToString( XmlNode.Attributes["LogFileID"].Value );
                }
                if( XmlNode.Attributes["LogServerName"] != null )
                {
                    Log.LogServerName = Convert.ToString( XmlNode.Attributes["LogServerName"].Value );
                }

                XmlNodeList xmlPropertyNodes;
                xmlPropertyNodes = XmlNode.SelectNodes( "properties/property" );

                XmlNode PropertyNode;
                string propertyName;
                string propertyValue;
                foreach( XmlNode tempLoopVar_PropertyNode in xmlPropertyNodes )
                {
                    PropertyNode = tempLoopVar_PropertyNode;
                    LogDetailInfo logDetails = new LogDetailInfo();
                    propertyName = XmlUtils.GetNodeValue( PropertyNode, "name", "" );
                    if( propertyName == "logdetail" )
                    {
                        XmlDocument xmlDetail = new XmlDocument();
                        xmlDetail.LoadXml( XmlUtils.GetNodeValue( PropertyNode, "value", "" ) );
                        XmlNode childNode;
                        foreach( XmlNode tempLoopVar_childNode in xmlDetail.DocumentElement.ChildNodes )
                        {
                            childNode = tempLoopVar_childNode;
                            if( childNode.HasChildNodes == false )
                            {
                                propertyName = childNode.Name;
                                propertyValue = childNode.InnerText;
                                logProp.Add( new LogDetailInfo( propertyName, propertyValue ) );
                            }
                        }
                    }
                    else
                    {
                        propertyValue = XmlUtils.GetNodeValue( PropertyNode, "value", "" );
                        logProp.Add( new LogDetailInfo( propertyName, propertyValue ) );
                    }
                }
                Log.LogProperties = logProp;
                arrLog.Add( Log );
            }
            return arrLog;
        }

        private LogTypeConfigInfo GetLogTypeConfig( string ConfigPortalID, string LogTypeKey )
        {
            //--------------------------------------------------------------
            //First see if there is a log file specified
            //for this log type and this PortalID
            //--------------------------------------------------------------
            XmlNode xmlLogTypeInfo = xmlConfigDoc.SelectSingleNode( "/LogConfig/LogTypeConfig[@LogTypeKey='" + LogTypeKey + "' and @LogTypePortalID='" + ConfigPortalID + "']" );
            if( xmlLogTypeInfo != null )
            {
                return GetLogTypeConfigInfoFromXML( xmlLogTypeInfo );
            }

            //--------------------------------------------------------------
            //There's no log file specified for this
            //log type and PortalID, so check to see
            //if there is a log file specified for
            //all LogTypes for this PortalID
            //--------------------------------------------------------------
            xmlLogTypeInfo = xmlConfigDoc.SelectSingleNode( "/LogConfig/LogTypeConfig[@LogTypeKey='*' and @LogTypePortalID='" + ConfigPortalID + "']" );
            if( xmlLogTypeInfo != null )
            {
                return GetLogTypeConfigInfoFromXML( xmlLogTypeInfo );
            }

            //--------------------------------------------------------------
            //No logfile has been found yet, so let's
            //check if there is a logfile specified
            //for this log type and all PortalIDs
            //--------------------------------------------------------------
            xmlLogTypeInfo = xmlConfigDoc.SelectSingleNode( "/LogConfig/LogTypeConfig[@LogTypeKey='" + LogTypeKey + "' and @LogTypePortalID='*']" );
            if( xmlLogTypeInfo != null )
            {
                return GetLogTypeConfigInfoFromXML( xmlLogTypeInfo );
            }

            //--------------------------------------------------------------
            //This is a catch all...it gets the default log file name.
            //--------------------------------------------------------------
            xmlLogTypeInfo = xmlConfigDoc.SelectSingleNode( "/LogConfig/LogTypeConfig[@LogTypeKey='*' and @LogTypePortalID='*']" );
            if( xmlLogTypeInfo != null )
            {
                return GetLogTypeConfigInfoFromXML( xmlLogTypeInfo );
            }

            return null;
        }

        //--------------------------------------------------------------
        //Method to get Log Type Configuration
        //--------------------------------------------------------------
        public override ArrayList GetLogTypeConfigInfo()
        {
            XmlDocument xmlConfigDoc = GetConfigDoc();

            XmlNodeList xmlLogTypeConfigInfoList;
            xmlLogTypeConfigInfoList = xmlConfigDoc.SelectNodes( "/LogConfig/LogTypeConfig " );

            ArrayList arrLogTypeInfo = new ArrayList();

            XmlNode xmlLogTypeConfigInfo;
            foreach( XmlNode tempLoopVar_xmlLogTypeConfigInfo in xmlLogTypeConfigInfoList )
            {
                xmlLogTypeConfigInfo = tempLoopVar_xmlLogTypeConfigInfo;
                arrLogTypeInfo.Add( GetLogTypeConfigInfoFromXML( xmlLogTypeConfigInfo ) );
            }
            return arrLogTypeInfo;
        }

        //--------------------------------------------------------------
        //Method to get Log Type Configuration by ID
        //--------------------------------------------------------------
        public override LogTypeConfigInfo GetLogTypeConfigInfoByID( string ID )
        {
            XmlDocument xmlConfigDoc = GetConfigDoc();

            XmlNode xmlLogTypeConfigInfo;
            xmlLogTypeConfigInfo = xmlConfigDoc.SelectSingleNode( "/LogConfig/LogTypeConfig[@LogFileID='" + ID + "']" );

            return GetLogTypeConfigInfoFromXML( xmlLogTypeConfigInfo );
        }

        private LogTypeConfigInfo GetLogTypeConfigInfoFromXML( XmlNode xmlLogTypeInfo )
        {
            LogTypeConfigInfo objLogTypeConfigInfo = new LogTypeConfigInfo();
            objLogTypeConfigInfo.LogFileNameWithPath = GetFilePath( xmlLogTypeInfo.Attributes["FileName"].Value );
            objLogTypeConfigInfo.LogFileName = xmlLogTypeInfo.Attributes["FileName"].Value;
            objLogTypeConfigInfo.LogTypeKey = xmlLogTypeInfo.Attributes["LogTypeKey"].Value;
            if( xmlLogTypeInfo.Attributes["LogTypePortalID"] != null )
            {
                objLogTypeConfigInfo.LogTypePortalID = xmlLogTypeInfo.Attributes["LogTypePortalID"].Value;
            }
            else
            {
                objLogTypeConfigInfo.LogTypePortalID = "*";
            }
            if( xmlLogTypeInfo.Attributes["KeepMostRecent"] != null )
            {
                objLogTypeConfigInfo.KeepMostRecent = xmlLogTypeInfo.Attributes["KeepMostRecent"].Value;
            }
            else
            {
                objLogTypeConfigInfo.KeepMostRecent = "*";
            }
            objLogTypeConfigInfo.ID = xmlLogTypeInfo.Attributes["LogFileID"].Value;
            objLogTypeConfigInfo.LoggingIsActive = Convert.ToBoolean( ( Strings.LCase( xmlLogTypeInfo.Attributes["LoggingStatus"].Value.ToString() ) == "on" ) ? true : false );
            if( xmlLogTypeInfo.Attributes["EmailNotificationStatus"] == null )
            {
                objLogTypeConfigInfo.EmailNotificationIsActive = Convert.ToBoolean( ( Strings.LCase( xmlLogTypeInfo.Attributes["EmailNotificationStatus"].Value.ToString() ) == "on" ) ? true : false );
            }
            else
            {
                objLogTypeConfigInfo.EmailNotificationIsActive = false;
            }
            if( xmlLogTypeInfo.Attributes["MailFromAddress"] != null )
            {
                objLogTypeConfigInfo.MailFromAddress = Convert.ToString( xmlLogTypeInfo.Attributes["MailFromAddress"].Value );
            }
            else
            {
                objLogTypeConfigInfo.MailFromAddress = Null.NullString;
            }
            if( xmlLogTypeInfo.Attributes["MailToAddress"] != null )
            {
                objLogTypeConfigInfo.MailToAddress = Convert.ToString( xmlLogTypeInfo.Attributes["MailToAddress"].Value );
            }
            else
            {
                objLogTypeConfigInfo.MailToAddress = Null.NullString;
            }
            if( xmlLogTypeInfo.Attributes["NotificationThreshold"] != null )
            {
                objLogTypeConfigInfo.NotificationThreshold = Convert.ToInt32( xmlLogTypeInfo.Attributes["NotificationThreshold"].Value );
            }
            else
            {
                objLogTypeConfigInfo.NotificationThreshold = - 1;
            }
            if( xmlLogTypeInfo.Attributes["NotificationThresholdTime"] != null )
            {
                objLogTypeConfigInfo.NotificationThresholdTime = Convert.ToInt32( xmlLogTypeInfo.Attributes["NotificationThresholdTime"].Value );
            }
            else
            {
                objLogTypeConfigInfo.NotificationThresholdTime = - 1;
            }
            if( xmlLogTypeInfo.Attributes["NotificationThresholdTimeType"] != null )
            {
                string NotificationThresholdTimeType = Convert.ToString( xmlLogTypeInfo.Attributes["NotificationThresholdTimeType"].Value );
                switch( NotificationThresholdTimeType )
                {
                    case "1":

                        objLogTypeConfigInfo.NotificationThresholdTimeType = LogTypeConfigInfo.NotificationThresholdTimeTypes.Seconds;
                        break;
                    case "2":

                        objLogTypeConfigInfo.NotificationThresholdTimeType = LogTypeConfigInfo.NotificationThresholdTimeTypes.Minutes;
                        break;
                    case "3":

                        objLogTypeConfigInfo.NotificationThresholdTimeType = LogTypeConfigInfo.NotificationThresholdTimeTypes.Hours;
                        break;
                    case "4":

                        objLogTypeConfigInfo.NotificationThresholdTimeType = LogTypeConfigInfo.NotificationThresholdTimeTypes.Days;
                        break;
                    default:

                        objLogTypeConfigInfo.NotificationThresholdTimeType = LogTypeConfigInfo.NotificationThresholdTimeTypes.None;
                        break;
                }
            }
            else
            {
                objLogTypeConfigInfo.NotificationThresholdTimeType = LogTypeConfigInfo.NotificationThresholdTimeTypes.None;
            }

            return objLogTypeConfigInfo;
        }

        //--------------------------------------------------------------
        //Methods to get the log configuration info
        //--------------------------------------------------------------
        public override ArrayList GetLogTypeInfo()
        {
            XmlDocument xmlConfigDoc = GetConfigDoc();

            XmlNodeList xmlLogTypeInfoList;
            xmlLogTypeInfoList = xmlConfigDoc.SelectNodes( "/LogConfig/LogTypes/LogType" );

            ArrayList arrLogTypeInfo = new ArrayList();

            XmlNode xmlLogTypeInfo;
            foreach( XmlNode tempLoopVar_xmlLogTypeInfo in xmlLogTypeInfoList )
            {
                xmlLogTypeInfo = tempLoopVar_xmlLogTypeInfo;
                arrLogTypeInfo.Add( GetLogTypeInfoFromXML( xmlLogTypeInfo ) );
            }
            return arrLogTypeInfo;
        }

        private LogTypeInfo GetLogTypeInfoFromXML( XmlNode xmlLogTypeInfo )
        {
            LogTypeInfo objLogTypeInfo = new LogTypeInfo();
            objLogTypeInfo.LogTypeKey = xmlLogTypeInfo.Attributes["LogTypeKey"].Value;
            objLogTypeInfo.LogTypeFriendlyName = xmlLogTypeInfo.Attributes["LogTypeFriendlyName"].Value;
            objLogTypeInfo.LogTypeDescription = xmlLogTypeInfo.Attributes["LogTypeDescription"].Value;
            objLogTypeInfo.LogTypeOwner = xmlLogTypeInfo.Attributes["LogTypeOwner"].Value;
            objLogTypeInfo.LogTypeCSSClass = xmlLogTypeInfo.Attributes["LogTypeCSSClass"].Value;
            return objLogTypeInfo;
        }

        public override object GetSingleLog( LogInfo objLogInfo, ReturnType objReturnType )
        {
            XmlDocument xmlConfigDoc = GetConfigDoc();

            string strFileName = GetLogFileByLogFileID( GetConfigDoc(), objLogInfo.LogFileID );

            XmlDocument xmlDoc = new XmlDocument();
            int intAttempts = 0;
            //wait for up to 100 milliseconds for the file
            //to be unlocked if it is not available
            while( !( xmlDoc.OuterXml != "" || intAttempts == 100 ) )
            {
                intAttempts++;
                try
                {
                    xmlDoc.Load( strFileName );
                }
                catch( IOException )
                {
                    Thread.Sleep( 1 );
                }
            }

            XmlNode objxmlNode;
            objxmlNode = xmlDoc.SelectSingleNode( "/logs/log[@LogGUID='" + objLogInfo.LogGUID + "']" );

            XmlDocument xmlDocOut = new XmlDocument();
            xmlDocOut.LoadXml( "<SingleLog></SingleLog>" );
            XmlNode xmlNewNode;
            xmlNewNode = xmlDocOut.ImportNode( objxmlNode, true );
            xmlDocOut.DocumentElement.AppendChild( xmlNewNode );

            if( objReturnType == ReturnType.XML )
            {
                return xmlDocOut.DocumentElement.SelectSingleNode( "log" );
            }
            else
            {
                int records = 0;
                return GetLogInfoFromXML( xmlDocOut, int.MaxValue, 1, ref records ).GetItem( 0 );
            }
        }

        private XmlElement GetXMLFromLogTypeConfigInfo( string LogFileID, bool IsActive, string LogTypeKey, string LogTypePortalID, string KeepMostRecent, string LogFileName, bool EmailNotificationIsActive, string NotificationThreshold, string NotificationThresholdTime, string NotificationThresholdTimeType, string MailFromAddress, string MailToAddress )
        {
            string BlankLogConfig;
            BlankLogConfig = "	<LogTypeConfig LogFileID=\"\" LoggingStatus=\"\" LogTypeKey=\"\" LogTypePortalID=\"\" KeepMostRecent=\"\" FileName=\"\" EmailNotificationStatus=\"\" NotificationThreshold=\"\" NotificationThresholdTime=\"\" NotificationThresholdTimeType=\"\" MailFromAddress=\"\" MailToAddress=\"\"/>";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml( BlankLogConfig );
            XmlElement xmlDocEl;
            xmlDocEl = xmlDoc.DocumentElement;
            xmlDocEl.Attributes["LogFileID"].Value = LogFileID;
            xmlDocEl.Attributes["LoggingStatus"].Value = Convert.ToString( IsActive == true ? "On" : "Off" );
            xmlDocEl.Attributes["LogTypeKey"].Value = LogTypeKey;
            xmlDocEl.Attributes["LogTypePortalID"].Value = LogTypePortalID;
            xmlDocEl.Attributes["KeepMostRecent"].Value = KeepMostRecent;
            xmlDocEl.Attributes["FileName"].Value = LogFileName;
            xmlDocEl.Attributes["EmailNotificationStatus"].Value = Convert.ToString( EmailNotificationIsActive == true ? "On" : "Off" );
            xmlDocEl.Attributes["NotificationThreshold"].Value = NotificationThreshold;
            xmlDocEl.Attributes["NotificationThresholdTime"].Value = NotificationThresholdTime;
            xmlDocEl.Attributes["NotificationThresholdTimeType"].Value = NotificationThresholdTimeType;
            xmlDocEl.Attributes["MailFromAddress"].Value = MailFromAddress;
            xmlDocEl.Attributes["MailToAddress"].Value = MailToAddress;
            return xmlDocEl;
        }

        private XmlElement GetXMLFromLogTypeInfo( string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner )
        {
            string BlankLogConfig;
            BlankLogConfig = "<LogType LogTypeKey=\"\" LogTypeFriendlyName=\"\" LogTypeDescription=\"\" LogTypeOwner=\"\" LogTypeCSSClass=\"\"/>";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml( BlankLogConfig );
            XmlElement xmlDocEl;
            xmlDocEl = xmlDoc.DocumentElement;
            xmlDocEl.Attributes["LogTypeKey"].Value = LogTypeKey;
            xmlDocEl.Attributes["LogTypeFriendlyName"].Value = LogTypeFriendlyName;
            xmlDocEl.Attributes["LogTypeDescription"].Value = LogTypeDescription;
            xmlDocEl.Attributes["LogTypeOwner"].Value = LogTypeOwner;
            xmlDocEl.Attributes["LogTypeCSSClass"].Value = LogTypeCSSClass;
            return xmlDocEl;
        }

        //--------------------------------------------------------------
        //Method to see if logging is enabled for a log type & portal
        //--------------------------------------------------------------
        public override bool LoggingIsEnabled( string LogType, int PortalID )
        {
            LogTypeConfigInfo objLogTypeConfigInfo;
            string ConfigPortalID = PortalID.ToString();
            if( PortalID == - 1 )
            {
                ConfigPortalID = "*";
            }
            objLogTypeConfigInfo = GetLogTypeConfig( ConfigPortalID, LogType );
            if( objLogTypeConfigInfo != null )
            {
                if( objLogTypeConfigInfo.LoggingIsActive )
                {
                    return true;
                }
            }
            return false;
        }

        //--------------------------------------------------------------
        //Methods to return functionality support indicators
        //--------------------------------------------------------------
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
            return true;
        }

        public override bool SupportsSendViaEmail()
        {
            return true;
        }

        //--------------------------------------------------------------
        //Method to add a log entry
        //--------------------------------------------------------------
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
            objLogTypeConfigInfo = GetLogTypeConfig( ConfigPortalID, objLogInfo.LogTypeKey );
            if( objLogTypeConfigInfo != null && objLogTypeConfigInfo.LoggingIsActive )
            {
                string LogString;
                objLogInfo.LogFileID = objLogTypeConfigInfo.ID;
                objLogInfo.LogCreateDateNum = DateToNum( objLogInfo.LogCreateDate );
                LogString = XmlUtils.Serialize( objLogInfo );
                LogQueueItem objLogQueueItem = new LogQueueItem();
                objLogQueueItem.LogString = LogString;
                objLogQueueItem.LogTypeConfigInfo = objLogTypeConfigInfo;

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

                SchedulingProvider scheduler = SchedulingProvider.Instance();
                if( objLogInfo.BypassBuffering || SchedulingProvider.Enabled == false || scheduler.GetScheduleStatus() == ScheduleStatus.STOPPED || UseEventLogBuffer == false )
                {
                    WriteLog( objLogQueueItem );
                }
                else
                {
                    LogQueue.Add( objLogQueueItem, null, null, null );
                }
            }
        }

        public override void AddLogType( string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner )
        {
            XmlDocument xmlDoc = GetConfigDoc();
            XmlNode xmlNode;
            xmlNode = xmlDoc.ImportNode( GetXMLFromLogTypeInfo( LogTypeKey, LogTypeFriendlyName, LogTypeDescription, LogTypeCSSClass, LogTypeOwner ), true );
            xmlDoc.DocumentElement.AppendChild( xmlNode );
            xmlDoc.Save( GetConfigDocFileName() );
            DataCache.RemoveCache( "LoggingGetConfigDoc" );
            xmlConfigDoc = GetConfigDoc();
        }

        //--------------------------------------------------------------
        //Method to add Log Type Configuration
        //--------------------------------------------------------------
        public override void AddLogTypeConfigInfo( string ID, bool IsActive, string LogTypeKey, string LogTypePortalID, string KeepMostRecent, string LogFileName, bool EmailNotificationIsActive, string NotificationThreshold, string NotificationThresholdTime, string NotificationThresholdTimeType, string MailFromAddress, string MailToAddress )
        {
            XmlDocument xmlDoc = GetConfigDoc();
            XmlNode xmlNode;
            xmlNode = xmlDoc.ImportNode( GetXMLFromLogTypeConfigInfo( ID, IsActive, LogTypeKey, LogTypePortalID, KeepMostRecent, LogFileName, EmailNotificationIsActive, NotificationThreshold, NotificationThresholdTime, NotificationThresholdTimeType, MailFromAddress, MailToAddress ), true );
            xmlDoc.DocumentElement.AppendChild( xmlNode );
            xmlDoc.Save( GetConfigDocFileName() );
            DataCache.RemoveCache( "LoggingGetConfigDoc" );
            xmlConfigDoc = GetConfigDoc();
        }

        //--------------------------------------------------------------
        //Method to delete the log files
        //--------------------------------------------------------------
        public override void ClearLog()
        {
            XmlDocument xmlConfigDoc = GetConfigDoc();

            ArrayList a;
            a = GetLogFiles( xmlConfigDoc, "*", "*" );

            int i;
            for( i = 0; i <= a.Count - 1; i++ )
            {
                File.Delete( Convert.ToString( a[i] ) );
            }
        }

        //--------------------------------------------------------------
        //Method to delete a log entry
        //--------------------------------------------------------------
        public override void DeleteLog( LogInfo objLogInfo )
        {
            XmlDocument xmlConfigDoc = GetConfigDoc();

            string strFileName = GetLogFileByLogFileID( GetConfigDoc(), objLogInfo.LogFileID );

            XmlDocument xmlDoc = new XmlDocument();
            int intAttempts = 0;
            //wait for up to 100 milliseconds for the file
            //to be unlocked if it is not available
            while( !( xmlDoc.OuterXml != "" || intAttempts == 100 ) )
            {
                intAttempts++;
                try
                {
                    xmlDoc.Load( strFileName );
                }
                catch( IOException )
                {
                    Thread.Sleep( 1 );
                }
            }

            XmlNode xmlNode;
            xmlNode = xmlDoc.SelectSingleNode( "/logs/log[@LogGUID='" + objLogInfo.LogGUID + "']" );
            if( xmlNode != null )
            {
                xmlDoc.DocumentElement.RemoveChild( xmlNode );
            }
            xmlDoc.Save( strFileName );
        }

        public override void DeleteLogType( string LogTypeKey )
        {
            XmlDocument xmlDoc = GetConfigDoc();
            XmlNode xmlNode = xmlDoc.DocumentElement.SelectSingleNode( "LogTypes/LogType[@LogTypeKey='" + LogTypeKey + "']" );
            if( xmlNode != null )
            {
                xmlDoc.DocumentElement.RemoveChild( xmlNode );
                xmlDoc.Save( GetConfigDocFileName() );
            }
            DataCache.RemoveCache( "LoggingGetConfigDoc" );
            xmlConfigDoc = GetConfigDoc();
        }

        //--------------------------------------------------------------
        //Method to delete Log Type Configuration
        //--------------------------------------------------------------
        public override void DeleteLogTypeConfigInfo( string ID )
        {
            XmlDocument xmlDoc = GetConfigDoc();
            XmlNode xmlNode = xmlDoc.DocumentElement.SelectSingleNode( "LogTypeConfig[@LogFileID='" + ID + "']" );
            if( xmlNode != null )
            {
                xmlDoc.DocumentElement.RemoveChild( xmlNode );
                xmlDoc.Save( GetConfigDocFileName() );
            }
            DataCache.RemoveCache( "LoggingGetConfigDoc" );
            xmlConfigDoc = GetConfigDoc();
        }

        private void DeleteOldPendingNotifications()
        {
            XmlDocument xmlPendingNotificationsDoc = new XmlDocument();
            try
            {
                xmlPendingNotificationsDoc.Load( GetFilePath( PendingNotificationsFile ) );
            }
            catch( FileNotFoundException )
            {
                //file not found
                return;
            }
            //Check to see if we have had any
            //errors sending notifications.
            //If so, get out of this sub
            //so we don't delete any pending
            //notifications.  We only want
            //to delete old notifications
            //if the last notification succeeded.
            DateTime LastNotificationSuccess = Null.NullDate;
            DateTime LastNotificationFailure = Null.NullDate;
            if( xmlPendingNotificationsDoc.DocumentElement.Attributes["LastNotificationFailure"] != null )
            {
                LastNotificationFailure = Convert.ToDateTime( xmlPendingNotificationsDoc.DocumentElement.Attributes["LastNotificationFailure"].Value );
            }
            if( xmlPendingNotificationsDoc.DocumentElement.Attributes["LastNotificationSuccess"] != null )
            {
                LastNotificationSuccess = Convert.ToDateTime( xmlPendingNotificationsDoc.DocumentElement.Attributes["LastNotificationSuccess"].Value );
            }

            if( LastNotificationFailure > LastNotificationSuccess )
            {
                //the most recent notification cycle
                //failed, so we don't want to delete
                //any pending notifications
                return;
            }

            XmlDocument xmlConfigDoc = GetConfigDoc();
            ArrayList arrLogTypeInfo;
            arrLogTypeInfo = GetLogTypeConfigInfo();

            int a;
            for( a = 0; a <= arrLogTypeInfo.Count - 1; a++ )
            {
                LogTypeConfigInfo objLogTypeInfo;
                objLogTypeInfo = (LogTypeConfigInfo)arrLogTypeInfo[a];

                if( objLogTypeInfo.EmailNotificationIsActive )
                {
                    XmlNodeList xmlPendingNotifications = xmlPendingNotificationsDoc.DocumentElement.SelectNodes( "log[@LogTypeKey='" + objLogTypeInfo.LogTypeKey + "' and @LogTypePortalID='" + objLogTypeInfo.LogTypePortalID + "' and @CreateDateNum < '" + DateToNum( objLogTypeInfo.StartDateTime ).ToString() + "']" );

                    if( xmlPendingNotifications.Count > 0 )
                    {
                        //we have pending notifications to delete
                        //because time has elapsed putting
                        //them out of scope for the log type settings
                        XmlNode xmlPendingNotification;
                        foreach( XmlNode tempLoopVar_xmlPendingNotification in xmlPendingNotifications )
                        {
                            xmlPendingNotification = tempLoopVar_xmlPendingNotification;
                            //Remove the node from the list of pending notifications
                            xmlPendingNotificationsDoc.DocumentElement.RemoveChild( xmlPendingNotification );
                        }
                    }
                }
            }
            xmlPendingNotificationsDoc.Save( GetFilePath( PendingNotificationsFile ) );
        }

        public override void PurgeLogBuffer()
        {
            try
            {
                lockPurgeLog.AcquireWriterLock( WriterLockTimeout );
                int i;
                Collection c = LogQueue;
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

                XMLLoggingProvider objMe = new XMLLoggingProvider();
                ArrayList a = objMe.GetLogTypeConfigInfo();
                int k;
                for( k = 0; k <= a.Count - 1; k++ )
                {
                    LogTypeConfigInfo objLogTypeConfigInfo;
                    objLogTypeConfigInfo = (LogTypeConfigInfo)( a[k] );
                    //--------------------------------------------------------------
                    //if the KeepMostRecent setting has a numeric value,
                    //use it to limit the log file to "x" of the most recent
                    //logs, where "x" is the value of KeepMostRecent.
                    //A value of "*" signifies to keep all log entries.
                    //--------------------------------------------------------------
                    if( objLogTypeConfigInfo.KeepMostRecent != "*" && !String.IsNullOrEmpty(objLogTypeConfigInfo.LogFileName) )
                    {
                        XmlDocument xmlLog = new XmlDocument();
                        bool FileExists = false;
                        try
                        {
                            int intAttempts = 0;
                            //wait for up to 100 milliseconds for the file
                            //to be unlocked if it is not available
                            while( !( xmlLog.OuterXml != "" || intAttempts == 100 ) )
                            {
                                intAttempts++;
                                try
                                {
                                    xmlLog.Load( objLogTypeConfigInfo.LogFileNameWithPath );
                                    FileExists = true;
                                }
                                catch( IOException )
                                {
                                    Thread.Sleep( 1 );
                                }
                            }
                        }
                        catch( FileNotFoundException )
                        {
                            FileExists = false;
                            //file doesn't exist
                        }
                        catch( XmlException )
                        {
                            FileExists = false;
                            //file is corrupt
                        }
                        if( FileExists )
                        {
                            XmlNodeList objTotalNodes;
                            if( objLogTypeConfigInfo.LogTypePortalID == "*" )
                            {
                                objTotalNodes = xmlLog.DocumentElement.SelectNodes( "log[@LogTypeKey='" + objLogTypeConfigInfo.LogTypeKey + "']" );
                            }
                            else
                            {
                                objTotalNodes = xmlLog.DocumentElement.SelectNodes( "log[@LogTypeKey='" + objLogTypeConfigInfo.LogTypeKey + "' and LogPortalID='" + objLogTypeConfigInfo.LogTypePortalID + "']" );
                            }

                            int intNodeCount = objTotalNodes.Count;
                            int intKeepMostRecent = Convert.ToInt32( objLogTypeConfigInfo.KeepMostRecent );
                            if( intNodeCount > intKeepMostRecent )
                            {
                                XmlNode objTotalNode;
                                int m = 0;

                                foreach( XmlNode tempLoopVar_objTotalNode in objTotalNodes )
                                {
                                    objTotalNode = tempLoopVar_objTotalNode;
                                    if( intNodeCount - m > intKeepMostRecent )
                                    {
                                        xmlLog.DocumentElement.RemoveChild( objTotalNode );
                                    }
                                    m++;
                                }
                                xmlLog.Save( objLogTypeConfigInfo.LogFileNameWithPath );
                            }
                            else
                            {
                                xmlLog = null;
                            }
                        }
                    }
                }
            }
            finally
            {
                lockPurgeLog.ReleaseWriterLock();
            }
        }

        //--------------------------------------------------------------
        //Method to send email notifications
        //--------------------------------------------------------------
        public override void SendLogNotifications()
        {
            try
            {
                lockNotif.AcquireWriterLock( WriterLockTimeout );
                XmlDocument xmlPendingNotificationsDoc = new XmlDocument();
                try
                {
                    xmlPendingNotificationsDoc.Load( GetFilePath( PendingNotificationsFile ) );
                }
                catch( FileNotFoundException )
                {
                    //file not found
                    return;
                }

                ArrayList arrLogTypeInfo;
                XMLLoggingProvider x = new XMLLoggingProvider();
                arrLogTypeInfo = x.GetLogTypeConfigInfo();

                PurgeLogBuffer();

                int a;
                for( a = 0; a <= arrLogTypeInfo.Count - 1; a++ )
                {
                    LogTypeConfigInfo objLogTypeInfo;
                    objLogTypeInfo = (LogTypeConfigInfo)arrLogTypeInfo[a];

                    if( objLogTypeInfo.EmailNotificationIsActive )
                    {
                        XmlNodeList xmlPendingNotifications = xmlPendingNotificationsDoc.DocumentElement.SelectNodes( "log[@NotificationLogTypeKey='" + objLogTypeInfo.LogTypeKey + "' and @LogTypePortalID='" + objLogTypeInfo.LogTypePortalID + "' and number(@LogCreateDateNum) > " + DateToNum( objLogTypeInfo.StartDateTime ).ToString() + "]" );

                        if( xmlPendingNotifications.Count >= objLogTypeInfo.NotificationThreshold )
                        {
                            //we have notifications to send out
                            XmlNode xmlPendingNotification;
                            XmlDocument xmlOut = new XmlDocument();
                            xmlOut.LoadXml( "<notification></notification>" );
                            foreach( XmlNode tempLoopVar_xmlPendingNotification in xmlPendingNotifications )
                            {
                                xmlPendingNotification = tempLoopVar_xmlPendingNotification;

                                XmlNode tmpNode;
                                tmpNode = xmlOut.ImportNode( xmlPendingNotification, true );
                                xmlOut.DocumentElement.AppendChild( tmpNode );

                                //Remove the node from the list of pending notifications
                                xmlPendingNotificationsDoc.DocumentElement.RemoveChild( xmlPendingNotification );
                            }

                            bool NotificationFailed = false;
                            string errSendNotif;

                            errSendNotif = Mail.Mail.SendMail( objLogTypeInfo.MailFromAddress, objLogTypeInfo.MailToAddress, "", "Log Notification", xmlOut.OuterXml, "", "", "", "", "", "" );

                            if( !String.IsNullOrEmpty(errSendNotif) )
                            {
                                //notification failed to send
                                NotificationFailed = true;
                            }

                            EventLogController objEventLogController = new EventLogController();
                            if( NotificationFailed )
                            {
                                //Notification failed, log it
                                LogInfo objEventLogInfo = new LogInfo();
                                objEventLogInfo.LogTypeKey = EventLogController.EventLogType.LOG_NOTIFICATION_FAILURE.ToString();
                                objEventLogInfo.AddProperty( "Log Notification Failed: ", errSendNotif );
                                objEventLogController.AddLog( objEventLogInfo );

                                //need to reload the xml doc because
                                //we removed xml nodes above
                                xmlPendingNotificationsDoc.Load( GetFilePath( PendingNotificationsFile ) );

                                if( xmlPendingNotificationsDoc.DocumentElement.Attributes["LastNotificationFailure"] == null )
                                {
                                    XmlAttribute xmlNotificationFailed;
                                    xmlNotificationFailed = xmlPendingNotificationsDoc.CreateAttribute( "LastNotificationFailure" );
                                    xmlNotificationFailed.Value = DateTime.Now.ToString();
                                    xmlPendingNotificationsDoc.DocumentElement.Attributes.Append( xmlNotificationFailed );
                                }
                                else
                                {
                                    xmlPendingNotificationsDoc.DocumentElement.Attributes["LastNotificationFailure"].Value = DateTime.Now.ToString();
                                }
                                xmlPendingNotificationsDoc.Save( GetFilePath( PendingNotificationsFile ) );
                            }
                            else
                            {
                                //Notification succeeded.
                                //Save the updated pending notifications file
                                //so we remove the notifications that have been completed.
                                if( xmlPendingNotificationsDoc.DocumentElement.Attributes["LastNotificationFailure"] != null )
                                {
                                    xmlPendingNotificationsDoc.DocumentElement.Attributes.Remove( xmlPendingNotificationsDoc.DocumentElement.Attributes["LastNotificationFailure"] );
                                }

                                if( xmlPendingNotificationsDoc.DocumentElement.Attributes["LastNotificationSuccess"] == null )
                                {
                                    XmlAttribute xmlNotificationSucceeded;
                                    xmlNotificationSucceeded = xmlPendingNotificationsDoc.CreateAttribute( "LastNotificationSuccess" );
                                    xmlNotificationSucceeded.Value = DateTime.Now.ToString();
                                    xmlPendingNotificationsDoc.DocumentElement.Attributes.Append( xmlNotificationSucceeded );
                                }
                                else
                                {
                                    xmlPendingNotificationsDoc.DocumentElement.Attributes["LastNotificationSuccess"].Value = DateTime.Now.ToString();
                                }
                                xmlPendingNotificationsDoc.Save( GetFilePath( PendingNotificationsFile ) );
                            }
                        }
                    }
                }

                x.DeleteOldPendingNotifications();
            }
            catch( Exception exc )
            {
                Exceptions.Exceptions.LogException( exc );
            }
            finally
            {
                lockNotif.ReleaseWriterLock();
            }
        }

        public override void UpdateLogType( string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner )
        {
            DeleteLogType( LogTypeKey );
            AddLogType( LogTypeKey, LogTypeFriendlyName, LogTypeDescription, LogTypeCSSClass, LogTypeOwner );
        }

        //--------------------------------------------------------------
        //Method to update Log Type Configuration
        //--------------------------------------------------------------
        public override void UpdateLogTypeConfigInfo( string ID, bool IsActive, string LogTypeKey, string LogTypePortalID, string KeepMostRecent, string LogFileName, bool EmailNotificationIsActive, string NotificationThreshold, string NotificationThresholdTime, string NotificationThresholdTimeType, string MailFromAddress, string MailToAddress )
        {
            DeleteLogTypeConfigInfo( ID );
            AddLogTypeConfigInfo( ID, IsActive, LogTypeKey, LogTypePortalID, KeepMostRecent, LogFileName, EmailNotificationIsActive, NotificationThreshold, NotificationThresholdTime, NotificationThresholdTimeType, MailFromAddress, MailToAddress );
        }

        private void WriteLog( LogQueueItem objLogQueueItem )
        {
            //--------------------------------------------------------------
            //Write the log entry
            //--------------------------------------------------------------
            FileStream fs = null;
            StreamWriter sw = null;

            LogTypeConfigInfo objLogTypeConfigInfo = objLogQueueItem.LogTypeConfigInfo;
            string LogString = objLogQueueItem.LogString;

            try
            {
                if( !String.IsNullOrEmpty(objLogTypeConfigInfo.LogFileNameWithPath) )
                {
                    //--------------------------------------------------------------
                    // Write the entry to the log.
                    //--------------------------------------------------------------
                    lockLog.AcquireWriterLock( WriterLockTimeout );
                    int intAttempts = 0;
                    //wait for up to 100 milliseconds for the file
                    //to be unlocked if it is not available
                    while( !( fs != null || intAttempts == 100 ) )
                    {
                        intAttempts++;
                        try
                        {
                            fs = new FileStream( objLogTypeConfigInfo.LogFileNameWithPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None );
                        }
                        catch( IOException )
                        {
                            Thread.Sleep( 1 );
                        }
                    }

                    if( fs == null )
                    {
                        if( HttpContext.Current != null )
                        {
                            HttpContext.Current.Response.Write( "An error has occurred writing to the exception log." );
                            HttpContext.Current.Response.End();
                        }
                    }
                    else
                    {
                        //--------------------------------------------------------------
                        //Instantiate a new StreamWriter
                        //--------------------------------------------------------------
                        sw = new StreamWriter( fs, Encoding.UTF8 );
                        long FileLength;
                        FileLength = fs.Length;
                        //--------------------------------------------------------------
                        //check to see if this file is new
                        //--------------------------------------------------------------
                        if( FileLength > 0 )
                        {
                            //--------------------------------------------------------------
                            //file is not new, set the position to just before
                            //the closing root element tag
                            //--------------------------------------------------------------
                            fs.Position = FileLength - 9;
                        }
                        else
                        {
                            //--------------------------------------------------------------
                            //file is new, create the opening root element tag
                            //--------------------------------------------------------------
                            LogString = "<logs>" + LogString;
                        }

                        //--------------------------------------------------------------
                        //write out our exception
                        //--------------------------------------------------------------
                        sw.WriteLine( LogString + "</logs>" );
                        sw.Flush();
                    }
                    if( sw != null )
                    {
                        sw.Close();
                    }
                    if( fs != null )
                    {
                        fs.Close();
                    }
                }
                if( objLogTypeConfigInfo.EmailNotificationIsActive == true )
                {
                    try
                    {
                        lockNotif.AcquireWriterLock( ReaderLockTimeout );

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml( LogString );

                        //If the threshold for email notifications is
                        //set to 0, send an email notification each
                        //time a log entry is written.

                        if( objLogTypeConfigInfo.NotificationThreshold == 0 )
                        {
                            Mail.Mail.SendMail( objLogTypeConfigInfo.MailFromAddress, objLogTypeConfigInfo.MailToAddress, "", "Event Notification", xmlDoc.InnerXml, "", "", "", "", "", "" );
                        }
                        else if( objLogTypeConfigInfo.LogTypeKey != "LOG_NOTIFICATION_FAILURE" )
                        {
                            XmlDocument xmlPendingNotificationsDoc = new XmlDocument();
                            try
                            {
                                xmlPendingNotificationsDoc.Load( GetFilePath( PendingNotificationsFile ) );
                            }
                            catch( FileNotFoundException )
                            {
                                //file not created yet
                                xmlPendingNotificationsDoc.LoadXml( "<PendingNotifications></PendingNotifications>" );
                            }
                            XmlNode xmlLogNode;
                            xmlLogNode = xmlPendingNotificationsDoc.ImportNode( xmlDoc.FirstChild, true );

                            XmlAttribute xmlAttrib;
                            xmlAttrib = xmlPendingNotificationsDoc.CreateAttribute( "MailFromAddress" );
                            xmlAttrib.Value = Convert.ToString( objLogTypeConfigInfo.MailFromAddress );
                            xmlLogNode.Attributes.Append( xmlAttrib );

                            xmlAttrib = xmlPendingNotificationsDoc.CreateAttribute( "NotificationLogTypeKey" );
                            xmlAttrib.Value = Convert.ToString( objLogTypeConfigInfo.LogTypeKey );
                            xmlLogNode.Attributes.Append( xmlAttrib );

                            xmlAttrib = xmlPendingNotificationsDoc.CreateAttribute( "LogTypePortalID" );
                            if( objLogTypeConfigInfo.LogTypePortalID == "-1" )
                            {
                                xmlAttrib.Value = "*";
                            }
                            else
                            {
                                xmlAttrib.Value = objLogTypeConfigInfo.LogTypePortalID;
                            }
                            xmlLogNode.Attributes.Append( xmlAttrib );

                            XmlElement x;
                            x = xmlPendingNotificationsDoc.CreateElement( "EmailAddress" );
                            x.InnerText = Convert.ToString( objLogTypeConfigInfo.MailToAddress );
                            xmlLogNode.AppendChild( x );

                            xmlPendingNotificationsDoc.DocumentElement.AppendChild( xmlLogNode );
                            xmlPendingNotificationsDoc.Save( GetFilePath( PendingNotificationsFile ) );
                        }
                    }
                    finally
                    {
                        lockNotif.ReleaseWriterLock();
                    }
                }
                //--------------------------------------------------------------
                //handle the more common exceptions up
                //front, leave less common ones to the end
                //--------------------------------------------------------------
            }
            catch( UnauthorizedAccessException exc )
            {
                if( HttpContext.Current != null )
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader( response, "Unauthorized Access Error" );

                    string strMessage = exc.Message + " The Windows User Account listed below must have Read/Write Privileges to this path.";
                    HtmlUtils.WriteError( response, objLogTypeConfigInfo.LogFileNameWithPath, strMessage );

                    HtmlUtils.WriteFooter( response );
                    response.End();
                }
            }
            catch( DirectoryNotFoundException exc )
            {
                if( HttpContext.Current != null )
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader( response, "Directory Not Found Error" );

                    string strMessage = exc.Message;
                    HtmlUtils.WriteError( response, objLogTypeConfigInfo.LogFileNameWithPath, strMessage );

                    HtmlUtils.WriteFooter( response );
                    response.End();
                }
            }
            catch( PathTooLongException exc )
            {
                if( HttpContext.Current != null )
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader( response, "Path Too Long Error" );

                    string strMessage = exc.Message;
                    HtmlUtils.WriteError( response, objLogTypeConfigInfo.LogFileNameWithPath, strMessage );

                    HtmlUtils.WriteFooter( response );
                    response.End();
                }
            }
            catch( IOException exc )
            {
                if( HttpContext.Current != null )
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader( response, "IO Error" );

                    string strMessage = exc.Message;
                    HtmlUtils.WriteError( response, objLogTypeConfigInfo.LogFileNameWithPath, strMessage );

                    HtmlUtils.WriteFooter( response );
                    response.End();
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
            finally
            {
                if( sw != null )
                {
                    sw.Close();
                }
                if( fs != null )
                {
                    fs.Close();
                }

                lockLog.ReleaseWriterLock();
            }
        }
    }
}