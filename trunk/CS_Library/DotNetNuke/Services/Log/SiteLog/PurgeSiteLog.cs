using System;
using System.Collections;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Scheduling;
using Microsoft.VisualBasic;

namespace DotNetNuke.Services.Log.SiteLog
{

    public class PurgeSiteLog : SchedulerClient
    {
        public PurgeSiteLog(ScheduleHistoryItem objScheduleHistoryItem)
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }

        public override void DoWork()
        {
            try
            {
                //notification that the event is progressing
                this.Progressing(); //OPTIONAL

                PurgeSiteLog_Renamed();

                this.ScheduleHistoryItem.Succeeded = true; //REQUIRED

                this.ScheduleHistoryItem.AddLogNote("Site Log purged."); //OPTIONAL
            }
            catch (Exception exc) //REQUIRED
            {
                this.ScheduleHistoryItem.Succeeded = false; //REQUIRED

                this.ScheduleHistoryItem.AddLogNote("Site Log purge failed. " + exc.ToString()); //OPTIONAL

                //notification that we have errored
                this.Errored(ref exc); //REQUIRED

                //log the exception
                Exceptions.Exceptions.LogException(exc); //OPTIONAL
            }
        }

        private void PurgeSiteLog_Renamed()
        {
            SiteLogController objSiteLog = new SiteLogController();

            PortalController objPortals = new PortalController();
            ArrayList arrPortals = objPortals.GetPortals();
            PortalInfo objPortal;
            DateTime PurgeDate;

            int intIndex;
            for (intIndex = 0; intIndex <= arrPortals.Count - 1; intIndex++)
            {
                objPortal = (PortalInfo)arrPortals[intIndex];
                if (objPortal.SiteLogHistory > 0)
                {
                    PurgeDate = DateAndTime.DateAdd(DateInterval.Day, -(objPortal.SiteLogHistory), DateTime.Now);
                    objSiteLog.DeleteSiteLog(PurgeDate, objPortal.PortalID);
                }
            }
        }
    }
}