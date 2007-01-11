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
using System.Text;
using DotNetNuke.Security.Roles;

namespace DotNetNuke.Security.Permissions.Controls
{
    public class ModulePermissionsGrid : PermissionsGrid
    {
        private bool _InheritViewPermissionsFromTab = false;
        private int _ModuleID = -1;
        private int _ViewColumnIndex;
        private ModulePermissionCollection ModulePermissions;

        /// <summary>
        /// Gets and Sets whether the Module inherits the Page's(Tab's) permissions
        /// </summary>
        public bool InheritViewPermissionsFromTab
        {
            get
            {
                return _InheritViewPermissionsFromTab;
            }
            set
            {
                _InheritViewPermissionsFromTab = value;
            }
        }

        /// <summary>
        /// Gets and Sets the Id of the Module
        /// </summary>
        public int ModuleID
        {
            get
            {
                return _ModuleID;
            }
            set
            {
                _ModuleID = value;
                if (!Page.IsPostBack)
                {
                    GetModulePermissions();
                }
            }
        }

        /// <summary>
        /// Gets the ModulePermission Collection
        /// </summary>
        public ModulePermissionCollection Permissions
        {
            get
            {
                //First Update Permissions in case they have been changed
                UpdatePermissions();

                //Return the ModulePermissions
                return ModulePermissions;
            }
        }

        /// <summary>
        /// Gets the Enabled status of the permission
        /// </summary>
        /// <param name="objPerm">The permission being loaded</param>
        /// <param name="role">The role</param>
        /// <param name="column">The column of the Grid</param>
        protected override bool GetEnabled(PermissionInfo objPerm, RoleInfo role, int column)
        {
            bool enabled;

            if (InheritViewPermissionsFromTab && column == _ViewColumnIndex)
            {
                enabled = false;
            }
            else
            {
                if (role.RoleID == AdministratorRoleId)
                {
                    enabled = false;
                }
                else
                {
                    enabled = true;
                }
            }

            return enabled;
        }

        /// <summary>
        /// Gets the Value of the permission
        /// </summary>
        /// <param name="objPerm">The permission being loaded</param>
        /// <param name="role">The role</param>
        /// <param name="column">The column of the Grid</param>
        /// <returns>A Boolean (True or False)</returns>
        protected override bool GetPermission(PermissionInfo objPerm, RoleInfo role, int column)
        {
            bool permission;

            if (InheritViewPermissionsFromTab && column == _ViewColumnIndex)
            {
                permission = false;
            }
            else
            {
                if (role.RoleID == AdministratorRoleId)
                {
                    permission = true;
                }
                else
                {
                    ModulePermissionInfo objModulePermission = ModuleHasPermission(objPerm.PermissionID, role.RoleID);
                    if (objModulePermission != null)
                    {
                        permission = objModulePermission.AllowAccess;
                    }
                    else
                    {
                        permission = false;
                    }
                }
            }

            return permission;
        }

        /// <summary>
        /// Gets the Permissions from the Data Store
        /// </summary>
        protected override ArrayList GetPermissions()
        {
            PermissionController objPermissionController = new PermissionController();
            ArrayList arrPermissions = objPermissionController.GetPermissionsByModuleID(this.ModuleID);

            int i;
            for (i = 0; i <= arrPermissions.Count - 1; i++)
            {
                PermissionInfo objPermission;
                objPermission = (PermissionInfo)arrPermissions[i];
                if (objPermission.PermissionKey == "VIEW")
                {
                    _ViewColumnIndex = i + 1;
                }
            }

            return arrPermissions;
        }

        /// <summary>
        /// Check if the Role has the permission specified
        /// </summary>
        /// <param name="permissionID">The Id of the Permission to check</param>
        /// <param name="roleId">The role id to check</param>
        private ModulePermissionInfo ModuleHasPermission(int permissionID, int roleId)
        {
            int i;
            for (i = 0; i <= ModulePermissions.Count - 1; i++)
            {
                ModulePermissionInfo objModulePermission = ModulePermissions[i];
                if (objModulePermission.RoleID == roleId && permissionID == objModulePermission.PermissionID)
                {
                    return objModulePermission;
                }
            }
            return null;
        }

        /// <summary>
        /// Saves the ViewState
        /// </summary>
        protected override object SaveViewState()
        {
            object[] allStates = new object[4];

            // Save the Base Controls ViewState
            allStates[0] = base.SaveViewState();

            //Save the ModuleID
            allStates[1] = ModuleID;

            //Save the InheritViewPermissionsFromTab
            allStates[2] = InheritViewPermissionsFromTab;

            //Persist the ModulePermissions
            StringBuilder sb = new StringBuilder();
            bool addDelimiter = false;
            foreach (ModulePermissionInfo objModulePermission in ModulePermissions)
            {
                if (addDelimiter)
                {
                    sb.Append("##");
                }
                else
                {
                    addDelimiter = true;
                }
                sb.Append(BuildKey(objModulePermission.AllowAccess, objModulePermission.PermissionID, objModulePermission.ModulePermissionID, objModulePermission.RoleID, objModulePermission.RoleName));
            }
            allStates[3] = sb.ToString();

            return allStates;
        }

