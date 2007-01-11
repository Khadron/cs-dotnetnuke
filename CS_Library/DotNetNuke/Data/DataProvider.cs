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
using System.Data;
using DotNetNuke.Framework;

namespace DotNetNuke.Data
{
    public abstract class DataProvider
    {
        // singleton reference to the instantiated object
        private static DataProvider objProvider = null;

        // constructor
        static DataProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (DataProvider)Reflection.CreateObject( "data" );
        }

        // return the provider
        public static DataProvider Instance()
        {
            return objProvider;
        }

        //Generic Methods
        //===============
        public abstract void ExecuteNonQuery( string ProcedureName, params object[] commandParameters );
        public abstract IDataReader ExecuteReader( string ProcedureName, params object[] commandParameters );
        public abstract object ExecuteScalar( string ProcedureName, params object[] commandParameters );
        public abstract IDataReader ExecuteSQL( string Script );

        // general
        public abstract object GetNull( object Field );

        // upgrade
        public abstract string GetProviderPath();
        public abstract string ExecuteScript( string SQL );
        public abstract string ExecuteScript( string SQL, bool UseTransactions );
        public abstract IDataReader GetDatabaseVersion();
        public abstract void UpdateDatabaseVersion( int Major, int Minor, int Build );
        public abstract IDataReader FindDatabaseVersion( int Major, int Minor, int Build );
        public abstract void UpgradeDatabaseSchema( int Major, int Minor, int Build );

        // host
        public abstract IDataReader GetHostSettings();
        public abstract IDataReader GetHostSetting( string SettingName );
        public abstract void AddHostSetting( string SettingName, string SettingValue, bool SettingIsSecure );
        public abstract void UpdateHostSetting( string SettingName, string SettingValue, bool SettingIsSecure );

        // portal
        public abstract int AddPortalInfo( string PortalName, string Currency, string FirstName, string LastName, string Username, string Password, string Email, DateTime ExpiryDate, double HostFee, double HostSpace, int SiteLogHistory, string HomeDirectory );
        public abstract int CreatePortal( string PortalName, string Currency, DateTime ExpiryDate, double HostFee, double HostSpace, int SiteLogHistory, string HomeDirectory );
        public abstract void DeletePortalInfo( int PortalId );
        public abstract IDataReader GetPortal( int PortalId );
        public abstract IDataReader GetPortalByAlias( string PortalAlias );
        public abstract IDataReader GetPortalByTab( int TabId, string PortalAlias );
        public abstract IDataReader GetPortals();
        public abstract IDataReader GetPortalSpaceUsed( int PortalId );
        public abstract void UpdatePortalInfo( int PortalId, string PortalName, string LogoFile, string FooterText, DateTime ExpiryDate, int UserRegistration, int BannerAdvertising, string Currency, int AdministratorId, double HostFee, double HostSpace, string PaymentProcessor, string ProcessorUserId, string ProcessorPassword, string Description, string KeyWords, string BackgroundFile, int SiteLogHistory, int SplashTabId, int HomeTabId, int LoginTabId, int UserTabId, string DefaultLanguage, int TimeZoneOffset, string HomeDirectory );
        public abstract void UpdatePortalSetup( int PortalId, int AdministratorId, int AdministratorRoleId, int RegisteredRoleId, int SplashTabId, int HomeTabId, int LoginTabId, int UserTabId, int AdminTabId );
        public abstract IDataReader VerifyPortalTab( int PortalId, int TabId );
        public abstract IDataReader VerifyPortal( int PortalId );

