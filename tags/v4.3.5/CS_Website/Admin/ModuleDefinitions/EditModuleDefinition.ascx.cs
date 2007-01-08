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
using System.IO;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Security;
using DotNetNuke.Services.EventQueue;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.ModuleDefinitions
{
    /// <summary>
    /// The EditModuleDefinition PortalModuleBase is used to edit a Module
    /// Definition
    /// </summary>
    /// <history>
    /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    ///     [cnurse]    01/13/2005  Added IActionable Implementation for the Private Assembly Package creator
    ///     [cnurse]    04/18/2005  Added support for FolderName, ModuleName and BusinessControllerClass
    ///     [cnurse]    04/21/2005  Added DefaultCacheTime properties for Module Definition
    /// </history>
    public partial class EditModuleDefinition : PortalModuleBase, IActionable
    {
        private int DesktopModuleId;

        /// <param name="strRoot">The Root folder to parse from</param>
        /// <param name="blnRecurse">True to iterate sub-folders</param>
        /// <remarks>
        /// Loads the cboSource control list with locations of controls.
        /// </remarks>
        private void BindManifestList( string strRoot, bool blnRecurse )
        {
            if( Directory.Exists( Request.MapPath( Globals.ApplicationPath + "/" + strRoot ) ) )
            {
                string[] arrFolders = Directory.GetDirectories( Request.MapPath( Globals.ApplicationPath + "/" + strRoot ) );
                if( blnRecurse )
                {
                    foreach( string tempLoopVar_strFolder in arrFolders )
                    {
                        string strFolder = tempLoopVar_strFolder;
                        BindManifestList( strFolder.Substring( Request.MapPath( Globals.ApplicationPath ).Length + 1 ).Replace( '\\', '/' ), blnRecurse );
                    }
                }
                string[] arrFiles = Directory.GetFiles( Request.MapPath( Globals.ApplicationPath + "/" + strRoot ), "*.dnn" );
                foreach( string tempLoopVar_strFile in arrFiles )
                {
                    string strFile = tempLoopVar_strFile;
                    cboManifest.Items.Add( new ListItem( Path.GetFileName( strFile ), strFile ) );
                }
            }
        }

        /// <summary>
        /// DeleteParentFolders deletes parent folders that are empty
        /// </summary>
        /// <history>
        /// 	[cnurse]	11/17/2005	created
        /// </history>
        protected void DeleteParentFolders( string folder, bool isRecursive )
        {
            try
            {
                if( folder.ToLower() != Request.MapPath( "~/DesktopModules" ).ToLower() && folder.ToLower() != Request.MapPath( "~/" ).ToLower() )
                {
                    DirectoryInfo folderInfo = new DirectoryInfo( folder );
                    DirectoryInfo parentInfo = folderInfo.Parent;

                    //Check if Folder is empty
                    if( Directory.GetFileSystemEntries( parentInfo.FullName ).Length == 0 )
                    {
                        Directory.Delete( parentInfo.FullName );
                    }

                    //recursively check parent Folders
                    if( isRecursive )
                    {
                        DeleteParentFolders( parentInfo.FullName, isRecursive );
                    }
                }
            }
            catch( Exception )
            {
            }
        }

        /// <summary>
        /// DeleteSubFolders deletes sub-folders
        /// </summary>
        /// <history>
        /// 	[cnurse]	11/17/2005	created
        /// </history>
        private void DeleteSubFolders( string folder, bool isRecursive )
        {
            try
            {
                string strDesktopModules = Request.MapPath( "~/DesktopModules" );
                if( folder.Contains( "DesktopModules" ) && ( folder == strDesktopModules || folder == strDesktopModules + "\\" || folder.Replace( strDesktopModules, "" ) == folder ) )
                {
                    return;
                }

                string strAppCode = Request.MapPath( "~/App_Code" );
                if( folder.Contains( "App_Code" ) && ( folder == strAppCode || folder == strAppCode + "\\" || folder.Replace( strAppCode, "" ) == folder ) )
                {
                    return;
                }

                Directory.Delete( folder, isRecursive );
            }
            catch( Exception )
            {
            }
        }

        /// <remarks>
        /// Loads the cboSource control list with locations of controls.
        /// </remarks>
        private void InstallManifest( string strManifest )
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load( strManifest );

                XmlNode dnnRoot = doc.DocumentElement;
                XmlElement FolderElement;
                foreach( XmlElement tempLoopVar_FolderElement in dnnRoot.SelectNodes( "folders/folder" ) )
                {
                    FolderElement = tempLoopVar_FolderElement;
                    DesktopModuleController objDesktopModules = new DesktopModuleController();
                    DesktopModuleInfo objDesktopModule = new DesktopModuleInfo();

                    objDesktopModule.DesktopModuleID = Null.NullInteger;
                    objDesktopModule.ModuleName = XmlUtils.GetNodeValue( FolderElement, "modulename", "" );
                    objDesktopModule.FolderName = XmlUtils.GetNodeValue( FolderElement, "foldername", "" );
                    objDesktopModule.FriendlyName = XmlUtils.GetNodeValue( FolderElement, "friendlyname", "" );
                    if( objDesktopModule.FolderName == "" )
                    {
                        objDesktopModule.FolderName = objDesktopModule.ModuleName;
                    }
                    objDesktopModule.Description = XmlUtils.GetNodeValue( FolderElement, "description", "" );
                    objDesktopModule.IsPremium = false;
                    objDesktopModule.IsAdmin = false;
                    objDesktopModule.Version = XmlUtils.GetNodeValue( FolderElement, "version", "" );
                    objDesktopModule.BusinessControllerClass = XmlUtils.GetNodeValue( FolderElement, "businesscontrollerclass", "" );

                    objDesktopModule.DesktopModuleID = objDesktopModules.AddDesktopModule( objDesktopModule );

                    XmlElement ModuleElement;
                    foreach( XmlElement tempLoopVar_ModuleElement in FolderElement.SelectNodes( "modules/module" ) )
                    {
                        ModuleElement = tempLoopVar_ModuleElement;
                        ModuleDefinitionController objModuleDefinitions = new ModuleDefinitionController();
                        ModuleDefinitionInfo objModuleDefinition = new ModuleDefinitionInfo();

                        objModuleDefinition.ModuleDefID = Null.NullInteger;
                        objModuleDefinition.DesktopModuleID = objDesktopModule.DesktopModuleID;
                        objModuleDefinition.FriendlyName = XmlUtils.GetNodeValue( ModuleElement, "friendlyname", "" );
                        objModuleDefinition.DefaultCacheTime = XmlUtils.GetNodeValueInt( ModuleElement, "cachetime", 0 );

                        objModuleDefinition.ModuleDefID = objModuleDefinitions.AddModuleDefinition( objModuleDefinition );

                        XmlElement ControlElement;
                        foreach( XmlElement tempLoopVar_ControlElement in ModuleElement.SelectNodes( "controls/control" ) )
                        {
                            ControlElement = tempLoopVar_ControlElement;
                            ModuleControlController objModuleControls = new ModuleControlController();
                            ModuleControlInfo objModuleControl = new ModuleControlInfo();

                            objModuleControl.ModuleControlID = Null.NullInteger;
                            objModuleControl.ModuleDefID = objModuleDefinition.ModuleDefID;
                            objModuleControl.ControlKey = XmlUtils.GetNodeValue( ControlElement, "key", "" );
                            objModuleControl.ControlSrc = XmlUtils.GetNodeValue( ControlElement, "src", "" );
                            objModuleControl.ControlTitle = XmlUtils.GetNodeValue( ControlElement, "title", "" );
                            switch( XmlUtils.GetNodeValue( ControlElement, "type", "" ) )
                            {
                                case "Anonymous":

                                    objModuleControl.ControlType = SecurityAccessLevel.Anonymous;
                                    break;
                                case "View":

                                    objModuleControl.ControlType = SecurityAccessLevel.View;
                                    break;
                                case "Edit":

                                    objModuleControl.ControlType = SecurityAccessLevel.Edit;
                                    break;
                                case "Admin":

                                    objModuleControl.ControlType = SecurityAccessLevel.Admin;
                                    break;
                                case "Host":

                                    objModuleControl.ControlType = SecurityAccessLevel.Host;
                                    break;
                            }
                            objModuleControl.HelpURL = XmlUtils.GetNodeValue( ControlElement, "helpurl", "" );
                            objModuleControl.IconFile = XmlUtils.GetNodeValue( ControlElement, "iconfile", "" );
                            objModuleControl.ViewOrder = XmlUtils.GetNodeValueInt( ControlElement, "vieworder", 0 );

                            objModuleControls.AddModuleControl( objModuleControl );
                        }
                    }
                }

                Response.Redirect( Globals.NavigateURL(), true );
            }
            catch
            {
                // can not open manifest
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "InstallManifest.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.RedError );
            }
        }

        /// <summary>
        /// LoadDefinitions fetches the control data from the database
        /// </summary>
        /// <param name="ModuleDefId">The Module definition Id</param>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void LoadControls( int ModuleDefId )
        {
            ModuleControlController objModuleControls = new ModuleControlController();
            ArrayList arrModuleControls = objModuleControls.GetModuleControls( ModuleDefId );

            if( DesktopModuleId == - 2 )
            {
                ModuleControlInfo objModuleControl;
                int intIndex;
                for( intIndex = arrModuleControls.Count - 1; intIndex >= 0; intIndex-- )
                {
                    objModuleControl = (ModuleControlInfo)arrModuleControls[intIndex];
                    if( objModuleControl.ControlType != SecurityAccessLevel.SkinObject )
                    {
                        arrModuleControls.RemoveAt( intIndex );
                    }
                }
            }

            grdControls.DataSource = arrModuleControls;
            grdControls.DataBind();

            cmdAddControl.Visible = true;
            grdControls.Visible = true;
        }

        /// <summary>
        /// LoadDefinitions fetches the definitions from the database and updates the controls
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void LoadDefinitions()
        {
            ModuleDefinitionController objModuleDefinitions = new ModuleDefinitionController();
            cboDefinitions.DataSource = objModuleDefinitions.GetModuleDefinitions( DesktopModuleId );
            cboDefinitions.DataBind();

            if( cboDefinitions.Items.Count != 0 )
            {
                cboDefinitions.SelectedIndex = 0;
                int ModuleDefId = int.Parse( cboDefinitions.SelectedItem.Value );
                LoadCacheProperties( ModuleDefId );
                LoadControls( ModuleDefId );
                tabCache.Visible = true;
            }
            else
            {
                cmdAddControl.Visible = false;
                grdControls.Visible = false;
                txtCacheTime.Text = "0";
                tabCache.Visible = false;
            }
        }

        /// <summary>
        /// LoadCacheProperties loads the Module Definitions Default Cache Time properties
        /// </summary>
        /// <history>
        /// 	[cnurse]	4/21/2005   created
        /// </history>
        private void LoadCacheProperties( int ModuleDefId )
        {
            ModuleDefinitionController objModuleDefinitionController = new ModuleDefinitionController();
            ModuleDefinitionInfo objModuleDefinition = objModuleDefinitionController.GetModuleDefinition( ModuleDefId );

            txtCacheTime.Text = objModuleDefinition.DefaultCacheTime.ToString();
        }

        /// <summary>
        /// FormatURL formats the url correctly (added a key=value onto the Querystring
        /// </summary>
        /// <param name="strKeyName">A Key</param>
        /// <param name="strKeyValue">The Module definition Id</param>
        /// <returns>A correctly formatted url</returns>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// 	[smcculloch]10/11/2004	Updated to use EditUrl overload
        /// </history>
        public string FormatURL( string strKeyName, string strKeyValue )
        {
            string _FormatUrl = Null.NullString;
            try
            {
                if( DesktopModuleId != - 2 )
                {
                    _FormatUrl = EditUrl( strKeyName, strKeyValue, "Control", "desktopmoduleid=" + DesktopModuleId.ToString(), "moduledefid=" + cboDefinitions.SelectedItem.Value );
                }
                else
                {
                    _FormatUrl = EditUrl( strKeyName, strKeyValue, "Control", "desktopmoduleid=" + DesktopModuleId.ToString(), "moduledefid=-1" );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }

            return _FormatUrl;
        }

        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        ///     [vmasanas]  31/10/2004  Populate Premium list when we are adding
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                DesktopModuleController objDesktopModules = new DesktopModuleController();

                if( ( Request.QueryString["desktopmoduleid"] != null ) )
                {
                    DesktopModuleId = int.Parse( Request.QueryString["desktopmoduleid"] );
                }
                else
                {
                    DesktopModuleId = Null.NullInteger;
                }

                if( Page.IsPostBack == false )
                {
                    //Localize Grid
                    Localization.LocalizeDataGrid(ref grdControls, this.LocalResourceFile);

                    ClientAPI.AddButtonConfirm( cmdDelete, Localization.GetString( "DeleteItem" ) );
                    ClientAPI.AddButtonConfirm( cmdDeleteDefinition, Localization.GetString( "DeleteItem" ) );

                    if( Null.IsNull( DesktopModuleId ) )
                    {
                        //Enable ReadOnly Controls for Add Mode only
                        rowManifest.Visible = true;
                        BindManifestList( "DesktopModules", true );
                        cboManifest.Items.Insert( 0, new ListItem( "<" + Localization.GetString( "None_Specified" ) + ">", "" ) );
                        txtModuleName.Enabled = true;
                        txtFolderName.Enabled = true;
                        txtVersion.Enabled = true;
                        txtBusinessClass.Enabled = true;

                        cmdDelete.Visible = false;
                        tabDefinitions.Visible = false;
                        tabCache.Visible = false;
                        tabControls.Visible = false;
                    }
                    else
                    {
                        DesktopModuleInfo objDesktopModule;

                        if( DesktopModuleId == - 2 )
                        {
                            objDesktopModule = new DesktopModuleInfo();
                            objDesktopModule.ModuleName = Localization.GetString( "SkinObjects" );
                            objDesktopModule.FolderName = Localization.GetString( "SkinObjects" );
                            objDesktopModule.FriendlyName = Localization.GetString( "SkinObjects" );
                            objDesktopModule.Description = Localization.GetString( "SkinObjectsDescription" );
                            objDesktopModule.IsPremium = false;
                            objDesktopModule.Version = "";

                            cmdUpdate.Visible = false;
                            cmdDelete.Visible = false;
                            tabDefinitions.Visible = false;
                            tabCache.Visible = false;
                            txtDescription.Enabled = false;
                            chkPremium.Enabled = false;

                            LoadControls( Null.NullInteger );
                        }
                        else
                        {
                            objDesktopModule = objDesktopModules.GetDesktopModule( DesktopModuleId );

                            LoadDefinitions();
                        }

                        rowManifest.Visible = false;

                        if( objDesktopModule != null )
                        {
                            txtModuleName.Text = objDesktopModule.ModuleName;
                            txtFolderName.Text = objDesktopModule.FolderName;
                            txtFriendlyName.Text = objDesktopModule.FriendlyName;
                            txtDescription.Text = objDesktopModule.Description;
                            txtVersion.Text = objDesktopModule.Version;
                            txtBusinessClass.Text = objDesktopModule.BusinessControllerClass;
                            chkUpgradeable.Checked = objDesktopModule.IsUpgradeable;
                            chkPortable.Checked = objDesktopModule.IsPortable;
                            chkSearchable.Checked = objDesktopModule.IsSearchable;
                            chkPremium.Checked = objDesktopModule.IsPremium;
                        }
                    }

                    PortalController objPortals = new PortalController();
                    ArrayList arrPortals = objPortals.GetPortals();
                    ArrayList arrPortalDesktopModules = objDesktopModules.GetPortalDesktopModules( Null.NullInteger, DesktopModuleId );

                    PortalInfo objPortal;
                    PortalDesktopModuleInfo objPortalDesktopModule;
                    foreach( PortalDesktopModuleInfo tempLoopVar_objPortalDesktopModule in arrPortalDesktopModules )
                    {
                        objPortalDesktopModule = tempLoopVar_objPortalDesktopModule;
                        foreach( PortalInfo tempLoopVar_objPortal in arrPortals )
                        {
                            objPortal = tempLoopVar_objPortal;
                            if( objPortal.PortalID == objPortalDesktopModule.PortalID )
                            {
                                arrPortals.Remove( objPortal );
                                goto endOfForLoop;
                            }
                        }
                        endOfForLoop:
                        1.GetHashCode(); //nop
                    }

                    ctlPortals.Available = arrPortals;
                    ctlPortals.Assigned = arrPortalDesktopModules;
                    ctlPortals.Visible = chkPremium.Checked;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdAddControl_Click runs when the Add Control Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdAddControl_Click( object sender, EventArgs e )
        {
            Response.Redirect( FormatURL( "modulecontrolid", "-1" ) );
        }

        /// <summary>
        /// cmdAddDefinition_Click runs when the Add Definition Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdAddDefinition_Click( object sender, EventArgs e )
        {
            int ModuleDefId;

            if( !String.IsNullOrEmpty(txtDefinition.Text) )
            {
                ModuleDefinitionInfo objModuleDefinition = new ModuleDefinitionInfo();

                objModuleDefinition.DesktopModuleID = DesktopModuleId;
                objModuleDefinition.FriendlyName = txtDefinition.Text;

                try
                {
                    objModuleDefinition.DefaultCacheTime = int.Parse( txtCacheTime.Text );
                    if( objModuleDefinition.DefaultCacheTime < 0 )
                    {
                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "UpdateCache.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.RedError );
                        return;
                    }
                }
                catch
                {
                    UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "UpdateCache.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.RedError );
                    return;
                }

                ModuleDefinitionController objModuleDefinitions = new ModuleDefinitionController();

                try
                {
                    ModuleDefId = objModuleDefinitions.AddModuleDefinition( objModuleDefinition );
                }
                catch
                {
                    UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "AddDefinition.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.RedError );
                    return;
                }

                LoadDefinitions();

                if( ModuleDefId > - 1 )
                {
                    //Set the Combo
                    cboDefinitions.SelectedIndex = - 1;
                    cboDefinitions.Items.FindByValue( ModuleDefId.ToString() ).Selected = true;
                    LoadCacheProperties( ModuleDefId );
                    LoadControls( ModuleDefId );
                    //Clear the Text Box
                    txtDefinition.Text = "";
                }
            }
            else
            {
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "MissingDefinition.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.RedError );
            }
        }

        /// <summary>
        /// cmdCancel_Click runs when the Cancel Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdCancel_Click( object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Globals.NavigateURL(), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cboDefinitions_SelectedIndexChanged runs when item in the Definitions combo is changed
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cboDefinitions_SelectedIndexChanged( object sender, EventArgs e )
        {
            int ModuleDefId = int.Parse( cboDefinitions.SelectedItem.Value );
            LoadCacheProperties( ModuleDefId );
            LoadControls( ModuleDefId );
        }

        /// <summary>
        /// cmdDelete_Click runs when the Delete Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdDelete_Click( object sender, EventArgs e )
        {
            try
            {
                string strFileName;
                string strFileExtension;
                string[] arrFiles;

                if( ! Null.IsNull( DesktopModuleId ) )
                {
                    string strRoot = Request.MapPath( "~/DesktopModules/" + txtFolderName.Text ) + "\\";

                    // process uninstall script
                    string strProviderType = "data";
                    ProviderConfiguration objProviderConfiguration = ProviderConfiguration.GetProviderConfiguration( strProviderType );
                    string strUninstallScript = "Uninstall." + objProviderConfiguration.DefaultProvider;
                    if( File.Exists( strRoot + strUninstallScript ) )
                    {
                        // read uninstall script
                        StreamReader objStreamReader;
                        objStreamReader = File.OpenText( strRoot + strUninstallScript );
                        string strScript = objStreamReader.ReadToEnd();
                        objStreamReader.Close();

                        // execute uninstall script
                        PortalSettings.ExecuteScript( strScript );
                    }

                    if( Directory.Exists( strRoot ) )
                    {
                        // check for existence of project file ( this indicates a development environment )
                        arrFiles = Directory.GetFiles( strRoot, "*.??proj" );
                        bool isRunTime = ( arrFiles.Length == 0 ) && ( ! Request.IsLocal );
                        if( isRunTime )
                        {
                            //runtime so remove files/folders
                            // find dnn manifest file
                            arrFiles = Directory.GetFiles( strRoot, "*.dnn.config" );
                            if( arrFiles.Length == 0 )
                            {
                                arrFiles = Directory.GetFiles( strRoot, "*.dnn" ); // legacy versions stored the *.dnn files unprotected
                            }
                            if( arrFiles.Length != 0 )
                            {
                                if( File.Exists( strRoot + Path.GetFileName( arrFiles[0] ) ) )
                                {
                                    XmlDocument xmlDoc = new XmlDocument();
                                    XmlNode nodeFile;

                                    // load the manifest file
                                    xmlDoc.Load( strRoot + Path.GetFileName( arrFiles[0] ) );

                                    // check version
                                    XmlNode nodeModule = null;
                                    switch( xmlDoc.DocumentElement.LocalName.ToLower() )
                                    {
                                        case "module":

                                            nodeModule = xmlDoc.SelectSingleNode( "//module" );
                                            break;
                                        case "dotnetnuke":

                                            string version = xmlDoc.DocumentElement.Attributes["version"].InnerText;
                                            switch( version )
                                            {
                                                case "2.0":

                                                    // V2 allows for multiple folders in a single DNN definition - we need to identify the correct node
                                                    foreach( XmlNode tempLoopVar_nodeModule in xmlDoc.SelectNodes( "//dotnetnuke/folders/folder" ) )
                                                    {
                                                        nodeModule = tempLoopVar_nodeModule;
                                                        if( nodeModule.SelectSingleNode( "name" ).InnerText.Trim() == txtFriendlyName.Text )
                                                        {
                                                            goto endOfForLoop;
                                                        }
                                                    }
                                                    endOfForLoop:
                                                    break;
                                                case "3.0":

                                                    // V3 also allows for multiple folders in a single DNN definition - but uses module name
                                                    foreach( XmlNode tempLoopVar_nodeModule in xmlDoc.SelectNodes( "//dotnetnuke/folders/folder" ) )
                                                    {
                                                        nodeModule = tempLoopVar_nodeModule;
                                                        if( nodeModule.SelectSingleNode( "name" ).InnerText.Trim() == txtModuleName.Text )
                                                        {
                                                            goto endOfForLoop1;
                                                        }
                                                    }
                                                    endOfForLoop1:
                                                    break;
                                            }
                                            break;
                                    }

                                    // loop through file nodes
                                    foreach( XmlNode tempLoopVar_nodeFile in nodeModule.SelectNodes( "files/file" ) )
                                    {
                                        nodeFile = tempLoopVar_nodeFile;
                                        strFileName = nodeFile.SelectSingleNode( "name" ).InnerText.Trim();
                                        strFileExtension = Path.GetExtension( strFileName ).Replace( ".", "" );
                                        if( strFileExtension == "dll" )
                                        {
                                            // remove DLL from /bin
                                            strFileName = Request.MapPath( "~/bin/" ) + strFileName;
                                        }
                                        if( File.Exists( strFileName ) )
                                        {
                                            File.SetAttributes( strFileName, FileAttributes.Normal );
                                            File.Delete( strFileName );
                                        }
                                    }

                                    //Recursively Delete any sub Folders
                                    DeleteSubFolders( strRoot, true );

                                    //Recursively delete AppCode folders
                                    string appCode = strRoot.Replace( "DesktopModules", "App_Code" );
                                    DeleteSubFolders( appCode, true );

                                    //Delete the <codeSubDirectory> node in web.config
                                    Config.RemoveCodeSubDirectory( txtFolderName.Text );
                                }
                            }
                        }
                    }

                    // delete the desktopmodule database record
                    DesktopModuleController objDesktopModules = new DesktopModuleController();
                    objDesktopModules.DeleteDesktopModule( DesktopModuleId );
                }

                Response.Redirect( Globals.NavigateURL(), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdDeleteDefinition_Click runs when the Delete Definition Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdDeleteDefinition_Click( object sender, EventArgs e )
        {
            ModuleDefinitionController objModuleDefinitions = new ModuleDefinitionController();

            objModuleDefinitions.DeleteModuleDefinition( int.Parse( cboDefinitions.SelectedItem.Value ) );

            LoadDefinitions();
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update Button is clicked
        /// </summary>
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
                    DesktopModuleInfo objDesktopModule = new DesktopModuleInfo();

                    objDesktopModule.DesktopModuleID = DesktopModuleId;
                    objDesktopModule.ModuleName = txtModuleName.Text;
                    objDesktopModule.FolderName = txtFolderName.Text;
                    objDesktopModule.FriendlyName = txtFriendlyName.Text;
                    if( objDesktopModule.FolderName == "" )
                    {
                        objDesktopModule.FolderName = objDesktopModule.ModuleName;
                    }
                    objDesktopModule.Description = txtDescription.Text;
                    objDesktopModule.IsPremium = chkPremium.Checked;
                    objDesktopModule.IsAdmin = false;

                    if( !String.IsNullOrEmpty(txtVersion.Text) )
                    {
                        objDesktopModule.Version = txtVersion.Text;
                    }
                    else
                    {
                        objDesktopModule.Version = Null.NullString;
                    }

                    if( !String.IsNullOrEmpty(txtBusinessClass.Text) )
                    {
                        objDesktopModule.BusinessControllerClass = txtBusinessClass.Text;
                    }
                    else
                    {
                        objDesktopModule.BusinessControllerClass = Null.NullString;
                    }

                    DesktopModuleController objDesktopModules = new DesktopModuleController();

                    if( Null.IsNull( DesktopModuleId ) )
                    {
                        try
                        {
                            objDesktopModule.DesktopModuleID = objDesktopModules.AddDesktopModule( objDesktopModule );
                        }
                        catch
                        {
                            UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "AddModule.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.RedError );
                            return;
                        }
                    }
                    else
                    {
                        objDesktopModules.UpdateDesktopModule( objDesktopModule );
                    }

                    // delete old portal module assignments
                    objDesktopModules.DeletePortalDesktopModules( Null.NullInteger, objDesktopModule.DesktopModuleID );
                    // add new portal module assignments
                    if( objDesktopModule.IsPremium )
                    {
                        ListItem objListItem;
                        foreach( ListItem tempLoopVar_objListItem in ctlPortals.Assigned )
                        {
                            objListItem = tempLoopVar_objListItem;
                            objDesktopModules.AddPortalDesktopModule( int.Parse( objListItem.Value ), objDesktopModule.DesktopModuleID );
                        }
                    }
                    //Check to see if Interfaces (SupportedFeatures) Need to be Updated
                    if( !String.IsNullOrEmpty(objDesktopModule.BusinessControllerClass) )
                    {
                        //this cannot be done directly at this time because
                        //the module may not be loaded into the app domain yet
                        //So send an EventMessage that will process the update
                        //after the App recycles
                        EventMessage oAppStartMessage = new EventMessage();
                        oAppStartMessage.ProcessorType = "DotNetNuke.Entities.Modules.EventMessageProcessor, DotNetNuke";
                        oAppStartMessage.Attributes.Add( "ProcessCommand", "UpdateSupportedFeatures" );
                        oAppStartMessage.Attributes.Add( "BusinessControllerClass", objDesktopModule.BusinessControllerClass );
                        oAppStartMessage.Attributes.Add( "DesktopModuleId", objDesktopModule.DesktopModuleID.ToString() );
                        oAppStartMessage.Priority = MessagePriority.High;
                        oAppStartMessage.SentDate = DateTime.Now;
                        oAppStartMessage.Body = "";
                        //make it expire as soon as it's processed
                        oAppStartMessage.ExpirationDate = DateTime.Now.AddYears( - 1 );
                        //send it
                        EventQueueController oEventQueueController = new EventQueueController();
                        oEventQueueController.SendMessage( oAppStartMessage, "Application_Start" );

                        //force an app restart
                        Config.Touch();
                    }

                    Response.Redirect( EditUrl( "desktopmoduleid", objDesktopModule.DesktopModuleID.ToString() ), true );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdUpdateCacheTime_Click runs when the Update Cache Time Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	4/20/2005	Created
        /// </history>
        protected void cmdUpdateCacheTime_Click( object sender, EventArgs e )
        {
            if( cboDefinitions.SelectedItem != null )
            {
                int ModuleDefId = int.Parse( cboDefinitions.SelectedItem.Value );
                ModuleDefinitionController objModuleDefinitions = new ModuleDefinitionController();
                ModuleDefinitionInfo objModuleDefinition = objModuleDefinitions.GetModuleDefinition( ModuleDefId );

                try
                {
                    objModuleDefinition.DefaultCacheTime = int.Parse( txtCacheTime.Text );
                    objModuleDefinitions.UpdateModuleDefinition( objModuleDefinition );
                }
                catch( Exception )
                {
                    UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "UpdateCache.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.RedError );
                }
            }
            else
            {
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "MissingDefinition.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.RedError );
            }
        }

        protected void chkPremium_CheckedChanged( object sender, EventArgs e )
        {
            ctlPortals.Visible = chkPremium.Checked;
        }

        protected void cmdInstall_Click( object sender, EventArgs e )
        {
            if( cboManifest.SelectedItem != null )
            {
                if( !String.IsNullOrEmpty(cboManifest.SelectedItem.Value) )
                {
                    InstallManifest( cboManifest.SelectedItem.Value );
                }
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                DesktopModuleController objDesktopModules = new DesktopModuleController();
                int mid;
                if( ( Request.QueryString["desktopmoduleid"] != null ) )
                {
                    mid = int.Parse( Request.QueryString["desktopmoduleid"] );
                }
                else
                {
                    mid = Null.NullInteger;
                }
                DesktopModuleInfo objDesktopModule = objDesktopModules.GetDesktopModule( mid );
                ModuleActionCollection actions = new ModuleActionCollection();
                if( objDesktopModule != null )
                {
                    if( ! objDesktopModule.IsAdmin )
                    {
                        //Create the DirectoryInfo object for the folder
                        DirectoryInfo folder = new DirectoryInfo( Globals.ApplicationMapPath + "\\DesktopModules\\" + objDesktopModule.FolderName );
                        if( folder.Exists )
                        {
                            //Check for app_code folder
                            DirectoryInfo appCodeFolder = new DirectoryInfo( Globals.ApplicationMapPath + "\\App_Code\\" + objDesktopModule.FolderName );

                            if( appCodeFolder.Exists )
                            {
                                //Add menu item to Actionmenu collectio
                                actions.Add( GetNextActionID(), Localization.GetString( "PrivateAssemblyCreate.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl( "desktopmoduleid", mid.ToString(), "package" ), false, SecurityAccessLevel.Host, true, false );
                            }
                        }
                    }
                }
                return actions;
            }
        }
    }
}