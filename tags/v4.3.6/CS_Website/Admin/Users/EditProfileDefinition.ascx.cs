#region DotNetNuke License
// DotNetNuke� - http://www.dotnetnuke.com
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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Users
{
    /// <summary>
    /// The EditProfileDefinition PortalModuleBase is used to manage a Profile Property
    /// for a portal
    /// </summary>
    /// <history>
    /// 	[cnurse]	02/22/2006  Created
    /// </history>
    public partial class EditProfileDefinition : PortalModuleBase
    {
        private int PropertyDefinitionId;

        /// <summary>
        /// Gets whether we are dealing with SuperUsers
        /// </summary>
        /// <history>
        /// 	[cnurse]	05/11/2006  Created
        /// </history>
        protected bool IsSuperUser
        {
            get
            {
                if( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the Portal Id whose Users we are managing
        /// </summary>
        /// <history>
        /// 	[cnurse]	05/11/2006  Created
        /// </history>
        protected int UsersPortalId
        {
            get
            {
                int intPortalId = PortalId;
                if( IsSuperUser )
                {
                    intPortalId = Null.NullInteger;
                }
                return intPortalId;
            }
        }

        /// <summary>
        /// Page_Init runs when the control is initialised
        /// </summary>
        /// <history>
        /// 	[cnurse]	02/22/2006  Created
        /// </history>
        private void Page_Init( Object sender, EventArgs e )
        {
            this.cmdCancel.Click+=new EventHandler(cmdCancel_Click);
            this.cmdDelete.Click +=new EventHandler(cmdDelete_Click);
            this.cmdUpdate.Click +=new EventHandler(cmdUpdate_Click);

            //Get Property Definition Id from Querystring
            if( ( Request.QueryString["PropertyDefinitionId"] != null ) )
            {
                PropertyDefinitionId = int.Parse( Request.QueryString["PropertyDefinitionId"] );
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <history>
        /// 	[cnurse]	02/22/2006  Created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                //Declare Property Definition object
                ProfilePropertyDefinition propertyDefinition;

                if( PropertyDefinitionId != Null.NullInteger )
                {
                    //Get Property Definition from Data Store
                    propertyDefinition = ProfileController.GetPropertyDefinition( PropertyDefinitionId );
                }
                else
                {
                    //Create New Property Definition
                    propertyDefinition = new ProfilePropertyDefinition();
                    propertyDefinition.PortalId = PortalId;
                }

                cmdDelete.Visible = true;
                ClientAPI.AddButtonConfirm( cmdDelete, Localization.GetString( "DeleteItem" ) );

                //Bind Property Definition to Data Store
                Properties.LocalResourceFile = this.LocalResourceFile;
                Properties.DataSource = propertyDefinition;
                Properties.DataBind();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdCancel_Click runs when the Cancel button is clciked
        /// </summary>
        /// <history>
        /// 	[cnurse]	02/22/2006  Created
        /// </history>
        protected void cmdCancel_Click( object sender, EventArgs e )
        {
            try
            {
                //Redirect to Definitions page
                Response.Redirect( Globals.NavigateURL( TabId, "ManageProfile", "mid=" + ModuleId ), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdDelete_Click runs when the Delete button is clciked
        /// </summary>
        /// <history>
        /// 	[cnurse]	02/22/2006  Created
        /// </history>
        protected void cmdDelete_Click( object sender, EventArgs e )
        {
            try
            {
                if( PropertyDefinitionId != Null.NullInteger )
                {
                    //Declare Definition and "retrieve" it from the Property Editor
                    ProfilePropertyDefinition propertyDefinition;
                    propertyDefinition = (ProfilePropertyDefinition)Properties.DataSource;

                    //Delete the Property Definition
                    ProfileController.DeletePropertyDefinition( propertyDefinition );
                }

                //Clear Profile Definition Cache
                ProfileController.ClearProfileDefinitionCache( UsersPortalId );

                //Redirect to Definitions page
                Response.Redirect( Globals.NavigateURL( TabId, "ManageProfile", "mid=" + ModuleId ), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update button is clciked
        /// </summary>
        /// <history>
        /// 	[cnurse]	02/22/2006  Created
        /// </history>
        protected void cmdUpdate_Click( object sender, EventArgs e )
        {
            try
            {
                //Check if Property Editor has been updated by user
                if( Properties.IsDirty && Properties.IsValid )
                {
                    //Declare Definition and "retrieve" it from the Property Editor
                    ProfilePropertyDefinition propertyDefinition;
                    propertyDefinition = (ProfilePropertyDefinition)Properties.DataSource;

                    if( PropertyDefinitionId == Null.NullInteger )
                    {
                        //Add the Property Definition
                        PropertyDefinitionId = ProfileController.AddPropertyDefinition( propertyDefinition );

                        if( PropertyDefinitionId < Null.NullInteger )
                        {
                            UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("DuplicateName", this.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessageType.RedError);
                        }
                    }
                    else
                    {
                        //Update the Property Definition
                        ProfileController.UpdatePropertyDefinition( propertyDefinition );
                    }
                }

                //Clear Profile Definition Cache
                ProfileController.ClearProfileDefinitionCache( PortalId );

                //Redirect to Definitions page
                if( PropertyDefinitionId > Null.NullInteger )
                {
                    Response.Redirect( Globals.NavigateURL( TabId, "ManageProfile", "mid=" + ModuleId ), true );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}