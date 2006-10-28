using System;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Services.FileSystem;

namespace DotNetNuke.Entities.Portals
{
    [XmlRoot( "settings", IsNullable = false )]
    public class PortalInfo
    {
        private int _AdministratorId;
        private int _AdministratorRoleId;
        private string _AdministratorRoleName;
        private int _AdminTabId;
        private string _BackgroundFile;
        private int _BannerAdvertising;
        private string _Currency;
        private string _DefaultLanguage;
        private string _Description;
        private string _Email;
        private DateTime _ExpiryDate;
        private string _FooterText;
        private Guid _GUID;
        private string _HomeDirectory;
        private int _HomeTabId;
        private float _HostFee;
        private int _HostSpace;
        private string _KeyWords;
        private int _LoginTabId;
        private string _LogoFile;
        private string _PaymentProcessor;
        private int _PortalID;
        private string _PortalName;
        private string _ProcessorPassword;
        private string _ProcessorUserId;
        private int _RegisteredRoleId;
        private string _RegisteredRoleName;
        private int _SiteLogHistory;
        private int _SplashTabId;
        private int _SuperTabId;
        private int _TimeZoneOffset;
        private int _UserRegistration;
        private int _Users;
        private int _UserTabId;
        private string _Version;

        [XmlElementAttribute( "administratorid" )]
        public int AdministratorId
        {
            get
            {
                return this._AdministratorId;
            }
            set
            {
                this._AdministratorId = value;
            }
        }

        [XmlElementAttribute( "administratorroleid" )]
        public int AdministratorRoleId
        {
            get
            {
                return this._AdministratorRoleId;
            }
            set
            {
                this._AdministratorRoleId = value;
            }
        }

        [XmlElementAttribute( "administratorrolename" )]
        public string AdministratorRoleName
        {
            get
            {
                return this._AdministratorRoleName;
            }
            set
            {
                this._AdministratorRoleName = value;
            }
        }

        [XmlElementAttribute( "admintabid" )]
        public int AdminTabId
        {
            get
            {
                return this._AdminTabId;
            }
            set
            {
                this._AdminTabId = value;
            }
        }

        [XmlElementAttribute( "backgroundfile" )]
        public string BackgroundFile
        {
            get
            {
                return this._BackgroundFile;
            }
            set
            {
                this._BackgroundFile = value;
            }
        }

        [XmlElementAttribute( "banneradvertising" )]
        public int BannerAdvertising
        {
            get
            {
                return this._BannerAdvertising;
            }
            set
            {
                this._BannerAdvertising = value;
            }
        }

        [XmlElementAttribute( "currency" )]
        public string Currency
        {
            get
            {
                return this._Currency;
            }
            set
            {
                this._Currency = value;
            }
        }

        [XmlElementAttribute( "defaultlanguage" )]
        public string DefaultLanguage
        {
            get
            {
                return this._DefaultLanguage;
            }
            set
            {
                this._DefaultLanguage = value;
            }
        }

        [XmlElementAttribute( "description" )]
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

        [XmlElementAttribute( "email" )]
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

        [XmlElementAttribute( "expirydate" )]
        public DateTime ExpiryDate
        {
            get
            {
                return this._ExpiryDate;
            }
            set
            {
                this._ExpiryDate = value;
            }
        }

        [XmlElementAttribute( "footertext" )]
        public string FooterText
        {
            get
            {
                return this._FooterText;
            }
            set
            {
                this._FooterText = value;
            }
        }

        [XmlIgnoreAttribute()]
        public Guid GUID
        {
            get
            {
                return this._GUID;
            }
            set
            {
                this._GUID = value;
            }
        }

        [XmlElementAttribute( "homedirectory" )]
        public string HomeDirectory
        {
            get
            {
                return this._HomeDirectory;
            }
            set
            {
                this._HomeDirectory = value;
            }
        }

