using System;

namespace DotNetNuke.Services.Cache.BroadcastPollingCachingProvider
{
    public class BroadcastInfo
    {
        private int _BroadcastID;
        private int _SourceServerID;
        private string _SourceServerName;
        private string _BroadcastType;
        private string _BroadcastMessage;
        private DateTime _BroadcastDate;

        public int BroadcastID
        {
            get
            {
                return _BroadcastID;
            }
            set
            {
                _BroadcastID = value;
            }
        }

        public int SourceServerID
        {
            get
            {
                return _SourceServerID;
            }
            set
            {
                _SourceServerID = value;
            }
        }

        public string SourceServerName
        {
            get
            {
                return _SourceServerName;
            }
            set
            {
                _SourceServerName = value;
            }
        }

        public string BroadcastType
        {
            get
            {
                return _BroadcastType;
            }
            set
            {
                _BroadcastType = value;
            }
        }

        public string BroadcastMessage
        {
            get
            {
                return _BroadcastMessage;
            }
            set
            {
                _BroadcastMessage = value;
            }
        }

        public DateTime BroadcastDate
        {
            get
            {
                return _BroadcastDate;
            }
            set
            {
                _BroadcastDate = value;
            }
        }
    }
}