        /// <summary>
        /// Overrides the Base method to Generate the Data Grid
        /// </summary>
        public override void GenerateDataGrid()
        {
        }

        /// <summary>
        /// Gets the ModulePermissions from the Data Store
        /// </summary>
        private void GetModulePermissions()
        {
            ModulePermissionController objModulePermissionController = new ModulePermissionController();
            ModulePermissions = objModulePermissionController.GetModulePermissionsCollectionByModuleID(this.ModuleID);
        }

        /// <summary>
        /// Load the ViewState
        /// </summary>
        /// <param name="savedState">The saved state</param>
        protected override void LoadViewState(object savedState)
        {
            if (!(savedState == null))
            {
                // Load State from the array of objects that was saved with SaveViewState.

                object[] myState = (object[])savedState;

                //Load Base Controls ViewStte
                if (!(myState[0] == null))
                {
                    base.LoadViewState(myState[0]);
                }

                //Load ModuleID
                if (!(myState[1] == null))
                {
                    ModuleID = Convert.ToInt32(myState[1]);
                }

                //Load InheritViewPermissionsFromTab
                if (!(myState[2] == null))
                {
                    InheritViewPermissionsFromTab = Convert.ToBoolean(myState[2]);
                }

                //Load ModulePermissions
                if (!(myState[3] == null))
                {
                    ArrayList arrPermissions = new ArrayList();
                    string state = myState[3].ToString();
                    if (!String.IsNullOrEmpty(state))
                    {
                        //First Break the String into individual Keys
                        string[] permissionKeys = state.Split("##".ToCharArray()[0]);
                        foreach (string key in permissionKeys)
                        {
                            string[] Settings = key.Split('|');
                            ParsePermissionKeys(Settings, arrPermissions);
                        }
                    }
                    ModulePermissions = new ModulePermissionCollection(arrPermissions);
                }
            }
        }

        /// <summary>
        /// Parse the Permission Keys used to persist the Permissions in the ViewState
        /// </summary>
        /// <param name="Settings">A string array of settings</param>
        /// <param name="arrPermisions">An Arraylist to add the Permission object to</param>
        private void ParsePermissionKeys(string[] Settings, ArrayList arrPermisions)
        {
            RoleController objRoleController = new RoleController();
            ModulePermissionController objModulePermissionController = new ModulePermissionController();
            ModulePermissionInfo objModulePermission;

            objModulePermission = new ModulePermissionInfo();
            objModulePermission.PermissionID = Convert.ToInt32(Settings[1]);
            objModulePermission.RoleID = Convert.ToInt32(Settings[4]);

            if (Settings[2] == "")
            {
                objModulePermission.ModulePermissionID = -1;
            }
            else
            {
                objModulePermission.ModulePermissionID = Convert.ToInt32(Settings[2]);
            }
            objModulePermission.RoleName = Settings[3];
            objModulePermission.AllowAccess = true;

            objModulePermission.ModuleID = ModuleID;
            arrPermisions.Add(objModulePermission);
        }

        /// <summary>
        /// Updates a Permission
        /// </summary>
        /// <param name="permission">The permission being updated</param>
        /// <param name="roleName">The name of the role</param>
        /// <param name="roleId">The id of the role</param>
        /// <param name="allowAccess">The value of the permission</param>
        protected override void UpdatePermission(PermissionInfo permission, int roleid, string roleName, bool allowAccess)
        {
            bool isMatch = false;
            ModulePermissionInfo objPermission;
            int permissionId = permission.PermissionID;

            //Search ModulePermission Collection for the permission to Update
            foreach (ModulePermissionInfo tempLoopVar_objPermission in ModulePermissions)
            {
                objPermission = tempLoopVar_objPermission;
                if (objPermission.PermissionID == permissionId && objPermission.RoleID == roleid)
                {
                    //ModulePermission is in collection
                    if (!allowAccess)
                    {
                        //Remove from collection as we only keep AllowAccess permissions
                        ModulePermissions.Remove(objPermission);
                    }
                    isMatch = true;
                    break;
                }
            }
        

            //ModulePermission not found so add new
            if (!isMatch && allowAccess)
            {
                objPermission = new ModulePermissionInfo();
                objPermission.PermissionID = permissionId;
                objPermission.RoleName = roleName;
                objPermission.RoleID = roleid;
                objPermission.AllowAccess = allowAccess;
                objPermission.ModuleID = ModuleID;
                ModulePermissions.Add(objPermission);
            }
        }
    }
}