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
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Entities.Modules
{
    public class ModuleController
    {
        public int AddModule( ModuleInfo objModule )
        {
            // add module
            if( Null.IsNull( objModule.ModuleID ) )
            {
                objModule.ModuleID = DataProvider.Instance().AddModule( objModule.PortalID, objModule.ModuleDefID, objModule.ModuleTitle, objModule.AllTabs, objModule.Header, objModule.Footer, objModule.StartDate, objModule.EndDate, objModule.InheritViewPermissions, objModule.IsDeleted );

                // set module permissions
                if( objModule.ModulePermissions != null )
                {
                    ModulePermissionController objModulePermissionController = new ModulePermissionController();
                    ModulePermissionCollection objModulePermissions;
                    objModulePermissions = objModule.ModulePermissions;
                    ModulePermissionInfo objModulePermission = new ModulePermissionInfo();
                    foreach( ModulePermissionInfo tempLoopVar_objModulePermission in objModulePermissions )
                    {
                        objModulePermission = tempLoopVar_objModulePermission;
                        objModulePermission.ModuleID = objModule.ModuleID;
                        objModulePermissionController.AddModulePermission( objModulePermission );
                    }
                }
            }

            //This will fail if the page already contains this module
            try
            {
                // add tabmodule
                DataProvider.Instance().AddTabModule( objModule.TabID, objModule.ModuleID, objModule.ModuleOrder, objModule.PaneName, objModule.CacheTime, objModule.Alignment, objModule.Color, objModule.Border, objModule.IconFile, (int)objModule.Visibility, objModule.ContainerSrc, objModule.DisplayTitle, objModule.DisplayPrint, objModule.DisplaySyndicate );

                if( objModule.ModuleOrder == -1 )
                {
                    // position module at bottom of pane
                    UpdateModuleOrder( objModule.TabID, objModule.ModuleID, objModule.ModuleOrder, objModule.PaneName );
                }
                else
                {
                    // position module in pane
                    UpdateTabModuleOrder( objModule.TabID, objModule.PortalID );
                }
            }
            catch
            {
                // module already in the page, ignore error
            }

            return objModule.ModuleID;
        }

        private ModuleInfo FillModuleInfo( IDataReader dr )
        {
            return FillModuleInfo( dr, true, true );
        }

        private ModuleInfo FillModuleInfo( IDataReader dr, bool CheckForOpenDataReader )
        {
            return FillModuleInfo( dr, CheckForOpenDataReader, true );
        }

        private ModuleInfo FillModuleInfo( IDataReader dr, bool CheckForOpenDataReader, bool IncludePermissions )
        {
            ModuleInfo objModuleInfo = new ModuleInfo();
            ModulePermissionController objModulePermissionController = new ModulePermissionController();
            // read datareader
            bool doContinue = true;

            if( CheckForOpenDataReader )
            {
                doContinue = false;
                if( dr.Read() )
                {
                    doContinue = true;
                }
            }
            if( doContinue )
            {
                try
                {
                    objModuleInfo.PortalID = Convert.ToInt32( Null.SetNull( dr["PortalID"], objModuleInfo.PortalID ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.TabID = Convert.ToInt32( Null.SetNull( dr["TabID"], objModuleInfo.TabID ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.TabModuleID = Convert.ToInt32( Null.SetNull( dr["TabModuleID"], objModuleInfo.TabModuleID ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.ModuleID = Convert.ToInt32( Null.SetNull( dr["ModuleID"], objModuleInfo.ModuleID ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.ModuleDefID = Convert.ToInt32( Null.SetNull( dr["ModuleDefID"], objModuleInfo.ModuleDefID ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.ModuleOrder = Convert.ToInt32( Null.SetNull( dr["ModuleOrder"], objModuleInfo.ModuleOrder ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.PaneName = Convert.ToString( Null.SetNull( dr["PaneName"], objModuleInfo.PaneName ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.ModuleTitle = Convert.ToString( Null.SetNull( dr["ModuleTitle"], objModuleInfo.ModuleTitle ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.CacheTime = Convert.ToInt32( Null.SetNull( dr["CacheTime"], objModuleInfo.CacheTime ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.Alignment = Convert.ToString( Null.SetNull( dr["Alignment"], objModuleInfo.Alignment ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.Color = Convert.ToString( Null.SetNull( dr["Color"], objModuleInfo.Color ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.Border = Convert.ToString( Null.SetNull( dr["Border"], objModuleInfo.Border ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.IconFile = Convert.ToString( Null.SetNull( dr["IconFile"], objModuleInfo.IconFile ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.AllTabs = Convert.ToBoolean( Null.SetNull( dr["AllTabs"], objModuleInfo.AllTabs ) );
                }
                catch
                {
                }
                try
                {
                    int intVisibility = 0;
                    if( Convert.ToInt32( Null.SetNull( dr["Visibility"], intVisibility ) ) == 0 )
                    {
                        objModuleInfo.Visibility = VisibilityState.Maximized;
                    }
                    else if( Convert.ToInt32( Null.SetNull( dr["Visibility"], intVisibility ) ) == Null.NullInteger )
                    {
                        objModuleInfo.Visibility = VisibilityState.Maximized;
                    }
                    else if( Convert.ToInt32( Null.SetNull( dr["Visibility"], intVisibility ) ) == 1 )
                    {
                        objModuleInfo.Visibility = VisibilityState.Minimized;
                    }
                    else if( Convert.ToInt32( Null.SetNull( dr["Visibility"], intVisibility ) ) == 2 )
                    {
                        objModuleInfo.Visibility = VisibilityState.None;
                    }
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.IsDeleted = Convert.ToBoolean( Null.SetNull( dr["IsDeleted"], objModuleInfo.IsDeleted ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.Header = Convert.ToString( Null.SetNull( dr["Header"], objModuleInfo.Header ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.Footer = Convert.ToString( Null.SetNull( dr["Footer"], objModuleInfo.Footer ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.StartDate = Convert.ToDateTime( Null.SetNull( dr["StartDate"], objModuleInfo.StartDate ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.EndDate = Convert.ToDateTime( Null.SetNull( dr["EndDate"], objModuleInfo.EndDate ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.ContainerSrc = Convert.ToString( Null.SetNull( dr["ContainerSrc"], objModuleInfo.ContainerSrc ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.DisplayTitle = Convert.ToBoolean( Null.SetNull( dr["DisplayTitle"], objModuleInfo.DisplayTitle ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.DisplayPrint = Convert.ToBoolean( Null.SetNull( dr["DisplayPrint"], objModuleInfo.DisplayPrint ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.DisplaySyndicate = Convert.ToBoolean( Null.SetNull( dr["DisplaySyndicate"], objModuleInfo.DisplaySyndicate ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.InheritViewPermissions = Convert.ToBoolean( Null.SetNull( dr["InheritViewPermissions"], objModuleInfo.InheritViewPermissions ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.DesktopModuleID = Convert.ToInt32( Null.SetNull( dr["DesktopModuleID"], objModuleInfo.DesktopModuleID ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.FriendlyName = Convert.ToString( Null.SetNull( dr["FriendlyName"], objModuleInfo.FriendlyName ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.Description = Convert.ToString( Null.SetNull( dr["Description"], objModuleInfo.Description ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.Version = Convert.ToString( Null.SetNull( dr["Version"], objModuleInfo.Version ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.IsPremium = Convert.ToBoolean( Null.SetNull( dr["IsPremium"], objModuleInfo.IsPremium ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.IsAdmin = Convert.ToBoolean( Null.SetNull( dr["IsAdmin"], objModuleInfo.IsAdmin ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.BusinessControllerClass = Convert.ToString( Null.SetNull( dr["BusinessControllerClass"], objModuleInfo.BusinessControllerClass ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.SupportedFeatures = Convert.ToInt32( Null.SetNull( dr["SupportedFeatures"], objModuleInfo.SupportedFeatures ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.ModuleControlId = Convert.ToInt32( Null.SetNull( dr["ModuleControlId"], objModuleInfo.ModuleControlId ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.ControlSrc = Convert.ToString( Null.SetNull( dr["ControlSrc"], objModuleInfo.ControlSrc ) );
                }
                catch
                {
                }
                try
                {
                    int intControlType = 0;
                    if( Convert.ToInt32( Null.SetNull( dr["ControlType"], intControlType ) ) == -3 )
                    {
                        objModuleInfo.ControlType = SecurityAccessLevel.ControlPanel;
                    }
                    else if( Convert.ToInt32( Null.SetNull( dr["ControlType"], intControlType ) ) == -2 )
                    {
                        objModuleInfo.ControlType = SecurityAccessLevel.SkinObject;
                    }
                    else if( Convert.ToInt32( Null.SetNull( dr["ControlType"], intControlType ) ) == -1 )
                    {
                        objModuleInfo.ControlType = SecurityAccessLevel.Anonymous;
                    }
                    else if( Convert.ToInt32( Null.SetNull( dr["ControlType"], intControlType ) ) == Null.NullInteger )
                    {
                        objModuleInfo.ControlType = SecurityAccessLevel.Anonymous;
                    }
                    else if( Convert.ToInt32( Null.SetNull( dr["ControlType"], intControlType ) ) == 0 )
                    {
                        objModuleInfo.ControlType = SecurityAccessLevel.View;
                    }
                    else if( Convert.ToInt32( Null.SetNull( dr["ControlType"], intControlType ) ) == 1 )
                    {
                        objModuleInfo.ControlType = SecurityAccessLevel.Edit;
                    }
                    else if( Convert.ToInt32( Null.SetNull( dr["ControlType"], intControlType ) ) == 2 )
                    {
                        objModuleInfo.ControlType = SecurityAccessLevel.Admin;
                    }
                    else if( Convert.ToInt32( Null.SetNull( dr["ControlType"], intControlType ) ) == 3 )
                    {
                        objModuleInfo.ControlType = SecurityAccessLevel.Host;
                    }
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.ControlTitle = Convert.ToString( Null.SetNull( dr["ControlTitle"], objModuleInfo.ControlTitle ) );
                }
                catch
                {
                }
                try
                {
                    objModuleInfo.HelpUrl = Convert.ToString( Null.SetNull( dr["HelpUrl"], objModuleInfo.HelpUrl ) );
                }
                catch
                {
                }

                if( IncludePermissions )
                {
                    if( objModuleInfo != null )
                    {
                        //Get the Module permissions first (then we can parse the collection to determine the View/Edit Roles)
                        try
                        {
                            objModuleInfo.ModulePermissions = objModulePermissionController.GetModulePermissionsCollectionByModuleID( objModuleInfo.ModuleID );
                        }
                        catch
                        {
                        }
                        try
                        {
                            objModuleInfo.AuthorizedEditRoles = objModulePermissionController.GetModulePermissionsByModuleID( objModuleInfo, "EDIT" );
                            if( objModuleInfo.AuthorizedEditRoles == ";" )
                            {
                                // this code is here for legacy support - the AuthorizedEditRoles were stored as a concatenated list of roleids prior to DNN 3.0
                                try
                                {
                                    objModuleInfo.AuthorizedEditRoles = Convert.ToString( Null.SetNull( dr["AuthorizedEditRoles"], objModuleInfo.AuthorizedEditRoles ) );
                                }
                                catch
                                {
                                    // the AuthorizedEditRoles field was removed from the Tabs table in 3.0
                                }
                            }
                        }
                        catch
                        {
                        }
                        try
                        {
                            if( objModuleInfo.InheritViewPermissions )
                            {
                                TabPermissionController objTabPermissionController = new TabPermissionController();
                                ArrayList arrTabPermissions = objTabPermissionController.GetTabPermissionsByPortal( objModuleInfo.PortalID );
                                objModuleInfo.AuthorizedViewRoles = objTabPermissionController.GetTabPermissionsByTabID( arrTabPermissions, objModuleInfo.TabID, "VIEW" );
                            }
                            else
                            {
                                objModuleInfo.AuthorizedViewRoles = objModulePermissionController.GetModulePermissionsByModuleID( objModuleInfo, "VIEW" );
                            }
                            if( objModuleInfo.AuthorizedViewRoles == ";" )
                            {
                                // this code is here for legacy support - the AuthorizedViewRoles were stored as a concatenated list of roleids prior to DNN 3.0
                                try
                                {
                                    objModuleInfo.AuthorizedViewRoles = Convert.ToString( Null.SetNull( dr["AuthorizedViewRoles"], objModuleInfo.AuthorizedViewRoles ) );
                                }
                                catch
                                {
                                    // the AuthorizedViewRoles field was removed from the Tabs table in 3.0
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            else
            {
                objModuleInfo = null;
            }
            return objModuleInfo;
        }

        internal ArrayList FillModuleInfoCollection( IDataReader dr )
        {
            return FillModuleInfoCollection( dr, true );
        }

        internal ArrayList FillModuleInfoCollection( IDataReader dr, bool IncludePermissions )
        {
            ArrayList arr = new ArrayList();
            try
            {
                ModuleInfo obj;
                while( dr.Read() )
                {
                    // fill business object
                    obj = FillModuleInfo( dr, false, IncludePermissions );
                    // add to collection
                    arr.Add( obj );
                }
            }
            catch( Exception exc )
            {
                Exceptions.LogException( exc );
            }
            finally
            {
                // close datareader
                if( dr != null )
                {
                    dr.Close();
                }
            }

            return arr;
        }

        public ArrayList GetAllModules()
        {
            return FillModuleInfoCollection( DataProvider.Instance().GetAllModules() );
        }

        public ArrayList GetAllTabsModules( int PortalID, bool AllTabs )
        {
            return FillModuleInfoCollection( DataProvider.Instance().GetAllTabsModules( PortalID, AllTabs ) );
        }

        public ModuleInfo GetModule( int ModuleId, int TabId )
        {
            IDataReader dr = DataProvider.Instance().GetModule( ModuleId, TabId );
            try
            {
                return FillModuleInfo( dr );
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
        }

        public ModuleInfo GetModuleByDefinition( int PortalId, string FriendlyName )
        {
            IDataReader dr = DataProvider.Instance().GetModuleByDefinition( PortalId, FriendlyName );
            try
            {
                return FillModuleInfo( dr );
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
        }

        public ArrayList GetModules( int PortalID )
        {
            return FillModuleInfoCollection( DataProvider.Instance().GetModules( PortalID ) );
        }

        public ArrayList GetModules( int PortalID, bool IncludePermissions )
        {
            return FillModuleInfoCollection( DataProvider.Instance().GetModules( PortalID ), IncludePermissions );
        }

        public ArrayList GetModulesByDefinition( int PortalId, string FriendlyName )
        {
            return FillModuleInfoCollection( DataProvider.Instance().GetModuleByDefinition( PortalId, FriendlyName ) );
        }

        public Hashtable GetModuleSettings( int ModuleId )
        {
            Hashtable objSettings;

            string strCacheKey = "GetModuleSettings" + ModuleId.ToString();

            objSettings = (Hashtable)DataCache.GetCache( strCacheKey );

            if( objSettings == null )
            {
                objSettings = new Hashtable();

                IDataReader dr = DataProvider.Instance().GetModuleSettings( ModuleId );

                while( dr.Read() )
                {
                    if( !dr.IsDBNull( 1 ) )
                    {
                        objSettings[dr.GetString( 0 )] = dr.GetString( 1 );
                    }
                    else
                    {
                        objSettings[dr.GetString( 0 )] = "";
                    }
                }

                dr.Close();

                // cache data
                int intCacheTimeout = 20*Convert.ToInt32( Globals.PerformanceSetting );
                DataCache.SetCache( strCacheKey, objSettings, TimeSpan.FromMinutes( intCacheTimeout ) );
            }

            return objSettings;
        }

        public ArrayList GetPortalTabModules( int PortalId, int TabId )
        {
            return FillModuleInfoCollection( DataProvider.Instance().GetPortalTabModules( PortalId, TabId ) );
        }

        public ArrayList GetSearchModules( int PortalId )
        {
            return FillModuleInfoCollection( DataProvider.Instance().GetSearchModules( PortalId ) );
        }

        public Hashtable GetTabModuleSettings( int TabModuleId )
        {
            Hashtable objSettings;

            string strCacheKey = "GetTabModuleSettings" + TabModuleId.ToString();

            objSettings = (Hashtable)DataCache.GetCache( strCacheKey );

            if( objSettings == null )
            {
                objSettings = new Hashtable();

                IDataReader dr = DataProvider.Instance().GetTabModuleSettings( TabModuleId );

                while( dr.Read() )
                {
                    if( !dr.IsDBNull( 1 ) )
                    {
                        objSettings[dr.GetString( 0 )] = dr.GetString( 1 );
                    }
                    else
                    {
                        objSettings[dr.GetString( 0 )] = "";
                    }
                }

                dr.Close();

                // cache data
                int intCacheTimeout = 20*Convert.ToInt32( Globals.PerformanceSetting );
                DataCache.SetCache( strCacheKey, objSettings, TimeSpan.FromMinutes( intCacheTimeout ) );
            }

            return objSettings;
        }

        /// <summary>
        /// CopyModule copies a Module from one Tab to another optionally including all the
        ///	TabModule settings
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="moduleId">The Id of the module to copy</param>
        ///	<param name="fromTabId">The Id of the source tab</param>
        ///	<param name="toTabId">The Id of the destination tab</param>
        ///	<param name="toPaneName">The name of the Pane on the destination tab where the module will end up</param>
        ///	<param name="includeSettings">A flag to indicate whether the settings are copied to the new Tab</param>
        public void CopyModule( int moduleId, int fromTabId, int toTabId, string toPaneName, bool includeSettings )
        {
            //First Get the Module itself
            ModuleInfo objModule = GetModule( moduleId, fromTabId );

            //If the Destination Pane Name is not set, assume the same name as the source
            if( toPaneName == "" )
            {
                toPaneName = objModule.PaneName;
            }

            //This will fail if the page already contains this module
            try
            {
                //Add a copy of the module to the bottom of the Pane for the new Tab
                DataProvider.Instance().AddTabModule( toTabId, moduleId, -1, toPaneName, objModule.CacheTime, objModule.Alignment, objModule.Color, objModule.Border, objModule.IconFile, (int)objModule.Visibility, objModule.ContainerSrc, objModule.DisplayTitle, objModule.DisplayPrint, objModule.DisplaySyndicate );

                //Optionally copy the TabModuleSettings
                if( includeSettings )
                {
                    ModuleInfo toModule = GetModule( moduleId, toTabId );
                    CopyTabModuleSettings( objModule, toModule );
                }
            }
            catch
            {
                // module already in the page, ignore error
            }
        }

        /// <summary>
        /// CopyModule copies a Module from one Tab to a collection of Tabs optionally
        ///	including the TabModule settings
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="moduleId">The Id of the module to copy</param>
        ///	<param name="fromTabId">The Id of the source tab</param>
        ///	<param name="toTabs">An ArrayList of TabItem objects</param>
        ///	<param name="includeSettings">A flag to indicate whether the settings are copied to the new Tab</param>
        public void CopyModule( int moduleId, int fromTabId, ArrayList toTabs, bool includeSettings )
        {
            int intTab;
            TabInfo objTab;

            //Iterate through collection copying the module to each Tab (except the source)
            for( intTab = 0; intTab <= toTabs.Count - 1; intTab++ )
            {
                objTab = (TabInfo)toTabs[intTab];
                if( objTab.TabID != fromTabId )
                {
                    CopyModule( moduleId, fromTabId, objTab.TabID, "", includeSettings );
                }
            }
        }

        /// <summary>
        /// CopyTabModuleSettings copies the TabModuleSettings from one instance to another
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="fromModule">The module to copy from</param>
        ///	<param name="toModule">The module to copy to</param>
        public void CopyTabModuleSettings( ModuleInfo fromModule, ModuleInfo toModule )
        {
            //Get the TabModuleSettings
            Hashtable settings = GetTabModuleSettings( fromModule.TabModuleID );

            //Copy each setting to the new TabModule instance
            foreach( DictionaryEntry setting in settings )
            {
                UpdateTabModuleSetting( toModule.TabModuleID, Convert.ToString( setting.Key ), Convert.ToString( setting.Value ) );
            }
        }

        /// <summary>
        /// DeleteAllModules deletes all instaces of a Module (from a collection), optionally excluding the
        ///	current instance, and optionally including deleting the Module itself.
        /// </summary>
        /// <remarks>
        ///	Note - the base module is not removed unless both the flags are set, indicating
        ///	to delete all instances AND to delete the Base Module
        /// </remarks>
        ///	<param name="moduleId">The Id of the module to copy</param>
        ///	<param name="tabId">The Id of the current tab</param>
        ///	<param name="fromTabs">An ArrayList of TabItem objects</param>
        ///	<param name="includeCurrent">A flag to indicate whether to delete from the current tab
        ///		as identified ny tabId</param>
        ///	<param name="deleteBaseModule">A flag to indicate whether to delete the Module itself</param>
        public void DeleteAllModules( int moduleId, int tabId, ArrayList fromTabs, bool includeCurrent, bool deleteBaseModule )
        {
            int intTab;
            TabInfo objTab;

            //Iterate through collection deleting the module from each Tab (except the current)
            for( intTab = 0; intTab <= fromTabs.Count - 1; intTab++ )
            {
                objTab = (TabInfo)fromTabs[intTab];
                if( objTab.TabID != tabId || includeCurrent )
                {
                    DeleteTabModule( objTab.TabID, moduleId );
                }
            }

            //Optionally delete the Module
            if( includeCurrent && deleteBaseModule )
            {
                DeleteModule( moduleId );
            }
        }

        public void DeleteModule( int ModuleId )
        {
            DataProvider.Instance().DeleteModule( ModuleId );

            //Delete Search Items for this Module
            DataProvider.Instance().DeleteSearchItems( ModuleId );
        }

        public void DeleteModuleSetting( int ModuleId, string SettingName )
        {
            DataProvider.Instance().DeleteModuleSetting( ModuleId, SettingName );
            DataCache.RemoveCache( "GetModuleSettings" + ModuleId.ToString() );
        }

        public void DeleteModuleSettings( int ModuleId )
        {
            DataProvider.Instance().DeleteModuleSettings( ModuleId );
            DataCache.RemoveCache( "GetModuleSettings" + ModuleId.ToString() );
        }

        public void DeleteTabModule( int TabId, int ModuleId )
        {
            // save moduleinfo
            ModuleInfo objModule = GetModule( ModuleId, TabId );

            if( objModule != null )
            {
                // delete the module instance for the tab
                DataProvider.Instance().DeleteTabModule( TabId, ModuleId );

                // reorder all modules on tab
                UpdateTabModuleOrder( TabId, objModule.PortalID );

                // check if all modules instances have been deleted
                if( GetModule( ModuleId, Null.NullInteger ).TabID == Null.NullInteger )
                {
                    // soft delete the module
                    objModule.TabID = Null.NullInteger;
                    objModule.IsDeleted = true;
                    UpdateModule( objModule );

                    //Delete Search Items for this Module
                    DataProvider.Instance().DeleteSearchItems( ModuleId );
                }
            }
        }

        public void DeleteTabModuleSetting( int TabModuleId, string SettingName )
        {
            DataProvider.Instance().DeleteTabModuleSetting( TabModuleId, SettingName );
            DataCache.RemoveCache( "GetTabModuleSettings" + TabModuleId.ToString() );
        }

        public void DeleteTabModuleSettings( int TabModuleId )
        {
            DataProvider.Instance().DeleteTabModuleSettings( TabModuleId );
            DataCache.RemoveCache( "GetTabModuleSettings" + TabModuleId.ToString() );
        }

        /// <summary>
        /// MoveModule moes a Module from one Tab to another including all the
        ///	TabModule settings
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="moduleId">The Id of the module to move</param>
        ///	<param name="fromTabId">The Id of the source tab</param>
        ///	<param name="toTabId">The Id of the destination tab</param>
        ///	<param name="toPaneName">The name of the Pane on the destination tab where the module will end up</param>
        public void MoveModule( int moduleId, int fromTabId, int toTabId, string toPaneName )
        {
            //First copy the Module to the new Tab (including the TabModuleSettings)
            CopyModule( moduleId, fromTabId, toTabId, toPaneName, true );

            //Next Remove the Module from the source tab
            DeleteTabModule( fromTabId, moduleId );
        }

        public void UpdateModule( ModuleInfo objModule )
        {
            // update module
            DataProvider.Instance().UpdateModule( objModule.ModuleID, objModule.ModuleTitle, objModule.AllTabs, objModule.Header, objModule.Footer, objModule.StartDate, objModule.EndDate, objModule.InheritViewPermissions, objModule.IsDeleted );

            // update module permissions
            ModulePermissionController objModulePermissionController = new ModulePermissionController();
            ModulePermissionCollection objCurrentModulePermissions;
            objCurrentModulePermissions = objModulePermissionController.GetModulePermissionsCollectionByModuleID( objModule.ModuleID );
            if( !objCurrentModulePermissions.CompareTo( objModule.ModulePermissions ) )
            {
                objModulePermissionController.DeleteModulePermissionsByModuleID( objModule.ModuleID );
                ModulePermissionInfo objModulePermission;
                foreach( ModulePermissionInfo tempLoopVar_objModulePermission in objModule.ModulePermissions )
                {
                    objModulePermission = tempLoopVar_objModulePermission;
                    objModulePermission.ModuleID = objModule.ModuleID;
                    if( objModule.InheritViewPermissions && objModulePermission.PermissionKey == "VIEW" )
                    {
                        objModulePermissionController.DeleteModulePermission( objModulePermission.ModulePermissionID );
                    }
                    else
                    {
                        if( objModulePermission.AllowAccess )
                        {
                            objModulePermissionController.AddModulePermission( objModulePermission );
                        }
                    }
                }
            }

            if( !Null.IsNull( objModule.TabID ) )
            {
                // update tabmodule
                DataProvider.Instance().UpdateTabModule( objModule.TabID, objModule.ModuleID, objModule.ModuleOrder, objModule.PaneName, objModule.CacheTime, objModule.Alignment, objModule.Color, objModule.Border, objModule.IconFile, (int)objModule.Visibility, objModule.ContainerSrc, objModule.DisplayTitle, objModule.DisplayPrint, objModule.DisplaySyndicate );

                // update module order in pane
                UpdateModuleOrder( objModule.TabID, objModule.ModuleID, objModule.ModuleOrder, objModule.PaneName );

                // set the default module
                if( objModule.IsDefaultModule )
                {
                    PortalSettings.UpdatePortalSetting( objModule.PortalID, "defaultmoduleid", objModule.ModuleID.ToString() );
                    PortalSettings.UpdatePortalSetting( objModule.PortalID, "defaulttabid", objModule.TabID.ToString() );
                }

                // apply settings to all desktop modules in portal
                if( objModule.AllModules )
                {
                    ArrayList arrModules;
                    ModuleInfo objTargetModule;
                    TabController objTabs = new TabController();
                    ArrayList arrTabs = objTabs.GetTabs( objModule.PortalID );
                    TabInfo objTab;
                    foreach( TabInfo tempLoopVar_objTab in arrTabs )
                    {
                        objTab = tempLoopVar_objTab;
                        if( !objTab.IsAdminTab )
                        {
                            arrModules = GetPortalTabModules( objTab.PortalID, objTab.TabID );
                            foreach( ModuleInfo tempLoopVar_objTargetModule in arrModules )
                            {
                                objTargetModule = tempLoopVar_objTargetModule;
                                DataProvider.Instance().UpdateTabModule( objTargetModule.TabID, objTargetModule.ModuleID, objTargetModule.ModuleOrder, objTargetModule.PaneName, objModule.CacheTime, objModule.Alignment, objModule.Color, objModule.Border, objModule.IconFile, (int)objModule.Visibility, objModule.ContainerSrc, objModule.DisplayTitle, objModule.DisplayPrint, objModule.DisplaySyndicate );
                            }
                        }
                    }
                }
            }
        }

        public void UpdateModuleOrder( int TabId, int ModuleId, int ModuleOrder, string PaneName )
        {
            ModuleInfo objModule = GetModule( ModuleId, TabId );
            if( objModule != null )
            {
                // adding a module to a new pane - places the module at the bottom of the pane
                if( ModuleOrder == -1 )
                {
                    IDataReader dr = DataProvider.Instance().GetTabModuleOrder( TabId, PaneName );
                    while( dr.Read() )
                    {
                        ModuleOrder = Convert.ToInt32( dr["ModuleOrder"] );
                    }
                    dr.Close();
                    ModuleOrder += 2;
                }

                DataProvider.Instance().UpdateModuleOrder( TabId, ModuleId, ModuleOrder, PaneName );

                // clear cache
                if( objModule.AllTabs == false )
                {
                    DataCache.ClearTabCache( TabId, objModule.PortalID );
                }
                else
                {
                    TabController objTabs = new TabController();
                    TabInfo objTab = objTabs.GetTab( TabId );
                    if( objTab != null )
                    {
                        DataCache.ClearPortalCache( objTab.PortalID, true );
                    }
                }
            }
        }

        public void UpdateModuleSetting( int ModuleId, string SettingName, string SettingValue )
        {
            IDataReader dr = DataProvider.Instance().GetModuleSetting( ModuleId, SettingName );

            if( dr.Read() )
            {
                DataProvider.Instance().UpdateModuleSetting( ModuleId, SettingName, SettingValue );
            }
            else
            {
                DataProvider.Instance().AddModuleSetting( ModuleId, SettingName, SettingValue );
            }
            dr.Close();

            DataCache.RemoveCache( "GetModuleSettings" + ModuleId.ToString() );
        }

        public void UpdateTabModuleOrder( int TabId, int PortalId )
        {
            int ModuleCounter;
            IDataReader dr = DataProvider.Instance().GetTabPanes( TabId );
            while( dr.Read() )
            {
                ModuleCounter = 0;
                IDataReader dr2 = DataProvider.Instance().GetTabModuleOrder( TabId, Convert.ToString( dr["PaneName"] ) );
                while( dr2.Read() )
                {
                    ModuleCounter++;
                    DataProvider.Instance().UpdateModuleOrder( TabId, Convert.ToInt32( dr2["ModuleID"] ), ( ModuleCounter*2 ) - 1, Convert.ToString( dr["PaneName"] ) );
                }
                dr2.Close();
            }
            dr.Close();

            // clear tab cache
            DataCache.ClearTabCache( TabId, PortalId );
        }

        [Obsolete( "Use the new UpdateTabModuleOrder(tabid,portalid)" )]
        public void UpdateTabModuleOrder( int TabId )
        {
            int ModuleCounter;
            IDataReader dr = DataProvider.Instance().GetTabPanes( TabId );
            while( dr.Read() )
            {
                ModuleCounter = 0;
                IDataReader dr2 = DataProvider.Instance().GetTabModuleOrder( TabId, Convert.ToString( dr["PaneName"] ) );
                while( dr2.Read() )
                {
                    ModuleCounter++;
                    DataProvider.Instance().UpdateModuleOrder( TabId, Convert.ToInt32( dr2["ModuleID"] ), ( ModuleCounter*2 ) - 1, Convert.ToString( dr["PaneName"] ) );
                }
                dr2.Close();
            }
            dr.Close();

            // clear tab cache
            TabController objTabs = new TabController();
            TabInfo objTab = objTabs.GetTab( TabId );
            DataCache.ClearTabCache( TabId, objTab.PortalID );
        }

        public void UpdateTabModuleSetting( int TabModuleId, string SettingName, string SettingValue )
        {
            IDataReader dr = DataProvider.Instance().GetTabModuleSetting( TabModuleId, SettingName );

            if( dr.Read() )
            {
                DataProvider.Instance().UpdateTabModuleSetting( TabModuleId, SettingName, SettingValue );
            }
            else
            {
                DataProvider.Instance().AddTabModuleSetting( TabModuleId, SettingName, SettingValue );
            }
            dr.Close();

            DataCache.RemoveCache( "GetTabModuleSettings" + TabModuleId.ToString() );
        }
    }
}