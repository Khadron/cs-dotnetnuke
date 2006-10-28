namespace DotNetNuke.Services.Scheduling
{
    public enum ScheduleSource
    {
        NOT_SET = 0,
        STARTED_FROM_SCHEDULE_CHANGE = 1,
        STARTED_FROM_EVENT = 2,
        STARTED_FROM_TIMER = 3,
        STARTED_FROM_BEGIN_REQUEST = 4,
    }
}