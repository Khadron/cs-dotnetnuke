using System;
using System.Collections;
using System.IO;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

namespace DotNetNuke.Modules.Admin.Modules
{
    public partial class Import : PortalModuleBase
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
                    ArrayList arrFiles = Globals.GetFileList( PortalId, "xml", false );
                    FileItem objFile;
                    foreach( FileItem tempLoopVar_objFile in arrFiles )
                    {
                        objFile = tempLoopVar_objFile;
                        if( objFile.Text.IndexOf( "content." + CleanName( objModule.FriendlyName ) ) != - 1 )
                        {
                            cboFiles.Items.Add( objFile.Text );
                        }
                    }
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

        
        protected void cmdImport_Click( object sender, EventArgs e )
        {
            try
            {
                if( cboFiles.SelectedItem != null )
                {
                    string strMessage = ImportModule( moduleId, cboFiles.SelectedItem.Value );
                    if( strMessage == "" )
                    {
                        Response.Redirect( Globals.NavigateURL(), true );
                    }
                    else
                    {
                        UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessage.ModuleMessageType.RedError );
                    }
                }
                else
                {
                    UI.Skins.Skin.AddModuleMessage( this, "Please specify the file to import", ModuleMessage.ModuleMessageType.RedError );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        private string ImportModule( int moduleId, string FileName )
        {
            string strMessage = "";

            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModule( moduleId, TabId );
            if( objModule != null )
            {
                if( FileName.IndexOf( "." + CleanName( objModule.FriendlyName ) + "." ) != - 1 )
                {
                    if( objModule.BusinessControllerClass != "" && objModule.IsPortable )
                    {
                        try
                        {
                            object objObject = Reflection.CreateObject( objModule.BusinessControllerClass, objModule.BusinessControllerClass );

                            if( objObject is IPortable )
                            {
                                StreamReader objStreamReader;
                                objStreamReader = File.OpenText( PortalSettings.HomeDirectoryMapPath + FileName );
                                string Content = objStreamReader.ReadToEnd();
                                objStreamReader.Close();

                                XmlDocument xmlDoc = new XmlDocument();
                                try
                                {
                                    xmlDoc.LoadXml( Content );
                                }
                                catch
                                {
                                    strMessage = Localization.GetString( "NotValidXml", this.LocalResourceFile );
                                }

                                if( strMessage == "" )
                                {
                                    string strType = xmlDoc.DocumentElement.GetAttribute( "type" ).ToString();
                                    if( strType == CleanName( objModule.FriendlyName ) )
                                    {
                                        string strVersion = xmlDoc.DocumentElement.GetAttribute( "version" ).ToString();

                                        ( (IPortable)objObject ).ImportModule( moduleId, xmlDoc.DocumentElement.InnerXml, strVersion, UserInfo.UserID );

                                        Response.Redirect( Globals.NavigateURL(), true );
                                    }
                                    else
                                    {
                                        strMessage = Localization.GetString( "NotCorrectType", this.LocalResourceFile );
                                    }
                                }
                            }
                            else
                            {
                                strMessage = Localization.GetString( "ImportNotSupported", this.LocalResourceFile );
                            }
                        }
                        catch
                        {
                            strMessage = Localization.GetString( "Error", this.LocalResourceFile );
                        }
                    }
                    else
                    {
                        strMessage = Localization.GetString( "ImportNotSupported", this.LocalResourceFile );
                    }
                }
                else
                {
                    strMessage = Localization.GetString( "NotCorrectType", this.LocalResourceFile );
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