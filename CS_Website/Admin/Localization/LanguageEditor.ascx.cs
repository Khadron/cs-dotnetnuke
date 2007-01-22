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
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;
using Globals=DotNetNuke.Common.Globals;
using TreeNode=DotNetNuke.UI.WebControls.TreeNode;
using TreeNodeCollection=DotNetNuke.UI.WebControls.TreeNodeCollection;

namespace DotNetNuke.Services.Localization
{
    /// <summary>
    /// Manages translations for Resource files
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[vmasanas]	10/04/2004  Created
    /// 	[vmasanas]	25/03/2006	Modified to support new host resources and incremental saving
    /// </history>
    public partial class LanguageEditor : PortalModuleBase
    {
        /// <summary>
        /// Identifies images in TreeView
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	07/10/2004	Created
        /// </history>
        private enum eImageType
        {
            Folder = 0,
            Page = 1
        }

        /// <summary>
        /// Initializes ResourceFile treeView
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	25/03/2006	Created
        /// </history>
        private void InitTree()
        {
            int idx;
            int idx2;

            // configure tree
            DNNTree.SystemImagesPath = ResolveUrl( "~/images/" );
            DNNTree.ImageList.Add( ResolveUrl( "~/images/folder.gif" ) );
            DNNTree.ImageList.Add( ResolveUrl( "~/images/file.gif" ) );
            DNNTree.IndentWidth = 10;
            DNNTree.CollapsedNodeImage = ResolveUrl( "~/images/max.gif" );
            DNNTree.ExpandedNodeImage = ResolveUrl( "~/images/min.gif" );

            //Local resources
            idx = DNNTree.TreeNodes.Add( "Local Resources" );
            DNNTree.TreeNodes[ idx ].Key = "Local Resources";
            DNNTree.TreeNodes[ idx ].ToolTip = "Local Resources";
            DNNTree.TreeNodes[idx].ImageIndex = (int)eImageType.Folder;
            DNNTree.TreeNodes[ idx ].ClickAction = eClickAction.Expand;

            //admin
            idx2 = DNNTree.TreeNodes[ idx ].TreeNodes.Add( "Admin" );
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].Key = "Admin";
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].ToolTip = "Admin";
            DNNTree.TreeNodes[idx].TreeNodes[idx2].ImageIndex = (int)eImageType.Folder;
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].ClickAction = eClickAction.Expand;
            PopulateTree( DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].TreeNodes, Server.MapPath( "~\\admin" ) );
            //controls
            idx2 = DNNTree.TreeNodes[ idx ].TreeNodes.Add( "Controls" );
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].Key = "Controls";
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].ToolTip = "Controls";
            DNNTree.TreeNodes[idx].TreeNodes[idx2].ImageIndex = (int)eImageType.Folder;
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].ClickAction = eClickAction.Expand;
            PopulateTree( DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].TreeNodes, Server.MapPath( "~\\controls" ) );
            //desktopmodules
            idx2 = DNNTree.TreeNodes[ idx ].TreeNodes.Add( "DesktopModules" );
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].Key = "DesktopModules";
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].ToolTip = "DesktopModules";
            DNNTree.TreeNodes[idx].TreeNodes[idx2].ImageIndex = (int)eImageType.Folder;
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].ClickAction = eClickAction.Expand;
            PopulateTree( DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].TreeNodes, Server.MapPath( "~\\desktopmodules" ) );
            //providers
            idx2 = DNNTree.TreeNodes[ idx ].TreeNodes.Add( "Providers" );
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].Key = "Providers";
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].ToolTip = "Providers";
            DNNTree.TreeNodes[idx].TreeNodes[idx2].ImageIndex = (int)eImageType.Folder;
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].ClickAction = eClickAction.Expand;
            PopulateTree( DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].TreeNodes, Server.MapPath( "~\\providers" ) );

            // add application resources
            idx = DNNTree.TreeNodes.Add( "Global Resources" );
            DNNTree.TreeNodes[ idx ].Key = "Global Resources";
            DNNTree.TreeNodes[ idx ].ToolTip = "Global Resources";
            DNNTree.TreeNodes[idx].ImageIndex = (int)eImageType.Folder;
            DNNTree.TreeNodes[ idx ].ClickAction = eClickAction.Expand;
            idx2 = DNNTree.TreeNodes[ idx ].TreeNodes.Add( Path.GetFileNameWithoutExtension( Localization.GlobalResourceFile ) );
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].Key = Server.MapPath( Localization.GlobalResourceFile );
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].ToolTip = DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].Text;
            DNNTree.TreeNodes[idx].TreeNodes[idx2].ImageIndex = (int)eImageType.Page;
            idx2 = DNNTree.TreeNodes[ idx ].TreeNodes.Add( Path.GetFileNameWithoutExtension( Localization.SharedResourceFile ) );
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].Key = Server.MapPath( Localization.SharedResourceFile );
            DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].ToolTip = DNNTree.TreeNodes[ idx ].TreeNodes[ idx2 ].Text;
            DNNTree.TreeNodes[idx].TreeNodes[idx2].ImageIndex = (int)eImageType.Page;
        }

        /// <summary>
        /// Loads Local Resource files in the tree
        /// </summary>
        /// <param name="Nodes">Node collection where to add new nodes</param>
        /// <param name="_path">Folder to search for</param>
        /// <returns>true if a Local Resource file is found in the given path</returns>
        /// <remarks>
        /// The Node collection will only contain en-US resources
        /// Only folders with Resource files will be included in the tree
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	07/10/2004	Created
        /// </history>
        private bool PopulateTree( TreeNodeCollection Nodes, string _path )
        {
            string[] folders = Directory.GetDirectories( _path );
            bool found = false;

            foreach( string folder in folders )
            {
                DirectoryInfo objFolder = new DirectoryInfo( folder );
                TreeNode node = new TreeNode( objFolder.Name );
                node.Key = objFolder.FullName;
                node.ToolTip = objFolder.Name;
                node.ImageIndex = (int)eImageType.Folder;
                node.ClickAction = eClickAction.Expand;
                Nodes.Add( node );

                if( objFolder.Name == Localization.LocalResourceDirectory )
                {
                    // found local resource folder, add resources
                    foreach( FileInfo objFile in objFolder.GetFiles( "*.ascx.resx" ) )
                    {
                        TreeNode leaf = new TreeNode( Path.GetFileNameWithoutExtension( objFile.Name ) );
                        leaf.Key = objFile.FullName;
                        leaf.ToolTip = objFile.Name;
                        leaf.ImageIndex = (int)eImageType.Page;
                        node.TreeNodes.Add( leaf );
                    }
                    foreach( FileInfo objFile in objFolder.GetFiles( "*.aspx.resx" ) )
                    {
                        TreeNode leaf = new TreeNode( Path.GetFileNameWithoutExtension( objFile.Name ) );
                        leaf.Key = objFile.FullName;
                        leaf.ToolTip = objFile.Name;
                        leaf.ImageIndex = (int)eImageType.Page;
                        node.TreeNodes.Add( leaf );
                    }
                    // add LocalSharedResources if found
                    if( File.Exists( Path.Combine( folder, Localization.LocalSharedResourceFile ) ) )
                    {
                        FileInfo objFile = new FileInfo( Path.Combine( folder, Localization.LocalSharedResourceFile ) );
                        TreeNode leaf = new TreeNode( Path.GetFileNameWithoutExtension( objFile.Name ) );
                        leaf.Key = objFile.FullName;
                        leaf.ToolTip = objFile.Name;
                        leaf.ImageIndex = (int)eImageType.Page;
                        node.TreeNodes.Add( leaf );
                    }
                    found = true;
                }
                else
                {
                    //recurse
                    if( PopulateTree( node.TreeNodes, folder ) )
                    {
                        // found resources
                        found = true;
                    }
                    else
                    {
                        // not found, remove node
                        Nodes.Remove( node );
                    }
                }
            }

            return found;
        }

        /// <summary>
        /// Loads suported locales
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        private void BindLocaleList()
        {
            DataSet ds = new DataSet();

            ds.ReadXml( Server.MapPath( Localization.SupportedLocalesFile ) );
            DataView dv = ds.Tables[ 0 ].DefaultView;
            dv.Sort = "name ASC";

            cboLocales.Items.Clear();
            for( int i = 0; i < dv.Count; i++ )
            {
                string localeKey = Convert.ToString( dv[ i ]["key"] );
                CultureInfo cinfo = new CultureInfo( localeKey );

                string localeName;
                try
                {
                    if( rbDisplay.SelectedValue == "Native" )
                    {
                        localeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase( cinfo.NativeName );
                    }
                    else
                    {
                        localeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase( cinfo.EnglishName );
                    }
                }
                catch
                {
                    localeName = Convert.ToString( dv[ i ]["name"] ) + " (" + localeKey + ")";
                }
                cboLocales.Items.Add( new ListItem( localeName, localeKey ) );
            }
        }

        /// <summary>
        /// Saves / Gets the selected resource file being edited in viewstate
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	07/10/2004	Created
        /// </history>
        private string SelectedResourceFile
        {
            get
            {
                return ViewState["SelectedResourceFile"].ToString();
            }
            set
            {
                ViewState["SelectedResourceFile"] = value;
                lblResourceFile.Text = value.Replace(Globals.ApplicationMapPath, "");
            }
        }

        /// <summary>
        /// Loads Resource information into the datagrid
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        ///     [vmasanas}  25/03/2006  Modified to support new features
        /// </history>
        private void BindGrid()
        {
            Hashtable EditTable;
            Hashtable DefaultTable;

            EditTable = LoadFile( rbMode.SelectedValue, "Edit" );
            DefaultTable = LoadFile( rbMode.SelectedValue, "Default" );

            // check edit table
            // if empty, just use default
            if( EditTable.Count == 0 )
            {
                EditTable = DefaultTable;
            }
            else
            {
                //remove obsolete keys
                ArrayList ToBeDeleted = new ArrayList();
                foreach( string key in EditTable.Keys )
                {
                    if( ! DefaultTable.Contains( key ) )
                    {
                        ToBeDeleted.Add( key );
                    }
                }
                if( ToBeDeleted.Count > 0 )
                {
                    UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Obsolete", LocalResourceFile ), ModuleMessageType.YellowWarning );
                    foreach( string key in ToBeDeleted )
                    {
                        EditTable.Remove( key );
                    }
                }

                //add missing keys
                foreach( string key in DefaultTable.Keys )
                {
                    if( ! EditTable.Contains( key ) )
                    {
                        EditTable.Add( key, DefaultTable[key] );
                    }
                    else
                    {
                        // Update default value
                        Pair p = (Pair)EditTable[key];
                        p.Second = ( (Pair)DefaultTable[key] ).First;
                        EditTable[key] = p;
                    }
                }
            }

            SortedList s = new SortedList( EditTable );

            dgEditor.DataSource = s;
            dgEditor.DataBind();
        }

        /// <summary>
        /// Returns the resource file name for a given resource and language
        /// </summary>
        /// <param name="language"></param>
        /// <param name="mode">Identifies the resource being searched (System, Host, Portal)</param>
        /// <returns>Localized File Name</returns>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// 	[vmasanas]	25/03/2006	Modified to support new host resources and incremental saving
        /// </history>
        private string ResourceFile( string language, string mode )
        {
            string resourcefilename = SelectedResourceFile;

            if( ! resourcefilename.EndsWith( ".resx" ) )
            {
                resourcefilename += ".resx";
            }

            if( language != Localization.SystemLocale )
            {
                resourcefilename = resourcefilename.Substring( 0, resourcefilename.Length - 5 ) + "." + language + ".resx";
            }

            if( mode == "Host" )
            {
                resourcefilename = resourcefilename.Substring( 0, resourcefilename.Length - 5 ) + "." + "Host.resx";
            }
            else if( mode == "Portal" )
            {
                resourcefilename = resourcefilename.Substring( 0, resourcefilename.Length - 5 ) + "." + "Portal-" + PortalId + ".resx";
            }

            return resourcefilename;
        }

        /// <summary>
        /// Loads resources from file
        /// </summary>
        /// <param name="mode">Active editor mode</param>
        /// <param name="type">Resource being loaded (edit or default)</param>
        /// <returns></returns>
        /// <remarks>
        /// Depending on the editor mode, resources will be overrided using default DNN schema.
        /// "Edit" resources will only load selected file.
        /// When loading "Default" resources (to be used on the editor as helpers) fallback resource
        /// chain will be used in order for the editor to be able to correctly see what
        /// is the current default value for the any key. This process depends on the current active
        /// editor mode:
        /// - System: when editing system base resources on en-US needs to be loaded
        /// - Host: base en-US, and base locale especific resource
        /// - Portal: base en-US, host override for en-US, base locale especific resource, and host override
        /// for locale
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	25/03/2006	Created
        /// </history>
        private Hashtable LoadFile( string mode, string type )
        {
            Hashtable ht = new Hashtable();

            switch( type )
            {
                case "Edit":

                    // Only load resources from the file being edited
                    string file = ResourceFile( cboLocales.SelectedValue, mode );
                    ht = LoadResource( ht, file );
                    break;
                case "Default":

                    // Load system default
                    file = ResourceFile( Localization.SystemLocale, "System" );
                    ht = LoadResource( ht, file );

                    switch( mode )
                    {
                        case "Host":

                            if( cboLocales.SelectedValue != Localization.SystemLocale )
                            {
                                // Load base file for selected locale
                                file = ResourceFile( cboLocales.SelectedValue, "System" );
                                ht = LoadResource( ht, file );
                            }
                            break;
                        case "Portal":

                            //Load host override for default locale
                            file = ResourceFile( Localization.SystemLocale, "Host" );
                            ht = LoadResource( ht, file );

                            if( cboLocales.SelectedValue != Localization.SystemLocale )
                            {
                                // Load base file for locale
                                file = ResourceFile( cboLocales.SelectedValue, "System" );
                                ht = LoadResource( ht, file );

                                //Load host override for selected locale
                                file = ResourceFile( cboLocales.SelectedValue, "Host" );
                                ht = LoadResource( ht, file );
                            }
                            break;
                    }
                    break;
            }

            return ht;
        }

        /// <summary>
        /// Loads resources from file into the HastTable
        /// </summary>
        /// <param name="ht">Current resources HashTable</param>
        /// <param name="filepath">Resources file</param>
        /// <returns>Base table updated with new resources </returns>
        /// <remarks>
        /// Returned hashtable uses resourcekey as key.
        /// Value contains a Pair object where:
        ///  First=>value to be edited
        ///  Second=>default value
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	25/03/2006	Created
        /// </history>
        private Hashtable LoadResource( Hashtable ht, string filepath )
        {
            XmlDocument d = new XmlDocument();
            bool xmlLoaded;
            try
            {
                d.Load( filepath );
                xmlLoaded = true;
            }
            catch //exc As Exception
            {
                xmlLoaded = false;
            }
            if( xmlLoaded )
            {                
                foreach( XmlNode n in d.SelectNodes( "root/data" ) )
                {
                    if( n.NodeType != XmlNodeType.Comment )
                    {
                        string val = n.SelectSingleNode( "value" ).InnerXml;
                        if( ht[n.Attributes["name"].Value] == null )
                        {
                            ht.Add( n.Attributes["name"].Value, new Pair( val, val ) );
                        }
                        else
                        {
                            ht[n.Attributes["name"].Value] = new Pair( val, val );
                        }
                    }
                }
            }
            return ht;
        }

        /// <summary>
        /// Configures the initial visibility status of the default label
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Vicenç]	26/03/2006	Created
        /// </history>
        protected bool ExpandDefault( Pair p )
        {
            return p.Second.ToString().Length < 150;
        }

        /// <summary>
        /// Builds the url for the lang. html editor control
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	07/10/2004	Created
        /// </history>
        protected string OpenFullEditor( string name )
        {
            string file;
            file = SelectedResourceFile.Replace( Server.MapPath( Globals.ApplicationPath + "/" ), "" );
            return EditUrl( "name", name, "fulleditor", "locale=" + cboLocales.SelectedValue, "resourcefile=" + Globals.QueryStringEncode( file ), "mode=" + rbMode.SelectedValue );
        }

        /// <summary>
        /// Loads suported locales and shows default values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// 	[vmasanas]	25/03/2006	Modified to support new host resources and incremental saving
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ! Page.IsPostBack )
                {
                    ClientAPI.AddButtonConfirm( cmdDelete, Localization.GetString( "DeleteItem" ) );

                    // initialized available locales
                    BindLocaleList();

                    // init tree
                    InitTree();

                    // If returning from full editor, use params
                    // else load system global resource file by default
                    if( Request.QueryString["locale"] != "" )
                    {
                        cboLocales.SelectedValue = Request.QueryString["locale"].ToString();
                    }
                    else
                    {
                        cboLocales.SelectedValue = Localization.SystemLocale;
                    }
                    if( PortalSettings.ActiveTab.ParentId == PortalSettings.AdminTabId )
                    {
                        rbMode.SelectedValue = "Portal";
                        rowMode.Visible = false;
                    }
                    else
                    {
                        // portal mode only available on admin menu
                        rbMode.Items.RemoveAt( 2 );
                        string mode = Request.QueryString["mode"];
                        if( !String.IsNullOrEmpty(mode) && rbMode.Items.FindByValue( mode ) != null )
                        {
                            rbMode.SelectedValue = mode;
                        }
                    }
                    if( Request.QueryString["resourcefile"] != "" )
                    {
                        SelectedResourceFile = Server.MapPath( "~/" + Globals.QueryStringDecode( Request.QueryString["resourcefile"] ) );
                    }
                    else
                    {
                        SelectedResourceFile = Server.MapPath( Localization.GlobalResourceFile );
                    }
                    DNNTree.SelectNodeByKey( SelectedResourceFile );

                    BindGrid();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Loads localized file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// If a localized file does not exist for the selected language it is created using default values
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// 	[vmasanas]	25/03/2006	Modified to support new host resources and incremental saving
        /// </history>
        protected void cboLocales_SelectedIndexChanged( Object sender, EventArgs e )
        {
            try
            {
                BindGrid();
            }
            catch
            {
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
            }
        }

        /// <summary>
        /// Rebinds the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	25/03/2006	Created
        /// </history>
        protected void rbMode_SelectedIndexChanged( object sender, EventArgs e )
        {
            try
            {
                BindGrid();
            }
            catch
            {
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
            }
        }

        /// <summary>
        /// Rebinds the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	25/03/2006	Created
        /// </history>
        protected void chkHighlight_CheckedChanged( Object sender, EventArgs e )
        {
            try
            {
                BindGrid();
            }
            catch( Exception ) //Module failed to load
            {
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
            }
        }

        /// <summary>
        /// Open the selected resource file in editor or expand/collapse node if is folder
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	07/10/2004	Created
        /// </history>
        internal void DNNTree_NodeClick( object source, DNNTreeNodeClickEventArgs e )
        {
            try
            {
                if( e.Node.ImageIndex == (int)eImageType.Page )
                {
                    SelectedResourceFile = e.Node.Key;
                    try
                    {
                        BindGrid();
                    }
                    catch
                    {
                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                    }
                }
                else if( e.Node.IsExpanded )
                {
                    e.Node.Collapse();
                }
                else
                {
                    e.Node.Expand();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Updates all values from the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// 	[vmasanas]	25/03/2006	Modified to support new host resources and incremental saving
        /// </history>
        protected void cmdUpdate_Click( Object sender, EventArgs e )
        {
            XmlDocument resDoc = new XmlDocument();
            XmlDocument defDoc = new XmlDocument();

            try
            {
                string filename = ResourceFile( cboLocales.SelectedValue, rbMode.SelectedValue );
                if( ! File.Exists( filename ) )
                {
                    // load system default
                    resDoc.Load( ResourceFile( Localization.SystemLocale, "System" ) );
                }
                else
                {
                    resDoc.Load( filename );
                }
                defDoc.Load( ResourceFile( Localization.SystemLocale, "System" ) );

                XmlNode nodeData;
                XmlAttribute attr;
                switch( rbMode.SelectedValue )
                {
                    case "System":

                        // this will save all items
                        foreach( DataGridItem di in dgEditor.Items )
                        {
                            if( di.ItemType == ListItemType.Item || di.ItemType == ListItemType.AlternatingItem )
                            {
                                TextBox ctl1 = (TextBox)di.Cells[ 0 ].FindControl( "txtValue" );
                                XmlNode node = resDoc.SelectSingleNode( "//root/data[@name='" + di.Cells[ 1 ].Text + "']/value" );
                                if( node == null )
                                {
                                    // missing entry
                                    nodeData = resDoc.CreateElement( "data" );
                                    attr = resDoc.CreateAttribute( "name" );
                                    attr.Value = di.Cells[ 1 ].Text;
                                    nodeData.Attributes.Append( attr );
                                    resDoc.SelectSingleNode( "//root" ).AppendChild( nodeData );

                                    node = nodeData.AppendChild( resDoc.CreateElement( "value" ) );
                                }
                                node.InnerXml = Server.HtmlEncode( ctl1.Text );
                            }
                        }
                        break;
                    case "Host":
                        // only items different from default will be saved
                        foreach( DataGridItem di in dgEditor.Items )
                        {
                            if( di.ItemType == ListItemType.Item || di.ItemType == ListItemType.AlternatingItem )
                            {
                                TextBox ctl1 = (TextBox)di.Cells[ 0 ].FindControl( "txtValue" );
                                Label ctl2 = (Label)di.Cells[ 0 ].FindControl( "lblDefault" );

                                XmlNode node = resDoc.SelectSingleNode( "//root/data[@name='" + di.Cells[ 1 ].Text + "']/value" );
                                if( ctl1.Text != ctl2.Text )
                                {
                                    if( node == null )
                                    {
                                        // missing entry
                                        nodeData = resDoc.CreateElement( "data" );
                                        attr = resDoc.CreateAttribute( "name" );
                                        attr.Value = di.Cells[ 1 ].Text;
                                        nodeData.Attributes.Append( attr );
                                        resDoc.SelectSingleNode( "//root" ).AppendChild( nodeData );

                                        node = nodeData.AppendChild( resDoc.CreateElement( "value" ) );
                                    }
                                    node.InnerXml = Server.HtmlEncode( ctl1.Text );
                                }
                                else if( node != null )
                                {
                                    // remove item = default
                                    resDoc.SelectSingleNode( "//root" ).RemoveChild( node.ParentNode );
                                }
                            }
                        }
                        break;

                    case "Portal":

                        // only items different from default will be saved
                        foreach( DataGridItem di in dgEditor.Items )
                        {
                            if( di.ItemType == ListItemType.Item || di.ItemType == ListItemType.AlternatingItem )
                            {
                                TextBox ctl1 = (TextBox)di.Cells[ 0 ].FindControl( "txtValue" );
                                Label ctl2 = (Label)di.Cells[ 0 ].FindControl( "lblDefault" );

                                XmlNode node = resDoc.SelectSingleNode( "//root/data[@name='" + di.Cells[ 1 ].Text + "']/value" );
                                if( ctl1.Text != ctl2.Text )
                                {
                                    if( node == null )
                                    {
                                        // missing entry
                                        nodeData = resDoc.CreateElement( "data" );
                                        attr = resDoc.CreateAttribute( "name" );
                                        attr.Value = di.Cells[ 1 ].Text;
                                        nodeData.Attributes.Append( attr );
                                        resDoc.SelectSingleNode( "//root" ).AppendChild( nodeData );

                                        node = nodeData.AppendChild( resDoc.CreateElement( "value" ) );
                                    }
                                    node.InnerXml = Server.HtmlEncode( ctl1.Text );
                                }
                                else if( node != null )
                                {
                                    // remove item = default
                                    resDoc.SelectSingleNode( "//root" ).RemoveChild( node.ParentNode );
                                }
                            }
                        }
                        break;
                }

                // remove obsolete keys
                XmlNode parent;
                foreach( XmlNode node in resDoc.SelectNodes( "//root/data" ) )
                {
                    if( defDoc.SelectSingleNode( "//root/data[@name='" + node.Attributes["name"].Value + "']" ) == null )
                    {
                        parent = node.ParentNode;
                        parent.RemoveChild( node );
                    }
                }
                // remove duplicate keys
                foreach( XmlNode node in resDoc.SelectNodes( "//root/data" ) )
                {
                    if( resDoc.SelectNodes( "//root/data[@name='" + node.Attributes["name"].Value + "']" ).Count > 1 )
                    {
                        parent = node.ParentNode;
                        parent.RemoveChild( node );
                    }
                }

                switch( rbMode.SelectedValue )
                {
                    case "System":

                        resDoc.Save( filename );
                        break;
                    case "Host":
                        if( resDoc.SelectNodes( "//root/data" ).Count > 0 )
                        {
                            // there's something to save
                            resDoc.Save( filename );
                        }
                        else
                        {
                            // nothing to be saved, if file exists delete
                            if( File.Exists( filename ) )
                            {
                                File.Delete( filename );
                            }
                        }
                        break;

                    case "Portal":

                        if( resDoc.SelectNodes( "//root/data" ).Count > 0 )
                        {
                            // there's something to save
                            resDoc.Save( filename );
                        }
                        else
                        {
                            // nothing to be saved, if file exists delete
                            if( File.Exists( filename ) )
                            {
                                File.Delete( filename );
                            }
                        }
                        break;
                }
                BindGrid();
            }
            catch( Exception ) //Module failed to load
            {
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
            }
        }

        /// <summary>
        /// Deletes the localized file for a given locale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// System Default file cannot be deleted
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// 	[vmasanas]	25/03/2006	Modified to support new host resources and incremental saving
        /// </history>
        protected void cmdDelete_Click( Object sender, EventArgs e )
        {
            try
            {
                if( cboLocales.SelectedValue == Localization.SystemLocale && rbMode.SelectedValue == "System" )
                {
                    UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Delete.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                }
                else
                {
                    try
                    {
                        if( File.Exists( ResourceFile( cboLocales.SelectedValue, rbMode.SelectedValue ) ) )
                        {
                            File.SetAttributes( ResourceFile( cboLocales.SelectedValue, rbMode.SelectedValue ), FileAttributes.Normal );
                            File.Delete( ResourceFile( cboLocales.SelectedValue, rbMode.SelectedValue ) );
                            UI.Skins.Skin.AddModuleMessage( this, string.Format( Localization.GetString( "Deleted", this.LocalResourceFile ), ResourceFile( cboLocales.SelectedValue, rbMode.SelectedValue ), null ), ModuleMessageType.GreenSuccess );

                            cboLocales.SelectedValue = Localization.SystemLocale;
                            BindGrid();
                        }
                    }
                    catch
                    {
                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Binds a data item to the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Adds a warning message before leaving the page to edit in full editor so the user can save changes
        /// Customizes edit textbox and default value
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	20/10/2004	Created
        /// 	[vmasanas]	25/03/2006	Modified to support new host resources and incremental saving
        /// </history>
        protected void dgEditor_ItemDataBound( object sender, DataGridItemEventArgs e )
        {
            try
            {
                if( e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item )
                {
                    HyperLink c;
                    c = (HyperLink)e.Item.FindControl( "lnkEdit" );
                    if( c != null )
                    {
                        ClientAPI.AddButtonConfirm( c, Localization.GetString( "SaveWarning", this.LocalResourceFile ) );
                    }

                    Pair p = (Pair)( ( (DictionaryEntry)e.Item.DataItem ).Value );

                    TextBox t;
                    t = (TextBox)e.Item.FindControl( "txtValue" );
                    if( p.First.ToString() == p.Second.ToString() && chkHighlight.Checked && p.Second.ToString() != "" )
                    {
                        t.CssClass = "Pending";
                    }
                    if( p.First.ToString().Length > 30 )
                    {
                        t.Height = new Unit( "100" );
                    }
                    t.Text = Server.HtmlDecode( p.First.ToString() );

                    Label l;
                    l = (Label)e.Item.FindControl( "lblDefault" );
                    l.Text = Server.HtmlDecode(p.Second.ToString());                    
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Returns to main control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        protected void cmdCancel_Click( Object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Globals.NavigateURL() );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rbDisplay_SelectedIndexChanged( Object sender, EventArgs e )
        {
            BindLocaleList();
        }
    }
}