using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualBasic;
using FileInfo=System.IO.FileInfo;

namespace DotNetNuke.Common.Utilities
{
    public class FileSystemUtils
    {
        /// <summary>
        /// Adds a File
        /// </summary>
        /// <param name="strFile">The File Name</param>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <param name="HomeDirectory">The home directory for the Portal</param>
        /// <param name="ClearCache">A flag that indicates whether the file cache should be cleared</param>
        /// <remarks>This method is called by the SynchonizeFolder method, when the file exists in the file system
        /// but not in the Database
        /// </remarks>
        private static string AddFile( string strFile, int PortalId, string HomeDirectory, bool ClearCache )
        {
            string retValue = "";
            try
            {
                FileController objFileController = new FileController();
                FileInfo fInfo = new FileInfo( strFile );
                string sourceFolderName = Globals.GetSubFolderPath( strFile );
                string sourceFileName = GetFileName( strFile );
                Services.FileSystem.FileInfo file = objFileController.GetFile( sourceFileName, PortalId, sourceFolderName );

                if( file == null )
                {
                    //Add the new File
                    AddFile( PortalId, fInfo.OpenRead(), strFile, "", fInfo.Length, sourceFolderName, HomeDirectory, true, ClearCache );
                }
            }
            catch( Exception ex )
            {
                retValue = ex.Message;
            }
            return retValue;
        }

        /// <summary>
        /// Adds a File
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <param name="inStream">The stream to add</param>
        /// <param name="contentType">The type of the content</param>
        /// <param name="length">The length of the content</param>
        /// <param name="folderName">The name of the folder</param>
        /// <param name="closeInputStream">A flag that dermines if the Input Stream should be closed.</param>
        /// <param name="ClearCache">A flag that indicates whether the file cache should be cleared</param>
        /// <remarks>This method adds a new file
        /// </remarks>
        private static string AddFile( int PortalId, Stream inStream, string fileName, string contentType, long length, string folderName, string HomeDirectory, bool closeInputStream, bool clearCache )
        {
            FolderController objFolderController = new FolderController();
            FileController objFileController = new FileController();
            string sourceFolderName = Globals.GetSubFolderPath( fileName );
            FolderInfo folder = objFolderController.GetFolder( PortalId, sourceFolderName );
            string sourceFileName = GetFileName( fileName );
            Image imgImage;
            int imageWidth = 0;
            int imageHeight = 0;
            int intFileID;
            string retValue = "";

            retValue += CheckValidFileName( fileName );
            if( retValue.Length > 0 )
            {
                return retValue;
            }

            string extension = Path.GetExtension( fileName ).Replace( ".", "" );
            if( contentType == "" )
            {
                contentType = GetContentType( extension );
            }

            //Add file to Database
            intFileID = objFileController.AddFile( PortalId, sourceFileName, extension, length, imageWidth, imageHeight, contentType, folderName, clearCache );

            //Save file to File Storage
            WriteStream( intFileID, inStream, fileName, folder.StorageLocation, closeInputStream );

            //If an image lets try and find out the image size and the file size.  In this scenario the file will exist
            //on the file system so lets load the file
            if( Convert.ToBoolean( Strings.InStr( 1, Globals.glbImageFileTypes + ",", extension.ToLower() + ",", 0 ) ) )
            {
                try
                {
                    Services.FileSystem.FileInfo objFile = objFileController.GetFileById( intFileID, PortalId );
                    Stream imageStream = GetFileStream( objFile, PortalId, HomeDirectory );
                    imgImage = Image.FromStream( imageStream );
                    imageHeight = imgImage.Height;
                    imageWidth = imgImage.Width;
                    imgImage.Dispose();
                    imageStream.Close();
                }
                catch
                {
                    // error loading image file
                    contentType = "application/octet-stream";
                }
                finally
                {
                    //Update the File info
                    objFileController.UpdateFile( PortalId, sourceFileName, sourceFileName, extension, length, imageWidth, imageHeight, contentType, folderName, folderName );
                }
            }

            if( folder.StorageLocation != (int)FolderController.StorageLocationTypes.InsecureFileSystem )
            {
                //try and delete the Insecure file
                AttemptFileDeletion( fileName );
            }

            if( folder.StorageLocation != (int)FolderController.StorageLocationTypes.SecureFileSystem )
            {
                //try and delete the Secure file
                AttemptFileDeletion( fileName + Globals.glbProtectedExtension );
            }

            return retValue;
        }

        /// <summary>
        /// Adds a Folder
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <param name="AdministratorRoleId">The Id of the Administrator role</param>
        /// <param name="relativePath">The relative path of the folder</param>
        /// <param name="StorageLocation">The type of storage location</param>
        private static int AddFolder( int PortalId, int AdministratorRoleId, string relativePath, int StorageLocation )
        {
            FolderController objFolderController = new FolderController();

            bool isProtected;
            isProtected = DefaultProtectedFolders( relativePath );

            int FolderID = objFolderController.AddFolder( PortalId, relativePath, StorageLocation, isProtected, false );

            if( PortalId != Null.NullInteger )
            {
                SetFolderPermissions( PortalId, FolderID, AdministratorRoleId, relativePath );
            }

            return FolderID;
        }

        public static string AddTrailingSlash( string strSource )
        {
            if( !strSource.EndsWith( "\\" ) )
            {
                strSource = strSource + "\\";
            }
            return strSource;
        }

        /// <summary>
        /// Checks that the file name is valid
        /// </summary>
        /// <param name="strFileName">The name of the file</param>
        private static string CheckValidFileName( string strFileName )
        {
            string retValue = Null.NullString;
            if( strFileName.IndexOf( "\'" ) > -1 )
            {
                // check if context is valid since this method is called from the scheduller too
                if( HttpContext.Current != null )
                {
                    retValue = Localization.GetString( "InvalidFileName" );
                }
                else
                {
                    retValue = "InvalidFileName";
                }
            }

            //Return
            return retValue;
        }

