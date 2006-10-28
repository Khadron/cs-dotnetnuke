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
        public int AddFile(FileInfo file, string FolderPath)
        {
            return AddFile(file.PortalId, file.FileName, file.Extension, file.Size, file.Width, file.Height, file.ContentType, FolderPath);
        }

        public int AddFile(int PortalId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string FolderPath)
        {
            return AddFile(PortalId, FileName, Extension, Size, Width, Height, ContentType, FolderPath, true);
        }

        public int AddFile(int PortalId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string FolderPath, bool ClearCache)
        {
            FolderController objFolders = new FolderController();

            FolderInfo objFolder = objFolders.GetFolder(PortalId, FolderPath);

            return AddFile(PortalId, FileName, Extension, Size, Width, Height, ContentType, FolderPath, objFolder.FolderID, ClearCache);
        }

        public int AddFile(int PortalId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string FolderPath, int FolderID, bool ClearCache)
        {
            FolderPath = FileSystemUtils.FormatFolderPath(FolderPath);

            bool IsDirty = false;
            int FileId = 0;
            try
            {
                DataTable dt;
                dt = GetAllFiles();
                DataRow[] dr;
                DataRow OriginalFile;
                dr = dt.Select("FileName=\'" + FileName + "\' and PortalId " + (PortalId == Null.NullInteger ? "IS NULL" : "=" + PortalId.ToString()).ToString() + " and Folder=\'" + FolderPath + "\'");

                if (dr.Length > 0)
                {
                    OriginalFile = dr[0];
                    FileId = Convert.ToInt32(OriginalFile["FileId"]);
                    if (FileChanged(OriginalFile, FileName, Extension, Size, Width, Height, ContentType, FolderPath))
                    {
                        DataProvider.Instance().UpdateFile(FileId, FileName, Extension, Size, Width, Height, ContentType, FolderPath, FolderID);
                        IsDirty = true;
                    }
                }
                else
                {
                    FileId = DataProvider.Instance().AddFile(PortalId, FileName, Extension, Size, Width, Height, ContentType, FolderPath, FolderID);
                    IsDirty = true;
                }

                return FileId;
            }
            finally
            {
                if (IsDirty && ClearCache)
                {
                    DataCache.RemoveCache("GetFileById" + FileId.ToString());
                    GetAllFilesRemoveCache();
                }
            }
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
            FileInfo objFile = objFiles.GetFile(FileName, PortalID, FolderName);
            if (objFile != null)
            {
                FileId = objFile.FileId;
            }

            return FileId;
        }

        public void DeleteFile(int PortalId, string FileName, string FolderPath)
        {
            DeleteFile(PortalId, FileName, FolderPath, true);
        }

        public void DeleteFile(int PortalId, string FileName, string FolderPath, bool ClearCache)
        {
            FolderController objFolders = new FolderController();

            FolderInfo objFolder = objFolders.GetFolder(PortalId, FolderPath);

            DeleteFile(PortalId, FileName, objFolder.FolderID, ClearCache);
        }

        public void DeleteFile(int PortalId, string FileName, int FolderID, bool ClearCache)
        {
            DataProvider.Instance().DeleteFile(PortalId, FileName, FolderID);

            if (ClearCache)
            {
                GetAllFilesRemoveCache();
            }

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

        internal bool FileChanged(DataRow drOriginalFile, string NewFileName, string NewExtension, long NewSize, int NewWidth, int NewHeight, string NewContentType, string NewFolder)
        {
            if (Convert.ToString(drOriginalFile["FileName"]) != NewFileName || Convert.ToString(drOriginalFile["Extension"]) != NewExtension || Convert.ToInt32(drOriginalFile["Size"]) != NewSize || Convert.ToInt32(drOriginalFile["Width"]) != NewWidth || Convert.ToInt32(drOriginalFile["Height"]) != NewHeight || Convert.ToString(drOriginalFile["ContentType"]) != NewContentType || Convert.ToString(drOriginalFile["Folder"]) != NewFolder)
            {
                return true;
            }
            return false;
        }

        public DataTable GetAllFiles()
        {
            DataTable dt = (DataTable)DataCache.GetCache("GetAllFiles");

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

        public FileInfo GetFile(string FilePath, int PortalId)
        {
            return GetFile(Path.GetFileName(FilePath), PortalId, FilePath.Replace(Path.GetFileName(FilePath), ""));
        }

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

        public FileInfo GetFile(string FileName, int PortalId, int FolderID)
        {
            return ((FileInfo)CBO.FillObject(DataProvider.Instance().GetFile(FileName, PortalId, FolderID), typeof(FileInfo)));
        }

        public FileInfo GetFileById(int FileId, int PortalId)
        {
            FileInfo objFile;

            string strCacheKey = "GetFileById" + FileId.ToString();

            objFile = (FileInfo)DataCache.GetCache(strCacheKey);

            if (objFile == null)
            {
                objFile = (FileInfo)CBO.FillObject(DataProvider.Instance().GetFileById(FileId, PortalId), typeof(FileInfo));

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
                objContent = (byte[])dr["Content"];
            }
            dr.Close();
            return objContent;
        }

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

        public IDataReader GetFiles(int PortalId, int FolderID)
        {
            return DataProvider.Instance().GetFiles(PortalId, FolderID);
        }

        public ArrayList GetFilesByFolder(int PortalId, string FolderPath)
        {
            return CBO.FillCollection(GetFiles(PortalId, FolderPath), typeof(FileInfo));
        }

        public void UpdateFile(int PortalId, string OriginalFileName, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string SourceFolder, string DestinationFolder)
        {
            UpdateFile(PortalId, OriginalFileName, FileName, Extension, Size, Width, Height, ContentType, SourceFolder, DestinationFolder, true);
        }

        public void UpdateFile(int PortalId, string OriginalFileName, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string SourceFolder, string DestinationFolder, bool ClearCache)
        {
            FolderController objFolders = new FolderController();

            FolderInfo objFolder = objFolders.GetFolder(PortalId, DestinationFolder);

            UpdateFile(PortalId, OriginalFileName, FileName, Extension, Size, Width, Height, ContentType, SourceFolder, DestinationFolder, objFolder.FolderID, true);
        }

        public void UpdateFile(int PortalId, string OriginalFileName, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string SourceFolder, string DestinationFolder, int FolderID, bool ClearCache)
        {
            bool IsDirty = false;
            try
            {
                DataTable dt;
                dt = GetAllFiles();
                DataRow[] dr;
                DataRow OriginalFile;
                dr = dt.Select("FileName=\'" + OriginalFileName + "\' and PortalId" + (PortalId == Null.NullInteger ? "IS NULL" : "=" + PortalId.ToString()).ToString() + " and Folder=\'" + SourceFolder + "\'");

                int FileId;
                if (dr.Length > 0)
                {
                    OriginalFile = dr[0];
                    FileId = Convert.ToInt32(OriginalFile["FileId"]);
                    if (FileChanged(OriginalFile, FileName, Extension, Size, Width, Height, ContentType, DestinationFolder))
                    {
                        DataProvider.Instance().UpdateFile(FileId, FileName, Extension, Size, Width, Height, ContentType, FileSystemUtils.FormatFolderPath(DestinationFolder), FolderID);
                        IsDirty = true;
                    }
                }
            }
            finally
            {
                if (IsDirty && ClearCache)
                {
                    GetAllFilesRemoveCache();
                }
            }
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
    }
}