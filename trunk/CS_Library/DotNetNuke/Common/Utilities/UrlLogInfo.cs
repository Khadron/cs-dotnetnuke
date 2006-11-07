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