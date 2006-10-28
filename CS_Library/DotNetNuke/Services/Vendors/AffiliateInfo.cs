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