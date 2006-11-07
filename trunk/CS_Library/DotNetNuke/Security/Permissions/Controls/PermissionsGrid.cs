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
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.Security.Permissions.Controls
{
    public abstract class PermissionsGrid : Control, INamingContainer
    {
        private DataTable _permissionsDataTable = new DataTable();
        private string _resourceFile;
        private ArrayList _roles;
        private DropDownList cboRoleGroups;
        private DataGrid dgPermissions;
        private Label lblGroups;
        private ArrayList m_Permissions;
        private Panel pnlPermissions;

        /// <summary>
        /// Gets the Id of the Administrator Role
        /// </summary>
        public int AdministratorRoleId
        {
            get
            {
                return PortalController.GetCurrentPortalSettings().AdministratorRoleId;
            }
        }

        public TableItemStyle AlternatingItemStyle
        {
            get
            {
                return dgPermissions.AlternatingItemStyle;
            }
        }

        public bool AutoGenerateColumns
        {
            get
            {
                return dgPermissions.AutoGenerateColumns;
            }
            set
            {
                dgPermissions.AutoGenerateColumns = value;
            }
        }

        public int CellSpacing
        {
            get
            {
                return dgPermissions.CellSpacing;
            }
            set
            {
                dgPermissions.CellSpacing = value;
            }
        }

        public DataGridColumnCollection Columns
        {
            get
            {
                return dgPermissions.Columns;
            }
        }

        /// <summary>
        /// Gets and Sets whether as Dynamic Column has been added
        /// </summary>
        public bool DynamicColumnAdded
        {
            get
            {
                if( ViewState["ColumnAdded"] == null )
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                ViewState["ColumnAdded"] = value;
            }
        }

        public TableItemStyle FooterStyle
        {
            get
            {
                return dgPermissions.FooterStyle;
            }
        }

        public GridLines GridLines
        {
            get
            {
                return dgPermissions.GridLines;
            }
            set
            {
                dgPermissions.GridLines = value;
            }
        }

        public TableItemStyle HeaderStyle
        {
            get
            {
                return dgPermissions.HeaderStyle;
            }
        }

        public DataGridItemCollection Items
        {
            get
            {
                return dgPermissions.Items;
            }
        }

        public TableItemStyle ItemStyle
        {
            get
            {
                return dgPermissions.ItemStyle;
            }
        }

        /// <summary>
        /// Gets the underlying Permissions Data Table
        /// </summary>
        public DataTable PermissionsDataTable
        {
            get
            {
                return _permissionsDataTable;
            }
        }

        /// <summary>
        /// Gets the Id of the Portal
        /// </summary>
        public int PortalId
        {
            get
            {
                // Obtain PortalSettings from Current Context
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                int intPortalID;

                if( _portalSettings.ActiveTab.ParentId == _portalSettings.SuperTabId ) //if we are in host filemanager then we need to pass a null portal id
                {
                    intPortalID = Null.NullInteger;
                }
                else
                {
                    intPortalID = _portalSettings.PortalId;
                }

                return intPortalID;
            }
        }

        /// <summary>
        /// Gets the Id of the Registered Users Role
        /// </summary>
        public int RegisteredUsersRoleId
        {
            get
            {
                return PortalController.GetCurrentPortalSettings().RegisteredRoleId;
            }
        }

        /// <summary>
        /// Gets and Sets the ResourceFile to localize permissions
        /// </summary>
        public string ResourceFile
        {
            get
            {
                return _resourceFile;
            }
            set
            {
                _resourceFile = value;
            }
        }

        /// <summary>
        /// Gets and Sets the collection of Roles to display
        /// </summary>
        public ArrayList Roles
        {
            get
            {
                return _roles;
            }
            set
            {
                _roles = value;
            }
        }

        public TableItemStyle SelectedItemStyle
        {
            get
            {
                return dgPermissions.SelectedItemStyle;
            }
        }

        /// <summary>
        /// Builds the key used to store the "permission" information in the ViewState
        /// </summary>
        /// <param name="checked">Is the checkbox checked</param>
        /// <param name="permissionId">The Id of the permission</param>
        /// <param name="objectPermissionId">The Id of the object permission</param>
        /// <param name="roleId">The role id</param>
        /// <param name="roleName">The role name</param>
        protected string BuildKey( bool @checked, int permissionId, int objectPermissionId, int roleId, string roleName )
        {
            string key;

            if( @checked )
            {
                key = "True";
            }
            else
            {
                key = "False";
            }

            key += "|" + Convert.ToString( permissionId );

            //Add objectPermissionId
            key += "|";
            if( objectPermissionId > -1 )
            {
                key += Convert.ToString( objectPermissionId );
            }

            key += "|" + roleName;
            key += "|" + roleId.ToString();

            return key;
        }

        /// <summary>
        /// Gets the Enabled status of the permission
        /// </summary>
        /// <param name="objPerm">The permission being loaded</param>
        /// <param name="role">The role</param>
        /// <param name="column">The column of the Grid</param>
        protected virtual bool GetEnabled( PermissionInfo objPerm, RoleInfo role, int column )
        {
            return true;
        }

        /// <summary>
        /// Gets the Value of the permission
        /// </summary>
        /// <param name="objPerm">The permission being loaded</param>
        /// <param name="role">The role</param>
        /// <param name="column">The column of the Grid</param>
        protected virtual bool GetPermission( PermissionInfo objPerm, RoleInfo role, int column )
        {
            return true;
        }

        /// <summary>
        /// Gets the permissions from the Database
        /// </summary>
        protected virtual ArrayList GetPermissions()
        {
            return null;
        }

        /// <summary>
        /// Bind the Grid to the PermissionsDataTable
        /// </summary>
        private void BindGrid()
        {
            this.EnsureChildControls();

            PermissionsDataTable.Columns.Clear();
            PermissionsDataTable.Rows.Clear();

            DataColumn col;

            //Add Roles Column
            col = new DataColumn( "RoleId" );
            PermissionsDataTable.Columns.Add( col );

            //Add Roles Column
            col = new DataColumn( "RoleName" );
            PermissionsDataTable.Columns.Add( col );

            int i;
            for( i = 0; i <= m_Permissions.Count - 1; i++ )
            {
                PermissionInfo objPerm;
                objPerm = (PermissionInfo)m_Permissions[i];

                //Add Enabled Column
                col = new DataColumn( objPerm.PermissionName + "_Enabled" );
                PermissionsDataTable.Columns.Add( col );

                //Add Permission Column
                col = new DataColumn( objPerm.PermissionName );
                PermissionsDataTable.Columns.Add( col );
            }

            GetRoles();

            UpdatePermissions();
            DataRow row;
            for( i = 0; i <= Roles.Count - 1; i++ )
            {
                RoleInfo role = (RoleInfo)Roles[i];
                row = PermissionsDataTable.NewRow();
                row["RoleId"] = role.RoleID;
                row["RoleName"] = Localization.LocalizeRole( role.RoleName );

                int j;
                for( j = 0; j <= m_Permissions.Count - 1; j++ )
                {
                    PermissionInfo objPerm;
                    objPerm = (PermissionInfo)m_Permissions[j];

                    row[objPerm.PermissionName + "_Enabled"] = GetEnabled( objPerm, role, j + 1 );
                    row[objPerm.PermissionName] = GetPermission( objPerm, role, j + 1 );
                }
                PermissionsDataTable.Rows.Add( row );
            }

            dgPermissions.DataSource = PermissionsDataTable;
            dgPermissions.DataBind();
        }

        /// <summary>
        /// Creates the Child Controls
        /// </summary>
        protected override void CreateChildControls()
        {
            pnlPermissions = new Panel();
            pnlPermissions.CssClass = "DataGrid_Container";

            //Optionally Add Role Group Filter
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            ArrayList arrGroups = RoleController.GetRoleGroups( _portalSettings.PortalId );
            if( arrGroups.Count > 0 )
            {
                lblGroups = new Label();
                lblGroups.Text = Localization.GetString( "RoleGroupFilter" );
                lblGroups.CssClass = "SubHead";
                pnlPermissions.Controls.Add( lblGroups );

                pnlPermissions.Controls.Add( new LiteralControl( "&nbsp;&nbsp;" ) );

                cboRoleGroups = new DropDownList();
                cboRoleGroups.SelectedIndexChanged += new EventHandler( RoleGroupsSelectedIndexChanged );
                cboRoleGroups.AutoPostBack = true;

                cboRoleGroups.Items.Add( new ListItem( Localization.GetString( "AllRoles" ), "-2" ) );
                ListItem liItem = new ListItem( Localization.GetString( "GlobalRoles" ), "-1" );
                liItem.Selected = true;
                cboRoleGroups.Items.Add( liItem );
                foreach( RoleGroupInfo roleGroup in arrGroups )
                {
                    cboRoleGroups.Items.Add( new ListItem( roleGroup.RoleGroupName, roleGroup.RoleGroupID.ToString() ) );
                }
                pnlPermissions.Controls.Add( cboRoleGroups );
            }

            dgPermissions = new DataGrid();

            AutoGenerateColumns = false;

            CellSpacing = 0;
            GridLines = GridLines.None;

            FooterStyle.CssClass = "DataGrid_Footer";
            HeaderStyle.CssClass = "DataGrid_Header";
            ItemStyle.CssClass = "DataGrid_Item";
            AlternatingItemStyle.CssClass = "DataGrid_AlternatingItem";

            SetUpDataGrid();

            pnlPermissions.Controls.Add( dgPermissions );

            this.Controls.Add( pnlPermissions );
        }

        /// <summary>
        /// Generate the Data Grid
        /// </summary>
        public abstract void GenerateDataGrid();

        /// <summary>
        /// Gets the roles from the Database and loads them into the Roles property
        /// </summary>
        private void GetRoles()
        {
            RoleController objRoleController = new RoleController();
            int RoleGroupId = -2;
            if( ( cboRoleGroups != null ) && ( cboRoleGroups.SelectedValue != null ) )
            {
                RoleGroupId = int.Parse( cboRoleGroups.SelectedValue );
            }

            if( RoleGroupId > -2 )
            {
                _roles = objRoleController.GetRolesByGroup( PortalController.GetCurrentPortalSettings().PortalId, RoleGroupId );
            }
            else
            {
                _roles = objRoleController.GetPortalRoles( PortalController.GetCurrentPortalSettings().PortalId );
            }

            if( RoleGroupId < 0 )
            {
                RoleInfo r = new RoleInfo();
                r.RoleID = int.Parse( Globals.glbRoleUnauthUser );
                r.RoleName = Globals.glbRoleUnauthUserName;
                _roles.Add( r );
                r = new RoleInfo();
                r.RoleID = int.Parse( Globals.glbRoleAllUsers );
                r.RoleName = Globals.glbRoleAllUsersName;
                _roles.Add( r );
            }
            _roles.Reverse();
            _roles.Sort( new RoleComparer() );
        }

        protected override void OnLoad( EventArgs e )
        {
        }

        /// <summary>
        /// Overrides the base OnPreRender method to Bind the Grid to the Permissions
        /// </summary>
        protected override void OnPreRender( EventArgs e )
        {
            BindGrid();
        }

        /// <summary>
        /// RoleGroupsSelectedIndexChanged runs when the Role Group is changed
        /// </summary>
        private void RoleGroupsSelectedIndexChanged( object sender, EventArgs e )
        {
            UpdatePermissions();
        }

        /// <summary>
        /// Sets up the columns for the Grid
        /// </summary>
        private void SetUpDataGrid()
        {
            Columns.Clear();

            BoundColumn textCol = new BoundColumn();
            textCol.HeaderText = "";
            textCol.DataField = "RoleName";
            Columns.Add( textCol );

            BoundColumn idCol = new BoundColumn();
            idCol.HeaderText = "";
            idCol.DataField = "roleid";
            idCol.Visible = false;
            Columns.Add( idCol );

            TemplateColumn checkCol;
            int i;

            m_Permissions = GetPermissions();
            for( i = 0; i <= m_Permissions.Count - 1; i++ )
            {
                PermissionInfo objPermission;
                objPermission = (PermissionInfo)m_Permissions[i];

                checkCol = new TemplateColumn();
                CheckBoxColumnTemplate columnTemplate = new CheckBoxColumnTemplate();
                columnTemplate.DataField = objPermission.PermissionName;
                columnTemplate.EnabledField = objPermission.PermissionName + "_Enabled";
                checkCol.ItemTemplate = columnTemplate;
                string locName = "";
                if( objPermission.ModuleDefID > 0 )
                {
                    if( ResourceFile != "" )
                    {
                        // custom permission
                        locName = Localization.GetString( objPermission.PermissionName + ".Permission", ResourceFile );
                    }
                }
                else
                {
                    // system permission
                    locName = Localization.GetString( objPermission.PermissionName + ".Permission", Localization.GlobalResourceFile );
                }
                checkCol.HeaderText = ( locName != "" ? locName : objPermission.PermissionName ).ToString();
                checkCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                checkCol.HeaderStyle.Wrap = true;
                Columns.Add( checkCol );
            }
        }

        /// <summary>
        /// Updates a Permission
        /// </summary>
        /// <param name="permission">The permission being updated</param>
        /// <param name="roleName">The name of the role</param>
        /// <param name="allowAccess">The value of the permission</param>
        protected virtual void UpdatePermission( PermissionInfo permission, int roleId, string roleName, bool allowAccess )
        {
        }

        /// <summary>
        /// Updates the permissions
        /// </summary>
        protected void UpdatePermissions()
        {
            this.EnsureChildControls();

            DataGridItem dgi;
            foreach( DataGridItem tempLoopVar_dgi in Items )
            {
                dgi = tempLoopVar_dgi;
                int i;
                for( i = 2; i <= dgi.Cells.Count - 1; i++ )
                {
                    //all except first two cells which is role names and role ids
                    if( dgi.Cells[i].Controls.Count > 0 )
                    {
                        CheckBox cb = (CheckBox)dgi.Cells[i].Controls[0];
                        UpdatePermission( ( (PermissionInfo)m_Permissions[i - 2] ), int.Parse( dgi.Cells[1].Text ), dgi.Cells[0].Text, cb.Checked );
                    }
                }
            }
        }
    }
}