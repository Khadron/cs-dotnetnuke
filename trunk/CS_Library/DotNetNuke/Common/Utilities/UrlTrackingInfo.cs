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