using System;
using DotNetNuke.Services.Scheduling;

namespace DotNetNuke.Entities.Users
{
    /// <Summary>
    /// The PurgeUsersOnline class provides a Scheduler for purging the Users Online
    /// data
    /// </Summary>
    public class PurgeUsersOnline : SchedulerClient
    {
        /// <summary>
        /// Constructs a PurgeUsesOnline SchedulerClient
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="objScheduleHistoryItem">A SchedulerHistiryItem</param>
        public PurgeUsersOnline(ScheduleHistoryItem objScheduleHistoryItem)
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }

        /// <summary>
        /// UpdateUsersOnline updates the Users Online information
        /// </summary>
        private void UpdateUsersOnline()
        {
            UserOnlineController objUserOnlineController = new UserOnlineController();

            // Is Users Online Enabled?
            if (objUserOnlineController.IsEnabled())
            {
                // Update the Users Online records from Cache
                this.Status = "Updating Users Online";
                objUserOnlineController.UpdateUsersOnline();
                this.Status = "Update Users Online Successfully";
                this.ScheduleHistoryItem.Succeeded = true;
            }
        }

        /// <summary>
        /// DoWork does th4 Scheduler work
        /// </summary>
        public override void DoWork()
        {
            try
            {
                //notification that the event is progressing
                this.Progressing(); //OPTIONAL
                UpdateUsersOnline();
                this.ScheduleHistoryItem.Succeeded = true; //REQUIRED
                this.ScheduleHistoryItem.AddLogNote("UsersOnline purge completed.");
            }
            catch (Exception exc) //REQUIRED
            {
                this.ScheduleHistoryItem.Succeeded = false; //REQUIRED
                this.ScheduleHistoryItem.AddLogNote("UsersOnline purge failed." + exc.ToString()); //OPTIONAL

                //notification that we have errored
                this.Errored(ref exc); //REQUIRED

                //log the exception
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc); //OPTIONAL
            }
        }
    }
}