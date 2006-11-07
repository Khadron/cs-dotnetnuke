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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using Microsoft.VisualBasic;
using FileInfo=DotNetNuke.Services.FileSystem.FileInfo;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.UI.UserControls
{

    public abstract class UrlControl : UserControlBase
    {
        private string _localResourceFile;

        private int _ModuleID = -2;
        private PortalInfo _objPortal;
        private bool _Required = true;
        private bool _ShowDatabase = true;
        private bool _ShowFiles = true;
        private bool _ShowLog = true;
        private bool _ShowNewWindow = false;
        private bool _ShowNone = false;
        private bool _ShowSecure = true;
        private bool _ShowTabs = true;
        private bool _ShowTrack = true;
        private bool _ShowUpLoad = true;
        private bool _ShowUrls = true;
        private bool _ShowUsers = false;
        private string _Url = "";
        private string _UrlType = "";
        private string _Width = "";
        protected DropDownList cboFiles;
        protected DropDownList cboFolders;
        protected DropDownList cboTabs;
        protected DropDownList cboUrls;
        protected CheckBox chkLog;

        protected CheckBox chkNewWindow;
        protected CheckBox chkTrack;
        protected LinkButton cmdAdd;
        protected LinkButton cmdCancel;
        protected LinkButton cmdDelete;
        protected LinkButton cmdSave;
        protected LinkButton cmdSelect;
        protected LinkButton cmdUpload;

        protected HtmlTableRow ErrorRow;

        protected HtmlTableRow FileRow;
        protected Image imgStorageLocationType;
        protected Label lblFile;
        protected Label lblFolder;
        protected Label lblMessage;
        protected Label lblTab;
        protected Label lblURL;
        protected Label lblURLType;
        protected Label lblUser;
        protected RadioButtonList optType;

        protected HtmlTableRow TabRow;
        protected HtmlInputFile txtFile;
        protected TextBox txtUrl;
        protected TextBox txtUser;
        protected HtmlTableRow TypeRow;

        protected HtmlTableRow URLRow;

        protected HtmlTableRow UserRow;

       
//
//        protected virtual DropDownList cboFolders
//        {
//            get
//            {
//                return this._cboFolders;
//            }
//            set
//            {
//                if( this._cboFolders != null )
//                {
//                    this._cboFolders.SelectedIndexChanged -= new EventHandler( this.cboFolders_SelectedIndexChanged );
//                }
//                this._cboFolders = value;
//                if( this._cboFolders == null )
//                {
//                    return;
//                }
//                this._cboFolders.SelectedIndexChanged += new EventHandler( this.cboFolders_SelectedIndexChanged );
//            }
//        }

       

       

       

//        protected virtual LinkButton cmdAdd
//        {
//            get
//            {
//                return this._cmdAdd;
//            }
//            set
//            {
//                if( this._cmdAdd != null )
//                {
//                    this._cmdAdd.Click -= new EventHandler( this.cmdAdd_Click );
//                }
//                this._cmdAdd = value;
//                if( this._cmdAdd == null )
//                {
//                    return;
//                }
//                this._cmdAdd.Click += new EventHandler( this.cmdAdd_Click );
//            }
//        }

//        protected virtual LinkButton cmdCancel
//        {
//            get
//            {
//                return this._cmdCancel;
//            }
//            set
//            {
//                if( this._cmdCancel != null )
//                {
//                    this._cmdCancel.Click -= new EventHandler( this.cmdCancel_Click );
//                }
//                this._cmdCancel = value;
//                if( this._cmdCancel == null )
//                {
//                    return;
//                }
//                this._cmdCancel.Click += new EventHandler( this.cmdCancel_Click );
//            }
//        }
//
//        protected virtual LinkButton cmdDelete
//        {
//            get
//            {
//                return this._cmdDelete;
//            }
//            set
//            {
//                if( this._cmdDelete != null )
//                {
//                    this._cmdDelete.Click -= new EventHandler( this.cmdDelete_Click );
//                }
//                this._cmdDelete = value;
//                if( this._cmdDelete == null )
//                {
//                    return;
//                }
//                this._cmdDelete.Click += new EventHandler( this.cmdDelete_Click );
//            }
//        }

//        protected virtual LinkButton cmdSave
//        {
//            get
//            {
//                return this._cmdSave;
//            }
//            set
//            {
//                if( this._cmdSave != null )
//                {
//                    this._cmdSave.Click -= new EventHandler( this.cmdSave_Click );
//                }
//                this._cmdSave = value;
//                if( this._cmdSave == null )
//                {
//                    return;
//                }
//                this._cmdSave.Click += new EventHandler( this.cmdSave_Click );
//            }
//        }
//
//        protected virtual LinkButton cmdSelect
//        {
//            get
//            {
//                return this._cmdSelect;
//            }
//            set
//            {
//                if( this._cmdSelect != null )
//                {
//                    this._cmdSelect.Click -= new EventHandler( this.cmdSelect_Click );
//                }
//                this._cmdSelect = value;
//                if( this._cmdSelect == null )
//                {
//                    return;
//                }
//                this._cmdSelect.Click += new EventHandler( this.cmdSelect_Click );
//            }
//        }
//
//        protected virtual LinkButton cmdUpload
//        {
//            get
//            {
//                return this._cmdUpload;
//            }
//            set
//            {
//                if( this._cmdUpload != null )
//                {
//                    this._cmdUpload.Click -= new EventHandler( this.cmdUpload_Click );
//                }
//                this._cmdUpload = value;
//                if( this._cmdUpload == null )
//                {
//                    return;
//                }
//                this._cmdUpload.Click += new EventHandler( this.cmdUpload_Click );
//            }
//        }

       

        public string FileFilter
        {
            get
            {
                if (ViewState["_FileFilter"] != null)
                {
                    return Convert.ToString(ViewState["_FileFilter"]);
                }
                else
                {
                    return "";
                }
            }
            set
            {
                this.ViewState["_FileFilter"] = value;
            }
        }

       

       

       

        public string LocalResourceFile
        {
            get
            {
                string fileRoot;

                if (_localResourceFile == "")
                {
                    fileRoot = this.TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/URLControl.ascx";
                }
                else
                {
                    fileRoot = _localResourceFile;
                }
                return fileRoot;
            }
            set
            {
                this._localResourceFile = value;
            }
        }

        public bool Log
        {
            get
            {
                if( this.chkLog.Visible )
                {
                    return this.chkLog.Checked;
                }
                else
                {
                    return false;
                }
            }
        }

        public int ModuleID
        {
            get
            {
                int returnValue;
                returnValue = Convert.ToInt32(ViewState["ModuleId"]);
                if (returnValue == -2)
                {
                    if (Request.QueryString["mid"] != null)
                    {
                        returnValue = int.Parse(Request.QueryString["mid"]);
                    }
                }
                return returnValue;
            }
            set
            {
                this._ModuleID = value;
            }
        }

        public bool NewWindow
        {
            get
            {
                if( this.chkNewWindow.Visible )
                {
                    return this.chkNewWindow.Checked;
                }
                else
                {
                    return false;
                }
            }
        }

//        protected virtual RadioButtonList optType
//        {
//            get
//            {
//                return this._optType;
//            }
//            set
//            {
//                if( this._optType != null )
//                {
//                    this._optType.SelectedIndexChanged -= new EventHandler( this.optType_SelectedIndexChanged );
//                }
//                this._optType = value;
//                if( this._optType == null )
//                {
//                    return;
//                }
//                this._optType.SelectedIndexChanged += new EventHandler( this.optType_SelectedIndexChanged );
//            }
//        }

        public bool Required
        {
            set
            {
                this._Required = value;
            }
        }

        public bool ShowDatabase
        {
            set
            {
                this._ShowDatabase = value;
            }
        }

        public bool ShowFiles
        {
            set
            {
                this._ShowFiles = value;
            }
        }

        public bool ShowLog
        {
            set
            {
                this._ShowLog = value;
            }
        }

        public bool ShowNewWindow
        {
            set
            {
                this._ShowNewWindow = value;
            }
        }

        public bool ShowNone
        {
            set
            {
                this._ShowNone = value;
            }
        }

        public bool ShowSecure
        {
            set
            {
                this._ShowSecure = value;
            }
        }

        public bool ShowTabs
        {
            set
            {
                this._ShowTabs = value;
            }
        }

        public bool ShowTrack
        {
            set
            {
                this._ShowTrack = value;
            }
        }

        public bool ShowUpLoad
        {
            set
            {
                this._ShowUpLoad = value;
            }
        }

        public bool ShowUrls
        {
            set
            {
                this._ShowUrls = value;
            }
        }

        public bool ShowUsers
        {
            set
            {
                this._ShowUsers = value;
            }
        }

       
        public bool Track
        {
            get
            {
                if( this.chkTrack.Visible )
                {
                    return this.chkTrack.Checked;
                }
                else
                {
                    return false;
                }
            }
        }

        

       
        public string Url
        {
            get
            {
                string returnValue;
                returnValue = "";
                switch (optType.SelectedItem.Value)
                {
                    case "U":

                        if (cboUrls.Visible)
                        {
                            if (cboUrls.SelectedItem != null)
                            {
                                returnValue = cboUrls.SelectedItem.Value;
                            }
                        }
                        else
                        {
                            if (txtUrl.Text == "http://")
                            {
                                txtUrl.Text = "";
                            }
                            returnValue = Globals.AddHTTP(txtUrl.Text);
                        }
                        break;
                    case "T":

                        if (cboTabs.SelectedItem != null)
                        {
                            returnValue = cboTabs.SelectedItem.Value;
                        }
                        break;
                    case "F":

                        if (cboFiles.SelectedItem != null)
                        {
                            if (cboFiles.SelectedItem.Value != "")
                            {
                                returnValue = "FileID=" + cboFiles.SelectedItem.Value;
                            }
                            else
                            {
                                returnValue = "";
                            }
                        }
                        break;
                    case "M":

                        if (txtUser.Text != "")
                        {
                            UserInfo objUser = UserController.GetUserByName(_objPortal.PortalID, txtUser.Text, false);
                            if (objUser != null)
                            {
                                returnValue = "UserID=" + objUser.UserID.ToString();
                            }
                        }
                        break;
                }
                return returnValue;
            }
            set
            {
                this._Url = value;
            }
        }

        

        public string UrlType
        {
            get
            {
                return this.optType.SelectedItem.Value;
            }
            set
            {
                this._UrlType = value;
            }
        }

        

        public string Width
        {
            get
            {
                return Convert.ToString( this.ViewState["SkinControlWidth"] );
            }
            set
            {
                this._Width = value;
            }
        }

        public UrlControl()
        {
            base.Load += new EventHandler( this.Page_Load );
            this._ModuleID = -2;
            this._Required = true;
            this._ShowDatabase = true;
            this._ShowFiles = true;
            this._ShowLog = true;
            this._ShowNewWindow = false;
            this._ShowNone = false;
            this._ShowSecure = true;
            this._ShowTabs = true;
            this._ShowTrack = true;
            this._ShowUpLoad = true;
            this._ShowUrls = true;
            this._ShowUsers = false;
            this._Url = "";
            this._UrlType = "";
            this._Width = "";
        }

        private ArrayList GetFileList( string strExtensions, bool NoneSpecified, string Folder )
        {
            if( this.PortalSettings.ActiveTab.ParentId == this.PortalSettings.SuperTabId )
            {
                return Globals.GetFileList( Null.NullInteger, this.FileFilter, NoneSpecified, this.cboFolders.SelectedItem.Value, false );
            }
            else
            {
                return Globals.GetFileList( this._objPortal.PortalID, this.FileFilter, NoneSpecified, this.cboFolders.SelectedItem.Value, false );
            }
        }

        private string GetReadRoles( string Folder )
        {
            return FileSystemUtils.GetRoles( Folder, this._objPortal.PortalID, "READ" );
        }

        private string GetWriteRoles( string Folder )
        {
            return FileSystemUtils.GetRoles( Folder, this._objPortal.PortalID, "WRITE" );
        }

        private void cboFolders_SelectedIndexChanged( object sender, EventArgs e )
        {
            string strWriteRoles = GetWriteRoles(cboFolders.SelectedValue);
            if (PortalSecurity.IsInRoles(strWriteRoles))
            {
                if (!txtFile.Visible)
                {
                    // only show if not already in upload mode and not disabled
                    cmdUpload.Visible = _ShowUpLoad;
                }
            }
            else
            {
                //reset controls
                cboFiles.Visible = true;
                cmdUpload.Visible = false;
                txtFile.Visible = false;
                cmdSave.Visible = false;
                cmdCancel.Visible = false;
            }

            cboFiles.Items.Clear();
            cboFiles.DataSource = GetFileList(FileFilter, !_Required, cboFolders.SelectedItem.Value);
            cboFiles.DataBind();

            SetStorageLocationType();
        }

        private void cmdAdd_Click( object sender, EventArgs e )
        {
            this.cboUrls.Visible = false;
            this.cmdSelect.Visible = true;
            this.txtUrl.Visible = true;
            this.cmdAdd.Visible = false;
            this.cmdDelete.Visible = false;
        }

        private void cmdCancel_Click( object sender, EventArgs e )
        {
            this.cboFiles.Visible = true;
            this.cmdUpload.Visible = true;
            this.txtFile.Visible = false;
            this.cmdSave.Visible = false;
            this.cmdCancel.Visible = false;
        }

        private void cmdDelete_Click( object sender, EventArgs e )
        {
            if (cboUrls.SelectedItem != null)
            {
                UrlController objUrls = new UrlController();
                objUrls.DeleteUrl(_objPortal.PortalID, cboUrls.SelectedItem.Value);

                ShowControls();
            }
        }

        private void cmdSave_Click( object sender, EventArgs e )
        {
            // if no file is selected exit
            if (txtFile.PostedFile.FileName == "")
            {
                return;
            }

            string ParentFolderName;
            if (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId)
            {
                ParentFolderName = Globals.HostMapPath;
            }
            else
            {
                ParentFolderName = PortalSettings.HomeDirectoryMapPath;
            }
            ParentFolderName += cboFolders.SelectedItem.Value;

            string strExtension = Path.GetExtension(txtFile.PostedFile.FileName).Replace(".", "");
            if (FileFilter != "" && Strings.InStr("," + FileFilter.ToLower(), "," + strExtension.ToLower(), 0) == 0)
            {
                // trying to upload a file not allowed for current filter
                lblMessage.Text = string.Format(Localization.GetString("UploadError", this.LocalResourceFile), FileFilter, strExtension);
            }
            else
            {
                lblMessage.Text = FileSystemUtils.UploadFile(ParentFolderName.Replace("/", "\\"), txtFile.PostedFile, false);
            }

            if (lblMessage.Text == string.Empty)
            {
                cboFiles.Visible = true;
                cmdUpload.Visible = _ShowUpLoad;
                txtFile.Visible = false;
                cmdSave.Visible = false;
                cmdCancel.Visible = false;

                DirectoryInfo Root = new DirectoryInfo(ParentFolderName);
                cboFiles.Items.Clear();
                cboFiles.DataSource = GetFileList(FileFilter, false, cboFolders.SelectedItem.Value);
                cboFiles.DataBind();

                string FileName = txtFile.PostedFile.FileName.Substring(txtFile.PostedFile.FileName.LastIndexOf("\\") + 1);
                if (cboFiles.Items.FindByText(FileName) != null)
                {
                    cboFiles.Items.FindByText(FileName).Selected = true;
                }
            }
        }

        private void cmdSelect_Click( object sender, EventArgs e )
        {
            this.cboUrls.Visible = true;
            this.cmdSelect.Visible = false;
            this.txtUrl.Visible = false;
            this.cmdAdd.Visible = true;
            this.cmdDelete.Visible = PortalSecurity.IsInRole( this._objPortal.AdministratorRoleName );
        }

        private void cmdUpload_Click( object sender, EventArgs e )
        {
            this.cboFiles.Visible = false;
            this.cmdUpload.Visible = false;
            this.txtFile.Visible = true;
            this.cmdSave.Visible = true;
            this.cmdCancel.Visible = true;
        }

        private void LoadFolders()
        {
             int PortalId = Null.NullInteger;
            string ReadRoles = Null.NullString;
            string WriteRoles = Null.NullString;

            cboFolders.Items.Clear();

            if( ! IsHostMenu )
            {
                PortalId = _objPortal.PortalID;
            }

            ArrayList folders = FileSystemUtils.GetFolders(PortalId);
            foreach( FolderInfo folder in folders )
            {
                ListItem FolderItem = new ListItem();
                if( folder.FolderPath == Null.NullString )
                {
                    FolderItem.Text = Localization.GetString( "Root", this.LocalResourceFile );
                    ReadRoles = GetReadRoles( "" );
                    WriteRoles = GetWriteRoles( "" );
                }
                else
                {
                    FolderItem.Text = folder.FolderPath;
                    ReadRoles = GetReadRoles( FolderItem.Text );
                    WriteRoles = GetWriteRoles( FolderItem.Text );
                }
                FolderItem.Value = folder.FolderPath;

                if( PortalSecurity.IsInRoles( ReadRoles ) || PortalSecurity.IsInRoles( WriteRoles ) )
                {
                    bool canAdd = true;
                    switch( folder.StorageLocation )
                    {
                        case (int)FolderController.StorageLocationTypes.DatabaseSecure:

                            canAdd = _ShowDatabase;
                            break;
                        case (int)FolderController.StorageLocationTypes.SecureFileSystem:

                            canAdd = _ShowSecure;
                            break;
                    }
                    if( canAdd )
                    {
                        cboFolders.Items.Add( FolderItem );
                    }
                }
            }
        }

        private void optType_SelectedIndexChanged( object sender, EventArgs e )
        {
            this.ShowControls();
        }

        private void Page_Load( object sender, EventArgs e )
        {
            try
            {
                PortalController objPortals = new PortalController();
                if (!(Request.QueryString["pid"] == null) && (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId || UserController.GetCurrentUserInfo().IsSuperUser))
                {
                    _objPortal = objPortals.GetPortal(int.Parse(Request.QueryString["pid"]));
                }
                else
                {
                    _objPortal = objPortals.GetPortal(PortalSettings.PortalId);
                }

                if (!Page.IsPostBack)
                {
                    ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteItem"));

                    // set width of control
                    if (_Width != "")
                    {
                        cboUrls.Width = Unit.Parse(_Width);
                        txtUrl.Width = Unit.Parse(_Width);
                        cboTabs.Width = Unit.Parse(_Width);
                        cboFolders.Width = Unit.Parse(_Width);
                        cboFiles.Width = Unit.Parse(_Width);
                        txtUser.Width = Unit.Parse(_Width);
                    }

                    if (_ShowNone)
                    {
                        optType.Items.Add(new ListItem(Localization.GetString("NoneType", LocalResourceFile), "N"));
                    }
                    if (_ShowUrls)
                    {
                        optType.Items.Add(new ListItem(Localization.GetString("URLType", LocalResourceFile), "U"));
                    }
                    if (_ShowTabs)
                    {
                        optType.Items.Add(new ListItem(Localization.GetString("TabType", LocalResourceFile), "T"));
                    }
                    if (_ShowFiles)
                    {
                        optType.Items.Add(new ListItem(Localization.GetString("FileType", LocalResourceFile), "F"));
                    }
                    if (_ShowUsers)
                    {
                        optType.Items.Add(new ListItem(Localization.GetString("UserType", LocalResourceFile), "M"));
                    }
                    chkNewWindow.Visible = _ShowNewWindow;
                    chkLog.Visible = _ShowLog;
                    chkTrack.Visible = _ShowTrack;
                    int URLType = Convert.ToInt32(_ShowUrls) + Convert.ToInt32(_ShowTabs) + Convert.ToInt32(_ShowFiles);
                    if (URLType == -1)
                    {
                        TypeRow.Visible = false;
                    }
                    // save persistent values
                    ViewState["ModuleId"] = Convert.ToString(_ModuleID);
                    ViewState["SkinControlWidth"] = _Width;

                    ShowControls();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void SetStorageLocationType()
        {
             FolderController objFolder = new FolderController();
            FolderInfo objFolderInfo = new FolderInfo();
            string FolderName = cboFolders.SelectedValue;

            // Check to see if this is the 'Root' folder, if so we cannot rely on its text value because it is something and not an empty string that we need to lookup the 'root' folder
            if( cboFolders.SelectedValue == string.Empty )
            {
                FolderName = "";
            }

            objFolderInfo = objFolder.GetFolder( PortalSettings.PortalId, FolderName );

            if( objFolderInfo != null )
            {
                switch( objFolderInfo.StorageLocation )
                {
                    case (int)FolderController.StorageLocationTypes.InsecureFileSystem:

                        imgStorageLocationType.Visible = false;
                        break;
                    case (int)FolderController.StorageLocationTypes.SecureFileSystem:

                        imgStorageLocationType.ImageUrl = ResolveUrl( "~/images/icon_securityroles_16px.gif" );
                        imgStorageLocationType.Visible = true;
                        break;
                    case (int)FolderController.StorageLocationTypes.DatabaseSecure:

                        imgStorageLocationType.ImageUrl = ResolveUrl( "~/images/icon_sql_16px.gif" );
                        imgStorageLocationType.Visible = true;
                        break;
                }
            }
        }

        private void ShowControls()
        {
            UrlController objUrls = new UrlController();

            // set url type
            if (optType.SelectedItem == null)
            {
                if (_Url != "")
                {
                    string TrackingUrl = _Url;

                    _UrlType = Globals.GetURLType(_Url).ToString("g").Substring(0, 1);

                    if (_UrlType == "F")
                    {
                        FileController objFiles = new FileController();
                        if (_Url.ToLower().StartsWith("fileid="))
                        {
                            TrackingUrl = _Url;
                            FileInfo objFile = objFiles.GetFileById(int.Parse(_Url.Substring(7)), _objPortal.PortalID);
                            if (objFile != null)
                            {
                                _Url = objFile.Folder + objFile.FileName;
                            }
                        }
                        else
                        {
                            // to handle legacy scenarios before the introduction of the FileServerHandler
                            TrackingUrl = "FileID=" + objFiles.ConvertFilePathToFileId(_Url, _objPortal.PortalID).ToString();
                        }
                    }

                    if (_UrlType == "M")
                    {
                        if (_Url.ToLower().StartsWith("userid="))
                        {
                            UserInfo objUser = UserController.GetUser(_objPortal.PortalID, int.Parse(_Url.Substring(7)), false);
                            if (objUser != null)
                            {
                                _Url = objUser.Username;
                            }
                        }
                    }

                    UrlTrackingInfo objUrlTracking = objUrls.GetUrlTracking(_objPortal.PortalID, TrackingUrl, ModuleID);
                    if (objUrlTracking != null)
                    {
                        optType.Items.FindByValue(objUrlTracking.UrlType).Selected = true;
                        chkNewWindow.Checked = objUrlTracking.NewWindow;
                        chkTrack.Checked = objUrlTracking.TrackClicks;
                        chkLog.Checked = objUrlTracking.LogActivity;
                    }
                    else // the url does not exist in the tracking table
                    {
                        optType.Items.FindByValue(_UrlType).Selected = true;
                        chkNewWindow.Checked = false;
                        chkTrack.Checked = true;
                        chkLog.Checked = false;
                    }
                }
                else
                {
                    if (_UrlType != "")
                    {
                        if (optType.Items.FindByValue(_UrlType) != null)
                        {
                            optType.Items.FindByValue(_UrlType).Selected = true;
                        }
                        else
                        {
                            optType.Items[0].Selected = true;
                        }
                    }
                    else
                    {
                        optType.Items[0].Selected = true;
                    }
                    chkNewWindow.Checked = false;
                    chkTrack.Checked = true;
                    chkLog.Checked = false;
                }
            }

            // load listitems
            switch (optType.SelectedItem.Value)
            {
                case "N": // None

                    URLRow.Visible = false;
                    TabRow.Visible = false;
                    FileRow.Visible = false;
                    UserRow.Visible = false;
                    break;
                case "U": // Url

                    URLRow.Visible = true;
                    TabRow.Visible = false;
                    FileRow.Visible = false;
                    UserRow.Visible = false;

                    if (txtUrl.Text == "")
                    {
                        txtUrl.Text = _Url;
                    }
                    if (txtUrl.Text == "")
                    {
                        txtUrl.Text = "http://";
                    }
                    txtUrl.Visible = true;

                    cmdSelect.Visible = true;

                    cboUrls.Items.Clear();
                    cboUrls.DataSource = objUrls.GetUrls(_objPortal.PortalID);
                    cboUrls.DataBind();
                    if (cboUrls.Items.FindByValue(_Url) != null)
                    {
                        cboUrls.Items.FindByValue(_Url).Selected = true;
                    }
                    cboUrls.Visible = false;

                    cmdAdd.Visible = false;
                    cmdDelete.Visible = false;
                    break;
                case "T": // tab

                    URLRow.Visible = false;
                    TabRow.Visible = true;
                    FileRow.Visible = false;
                    UserRow.Visible = false;

                    cboTabs.Items.Clear();

                    cboTabs.DataSource = Globals.GetPortalTabs(_objPortal.PortalID, !_Required, true, false, false, false);
                    cboTabs.DataBind();
                    if (cboTabs.Items.FindByValue(_Url) != null)
                    {
                        cboTabs.Items.FindByValue(_Url).Selected = true;
                    }
                    break;
                case "F": // file

                    URLRow.Visible = false;
                    TabRow.Visible = false;
                    FileRow.Visible = true;
                    UserRow.Visible = false;

                    LoadFolders();

                    if (cboFolders.Items.Count == 0)
                    {
                        lblMessage.Text = Localization.GetString("NoPermission", this.LocalResourceFile);
                        FileRow.Visible = false;
                        return;
                    }

                    // select folder
                    string FileName;
                    string FolderPath;
                    if (_Url != string.Empty)
                    {
                        FileName = _Url.Substring(_Url.LastIndexOf("/") + 1);
                        FolderPath = _Url.Replace(FileName, "");
                    }
                    else
                    {
                        FileName = _Url;
                        FolderPath = string.Empty;
                    }
                    if (cboFolders.Items.FindByValue(FolderPath) != null)
                    {
                        cboFolders.Items.FindByValue(FolderPath).Selected = true;
                    }
                    else
                    {
                        cboFolders.Items[0].Selected = true;
                        FolderPath = cboFolders.SelectedValue;
                    }

                    cboFiles.DataSource = GetFileList(FileFilter, !_Required, cboFolders.SelectedItem.Value);
                    cboFiles.DataBind();
                    if (cboFiles.Items.FindByText(FileName) != null)
                    {
                        cboFiles.Items.FindByText(FileName).Selected = true;
                    }
                    cboFiles.Visible = true;
                    txtFile.Visible = false;

                    string strWriteRoles = GetWriteRoles(FolderPath);
                    cmdUpload.Visible = PortalSecurity.IsInRoles(strWriteRoles) && _ShowUpLoad;

                    SetStorageLocationType();

                    txtUrl.Visible = false;
                    cmdSave.Visible = false;
                    cmdCancel.Visible = false;
                    break;
                case "M": // membership users

                    URLRow.Visible = false;
                    TabRow.Visible = false;
                    FileRow.Visible = false;
                    UserRow.Visible = true;

                    if (txtUser.Text == "")
                    {
                        txtUser.Text = _Url;
                    }
                    break;
            }
        }
    }
}