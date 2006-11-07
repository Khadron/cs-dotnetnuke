#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.FileSystem;
using FreeTextBoxControls;
using FileInfo=System.IO.FileInfo;
//using Common;
//using Utilities;
//using Portals;
//using Security;
//using Services.FileSystem;

namespace DotNetNuke.HtmlEditor
{
    /// <summary>
    /// The FTBImageGallery Class provides the Image Gallery for the FTB Provider
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	12/14/2004  documented and updated to work with FTB3.0
    /// </history>
    public class FTBImageGallery : PageBase
    {
        protected DNNImageGallery imgGallery;

        protected string CurrentFolder
        {
            get
            {
                string strCurrentFolder = Server.MapPath( imgGallery.CurrentImagesFolder );
                if( ! strCurrentFolder.EndsWith( "\\" ) )
                {
                    strCurrentFolder += "\\";
                }

                if( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId )
                {
                    strCurrentFolder = strCurrentFolder.Substring( Globals.HostMapPath.Length );
                }
                else
                {
                    strCurrentFolder = strCurrentFolder.Substring( PortalSettings.HomeDirectoryMapPath.Length );
                }

                return strCurrentFolder.Replace( "\\", "/" );
            }
        }

        protected int FolderPortalId
        {
            get
            {
                if( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId )
                {
                    return Null.NullInteger;
                }
                else
                {
                    return PortalSettings.PortalId;
                }
            }
        }

        private void InitFolderAccess()
        {
            bool AllowWriteAccess = false;
            bool AllowReadAccess = false;
            try
            {
                // Check permissions for current directory
                string roles = FileSystemUtils.GetRoles( CurrentFolder, PortalSettings.PortalId, "WRITE" );
                if( PortalSecurity.IsInRoles( roles ) )
                {
                    AllowWriteAccess = true;
                }
                roles = FileSystemUtils.GetRoles( CurrentFolder, PortalSettings.PortalId, "READ" );
                if( PortalSecurity.IsInRoles( roles ) )
                {
                    AllowReadAccess = true;
                }
            }
            catch( Exception ) //wrong directory
            {
                AllowWriteAccess = false;
                AllowReadAccess = false;
            }
            // Set WRITE rights
            imgGallery.AllowDirectoryCreate = AllowWriteAccess;
            imgGallery.AllowDirectoryDelete = AllowWriteAccess;
            imgGallery.AllowImageDelete = AllowWriteAccess;
            imgGallery.AllowImageUpload = AllowWriteAccess;

            if( ! AllowReadAccess )
            {
                //NO ACCESS
                FileInfo[] noimage = new FileInfo[] {};
                imgGallery.CurrentImages = noimage;
            }
        }

        private void Page_Load( Object sender, EventArgs e )
        {
            // set page title
            string strTitle = PortalSettings.PortalName + " > Image Gallery";

            // show copyright credits?
            if( Globals.GetHashValue( Globals.HostSettings["Copyright"], "Y" ) == "Y" )
            {
                strTitle += " ( DNN " + PortalSettings.Version + " )";
            }

            Title = strTitle;
            imgGallery.JavaScriptLocation = ResourceLocation.ExternalFile;
            imgGallery.UtilityImagesLocation = ResourceLocation.ExternalFile;
            imgGallery.SupportFolder = Globals.ResolveUrl( "~/Providers/HtmlEditorProviders/Ftb3HtmlEditorProvider/ftb3/" );
        }

        private void Page_PreRender( object sender, EventArgs e )
        {
            InitFolderAccess();

            //Get the list of sub-directories
            ArrayList arrFolders = FileSystemUtils.GetFoldersByParentFolder( FolderPortalId, CurrentFolder );
            ArrayList alFolders = new ArrayList();

            foreach( FolderInfo folder in arrFolders )
            {
                if( folder.StorageLocation == (int)FolderController.StorageLocationTypes.InsecureFileSystem )
                {
                    string roles = FileSystemUtils.GetRoles( folder.FolderPath, PortalSettings.PortalId, "READ" );
                    if( PortalSecurity.IsInRoles( roles ) )
                    {
                        //add folder to list
                        alFolders.Add( folder.FolderName );
                    }
                }
            }

            imgGallery.CurrentDirectories = (string[])alFolders.ToArray( typeof( String ) );
        }
    }
}