        // tab
        public abstract int AddTab( int PortalId, string TabName, bool IsVisible, bool DisableLink, int ParentId, string IconFile, string Title, string Description, string KeyWords, string Url, string SkinSrc, string ContainerSrc, string TabPath, DateTime StartDate, DateTime EndDate, int RefreshInterval, string PageHeadText );
        public abstract void UpdateTab( int TabId, string TabName, bool IsVisible, bool DisableLink, int ParentId, string IconFile, string Title, string Description, string KeyWords, bool IsDeleted, string Url, string SkinSrc, string ContainerSrc, string TabPath, DateTime StartDate, DateTime EndDate );
        public abstract void UpdateTab( int TabId, string TabName, bool IsVisible, bool DisableLink, int ParentId, string IconFile, string Title, string Description, string KeyWords, bool IsDeleted, string Url, string SkinSrc, string ContainerSrc, string TabPath, DateTime StartDate, DateTime EndDate, int RefreshInterval, string PageHeadText );
        public abstract void UpdateTabOrder( int TabId, int TabOrder, int Level, int ParentId );
        public abstract void DeleteTab( int TabId );
        public abstract IDataReader GetTabs( int PortalId );
        public abstract IDataReader GetAllTabs();
        public abstract IDataReader GetTab( int TabId );
        public abstract IDataReader GetTabByName( string TabName, int PortalId );
        public abstract IDataReader GetTabsByParentId( int ParentId );
        public abstract IDataReader GetPortalTabModules( int PortalId, int TabId );
        public abstract IDataReader GetTabPanes( int TabId );

        // module
        public abstract IDataReader GetAllModules();
        public abstract IDataReader GetModules( int PortalId );
        public abstract IDataReader GetAllTabsModules( int PortalId, bool AllTabs );
        public abstract IDataReader GetModule( int ModuleId, int TabId );
        public abstract IDataReader GetModuleByDefinition( int PortalId, string FriendlyName );
        public abstract IDataReader GetSearchModules( int PortalId );
        public abstract int AddModule( int PortalID, int ModuleDefID, string ModuleTitle, bool AllTabs, string Header, string Footer, DateTime StartDate, DateTime EndDate, bool InheritViewPermissions, bool IsDeleted );
        public abstract void UpdateModule( int ModuleId, string ModuleTitle, bool AllTabs, string Header, string Footer, DateTime StartDate, DateTime EndDate, bool InheritViewPermissions, bool IsDeleted );
        public abstract void DeleteModule( int ModuleId );
        public abstract IDataReader GetTabModuleOrder( int TabId, string PaneName );
        public abstract void UpdateModuleOrder( int TabId, int ModuleId, int ModuleOrder, string PaneName );

        public abstract void AddTabModule( int TabId, int ModuleId, int ModuleOrder, string PaneName, int CacheTime, string Alignment, string Color, string Border, string IconFile, int Visibility, string ContainerSrc, bool DisplayTitle, bool DisplayPrint, bool DisplaySyndicate );
        public abstract void UpdateTabModule( int TabId, int ModuleId, int ModuleOrder, string PaneName, int CacheTime, string Alignment, string Color, string Border, string IconFile, int Visibility, string ContainerSrc, bool DisplayTitle, bool DisplayPrint, bool DisplaySyndicate );
        public abstract void DeleteTabModule( int TabId, int ModuleId );

        public abstract IDataReader GetModuleSettings( int ModuleId );
        public abstract IDataReader GetModuleSetting( int ModuleId, string SettingName );
        public abstract void AddModuleSetting( int ModuleId, string SettingName, string SettingValue );
        public abstract void UpdateModuleSetting( int ModuleId, string SettingName, string SettingValue );
        public abstract void DeleteModuleSetting( int ModuleId, string SettingName );
        public abstract void DeleteModuleSettings( int ModuleId );

        public abstract IDataReader GetTabModuleSettings( int TabModuleId );
        public abstract IDataReader GetTabModuleSetting( int TabModuleId, string SettingName );
        public abstract void AddTabModuleSetting( int TabModuleId, string SettingName, string SettingValue );
        public abstract void UpdateTabModuleSetting( int TabModuleId, string SettingName, string SettingValue );
        public abstract void DeleteTabModuleSetting( int TabModuleId, string SettingName );
        public abstract void DeleteTabModuleSettings( int TabModuleId );