        /// <summary>
        /// Copies a File
        /// </summary>
        /// <param name="strSourceFile">The original File Name</param>
        /// <param name="strDestFile">The new File Name</param>
        /// <param name="settings">The Portal Settings for the Portal/Host Account</param>
        /// <remarks>
        /// </remarks>
        public static string CopyFile( string strSourceFile, string strDestFile, PortalSettings settings )
        {
            int FolderPortalId = Null.NullInteger;
            string HomeDirectory = Globals.HostMapPath;
            bool isHost = Convert.ToBoolean( settings.ActiveTab.ParentId == settings.SuperTabId ? true : false );
            if( !isHost )
            {
                FolderPortalId = settings.PortalId;
                HomeDirectory = settings.HomeDirectoryMapPath;
            }
            return UpdateFile( strSourceFile, strDestFile, FolderPortalId, HomeDirectory, true, false, true );
        }

        /// <summary>
        /// This checks to see if the folder is a protected type of folder
        /// </summary>
        /// <param name="folderPath">String</param>
        /// <returns>Boolean</returns>
        /// <remarks>
        /// </remarks>
        public static bool DefaultProtectedFolders( string folderPath )
        {
            if( folderPath == "" || folderPath.ToLower() == "skins" || folderPath.ToLower() == "containers" || folderPath.ToLower().StartsWith( "skins/" ) == true || folderPath.ToLower().StartsWith( "containers/" ) == true )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="strSourceFile">The File to delete</param>
        /// <param name="settings">The Portal Settings for the Portal/Host Account</param>
        /// <remarks>
        /// </remarks>
        public static string DeleteFile( string strSourceFile, PortalSettings settings )
        {
            return DeleteFile( strSourceFile, settings, true );
        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="strSourceFile">The File to delete</param>
        /// <param name="settings">The Portal Settings for the Portal/Host Account</param>
        /// <remarks>
        /// </remarks>
        public static string DeleteFile( string strSourceFile, PortalSettings settings, bool ClearCache )
        {
            string retValue = "";
            try
            {
                string folderName = Globals.GetSubFolderPath( strSourceFile );
                string fileName = GetFileName( strSourceFile );

                int PortalId;
                if( settings.ActiveTab.ParentId == settings.SuperTabId )
                {
                    PortalId = Null.NullInteger;
                }
                else
                {
                    PortalId = settings.PortalId;
                }

                //Remove file from DataBase
                FileController objFileController = new FileController();
                objFileController.DeleteFile( PortalId, fileName, folderName, ClearCache );

                //try and delete the Insecure file
                AttemptFileDeletion( strSourceFile );

                //try and delete the Secure file
                AttemptFileDeletion( strSourceFile + Globals.glbProtectedExtension );
            }
            catch( Exception ex )
            {
                retValue = ex.Message;
            }

            return retValue;
        }

        /// <summary>
        /// Streams a file to the output stream if the user has the proper permissions
        /// </summary>
        /// <param name="FileId">FileId identifying file in database</param>
        /// <param name="ClientCache">Cache file in client browser - true/false</param>
        /// <param name="ForceDownload">Force Download File dialog box - true/false</param>
        /// <remarks>
        /// </remarks>
        public static bool DownloadFile( PortalSettings settings, int FileId, bool ClientCache, bool ForceDownload )
        {
            bool blnDownload = false;

            int FolderPortalId = Null.NullInteger;
            string HomeDirectory = Globals.HostMapPath;
            bool isHost = Convert.ToBoolean( settings.ActiveTab.ParentId == settings.SuperTabId ? true : false );
            if( !isHost )
            {
                FolderPortalId = settings.PortalId;
                HomeDirectory = settings.HomeDirectoryMapPath;
            }

            // get file
            FileController objFiles = new FileController();
            Services.FileSystem.FileInfo objFile = objFiles.GetFileById( FileId, FolderPortalId );

            if( objFile != null )
            {
                // check folder view permissions
                if( PortalSecurity.IsInRoles( GetRoles( objFile.Folder, FolderPortalId, "READ" ) ) )
                {
                    // serve file
                    byte[] objContent = null;
                    string strCacheKey = "GetFileContent" + FileId.ToString();

                    // check cache
                    if( objFile.IsCached )
                    {
                        objContent = (byte[])DataCache.GetCache( strCacheKey );
                    }

                    if( Information.IsNothing( objContent ) )
                    {
                        // get file from storage location
                        objContent = GetFileContent( objFile, FolderPortalId, HomeDirectory );

                        if( objFile.IsCached )
                        {
                            // insert into cache
                            int intCacheTimeout = 20*Convert.ToInt32( Globals.PerformanceSetting );
                            DataCache.SetCache( strCacheKey, objContent, TimeSpan.FromMinutes( intCacheTimeout ) );
                        }
                    }

                    HttpResponse objResponse = HttpContext.Current.Response;

                    // client side caching
                    if( ClientCache )
                    {
                        objResponse.Cache.SetExpires( DateTime.Now.AddDays( 1 ) );
                        objResponse.Cache.SetCacheability( HttpCacheability.Public );
                        objResponse.Cache.SetValidUntilExpires( false );
                        objResponse.Cache.VaryByParams["fileticket"] = true;
                    }
                    else
                    {
                        objResponse.Cache.SetExpires( DateTime.Now.AddDays( -1 ) );
                        objResponse.Cache.SetCacheability( HttpCacheability.NoCache );
                    }

                    // force download dialog
                    if( ForceDownload )
                    {
                        objResponse.AppendHeader( "content-disposition", "attachment; filename=" + objFile.FileName );
                    }
                    else
                    {
                        //use proper file name when browser forces download because of file type (save as name should match file name)
                        objResponse.AppendHeader( "content-disposition", "inline; filename=" + objFile.FileName );
                    }

                    // stream to response
                    objResponse.ContentType = "application/octet-stream";
                    objResponse.Clear();
                    objResponse.BufferOutput = true;
                    objResponse.OutputStream.Write( objContent, 0, objContent.Length );
                    objResponse.Flush();
                    objResponse.End();

                    blnDownload = true;
                }
            }

            return blnDownload;
        }

        public static string FormatFolderPath( string strFolderPath )
        {
            if( strFolderPath == "" )
            {
                return "";
            }
            else
            {
                if( strFolderPath.EndsWith( "/" ) )
                {
                    return strFolderPath;
                }
                else
                {
                    return strFolderPath + "/";
                }
            }
        }

        /// <summary>
        /// gets the content type based on the extension
        /// </summary>
        /// <param name="extension">The extension</param>
        /// <remarks>
        /// </remarks>
        public static string GetContentType( string extension )
        {
            string contentType;

            switch( extension.ToLower() )
            {
                case "txt":

                    contentType = "text/plain";
                    break;
                case "htm":
                    contentType = "text/html";
                    break;

                case "html":

                    contentType = "text/html";
                    break;
                case "rtf":

                    contentType = "text/richtext";
                    break;
                case "jpg":
                    contentType = "image/jpeg";
                    break;

                case "jpeg":

                    contentType = "image/jpeg";
                    break;
                case "gif":

                    contentType = "image/gif";
                    break;
                case "bmp":

                    contentType = "image/bmp";
                    break;
                case "mpg":
                    contentType = "video/mpeg";
                    break;

                case "mpeg":

                    contentType = "video/mpeg";
                    break;
                case "avi":

                    contentType = "video/avi";
                    break;
                case "pdf":

                    contentType = "application/pdf";
                    break;
                case "doc":
                    contentType = "application/msword";
                    break;

                case "dot":

                    contentType = "application/msword";
                    break;
                case "csv":
                    contentType = "application/x-msexcel";
                    break;

                case "xls":
                    contentType = "application/x-msexcel";
                    break;

                case "xlt":

                    contentType = "application/x-msexcel";
                    break;
                default:

                    contentType = "application/octet-stream";
                    break;
            }

            return contentType;
        }

        public static byte[] GetFileContent( Services.FileSystem.FileInfo objFile, int PortalId, string HomeDirectory )
        {
            Stream objStream = GetFileStream( objFile, PortalId, HomeDirectory );
            BinaryReader objBinaryReader = new BinaryReader( objStream );
            byte[] objContent = objBinaryReader.ReadBytes( Convert.ToInt32( objStream.Length ) );
            objBinaryReader.Close();

            return objContent;
        }

        /// <summary>
        /// Gets the filename for a file path
        /// </summary>
        /// <param name="filePath">The full name of the file</param>
        private static string GetFileName( string filePath )
        {
            return Path.GetFileName( filePath ).Replace( Globals.glbProtectedExtension, "" );
        }

        public static Stream GetFileStream( Services.FileSystem.FileInfo objFile, int PortalId, string HomeDirectory )
        {
            FileController objFileController = new FileController();
            string strFileName;
            Stream fileStream = null;

            switch( objFile.StorageLocation )
            {
                case (int)FolderController.StorageLocationTypes.InsecureFileSystem:

                    // read from file system
                    strFileName = HomeDirectory + objFile.Folder + objFile.FileName;
                    fileStream = new FileStream( strFileName, FileMode.Open, FileAccess.Read );
                    break;
                case (int)FolderController.StorageLocationTypes.SecureFileSystem:

                    // read from file system
                    strFileName = HomeDirectory + objFile.Folder + objFile.FileName + Globals.glbProtectedExtension;
                    fileStream = new FileStream( strFileName, FileMode.Open, FileAccess.Read );
                    break;
                case (int)FolderController.StorageLocationTypes.DatabaseSecure:

                    // read from database
                    fileStream = new MemoryStream( objFileController.GetFileContent( objFile.FileId, PortalId ) );
                    break;
            }

            return fileStream;
        }

        /// <summary>
        /// Gets all the folders for a Portal
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <remarks>
        /// </remarks>
        public static ArrayList GetFolders( int PortalID )
        {
            ArrayList arrFolders = (ArrayList)DataCache.GetCache( "Folders:" + PortalID.ToString() );
            if( arrFolders == null )
            {
                FolderController objFolderController = new FolderController();
                arrFolders = objFolderController.GetFoldersByPortal( PortalID );
                DataCache.SetCache( "Folders:" + PortalID.ToString(), arrFolders );
            }
            return arrFolders;
        }

        /// <summary>
        /// Gets all the subFolders for a Parent
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <remarks>
        /// </remarks>
        public static ArrayList GetFoldersByParentFolder( int PortalId, string ParentFolder )
        {
            ArrayList folders = GetFolders( PortalId );
            ArrayList subFolders = new ArrayList();
            foreach( FolderInfo folder in folders )
            {
                string strfolderPath = folder.FolderPath;
                if( folder.FolderPath.IndexOf( ParentFolder ) > -1 && folder.FolderPath != Null.NullString && folder.FolderPath != ParentFolder )
                {
                    if( ParentFolder == Null.NullString )
                    {
                        if( strfolderPath.IndexOf( "/" ) == strfolderPath.LastIndexOf( "/" ) )
                        {
                            subFolders.Add( folder );
                        }
                    }
                    else if( strfolderPath.StartsWith( ParentFolder ) )
                    {
                        strfolderPath = strfolderPath.Substring( ParentFolder.Length + 1 );
                        if( strfolderPath.IndexOf( "/" ) == strfolderPath.LastIndexOf( "/" ) )
                        {
                            subFolders.Add( folder );
                        }
                    }
                }
            }

            return subFolders;
        }

        /// <summary>
        /// Gets the Roles that have a particualr Permission for a Folder
        /// </summary>
        /// <param name="Folder">The Folder</param>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <param name="Permission">The Permissions to find</param>
        /// <remarks>
        /// </remarks>
        public static string GetRoles( string Folder, int PortalId, string Permission )
        {
            StringBuilder Roles = new StringBuilder();
            FolderPermissionController objFolderPermissionController = new FolderPermissionController();

            FolderPermissionCollection objCurrentFolderPermissions;
            objCurrentFolderPermissions = objFolderPermissionController.GetFolderPermissionsCollectionByFolderPath( PortalId, Folder );
            FolderPermissionInfo objFolderPermission;
            foreach( FolderPermissionInfo tempLoopVar_objFolderPermission in objCurrentFolderPermissions )
            {
                objFolderPermission = tempLoopVar_objFolderPermission;
                if( objFolderPermission.AllowAccess && objFolderPermission.PermissionKey == Permission )
                {
                    Roles.Append( objFolderPermission.RoleName );
                    Roles.Append( ";" );
                }
            }
            return Roles.ToString();
        }

        /// <summary>
        /// Moves (Renames) a File
        /// </summary>
        /// <param name="strSourceFile">The original File Name</param>
        /// <param name="strDestFile">The new File Name</param>
        /// <param name="settings">The Portal Settings for the Portal/Host Account</param>
        /// <remarks>
        /// </remarks>
        public static string MoveFile( string strSourceFile, string strDestFile, PortalSettings settings )
        {
            int FolderPortalId = Null.NullInteger;
            string HomeDirectory = Globals.HostMapPath;
            bool isHost = Convert.ToBoolean( settings.ActiveTab.ParentId == settings.SuperTabId ? true : false );
            if( !isHost )
            {
                FolderPortalId = settings.PortalId;
                HomeDirectory = settings.HomeDirectoryMapPath;
            }
            return UpdateFile( strSourceFile, strDestFile, FolderPortalId, HomeDirectory, false, false, true );
        }

        public static string RemoveTrailingSlash( string strSource )
        {
            if( strSource == "" )
            {
                return "";
            }
            if( strSource.Substring( strSource.Length - 1, 1 ) == "\\" || strSource.Substring( strSource.Length - 1, 1 ) == "/" )
            {
                return strSource.Substring( 0, strSource.Length - 1 );
            }
            else
            {
                return strSource;
            }
        }

        public static string StripFolderPath( string strOrigPath )
        {
            if( strOrigPath.IndexOf( "\\" ) != -1 )
            {
                return Strings.Replace( strOrigPath, "0\\", "", 1, 1, 0 );
            }
            else
            {
                return Strings.Replace( strOrigPath, "0", "", 1, 1, 0 );
            }
        }

        /// <summary>
        /// Unzips a File
        /// </summary>
        /// <param name="fileName">The zip File Name</param>
        /// <param name="DestFolder">The folder where the file is extracted to</param>
        /// <param name="settings">The Portal Settings for the Portal/Host Account</param>
        /// <remarks>
        /// </remarks>
        public static string UnzipFile( string fileName, string DestFolder, PortalSettings settings )
        {
            int FolderPortalId = Null.NullInteger;
            string HomeDirectory = Globals.HostMapPath;
            bool isHost = Convert.ToBoolean( settings.ActiveTab.ParentId == settings.SuperTabId ? true : false );
            if( !isHost )
            {
                FolderPortalId = settings.PortalId;
                HomeDirectory = settings.HomeDirectoryMapPath;
            }

            PortalController objPortalController = new PortalController();
            FolderController objFolderController = new FolderController();
            FileController objFileController = new FileController();
            string sourceFolderName = Globals.GetSubFolderPath( fileName );
            string sourceFileName = GetFileName( fileName );
            FolderInfo folder = objFolderController.GetFolder( FolderPortalId, sourceFolderName );
            Services.FileSystem.FileInfo file = objFileController.GetFile( sourceFileName, FolderPortalId, sourceFolderName );
            int storageLocation = folder.StorageLocation;
            ZipInputStream objZipInputStream;
            ZipEntry objZipEntry;
            string strMessage = "";
            string strFileName = "";
            string strExtension;

            //Get the source Content from wherever it is

            //Create a Zip Input Stream
            try
            {
                objZipInputStream = new ZipInputStream( GetFileStream( file, FolderPortalId, HomeDirectory ) );
            }
            catch( Exception ex )
            {
                return ex.Message;
            }

            objZipEntry = objZipInputStream.GetNextEntry();
            while( objZipEntry != null )
            {
                if( objZipEntry.IsDirectory )
                {
                    try
                    {
                        AddFolder( settings, DestFolder, objZipEntry.Name, storageLocation );
                    }
                    catch( Exception ex )
                    {
                        objZipInputStream.Close();
                        return ex.Message;
                    }
                }
                objZipEntry = objZipInputStream.GetNextEntry();
            }

            //Recreate the Zip Input Stream and parse it for the files
            objZipInputStream = new ZipInputStream( GetFileStream( file, FolderPortalId, HomeDirectory ) );
            objZipEntry = objZipInputStream.GetNextEntry();
            while( objZipEntry != null )
            {
                if( !objZipEntry.IsDirectory )
                {
                    if( objPortalController.HasSpaceAvailable( FolderPortalId, objZipEntry.Size ) || isHost )
                    {
                        strFileName = Path.GetFileName( objZipEntry.Name );
                        if( strFileName != "" )
                        {
                            strExtension = Path.GetExtension( strFileName ).Replace( ".", "" );
                            if( Strings.InStr( 1, "," + settings.HostSettings["FileExtensions"].ToString().ToLower(), "," + strExtension.ToLower(), 0 ) != 0 || isHost )
                            {
                                try
                                {
                                    string folderPath = Path.GetDirectoryName( DestFolder + objZipEntry.Name.Replace( "/", "\\" ) );
                                    DirectoryInfo Dinfo = new DirectoryInfo( folderPath );
                                    if( !Dinfo.Exists )
                                    {
                                        AddFolder( settings, DestFolder, objZipEntry.Name.Substring( 0, objZipEntry.Name.Replace( "/", "\\" ).LastIndexOf( "\\" ) ) );
                                    }

                                    string zipEntryFileName = DestFolder + objZipEntry.Name.Replace( "/", "\\" );
                                    strMessage += AddFile( FolderPortalId, objZipInputStream, zipEntryFileName, "", objZipEntry.Size, Globals.GetSubFolderPath( zipEntryFileName ), HomeDirectory, false, false );
                                }
                                catch( Exception ex )
                                {
                                    if( objZipInputStream != null )
                                    {
                                        objZipInputStream.Close();
                                    }
                                    return ex.Message;
                                }
                            }
                            else
                            {
                                // restricted file type
                                strMessage += "<br>" + string.Format( Localization.GetString( "RestrictedFileType" ), strFileName, Strings.Replace( settings.HostSettings["FileExtensions"].ToString(), ",", ", *.", 1, -1, 0 ) );
                            }
                        }
                    }
                    else // file too large
                    {
                        strMessage += "<br>" + string.Format( Localization.GetString( "DiskSpaceExceeded" ), strFileName );
                    }
                }

                objZipEntry = objZipInputStream.GetNextEntry();
            }

            objZipInputStream.Close();

            FileController f = new FileController();
            f.GetAllFilesRemoveCache();

            return strMessage;
        }

        /// <summary>
        /// Updates a File
        /// </summary>
        /// <param name="strSourceFile">The original File Name</param>
        /// <param name="strDestFile">The new File Name</param>
        /// <param name="isCopy">Flag determines whether file is to be be moved or copied</param>
        /// <remarks>
        /// </remarks>
        private static string UpdateFile( string strSourceFile, string strDestFile, int PortalId, string HomeDirectory, bool isCopy, bool isNew, bool ClearCache )
        {
            string retValue = "";
            retValue += CheckValidFileName( strSourceFile ) + " ";
            retValue += CheckValidFileName( strDestFile );
            if( retValue.Length > 1 )
            {
                return retValue;
            }
            retValue = "";

            try
            {
                FolderController objFolderController = new FolderController();
                FileController objFileController = new FileController();
                string sourceFolderName = Globals.GetSubFolderPath( strSourceFile );
                string sourceFileName = GetFileName( strSourceFile );
                FolderInfo sourceFolder = objFolderController.GetFolder( PortalId, sourceFolderName );

                string destFileName = GetFileName( strDestFile );
                string destFolderName = Globals.GetSubFolderPath( strDestFile );

                Services.FileSystem.FileInfo file = objFileController.GetFile( sourceFileName, PortalId, sourceFolderName );

                //Get the source Content from wherever it is
                Stream sourceStream = GetFileStream( file, PortalId, HomeDirectory );

                if( isCopy )
                {
                    //Add the new File
                    AddFile( PortalId, sourceStream, strDestFile, "", file.Size, destFolderName, HomeDirectory, true, ClearCache );
                }
                else
                {
                    //Move/Update existing file
                    if( file != null )
                    {
                        objFileController.UpdateFile( PortalId, sourceFileName, destFileName, file.Extension, file.Size, file.Width, file.Height, file.ContentType, sourceFolderName, destFolderName, ClearCache );

                        FolderInfo destinationFolder = objFolderController.GetFolder( PortalId, destFolderName );

                        //Now move the file
                        if( ( sourceFolder != null ) && ( destinationFolder != null ) )
                        {
                            //Write the content to the Destination
                            WriteStream( file.FileId, sourceStream, strDestFile, destinationFolder.StorageLocation, true );

                            //Now we need to clean up the original files
                            if( sourceFolder.StorageLocation == (int)FolderController.StorageLocationTypes.InsecureFileSystem )
                            {
                                //try and delete the Insecure file
                                AttemptFileDeletion( strSourceFile );
                            }
                            if( sourceFolder.StorageLocation == (int)FolderController.StorageLocationTypes.SecureFileSystem )
                            {
                                //try and delete the Secure file
                                AttemptFileDeletion( strSourceFile + Globals.glbProtectedExtension );
                            }
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                retValue = ex.Message;
            }

            return retValue;
        }

        /// <summary>
        /// UploadFile pocesses a single file
        /// </summary>
        /// <param name="RootPath">The folder wherr the file will be put</param>
        /// <param name="objHtmlInputFile">The file to upload</param>
        /// <remarks>
        /// </remarks>
        public static string UploadFile( string RootPath, HttpPostedFile objHtmlInputFile, bool Unzip )
        {
            // Obtain PortalSettings from Current Context
            PortalSettings settings = PortalController.GetCurrentPortalSettings();
            int PortalId = Null.NullInteger;
            string HomeDirectory = Globals.HostMapPath;
            bool isHost = Convert.ToBoolean( settings.ActiveTab.ParentId == settings.SuperTabId ? true : false );
            if( !isHost )
            {
                PortalId = settings.PortalId;
                HomeDirectory = settings.HomeDirectoryMapPath;
            }

            PortalController objPortalController = new PortalController();
            string strMessage = "";
            string strFileName = RootPath + Path.GetFileName( objHtmlInputFile.FileName );
            string strExtension = Path.GetExtension( strFileName ).Replace( ".", "" );
            string strFolderpath = Globals.GetSubFolderPath( strFileName );

            if( ( ( ( ( objPortalController.GetPortalSpaceUsedBytes( PortalId ) + objHtmlInputFile.ContentLength )/Math.Pow( 1024, 2 ) ) <= settings.HostSpace ) || settings.HostSpace == 0 ) || isHost )
            {
                if( Strings.InStr( 1, "," + settings.HostSettings["FileExtensions"].ToString().ToUpper(), "," + strExtension.ToUpper(), 0 ) != 0 || isHost )
                {
                    //Save Uploaded file to server
                    try
                    {
                        strMessage += AddFile( PortalId, objHtmlInputFile.InputStream, strFileName, objHtmlInputFile.ContentType, objHtmlInputFile.ContentLength, strFolderpath, HomeDirectory, true, true );

                        //Optionally Unzip File?
                        if( Path.GetExtension( strFileName ).ToLower() == ".zip" && Unzip == true )
                        {
                            strMessage += UnzipFile( strFileName, RootPath, settings );
                        }
                    }
                    catch( Exception )
                    {
                        // save error - can happen if the security settings are incorrect
                        strMessage += "<br>" + string.Format( Localization.GetString( "SaveFileError" ), strFileName );
                    }
                }
                else
                {
                    // restricted file type
                    strMessage += "<br>" + string.Format( Localization.GetString( "RestrictedFileType" ), strFileName, Strings.Replace( settings.HostSettings["FileExtensions"].ToString(), ",", ", *.", 1, -1, 0 ) );
                }
            }
            else // file too large
            {
                strMessage += "<br>" + String.Format( Localization.GetString( "DiskSpaceExceeded" ), strFileName );
            }
            return strMessage;
        }

        /// <summary>
        /// Adds a Folder
        /// </summary>
        /// <param name="_PortalSettings">Portal Settings for the Portal</param>
        /// <param name="parentFolder">The Parent Folder Name</param>
        /// <param name="newFolder">The new Folder Name</param>
        /// <remarks>
        /// </remarks>
        public static void AddFolder( PortalSettings _PortalSettings, string parentFolder, string newFolder )
        {
            AddFolder( _PortalSettings, parentFolder, newFolder, (int)FolderController.StorageLocationTypes.InsecureFileSystem );
        }

        /// <summary>
        /// Adds a Folder
        /// </summary>
        /// <param name="_PortalSettings">Portal Settings for the Portal</param>
        /// <param name="parentFolder">The Parent Folder Name</param>
        /// <param name="newFolder">The new Folder Name</param>
        /// <param name="StorageLocation">The Storage Location</param>
        /// <remarks>
        /// </remarks>
        public static void AddFolder( PortalSettings _PortalSettings, string parentFolder, string newFolder, int StorageLocation )
        {
            int PortalId;
            string ParentFolderName;
            DirectoryInfo dinfo = new DirectoryInfo( parentFolder );
            DirectoryInfo dinfoNew;

            if( _PortalSettings.ActiveTab.ParentId == _PortalSettings.SuperTabId )
            {
                PortalId = Null.NullInteger;
                ParentFolderName = Globals.HostMapPath;
            }
            else
            {
                PortalId = _PortalSettings.PortalId;
                ParentFolderName = _PortalSettings.HomeDirectoryMapPath;
            }

            dinfoNew = new DirectoryInfo( parentFolder + newFolder );
            if( dinfoNew.Exists )
            {
                throw ( new ArgumentException( Localization.GetString( "EXCEPTION_DirectoryExists" ) ) );
            }
            string strResult = dinfo.CreateSubdirectory( newFolder ).FullName;

            string FolderPath = "";
            FolderPath = strResult.Substring( ParentFolderName.Length ).Replace( "\\", "/" );

            //Persist in Database
            AddFolder( PortalId, _PortalSettings.AdministratorRoleId, FolderPath, StorageLocation );
        }

        /// <summary>
        /// Adds a File to a Zip File
        /// </summary>
        public static void AddToZip( ref ZipOutputStream ZipFile, string filePath, string fileName, string folder )
        {
            Crc32 crc = new Crc32();

            //Open File Stream
            FileStream fs = File.OpenRead( filePath );

            //Read file into byte array buffer
            byte[] buffer;
            buffer = new byte[Convert.ToInt32( fs.Length ) - 1 + 1];
            fs.Read( buffer, 0, buffer.Length );

            //Create Zip Entry
            ZipEntry entry = new ZipEntry( folder + fileName );
            entry.DateTime = DateTime.Now;
            entry.Size = fs.Length;
            fs.Close();
            crc.Reset();
            crc.Update( buffer );
            entry.Crc = crc.Value;

            //Compress file and add to Zip file
            ZipFile.PutNextEntry( entry );
            ZipFile.Write( buffer, 0, buffer.Length );
        }

        /// <summary>
        /// Trys to delete a file from the file system
        /// </summary>
        /// <param name="strFileName">The name of the file</param>
        private static void AttemptFileDeletion( string strFileName )
        {
            if( File.Exists( strFileName ) )
            {
                File.SetAttributes( strFileName, FileAttributes.Normal );
                File.Delete( strFileName );
            }
        }

        /// <summary>
        /// Deletes a folder
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <param name="folder">The Directory Info object to delete</param>
        /// <param name="folderName">The Name of the folder relative to the Root of the Portal</param>
        /// <remarks>
        /// </remarks>
        public static void DeleteFolder( int PortalId, DirectoryInfo folder, string folderName )
        {
            //Delete Folder
            folder.Delete( false );

            //Remove Folder from DataBase
            FolderController objFolderController = new FolderController();
            objFolderController.DeleteFolder( PortalId, folderName.Replace( "\\", "/" ) );
        }

        /// <summary>
        /// Moved directly from FileManager code, probably should make extension lookup more generic
        /// </summary>
        /// <param name="FileLoc">File Location</param>
        /// <remarks>
        /// </remarks>
        public static void DownloadFile( string FileLoc )
        {
            FileInfo objFile = new FileInfo( FileLoc );
            HttpResponse objResponse = HttpContext.Current.Response;
            if( objFile.Exists )
            {
                objResponse.ClearContent();
                objResponse.ClearHeaders();
                objResponse.AppendHeader( "content-disposition", "attachment; filename=" + objFile.Name.ToString() );
                objResponse.AppendHeader( "Content-Length", objFile.Length.ToString() );

                objResponse.ContentType = GetContentType( objFile.Extension.Replace( ".", "" ) );

                WriteFile( objFile.FullName );

                objResponse.Flush();
                objResponse.Close();
            }
        }

        public static void SaveFile( string FullFileName, byte[] Buffer )
        {
            if( File.Exists( FullFileName ) )
            {
                File.SetAttributes( FullFileName, FileAttributes.Normal );
            }
            FileStream fs = new FileStream( FullFileName, FileMode.Create, FileAccess.Write );
            fs.Write( Buffer, 0, Buffer.Length );
            fs.Close();
        }

        /// <summary>
        /// Assigns 1 or more attributes to a file
        /// </summary>
        /// <param name="FileLoc">File Location</param>
        /// <param name="FileAttributesOn">Pass in Attributes you wish to switch on (i.e. FileAttributes.Hidden + FileAttributes.ReadOnly)</param>
        /// <remarks>
        /// </remarks>
        public static void SetFileAttributes( string FileLoc, int FileAttributesOn )
        {
            File.SetAttributes( FileLoc, ( (FileAttributes)FileAttributesOn ) );
        }

        /// <summary>
        /// Sets a Folder Permission
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <param name="FolderId">The Id of the Folder</param>
        /// <param name="PermissionId">The Id of the Permission</param>
        /// <param name="RoleId">The Id of the Role</param>
        /// <param name="relativePath">The folder's Relative Path</param>
        /// <remarks>
        /// </remarks>
        public static void SetFolderPermission( int PortalId, int FolderId, int PermissionId, int RoleId, string relativePath )
        {
            FolderPermissionController objFolderPermissionController = new FolderPermissionController();
            FolderPermissionCollection objCurrentFolderPermissions;
            FolderPermissionInfo objFolderPermissionInfo = new FolderPermissionInfo();
            objCurrentFolderPermissions = objFolderPermissionController.GetFolderPermissionsCollectionByFolderPath( PortalId, relativePath );

            //Iterate current permissions to see if permisison has already been added
            foreach( FolderPermissionInfo tempLoopVar_objFolderPermissionInfo in objCurrentFolderPermissions )
            {
                objFolderPermissionInfo = tempLoopVar_objFolderPermissionInfo;
                if( objFolderPermissionInfo.FolderID == FolderId && objFolderPermissionInfo.PermissionID == PermissionId && objFolderPermissionInfo.RoleID == RoleId && objFolderPermissionInfo.AllowAccess == true )
                {
                    return;
                }
            }

            //Permission not found so Add
            objFolderPermissionInfo = (FolderPermissionInfo)CBO.InitializeObject( objFolderPermissionInfo, typeof( FolderPermissionInfo ) );
            objFolderPermissionInfo.FolderID = FolderId;
            objFolderPermissionInfo.PermissionID = PermissionId;
            objFolderPermissionInfo.RoleID = RoleId;
            objFolderPermissionInfo.AllowAccess = true;
            objFolderPermissionController.AddFolderPermission( objFolderPermissionInfo );
        }

        /// <summary>
        /// Sets a Folders Permissions
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <param name="FolderId">The Id of the Folder</param>
        /// <param name="AdministratorRoleId">The Id of the Administrator Role</param>
        /// <param name="relativePath">The folder's Relative Path</param>
        /// <remarks>
        /// </remarks>
        public static void SetFolderPermissions( int PortalId, int FolderId, int AdministratorRoleId, string relativePath )
        {
            //Set Permissions
            PermissionController objPermissionController = new PermissionController();
            ArrayList Permissions = objPermissionController.GetPermissionsByFolder( PortalId, "" );
            PermissionInfo objPermssionsInfo;
            FolderPermissionController objFolderPermissionController = new FolderPermissionController();
            foreach( PermissionInfo tempLoopVar_objPermssionsInfo in Permissions )
            {
                objPermssionsInfo = tempLoopVar_objPermssionsInfo;
                SetFolderPermission( PortalId, FolderId, objPermssionsInfo.PermissionID, AdministratorRoleId, relativePath );
            }
        }

        public static void Synchronize( int PortalId, int AdministratorRoleId, string HomeDirectory )
        {
            string PhysicalRoot = HomeDirectory;
            string VirtualRoot = "";

            //Call Synchronize Folder that recursively parses the subfolders
            SynchronizeFolder( PortalId, AdministratorRoleId, HomeDirectory, PhysicalRoot, VirtualRoot, true );

            FileController f = new FileController();
            f.GetAllFilesRemoveCache();
        }

        public static void SynchronizeFolder( int PortalId, int AdministratorRoleId, string HomeDirectory, string physicalPath, string relativePath, bool isRecursive )
        {
            string folder;
            string fileName;
            int FolderId;

            if( Directory.Exists( physicalPath ) )
            {
                //Add the folder
                IDataReader dr = DataProvider.Instance().GetFolder( PortalId, relativePath );
                int storageLocation;
                bool IsProtected = false;

                // check to see if the folder exists in the db, if it does then use the current file system storage method
                if( dr.Read() )
                {
                    storageLocation = Convert.ToInt16( dr["StorageLocation"] );
                }
                else
                {
                    // folder is not in db, use default type of storage
                    storageLocation = (int)FolderController.StorageLocationTypes.InsecureFileSystem;
                }
                dr.Close();

                FolderId = AddFolder( PortalId, AdministratorRoleId, relativePath, storageLocation );

                //Get Sub Folders (and synchronize recursively)
                if( isRecursive )
                {
                    string[] folders = Directory.GetDirectories( physicalPath );
                    foreach( string tempLoopVar_folder in folders )
                    {
                        folder = tempLoopVar_folder;
                        DirectoryInfo dir = new DirectoryInfo( folder );
                        string relPath = Null.NullString;
                        if( relativePath == "" )
                        {
                            relPath = dir.Name + "/";
                        }
                        else
                        {
                            relPath = relativePath;
                            if( !relativePath.EndsWith( "/" ) )
                            {
                                relPath = relPath + "/";
                            }
                            relPath = relPath + dir.Name + "/";
                        }
                        SynchronizeFolder( PortalId, AdministratorRoleId, HomeDirectory, folder, relPath, true );
                    }
                }

                //Get Files in this Folder
                string[] files = Directory.GetFiles( physicalPath );
                foreach( string tempLoopVar_fileName in files )
                {
                    fileName = tempLoopVar_fileName;
                    //Add the File if it doesn't exist
                    AddFile( fileName, PortalId, HomeDirectory, false );
                }
            }
        }

        public static void UnzipResources( ZipInputStream zipStream, string destPath )
        {
            ZipEntry objZipEntry;
            string LocalFileName;
            string RelativeDir;
            string FileNamePath;

            objZipEntry = zipStream.GetNextEntry();
            while( objZipEntry != null )
            {
                // This gets the Zipped FileName (including the path)
                LocalFileName = objZipEntry.Name;

                // This creates the necessary directories if they don't
                // already exist.
                RelativeDir = Path.GetDirectoryName( objZipEntry.Name );
                if( ( RelativeDir != string.Empty ) && ( !Directory.Exists( Path.Combine( destPath, RelativeDir ) ) ) )
                {
                    Directory.CreateDirectory( Path.Combine( destPath, RelativeDir ) );
                }

                // This block creates the file using buffered reads from the zipfile
                if( ( !objZipEntry.IsDirectory ) && ( LocalFileName != "" ) )
                {
                    FileNamePath = Path.Combine( destPath, LocalFileName ).Replace( "/", "\\" );

                    try
                    {
                        // delete the file if it already exists
                        if( File.Exists( FileNamePath ) )
                        {
                            File.SetAttributes( FileNamePath, FileAttributes.Normal );
                            File.Delete( FileNamePath );
                        }

                        // create the file
                        FileStream objFileStream = File.Create( FileNamePath );

                        int intSize = 2048;
                        byte[] arrData = new byte[2049];

                        intSize = zipStream.Read( arrData, 0, arrData.Length );
                        while( intSize > 0 )
                        {
                            objFileStream.Write( arrData, 0, intSize );
                            intSize = zipStream.Read( arrData, 0, arrData.Length );
                        }

                        objFileStream.Close();
                    }
                    catch
                    {
                        // an error occurred saving a file in the resource file
                    }
                }

                objZipEntry = zipStream.GetNextEntry();
            }
            if( zipStream != null )
            {
                zipStream.Close();
            }
        }

        /// <summary>
        /// Writes file to response stream.  Workaround offered by MS for large files
        /// http://support.microsoft.com/default.aspx?scid=kb;EN-US;812406
        /// </summary>
        /// <param name="strFileName">FileName</param>
        /// <remarks>
        /// </remarks>
        public static void WriteFile( string strFileName )
        {
            HttpResponse objResponse = HttpContext.Current.Response;
            Stream objStream = null;

            // Buffer to read 10K bytes in chunk:
            byte[] bytBuffer = new byte[10001];

            // Length of the file:
            int intLength;

            // Total bytes to read:
            long lngDataToRead;

            try
            {
                // Open the file.
                objStream = new FileStream( strFileName, FileMode.Open, FileAccess.Read, FileShare.Read );

                // Total bytes to read:
                lngDataToRead = objStream.Length;

                objResponse.ContentType = "application/octet-stream";

                // Read the bytes.
                while( lngDataToRead > 0 )
                {
                    // Verify that the client is connected.
                    if( objResponse.IsClientConnected )
                    {
                        // Read the data in buffer
                        intLength = objStream.Read( bytBuffer, 0, 10000 );

                        // Write the data to the current output stream.
                        objResponse.OutputStream.Write( bytBuffer, 0, intLength );

                        // Flush the data to the HTML output.
                        objResponse.Flush();

                        bytBuffer = new byte[10001]; // Clear the buffer
                        lngDataToRead = lngDataToRead - intLength;
                    }
                    else
                    {
                        //prevent infinite loop if user disconnects
                        lngDataToRead = -1;
                    }
                }
            }
            catch( Exception ex )
            {
                // Trap the error, if any.
                objResponse.Write( "Error : " + ex.Message );
            }
            finally
            {
                if( Information.IsNothing( objStream ) == false )
                {
                    // Close the file.
                    objStream.Close();
                }
            }
        }

        /// <summary>
        /// Writes a Stream to the appropriate File Storage
        /// </summary>
        /// <param name="fileId">The Id of the File</param>
        /// <param name="inStream">The Input Stream</param>
        /// <param name="fileName">The name of the file</param>
        /// <param name="StorageLocation">The type of storage location</param>
        /// <param name="closeInputStream">A flag that dermines if the Input Stream should be closed.</param>
        /// <remarks>
        /// </remarks>
        private static void WriteStream( int fileId, Stream inStream, string fileName, int storageLocation, bool closeInputStream )
        {
            FileController objFileController = new FileController();

            //Clear any existing content from the db
            objFileController.ClearFileContent( fileId );

            // Buffer to read 10K bytes in chunk:
            byte[] arrData = new byte[2049];
            Stream outStream = null;
            if( storageLocation == (int)FolderController.StorageLocationTypes.DatabaseSecure )
            {
                outStream = new MemoryStream();
            }
            else if( storageLocation == (int)FolderController.StorageLocationTypes.SecureFileSystem )
            {
                outStream = new FileStream( fileName + Globals.glbProtectedExtension, FileMode.Create );
            }
            else if( storageLocation == (int)FolderController.StorageLocationTypes.InsecureFileSystem )
            {
                outStream = new FileStream( fileName, FileMode.Create );
            }

            try
            {
                // Total bytes to read:
                int intLength;
                // Read the data in buffer
                intLength = inStream.Read( arrData, 0, arrData.Length );
                while( intLength > 0 )
                {
                    // Write the data to the current output stream.
                    outStream.Write( arrData, 0, intLength );

                    //Read the next chunk
                    intLength = inStream.Read( arrData, 0, arrData.Length );
                }

                if( storageLocation == (int)FolderController.StorageLocationTypes.DatabaseSecure )
                {
                    outStream.Seek( 0, SeekOrigin.Begin );
                    objFileController.UpdateFileContent( fileId, outStream );
                }
            }
            catch( Exception )
            {
            }
            finally
            {
                if( Information.IsNothing( inStream ) == false && closeInputStream )
                {
                    // Close the file.
                    inStream.Close();
                }
                if( Information.IsNothing( outStream ) == false )
                {
                    // Close the file.
                    outStream.Close();
                }
            }
        }
    }
}