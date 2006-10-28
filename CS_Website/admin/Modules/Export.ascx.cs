using System;
using System.IO;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using FileInfo=System.IO.FileInfo;

namespace DotNetNuke.Modules.Admin.Modules
{
    public partial class Export : PortalModuleBase
    {
        private int moduleId = - 1;

        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( Request.QueryString["moduleid"] != null )
                {
                    moduleId = int.Parse( Request.QueryString["moduleid"] );
                }

                ModuleController objModules = new ModuleController();
                ModuleInfo objModule = objModules.GetModule( moduleId, TabId );
                if( objModule != null )
                {
                    lblFile.Text = "content." + CleanName( objModule.FriendlyName ) + "." + CleanName( objModule.ModuleTitle ) + ".xml";
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }


        protected void cmdCancel_Click( object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Globals.NavigateURL(), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void cmdExport_Click( object sender, EventArgs e )
        {
            try
            {
                string strMessage = ExportModule( moduleId, lblFile.Text );
                if( strMessage == "" )
                {
                    Response.Redirect( Globals.NavigateURL(), true );
                }
                else
                {
                    UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessage.ModuleMessageType.RedError );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        private string ExportModule( int ModuleID, string FileName )
        {
            string strMessage = "";

            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModule( ModuleID, TabId );
            if( objModule != null )
            {
                if( objModule.BusinessControllerClass != "" && objModule.IsPortable )
                {
                    try
                    {
                        object objObject = Reflection.CreateObject( objModule.BusinessControllerClass, objModule.BusinessControllerClass );

                        //Double-check
                        if( objObject is IPortable )
                        {
                            string Content = Convert.ToString( ( (IPortable)objObject ).ExportModule( ModuleID ) );

                            if( Content != "" )
                            {
                                // add attributes to XML document
                                Content = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + "<content type=\"" + CleanName( objModule.FriendlyName ) + "\" version=\"" + objModule.Version + "\">" + Content + "</content>";

                                //First check the Portal limits will not be exceeded (this is approximate)
                                PortalController objPortalController = new PortalController();
                                string strFile = PortalSettings.HomeDirectoryMapPath + FileName;
                                if( objPortalController.HasSpaceAvailable( PortalId, Content.Length ) )
                                {
                                    // save the file
                                    StreamWriter objStream;
                                    objStream = File.CreateText( strFile );
                                    objStream.WriteLine( Content );
                                    objStream.Close();

                                    // add file to Files table
                                    FileController objFiles = new FileController();
                                    FileInfo finfo = new FileInfo( strFile );
                                    objFiles.AddFile( PortalId, lblFile.Text, "xml", finfo.Length, 0, 0, "application/octet-stream", "" );
                                }
                                else
                                {
                                    strMessage += "<br>" + string.Format( Localization.GetString( "DiskSpaceExceeded" ), strFile, null );
                                }
                            }
                            else
                            {
                                strMessage = Localization.GetString( "NoContent", this.LocalResourceFile );
                            }
                        }
                        else
                        {
                            strMessage = Localization.GetString( "ExportNotSupported", this.LocalResourceFile );
                        }
                    }
                    catch
                    {
                        strMessage = Localization.GetString( "Error", this.LocalResourceFile );
                    }
                }
                else
                {
                    strMessage = Localization.GetString( "ExportNotSupported", this.LocalResourceFile );
                }
            }

            return strMessage;
        }

        private string CleanName( string Name )
        {
            string strName = Name;
            string strBadChars = ". ~`!@#$%^&*()-_+={[}]|\\:;<,>?/" + '\u0022' + '\u0027';

            int intCounter;
            for( intCounter = 0; intCounter <= strBadChars.Length - 1; intCounter++ )
            {
                strName = strName.Replace( strBadChars.Substring( intCounter, 1 ), "" );
            }

            return strName;
        }


        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
    }
}