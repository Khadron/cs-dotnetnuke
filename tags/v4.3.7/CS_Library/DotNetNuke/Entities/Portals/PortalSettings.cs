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
using System.Collections;
using System.Data;
using System.IO;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;

namespace DotNetNuke.Entities.Portals
{
    /// <Summary>
    /// PortalSettings Class
    /// This class encapsulates all of the settings for the Portal, as well
    /// as the configuration settings required to execute the current tab
    /// view within the portal.
    /// </Summary>
    public class PortalSettings
    {
        private int _PortalId;
        private string _PortalName;
        private string _HomeDirectory;
        private string _LogoFile;
        private string _FooterText;
        private DateTime _ExpiryDate;
        private int _UserRegistration;
        private int _BannerAdvertising;
        private string _Currency;
        private int _AdministratorId;
        private string _Email;
        private float _HostFee;
        private int _HostSpace;
        private int _AdministratorRoleId;
        private string _AdministratorRoleName;
        private int _RegisteredRoleId;
        private string _RegisteredRoleName;
        private string _Description;
        private string _KeyWords;
        private string _BackgroundFile;
        private Guid _GUID;
        private int _SiteLogHistory;
        private int _AdminTabId;
        private int _SuperTabId;
        private int _SplashTabId;
        private int _HomeTabId;
        private int _LoginTabId;
        private int _UserTabId;
        private string _DefaultLanguage;
        private int _TimeZoneOffset;
        private string _Version;
        private ArrayList _DesktopTabs;
        private TabInfo _ActiveTab;
        private PortalAliasInfo _PortalAlias;

        public int PortalId
        {
            get
            {
                return _PortalId;
            }
            set
            {
                _PortalId = value;
            }
        }

        public string PortalName
        {
            get
            {
                return _PortalName;
            }
            set
            {
                _PortalName = value;
            }
        }

        public string HomeDirectory
        {
            get
            {
                return _HomeDirectory;
            }
            set
            {
                _HomeDirectory = value;
            }
        }

        public string HomeDirectoryMapPath
        {
            get
            {
                FolderController objFolderController = new FolderController();
                return objFolderController.GetMappedDirectory( HomeDirectory );
            }
        }

        public string LogoFile
        {
            get
            {
                return _LogoFile;
            }
            set
            {
                _LogoFile = value;
            }
        }

        public string FooterText
        {
            get
            {
                return _FooterText;
            }
            set
            {
                _FooterText = value;
            }
        }

        public DateTime ExpiryDate
        {
            get
            {
                return _ExpiryDate;
            }
            set
            {
                _ExpiryDate = value;
            }
        }

        public int UserRegistration
        {
            get
            {
                return _UserRegistration;
            }
            set
            {
                _UserRegistration = value;
            }
        }

        public int BannerAdvertising
        {
            get
            {
                return _BannerAdvertising;
            }
            set
            {
                _BannerAdvertising = value;
            }
        }

        public string Currency
        {
            get
            {
                return _Currency;
            }
            set
            {
                _Currency = value;
            }
        }

