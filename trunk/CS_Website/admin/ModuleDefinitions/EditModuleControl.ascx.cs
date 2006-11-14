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
using System.IO;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.ModuleDefinitions
{
    /// <summary>
    /// The EditModuleControl PortalModuleBase is used to edit a Module Control
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class EditModuleControl : PortalModuleBase
    {
        //tasks

        private int DesktopModuleId;
        private int ModuleDefId;
        private int ModuleControlId;

        /// <summary>
        /// Removed from Page_Load to allow for Skin Objects to be populated without duplicating code
        /// </summary>
        /// <param name="strRoot">The Root folder to parse from</param>
        /// <param name="blnRecurse">True to iterate sub-folders</param>
        /// <remarks>
        /// Loads the cboSource control list with locations of controls.
        /// </remarks>
        /// <history>
        /// 	[pgaryga]	18/08/2004	Created
        /// </history>
        private void BindControlList( string strRoot, bool blnRecurse )
        {
            string strFolder;
            string[] arrFolders;
            string strFile;
            string[] arrFiles;

            if( Directory.Exists( Request.MapPath( Globals.ApplicationPath + "/" + strRoot ) ) )
            {
                arrFolders = Directory.GetDirectories( Request.MapPath( Globals.ApplicationPath + "/" + strRoot ) );
                if( blnRecurse )
                {
                    foreach( string tempLoopVar_strFolder in arrFolders )
                    {
                        strFolder = tempLoopVar_strFolder;
                        BindControlList( strFolder.Substring( Request.MapPath( Globals.ApplicationPath ).Length + 1 ).Replace( '\\', '/' ), blnRecurse );
                    }
                }
                arrFiles = Directory.GetFiles( Request.MapPath( Globals.ApplicationPath + "/" + strRoot ), "*.ascx" );
                foreach( string tempLoopVar_strFile in arrFiles )
                {
                    strFile = tempLoopVar_strFile;
                    strFile = strRoot.Replace( '\\', '/' ) + "/" + Path.GetFileName( strFile );
                    cboSource.Items.Add( new ListItem( strFile, strFile.ToLower() ) );
                }
            }
        }

        /// <summary>
        /// LoadIcons load the Icons Combo
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void LoadIcons()
        {
            string strRoot;
            string strFile;
            string[] arrFiles;
            string strExtension;

            cboIcon.Items.Clear();
            cboIcon.Items.Add( "<" + Localization.GetString( "Not_Specified" ) + ">" );

            strRoot = cboSource.SelectedItem.Text;
            strRoot = Request.MapPath( Globals.ApplicationPath + "/" + strRoot.Substring( 0, strRoot.LastIndexOf( "/" ) ) );

            if( Directory.Exists( strRoot ) )
            {
                arrFiles = Directory.GetFiles( strRoot );
                foreach( string tempLoopVar_strFile in arrFiles )
                {
                    strFile = tempLoopVar_strFile;
                    strExtension = Path.GetExtension( strFile ).Replace( ".", "" );
                    if( Strings.InStr( 1, Globals.glbImageFileTypes + ",", strExtension + ",", 0 ) != 0 )
                    {
                        cboIcon.Items.Add( new ListItem( Path.GetFileName( strFile ), Path.GetFileName( strFile ).ToLower() ) );
                    }
                }
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ( Request.QueryString["desktopmoduleid"] != null ) )
                {
                    DesktopModuleId = int.Parse( Request.QueryString["desktopmoduleid"] );
                    if( DesktopModuleId == - 2 )
                    {
                        DesktopModuleId = Null.NullInteger;
                    }
                }
                else
                {
                    DesktopModuleId = Null.NullInteger;
                }

                if( ( Request.QueryString["moduledefid"] != null ) )
                {
                    ModuleDefId = int.Parse( Request.QueryString["moduledefid"] );
                }
                else
                {
                    ModuleDefId = Null.NullInteger;
                }

                if( ( Request.QueryString["modulecontrolid"] != null ) )
                {
                    ModuleControlId = int.Parse( Request.QueryString["modulecontrolid"] );
                }
                else
                {
                    ModuleControlId = Null.NullInteger;
                }

                if( Page.IsPostBack == false )
                {
                    DesktopModuleController objDesktopModules = new DesktopModuleController();
                    DesktopModuleInfo objDesktopModule;

                    objDesktopModule = objDesktopModules.GetDesktopModule( DesktopModuleId );
                    if( objDesktopModule != null )
                    {
                        txtModule.Text = objDesktopModule.FriendlyName;
                    }
                    else
                    {
                        txtModule.Text = Localization.GetString( "SkinObjects" );
                        txtTitle.Enabled = false;
                        cboType.Enabled = false;
                        txtViewOrder.Enabled = false;
                        cboIcon.Enabled = false;
                    }

                    ModuleDefinitionController objModuleDefinitions = new ModuleDefinitionController();
                    ModuleDefinitionInfo objModuleDefinition;

                    objModuleDefinition = objModuleDefinitions.GetModuleDefinition( ModuleDefId );
                    if( objModuleDefinition != null )
                    {
                        txtDefinition.Text = objModuleDefinition.FriendlyName;
                    }

                    ClientAPI.AddButtonConfirm( cmdDelete, Localization.GetString( "DeleteItem" ) );

                    ModuleControlController objModuleControls = new ModuleControlController();
                    ModuleControlInfo objModuleControl;

                    objModuleControl = objModuleControls.GetModuleControl( ModuleControlId );

                    // Added to populate cboSource with desktop module or skin controls
                    // Issue #586
                    BindControlList( "DesktopModules", true );
                    BindControlList( "Admin/Skins", false );
                    if( objDesktopModule == null ) // Add Container Controls
                    {
                        BindControlList( "Admin/Containers", false );
                    }

                    if( ! Null.IsNull( ModuleControlId ) )
                    {
                        if( objModuleControl != null )
                        {
                            txtKey.Text = objModuleControl.ControlKey;
                            txtTitle.Text = objModuleControl.ControlTitle;
                            if( cboSource.Items.FindByValue( objModuleControl.ControlSrc.ToString().ToLower() ) != null )
                            {
                                cboSource.Items.FindByValue( objModuleControl.ControlSrc.ToString().ToLower() ).Selected = true;
                                LoadIcons();
                            }
                            if( cboType.Items.FindByValue( Convert.ToInt32( objModuleControl.ControlType ).ToString() ) != null )
                            {
                                cboType.Items.FindByValue( Convert.ToInt32( objModuleControl.ControlType ).ToString() ).Selected = true;
                            }
                            if( ! Null.IsNull( objModuleControl.ViewOrder ) )
                            {
                                txtViewOrder.Text = objModuleControl.ViewOrder.ToString();
                            }
                            if( cboIcon.Items.FindByValue( objModuleControl.IconFile.ToLower() ) != null )
                            {
                                cboIcon.Items.FindByValue( objModuleControl.IconFile.ToLower() ).Selected = true;
                            }
                            if( ! Null.IsNull( objModuleControl.HelpURL ) )
                            {
                                txtHelpURL.Text = objModuleControl.HelpURL;
                            }
                        }
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cboSource_SelectedIndexChanged runs when the Selected Soure is changed
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cboSource_SelectedIndexChanged( object sender, EventArgs e )
        {
            LoadIcons();
        }

        /// <summary>
        /// cmdCancel_Click runs when the Cancel Button is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdCancel_Click( object sender, EventArgs e )
        {
            try
            {
                if( DesktopModuleId == - 1 )
                {
                    DesktopModuleId = - 2;
                }
                Response.Redirect( EditUrl( "desktopmoduleid", DesktopModuleId.ToString() ), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdDelete_Click runs when the Delete Button is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdDelete_Click( object sender, EventArgs e )
        {
            try
            {
                ModuleControlController objModuleControls = new ModuleControlController();

                if( ! Null.IsNull( ModuleControlId ) )
                {
                    objModuleControls.DeleteModuleControl( ModuleControlId );
                }

                if( DesktopModuleId == - 1 )
                {
                    DesktopModuleId = - 2;
                }
                Response.Redirect( EditUrl( "desktopmoduleid", DesktopModuleId.ToString() ), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update Button is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdUpdate_Click( object sender, EventArgs e )
        {
            try
            {
                if( Page.IsValid )
                {
                    ModuleControlInfo objModuleControl = new ModuleControlInfo();

                    objModuleControl.ModuleControlID = ModuleControlId;
                    objModuleControl.ModuleDefID = ModuleDefId;
                    if( txtKey.Text != "" )
                    {
                        objModuleControl.ControlKey = txtKey.Text;
                    }
                    else
                    {
                        objModuleControl.ControlKey = Null.NullString;
                    }
                    if( txtTitle.Text != "" )
                    {
                        objModuleControl.ControlTitle = txtTitle.Text;
                    }
                    else
                    {
                        objModuleControl.ControlTitle = Null.NullString;
                    }
                    objModuleControl.ControlSrc = cboSource.SelectedItem.Text;
                    objModuleControl.ControlType = (SecurityAccessLevel)Enum.Parse(typeof(SecurityAccessLevel), cboType.SelectedItem.Value);
                    if( txtViewOrder.Text != "" )
                    {
                        objModuleControl.ViewOrder = int.Parse( txtViewOrder.Text );
                    }
                    else
                    {
                        objModuleControl.ViewOrder = Null.NullInteger;
                    }
                    if( cboIcon.SelectedIndex > 0 )
                    {
                        objModuleControl.IconFile = cboIcon.SelectedItem.Text;
                    }
                    else
                    {
                        objModuleControl.IconFile = Null.NullString;
                    }

                    if( txtHelpURL.Text != "" )
                    {
                        objModuleControl.HelpURL = txtHelpURL.Text;
                    }
                    else
                    {
                        objModuleControl.HelpURL = Null.NullString;
                    }

                    ModuleControlController objModuleControls = new ModuleControlController();

                    if( Null.IsNull( ModuleControlId ) )
                    {
                        try
                        {
                            objModuleControls.AddModuleControl( objModuleControl );
                        }
                        catch
                        {
                            UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "AddControl.ErrorMessage", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.RedError );
                            return;
                        }
                    }
                    else
                    {
                        objModuleControls.UpdateModuleControl( objModuleControl );
                    }

                    if( DesktopModuleId == - 1 )
                    {
                        DesktopModuleId = - 2;
                    }
                    Response.Redirect( EditUrl( "desktopmoduleid", DesktopModuleId.ToString() ), true );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }


    }
}