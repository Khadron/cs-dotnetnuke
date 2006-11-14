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
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Modules.Admin.ResourceInstaller;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;

namespace DotNetNuke.Modules.Admin.FileSystem
{
    /// Project	 : DotNetNuke
    /// Class	 : WebUpload
    /// <summary>
    /// Supplies the functionality for uploading files to the Portal
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///   [cnurse] 16/9/2004  Updated for localization, Help and 508
    /// </history>
    public partial class WebUpload : PortalModuleBase
    {
        private UploadType _FileType; // content files
        private string _FileTypeName;
        private string _DestinationFolder; // content files
        private string _UploadRoles;
        private string _RootFolder;

        public string DestinationFolder
        {
            get
            {
                if( _DestinationFolder == null )
                {
                    _DestinationFolder = string.Empty;
                    if( ( Request.QueryString["dest"] != null ) )
                    {
                        _DestinationFolder = Globals.QueryStringDecode( Request.QueryString["dest"] );
                    }
                }
                return FileSystemUtils.RemoveTrailingSlash( _DestinationFolder.Replace( "\\", "/" ) );
            }
        }

        public UploadType FileType
        {
            get
            {
                _FileType = UploadType.File;
                if( ( Request.QueryString["ftype"] != null ) )
                {
                    //The select statement ensures that the parameter can be converted to UploadType
                    switch( Request.QueryString["ftype"].ToLower() )
                    {
                        case "file":
                            _FileType = (UploadType)Enum.Parse( typeof( UploadType ), Request.QueryString["ftype"] );
                            break;

                        case "container":
                            _FileType = (UploadType)Enum.Parse( typeof( UploadType ), Request.QueryString["ftype"] );
                            break;

                        case "skin":
                            _FileType = (UploadType)Enum.Parse( typeof( UploadType ), Request.QueryString["ftype"] );
                            break;

                        case "module":
                            _FileType = (UploadType)Enum.Parse( typeof( UploadType ), Request.QueryString["ftype"] );
                            break;

                        case "languagepack":

                            _FileType = (UploadType)Enum.Parse( typeof( UploadType ), Request.QueryString["ftype"] );
                            break;
                    }
                }
                return _FileType;
            }
        }

        public string FileTypeName
        {
            get
            {
                if( _FileTypeName == null )
                {
                    _FileTypeName = Localization.GetString( FileType.ToString(), this.LocalResourceFile );
                }
                return _FileTypeName;
            }
        }

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

        public string RootFolder
        {
            get
            {
                if( _RootFolder == null )
                {
                    if( IsHostMenu )
                    {
                        _RootFolder = Globals.HostMapPath;
                    }
                    else
                    {
                        _RootFolder = PortalSettings.HomeDirectoryMapPath;
                    }
                }
                return _RootFolder;
            }
        }

        public string UploadRoles
        {
            get
            {
                if( _UploadRoles == null )
                {
                    _UploadRoles = string.Empty;

                    ModuleController objModules = new ModuleController();
                    //TODO:  Should replace this with a finder method in PortalSettings to look in the cached modules of the activetab - jmb 11/25/2004
                    ModuleInfo ModInfo;

                    if( IsHostMenu )
                    {
                        ModInfo = objModules.GetModuleByDefinition( Null.NullInteger, "File Manager" );
                    }
                    else
                    {
                        ModInfo = objModules.GetModuleByDefinition( PortalId, "File Manager" );
                    }

                    Hashtable settings = PortalSettings.GetModuleSettings( ModInfo.ModuleID );
                    if( Convert.ToString( settings["uploadroles"] ) != null )
                    {
                        _UploadRoles = Convert.ToString( settings["uploadroles"] );
                    }
                }

                return _UploadRoles;
            }
        }

