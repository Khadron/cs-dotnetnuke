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
using System.Web.UI.WebControls;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;
using DataCache=DotNetNuke.Common.Utilities.DataCache;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Users
{
    /// <summary>
    /// The ProfileDefinitions PortalModuleBase is used to manage the Profile Properties
    /// for a portal
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	02/16/2006  Created
    /// </history>
    public partial class ProfileDefinitions : PortalModuleBase, IActionable
    {
        private const int COLUMN_REQUIRED = 10;
        private const int COLUMN_VISIBLE = 11;
        private const int COLUMN_MOVE_DOWN = 2;
        private const int COLUMN_MOVE_UP = 3;

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
        /// Gets the Return Url for the page
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/09/2006  Created
        /// </history>
        public string ReturnUrl
        {
            get
            {
                return Globals.NavigateURL( TabId );
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

        private ProfilePropertyDefinitionCollection m_objProperties;

        /// <summary>
        /// Gets the properties
        /// </summary>
        /// <history>
        ///     [cnurse]	02/23/2006	created
        /// </history>
        private ProfilePropertyDefinitionCollection GetProperties()
        {
            string strKey = ProfileController.PROPERTIES_CACHEKEY + "." + UsersPortalId;
            if( m_objProperties == null )
            {
                m_objProperties = (ProfilePropertyDefinitionCollection)DataCache.GetCache( strKey );
                if( m_objProperties == null )
                {
                    m_objProperties = ProfileController.GetPropertyDefinitionsByPortal( UsersPortalId );
                    DataCache.SetCache( strKey, m_objProperties );
                }
            }
            return m_objProperties;
        }

        /// <summary>
        /// Helper function that clears property cache
        /// </summary>
        /// <history>
        ///     [Jon Henning]	03/12/2006	created
        /// </history>
        private void ClearCachedProperties()
        {
            string strKey = ProfileController.PROPERTIES_CACHEKEY + "." + UsersPortalId;
            DataCache.RemoveCache( strKey );
        }

        /// <summary>
        /// Helper function that determines whether the client-side functionality is possible
        /// </summary>
        /// <history>
        ///     [Jon Henning]	03/12/2006	created
        /// </history>
        private bool SupportsRichClient()
        {
            return ClientAPI.BrowserSupportsFunctionality( ClientAPI.ClientFunctionality.DHTML );
        }

        /// <summary>
        /// Deletes a property
        /// </summary>
        /// <param name="index">The index of the Property to delete</param>
        /// <history>
        ///     [cnurse]	02/23/2006	created
        /// </history>
        private void DeleteProperty( int index )
        {
            ProfilePropertyDefinitionCollection profileProperties = GetProperties();
            ProfilePropertyDefinition objProperty = profileProperties[index];

            ProfileController.DeletePropertyDefinition( objProperty );

            RefreshGrid();
        }

        /// <summary>
        /// Moves a property
        /// </summary>
        /// <param name="index">The index of the Property to move</param>
        /// <param name="destIndex">The new index of the Property</param>
        /// <history>
        ///     [cnurse]	02/23/2006	created
        /// </history>
        private void MoveProperty( int index, int destIndex )
        {
            ProfilePropertyDefinitionCollection profileProperties = GetProperties();
            ProfilePropertyDefinition objProperty = profileProperties[index];
            ProfilePropertyDefinition objNext = profileProperties[destIndex];

            int currentOrder = objProperty.ViewOrder;
            int nextOrder = objNext.ViewOrder;

            //Swap ViewOrders
            objProperty.ViewOrder = nextOrder;
            objNext.ViewOrder = currentOrder;

            //'Refresh Grid
            profileProperties.Sort();
            BindGrid();
        }

        /// <summary>
        /// Moves a property down in the ViewOrder
        /// </summary>
        /// <param name="index">The index of the Property to move</param>
        /// <history>
        ///     [cnurse]	02/23/2006	created
        /// </history>
        private void MovePropertyDown( int index )
        {
            MoveProperty( index, index + 1 );
        }

        /// <summary>
        /// Moves a property up in the ViewOrder
        /// </summary>
        /// <param name="index">The index of the Property to move</param>
        /// <history>
        ///     [cnurse]	02/23/2006	created
        /// </history>
        private void MovePropertyUp( int index )
        {
            MoveProperty( index, index - 1 );
        }

        /// <summary>
        /// Binds the Property Collection to the Grid
        /// </summary>
        /// <history>
        ///     [cnurse]	02/23/2006	created
        /// </history>
        private void BindGrid()
        {
            ProfilePropertyDefinitionCollection properties = GetProperties();
            bool allRequired = true;
            bool allVisible = true;

            //Check whether the checkbox column headers are true or false
            foreach( ProfilePropertyDefinition profProperty in properties )
            {
                if( profProperty.Required == false )
                {
                    allRequired = false;
                }
                if( profProperty.Visible == false )
                {
                    allVisible = false;
                }

                if( ! allRequired && ! allVisible )
                {
                    goto endOfForLoop;
                }
            }
            endOfForLoop:

            foreach( DataGridColumn column in grdProfileProperties.Columns )
            {
                if( column.GetType() == typeof( CheckBoxColumn ) )
                {
                    //Manage CheckBox column events
                    CheckBoxColumn cbColumn = (CheckBoxColumn)column;
                    if( cbColumn.DataField == "Required" )
                    {
                        cbColumn.Checked = allRequired;
                    }
                    if( cbColumn.DataField == "Visible" )
                    {
                        cbColumn.Checked = allVisible;
                    }
                }
            }
            grdProfileProperties.DataSource = properties;
            grdProfileProperties.DataBind();
        }

        /// <summary>
        /// Refresh the Property Collection to the Grid
        /// </summary>
        /// <history>
        ///     [cnurse]	02/23/2006	created
        /// </history>
        private void RefreshGrid()
        {
            ProfileController.ClearProfileDefinitionCache( UsersPortalId );
            m_objProperties = null;
            BindGrid();
        }

        /// <summary>
        /// Updates any "dirty" properties
        /// </summary>
        /// <history>
        ///     [cnurse]	02/23/2006	created
        /// </history>
        private void UpdateProperties()
        {
            ProfilePropertyDefinitionCollection profileProperties = GetProperties();
            foreach( ProfilePropertyDefinition objProperty in profileProperties )
            {
                if( objProperty.IsDirty )
                {
                    ProfileController.UpdatePropertyDefinition( objProperty );
                }
            }
            ClearCachedProperties();
        }

        /// <summary>
        /// This method is responsible for taking in posted information from the grid and
        /// persisting it to the property definition collection
        /// </summary>
        /// <history>
        ///     [Jon Henning]	03/12/2006	created
        /// </history>
        private void ProcessPostBack()
        {
            try
            {
                ProfilePropertyDefinitionCollection objProperties = GetProperties();
                string[] aryNewOrder = ClientAPI.GetClientSideReorder( this.grdProfileProperties.ClientID, this.Page );
                for( int i = 0; i < this.grdProfileProperties.Items.Count; i++ )
                {
                    DataGridItem objItem = this.grdProfileProperties.Items[i];
                    ProfilePropertyDefinition objProperty = objProperties[i];
                    CheckBox chk = (CheckBox)objItem.Cells[COLUMN_REQUIRED].Controls[0];
                    objProperty.Required = chk.Checked;
                    chk = (CheckBox)objItem.Cells[COLUMN_VISIBLE].Controls[0];
                    objProperty.Visible = chk.Checked;
                }
                //assign vieworder
                for( int i = 0; i < aryNewOrder.Length; i++ )
                {
                    objProperties[Convert.ToInt32( aryNewOrder[i] )].ViewOrder = i;
                }
                objProperties.Sort();
            }
            catch( Exception ex )
            {
                throw ( ex );
            }
        }

        public string DisplayDataType( ProfilePropertyDefinition definition )
        {
            string retValue = Null.NullString;
            ListController objListController = new ListController();
            ListEntryInfo definitionEntry = objListController.GetListEntryInfo( definition.DataType );

            if( definitionEntry != null )
            {
                retValue = definitionEntry.Value;
            }

            return retValue;
        }

        /// <summary>
        /// Page_Init runs when the control is initialised
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	02/16/2006  Created
        /// </history>
        protected void Page_Init( Object sender, EventArgs e )
        {
            foreach( DataGridColumn column in grdProfileProperties.Columns )
            {
                if( column.GetType() == typeof( CheckBoxColumn ) )
                {
                    if( SupportsRichClient() == false )
                    {
                        CheckBoxColumn cbColumn = (CheckBoxColumn)column;
                        cbColumn.CheckedChanged += new DNNDataGridCheckedColumnEventHandler( grdProfileProperties_ItemCheckedChanged );
                    }
                }
                else if( column.GetType() == typeof( ImageCommandColumn ) )
                {
                    //Manage Delete Confirm JS
                    ImageCommandColumn imageColumn = (ImageCommandColumn)column;
                    switch( imageColumn.CommandName )
                    {
                        case "Delete":

                            imageColumn.OnClickJS = Localization.GetString( "DeleteItem" );
                            imageColumn.Text = Localization.GetString( "Delete", this.LocalResourceFile );
                            break;
                        case "Edit":

                            //The Friendly URL parser does not like non-alphanumeric characters
                            //so first create the format string with a dummy value and then
                            //replace the dummy value with the FormatString place holder
                            string formatString = EditUrl( "PropertyDefinitionID", "KEYFIELD", "EditProfileProperty" );
                            formatString = formatString.Replace( "KEYFIELD", "{0}" );
                            imageColumn.NavigateURLFormatString = formatString;
                            break;
                        case "MoveUp":

                            imageColumn.Text = Localization.GetString( "MoveUp", this.LocalResourceFile );
                            break;
                        case "MoveDown":

                            imageColumn.Text = Localization.GetString( "MoveDown", this.LocalResourceFile );
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	02/16/2006  Created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ! Page.IsPostBack )
                {
                    Localization.LocalizeDataGrid( ref grdProfileProperties, this.LocalResourceFile );
                    BindGrid();
                }
                else
                {
                    ProcessPostBack();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdRefresh_Click runs when the refresh button is clciked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	02/23/2006  Created
        /// </history>
        internal void cmdRefresh_Click( object sender, EventArgs e )
        {
            RefreshGrid();
        }

        /// <summary>
        /// cmdUpdate_Click runs when the update button is clciked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	02/23/2006  Created
        /// </history>
        protected void cmdUpdate_Click( object sender, EventArgs e )
        {
            try
            {
                UpdateProperties();

                //Redirect to Users page
                Response.Redirect( Globals.NavigateURL( TabId ), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// grdProfileProperties_ItemCheckedChanged runs when a checkbox in the grid
        /// is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	02/23/2006  Created
        /// </history>
        private void grdProfileProperties_ItemCheckedChanged( object sender, DNNDataGridCheckChangedEventArgs e )
        {
            string propertyName = e.Field;
            bool propertyValue = e.Checked;
            bool isAll = e.IsAll;
            int index = e.Item.ItemIndex;

            ProfilePropertyDefinitionCollection properties = GetProperties();
            ProfilePropertyDefinition profProperty;

            if( isAll )
            {
                //Update All the properties
                foreach( ProfilePropertyDefinition tempLoopVar_profProperty in properties )
                {
                    profProperty = tempLoopVar_profProperty;
                    switch( propertyName )
                    {
                        case "Required":

                            profProperty.Required = propertyValue;
                            break;
                        case "Visible":

                            profProperty.Visible = propertyValue;
                            break;
                    }
                }
            }
            else
            {
                //Update the indexed property
                profProperty = properties[index];
                switch( propertyName )
                {
                    case "Required":

                        profProperty.Required = propertyValue;
                        break;
                    case "Visible":

                        profProperty.Visible = propertyValue;
                        break;
                }
            }

            BindGrid();
        }

        /// <summary>
        /// grdProfileProperties_ItemCommand runs when a Command event is raised in the
        /// Grid
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	02/23/2006  Created
        /// </history>
        protected void grdProfileProperties_ItemCommand( object source, DataGridCommandEventArgs e )
        {
            string commandName = e.CommandName;
            int commandArgument = Convert.ToInt32( e.CommandArgument );
            int index = e.Item.ItemIndex;

            switch( commandName )
            {
                case "Delete":

                    DeleteProperty( index );
                    break;
                case "MoveUp":

                    MovePropertyUp( index );
                    break;
                case "MoveDown":

                    MovePropertyDown( index );
                    break;
            }
        }

        /// <summary>
        /// When it is determined that the client supports a rich interactivity the grdProfileProperties_ItemCreated
        /// event is responsible for disabling all the unneeded AutoPostBacks, along with assiging the appropriate
        ///	client-side script for each event handler
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [Jon Henning]	03/12/2006	created
        /// </history>
        protected void grdProfileProperties_ItemCreated( object sender, DataGridItemEventArgs e )
        {
            if( SupportsRichClient() )
            {
                switch( e.Item.ItemType )
                {
                    case ListItemType.Header:

                        //we combined the header label and checkbox in same place, so it is control 1 instead of 0
                        ( (WebControl)e.Item.Cells[COLUMN_REQUIRED].Controls[1] ).Attributes.Add( "onclick", "dnn.util.checkallChecked(this," + COLUMN_REQUIRED + ");" );
                        ( (CheckBox)e.Item.Cells[COLUMN_REQUIRED].Controls[1] ).AutoPostBack = false;
                        ( (WebControl)e.Item.Cells[COLUMN_VISIBLE].Controls[1] ).Attributes.Add( "onclick", "dnn.util.checkallChecked(this," + COLUMN_VISIBLE + ");" );
                        ( (CheckBox)e.Item.Cells[COLUMN_VISIBLE].Controls[1] ).AutoPostBack = false;
                        break;
                    case ListItemType.AlternatingItem:
                        ( (CheckBox)e.Item.Cells[COLUMN_REQUIRED].Controls[0] ).AutoPostBack = false;
                        ( (CheckBox)e.Item.Cells[COLUMN_VISIBLE].Controls[0] ).AutoPostBack = false;

                        ClientAPI.EnableClientSideReorder( e.Item.Cells[COLUMN_MOVE_DOWN].Controls[0], this.Page, false, this.grdProfileProperties.ClientID );
                        ClientAPI.EnableClientSideReorder( e.Item.Cells[COLUMN_MOVE_UP].Controls[0], this.Page, true, this.grdProfileProperties.ClientID );
                        break;

                    case ListItemType.Item:

                        ( (CheckBox)e.Item.Cells[COLUMN_REQUIRED].Controls[0] ).AutoPostBack = false;
                        ( (CheckBox)e.Item.Cells[COLUMN_VISIBLE].Controls[0] ).AutoPostBack = false;

                        ClientAPI.EnableClientSideReorder( e.Item.Cells[COLUMN_MOVE_DOWN].Controls[0], this.Page, false, this.grdProfileProperties.ClientID );
                        ClientAPI.EnableClientSideReorder( e.Item.Cells[COLUMN_MOVE_UP].Controls[0], this.Page, true, this.grdProfileProperties.ClientID );
                        break;
                }
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "add.gif", EditUrl( "EditProfileProperty" ), false, SecurityAccessLevel.Admin, true, false );

                actions.Add( GetNextActionID(), Localization.GetString( "Cancel.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "lt.gif", ReturnUrl, false, SecurityAccessLevel.Admin, true, false );
                return actions;
            }
        }
    }
}