using System;
using System.Collections;
using System.Xml.Serialization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;

namespace DotNetNuke.Entities.Modules
{
    [XmlRoot("module", IsNullable = false)]
    public class ModuleInfo
    {
        private int _PortalID;
        private int _TabID;
        private int _TabModuleID;
        private int _ModuleID;
        private int _ModuleDefID;
        private int _ModuleOrder;
        private string _PaneName;
        private string _ModuleTitle;
        private string _AuthorizedEditRoles;
        private int _CacheTime;
        private string _AuthorizedViewRoles;
        private string _Alignment;
        private string _Color;
        private string _Border;
        private string _IconFile;
        private bool _AllTabs;
        private VisibilityState _Visibility;
        private string _AuthorizedRoles;
        private bool _IsDeleted;
        private string _Header;
        private string _Footer;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private string _ContainerSrc;
        private bool _DisplayTitle;
        private bool _DisplayPrint;
        private bool _DisplaySyndicate;
        private bool _InheritViewPermissions;
        private ModulePermissionCollection _ModulePermissions;
        private int _DesktopModuleID;
        private string _FriendlyName;
        private string _Description;
        private string _Version;
        private bool _IsPremium;
        private bool _IsAdmin;
        private string _BusinessControllerClass;
        private int _SupportedFeatures;
        private int _ModuleControlId;
        private string _ControlSrc;
        private SecurityAccessLevel _ControlType;
        private string _ControlTitle;
        private string _HelpUrl;
        private string _ContainerPath;
        private int _PaneModuleIndex;
        private int _PaneModuleCount;
        private bool _IsDefaultModule;
        private bool _AllModules;