        // module definition
        public abstract IDataReader GetDesktopModule( int DesktopModuleId );
        public abstract IDataReader GetDesktopModuleByFriendlyName( string FriendlyName );
        public abstract IDataReader GetDesktopModuleByModuleName( string ModuleName );
        public abstract IDataReader GetDesktopModules();
        public abstract IDataReader GetDesktopModulesByPortal( int PortalID );
        public abstract int AddDesktopModule(string ModuleName, string FolderName, string FriendlyName, string Description, string Version, bool IsPremium, bool IsAdmin, string BusinessControllerClass, int SupportedFeatures, string CompatibleVersions);
        public abstract void UpdateDesktopModule(int DesktopModuleId, string ModuleName, string FolderName, string FriendlyName, string Description, string Version, bool IsPremium, bool IsAdmin, string BusinessControllerClass, int SupportedFeatures, string CompatibleVersions);
        public abstract void DeleteDesktopModule( int DesktopModuleId );

        public abstract IDataReader GetPortalDesktopModules( int PortalID, int DesktopModuleID );
        public abstract int AddPortalDesktopModule( int PortalID, int DesktopModuleID );
        public abstract void DeletePortalDesktopModules( int PortalID, int DesktopModuleID );

        public abstract IDataReader GetModuleDefinitions( int DesktopModuleId );
        public abstract IDataReader GetModuleDefinition( int ModuleDefId );
        public abstract IDataReader GetModuleDefinitionByName( int DesktopModuleId, string FriendlyName );
        public abstract int AddModuleDefinition( int DesktopModuleId, string FriendlyName, int DefaultCacheTime );
        public abstract void DeleteModuleDefinition( int ModuleDefId );
        public abstract void UpdateModuleDefinition( int ModuleDefId, string FriendlyName, int DefaultCacheTime );

        public abstract IDataReader GetModuleControl( int ModuleControlId );
        public abstract IDataReader GetModuleControls( int ModuleDefID );
        public abstract IDataReader GetModuleControlsByKey( string ControlKey, int ModuleDefId );
        public abstract IDataReader GetModuleControlByKeyAndSrc( int ModuleDefID, string ControlKey, string ControlSrc );
        public abstract int AddModuleControl( int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, int ControlType, int ViewOrder, string HelpUrl );
        public abstract void UpdateModuleControl( int ModuleControlId, int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, int ControlType, int ViewOrder, string HelpUrl );
        public abstract void DeleteModuleControl( int ModuleControlId );

        // files
        public abstract IDataReader GetFiles( int PortalId, int FolderID );
        public abstract IDataReader GetFile( string FileName, int PortalId, int FolderID );
        public abstract IDataReader GetFileById( int FileId, int PortalId );
        public abstract void DeleteFile( int PortalId, string FileName, int FolderID );
        public abstract void DeleteFiles( int PortalId );
        public abstract int AddFile( int PortalId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string Folder, int FolderID );
        public abstract void UpdateFile( int FileId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string Folder, int FolderID );
        public abstract DataTable GetAllFiles();
        public abstract IDataReader GetFileContent( int FileId, int PortalId );
        public abstract void UpdateFileContent( int FileId, byte[] StreamFile );

        // site log
        public abstract void AddSiteLog( DateTime DateTime, int PortalId, int UserId, string Referrer, string URL, string UserAgent, string UserHostAddress, string UserHostName, int TabId, int AffiliateId );
        public abstract IDataReader GetSiteLogReports();
        public abstract IDataReader GetSiteLog( int PortalId, string PortalAlias, string ReportName, DateTime StartDate, DateTime EndDate );
        public abstract void DeleteSiteLog( DateTime DateTime, int PortalId );

