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
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Services.FileSystem
{
    /// <Summary>
    /// Business Class that provides access to the Database for the functions within the calling classes
    /// Instantiates the instance of the DataProvider and returns the object, if any
    /// </Summary>
    public class FileController
    {
        internal bool FileChanged(DataRow drOriginalFile, string NewFileName, string NewExtension, long NewSize, int NewWidth, int NewHeight, string NewContentType, string NewFolder)
        {
            if (Convert.ToString(drOriginalFile["FileName"]) != NewFileName || Convert.ToString(drOriginalFile["Extension"]) != NewExtension || Convert.ToInt32(drOriginalFile["Size"]) != NewSize || Convert.ToInt32(drOriginalFile["Width"]) != NewWidth || Convert.ToInt32(drOriginalFile["Height"]) != NewHeight || Convert.ToString(drOriginalFile["ContentType"]) != NewContentType || Convert.ToString(drOriginalFile["Folder"]) != NewFolder)
            {
                return true;
            }
            return false;
        }

        public int AddFile(FileInfo file)
        {
            return AddFile(file.PortalId, file.FileName, file.Extension, file.Size, file.Width, file.Height, file.ContentType, file.Folder, file.FolderId, true);
        }

        public int AddFile(int PortalId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string FolderPath, int FolderID, bool ClearCache)
        {

            FolderPath = FileSystemUtils.FormatFolderPath(FolderPath);
            int FileId = DataProvider.Instance().AddFile(PortalId, FileName, Extension, Size, Width, Height, ContentType, FolderPath, FolderID);
            DataCache.RemoveCache("GetFileById" + FileId);

            if (ClearCache)
            {
                GetAllFilesRemoveCache();
            }
            return FileId;

        }

        
        public void ClearFileContent(int FileId)
        {
            DataProvider.Instance().UpdateFileContent(FileId, null);
        }

        public int ConvertFilePathToFileId(string FilePath, int PortalID)
        {
            string FileName = "";
            string FolderName = "";
            int FileId = -1;

            if (FilePath != "")
            {
                FileName = FilePath.Substring(FilePath.LastIndexOf("/") + 1);
                FolderName = FilePath.Replace(FileName, "");
            }

            FileController objFiles = new FileController();
            FolderController objFolders = new FolderController();
            FolderInfo objFolder = objFolders.GetFolder(PortalID, FolderName);
            if (objFolder != null)
            {
                FileInfo objFile = objFiles.GetFile(FileName, PortalID, objFolder.FolderID);
                if (objFile != null)
                {
                    FileId = objFile.FileId;
                }
            }
            return FileId;

        }

        public void DeleteFile(int PortalId, string FileName, int FolderID, bool ClearCache)
        {

            DataProvider.Instance().DeleteFile(PortalId, FileName, FolderID);

            if (ClearCache)
            {
                GetAllFilesRemoveCache();
            }

        }

        public void DeleteFiles(int PortalId)
        {
            DeleteFiles(PortalId, true);
        }

        public void DeleteFiles(int PortalId, bool ClearCache)
        {

            DataProvider.Instance().DeleteFiles(PortalId);

            if (ClearCache)
            {
                GetAllFilesRemoveCache();
            }

        }

        public DataTable GetAllFiles()
        {
            DataTable dt = (DataTable)(DataCache.GetCache("GetAllFiles"));

            if (dt == null)
            {
                dt = DataProvider.Instance().GetAllFiles();
                DataCache.SetCache("GetAllFiles", dt);
            }
            if (dt != null)
            {
                return dt.Copy();
            }
            else
            {
                return new DataTable();
            }
        }

        public void GetAllFilesRemoveCache()
        {
            DataCache.RemoveCache("GetAllFiles");
        }

        public FileInfo GetFile(string FileName, int PortalId, int FolderID)
        {
            return (FileInfo)(CBO.FillObject(DataProvider.Instance().GetFile(FileName, PortalId, FolderID), typeof(FileInfo)));
        }

        public FileInfo GetFileById(int FileId, int PortalId)
        {
            string strCacheKey = "GetFileById" + FileId;

            FileInfo objFile = (FileInfo)(DataCache.GetCache(strCacheKey));

            if (objFile == null)
            {
                objFile = (FileInfo)(CBO.FillObject(DataProvider.Instance().GetFileById(FileId, PortalId), typeof(FileInfo)));

                if (objFile != null)
                {
                    // cache data
                    int intCacheTimeout = 20 * Convert.ToInt32(Globals.PerformanceSetting);
                    DataCache.SetCache(strCacheKey, objFile, TimeSpan.FromMinutes(intCacheTimeout));
                }
            }

            return objFile;

        }

        public byte[] GetFileContent(int FileId, int PortalId)
        {
            byte[] objContent = null;
            IDataReader dr = DataProvider.Instance().GetFileContent(FileId, PortalId);
            if (dr.Read())
            {
                objContent = (byte[])(dr["Content"]);
            }
            dr.Close();
            return objContent;
        }

        public IDataReader GetFiles(int PortalId, int FolderID)
        {
            return DataProvider.Instance().GetFiles(PortalId, FolderID);
        }

        public void UpdateFile(int FileId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string DestinationFolder, int FolderID)
        {
            DataProvider.Instance().UpdateFile(FileId, FileName, Extension, Size, Width, Height, ContentType, FileSystemUtils.FormatFolderPath(DestinationFolder), FolderID);
        }

        public void UpdateFileContent(int FileId, Stream Content)
        {
            BinaryReader objBinaryReader = new BinaryReader(Content);
            byte[] objContent = objBinaryReader.ReadBytes(Convert.ToInt32(Content.Length));
            objBinaryReader.Close();
            Content.Close();

            DataProvider.Instance().UpdateFileContent(FileId, objContent);
        }

        public void UpdateFileContent(int FileId, byte[] Content)
        {
            DataProvider.Instance().UpdateFileContent(FileId, Content);
        }


        [Obsolete("This function has been replaced by ???")]
        public int AddFile(FileInfo file, string FolderPath)
        {
            FolderController objFolders = new FolderController();
            FolderInfo objFolder = objFolders.GetFolder(file.PortalId, FolderPath);
            file.FolderId = objFolder.FolderID;
            file.Folder = FolderPath;
            return AddFile(file);
        }

        [Obsolete("This function has been replaced by AddFile(PortalId, FileName, Extension, Size, Width, Height, ContentType, FolderPath, FolderID, ClearCache)")]
        public int AddFile(int PortalId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string FolderPath)
        {
            FolderController objFolders = new FolderController();
            FolderInfo objFolder = objFolders.GetFolder(PortalId, FolderPath);
            return AddFile(PortalId, FileName, Extension, Size, Width, Height, ContentType, FolderPath, objFolder.FolderID, true);
        }

        [Obsolete("This function has been replaced by AddFile(PortalId, FileName, Extension, Size, Width, Height, ContentType, FolderPath, FolderID, ClearCache)")]
        public int AddFile(int PortalId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string FolderPath, bool ClearCache)
        {
            FolderController objFolders = new FolderController();
            FolderInfo objFolder = objFolders.GetFolder(PortalId, FolderPath);
            return AddFile(PortalId, FileName, Extension, Size, Width, Height, ContentType, FolderPath, objFolder.FolderID, ClearCache);
        }

        [Obsolete("This function has been replaced by DeleteFile(PortalId, FileName, FolderID, ClearCache)")]
        public void DeleteFile(int PortalId, string FileName, string FolderPath, bool ClearCache)
        {
            FolderController objFolders = new FolderController();
            FolderInfo objFolder = objFolders.GetFolder(PortalId, FolderPath);
            DeleteFile(PortalId, FileName, objFolder.FolderID, ClearCache);
        }

        [Obsolete("This function has been replaced by DeleteFile(PortalId, FileName, FolderID, ClearCache)")]
        public void DeleteFile(int PortalId, string FileName, string FolderPath)
        {
            FolderController objFolders = new FolderController();
            FolderInfo objFolder = objFolders.GetFolder(PortalId, FolderPath);
            DeleteFile(PortalId, FileName, objFolder.FolderID, true);
        }

        [Obsolete("This function has been replaced by GetFile(FileName, PortalId, FolderID)")]
        public FileInfo GetFile(string FilePath, int PortalId)
        {
            FolderController objFolders = new FolderController();
            string FileName = Path.GetFileName(FilePath);
            FolderInfo objFolder = objFolders.GetFolder(PortalId, FilePath.Replace(FileName, ""));
            if (objFolder == null)
            {
                return null;
            }
            else
            {
                return GetFile(FileName, PortalId, objFolder.FolderID);
            }
        }

        [Obsolete("This function has been replaced by GetFile(FileName, PortalId, FolderID)")]
        public FileInfo GetFile(string FileName, int PortalId, string FolderPath)
        {
            FolderController objFolders = new FolderController();
            FolderInfo objFolder = objFolders.GetFolder(PortalId, FolderPath);
            if (objFolder == null)
            {
                return null;
            }
            else
            {
                return GetFile(FileName, PortalId, objFolder.FolderID);
            }
        }

        [Obsolete("This function has been replaced by GetFiles(PortalId, FolderID)")]
        public IDataReader GetFiles(int PortalId, string FolderPath)
        {
            FolderController objFolders = new FolderController();
            FolderInfo objFolder = objFolders.GetFolder(PortalId, FolderPath);
            if (objFolder == null)
            {
                return null;
            }
            return GetFiles(PortalId, objFolder.FolderID);
        }

        [Obsolete("This function has been replaced by ???")]
        public ArrayList GetFilesByFolder(int PortalId, string FolderPath)
        {
            FolderController objFolders = new FolderController();
            FolderInfo objFolder = objFolders.GetFolder(PortalId, FolderPath);
            if (objFolder == null)
            {
                return null;
            }
            return CBO.FillCollection(GetFiles(PortalId, objFolder.FolderID), typeof(FileInfo));
        }

        [Obsolete("This function has been replaced by UpdateFile(FileId, FileName, Extension, Size, Width, Height, ContentType, DestinationFolder, FolderID)")]
        public void UpdateFile(int PortalId, string OriginalFileName, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string SourceFolder, string DestinationFolder)
        {
            FolderController objFolders = new FolderController();
            FolderInfo objFolder = objFolders.GetFolder(PortalId, DestinationFolder);
            FileInfo objFile = GetFile(OriginalFileName, PortalId, objFolder.FolderID);

            if (objFile != null)
            {
                UpdateFile(objFile.FileId, FileName, Extension, Size, Width, Height, ContentType, DestinationFolder, objFolder.FolderID);
            }
        }

        [Obsolete("This function has been replaced by UpdateFile(FileId, FileName, Extension, Size, Width, Height, ContentType, DestinationFolder, FolderID)")]
        public void UpdateFile(int PortalId, string OriginalFileName, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string SourceFolder, string DestinationFolder, bool ClearCache)
        {
            FolderController objFolders = new FolderController();
            FolderInfo objFolder = objFolders.GetFolder(PortalId, DestinationFolder);
            FileInfo objFile = GetFile(OriginalFileName, PortalId, objFolder.FolderID);

            if (objFile != null)
            {
                UpdateFile(objFile.FileId, FileName, Extension, Size, Width, Height, ContentType, DestinationFolder, objFolder.FolderID);
            }
        }

        [Obsolete("This function has been replaced by UpdateFile(FileId, FileName, Extension, Size, Width, Height, ContentType, DestinationFolder, FolderID)")]
        public void UpdateFile(int PortalId, string OriginalFileName, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string SourceFolder, string DestinationFolder, int FolderID, bool ClearCache)
        {
            FileInfo objFile = GetFile(OriginalFileName, PortalId, FolderID);
            if (objFile != null)
            {
                UpdateFile(objFile.FileId, FileName, Extension, Size, Width, Height, ContentType, DestinationFolder, FolderID);
            }
        }


    }
}