        public int AdministratorId
        {
            get
            {
                return _AdministratorId;
            }
            set
            {
                _AdministratorId = value;
            }
        }

        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                _Email = value;
            }
        }

        public float HostFee
        {
            get
            {
                return _HostFee;
            }
            set
            {
                _HostFee = value;
            }
        }

        public int HostSpace
        {
            get
            {
                return _HostSpace;
            }
            set
            {
                _HostSpace = value;
            }
        }

        public int AdministratorRoleId
        {
            get
            {
                return _AdministratorRoleId;
            }
            set
            {
                _AdministratorRoleId = value;
            }
        }

        public string AdministratorRoleName
        {
            get
            {
                return _AdministratorRoleName;
            }
            set
            {
                _AdministratorRoleName = value;
            }
        }

        public int RegisteredRoleId
        {
            get
            {
                return _RegisteredRoleId;
            }
            set
            {
                _RegisteredRoleId = value;
            }
        }

        public string RegisteredRoleName
        {
            get
            {
                return _RegisteredRoleName;
            }
            set
            {
                _RegisteredRoleName = value;
            }
        }

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

        public string BackgroundFile
        {
            get
            {
                return _BackgroundFile;
            }
            set
            {
                _BackgroundFile = value;
            }
        }

        public Guid GUID
        {
            get
            {
                return _GUID;
            }
            set
            {
                _GUID = value;
            }
        }

        public int SiteLogHistory
        {
            get
            {
                return _SiteLogHistory;
            }
            set
            {
                _SiteLogHistory = value;
            }
        }

        public int AdminTabId
        {
            get
            {
                return _AdminTabId;
            }
            set
            {
                _AdminTabId = value;
            }
        }

        public int SuperTabId
        {
            get
            {
                return _SuperTabId;
            }
            set
            {
                _SuperTabId = value;
            }
        }

        public int SplashTabId
        {
            get
            {
                return _SplashTabId;
            }
            set
            {
                _SplashTabId = value;
            }
        }

        public int HomeTabId
        {
            get
            {
                return _HomeTabId;
            }
            set
            {
                _HomeTabId = value;
            }
        }

        public int LoginTabId
        {
            get
            {
                return _LoginTabId;
            }
            set
            {
                _LoginTabId = value;
            }
        }

        public int UserTabId
        {
            get
            {
                return _UserTabId;
            }
            set
            {
                _UserTabId = value;
            }
        }

        public string DefaultLanguage
        {
            get
            {
                return _DefaultLanguage;
            }
            set
            {
                _DefaultLanguage = value;
            }
        }

        public int TimeZoneOffset
        {
            get
            {
                return _TimeZoneOffset;
            }
            set
            {
                _TimeZoneOffset = value;
            }
        }

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

        public ArrayList DesktopTabs
        {
            get
            {
                return _DesktopTabs;
            }
            set
            {
                _DesktopTabs = value;
            }
        }

        public TabInfo ActiveTab
        {
            get
            {
                return _ActiveTab;
            }
            set
            {
                _ActiveTab = value;
            }
        }

        public Hashtable HostSettings
        {
            get
            {
                return Globals.HostSettings; //_HostSettings
            }
        }

        public PortalAliasInfo PortalAlias
        {
            get
            {
                return _PortalAlias;
            }
            set
            {
                _PortalAlias = value;
            }
        }

        /// <summary>
        /// The PortalSettings Constructor encapsulates all of the logic
        /// necessary to obtain configuration settings necessary to render
        /// a Portal Tab view for a given request.
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="tabId">The current tab</param>
        ///	<param name="objPortalAliasInfo">The current portal</param>
        public PortalSettings( int tabId, PortalAliasInfo objPortalAliasInfo )
        {
            _DesktopTabs = new ArrayList();
            _ActiveTab = new TabInfo();
            GetPortalSettings( tabId, objPortalAliasInfo );
        }

        public PortalSettings()
        {
        }

        private void GetBreadCrumbsRecursively( ref ArrayList objBreadCrumbs, int intTabId )
        {
            // find the tab in the desktoptabs collection
            bool blnFound = false;
            TabInfo objTab = null;
            foreach( TabInfo tempLoopVar_objTab in this.DesktopTabs )
            {
                objTab = tempLoopVar_objTab;
                if( objTab.TabID == intTabId )
                {
                    blnFound = true;
                    break;
                }
            }            

            // if tab was found
            if( blnFound )
            {
                // add tab to breadcrumb collection
                objBreadCrumbs.Insert( 0, objTab.Clone() );

                // get the tab parent
                if( !Null.IsNull( objTab.ParentId ) )
                {
                    GetBreadCrumbsRecursively( ref objBreadCrumbs, objTab.ParentId );
                }
            }
        }

        /// <summary>
        /// The GetPortalSettings method builds the site Settings
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="TabId">The current tabs id</param>
        ///	<param name="objPortalAliasInfo">The Portal Alias object</param>
        private void GetPortalSettings( int TabId, PortalAliasInfo objPortalAliasInfo )
        {
            PortalController objPortals = new PortalController();
            PortalInfo objPortal;
            TabController objTabs = new TabController();
            ArrayList arrTabs;
            TabInfo objTab;
            ModuleController objModules = new ModuleController();
            ArrayList arrModules;
            ModuleInfo objModule;
            SkinInfo objSkin;

            // data caching settings
            int intCacheTimeout;
            // calculate the cache settings based on the performance setting
            intCacheTimeout = 20*Convert.ToInt32( Globals.PerformanceSetting );

            PortalId = objPortalAliasInfo.PortalID;

            // get portal settings
            objPortal = (PortalInfo)DataCache.GetPersistentCacheItem( "GetPortalSettings" + PortalId.ToString(), typeof( PortalInfo ) );
            if( objPortal == null )
            {
                // get portal settings
                objPortal = objPortals.GetPortal( PortalId );
                if( objPortal != null )
                {
                    // set custom properties
                    if( Null.IsNull( objPortal.HostSpace ) )
                    {
                        objPortal.HostSpace = 0;
                    }
                    if( Null.IsNull( objPortal.DefaultLanguage ) )
                    {
                        objPortal.DefaultLanguage = Localization.SystemLocale;
                    }
                    if( Null.IsNull( objPortal.TimeZoneOffset ) )
                    {
                        objPortal.TimeZoneOffset = Localization.SystemTimeZoneOffset;
                    }
                    objPortal.HomeDirectory = Globals.ApplicationPath + "/" + objPortal.HomeDirectory + "/";

                    // get application version
                    Array arrVersion = Globals.glbAppVersion.Split( Convert.ToChar( "." ) );
                    int intMajor = Convert.ToInt32( arrVersion.GetValue( 0 ) );
                    int intMinor = Convert.ToInt32( arrVersion.GetValue( 1 ) );
                    int intBuild = Convert.ToInt32( arrVersion.GetValue( 2 ) );
                    objPortal.Version = intMajor.ToString() + "." + intMinor.ToString() + "." + intBuild.ToString();

                    // get administrator email
                    UserInfo objUser;
                    objUser = UserController.GetUser( objPortal.PortalID, objPortal.AdministratorId, false );
                    if( objUser != null )
                    {
                        objPortal.Email = objUser.Email;
                    }

                    // cache object
                    if( intCacheTimeout != 0 )
                    {
                        DataCache.SetCache( "GetPortalSettings" + PortalId.ToString(), objPortal, TimeSpan.FromMinutes( intCacheTimeout ), true );
                    }
                }
            }
            if( objPortal != null )
            {
                this.PortalAlias = objPortalAliasInfo;
                this.PortalId = objPortal.PortalID;
                this.PortalName = objPortal.PortalName;
                this.LogoFile = objPortal.LogoFile;
                this.FooterText = objPortal.FooterText;
                this.ExpiryDate = objPortal.ExpiryDate;
                this.UserRegistration = objPortal.UserRegistration;
                this.BannerAdvertising = objPortal.BannerAdvertising;
                this.Currency = objPortal.Currency;
                this.AdministratorId = objPortal.AdministratorId;
                this.Email = objPortal.Email;
                this.HostFee = objPortal.HostFee;
                this.HostSpace = objPortal.HostSpace;
                this.AdministratorRoleId = objPortal.AdministratorRoleId;
                this.AdministratorRoleName = objPortal.AdministratorRoleName;
                this.RegisteredRoleId = objPortal.RegisteredRoleId;
                this.RegisteredRoleName = objPortal.RegisteredRoleName;
                this.Description = objPortal.Description;
                this.KeyWords = objPortal.KeyWords;
                this.BackgroundFile = objPortal.BackgroundFile;
                this.GUID = objPortal.GUID;
                this.SiteLogHistory = objPortal.SiteLogHistory;
                this.AdminTabId = objPortal.AdminTabId;
                this.SuperTabId = objPortal.SuperTabId;
                this.SplashTabId = objPortal.SplashTabId;
                this.HomeTabId = objPortal.HomeTabId;
                this.LoginTabId = objPortal.LoginTabId;
                this.UserTabId = objPortal.UserTabId;
                this.DefaultLanguage = objPortal.DefaultLanguage;
                this.TimeZoneOffset = objPortal.TimeZoneOffset;
                this.HomeDirectory = objPortal.HomeDirectory;
                this.Version = objPortal.Version;
            }

            // get portal tabs
            arrTabs = (ArrayList)DataCache.GetCache( "GetTabs" + this.PortalId.ToString() );
            if( arrTabs == null )
            {
                arrTabs = objTabs.GetTabs( this.PortalId );
                if( arrTabs != null )
                {
                    // set custom properties
                    foreach( TabInfo tempLoopVar_objTab in arrTabs )
                    {
                        objTab = tempLoopVar_objTab;
                        if( objTab.TabOrder == 0 )
                        {
                            objTab.TabOrder = 999;
                        }
                        if( Null.IsNull( objTab.StartDate ) )
                        {
                            objTab.StartDate = DateTime.MinValue;
                        }
                        if( Null.IsNull( objTab.EndDate ) )
                        {
                            objTab.EndDate = DateTime.MaxValue;
                        }
                        objTab.IsSuperTab = false;
                    }

                    // host tab
                    objTab = objTabs.GetTab( this.SuperTabId );
                    if( objTab != null )
                    {
                        // set custom properties
                        objTab.PortalID = this.PortalId;
                        objTab.StartDate = DateTime.MinValue;
                        objTab.EndDate = DateTime.MaxValue;
                        objTab.IsSuperTab = true;
                        arrTabs.Add( objTab );
                    }

                    // host child tabs
                    ArrayList arrHostTabs = objTabs.GetTabsByParentId( this.SuperTabId );
                    if( arrHostTabs != null )
                    {
                        foreach( TabInfo tempLoopVar_objTab in arrHostTabs )
                        {
                            objTab = tempLoopVar_objTab;
                            // set custom properties
                            objTab.PortalID = this.PortalId;
                            objTab.StartDate = DateTime.MinValue;
                            objTab.EndDate = DateTime.MaxValue;
                            objTab.IsSuperTab = true;
                            arrTabs.Add( objTab );
                        }
                    }

                    // cache collection
                    if( intCacheTimeout != 0 )
                    {
                        DataCache.SetCache( "GetTabs" + this.PortalId.ToString(), arrTabs, TimeSpan.FromMinutes( intCacheTimeout ) );
                    }
                }
            }
            foreach( TabInfo tempLoopVar_objTab in arrTabs )
            {
                objTab = tempLoopVar_objTab;
                // clone the tab object ( to avoid creating an object reference to the data cache )
                this.DesktopTabs.Add( objTab.Clone() );
            }

            // verify tab for portal
            int intTabId = VerifyPortalTab( PortalId, TabId );

            //  current tab settings
            objTab = (TabInfo)DataCache.GetCache( "GetTab" + intTabId.ToString() );
            if( objTab == null )
            {
                objTab = objTabs.GetTab( intTabId );
                if( objTab != null )
                {
                    // set custom properties
                    if( Null.IsNull( objTab.StartDate ) )
                    {
                        objTab.StartDate = DateTime.MinValue;
                    }
                    if( Null.IsNull( objTab.EndDate ) )
                    {
                        objTab.EndDate = DateTime.MaxValue;
                    }
                    // skin
                    if( objTab.SkinSrc == "" )
                    {
                        if( Globals.IsAdminSkin( objTab.IsAdminTab ) )
                        {
                            objSkin = SkinController.GetSkin( SkinInfo.RootSkin, PortalId, SkinType.Admin );
                        }
                        else
                        {
                            objSkin = SkinController.GetSkin( SkinInfo.RootSkin, PortalId, SkinType.Portal );
                        }
                        if( objSkin != null )
                        {
                            objTab.SkinSrc = objSkin.SkinSrc;
                        }
                    }
                    if( objTab.SkinSrc == "" )
                    {
                        if( Globals.IsAdminSkin( objTab.IsAdminTab ) )
                        {
                            objTab.SkinSrc = "[G]" + SkinInfo.RootSkin + Globals.glbDefaultSkinFolder + Globals.glbDefaultAdminSkin;
                        }
                        else
                        {
                            objTab.SkinSrc = "[G]" + SkinInfo.RootSkin + Globals.glbDefaultSkinFolder + Globals.glbDefaultSkin;
                        }
                    }
                    objTab.SkinSrc = SkinController.FormatSkinSrc( objTab.SkinSrc, this );
                    objTab.SkinPath = SkinController.FormatSkinPath( objTab.SkinSrc );
                    // container
                    if( objTab.ContainerSrc == "" )
                    {
                        if( Globals.IsAdminSkin( objTab.IsAdminTab ) )
                        {
                            objSkin = SkinController.GetSkin( SkinInfo.RootContainer, PortalId, SkinType.Admin );
                        }
                        else
                        {
                            objSkin = SkinController.GetSkin( SkinInfo.RootContainer, PortalId, SkinType.Portal );
                        }
                        if( objSkin != null )
                        {
                            objTab.ContainerSrc = objSkin.SkinSrc;
                        }
                    }
                    if( objTab.ContainerSrc == "" )
                    {
                        if( Globals.IsAdminSkin( objTab.IsAdminTab ) )
                        {
                            objTab.ContainerSrc = "[G]" + SkinInfo.RootContainer + Globals.glbDefaultContainerFolder + Globals.glbDefaultAdminContainer;
                        }
                        else
                        {
                            objTab.ContainerSrc = "[G]" + SkinInfo.RootContainer + Globals.glbDefaultContainerFolder + Globals.glbDefaultContainer;
                        }
                    }
                    objTab.ContainerSrc = SkinController.FormatSkinSrc( objTab.ContainerSrc, this );
                    objTab.ContainerPath = SkinController.FormatSkinPath( objTab.ContainerSrc );

                    // initialize collections
                    objTab.BreadCrumbs = new ArrayList();
                    objTab.Panes = new ArrayList();
                    objTab.Modules = new ArrayList();
                    if( objTab.ParentId == this.SuperTabId )
                    {
                        objTab.IsSuperTab = true;
                    }

                    // get breadcrumbs for current tab
                    ArrayList breadCrumbs = objTab.BreadCrumbs;
                    GetBreadCrumbsRecursively( ref breadCrumbs, intTabId );

                    // cache object
                    if( intCacheTimeout != 0 )
                    {
                        DataCache.SetCache( "GetTab" + intTabId.ToString(), objTab, TimeSpan.FromMinutes( intCacheTimeout ) );
                    }
                }
                else
                {
                    return;
                }
            }
            if( objTab != null )
            {
                // clone the tab object ( to avoid creating an object reference to the data cache )
                this.ActiveTab = objTab.Clone();
            }

            // get current tab modules
            Hashtable objPaneModules = new Hashtable();
            arrModules = (ArrayList)DataCache.GetCache( "GetPortalTabModules" + intTabId.ToString() );
            if( arrModules == null )
            {
                arrModules = objModules.GetPortalTabModules( this.PortalId, this.ActiveTab.TabID );
                if( arrModules != null )
                {
                    // set custom properties
                    foreach( ModuleInfo tempLoopVar_objModule in arrModules )
                    {
                        objModule = tempLoopVar_objModule;
                        if( Null.IsNull( objModule.StartDate ) )
                        {
                            objModule.StartDate = DateTime.MinValue;
                        }
                        if( Null.IsNull( objModule.EndDate ) )
                        {
                            objModule.EndDate = DateTime.MaxValue;
                        }
                        // container
                        if( objModule.ContainerSrc == "" )
                        {
                            objModule.ContainerSrc = this.ActiveTab.ContainerSrc;
                        }
                        objModule.ContainerSrc = SkinController.FormatSkinSrc( objModule.ContainerSrc, this );
                        objModule.ContainerPath = SkinController.FormatSkinPath( objModule.ContainerSrc );
                        // process tab panes
                        if( objPaneModules.ContainsKey( objModule.PaneName ) == false )
                        {
                            objPaneModules.Add( objModule.PaneName, 0 );
                        }
                        objModule.PaneModuleCount = 0;
                        if( !objModule.IsDeleted )
                        {
                            objPaneModules[objModule.PaneName] = Convert.ToInt32( objPaneModules[objModule.PaneName] ) + 1;
                            objModule.PaneModuleIndex = Convert.ToInt32( objPaneModules[objModule.PaneName] ) - 1;
                        }
                    }

                    // set pane module count
                    foreach( ModuleInfo tempLoopVar_objModule in arrModules )
                    {
                        objModule = tempLoopVar_objModule;
                        objModule.PaneModuleCount = Convert.ToInt32( objPaneModules[objModule.PaneName] );
                    }

                    // cache collection
                    if( intCacheTimeout != 0 )
                    {
                        DataCache.SetCache( "GetPortalTabModules" + intTabId.ToString(), arrModules, TimeSpan.FromMinutes( intCacheTimeout ) );
                    }
                }
            }
            foreach( ModuleInfo tempLoopVar_objModule in arrModules )
            {
                objModule = tempLoopVar_objModule;
                // clone the module object ( to avoid creating an object reference to the data cache )
                this.ActiveTab.Modules.Add( objModule.Clone() );
            }
        }

        /// <summary>
        /// The VerifyPortalTab method verifies that the TabId/PortalId combination
        /// is allowed and returns default/home tab ids if not
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        ///	<param name="portalId">The Portal's id</param>
        ///	<param name="tabId">The current tab's id</param>
        private int VerifyPortalTab( int portalId, int tabId )
        {
            int returnValue;

            TabInfo objTab;
            returnValue = -1;

            if( tabId != -1 )
            {
                // find the tab in the desktoptabs collection
                foreach( TabInfo tempLoopVar_objTab in this.DesktopTabs )
                {
                    objTab = tempLoopVar_objTab;
                    if( objTab.TabID == tabId )
                    {
                        //Check if Tab has been deleted (is in recycle bin)
                        if( !( objTab.IsDeleted ) )
                        {
                            returnValue = objTab.TabID;
                            break;            
                        }
                    }
                }
            }

            // if tab was not found
            if( returnValue == -1 && this.SplashTabId > 0 )
            {
                // use the splash tab ( if specified )
                returnValue = this.SplashTabId;
            }

            // if tab was not found
            if( returnValue == -1 && this.HomeTabId > 0 )
            {
                // use the home tab ( if specified )
                returnValue = this.HomeTabId;
            }

            // if tab was not found
            if( returnValue == -1 )
            {
                // get the first tab in the collection (that is valid)
                int i;
                for( i = 0; i <= this.DesktopTabs.Count; i++ )
                {
                    objTab = (TabInfo)this.DesktopTabs[i];
                    //Check if Tab has not been deleted (not in recycle bin) and is visible
                    if( !( objTab.IsDeleted ) && objTab.IsVisible )
                    {
                        returnValue = objTab.TabID;
                        break;
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// The GetHostSettings method returns a hashtable of
        /// host settings from the database.
        /// </summary>
        /// <returns>A Hashtable of settings (key/value pairs)</returns>
        /// <remarks>
        /// </remarks>
        public static Hashtable GetHostSettings()
        {
            return Globals.HostSettings;
        }

        /// <summary>
        /// The GetSiteSettings method returns a hashtable of
        /// portal specific settings from the database.  This method
        /// uses the Site Settings module as a convenient storage area for
        /// portal-wide settings.
        /// </summary>
        /// <returns>A Hashtable of settings (key/value pairs)</returns>
        /// <remarks>
        /// </remarks>
        ///	<param name="PortalId">The Portal</param>
        public static Hashtable GetSiteSettings( int PortalId )
        {
            ModuleController objModules = new ModuleController();

            int ModuleId = objModules.GetModuleByDefinition( PortalId, "Site Settings" ).ModuleID;

            return GetModuleSettings( ModuleId );
        }

        /// <summary>
        /// The UpdatePortalSetting method updates a specific portal setting
        /// in the database. Since this is a portal-wide storage area you must
        /// be careful to avoid naming collisions on SettingNames.
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="PortalId">The Portal</param>
        ///	<param name="SettingName">The Setting Name</param>
        ///	<param name="SettingValue">The Setting Value</param>
        public static void UpdatePortalSetting( int PortalId, string SettingName, string SettingValue )
        {
            ModuleController objModules = new ModuleController();

            int ModuleId = objModules.GetModuleByDefinition( PortalId, "Site Settings" ).ModuleID;

            objModules.UpdateModuleSetting( ModuleId, SettingName, SettingValue );
        }

        /// <summary>
        /// The GetModuleSettings Method returns a hashtable of
        /// custom module specific settings from the database.  This method is
        /// used by some user control modules (Xml, Image, etc) to access misc
        /// settings.
        /// </summary>
        /// <returns>A Hashtable of settings (key/value pairs)</returns>
        /// <remarks>
        /// </remarks>
        ///	<param name="ModuleId">The Module</param>
        public static Hashtable GetModuleSettings( int ModuleId )
        {
            ModuleController objModules = new ModuleController();

            return objModules.GetModuleSettings( ModuleId );
        }

        /// <summary>
        /// The GetTabModuleSettings Method returns a hashtable of
        /// custom module/tab specific settings from the database.  This method is
        /// used by some user control modules (Xml, Image, etc) to access misc
        /// settings.
        /// </summary>
        /// <returns>A Hashtable of settings (key/value pairs)</returns>
        /// <remarks>
        /// </remarks>
        ///	<param name="TabModuleId">The current tabModule</param>
        public static Hashtable GetTabModuleSettings( int TabModuleId )
        {
            ModuleController objModules = new ModuleController();

            return objModules.GetTabModuleSettings( TabModuleId );
        }

        /// <summary>
        /// The GetTabModuleSettings Method returns a hashtable of
        /// custom module/tab specific settings from the database.  This method is
        /// used by some user control modules (Xml, Image, etc) to access misc
        /// settings.
        /// </summary>
        /// <returns>A Hashtable of settings (key/value pairs)</returns>
        /// <remarks>
        /// </remarks>
        ///	<param name="TabModuleId">The current tabmodule</param>
        ///	<param name="settings">A Hashtable to add the Settings to</param>
        public static Hashtable GetTabModuleSettings( int TabModuleId, Hashtable settings )
        {
            return GetTabModuleSettings( new Hashtable( settings ), new Hashtable( GetTabModuleSettings( TabModuleId ) ) );
        }

        /// <summary>
        /// The GetTabModuleSettings Method returns a hashtable of
        /// custom module/tab specific settings from the database.  This method is
        /// used by some user control modules (Xml, Image, etc) to access misc
        /// settings.
        /// </summary>
        /// <returns>A Hashtable of settings (key/value pairs)</returns>
        /// <remarks>
        /// </remarks>
        ///	<param name="moduleSettings">A Hashtable of module Settings</param>
        ///	<param name="tabModuleSettings">A Hashtable of tabModule Settings to</param>
        public static Hashtable GetTabModuleSettings( Hashtable moduleSettings, Hashtable tabModuleSettings )
        {
            // add the TabModuleSettings to the ModuleSettings
            foreach( string strKey in tabModuleSettings.Keys )
            {
                moduleSettings[strKey] = tabModuleSettings[strKey];
            }

            //Return the modifed ModuleSettings
            return moduleSettings;
        }

        public static PortalAliasInfo GetPortalAliasInfo( string PortalAlias )
        {
            string strPortalAlias;

            // try the specified alias first
            PortalAliasInfo objPortalAliasInfo = GetPortalAliasLookup()[PortalAlias.ToLower()];

            // domain.com and www.domain.com should be synonymous
            if( objPortalAliasInfo == null )
            {
                if( PortalAlias.ToLower().StartsWith( "www." ) )
                {
                    // try alias without the "www." prefix
                    strPortalAlias = PortalAlias.Replace( "www.", "" );
                }
                else // try the alias with the "www." prefix
                {
                    strPortalAlias = string.Concat( "www.", PortalAlias );
                }
                // perform the lookup
                objPortalAliasInfo = GetPortalAliasLookup()[strPortalAlias.ToLower()];
            }

            // allow domain wildcards
            if( objPortalAliasInfo == null )
            {
                // remove the domain prefix ( ie. anything.domain.com = domain.com )
                if( PortalAlias.IndexOf( "." ) != -1 )
                {
                    strPortalAlias = PortalAlias.Substring( PortalAlias.IndexOf( "." ) + 1 );
                }
                else // be sure we have a clean string (without leftovers from preceding 'if' block)
                {
                    strPortalAlias = PortalAlias;
                }
                if( objPortalAliasInfo == null )
                {
                    // try an explicit lookup using the wildcard entry ( ie. *.domain.com )
                    objPortalAliasInfo = GetPortalAliasLookup()["*." + strPortalAlias.ToLower()];
                }
                if( objPortalAliasInfo == null )
                {
                    // try a lookup using the raw domain
                    objPortalAliasInfo = GetPortalAliasLookup()[strPortalAlias.ToLower()];
                }
                if( objPortalAliasInfo == null )
                {
                    // try a lookup using "www." + raw domain
                    objPortalAliasInfo = GetPortalAliasLookup()["www." + strPortalAlias.ToLower()];
                }
            }

            if( objPortalAliasInfo == null )
            {
                // check if this is a fresh install ( no alias values in collection )
                PortalAliasCollection objPortalAliasCollection = GetPortalAliasLookup();
                if( !objPortalAliasCollection.HasKeys || ( objPortalAliasCollection.Count == 1 && objPortalAliasCollection.Contains( "_default" ) ) )
                {
                    // relate the PortalAlias to the default portal on a fresh database installation
                    DataProvider.Instance().UpdatePortalAlias( PortalAlias.ToLower() );

                    //clear the cachekey "GetPortalByAlias" otherwise portalalias "_default" stays in cache after first install
                    DataCache.RemoveCache( "GetPortalByAlias" );

                    //try again
                    objPortalAliasInfo = GetPortalAliasLookup()[PortalAlias.ToLower()];
                }
            }

            return objPortalAliasInfo;
        }

        public static string GetPortalByID( int PortalId, string PortalAlias )
        {
            string retValue = "";

            // get the portal alias collection from the cache
            PortalAliasCollection objPortalAliasCollection = GetPortalAliasLookup();
            string strHTTPAlias;
            bool bFound = false;

            //Do a specified PortalAlias check first
            PortalAliasInfo objPortalAliasInfo = objPortalAliasCollection[PortalAlias.ToLower()];
            if( objPortalAliasInfo != null )
            {
                if( objPortalAliasInfo.PortalID == PortalId )
                {
                    // set the alias
                    retValue = objPortalAliasInfo.HTTPAlias;
                    bFound = true;
                }
            }

            //No match so iterate through the alias keys
            if( !bFound )
            {
                foreach( string key in objPortalAliasCollection.Keys )
                {
                    // check if the alias key starts with the portal alias value passed in - we use
                    // StartsWith because child portals are redirected to the parent portal domain name
                    // eg. child = 'www.domain.com/child' and parent is 'www.domain.com'
                    // this allows the parent domain name to resolve to the child alias ( the tabid still identifies the child portalid )
                    objPortalAliasInfo = objPortalAliasCollection[key];

                    strHTTPAlias = objPortalAliasInfo.HTTPAlias.ToLower();
                    if( strHTTPAlias.StartsWith( PortalAlias.ToLower() ) == true && objPortalAliasInfo.PortalID == PortalId )
                    {
                        // set the alias
                        retValue = objPortalAliasInfo.HTTPAlias;
                        goto endOfForLoop;
                    }

                    // domain.com and www.domain.com should be synonymous
                    if( strHTTPAlias.StartsWith( "www." ) )
                    {
                        // try alias without the "www." prefix
                        strHTTPAlias = strHTTPAlias.Replace( "www.", "" );
                    }
                    else // try the alias with the "www." prefix
                    {
                        strHTTPAlias = string.Concat( "www.", strHTTPAlias );
                    }
                    if( strHTTPAlias.StartsWith( PortalAlias.ToLower() ) == true && objPortalAliasInfo.PortalID == PortalId )
                    {
                        // set the alias
                        retValue = objPortalAliasInfo.HTTPAlias;
                        goto endOfForLoop;
                    }
                }
                endOfForLoop:
                1.GetHashCode(); //nop
            }

            return retValue;
        }

        public static string GetPortalByTab( int TabID, string PortalAlias )
        {
            string returnValue;

            int intPortalId = -2;

            // get the tab
            TabController objTabs = new TabController();
            TabInfo objTab = objTabs.GetTab( TabID );
            if( objTab != null )
            {
                // ignore deleted tabs
                if( !objTab.IsDeleted )
                {
                    intPortalId = objTab.PortalID;
                }
            }

            returnValue = null;

            switch( intPortalId )
            {
                case -2: // tab does not exist

                    break;
                case -1: // host tab

                    // host tabs are not verified to determine if they belong to the portal alias
                    returnValue = PortalAlias;
                    break;
                default: // portal tab

                    returnValue = GetPortalByID( intPortalId, PortalAlias );
                    break;
            }

            return returnValue;
        }

        public static PortalAliasCollection GetPortalAliasLookup()
        {
            PortalAliasCollection objPortalAliasCollection = (PortalAliasCollection)DataCache.GetCache( "GetPortalByAlias" );

            try
            {
                if( objPortalAliasCollection == null )
                {
                    PortalAliasController objPortalAliasController = new PortalAliasController();
                    objPortalAliasCollection = objPortalAliasController.GetPortalAliases();
                    DataCache.SetCache( "GetPortalByAlias", objPortalAliasCollection );
                }
            }
            catch( Exception exc )
            {
                // this is the first data access in Begin_Request and will catch any general connection issues
                HttpContext objHttpContext = HttpContext.Current;
                StreamReader objStreamReader;
                objStreamReader = File.OpenText( objHttpContext.Server.MapPath( "~/500.htm" ) );
                string strHTML = objStreamReader.ReadToEnd();
                objStreamReader.Close();
                strHTML = strHTML.Replace( "[MESSAGE]", "ERROR: Could not connect to database.<br><br>" + exc.Message );
                objHttpContext.Response.Write( strHTML );
                objHttpContext.Response.End();
            }

            return objPortalAliasCollection;
        }

        public static IDataReader GetDatabaseVersion()
        {
            return DataProvider.Instance().GetDatabaseVersion();
        }

        public static void UpdateDatabaseVersion( int Major, int Minor, int Build )
        {
            DataProvider.Instance().UpdateDatabaseVersion( Major, Minor, Build );
        }

        public static void UpgradeDatabaseSchema( int Major, int Minor, int Build )
        {
            DataProvider.Instance().UpgradeDatabaseSchema( Major, Minor, Build );
        }

        public static bool FindDatabaseVersion( int Major, int Minor, int Build )
        {
            bool returnValue;

            returnValue = false;
            IDataReader dr = DataProvider.Instance().FindDatabaseVersion( Major, Minor, Build );
            if( dr.Read() )
            {
                returnValue = true;
            }
            dr.Close();

            return returnValue;
        }

        public static string GetProviderPath()
        {
            return DataProvider.Instance().GetProviderPath();
        }

        public static string ExecuteScript( string strScript )
        {
            return DataProvider.Instance().ExecuteScript( strScript );
        }

        public static string ExecuteScript( string strScript, bool UseTransactions )
        {
            return DataProvider.Instance().ExecuteScript( strScript, UseTransactions );
        }
    }
}