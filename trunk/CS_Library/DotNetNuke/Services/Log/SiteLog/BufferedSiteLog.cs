using System;
using System.Collections;
using DotNetNuke.Data;

namespace DotNetNuke.Services.Log.SiteLog
{
    public class BufferedSiteLog
    {
        public ArrayList SiteLog;
        public string SiteLogStorage;

        public void AddSiteLog()
        {
            try
            {
                SiteLogInfo objSiteLog;
                SiteLogController objSiteLogs = new SiteLogController();

                // iterate through buffered sitelog items and insert into database
                int intIndex;
                for (intIndex = 0; intIndex <= SiteLog.Count - 1; intIndex++)
                {
                    objSiteLog = (SiteLogInfo)SiteLog[intIndex];
                    switch (SiteLogStorage)
                    {
                        case "D": // database

                            DataProvider.Instance().AddSiteLog(objSiteLog.DateTime, objSiteLog.PortalId, objSiteLog.UserId, objSiteLog.Referrer, objSiteLog.URL, objSiteLog.UserAgent, objSiteLog.UserHostAddress, objSiteLog.UserHostName, objSiteLog.TabId, objSiteLog.AffiliateId);
                            break;
                        case "F": // file system

                            objSiteLogs.W3CExtendedLog(objSiteLog.DateTime, objSiteLog.PortalId, objSiteLog.UserId, objSiteLog.Referrer, objSiteLog.URL, objSiteLog.UserAgent, objSiteLog.UserHostAddress, objSiteLog.UserHostName, objSiteLog.TabId, objSiteLog.AffiliateId);
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}