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
using System.Collections.Generic;
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
using DotNetNuke.Entities.Profile;
using DotNetNuke.Common.Lists;
using ICSharpCode.SharpZipLib.Zip;
using FileInfo=DotNetNuke.Services.FileSystem.FileInfo;

namespace DotNetNuke.Modules.Admin.PortalManagement
{
    /// <summary>
    /// The Template PortalModuleBase is used to export a Portal as a Template
    /// </summary>
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
        private static void AddSkinXml( XmlDocument xml, XmlNode nodeToAdd, string skinRoot, string skinLevel, int id )
        {
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

                    SkinInfo skin = SkinController.GetSkin( skinRoot, id, SkinType.Portal );
                    if( skin != null )
                    {
                        XmlElement newnode = xml.CreateElement( elementprefix + "src" );
                        newnode.InnerText = skin.SkinSrc;
                        nodeToAdd.AppendChild( newnode );
                    }
                    skin = SkinController.GetSkin( skinRoot, id, SkinType.Admin );
                    if( skin != null )
                    {
                        XmlElement newnode = xml.CreateElement( elementprefix + "srcadmin" );
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
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "pages" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "splashtabid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "hometabid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "logintabid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "usertabid" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "homedirectory" ) );
            nodeSettings.RemoveChild(nodeSettings.SelectSingleNode("expirydate"));
            nodeSettings.RemoveChild(nodeSettings.SelectSingleNode("currency"));
            nodeSettings.RemoveChild(nodeSettings.SelectSingleNode("hostfee"));
            nodeSettings.RemoveChild(nodeSettings.SelectSingleNode("hostspace"));
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "pagequota" ) );
            nodeSettings.RemoveChild( nodeSettings.SelectSingleNode( "userquota" ) );
            nodeSettings.RemoveChild(nodeSettings.SelectSingleNode("backgroundfile"));
            nodeSettings.RemoveChild(nodeSettings.SelectSingleNode("paymentprocessor"));
            nodeSettings.RemoveChild(nodeSettings.SelectSingleNode("siteloghistory"));

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
        /// <param name="zipFile"></param>
        /// <remarks>
        /// The serialization uses the xml attributes defined in FileInfo class.
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/08/2004	Created
        ///     [cnurse]    05/20/2004  Extracted adding of file to zip to new FileSystemUtils method
        /// </history>
        
        public void SerializeFiles(XmlDocument xmlTemplate, XmlNode nodeFiles, PortalInfo objportal, string folderPath, ref ZipOutputStream zipFile)
        {            
            FolderController objFolders = new FolderController();
            FolderInfo objFolder = objFolders.GetFolder( objportal.PortalID, folderPath );
            ArrayList arrFiles = FileSystemUtils.GetFilesByFolder(objportal.PortalID, objFolder.FolderID);

            XmlSerializer xser = new XmlSerializer(typeof(FileInfo));

            foreach (FileInfo objFile in arrFiles)
            {
                // verify that the file exists on the file system
                string filePath = objportal.HomeDirectoryMapPath + folderPath + objFile.FileName;
                if (File.Exists(filePath))
                {
                    StringWriter sw = new StringWriter();
                    xser.Serialize(sw, objFile);

                    //Add node to template
                    XmlDocument xmlFile = new XmlDocument();
                    xmlFile.LoadXml(sw.GetStringBuilder().ToString());
                    XmlNode nodeFile = xmlFile.SelectSingleNode("file");
                    nodeFile.Attributes.Remove(nodeFile.Attributes["xmlns:xsd"]);
                    nodeFile.Attributes.Remove(nodeFile.Attributes["xmlns:xsi"]);
                    nodeFiles.AppendChild(xmlTemplate.ImportNode(nodeFile, true));

                    FileSystemUtils.AddToZip(ref zipFile, filePath, objFile.FileName, folderPath);

                }
            }
        }

