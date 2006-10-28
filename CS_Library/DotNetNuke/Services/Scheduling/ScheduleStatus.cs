namespace DotNetNuke.Services.Scheduling
{
    public enum ScheduleStatus
    {
        NOT_SET = 0,
        WAITING_FOR_OPEN_THREAD = 1,
        RUNNING_EVENT_SCHEDULE = 2,
        RUNNING_TIMER_SCHEDULE = 3,
        RUNNING_REQUEST_SCHEDULE = 4,
        WAITING_FOR_REQUEST = 5,
        SHUTTING_DOWN = 6,
        STOPPED = 7,
    }
}