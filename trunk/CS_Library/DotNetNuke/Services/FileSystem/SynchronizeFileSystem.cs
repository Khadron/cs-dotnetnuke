using System;
using System.Collections;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Scheduling;

namespace DotNetNuke.Services.FileSystem
{
    public class SynchronizeFileSystem : SchedulerClient
    {
        public SynchronizeFileSystem( ScheduleHistoryItem objScheduleHistoryItem )
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }

        public override void DoWork()
        {
            try
            {
                //notification that the event is progressing
                this.Progressing(); //OPTIONAL

                Synchronize();

                this.ScheduleHistoryItem.Succeeded = true; //REQUIRED

                this.ScheduleHistoryItem.AddLogNote("File System Synchronized."); //OPTIONAL
            }
            catch (Exception exc) //REQUIRED
            {
                this.ScheduleHistoryItem.Succeeded = false; //REQUIRED

                this.ScheduleHistoryItem.AddLogNote("File System Synchronization failed. " + exc.ToString()); //OPTIONAL

                //notification that we have errored
                this.Errored(ref exc); //REQUIRED

                //log the exception
                Exceptions.Exceptions.LogException(exc); //OPTIONAL
            }
        }

        private void Synchronize()
        {
            PortalController objPortals = new PortalController();
            ArrayList arrPortals = objPortals.GetPortals();
            PortalInfo objPortal;

            //Sync Host
            FileSystemUtils.Synchronize(Null.NullInteger, Null.NullInteger, Globals.HostMapPath);

            //Sync Portals
            int intIndex;
            for (intIndex = 0; intIndex <= arrPortals.Count - 1; intIndex++)
            {
                objPortal = (PortalInfo)arrPortals[intIndex];
                FileSystemUtils.Synchronize(objPortal.PortalID, objPortal.AdministratorRoleId, objPortal.HomeDirectoryMapPath);
            }
        }
    }
}