using System;
using System.Collections;
using System.Text;
using DotNetNuke.Entities.Portals;
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
            int i;
            for (i = 0; i <= FolderPermissions.Count - 1; i++)
            {
                FolderPermissionInfo objFolderPermission = FolderPermissions[i];
                if (objFolderPermission.RoleID == roleid && permissionID == objFolderPermission.PermissionID)
                {
                    return objFolderPermission;
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
            return objPermissionController.GetPermissionsByFolder(PortalId, this.FolderPath)
            ;
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
                    if (state != "")
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
        /// <param name="Settings">A string array of settings</param>
        /// <param name="arrPermisions">An Arraylist to add the Permission object to</param>
        private void ParsePermissionKeys(string[] Settings, ArrayList arrPermisions)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            FolderPermissionInfo objFolderPermission;

            objFolderPermission = new FolderPermissionInfo();
            objFolderPermission.PermissionID = Convert.ToInt32(Settings[1]);
            objFolderPermission.RoleID = Convert.ToInt32(Settings[4]);
            objFolderPermission.RoleName = Settings[3];
            objFolderPermission.AllowAccess = true;

            if (Settings[2] == "")
            {
                objFolderPermission.FolderPermissionID = -1;
            }
            else
            {
                objFolderPermission.FolderPermissionID = Convert.ToInt32(Settings[2]);
            }
            objFolderPermission.FolderPath = FolderPath;

            //Add FolderPermission to array
            arrPermisions.Add(objFolderPermission);
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
            FolderPermissionInfo objPermission;
            int permissionId = permission.PermissionID;

            //Search FolderPermission Collection for the permission to Update
            foreach (FolderPermissionInfo tempLoopVar_objPermission in FolderPermissions)
            {
                objPermission = tempLoopVar_objPermission;
                if (objPermission.PermissionID == permissionId && objPermission.RoleID == roleid)
                {
                    //FolderPermission is in collection
                    if (!allowAccess)
                    {
                        //Remove from collection as we only keep AllowAccess permissions
                        FolderPermissions.Remove(objPermission);
                    }
                    isMatch = true;
                    break;
                }
            }
        

            //FolderPermission not found so add new
            if (!isMatch && allowAccess)
            {
                objPermission = new FolderPermissionInfo();
                objPermission.PermissionID = permissionId;
                objPermission.RoleName = roleName;
                objPermission.RoleID = roleid;
                objPermission.AllowAccess = allowAccess;
                objPermission.FolderPath = FolderPath;
                FolderPermissions.Add(objPermission);
            }
        }
    }
}