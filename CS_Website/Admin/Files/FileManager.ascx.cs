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
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;
using ICSharpCode.SharpZipLib.Zip;
using DataCache=DotNetNuke.Common.Utilities.DataCache;
using FileInfo=System.IO.FileInfo;
using Globals=DotNetNuke.Common.Globals;
using TreeNode=DotNetNuke.UI.WebControls.TreeNode;
using TreeNodeCollection=DotNetNuke.UI.WebControls.TreeNodeCollection;

namespace DotNetNuke.Modules.Admin.FileSystem
{
    /// <summary>
    /// Supplies the functionality for uploading files to the Portal
    /// Synchronizing Files within the folder and the database
    /// and Provides status of available disk space for the portal
    /// as well as limiting uploads to the restricted allocated file space
    /// </summary>
    /// <history>
    /// 	[DYNST]	        2/1/2004	Created
    ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
    ///     [cnurse]        12/2/2004   Database Synchronization added
    /// </history>
    public partial class FileManager : PortalModuleBase
    {
        protected Label Label7;
        protected Panel pnlCopyButton;
        protected Panel pnlEditButtons;

        private enum eImageType
        {
            Folder = 0,
            SecureFolder = 1,
            DatabaseFolder = 2,
            Page = 3
        }

        private bool _DisplayingMessage;
        private string m_strParentFolderName;
        private string imageDirectory = "~/images/FileManager/Icons/";
        private string _ErrorMessage = "<TABLE><TR><TD height=100% class=NormalRed>{0}</TD></TR><TR valign=bottom><TD align=center><INPUT id=btnClearError onclick=clearErrorMessage(); type=button value=OK></TD></TR></TABLE>";

        public int FolderPortalID
        {
            get
            {
                if( IsHostMenu )
                {
                    return Null.NullInteger;
                }
                else
                {
                    return PortalId;
                }
            }
        }

