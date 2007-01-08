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
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.UserControls;

namespace DotNetNuke.Services.Localization
{
    /// <summary>
    /// Manages translations for Resource files
    /// </summary>
    /// <history>
    /// 	[vmasanas]	10/04/2004  Created
    /// </history>
    public partial class ResourceVerifier : PortalModuleBase
    {
        /// <summary>
        /// Verifies all resource files for all currently supported locales
        /// </summary>
        /// <remarks>
        /// For each file and locale it will:
        /// - check for file existence: the file is localized for each locale
        /// - check for missing keys: all keys is default file are in localized versions
        /// - check for obsolete keys: all keys is localized versions are in default file
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	05/11/2004	Created
        /// </history>
        protected void cmdVerify_Click( object sender, EventArgs e )
        {
            try
            {
                SortedList files = new SortedList();
                LocaleCollection locales = Localization.GetSupportedLocales();

                GetResourceFiles( files, Server.MapPath( "~\\admin" ) );
                GetResourceFiles( files, Server.MapPath( "~\\controls" ) );
                GetResourceFiles( files, Server.MapPath( "~\\desktopmodules" ) );
                // Add global and shared resource files
                files.Add( Server.MapPath( Localization.GlobalResourceFile ), new FileInfo( Server.MapPath( Localization.GlobalResourceFile ) ) );
                files.Add( Server.MapPath( Localization.SharedResourceFile ), new FileInfo( Server.MapPath( Localization.SharedResourceFile ) ) );

                foreach( string localeWithinLoop in locales )
                {
                    string locale = localeWithinLoop;
                    // SectionHead for Locale
                    SectionHeadControl shcTop = (SectionHeadControl)( LoadControl( "~/controls/sectionheadcontrol.ascx" ) );
                    shcTop.Section = localeWithinLoop;
                    shcTop.IncludeRule = true;
                    shcTop.IsExpanded = true;
                    shcTop.CssClass = "Head";
                    shcTop.Text = Localization.GetString( "Locale", this.LocalResourceFile ) + locales[localeWithinLoop].Text + " (" + localeWithinLoop + ")";

                    HtmlTable tableTop = new HtmlTable();
                    tableTop.ID = localeWithinLoop;
                    HtmlTableRow rowTop = new HtmlTableRow();
                    HtmlTableCell cellTop = new HtmlTableCell();

                    HtmlTable tableMissing = new HtmlTable();
                    tableMissing.ID = "Missing" + localeWithinLoop;
                    HtmlTable tableEntries = new HtmlTable();
                    tableEntries.ID = "Entry" + localeWithinLoop;
                    HtmlTable tableObsolete = new HtmlTable();
                    tableObsolete.ID = "Obsolete" + localeWithinLoop;
                    HtmlTable tableOld = new HtmlTable();
                    tableOld.ID = "Old" + localeWithinLoop;
                    HtmlTable tableDuplicate = new HtmlTable();
                    tableDuplicate.ID = "Duplicate" + localeWithinLoop;
                    HtmlTable tableError = new HtmlTable();
                    tableError.ID = "Error" + localeWithinLoop;

                    foreach( DictionaryEntry file in files )
                    {
                        // check for existance
                        if( !( File.Exists( ResourceFile( file.Key.ToString(), localeWithinLoop ) ) ) )
                        {
                            HtmlTableRow row = new HtmlTableRow();
                            HtmlTableCell cell = new HtmlTableCell();
                            cell.InnerText = ResourceFile( file.Key.ToString(), localeWithinLoop ).Replace( Server.MapPath( "~" ), "" );
                            cell.Attributes["Class"] = "Normal";
                            row.Cells.Add( cell );
                            tableMissing.Rows.Add( row );
                        }
                        else
                        {
                            DataSet dsDef = new DataSet();
                            DataSet dsRes = new DataSet();
                            DataTable dtDef = null;
                            DataTable dtRes = null;

                            try
                            {
                                dsDef.ReadXml( file.Key.ToString() );
                            }
                            catch
                            {
                                HtmlTableRow row = new HtmlTableRow();
                                HtmlTableCell cell = new HtmlTableCell();
                                cell.InnerText = file.Key.ToString().Replace( Server.MapPath( "~" ), "" );
                                cell.Attributes["Class"] = "Normal";
                                row.Cells.Add( cell );
                                tableError.Rows.Add( row );
                            }
                            try
                            {
                                dsRes.ReadXml( ResourceFile( file.Key.ToString(), localeWithinLoop ) );
                            }
                            catch
                            {
                                if( localeWithinLoop != Localization.SystemLocale )
                                {
                                    HtmlTableRow row = new HtmlTableRow();
                                    HtmlTableCell cell = new HtmlTableCell();
                                    cell.InnerText = ResourceFile( file.Key.ToString(), localeWithinLoop ).Replace( Server.MapPath( "~" ), "" );
                                    cell.Attributes["Class"] = "Normal";
                                    row.Cells.Add( cell );
                                    tableError.Rows.Add( row );
                                }
                            }

                            if( dsRes != null & dsDef != null )
                            {
                                dtDef = dsDef.Tables["data"];
                                dtDef.TableName = "default";
                                dtRes = dsRes.Tables["data"].Copy();
                                dtRes.TableName = "localized";
                                dsDef.Tables.Add( dtRes );

                                // Check for duplicate entries in localized file
                                try
                                {
                                    // if this fails-> file contains duplicates
                                    UniqueConstraint c = new UniqueConstraint( "uniqueness", dtRes.Columns["name"] );
                                    dtRes.Constraints.Add( c );
                                    dtRes.Constraints.Remove( "uniqueness" );
                                }
                                catch
                                {
                                    HtmlTableRow row = new HtmlTableRow();
                                    HtmlTableCell cell = new HtmlTableCell();
                                    cell.InnerText = ResourceFile( file.Key.ToString(), localeWithinLoop ).Replace( Server.MapPath( "~" ), "" );
                                    cell.Attributes["Class"] = "Normal";
                                    row.Cells.Add( cell );
                                    tableDuplicate.Rows.Add( row );
                                }

                                // Check for missing entries in localized file
                                try
                                {
                                    // if this fails-> some entries in System default file are not found in Resource file
                                    dsDef.Relations.Add( "missing", dtRes.Columns["name"], dtDef.Columns["name"] );
                                }
                                catch
                                {
                                    HtmlTableRow row = new HtmlTableRow();
                                    HtmlTableCell cell = new HtmlTableCell();
                                    cell.InnerText = ResourceFile( file.Key.ToString(), localeWithinLoop ).Replace( Server.MapPath( "~" ), "" );
                                    cell.Attributes["Class"] = "Normal";
                                    row.Cells.Add( cell );
                                    tableEntries.Rows.Add( row );
                                }
                                finally
                                {
                                    dsDef.Relations.Remove( "missing" );
                                }

                                // Check for obsolete entries in localized file
                                try
                                {
                                    // if this fails-> some entries in Resource File are not found in System default
                                    dsDef.Relations.Add( "obsolete", dtDef.Columns["name"], dtRes.Columns["name"] );
                                }
                                catch
                                {
                                    HtmlTableRow row = new HtmlTableRow();
                                    HtmlTableCell cell = new HtmlTableCell();
                                    cell.InnerText = ResourceFile( file.Key.ToString(), localeWithinLoop ).Replace( Server.MapPath( "~" ), "" );
                                    cell.Attributes["Class"] = "Normal";
                                    row.Cells.Add( cell );
                                    tableObsolete.Rows.Add( row );
                                }
                                finally
                                {
                                    dsDef.Relations.Remove( "obsolete" );
                                }

                                // Check older files
                                FileInfo resFile = new FileInfo( ResourceFile( file.Key.ToString(), locale ) );
                                if( ( (FileInfo)file.Value ).LastWriteTime > resFile.LastWriteTime )
                                {
                                    HtmlTableRow row = new HtmlTableRow();
                                    HtmlTableCell cell = new HtmlTableCell();
                                    cell.InnerText = ResourceFile( file.Key.ToString(), localeWithinLoop ).Replace( Server.MapPath( "~" ), "" );
                                    cell.Attributes["Class"] = "Normal";
                                    row.Cells.Add( cell );
                                    tableOld.Rows.Add( row );
                                }
                            }
                        }
                    }

                    SectionHeadControl shc;
                    if( tableMissing.Rows.Count > 0 )
                    {
                        // ------- Missing files
                        shc = (SectionHeadControl)( LoadControl( "~/controls/sectionheadcontrol.ascx" ) );
                        shc.Section = "Missing" + localeWithinLoop;
                        shc.IncludeRule = false;
                        shc.IsExpanded = false;
                        shc.CssClass = "SubHead";
                        shc.Text = Localization.GetString( "MissingFiles", this.LocalResourceFile ) + tableMissing.Rows.Count;
                        cellTop.Controls.Add( shc );
                        cellTop.Controls.Add( tableMissing );
                    }

                    if( tableDuplicate.Rows.Count > 0 )
                    {
                        // ------- Duplicate keys
                        shc = (SectionHeadControl)( LoadControl( "~/controls/sectionheadcontrol.ascx" ) );
                        shc.Section = "Duplicate" + localeWithinLoop;
                        shc.IncludeRule = false;
                        shc.IsExpanded = false;
                        shc.CssClass = "SubHead";
                        shc.Text = Localization.GetString( "DuplicateEntries", this.LocalResourceFile ) + tableDuplicate.Rows.Count;
                        cellTop.Controls.Add( shc );
                        cellTop.Controls.Add( tableDuplicate );
                    }

                    if( tableEntries.Rows.Count > 0 )
                    {
                        // ------- Missing entries
                        shc = (SectionHeadControl)( LoadControl( "~/controls/sectionheadcontrol.ascx" ) );
                        shc.Section = "Entry" + localeWithinLoop;
                        shc.IncludeRule = false;
                        shc.IsExpanded = false;
                        shc.CssClass = "SubHead";
                        shc.Text = Localization.GetString( "MissingEntries", this.LocalResourceFile ) + tableEntries.Rows.Count;
                        cellTop.Controls.Add( shc );
                        cellTop.Controls.Add( tableEntries );
                    }

                    if( tableObsolete.Rows.Count > 0 )
                    {
                        // ------- Missing entries
                        shc = (SectionHeadControl)( LoadControl( "~/controls/sectionheadcontrol.ascx" ) );
                        shc.Section = "Obsolete" + localeWithinLoop;
                        shc.IncludeRule = false;
                        shc.IsExpanded = false;
                        shc.CssClass = "SubHead";
                        shc.Text = Localization.GetString( "ObsoleteEntries", this.LocalResourceFile ) + tableObsolete.Rows.Count;
                        cellTop.Controls.Add( shc );
                        cellTop.Controls.Add( tableObsolete );
                    }

                    if( tableOld.Rows.Count > 0 )
                    {
                        // ------- Old files
                        shc = (SectionHeadControl)( LoadControl( "~/controls/sectionheadcontrol.ascx" ) );
                        shc.Section = "Old" + localeWithinLoop;
                        shc.IncludeRule = false;
                        shc.IsExpanded = false;
                        shc.CssClass = "SubHead";
                        shc.Text = Localization.GetString( "OldFiles", this.LocalResourceFile ) + tableOld.Rows.Count;
                        cellTop.Controls.Add( shc );
                        cellTop.Controls.Add( tableOld );
                    }

                    if( tableError.Rows.Count > 0 )
                    {
                        // ------- Error files
                        shc = (SectionHeadControl)( LoadControl( "~/controls/sectionheadcontrol.ascx" ) );
                        shc.Section = "Error" + localeWithinLoop;
                        shc.IncludeRule = false;
                        shc.IsExpanded = false;
                        shc.CssClass = "SubHead";
                        shc.Text = Localization.GetString( "ErrorFiles", this.LocalResourceFile ) + tableError.Rows.Count;
                        cellTop.Controls.Add( shc );
                        cellTop.Controls.Add( tableError );
                    }

                    rowTop.Cells.Add( cellTop );
                    tableTop.Rows.Add( rowTop );
                    PlaceHolder1.Controls.Add( shcTop );
                    PlaceHolder1.Controls.Add( tableTop );
                    PlaceHolder1.Controls.Add( new LiteralControl( "<br>" ) );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

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
        /// Gets all system default resource files
        /// </summary>
        /// <param name="fileList">List of found resource files</param>
        /// <param name="_path">Folder to search at</param>
        /// <history>
        /// 	[Vicenç]	05/11/2004	Created
        /// </history>
        private void GetResourceFiles( SortedList fileList, string _path )
        {
            string[] folders = Directory.GetDirectories( _path );

            foreach( string folder in folders )
            {
                DirectoryInfo objFolder = new DirectoryInfo( folder );

                if( objFolder.Name == Localization.LocalResourceDirectory )
                {
                    // found local resource folder, add resources
                    foreach( FileInfo objFile in objFolder.GetFiles( "*.ascx.resx" ) )
                    {
                        
                        fileList.Add( objFile.FullName, objFile );
                    }
                    foreach( FileInfo objFile in objFolder.GetFiles( "*.aspx.resx" ) )
                    {                        
                        fileList.Add( objFile.FullName, objFile );
                    }
                    // add LocalSharedResources if found
                    if( File.Exists( Path.Combine( folder, Localization.LocalSharedResourceFile ) ) )
                    {
                        fileList.Add( Path.Combine( folder, Localization.LocalSharedResourceFile ), new FileInfo( Path.Combine( folder, Localization.LocalSharedResourceFile ) ) );
                    }
                }
                else
                {
                    GetResourceFiles( fileList, folder );
                }
            }
        }

        /// <summary>
        /// Returns the resource file name for a given locale
        /// </summary>
        /// <param name="filename">Resource file</param>
        /// <param name="language">Locale</param>
        /// <history>
        /// 	[Vicenç]	05/11/2004	Created
        /// </history>
        private string ResourceFile( string filename, string language )
        {
            string resourcefilename = filename;

            if( language != Localization.SystemLocale )
            {
                resourcefilename = resourcefilename.Replace( ".resx", "." + language + ".resx" );
            }

            return resourcefilename;
        }
    }
}