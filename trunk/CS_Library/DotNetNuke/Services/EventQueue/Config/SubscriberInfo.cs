using System;
using DotNetNuke.Security;

namespace DotNetNuke.Services.EventQueue.Config
{
    public class SubscriberInfo
    {
        private string _address;
        private string _description;
        private string _id;
        private string _name;
        private string _privateKey;

        public SubscriberInfo()
        {
            this._id = Guid.NewGuid().ToString();
            this._name = "";
            this._description = "";
            this._address = "";
            PortalSecurity portalSecurity = new PortalSecurity();
            this._privateKey = portalSecurity.CreateKey( 16 );
        }

        public SubscriberInfo( string subscriberName ) : this()
        {
            this._name = subscriberName;
        }

        public string Address
        {
            get
            {
                return this._address;
            }
            set
            {
                this._address = value;
            }
        }

        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
            }
        }

        public string ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public string PrivateKey
        {
            get
            {
                return this._privateKey;
            }
            set
            {
                this._privateKey = value;
            }
        }
    }
}