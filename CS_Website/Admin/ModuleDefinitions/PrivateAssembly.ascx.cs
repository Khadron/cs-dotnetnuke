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
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

namespace DotNetNuke.Modules.Admin.ModuleDefinitions
{
    /// <summary>
    /// The PrivateAssembly PortalModuleBase is used to create a Private Assembly Package
    /// from this module
    /// </summary>
    /// <history>
    /// 	[cnurse]	1/14/2005	created
    /// </history>
    public partial class PrivateAssembly : PortalModuleBase
    {
        /// <summary>
        /// Page_Init runs when the control is initialised.
        /// </summary>
        /// <history>
        /// 	[cnurse]	1/14/2005	created
        /// </history>
        protected void Page_Init( Object sender, EventArgs e )
        {
        }

        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
        /// <history>
        /// 	[cnurse]	1/14/2005	created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["desktopmoduleid"] != null)
                {
                    DesktopModuleController objDesktopModules = new DesktopModuleController();
                    DesktopModuleInfo objDesktopModule = objDesktopModules.GetDesktopModule(Int32.Parse(Request.QueryString["desktopmoduleid"]));
                    if (objDesktopModule != null)
                    {
                        txtFileName.Text = objDesktopModule.ModuleName + ".zip";

                        if (!objDesktopModule.IsAdmin)
                        {
                            //Create the DirectoryInfo object for the folder
                            DirectoryInfo folder = new DirectoryInfo(Globals.ApplicationMapPath + "\\DesktopModules\\" + objDesktopModule.FolderName);
                            if (folder.Exists)
                            {
                                //Determine Visibility of Source check box
                                rowSource.Visible = (folder.GetFiles("*.??proj").Length > 0);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// cmdCancel_Click runs when the Cancel Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	1/14/2005	created
        /// </history>
        protected void cmdCancel_Click( Object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Globals.NavigateURL() );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdCreate_Click runs when the Create Button is clicked
        /// </summary>
        /// <history>
        /// 	[cnurse]	1/14/2005	created
        /// </history>
        protected void cmdCreate_Click( Object sender, EventArgs e )
        {
            string strFileName = txtFileName.Text;
            if (String.IsNullOrEmpty( strFileName))
            {
                UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("Create.ErrorMessage", this.LocalResourceFile), ModuleMessageType.YellowWarning);
            }
            else
            {
                if( !( strFileName.ToLower().EndsWith( ".zip" ) ) )
                {
                    strFileName += ".zip";
                }

                int mid = Null.NullInteger;
                if( Request.QueryString["desktopmoduleid"] != null )
                {
                    mid = Int32.Parse( Request.QueryString["desktopmoduleid"] );
                }

                if( mid > 0 )
                {
                    PaWriter PaWriter = new PaWriter(chkSource.Checked, chkManifest.Checked, chkPrivate.Checked, strFileName);
                    string PaZipName = PaWriter.CreatePrivateAssembly( mid );

                    if( PaWriter.ProgressLog.Valid )
                    {
                        lblMessage.Text = string.Format( Localization.GetString( "LOG.MESSAGE.Success", LocalResourceFile ), PaZipName );
                        lblMessage.CssClass = "Head";
                    }
                    else
                    {
                        lblMessage.Text = Localization.GetString( "LOG.MESSAGE.Error", LocalResourceFile );
                        lblMessage.CssClass = "NormalRed";
                    }

                    ModuleInfo FileManagerModule = ( new ModuleController() ).GetModuleByDefinition( Null.NullInteger, "File Manager" );

                    if( FileManagerModule != null )
                    {
                        lblLink.Text = string.Format( Localization.GetString( "lblLink", LocalResourceFile ), Globals.NavigateURL( FileManagerModule.TabID ) );
                    }
                    divLog.Controls.Add( PaWriter.ProgressLog.GetLogsTable() );
                    pnlLogs.Visible = true;
                }
            }
        }
    }
}