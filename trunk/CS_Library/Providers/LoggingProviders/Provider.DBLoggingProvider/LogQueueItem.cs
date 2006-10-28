namespace DotNetNuke.Services.Log.EventLog.DBLoggingProvider
{
    public class LogQueueItem
    {
        private LogInfo _LogInfo;
        private LogTypeConfigInfo _LogTypeConfigInfo;

        public LogInfo LogInfo
        {
            get
            {
                return _LogInfo;
            }
            set
            {
                _LogInfo = value;
            }
        }

        public LogTypeConfigInfo LogTypeConfigInfo
        {
            get
            {
                return _LogTypeConfigInfo;
            }
            set
            {
                _LogTypeConfigInfo = value;
            }
        }
    }
}