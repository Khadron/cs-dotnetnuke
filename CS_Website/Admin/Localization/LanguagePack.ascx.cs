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
using System.IO;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;


namespace DotNetNuke.Services.Localization
{
    public partial class LanguagePack : PortalModuleBase
    {
        protected void Page_Load( Object sender, EventArgs e )
        {
            if( ! Page.IsPostBack )
            {
                LocaleCollection locales = Localization.GetSupportedLocales();
                cboLanguage.DataSource = new LocaleCollectionWrapper( locales );
                cboLanguage.DataTextField = "Text";
                cboLanguage.DataValueField = "Code";
                cboLanguage.DataBind();
                cboLanguage.Items.Insert( 0, new ListItem( "English", Localization.SystemLocale ) );

                rowitems.Visible = false;

                lbItems.Attributes.Add( "onchange", "changeItem('" + lbItems.ClientID + "','" + txtFileName.ClientID + "')" );
            }
        }

        protected void rbPackType_SelectedIndexChanged( Object sender, EventArgs e )
        {
            pnlLogs.Visible = false;
            switch( (LanguagePackType)Enum.Parse( typeof( LanguagePackType ), rbPackType.SelectedValue ) )
            {
                case LanguagePackType.Core:

                    rowitems.Visible = false;
                    txtFileName.Text = "Core";
                    lblFilenameFix.Text = Server.HtmlEncode( ".<version>.<locale>.zip" );
                    break;

                case LanguagePackType.Module:

                    rowitems.Visible = true;
                    lbItems.Items.Clear();
                    lbItems.ClearSelection();
                    lbItems.SelectionMode = ListSelectionMode.Multiple;
                    txtFileName.Text = "Module.version";
                    lblFilenameFix.Text = Server.HtmlEncode( ".<locale>.zip" );

                    DesktopModuleController objDesktopModules = new DesktopModuleController();
                    foreach( DesktopModuleInfo objDM in objDesktopModules.GetDesktopModulesByPortal( PortalSettings.PortalId ) )
                    {
                        if( Null.IsNull( objDM.Version ) )
                        {
                            lbItems.Items.Add( new ListItem( objDM.FriendlyName, objDM.FolderName ) );
                        }
                        else
                        {
                            lbItems.Items.Add( new ListItem( objDM.FriendlyName + " [" + objDM.Version + "]", objDM.FolderName ) );
                        }
                    }
                    lblItems.Text = Localization.GetString( "SelectModules", LocalResourceFile );
                    break;

                case LanguagePackType.Provider:

                    rowitems.Visible = true;
                    lbItems.Items.Clear();
                    lbItems.ClearSelection();
                    lbItems.SelectionMode = ListSelectionMode.Multiple;
                    txtFileName.Text = "Provider.version";
                    lblFilenameFix.Text = Server.HtmlEncode( ".<locale>.zip" );

                    DirectoryInfo objFolder = new DirectoryInfo( Server.MapPath( "~/Providers/HtmlEditorProviders" ) );
                    foreach( DirectoryInfo folder in objFolder.GetDirectories() )
                    {
                        lbItems.Items.Add( new ListItem( folder.Name, folder.Name ) );
                    }

                    lblItems.Text = Localization.GetString( "SelectProviders", LocalResourceFile );
                    break;

                case LanguagePackType.Full:

                    rowitems.Visible = false;
                    txtFileName.Text = "Full";
                    lblFilenameFix.Text = Server.HtmlEncode( ".<version>.<locale>.zip" );
                    break;
            }
        }

        protected void cmdCreate_Click( Object sender, EventArgs e )
        {
            LocaleFilePackWriter LangPackWriter = new LocaleFilePackWriter();
            Locale LocaleCulture = new Locale();
            LocaleCulture.Code = cboLanguage.SelectedValue;
            LocaleCulture.Text = cboLanguage.SelectedItem.Text;

            LanguagePackType packtype = (LanguagePackType)( @Enum.Parse( typeof( LanguagePackType ), rbPackType.SelectedValue ) );
            ArrayList basefolders = new ArrayList();
            if( packtype == LanguagePackType.Module || packtype == LanguagePackType.Provider )
            {
                foreach( ListItem l in lbItems.Items )
                {
                    if( l.Selected )
                    {
                        basefolders.Add( l.Value );
                    }
                }
            }
            //verify filename
            txtFileName.Text = Globals.CleanFileName( txtFileName.Text );

            string LangPackName = LangPackWriter.SaveLanguagePack( LocaleCulture, packtype, basefolders, txtFileName.Text );

            if( LangPackWriter.ProgressLog.Valid )
            {
                lblMessage.Text = string.Format( Localization.GetString( "LOG.MESSAGE.Success", LocalResourceFile ), LangPackName, null );
                lblMessage.CssClass = "Head";
                hypLink.Text = string.Format( Localization.GetString( "Download", LocalResourceFile ), Path.GetFileName( LangPackName ), null );
                hypLink.NavigateUrl = Globals.HostPath + Path.GetFileName( LangPackName );
                hypLink.Visible = true;
            }
            else
            {
                lblMessage.Text = Localization.GetString( "LOG.MESSAGE.Error", LocalResourceFile );
                lblMessage.CssClass = "NormalRed";
                hypLink.Visible = false;
            }
            divLog.Controls.Add( LangPackWriter.ProgressLog.GetLogsTable() );
            pnlLogs.Visible = true;
        }

        protected void cmdCancel_Click( Object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Globals.NavigateURL() );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}