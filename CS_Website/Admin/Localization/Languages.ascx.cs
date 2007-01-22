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
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( File.Exists( Server.MapPath( Localization.ApplicationResourceDirectory + "/Locales.Portal-" + PortalId + ".xml" ) ) )
                {
                    try
                    {
                        xmlLocales.Load( Server.MapPath( Localization.ApplicationResourceDirectory + "/Locales.Portal-" + PortalId + ".xml" ) );
                        bXmlLoaded = true;
                    }
                    catch
                    {
                    }
                }

                if( ! Page.IsPostBack )
                {
                    //only host can add and delete, this should also only be visible on the host menu
                    if (UserInfo.IsSuperUser & (PortalSettings.ActiveTab.ParentId != PortalSettings.AdminTabId))
                    {
                        chkEnableBrowser.Text = Localization.GetString("EnableBrowserHost", LocalResourceFile);
                    }
                    else
                    {
                        chkEnableBrowser.Text = Localization.GetString("EnableBrowserPortal", LocalResourceFile);
                        pnlAdd.Visible = false;
                        chkDeleteFiles.Visible = false;
                    }

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

        protected void dgLocales_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
                {
//                    LinkButton ctldelete = (LinkButton)(e.Item.FindControl("cmdDelete"));
//                    LinkButton ctldisable = (LinkButton)(e.Item.FindControl("cmdDisable"));

                    if (PortalSettings.ActiveTab.ParentId == PortalSettings.AdminTabId)
                    {
                        // we are on the Admin menu, hide delete button, since this is only for host
                        dgLocales.Columns[4].Visible = false;
                    }
                    else
                    {
                        // we are on the host menu, hide enable button and status, since this is only for admins
                        dgLocales.Columns[2].Visible = false;
                        dgLocales.Columns[3].Visible = false;
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
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
                                File.Copy( Server.MapPath( Localization.ApplicationResourceDirectory + "/Locales.Portal.xml.config" ), Server.MapPath( Localization.ApplicationResourceDirectory + "/Locales.Portal-" + PortalId + ".xml" ) );
                                xmlLocales.Load( Server.MapPath( Localization.ApplicationResourceDirectory + "/Locales.Portal-" + PortalId + ".xml" ) );
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
                            xmlLocales.Save( Server.MapPath( Localization.ApplicationResourceDirectory + "/Locales.Portal-" + PortalId + ".xml" ) );
                            BindGrid();
                        }
                        break;
                    case "Delete":

                        XmlDocument resDoc = new XmlDocument();

                        string key = e.Item.Cells[1].Text;
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
                                    XmlNode node = resDoc.SelectSingleNode( "//root/language[@key='" + key + "']" );
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

    
        protected void chkEnableBrowser_CheckedChanged(object sender, EventArgs e)
        {
            XmlNode browserLanguage;

            if (PortalSettings.ActiveTab.ParentId == PortalSettings.AdminTabId)
            {
                if (! bXmlLoaded)
                {
                    try
                    {
                        // First access to file, create using template
                        File.Copy(Server.MapPath(Localization.ApplicationResourceDirectory + "/Locales.Portal.xml.config"), Server.MapPath(Localization.ApplicationResourceDirectory + "/Locales.Portal-" + PortalId + ".xml"));
                        xmlLocales.Load(Server.MapPath(Localization.ApplicationResourceDirectory + "/Locales.Portal-" + PortalId + ".xml"));
                        bXmlLoaded = true;
                    }
                    catch
                    {
                        UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("Save.ErrorMessage", this.LocalResourceFile), ModuleMessageType.YellowWarning);
                    }
                }
                if (bXmlLoaded)
                {
                    browserLanguage = xmlLocales.SelectSingleNode("//locales/browserDetection");
                    if (browserLanguage == null)
                    {
//                        XmlNode attr = xmlLocales.CreateNode(XmlNodeType.Attribute, "enabled", "");
                        XmlAttribute attr = xmlLocales.CreateAttribute("enabled", "");

                        browserLanguage = xmlLocales.CreateElement("browserDetection");
                        browserLanguage.Attributes.Append(attr);
                        xmlLocales.SelectSingleNode("//locales").AppendChild(browserLanguage);
                    }
                    browserLanguage.Attributes["enabled"].Value = chkEnableBrowser.Checked.ToString().ToLower();
                    xmlLocales.Save(Server.MapPath(Localization.ApplicationResourceDirectory + "/Locales.Portal-" + PortalId + ".xml"));
                }
            }
            else
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(Server.MapPath(Localization.ApplicationResourceDirectory + "/Locales.xml"));
                browserLanguage = xmldoc.SelectSingleNode("//root/browserDetection");
                if (browserLanguage == null)
                {
//                    XmlNode attr = xmldoc.CreateNode(XmlNodeType.Attribute, "enabled", "");
                    XmlAttribute attr = xmldoc.CreateAttribute("enabled", "");

                    browserLanguage = xmldoc.CreateElement("browserDetection");
                    browserLanguage.Attributes.Append(attr);
                    xmldoc.SelectSingleNode("//root").AppendChild(browserLanguage);
                }
                browserLanguage.Attributes["enabled"].Value = chkEnableBrowser.Checked.ToString().ToLower();
                xmldoc.Save(Server.MapPath(Localization.ApplicationResourceDirectory + "/Locales.xml"));
            }

        }

        protected void rbDisplay_SelectedIndexChanged( Object sender, EventArgs e )
        {
            LoadLocales();
        }

        /// <summary>
        /// Reads XML file and binds to the datagrid
        /// </summary>
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
            // Delete LocalResources from following folders
            DeleteLocalizedFiles( Server.MapPath( "~" ), locale, true );

            // Delete Global/Shared resources
            foreach( string fil in Directory.GetFiles( Server.MapPath( Localization.ApplicationResourceDirectory ) ) )
            {
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
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        private void DeleteLocalizedFiles( string folder, string locale, bool recurse )
        {
            locale = locale.ToLower();

            foreach( string fol in Directory.GetDirectories( folder ) )
            {
                if( Path.GetFileName( fol ) == Localization.LocalResourceDirectory )
                {
                    // Found LocalResources folder
                    foreach( string fil in Directory.GetFiles( fol ) )
                    {
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
            cboLocales.Items.Clear();
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            Array.Sort(cultures, new CultureInfoComparer(rbDisplay.SelectedValue));
            foreach (CultureInfo cinfo in cultures)
            {
                string localeKey = Convert.ToString(cinfo.Name);
                string localeName;
                if (rbDisplay.SelectedValue == "Native")
                {
                    localeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cinfo.NativeName);
                }
                else
                {
                    localeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cinfo.EnglishName);
                }
                cboLocales.Items.Add(new ListItem(localeName, localeKey));
            }

            chkEnableBrowser.Checked = true;
            try
            {
                XmlNode browserLanguage;
                if (PortalSettings.ActiveTab.ParentId == PortalSettings.AdminTabId)
                {
                    if (bXmlLoaded)
                    {
                        browserLanguage = xmlLocales.SelectSingleNode("//locales/browserDetection");
                        if (browserLanguage != null)
                        {
                            chkEnableBrowser.Checked = bool.Parse(browserLanguage.Attributes["enabled"].InnerText);
                        }
                    }
                }
                else
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(Server.MapPath(Localization.ApplicationResourceDirectory + "/Locales.xml"));
                    browserLanguage = xmldoc.SelectSingleNode("//root/browserDetection");
                    if (browserLanguage != null)
                    {
                        chkEnableBrowser.Checked = bool.Parse(browserLanguage.Attributes["enabled"].InnerText);
                    }
                }
            }
            catch
            {
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleInfo FileManagerModule = ( new ModuleController() ).GetModuleByDefinition( Null.NullInteger, "File Manager" );
                string[] args = new string[3];

                args[0] = "mid=" + FileManagerModule.ModuleID;
                args[1] = "ftype=" + UploadType.LanguagePack;
                args[2] = "rtab=" + this.TabId;

                ModuleActionCollection actions = new ModuleActionCollection();
                actions.Add(GetNextActionID(), Localization.GetString("Languages.Action", LocalResourceFile), ModuleActionType.AddContent, "", "", EditUrl("language"), false, SecurityAccessLevel.Admin, true, false);
                actions.Add(GetNextActionID(), Localization.GetString("TimeZones.Action", LocalResourceFile), ModuleActionType.AddContent, "", "", EditUrl("timezone"), false, SecurityAccessLevel.Host, true, false);
                actions.Add(GetNextActionID(), Localization.GetString("Verify.Action", LocalResourceFile), ModuleActionType.AddContent, "", "", EditUrl("verify"), false, SecurityAccessLevel.Host, true, false);
                actions.Add(GetNextActionID(), Localization.GetString("PackageGenerate.Action", LocalResourceFile), ModuleActionType.AddContent, "", "", EditUrl("package"), false, SecurityAccessLevel.Host, true, false);
                actions.Add( GetNextActionID(), Localization.GetString( "PackageImport.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "", Globals.NavigateURL( FileManagerModule.TabID, "Edit", args ), false, SecurityAccessLevel.Host, true, false );
                return actions;
            }
        }


    }
}