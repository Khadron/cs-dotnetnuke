using System;

namespace DotNetNuke.Common.Utilities
{
    public class UrlLogInfo
    {
        private DateTime _ClickDate;
        private string _FullName;
        private int _UrlLogID;
        private int _UrlTrackingID;
        private int _UserID;

        public DateTime ClickDate
        {
            get
            {
                return this._ClickDate;
            }
            set
            {
                this._ClickDate = value;
            }
        }

        public string FullName
        {
            get
            {
                return this._FullName;
            }
            set
            {
                this._FullName = value;
            }
        }

        public int UrlLogID
        {
            get
            {
                return this._UrlLogID;
            }
            set
            {
                this._UrlLogID = value;
            }
        }

        public int UrlTrackingID
        {
            get
            {
                return this._UrlTrackingID;
            }
            set
            {
                this._UrlTrackingID = value;
            }
        }

        public int UserID
        {
            get
            {
                return this._UserID;
            }
            set
            {
                this._UserID = value;
            }
        }
    }
}