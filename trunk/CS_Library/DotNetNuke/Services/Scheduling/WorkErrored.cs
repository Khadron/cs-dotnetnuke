using System;

namespace DotNetNuke.Services.Scheduling
{
    public delegate void WorkErrored( ref SchedulerClient objSchedulerClient, ref Exception objException );
}