using System;
using System.Text;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Services.Scheduling
{
    public class ScheduleHistoryItem : ScheduleItem
    {
        private DateTime _EndDate;
        private StringBuilder _LogNotes;
        private int _ScheduleHistoryID;
        private string _Server;
        private DateTime _StartDate;
        private bool _Succeeded;

        public double ElapsedTime
        {
            get
            {
                double returnValue;
                try
                {
                    if (_EndDate == Null.NullDate && _StartDate != Null.NullDate)
                    {
                        return DateTime.Now.Subtract(_StartDate).TotalSeconds;
                    }
                    else if (_StartDate != Null.NullDate)
                    {
                        return _EndDate.Subtract(_StartDate).TotalSeconds;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    returnValue = 0;
                }
                return returnValue;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return this._EndDate;
            }
            set
            {
                this._EndDate = value;
            }
        }

        public string LogNotes
        {
            get
            {
                return this._LogNotes.ToString();
            }
            set
            {
                this._LogNotes = new StringBuilder( value );
            }
        }

        public bool Overdue
        {
            get
            {
                if (NextStart < DateTime.Now && EndDate == Null.NullDate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public double OverdueBy
        {
            get
            {
                double returnValue;
                try
                {
                    if (NextStart <= DateTime.Now && EndDate == Null.NullDate)
                    {
                        return DateTime.Now.Subtract(NextStart).TotalSeconds;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    returnValue = 0;
                }
                return returnValue;
            }
        }

        public double RemainingTime
        {
            get
            {
                double returnValue;
                try
                {
                    if (NextStart > DateTime.Now && EndDate == Null.NullDate)
                    {
                        return NextStart.Subtract(DateTime.Now).TotalSeconds;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    returnValue = 0;
                }
                return returnValue;
            }
        }

        public int ScheduleHistoryID
        {
            get
            {
                return this._ScheduleHistoryID;
            }
            set
            {
                this._ScheduleHistoryID = value;
            }
        }

        public string Server
        {
            get
            {
                return this._Server;
            }
            set
            {
                this._Server = value;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return this._StartDate;
            }
            set
            {
                this._StartDate = value;
            }
        }

        public bool Succeeded
        {
            get
            {
                return this._Succeeded;
            }
            set
            {
                this._Succeeded = value;
            }
        }

        public ScheduleHistoryItem()
        {
            this._ScheduleHistoryID = Null.NullInteger;
            this._StartDate = Null.NullDate;
            this._EndDate = Null.NullDate;
            this._Succeeded = Null.NullBoolean;
            this._LogNotes = new StringBuilder();
            this._Server = Null.NullString;
        }

        public ScheduleHistoryItem( ScheduleItem objScheduleItem )
        {
            this.AttachToEvent = objScheduleItem.AttachToEvent;
            this.CatchUpEnabled = objScheduleItem.CatchUpEnabled;
            this.Enabled = objScheduleItem.Enabled;
            this.NextStart = objScheduleItem.NextStart;
            this.ObjectDependencies = objScheduleItem.ObjectDependencies;
            this.ProcessGroup = objScheduleItem.ProcessGroup;
            this.RetainHistoryNum = objScheduleItem.RetainHistoryNum;
            this.RetryTimeLapse = objScheduleItem.RetryTimeLapse;
            this.RetryTimeLapseMeasurement = objScheduleItem.RetryTimeLapseMeasurement;
            this.ScheduleID = objScheduleItem.ScheduleID;
            this.ScheduleSource = objScheduleItem.ScheduleSource;
            this.ThreadID = objScheduleItem.ThreadID;
            this.TimeLapse = objScheduleItem.TimeLapse;
            this.TimeLapseMeasurement = objScheduleItem.TimeLapseMeasurement;
            this.TypeFullName = objScheduleItem.TypeFullName;
            this._ScheduleHistoryID = Null.NullInteger;
            this._StartDate = Null.NullDate;
            this._EndDate = Null.NullDate;
            this._Succeeded = Null.NullBoolean;
            this._LogNotes = new StringBuilder();
            this._Server = Null.NullString;
        }

        public void AddLogNote( string Notes )
        {
            StringBuilder stringBuilder1 = this._LogNotes.Append( ( Notes + "\r\n" ) );
        }
    }
}