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
using System.Diagnostics;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Security
{
    /// <summary>
    /// The EditRoles PortalModuleBase is used to manage a Security Role
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class EditRoles : PortalModuleBase
    {
        private int RoleID = - 1;

        private void ActivateControls( bool enabled )
        {
            txtDescription.Enabled = enabled;
            cboRoleGroups.Enabled = enabled;
            chkIsPublic.Enabled = enabled;
            chkAutoAssignment.Enabled = enabled;
            txtServiceFee.Enabled = enabled;
            txtBillingPeriod.Enabled = enabled;
            cboBillingFrequency.Enabled = enabled;
            txtTrialFee.Enabled = enabled;
            txtTrialPeriod.Enabled = enabled;
            cboTrialFrequency.Enabled = enabled;
            txtRSVPCode.Enabled = enabled;
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

            cboRoleGroups.Items.Add( new ListItem( Localization.GetString( "GlobalRoles" ), "-1" ) );

            foreach( RoleGroupInfo roleGroup in arrGroups )
            {
                cboRoleGroups.Items.Add( new ListItem( roleGroup.RoleGroupName, roleGroup.RoleGroupID.ToString() ) );
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ( Request.QueryString["RoleID"] != null ) )
                {
                    RoleID = int.Parse( Request.QueryString["RoleID"] );
                }

                if( Page.IsPostBack == false )
                {
                    ClientAPI.AddButtonConfirm( cmdDelete, Localization.GetString( "DeleteItem" ) );

                    RoleController objUser = new RoleController();

                    ListController ctlList = new ListController();
                    ListEntryInfoCollection colFrequencies = ctlList.GetListEntryInfoCollection( "Frequency", "" );

                    cboBillingFrequency.DataSource = colFrequencies;
                    cboBillingFrequency.DataBind();
                    cboBillingFrequency.Items.FindByValue( "N" ).Selected = true;

                    cboTrialFrequency.DataSource = colFrequencies;
                    cboTrialFrequency.DataBind();
                    cboTrialFrequency.Items.FindByValue( "N" ).Selected = true;

                    BindGroups();

                    ctlIcon.FileFilter = Globals.glbImageFileTypes;

                    if( RoleID != - 1 )
                    {
                        lblRoleName.Visible = true;
                        txtRoleName.Visible = false;
                        valRoleName.Enabled = false;

                        RoleInfo objRoleInfo = objUser.GetRole( RoleID, PortalSettings.PortalId );

                        if( objRoleInfo != null )
                        {
                            lblRoleName.Text = objRoleInfo.RoleName;
                            txtDescription.Text = objRoleInfo.Description;
                            if( cboRoleGroups.Items.FindByValue( objRoleInfo.RoleGroupID.ToString() ) != null )
                            {
                                cboRoleGroups.ClearSelection();
                                cboRoleGroups.Items.FindByValue( objRoleInfo.RoleGroupID.ToString() ).Selected = true;
                            }
                            if( Strings.Format( objRoleInfo.ServiceFee, "#,##0.00" ) != "0.00" )
                            {
                                txtServiceFee.Text = Strings.Format( objRoleInfo.ServiceFee, "#,##0.00" );
                                txtBillingPeriod.Text = objRoleInfo.BillingPeriod.ToString();
                                if( cboBillingFrequency.Items.FindByValue( objRoleInfo.BillingFrequency ) != null )
                                {
                                    cboBillingFrequency.ClearSelection();
                                    cboBillingFrequency.Items.FindByValue( objRoleInfo.BillingFrequency ).Selected = true;
                                }
                            }
                            if( objRoleInfo.TrialFrequency != "N" )
                            {
                                txtTrialFee.Text = Strings.Format( objRoleInfo.TrialFee, "#,##0.00" );
                                txtTrialPeriod.Text = objRoleInfo.TrialPeriod.ToString();
                                if( cboTrialFrequency.Items.FindByValue( objRoleInfo.TrialFrequency ) != null )
                                {
                                    cboTrialFrequency.ClearSelection();
                                    cboTrialFrequency.Items.FindByValue( objRoleInfo.TrialFrequency ).Selected = true;
                                }
                            }
                            chkIsPublic.Checked = objRoleInfo.IsPublic;
                            chkAutoAssignment.Checked = objRoleInfo.AutoAssignment;
                            txtRSVPCode.Text = objRoleInfo.RSVPCode;
                            ctlIcon.Url = objRoleInfo.IconFile;
                        }
                        else // security violation attempt to access item not related to this Module
                        {
                            Response.Redirect( Globals.NavigateURL( "Security Roles" ) );
                        }

                        if( RoleID == PortalSettings.AdministratorRoleId || RoleID == PortalSettings.RegisteredRoleId )
                        {
                            cmdDelete.Visible = false;
                            cmdUpdate.Visible = false;
                            ActivateControls( false );
                        }

                        if( RoleID == PortalSettings.RegisteredRoleId )
                        {
                            cmdManage.Visible = false;
                        }
                    }
                    else
                    {
                        cmdDelete.Visible = false;
                        cmdManage.Visible = false;
                        lblRoleName.Visible = false;
                        txtRoleName.Visible = true;
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdUpdate_Click runs when the update Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdUpdate_Click( object sender, EventArgs e )
        {
            try
            {
                if( Page.IsValid )
                {
                    float sglServiceFee = 0;
                    int intBillingPeriod = 1;
                    string strBillingFrequency = "N";

                    if( !String.IsNullOrEmpty(txtServiceFee.Text) && !String.IsNullOrEmpty(txtBillingPeriod.Text) && cboBillingFrequency.SelectedItem.Value != "N" )
                    {
                        sglServiceFee = float.Parse( txtServiceFee.Text );
                        intBillingPeriod = int.Parse( txtBillingPeriod.Text );
                        strBillingFrequency = cboBillingFrequency.SelectedItem.Value;
                    }

                    float sglTrialFee = 0;
                    int intTrialPeriod = 1;
                    string strTrialFrequency = "N";

                    if( sglServiceFee != 0 && !String.IsNullOrEmpty(txtTrialFee.Text) && !String.IsNullOrEmpty(txtTrialPeriod.Text) && cboTrialFrequency.SelectedItem.Value != "N" )
                    {
                        sglTrialFee = float.Parse( txtTrialFee.Text );
                        intTrialPeriod = int.Parse( txtTrialPeriod.Text );
                        strTrialFrequency = cboTrialFrequency.SelectedItem.Value;
                    }

                    RoleController objRoleController = new RoleController();
                    RoleInfo objRoleInfo = new RoleInfo();
                    objRoleInfo.PortalID = PortalId;
                    objRoleInfo.RoleID = RoleID;
                    objRoleInfo.RoleGroupID = int.Parse( cboRoleGroups.SelectedValue );
                    objRoleInfo.RoleName = txtRoleName.Text;
                    objRoleInfo.Description = txtDescription.Text;
                    objRoleInfo.ServiceFee = sglServiceFee;
                    objRoleInfo.BillingPeriod = intBillingPeriod;
                    objRoleInfo.BillingFrequency = strBillingFrequency;
                    objRoleInfo.TrialFee = sglTrialFee;
                    objRoleInfo.TrialPeriod = intTrialPeriod;
                    objRoleInfo.TrialFrequency = strTrialFrequency;
                    objRoleInfo.IsPublic = chkIsPublic.Checked;
                    objRoleInfo.AutoAssignment = chkAutoAssignment.Checked;
                    objRoleInfo.RSVPCode = txtRSVPCode.Text;
                    objRoleInfo.IconFile = ctlIcon.Url;

                    EventLogController objEventLog = new EventLogController();
                    if( RoleID == - 1 )
                    {
                        if( objRoleController.GetRoleByName( PortalId, objRoleInfo.RoleName ) == null )
                        {
                            objRoleController.AddRole( objRoleInfo );
                            objEventLog.AddLog( objRoleInfo, PortalSettings, UserId, "", EventLogController.EventLogType.ROLE_CREATED );
                        }
                        else
                        {
                            UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "DuplicateRole", this.LocalResourceFile ), ModuleMessageType.RedError );
                            return;
                        }
                    }
                    else
                    {
                        objRoleController.UpdateRole( objRoleInfo );
                        objEventLog.AddLog( objRoleInfo, PortalSettings, UserId, "", EventLogController.EventLogType.ROLE_UPDATED );
                    }

                    //Clear Roles Cache
                    DataCache.RemoveCache( "GetRoles" );

                    Response.Redirect( Globals.NavigateURL() );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdDelete_Click runs when the delete Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdDelete_Click( object sender, EventArgs e )
        {
            try
            {
                RoleController objUser = new RoleController();

                objUser.DeleteRole( RoleID, PortalSettings.PortalId );
                EventLogController objEventLog = new EventLogController();
                objEventLog.AddLog( "RoleID", RoleID.ToString(), PortalSettings, UserId, EventLogController.EventLogType.ROLE_DELETED );

                //Clear Roles Cache
                DataCache.RemoveCache( "GetRoles" );

                Response.Redirect( Globals.NavigateURL() );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdCancel_Click runs when the cancel Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdCancel_Click( object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Globals.NavigateURL() );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdManage_Click runs when the Manage Users Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdManage_Click( Object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Globals.NavigateURL( this.TabId, "User Roles", "RoleId=" + RoleID ) );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }


    }
}