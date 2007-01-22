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

        private void UpdateParentFolder( int PortalID, string FolderPath )
        {
            if( !String.IsNullOrEmpty( FolderPath ) )
            {
                string parentFolderPath = FolderPath.Substring( 0, FolderPath.Substring( 0, FolderPath.Length - 1 ).LastIndexOf( "/" ) + 1 );

                FolderInfo objFolder = GetFolder( PortalID, parentFolderPath );
                
                if (objFolder != null)
                {
                    UpdateFolder(objFolder);
                }
            }
        }

        public int AddFolder( int PortalID, string FolderPath )
        {
            return this.AddFolder( PortalID, FolderPath, 0, false, false );
        }

        public int AddFolder( int PortalID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached )
        {
            return AddFolder( PortalID, FolderPath, StorageLocation, IsProtected, IsCached, Null.NullDate );
        }

        public int AddFolder( int PortalID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached, DateTime LastUpdated )
        {
            FolderPath = FileSystemUtils.FormatFolderPath( FolderPath );

            int FolderId;

            IDataReader dr = DataProvider.Instance().GetFolder( PortalID, FolderPath );
            if( dr.Read() )
            {
                FolderId = Convert.ToInt32( dr["FolderId"] );
                DataProvider.Instance().UpdateFolder( PortalID, FolderId, FolderPath, StorageLocation, IsProtected, IsCached, LastUpdated );
            }
            else
            {
                FolderId = DataProvider.Instance().AddFolder( PortalID, FolderPath, StorageLocation, IsProtected, IsCached, LastUpdated );
                UpdateParentFolder( PortalID, FolderPath );
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

        public ArrayList GetFoldersByUser( int PortalID, int UserID, bool IncludeSecure, bool IncludeDatabase, bool AllowAccess, string Permissions )
        {
            return CBO.FillCollection( DataProvider.Instance().GetFoldersByUser( PortalID, UserID, IncludeSecure, IncludeDatabase, AllowAccess, Permissions ), typeof( FolderInfo ) );
        }

        public string GetMappedDirectory( string VirtualDirectory )
        {
            string MappedDir = Convert.ToString( DataCache.GetCache( "DirMap:" + VirtualDirectory ) );
            if( MappedDir == "" )
            {
                MappedDir = FileSystemUtils.AddTrailingSlash( HttpContext.Current.Server.MapPath( VirtualDirectory ) );
                DataCache.SetCache( "DirMap:" + VirtualDirectory, MappedDir );
            }
            return MappedDir;
        }

        public void DeleteFolder( int PortalID, string FolderPath )
        {
            DataProvider.Instance().DeleteFolder( PortalID, FileSystemUtils.FormatFolderPath( FolderPath ) );
            UpdateParentFolder( PortalID, FolderPath );
        }

        public void SetMappedDirectory( string VirtualDirectory )
        {
            try
            {
                string MappedDir = FileSystemUtils.AddTrailingSlash( HttpContext.Current.Server.MapPath( VirtualDirectory ) );
                DataCache.SetCache( "DirMap:" + VirtualDirectory, MappedDir );
            }
            catch( Exception exc )
            {
                Exceptions.Exceptions.LogException( exc );
            }
        }

        public void SetMappedDirectory( string VirtualDirectory, HttpContext context )
        {
            try
            {
                string MappedDir = FileSystemUtils.AddTrailingSlash( context.Server.MapPath( VirtualDirectory ) );
                DataCache.SetCache( "DirMap:" + VirtualDirectory, MappedDir );
            }
            catch( Exception exc )
            {
                Exceptions.Exceptions.LogException( exc );
            }
        }

        public void SetMappedDirectory( PortalInfo portalInfo, HttpContext context )
        {
            try
            {
                string VirtualDirectory = Globals.ApplicationPath + "/" + portalInfo.HomeDirectory + "/";
                SetMappedDirectory( VirtualDirectory, context );
            }
            catch( Exception exc )
            {
                Exceptions.Exceptions.LogException( exc );
            }
        }

        public void UpdateFolder( FolderInfo objFolderInfo )
        {
            DataProvider.Instance().UpdateFolder( objFolderInfo.PortalID, objFolderInfo.FolderID, FileSystemUtils.FormatFolderPath( objFolderInfo.FolderPath ), objFolderInfo.StorageLocation, objFolderInfo.IsProtected, objFolderInfo.IsCached, objFolderInfo.LastUpdated );
        }
    }
}