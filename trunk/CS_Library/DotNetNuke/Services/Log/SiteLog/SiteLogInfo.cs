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

namespace DotNetNuke.Services.Log.SiteLog
{
    public class SiteLogInfo
    {
        private int _AffiliateId;
        private DateTime _DateTime;
        private int _PortalId;
        private string _Referrer;
        private int _TabId;
        private string _URL;
        private string _UserAgent;
        private string _UserHostAddress;
        private string _UserHostName;
        private int _UserId;

        public int AffiliateId
        {
            get
            {
                return this._AffiliateId;
            }
            set
            {
                this._AffiliateId = value;
            }
        }

        public DateTime DateTime
        {
            get
            {
                return this._DateTime;
            }
            set
            {
                this._DateTime = value;
            }
        }

        public int PortalId
        {
            get
            {
                return this._PortalId;
            }
            set
            {
                this._PortalId = value;
            }
        }

        public string Referrer
        {
            get
            {
                return this._Referrer;
            }
            set
            {
                this._Referrer = value;
            }
        }

        public int TabId
        {
            get
            {
                return this._TabId;
            }
            set
            {
                this._TabId = value;
            }
        }

        public string URL
        {
            get
            {
                return this._URL;
            }
            set
            {
                this._URL = value;
            }
        }

        public string UserAgent
        {
            get
            {
                return this._UserAgent;
            }
            set
            {
                this._UserAgent = value;
            }
        }

        public string UserHostAddress
        {
            get
            {
                return this._UserHostAddress;
            }
            set
            {
                this._UserHostAddress = value;
            }
        }

        public string UserHostName
        {
            get
            {
                return this._UserHostName;
            }
            set
            {
                this._UserHostName = value;
            }
        }

        public int UserId
        {
            get
            {
                return this._UserId;
            }
            set
            {
                this._UserId = value;
            }
        }
    }
}