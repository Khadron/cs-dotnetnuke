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