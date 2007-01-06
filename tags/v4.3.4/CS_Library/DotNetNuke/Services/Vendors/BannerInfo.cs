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
    public class BannerInfo
    {
        private int _BannerId;
        private string _BannerName;
        private int _BannerTypeId;
        private int _ClickThroughs;
        private double _CPM;
        private string _CreatedByUser;
        private DateTime _CreatedDate;
        private int _Criteria;
        private string _Description;
        private DateTime _EndDate;
        private string _GroupName;
        private int _Height;
        private string _ImageFile;
        private int _Impressions;
        private DateTime _StartDate;
        private string _URL;
        private int _VendorId;
        private int _Views;
        private int _Width;

        public int BannerId
        {
            get
            {
                return this._BannerId;
            }
            set
            {
                this._BannerId = value;
            }
        }

        public string BannerName
        {
            get
            {
                return this._BannerName;
            }
            set
            {
                this._BannerName = value;
            }
        }

        public int BannerTypeId
        {
            get
            {
                return this._BannerTypeId;
            }
            set
            {
                this._BannerTypeId = value;
            }
        }

        public int ClickThroughs
        {
            get
            {
                return this._ClickThroughs;
            }
            set
            {
                this._ClickThroughs = value;
            }
        }

        public double CPM
        {
            get
            {
                return this._CPM;
            }
            set
            {
                this._CPM = value;
            }
        }

        public string CreatedByUser
        {
            get
            {
                return this._CreatedByUser;
            }
            set
            {
                this._CreatedByUser = value;
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

        public int Criteria
        {
            get
            {
                return this._Criteria;
            }
            set
            {
                this._Criteria = value;
            }
        }

        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                this._Description = value;
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

        public string GroupName
        {
            get
            {
                return this._GroupName;
            }
            set
            {
                this._GroupName = value;
            }
        }

        public int Height
        {
            get
            {
                return this._Height;
            }
            set
            {
                this._Height = value;
            }
        }

        public string ImageFile
        {
            get
            {
                return this._ImageFile;
            }
            set
            {
                this._ImageFile = value;
            }
        }

        public int Impressions
        {
            get
            {
                return this._Impressions;
            }
            set
            {
                this._Impressions = value;
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

        public int Views
        {
            get
            {
                return this._Views;
            }
            set
            {
                this._Views = value;
            }
        }

        public int Width
        {
            get
            {
                return this._Width;
            }
            set
            {
                this._Width = value;
            }
        }
    }
}