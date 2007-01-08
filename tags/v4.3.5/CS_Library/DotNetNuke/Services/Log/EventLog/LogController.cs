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
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Services.Log.EventLog
{
    public class LogController
    {
        private const int ReaderLockTimeout = 10000; //milliseconds
        private const int WriterLockTimeout = 10000; //milliseconds
        private static ReaderWriterLock lockLog = new ReaderWriterLock();

        [Obsolete("This method has been replaced with one that supports record paging.")]
        public virtual LogInfoArray GetLog()
        {
            return LoggingProvider.Instance().GetLog();
        }

        [Obsolete("This method has been replaced with one that supports record paging.")]
        public virtual LogInfoArray GetLog(int PortalID)
        {
            return LoggingProvider.Instance().GetLog(PortalID);
        }

        [Obsolete("This method has been replaced with one that supports record paging.")]
        public virtual LogInfoArray GetLog(int PortalID, string LogType)
        {
            return LoggingProvider.Instance().GetLog(PortalID, LogType);
        }

        [Obsolete("This method has been replaced with one that supports record paging.")]
        public virtual LogInfoArray GetLog(string LogType)
        {
            return LoggingProvider.Instance().GetLog(LogType);
        }

        public virtual LogInfoArray GetLog(int PageSize, int PageIndex, ref int TotalRecords)
        {
            return LoggingProvider.Instance().GetLog(PageSize, PageIndex, ref TotalRecords);
        }

        public virtual LogInfoArray GetLog(int PortalID, int PageSize, int PageIndex, ref int TotalRecords)
        {
            return LoggingProvider.Instance().GetLog(PortalID, PageSize, PageIndex, ref TotalRecords);
        }

        public virtual LogInfoArray GetLog(int PortalID, string LogType, int PageSize, int PageIndex, ref int TotalRecords)
        {
            return LoggingProvider.Instance().GetLog(PortalID, LogType, PageSize, PageIndex, ref TotalRecords);
        }

        public virtual LogInfoArray GetLog(string LogType, int PageSize, int PageIndex, ref int TotalRecords)
        {
            return LoggingProvider.Instance().GetLog(LogType, PageSize, PageIndex, ref TotalRecords);
        }

        public virtual ArrayList GetLogTypeConfigInfo()
        {
            return LoggingProvider.Instance().GetLogTypeConfigInfo();
        }

        public virtual LogTypeConfigInfo GetLogTypeConfigInfoByID(string ID)
        {
            return LoggingProvider.Instance().GetLogTypeConfigInfoByID(ID);
        }

        public virtual ArrayList GetLogTypeInfo()
        {
            return LoggingProvider.Instance().GetLogTypeInfo();
        }

        public virtual object GetSingleLog(LogInfo objLogInfo, LoggingProvider.ReturnType objReturnType)
        {
            return LoggingProvider.Instance().GetSingleLog(objLogInfo, objReturnType);
        }

        public bool LoggingIsEnabled(string LogType, int PortalID)
        {
            return LoggingProvider.Instance().LoggingIsEnabled(LogType, PortalID);
        }

        public virtual bool SupportsEmailNotification()
        {
            return LoggingProvider.Instance().SupportsEmailNotification();
        }

        public virtual bool SupportsInternalViewer()
        {
            return LoggingProvider.Instance().SupportsInternalViewer();
        }

        public void AddLog(LogInfo objLogInfo)
        {
            try
            {
                objLogInfo.LogCreateDate = DateTime.Now;
                objLogInfo.LogServerName = Globals.ServerName;

                if (objLogInfo.LogUserName == "")
                {
                    if (HttpContext.Current != null)
                    {
                        if (HttpContext.Current.Request.IsAuthenticated)
                        {
                            UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                            objLogInfo.LogUserName = objUserInfo.Username;
                        }
                    }
                }

                LoggingProvider.Instance().AddLog(objLogInfo);
            }
            catch (Exception e)
            {
                try
                {
                    string str = XmlUtils.Serialize(objLogInfo);
                    string f;
                    f = Globals.HostMapPath + "\\Logs\\LogFailures.xml.resources";
                    WriteLog(f, str);
                }
                catch (Exception)
                {
                    //critical error writing
                }
            }
        }

        public virtual void AddLogType(LogTypeInfo objLogTypeInfo)
        {
            LoggingProvider.Instance().AddLogType(objLogTypeInfo.LogTypeKey, objLogTypeInfo.LogTypeFriendlyName, objLogTypeInfo.LogTypeDescription, objLogTypeInfo.LogTypeCSSClass, objLogTypeInfo.LogTypeOwner);
        }

        public virtual void AddLogTypeConfigInfo(LogTypeConfigInfo objLogTypeConfigInfo)
        {
            LoggingProvider.Instance().AddLogTypeConfigInfo(objLogTypeConfigInfo.ID, objLogTypeConfigInfo.LoggingIsActive, objLogTypeConfigInfo.LogTypeKey, objLogTypeConfigInfo.LogTypePortalID, objLogTypeConfigInfo.KeepMostRecent, objLogTypeConfigInfo.LogFileName, objLogTypeConfigInfo.EmailNotificationIsActive, Convert.ToString(objLogTypeConfigInfo.NotificationThreshold), Convert.ToString(objLogTypeConfigInfo.NotificationThresholdTime), Convert.ToString(objLogTypeConfigInfo.NotificationThresholdTimeType), objLogTypeConfigInfo.MailFromAddress, objLogTypeConfigInfo.MailToAddress);
        }

        public void ClearLog()
        {
            LoggingProvider.Instance().ClearLog();
        }

        public void DeleteLog(LogInfo objLogInfo)
        {
            LoggingProvider.Instance().DeleteLog(objLogInfo);
        }

        public virtual void DeleteLogType(LogTypeInfo objLogTypeInfo)
        {
            LoggingProvider.Instance().DeleteLogType(objLogTypeInfo.LogTypeKey);
        }

        public virtual void DeleteLogTypeConfigInfo(LogTypeConfigInfo objLogTypeConfigInfo)
        {
            LoggingProvider.Instance().DeleteLogTypeConfigInfo(objLogTypeConfigInfo.ID);
        }

        public void PurgeLogBuffer()
        {
            LoggingProvider.Instance().PurgeLogBuffer();
        }

        public virtual void UpdateLogType(LogTypeInfo objLogTypeInfo)
        {
            LoggingProvider.Instance().UpdateLogType(objLogTypeInfo.LogTypeKey, objLogTypeInfo.LogTypeFriendlyName, objLogTypeInfo.LogTypeDescription, objLogTypeInfo.LogTypeCSSClass, objLogTypeInfo.LogTypeOwner);
        }

        public virtual void UpdateLogTypeConfigInfo(LogTypeConfigInfo objLogTypeConfigInfo)
        {
            LoggingProvider.Instance().UpdateLogTypeConfigInfo(objLogTypeConfigInfo.ID, objLogTypeConfigInfo.LoggingIsActive, objLogTypeConfigInfo.LogTypeKey, objLogTypeConfigInfo.LogTypePortalID, objLogTypeConfigInfo.KeepMostRecent, objLogTypeConfigInfo.LogFileName, objLogTypeConfigInfo.EmailNotificationIsActive, Convert.ToString(objLogTypeConfigInfo.NotificationThreshold), Convert.ToString(objLogTypeConfigInfo.NotificationThresholdTime), Convert.ToString(objLogTypeConfigInfo.NotificationThresholdTimeType), objLogTypeConfigInfo.MailFromAddress, objLogTypeConfigInfo.MailToAddress);
        }

        private void WriteLog(string FilePath, string Message)
        {
            //--------------------------------------------------------------
            //Write the log entry
            //--------------------------------------------------------------
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                //--------------------------------------------------------------
                // Write the entry to the log.
                //--------------------------------------------------------------
                lockLog.AcquireWriterLock(WriterLockTimeout);
                int intAttempts = 0;
                //wait for up to 100 milliseconds for the file
                //to be unlocked if it is not available
                while (!(fs != null || intAttempts == 100))
                {
                    intAttempts++;
                    try
                    {
                        fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(1);
                    }
                }

                if (fs == null)
                {
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.Response.Write("An error has occurred writing to the exception log.");
                        HttpContext.Current.Response.End();
                    }
                }
                else
                {
                    //--------------------------------------------------------------
                    //Instantiate a new StreamWriter
                    //--------------------------------------------------------------
                    sw = new StreamWriter(fs, Encoding.UTF8);
                    long FileLength;
                    FileLength = fs.Length;
                    //--------------------------------------------------------------
                    //check to see if this file is new
                    //--------------------------------------------------------------
                    if (FileLength > 0)
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
                        Message = "<logs>" + Message;
                    }

                    //--------------------------------------------------------------
                    //write out our exception
                    //--------------------------------------------------------------
                    sw.WriteLine(Message + "</logs>");
                    sw.Flush();
                }
                if (sw != null)
                {
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
                //--------------------------------------------------------------
                //handle the more common exceptions up
                //front, leave less common ones to the end
                //--------------------------------------------------------------
            }
            catch (UnauthorizedAccessException exc)
            {
                if (HttpContext.Current != null)
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader(response, "Unauthorized Access Error");

                    string strMessage = exc.Message + " The Windows User Account listed below must have Read/Write Privileges to this path.";
                    HtmlUtils.WriteError(response, FilePath, strMessage);

                    HtmlUtils.WriteFooter(response);
                    response.End();
                }
            }
            catch (DirectoryNotFoundException exc)
            {
                if (HttpContext.Current != null)
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader(response, "Directory Not Found Error");

                    string strMessage = exc.Message;
                    HtmlUtils.WriteError(response, FilePath, strMessage);

                    HtmlUtils.WriteFooter(response);
                    response.End();
                }
            }
            catch (PathTooLongException exc)
            {
                if (HttpContext.Current != null)
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader(response, "Path Too Long Error");

                    string strMessage = exc.Message;
                    HtmlUtils.WriteError(response, FilePath, strMessage);

                    HtmlUtils.WriteFooter(response);
                    response.End();
                }
            }
            catch (IOException exc)
            {
                if (HttpContext.Current != null)
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader(response, "IO Error");

                    string strMessage = exc.Message;
                    HtmlUtils.WriteError(response, FilePath, strMessage);

                    HtmlUtils.WriteFooter(response);
                    response.End();
                }
            }
            catch (Exception exc)
            {
                if (HttpContext.Current != null)
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader(response, "Unhandled Error");

                    string strMessage = exc.Message;
                    HtmlUtils.WriteError(response, FilePath, strMessage);

                    HtmlUtils.WriteFooter(response);
                    response.End();
                }
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }

                lockLog.ReleaseWriterLock();
            }
        }
    }
}