        // database
        public abstract IDataReader GetTables();
        public abstract IDataReader GetFields( string TableName );
        // vendors
        public abstract IDataReader GetVendors( int PortalId, bool UnAuthorized, int PageIndex, int PageSize );
        public abstract IDataReader GetVendorsByEmail( string Filter, int PortalId, int PageIndex, int PageSize );
        public abstract IDataReader GetVendorsByName( string Filter, int PortalId, int PageIndex, int PageSize );
        public abstract IDataReader GetVendor( int VendorID, int PortalID );
        public abstract void DeleteVendor( int VendorID );
        public abstract int AddVendor( int PortalID, string VendorName, string Unit, string Street, string City, string Region, string Country, string PostalCode, string Telephone, string Fax, string Cell, string Email, string Website, string FirstName, string LastName, string UserName, string LogoFile, string KeyWords, string Authorized );
        public abstract void UpdateVendor( int VendorID, string VendorName, string Unit, string Street, string City, string Region, string Country, string PostalCode, string Telephone, string Fax, string Cell, string Email, string Website, string FirstName, string LastName, string UserName, string LogoFile, string KeyWords, string Authorized );
        public abstract IDataReader GetVendorClassifications( int VendorId );
        public abstract void DeleteVendorClassifications( int VendorId );
        public abstract int AddVendorClassification( int VendorId, int ClassificationId );

        // banners
        public abstract IDataReader GetBanners( int VendorId );
        public abstract IDataReader GetBanner( int BannerId, int VendorId, int PortalID );
        public abstract void DeleteBanner( int BannerId );
        public abstract int AddBanner( string BannerName, int VendorId, string ImageFile, string URL, int Impressions, double CPM, DateTime StartDate, DateTime EndDate, string UserName, int BannerTypeId, string Description, string GroupName, int Criteria, int Width, int Height );
        public abstract void UpdateBanner( int BannerId, string BannerName, string ImageFile, string URL, int Impressions, double CPM, DateTime StartDate, DateTime EndDate, string UserName, int BannerTypeId, string Description, string GroupName, int Criteria, int Width, int Height );
        public abstract IDataReader FindBanners( int PortalId, int BannerTypeId, string GroupName );
        public abstract void UpdateBannerViews( int BannerId, DateTime StartDate, DateTime EndDate );
        public abstract void UpdateBannerClickThrough( int BannerId, int VendorId );

        // affiliates
        public abstract IDataReader GetAffiliates( int VendorId );
        public abstract IDataReader GetAffiliate( int AffiliateId, int VendorId, int PortalID );
        public abstract void DeleteAffiliate( int AffiliateId );
        public abstract int AddAffiliate( int VendorId, DateTime StartDate, DateTime EndDate, double CPC, double CPA );
        public abstract void UpdateAffiliate( int AffiliateId, DateTime StartDate, DateTime EndDate, double CPC, double CPA );
        public abstract void UpdateAffiliateStats( int AffiliateId, int Clicks, int Acquisitions );

        // skins/containers
        public abstract IDataReader GetSkin( string SkinRoot, int PortalId, int SkinType );
        public abstract void DeleteSkin( string SkinRoot, int PortalId, int SkinType );
        public abstract int AddSkin( string SkinRoot, int PortalId, int SkinType, string SkinSrc );

        // personalization
        public abstract IDataReader GetAllProfiles();
        public abstract IDataReader GetProfile( int UserId, int PortalId );
        public abstract void AddProfile( int UserId, int PortalId );
        public abstract void UpdateProfile( int UserId, int PortalId, string ProfileData );

        //profile property definitions
        public abstract int AddPropertyDefinition( int PortalId, int ModuleDefId, int DataType, string DefaultValue, string PropertyCategory, string PropertyName, bool Required, string ValidationExpression, int ViewOrder, bool Visible, int Length );
        public abstract void DeletePropertyDefinition( int definitionId );
        public abstract IDataReader GetPropertyDefinition( int definitionId );
        public abstract IDataReader GetPropertyDefinitionByName( int portalId, string name );
        public abstract IDataReader GetPropertyDefinitionsByCategory( int portalId, string category );
        public abstract IDataReader GetPropertyDefinitionsByPortal( int portalId );
        public abstract void UpdatePropertyDefinition( int PropertyDefinitionId, int DataType, string DefaultValue, string PropertyCategory, string PropertyName, bool Required, string ValidationExpression, int ViewOrder, bool Visible, int Length );

