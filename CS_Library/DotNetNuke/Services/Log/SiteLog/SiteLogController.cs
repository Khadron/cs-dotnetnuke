using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Security;

namespace DotNetNuke.Services.Log.SiteLog
{
    public class SiteLogController
    {
        public void AddSiteLog(int PortalId, int UserId, string Referrer, string URL, string UserAgent, string UserHostAddress, string UserHostName, int TabId, int AffiliateId, int SiteLogBuffer, string SiteLogStorage)
        {
            PortalSecurity objSecurity = new PortalSecurity();
            try
            {
                if (Globals.PerformanceSetting == Globals.PerformanceSettings.NoCaching)
                {
                    SiteLogBuffer = 1;
                }

                switch (SiteLogBuffer)
                {
                    case 0: // logging disabled

                        break;
                    case 1: // no buffering

                        switch (SiteLogStorage)
                        {
                            case "D": // database

                                DataProvider.Instance().AddSiteLog(DateTime.Now, PortalId, UserId, objSecurity.InputFilter(Referrer, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(URL, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(UserAgent, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(UserHostAddress, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(UserHostName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup), TabId, AffiliateId);
                                break;
                            case "F": // file system

                                W3CExtendedLog(DateTime.Now, PortalId, UserId, objSecurity.InputFilter(Referrer, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(URL, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(UserAgent, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(UserHostAddress, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(UserHostName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup), TabId, AffiliateId);
                                break;
                        }
                        break;
                    default: // buffered logging

                        string key = "SiteLog" + PortalId.ToString();
                        ArrayList arrSiteLog = (ArrayList)DataCache.GetCache(key);

                        // get buffered site log records from the cache
                        if (arrSiteLog == null)
                        {
                            arrSiteLog = new ArrayList();
                            DataCache.SetCache(key, arrSiteLog);
                        }

                        // create new sitelog object
                        SiteLogInfo objSiteLog = new SiteLogInfo();
                        objSiteLog.DateTime = DateTime.Now;
                        objSiteLog.PortalId = PortalId;
                        objSiteLog.UserId = UserId;
                        objSiteLog.Referrer = objSecurity.InputFilter(Referrer, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                        objSiteLog.URL = objSecurity.InputFilter(URL, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                        objSiteLog.UserAgent = objSecurity.InputFilter(UserAgent, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                        objSiteLog.UserHostAddress = objSecurity.InputFilter(UserHostAddress, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                        objSiteLog.UserHostName = objSecurity.InputFilter(UserHostName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                        objSiteLog.TabId = TabId;
                        objSiteLog.AffiliateId = AffiliateId;

                        // add sitelog object to cache
                        arrSiteLog.Add(objSiteLog);

                        if (arrSiteLog.Count >= SiteLogBuffer)
                        {
                            // create the buffered sitelog object
                            BufferedSiteLog objBufferedSiteLog = new BufferedSiteLog();
                            objBufferedSiteLog.SiteLogStorage = SiteLogStorage;
                            objBufferedSiteLog.SiteLog = arrSiteLog;

                            // clear the current sitelogs from the cache
                            DataCache.RemoveCache(key);

                            // process buffered sitelogs on a background thread
                            Thread objThread = new Thread(new ThreadStart(objBufferedSiteLog.AddSiteLog));
                            objThread.Start();
                        }
                        break;
                }
            }
            catch (Exception)
            {
            }
        }

        public IDataReader GetSiteLog(int PortalId, string PortalAlias, int ReportType, DateTime StartDate, DateTime EndDate)
        {
            return DataProvider.Instance().GetSiteLog(PortalId, PortalAlias, "GetSiteLog" + ReportType.ToString(), StartDate, EndDate);
        }

        public void DeleteSiteLog(DateTime DateTime, int PortalId)
        {
            DataProvider.Instance().DeleteSiteLog(DateTime, PortalId);
        }

        public void W3CExtendedLog(DateTime DateTime, int PortalId, int UserId, string Referrer, string URL, string UserAgent, string UserHostAddress, string UserHostName, int TabId, int AffiliateId)
        {
            StreamWriter objStream;

            // create log file path
            string LogFilePath = Globals.ApplicationMapPath + "\\Portals\\" + PortalId.ToString() + "\\Logs\\";
            string LogFileName = "ex" + DateTime.Now.ToString("yyMMdd") + ".log";

            // check if log file exists
            if (!File.Exists(LogFilePath + LogFileName))
            {
                try
                {
                    // create log file
                    Directory.CreateDirectory(LogFilePath);

                    // open log file for append ( position the stream at the end of the file )
                    objStream = File.AppendText(LogFilePath + LogFileName);

                    // add standard log file headers
                    objStream.WriteLine("#Software: Microsoft Internet Information Services 6.0");
                    objStream.WriteLine("#Version: 1.0");
                    objStream.WriteLine("#Date: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    objStream.WriteLine("#Fields: date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) sc-status sc-substatus sc-win32-status");

                    // close stream
                    objStream.Flush();
                    objStream.Close();
                }
                catch
                {
                    // can not create file
                }
            }

            try
            {
                // open log file for append ( position the stream at the end of the file )
                objStream = File.AppendText(LogFilePath + LogFileName);

                // declare a string builder
                StringBuilder objStringBuilder = new StringBuilder(1024);

                // build W3C extended log item
                objStringBuilder.Append(DateTime.ToString("yyyy-MM-dd hh:mm:ss") + " ");
                objStringBuilder.Append(UserHostAddress + " ");
                objStringBuilder.Append("GET" + " ");
                objStringBuilder.Append(URL + " ");
                objStringBuilder.Append("-" + " ");
                objStringBuilder.Append("80" + " ");
                objStringBuilder.Append("-" + " ");
                objStringBuilder.Append(UserAgent + " ");
                objStringBuilder.Append("200" + " ");
                objStringBuilder.Append("0" + " ");
                objStringBuilder.Append("0");

                // write to log file
                objStream.WriteLine(objStringBuilder.ToString());

                // close stream
                objStream.Flush();
                objStream.Close();
            }
            catch
            {
                // can not open file
            }
        }
    }
}