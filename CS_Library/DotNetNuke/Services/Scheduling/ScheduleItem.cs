#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
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
using System.Collections;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Services.Scheduling
{
    /// <summary>
    /// This custom business object represents a single item on the schedule.
    /// </summary>
    public class ScheduleItem
    {
        private string _AttachToEvent;
        private bool _CatchUpEnabled;
        private bool _Enabled;
        private DateTime _NextStart;
        private string _ObjectDependencies;
        private int _ProcessGroup;
        private int _RetainHistoryNum;
        private int _RetryTimeLapse;
        private string _RetryTimeLapseMeasurement;
        private int _ScheduleID;
        private Hashtable _ScheduleItemSettings;
        private ScheduleSource _ScheduleSource;
        private string _Servers;
        private int _ThreadID;
        private int _TimeLapse;
        private string _TimeLapseMeasurement;
        private string _TypeFullName;

        public ScheduleItem()
        {
            this._ScheduleID = Null.NullInteger;
            this._TypeFullName = Null.NullString;
            this._TimeLapse = Null.NullInteger;
            this._TimeLapseMeasurement = Null.NullString;
            this._RetryTimeLapse = Null.NullInteger;
            this._RetryTimeLapseMeasurement = Null.NullString;
            this._ObjectDependencies = Null.NullString;
            this._RetainHistoryNum = Null.NullInteger;
            this._NextStart = Null.NullDate;
            this._CatchUpEnabled = Null.NullBoolean;
            this._Enabled = Null.NullBoolean;
            this._AttachToEvent = Null.NullString;
            this._ThreadID = Null.NullInteger;
            this._ProcessGroup = Null.NullInteger;
            this._Servers = Null.NullString;
        }

        public string AttachToEvent
        {
            get
            {
                return this._AttachToEvent;
            }
            set
            {
                this._AttachToEvent = value;
            }
        }

        public bool CatchUpEnabled
        {
            get
            {
                return this._CatchUpEnabled;
            }
            set
            {
                this._CatchUpEnabled = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return this._Enabled;
            }
            set
            {
                this._Enabled = value;
            }
        }

        public DateTime NextStart
        {
            get
            {
                if (_NextStart == Null.NullDate)
                {
                    _NextStart = DateTime.Now;
                }
                return _NextStart;
            }
            set
            {
                this._NextStart = value;
            }
        }

        public string ObjectDependencies
        {
            get
            {
                return this._ObjectDependencies;
            }
            set
            {
                this._ObjectDependencies = value;
            }
        }

        public int ProcessGroup
        {
            get
            {
                return this._ProcessGroup;
            }
            set
            {
                this._ProcessGroup = value;
            }
        }

        public int RetainHistoryNum
        {
            get
            {
                return this._RetainHistoryNum;
            }
            set
            {
                this._RetainHistoryNum = value;
            }
        }

        public int RetryTimeLapse
        {
            get
            {
                return this._RetryTimeLapse;
            }
            set
            {
                this._RetryTimeLapse = value;
            }
        }

        public string RetryTimeLapseMeasurement
        {
            get
            {
                return this._RetryTimeLapseMeasurement;
            }
            set
            {
                this._RetryTimeLapseMeasurement = value;
            }
        }

        public int ScheduleID
        {
            get
            {
                return this._ScheduleID;
            }
            set
            {
                this._ScheduleID = value;
            }
        }

        public ScheduleSource ScheduleSource
        {
            get
            {
                return this._ScheduleSource;
            }
            set
            {
                this._ScheduleSource = value;
            }
        }

        public string Servers
        {
            get
            {
                return this._Servers;
            }
            set
            {
                value = value.Trim();
                if (value.Length > 0)
                {
                    if (value.IndexOf(",") > 0)
                    {
                        value = "," + value;
                    }
                    if (value.LastIndexOf(",") < value.Length - 1)
                    {
                        value += ",";
                    }
                }
                _Servers = value;
            }
        }

        public int ThreadID
        {
            get
            {
                return this._ThreadID;
            }
            set
            {
                this._ThreadID = value;
            }
        }

        public int TimeLapse
        {
            get
            {
                return this._TimeLapse;
            }
            set
            {
                this._TimeLapse = value;
            }
        }

        public string TimeLapseMeasurement
        {
            get
            {
                return this._TimeLapseMeasurement;
            }
            set
            {
                this._TimeLapseMeasurement = value;
            }
        }

        public string TypeFullName
        {
            get
            {
                return this._TypeFullName;
            }
            set
            {
                this._TypeFullName = value;
            }
        }

        public void AddSetting( string Key, string Value )
        {
            this._ScheduleItemSettings.Add( Key, Value );
        }

        public string GetSetting( string Key )
        {
            if (_ScheduleItemSettings == null)
            {
                GetSettings();
            }
            if (_ScheduleItemSettings.ContainsKey(Key))
            {
                return Convert.ToString(_ScheduleItemSettings[Key]);
            }
            else
            {
                return "";
            }
        }

        public Hashtable GetSettings()
        {
            _ScheduleItemSettings = SchedulingProvider.Instance().GetScheduleItemSettings(this.ScheduleID);
            return _ScheduleItemSettings;
        }

        public bool HasObjectDependencies( string strObjectDependencies )
        {
            if (strObjectDependencies.IndexOf(",") > -1)
            {
                string[] a;
                a = strObjectDependencies.ToLower().Split(',');
                int i;
                for (i = 0; i <= a.Length - 1; i++)
                {
                    if (a[i] == strObjectDependencies.ToLower())
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (ObjectDependencies.ToLower().IndexOf(strObjectDependencies.ToLower()) > -1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}