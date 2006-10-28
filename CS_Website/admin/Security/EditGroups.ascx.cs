using System;
using System.Diagnostics;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
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
    public partial class EditGroups : PortalModuleBase
    {
        private int RoleGroupID = - 1;

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
                if( ( Request.QueryString["RoleGroupID"] != null ) )
                {
                    RoleGroupID = int.Parse( Request.QueryString["RoleGroupID"] );
                }

                if( Page.IsPostBack == false )
                {
                    ClientAPI.AddButtonConfirm( cmdDelete, Localization.GetString( "DeleteItem" ) );
                    RoleController objRoles = new RoleController();
                    if( RoleGroupID != - 1 )
                    {
                        RoleGroupInfo objRoleGroupInfo = RoleController.GetRoleGroup( PortalId, RoleGroupID );
                        if( objRoleGroupInfo != null )
                        {
                            txtRoleGroupName.Text = objRoleGroupInfo.RoleGroupName;
                            txtDescription.Text = objRoleGroupInfo.Description;

                            //Check if Group has any roles assigned
                            int roleCount = objRoles.GetRolesByGroup( PortalId, RoleGroupID ).Count;

                            if( roleCount > 0 )
                            {
                                cmdDelete.Visible = false;
                            }
                        }
                        else // security violation attempt to access item not related to this Module
                        {
                            Response.Redirect( Globals.NavigateURL( "Security Roles" ) );
                        }
                    }
                    else
                    {
                        cmdDelete.Visible = false;
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
                    RoleGroupInfo objRoleGroupInfo = new RoleGroupInfo();
                    objRoleGroupInfo.PortalID = PortalId;
                    objRoleGroupInfo.RoleGroupID = RoleGroupID;
                    objRoleGroupInfo.RoleGroupName = txtRoleGroupName.Text;
                    objRoleGroupInfo.Description = txtDescription.Text;

                    if( RoleGroupID == - 1 )
                    {
                        try
                        {
                            RoleController.AddRoleGroup( objRoleGroupInfo );
                        }
                        catch
                        {
                            UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "DuplicateRoleGroup", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.RedError );
                            return;
                        }
                        Response.Redirect( Globals.NavigateURL( TabId, "" ) );
                    }
                    else
                    {
                        RoleController.UpdateRoleGroup( objRoleGroupInfo );
                        Response.Redirect( Globals.NavigateURL( TabId, "", "RoleGroupID=" + RoleGroupID.ToString() ) );
                    }
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
                RoleController objRoles = new RoleController();
                RoleController.DeleteRoleGroup( PortalId, RoleGroupID );
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
                if( RoleGroupID == - 1 )
                {
                    Response.Redirect( Globals.NavigateURL( TabId, "" ) );
                }
                else
                {
                    Response.Redirect( Globals.NavigateURL( TabId, "", "RoleGroupID=" + RoleGroupID.ToString() ) );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
    }
}