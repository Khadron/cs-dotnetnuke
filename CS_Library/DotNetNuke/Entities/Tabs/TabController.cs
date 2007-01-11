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

            public TabOrderHelper(int inttaborder, int intlevel, int intparentid)
            {
                TabOrder = inttaborder;
                Level = intlevel;
                ParentId = intparentid;
            }
        }

        public int AddTab(TabInfo objTab)
        {
            return AddTab(objTab, true);
        }

        public int AddTab(TabInfo objTab, bool AddAllTabsModules)
        {
            int intTabId;

            objTab.TabPath = Globals.GenerateTabPath(objTab.ParentId, objTab.TabName);
            intTabId = DataProvider.Instance().AddTab(objTab.PortalID, objTab.TabName, objTab.IsVisible, objTab.DisableLink, objTab.ParentId, objTab.IconFile, objTab.Title, objTab.Description, objTab.KeyWords, objTab.Url, objTab.SkinSrc, objTab.ContainerSrc, objTab.TabPath, objTab.StartDate, objTab.EndDate, objTab.RefreshInterval, objTab.PageHeadText);

            TabController objTabController = new TabController();
            TabPermissionController objTabPermissionController = new TabPermissionController();

            if (objTab.TabPermissions != null)
            {
                TabPermissionCollection objTabPermissions;
                objTabPermissions = objTab.TabPermissions;

                TabPermissionInfo objTabPermission = new TabPermissionInfo();
                foreach (TabPermissionInfo tempLoopVar_objTabPermission in objTabPermissions)
                {
                    objTabPermission = tempLoopVar_objTabPermission;
                    objTabPermission.TabID = intTabId;
                    if (objTabPermission.AllowAccess)
                    {
                        objTabPermissionController.AddTabPermission(objTabPermission);
                    }
                }
            }
            if (!Null.IsNull(objTab.PortalID))
            {
                UpdatePortalTabOrder(objTab.PortalID, intTabId, objTab.ParentId, 0, 0, objTab.IsVisible, true);
            }
            else // host tab
            {
                ArrayList arrTabs = GetTabsByParentId(objTab.ParentId);
                UpdateTabOrder(objTab.PortalID, intTabId, (arrTabs.Count * 2) - 1, 1, objTab.ParentId);
            }

            if (AddAllTabsModules)
            {
                ModuleController objmodules = new ModuleController();
                ArrayList arrMods = objmodules.GetAllTabsModules(objTab.PortalID, true);

                foreach (ModuleInfo objModule in arrMods)
                {
                    objmodules.CopyModule(objModule.ModuleID, objModule.TabID, intTabId, "", true);
                }
            }

            return intTabId;
        }

        private TabInfo FillTabInfo(IDataReader dr)
        {
            return FillTabInfo(dr, true);
        }

        private TabInfo FillTabInfo(IDataReader dr, bool CheckForOpenDataReader)
        {
            TabInfo objTabInfo = new TabInfo();
            TabPermissionController objTabPermissionController = new TabPermissionController();
            // read datareader
            bool @Continue = true;

            if (CheckForOpenDataReader)
            {
                @Continue = false;
                if (dr.Read())
                {
                    @Continue = true;
                }
            }
            if (@Continue)
            {
                try
                {
                    objTabInfo.TabID = Convert.ToInt32(Null.SetNull(dr["TabID"], objTabInfo.TabID));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.TabOrder = Convert.ToInt32(Null.SetNull(dr["TabOrder"], objTabInfo.TabOrder));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.PortalID = Convert.ToInt32(Null.SetNull(dr["PortalID"], objTabInfo.PortalID));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.TabName = Convert.ToString(Null.SetNull(dr["TabName"], objTabInfo.TabName));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.IsVisible = Convert.ToBoolean(Null.SetNull(dr["IsVisible"], objTabInfo.IsVisible));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.ParentId = Convert.ToInt32(Null.SetNull(dr["ParentId"], objTabInfo.ParentId));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.Level = Convert.ToInt32(Null.SetNull(dr["Level"], objTabInfo.Level));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.IconFile = Convert.ToString(Null.SetNull(dr["IconFile"], objTabInfo.IconFile));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.DisableLink = Convert.ToBoolean(Null.SetNull(dr["DisableLink"], objTabInfo.DisableLink));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.Title = Convert.ToString(Null.SetNull(dr["Title"], objTabInfo.Title));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.Description = Convert.ToString(Null.SetNull(dr["Description"], objTabInfo.Description));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.KeyWords = Convert.ToString(Null.SetNull(dr["KeyWords"], objTabInfo.KeyWords));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.IsDeleted = Convert.ToBoolean(Null.SetNull(dr["IsDeleted"], objTabInfo.IsDeleted));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.Url = Convert.ToString(Null.SetNull(dr["Url"], objTabInfo.Url));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.SkinSrc = Convert.ToString(Null.SetNull(dr["SkinSrc"], objTabInfo.SkinSrc));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.ContainerSrc = Convert.ToString(Null.SetNull(dr["ContainerSrc"], objTabInfo.ContainerSrc));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.TabPath = Convert.ToString(Null.SetNull(dr["TabPath"], objTabInfo.TabPath));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.StartDate = Convert.ToDateTime(Null.SetNull(dr["StartDate"], objTabInfo.StartDate));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.EndDate = Convert.ToDateTime(Null.SetNull(dr["EndDate"], objTabInfo.EndDate));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.HasChildren = Convert.ToBoolean(Null.SetNull(dr["HasChildren"], objTabInfo.HasChildren));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.RefreshInterval = Convert.ToInt32(Null.SetNull(dr["RefreshInterval"], objTabInfo.RefreshInterval));
                }
                catch
                {
                }
                try
                {
                    objTabInfo.PageHeadText = Convert.ToString(Null.SetNull(dr["PageHeadText"], objTabInfo.PageHeadText));
                }
                catch
                {
                }

                if (objTabInfo != null)
                {
                    ArrayList arrTabPermissions = objTabPermissionController.GetTabPermissionsByPortal(objTabInfo.PortalID);
                    try
                    {
                        objTabInfo.AdministratorRoles = objTabPermissionController.GetTabPermissionsByTabID(arrTabPermissions, objTabInfo.TabID, "EDIT");
                        if (objTabInfo.AdministratorRoles == ";")
                        {
                            // this code is here for legacy support - the AdministratorRoles were stored as a concatenated list of roleids prior to DNN 3.0
                            try
                            {
                                objTabInfo.AdministratorRoles = Convert.ToString(Null.SetNull(dr["AdministratorRoles"], objTabInfo.AdministratorRoles));
                            }
                            catch
                            {
                                // the AdministratorRoles field was removed from the Tabs table in 3.0
                            }
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        objTabInfo.AuthorizedRoles = objTabPermissionController.GetTabPermissionsByTabID(arrTabPermissions, objTabInfo.TabID, "VIEW");
                        if (objTabInfo.AuthorizedRoles == ";")
                        {
                            // this code is here for legacy support - the AuthorizedRoles were stored as a concatenated list of roleids prior to DNN 3.0
                            try
                            {
                                objTabInfo.AuthorizedRoles = Convert.ToString(Null.SetNull(dr["AuthorizedRoles"], objTabInfo.AuthorizedRoles));
                            }
                            catch
                            {
                                // the AuthorizedRoles field was removed from the Tabs table in 3.0
                            }
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        objTabInfo.TabPermissions = objTabPermissionController.GetTabPermissionsCollectionByTabID(arrTabPermissions, objTabInfo.TabID);
                    }
                    catch
                    {
                    }

                    objTabInfo.BreadCrumbs = null;
                    objTabInfo.Panes = null;
                    objTabInfo.Modules = null;
                }
            }
            else
            {
                objTabInfo = null;
            }

            return objTabInfo;
        }

        private ArrayList FillTabInfoCollection(IDataReader dr)
        {
            ArrayList arr = new ArrayList();
            try
            {
                TabInfo obj;
                while (dr.Read())
                {
                    // fill business object
                    obj = FillTabInfo(dr, false);
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

        public ArrayList GetAllTabs()
        {
            return FillTabInfoCollection(DataProvider.Instance().GetAllTabs());
        }

        public TabInfo GetTab(int TabId)
        {
            IDataReader dr = DataProvider.Instance().GetTab(TabId);
            try
            {
                return FillTabInfo(dr);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }
        }

        public TabInfo GetTabByName(string TabName, int PortalId)
        {
            return GetTabByName(TabName, PortalId, int.MinValue);
        }

        public TabInfo GetTabByName(string TabName, int PortalId, int ParentId)
        {
            ArrayList arrTabs = FillTabInfoCollection(DataProvider.Instance().GetTabByName(TabName, PortalId));

            int intTab = -1;

            if (arrTabs != null)
            {
                switch (arrTabs.Count)
                {
                    case 0: // no results

                        break;
                    case 1: // exact match

                        intTab = 0;
                        break;
                    default: // multiple matches

                        int intIndex;
                        TabInfo objTab;
                        for (intIndex = 0; intIndex <= arrTabs.Count - 1; intIndex++)
                        {
                            objTab = (TabInfo)arrTabs[intIndex];
                            // check if the parentids match
                            if (objTab.ParentId == ParentId)
                            {
                                intTab = intIndex;
                            }
                        }
                        if (intTab == -1)
                        {
                            // no match - return the first item
                            intTab = 0;
                        }
                        break;
                }
            }

            if (intTab != -1)
            {
                return ((TabInfo)arrTabs[intTab]);
            }
            else
            {
                return null;
            }
        }

        public ArrayList GetTabs(int PortalId)
        {
            return FillTabInfoCollection(DataProvider.Instance().GetTabs(PortalId));
        }

        public ArrayList GetTabsByParentId(int ParentId)
        {
            return FillTabInfoCollection(DataProvider.Instance().GetTabsByParentId(ParentId));
        }

        public void CopyTab(int PortalId, int FromTabId, int ToTabId, bool IncludeContent)
        {
            ArrayList arrModules;
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule;
            int intIndex;

            arrModules = objModules.GetPortalTabModules(PortalId, FromTabId);
            for (intIndex = 0; intIndex <= arrModules.Count - 1; intIndex++)
            {
                objModule = (ModuleInfo)arrModules[intIndex];

                // if the module shows on all pages does not need to be copied since it will
                // be already added to this page
                if (!objModule.AllTabs)
                {
                    if (IncludeContent == false)
                    {
                        objModule.ModuleID = Null.NullInteger;
                    }

                    objModule.TabID = ToTabId;
                    objModules.AddModule(objModule);
                }
            }
        }

        /// <summary>
        /// Deletes a tab premanently from the database
        /// </summary>
        /// <param name="TabId">TabId of the tab to be deleted</param>
        /// <param name="PortalId">PortalId of the portal</param>
        /// <remarks>
        /// </remarks>
        public void DeleteTab(int TabId, int PortalId)
        {
            // parent tabs can not be deleted
            ArrayList arrTabs = GetTabsByParentId(TabId);

            if (arrTabs.Count == 0)
            {
                DataProvider.Instance().DeleteTab(TabId);
                UpdatePortalTabOrder(PortalId, TabId, -2, 0, 0, true, false);
            }
        }

        private void MoveTab(ArrayList objDesktopTabs, int intFromIndex, int intToIndex, int intNewLevel, bool blnAddChild)
        {
            int intCounter;
            TabInfo objTab;
            bool blnInsert = false;
            int intIncrement;

            int intOldLevel = ((TabInfo)objDesktopTabs[intFromIndex]).Level;
            if (intToIndex != objDesktopTabs.Count - 1)
            {
                blnInsert = true;
            }

            // clone tab and children from old parent
            ArrayList objClone = new ArrayList();
            intCounter = intFromIndex;
            while (intCounter <= objDesktopTabs.Count - 1)
            {
                if (((TabInfo)objDesktopTabs[intCounter]).TabID == ((TabInfo)objDesktopTabs[intFromIndex]).TabID || ((TabInfo)objDesktopTabs[intCounter]).Level > intOldLevel)
                {
                    objClone.Add(objDesktopTabs[intCounter]);
                    intCounter++;
                }
                else
                {
                    break;
                }
            }

            // remove tab and children from old parent
            objDesktopTabs.RemoveRange(intFromIndex, objClone.Count);
            if (intToIndex > intFromIndex)
            {
                intToIndex -= objClone.Count;
            }

            // add clone to new parent
            if (blnInsert)
            {
                objClone.Reverse();
            }

            foreach (TabInfo tempLoopVar_objTab in objClone)
            {
                objTab = tempLoopVar_objTab;
                objTab.TabPath = Globals.GenerateTabPath(objTab.ParentId, objTab.TabName);
                if (blnInsert)
                {
                    objTab.Level += intNewLevel - intOldLevel;
                    if (blnAddChild)
                    {
                        intIncrement = 1;
                    }
                    else
                    {
                        intIncrement = 0;
                    }
                    objDesktopTabs.Insert(intToIndex + intIncrement, objTab);
                }
                else
                {
                    objTab.Level += intNewLevel - intOldLevel;
                    objDesktopTabs.Add(objTab);
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
        private void UpdateChildTabPath(int intTabid)
        {
            TabController objtabs = new TabController();
            TabInfo objtab;
            ArrayList arrTabs = objtabs.GetTabsByParentId(intTabid);

            foreach (TabInfo tempLoopVar_objtab in arrTabs)
            {
                objtab = tempLoopVar_objtab;
                string oldTabPath = objtab.TabPath;
                objtab.TabPath = Globals.GenerateTabPath(objtab.ParentId, objtab.TabName);
                if (oldTabPath != objtab.TabPath)
                {
                    DataProvider.Instance().UpdateTab(objtab.TabID, objtab.TabName, objtab.IsVisible, objtab.DisableLink, objtab.ParentId, objtab.IconFile, objtab.Title, objtab.Description, objtab.KeyWords, objtab.IsDeleted, objtab.Url, objtab.SkinSrc, objtab.ContainerSrc, objtab.TabPath, objtab.StartDate, objtab.EndDate, objtab.RefreshInterval, objtab.PageHeadText);
                    UpdateChildTabPath(objtab.TabID);
                }
            }
        }

        public void UpdatePortalTabOrder(int PortalId, int TabId, int NewParentId, int Level, int Order, bool IsVisible, bool NewTab)
        {
            TabInfo objTab;

            int intCounter = 0;
            int intFromIndex = -1;
            int intOldParentId = -2;
            int intToIndex = -1;
            int intNewParentIndex = 0;
            int intLevel;
            int intAddTabLevel = 0;

            PortalController objPortals = new PortalController();
            PortalInfo objPortal = objPortals.GetPortal(PortalId);

            //hashtable to prevent db calls when no change
            Hashtable htabs = new Hashtable();

            // create temporary tab collection
            ArrayList objTabs = new ArrayList();

            // populate temporary tab collection
            ArrayList arrTabs = GetTabs(PortalId);
            foreach (TabInfo tempLoopVar_objTab in arrTabs)
            {
                objTab = tempLoopVar_objTab;
                if (NewTab == false || objTab.TabID != TabId)
                {
                    // save old data
                    htabs.Add(objTab.TabID, new TabOrderHelper(objTab.TabOrder, objTab.Level, objTab.ParentId));

                    if (objTab.TabOrder == 0)
                    {
                        objTab.TabOrder = 999;
                    }
                    objTabs.Add(objTab);
                    if (objTab.TabID == TabId)
                    {
                        intOldParentId = objTab.ParentId;
                        intFromIndex = intCounter;
                    }
                    if (objTab.TabID == NewParentId)
                    {
                        intNewParentIndex = intCounter;
                        intAddTabLevel = objTab.Level + 1;
                    }
                    intCounter++;
                }
            }

            if (NewParentId != -2) // not deleted
            {
                // adding new tab
                if (intFromIndex == -1)
                {
                    objTab = new TabInfo();
                    objTab.TabID = TabId;
                    objTab.ParentId = NewParentId;
                    objTab.IsVisible = IsVisible;
                    objTab.Level = intAddTabLevel;
                    objTabs.Add(objTab);
                    intFromIndex = objTabs.Count - 1;
                }

                if (Level == 0 && Order == 0)
                {
                    ((TabInfo)objTabs[intFromIndex]).IsVisible = IsVisible;
                }
            }

            if (NewParentId != -2) // not deleted
            {
                // if the parent changed or we added a new non root level tab
                if (intOldParentId != NewParentId && !(intOldParentId == -2 && NewParentId == -1))
                {
                    // locate position of last child for new parent
                    if (NewParentId != -1)
                    {
                        intLevel = ((TabInfo)objTabs[intNewParentIndex]).Level;
                    }
                    else
                    {
                        intLevel = -1;
                    }

                    intCounter = intNewParentIndex + 1;
                    while (intCounter <= objTabs.Count - 1)
                    {
                        if (((TabInfo)objTabs[intCounter]).Level > intLevel)
                        {
                            intToIndex = intCounter;
                        }
                        else
                        {
                            break;
                        }
                        intCounter++;
                    }
                    // adding to parent with no children
                    if (intToIndex == -1)
                    {
                        intToIndex = intNewParentIndex;
                    }
                    // move tab
                    ((TabInfo)objTabs[intFromIndex]).ParentId = NewParentId;
                    MoveTab(objTabs, intFromIndex, intToIndex, intLevel + 1, true);
                }
                else
                {
                    // if level has changed
                    if (Level != 0)
                    {
                        intLevel = ((TabInfo)objTabs[intFromIndex]).Level;

                        bool blnValid = true;
                        switch (Level)
                        {
                            case -1:

                                if (intLevel == 0)
                                {
                                    blnValid = false;
                                }
                                break;
                            case 1:

                                if (intFromIndex > 0)
                                {
                                    if (intLevel > ((TabInfo)objTabs[intFromIndex - 1]).Level)
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

                        if (blnValid)
                        {
                            int intNewLevel;
                            if (Level == -1)
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
                            while (intCounter >= 0 && NewParentId == -2)
                            {
                                if (((TabInfo)objTabs[intCounter]).Level == intNewLevel)
                                {
                                    if (Level == -1)
                                    {
                                        NewParentId = ((TabInfo)objTabs[intCounter]).ParentId;
                                    }
                                    else
                                    {
                                        NewParentId = ((TabInfo)objTabs[intCounter]).TabID;
                                    }
                                    intNewParentIndex = intCounter;
                                }
                                intCounter--;
                            }
                            ((TabInfo)objTabs[intFromIndex]).ParentId = NewParentId;

                            if (Level == -1)
                            {
                                // locate position of next child for parent
                                intToIndex = -1;
                                intCounter = intNewParentIndex + 1;
                                while (intCounter <= objTabs.Count - 1)
                                {
                                    if (((TabInfo)objTabs[intCounter]).Level > intNewLevel)
                                    {
                                        intToIndex = intCounter;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    intCounter++;
                                }
                                // adding to parent with no children
                                if (intToIndex == -1)
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
                            if (intFromIndex == intToIndex)
                            {
                                ((TabInfo)objTabs[intFromIndex]).Level = intNewLevel;
                            }
                            else
                            {
                                MoveTab(objTabs, intFromIndex, intToIndex, intNewLevel, true);
                            }
                        }
                    }
                    else
                    {
                        // if order has changed
                        if (Order != 0)
                        {
                            intLevel = ((TabInfo)objTabs[intFromIndex]).Level;

                            // find previous/next item for parent
                            intToIndex = -1;
                            intCounter = intFromIndex + Order;
                            while (intCounter >= 0 && intCounter <= objTabs.Count - 1 && intToIndex == -1)
                            {
                                if (((TabInfo)objTabs[intCounter]).ParentId == NewParentId)
                                {
                                    intToIndex = intCounter;
                                }
                                intCounter += Order;
                            }
                            if (intToIndex != -1)
                            {
                                if (Order == 1)
                                {
                                    // locate position of next child for parent
                                    intNewParentIndex = intToIndex;
                                    intToIndex = -1;
                                    intCounter = intNewParentIndex + 1;
                                    while (intCounter <= objTabs.Count - 1)
                                    {
                                        if (((TabInfo)objTabs[intCounter]).Level > intLevel)
                                        {
                                            intToIndex = intCounter;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        intCounter++;
                                    }
                                    // adding to parent with no children
                                    if (intToIndex == -1)
                                    {
                                        intToIndex = intNewParentIndex;
                                    }
                                    intToIndex++;
                                }
                                MoveTab(objTabs, intFromIndex, intToIndex, intLevel, false);
                            }
                        }
                    }
                }
            }

            // update the tabs
            int intTabOrder;
            int intDesktopTabOrder = -1;
            int intAdminTabOrder = 9999; // this seeds the taborder for the admin tab so that they are always at the end of the tab list ( max = 5000 desktop tabs per portal )
            foreach (TabInfo tempLoopVar_objTab in objTabs)
            {
                objTab = tempLoopVar_objTab;
                if (((objTab.TabID == objPortal.AdminTabId) || (objTab.ParentId == objPortal.AdminTabId) || (objTab.TabID == objPortal.SuperTabId) || (objTab.ParentId == objPortal.SuperTabId)) && (objPortal.AdminTabId != -1)) // special case when creating new portals
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
                if (htabs.Contains(objTab.TabID))
                {
                    TabOrderHelper ttab = (TabOrderHelper)htabs[objTab.TabID];
                    if (intTabOrder != ttab.TabOrder || objTab.Level != ttab.Level || objTab.ParentId != ttab.ParentId)
                    {
                        UpdateTabOrder(objTab.PortalID, objTab.TabID, intTabOrder, objTab.Level, objTab.ParentId);
                    }
                }
                else
                {
                    UpdateTabOrder(objTab.PortalID, objTab.TabID, intTabOrder, objTab.Level, objTab.ParentId);
                }
            }

            // clear portal cache
            DataCache.ClearPortalCache(PortalId, true);
        }

        public void UpdateTab(TabInfo objTab)
        {
            bool updateChildren = false;
            TabInfo objTmpTab = GetTab(objTab.TabID);
            if (objTmpTab.TabName != objTab.TabName || objTmpTab.ParentId != objTab.ParentId)
            {
                updateChildren = true;
            }

            UpdatePortalTabOrder(objTab.PortalID, objTab.TabID, objTab.ParentId, 0, 0, objTab.IsVisible, false);

            DataProvider.Instance().UpdateTab(objTab.TabID, objTab.TabName, objTab.IsVisible, objTab.DisableLink, objTab.ParentId, objTab.IconFile, objTab.Title, objTab.Description, objTab.KeyWords, objTab.IsDeleted, objTab.Url, objTab.SkinSrc, objTab.ContainerSrc, objTab.TabPath, objTab.StartDate, objTab.EndDate, objTab.RefreshInterval, objTab.PageHeadText);

            TabController objTabController = new TabController();
            TabPermissionController objTabPermissionController = new TabPermissionController();

            TabPermissionCollection objTabPermissions;
            objTabPermissions = objTab.TabPermissions;

            TabPermissionCollection objCurrentTabPermissions;
            objCurrentTabPermissions = objTabPermissionController.GetTabPermissionsCollectionByTabID(objTab.TabID);

            if (!objCurrentTabPermissions.CompareTo(objTab.TabPermissions))
            {
                objTabPermissionController.DeleteTabPermissionsByTabID(objTab.TabID);

                TabPermissionInfo objTabPermission = new TabPermissionInfo();
                foreach (TabPermissionInfo tempLoopVar_objTabPermission in objTabPermissions)
                {
                    objTabPermission = tempLoopVar_objTabPermission;
                    if (objTabPermission.AllowAccess)
                    {
                        objTabPermissionController.AddTabPermission(objTabPermission);
                    }
                }
            }
            if (updateChildren)
            {
                UpdateChildTabPath(objTab.TabID);
            }
        }

        public void UpdateTabOrder(int PortalID, int TabId, int TabOrder, int Level, int ParentId)
        {
            DataProvider.Instance().UpdateTabOrder(TabId, TabOrder, Level, ParentId);
        }
    }
}