        /// <summary>
        /// Serializes all Folders including Permissions
        /// </summary>
        /// <param name="xmlTemplate">Reference to XmlDocument context</param>
        /// <param name="nodeFolders"></param>
        /// <param name="objportal">Portal to serialize</param>
        /// <param name="zipFile"></param>
        /// <remarks>
        /// The serialization uses the xml attributes defined in FolderInfo class.
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/08/2004	Created
        /// </history>        
        public void SerializeFolders( XmlDocument xmlTemplate, XmlNode nodeFolders, PortalInfo objportal, ref ZipOutputStream zipFile )
        {
            // Sync db and filesystem before exporting so all required files are found
            FileSystemUtils.Synchronize(objportal.PortalID, objportal.AdministratorRoleId, objportal.HomeDirectoryMapPath);

            FolderController objFolders = new FolderController();
            ArrayList arrFolders = objFolders.GetFoldersByPortal( objportal.PortalID );

            XmlSerializer xser = new XmlSerializer( typeof( FolderInfo ) );
            foreach( FolderInfo objFolder in arrFolders )
            {
                StringWriter sw = new StringWriter();
                xser.Serialize( sw, objFolder );

                XmlDocument xmlFolder = new XmlDocument();
                xmlFolder.LoadXml( sw.GetStringBuilder().ToString() );
                XmlNode nodeFolder = xmlFolder.SelectSingleNode( "folder" );
                nodeFolder.Attributes.Remove( nodeFolder.Attributes["xmlns:xsd"] );
                nodeFolder.Attributes.Remove( nodeFolder.Attributes["xmlns:xsi"] );

                //Serialize Folder Permissions
                XmlNode nodePermissions = xmlTemplate.CreateElement( "folderpermissions" );
                SerializeFolderPermissions( xmlTemplate, nodePermissions, objportal, objFolder.FolderPath );
                nodeFolder.AppendChild( xmlFolder.ImportNode( nodePermissions, true ) );

                // Serialize files
                XmlNode nodeFiles = xmlTemplate.CreateElement( "files" );
                SerializeFiles( xmlTemplate, nodeFiles, objportal, objFolder.FolderPath, ref zipFile );
                nodeFolder.AppendChild( xmlFolder.ImportNode( nodeFiles, true ) );

                nodeFolders.AppendChild( xmlTemplate.ImportNode( nodeFolder, true ) );
            }
        }

        /// <summary>
        /// Serializes all Folder Permissions
        /// </summary>
        /// <param name="xmlTemplate">Reference to XmlDocument context</param>
        /// <param name="nodePermissions"></param>
        /// <param name="objportal">Portal to serialize</param>
        /// <param name="folderPath">The folder containing the files</param>
        /// <remarks>
        /// The serialization uses the xml attributes defined in FolderInfo class.
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/08/2004	Created
        /// </history> 
        public void SerializeFolderPermissions( XmlDocument xmlTemplate, XmlNode nodePermissions, PortalInfo objportal, string folderPath )
        {
            FolderPermissionController objPermissions = new FolderPermissionController();
            ArrayList arrPermissions = objPermissions.GetFolderPermissionsByFolder( objportal.PortalID, folderPath );

            foreach( FolderPermissionInfo objPermission in arrPermissions )
            {
                XmlElement nodePermission = xmlTemplate.CreateElement( "permission" );
                nodePermission.AppendChild( XmlUtils.CreateElement( xmlTemplate, "permissioncode", objPermission.PermissionCode ) );
                nodePermission.AppendChild( XmlUtils.CreateElement( xmlTemplate, "permissionkey", objPermission.PermissionKey ) );
                nodePermission.AppendChild( XmlUtils.CreateElement( xmlTemplate, "rolename", objPermission.RoleName ) );
                nodePermission.AppendChild( XmlUtils.CreateElement( xmlTemplate, "allowaccess", objPermission.AllowAccess.ToString().ToLower() ) );
                nodePermissions.AppendChild( nodePermission );
            }
        }

