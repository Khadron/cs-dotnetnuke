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

namespace DotNetNuke.Services.Vendors
{
    public class AffiliateInfo
    {
        private int _Acquisitions;
        private int _AffiliateId;
        private int _Clicks;
        private double _CPA;
        private double _CPATotal;
        private double _CPC;
        private double _CPCTotal;
        private DateTime _EndDate;
        private DateTime _StartDate;
        private int _VendorId;

        public int Acquisitions
        {
            get
            {
                return this._Acquisitions;
            }
            set
            {
                this._Acquisitions = value;
            }
        }

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

        public double CPA
        {
            get
            {
                return this._CPA;
            }
            set
            {
                this._CPA = value;
            }
        }

        public double CPATotal
        {
            get
            {
                return this._CPATotal;
            }
            set
            {
                this._CPATotal = value;
            }
        }

        public double CPC
        {
            get
            {
                return this._CPC;
            }
            set
            {
                this._CPC = value;
            }
        }

        public double CPCTotal
        {
            get
            {
                return this._CPCTotal;
            }
            set
            {
                this._CPCTotal = value;
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

        public int VendorId
        {
            get
            {
                return this._VendorId;
            }
            set
            {
                this._VendorId = value;
            }
        }
    }
}