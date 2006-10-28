using System;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Services.Log.EventLog
{
    public class LogTypeConfigInfo : LogTypeInfo
    {
        private bool _EmailNotificationIsActive;
        private string _ID;
        private string _KeepMostRecent;
        private string _LogFileName;
        private string _LogFileNameWithPath;
        private bool _LoggingIsActive;
        private string _LogTypePortalID;
        private string _MailFromAddress;
        private string _MailToAddress;
        private int _NotificationThreshold;
        private int _NotificationThresholdTime;
        private NotificationThresholdTimeTypes _NotificationThresholdTimeType;

        public enum NotificationThresholdTimeTypes
        {
            None = 0,
            Seconds = 1,
            Minutes = 2,
            Hours = 3,
            Days = 4,
        }

        public bool EmailNotificationIsActive
        {
            get
            {
                return this._EmailNotificationIsActive;
            }
            set
            {
                this._EmailNotificationIsActive = value;
            }
        }

        public string ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        public string KeepMostRecent
        {
            get
            {
                return this._KeepMostRecent;
            }
            set
            {
                this._KeepMostRecent = value;
            }
        }

        public string LogFileName
        {
            get
            {
                return this._LogFileName;
            }
            set
            {
                this._LogFileName = value;
            }
        }

        public string LogFileNameWithPath
        {
            get
            {
                return this._LogFileNameWithPath;
            }
            set
            {
                this._LogFileNameWithPath = value;
            }
        }

        public bool LoggingIsActive
        {
            get
            {
                return this._LoggingIsActive;
            }
            set
            {
                this._LoggingIsActive = value;
            }
        }

        public string LogTypePortalID
        {
            get
            {
                return this._LogTypePortalID;
            }
            set
            {
                this._LogTypePortalID = value;
            }
        }

        public string MailFromAddress
        {
            get
            {
                return this._MailFromAddress;
            }
            set
            {
                this._MailFromAddress = value;
            }
        }

        public string MailToAddress
        {
            get
            {
                return this._MailToAddress;
            }
            set
            {
                this._MailToAddress = value;
            }
        }

        public int NotificationThreshold
        {
            get
            {
                return this._NotificationThreshold;
            }
            set
            {
                this._NotificationThreshold = value;
            }
        }

        public int NotificationThresholdTime
        {
            get
            {
                return this._NotificationThresholdTime;
            }
            set
            {
                this._NotificationThresholdTime = value;
            }
        }

        public NotificationThresholdTimeTypes NotificationThresholdTimeType
        {
            get
            {
                return this._NotificationThresholdTimeType;
            }
            set
            {
                this._NotificationThresholdTimeType = value;
            }
        }

        public DateTime StartDateTime
        {
            get
            {
                switch( this.NotificationThresholdTimeType )
                {
                    case NotificationThresholdTimeTypes.None:
                        {
                            return Null.NullDate;
                        }
                    case NotificationThresholdTimeTypes.Seconds:
                        {
                            return DateTime.Now.AddSeconds( this.NotificationThresholdTime*-1 );
                        }
                    case NotificationThresholdTimeTypes.Minutes:
                        {
                            return DateTime.Now.AddMinutes( this.NotificationThresholdTime*-1 );
                        }
                    case NotificationThresholdTimeTypes.Hours:
                        {
                            return DateTime.Now.AddHours( this.NotificationThresholdTime*-1 );
                        }
                    case NotificationThresholdTimeTypes.Days:
                        {
                            return DateTime.Now.AddDays( this.NotificationThresholdTime*-1 );
                        }
                }
                return Null.NullDate;
            }
        }
    }
}