        public ModuleInfo()
        {
            //initialize the properties that can be null
            //in the database
            _PortalID = Null.NullInteger;
            _TabID = Null.NullInteger;
            _TabModuleID = Null.NullInteger;
            _ModuleID = Null.NullInteger;
            _ModuleDefID = Null.NullInteger;
            _ModuleTitle = Null.NullString;
            _AuthorizedEditRoles = Null.NullString;
            _AuthorizedViewRoles = Null.NullString;
            _Alignment = Null.NullString;
            _Color = Null.NullString;
            _Border = Null.NullString;
            _IconFile = Null.NullString;
            _Header = Null.NullString;
            _Footer = Null.NullString;
            _StartDate = Null.NullDate;
            _EndDate = Null.NullDate;
            _ContainerSrc = Null.NullString;
            _DisplayTitle = true;
            _DisplayPrint = true;
            _DisplaySyndicate = false;
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
        public int TabModuleID
        {
            get
            {
                return _TabModuleID;
            }
            set
            {
                _TabModuleID = value;
            }
        }

        [XmlElement("moduleID")]
        public int ModuleID
        {
            get
            {
                return _ModuleID;
            }
            set
            {
                _ModuleID = value;
            }
        }

        [XmlIgnore()]
        public int ModuleDefID
        {
            get
            {
                return _ModuleDefID;
            }
            set
            {
                _ModuleDefID = value;
            }
        }

        [XmlIgnore()]
        public int ModuleOrder
        {
            get
            {
                return _ModuleOrder;
            }
            set
            {
                _ModuleOrder = value;
            }
        }

        [XmlIgnore()]
        public string PaneName
        {
            get
            {
                return _PaneName;
            }
            set
            {
                _PaneName = value;
            }
        }

        [XmlElement("title")]
        public string ModuleTitle
        {
            get
            {
                return _ModuleTitle;
            }
            set
            {
                _ModuleTitle = value;
            }
        }

        [XmlIgnore()]
        public string AuthorizedEditRoles
        {
            get
            {
                return _AuthorizedEditRoles;
            }
            set
            {
                _AuthorizedEditRoles = value;
            }
        }

        [XmlElement("cachetime")]
        public int CacheTime
        {
            get
            {
                return _CacheTime;
            }
            set
            {
                _CacheTime = value;
            }
        }

        [XmlIgnore()]
        public string AuthorizedViewRoles
        {
            get
            {
                return _AuthorizedViewRoles;
            }
            set
            {
                _AuthorizedViewRoles = value;
            }
        }

        [XmlElement("alignment")]
        public string Alignment
        {
            get
            {
                return _Alignment;
            }
            set
            {
                _Alignment = value;
            }
        }

        [XmlElement("color")]
        public string Color
        {
            get
            {
                return _Color;
            }
            set
            {
                _Color = value;
            }
        }

        [XmlElement("border")]
        public string Border
        {
            get
            {
                return _Border;
            }
            set
            {
                _Border = value;
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

        [XmlElement("alltabs")]
        public bool AllTabs
        {
            get
            {
                return _AllTabs;
            }
            set
            {
                _AllTabs = value;
            }
        }

        [XmlElement("visibility")]
        public VisibilityState Visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                _Visibility = value;
            }
        }

        //should be deprecated due to roles being abstracted
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

        [XmlElement("header")]
        public string Header
        {
            get
            {
                return _Header;
            }
            set
            {
                _Header = value;
            }
        }

        [XmlElement("footer")]
        public string Footer
        {
            get
            {
                return _Footer;
            }
            set
            {
                _Footer = value;
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

        [XmlElement("displaytitle")]
        public bool DisplayTitle
        {
            get
            {
                return _DisplayTitle;
            }
            set
            {
                _DisplayTitle = value;
            }
        }

        [XmlElement("displayprint")]
        public bool DisplayPrint
        {
            get
            {
                return _DisplayPrint;
            }
            set
            {
                _DisplayPrint = value;
            }
        }

        [XmlElement("displaysyndicate")]
        public bool DisplaySyndicate
        {
            get
            {
                return _DisplaySyndicate;
            }
            set
            {
                _DisplaySyndicate = value;
            }
        }

        [XmlElement("inheritviewpermissions")]
        public bool InheritViewPermissions
        {
            get
            {
                return _InheritViewPermissions;
            }
            set
            {
                _InheritViewPermissions = value;
            }
        }

        [XmlArray("modulepermissions"), XmlArrayItem("permission")]
        public ModulePermissionCollection ModulePermissions
        {
            get
            {
                return _ModulePermissions;
            }
            set
            {
                _ModulePermissions = value;
            }
        }

        [XmlIgnore()]
        public int DesktopModuleID
        {
            get
            {
                return _DesktopModuleID;
            }
            set
            {
                _DesktopModuleID = value;
            }
        }

        [XmlIgnore()]
        public string FriendlyName
        {
            get
            {
                return _FriendlyName;
            }
            set
            {
                _FriendlyName = value;
            }
        }

        [XmlIgnore()]
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

        [XmlIgnore()]
        public string Version
        {
            get
            {
                return _Version;
            }
            set
            {
                _Version = value;
            }
        }

        [XmlIgnore()]
        public bool IsPremium
        {
            get
            {
                return _IsPremium;
            }
            set
            {
                _IsPremium = value;
            }
        }

        [XmlIgnore()]
        public bool IsAdmin
        {
            get
            {
                return _IsAdmin;
            }
            set
            {
                _IsAdmin = value;
            }
        }

        [XmlIgnore()]
        public string BusinessControllerClass
        {
            get
            {
                return _BusinessControllerClass;
            }
            set
            {
                _BusinessControllerClass = value;
            }
        }

        [XmlIgnore()]
        public int ModuleControlId
        {
            get
            {
                return _ModuleControlId;
            }
            set
            {
                _ModuleControlId = value;
            }
        }

        [XmlIgnore()]
        public string ControlSrc
        {
            get
            {
                return _ControlSrc;
            }
            set
            {
                _ControlSrc = value;
            }
        }

        [XmlIgnore()]
        public SecurityAccessLevel ControlType
        {
            get
            {
                return _ControlType;
            }
            set
            {
                _ControlType = value;
            }
        }

        [XmlIgnore()]
        public string ControlTitle
        {
            get
            {
                return _ControlTitle;
            }
            set
            {
                _ControlTitle = value;
            }
        }

        [XmlIgnore()]
        public string HelpUrl
        {
            get
            {
                return _HelpUrl;
            }
            set
            {
                _HelpUrl = value;
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
        public int PaneModuleIndex
        {
            get
            {
                return _PaneModuleIndex;
            }
            set
            {
                _PaneModuleIndex = value;
            }
        }

        [XmlIgnore()]
        public int PaneModuleCount
        {
            get
            {
                return _PaneModuleCount;
            }
            set
            {
                _PaneModuleCount = value;
            }
        }

        [XmlIgnore()]
        public bool IsDefaultModule
        {
            get
            {
                return _IsDefaultModule;
            }
            set
            {
                _IsDefaultModule = value;
            }
        }

        [XmlIgnore()]
        public bool AllModules
        {
            get
            {
                return _AllModules;
            }
            set
            {
                _AllModules = value;
            }
        }

        [XmlIgnore()]
        public int SupportedFeatures
        {
            get
            {
                return (_SupportedFeatures);
            }
            set
            {
                _SupportedFeatures = value;
            }
        }

        [XmlIgnore()]
        public bool IsPortable
        {
            get
            {
                return GetFeature(DesktopModuleSupportedFeature.IsPortable);
            }
        }

        [XmlIgnore()]
        public bool IsSearchable
        {
            get
            {
                return GetFeature(DesktopModuleSupportedFeature.IsSearchable);
            }
        }

        [XmlIgnore()]
        public bool IsUpgradeable
        {
            get
            {
                return GetFeature(DesktopModuleSupportedFeature.IsUpgradeable);
            }
        }

        public void Initialize(int PortalId)
        {
            _PortalID = PortalId;
            _TabID = -1;
            _ModuleID = -1;
            _ModuleDefID = -1;
            _ModuleOrder = -1;
            _PaneName = "";
            _ModuleTitle = "";
            _AuthorizedEditRoles = "";
            _CacheTime = -1;
            _AuthorizedViewRoles = "";
            _Alignment = "";
            _Color = "";
            _Border = "";
            _IconFile = "";
            _AllTabs = false;
            _Visibility = VisibilityState.Maximized;
            _IsDeleted = false;
            _Header = "";
            _Footer = "";
            _StartDate = Null.NullDate;
            _EndDate = Null.NullDate;
            _DisplayTitle = true;
            _DisplayPrint = true;
            _DisplaySyndicate = false;
            _InheritViewPermissions = false;
            _ContainerSrc = "";
            _DesktopModuleID = -1;
            _FriendlyName = "";
            _Description = "";
            _Version = "";
            _IsPremium = false;
            _IsAdmin = false;
            _SupportedFeatures = 0;
            _BusinessControllerClass = "";
            _ModuleControlId = -1;
            _ControlSrc = "";
            _ControlType = SecurityAccessLevel.Anonymous;
            _ControlTitle = "";
            _HelpUrl = "";
            _ContainerPath = "";
            _PaneModuleIndex = 0;
            _PaneModuleCount = 0;
            _IsDefaultModule = false;
            _AllModules = false;

            // get default module settings
            Hashtable settings = PortalSettings.GetSiteSettings(PortalId);
            if (Convert.ToString(settings["defaultmoduleid"]) != "" && Convert.ToString(settings["defaulttabid"]) != "")
            {
                ModuleController objModules = new ModuleController();
                ModuleInfo objModule = objModules.GetModule(int.Parse(Convert.ToString(settings["defaultmoduleid"])), int.Parse(Convert.ToString(settings["defaulttabid"])));
                if (objModule != null)
                {
                    _CacheTime = objModule.CacheTime;
                    _Alignment = objModule.Alignment;
                    _Color = objModule.Color;
                    _Border = objModule.Border;
                    _IconFile = objModule.IconFile;
                    _Visibility = objModule.Visibility;
                    _ContainerSrc = objModule.ContainerSrc;
                    _DisplayTitle = objModule.DisplayTitle;
                    _DisplayPrint = objModule.DisplayPrint;
                    _DisplaySyndicate = objModule.DisplaySyndicate;
                }
            }
        }

        public ModuleInfo Clone()
        {
            // create the object
            ModuleInfo objModuleInfo = new ModuleInfo();

            // assign the property values
            objModuleInfo.PortalID = this.PortalID;
            objModuleInfo.TabID = this.TabID;
            objModuleInfo.TabModuleID = this.TabModuleID;
            objModuleInfo.ModuleID = this.ModuleID;
            objModuleInfo.ModuleDefID = this.ModuleDefID;
            objModuleInfo.ModuleOrder = this.ModuleOrder;
            objModuleInfo.PaneName = this.PaneName;
            objModuleInfo.ModuleTitle = this.ModuleTitle;
            objModuleInfo.AuthorizedEditRoles = this.AuthorizedEditRoles;
            objModuleInfo.CacheTime = this.CacheTime;
            objModuleInfo.AuthorizedViewRoles = this.AuthorizedViewRoles;
            objModuleInfo.Alignment = this.Alignment;
            objModuleInfo.Color = this.Color;
            objModuleInfo.Border = this.Border;
            objModuleInfo.IconFile = this.IconFile;
            objModuleInfo.AllTabs = this.AllTabs;
            objModuleInfo.Visibility = this.Visibility;
            objModuleInfo.AuthorizedRoles = this.AuthorizedRoles;
            objModuleInfo.IsDeleted = this.IsDeleted;
            objModuleInfo.Header = this.Header;
            objModuleInfo.Footer = this.Footer;
            objModuleInfo.StartDate = this.StartDate;
            objModuleInfo.EndDate = this.EndDate;
            objModuleInfo.ContainerSrc = this.ContainerSrc;
            objModuleInfo.DisplayTitle = this.DisplayTitle;
            objModuleInfo.DisplayPrint = this.DisplayPrint;
            objModuleInfo.DisplaySyndicate = this.DisplaySyndicate;
            objModuleInfo.InheritViewPermissions = this.InheritViewPermissions;
            objModuleInfo.DesktopModuleID = this.DesktopModuleID;
            objModuleInfo.FriendlyName = this.FriendlyName;
            objModuleInfo.Description = this.Description;
            objModuleInfo.Version = this.Version;
            objModuleInfo.IsAdmin = this.IsAdmin;
            objModuleInfo.IsPremium = this.IsPremium;
            objModuleInfo.SupportedFeatures = this.SupportedFeatures;
            objModuleInfo.BusinessControllerClass = this.BusinessControllerClass;
            objModuleInfo.ModuleControlId = this.ModuleControlId;
            objModuleInfo.ControlSrc = this.ControlSrc;
            objModuleInfo.ControlType = this.ControlType;
            objModuleInfo.ControlTitle = this.ControlTitle;
            objModuleInfo.HelpUrl = this.HelpUrl;
            objModuleInfo.ContainerPath = this.ContainerPath;
            objModuleInfo.PaneModuleIndex = this.PaneModuleIndex;
            objModuleInfo.PaneModuleCount = this.PaneModuleCount;
            objModuleInfo.IsDefaultModule = this.IsDefaultModule;
            objModuleInfo.AllModules = this.AllModules;

            objModuleInfo.ModulePermissions = this.ModulePermissions;

            return objModuleInfo;
        }

        private bool GetFeature(DesktopModuleSupportedFeature Feature)
        {
            bool isSet = false;
            //And with the Feature to see if the flag is set
            if (((DesktopModuleSupportedFeature)SupportedFeatures & Feature) == Feature)
            {
                isSet = true;
            }

            return isSet;
        }
    }
}