        // urls
        public abstract IDataReader GetUrls( int PortalID );
        public abstract IDataReader GetUrl( int PortalID, string Url );
        public abstract void AddUrl( int PortalID, string Url );
        public abstract void DeleteUrl( int PortalID, string Url );
        public abstract IDataReader GetUrlTracking( int PortalID, string Url, int ModuleId );
        public abstract void AddUrlTracking( int PortalID, string Url, string UrlType, bool LogActivity, bool TrackClicks, int ModuleId, bool NewWindow );
        public abstract void UpdateUrlTracking( int PortalID, string Url, bool LogActivity, bool TrackClicks, int ModuleId, bool NewWindow );
        public abstract void DeleteUrlTracking( int PortalID, string Url, int ModuleId );
        public abstract void UpdateUrlTrackingStats( int PortalID, string Url, int ModuleId );
        public abstract IDataReader GetUrlLog( int UrlTrackingID, DateTime StartDate, DateTime EndDate );
        public abstract void AddUrlLog( int UrlTrackingID, int UserID );

        //Folders
        public abstract IDataReader GetFoldersByPortal(int PortalID);
        public abstract IDataReader GetFoldersByUser(int PortalID, int UserID, bool IncludeSecure, bool IncludeDatabase, bool AllowAccess, string Permissions);
        public abstract IDataReader GetFolder(int PortalID, int FolderID);
        public abstract IDataReader GetFolder(int PortalID, string FolderPath);
        public abstract int AddFolder(int PortalID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached, System.DateTime LastUpdated);
        public abstract void UpdateFolder(int PortalID, int FolderID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached, System.DateTime LastUpdated);
        public abstract void DeleteFolder(int PortalID, string FolderPath);


        //Permission
        public abstract IDataReader GetPermission( int permissionID );
        public abstract IDataReader GetPermissionsByModuleDefID(int ModuleDefID);
        public abstract IDataReader GetPermissionsByModuleID( int ModuleID );
        public abstract IDataReader GetPermissionsByFolderPath( int PortalID, string Folder );
        public abstract IDataReader GetPermissionByCodeAndKey( string PermissionCode, string PermissionKey );
        public abstract IDataReader GetPermissionsByTabID( int TabID );
        public abstract void DeletePermission( int permissionID );
        public abstract int AddPermission( string permissionCode, int moduleDefID, string permissionKey, string permissionName );
        public abstract void UpdatePermission( int permissionID, string permissionCode, int moduleDefID, string permissionKey, string permissionName );

        //ModulePermission
        public abstract IDataReader GetModulePermission( int modulePermissionID );
        public abstract IDataReader GetModulePermissionsByModuleID( int moduleID, int PermissionID );
        public abstract IDataReader GetModulePermissionsByPortal( int PortalID );
        public abstract void DeleteModulePermissionsByModuleID( int ModuleID );
        public abstract void DeleteModulePermission( int modulePermissionID );
        public abstract int AddModulePermission( int moduleID, int PermissionID, int roleID, bool AllowAccess );
        public abstract void UpdateModulePermission( int modulePermissionID, int moduleID, int PermissionID, int roleID, bool AllowAccess );

        //TabPermission
        public abstract IDataReader GetTabPermissionsByPortal( int PortalID );
        public abstract IDataReader GetTabPermissionsByTabID( int TabID, int PermissionID );
        public abstract void DeleteTabPermissionsByTabID( int TabID );
        public abstract void DeleteTabPermission( int TabPermissionID );
        public abstract int AddTabPermission( int TabID, int PermissionID, int roleID, bool AllowAccess );
        public abstract void UpdateTabPermission( int TabPermissionID, int TabID, int PermissionID, int roleID, bool AllowAccess );

