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

namespace DotNetNuke.Services.Vendors
{
    public class VendorInfo
    {
        private bool _Authorized;
        private int _Banners;
        private string _Cell;
        private string _City;
        private int _ClickThroughs;
        private string _Country;
        private string _CreatedByUser;
        private DateTime _CreatedDate;
        private string _Email;
        private string _Fax;
        private string _FirstName;
        private string _KeyWords;
        private string _LastName;
        private string _LogoFile;
        private int _PortalId;
        private string _PostalCode;
        private string _Region;
        private string _Street;
        private string _Telephone;
        private string _Unit;
        private string _UserName;
        private int _VendorId;
        private string _VendorName;
        private int _Views;
        private string _Website;

        public VendorInfo()
        {
        }

        public bool Authorized
        {
            get
            {
                return this._Authorized;
            }
            set
            {
                this._Authorized = value;
            }
        }

        public int Banners
        {
            get
            {
                return this._Banners;
            }
            set
            {
                this._Banners = value;
            }
        }

        public string Cell
        {
            get
            {
                return this._Cell;
            }
            set
            {
                this._Cell = value;
            }
        }

        public string City
        {
            get
            {
                return this._City;
            }
            set
            {
                this._City = value;
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

        public string Country
        {
            get
            {
                return this._Country;
            }
            set
            {
                this._Country = value;
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

        public string Email
        {
            get
            {
                return this._Email;
            }
            set
            {
                this._Email = value;
            }
        }

        public string Fax
        {
            get
            {
                return this._Fax;
            }
            set
            {
                this._Fax = value;
            }
        }

        public string FirstName
        {
            get
            {
                return this._FirstName;
            }
            set
            {
                this._FirstName = value;
            }
        }

        public string KeyWords
        {
            get
            {
                return this._KeyWords;
            }
            set
            {
                this._KeyWords = value;
            }
        }

        public string LastName
        {
            get
            {
                return this._LastName;
            }
            set
            {
                this._LastName = value;
            }
        }

        public string LogoFile
        {
            get
            {
                return this._LogoFile;
            }
            set
            {
                this._LogoFile = value;
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

        public string PostalCode
        {
            get
            {
                return this._PostalCode;
            }
            set
            {
                this._PostalCode = value;
            }
        }

        public string Region
        {
            get
            {
                return this._Region;
            }
            set
            {
                this._Region = value;
            }
        }

        public string Street
        {
            get
            {
                return this._Street;
            }
            set
            {
                this._Street = value;
            }
        }

        public string Telephone
        {
            get
            {
                return this._Telephone;
            }
            set
            {
                this._Telephone = value;
            }
        }

        public string Unit
        {
            get
            {
                return this._Unit;
            }
            set
            {
                this._Unit = value;
            }
        }

        public string UserName
        {
            get
            {
                return this._UserName;
            }
            set
            {
                this._UserName = value;
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

        public string VendorName
        {
            get
            {
                return this._VendorName;
            }
            set
            {
                this._VendorName = value;
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

        public string Website
        {
            get
            {
                return this._Website;
            }
            set
            {
                this._Website = value;
            }
        }
    }
}