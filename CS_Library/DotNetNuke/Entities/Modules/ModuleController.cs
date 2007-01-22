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
using System.Collections.Generic;
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
                    
                    foreach( ModulePermissionInfo objModulePermission in objModulePermissions )
                    {
                        objModulePermission.ModuleID = objModule.ModuleID;
                        objModulePermissionController.AddModulePermission( objModulePermission, objModule.TabID );
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
            ClearCache( objModule.TabID );
            return objModule.ModuleID;
        }

        private static void ClearCache(int TabId)
        {
            DataCache.ClearModuleCache(TabId);
        }

        private ModuleInfo FillModuleInfo(IDataReader dr)
        {
            return FillModuleInfo(dr, true, true);
        }

        private ModuleInfo FillModuleInfo(IDataReader dr, bool CheckForOpenDataReader)
        {
            return FillModuleInfo(dr, CheckForOpenDataReader, true);
        }

        private ModuleInfo FillModuleInfo(IDataReader dr, bool CheckForOpenDataReader, bool IncludePermissions)
        {
            ModuleInfo objModuleInfo = new ModuleInfo();
            ModulePermissionController objModulePermissionController = new ModulePermissionController();
            // read datareader
            bool canContinue = true;

            if (CheckForOpenDataReader)
            {
                canContinue = false;
                if (dr.Read())
                {
                    canContinue = true;
                }
            }
            if (canContinue)
            {
                objModuleInfo.PortalID = Convert.ToInt32(Null.SetNull(dr["PortalID"], objModuleInfo.PortalID));
                objModuleInfo.TabID = Convert.ToInt32(Null.SetNull(dr["TabID"], objModuleInfo.TabID));
                objModuleInfo.TabModuleID = Convert.ToInt32(Null.SetNull(dr["TabModuleID"], objModuleInfo.TabModuleID));
                objModuleInfo.ModuleID = Convert.ToInt32(Null.SetNull(dr["ModuleID"], objModuleInfo.ModuleID));
                objModuleInfo.ModuleDefID = Convert.ToInt32(Null.SetNull(dr["ModuleDefID"], objModuleInfo.ModuleDefID));
                objModuleInfo.ModuleOrder = Convert.ToInt32(Null.SetNull(dr["ModuleOrder"], objModuleInfo.ModuleOrder));
                objModuleInfo.PaneName = Convert.ToString(Null.SetNull(dr["PaneName"], objModuleInfo.PaneName));
                objModuleInfo.ModuleTitle = Convert.ToString(Null.SetNull(dr["ModuleTitle"], objModuleInfo.ModuleTitle));
                objModuleInfo.CacheTime = Convert.ToInt32(Null.SetNull(dr["CacheTime"], objModuleInfo.CacheTime));
                objModuleInfo.Alignment = Convert.ToString(Null.SetNull(dr["Alignment"], objModuleInfo.Alignment));
                objModuleInfo.Color = Convert.ToString(Null.SetNull(dr["Color"], objModuleInfo.Color));
                objModuleInfo.Border = Convert.ToString(Null.SetNull(dr["Border"], objModuleInfo.Border));
                objModuleInfo.IconFile = Convert.ToString(Null.SetNull(dr["IconFile"], objModuleInfo.IconFile));
                objModuleInfo.AllTabs = Convert.ToBoolean(Null.SetNull(dr["AllTabs"], objModuleInfo.AllTabs));
                int intVisibility = 0;
                if (((Convert.ToInt32(Null.SetNull(dr["Visibility"], intVisibility))) == 0) || ((Convert.ToInt32(Null.SetNull(dr["Visibility"], intVisibility))) == Null.NullInteger))
                {
                    objModuleInfo.Visibility = VisibilityState.Maximized;
                }
                else if ((Convert.ToInt32(Null.SetNull(dr["Visibility"], intVisibility))) == 1)
                {
                    objModuleInfo.Visibility = VisibilityState.Minimized;
                }
                else if ((Convert.ToInt32(Null.SetNull(dr["Visibility"], intVisibility))) == 2)
                {
                    objModuleInfo.Visibility = VisibilityState.None;
                }
                objModuleInfo.IsDeleted = Convert.ToBoolean(Null.SetNull(dr["IsDeleted"], objModuleInfo.IsDeleted));
                objModuleInfo.Header = Convert.ToString(Null.SetNull(dr["Header"], objModuleInfo.Header));
                objModuleInfo.Footer = Convert.ToString(Null.SetNull(dr["Footer"], objModuleInfo.Footer));
                objModuleInfo.StartDate = Convert.ToDateTime(Null.SetNull(dr["StartDate"], objModuleInfo.StartDate));
                objModuleInfo.EndDate = Convert.ToDateTime(Null.SetNull(dr["EndDate"], objModuleInfo.EndDate));
                objModuleInfo.ContainerSrc = Convert.ToString(Null.SetNull(dr["ContainerSrc"], objModuleInfo.ContainerSrc));
                objModuleInfo.DisplayTitle = Convert.ToBoolean(Null.SetNull(dr["DisplayTitle"], objModuleInfo.DisplayTitle));
                objModuleInfo.DisplayPrint = Convert.ToBoolean(Null.SetNull(dr["DisplayPrint"], objModuleInfo.DisplayPrint));
                objModuleInfo.DisplaySyndicate = Convert.ToBoolean(Null.SetNull(dr["DisplaySyndicate"], objModuleInfo.DisplaySyndicate));
                objModuleInfo.InheritViewPermissions = Convert.ToBoolean(Null.SetNull(dr["InheritViewPermissions"], objModuleInfo.InheritViewPermissions));
                objModuleInfo.DesktopModuleID = Convert.ToInt32(Null.SetNull(dr["DesktopModuleID"], objModuleInfo.DesktopModuleID));
                objModuleInfo.FriendlyName = Convert.ToString(Null.SetNull(dr["FriendlyName"], objModuleInfo.FriendlyName));
                objModuleInfo.Description = Convert.ToString(Null.SetNull(dr["Description"], objModuleInfo.Description));
                objModuleInfo.Version = Convert.ToString(Null.SetNull(dr["Version"], objModuleInfo.Version));
                objModuleInfo.IsPremium = Convert.ToBoolean(Null.SetNull(dr["IsPremium"], objModuleInfo.IsPremium));
                objModuleInfo.IsAdmin = Convert.ToBoolean(Null.SetNull(dr["IsAdmin"], objModuleInfo.IsAdmin));
                objModuleInfo.BusinessControllerClass = Convert.ToString(Null.SetNull(dr["BusinessControllerClass"], objModuleInfo.BusinessControllerClass));
                objModuleInfo.SupportedFeatures = Convert.ToInt32(Null.SetNull(dr["SupportedFeatures"], objModuleInfo.SupportedFeatures));
                objModuleInfo.ModuleControlId = Convert.ToInt32(Null.SetNull(dr["ModuleControlId"], objModuleInfo.ModuleControlId));
                objModuleInfo.ControlSrc = Convert.ToString(Null.SetNull(dr["ControlSrc"], objModuleInfo.ControlSrc));
                int intControlType = 0;
                if ((Convert.ToInt32(Null.SetNull(dr["ControlType"], intControlType))) == -3)
                {
                    objModuleInfo.ControlType = SecurityAccessLevel.ControlPanel;
                }
                else if ((Convert.ToInt32(Null.SetNull(dr["ControlType"], intControlType))) == -2)
                {
                    objModuleInfo.ControlType = SecurityAccessLevel.SkinObject;
                }
                else if (((Convert.ToInt32(Null.SetNull(dr["ControlType"], intControlType))) == -1) || ((Convert.ToInt32(Null.SetNull(dr["ControlType"], intControlType))) == Null.NullInteger))
                {
                    objModuleInfo.ControlType = SecurityAccessLevel.Anonymous;
                }
                else if ((Convert.ToInt32(Null.SetNull(dr["ControlType"], intControlType))) == 0)
                {
                    objModuleInfo.ControlType = SecurityAccessLevel.View;
                }
                else if ((Convert.ToInt32(Null.SetNull(dr["ControlType"], intControlType))) == 1)
                {
                    objModuleInfo.ControlType = SecurityAccessLevel.Edit;
                }
                else if ((Convert.ToInt32(Null.SetNull(dr["ControlType"], intControlType))) == 2)
                {
                    objModuleInfo.ControlType = SecurityAccessLevel.Admin;
                }
                else if ((Convert.ToInt32(Null.SetNull(dr["ControlType"], intControlType))) == 3)
                {
                    objModuleInfo.ControlType = SecurityAccessLevel.Host;
                }
                objModuleInfo.ControlTitle = Convert.ToString(Null.SetNull(dr["ControlTitle"], objModuleInfo.ControlTitle));
                objModuleInfo.HelpUrl = Convert.ToString(Null.SetNull(dr["HelpUrl"], objModuleInfo.HelpUrl));

                if (IncludePermissions)
                {
                    if (objModuleInfo != null)
                    {
                        //Get the Module permissions first (then we can parse the collection to determine the View/Edit Roles)
                        objModuleInfo.ModulePermissions = objModulePermissionController.GetModulePermissionsCollectionByModuleID(objModuleInfo.ModuleID, objModuleInfo.TabID);
                        objModuleInfo.AuthorizedEditRoles = objModulePermissionController.GetModulePermissions(objModuleInfo.ModulePermissions, "EDIT");
                        if (objModuleInfo.AuthorizedEditRoles == ";")
                        {
                            // this code is here for legacy support - the AuthorizedEditRoles were stored as a concatenated list of roleids prior to DNN 3.0
                            try
                            {
                                objModuleInfo.AuthorizedEditRoles = Convert.ToString(Null.SetNull(dr["AuthorizedEditRoles"], objModuleInfo.AuthorizedEditRoles));
                            }
                            catch
                            {
                                // the AuthorizedEditRoles field was removed from the Tabs table in 3.0
                            }
                        }
                        try
                        {
                            if (objModuleInfo.InheritViewPermissions)
                            {
                                TabPermissionController objTabPermissionController = new TabPermissionController();
                                TabPermissionCollection objTabPermissionCollection = objTabPermissionController.GetTabPermissionsCollectionByTabID(objModuleInfo.TabID, objModuleInfo.PortalID);
                                objModuleInfo.AuthorizedViewRoles = objTabPermissionController.GetTabPermissions(objTabPermissionCollection, "VIEW");
                            }
                            else
                            {
                                objModuleInfo.AuthorizedViewRoles = objModulePermissionController.GetModulePermissions(objModuleInfo.ModulePermissions, "VIEW");
                            }
                            if (objModuleInfo.AuthorizedViewRoles == ";")
                            {
                                // this code is here for legacy support - the AuthorizedViewRoles were stored as a concatenated list of roleids prior to DNN 3.0
                                try
                                {
                                    objModuleInfo.AuthorizedViewRoles = Convert.ToString(Null.SetNull(dr["AuthorizedViewRoles"], objModuleInfo.AuthorizedViewRoles));
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

        private ArrayList FillModuleInfoCollection(IDataReader dr)
        {
            return FillModuleInfoCollection(dr, true);
        }

        private ArrayList FillModuleInfoCollection(IDataReader dr, bool IncludePermissions)
        {
            ArrayList arr = new ArrayList();
            try
            {
                while (dr.Read())
                {
                    // fill business object
                    ModuleInfo obj = FillModuleInfo(dr, false, IncludePermissions);
                    // add to collection
                    arr.Add(obj);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                // close datareader
                if (dr != null)
                {
                    dr.Close();
                }
            }

            return arr;
        }

        private Dictionary<int, ModuleInfo> FillModuleInfoDictionary(IDataReader dr)
        {
            Dictionary<int, ModuleInfo> dic = new Dictionary<int, ModuleInfo>();
            try
            {
                while (dr.Read())
                {
                    // fill business object
                    ModuleInfo obj = FillModuleInfo(dr, false, true);
                    // add to dictionary
                    dic.Add(obj.ModuleID, obj);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                // close datareader
                if (dr != null)
                {
                    dr.Close();
                }
            }

            return dic;
        }

        public ArrayList GetAllModules()
        {
            return FillModuleInfoCollection( DataProvider.Instance().GetAllModules() );
        }

        public ArrayList GetAllTabsModules( int PortalID, bool AllTabs )
        {
            return FillModuleInfoCollection( DataProvider.Instance().GetAllTabsModules( PortalID, AllTabs ) );
        }

        public ModuleInfo GetModule(int ModuleId, int TabId, bool ignoreCache)
        {
            ModuleInfo modInfo = null;
            bool bFound = false;
            if (!ignoreCache)
            {
                //First try the cache
                Dictionary<int, ModuleInfo> dicModules = GetTabModules(TabId);
                bFound = dicModules.TryGetValue(ModuleId, out modInfo);
            }

            if (ignoreCache | !bFound)
            {
                IDataReader dr = DataProvider.Instance().GetModule(ModuleId, TabId);
                try
                {
                    modInfo = FillModuleInfo(dr);
                }
                finally
                {
                    if (dr != null)
                    {
                        dr.Close();
                    }
                }
            }
            return modInfo;
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
            string strCacheKey = "GetModuleSettings" + ModuleId;

            Hashtable objSettings = (Hashtable)DataCache.GetCache( strCacheKey );

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


        public ArrayList GetSearchModules(int PortalId)
        {
            return FillModuleInfoCollection(DataProvider.Instance().GetSearchModules(PortalId));
        }

        public Dictionary<int, ModuleInfo> GetTabModules(int TabId)
        {
            string key = string.Format(DataCache.TabModuleCacheKey, TabId);

            //First Check the Tab Cache
            Dictionary<int, ModuleInfo> modules = DataCache.GetCache(key) as Dictionary<int, ModuleInfo>;

            if (modules == null)
            {
                //tabmodule caching settings
                Int32 timeOut = DataCache.TabModuleCacheTimeOut * Convert.ToInt32(Globals.PerformanceSetting);

                //Get modules form Database
                modules = FillModuleInfoDictionary(DataProvider.Instance().GetTabModules(TabId));

                //Cache tabs
                if (timeOut > 0)
                {
                    DataCache.SetCache(key, modules, TimeSpan.FromMinutes(timeOut));
                }
            }
            return modules;
        }

        public Hashtable GetTabModuleSettings( int TabModuleId )
        {
            string strCacheKey = "GetTabModuleSettings" + TabModuleId;

            Hashtable objSettings = (Hashtable)DataCache.GetCache( strCacheKey );

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
            ModuleInfo objModule = GetModule( moduleId, fromTabId, false );

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
                    ModuleInfo toModule = GetModule( moduleId, toTabId, false );
                    CopyTabModuleSettings( objModule, toModule );
                }
            }
            catch
            {
                // module already in the page, ignore error
            }

            ClearCache( fromTabId );
            ClearCache( toTabId );
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
            //Iterate through collection copying the module to each Tab (except the source)
            foreach (TabInfo objTab in toTabs)
            {
                if (objTab.TabID != fromTabId)
                {
                    CopyModule(moduleId, fromTabId, objTab.TabID, "", includeSettings);
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
            //Iterate through collection deleting the module from each Tab (except the current)
            foreach (TabInfo objTab in fromTabs)
            {
                if (objTab.TabID != tabId | includeCurrent)
                {
                    DeleteTabModule(objTab.TabID, moduleId);
                }
            }

            //Optionally delete the Module
            if (includeCurrent & deleteBaseModule)
            {
                DeleteModule(moduleId);
            }

            ClearCache(tabId);
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
            DataCache.RemoveCache( "GetModuleSettings" + ModuleId );
        }

        public void DeleteModuleSettings( int ModuleId )
        {
            DataProvider.Instance().DeleteModuleSettings( ModuleId );
            DataCache.RemoveCache( "GetModuleSettings" + ModuleId );
        }

        public void DeleteTabModule( int TabId, int ModuleId )
        {
            // save moduleinfo
            ModuleInfo objModule = GetModule( ModuleId, TabId, false );

            if( objModule != null )
            {
                // delete the module instance for the tab
                DataProvider.Instance().DeleteTabModule( TabId, ModuleId );

                // reorder all modules on tab
                UpdateTabModuleOrder( TabId, objModule.PortalID );

                // check if all modules instances have been deleted
                if( GetModule( ModuleId, Null.NullInteger, true ).TabID == Null.NullInteger )
                {
                    // soft delete the module
                    objModule.TabID = Null.NullInteger;
                    objModule.IsDeleted = true;
                    UpdateModule( objModule );

                    //Delete Search Items for this Module
                    DataProvider.Instance().DeleteSearchItems( ModuleId );
                }
            }
            ClearCache( TabId );
        }

        public void DeleteTabModuleSetting( int TabModuleId, string SettingName )
        {
            DataProvider.Instance().DeleteTabModuleSetting( TabModuleId, SettingName );
            DataCache.RemoveCache( "GetTabModuleSettings" + TabModuleId );
        }

        public void DeleteTabModuleSettings( int TabModuleId )
        {
            DataProvider.Instance().DeleteTabModuleSettings( TabModuleId );
            DataCache.RemoveCache( "GetTabModuleSettings" + TabModuleId );
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
            objCurrentModulePermissions = objModulePermissionController.GetModulePermissionsCollectionByModuleID( objModule.ModuleID, objModule.TabID );
            if( !objCurrentModulePermissions.CompareTo( objModule.ModulePermissions ) )
            {
                objModulePermissionController.DeleteModulePermissionsByModuleID(objModule.ModuleID);
                foreach( ModulePermissionInfo objModulePermission in objModule.ModulePermissions )
                {
                    objModulePermission.ModuleID = objModule.ModuleID;
                    if( objModule.InheritViewPermissions && objModulePermission.PermissionKey == "VIEW" )
                    {
                        objModulePermissionController.DeleteModulePermission( objModulePermission.ModulePermissionID );
                    }
                    else
                    {
                        if( objModulePermission.AllowAccess )
                        {
                            objModulePermissionController.AddModulePermission( objModulePermission, objModule.TabID );
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
                if (objModule.AllModules)
                {
                    TabController objTabs = new TabController();
                    foreach (KeyValuePair<int, TabInfo> tabPair in objTabs.GetTabsByPortal(objModule.PortalID))
                    {
                        TabInfo objTab = tabPair.Value;
                        if (!objTab.IsAdminTab)
                        {
                            foreach (KeyValuePair<int, ModuleInfo> modulePair in GetTabModules(objTab.TabID))
                            {
                                ModuleInfo objTargetModule = modulePair.Value;
                                DataProvider.Instance().UpdateTabModule(objTargetModule.TabID, objTargetModule.ModuleID, objTargetModule.ModuleOrder, objTargetModule.PaneName, objModule.CacheTime, objModule.Alignment, objModule.Color, objModule.Border, objModule.IconFile, (int)objModule.Visibility, objModule.ContainerSrc, objModule.DisplayTitle, objModule.DisplayPrint, objModule.DisplaySyndicate);
                            }
                        }
                    }
                }
            }
            ClearCache( objModule.TabID );
        }

        public void UpdateModuleOrder( int TabId, int ModuleId, int ModuleOrder, string PaneName )
        {
            ModuleInfo objModule = GetModule(ModuleId, TabId, false);
            if (objModule != null)
            {
                // adding a module to a new pane - places the module at the bottom of the pane 
                if (ModuleOrder == -1)
                {
                    IDataReader dr = DataProvider.Instance().GetTabModuleOrder(TabId, PaneName);
                    while (dr.Read())
                    {
                        ModuleOrder = Convert.ToInt32(dr["ModuleOrder"]);
                    }
                    dr.Close();
                    ModuleOrder += 2;
                }

                DataProvider.Instance().UpdateModuleOrder(TabId, ModuleId, ModuleOrder, PaneName);

                // clear cache
                if (objModule.AllTabs == false)
                {
                    ClearCache(TabId);
                }
                else
                {
                    TabController objTabs = new TabController();
                    foreach (KeyValuePair<int, TabInfo> tabPair in objTabs.GetTabsByPortal(objModule.PortalID))
                    {
                        TabInfo objTab = tabPair.Value;
                        ClearCache(objTab.TabID);
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

            DataCache.RemoveCache( "GetModuleSettings" + ModuleId );
        }

        public void UpdateTabModuleOrder( int TabId, int PortalId )
        {
            IDataReader dr = DataProvider.Instance().GetTabPanes( TabId );
            while( dr.Read() )
            {
                int ModuleCounter = 0;
                IDataReader dr2 = DataProvider.Instance().GetTabModuleOrder( TabId, Convert.ToString( dr["PaneName"] ) );
                while( dr2.Read() )
                {
                    ModuleCounter++;
                    DataProvider.Instance().UpdateModuleOrder( TabId, Convert.ToInt32( dr2["ModuleID"] ), ( ModuleCounter*2 ) - 1, Convert.ToString( dr["PaneName"] ) );
                }
                dr2.Close();
            }
            dr.Close();

            // clear module cache
            ClearCache( TabId );
        }

        [Obsolete("")]
        public ModuleInfo GetModule(int ModuleId, int TabId)
        {
            return GetModule(ModuleId, TabId, true);
        }

        [Obsolete("Use the new GetTabModules(int TabId)")]
        public ArrayList GetPortalTabModules(int PortalId, int TabId)
        {
            ArrayList arr = new ArrayList();
            foreach (KeyValuePair<int, ModuleInfo> kvp in GetTabModules(TabId))
            {
                arr.Add(kvp.Value);
            }
            return arr;
        }

        [Obsolete( "Use the new UpdateTabModuleOrder(tabid,portalid)" )]
        public void UpdateTabModuleOrder( int TabId )
        {
            IDataReader dr = DataProvider.Instance().GetTabPanes( TabId );
            while( dr.Read() )
            {
                int ModuleCounter = 0;
                IDataReader dr2 = DataProvider.Instance().GetTabModuleOrder( TabId, Convert.ToString( dr["PaneName"] ) );
                while( dr2.Read() )
                {
                    ModuleCounter++;
                    DataProvider.Instance().UpdateModuleOrder( TabId, Convert.ToInt32( dr2["ModuleID"] ), ( ModuleCounter*2 ) - 1, Convert.ToString( dr["PaneName"] ) );
                }
                dr2.Close();
            }
            dr.Close();

            // clear module cache
            DataCache.ClearModuleCache( TabId );
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

            DataCache.RemoveCache( "GetTabModuleSettings" + TabModuleId );
        }
    }
}