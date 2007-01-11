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
using System.Web;
using System.Web.UI;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;

namespace DotNetNuke.UI.ControlPanels
{
    /// <Summary>
    /// The ControlPanel class defines a custom base class inherited by all
    /// ControlPanel controls.
    /// </Summary>
    public class ControlPanelBase : UserControl
    {
        protected enum ViewPermissionType
        {
            View = 0,
            Edit = 1
        }

        private string m_localResourceFile;

        protected bool ShowContent
        {
            get
            {
                if (!(Request.Cookies["_Tab_Admin_Content" + PortalSettings.PortalId] == null))
                {
                    HttpCookie objContent = Request.Cookies["_Tab_Admin_Content" + PortalSettings.PortalId];
                    if (objContent.Value == "True")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        protected bool IsPreview
        {
            get
            {
                if (!(Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId] == null))
                {
                    HttpCookie objPreview = Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId];
                    if (objPreview.Value == "True")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        private ModulePermissionInfo AddModulePermission(int moduleId, PermissionInfo permission, int roleId)
        {
            RoleController objRoles = new RoleController();
            ModulePermissionInfo objModulePermission = new ModulePermissionInfo();
            objModulePermission.ModuleID = moduleId;
            objModulePermission.PermissionID = permission.PermissionID;
            objModulePermission.RoleID = roleId;
            objModulePermission.PermissionKey = permission.PermissionKey;
            objModulePermission.AllowAccess = false;

            // allow access to the permission if the role is in the list of administrator roles for the page
            RoleInfo objRole = objRoles.GetRole(objModulePermission.RoleID, PortalSettings.PortalId);
            if (objRole != null)
            {
                if (PortalSettings.ActiveTab.AdministratorRoles.IndexOf(objRole.RoleName) != -1)
                {
                    objModulePermission.AllowAccess = true;
                }
            }

            return objModulePermission;

        }

        protected void AddExistingModule(int moduleId, int tabId, string paneName, int position, string align)
        {

            ModuleController objModules = new ModuleController();
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();

            int UserId = -1;
            if (Request.IsAuthenticated)
            {
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                UserId = objUserInfo.UserID;
            }

            ModuleInfo objModule = objModules.GetModule(moduleId, tabId);
            if (objModule != null)
            {
                objModule.TabID = PortalSettings.ActiveTab.TabID;
                objModule.ModuleOrder = position;
                objModule.PaneName = paneName;
                objModule.Alignment = align;
                objModules.AddModule(objModule);
                objEventLog.AddLog(objModule, PortalSettings, UserId, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_CREATED);
            }


        }

        protected void AddNewModule(string title, int desktopModuleId, string paneName, int position, ViewPermissionType permissionType, string align)
        {

            TabPermissionCollection objTabPermissions = PortalSettings.ActiveTab.TabPermissions;
            PermissionController objPermissionController = new PermissionController();
            ModuleController objModules = new ModuleController();
            ModuleDefinitionController objModuleDefinitions = new ModuleDefinitionController();
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            int intIndex;
            int j;

            try
            {
                DesktopModuleController objDesktopModules = new DesktopModuleController();
                ArrayList arrDM = objDesktopModules.GetDesktopModulesByPortal(PortalSettings.PortalId);
                bool isSelectable = false;
                for (int intloop = 0; intloop < arrDM.Count; intloop++)
                {
                    if (((DesktopModuleInfo)(arrDM[intloop])).DesktopModuleID == desktopModuleId)
                    {
                        isSelectable = true;
                        break;
                    }
                }
                if (isSelectable == false)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            int UserId = -1;
            if (Request.IsAuthenticated)
            {
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                UserId = objUserInfo.UserID;
            }

            ArrayList arrModuleDefinitions = objModuleDefinitions.GetModuleDefinitions(desktopModuleId);
            for (intIndex = 0; intIndex < arrModuleDefinitions.Count; intIndex++)
            {
                ModuleDefinitionInfo objModuleDefinition = (ModuleDefinitionInfo)(arrModuleDefinitions[intIndex]);

                if (title == "")
                {
                    title = objModuleDefinition.FriendlyName;
                }

                ModuleInfo objModule = new ModuleInfo();
                objModule.Initialize(PortalSettings.PortalId);

                objModule.PortalID = PortalSettings.PortalId;
                objModule.TabID = PortalSettings.ActiveTab.TabID;
                objModule.ModuleOrder = position;
                objModule.ModuleTitle = title;
                objModule.PaneName = paneName;
                objModule.ModuleDefID = objModuleDefinition.ModuleDefID;
                objModule.CacheTime = objModuleDefinition.DefaultCacheTime;

                // initialize module permissions
                ModulePermissionCollection objModulePermissions = new ModulePermissionCollection();
                objModule.ModulePermissions = objModulePermissions;
                objModule.InheritViewPermissions = false;

                // get the default module view permissions
                ArrayList arrSystemModuleViewPermissions = objPermissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "VIEW");

                // get the permissions from the page
                foreach (TabPermissionInfo objTabPermission in objTabPermissions)
                {
                    // get the system module permissions for the permissionkey
                    ArrayList arrSystemModulePermissions = objPermissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", objTabPermission.PermissionKey);
                    // loop through the system module permissions
                    for (j = 0; j < arrSystemModulePermissions.Count; j++)
                    {
                        // create the module permission
                        PermissionInfo objSystemModulePermission = (PermissionInfo)(arrSystemModulePermissions[j]);
                        ModulePermissionInfo objModulePermission = AddModulePermission(objModule.ModuleID, objSystemModulePermission, objTabPermission.RoleID);

                        // add the permission to the collection
                        if (!(objModulePermissions.Contains(objModulePermission)) & objModulePermission.AllowAccess)
                        {
                            objModulePermissions.Add(objModulePermission);
                        }

                        // ensure that every EDIT permission which allows access also provides VIEW permission
                        if (objModulePermission.PermissionKey == "EDIT" & objModulePermission.AllowAccess)
                        {
                            ModulePermissionInfo objModuleViewperm = new ModulePermissionInfo();
                            objModuleViewperm.ModuleID = objModulePermission.ModuleID;
                            objModuleViewperm.PermissionID = ((PermissionInfo)(arrSystemModuleViewPermissions[0])).PermissionID;
                            objModuleViewperm.RoleID = objModulePermission.RoleID;
                            objModuleViewperm.PermissionKey = "VIEW";
                            objModuleViewperm.AllowAccess = true;
                            if (!(objModulePermissions.Contains(objModuleViewperm)))
                            {
                                objModulePermissions.Add(objModuleViewperm);
                            }
                        }
                    }

                    //Get the custom Module Permissions,  Assume that roles with Edit Tab Permissions
                    //are automatically assigned to the Custom Module Permissions
                    if (objTabPermission.PermissionKey == "EDIT")
                    {
                        ArrayList arrCustomModulePermissions = objPermissionController.GetPermissionsByModuleDefID(objModule.ModuleDefID);

                        // loop through the custom module permissions
                        for (j = 0; j < arrCustomModulePermissions.Count; j++)
                        {
                            // create the module permission
                            PermissionInfo objCustomModulePermission = (PermissionInfo)(arrCustomModulePermissions[j]);
                            ModulePermissionInfo objModulePermission = AddModulePermission(objModule.ModuleID, objCustomModulePermission, objTabPermission.RoleID);

                            // add the permission to the collection
                            if (!(objModulePermissions.Contains(objModulePermission)) & objModulePermission.AllowAccess)
                            {
                                objModulePermissions.Add(objModulePermission);
                            }
                        }
                    }
                }

                switch (permissionType)
                {
                    case ViewPermissionType.View:
                        objModule.InheritViewPermissions = true;
                        break;
                    case ViewPermissionType.Edit:
                        objModule.ModulePermissions = objModulePermissions;
                        break;
                }

                objModule.AllTabs = false;
                objModule.Visibility = VisibilityState.Maximized;
                objModule.Alignment = align;

                objModules.AddModule(objModule);
                objEventLog.AddLog(objModule, PortalSettings, UserId, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_CREATED);
            }

        }

        protected string BuildURL(int PortalID, string FriendlyName)
        {
            string strURL = "~/" + Globals.glbDefaultPage;

            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModuleByDefinition(PortalID, FriendlyName);
            if (objModule != null)
            {
                strURL = Globals.NavigateURL(objModule.TabID);
            }

            return strURL;
        }

        protected void SetContentMode(bool showContent)
        {
            HttpCookie objContent;

            if (Request.Cookies["_Tab_Admin_Content" + PortalSettings.PortalId] == null)
            {
                objContent = new HttpCookie("_Tab_Admin_Content" + PortalSettings.PortalId);
                objContent.Expires = DateTime.MaxValue; // never expires
                Response.AppendCookie(objContent);
            }

            objContent = Request.Cookies["_Tab_Admin_Content" + PortalSettings.PortalId];
            objContent.Value = showContent.ToString();
            Response.SetCookie(objContent);
        }

        protected void SetPreviewMode(bool isPreview)
        {
            HttpCookie objPreview;

            if (Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId] == null)
            {
                objPreview = new HttpCookie("_Tab_Admin_Preview" + PortalSettings.PortalId);
                objPreview.Value = "False";
                Response.AppendCookie(objPreview);
            }

            objPreview = Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId];
            objPreview.Value = isPreview.ToString();
            Response.SetCookie(objPreview);
        }


        public string LocalResourceFile
        {
            get
            {
                if( String.IsNullOrEmpty( m_localResourceFile ) )
                {
                    return ( this.TemplateSourceDirectory + "/App_LocalResources/" + this.ID );
                }
                else
                {
                    return this.m_localResourceFile;
                }
            }
            set
            {
                this.m_localResourceFile = value;
            }
        }

        public PortalSettings PortalSettings
        {
            get
            {
                return ( (PortalSettings)HttpContext.Current.Items["PortalSettings"] );
            }
        }
    }
}