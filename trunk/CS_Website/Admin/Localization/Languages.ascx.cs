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
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Services.Localization
{
    /// <summary>
    /// Manages the suported locales file
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[vmasanas]	10/04/2004  Created
    /// </history>
    public partial class Languages : PortalModuleBase, IActionable
    {
        private XmlDocument xmlLocales = new XmlDocument();
        private bool bXmlLoaded = false;

        /// <summary>
        /// Loads defined locales
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( File.Exists( Server.MapPath( Localization.ApplicationResourceDirectory + "/Locales.Portal-" + PortalId.ToString() + ".xml" ) ) )
                {
                    try
                    {
                        xmlLocales.Load( Server.MapPath( Localization.ApplicationResourceDirectory + "/Locales.Portal-" + PortalId.ToString() + ".xml" ) );
                        bXmlLoaded = true;
                    }
                    catch
                    {
                    }
                }

                if( ! Page.IsPostBack )
                {
                    //only host can add and delete
                    pnlAdd.Visible = UserInfo.IsSuperUser;
                    chkDeleteFiles.Visible = UserInfo.IsSuperUser;

                    //Localize Grid
                    Localization.LocalizeDataGrid( ref dgLocales, this.LocalResourceFile );
                    BindGrid();

                    LoadLocales();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// Adds a new locale to the locales xml file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Only one definition for a given locale key can be defined
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        protected void cmdAdd_Click( Object sender, EventArgs e )
        {
            try
            {
                switch( new Localization().AddLocale( cboLocales.SelectedValue, cboLocales.SelectedItem.Text ) )
                {
                    case "Duplicate.ErrorMessage":

                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Duplicate.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                        break;
                    case "NewLocale.ErrorMessage":

                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "NewLocale.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.GreenSuccess );
                        BindGrid();
                        break;
                    case "Save.ErrorMessage":

                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                        break;
                    case "":

                        BindGrid();
                        break;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void dgLocales_ItemDataBound( object sender, DataGridItemEventArgs e )
        {
            try
            {
                if( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
                {
                    LinkButton ctl;
                    ctl = (LinkButton)e.Item.FindControl( "cmdDelete" );
                    ClientAPI.AddButtonConfirm( ctl, Localization.GetString( "DeleteItem" ) );

                    Label lbl;
                    LinkButton ctlStatus;
                    lbl = (Label)e.Item.FindControl( "lblStatus" );
                    ctlStatus = (LinkButton)e.Item.FindControl( "cmdDisable" );

                    if( ! bXmlLoaded || xmlLocales.SelectSingleNode( "//locales/inactive/locale[.='" + Convert.ToString( dgLocales.DataKeys[e.Item.ItemIndex] ) + "']" ) == null )
                    {
                        lbl.Text = Localization.GetString( "Enabled", LocalResourceFile );
                        ctlStatus.Text = Localization.GetString( "Disable", LocalResourceFile );
                    }
                    else
                    {
                        lbl.Text = Localization.GetString( "Disabled", LocalResourceFile );
                        ctlStatus.Text = Localization.GetString( "Enable", LocalResourceFile );
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void dgLocales_ItemCommand( object source, DataGridCommandEventArgs e )
        {
            try
            {
                switch( e.CommandName )
                {
                    case "Disable":

                        if( ! bXmlLoaded )
                        {
                            try
                            {
                                // First access to file, create using template
                                File.Copy( Server.MapPath( Localization.ApplicationResourceDirectory + "/Locales.Portal.xml.config" ), Server.MapPath( Localization.ApplicationResourceDirectory + "/Locales.Portal-" + PortalId.ToString() + ".xml" ) );
                                xmlLocales.Load( Server.MapPath( Localization.ApplicationResourceDirectory + "/Locales.Portal-" + PortalId.ToString() + ".xml" ) );
                                bXmlLoaded = true;
                            }
                            catch
                            {
                                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                            }
                        }

                        if( bXmlLoaded )
                        {
                            XmlNode inactive;
                            XmlNode current;
                            inactive = xmlLocales.SelectSingleNode( "//locales/inactive" );
                            current = inactive.SelectSingleNode( "locale[.='" + Convert.ToString( dgLocales.DataKeys[e.Item.ItemIndex] ) + "']" );
                            if( current == null ) //disable
                            {
                                //can only disable if not last one enabled
                                int found = 0;
                                foreach( DataGridItem l in dgLocales.Items )
                                {
                                    Label lbl;
                                    lbl = (Label)l.FindControl( "lblStatus" );
                                    if( lbl.Text == Localization.GetString( "Enabled", LocalResourceFile ) )
                                    {
                                        found++;
                                    }
                                }
                                if( found > 1 )
                                {
                                    // current portal locale cannot be disabled
                                    if( PortalSettings.DefaultLanguage != Convert.ToString( dgLocales.DataKeys[e.Item.ItemIndex] ) )
                                    {
                                        XmlNode newnode = xmlLocales.CreateElement( "locale" );
                                        newnode.InnerText = Convert.ToString( dgLocales.DataKeys[e.Item.ItemIndex] );
                                        inactive.AppendChild( newnode );
                                    }
                                    else
                                    {
                                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "DisableCurrent.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                                        return;
                                    }
                                }
                                else
                                {
                                    UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Disable.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                                    return;
                                }
                            }
                            else // enable
                            {
                                inactive.RemoveChild( current );
                            }
                            xmlLocales.Save( Server.MapPath( Localization.ApplicationResourceDirectory + "/Locales.Portal-" + PortalId.ToString() + ".xml" ) );
                            BindGrid();
                        }
                        break;
                    case "Delete":

                        string key;
                        XmlNode node;
                        XmlDocument resDoc = new XmlDocument();

                        key = e.Item.Cells[1].Text;
                        if( key == Localization.SystemLocale )
                        {
                            UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Delete.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                        }
                        else
                        {
                            //can only delete if not last one enabled
                            int found = 0;
                            foreach( DataGridItem l in dgLocales.Items )
                            {
                                Label lbl;
                                lbl = (Label)l.FindControl( "lblStatus" );
                                if( lbl.Text == Localization.GetString( "Enabled", LocalResourceFile ) )
                                {
                                    found++;
                                }
                            }
                            if( found > 1 )
                            {
                                // current portal locale cannot be disabled
                                if( PortalSettings.DefaultLanguage != Convert.ToString( dgLocales.DataKeys[e.Item.ItemIndex] ) )
                                {
                                    resDoc.Load( Server.MapPath( Localization.SupportedLocalesFile ) );
                                    node = resDoc.SelectSingleNode( "//root/language[@key='" + key + "']" );
                                    node.ParentNode.RemoveChild( node );

                                    try
                                    {
                                        resDoc.Save( Server.MapPath( Localization.SupportedLocalesFile ) );
                                        BindGrid();
                                        // check if files needs to be deleted.
                                        if( chkDeleteFiles.Checked )
                                        {
                                            DeleteLocalizedFiles( key );
                                            chkDeleteFiles.Checked = false;
                                        }
                                    }
                                    catch
                                    {
                                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                                    }
                                }
                                else
                                {
                                    UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "DisableCurrent.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                                    return;
                                }
                            }
                            else
                            {
                                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Disable.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                                return;
                            }
                        }
                        break;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void rbDisplay_SelectedIndexChanged( Object sender, EventArgs e )
        {
            LoadLocales();
        }

        /// <summary>
        /// Reads XML file and binds to the datagrid
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        private void BindGrid()
        {
            DataSet ds = new DataSet();
            DataView dv;

            ds.ReadXml( Server.MapPath( Localization.SupportedLocalesFile ) );
            dv = ds.Tables[0].DefaultView;
            dv.Sort = "name ASC";

            dgLocales.DataSource = dv;
            dgLocales.DataKeyField = "key";
            dgLocales.DataBind();
        }

        /// <summary>
        /// Removes all localized files for a given locale
        /// </summary>
        /// <param name="locale">Locale to delete</param>
        /// <remarks>
        /// LocalResources files are only found in \admin, \controls, \DesktopModules
        /// Global and shared resource files are in \Resources
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        private void DeleteLocalizedFiles( string locale )
        {
            string fil;

            // Delete LocalResources from following folders
            DeleteLocalizedFiles( Server.MapPath( "~" ), locale, true );

            // Delete Global/Shared resources
            foreach( string tempLoopVar_fil in Directory.GetFiles( Server.MapPath( Localization.ApplicationResourceDirectory ) ) )
            {
                fil = tempLoopVar_fil;
                // find the locale substring, ex: .nl-NL.
                if( Path.GetFileName( fil ).ToLower().IndexOf( "." + locale.ToLower() + "." ) > - 1 )
                {
                    try
                    {
                        File.Delete( fil );
                    }
                    catch
                    {
                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                    }
                }
            }
        }

        /// <summary>
        /// Recursively deletes files on a given folder
        /// </summary>
        /// <param name="folder">Initial folder</param>
        /// <param name="locale">Locale files to be deleted</param>
        /// <param name="recurse">Delete recursively or not</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        private void DeleteLocalizedFiles( string folder, string locale, bool recurse )
        {
            string fol;
            string fil;
            locale = locale.ToLower();

            foreach( string tempLoopVar_fol in Directory.GetDirectories( folder ) )
            {
                fol = tempLoopVar_fol;
                if( Path.GetFileName( fol ) == Localization.LocalResourceDirectory )
                {
                    // Found LocalResources folder
                    foreach( string tempLoopVar_fil in Directory.GetFiles( fol ) )
                    {
                        fil = tempLoopVar_fil;
                        // find the locale substring, ex: .nl-NL.
                        if( Path.GetFileName( fil ).ToLower().IndexOf( "." + locale + ".resx" ) > - 1 )
                        {
                            try
                            {
                                File.Delete( fil );
                            }
                            catch
                            {
                                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                            }
                        }
                    }
                }
                else
                {
                    if( recurse )
                    {
                        //recurse
                        DeleteLocalizedFiles( fol, locale, recurse );
                    }
                }
            }
        }

        private void LoadLocales()
        {
            string localeKey;
            string localeName;

            cboLocales.Items.Clear();
            CultureInfo cinfo;
            foreach( CultureInfo tempLoopVar_cinfo in CultureInfo.GetCultures( CultureTypes.SpecificCultures ) )
            {
                cinfo = tempLoopVar_cinfo;
                localeKey = Convert.ToString( cinfo.Name );
                if( rbDisplay.SelectedValue == "Native" )
                {
                    localeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase( cinfo.NativeName );
                }
                else
                {
                    localeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase( cinfo.EnglishName );
                }
                cboLocales.Items.Add( new ListItem( localeName, localeKey ) );
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleInfo FileManagerModule = ( new ModuleController() ).GetModuleByDefinition( Null.NullInteger, "File Manager" );
                string[] args = new string[3];

                args[0] = "mid=" + FileManagerModule.ModuleID;
                args[1] = "ftype=" + UploadType.LanguagePack.ToString();
                args[2] = "rtab=" + this.TabId;

                ModuleActionCollection actions = new ModuleActionCollection();
                actions.Add( GetNextActionID(), Localization.GetString( "Languages.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl( "language" ), false, SecurityAccessLevel.Admin, true, false );
                actions.Add( GetNextActionID(), Localization.GetString( "TimeZones.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl( "timezone" ), false, SecurityAccessLevel.Host, true, false );
                actions.Add( GetNextActionID(), Localization.GetString( "Verify.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl( "verify" ), false, SecurityAccessLevel.Host, true, false );
                actions.Add( GetNextActionID(), Localization.GetString( "PackageGenerate.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl( "package" ), false, SecurityAccessLevel.Host, true, false );
                actions.Add( GetNextActionID(), Localization.GetString( "PackageImport.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "", Globals.NavigateURL( FileManagerModule.TabID, "Edit", args ), false, SecurityAccessLevel.Host, true, false );
                return actions;
            }
        }


    }
}