        /// <summary>
        /// This routine checks the Access Security
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///   [cnurse] 1/21/2005  Documented
        /// </history>
        private void CheckSecurity()
        {
            bool DenyAccess = false;
            if( PortalSecurity.IsInRole( PortalSettings.AdministratorRoleName.ToString() ) == false && PortalSecurity.IsInRoles( UploadRoles ) == false )
            {
                DenyAccess = true;
            }
            else
            {
                if( IsAdminMenu )
                {
                    switch( FileType )
                    {
                        case UploadType.LanguagePack:
                            DenyAccess = true;
                            break;

                        case UploadType.Module:

                            DenyAccess = true;
                            break;
                        case UploadType.Skin:
                            if( ( Convert.ToString( PortalSettings.HostSettings["SkinUpload"] ) == "G" ) && ( UserInfo.IsSuperUser == false ) )
                            {
                                DenyAccess = true;
                            }
                            break;

                        case UploadType.Container:

                            if( ( Convert.ToString( PortalSettings.HostSettings["SkinUpload"] ) == "G" ) && ( UserInfo.IsSuperUser == false ) )
                            {
                                DenyAccess = true;
                            }
                            break;
                    }
                }

                if( IsHostMenu )
                {
                    if( ! UserInfo.IsSuperUser )
                    {
                        DenyAccess = true;
                    }
                }
            }

            if( DenyAccess )
            {
                Response.Redirect( Globals.NavigateURL( "Access Denied" ), true );
            }
        }

        /// <summary>
        /// This routine populates the Folder List Drop Down
        /// There is no reference to permissions here as all folders should be available to the admin.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [Philip Beadle]     5/10/2004  Added
        ///     [cnurse]            04/24/2006  Converted to use Database as folder source
        /// </history>
        private void LoadFolders()
        {
            ddlFolders.Items.Clear();

            ArrayList folders = FileSystemUtils.GetFolders( FolderPortalID );
            foreach( FolderInfo folder in folders )
            {
                ListItem FolderItem = new ListItem();
                if( folder.FolderPath == Null.NullString )
                {
                    if( IsHostMenu )
                    {
                        FolderItem.Text = Localization.GetString( "HostRoot", this.LocalResourceFile );
                    }
                    else
                    {
                        FolderItem.Text = Localization.GetString( "PortalRoot", this.LocalResourceFile );
                    }
                }
                else
                {
                    FolderItem.Text = FileSystemUtils.RemoveTrailingSlash( folder.FolderPath );
                }
                FolderItem.Value = folder.FolderPath;
                ddlFolders.Items.Add( FolderItem );
            }

            if( DestinationFolder.Length > 0 )
            {
                if( ddlFolders.Items.FindByText( DestinationFolder ) != null )
                {
                    ddlFolders.Items.FindByText( DestinationFolder ).Selected = true;
                }
            }
        }

        /// <summary>
        /// This routine determines the Return Url
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///   [cnurse] 1/21/2005  Documented
        /// </history>
        public string ReturnURL()
        {
            int TabID = PortalSettings.HomeTabId;

            if( Request.Params["rtab"] != null )
            {
                TabID = int.Parse( Request.Params["rtab"] );
            }
            return Globals.NavigateURL( TabID );
        }


        /// <summary>
        /// The Page_Load runs when the page loads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///   [cnurse] 16/9/2004  Updated for localization, Help and 508
        ///   [VMasanas]  9/28/2004   Changed redirect to Access Denied
        ///   [Philip Beadle]  5/10/2004  Added folder population section.
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                CheckSecurity();

                //Get localized Strings
                string strHost = Localization.GetString( "HostRoot", this.LocalResourceFile );
                string strPortal = Localization.GetString( "PortalRoot", this.LocalResourceFile );

