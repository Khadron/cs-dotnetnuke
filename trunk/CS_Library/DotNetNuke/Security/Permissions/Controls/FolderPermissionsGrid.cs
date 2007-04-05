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
    public class FolderPermissionsGrid : PermissionsGrid
    {
        private string _FolderPath = "";
        private FolderPermissionCollection FolderPermissions;

        /// <summary>
        /// Gets and Sets the path of the Folder
        /// </summary>
        public string FolderPath
        {
            get
            {
                return _FolderPath;
            }
            set
            {
                _FolderPath = value;
                GetFolderPermissions();
            }
        }

        /// <summary>
        /// Gets the Permission Collection
        /// </summary>
        public FolderPermissionCollection Permissions
        {
            get
            {
                //First Update Permissions in case they have been changed
                UpdatePermissions();

                //Return the FolderPermissions
                return FolderPermissions;
            }
        }

        /// <summary>
        /// Check if the Role has the permission specified
        /// </summary>
        /// <param name="permissionID">The Id of the Permission to check</param>
        /// <param name="roleid">The role id to check</param>
        private FolderPermissionInfo FolderHasPermission(int permissionID, int roleid)
        {            
            for (int i = 0; i < FolderPermissions.Count; i++)
            {
                FolderPermissionInfo folderPermission = FolderPermissions[i];
                if (folderPermission.RoleID == roleid && permissionID == folderPermission.PermissionID)
                {
                    return folderPermission;
                }
            }
            return null;
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

            if (role.RoleID == AdministratorRoleId)
            {
                enabled = false;
            }
            else
            {
                enabled = true;
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

            if (role.RoleID == AdministratorRoleId)
            {
                permission = true;
            }
            else
            {
                FolderPermissionInfo objFolderPermission = FolderHasPermission(objPerm.PermissionID, role.RoleID);
                if (objFolderPermission != null)
                {
                    permission = objFolderPermission.AllowAccess;
                }
                else
                {
                    permission = false;
                }
            }

            return permission;
        }

        /// <summary>
        /// Gets the permissions from the Database
        /// </summary>
        protected override ArrayList GetPermissions()
        {
            PermissionController objPermissionController = new PermissionController();
            return objPermissionController.GetPermissionsByFolder( PortalId, this.FolderPath );
        }

        /// <summary>
        /// Saves the ViewState
        /// </summary>
        protected override object SaveViewState()
        {
            object[] allStates = new object[3];

            // Save the Base Controls ViewState
            allStates[0] = base.SaveViewState();

            //Save the Tab Id
            allStates[1] = FolderPath;

            //Persist the TabPermisisons
            StringBuilder sb = new StringBuilder();
            bool addDelimiter = false;
            if( FolderPermissions != null )
            {
                foreach (FolderPermissionInfo objFolderPermission in FolderPermissions)
                {
                    if (addDelimiter)
                    {
                        sb.Append("##");
                    }
                    else
                    {
                        addDelimiter = true;
                    }
                    sb.Append(BuildKey(objFolderPermission.AllowAccess, objFolderPermission.PermissionID, objFolderPermission.FolderPermissionID, objFolderPermission.RoleID, objFolderPermission.RoleName));
                }
            }
            allStates[2] = sb.ToString();

            return allStates;
        }

        /// <summary>
        /// Overrides the Base method to Generate the Data Grid
        /// </summary>
        public override void GenerateDataGrid()
        {
        }

        /// <summary>
        /// Gets the TabPermissions from the Data Store
        /// </summary>
        private void GetFolderPermissions()
        {
            FolderPermissionController objFolderPermissionController = new FolderPermissionController();
            FolderPermissions = objFolderPermissionController.GetFolderPermissionsCollectionByFolderPath(PortalId, this.FolderPath);
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

                //Load FolderPath
                if (!(myState[1] == null))
                {
                    FolderPath = myState[1].ToString();
                }

                //Load FolderPermissions
                if (!(myState[2] == null))
                {
                    ArrayList arrPermissions = new ArrayList();
                    string state = myState[2].ToString();
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
                    FolderPermissions = new FolderPermissionCollection(arrPermissions);
                }
            }
        }

        /// <summary>
        /// Parse the Permission Keys used to persist the Permissions in the ViewState
        /// </summary>
        /// <param name="settings">A string array of settings</param>
        /// <param name="permisions">An Arraylist to add the Permission object to</param>
        private void ParsePermissionKeys(string[] settings, ArrayList permisions)
        {
            FolderPermissionInfo pi = new FolderPermissionInfo();
            pi.PermissionID = settings.Length > 1 ? Convert.ToInt32(settings[1]) : 0;
            pi.RoleID = settings.Length > 4 ? Convert.ToInt32(settings[4]) : 0;
            pi.RoleName = settings.Length > 3 ? settings[3] : "";
            pi.AllowAccess = true;

            if (settings.Length < 2 || settings[2] == "")
            {
                pi.FolderPermissionID = -1;
            }
            else
            {
                pi.FolderPermissionID = Convert.ToInt32(settings[2]);
            }
            pi.FolderPath = FolderPath;

            //Add FolderPermission to array
            permisions.Add(pi);
        }

        /// <summary>
        /// Updates a Permission
        /// </summary>
        /// <param name="permission">The permission being updated</param>
        /// <param name="roleid">The id of the role</param>
        /// <param name="roleName">The name of the role</param>
        /// <param name="allowAccess">The value of the permission</param>
        protected override void UpdatePermission(PermissionInfo permission, int roleid, string roleName, bool allowAccess)
        {
            bool isMatch = false;
            int permissionId = permission.PermissionID;

            //Search FolderPermission Collection for the permission to Update
            if( FolderPermissions != null )
            {
                foreach (FolderPermissionInfo info in FolderPermissions)
                {
                    if (info.PermissionID == permissionId && info.RoleID == roleid)
                    {
                        //FolderPermission is in collection
                        if (!allowAccess)
                        {
                            //Remove from collection as we only keep AllowAccess permissions
                            FolderPermissions.Remove(info);
                        }
                        isMatch = true;
                        break;
                    }
                }
            }

            //FolderPermission not found so add new
            if (!isMatch && allowAccess)
            {
                FolderPermissionInfo folderPermissionInfo = new FolderPermissionInfo();
                folderPermissionInfo.PermissionID = permissionId;
                folderPermissionInfo.RoleName = roleName;
                folderPermissionInfo.RoleID = roleid;
                folderPermissionInfo.AllowAccess = allowAccess;
                folderPermissionInfo.FolderPath = FolderPath;
                FolderPermissions.Add(folderPermissionInfo);
            }
        }
    }
}