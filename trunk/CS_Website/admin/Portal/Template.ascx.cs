using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;
using ICSharpCode.SharpZipLib.Zip;
using FileInfo=DotNetNuke.Services.FileSystem.FileInfo;

namespace DotNetNuke.Modules.Admin.PortalManagement
{
    /// <summary>
    /// The Template PortalModuleBase is used to export a Portal as a Template
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class Template : PortalModuleBase
    {
        /// <summary>
        /// Helper method to read skins assigned at the three diferent levels: Portal, Tab, Module
        /// </summary>
        /// <param name="xml">Reference to xml document to create new elements</param>
        /// <param name="nodeToAdd">Node to add the skin information</param>
        /// <param name="skinRoot">Skin object to get: skins or containers</param>
        /// <param name="skinLevel">Skin level to get: portal, tab, module</param>
        /// <param name="id">ID of the object to get the skin. Used in conjunction with <paramref name="skinLevel"/> parameter.
        ///     Ex: if skinLevel is portal, <paramref="id"/> will be PortalID.
        /// </param>
        /// <remarks>
        /// Skin information nodes are added to <paramref="nodeToAdd"/> node.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	23/09/2004	Created
        /// </history>
        private void AddSkinXml( XmlDocument xml, XmlNode nodeToAdd, string skinRoot, string skinLevel, int id )
        {
            XmlElement newnode;
            SkinController sk = new SkinController();
            SkinInfo skin;
            string elementprefix;

            if( skinRoot == SkinInfo.RootSkin )
            {
                elementprefix = "skin";
            }
            else
            {
                elementprefix = "container";
            }

            switch( skinLevel )
            {
                case "portal":

                    skin = SkinController.GetSkin( skinRoot, id, SkinType.Portal );
                    if( skin != null )
                    {
                        newnode = xml.CreateElement( elementprefix + "src" );
                        newnode.InnerText = skin.SkinSrc;
                        nodeToAdd.AppendChild( newnode );
                    }
                    skin = SkinController.GetSkin( skinRoot, id, SkinType.Admin );
                    if( skin != null )
                    {
                        newnode = xml.CreateElement( elementprefix + "srcadmin" );
                        newnode.InnerText = skin.SkinSrc;
                        nodeToAdd.AppendChild( newnode );
                    }
                    break;
            }
        }

        /// <summary>
        /// Serializes a PortalInfo object
        /// </summary>
        /// <param name="xmlTemplate">Reference to XmlDocument context</param>
        /// <param name="nodePortal">Node to add the serialized objects</param>
        /// <param name="objportal">Portal to serialize</param>
        /// <remarks>
        /// The serialization uses the xml attributes defined in PortalInfo class.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	23/09/2004	Created
        /// </history>
        public void SerializeSettings( XmlDocument xmlTemplate, XmlNode nodePortal, PortalInfo objportal )
        {
            XmlSerializer xser;
            StringWriter sw;
            XmlNode nodeSettings;
            XmlDocument xmlSettings;

            xser = new XmlSerializer( typeof( PortalInfo ) );
            sw = new StringWriter();
            xser.Serialize( sw, objportal );

            xmlSettings = new XmlDocument();
            xmlSettings.LoadXml( sw.GetStringBuilder().ToString() );
            nodeSettings = xmlSettings.SelectSingleNode( "settings" );
            nodeSettings.Attributes.Remove( nodeSettings.Attributes["xmlns:xsd"] );
            nodeSettings.Attributes.Remove( nodeSettings.Attributes["xmlns:xsi"] );
            //remove unwanted elements
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "portalid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "portalname" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "administratorid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "administratorroleid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "administratorrolename" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "registeredroleid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "registeredrolename" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "description" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "keywords" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "processorpassword" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "processoruserid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "admintabid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "supertabid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "users" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "splashtabid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "hometabid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "logintabid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "usertabid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "homedirectory" ) );

            AddSkinXml( xmlSettings, nodeSettings, SkinInfo.RootSkin, "portal", objportal.PortalID );
            AddSkinXml( xmlSettings, nodeSettings, SkinInfo.RootContainer, "portal", objportal.PortalID );
            nodePortal.AppendChild( xmlTemplate.ImportNode( nodeSettings, true ) );
        }

        /// <summary>
        /// Serializes all Files
        /// </summary>
        /// <param name="xmlTemplate">Reference to XmlDocument context</param>
        /// <param name="nodeFiles">Node to add the serialized objects</param>
        /// <param name="objportal">Portal to serialize</param>
        /// <param name="folderPath">The folder containing the files</param>
        /// <remarks>
        /// The serialization uses the xml attributes defined in FileInfo class.
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/08/2004	Created
        ///     [cnurse]    05/20/2004  Extracted adding of file to zip to new FileSystemUtils method
        public void SerializeFiles( XmlDocument xmlTemplate, XmlNode nodeFiles, PortalInfo objportal, string folderPath, ZipOutputStream zipFile )
        {
            XmlSerializer xser;
            StringWriter sw;
            XmlNode nodeFile;
            XmlNode newnode;
            XmlDocument xmlFile;
            FileInfo objFile;
            FileController objFiles = new FileController();
            ArrayList arrFiles = objFiles.GetFilesByFolder( objportal.PortalID, folderPath );
            string filePath;

            xser = new XmlSerializer( typeof( FileInfo ) );

            foreach( FileInfo tempLoopVar_objFile in arrFiles )
            {
                objFile = tempLoopVar_objFile;
                // verify that the file exists on the file system
                filePath = PortalSettings.HomeDirectoryMapPath + folderPath + objFile.FileName;
                if( File.Exists( filePath ) )
                {
                    sw = new StringWriter();
                    xser.Serialize( sw, objFile );

                    //Add node to template
                    xmlFile = new XmlDocument();
                    xmlFile.LoadXml( sw.GetStringBuilder().ToString() );
                    nodeFile = xmlFile.SelectSingleNode( "file" );
                    nodeFile.Attributes.Remove( nodeFile.Attributes["xmlns:xsd"] );
                    nodeFile.Attributes.Remove( nodeFile.Attributes["xmlns:xsi"] );
                    nodeFiles.AppendChild( xmlTemplate.ImportNode( nodeFile, true ) );

                    ZipOutputStream zip = zipFile;
                    FileSystemUtils.AddToZip( ref zip, filePath, objFile.FileName, folderPath );
                }
            }
        }

        /// <summary>
        /// Serializes all Folders including Permissions
        /// </summary>
        /// <param name="xmlTemplate">Reference to XmlDocument context</param>
        /// <param name="nodeFiles">Node to add the serialized objects</param>
        /// <param name="objportal">Portal to serialize</param>
        /// <remarks>
        /// The serialization uses the xml attributes defined in FolderInfo class.
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/08/2004	Created
        public void SerializeFolders( XmlDocument xmlTemplate, XmlNode nodeFolders, PortalInfo objportal, ref ZipOutputStream zipFile )
        {
            XmlSerializer xser;
            StringWriter sw;
            XmlNode nodeFolder;
            XmlNode newnode;
            XmlDocument xmlFolder;
            FolderInfo objFolder;
            FolderController objFolders = new FolderController();
            ArrayList arrFolders = objFolders.GetFoldersByPortal( objportal.PortalID );

            xser = new XmlSerializer( typeof( FolderInfo ) );
            foreach( FolderInfo tempLoopVar_objFolder in arrFolders )
            {
                objFolder = tempLoopVar_objFolder;
                sw = new StringWriter();
                xser.Serialize( sw, objFolder );

                xmlFolder = new XmlDocument();
                xmlFolder.LoadXml( sw.GetStringBuilder().ToString() );
                nodeFolder = xmlFolder.SelectSingleNode( "folder" );
                nodeFolder.Attributes.Remove( nodeFolder.Attributes["xmlns:xsd"] );
                nodeFolder.Attributes.Remove( nodeFolder.Attributes["xmlns:xsi"] );

                //Serialize Folder Permissions
                XmlNode nodePermissions;
                nodePermissions = xmlTemplate.CreateElement( "folderpermissions" );
                SerializeFolderPermissions( xmlTemplate, nodePermissions, objportal, objFolder.FolderPath );
                nodeFolder.AppendChild( xmlFolder.ImportNode( nodePermissions, true ) );

                // Serialize files
                XmlNode nodeFiles;
                nodeFiles = xmlTemplate.CreateElement( "files" );
                SerializeFiles( xmlTemplate, nodeFiles, objportal, objFolder.FolderPath, zipFile );
                nodeFolder.AppendChild( xmlFolder.ImportNode( nodeFiles, true ) );

                nodeFolders.AppendChild( xmlTemplate.ImportNode( nodeFolder, true ) );
            }
        }

        /// <summary>
        /// Serializes all Folder Permissions
        /// </summary>
        /// <param name="xmlTemplate">Reference to XmlDocument context</param>
        /// <param name="nodeFiles">Node to add the serialized objects</param>
        /// <param name="objportal">Portal to serialize</param>
        /// <param name="folderPath">The folder containing the files</param>
        /// <remarks>
        /// The serialization uses the xml attributes defined in FolderInfo class.
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/08/2004	Created
        public void SerializeFolderPermissions( XmlDocument xmlTemplate, XmlNode nodePermissions, PortalInfo objportal, string folderPath )
        {
            XmlElement nodePermission;
            XmlElement newNode;
            FolderPermissionInfo objPermission;
            FolderPermissionController objPermissions = new FolderPermissionController();
            ArrayList arrPermissions = objPermissions.GetFolderPermissionsByFolder( objportal.PortalID, folderPath );

            foreach( FolderPermissionInfo tempLoopVar_objPermission in arrPermissions )
            {
                objPermission = tempLoopVar_objPermission;
                nodePermission = xmlTemplate.CreateElement( "permission" );
                nodePermission.AppendChild( XmlUtils.CreateElement( xmlTemplate, "permissioncode", objPermission.PermissionCode ) );
                nodePermission.AppendChild( XmlUtils.CreateElement( xmlTemplate, "permissionkey", objPermission.PermissionKey ) );
                nodePermission.AppendChild( XmlUtils.CreateElement( xmlTemplate, "rolename", objPermission.RoleName ) );
                nodePermission.AppendChild( XmlUtils.CreateElement( xmlTemplate, "allowaccess", objPermission.AllowAccess.ToString().ToLower() ) );
                nodePermissions.AppendChild( nodePermission );
            }
        }

        /// <summary>
        /// Serializes all Roles
        /// </summary>
        /// <param name="xmlTemplate">Reference to XmlDocument context</param>
        /// <param name="nodeRoles">Node to add the serialized objects</param>
        /// <param name="objportal">Portal to serialize</param>
        /// <returns>A hastable with all serialized roles. Will be used later to translate RoleId to RoleName</returns>
        /// <remarks>
        /// The serialization uses the xml attributes defined in RoleInfo class.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	23/09/2004	Created
        public Hashtable SerializeRoles( XmlDocument xmlTemplate, XmlNode nodeRoles, PortalInfo objportal )
        {
            XmlSerializer xser;
            StringWriter sw;
            XmlNode nodeRole;
            XmlNode newnode;
            XmlDocument xmlRole;
            RoleInfo objrole;
            RoleController objroles = new RoleController();
            Hashtable hRoles = new Hashtable();

            xser = new XmlSerializer( typeof( RoleInfo ) );
            foreach( RoleInfo tempLoopVar_objrole in objroles.GetPortalRoles( objportal.PortalID ) )
            {
                objrole = tempLoopVar_objrole;
                sw = new StringWriter();
                xser.Serialize( sw, objrole );

                xmlRole = new XmlDocument();
                xmlRole.LoadXml( sw.GetStringBuilder().ToString() );
                nodeRole = xmlRole.SelectSingleNode( "role" );
                nodeRole.Attributes.Remove( nodeRole.Attributes["xmlns:xsd"] );
                nodeRole.Attributes.Remove( nodeRole.Attributes["xmlns:xsi"] );
                if( objrole.RoleID == objportal.AdministratorRoleId )
                {
                    newnode = xmlRole.CreateElement( "roletype" );
                    newnode.InnerXml = "adminrole";
                    nodeRole.AppendChild( newnode );
                }
                if( objrole.RoleID == objportal.RegisteredRoleId )
                {
                    newnode = xmlRole.CreateElement( "roletype" );
                    newnode.InnerXml = "registeredrole";
                    nodeRole.AppendChild( newnode );
                }
                if( objrole.RoleName == "Subscribers" )
                {
                    newnode = xmlRole.CreateElement( "roletype" );
                    newnode.InnerXml = "subscriberrole";
                    nodeRole.AppendChild( newnode );
                }
                nodeRoles.AppendChild( xmlTemplate.ImportNode( nodeRole, true ) );
                // save role, we'll need them later
                hRoles.Add( objrole.RoleID.ToString(), objrole.RoleName );
            }

            // Add default DNN roles
            hRoles.Add(Globals.glbRoleAllUsers, "All");
            hRoles.Add(Globals.glbRoleUnauthUser, "Unauthenticated");
            hRoles.Add(Globals.glbRoleSuperUser, "Super");

            return hRoles;
        }

        /// <summary>
        /// Serializes all portal Tabs
        /// </summary>
        /// <param name="xmlTemplate">Reference to XmlDocument context</param>
        /// <param name="nodeTabs">Node to add the serialized objects</param>
        /// <param name="objportal">Portal to serialize</param>
        /// <param name="hRoles">A hastable with all serialized roles</param>
        /// <remarks>
        /// Only portal tabs will be exported to the template, Admin tabs are not exported.
        /// On each tab, all modules will also be exported.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	23/09/2004	Created
        /// </history>
        public void SerializeTabs( XmlDocument xmlTemplate, XmlNode nodeTabs, PortalInfo objportal, Hashtable hRoles )
        {
            XmlSerializer xserTabs;
            XmlSerializer xserModules;
            StringWriter sw;
            XmlNode nodeTab;
            XmlNode tempnode;
            XmlNode newnode;
            XmlDocument xmlTab;
            TabInfo objtab;
            TabController objtabs = new TabController();

            //supporting object to build the tab hierarchy
            Hashtable hTabs = new Hashtable();

            xserTabs = new XmlSerializer( typeof( TabInfo ) );
            foreach( TabInfo tempLoopVar_objtab in objtabs.GetTabs( objportal.PortalID ) )
            {
                objtab = tempLoopVar_objtab;
                //if not an admin tab & not deleted
                if( objtab.TabOrder < 10000 && ! objtab.IsDeleted )
                {
                    sw = new StringWriter();
                    xserTabs.Serialize( sw, objtab );

                    xmlTab = new XmlDocument();
                    xmlTab.LoadXml( sw.GetStringBuilder().ToString() );
                    nodeTab = xmlTab.SelectSingleNode( "tab" );
                    nodeTab.Attributes.Remove( nodeTab.Attributes["xmlns:xsd"] );
                    nodeTab.Attributes.Remove( nodeTab.Attributes["xmlns:xsi"] );

                    if( objtab.TabID == objportal.SplashTabId )
                    {
                        newnode = xmlTab.CreateElement( "tabtype" );
                        newnode.InnerXml = "splashtab";
                        nodeTab.AppendChild( newnode );
                    }
                    else if( objtab.TabID == objportal.HomeTabId )
                    {
                        newnode = xmlTab.CreateElement( "tabtype" );
                        newnode.InnerXml = "hometab";
                        nodeTab.AppendChild( newnode );
                    }
                    else if( objtab.TabID == objportal.UserTabId )
                    {
                        newnode = xmlTab.CreateElement( "tabtype" );
                        newnode.InnerXml = "usertab";
                        nodeTab.AppendChild( newnode );
                    }
                    else if( objtab.TabID == objportal.LoginTabId )
                    {
                        newnode = xmlTab.CreateElement( "tabtype" );
                        newnode.InnerXml = "logintab";
                        nodeTab.AppendChild( newnode );
                    }

                    if( ! Null.IsNull( objtab.ParentId ) )
                    {
                        newnode = xmlTab.CreateElement( "parent" );
                        newnode.InnerXml = Server.HtmlEncode( hTabs[objtab.ParentId].ToString() );
                        nodeTab.AppendChild( newnode );

                        // save tab as: ParentTabName/CurrentTabName
                        hTabs.Add( objtab.TabID, hTabs[objtab.ParentId].ToString() + "/" + objtab.TabName );
                    }
                    else
                    {
                        // save tab as: CurrentTabName
                        hTabs.Add( objtab.TabID, objtab.TabName );
                    }

                    // Serialize modules
                    XmlNode nodePanes;
                    XmlNode nodePane;
                    XmlNode nodeName;
                    XmlNode nodeModules;
                    XmlNode nodeModule;
                    XmlDocument xmlModule;
                    nodePanes = nodeTab.AppendChild( xmlTab.CreateElement( "panes" ) );
                    ModuleInfo objmodule;
                    ModuleController objmodules = new ModuleController();
                    DesktopModuleController objDesktopModules = new DesktopModuleController();
                    ModuleDefinitionController objModuleDef = new ModuleDefinitionController();

                    xserModules = new XmlSerializer( typeof( ModuleInfo ) );
                    foreach( ModuleInfo tempLoopVar_objmodule in objmodules.GetPortalTabModules( objtab.PortalID, objtab.TabID ) )
                    {
                        objmodule = tempLoopVar_objmodule;
                        if( ! objmodule.IsDeleted )
                        {
                            sw = new StringWriter();
                            xserModules.Serialize( sw, objmodule );

                            xmlModule = new XmlDocument();
                            xmlModule.LoadXml( sw.GetStringBuilder().ToString() );
                            nodeModule = xmlModule.SelectSingleNode( "module" );
                            nodeModule.Attributes.Remove( nodeModule.Attributes["xmlns:xsd"] );
                            nodeModule.Attributes.Remove( nodeModule.Attributes["xmlns:xsi"] );

                            if( nodePanes.SelectSingleNode( "descendant::pane[name=\'" + objmodule.PaneName + "\']" ) == null )
                            {
                                // new pane found
                                nodePane = xmlModule.CreateElement( "pane" );
                                nodeName = nodePane.AppendChild( xmlModule.CreateElement( "name" ) );
                                nodeName.InnerText = objmodule.PaneName;
                                nodePane.AppendChild( xmlModule.CreateElement( "modules" ) );
                                nodePanes.AppendChild( xmlTab.ImportNode( nodePane, true ) );
                            }
                            nodeModules = nodePanes.SelectSingleNode( "descendant::pane[name=\'" + objmodule.PaneName + "\']/modules" );
                            newnode = xmlModule.CreateElement( "definition" );

                            newnode.InnerText = objDesktopModules.GetDesktopModule( objModuleDef.GetModuleDefinition( objmodule.ModuleDefID ).DesktopModuleID ).ModuleName;
                            nodeModule.AppendChild( newnode );

                            if( chkContent.Checked )
                            {
                                AddContent( nodeModule, objmodule );
                            }

                            nodeModules.AppendChild( xmlTab.ImportNode( nodeModule, true ) );
                        }
                    }
                    nodeTabs.AppendChild( xmlTemplate.ImportNode( nodeTab, true ) );
                }
            }
        }

        /// <summary>
        /// Adds module content to the node module
        /// </summary>
        /// <param name="nodeModule">Node where to add the content</param>
        /// <param name="objModule">Module</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	25/10/2004	Created
        /// </history>
        private void AddContent( XmlNode nodeModule, ModuleInfo objModule )
        {
            XmlAttribute xmlattr;

            if( objModule.BusinessControllerClass != "" && objModule.IsPortable )
            {
                try
                {
                    object objObject = Reflection.CreateObject( objModule.BusinessControllerClass, objModule.BusinessControllerClass );
                    if( objObject is IPortable )
                    {
                        string Content = Convert.ToString( ( (IPortable)objObject ).ExportModule( objModule.ModuleID ) );
                        if( Content != "" )
                        {
                            // add attributes to XML document
                            XmlNode newnode = nodeModule.OwnerDocument.CreateElement( "content" );
                            xmlattr = nodeModule.OwnerDocument.CreateAttribute( "type" );
                            xmlattr.Value = CleanName( objModule.FriendlyName );
                            newnode.Attributes.Append( xmlattr );
                            xmlattr = nodeModule.OwnerDocument.CreateAttribute( "version" );
                            xmlattr.Value = objModule.Version;
                            newnode.Attributes.Append( xmlattr );

                            Content = Server.HtmlEncode( Content );
                            newnode.InnerXml = XmlUtils.XMLEncode( Content );

                            nodeModule.AppendChild( newnode );
                        }
                    }
                }
                catch
                {
                    //ignore errors
                }
            }
        }

        private string CleanName( string Name )
        {
            string strName = Name;
            string strBadChars = ". ~`!@#$%^&*()-_+={[}]|\\:;<,>?/" + '\u0022' + '\u0027';
            int intCounter;
            for( intCounter = 0; intCounter <= strBadChars.Length - 1; intCounter++ )
            {
                strName = strName.Replace( strBadChars.Substring( intCounter, 1 ), "" );
            }
            return strName;
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	23/09/2004	Created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ! Page.IsPostBack )
                {
                    PortalController objportals = new PortalController();
                    cboPortals.DataTextField = "PortalName";
                    cboPortals.DataValueField = "PortalId";
                    cboPortals.DataSource = objportals.GetPortals();
                    cboPortals.DataBind();
                }
            }
            catch( Exception exc )
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// Exports the selected portal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Template will be saved in Portals\_default folder.
        /// An extension of .template will be added to filename if not entered
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	23/09/2004	Created
        /// 	[cnurse]	11/08/2004	Addition of files to template
        /// </history>
        protected void cmdExport_Click( Object sender, EventArgs e )
        {
            try
            {
                XmlSerializer xser;
                StringWriter sw;
                XmlDocument xmlTemplate;
                XmlNode nodePortal;
                Hashtable hRoles;
                ZipOutputStream resourcesFile;

                if( ! Page.IsValid )
                {
                    return;
                }

                string filename;
                filename = Globals.HostMapPath + txtTemplateName.Text;
                if( ! filename.EndsWith( ".template" ) )
                {
                    filename += ".template";
                }

                xmlTemplate = new XmlDocument();
                nodePortal = xmlTemplate.AppendChild( xmlTemplate.CreateElement( "portal" ) );
                nodePortal.Attributes.Append( XmlUtils.CreateAttribute( xmlTemplate, "version", "3.0" ) );

                //Add template description
                XmlElement node = xmlTemplate.CreateElement( "description" );
                node.InnerXml = Server.HtmlEncode( txtDescription.Text );
                nodePortal.AppendChild( node );

                //Serialize portal settings
                PortalInfo objportal;
                PortalController objportals = new PortalController();
                objportal = objportals.GetPortal( Convert.ToInt32( cboPortals.SelectedValue ) );

                // Sync db and filesystem before exporting so all required files are found
                FileSystemUtils.Synchronize( objportal.PortalID, objportal.AdministratorRoleId, objportal.HomeDirectoryMapPath );

                SerializeSettings( xmlTemplate, nodePortal, objportal );

                //Serialize Roles
                XmlNode nodeRoles;
                nodeRoles = nodePortal.AppendChild( xmlTemplate.CreateElement( "roles" ) );
                hRoles = SerializeRoles( xmlTemplate, nodeRoles, objportal );

                // Serialize tabs
                XmlNode nodeTabs;
                nodeTabs = nodePortal.AppendChild( xmlTemplate.CreateElement( "tabs" ) );
                SerializeTabs( xmlTemplate, nodeTabs, objportal, hRoles );

                if( chkContent.Checked )
                {
                    //Create Zip File to hold files
                    resourcesFile = new ZipOutputStream( File.Create( filename + ".resources" ) );
                    resourcesFile.SetLevel( 6 );

                    // Serialize folders (while adding files to zip file)
                    XmlNode nodeFolders;
                    nodeFolders = nodePortal.AppendChild( xmlTemplate.CreateElement( "folders" ) );
                    SerializeFolders( xmlTemplate, nodeFolders, objportal,ref  resourcesFile );

                    //Finish and Close Zip file
                    resourcesFile.Finish();
                    resourcesFile.Close();
                }

                xmlTemplate.Save( filename );
                lblMessage.Text = string.Format( Localization.GetString( "ExportedMessage", this.LocalResourceFile ), filename, null );
            }
            catch( Exception exc )
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

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