using System;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Scheduling;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    public class InstallResources : SchedulerClient
    {
        public InstallResources( ScheduleHistoryItem objScheduleHistoryItem )
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }

        public override void DoWork()
        {
            try
            {
                //notification that the event is progressing
                this.Progressing(); //OPTIONAL

                ResourceInstaller objResourceInstaller = new ResourceInstaller();
                objResourceInstaller.Install();

                this.ScheduleHistoryItem.Succeeded = true; //REQUIRED

                this.ScheduleHistoryItem.AddLogNote( "Resource Installation Complete." ); //OPTIONAL
            }
            catch( Exception exc ) //REQUIRED
            {
                this.ScheduleHistoryItem.Succeeded = false; //REQUIRED

                this.ScheduleHistoryItem.AddLogNote( "Resource Installation Failed. " + exc.ToString() ); //OPTIONAL

                //notification that we have errored
                this.Errored( ref exc ); //REQUIRED

                //log the exception
                Exceptions.LogException( exc ); //OPTIONAL
            }
        }
    }
}