        //FolderPermission
        public abstract IDataReader GetFolderPermission( int FolderPermissionID );
        public abstract IDataReader GetFolderPermissionsByFolderPath( int PortalID, string FolderPath, int PermissionID );
        public abstract void DeleteFolderPermissionsByFolderPath( int PortalID, string FolderPath );
        public abstract void DeleteFolderPermission( int FolderPermissionID );
        public abstract int AddFolderPermission( int FolderID, int PermissionID, int roleID, bool AllowAccess );
        public abstract void UpdateFolderPermission( int FolderPermissionID, int FolderID, int PermissionID, int roleID, bool AllowAccess );

        // search engine
        public abstract IDataReader GetSearchIndexers();
        public abstract IDataReader GetSearchResultModules( int PortalID );

        // content search datastore
        public abstract void DeleteSearchItems( int ModuleID );
        public abstract void DeleteSearchItem( int SearchItemId );
        public abstract void DeleteSearchItemWords( int SearchItemId );
        public abstract int AddSearchItem( string Title, string Description, int Author, DateTime PubDate, int ModuleId, string Key, string Guid, int ImageFileId );
        public abstract IDataReader GetSearchCommonWordsByLocale( string Locale );
        public abstract IDataReader GetDefaultLanguageByModule( string ModuleList );
        public abstract IDataReader GetSearchSettings( int ModuleId );
        public abstract IDataReader GetSearchWords();
        public abstract int AddSearchWord( string Word );
        public abstract int AddSearchItemWord( int SearchItemId, int SearchWordsID, int Occurrences );
        public abstract void AddSearchItemWordPosition( int SearchItemWordID, string ContentPositions );
        public abstract IDataReader GetSearchResults( int PortalID, string Word );
        public abstract IDataReader GetSearchItems( int PortalID, int TabID, int ModuleID );
        public abstract IDataReader GetSearchItem( int ModuleID, string SearchKey );
        public abstract void UpdateSearchItem( int SearchItemId, string Title, string Description, int Author, DateTime PubDate, int ModuleId, string Key, string Guid, int HitCount, int ImageFileId );

        //Lists
        public abstract IDataReader GetListGroup();
        public abstract IDataReader GetList( string ListName, string ParentKey, int DefinitionID );
        public abstract IDataReader GetListEntries( string ListName, string ParentKey, int EntryID, int DefinitionID );
        public abstract IDataReader GetListEntriesByListName( string ListName, string Value, string ParentKey );
        public abstract int AddListEntry( string ListName, string Value, string Text, string ParentKey, bool EnableSortOrder, int DefinitionID, string Description );
        public abstract void UpdateListEntry( int EntryID, string ListName, string Value, string Text, string Description );
        public abstract void DeleteListEntryByID( int EntryID, bool DeleteChild );
        public abstract void DeleteList( string ListName, string ParentKey );
        public abstract void DeleteListEntryByListName( string ListName, string Value, bool DeleteChild );
        public abstract void UpdateListSortOrder( int EntryID, bool MoveUp );

        //portal alias
        public abstract IDataReader GetPortalAlias( string PortalAlias, int PortalID );
        public abstract IDataReader GetPortalAliasByPortalID( int PortalID );
        public abstract IDataReader GetPortalAliasByPortalAliasID( int PortalAliasID );
        public abstract IDataReader GetPortalByPortalAliasID( int PortalAliasId );
        public abstract void UpdatePortalAlias( string PortalAlias );
        public abstract void UpdatePortalAliasInfo( int PortalAliasID, int PortalID, string HTTPAlias );
        public abstract int AddPortalAlias( int PortalID, string HTTPAlias );
        public abstract void DeletePortalAlias( int PortalAliasID );
    }
}