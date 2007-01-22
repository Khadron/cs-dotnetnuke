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
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Security
{
    /// <summary>
    /// The Roles PortalModuleBase is used to manage the Security Roles for the
    /// portal.
    /// </summary>
    /// <history>
    /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class Roles : PortalModuleBase, IActionable
    {
        private int RoleGroupId = - 1;

        /// <summary>
        /// BindData gets the roles from the Database and binds them to the DataGrid
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        ///     [cnurse]    01/05/2006  Updated to reflect Use of Role Groups
        /// </history>
        private void BindData()
        {
            // Get the portal's roles from the database
            RoleController objRoles = new RoleController();
            ArrayList arrRoles;

            if( RoleGroupId < - 1 )
            {
                arrRoles = objRoles.GetPortalRoles( PortalId );
            }
            else
            {
                arrRoles = objRoles.GetRolesByGroup( PortalId, RoleGroupId );
            }
            grdRoles.DataSource = arrRoles;

            if( RoleGroupId < 0 )
            {
                lnkEditGroup.Visible = false;
                cmdDelete.Visible = false;
            }
            else
            {
                lnkEditGroup.Visible = true;
                lnkEditGroup.NavigateUrl = EditUrl( "RoleGroupId", RoleGroupId.ToString(), "EditGroup" );
                cmdDelete.Visible = !( arrRoles.Count > 0 );
                ClientAPI.AddButtonConfirm( cmdDelete, Localization.GetString( "DeleteItem" ) );
            }

            Localization.LocalizeDataGrid( ref grdRoles, this.LocalResourceFile );

            grdRoles.DataBind();
        }

        /// <summary>
        /// BindGroups gets the role Groups from the Database and binds them to the DropDown
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [cnurse]    01/05/2006  Created
        /// </history>
        private void BindGroups()
        {
            ArrayList arrGroups = RoleController.GetRoleGroups( PortalId );

            if( arrGroups.Count > 0 )
            {
                cboRoleGroups.Items.Clear();
                cboRoleGroups.Items.Add( new ListItem( Localization.GetString( "AllRoles" ), "-2" ) );

                ListItem liItem = new ListItem( Localization.GetString( "GlobalRoles" ), "-1" );
                if( RoleGroupId < 0 )
                {
                    liItem.Selected = true;
                }
                cboRoleGroups.Items.Add( liItem );

                foreach( RoleGroupInfo roleGroup in arrGroups )
                {
                    liItem = new ListItem( roleGroup.RoleGroupName, roleGroup.RoleGroupID.ToString() );
                    if( RoleGroupId == roleGroup.RoleGroupID )
                    {
                        liItem.Selected = true;
                    }
                    cboRoleGroups.Items.Add( liItem );
                }
                trGroups.Visible = true;
            }
            else
            {
                RoleGroupId = - 2;
                trGroups.Visible = false;
            }

            BindData();
        }

        /// <summary>
        /// FormatPeriod filters out Null values from the Period column of the Grid
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatPeriod( int period )
        {
            string _FormatPeriod = Null.NullString;
            try
            {
                if( period != Null.NullInteger )
                {
                    _FormatPeriod = period.ToString();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return _FormatPeriod;
        }

        /// <summary>
        /// FormatPrice correctly formats the fee
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatPrice( float price )
        {
            string _FormatPrice = Null.NullString;
            try
            {
                if( price != Null.NullSingle )
                {
                    _FormatPrice = price.ToString( "##0.00" );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return _FormatPrice;
        }

        /// <summary>
        /// Page_Init runs when the control is initialised
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Init( Object sender, EventArgs e )
        {
            foreach( DataGridColumn column in grdRoles.Columns )
            {
                if( column.GetType() == typeof( ImageCommandColumn ) )
                {
                    //Manage Delete Confirm JS
                    ImageCommandColumn imageColumn = (ImageCommandColumn)column;
                    if( imageColumn.CommandName == "Delete" )
                    {
                        imageColumn.OnClickJS = Localization.GetString( "DeleteItem" );
                    }
                    //Manage Edit Column NavigateURLFormatString
                    if( imageColumn.CommandName == "Edit" )
                    {
                        //The Friendly URL parser does not like non-alphanumeric characters
                        //so first create the format string with a dummy value and then
                        //replace the dummy value with the FormatString place holder
                        string formatString = EditUrl( "RoleID", "KEYFIELD", "Edit" );
                        formatString = formatString.Replace( "KEYFIELD", "{0}" );
                        imageColumn.NavigateURLFormatString = formatString;
                    }
                    //Manage Roles Column NavigateURLFormatString
                    if( imageColumn.CommandName == "UserRoles" )
                    {
                        //The Friendly URL parser does not like non-alphanumeric characters
                        //so first create the format string with a dummy value and then
                        //replace the dummy value with the FormatString place holder
                        string formatString = Globals.NavigateURL( TabId, "User Roles", "RoleId=KEYFIELD" );
                        formatString = formatString.Replace( "KEYFIELD", "{0}" );
                        imageColumn.NavigateURLFormatString = formatString;
                    }

                    //Localize Image Column Text
                    if( !String.IsNullOrEmpty(imageColumn.CommandName) )
                    {
                        imageColumn.Text = Localization.GetString( imageColumn.CommandName, this.LocalResourceFile );
                    }
                }
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ! Page.IsPostBack )
                {
                    if( ( Request.QueryString["RoleGroupID"] != null ) )
                    {
                        RoleGroupId = int.Parse( Request.QueryString["RoleGroupID"] );
                    }
                    BindGroups();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// Runs when the Index of the RoleGroups combo box changes
        /// </summary>
        /// <history>
        /// 	[cnurse]	01/06/2006  created
        /// </history>
        protected void cboRoleGroups_SelectedIndexChanged( object sender, EventArgs e )
        {
            RoleGroupId = int.Parse( cboRoleGroups.SelectedValue );
            BindData();
        }

        /// <summary>
        /// Runs when the Delete Button is clicked to delete a role group
        /// </summary>
        /// <history>
        /// 	[cnurse]	01/06/2006  created
        /// </history>
        protected void cmdDelete_Click( object sender, ImageClickEventArgs e )
        {
            RoleGroupId = int.Parse( cboRoleGroups.SelectedValue );
            if( RoleGroupId > - 1 )
            {
                RoleController.DeleteRoleGroup( PortalId, RoleGroupId );
                RoleGroupId = - 1;
            }
            BindGroups();
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                actions.Add( GetNextActionID(), Localization.GetString( "AddGroup.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "add.gif", EditUrl( "EditGroup" ), false, SecurityAccessLevel.Admin, true, false );
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "add.gif", EditUrl(), false, SecurityAccessLevel.Admin, true, false );
                actions.Add( GetNextActionID(), Localization.GetString( "UserSettings.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "settings.gif", EditUrl( "UserSettings" ), false, SecurityAccessLevel.Admin, true, false );
                return actions;
            }
        }
    }
}