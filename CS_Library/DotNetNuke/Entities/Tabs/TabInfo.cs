using System;
using System.Collections;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Permissions;

namespace DotNetNuke.Entities.Tabs
{
    [XmlRoot("tab", IsNullable = false)]
    public class TabInfo
    {
        private int _TabID;
        private int _TabOrder;
        private int _PortalID;
        private string _TabName;
        private string _AuthorizedRoles;
        private bool _IsVisible;
        private int _ParentId;
        private int _Level;
        private string _IconFile;
        private string _AdministratorRoles;
        private bool _DisableLink;
        private string _Title;
        private string _Description;
        private string _KeyWords;
        private bool _IsDeleted;
        private string _Url;
        private string _SkinSrc;
        private string _ContainerSrc;
        private string _TabPath;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private TabPermissionCollection _TabPermissions;
        private bool _HasChildren;
        private int _RefreshInterval;
        private string _PageHeadText;

        // properties loaded in PortalSettings
        private string _SkinPath;
        private string _ContainerPath;
        private ArrayList _BreadCrumbs;
        private ArrayList _Panes;
        private ArrayList _Modules;
        private bool _IsSuperTab;

        public TabInfo()
        {
            //initialize the properties that
            //can be null in the database
            _PortalID = Null.NullInteger;
            _AuthorizedRoles = Null.NullString;
            _ParentId = Null.NullInteger;
            _IconFile = Null.NullString;
            _AdministratorRoles = Null.NullString;
            _Title = Null.NullString;
            _Description = Null.NullString;
            _KeyWords = Null.NullString;
            _Url = Null.NullString;
            _SkinSrc = Null.NullString;
            _ContainerSrc = Null.NullString;
            _TabPath = Null.NullString;
            _StartDate = Null.NullDate;
            _EndDate = Null.NullDate;
            _RefreshInterval = Null.NullInteger;
            _PageHeadText = Null.NullString;
        }

        [XmlIgnore()]
        public int TabID
        {
            get
            {
                return _TabID;
            }
            set
            {
                _TabID = value;
            }
        }

        [XmlIgnore()]
        public int TabOrder
        {
            get
            {
                return _TabOrder;
            }
            set
            {
                _TabOrder = value;
            }
        }

        [XmlIgnore()]
        public int PortalID
        {
            get
            {
                return _PortalID;
            }
            set
            {
                _PortalID = value;
            }
        }

        [XmlElement("name")]
        public string TabName
        {
            get
            {
                return _TabName;
            }
            set
            {
                _TabName = value;
            }
        }

        [XmlIgnore()]
        public string AuthorizedRoles
        {
            get
            {
                return _AuthorizedRoles;
            }
            set
            {
                _AuthorizedRoles = value;
            }
        }

        [XmlElement("visible")]
        public bool IsVisible
        {
            get
            {
                return _IsVisible;
            }
            set
            {
                _IsVisible = value;
            }
        }

        [XmlIgnore()]
        public int ParentId
        {
            get
            {
                return _ParentId;
            }
            set
            {
                _ParentId = value;
            }
        }

        [XmlIgnore()]
        public int Level
        {
            get
            {
                return _Level;
            }
            set
            {
                _Level = value;
            }
        }

        [XmlElement("iconfile")]
        public string IconFile
        {
            get
            {
                return _IconFile;
            }
            set
            {
                _IconFile = value;
            }
        }

        [XmlIgnore()]
        public string AdministratorRoles
        {
            get
            {
                return _AdministratorRoles;
            }
            set
            {
                _AdministratorRoles = value;
            }
        }

        [XmlElement("disabled")]
        public bool DisableLink
        {
            get
            {
                return _DisableLink;
            }
            set
            {
                _DisableLink = value;
            }
        }

        [XmlElement("title")]
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }

        [XmlElement("description")]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        [XmlElement("keywords")]
        public string KeyWords
        {
            get
            {
                return _KeyWords;
            }
            set
            {
                _KeyWords = value;
            }
        }

