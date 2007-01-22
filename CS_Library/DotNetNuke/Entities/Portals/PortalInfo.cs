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
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
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
        private int _PageQuota;
        private int _UserQuota;
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
        private int _Users = Null.NullInteger;
        private int _Pages = Null.NullInteger;
        private int _UserTabId;
        private string _Version;

        [XmlElement( "administratorid" )]
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

        [XmlElement("pagequota")]
        public int PageQuota
        {
            get
            {
                return _PageQuota;
            }
            set
            {
                _PageQuota = value;
            }
        }
        [XmlElement("userquota")]
        public int UserQuota
        {
            get
            {
                return _UserQuota;
            }
            set
            {
                _UserQuota = value;
            }
        }

        [XmlElement( "administratorroleid" )]
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

        [XmlElement( "administratorrolename" )]
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

        [XmlElement( "admintabid" )]
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

        [XmlElement( "backgroundfile" )]
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

        [XmlElement( "banneradvertising" )]
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

        [XmlElement( "currency" )]
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

        [XmlElement( "defaultlanguage" )]
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

        [XmlElement( "description" )]
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

        [XmlElement( "email" )]
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

        [XmlElement( "expirydate" )]
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

        [XmlElement( "footertext" )]
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

        [XmlIgnore()]
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

        [XmlElement( "homedirectory" )]
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

        [XmlIgnore()]
        public string HomeDirectoryMapPath
        {
            get
            {
                FolderController folderController1 = new FolderController();
                return folderController1.GetMappedDirectory( ( Globals.ApplicationPath + "/" + this.HomeDirectory + "/" ) );
            }
        }

        [XmlElement( "hometabid" )]
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

        [XmlElement( "hostfee" )]
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

        [XmlElement( "hostspace" )]
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

        [XmlElement( "keywords" )]
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

        [XmlElement( "logintabid" )]
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

        [XmlElement( "logofile" )]
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

        [XmlElement( "paymentprocessor" )]
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

        [XmlElement( "portalid" )]
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

        [XmlElement( "portalname" )]
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

        [XmlElement( "processorpassword" )]
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

        [XmlElement( "processoruserid" )]
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

        [XmlElement( "registeredroleid" )]
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

        [XmlElement( "registeredrolename" )]
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

        [XmlElement( "siteloghistory" )]
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

        [XmlElement( "splashtabid" )]
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

        [XmlElement( "supertabid" )]
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

        [XmlElement( "timezoneoffset" )]
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

        [XmlElement( "userregistration" )]
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

        [XmlElement( "users" )]
        public int Users
        {
            get
            {                
                if (_Users < 0)
                {
                    _Users = UserController.GetUserCountByPortal(PortalID);
                }
                return this._Users;
            }
            set
            {
                this._Users = value;
            }
        }

        [XmlElement("pages")]
        public int Pages
        {
            get
            {
                if (_Pages < 0)
                {
                    TabController objTabController = new TabController();
                    _Pages = objTabController.GetTabCount(PortalID);
                }
                return _Pages;
            }
            set
            {
                _Pages = value;
            }
        }

        [XmlElement( "usertabid" )]
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

        [XmlElement( "version" )]
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