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
using System.IO;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.UI.Skins
{
    public class SkinControl : UserControlBase
    {
        private string _localResourceFile;
        private PortalInfo _objPortal;
        private string _SkinRoot;
        private string _SkinSrc;

        private string _Width = "";
        protected DropDownList cboSkin;
        protected LinkButton cmdPreview;
        protected RadioButton optHost;
        protected RadioButton optSite;

        public string LocalResourceFile
        {
            get
            {
                if( String.IsNullOrEmpty( _localResourceFile ))
                {
                    return ( this.TemplateSourceDirectory + "/App_LocalResources/SkinControl.ascx" );
                }
                else
                {
                    return this._localResourceFile;
                }
            }
            set
            {
                this._localResourceFile = value;
            }
        }

        public string SkinRoot
        {
            get
            {
                return Convert.ToString( this.ViewState["SkinRoot"] );
            }
            set
            {
                this._SkinRoot = value;
            }
        }

        public string SkinSrc
        {
            get
            {
                if( this.cboSkin.SelectedItem != null )
                {
                    return this.cboSkin.SelectedItem.Value;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                this._SkinSrc = value;
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

        public SkinControl()
        {
            Load += new EventHandler( this.Page_Load );
            Init += new EventHandler(SkinControl_Init);
            this._Width = String.Empty;
        }

        void SkinControl_Init(object sender, EventArgs e)
        {
            this.optSite.CheckedChanged += new EventHandler(this.optSite_CheckedChanged);
            this.optHost.CheckedChanged += new EventHandler(this.optHost_CheckedChanged);
            this.cmdPreview.Click += new EventHandler(this.cmdPreview_Click);

        }

        /// <Summary>format skin name</Summary>
        /// <Param name="strSkinFolder">The Folder Name</Param>
        /// <Param name="strSkinFile">The File Name without extension</Param>
        private string FormatSkinName( string strSkinFolder, string strSkinFile )
        {
            if( strSkinFolder.ToLower() == "_default" )
            {
                // host folder
                return strSkinFile;
            }
            else // portal folder
            {
                switch( strSkinFile.ToLower() )
                {
                    case "skin":
                        return strSkinFolder;

                    case "container":
                        return strSkinFolder;

                    case "default":
                        return strSkinFolder + " - " + strSkinFile;
                    default:
                        return strSkinFolder;
                }
            }
        }

        protected void cmdPreview_Click( object sender, EventArgs e )
        {
            if( !String.IsNullOrEmpty(SkinSrc) )
            {
                string strType = SkinRoot.Substring( 0, SkinRoot.Length - 1 );

                string strURL = Globals.AddHTTP( Globals.GetDomainName( Request ) ) + Globals.ApplicationURL( _objPortal.HomeTabId ).Replace( "~", "" );
                //detect if there is already a '?' in the URL (in case of a child portal)
                if( strURL.IndexOf( "?" ) > 0 )
                {
                    strURL += "&";
                }
                else
                {
                    strURL += "?";
                }
                strURL += "portalid=" + _objPortal.PortalID + "&" + strType + "Src=" + Globals.QueryStringEncode( SkinSrc.Replace( ".ascx", "" ) );

                if( SkinRoot == SkinInfo.RootContainer )
                {
                    if( Request.QueryString["ModuleId"] != null )
                    {
                        strURL += "&ModuleId=" + Request.QueryString["ModuleId"];
                    }
                }

                Response.Write( "<script>window.open('" + strURL + "','_blank')</script>" );
            }
        }

        private void LoadSkins()
        {
            string strRoot;
            string strFolder;
            string[] arrFolders;
            string strFile;
            string[] arrFiles;
            string strLastFolder;
            string strSeparator = "----------------------------------------";

            cboSkin.Items.Clear();

            if( optHost.Checked )
            {
                // load host skins
                strLastFolder = "";
                strRoot = Globals.HostMapPath + SkinRoot;
                if( Directory.Exists( strRoot ) )
                {
                    arrFolders = Directory.GetDirectories( strRoot );
                    foreach( string tempLoopVar_strFolder in arrFolders )
                    {
                        strFolder = tempLoopVar_strFolder;
                        if( !strFolder.EndsWith( Globals.glbHostSkinFolder ) )
                        {
                            arrFiles = Directory.GetFiles( strFolder, "*.ascx" );
                            foreach( string tempLoopVar_strFile in arrFiles )
                            {
                                strFile = tempLoopVar_strFile;
                                strFolder = strFolder.Substring( Strings.InStrRev( strFolder, "\\", -1, 0 ) + 1 - 1 );
                                if( strLastFolder != strFolder )
                                {
                                    if( !String.IsNullOrEmpty(strLastFolder) )
                                    {
                                        cboSkin.Items.Add( new ListItem( strSeparator, "" ) );
                                    }
                                    strLastFolder = strFolder;
                                }
                                cboSkin.Items.Add( new ListItem( FormatSkinName( strFolder, Path.GetFileNameWithoutExtension( strFile ) ), "[G]" + SkinRoot + "/" + strFolder + "/" + Path.GetFileName( strFile ) ) );
                            }
                        }
                    }
                }
            }

            if( optSite.Checked )
            {
                // load portal skins
                strLastFolder = "";
                strRoot = _objPortal.HomeDirectoryMapPath + SkinRoot;
                if( Directory.Exists( strRoot ) )
                {
                    arrFolders = Directory.GetDirectories( strRoot );
                    foreach( string tempLoopVar_strFolder in arrFolders )
                    {
                        strFolder = tempLoopVar_strFolder;
                        arrFiles = Directory.GetFiles( strFolder, "*.ascx" );
                        foreach( string tempLoopVar_strFile in arrFiles )
                        {
                            strFile = tempLoopVar_strFile;
                            strFolder = strFolder.Substring( Strings.InStrRev( strFolder, "\\", -1, 0 ) + 1 - 1 );
                            if( strLastFolder != strFolder )
                            {
                                if( !String.IsNullOrEmpty(strLastFolder) )
                                {
                                    cboSkin.Items.Add( new ListItem( strSeparator, "" ) );
                                }
                                strLastFolder = strFolder;
                            }
                            cboSkin.Items.Add( new ListItem( FormatSkinName( strFolder, Path.GetFileNameWithoutExtension( strFile ) ), "[L]" + SkinRoot + "/" + strFolder + "/" + Path.GetFileName( strFile ) ) );
                        }
                    }
                }
            }

            // default value
            if( cboSkin.Items.Count > 0 )
            {
                cboSkin.Items.Insert( 0, new ListItem( strSeparator, "" ) );
            }
            cboSkin.Items.Insert( 0, new ListItem( "<" + Localization.GetString( "Not_Specified" ) + ">", "" ) );

            // select current skin
            int intIndex;
            for( intIndex = 0; intIndex <= cboSkin.Items.Count - 1; intIndex++ )
            {
                if( cboSkin.Items[intIndex].Value.ToLower() == Convert.ToString( ViewState["SkinSrc"] ).ToLower() )
                {
                    cboSkin.Items[intIndex].Selected = true;
                    break;
                }
            }
        }

        protected void optHost_CheckedChanged( object sender, EventArgs e )
        {
            this.LoadSkins();
        }

        protected void optSite_CheckedChanged( object sender, EventArgs e )
        {
            this.LoadSkins();
        }

        protected void Page_Load( object sender, EventArgs e )
        {
            try
            {
                PortalController objPortals = new PortalController();
                if( Request.QueryString["pid"] != null && ( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId || UserController.GetCurrentUserInfo().IsSuperUser ) )
                {
                    _objPortal = objPortals.GetPortal( int.Parse( Request.QueryString["pid"] ) );
                }
                else
                {
                    _objPortal = objPortals.GetPortal( PortalSettings.PortalId );
                }

                if( !Page.IsPostBack )
                {
                    // save persistent values
                    ViewState["SkinControlWidth"] = _Width;
                    ViewState["SkinRoot"] = _SkinRoot;
                    ViewState["SkinSrc"] = _SkinSrc;

                    // set width of control
                    if( !String.IsNullOrEmpty(_Width) )
                    {
                        cboSkin.Width = Unit.Parse( _Width );
                    }

                    // set selected skin
                    if( !String.IsNullOrEmpty(_SkinSrc) )
                    {
                        switch( _SkinSrc.Substring( 0, 3 ) )
                        {
                            case "[L]":

                                optHost.Checked = false;
                                optSite.Checked = true;
                                break;
                            case "[G]":

                                optSite.Checked = false;
                                optHost.Checked = true;
                                break;
                        }
                    }
                    else
                    {
                        // no skin selected, initialized to site skin if any exists
                        string strRoot = _objPortal.HomeDirectoryMapPath + SkinRoot;
                        if( Directory.Exists( strRoot ) && Directory.GetDirectories( strRoot ).Length > 0 )
                        {
                            optHost.Checked = false;
                            optSite.Checked = true;
                        }
                    }

                    LoadSkins();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}