        public string RootFolderName
        {
            get
            {
                if( ViewState["RootFolderName"] != null )
                {
                    return ViewState["RootFolderName"].ToString();
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ViewState["RootFolderName"] = value;
            }
        }

        public string RootFolderPath
        {
            get
            {
                string _CurRootFolder;
                if( IsHostMenu )
                {
                    _CurRootFolder = Globals.HostMapPath;
                }
                else
                {
                    _CurRootFolder = PortalSettings.HomeDirectoryMapPath;
                }
                return _CurRootFolder;
            }
        }

        public string Sort
        {
            get
            {
                return ViewState["strSort"].ToString();
            }
            set
            {
                ViewState.Add( "strSort", value );
            }
        }

        public string LastSort
        {
            get
            {
                return ViewState["strLastSort"].ToString();
            }
            set
            {
                ViewState.Add( "strLastSort", value );
            }
        }

        public string FilterFiles
        {
            get
            {
                return ViewState["strFilterFiles"].ToString();
            }
            set
            {
                ViewState.Add( "strFilterFiles", value );
            }
        }

        public string LastPath
        {
            get
            {
                return UnMaskPath( ClientAPI.GetClientVariable( Page, "LastPath" ) );
            }
            set
            {
                value = MaskPath( value );
                ClientAPI.RegisterClientVariable( Page, "LastPath", value, true );
            }
        }

        public string DestPath
        {
            get
            {
                return ClientAPI.GetClientVariable( Page, "DestPath" );
            }
            set
            {
                ClientAPI.RegisterClientVariable( Page, "DestPath", value, true );
            }
        }

        public string SourcePath
        {
            get
            {
                return ClientAPI.GetClientVariable( Page, "SourcePath" );
            }
            set
            {
                ClientAPI.RegisterClientVariable( Page, "SourcePath", value, true );
            }
        }

        public string MoveFiles
        {
            get
            {
                return ClientAPI.GetClientVariable( Page, "MoveFiles" );
            }
            set
            {
                ClientAPI.RegisterClientVariable( Page, "MoveFiles", value, true );
            }
        }

        public bool IsRefresh
        {
            get
            {
                return Convert.ToBoolean( ClientAPI.GetClientVariable( Page, "IsRefresh" ) );
            }
            set
            {
                ClientAPI.RegisterClientVariable( Page, "IsRefresh", Convert.ToInt32( value ).ToString(), true );
            }
        }

        public bool DisabledButtons
        {
            get
            {
                return Convert.ToBoolean( ClientAPI.GetClientVariable( Page, "DisabledButtons" ) );
            }
            set
            {
                ClientAPI.RegisterClientVariable( Page, "DisabledButtons", Convert.ToInt32( value ).ToString(), true );
            }
        }

        public string MoveStatus
        {
            get
            {
                return ClientAPI.GetClientVariable( Page, "MoveStatus" );
            }
            set
            {
                ClientAPI.RegisterClientVariable( Page, "MoveStatus", value, true );
            }
        }

        public string LastFolderPath
        {
            get
            {
                if( ViewState["LastFolderPath"] != null )
                {
                    return ViewState["LastFolderPath"].ToString();
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ViewState["LastFolderPath"] = value;
            }
        }

        public int PageSize
        {
            get
            {
                return Convert.ToInt32( selPageSize.SelectedValue );
            }
        }

        public int PageIndex
        {
            get
            {
                if( ViewState["PageIndex"] != null )
                {
                    return Convert.ToInt32( ViewState["PageIndex"] );
                }
                return 0;
            }
            set
            {
                if( value >= 0 && value < dgFileList.PageCount )
                {
                    ViewState["PageIndex"] = value;
                }
            }
        }

        /// <summary>
        /// Adds a File to the DataTable used for the File List grid
        /// </summary>
        /// <param name="tblFiles">The DataTable</param>
        /// <param name="objFile">The FileInfo object to add</param>
        /// <history>
        /// 	[cnurse]	    12/3/2004	documented
        ///     [cnurse]        04/24/2006  Updated to use new Secure Storage
        /// </history>
        private void AddFileToTable( DataTable tblFiles, Services.FileSystem.FileInfo objFile )
        {
            DataRow dRow;
            dRow = tblFiles.NewRow();
            dRow["FileType"] = "File";
            dRow["FileId"] = objFile.FileId;
            dRow["FileName"] = objFile.FileName;
            dRow["FileSize"] = objFile.Size.ToString( "##,##0" );
            dRow["IntFileSize"] = objFile.Size;
            if( !String.IsNullOrEmpty(objFile.Extension) )
            {
                dRow["Extension"] = objFile.Extension;
            }
            else
            {
                dRow["Extension"] = "none";
            }
            dRow["StorageLocation"] = objFile.StorageLocation;

            if( objFile.StorageLocation == (int)FolderController.StorageLocationTypes.InsecureFileSystem )
            {
                string strSourcePath = UnMaskPath( DestPath );
                FileInfo fsFile = new FileInfo( strSourcePath + objFile.FileName );
                dRow["DateModified"] = fsFile.LastWriteTime;
                dRow["Archive"] = fsFile.Attributes & FileAttributes.Archive;
                dRow["ReadOnly"] = fsFile.Attributes & FileAttributes.ReadOnly;
                dRow["Hidden"] = fsFile.Attributes & FileAttributes.Hidden;
                dRow["System"] = fsFile.Attributes & FileAttributes.System;
                dRow["AttributeString"] = GetAttributeString( fsFile.Attributes );
            }
            else if( objFile.StorageLocation == (int)FolderController.StorageLocationTypes.SecureFileSystem )
            {
                string strSourcePath = UnMaskPath( DestPath );
                FileInfo fsFile = new FileInfo( strSourcePath + objFile.FileName + Globals.glbProtectedExtension );
                dRow["DateModified"] = fsFile.LastWriteTime;
                dRow["Archive"] = fsFile.Attributes & FileAttributes.Archive;
                dRow["ReadOnly"] = fsFile.Attributes & FileAttributes.ReadOnly;
                dRow["Hidden"] = fsFile.Attributes & FileAttributes.Hidden;
                dRow["System"] = fsFile.Attributes & FileAttributes.System;
                dRow["AttributeString"] = GetAttributeString( fsFile.Attributes );
            }
            else if( objFile.StorageLocation == (int)FolderController.StorageLocationTypes.DatabaseSecure )
            {
                dRow["Archive"] = false;
                dRow["ReadOnly"] = false;
                dRow["Hidden"] = false;
                dRow["System"] = false;
                dRow["AttributeString"] = "";
            }
            tblFiles.Rows.Add( dRow );
        }

        /// <summary>
        /// Adds node to tree
        /// </summary>
        /// <param name="strName">Name of folder to display</param>
        /// <param name="strKey">Masked Key of folder location</param>
        /// <param name="eImage">Type of image</param>
        /// <param name="objNodes">Node collection to add to</param>
        /// <history>
        /// 	[Jon Henning]	10/26/2004	Created
        /// 	[Jon Henning]	8/24/2005	Added Populate on Demand (POD) logic
        /// </history>
        private TreeNode AddNode( string strName, string strKey, eImageType eImage, TreeNodeCollection objNodes )
        {
            TreeNode objNode;
            objNode = new TreeNode( strName );
            objNode.Key = strKey;
            objNode.ToolTip = strName;
            objNode.ImageIndex = (int)eImage;
            objNode.CssClass = "FileManagerTreeNode";
            objNodes.Add( objNode );

            if( objNode.Key == DestPath )
            {
                objNode.Selected = true;
                objNode.MakeNodeVisible();
            }

            return objNode;
        }

        /// <summary>
        /// Adds node to tree
        /// </summary>
        /// <param name="folder">The FolderInfo object to add</param>
        /// <param name="objNodes">Node collection to add to</param>
        /// <returns></returns>
        /// <history>
        /// 	[cnurse]	04/24/2006	Created
        /// </history>
        private TreeNode AddNode( FolderInfo folder, TreeNodeCollection objNodes )
        {
            TreeNode objNode;
            string strName = folder.FolderName;
            string strKey = MaskPath( RootFolderPath + folder.FolderPath );
            ArrayList subFolders = FileSystemUtils.GetFoldersByParentFolder( FolderPortalID, folder.FolderPath );
            eImageType image = eImageType.Folder;
            if( folder.StorageLocation == (int)FolderController.StorageLocationTypes.InsecureFileSystem )
            {
                image = eImageType.Folder;
            }
            else if( folder.StorageLocation == (int)FolderController.StorageLocationTypes.SecureFileSystem )
            {
                image = eImageType.SecureFolder;
            }
            else if( folder.StorageLocation == (int)FolderController.StorageLocationTypes.DatabaseSecure )
            {
                image = eImageType.DatabaseFolder;
            }
            objNode = AddNode( strName, strKey, image, objNodes );
            objNode.HasNodes = subFolders.Count > 0;

            return objNode;
        }

        /// <summary>
        /// BindFileList
        /// </summary>
        /// <history>
        /// 	[Jon Henning]	11/1/2004	Created
        /// </history>
        private void BindFileList()
        {
            LastPath = FileSystemUtils.RemoveTrailingSlash( UnMaskPath( DestPath ) );
            dgFileList.PageSize = PageSize;
            dgFileList.CurrentPageIndex = PageIndex;

            GetFilesByFolder( FileSystemUtils.StripFolderPath( DestPath ).Replace( "\\", "/" ) );

            if( dgFileList.PageCount > 1 )
            {
                tblMessagePager.Visible = true;
                string strCurPage = Localization.GetString( "Pages" );
                lblCurPage.Text = string.Format( strCurPage, ( dgFileList.CurrentPageIndex + 1 ), ( dgFileList.PageCount ) );
                lnkMoveFirst.Text = "<img border=0 Alt='" + Localization.GetString( "First" ) + "' src='" + ResolveUrl( "~/images/FileManager/movefirst.gif" ) + "'>";
                lnkMovePrevious.Text = "<img border=0 Alt='" + Localization.GetString( "Previous" ) + "' src='" + ResolveUrl( "~/images/FileManager/moveprevious.gif" ) + "'>";
                lnkMoveNext.Text = "<img border=0 Alt='" + Localization.GetString( "Next" ) + "' src='" + ResolveUrl( "~/images/FileManager/movenext.gif" ) + "'>";
                lnkMoveLast.Text = "<img border=0 Alt='" + Localization.GetString( "Last" ) + "' src='" + ResolveUrl( "~/images/FileManager/movelast.gif" ) + "'>";
            }
            else
            {
                tblMessagePager.Visible = false;
            }

            lblCurFolder.Text = DestPath.Replace( "0\\", RootFolderName + "\\" );
            MoveFiles = "";

            UpdateSpaceUsed();
        }

        /// <history>
        /// 	[cpaterra]	4/6/2006	Created
        /// </history>
        private void BindStorageLocationTypes()
        {
            ddlStorageLocation.Items.Add( new ListItem( Localization.GetString( "InsecureFileSystem", this.LocalResourceFile ), "0" ) );
            ddlStorageLocation.Items.Add( new ListItem( Localization.GetString( "SecureFileSystem", this.LocalResourceFile ), "1" ) );
            ddlStorageLocation.Items.Add( new ListItem( Localization.GetString( "SecureDatabase", this.LocalResourceFile ), "2" ) );
        }

        /// <summary>
        /// The BindFolderTree helper method is used to bind the list of
        /// files for this portal or for the hostfolder, to an asp:DATAGRID server control
        /// </summary>
        /// <history>
        /// 	[DYNST]	        2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ///     [cnurse]        12/2/2004   Updated to use Localization for Root
        /// 	[Jon Henning]	8/24/2005	Added Populate on Demand (POD) logic
        ///     [cnurse]        04/24/2006  Updated to use new Secure Storage
        /// </history>
        private void BindFolderTree()
        {
            TreeNode objNode;

            //Clear the Tree Nodes Collection
            DNNTree.TreeNodes.Clear();

            objNode = AddNode( RootFolderName, MaskPath( RootFolderPath ), eImageType.Folder, DNNTree.TreeNodes );

            ArrayList arrFolders = FileSystemUtils.GetFolders( FolderPortalID );
            objNode.HasNodes = arrFolders.Count > 1;
            if( this.DNNTree.PopulateNodesFromClient == false || this.DNNTree.IsDownLevel )
            {
                PopulateTree( objNode.TreeNodes, RootFolderPath );
            }

            if( DNNTree.SelectedTreeNodes.Count == 0 )
            {
                objNode.Selected = true;
            }
        }

        /// <summary>
        /// The CheckDestFolderAccess helper method Checks to make sure file copy/move
        /// operation will not exceed portal available space
        /// </summary>
        /// <history>
        /// 	[DYNST]	        2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ///     [cnurse]        12/2/2004   Updated to use Localization
        /// </history>
        public string CheckDestFolderAccess( long intSize )
        {
            if( Request.IsAuthenticated )
            {
                FileSystemUtils.AddTrailingSlash( UnMaskPath( DestPath ) );
                PortalController objPortalController = new PortalController();

                if( objPortalController.HasSpaceAvailable( FolderPortalID, intSize ) || ( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId ) )
                {
                    return "";
                }
                else
                {
                    return Localization.GetString( "NotEnoughSpace", this.LocalResourceFile );
                }
            }
            else
            {
                return Localization.GetString( "PleaseLogin", this.LocalResourceFile );
            }
        }

        /// <summary>
        /// GetCheckAllString
        /// </summary>
        /// <history>
        /// 	[Jon Henning]	11/1/2004	Created
        /// </history>
        private string GetCheckAllString()
        {
            int intCount = dgFileList.Items.Count;

            int i;
            string strResult;
            strResult = "setMoveFiles('');" + "\r\n";
            for( i = 0; i <= intCount - 1; i++ )
            {
                CheckBox chkFile = (CheckBox)dgFileList.Items[i].FindControl( "chkFile" );
                if( chkFile != null )
                {
                    strResult = strResult + "var chk1 = dnn.dom.getById('" + chkFile.ClientID + "');";
                    strResult = strResult + "chk1.checked = blValue;" + "\r\n";
                    strResult = strResult + "if (!chk1.onclick) {chk1.parentElement.onclick();}else{chk1.onclick();}" + "\r\n";
                }
            }
            strResult = "function CheckAllFiles(blValue) {" + strResult + "}" + "\r\n";

            strResult = "<script language=javascript>" + strResult + "</script>";

            return strResult;
        }

        /// <summary>
        /// The DeleteFiles helper method is used to delete the files in the list
        /// </summary>
        /// <param name="strFiles">The list of files to delete</param>
        /// <history>
        /// 	[DYNST]	        2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void DeleteFiles( string strFiles )
        {
            string[] arFiles = strFiles.Split( ';' );
            if( arFiles.Length == 0 )
            {
                return;
            }
            string strSourcePath;
            string strErrorMessage = "";
            strSourcePath = FileSystemUtils.AddTrailingSlash( LastPath );

            for( int i = 0; i < arFiles.Length; i++ )
            {
                if( arFiles[i] != "" )
                {
                    string strCurError = FileSystemUtils.DeleteFile( strSourcePath + arFiles[i], PortalSettings, false );
                    if( !String.IsNullOrEmpty(strCurError) )
                    {
                        strErrorMessage = strErrorMessage + Localization.GetString( "ErrorDeletingFile", this.LocalResourceFile ) + FileSystemUtils.AddTrailingSlash( UnMaskPath( DestPath ) ) + arFiles[i] + "<BR>&nbsp;&nbsp;&nbsp;" + strCurError + "<BR>";
                    }
                }
            }

            if( !String.IsNullOrEmpty(strErrorMessage) )
            {
                strErrorMessage = MaskString( strErrorMessage );
                ShowErrorMessage( strErrorMessage );
            }

            BindFileList();
        }

        /// <summary>
        /// GeneratePermissionsGrid generates the permissions grid for the folder
        /// </summary>
        /// <history>
        ///     [cnurse]        12/2/2004   documented
        /// </history>
        private void GeneratePermissionsGrid()
        {
            string folderPath = FileSystemUtils.StripFolderPath( DestPath ).Replace( "\\", "/" );

            dgPermissions.FolderPath = folderPath;
            if( ! IsHostMenu )
            {
                dgPermissions.DataBind();
            }

            FolderController objFolderController = new FolderController();
            FolderInfo objFolderInfo = objFolderController.GetFolder( FolderPortalID, folderPath );

            if( objFolderInfo != null )
            {
                ddlStorageLocation.SelectedValue = Convert.ToString( objFolderInfo.StorageLocation );
            }
        }

        /// <summary>
        /// GetAttributeString generates the attributes string from the FileAttributes
        /// </summary>
        /// <history>
        ///     [cnurse]        12/2/2004   documented
        /// </history>
        private string GetAttributeString( FileAttributes attributes )
        {
            string strResult = "";
            if( ( attributes & FileAttributes.Archive ) == FileAttributes.Archive )
            {
                strResult += "A";
            }
            if( ( attributes & FileAttributes.ReadOnly ) == FileAttributes.ReadOnly )
            {
                strResult += "R";
            }
            if( ( attributes & FileAttributes.Hidden ) == FileAttributes.Hidden )
            {
                strResult += "H";
            }
            if( ( attributes & FileAttributes.System ) == FileAttributes.System )
            {
                strResult += "S";
            }
            return strResult;
        }

        /// <summary>
        /// GetFilesByFolder gets the Files/Folders to display
        /// </summary>
        /// <history>
        ///     [cnurse]        12/2/2004   documented and modified to display Folders in
        ///                                 the grid
        ///     [cnurse]        04/24/2006  Updated to use new Secure Storage
        /// </history>
        private void GetFilesByFolder(string strFolderName)
        {

            DataTable tblFiles = GetFileTable();

            FolderInfo objFolder = FileSystemUtils.GetFolder(FolderPortalID, strFolderName);
            if (objFolder != null)
            {
                ArrayList arrFiles = FileSystemUtils.GetFilesByFolder(FolderPortalID, objFolder.FolderID);
                foreach (DotNetNuke.Services.FileSystem.FileInfo objFile in arrFiles)
                {
                    AddFileToTable(tblFiles, objFile);
                }
            }

            DataView dv = new DataView();
            dv.Table = tblFiles;
            dv.Sort = Sort;
            if (FilterFiles != "")
            {
                dv.RowFilter = "FileName like '%" + this.FilterFiles + "%'";
            }

            dgFileList.DataSource = dv;
            dgFileList.DataBind();

        }

        /// <summary>
        /// GetFileTable creates the DataTable used to store the list of files and folders
        /// </summary>
        /// <history>
        ///     [cnurse]        12/3/2004   documented and modified to display Folders in
        ///                                 the grid
        /// </history>
        private DataTable GetFileTable()
        {
            DataTable tblFiles = new DataTable( "Files" );

            DataColumn myColumns = new DataColumn();
            myColumns.DataType = Type.GetType( "System.String" );
            myColumns.ColumnName = "FileType";
            tblFiles.Columns.Add( myColumns );

            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType( "System.Int32" );
            myColumns.ColumnName = "FileId";
            tblFiles.Columns.Add( myColumns );

            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType( "System.String" );
            myColumns.ColumnName = "FileName";
            tblFiles.Columns.Add( myColumns );

            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType( "System.String" );
            myColumns.ColumnName = "FileSize";
            tblFiles.Columns.Add( myColumns );

            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType( "System.Int32" );
            myColumns.ColumnName = "IntFileSize";
            tblFiles.Columns.Add( myColumns );

            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType( "System.Int32" );
            myColumns.ColumnName = "StorageLocation";
            tblFiles.Columns.Add( myColumns );

            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType( "System.DateTime" );
            myColumns.ColumnName = "DateModified";
            tblFiles.Columns.Add( myColumns );

            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType( "System.Boolean" );
            myColumns.ColumnName = "ReadOnly";
            tblFiles.Columns.Add( myColumns );

            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType( "System.Boolean" );
            myColumns.ColumnName = "Hidden";
            tblFiles.Columns.Add( myColumns );

            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType( "System.Boolean" );
            myColumns.ColumnName = "System";
            tblFiles.Columns.Add( myColumns );

            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType( "System.Boolean" );
            myColumns.ColumnName = "Archive";
            tblFiles.Columns.Add( myColumns );

            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType( "System.String" );
            myColumns.ColumnName = "AttributeString";
            tblFiles.Columns.Add( myColumns );

            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType( "System.String" );
            myColumns.ColumnName = "Extension";
            tblFiles.Columns.Add( myColumns );

            return tblFiles;
        }

        /// <summary>
        /// Gets the Image associated with the File/Folder
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/4/2004	Created
        /// </history>
        public string GetImageUrl( string type )
        {
            string url = "";

            try
            {
                if( type == "folder" )
                {
                    url = imageDirectory + "ClosedFolder.gif";
                }
                else
                {
                    if( !String.IsNullOrEmpty(type) && File.Exists( Server.MapPath( imageDirectory + type + ".gif" ) ) )
                    {
                        url = imageDirectory + type + ".gif";
                    }
                    else
                    {
                        url = imageDirectory + "File.gif";
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return url;
        }

        /// <summary>
        /// Gets the size of the all the files in the zip file
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/4/2004	Created
        /// </history>
        private long GetZipFileExtractSize( string strFileName )
        {
            ZipEntry objZipEntry;
            ZipInputStream objZipInputStream;
            try
            {
                objZipInputStream = new ZipInputStream( File.OpenRead( strFileName ) );
            }
            catch( Exception ex )
            {
                ShowErrorMessage( MaskString( ex.Message ) );
                return - 1;
            }

            objZipEntry = objZipInputStream.GetNextEntry();
            long iTemp=0;

            while( objZipEntry != null )
            {
                iTemp = iTemp + objZipEntry.Size;
                objZipEntry = objZipInputStream.GetNextEntry();
            }
            objZipInputStream.Close();
            return iTemp;
        }

        /// <summary>
        /// Sets common properties on DNNTree control
        /// </summary>
        /// <history>
        /// 	[Jon Henning]	11/1/2004	Created
        /// 	[Jon Henning]	8/24/2005	Added Populate on Demand (POD) logic
        /// </history>
        private void InitializeTree()
        {
            DNNTree.SystemImagesPath = ResolveUrl( "~/images/" );
            DNNTree.ImageList.Add( ResolveUrl( "~/images/folder.gif" ) );
            DNNTree.ImageList.Add( ResolveUrl( "~/images/icon_securityroles_16px.gif" ) );
            DNNTree.ImageList.Add( ResolveUrl( "~/images/icon_sql_16px.gif" ) );
            DNNTree.ImageList.Add( ResolveUrl( "~/images/file.gif" ) );
            DNNTree.IndentWidth = 10;
            DNNTree.CollapsedNodeImage = ResolveUrl( "~/images/max.gif" );
            DNNTree.ExpandedNodeImage = ResolveUrl( "~/images/min.gif" );
            DNNTree.PopulateNodesFromClient = true;
            DNNTree.JSFunction = "nodeSelected();";
        }

        /// <summary>
        /// Masks the path
        /// </summary>
        /// <history>
        /// 	[Jon Henning]	11/1/2004	Created
        /// </history>
        private string MaskPath( string strOrigPath )
        {
            return strOrigPath.Replace( FileSystemUtils.RemoveTrailingSlash( RootFolderPath ), "0" ).Replace( "/", "\\" );
        }

        /// <summary>
        /// Masks a string
        /// </summary>
        /// <history>
        /// 	[Jon Henning]	11/1/2004	Created
        /// </history>
        private string MaskString( string strSource )
        {
            return FileManagerFunctions.CReplace( strSource, FileSystemUtils.RemoveTrailingSlash( RootFolderPath ), Localization.GetString( "PortalRoot", this.LocalResourceFile ), 1 );
        }

        /// <summary>
        /// Populates DNNTree control with folder hiearachy
        /// </summary>
        /// <param name="objNodes">Node collection to add children to</param>
        /// <param name="strPath">Path of parent node</param>
        /// <history>
        /// 	[Jon Henning]	10/26/2004	Created
        /// 	[Jon Henning]	8/24/2005	Added Populate on Demand (POD) logic
        ///     [cnurse]        04/24/2006  Updated to use new Secure Storage
        /// </history>
        private void PopulateTree( TreeNodeCollection objNodes, string strPath )
        {
            string folderPath = strPath.Replace( RootFolderPath, "" ).Replace( "\\", "/" );
            ArrayList folders = FileSystemUtils.GetFoldersByParentFolder( FolderPortalID, folderPath );

            //Iterate through the SubFolders
            foreach( FolderInfo folder in folders )
            {
                TreeNode objNode = AddNode( folder, objNodes );
                if( this.DNNTree.PopulateNodesFromClient == false )
                {
                    PopulateTree( objNode.TreeNodes, folder.FolderPath );
                }
            }
        }

        /// <summary>
        /// Sets up the file manager for Edit Mode
        /// </summary>
        /// <history>
        /// 	[Jon Henning]	11/1/2004	Created
        /// </history>
        private void SetEditMode()
        {
            if( dgFileList.EditItemIndex > - 1 )
            {
                //In Edit Mode
                int intCount = dgFileList.Items.Count;
                int i;
                for( i = 0; i <= intCount - 1; i++ )
                {
                    if( i != dgFileList.EditItemIndex )
                    {
                        CheckBox chkFile2 = (CheckBox)dgFileList.Items[i].FindControl( "chkFile2" );
                        CheckBox chkFile = (CheckBox)dgFileList.Items[i].FindControl( "chkFile" );
                        ImageButton lnkDeleteFile = (ImageButton)dgFileList.Items[i].FindControl( "lnkDeleteFile" );
                        ImageButton lnkEditFile = (ImageButton)dgFileList.Items[i].FindControl( "lnkEditFile" );
                        if( chkFile2 != null )
                        {
                            chkFile2.Enabled = false;
                        }
                        if( chkFile != null )
                        {
                            chkFile.Enabled = false;
                        }
                        if( lnkDeleteFile != null )
                        {
                            lnkDeleteFile.Enabled = false;
                            lnkDeleteFile.ImageUrl = "~/images/FileManager/DNNExplorer_trash_disabled.gif";
                            lnkDeleteFile.AlternateText = "";
                        }
                        if( lnkEditFile != null )
                        {
                            lnkEditFile.Enabled = false;
                            lnkEditFile.ImageUrl = "~/images/FileManager/DNNExplorer_Edit_disabled.gif";
                            lnkEditFile.AlternateText = "";
                        }
                    }
                }
                this.DisabledButtons = true;
            }
            else
            {
            }
            dgFileList.Columns[0].HeaderStyle.Width = Unit.Percentage( 5 );
            dgFileList.Columns[1].HeaderStyle.Width = Unit.Percentage( 25 );
            dgFileList.Columns[2].HeaderStyle.Width = Unit.Percentage( 25 );
            dgFileList.Columns[3].HeaderStyle.Width = Unit.Percentage( 7 );
            dgFileList.Columns[4].HeaderStyle.Width = Unit.Percentage( 15 );
        }

        /// <summary>
        /// Sets up the Error Message
        /// </summary>
        /// <history>
        /// 	[Jon Henning]	11/1/2004	Created
        /// </history>
        private void ShowErrorMessage( string strMessage )
        {
            strMessage = strMessage.Replace( @"\", @"\\" );
            strMessage = strMessage.Replace( "'", "\'" );
            strMessage = strMessage.Replace( "\r\n", "\n" );
            strMessage = string.Format( _ErrorMessage, strMessage );
            _DisplayingMessage = true;
            ClientAPI.RegisterClientVariable( this.Page, "ErrorMessage", strMessage, true );
        }

        /// <summary>
        /// Synchronizes the complete File System
        /// </summary>
        /// <history>
        /// 	[Jon Henning]	11/1/2004	Created
        /// </history>
        private void Synchronize()
        {
            if( IsHostMenu )
            {
                FileSystemUtils.Synchronize( Null.NullInteger, Null.NullInteger, Globals.HostMapPath );
            }
            else
            {
                FileSystemUtils.Synchronize( PortalId, PortalSettings.AdministratorRoleId, PortalSettings.HomeDirectoryMapPath );
            }
        }

        /// <summary>
        /// Unmasks the path
        /// </summary>
        /// <history>
        /// 	[Jon Henning]	11/1/2004	Created
        /// </history>
        private string UnMaskPath( string strOrigPath )
        {
            strOrigPath = FileSystemUtils.AddTrailingSlash( RootFolderPath ) + FileSystemUtils.StripFolderPath( strOrigPath );
            return strOrigPath.Replace( "/", "\\" );
        }

        /// <summary>
        /// Updates the space Used label
        /// </summary>
        /// <history>
        /// 	[Jon Henning]	11/1/2004	Created
        /// </history>
        private void UpdateSpaceUsed()
        {
            FileSystemUtils.AddTrailingSlash( UnMaskPath( DestPath ) );
            PortalController objPortalController = new PortalController();
            string strQuota;

            if( PortalSettings.HostSpace == 0 )
            {
                strQuota = Localization.GetString( "UnlimitedSpace", this.LocalResourceFile );
            }
            else
            {
                strQuota = PortalSettings.HostSpace + "MB";
            }

            if( IsHostMenu )
            {
                lblFileSpace.Text = "&nbsp;";
            }
            else
            {
                long spaceUsed = objPortalController.GetPortalSpaceUsedBytes( FolderPortalID );
                string strUsed;
                if( spaceUsed < 1024 )
                {
                    strUsed = spaceUsed.ToString( "0.00" ) + "B";
                }
                else if( spaceUsed < ( 1024*1024 ) )
                {
                    strUsed = ( spaceUsed/1024 ).ToString( "0.00" ) + "KB";
                }
                else
                {
                    strUsed = ( spaceUsed/( 1024*1024 ) ).ToString( "0.00" ) + "MB";
                }

                lblFileSpace.Text = string.Format( Localization.GetString( "SpaceUsed", this.LocalResourceFile ), strUsed, strQuota );
            }
        }

        /// <summary>
        /// Renders the page output
        /// </summary>
        /// <history>
        /// 	[Jon Henning]	11/1/2004	Created
        /// </history>
        protected override void Render( HtmlTextWriter output )
        {
            string strTemp = GetCheckAllString();

            pnlScripts2.Controls.Add( new LiteralControl( strTemp ) );
            if( dgFileList.Items.Count <= 10 && dgFileList.PageCount == 1 )
            {
                dgFileList.PagerStyle.Visible = false;
            }

            base.Render( output );
        }

        /// <summary>
        /// The Page_Load server event handler on this user control is used
        /// to populate the current files from the appropriate PortalUpload Directory or the HostFolder
        /// and binds this list to the Datagrid
        /// </summary>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                //FileManager requires at a bare minimum the dnn namespace, so regardless of whether the ClientAPI is disabled of not we
                //need to register it.
                ClientAPI.RegisterClientReference( this.Page, ClientAPI.ClientNamespaceReferences.dnn );

                DNNClientAPI.AddBodyOnloadEventHandler( this.Page, "initFileManager();" );
                ClientAPI.RegisterClientVariable( this.Page, "UCPrefixID", DNNTree.ClientID.Replace( DNNTree.ID, "" ), true );
                ClientAPI.RegisterClientVariable( this.Page, "UCPrefixName", DNNTree.UniqueID.Replace( DNNTree.ID, "" ), true );

                if( IsHostMenu )
                {
                    RootFolderName = Localization.GetString( "HostRoot", this.LocalResourceFile );
                    pnlSecurity.Visible = false;
                }
                else
                {
                    RootFolderName = Localization.GetString( "PortalRoot", this.LocalResourceFile );
                    pnlSecurity.Visible = true;
                }

                if( DNNTree.IsDownLevel )
                {
                    this.DisabledButtons = true;
                }
                else
                {
                    this.DisabledButtons = false;
                }

                if( Page.IsPostBack == false )
                {
                    DataCache.RemoveCache( "Folders:" + FolderPortalID );
                    Localization.LocalizeDataGrid( ref dgFileList, this.LocalResourceFile );
                    InitializeTree();
                    BindFolderTree();
                    IsRefresh = true;
                    PageIndex = 0;
                    Sort = "FileType ASC, FileName ASC";
                    LastSort = "FileType ASC, FileName ASC";
                    MoveStatus = "";
                    FilterFiles = "";
                    DestPath = "0\\";
                    BindFileList();
                    BindStorageLocationTypes();
                }

                if( LastFolderPath != DestPath )
                {
                    PageIndex = 0;
                    GeneratePermissionsGrid();
                }
                LastFolderPath = DestPath;
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// The cmdCancel_Click server event handler on this user control runs when the
        /// Cancel Button is clicked
        /// </summary>
        /// <history>
        /// 	[DYNST]	        2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void cmdCancel_Click( Object sender, EventArgs e )
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
        /// The cmdUpdate_Click server event handler on this user control runs when the
        /// Update button is clicked
        /// </summary>
        /// <history>
        /// 	[DYNST]	        2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ///     [Jon Henning]	4/21/2004	Rebind grid after update to reflect update - DNN-178
        /// </history>
        protected void cmdUpdate_Click( Object sender, EventArgs e )
        {
            string strFolderPath = FileSystemUtils.StripFolderPath( this.LastFolderPath ).Replace( "\\", "/" );
            FolderController objFolderController = new FolderController();
            FolderInfo objFolderInfo = objFolderController.GetFolder( FolderPortalID, strFolderPath );
            if( objFolderInfo == null )
            {
                //file system needs synchronizing
                //with database...this folder is new.
                Synchronize();
                objFolderInfo = objFolderController.GetFolder( FolderPortalID, strFolderPath );
            }

            FolderPermissionController objFolderPermissionController = new FolderPermissionController();
            FolderPermissionCollection objCurrentFolderPermissions;
            objCurrentFolderPermissions = objFolderPermissionController.GetFolderPermissionsCollectionByFolderPath( FolderPortalID, strFolderPath );
            if( ! objCurrentFolderPermissions.CompareTo( dgPermissions.Permissions ) )
            {
                objFolderPermissionController.DeleteFolderPermissionsByFolder( FolderPortalID, strFolderPath );
                foreach (FolderPermissionInfo objFolderPermission in dgPermissions.Permissions)
                {
                    objFolderPermission.FolderID = objFolderInfo.FolderID;
                    if( objFolderPermission.AllowAccess )
                    {
                        objFolderPermissionController.AddFolderPermission( objFolderPermission );
                    }
                }
                GeneratePermissionsGrid(); //rebind the grid to reflect updated values - it is possible for the grid controls and the database to become out of sync
            }
        }

        /// <summary>
        /// The dgFileList_ItemDataBound server event handler on this user control runs when a
        /// File or Folder is added to the Files Table
        /// </summary>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ///     [cnurse]        12/3/2004   modified to handle folders and to use
        ///                                 custom images
        /// </history>
        protected void dgFileList_ItemDataBound( object sender, DataGridItemEventArgs e )
        {
            if( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.EditItem || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem )
            {
                CheckBox chkFile = (CheckBox)e.Item.FindControl( "chkFile" );
                if( chkFile != null )
                {
                    string sDefCssClass = dgFileList.ItemStyle.CssClass;
                    if( e.Item.ItemType == ListItemType.AlternatingItem )
                    {
                        sDefCssClass = dgFileList.AlternatingItemStyle.CssClass;
                    }

                    chkFile.Attributes.Add( "onclick", "addFileToMoveList('" + ClientAPI.GetSafeJSString( chkFile.Attributes["filename"] ) + "', this, '" + dgFileList.SelectedItemStyle.CssClass + "', '" + sDefCssClass + "');" );
                }

                ImageButton lnkEditFile = (ImageButton)e.Item.FindControl( "lnkEditFile" );
                if( lnkEditFile != null )
                {
                    lnkEditFile.CommandName = e.Item.ItemIndex.ToString();
                }

                Image lnkUnzip = (Image)e.Item.FindControl( "lnkUnzip" );
                if( lnkUnzip != null )
                {
                    if( lnkUnzip.Attributes["extension"] != "zip" )
                    {
                        lnkUnzip.Visible = false;
                    }
                    else
                    {
                        if( e.Item.ItemType == ListItemType.EditItem )
                        {
                            lnkUnzip.Visible = false;
                        }
                        else
                        {
                            lnkUnzip.Attributes.Add( "onclick", "return unzipFile('" + ClientAPI.GetSafeJSString( lnkUnzip.Attributes["filename"] ) + "');" );
                        }
                    }
                }

                ImageButton lnkDeleteFile = (ImageButton)e.Item.FindControl( "lnkDeleteFile" );
                if( lnkDeleteFile != null )
                {
                    if( dgFileList.EditItemIndex == - 1 )
                    {
                        ClientAPI.AddButtonConfirm( lnkDeleteFile, string.Format( Localization.GetString( "EnsureDeleteFile", this.LocalResourceFile ), lnkDeleteFile.CommandName ) );
                    }
                }

                ImageButton lnkOkRename = (ImageButton)e.Item.FindControl( "lnkOkRename" );
                if( lnkOkRename != null )
                {
                    lnkOkRename.CommandName = e.Item.ItemIndex.ToString();
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The dgFileList_SortCommand server event handler on this user control runs when one
        /// of the Column Header Links is clicked
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	01/12/2007	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected void dgFileList_SortCommand(object source, DataGridSortCommandEventArgs e)
        {
            BindFolderTree();
            this.IsRefresh = true;
            LastSort = Sort;
            if (Sort.Replace(" ASC", "").Replace(" DESC", "") == e.SortExpression)
            {
                //Switch order
                if (Sort.Contains("ASC"))
                {
                    Sort = Sort.Replace("ASC", "DESC");
                }        
                else
                {
                    Sort = Sort.Replace("DESC", "ASC");
                }
            }
            else
            {
                Sort = e.SortExpression + " ASC";
            }
            MoveStatus = "";
            FilterFiles = "";
            BindFileList();
        }

        /// <summary>
        /// The DNNTree_NodeClick server event handler on this user control runs when a
        /// Node (Folder in the) in the TreeView is clicked
        /// </summary>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ///     [cnurse]        12/3/2004   modified to handle folders and to use
        ///                                 custom images
        /// </history>
        protected void DNNTree_NodeClick( object source, DNNTreeNodeClickEventArgs e )
        {
            if( DNNTree.IsDownLevel )
            {
                DestPath = e.Node.Key;
                LastPath = e.Node.Key;
            }
            BindFileList();
            GeneratePermissionsGrid();
        }

        /// <summary>
        /// This method is called from the client to populate send new nodes down to the client
        /// </summary>
        /// <history>
        /// 	[Jon Henning]	8/24/2005	Created
        /// </history>
        protected void DNNTree_PopulateOnDemand( object source, DNNTreeEventArgs e )
        {
            DestPath = e.Node.Key;
            PopulateTree( e.Node.TreeNodes, UnMaskPath( e.Node.Key.Replace( "\\\\", "\\" ) ) );
            GeneratePermissionsGrid();
        }

        /// <summary>
        /// The lnkAddFolder_Command server event handler on this user control runs when the
        /// Add Folder button is clicked
        /// </summary>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkAddFolder_Command( object sender, CommandEventArgs e )
        {
            if( this.txtNewFolder.Text == "" )
            {
                return;
            }
            string strSourcePath;

            strSourcePath = UnMaskPath( DestPath );

            try
            {
                ArrayList colNodes = DNNTree.SelectedTreeNodes;
                if( colNodes.Count > 0 )
                {
                    TreeNode parentNode = (TreeNode)colNodes[0];

                    string filterFolderName;
                    filterFolderName = txtNewFolder.Text.Replace( ".", "_" );
                    //Add Folder to Database
                    FileSystemUtils.AddFolder( PortalSettings, strSourcePath, filterFolderName, Convert.ToInt16( ddlStorageLocation.SelectedValue ) );
                    DestPath = MaskPath( FileSystemUtils.AddTrailingSlash( strSourcePath ) + filterFolderName );

                    DataCache.RemoveCache( "Folders:" + FolderPortalID );
                    parentNode.TreeNodes.Clear();
                    PopulateTree( parentNode.TreeNodes, UnMaskPath( parentNode.Key.Replace( "\\\\", "\\" ) ) );
                    parentNode.Selected = false;
                }
            }
            catch( Exception ex )
            {
                string strErrorMessage = MaskString( ex.Message );
                ShowErrorMessage( strErrorMessage );
            }

            txtNewFolder.Text = "";

            PageIndex = 0;
            BindFileList();
        }

        /// <summary>
        /// The lnkDeleteFolder_Command server event handler on this user control runs when the
        /// Add Folder ibutton is clicked
        /// </summary>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkDeleteFolder_Command( object sender, CommandEventArgs e )
        {
            string strSourcePath;            

            if( DestPath == DNNTree.TreeNodes[ 0 ].Key )
            {
                //Delete Root Node?  Then what? :/
                ShowErrorMessage( Localization.GetString( "NotAllowedToDeleteRootFolder", this.LocalResourceFile ) );
                BindFileList();
                return;
            }
            else
            {
                strSourcePath = UnMaskPath( DestPath );
            }

            DirectoryInfo dinfo = new DirectoryInfo( strSourcePath );
            if( dinfo.Exists == false )
            {
                //ODD...
                ShowErrorMessage( Localization.GetString( "FolderAlreadyRemoved", this.LocalResourceFile ) );
                BindFileList();
                return;
            }

            if( ( Directory.GetDirectories( strSourcePath ).Length > 0 ) || ( dgFileList.Items.Count > 0 ) )
            {
                //Files and/or folders exist in directory..
                //Files in current folder, make them delete first
                //Recursive Folder-delete can be enabled by adjusting this Sub
                ShowErrorMessage( Localization.GetString( "PleaseRemoveFilesBeforeDeleting", this.LocalResourceFile ) );
                BindFileList();
                return;
            }

            try
            {
                //Delete Folder
                string folderName = FileSystemUtils.StripFolderPath( DestPath );
                FileSystemUtils.DeleteFolder( FolderPortalID, dinfo, folderName );

                int intEnd;
                if( DestPath.EndsWith( "\\" ) )
                {
                    DestPath = DestPath.Substring( 0, DestPath.Length - 1 );
                }

                intEnd = DestPath.LastIndexOf( "\\" );
                DestPath = DestPath.Substring( 0, intEnd );

                //since we removed folder, we will select parent folder
                ArrayList colNodes = DNNTree.SelectedTreeNodes;
                if( colNodes.Count > 0 )
                {
                    TreeNode objNode = (TreeNode)colNodes[0];
                    objNode.Selected = false;
                    objNode.Parent.Selected = true;
                    objNode.Parent.DNNNodes.Remove( objNode );
                }

                DataCache.RemoveCache( "Folders:" + FolderPortalID );

                BindFileList();
                GeneratePermissionsGrid();
            }
            catch( Exception ex )
            {
                ShowErrorMessage( Localization.GetString( "ErrorDeletingFolder", this.LocalResourceFile ) + ex.Message );
            }
        }

        /// <summary>
        /// The lnkDLFile_Command server event handler on this user control runs when the
        /// Download File button is clicked
        /// </summary>
        /// <remarks>
        /// The method calls the FileSystemUtils DownLoad method
        /// </remarks>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkDLFile_Command( object sender, CommandEventArgs e )
        {
            FileSystemUtils.DownloadFile( PortalSettings, Convert.ToInt32( e.CommandArgument ), false, true );
            BindFolderTree();
        }

        /// <summary>
        /// The lnkEditFile_Command server event handler on this user control runs when the
        /// Edit File button is clicked
        /// </summary>
        /// <remarks>
        /// The DataGrid is switched to Edit Mode
        /// </remarks>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkEditFile_Command( object sender, CommandEventArgs e )
        {
            dgFileList.EditItemIndex = Convert.ToInt32( e.CommandName );
            BindFileList();
            SetEditMode();
        }

        /// <summary>
        /// The lnkCancelRename_Command server event handler on this user control runs when the
        /// Cancel Edit button is clicked when in Edit Mode
        /// </summary>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkCancelRename_Command( object sender, CommandEventArgs e )
        {
            dgFileList.EditItemIndex = - 1;
            BindFileList();
            SetEditMode();
        }

        /// <summary>
        /// The lnkDeleteAllCheckedFiles_Command server event handler on this user control runs when the
        /// Javascript in the page triggers the event
        /// </summary>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkDeleteAllCheckedFiles_Command( object sender, CommandEventArgs e )
        {
            if( !String.IsNullOrEmpty(MoveFiles) )
            {
                DeleteFiles( MoveFiles );
            }
        }

        /// <summary>
        /// The lnkDeleteFile_Command server event handler on this user control runs when the
        /// Javascript in the page triggers the event
        /// </summary>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkDeleteFile_Command( object sender, CommandEventArgs e )
        {
            DeleteFiles( e.CommandName );
        }

        /// <summary>
        /// The lnkFilter_Command server event handler on this user control runs when the
        /// Filter Files button is clicked.
        /// </summary>
        /// <remarks>
        /// The method calls the relevant FileSystemUtils method
        /// </remarks>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkFilter_Command( object sender, CommandEventArgs e )
        {
            this.dgFileList.CurrentPageIndex = 0;
            FilterFiles = txtFilter.Text;
            BindFileList();
        }

        /// <summary>
        /// The lnkMoveFiles_Command server event handler on this user control runs when the
        /// Move Files button is clicked.
        /// </summary>
        /// <remarks>
        /// The method calls the relevant FileSystemUtils method
        /// </remarks>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkMoveFiles_Command( object sender, CommandEventArgs e )
        {
            string[] arFiles = MoveFiles.Split( ';' );
            
            string strErrorMessages = "";

            string strDestPath = FileSystemUtils.AddTrailingSlash( UnMaskPath( DestPath ) );
            string strSourcePath = FileSystemUtils.AddTrailingSlash( UnMaskPath( SourcePath ) );

            if( !String.IsNullOrEmpty(strErrorMessages) )
            {
                ShowErrorMessage( MaskString( strErrorMessages ) );
                MoveFiles = "";
                MoveStatus = "";
                SourcePath = "";
                return;
            }

            for( int i = 0; i <= arFiles.Length - 1; i++ )
            {
                if( arFiles[i] != "" )
                {
                    string strSourceFile = strSourcePath + arFiles[i];
                    string strDestFile = strDestPath + arFiles[i];

                    string strCurErrorMessage = "";
                    switch( MoveStatus )
                    {
                        case "copy":

                            strCurErrorMessage = FileSystemUtils.CopyFile( strSourceFile, strDestFile, PortalSettings );
                            break;
                        case "move":

                            strCurErrorMessage = FileSystemUtils.MoveFile( strSourceFile, strDestFile, PortalSettings );
                            break;
                        case "unzip":

                            strCurErrorMessage = FileSystemUtils.UnzipFile( strSourceFile, strDestPath, PortalSettings );
                            BindFolderTree();
                            break;
                    }

                    if( !String.IsNullOrEmpty(strCurErrorMessage) )
                    {
                        //Unmask paths here, remask with title before showining error message
                        if( MoveStatus == "copy" )
                        {
                            strErrorMessages = strErrorMessages + Localization.GetString( "ErrorCopyingFile", this.LocalResourceFile ) + FileSystemUtils.AddTrailingSlash( UnMaskPath( SourcePath ) ) + arFiles[i] + "&nbsp;&nbsp; to " + FileSystemUtils.AddTrailingSlash( UnMaskPath( DestPath ) ) + "<BR>&nbsp;&nbsp;&nbsp;" + strCurErrorMessage + "<BR>";
                        }
                        else
                        {
                            strErrorMessages = strErrorMessages + Localization.GetString( "ErrorMovingFile", this.LocalResourceFile ) + FileSystemUtils.AddTrailingSlash( UnMaskPath( SourcePath ) ) + arFiles[i] + "&nbsp;&nbsp; to " + FileSystemUtils.AddTrailingSlash( UnMaskPath( DestPath ) ) + "<BR>&nbsp;&nbsp;&nbsp;" + strCurErrorMessage + "<BR>";
                        }
                    }
                }
            }

            if( strErrorMessages == "" )
            {
                LastPath = FileSystemUtils.RemoveTrailingSlash( DestPath );
            }
            else
            {
                strErrorMessages = MaskString( strErrorMessages );
                strErrorMessages = MaskString( strErrorMessages );
                ShowErrorMessage( strErrorMessages );
            }
            BindFileList();
            MoveStatus = "";
            SourcePath = "";
        }

        /// <summary>
        /// The lnkMoveFirst_Command server event handler on this user control runs when the
        /// Move First Page button is clicked.
        /// </summary>
        /// <remarks>
        /// The method calls the relevant FileSystemUtils method
        /// </remarks>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkMoveFirst_Command( object sender, CommandEventArgs e )
        {
            PageIndex = 0;
            BindFileList();
        }

        /// <summary>
        /// The lnkMoveLast_Command server event handler on this user control runs when the
        /// Move Last Page button is clicked.
        /// </summary>
        /// <remarks>
        /// The method calls the relevant FileSystemUtils method
        /// </remarks>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkMoveLast_Command( object sender, CommandEventArgs e )
        {
            this.PageIndex = this.dgFileList.PageCount - 1;
            BindFileList();
        }

        /// <summary>
        /// The lnkMoveNext_Command server event handler on this user control runs when the
        /// Move Next Page button is clicked.
        /// </summary>
        /// <remarks>
        /// The method calls the relevant FileSystemUtils method
        /// </remarks>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkMoveNext_Command( object sender, CommandEventArgs e )
        {
            PageIndex++;
            if( PageIndex > dgFileList.PageCount - 1 )
            {
                PageIndex = dgFileList.PageCount - 1;
            }
            BindFileList();
        }

        /// <summary>
        /// The lnkMoveNext_Command server event handler on this user control runs when the
        /// Move Previous Page button is clicked.
        /// </summary>
        /// <remarks>
        /// The method calls the relevant FileSystemUtils method
        /// </remarks>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkMovePrevious_Command( object sender, CommandEventArgs e )
        {
            PageIndex--;
            if( PageIndex < 0 )
            {
                PageIndex = 0;
            }
            BindFileList();
        }

        /// <summary>
        /// The lnkOkRename_Command server event handler on this user control runs when the
        /// Save Changes (Ok) button is clicked when in Edit Mode
        /// </summary>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkOkRename_Command( object sender, CommandEventArgs e )
        {
            string strSourcePath;
            int intItemID = Convert.ToInt32( e.CommandName );
            strSourcePath = FileSystemUtils.AddTrailingSlash( UnMaskPath( DestPath ) );

            TextBox txtEdit;
            txtEdit = (TextBox)dgFileList.Items[intItemID].FindControl( "txtEditFileName" );

            string strSourceFile;

            strSourceFile = strSourcePath + e.CommandArgument;

            string strDestFile = strSourcePath + txtEdit.Text;
            string strFileRenameError = "";
            string strSetAttributeError = "";

            string strExtension = Path.GetExtension( strDestFile ).Replace( ".", "" );
            string fileExtensions = "," + PortalSettings.HostSettings["FileExtensions"].ToString().ToLower();
            string fileExt = "," + strExtension.ToLower();
            if( fileExtensions.IndexOf( fileExt ) != 0 || PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId )
            {
                if( strSourceFile != strDestFile )
                {
                    //Move(Rename File)
                    strFileRenameError = FileSystemUtils.MoveFile( strSourceFile, strDestFile, PortalSettings );
                    strSourceFile = strDestFile;
                }
                if( strFileRenameError == "" )
                {
                    CheckBox chkReadOnly = (CheckBox)dgFileList.Items[intItemID].FindControl( "chkReadOnly" );
                    CheckBox chkHidden = (CheckBox)dgFileList.Items[intItemID].FindControl( "chkHidden" );
                    CheckBox chkSystem = (CheckBox)dgFileList.Items[intItemID].FindControl( "chkSystem" );
                    CheckBox chkArchive = (CheckBox)dgFileList.Items[intItemID].FindControl( "chkArchive" );
                    if( ( chkReadOnly.Attributes["original"] != chkReadOnly.Checked.ToString() ) || ( chkHidden.Attributes["original"] != chkHidden.Checked.ToString() ) || ( chkSystem.Attributes["original"] != chkSystem.Checked.ToString() ) || ( chkArchive.Attributes["original"] != chkArchive.Checked.ToString() ) )
                    {
                        //  Attributes were change, update 'dem
                        int iAttr=0;
                        if( chkReadOnly.Checked )
                        {
                            iAttr += (int)FileAttributes.ReadOnly;
                        }
                        if( chkHidden.Checked )
                        {
                            iAttr += (int)FileAttributes.Hidden;
                        }
                        if( chkSystem.Checked )
                        {
                            iAttr += (int)FileAttributes.System;
                        }
                        if( chkArchive.Checked )
                        {
                            iAttr += (int)FileAttributes.Archive;
                        }

                        try
                        {
                            FileSystemUtils.SetFileAttributes( strSourceFile, iAttr );
                        }
                        catch( Exception ex )
                        {
                            strSetAttributeError = ex.Message;
                        }
                    }
                }
            }
            else
            {
                // restricted file type
                strFileRenameError += "<br>" + string.Format( Localization.GetString( "RestrictedFileType" ), strDestFile, PortalSettings.HostSettings["FileExtensions"].ToString().Replace(",", ", *.") );
            }

            if( !String.IsNullOrEmpty(strFileRenameError) )
            {
                strFileRenameError = Localization.GetString( "Rename.Error", this.LocalResourceFile ) + strFileRenameError;
                ShowErrorMessage( MaskString( strFileRenameError ) );
            }
            if( !String.IsNullOrEmpty(strSetAttributeError) )
            {
                strSetAttributeError = Localization.GetString( "SetAttrubute.Error", this.LocalResourceFile ) + strSetAttributeError;
                ShowErrorMessage( strSetAttributeError );
            }

            dgFileList.EditItemIndex = - 1;
            BindFileList();
            SetEditMode();
        }

        /// <summary>
        /// The lnkRefresh_Command server event handler on this user control runs when the
        /// Refresh button is clicked.
        /// </summary>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkRefresh_Command( object sender, CommandEventArgs e )
        {
            BindFolderTree();
            this.IsRefresh = true;
            Sort = "FileType ASC, FileName ASC";
            LastSort = "FileType ASC, FileName ASC";
            MoveStatus = "";
            FilterFiles = "";
            BindFileList();
        }

        /// <summary>
        /// The lnkSelectFolder_Command server event handler on this user control runs when a
        /// Folder is selected.
        /// </summary>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkSelectFolder_Command( object sender, CommandEventArgs e )
        {
            string strSourcePath = DestPath;
            string strFriendlyPath = strSourcePath.Replace( "0\\", "Portal Root\\" );

            dgFileList.CurrentPageIndex = 0;
            ClientAPI.AddButtonConfirm( lnkDeleteFolder, string.Format( Localization.GetString( "EnsureDeleteFolder", this.LocalResourceFile ), strFriendlyPath, null ) );
            strSourcePath = UnMaskPath( strSourcePath.Replace( "\\\\", "\\" ) );
            LastPath = strSourcePath;
            GetFilesByFolder( FileSystemUtils.AddTrailingSlash( strSourcePath ) );
        }

        /// <summary>
        /// The lnkSyncFolder_Command server event handler on this user control runs when the
        /// Synchronize Folder button is clicked.
        /// </summary>
        /// <history>
        /// 	[cnurse]	04/24/2006	Created
        /// </history>
        protected void lnkSyncFolder_Command(object sender, CommandEventArgs e)
        {
            string syncFolderPath = UnMaskPath(DestPath);
            bool isRecursive = chkRecursive.Checked;
            string relPath = syncFolderPath.Replace(RootFolderPath, "").Replace("\\", "/");

            FileSystemUtils.SynchronizeFolder(FolderPortalID, syncFolderPath, relPath, isRecursive);
            DataCache.RemoveCache("Folders:" + FolderPortalID);
            BindFolderTree();
            BindFileList();

        }

        /// <summary>
        /// The lnkSyncFolders_Click server event handler on this user control runs when the
        /// Synchronize Folders button is clicked.
        /// </summary>
        protected void lnkSyncFolders_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (IsHostMenu)
            {
                FileSystemUtils.SynchronizeFolder(Null.NullInteger, Common.Globals.HostMapPath, "", true, false, true);
            }
            else
            {
                FileSystemUtils.SynchronizeFolder(PortalId, PortalSettings.HomeDirectoryMapPath, "", true, false, true);
            }

            DataCache.RemoveCache("Folders:" + FolderPortalID.ToString());
            BindFolderTree();
            BindFileList();
        }

        /// <summary>
        /// The lnkUpload_Command server event handler on this user control runs when the
        /// Upload button is clicked
        /// </summary>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void lnkUpload_Command( object sender, CommandEventArgs e )
        {
            string strDestPath = DestPath.Replace( "0\\", "" );
            string WebUploadParam = "ftype=" + UploadType.File;
            string returnTab = "rtab=" + TabId;
            string destUrl = EditUrl( "dest", Globals.QueryStringEncode( strDestPath ), "Edit", WebUploadParam, returnTab );
            Response.Redirect( destUrl );
        }

        /// <summary>
        /// The selPageSize_SelectedIndexChanged server event handler on this user control
        /// runs when the Page Size combo's index/value is changed
        /// </summary>
        /// <history>
        /// 	[DYNST]	2/1/2004	Created
        ///     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        /// </history>
        protected void selPageSize_SelectedIndexChanged( Object sender, EventArgs e )
        {
            PageIndex = 0;
            BindFileList();
        }

    }
}