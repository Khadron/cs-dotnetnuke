#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
    public class TabPermissionsGrid : PermissionsGrid
    {
        private int _TabID = -1;
        private TabPermissionCollection TabPermissions;

        /// <summary>
        /// Gets the Permissions Collection
        /// </summary>
        public TabPermissionCollection Permissions
        {
            get
            {
                //First Update Permissions in case they have been changed
                UpdatePermissions();

                //Return the TabPermissions
                return TabPermissions;
            }
        }

        /// <summary>
        /// Gets and Sets the Id of the Tab
        /// </summary>
        public int TabID
        {
            get
            {
                return _TabID;
            }
            set
            {
                _TabID = value;
                if( !Page.IsPostBack )
                {
                    GetTabPermissions();
                }
            }
        }

        /// <summary>
        /// Gets the Enabled status of the permission
        /// </summary>
        /// <param name="objPerm">The permission being loaded</param>
        /// <param name="role">The role</param>
        /// <param name="column">The column of the Grid</param>
        protected override bool GetEnabled( PermissionInfo objPerm, RoleInfo role, int column )
        {
            bool enabled;

            if( role.RoleID == AdministratorRoleId )
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
        protected override bool GetPermission( PermissionInfo objPerm, RoleInfo role, int column )
        {
            bool permission;

            if( role.RoleID == AdministratorRoleId )
            {
                permission = true;
            }
            else
            {
                TabPermissionInfo objTabPermission = TabHasPermission( objPerm.PermissionID, role.RoleID );
                if( objTabPermission != null )
                {
                    permission = objTabPermission.AllowAccess;
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
            return objPermissionController.GetPermissionsByTabID( this.TabID );
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
            allStates[1] = TabID;

            //Persist the TabPermisisons
            StringBuilder sb = new StringBuilder();
            bool addDelimiter = false;
            foreach( TabPermissionInfo objTabPermission in TabPermissions )
            {
                if( addDelimiter )
                {
                    sb.Append( "##" );
                }
                else
                {
                    addDelimiter = true;
                }
                sb.Append( BuildKey( objTabPermission.AllowAccess, objTabPermission.PermissionID, objTabPermission.TabPermissionID, objTabPermission.RoleID, objTabPermission.RoleName ) );
            }
            allStates[2] = sb.ToString();

            return allStates;
        }

        /// <summary>
        /// Check if the Role has the permission specified
        /// </summary>
        /// <param name="permissionID">The Id of the Permission to check</param>
        /// <param name="roleid">The role id to check</param>
        private TabPermissionInfo TabHasPermission( int permissionID, int roleid )
        {
            int i;
            for( i = 0; i <= TabPermissions.Count - 1; i++ )
            {
                TabPermissionInfo objTabPermission = TabPermissions[i];
                if( objTabPermission.RoleID == roleid && permissionID == objTabPermission.PermissionID )
                {
                    return objTabPermission;
                }
            }
            return null;
        }

        public override void GenerateDataGrid()
        {
        }

        /// <summary>
        /// Gets the TabPermissions from the Data Store
        /// </summary>
        private void GetTabPermissions()
        {
            TabPermissionController objTabPermissionController = new TabPermissionController();
            TabPermissions = objTabPermissionController.GetTabPermissionsCollectionByTabID( this.TabID );
        }

        /// <summary>
        /// Load the ViewState
        /// </summary>
        /// <param name="savedState">The saved state</param>
        protected override void LoadViewState( object savedState )
        {
            if( !( savedState == null ) )
            {
                // Load State from the array of objects that was saved with SaveViewState.

                object[] myState = (object[])savedState;

                //Load Base Controls ViewStte
                if( !( myState[0] == null ) )
                {
                    base.LoadViewState( myState[0] );
                }

                //Load TabId
                if( !( myState[1] == null ) )
                {
                    TabID = Convert.ToInt32( myState[1] );
                }

                //Load TabPermissions
                if( !( myState[2] == null ) )
                {
                    ArrayList arrPermissions = new ArrayList();
                    string state = myState[2].ToString();
                    if( state != "" )
                    {
                        //First Break the String into individual Keys
                        string[] permissionKeys = state.Split( "##".ToCharArray()[0] );
                        foreach( string key in permissionKeys )
                        {
                            string[] Settings = key.Split( '|' );
                            ParsePermissionKeys( Settings, arrPermissions );
                        }
                    }
                    TabPermissions = new TabPermissionCollection( arrPermissions );
                }
            }
        }

        /// <summary>
        /// Parse the Permission Keys used to persist the Permissions in the ViewState
        /// </summary>
        /// <param name="Settings">A string array of settings</param>
        /// <param name="arrPermisions">An Arraylist to add the Permission object to</param>
        private void ParsePermissionKeys( string[] Settings, ArrayList arrPermisions )
        {
            TabPermissionInfo objTabPermission;

            objTabPermission = new TabPermissionInfo();
            objTabPermission.PermissionID = Convert.ToInt32( Settings[1] );
            objTabPermission.RoleID = Convert.ToInt32( Settings[4] );
            objTabPermission.RoleName = Settings[3];
            objTabPermission.AllowAccess = true;

            if( Settings[2] == "" )
            {
                objTabPermission.TabPermissionID = -1;
            }
            else
            {
                objTabPermission.TabPermissionID = Convert.ToInt32( Settings[2] );
            }
            objTabPermission.TabID = TabID;

            //Add TabPermission to array
            arrPermisions.Add( objTabPermission );
        }

        /// <summary>
        /// Updates a Permission
        /// </summary>
        /// <param name="permission">The permission being updated</param>
        /// <param name="roleid">The id of the role</param>
        /// <param name="roleName">The name of the role</param>
        /// <param name="allowAccess">The value of the permission</param>
        protected override void UpdatePermission( PermissionInfo permission, int roleid, string roleName, bool allowAccess )
        {
            bool isMatch = false;
            TabPermissionInfo objPermission;
            int permissionId = permission.PermissionID;

            //Search TabPermission Collection for the permission to Update
            foreach( TabPermissionInfo tempLoopVar_objPermission in TabPermissions )
            {
                objPermission = tempLoopVar_objPermission;
                if( objPermission.PermissionID == permissionId && objPermission.RoleID == roleid )
                {
                    //TabPermission is in collection
                    if( !allowAccess )
                    {
                        //Remove from collection as we only keep AllowAccess permissions
                        TabPermissions.Remove( objPermission );
                    }
                    isMatch = true;
                    break;
                }
            }
            

            //TabPermission not found so add new
            if( !isMatch && allowAccess )
            {
                objPermission = new TabPermissionInfo();
                objPermission.PermissionID = permissionId;
                objPermission.RoleName = roleName;
                objPermission.RoleID = roleid;
                objPermission.AllowAccess = allowAccess;
                objPermission.TabID = TabID;
                TabPermissions.Add( objPermission );
            }
        }
    }
}