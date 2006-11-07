#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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