                if( ! Page.IsPostBack )
                {
                    lblUploadType.Text = Localization.GetString( "UploadType" + FileType.ToString(), this.LocalResourceFile );
                    if( FileType == UploadType.File )
                    {
                        trFolders.Visible = true;
                        trRoot.Visible = true;
                        trUnzip.Visible = true;

                        if( IsHostMenu )
                        {
                            lblRootType.Text = strHost + ":";
                            lblRootFolder.Text = RootFolder;
                        }
                        else
                        {
                            lblRootType.Text = strPortal + ":";
                            lblRootFolder.Text = RootFolder;
                        }
                        LoadFolders();
                    }

                    chkUnzip.Checked = false;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// The cmdAdd_Click runs when the Add Button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///   [cnurse] 16/9/2004  Updated for localization, Help and 508
        /// </history>
        protected void cmdAdd_Click( object sender, EventArgs e )
        {
            try
            {
                string strFileName;
                string strExtension;
                string strMessage = "";

                HttpPostedFile postedFile = cmdBrowse.PostedFile;

                //Get localized Strings
                string strInvalid = Localization.GetString( "InvalidExt", this.LocalResourceFile );

                strFileName = Path.GetFileName( postedFile.FileName );
                strExtension = Path.GetExtension( strFileName );

                if( postedFile.FileName != "" )
                {
                    switch( FileType )
                    {
                        case UploadType.File: // content files

                            strMessage += FileSystemUtils.UploadFile( RootFolder + ddlFolders.SelectedItem.Value.Replace( "/", "\\" ), postedFile, chkUnzip.Checked );
                            break;
                        case UploadType.Skin: // skin package

                            if( strExtension.ToLower() == ".zip" )
                            {
                                //SkinController objSkins = new SkinController();
                                Label objLbl = new Label();
                                objLbl.CssClass = "Normal";
                                objLbl.Text = SkinController.UploadSkin( RootFolder, SkinInfo.RootSkin, Path.GetFileNameWithoutExtension( postedFile.FileName ), postedFile.InputStream );
                                phPaLogs.Controls.Add( objLbl );
                            }
                            else
                            {
                                strMessage += strInvalid + " " + FileTypeName + " " + strFileName;
                            }
                            break;
                        case UploadType.Container: // container package

                            if( strExtension.ToLower() == ".zip" )
                            {
                                //SkinController objSkins = new SkinController();
                                Label objLbl = new Label();
                                objLbl.CssClass = "Normal";
                                objLbl.Text = SkinController.UploadSkin( RootFolder, SkinInfo.RootContainer, Path.GetFileNameWithoutExtension( postedFile.FileName ), postedFile.InputStream );
                                phPaLogs.Controls.Add( objLbl );
                            }
                            else
                            {
                                strMessage += strInvalid + " " + FileTypeName + " " + strFileName;
                            }
                            break;
                        case UploadType.Module: // custom module

                            if( strExtension.ToLower() == ".zip" )
                            {
                                phPaLogs.Visible = true;
                                PaInstaller pa = new PaInstaller( postedFile.InputStream, Request.MapPath( "." ) );
                                pa.Install();
                                phPaLogs.Controls.Add( pa.InstallerInfo.Log.GetLogsTable() );
                            }
                            else
                            {
                                strMessage += strInvalid + " " + FileTypeName + " " + strFileName;
                            }
                            break;
                        case UploadType.LanguagePack:

                            if( strExtension.ToLower() == ".zip" )
                            {
                                LocaleFilePackReader objLangPack = new LocaleFilePackReader();
                                phPaLogs.Controls.Add( objLangPack.Install( postedFile.InputStream ).GetLogsTable() );
                            }
                            else
                            {
                                strMessage += strInvalid + " " + FileTypeName + " " + strFileName;
                            }
                            break;
                    }
                }

                if( phPaLogs.Controls.Count > 0 )
                {
                    tblLogs.Visible = true;
                }
                else if( strMessage == "" )
                {
                    Response.Redirect( ReturnURL(), true );
                }
                else
                {
                    lblMessage.Text = strMessage;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// The cmdReturn_Click runs when the Return Button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///   [cnurse] 16/9/2004  Updated for localization, Help and 508
        /// </history>
        protected void cmdReturn_Click( Object sender, EventArgs e )
        {
            Response.Redirect( ReturnURL(), true );
        }
    }
}