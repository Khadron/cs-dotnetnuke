#region
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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Entities.Tabs
{
    public class TabController
    {
        private struct TabOrderHelper
        {
            public int TabOrder;
            public int Level;
            public int ParentId;

            public TabOrderHelper( int inttaborder, int intlevel, int intparentid )
            {
                TabOrder = inttaborder;
                Level = intlevel;
                ParentId = intparentid;
            }
        }

        private static void ClearCache( int portalId )
        {
            DataCache.ClearTabsCache( portalId );
        }

        private static TabInfo FillTabInfo( IDataReader dr )
        {
            return FillTabInfo( dr, true );
        }

        private static TabInfo FillTabInfo( IDataReader dr, bool CheckForOpenDataReader )
        {
            TabInfo objTabInfo = new TabInfo();
            TabPermissionController objTabPermissionController = new TabPermissionController();

            // read datareader
            bool canContinue = true;
            if( CheckForOpenDataReader )
            {
                canContinue = false;
                if( dr.Read() )
                {
                    canContinue = true;
                }
            }
            if( canContinue )
            {
                objTabInfo.TabID = Convert.ToInt32( Null.SetNull( dr["TabID"], objTabInfo.TabID ) );
                objTabInfo.TabOrder = Convert.ToInt32( Null.SetNull( dr["TabOrder"], objTabInfo.TabOrder ) );
                objTabInfo.PortalID = Convert.ToInt32( Null.SetNull( dr["PortalID"], objTabInfo.PortalID ) );
                objTabInfo.TabName = Convert.ToString( Null.SetNull( dr["TabName"], objTabInfo.TabName ) );
                objTabInfo.IsVisible = Convert.ToBoolean( Null.SetNull( dr["IsVisible"], objTabInfo.IsVisible ) );
                objTabInfo.ParentId = Convert.ToInt32( Null.SetNull( dr["ParentId"], objTabInfo.ParentId ) );
                objTabInfo.Level = Convert.ToInt32( Null.SetNull( dr["Level"], objTabInfo.Level ) );
                objTabInfo.IconFile = Convert.ToString( Null.SetNull( dr["IconFile"], objTabInfo.IconFile ) );
                objTabInfo.DisableLink = Convert.ToBoolean( Null.SetNull( dr["DisableLink"], objTabInfo.DisableLink ) );
                objTabInfo.Title = Convert.ToString( Null.SetNull( dr["Title"], objTabInfo.Title ) );
                objTabInfo.Description = Convert.ToString( Null.SetNull( dr["Description"], objTabInfo.Description ) );
                objTabInfo.KeyWords = Convert.ToString( Null.SetNull( dr["KeyWords"], objTabInfo.KeyWords ) );
                objTabInfo.IsDeleted = Convert.ToBoolean( Null.SetNull( dr["IsDeleted"], objTabInfo.IsDeleted ) );
                objTabInfo.Url = Convert.ToString( Null.SetNull( dr["Url"], objTabInfo.Url ) );
                objTabInfo.SkinSrc = Convert.ToString( Null.SetNull( dr["SkinSrc"], objTabInfo.SkinSrc ) );
                objTabInfo.ContainerSrc = Convert.ToString( Null.SetNull( dr["ContainerSrc"], objTabInfo.ContainerSrc ) );
                objTabInfo.TabPath = Convert.ToString( Null.SetNull( dr["TabPath"], objTabInfo.TabPath ) );
                objTabInfo.StartDate = Convert.ToDateTime( Null.SetNull( dr["StartDate"], objTabInfo.StartDate ) );
                objTabInfo.EndDate = Convert.ToDateTime( Null.SetNull( dr["EndDate"], objTabInfo.EndDate ) );
                objTabInfo.HasChildren = Convert.ToBoolean( Null.SetNull( dr["HasChildren"], objTabInfo.HasChildren ) );
                objTabInfo.RefreshInterval = Convert.ToInt32( Null.SetNull( dr["RefreshInterval"], objTabInfo.RefreshInterval ) );
                objTabInfo.PageHeadText = Convert.ToString( Null.SetNull( dr["PageHeadText"], objTabInfo.PageHeadText ) );

                if( objTabInfo != null )
                {
                    objTabInfo.TabPermissions = objTabPermissionController.GetTabPermissionsCollectionByTabID( objTabInfo.TabID, objTabInfo.PortalID );
                    objTabInfo.AdministratorRoles = objTabPermissionController.GetTabPermissions( objTabInfo.TabPermissions, "EDIT" );
                    if( objTabInfo.AdministratorRoles == ";" )
                    {
                        // this code is here for legacy support - the AdministratorRoles were stored as a concatenated list of roleids prior to DNN 3.0
                        try
                        {
                            objTabInfo.AdministratorRoles = Convert.ToString( Null.SetNull( dr["AdministratorRoles"], objTabInfo.AdministratorRoles ) );
                        }
                        catch
                        {
                            // the AdministratorRoles field was removed from the Tabs table in 3.0
                        }
                    }
                    objTabInfo.AuthorizedRoles = objTabPermissionController.GetTabPermissions( objTabInfo.TabPermissions, "VIEW" );
                    if( objTabInfo.AuthorizedRoles == ";" )
                    {
                        // this code is here for legacy support - the AuthorizedRoles were stored as a concatenated list of roleids prior to DNN 3.0
                        try
                        {
                            objTabInfo.AuthorizedRoles = Convert.ToString( Null.SetNull( dr["AuthorizedRoles"], objTabInfo.AuthorizedRoles ) );
                        }
                        catch
                        {
                            // the AuthorizedRoles field was removed from the Tabs table in 3.0
                        }
                    }
                }

                objTabInfo.BreadCrumbs = null;
                objTabInfo.Panes = null;
                objTabInfo.Modules = null;
            }
            else
            {
                objTabInfo = null;
            }

            return objTabInfo;
        }

        private static ArrayList FillTabInfoCollection( IDataReader dr )
        {
            ArrayList arr = new ArrayList();
            try
            {
                while( dr.Read() )
                {
                    // fill business object
                    TabInfo obj = FillTabInfo( dr, false );
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

        private static Dictionary<int, TabInfo> FillTabInfoDictionary( IDataReader dr )
        {
            Dictionary<int, TabInfo> dic = new Dictionary<int, TabInfo>();
            try
            {
                while( dr.Read() )
                {
                    // fill business object
                    TabInfo obj = FillTabInfo( dr, false );
                    // add to dictionary
                    dic.Add( obj.TabID, obj );
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

            return dic;
        }

        private TabInfo GetTabByNameAndParent( string TabName, int PortalId, int ParentId )
        {
            ArrayList arrTabs = GetTabsByNameAndPortal( TabName, PortalId );
            int intTab = -1;

            if( arrTabs != null )
            {
                switch( arrTabs.Count )
                {
                    case 0: // no results
                        break;
                    case 1: // exact match
                        intTab = 0;
                        break;
                    default: // multiple matches
                        int intIndex;
                        for( intIndex = 0; intIndex < arrTabs.Count; intIndex++ )
                        {
                            TabInfo objTab = (TabInfo)( arrTabs[intIndex] );
                            // check if the parentids match
                            if( objTab.ParentId == ParentId )
                            {
                                intTab = intIndex;
                            }
                        }
                        if( intTab == -1 )
                        {
                            // no match - return the first item
                            intTab = 0;
                        }
                        break;
                }
            }

            if( intTab != -1 )
            {
                if( arrTabs != null )
                {
                    return (TabInfo)( arrTabs[intTab] );
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        private ArrayList GetTabsByNameAndPortal( string TabName, int PortalId )
        {
            ArrayList returnTabs = new ArrayList();
            foreach( KeyValuePair<int, TabInfo> kvp in GetTabsByPortal( PortalId ) )
            {
                TabInfo objTab = kvp.Value;

                if( objTab.TabName == TabName )
                {
                    returnTabs.Add( objTab );
                }
            }
            return returnTabs;
        }

        private ArrayList GetTabsByParent( int ParentId, int PortalId )
        {
            ArrayList childTabs = new ArrayList();
            foreach( KeyValuePair<int, TabInfo> kvp in GetTabsByPortal( PortalId ) )
            {
                TabInfo objTab = kvp.Value;

                if( objTab.ParentId == ParentId )
                {
                    childTabs.Add( objTab );
                }
            }
            return childTabs;
        }

        private static void MoveTab( ArrayList objDesktopTabs, int intFromIndex, int intToIndex, int intNewLevel )
        {
            MoveTab( objDesktopTabs, intFromIndex, intToIndex, intNewLevel, true );
        }

        private static void MoveTab( ArrayList objDesktopTabs, int intFromIndex, int intToIndex, int intNewLevel, bool blnAddChild )
        {
            int intCounter;
            bool blnInsert = false;

            int intOldLevel = ( (TabInfo)( objDesktopTabs[intFromIndex] ) ).Level;
            if( intToIndex != objDesktopTabs.Count - 1 )
            {
                blnInsert = true;
            }

            // clone tab and children from old parent
            ArrayList objClone = new ArrayList();
            intCounter = intFromIndex;
            while( intCounter <= objDesktopTabs.Count - 1 )
            {
                if( ( (TabInfo)( objDesktopTabs[intCounter] ) ).TabID == ( (TabInfo)( objDesktopTabs[intFromIndex] ) ).TabID | ( (TabInfo)( objDesktopTabs[intCounter] ) ).Level > intOldLevel )
                {
                    objClone.Add( objDesktopTabs[intCounter] );
                    intCounter += 1;
                }
                else
                {
                    break;
                }
            }

            // remove tab and children from old parent
            objDesktopTabs.RemoveRange( intFromIndex, objClone.Count );
            if( intToIndex > intFromIndex )
            {
                intToIndex -= objClone.Count;
            }

            // add clone to new parent
            if( blnInsert )
            {
                objClone.Reverse();
            }

            foreach( TabInfo objTab in objClone )
            {
                objTab.TabPath = Globals.GenerateTabPath( objTab.ParentId, objTab.TabName );
                if( blnInsert )
                {
                    objTab.Level += ( intNewLevel - intOldLevel );
                    int intIncrement;
                    if( blnAddChild )
                    {
                        intIncrement = 1;
                    }
                    else
                    {
                        intIncrement = 0;
                    }
                    objDesktopTabs.Insert( intToIndex + intIncrement, objTab );
                }
                else
                {
                    objTab.Level += ( intNewLevel - intOldLevel );
                    objDesktopTabs.Add( objTab );
                }
            }
        }

        /// <summary>
        /// Updates child tabs TabPath field
        /// </summary>
        /// <param name="intTabid">ID of the parent tab</param>
        /// <remarks>
        /// When a ParentTab is updated this method should be called to 
        /// ensure that the TabPath of the Child Tabs is consistent with the Parent
        /// </remarks>
        /// <history>
        /// 	[JWhite]	16/11/2004	Created
        /// </history>
        /// <param name="portalId"></param>
        private void UpdateChildTabPath( int intTabid, int portalId )
        {
            ArrayList arrTabs = GetTabsByParentId( intTabid, portalId );

            foreach( TabInfo objtab in arrTabs )
            {
                string oldTabPath = objtab.TabPath;
                objtab.TabPath = Globals.GenerateTabPath( objtab.ParentId, objtab.TabName );
                if( oldTabPath != objtab.TabPath )
                {
                    DataProvider.Instance().UpdateTab( objtab.TabID, objtab.TabName, objtab.IsVisible, objtab.DisableLink, objtab.ParentId, objtab.IconFile, objtab.Title, objtab.Description, objtab.KeyWords, objtab.IsDeleted, objtab.Url, objtab.SkinSrc, objtab.ContainerSrc, objtab.TabPath, objtab.StartDate, objtab.EndDate, objtab.RefreshInterval, objtab.PageHeadText );
                    UpdateChildTabPath( objtab.TabID, objtab.PortalID );
                }
            }

            ClearCache( portalId );
        }

        public int AddTab( TabInfo objTab )
        {
            return AddTab( objTab, true );
        }

        public int AddTab( TabInfo objTab, bool AddAllTabsModules )
        {
            int intTabId;

            objTab.TabPath = Globals.GenerateTabPath( objTab.ParentId, objTab.TabName );
            intTabId = DataProvider.Instance().AddTab( objTab.PortalID, objTab.TabName, objTab.IsVisible, objTab.DisableLink, objTab.ParentId, objTab.IconFile, objTab.Title, objTab.Description, objTab.KeyWords, objTab.Url, objTab.SkinSrc, objTab.ContainerSrc, objTab.TabPath, objTab.StartDate, objTab.EndDate, objTab.RefreshInterval, objTab.PageHeadText );

            TabPermissionController objTabPermissionController = new TabPermissionController();

            if( objTab.TabPermissions != null )
            {
                TabPermissionCollection objTabPermissions = objTab.TabPermissions;

                foreach( TabPermissionInfo objTabPermission in objTabPermissions )
                {
                    objTabPermission.TabID = intTabId;
                    if( objTabPermission.AllowAccess )
                    {
                        objTabPermissionController.AddTabPermission( objTabPermission );
                    }
                }
            }
            if( !( Null.IsNull( objTab.PortalID ) ) )
            {
                UpdatePortalTabOrder( objTab.PortalID, intTabId, objTab.ParentId, 0, 0, objTab.IsVisible, true );
            }
            else // host tab
            {
                ArrayList arrTabs = GetTabsByParentId( objTab.ParentId, objTab.PortalID );
                UpdateTabOrder( objTab.PortalID, intTabId, ( arrTabs.Count*2 ) - 1, 1, objTab.ParentId );
            }

            if( AddAllTabsModules )
            {
                ModuleController objmodules = new ModuleController();
                ArrayList arrMods = objmodules.GetAllTabsModules( objTab.PortalID, true );

                foreach( ModuleInfo objModule in arrMods )
                {
                    objmodules.CopyModule( objModule.ModuleID, objModule.TabID, intTabId, "", true );
                }
            }

            ClearCache( objTab.PortalID );

            return intTabId;
        }

        public void CopyTab( int PortalId, int FromTabId, int ToTabId, bool IncludeContent )
        {
            ModuleController objModules = new ModuleController();

            foreach( KeyValuePair<int, ModuleInfo> kvp in objModules.GetTabModules( FromTabId ) )
            {
                ModuleInfo objModule = kvp.Value;

                // if the module shows on all pages does not need to be copied since it will
                // be already added to this page
                if( !objModule.AllTabs )
                {
                    if( IncludeContent == false )
                    {
                        objModule.ModuleID = Null.NullInteger;
                    }

                    objModule.TabID = ToTabId;
                    objModules.AddModule( objModule );
                }
            }
        }

        /// <summary>
        /// Deletes a tab premanently from the database
        /// </summary>
        /// <param name="TabId">TabId of the tab to be deleted</param>
        /// <param name="PortalId">PortalId of the portal</param>
        /// <history>
        /// 	[Vicenç]	19/09/2004	Added skin deassignment before deleting the tab.
        /// </history>
        public void DeleteTab( int TabId, int PortalId )
        {
            // parent tabs can not be deleted
            ArrayList arrTabs = GetTabsByParentId( TabId, PortalId );

            if( arrTabs.Count == 0 )
            {
                DataProvider.Instance().DeleteTab( TabId );
                UpdatePortalTabOrder( PortalId, TabId, -2, 0, 0, true );
            }

            ClearCache( PortalId );
        }

        public ArrayList GetAllTabs()
        {
            return FillTabInfoCollection( DataProvider.Instance().GetAllTabs() );
        }

        public TabInfo GetTab( int TabId, int PortalId, bool ignoreCache )
        {
            TabInfo tab = null;
            bool bFound = false;
            if( !ignoreCache )
            {
                Dictionary<int, TabInfo> dicTabs;
                //First try the cache
                if( PortalId > Null.NullInteger )
                {
                    dicTabs = GetTabsByPortal( PortalId );
                    bFound = dicTabs.TryGetValue( TabId, out tab );
                }
                else
                {
                    //Check all the loaded portals
                    foreach( KeyValuePair<int, int> kvpPortal in PortalController.GetPortalDictionary() )
                    {
                        dicTabs = GetTabsByPortal( kvpPortal.Key );
                        bFound = dicTabs.TryGetValue( TabId, out tab );
                        if( bFound )
                        {
                            break;
                        }
                    }
                }
            }

            if( ignoreCache | !bFound )
            {
                IDataReader dr = DataProvider.Instance().GetTab( TabId );
                try
                {
                    tab = FillTabInfo( dr );
                }
                finally
                {
                    if( dr != null )
                    {
                        dr.Close();
                    }
                }
            }
            return tab;
        }

        public TabInfo GetTabByName( string TabName, int PortalId )
        {
            return GetTabByNameAndParent( TabName, PortalId, int.MinValue );
        }

        public TabInfo GetTabByName( string TabName, int PortalId, int ParentId )
        {
            return GetTabByNameAndParent( TabName, PortalId, ParentId );
        }

        public int GetTabCount( int portalId )
        {
            return DataProvider.Instance().GetTabCount( portalId );
        }

        public ArrayList GetTabs( int PortalId )
        {
            ArrayList arrTabs = new ArrayList();
            foreach( KeyValuePair<int, TabInfo> tabPair in GetTabsByPortal( PortalId ) )
            {
                arrTabs.Add( tabPair.Value );
            }
            return arrTabs;
        }

        public ArrayList GetTabsByParentId( int ParentId, int PortalId )
        {
            return GetTabsByParent( ParentId, PortalId );
        }

        public Dictionary<int, TabInfo> GetTabsByPortal( int PortalId )
        {
            string key = string.Format( DataCache.TabCacheKey, PortalId );

            //First Check the Tab Cache
            Dictionary<int, TabInfo> tabs = DataCache.GetCache( key ) as Dictionary<int, TabInfo>;

            if( tabs == null )
            {
                //tab caching settings
                Int32 timeOut = DataCache.TabCacheTimeOut*Convert.ToInt32( Globals.PerformanceSetting );

                //Get tabs form Database
                tabs = FillTabInfoDictionary( DataProvider.Instance().GetTabs( PortalId ) );

                //Cache tabs
                if( timeOut > 0 )
                {
                    DataCache.SetCache( key, tabs, TimeSpan.FromMinutes( timeOut ) );
                }
            }

            return tabs;
        }

        public void UpdatePortalTabOrder( int PortalId, int TabId, int NewParentId, int Level, int Order, bool IsVisible )
        {
            UpdatePortalTabOrder( PortalId, TabId, NewParentId, Level, Order, IsVisible, false );
        }

        public void UpdatePortalTabOrder( int PortalId, int TabId, int NewParentId, int Level, int Order, bool IsVisible, bool NewTab )
        {
            TabInfo objTab;
            int intCounter = 0;
            int intFromIndex = -1;
            int intOldParentId = -2;
            int intToIndex = -1;
            int intNewParentIndex = 0;
            int intAddTabLevel = 0;

            PortalController objPortals = new PortalController();
            PortalInfo objPortal = objPortals.GetPortal( PortalId );

            //hashtable to prevent db calls when no change
            Hashtable htabs = new Hashtable();

            // create temporary tab collection
            ArrayList objTabs = new ArrayList();

            // populate temporary tab collection
            foreach( KeyValuePair<int, TabInfo> tabPair in GetTabsByPortal( PortalId ) )
            {
                objTab = tabPair.Value;

                if( NewTab == false | objTab.TabID != TabId )
                {
                    // save old data
                    htabs.Add( objTab.TabID, new TabOrderHelper( objTab.TabOrder, objTab.Level, objTab.ParentId ) );

                    if( objTab.TabOrder == 0 )
                    {
                        objTab.TabOrder = 999;
                    }
                    objTabs.Add( objTab );
                    if( objTab.TabID == TabId )
                    {
                        intOldParentId = objTab.ParentId;
                        intFromIndex = intCounter;
                    }
                    if( objTab.TabID == NewParentId )
                    {
                        intNewParentIndex = intCounter;
                        intAddTabLevel = objTab.Level + 1;
                    }
                    intCounter += 1;
                }
            }

            if( NewParentId != -2 ) // not deleted
            {
                // adding new tab
                if( intFromIndex == -1 )
                {
                    objTab = new TabInfo();
                    objTab.TabID = TabId;
                    objTab.ParentId = NewParentId;
                    objTab.IsVisible = IsVisible;
                    objTab.Level = intAddTabLevel;
                    objTabs.Add( objTab );
                    intFromIndex = objTabs.Count - 1;
                }

                if( Level == 0 & Order == 0 )
                {
                    ( (TabInfo)( objTabs[intFromIndex] ) ).IsVisible = IsVisible;
                }
            }

            if( NewParentId != -2 ) // not deleted
            {
                // if the parent changed or we added a new non root level tab
                int intLevel;
                if( intOldParentId != NewParentId & !( intOldParentId == -2 & NewParentId == -1 ) )
                {
                    // locate position of last child for new parent
                    if( NewParentId != -1 )
                    {
                        intLevel = ( (TabInfo)( objTabs[intNewParentIndex] ) ).Level;
                    }
                    else
                    {
                        intLevel = -1;
                    }

                    intCounter = intNewParentIndex + 1;
                    while( intCounter <= objTabs.Count - 1 )
                    {
                        if( ( (TabInfo)( objTabs[intCounter] ) ).Level > intLevel )
                        {
                            intToIndex = intCounter;
                        }
                        else
                        {
                            break;
                        }
                        intCounter += 1;
                    }
                    // adding to parent with no children
                    if( intToIndex == -1 )
                    {
                        intToIndex = intNewParentIndex;
                    }
                    // move tab
                    ( (TabInfo)( objTabs[intFromIndex] ) ).ParentId = NewParentId;
                    MoveTab( objTabs, intFromIndex, intToIndex, intLevel + 1 );
                }
                else
                {
                    // if level has changed
                    if( Level != 0 )
                    {
                        intLevel = ( (TabInfo)( objTabs[intFromIndex] ) ).Level;

                        bool blnValid = true;
                        switch( Level )
                        {
                            case -1:
                                if( intLevel == 0 )
                                {
                                    blnValid = false;
                                }
                                break;
                            case 1:
                                if( intFromIndex > 0 )
                                {
                                    if( intLevel > ( (TabInfo)( objTabs[intFromIndex - 1] ) ).Level )
                                    {
                                        blnValid = false;
                                    }
                                }
                                else
                                {
                                    blnValid = false;
                                }
                                break;
                        }

                        if( blnValid )
                        {
                            int intNewLevel;
                            if( Level == -1 )
                            {
                                intNewLevel = intLevel + Level;
                            }
                            else
                            {
                                intNewLevel = intLevel;
                            }

                            // get new parent
                            NewParentId = -2;
                            intCounter = intFromIndex - 1;
                            while( intCounter >= 0 & NewParentId == -2 )
                            {
                                if( ( (TabInfo)( objTabs[intCounter] ) ).Level == intNewLevel )
                                {
                                    if( Level == -1 )
                                    {
                                        NewParentId = ( (TabInfo)( objTabs[intCounter] ) ).ParentId;
                                    }
                                    else
                                    {
                                        NewParentId = ( (TabInfo)( objTabs[intCounter] ) ).TabID;
                                    }
                                    intNewParentIndex = intCounter;
                                }
                                intCounter -= 1;
                            }
                            ( (TabInfo)( objTabs[intFromIndex] ) ).ParentId = NewParentId;

                            if( Level == -1 )
                            {
                                // locate position of next child for parent
                                intToIndex = -1;
                                intCounter = intNewParentIndex + 1;
                                while( intCounter <= objTabs.Count - 1 )
                                {
                                    if( ( (TabInfo)( objTabs[intCounter] ) ).Level > intNewLevel )
                                    {
                                        intToIndex = intCounter;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    intCounter += 1;
                                }
                                // adding to parent with no children
                                if( intToIndex == -1 )
                                {
                                    intToIndex = intNewParentIndex;
                                }
                            }
                            else
                            {
                                intToIndex = intFromIndex - 1;
                                intNewLevel = intLevel + Level;
                            }

                            // move tab
                            if( intFromIndex == intToIndex )
                            {
                                ( (TabInfo)( objTabs[intFromIndex] ) ).Level = intNewLevel;
                            }
                            else
                            {
                                MoveTab( objTabs, intFromIndex, intToIndex, intNewLevel );
                            }
                        }
                    }
                    else
                    {
                        // if order has changed
                        if( Order != 0 )
                        {
                            intLevel = ( (TabInfo)( objTabs[intFromIndex] ) ).Level;

                            // find previous/next item for parent
                            intToIndex = -1;
                            intCounter = intFromIndex + Order;
                            while( intCounter >= 0 & intCounter <= objTabs.Count - 1 & intToIndex == -1 )
                            {
                                if( ( (TabInfo)( objTabs[intCounter] ) ).ParentId == NewParentId )
                                {
                                    intToIndex = intCounter;
                                }
                                intCounter += Order;
                            }
                            if( intToIndex != -1 )
                            {
                                if( Order == 1 )
                                {
                                    // locate position of next child for parent
                                    intNewParentIndex = intToIndex;
                                    intToIndex = -1;
                                    intCounter = intNewParentIndex + 1;
                                    while( intCounter <= objTabs.Count - 1 )
                                    {
                                        if( ( (TabInfo)( objTabs[intCounter] ) ).Level > intLevel )
                                        {
                                            intToIndex = intCounter;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        intCounter += 1;
                                    }
                                    // adding to parent with no children
                                    if( intToIndex == -1 )
                                    {
                                        intToIndex = intNewParentIndex;
                                    }
                                    intToIndex += 1;
                                }
                                MoveTab( objTabs, intFromIndex, intToIndex, intLevel, false );
                            }
                        }
                    }
                }
            }

            // update the tabs
            int intDesktopTabOrder = -1;
            int intAdminTabOrder = 9999; // this seeds the taborder for the admin tab so that they are always at the end of the tab list ( max = 5000 desktop tabs per portal )
            foreach( TabInfo objTabWithinLoop in objTabs )
            {
                objTab = objTabWithinLoop;
                int intTabOrder;
                if( ( ( objTabWithinLoop.TabID == objPortal.AdminTabId ) | ( objTabWithinLoop.ParentId == objPortal.AdminTabId ) | ( objTabWithinLoop.TabID == objPortal.SuperTabId ) | ( objTabWithinLoop.ParentId == objPortal.SuperTabId ) ) & ( objPortal.AdminTabId != -1 ) ) // special case when creating new portals
                {
                    intAdminTabOrder += 2;
                    intTabOrder = intAdminTabOrder;
                }
                else // desktop tab
                {
                    intDesktopTabOrder += 2;
                    intTabOrder = intDesktopTabOrder;
                }
                // update only if changed
                if( htabs.Contains( objTabWithinLoop.TabID ) )
                {
                    TabOrderHelper ttab = (TabOrderHelper)( htabs[objTab.TabID] );
                    if( intTabOrder != ttab.TabOrder | objTabWithinLoop.Level != ttab.Level | objTabWithinLoop.ParentId != ttab.ParentId )
                    {
                        UpdateTabOrder( objTabWithinLoop.PortalID, objTabWithinLoop.TabID, intTabOrder, objTabWithinLoop.Level, objTabWithinLoop.ParentId );
                    }
                }
                else
                {
                    UpdateTabOrder( objTabWithinLoop.PortalID, objTabWithinLoop.TabID, intTabOrder, objTabWithinLoop.Level, objTabWithinLoop.ParentId );
                }
            }

            //clear Tabs cache
            ClearCache( PortalId );
        }

        public void UpdateTab( TabInfo objTab )
        {
            bool updateChildren = false;
            TabInfo objTmpTab = GetTab( objTab.TabID, objTab.PortalID, false );
            if( objTmpTab.TabName != objTab.TabName | objTmpTab.ParentId != objTab.ParentId )
            {
                updateChildren = true;
            }

            UpdatePortalTabOrder( objTab.PortalID, objTab.TabID, objTab.ParentId, 0, 0, objTab.IsVisible );

            DataProvider.Instance().UpdateTab( objTab.TabID, objTab.TabName, objTab.IsVisible, objTab.DisableLink, objTab.ParentId, objTab.IconFile, objTab.Title, objTab.Description, objTab.KeyWords, objTab.IsDeleted, objTab.Url, objTab.SkinSrc, objTab.ContainerSrc, objTab.TabPath, objTab.StartDate, objTab.EndDate, objTab.RefreshInterval, objTab.PageHeadText );

            TabPermissionController objTabPermissionController = new TabPermissionController();

            TabPermissionCollection objTabPermissions = objTab.TabPermissions;

            TabPermissionCollection objCurrentTabPermissions = objTabPermissionController.GetTabPermissionsCollectionByTabID( objTab.TabID, objTab.PortalID );

            if( !( objCurrentTabPermissions.CompareTo( objTab.TabPermissions ) ) )
            {
                objTabPermissionController.DeleteTabPermissionsByTabID( objTab.TabID );

                foreach( TabPermissionInfo objTabPermission in objTabPermissions )
                {
                    if( objTabPermission.AllowAccess )
                    {
                        objTabPermissionController.AddTabPermission( objTabPermission );
                    }
                }
            }
            if( updateChildren )
            {
                UpdateChildTabPath( objTab.TabID, objTab.PortalID );
            }

            ClearCache( objTab.PortalID );
        }

        public void UpdateTabOrder( int PortalID, int TabId, int TabOrder, int Level, int ParentId )
        {
            DataProvider.Instance().UpdateTabOrder( TabId, TabOrder, Level, ParentId );
            ClearCache( PortalID );
        }

        public void CopyDesignToChildren( ArrayList tabs, string skinSrc, string containerSrc )
        {
            foreach( TabInfo objTab in tabs )
            {
                DataProvider.Instance().UpdateTab( objTab.TabID, objTab.TabName, objTab.IsVisible, objTab.DisableLink, objTab.ParentId, objTab.IconFile, objTab.Title, objTab.Description, objTab.KeyWords, objTab.IsDeleted, objTab.Url, skinSrc, containerSrc, objTab.TabPath, objTab.StartDate, objTab.EndDate, objTab.RefreshInterval, objTab.PageHeadText );
            }

            if( tabs.Count > 0 )
            {
                DataCache.ClearTabsCache( ( (TabInfo)( tabs[0] ) ).PortalID );
            }
        }

        public void CopyPermissionsToChildren( ArrayList tabs, TabPermissionCollection newPermissions )
        {
            TabPermissionController objTabPermissionController = new TabPermissionController();

            foreach( TabInfo objTab in tabs )
            {
                TabPermissionCollection objCurrentTabPermissions = objTabPermissionController.GetTabPermissionsCollectionByTabID( objTab.TabID, objTab.PortalID );

                if( !( objCurrentTabPermissions.CompareTo( newPermissions ) ) )
                {
                    objTabPermissionController.DeleteTabPermissionsByTabID( objTab.TabID );

                    foreach( TabPermissionInfo objTabPermission in newPermissions )
                    {
                        if( objTabPermission.AllowAccess )
                        {
                            objTabPermission.TabID = objTab.TabID;
                            objTabPermissionController.AddTabPermission( objTabPermission );
                        }
                    }
                }
            }

            if( tabs.Count > 0 )
            {
                DataCache.ClearTabsCache( ( (TabInfo)( tabs[0] ) ).PortalID );
            }
        }

        [Obsolete( "This method is obsolete.  It has been replaced by GetTab(ByVal TabId As Integer, ByVal PortalId As Integer, ByVal ignoreCache As Boolean) " )]
        public TabInfo GetTab( int TabId )
        {
            IDataReader dr = DataProvider.Instance().GetTab( TabId );
            try
            {
                return FillTabInfo( dr );
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
        }

        [Obsolete( "This method is obsolete.  It has been replaced by GetTabsByParentId(ByVal ParentId As Integer, ByVal PortalId As Integer) " )]
        public ArrayList GetTabsByParentId( int ParentId )
        {
            return FillTabInfoCollection( DataProvider.Instance().GetTabsByParentId( ParentId ) );
        }
    }
}