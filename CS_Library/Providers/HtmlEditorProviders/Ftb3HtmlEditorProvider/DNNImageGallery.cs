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
using System.IO;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using FreeTextBoxControls;

namespace DotNetNuke.HtmlEditor
{
    /// <summary>
    /// The DNNImageGallery Class subclasses the FTB ImageGallery to provide additional
    /// security features, and support for the DNN File System
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	07/17/2006  Created
    /// </history>
    public class DNNImageGallery : ImageGallery
    {
        public DNNImageGallery()
        {
            FTB_FolderCreated = Localization.GetString( "FTB_FolderCreated" );
            FTB_FolderCreateError = Localization.GetString( "FTB_FolderCreate.Error" );
            FTB_FolderCreatePermission = Localization.GetString( "FTB_FolderCreate.Permission" );
            FTB_FolderDeleted = Localization.GetString( "FTB_FolderDeleted" );
            FTB_FolderDeleteError = Localization.GetString( "FTB_FolderDelete.Error" );
            FTB_FolderDeletePermission = Localization.GetString( "FTB_FolderDelete.Permission" );
            FTB_ImageDeleted = Localization.GetString( "FTB_ImageDeleted" );
            FTB_ImageDeleteError = Localization.GetString( "FTB_ImageDelete.Error" );
            FTB_ImageDeletePermission = Localization.GetString( "FTB_ImageDelete.Permission" );
            FTB_ImageUploaded = Localization.GetString( "FTB_ImageUploaded" );
            FTB_ImageUploadError = Localization.GetString( "FTB_ImageUpload.Error" );
            FTB_ImageUploadNoFile = Localization.GetString( "FTB_ImageUploadNoFile" );
            FTB_ImageUploadPermission = Localization.GetString( "FTB_ImageUpload.Permission" );
        }

        private string FTB_FolderCreated;
        private string FTB_FolderCreateError;
        private string FTB_FolderCreatePermission;
        private string FTB_FolderDeleted;
        private string FTB_FolderDeleteError;
        private string FTB_FolderDeletePermission;
        private string FTB_ImageDeleted;
        private string FTB_ImageDeleteError;
        private string FTB_ImageDeletePermission;
        private string FTB_ImageUploaded;
        private string FTB_ImageUploadError;
        private string FTB_ImageUploadNoFile;
        private string FTB_ImageUploadPermission;

        public int PortalId
        {
            get
            {
                int portalId = Null.NullInteger;
                if( PortalSettings.ActiveTab.ParentId != PortalSettings.SuperTabId )
                {
                    portalId = PortalSettings.PortalId;
                }
                return portalId;
            }
        }

        public PortalSettings PortalSettings
        {
            get
            {
                return PortalController.GetCurrentPortalSettings();
            }
        }

        private void CreateFolder( string newFolder )
        {
            if( this.AllowDirectoryCreate )
            {
                try
                {
                    string parentFolder = this.Context.Server.MapPath( CurrentImagesFolder );

                    //Can only support Standard File System
                    FileSystemUtils.AddFolder( PortalSettings, parentFolder, newFolder, 0 );

                    this.returnMessage = string.Format( FTB_FolderCreated, newFolder );
                }
                catch( Exception )
                {
                    this.returnMessage = FTB_FolderCreateError;
                }
            }
            else
            {
                this.returnMessage = FTB_FolderCreatePermission;
            }

            //Clear the Folders Cache
            DataCache.RemoveCache( "Folders:" + PortalId );
        }

        private void DeleteFolder( string filePath )
        {
            if( this.AllowDirectoryDelete )
            {
                try
                {
                    DirectoryInfo folder = new DirectoryInfo( Context.Server.MapPath( CurrentImagesFolder ) + "\\" + filePath );
                    string folderName = folder.FullName.Replace( Context.Server.MapPath( RootImagesFolder ) + "\\", "" );

                    FileSystemUtils.DeleteFolder( PortalId, folder, folderName );

                    this.returnMessage = string.Format( FTB_FolderDeleted, filePath );
                }
                catch( Exception )
                {
                    this.returnMessage = FTB_FolderDeleteError;
                }
            }
            else
            {
                this.returnMessage = FTB_FolderDeletePermission;
            }

            //Clear the Folders Cache
            DataCache.RemoveCache( "Folders:" + PortalId );
        }

        private void DeleteImage( string filePath )
        {
            if( this.AllowImageDelete )
            {
                try
                {
                    DirectoryInfo folder = new DirectoryInfo( Context.Server.MapPath( CurrentImagesFolder ) );
                    string sourcefile = folder.FullName + "\\" + filePath;

                    FileSystemUtils.DeleteFile( sourcefile, PortalSettings );

                    this.returnMessage = string.Format( FTB_ImageDeleted, filePath );
                }
                catch( Exception )
                {
                    this.returnMessage = FTB_ImageDeleteError;
                }
            }
            else
            {
                this.returnMessage = FTB_ImageDeletePermission;
            }
        }

        private void UploadImage( string filePath )
        {
            if( this.AllowImageUpload )
            {
                try
                {
                    string parentFolder = this.Context.Server.MapPath( CurrentImagesFolder ) + "\\";
                    EnsureChildControls();
                    if( this.inputFile == null || this.inputFile.PostedFile == null )
                    {
                        this.returnMessage = FTB_ImageUploadNoFile;
                    }
                    else
                    {
                        string strMessage = FileSystemUtils.UploadFile( parentFolder, this.inputFile.PostedFile, false );

                        if( !String.IsNullOrEmpty(strMessage) )
                        {
                            string strFileName = parentFolder + Path.GetFileName( this.inputFile.PostedFile.FileName );
                            strMessage = strMessage.Replace( "<br>", "" );
                            strMessage = strMessage.Replace( strFileName, this.inputFile.PostedFile.FileName );
                            this.returnMessage = strMessage;
                        }
                        else
                        {
                            this.returnMessage = string.Format( FTB_ImageUploaded, filePath );
                        }
                    }
                }
                catch( Exception )
                {
                    this.returnMessage = FTB_ImageUploadError;
                }
            }
            else
            {
                this.returnMessage = FTB_ImageUploadPermission;
            }
        }

        public override void RaisePostBackEvent( string eventArgument )
        {
            string command = Null.NullString;
            string param = Null.NullString;
            string[] argumentArray = eventArgument.Split( new char[] {':'} );
            if( argumentArray.Length > 0 && argumentArray[0] != null )
            {
                command = argumentArray[0];
            }
            if( argumentArray.Length > 1 && argumentArray[1] != null )
            {
                param = argumentArray[1];
            }

            switch( command )
            {
                case "CreateFolder":

                    CreateFolder( param );
                    break;
                case "DeleteFolder":

                    DeleteFolder( param );
                    break;
                case "DeleteImage":

                    DeleteImage( param );
                    break;
                case "UploadImage":

                    UploadImage( param );
                    break;
                default:

                    base.RaisePostBackEvent( eventArgument );
                    break;
            }
        }
    }
}