        /// <summary>
        /// Serializes all Profile Definitions
        /// </summary>
        /// <param name="xmlTemplate">Reference to XmlDocument context</param>
        /// <param name="nodeProfileDefinitions">Node to add the serialized objects</param>
        /// <param name="objportal">Portal to serialize</param>
        /// <remarks>
        /// The serialization uses the xml attributes defined in ProfilePropertyDefinition class.
        /// </remarks>
        /// <history>
        /// </history>
        public void SerializeProfileDefinitions(XmlDocument xmlTemplate, XmlNode nodeProfileDefinitions, PortalInfo objportal)
        {
            ListController objListController = new ListController();

            XmlSerializer xser = new XmlSerializer(typeof(ProfilePropertyDefinition));
            foreach (ProfilePropertyDefinition objProfileProperty in ProfileController.GetPropertyDefinitionsByPortal(objportal.PortalID))
            {
                StringWriter sw = new StringWriter();
                xser.Serialize(sw, objProfileProperty);

                XmlDocument xmlPropertyDefinition = new XmlDocument();
                xmlPropertyDefinition.LoadXml(sw.GetStringBuilder().ToString());
                XmlNode nodeProfileDefinition = xmlPropertyDefinition.SelectSingleNode("profiledefinition");
                nodeProfileDefinition.Attributes.Remove(nodeProfileDefinition.Attributes["xmlns:xsd"]);
                nodeProfileDefinition.Attributes.Remove(nodeProfileDefinition.Attributes["xmlns:xsi"]);
                ListEntryInfo objList = objListController.GetListEntryInfo(objProfileProperty.DataType);
                XmlNode newnode = xmlPropertyDefinition.CreateElement("datatype");
                if (objList == null)
                {
                    newnode.InnerXml = "Unknown";
                }
                else
                {
                    newnode.InnerXml = objList.Value;
                }
                nodeProfileDefinition.AppendChild(newnode);
                nodeProfileDefinitions.AppendChild(xmlTemplate.ImportNode(nodeProfileDefinition, true));
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
        /// </history>
        public Hashtable SerializeRoles( XmlDocument xmlTemplate, XmlNode nodeRoles, PortalInfo objportal )
        {
            RoleController objroles = new RoleController();
            Hashtable hRoles = new Hashtable();

            XmlSerializer xser = new XmlSerializer( typeof( RoleInfo ) );
            foreach( RoleInfo objrole in objroles.GetPortalRoles( objportal.PortalID ) )
            {
                StringWriter sw = new StringWriter();
                xser.Serialize( sw, objrole );

                XmlDocument xmlRole = new XmlDocument();
                xmlRole.LoadXml( sw.GetStringBuilder().ToString() );
                XmlNode nodeRole = xmlRole.SelectSingleNode( "role" );
                nodeRole.Attributes.Remove( nodeRole.Attributes["xmlns:xsd"] );
                nodeRole.Attributes.Remove( nodeRole.Attributes["xmlns:xsi"] );
                XmlNode newnode;
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
            hRoles.Add( Globals.glbRoleAllUsers, "All" );
            hRoles.Add( Globals.glbRoleUnauthUser, "Unauthenticated" );
            hRoles.Add( Globals.glbRoleSuperUser, "Super" );

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
            TabController objtabs = new TabController();

            //supporting object to build the tab hierarchy
            Hashtable hTabs = new Hashtable();

            XmlSerializer xserTabs = new XmlSerializer( typeof( TabInfo ) );
            foreach( TabInfo objtab in objtabs.GetTabs( objportal.PortalID ) )
            {
                //if not an admin tab & not deleted
                if( objtab.TabOrder < 10000 && ! objtab.IsDeleted )
                {
                    StringWriter sw = new StringWriter();
                    xserTabs.Serialize( sw, objtab );

                    XmlDocument xmlTab = new XmlDocument();
                    xmlTab.LoadXml( sw.GetStringBuilder().ToString() );
                    XmlNode nodeTab = xmlTab.SelectSingleNode( "tab" );
                    nodeTab.Attributes.Remove( nodeTab.Attributes["xmlns:xsd"] );
                    nodeTab.Attributes.Remove( nodeTab.Attributes["xmlns:xsi"] );

                    XmlNode newnode;
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
                        hTabs.Add( objtab.TabID, hTabs[objtab.ParentId] + "/" + objtab.TabName );
                    }
                    else
                    {
                        // save tab as: CurrentTabName
                        hTabs.Add( objtab.TabID, objtab.TabName );
                    }

                    // Serialize modules
                    XmlNode nodePanes;
                    nodePanes = nodeTab.AppendChild( xmlTab.CreateElement( "panes" ) );
                    ModuleController objmodules = new ModuleController();
                    DesktopModuleController objDesktopModules = new DesktopModuleController();
                    ModuleDefinitionController objModuleDefController = new ModuleDefinitionController();

                    XmlSerializer xserModules = new XmlSerializer( typeof( ModuleInfo ) );
                    Dictionary<int, ModuleInfo> dict = objmodules.GetTabModules(objtab.TabID);
                    foreach( KeyValuePair<int, ModuleInfo> pair in dict )
                    {                        
                        ModuleInfo objmodule = pair.Value;

                        if (!objmodule.IsDeleted)
                        {
                            sw = new StringWriter();
                            xserModules.Serialize(sw, objmodule);

                            XmlDocument xmlModule = new XmlDocument();
                            xmlModule.LoadXml(sw.GetStringBuilder().ToString());
                            XmlNode nodeModule = xmlModule.SelectSingleNode("module");
                            nodeModule.Attributes.Remove(nodeModule.Attributes["xmlns:xsd"]);
                            nodeModule.Attributes.Remove(nodeModule.Attributes["xmlns:xsi"]);

                            if (nodePanes.SelectSingleNode("descendant::pane[name='" + objmodule.PaneName + "']") == null)
                            {
                                // new pane found
                                XmlNode nodePane = xmlModule.CreateElement("pane");
                                XmlNode nodeName = nodePane.AppendChild(xmlModule.CreateElement("name"));
                                nodeName.InnerText = objmodule.PaneName;
                                nodePane.AppendChild(xmlModule.CreateElement("modules"));
                                nodePanes.AppendChild(xmlTab.ImportNode(nodePane, true));
                            }
                            XmlNode nodeModules = nodePanes.SelectSingleNode("descendant::pane[name='" + objmodule.PaneName + "']/modules");
                            newnode = xmlModule.CreateElement("definition");

                            ModuleDefinitionInfo objModuleDef = objModuleDefController.GetModuleDefinition(objmodule.ModuleDefID);
                            newnode.InnerText = objDesktopModules.GetDesktopModule(objModuleDef.DesktopModuleID).ModuleName;
                            nodeModule.AppendChild(newnode);

                            //Add Module Definition Info
                            XmlNode nodeDefinition = xmlModule.CreateElement("moduledefinition");
                            nodeDefinition.InnerText = objModuleDef.FriendlyName;
                            nodeModule.AppendChild(nodeDefinition);

                            if (chkContent.Checked)
                            {
                                AddContent(nodeModule, objmodule);
                            }

                            nodeModules.AppendChild(xmlTab.ImportNode(nodeModule, true));
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
        /// <history>
        /// 	[vmasanas]	25/10/2004	Created
        /// </history>
        private void AddContent( XmlNode nodeModule, ModuleInfo objModule )
        {
            if( !String.IsNullOrEmpty( objModule.BusinessControllerClass ) && objModule.IsPortable )
            {
                try
                {
                    object objObject = Reflection.CreateObject( objModule.BusinessControllerClass, objModule.BusinessControllerClass );
                    if( objObject is IPortable )
                    {
                        string Content = Convert.ToString( ( (IPortable)objObject ).ExportModule( objModule.ModuleID ) );
                        if( !String.IsNullOrEmpty( Content ) )
                        {
                            // add attributes to XML document
                            XmlNode newnode = nodeModule.OwnerDocument.CreateElement( "content" );
                            XmlAttribute xmlattr = nodeModule.OwnerDocument.CreateAttribute( "type" );
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

        private static string CleanName( string Name )
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
                if( ! Page.IsValid )
                {
                    return;
                }

                string filename = Globals.HostMapPath + txtTemplateName.Text;
                if( ! filename.EndsWith( ".template" ) )
                {
                    filename += ".template";
                }

                XmlDocument xmlTemplate = new XmlDocument();
                XmlNode nodePortal = xmlTemplate.AppendChild( xmlTemplate.CreateElement( "portal" ) );
                nodePortal.Attributes.Append( XmlUtils.CreateAttribute( xmlTemplate, "version", "3.0" ) );

                //Add template description
                XmlElement node = xmlTemplate.CreateElement( "description" );
                node.InnerXml = Server.HtmlEncode( txtDescription.Text );
                nodePortal.AppendChild( node );

                //Serialize portal settings
                PortalController objportals = new PortalController();
                PortalInfo objportal = objportals.GetPortal( Convert.ToInt32( cboPortals.SelectedValue ) );
                SerializeSettings(xmlTemplate, nodePortal, objportal);

                // Serialize Profile Definitions
                XmlNode nodeProfileDefinitions = nodePortal.AppendChild(xmlTemplate.CreateElement("profiledefinitions"));
                SerializeProfileDefinitions(xmlTemplate, nodeProfileDefinitions, objportal);

                SerializeSettings( xmlTemplate, nodePortal, objportal );

                //Serialize Roles
                XmlNode nodeRoles = nodePortal.AppendChild( xmlTemplate.CreateElement( "roles" ) );
                Hashtable hRoles = SerializeRoles( xmlTemplate, nodeRoles, objportal );

                // Serialize tabs
                XmlNode nodeTabs = nodePortal.AppendChild( xmlTemplate.CreateElement( "tabs" ) );
                SerializeTabs( xmlTemplate, nodeTabs, objportal, hRoles );

                if( chkContent.Checked )
                {
                    //Create Zip File to hold files
                    ZipOutputStream resourcesFile = new ZipOutputStream( File.Create( filename + ".resources" ) );
                    resourcesFile.SetLevel( 6 );

                    // Serialize folders (while adding files to zip file)
                    XmlNode nodeFolders;
                    nodeFolders = nodePortal.AppendChild( xmlTemplate.CreateElement( "folders" ) );
                    SerializeFolders( xmlTemplate, nodeFolders, objportal, ref resourcesFile );

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
    }
}