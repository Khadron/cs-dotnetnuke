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