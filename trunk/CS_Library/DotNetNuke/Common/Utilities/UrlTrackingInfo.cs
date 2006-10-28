using System;

namespace DotNetNuke.Common.Utilities
{
    public class UrlTrackingInfo
    {
        private int _Clicks;
        private DateTime _CreatedDate;
        private DateTime _LastClick;
        private bool _LogActivity;
        private int _ModuleID;
        private bool _NewWindow;
        private int _PortalID;
        private bool _TrackClicks;
        private string _Url;
        private int _UrlTrackingID;
        private string _UrlType;

        public int Clicks
        {
            get
            {
                return this._Clicks;
            }
            set
            {
                this._Clicks = value;
            }
        }

        public DateTime CreatedDate
        {
            get
            {
                return this._CreatedDate;
            }
            set
            {
                this._CreatedDate = value;
            }
        }

        public DateTime LastClick
        {
            get
            {
                return this._LastClick;
            }
            set
            {
                this._LastClick = value;
            }
        }

        public bool LogActivity
        {
            get
            {
                return this._LogActivity;
            }
            set
            {
                this._LogActivity = value;
            }
        }

        public int ModuleID
        {
            get
            {
                return this._ModuleID;
            }
            set
            {
                this._ModuleID = value;
            }
        }

        public bool NewWindow
        {
            get
            {
                return this._NewWindow;
            }
            set
            {
                this._NewWindow = value;
            }
        }

        public int PortalID
        {
            get
            {
                return this._PortalID;
            }
            set
            {
                this._PortalID = value;
            }
        }

        public bool TrackClicks
        {
            get
            {
                return this._TrackClicks;
            }
            set
            {
                this._TrackClicks = value;
            }
        }

        public string Url
        {
            get
            {
                return this._Url;
            }
            set
            {
                this._Url = value;
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

        public string UrlType
        {
            get
            {
                return this._UrlType;
            }
            set
            {
                this._UrlType = value;
            }
        }
    }
}