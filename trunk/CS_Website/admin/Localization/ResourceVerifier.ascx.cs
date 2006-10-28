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
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[vmasanas]	10/04/2004  Created
    /// </history>
    public partial class ResourceVerifier : PortalModuleBase
    {
        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }

        /// <summary>
        /// Verifies all resource files for all currently supported locales
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// For each file and locale it will:
        /// - check for file existence: the file is localized for each locale
        /// - check for missing keys: all keys is default file are in localized versions
        /// - check for obsolete keys: all keys is localized versions are in default file
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	05/11/2004	Created
        /// </history>
        protected void cmdVerify_Click( Object sender, EventArgs e )
        {
            try
            {
                SortedList files = new SortedList();
                LocaleCollection locales = Localization.GetSupportedLocales();
                string locale;
                SectionHeadControl shc;
                SectionHeadControl shcTop;

                GetResourceFiles( files, Server.MapPath( "~\\admin" ) );
                GetResourceFiles( files, Server.MapPath( "~\\controls" ) );
                GetResourceFiles( files, Server.MapPath( "~\\desktopmodules" ) );
                // Add global and shared resource files
                files.Add( Server.MapPath( Localization.GlobalResourceFile ), new FileInfo( Server.MapPath( Localization.GlobalResourceFile ) ) );
                files.Add( Server.MapPath( Localization.SharedResourceFile ), new FileInfo( Server.MapPath( Localization.SharedResourceFile ) ) );

                foreach( string tempLoopVar_locale in locales )
                {
                    locale = tempLoopVar_locale;
                    // SectionHead for Locale
                    shcTop = (SectionHeadControl)LoadControl( "~/controls/sectionheadcontrol.ascx" );
                    shcTop.Section = locale;
                    shcTop.IncludeRule = true;
                    shcTop.IsExpanded = true;
                    shcTop.CssClass = "Head";
                    shcTop.Text = Localization.GetString( "Locale", this.LocalResourceFile ) + locales[ locale ].Text + " (" + locale + ")";

                    HtmlTable tableTop = new HtmlTable();
                    tableTop.ID = locale;
                    HtmlTableRow rowTop = new HtmlTableRow();
                    HtmlTableCell cellTop = new HtmlTableCell();

                    HtmlTable tableMissing = new HtmlTable();
                    tableMissing.ID = "Missing" + locale;
                    HtmlTable tableEntries = new HtmlTable();
                    tableEntries.ID = "Entry" + locale;
                    HtmlTable tableObsolete = new HtmlTable();
                    tableObsolete.ID = "Obsolete" + locale;
                    HtmlTable tableOld = new HtmlTable();
                    tableOld.ID = "Old" + locale;
                    HtmlTable tableDuplicate = new HtmlTable();
                    tableDuplicate.ID = "Duplicate" + locale;

                    foreach( DictionaryEntry file in files )
                    {
                        // check for existance
                        if( ! File.Exists( ResourceFile( file.Key.ToString(), locale ) ) )
                        {
                            HtmlTableRow row = new HtmlTableRow();
                            HtmlTableCell cell = new HtmlTableCell();
                            cell.InnerText = ResourceFile( file.Key.ToString(), locale ).Replace( Server.MapPath( "~" ), "" );
                            cell.Attributes[ "Class"] = "Normal";
                            row.Cells.Add( cell );
                            tableMissing.Rows.Add( row );
                        }
                        else
                        {
                            DataSet dsDef = new DataSet();
                            DataSet dsRes = new DataSet();
                            DataTable dtDef;
                            DataTable dtRes;
                            dsDef.ReadXml( file.Key.ToString() );
                            dsRes.ReadXml( ResourceFile( file.Key.ToString(), locale ) );

                            dtDef = dsDef.Tables[ 0 ];
                            dtDef.TableName = "default";
                            dtRes = dsRes.Tables[ 0 ].Copy();
                            dtRes.TableName = "localized";
                            dsDef.Tables.Add( dtRes );

                            // Check for duplicate entries in localized file
                            try
                            {
                                // if this fails-> file contains duplicates
                                UniqueConstraint c = new UniqueConstraint( "uniqueness", dtRes.Columns[ "name" ] );
                                dtRes.Constraints.Add( c );
                                dtRes.Constraints.Remove( "uniqueness" );
                            }
                            catch
                            {
                                HtmlTableRow row = new HtmlTableRow();
                                HtmlTableCell cell = new HtmlTableCell();
                                cell.InnerText = ResourceFile( file.Key.ToString(), locale ).Replace( Server.MapPath( "~" ), "" );
                                cell.Attributes[ "Class" ] = "Normal";
                                row.Cells.Add( cell );
                                tableDuplicate.Rows.Add( row );
                            }

                            // Check for missing entries in localized file
                            try
                            {
                                // if this fails-> some entries in System default file are not found in Resource file
                                dsDef.Relations.Add( "missing", dtRes.Columns[ "name" ], dtDef.Columns[ "name" ] );
                            }
                            catch
                            {
                                HtmlTableRow row = new HtmlTableRow();
                                HtmlTableCell cell = new HtmlTableCell();
                                cell.InnerText = ResourceFile( file.Key.ToString(), locale ).Replace( Server.MapPath( "~" ), "" );
                                cell.Attributes[ "Class" ] = "Normal";
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
                                dsDef.Relations.Add( "obsolete", dtDef.Columns[ "name" ], dtRes.Columns[ "name" ] );
                            }
                            catch
                            {
                                HtmlTableRow row = new HtmlTableRow();
                                HtmlTableCell cell = new HtmlTableCell();
                                cell.InnerText = ResourceFile( file.Key.ToString(), locale ).Replace( Server.MapPath( "~" ), "" );
                                cell.Attributes[ "Class" ] = "Normal";
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
                                cell.InnerText = ResourceFile( file.Key.ToString(), locale ).Replace( Server.MapPath( "~" ), "" );
                                cell.Attributes[ "Class" ] = "Normal";
                                row.Cells.Add( cell );
                                tableOld.Rows.Add( row );
                            }
                        }
                    }

                    if( tableMissing.Rows.Count > 0 )
                    {
                        // ------- Missing files
                        shc = (SectionHeadControl)LoadControl( "~/controls/sectionheadcontrol.ascx" );
                        shc.Section = "Missing" + locale;
                        shc.IncludeRule = false;
                        shc.IsExpanded = false;
                        shc.CssClass = "SubHead";
                        shc.Text = Localization.GetString( "MissingFiles", this.LocalResourceFile ) + tableMissing.Rows.Count.ToString();
                        cellTop.Controls.Add( shc );
                        cellTop.Controls.Add( tableMissing );
                    }

                    if( tableDuplicate.Rows.Count > 0 )
                    {
                        // ------- Duplicate keys
                        shc = (SectionHeadControl)LoadControl( "~/controls/sectionheadcontrol.ascx" );
                        shc.Section = "Duplicate" + locale;
                        shc.IncludeRule = false;
                        shc.IsExpanded = false;
                        shc.CssClass = "SubHead";
                        shc.Text = Localization.GetString( "DuplicateEntries", this.LocalResourceFile ) + tableDuplicate.Rows.Count.ToString();
                        cellTop.Controls.Add( shc );
                        cellTop.Controls.Add( tableDuplicate );
                    }

                    if( tableEntries.Rows.Count > 0 )
                    {
                        // ------- Missing entries
                        shc = (SectionHeadControl)LoadControl( "~/controls/sectionheadcontrol.ascx" );
                        shc.Section = "Entry" + locale;
                        shc.IncludeRule = false;
                        shc.IsExpanded = false;
                        shc.CssClass = "SubHead";
                        shc.Text = Localization.GetString( "MissingEntries", this.LocalResourceFile ) + tableEntries.Rows.Count.ToString();
                        cellTop.Controls.Add( shc );
                        cellTop.Controls.Add( tableEntries );
                    }

                    if( tableObsolete.Rows.Count > 0 )
                    {
                        // ------- Missing entries
                        shc = (SectionHeadControl)LoadControl( "~/controls/sectionheadcontrol.ascx" );
                        shc.Section = "Obsolete" + locale;
                        shc.IncludeRule = false;
                        shc.IsExpanded = false;
                        shc.CssClass = "SubHead";
                        shc.Text = Localization.GetString( "ObsoleteEntries", this.LocalResourceFile ) + tableObsolete.Rows.Count.ToString();
                        cellTop.Controls.Add( shc );
                        cellTop.Controls.Add( tableObsolete );
                    }

                    if( tableOld.Rows.Count > 0 )
                    {
                        // ------- Old files
                        shc = (SectionHeadControl)LoadControl( "~/controls/sectionheadcontrol.ascx" );
                        shc.Section = "Old" + locale;
                        shc.IncludeRule = false;
                        shc.IsExpanded = false;
                        shc.CssClass = "SubHead";
                        shc.Text = Localization.GetString( "OldFiles", this.LocalResourceFile ) + tableOld.Rows.Count.ToString();
                        cellTop.Controls.Add( shc );
                        cellTop.Controls.Add( tableOld );
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
                Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Gets all system default resource files
        /// </summary>
        /// <param name="fileList">List of found resource files</param>
        /// <param name="_path">Folder to search at</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Vicenç]	05/11/2004	Created
        /// </history>
        private void GetResourceFiles( SortedList fileList, string _path )
        {
            string[] folders = Directory.GetDirectories( _path );
            string folder;
            FileInfo objFile;
            DirectoryInfo objFolder;

            foreach( string tempLoopVar_folder in folders )
            {
                folder = tempLoopVar_folder;
                objFolder = new DirectoryInfo( folder );

                if( objFolder.Name == Localization.LocalResourceDirectory )
                {
                    // found local resource folder, add resources
                    foreach( FileInfo tempLoopVar_objFile in objFolder.GetFiles( "*.ascx.resx" ) )
                    {
                        objFile = tempLoopVar_objFile;
                        fileList.Add( objFile.FullName, objFile );
                    }
                    foreach( FileInfo tempLoopVar_objFile in objFolder.GetFiles( "*.aspx.resx" ) )
                    {
                        objFile = tempLoopVar_objFile;
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
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
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