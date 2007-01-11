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
using System.IO;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.Skins.Controls;

namespace DotNetNuke.Services.Localization
{
    /// <summary>
    /// Manages translations for TimeZones file
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[vmasanas]	10/04/2004  Created
    /// </history>
    public partial class TimeZoneEditor : PortalModuleBase
    {
        /// <summary>
        /// Loads suported locales and shows default values
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
                if( ! Page.IsPostBack )
                {
                    // Localize datagrid
                    Localization.LocalizeDataGrid( ref dgEditor, this.LocalResourceFile );
                    BindList();
                    cboLocales.SelectedValue = Localization.SystemLocale;
                    BindGrid( Localization.SystemLocale );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// Loads localized file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// If a localized file does not exist for the selected language it is created using default values
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        protected void cboLocales_SelectedIndexChanged( Object sender, EventArgs e )
        {
            try
            {
                try
                {
                    if( ! File.Exists( Server.MapPath( ResourceFile( Localization.TimezonesFile, cboLocales.SelectedValue ) ) ) )
                    {
                        File.Copy( Server.MapPath( Localization.TimezonesFile ), Server.MapPath( ResourceFile( Localization.TimezonesFile, cboLocales.SelectedValue ) ) );
                    }
                }
                catch
                {
                    UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                }
                BindGrid( cboLocales.SelectedValue );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// Updates all values from the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        protected void cmdUpdate_Click( Object sender, EventArgs e )
        {
            DataGridItem di;
            XmlNode node;
            XmlNode parent;
            XmlDocument resDoc = new XmlDocument();

            try
            {
                resDoc.Load( Server.MapPath( ResourceFile( Localization.TimezonesFile, cboLocales.SelectedValue ) ) );
                foreach( DataGridItem tempLoopVar_di in dgEditor.Items )
                {
                    di = tempLoopVar_di;
                    if( di.ItemType == ListItemType.Item || di.ItemType == ListItemType.AlternatingItem )
                    {
                        TextBox ctl = (TextBox)di.Cells[0].Controls[1];
                        node = resDoc.SelectSingleNode( "//root/timezone[@key='" + di.Cells[1].Text + "']" );
                        node.Attributes["name"].Value = ctl.Text;
                    }
                }
                try
                {
                    File.SetAttributes( Server.MapPath( ResourceFile( Localization.TimezonesFile, cboLocales.SelectedValue ) ), FileAttributes.Normal );
                    resDoc.Save( Server.MapPath( ResourceFile( Localization.TimezonesFile, cboLocales.SelectedValue ) ) );
                    BindGrid( cboLocales.SelectedValue );
                }
                catch
                {
                    UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// Returns to main control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        protected void cmdCancel_Click( Object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Globals.NavigateURL() );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// Adds missing nodes from the System Default file to the Resource file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	05/10/2004	Created
        /// </history>
        protected void cmdAddMissing_Click( Object sender, EventArgs e )
        {
            XmlNode node;
            XmlNode parent;
            XmlDocument resDoc = new XmlDocument();
            XmlDocument defDoc = new XmlDocument();

            resDoc.Load( Server.MapPath( ResourceFile( Localization.TimezonesFile, cboLocales.SelectedValue ) ) );
            defDoc.Load( Server.MapPath( Localization.TimezonesFile ) );

            foreach( XmlNode tempLoopVar_node in defDoc.SelectNodes( "//root/timezone" ) )
            {
                node = tempLoopVar_node;
                if( resDoc.SelectSingleNode( "//root/timezone[@key='" + node.Attributes["key"].Value + "']" ) == null )
                {
                    resDoc.SelectSingleNode( "//root" ).AppendChild( resDoc.ImportNode( node, true ) );
                }
            }
            try
            {
                resDoc.Save( Server.MapPath( ResourceFile( Localization.TimezonesFile, cboLocales.SelectedValue ) ) );
                BindGrid( cboLocales.SelectedValue );
            }
            catch
            {
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
            }
        }

        /// <summary>
        /// Loads suported locales
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        private void BindList()
        {
            DataSet ds = new DataSet();
            DataView dv;

            ds.ReadXml( Server.MapPath( Localization.SupportedLocalesFile ) );
            dv = ds.Tables[0].DefaultView;
            dv.Sort = "name ASC";

            cboLocales.DataSource = dv;
            cboLocales.DataBind();
        }

        /// <summary>
        /// Loads TimeZone information
        /// </summary>
        /// <param name="language">Language to be loaded</param>
        /// <remarks>
        /// If Localized file contains entries not found in the System Default they will be deleted.
        /// If System Default contains entries not found in the Localized file user will be asked to add them.
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        private void BindGrid( string language )
        {
            DataSet ds = new DataSet();
            DataSet dsDef = new DataSet();
            DataTable dt;
            DataTable dtDef;
            DataView dv;

            ds.ReadXml( Server.MapPath( ResourceFile( Localization.TimezonesFile, language ) ) );
            ds.Tables[0].TableName = "Resource";
            dt = ds.Tables["Resource"];

            dsDef.ReadXml( Server.MapPath( Localization.TimezonesFile ) );
            dtDef = dsDef.Tables[0].Copy();
            dtDef.TableName = "Default";
            dtDef.Columns["name"].ColumnName = "defaultvalue";
            ds.Tables.Add( dtDef );

            // Check for missing entries in localized file
            try
            {
                pnlMissing.Visible = false;
                // if this fails-> some entries in System default file are not found in Resource file
                ds.Relations.Add( "missing", dt.Columns["key"], dtDef.Columns["key"] );
            }
            catch
            {
                pnlMissing.Visible = true;
            }
            finally
            {
                ds.Relations.Remove( "missing" );
            }

            // Relate localized entries to System default
            try
            {
                // if this fails-> some entries in Resource file are not found in System default
                ds.Relations.Add( "defaultvalues", dtDef.Columns["key"], dt.Columns["key"] );
            }
            catch
            {
                // delete orphan entries in localized file
                DeleteEntries( ResourceFile( Localization.TimezonesFile, language ), Localization.TimezonesFile );
                ds.Relations.Remove( "defaultvalues" );
                // reload data
                ds = new DataSet();
                dsDef = new DataSet();
                ds.ReadXml( Server.MapPath( ResourceFile( Localization.TimezonesFile, language ) ) );
                ds.Tables[0].TableName = "Resource";
                dt = ds.Tables["Resource"];

                dsDef.ReadXml( Server.MapPath( Localization.TimezonesFile ) );
                dtDef = dsDef.Tables[0].Copy();
                dtDef.TableName = "Default";
                dtDef.Columns["name"].ColumnName = "defaultvalue";
                ds.Tables.Add( dtDef );
                ds.Relations.Add( "defaultvalues", dtDef.Columns["key"], dt.Columns["key"] );
            }

            dgEditor.DataSource = ds;
            dgEditor.DataMember = "Resource";
            dgEditor.DataBind();
        }

        /// <summary>
        /// Removes nodes in localized file not found in System default
        /// </summary>
        /// <param name="resourceFile">Resource file</param>
        /// <param name="defaultFile">System Default resource file</param>
        /// <remarks>
        /// Deletes the nodes in the resource file as saves it
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	05/10/2004	Created
        /// </history>
        private void DeleteEntries( string resourceFile, string defaultFile )
        {
            XmlNode node;
            XmlNode parent;
            XmlDocument resDoc = new XmlDocument();
            XmlDocument defDoc = new XmlDocument();

            resDoc.Load( Server.MapPath( resourceFile ) );
            defDoc.Load( Server.MapPath( defaultFile ) );

            foreach( XmlNode tempLoopVar_node in resDoc.SelectNodes( "//root/timezone" ) )
            {
                node = tempLoopVar_node;
                if( defDoc.SelectSingleNode( "//root/timezone[@key='" + node.Attributes["key"].Value + "']" ) == null )
                {
                    parent = node.ParentNode;
                    parent.RemoveChild( node );
                }
            }

            try
            {
                resDoc.Save( Server.MapPath( resourceFile ) );
            }
            catch
            {
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessageType.YellowWarning );
            }
        }

        /// <summary>
        /// Returns the resource file name for a given resource and language
        /// </summary>
        /// <param name="filename">Resource File</param>
        /// <param name="language">Language</param>
        /// <returns>Localized File Name</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        private string ResourceFile( string filename, string language )
        {
            if( language == Localization.SystemLocale )
            {
                return filename;
            }
            else
            {
                return filename.Substring( 0, filename.Length - 4 ) + "." + language + ".xml";
            }
        }

    }
}