        [XmlIgnoreAttribute()]
        public string HomeDirectoryMapPath
        {
            get
            {
                FolderController folderController1 = new FolderController();
                return folderController1.GetMappedDirectory( ( Globals.ApplicationPath + "/" + this.HomeDirectory + "/" ) );
            }
        }

        [XmlElementAttribute( "hometabid" )]
        public int HomeTabId
        {
            get
            {
                return this._HomeTabId;
            }
            set
            {
                this._HomeTabId = value;
            }
        }

        [XmlElementAttribute( "hostfee" )]
        public float HostFee
        {
            get
            {
                return this._HostFee;
            }
            set
            {
                this._HostFee = value;
            }
        }

        [XmlElementAttribute( "hostspace" )]
        public int HostSpace
        {
            get
            {
                return this._HostSpace;
            }
            set
            {
                this._HostSpace = value;
            }
        }

        [XmlElementAttribute( "keywords" )]
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

        [XmlElementAttribute( "logintabid" )]
        public int LoginTabId
        {
            get
            {
                return this._LoginTabId;
            }
            set
            {
                this._LoginTabId = value;
            }
        }

        [XmlElementAttribute( "logofile" )]
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

        [XmlElementAttribute( "paymentprocessor" )]
        public string PaymentProcessor
        {
            get
            {
                return this._PaymentProcessor;
            }
            set
            {
                this._PaymentProcessor = value;
            }
        }

        [XmlElementAttribute( "portalid" )]
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

        [XmlElementAttribute( "portalname" )]
        public string PortalName
        {
            get
            {
                return this._PortalName;
            }
            set
            {
                this._PortalName = value;
            }
        }

        [XmlElementAttribute( "processorpassword" )]
        public string ProcessorPassword
        {
            get
            {
                return this._ProcessorPassword;
            }
            set
            {
                this._ProcessorPassword = value;
            }
        }

        [XmlElementAttribute( "processoruserid" )]
        public string ProcessorUserId
        {
            get
            {
                return this._ProcessorUserId;
            }
            set
            {
                this._ProcessorUserId = value;
            }
        }

        [XmlElementAttribute( "registeredroleid" )]
        public int RegisteredRoleId
        {
            get
            {
                return this._RegisteredRoleId;
            }
            set
            {
                this._RegisteredRoleId = value;
            }
        }

        [XmlElementAttribute( "registeredrolename" )]
        public string RegisteredRoleName
        {
            get
            {
                return this._RegisteredRoleName;
            }
            set
            {
                this._RegisteredRoleName = value;
            }
        }

        [XmlElementAttribute( "siteloghistory" )]
        public int SiteLogHistory
        {
            get
            {
                return this._SiteLogHistory;
            }
            set
            {
                this._SiteLogHistory = value;
            }
        }

        [XmlElementAttribute( "splashtabid" )]
        public int SplashTabId
        {
            get
            {
                return this._SplashTabId;
            }
            set
            {
                this._SplashTabId = value;
            }
        }

        [XmlElementAttribute( "supertabid" )]
        public int SuperTabId
        {
            get
            {
                return this._SuperTabId;
            }
            set
            {
                this._SuperTabId = value;
            }
        }

        [XmlElementAttribute( "timezoneoffset" )]
        public int TimeZoneOffset
        {
            get
            {
                return this._TimeZoneOffset;
            }
            set
            {
                this._TimeZoneOffset = value;
            }
        }

        [XmlElementAttribute( "userregistration" )]
        public int UserRegistration
        {
            get
            {
                return this._UserRegistration;
            }
            set
            {
                this._UserRegistration = value;
            }
        }

        [XmlElementAttribute( "users" )]
        public int Users
        {
            get
            {
                return this._Users;
            }
            set
            {
                this._Users = value;
            }
        }

        [XmlElementAttribute( "usertabid" )]
        public int UserTabId
        {
            get
            {
                return this._UserTabId;
            }
            set
            {
                this._UserTabId = value;
            }
        }

        [XmlElementAttribute( "version" )]
        public string Version
        {
            get
            {
                return this._Version;
            }
            set
            {
                this._Version = value;
            }
        }
    }
}