        [XmlIgnore()]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                _IsDeleted = value;
            }
        }

        [XmlElement("url")]
        public string Url
        {
            get
            {
                return _Url;
            }
            set
            {
                _Url = value;
            }
        }

        [XmlElement("skinsrc")]
        public string SkinSrc
        {
            get
            {
                return _SkinSrc;
            }
            set
            {
                _SkinSrc = value;
            }
        }

        [XmlElement("containersrc")]
        public string ContainerSrc
        {
            get
            {
                return _ContainerSrc;
            }
            set
            {
                _ContainerSrc = value;
            }
        }

        [XmlIgnore()]
        public string TabPath
        {
            get
            {
                return _TabPath;
            }
            set
            {
                _TabPath = value;
            }
        }

        [XmlElement("startdate")]
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                _StartDate = value;
            }
        }

        [XmlElement("enddate")]
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                _EndDate = value;
            }
        }

        [XmlArray("tabpermissions"), XmlArrayItem("permission")]
        public TabPermissionCollection TabPermissions
        {
            get
            {
                return _TabPermissions;
            }
            set
            {
                _TabPermissions = value;
            }
        }

        [XmlIgnore()]
        public bool HasChildren
        {
            get
            {
                return _HasChildren;
            }
            set
            {
                _HasChildren = value;
            }
        }

        // properties loaded in PortalSettings
        [XmlIgnore()]
        public string SkinPath
        {
            get
            {
                return _SkinPath;
            }
            set
            {
                _SkinPath = value;
            }
        }

        [XmlIgnore()]
        public string ContainerPath
        {
            get
            {
                return _ContainerPath;
            }
            set
            {
                _ContainerPath = value;
            }
        }

        [XmlIgnore()]
        public ArrayList BreadCrumbs
        {
            get
            {
                return _BreadCrumbs;
            }
            set
            {
                _BreadCrumbs = value;
            }
        }

        [XmlIgnore()]
        public ArrayList Panes
        {
            get
            {
                return _Panes;
            }
            set
            {
                _Panes = value;
            }
        }

        [XmlIgnore()]
        public ArrayList Modules
        {
            get
            {
                return _Modules;
            }
            set
            {
                _Modules = value;
            }
        }

        [XmlIgnore()]
        public bool IsSuperTab
        {
            get
            {
                return _IsSuperTab;
            }
            set
            {
                _IsSuperTab = value;
            }
        }

        [XmlIgnore()]
        public TabType TabType
        {
            get
            {
                return Globals.GetURLType(_Url);
            }
        }

        [XmlIgnore()]
        public string FullUrl
        {
            get
            {
                string strUrl = "";

                switch (TabType)
                {
                    case TabType.Normal:

                        // normal tab
                        strUrl = Globals.NavigateURL(TabID, IsSuperTab);
                        break;
                    case TabType.Tab:

                        // alternate tab url
                        strUrl = Globals.NavigateURL(Convert.ToInt32(_Url));
                        break;
                    case TabType.File:

                        // file url
                        PortalSettings settings = PortalController.GetCurrentPortalSettings();
                        strUrl = settings.HomeDirectory + _Url;
                        break;
                    case TabType.Url:

                        // external url
                        strUrl = _Url;
                        break;
                }

                return strUrl;
            }
        }

        [XmlElement("refreshinterval")]
        public int RefreshInterval
        {
            get
            {
                return _RefreshInterval;
            }
            set
            {
                _RefreshInterval = value;
            }
        }

        [XmlElement("pageheadtext")]
        public string PageHeadText
        {
            get
            {
                return _PageHeadText;
            }
            set
            {
                _PageHeadText = value;
            }
        }

        public bool IsAdminTab
        {
            get
            {
                if (IsSuperTab || PortalID == Null.NullInteger)
                {
                    //Host Tab
                    return true;
                }
                else
                {
                    //Portal Tab

                    //Get Portal Settings
                    PortalSettings settings = PortalController.GetCurrentPortalSettings();

                    if (settings == null)
                    {
                        //If there is no setings (no context) Get PortalInfo object from DB
                        PortalController objPortalController = new PortalController();
                        PortalInfo objPortal = objPortalController.GetPortal(PortalID);

                        return (TabID == objPortal.AdminTabId) || (ParentId == objPortal.AdminTabId);
                    }
                    else
                    {
                        return (TabID == settings.AdminTabId) || (ParentId == settings.AdminTabId);
                    }
                }
            }
        }

        public TabInfo Clone()
        {
            // create the object
            TabInfo objTabInfo = new TabInfo();

            // assign the property values
            objTabInfo.TabID = this.TabID;
            objTabInfo.TabOrder = this.TabOrder;
            objTabInfo.PortalID = this.PortalID;
            objTabInfo.TabName = this.TabName;
            objTabInfo.AuthorizedRoles = this.AuthorizedRoles;
            objTabInfo.IsVisible = this.IsVisible;
            objTabInfo.ParentId = this.ParentId;
            objTabInfo.Level = this.Level;
            objTabInfo.IconFile = this.IconFile;
            objTabInfo.AdministratorRoles = this.AdministratorRoles;
            objTabInfo.DisableLink = this.DisableLink;
            objTabInfo.Title = this.Title;
            objTabInfo.Description = this.Description;
            objTabInfo.KeyWords = this.KeyWords;
            objTabInfo.IsDeleted = this.IsDeleted;
            objTabInfo.Url = this.Url;
            objTabInfo.SkinSrc = this.SkinSrc;
            objTabInfo.ContainerSrc = this.ContainerSrc;
            objTabInfo.TabPath = this.TabPath;
            objTabInfo.StartDate = this.StartDate;
            objTabInfo.EndDate = this.EndDate;
            objTabInfo.TabPermissions = this.TabPermissions;
            objTabInfo.HasChildren = this.HasChildren;
            objTabInfo.SkinPath = this.SkinPath;
            objTabInfo.ContainerPath = this.ContainerPath;
            objTabInfo.IsSuperTab = this.IsSuperTab;
            objTabInfo.RefreshInterval = this.RefreshInterval;
            objTabInfo.PageHeadText = this.PageHeadText;
            if (this.BreadCrumbs != null)
            {
                objTabInfo.BreadCrumbs = new ArrayList();
                foreach (TabInfo t in this.BreadCrumbs)
                {
                    objTabInfo.BreadCrumbs.Add(t.Clone());
                }
            }

            // initialize collections which are populated later
            objTabInfo.Panes = new ArrayList();
            objTabInfo.Modules = new ArrayList();

            return objTabInfo;
        }
    }
}