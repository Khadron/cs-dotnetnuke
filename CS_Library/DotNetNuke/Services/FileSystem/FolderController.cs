using System;
using System.Collections;
using System.Data;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Services.FileSystem
{
    /// <Summary>
    /// Business Class that provides access to the Database for the functions within the calling classes
    /// Instantiates the instance of the DataProvider and returns the object, if any
    /// </Summary>
    public class FolderController
    {
        public enum StorageLocationTypes
        {
            InsecureFileSystem = 0,
            SecureFileSystem = 1,
            DatabaseSecure = 2,
        }

        public int AddFolder( int PortalID, string FolderPath )
        {
            return this.AddFolder( PortalID, FolderPath, 0, false, false );
        }

        public int AddFolder( int PortalID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached )
        {
            FolderPath = FileSystemUtils.FormatFolderPath(FolderPath);

            int FolderId;

            IDataReader dr = DataProvider.Instance().GetFolder(PortalID, FolderPath);
            if (dr.Read())
            {
                FolderId = Convert.ToInt32(dr["FolderId"]);
                DataProvider.Instance().UpdateFolder(PortalID, FolderId, FolderPath, StorageLocation, IsProtected, IsCached);
            }
            else
            {
                FolderId = DataProvider.Instance().AddFolder(PortalID, FolderPath, StorageLocation, IsProtected, IsCached);
            }
            dr.Close();

            return FolderId;
        }

        public ArrayList GetFolder( int PortalID, int FolderID )
        {
            return CBO.FillCollection( DataProvider.Instance().GetFolder( PortalID, FolderID ), typeof( FolderInfo ) );
        }

        public FolderInfo GetFolder( int PortalID, string FolderPath )
        {
            FolderPath = FileSystemUtils.FormatFolderPath( FolderPath );
            return ( (FolderInfo)CBO.FillObject( DataProvider.Instance().GetFolder( PortalID, FolderPath ), typeof( FolderInfo ) ) );
        }

        public FolderInfo GetFolderInfo( int PortalID, int FolderID )
        {
            return ( (FolderInfo)CBO.FillObject( DataProvider.Instance().GetFolder( PortalID, FolderID ), typeof( FolderInfo ) ) );
        }

        public ArrayList GetFoldersByPortal( int PortalID )
        {
            return CBO.FillCollection( DataProvider.Instance().GetFoldersByPortal( PortalID ), typeof( FolderInfo ) );
        }

        public string GetMappedDirectory( string VirtualDirectory )
        {
            string MappedDir = Convert.ToString(DataCache.GetCache("DirMap:" + VirtualDirectory));
            if (MappedDir == "")
            {
                MappedDir = FileSystemUtils.AddTrailingSlash(HttpContext.Current.Server.MapPath(VirtualDirectory));
                DataCache.SetCache("DirMap:" + VirtualDirectory, MappedDir);
            }
            return MappedDir;
        }

        public void DeleteFolder( int PortalID, string FolderPath )
        {
            DataProvider.Instance().DeleteFolder( PortalID, FileSystemUtils.FormatFolderPath( FolderPath ) );
        }

        public void SetMappedDirectory(string VirtualDirectory)
        {
            try
            {
                string MappedDir = FileSystemUtils.AddTrailingSlash(HttpContext.Current.Server.MapPath(VirtualDirectory));
                DataCache.SetCache("DirMap:" + VirtualDirectory, MappedDir);
            }
            catch (Exception exc)
            {
                Exceptions.Exceptions.LogException(exc);
            }
        }

        public void SetMappedDirectory(string VirtualDirectory, HttpContext context)
        {
            try
            {
                string MappedDir = FileSystemUtils.AddTrailingSlash(context.Server.MapPath(VirtualDirectory));
                DataCache.SetCache("DirMap:" + VirtualDirectory, MappedDir);
            }
            catch (Exception exc)
            {
                Exceptions.Exceptions.LogException(exc);
            }
        }

        public void SetMappedDirectory(PortalInfo portalInfo, HttpContext context)
        {
            try
            {
                string VirtualDirectory = Globals.ApplicationPath + "/" + portalInfo.HomeDirectory + "/";
                SetMappedDirectory(VirtualDirectory, context);
            }
            catch (Exception exc)
            {
                Exceptions.Exceptions.LogException(exc);
            }
        }

        public void UpdateFolder( FolderInfo objFolderInfo )
        {
            DataProvider.Instance().UpdateFolder( objFolderInfo.PortalID, objFolderInfo.FolderID, FileSystemUtils.FormatFolderPath( objFolderInfo.FolderPath ), objFolderInfo.StorageLocation, objFolderInfo.IsProtected, objFolderInfo.IsCached );
        }
    }
}