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
using System.Diagnostics;
using System.IO;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using DataCache=DotNetNuke.Common.Utilities.DataCache;
using Globals=DotNetNuke.Common.Globals;
using Image=System.Drawing.Image;

namespace DotNetNuke.Modules.Admin.Skins
{
    /// <summary>
    /// The EditSkins PortalModuleBase is used to manage the Available Skins
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class EditSkins : PortalModuleBase, IActionable
    {
        protected Label lblMessage;

        private void ShowSkins()
        {
            string strSkinPath = Globals.ApplicationMapPath.ToLower() + cboSkins.SelectedItem.Value;
            cboContainers.ClearSelection();

            string strGallery = "";

            if( cboSkins.SelectedIndex > 0 )
            {
                strGallery += ProcessSkins( strSkinPath );
                strGallery += ProcessContainers( strSkinPath.Replace( "\\" + SkinInfo.RootSkin.ToLower() + "\\", "\\" + SkinInfo.RootContainer.ToLower() + "\\" ) );
                pnlSkin.Visible = true;
                if( UserInfo.IsSuperUser || strSkinPath.IndexOf( Globals.HostMapPath.ToLower() ) == - 1 )
                {
                    cmdParse.Visible = true;
                    pnlParse.Visible = true;
                    cmdDelete.Visible = true;
                }
                else
                {
                    cmdParse.Visible = false;
                    pnlParse.Visible = false;
                    cmdDelete.Visible = false;
                }
            }
            else
            {
                pnlSkin.Visible = false;
                pnlParse.Visible = false;
            }

            lblGallery.Text = strGallery;
        }

        private void ShowContainers()
        {
            string strContainerPath = Globals.ApplicationMapPath.ToLower() + cboContainers.SelectedItem.Value;
            cboSkins.ClearSelection();

            string strGallery = "";

            if( cboContainers.SelectedIndex > 0 )
            {
                strGallery = ProcessContainers( strContainerPath );
                pnlSkin.Visible = true;
                if( UserInfo.IsSuperUser || strContainerPath.IndexOf( Globals.HostMapPath.ToLower() ) == - 1 )
                {
                    cmdParse.Visible = true;
                    pnlParse.Visible = true;
                    cmdDelete.Visible = true;
                }
                else
                {
                    cmdParse.Visible = false;
                    pnlParse.Visible = false;
                    cmdDelete.Visible = false;
                }
            }
            else
            {
                pnlSkin.Visible = false;
                pnlParse.Visible = false;
            }

            lblGallery.Text = strGallery;
        }

        private void LoadSkins()
        {
            string strRoot;
            string strFolder;
            string[] arrFolders;
            string strName;
            string strSkin;

            cboSkins.Items.Clear();
            cboSkins.Items.Add( "<" + Localization.GetString( "Not_Specified" ) + ">" );

            // load host skins
            if( chkHost.Checked )
            {
                strRoot = Request.MapPath( Globals.HostPath + SkinInfo.RootSkin );
                if( Directory.Exists( strRoot ) )
                {
                    arrFolders = Directory.GetDirectories( strRoot );
                    foreach( string tempLoopVar_strFolder in arrFolders )
                    {
                        strFolder = tempLoopVar_strFolder;
                        strName = strFolder.Substring( strFolder.LastIndexOf( "\\" ) + 1 - 1 );
                        strSkin = strFolder.Replace( Globals.ApplicationMapPath, "" );
                        if( strName != "_default" )
                        {
                            cboSkins.Items.Add( new ListItem( strName, strSkin.ToLower() ) );
                        }
                    }
                }
            }

            // load portal skins
            if( chkSite.Checked )
            {
                strRoot = PortalSettings.HomeDirectoryMapPath + SkinInfo.RootSkin;
                if( Directory.Exists( strRoot ) )
                {
                    arrFolders = Directory.GetDirectories( strRoot );
                    foreach( string tempLoopVar_strFolder in arrFolders )
                    {
                        strFolder = tempLoopVar_strFolder;
                        strName = strFolder.Substring( strFolder.LastIndexOf( "\\" ) + 1 - 1 );
                        strSkin = strFolder.Replace( Globals.ApplicationMapPath, "" );
                        cboSkins.Items.Add( new ListItem( strName, strSkin.ToLower() ) );
                    }
                }
            }

            if( ! Page.IsPostBack )
            {
                string strURL;
                if( Request.QueryString["Name"] != null )
                {
                    strURL = Request.MapPath( GetSkinPath( Convert.ToString( Request.QueryString["Type"] ), SkinInfo.RootSkin, Convert.ToString( Request.QueryString["Name"] ) ) );
                }
                else
                {
                    //Get the current portal skin
                    SkinController objSkins = new SkinController();
                    SkinInfo objSkin;
                    string skinSrc;
                    objSkin = SkinController.GetSkin( SkinInfo.RootSkin, PortalSettings.PortalId, SkinType.Portal );
                    if( objSkin != null )
                    {
                        skinSrc = objSkin.SkinSrc;
                    }
                    else
                    {
                        skinSrc = "[G]" + SkinInfo.RootSkin + Globals.glbDefaultSkinFolder + Globals.glbDefaultSkin;
                    }
                    strURL = Request.MapPath( SkinController.FormatSkinPath( SkinController.FormatSkinSrc( skinSrc, PortalSettings ) ) );
                    strURL = strURL.Substring( 0, strURL.LastIndexOf( "\\" ) );
                }
                strSkin = strURL.Replace( Globals.ApplicationMapPath, "" );
                if( cboSkins.Items.FindByValue( strSkin.ToLower() ) != null )
                {
                    cboSkins.Items.FindByValue( strSkin.ToLower() ).Selected = true;
                    ShowSkins();
                }
            }
        }

        private void LoadContainers()
        {
            string strRoot;
            string strFolder;
            string[] arrFolders;
            string strName;
            string strSkin;

            cboContainers.Items.Clear();
            cboContainers.Items.Add( "<" + Localization.GetString( "Not_Specified" ) + ">" );

            // load host containers
            if( chkHost.Checked )
            {
                strRoot = Request.MapPath( Globals.HostPath + SkinInfo.RootContainer );
                if( Directory.Exists( strRoot ) )
                {
                    arrFolders = Directory.GetDirectories( strRoot );
                    foreach( string tempLoopVar_strFolder in arrFolders )
                    {
                        strFolder = tempLoopVar_strFolder;
                        strName = strFolder.Substring( strFolder.LastIndexOf( "\\" ) + 1 - 1 );
                        strSkin = strFolder.Replace( Globals.ApplicationMapPath, "" );
                        if( strName != "_default" )
                        {
                            cboContainers.Items.Add( new ListItem( strName, strSkin.ToLower() ) );
                        }
                    }
                }
            }

            // load portal containers
            if( chkSite.Checked )
            {
                strRoot = PortalSettings.HomeDirectoryMapPath + SkinInfo.RootContainer;
                if( Directory.Exists( strRoot ) )
                {
                    arrFolders = Directory.GetDirectories( strRoot );
                    foreach( string tempLoopVar_strFolder in arrFolders )
                    {
                        strFolder = tempLoopVar_strFolder;
                        strName = strFolder.Substring( strFolder.LastIndexOf( "\\" ) + 1 - 1 );
                        strSkin = strFolder.Replace( Globals.ApplicationMapPath, "" );
                        cboContainers.Items.Add( new ListItem( strName, strSkin.ToLower() ) );
                    }
                }
            }

            if( ! Page.IsPostBack )
            {
                string strURL;
                if( Request.QueryString["Name"] != null )
                {
                    strURL = Request.MapPath( GetSkinPath( Convert.ToString( Request.QueryString["Type"] ), Convert.ToString( Request.QueryString["Root"] ), Convert.ToString( Request.QueryString["Name"] ) ) );
                    strSkin = strURL.Replace( Globals.ApplicationMapPath, "" );
                    if( cboContainers.Items.FindByValue( strSkin.ToLower() ) != null )
                    {
                        cboContainers.Items.FindByValue( strSkin.ToLower() ).Selected = true;
                        ShowContainers();
                    }
                }
            }
        }

        private string ProcessSkins( string strFolderPath )
        {
            string strFile;
            string strFolder;
            string[] arrFiles;
            string strGallery = "";
            string strSkinType = "";
            string strURL;
            int intIndex = 0;

            if( Directory.Exists( strFolderPath ) )
            {
                if( strFolderPath.IndexOf( Globals.HostMapPath.ToLower() ) != - 1 )
                {
                    strSkinType = "G";
                }
                else
                {
                    strSkinType = "L";
                }

                strGallery = "<table border=\"1\" cellspacing=\"0\" cellpadding=\"2\" width=\"100%\">";
                strGallery += "<tr><td align=\"center\" bgcolor=\"#CCCCCC\" class=\"Head\">" + Localization.GetString( "plSkins.Text", this.LocalResourceFile ) + "</td></tr>";
                strGallery += "<tr><td align=\"center\">";
                strGallery += "<table border=\"0\" cellspacing=\"4\" cellpadding=\"4\"><tr>";

                arrFiles = Directory.GetFiles( strFolderPath, "*.ascx" );
                if( arrFiles.Length == 0 )
                {
                    strGallery += "<td align=\"center\" valign=\"bottom\" class=\"NormalBold\">" + Localization.GetString( "NoSkin.ErrorMessage", this.LocalResourceFile ) + "</td>";
                }

                strFolder = strFolderPath.Substring( strFolderPath.LastIndexOf( "\\" ) + 1 - 1 );
                foreach( string tempLoopVar_strFile in arrFiles )
                {
                    strFile = tempLoopVar_strFile;
                    intIndex++;
                    if( intIndex == 4 )
                    {
                        strGallery += "</tr><tr>";
                        intIndex = 0;
                    }

                    // name
                    strFile = strFile.ToLower();
                    strGallery += "<td align=\"center\" valign=\"bottom\" class=\"NormalBold\">";
                    strGallery += Path.GetFileNameWithoutExtension( strFile ) + "<br>";
                    // thumbnail
                    if( File.Exists( strFile.Replace( ".ascx", ".jpg" ) ) )
                    {
                        strURL = strFile.Substring( strFile.IndexOf( "\\portals\\" ) );
                        strGallery += "<a href=\"" + ResolveUrl( "~" + strURL.Replace( ".ascx", ".jpg" ) ) + "\" target=\"_new\"><img src=\"" + CreateThumbnail( strFile.Replace( ".ascx", ".jpg" ) ) + "\" border=\"1\"></a>";
                    }
                    else
                    {
                        strGallery += "<img src=\"" + ResolveUrl( "~/images/thumbnail.jpg" ) + "\" border=\"1\">";
                    }
                    // options
                    strURL = strFile.Substring( strFile.IndexOf( "\\" + SkinInfo.RootSkin.ToLower() + "\\" ) );
                    strURL.Replace( ".ascx", "" );
                    strGallery += "<br><a class=\"CommandButton\" href=\"" + Globals.NavigateURL( PortalSettings.HomeTabId ) + "?SkinSrc=[" + strSkinType + "]" + Globals.QueryStringEncode( strURL.Replace( ".ascx", "" ).Replace( "\\", "/" ) ) + "\" target=\"_new\">" + Localization.GetString( "cmdPreview", this.LocalResourceFile ) + "</a>";
                    strGallery += "&nbsp;&nbsp;|&nbsp;&nbsp;";
                    strGallery += "<a class=\"CommandButton\" href=\"" + Globals.ApplicationPath + Globals.ApplicationURL().Replace( "~", "" ) + "&Root=" + SkinInfo.RootSkin + "&Type=" + strSkinType + "&Name=" + strFolder + "&Src=" + Path.GetFileName( strFile ) + "&action=apply\">" + Localization.GetString( "cmdApply", this.LocalResourceFile ) + "</a>";
                    if( UserInfo.IsSuperUser == true || strSkinType == "L" )
                    {
                        strGallery += "&nbsp;&nbsp;|&nbsp;&nbsp;";
                        strGallery += "<a class=\"CommandButton\" href=\"" + Globals.ApplicationPath + Globals.ApplicationURL().Replace( "~", "" ) + "&Root=" + SkinInfo.RootSkin + "&Type=" + strSkinType + "&Name=" + strFolder + "&Src=" + Path.GetFileName( strFile ) + "&action=delete\">" + Localization.GetString( "cmdDelete" ) + "</a>";
                    }
                    strGallery += "</td>";
                }

                strGallery += "</tr></table></td></tr>";
                if( File.Exists( strFolderPath + "/" + Globals.glbAboutPage ) )
                {
                    strGallery += AddCopyright( strFolderPath + "/" + Globals.glbAboutPage, strFolder );
                }
                strGallery += "</table><br>";
            }

            return strGallery;
        }

        private string ProcessContainers( string strFolderPath )
        {
            string strFile;
            string strFolder;
            string[] arrFiles;
            string strGallery = "";
            string strContainerType = "";
            string strURL;
            int intIndex = 0;

            if( Directory.Exists( strFolderPath ) )
            {
                if( cboContainers.Items.FindByValue( strFolderPath.Replace( Globals.ApplicationMapPath.ToLower(), "" ) ) != null )
                {
                    cboContainers.Items.FindByValue( strFolderPath.Replace( Globals.ApplicationMapPath.ToLower(), "" ) ).Selected = true;
                }

                if( strFolderPath.ToLower().IndexOf( Globals.HostMapPath.ToLower() ) != - 1 )
                {
                    strContainerType = "G";
                }
                else
                {
                    strContainerType = "L";
                }

                strGallery = "<table border=\"1\" cellspacing=\"0\" cellpadding=\"2\" width=\"100%\">";
                strGallery += "<tr><td align=\"center\" bgcolor=\"#CCCCCC\" class=\"Head\">" + Localization.GetString( "plContainers.Text", this.LocalResourceFile ) + "</td></tr>";
                strGallery += "<tr><td align=\"center\">";
                strGallery += "<table border=\"0\" cellspacing=\"4\" cellpadding=\"4\"><tr>";

                arrFiles = Directory.GetFiles( strFolderPath, "*.ascx" );
                if( arrFiles.Length == 0 )
                {
                    strGallery += "<td align=\"center\" valign=\"bottom\" class=\"NormalBold\">" + Localization.GetString( "NoContainer.ErrorMessage", this.LocalResourceFile ) + "</td>";
                }
                strFolder = strFolderPath.Substring( strFolderPath.LastIndexOf( "\\" ) + 1 - 1 );
                foreach( string tempLoopVar_strFile in arrFiles )
                {
                    strFile = tempLoopVar_strFile;
                    intIndex++;
                    if( intIndex == 4 )
                    {
                        strGallery += "</tr><tr>";
                        intIndex = 0;
                    }

                    // name
                    strFile = strFile.ToLower();
                    strGallery += "<td align=\"center\" valign=\"bottom\" class=\"NormalBold\">";
                    strGallery += Path.GetFileNameWithoutExtension( strFile ) + "<br>";
                    // thumbnail
                    if( File.Exists( strFile.Replace( ".ascx", ".jpg" ) ) )
                    {
                        strURL = strFile.Substring( strFile.IndexOf( "\\portals\\" ) );
                        strGallery += "<a href=\"" + ResolveUrl( "~" + strURL.Replace( ".ascx", ".jpg" ) ) + "\" target=\"_new\"><img src=\"" + CreateThumbnail( strFile.Replace( ".ascx", ".jpg" ) ) + "\" border=\"1\"></a>";
                    }
                    else
                    {
                        strGallery += "<img src=\"" + ResolveUrl( "~/images/thumbnail.jpg" ) + "\" border=\"1\">";
                    }
                    // options
                    strURL = strFile.Substring( strFile.IndexOf( "\\" + SkinInfo.RootContainer.ToLower() + "\\" ) );
                    strURL.Replace( ".ascx", "" );
                    strGallery += "<br><a class=\"CommandButton\" href=\"" + Globals.NavigateURL( PortalSettings.HomeTabId ) + "?ContainerSrc=[" + strContainerType + "]" + Globals.QueryStringEncode( strURL.Replace( ".ascx", "" ).Replace( "\\", "/" ) ) + "\" target=\"_new\">" + Localization.GetString( "cmdPreview", this.LocalResourceFile ) + "</a>";
                    strGallery += "&nbsp;&nbsp;|&nbsp;&nbsp;";
                    strGallery += "<a class=\"CommandButton\" href=\"" + Globals.ApplicationPath + Globals.ApplicationURL().Replace( "~", "" ) + "&Root=" + SkinInfo.RootContainer + "&Type=" + strContainerType + "&Name=" + strFolder + "&Src=" + Path.GetFileName( strFile ) + "&action=apply\">" + Localization.GetString( "cmdApply", this.LocalResourceFile ) + "</a>";
                    if( UserInfo.IsSuperUser == true || strContainerType == "L" )
                    {
                        strGallery += "&nbsp;&nbsp;|&nbsp;&nbsp;";
                        strGallery += "<a class=\"CommandButton\" href=\"" + Globals.ApplicationPath + Globals.ApplicationURL().Replace( "~", "" ) + "&Root=" + SkinInfo.RootContainer + "&Type=" + strContainerType + "&Name=" + strFolder + "&Src=" + Path.GetFileName( strFile ) + "&action=delete\">" + Localization.GetString( "cmdDelete" ) + "</a>";
                    }
                    strGallery += "</td>";
                }

                strGallery += "</tr></table></td></tr>";
                if( File.Exists( strFolderPath + "/" + Globals.glbAboutPage ) )
                {
                    strGallery += AddCopyright( strFolderPath + "/" + Globals.glbAboutPage, strFolder );
                }
                strGallery += "</table><br>";
            }

            return strGallery;
        }

        private string AddCopyright( string strFile, string Skin )
        {
            string strGallery = "";
            string strURL;

            strGallery += "<tr><td align=\"center\" bgcolor=\"#CCCCCC\">";
            strURL = strFile.Substring( strFile.IndexOf( "\\portals\\" ) );
            strGallery += "<a class=\"CommandButton\" href=\"" + ResolveUrl( "~" + strURL ) + "\" target=\"_new\">" + string.Format( Localization.GetString( "About", this.LocalResourceFile ), Skin, null ) + "</a>";
            strGallery += "</td></tr>";

            return strGallery;
        }

        private string CreateThumbnail( string strImage )
        {
            bool blnCreate = true;

            string strThumbnail = strImage.Replace( Path.GetFileName( strImage ), "thumbnail_" + Path.GetFileName( strImage ) );

            // check if image has changed
            if( File.Exists( strThumbnail ) )
            {
                DateTime d1 = File.GetLastWriteTime( strThumbnail );
                DateTime d2 = File.GetLastWriteTime( strImage );
                if( File.GetLastWriteTime( strThumbnail ) == File.GetLastWriteTime( strImage ) )
                {
                    blnCreate = false;
                }
            }

            if( blnCreate )
            {
                double dblScale;
                int intHeight;
                int intWidth;

                int intSize = 150; // size of the thumbnail

                Image objImage;
                try
                {
                    objImage = Image.FromFile( strImage );

                    // scale the image to prevent distortion
                    if( objImage.Height > objImage.Width )
                    {
                        //The height was larger, so scale the width
                        dblScale = intSize/objImage.Height;
                        intHeight = intSize;
                        intWidth = Convert.ToInt32( objImage.Width*dblScale );
                    }
                    else
                    {
                        //The width was larger, so scale the height
                        dblScale = intSize/objImage.Width;
                        intWidth = intSize;
                        intHeight = Convert.ToInt32( objImage.Height*dblScale );
                    }

                    // create the thumbnail image
                    Image objThumbnail;
                    objThumbnail = objImage.GetThumbnailImage( intWidth, intHeight, null, IntPtr.Zero );

                    // delete the old file ( if it exists )
                    if( File.Exists( strThumbnail ) )
                    {
                        File.Delete( strThumbnail );
                    }

                    // save the thumbnail image
                    objThumbnail.Save( strThumbnail, objImage.RawFormat );

                    // set the file attributes
                    File.SetAttributes( strThumbnail, FileAttributes.Normal );
                    File.SetLastWriteTime( strThumbnail, File.GetLastWriteTime( strImage ) );

                    // tidy up
                    objImage.Dispose();
                    objThumbnail.Dispose();
                }
                catch
                {
                    // problem creating thumbnail
                }
            }

            strThumbnail = Globals.ApplicationPath + "\\" + strThumbnail.Substring( strThumbnail.IndexOf( "portals\\" ) );

            // return thumbnail filename
            return strThumbnail;
        }

        private string ParseSkinPackage( string strType, string strRoot, string strName, string strFolder, string strParse )
        {
            string strRootPath = String.Empty;
            switch( strType )
            {
                case "G": // global

                    strRootPath = Request.MapPath( Globals.HostPath );
                    break;
                case "L": // local

                    strRootPath = Request.MapPath( PortalSettings.HomeDirectory );
                    break;
            }

            SkinFileProcessor objSkinFiles = new SkinFileProcessor( strRootPath, strRoot, strName );
            ArrayList arrSkinFiles = new ArrayList();

            string strFile;
            string[] arrFiles;

            if( Directory.Exists( strFolder ) )
            {
                arrFiles = Directory.GetFiles( strFolder );
                foreach( string tempLoopVar_strFile in arrFiles )
                {
                    strFile = tempLoopVar_strFile;
                    switch( Path.GetExtension( strFile ) )
                    {
                        case ".htm":
                            if( strFile.ToLower().IndexOf( Globals.glbAboutPage.ToLower() ) < 0 )
                            {
                                arrSkinFiles.Add( strFile );
                            }
                            break;

                        case ".html":
                            if( strFile.ToLower().IndexOf( Globals.glbAboutPage.ToLower() ) < 0 )
                            {
                                arrSkinFiles.Add( strFile );
                            }
                            break;

                        case ".css":

                            if( strFile.ToLower().IndexOf( Globals.glbAboutPage.ToLower() ) < 0 )
                            {
                                arrSkinFiles.Add( strFile );
                            }
                            break;
                        case ".ascx":

                            if( File.Exists( strFile.Replace( ".ascx", ".htm" ) ) == false && File.Exists( strFile.Replace( ".ascx", ".html" ) ) == false )
                            {
                                arrSkinFiles.Add( strFile );
                            }
                            break;
                    }
                }
            }

            if( strParse == "L" )
            {
// localized
                return objSkinFiles.ProcessList( arrSkinFiles, SkinParser.Localized );
            }
            else if( strParse == "P" )
            {
// portable
                return objSkinFiles.ProcessList( arrSkinFiles, SkinParser.Portable );
            }
            return strParse;
        }

        private string GetSkinPath( string Type, string Root, string Name )
        {
            string strPath = null;

            switch( Type )
            {
                case "G": // global

                    strPath = Globals.HostPath + Root + "/" + Name;
                    break;
                case "L": // local

                    strPath = PortalSettings.HomeDirectory + Root + "/" + Name;
                    break;
            }

            return strPath;
        }

        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( Page.IsPostBack == false )
                {
                    ClientAPI.AddButtonConfirm( cmdDelete, Localization.GetString( "DeleteItem" ) );

                    LoadSkins();
                    LoadContainers();

                    if( Request.QueryString["action"] != null )
                    {
                        string strType = Request.QueryString["Type"];
                        string strRoot = Request.QueryString["Root"];
                        string strName = Request.QueryString["Name"];
                        string strSrc = "[" + strType + "]" + strRoot + "/" + strName + "/" + Request.QueryString["Src"];

                        switch( Request.QueryString["action"] )
                        {
                            case "apply":

                                if( strRoot == SkinInfo.RootSkin )
                                {
                                    if( chkPortal.Checked )
                                    {
                                        SkinController.SetSkin( SkinInfo.RootSkin, PortalId, SkinType.Portal, strSrc );
                                    }
                                    if( chkAdmin.Checked )
                                    {
                                        SkinController.SetSkin( SkinInfo.RootSkin, PortalId, SkinType.Admin, strSrc );
                                    }
                                }
                                if( strRoot == SkinInfo.RootContainer )
                                {
                                    if( chkPortal.Checked )
                                    {
                                        SkinController.SetSkin( SkinInfo.RootContainer, PortalId, SkinType.Portal, strSrc );
                                    }
                                    if( chkAdmin.Checked )
                                    {
                                        SkinController.SetSkin( SkinInfo.RootContainer, PortalId, SkinType.Admin, strSrc );
                                    }
                                }
                                DataCache.ClearPortalCache( PortalId, true );
                                Response.Redirect( Request.RawUrl.Replace( "&action=apply", "" ) );
                                break;
                            case "delete":

                                File.Delete( Request.MapPath( SkinController.FormatSkinSrc( strSrc, PortalSettings ) ) );
                                LoadSkins();
                                LoadContainers();
                                break;
                        }
                    }
                }

                if( PortalSettings.ActiveTab.IsSuperTab )
                {
                    typeRow.Visible = false;
                }
                else
                {
                    typeRow.Visible = true;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void cboSkins_SelectedIndexChanged( object sender, EventArgs e )
        {
            ShowSkins();
        }

        protected void cboContainers_SelectedIndexChanged( object sender, EventArgs e )
        {
            ShowContainers();
        }

        protected void chkHost_CheckedChanged( object sender, EventArgs e )
        {
            LoadSkins();
            LoadContainers();

            ShowSkins();
            ShowContainers();
        }

        protected void chkSite_CheckedChanged( object sender, EventArgs e )
        {
            LoadSkins();
            LoadContainers();

            ShowSkins();
            ShowContainers();
        }

        protected void cmdRestore_Click( object sender, EventArgs e )
        {
            if( chkPortal.Checked )
            {
                SkinController.SetSkin( SkinInfo.RootSkin, PortalId, SkinType.Portal, "" );
                SkinController.SetSkin( SkinInfo.RootContainer, PortalId, SkinType.Portal, "" );
            }
            if( chkAdmin.Checked )
            {
                SkinController.SetSkin( SkinInfo.RootSkin, PortalId, SkinType.Admin, "" );
                SkinController.SetSkin( SkinInfo.RootContainer, PortalId, SkinType.Admin, "" );
            }
            DataCache.ClearPortalCache( PortalId, true );
            Response.Redirect( Request.RawUrl );
        }

        protected void cmdDelete_Click( object sender, EventArgs e )
        {
            string strSkinPath = Globals.ApplicationMapPath.ToLower() + cboSkins.SelectedItem.Value;
            string strContainerPath = Globals.ApplicationMapPath.ToLower() + cboContainers.SelectedItem.Value;

            string strMessage;

            if( UserInfo.IsSuperUser == false && cboSkins.SelectedItem.Value.IndexOf( "\\portals\\_default\\", 0 ) != - 1 )
            {
                strMessage = Localization.GetString( "SkinDeleteFailure", this.LocalResourceFile );
                UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessage.ModuleMessageType.RedError );
            }
            else
            {
                if( cboSkins.SelectedIndex > 0 )
                {
                    if( Directory.Exists( strSkinPath ) )
                    {
                        Globals.DeleteFolderRecursive( strSkinPath );
                    }
                    if( Directory.Exists( strSkinPath.Replace( "\\" + SkinInfo.RootSkin.ToLower() + "\\", "\\" + SkinInfo.RootContainer + "\\" ) ) )
                    {
                        Globals.DeleteFolderRecursive( strSkinPath.Replace( "\\" + SkinInfo.RootSkin.ToLower() + "\\", "\\" + SkinInfo.RootContainer + "\\" ) );
                    }
                }

                if( cboContainers.SelectedIndex > 0 )
                {
                    if( Directory.Exists( strContainerPath ) )
                    {
                        Globals.DeleteFolderRecursive( strContainerPath );
                    }
                }
            }

            LoadSkins();
            LoadContainers();

            ShowSkins();
            ShowContainers();
        }

        protected void cmdParse_Click( object sender, EventArgs e )
        {
            string strFolder;
            string strType;
            string strRoot;
            string strName;
            string strSkinPath = Globals.ApplicationMapPath.ToLower() + cboSkins.SelectedItem.Value;
            string strContainerPath = Globals.ApplicationMapPath.ToLower() + cboContainers.SelectedItem.Value;
            string strParse = "";

            if( cboSkins.SelectedIndex > 0 )
            {
                strFolder = strSkinPath;
                if( strFolder.IndexOf( Globals.HostMapPath.ToLower() ) != - 1 )
                {
                    strType = "G";
                }
                else
                {
                    strType = "L";
                }
                strRoot = SkinInfo.RootSkin;
                strName = cboSkins.SelectedItem.Text;
                strParse += ParseSkinPackage( strType, strRoot, strName, strFolder, optParse.SelectedItem.Value );

                strFolder = strSkinPath.Replace( "\\" + SkinInfo.RootSkin.ToLower() + "\\", "\\" + SkinInfo.RootContainer.ToLower() + "\\" );
                strRoot = SkinInfo.RootContainer;
                strParse += ParseSkinPackage( strType, strRoot, strName, strFolder, optParse.SelectedItem.Value );
            }

            if( cboContainers.SelectedIndex > 0 )
            {
                strFolder = strContainerPath;
                if( strFolder.IndexOf( Globals.HostMapPath.ToLower() ) != - 1 )
                {
                    strType = "G";
                }
                else
                {
                    strType = "L";
                }
                strRoot = SkinInfo.RootContainer;
                strName = cboContainers.SelectedItem.Text;
                strParse += ParseSkinPackage( strType, strRoot, strName, strFolder, optParse.SelectedItem.Value );
            }

            lblOutput.Text = strParse;

            if( cboSkins.SelectedIndex > 0 )
            {
                ShowSkins();
            }
            if( cboContainers.SelectedIndex > 0 )
            {
                ShowContainers();
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                if( Convert.ToString( PortalSettings.HostSettings["SkinUpload"] ) != "G" || UserInfo.IsSuperUser )
                {
                    int intPortalId;
                    if( PortalSettings.ActiveTab.IsSuperTab )
                    {
                        intPortalId = Null.NullInteger;
                    }
                    else
                    {
                        intPortalId = PortalId;
                    }
                    ModuleInfo FileManagerModule = ( new ModuleController() ).GetModuleByDefinition( intPortalId, "File Manager" );
                    string[] args = new string[3];

                    args[0] = "mid=" + FileManagerModule.ModuleID;
                    args[1] = "ftype=" + UploadType.Skin.ToString();
                    args[2] = "rtab=" + this.TabId;
                    actions.Add( GetNextActionID(), Localization.GetString( "SkinUpload.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "", Globals.NavigateURL( FileManagerModule.TabID, "Edit", args ), false, SecurityAccessLevel.Admin, true, false );

                    args[1] = "ftype=" + UploadType.Container.ToString();
                    actions.Add( GetNextActionID(), Localization.GetString( "ContainerUpload.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "", Globals.NavigateURL( FileManagerModule.TabID, "Edit", args ), false, SecurityAccessLevel.Admin, true, false );
                }
                return actions;
            }
        }

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
    }
}