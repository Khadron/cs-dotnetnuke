using System;
using System.IO;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.Skins.Controls;

namespace DotNetNuke.Services.Localization
{
    /// <summary>
    /// Manages translations for Resource files
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[vmasanas]	10/04/2004  Created
    /// </history>
    public partial class LanguageEditorExt : PortalModuleBase
    {
        private string name;
        private string resfile;
        private string locale;
        private string mode;

        /// <summary>
        /// Loads resources from file
        /// </summary>
        /// <param name="modeName">Active editor mode</param>
        /// <param name="type">Resource being loaded (edit or default)</param>
        /// <returns></returns>
        /// <remarks>
        /// Depending on the editor mode, resources will be overrided using default DNN schema.
        /// "Edit" resources will only load selected file.
        /// When loading "Default" resources (to be used on the editor as helpers) fallback resource
        /// chain will be used in order for the editor to be able to correctly see what
        /// is the current default value for the any key. This process depends on the current active
        /// editor mode:
        /// - System: when editing system base resources on en-US needs to be loaded
        /// - Host: base en-US, and base locale especific resource
        /// - Portal: base en-US, host override for en-US, base locale especific resource, and host override
        /// for locale
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	25/03/2006	Created
        /// </history>
        private string LoadFile( string modeName, string type )
        {
            string file;
            string t = "";

            switch( type )
            {
                case "Edit":

                    // Only load resources from the file being edited
                    file = ResourceFile( locale, modeName );
                    string temp = LoadResource( file );
                    if( temp != null )
                    {
                        t = temp;
                    }
                    break;
                case "Default":

                    // Load system default
                    file = ResourceFile( Localization.SystemLocale, "System" );
                    t = LoadResource( file );

                    switch( modeName )
                    {
                        case "Host":

                            if( locale != Localization.SystemLocale )
                            {
                                // Load base file for selected locale
                                file = ResourceFile( locale, "System" );
                                temp = LoadResource( file );
                                if( temp != null )
                                {
                                    t = temp;
                                }
                            }
                            break;
                        case "Portal":

                            //Load host override for default locale
                            file = ResourceFile( Localization.SystemLocale, "Host" );
                            temp = LoadResource( file );
                            if( temp != null )
                            {
                                t = temp;
                            }

                            if( locale != Localization.SystemLocale )
                            {
                                // Load base file for locale
                                file = ResourceFile( locale, "System" );
                                temp = LoadResource( file );
                                if( temp != null )
                                {
                                    t = temp;
                                }

                                //Load host override for selected locale
                                file = ResourceFile( locale, "Host" );
                                temp = LoadResource( file );
                                if( temp != null )
                                {
                                    t = temp;
                                }
                            }
                            break;
                    }
                    break;
            }

            return t;
        }

        /// <summary>
        /// Loads resource from file
        /// </summary>
        /// <param name="filepath">Resources file</param>
        /// <returns>Resource value</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	25/03/2006	Created
        /// </history>
        private string LoadResource( string filepath )
        {
            XmlDocument d = new XmlDocument();
            bool xmlLoaded;
            string ret = null;
            try
            {
                d.Load( filepath );
                xmlLoaded = true;
            }
            catch //exc As Exception
            {
                xmlLoaded = false;
            }
            if( xmlLoaded )
            {
                XmlNode node;
                node = d.SelectSingleNode( "//root/data[@name=\'" + name + "\']/value" );
                if( node != null )
                {
                    ret = node.InnerXml;
                }
            }
            return ret;
        }

        /// <summary>
        /// Returns the resource file name for a given resource and language
        /// </summary>
        /// <param name="modeName">Identifies the resource being searched (System, Host, Portal)</param>
        /// <returns>Localized File Name</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// 	[vmasanas]	25/03/2006	Modified to support new host resources and incremental saving
        /// </history>
        private string ResourceFile( string language, string modeName )
        {
            string resourcefilename = Server.MapPath( "~\\" + resfile );

            if( ! resourcefilename.EndsWith( ".resx" ) )
            {
                resourcefilename += ".resx";
            }

            if( language != Localization.SystemLocale )
            {
                resourcefilename = resourcefilename.Substring( 0, resourcefilename.Length - 5 ) + "." + language + ".resx";
            }

            if( modeName == "Host" )
            {
                resourcefilename = resourcefilename.Substring( 0, resourcefilename.Length - 5 ) + "." + "Host.resx";
            }
            else if( modeName == "Portal" )
            {
                resourcefilename = resourcefilename.Substring( 0, resourcefilename.Length - 5 ) + "." + "Portal-" + PortalId.ToString() + ".resx";
            }

            return resourcefilename;
        }

        /// <summary>
        /// Loads resource file and default data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	07/10/2004	Created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {

            try
            {
                string defaultValue;
                string editValue;

                name = Request.QueryString["name"];
                resfile = Globals.QueryStringDecode( Request.QueryString["resourcefile"] );
                locale = Request.QueryString["locale"];
                mode = Request.QueryString["mode"];
                if( PortalSettings.ActiveTab.ParentId == PortalSettings.AdminTabId )
                {
                    mode = "Portal";
                }
                if( ! Page.IsPostBack )
                {
                    lblName.Text = name;

                    defaultValue = LoadFile( mode, "Default" );
                    string temp = LoadFile( mode, "Edit" );
                    if( temp != null )
                    {
                        editValue = temp;
                    }
                    else
                    {
                        editValue = defaultValue;
                    }

                    teContent.Text = editValue;
                    lblDefault.Text = Server.HtmlDecode( defaultValue );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// Returns to language editor control
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
                if( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId )
                {
                    Response.Redirect( EditUrl( "locale", locale, "Language", "resourcefile=" + Globals.QueryStringEncode( resfile ) ), true );
                }
                else
                {
                    Response.Redirect( Globals.NavigateURL( TabId, "", "locale=" + locale, "resourcefile=" + Globals.QueryStringEncode( resfile ), "mode=" + mode ), true );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// Saves the translation to the resource file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	07/10/2004	Created
        /// </history>
        protected void cmdUpdate_Click( Object sender, EventArgs e )
        {
            XmlNode node;
            XmlNode nodeData;
            XmlNode parent;
            XmlAttribute attr;
            XmlDocument resDoc = new XmlDocument();
            string filename;
            bool IsNewFile = false;

            try
            {
                if( teContent.Text == "" )
                {
                    UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "RequiredField.ErrorMessage", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.YellowWarning );
                    return;
                }

                filename = ResourceFile( locale, mode );
                if( ! File.Exists( filename ) )
                {
                    // load system default
                    resDoc.Load( ResourceFile( Localization.SystemLocale, "System" ) );
                    IsNewFile = true;
                }
                else
                {
                    resDoc.Load( filename );
                }

                switch( mode )
                {
                    case "System":

                        node = resDoc.SelectSingleNode( "//root/data[@name=\'" + name + "\']/value" );
                        if( node == null )
                        {
                            // missing entry
                            nodeData = resDoc.CreateElement( "data" );
                            attr = resDoc.CreateAttribute( "name" );
                            attr.Value = name;
                            nodeData.Attributes.Append( attr );
                            resDoc.SelectSingleNode( "//root" ).AppendChild( nodeData );

                            node = nodeData.AppendChild( resDoc.CreateElement( "value" ) );
                        }
                        node.InnerXml = teContent.Text;

                        resDoc.Save( filename );
                        break;
                    case "Host":
                        if( IsNewFile )
                        {
                            if( teContent.Text != lblDefault.Text )
                            {
                                foreach( XmlNode n in resDoc.SelectNodes( "//root/data" ) )
                                {
                                    parent = n.ParentNode;
                                    parent.RemoveChild( n );
                                }
                                nodeData = resDoc.CreateElement( "data" );
                                attr = resDoc.CreateAttribute( "name" );
                                attr.Value = name;
                                nodeData.Attributes.Append( attr );
                                resDoc.SelectSingleNode( "//root" ).AppendChild( nodeData );

                                node = nodeData.AppendChild( resDoc.CreateElement( "value" ) );
                                node.InnerXml = teContent.Text;

                                resDoc.Save( filename );
                            }
                        }
                        else
                        {
                            node = resDoc.SelectSingleNode( "//root/data[@name=\'" + name + "\']/value" );
                            if( teContent.Text != lblDefault.Text )
                            {
                                if( node == null )
                                {
                                    // missing entry
                                    nodeData = resDoc.CreateElement( "data" );
                                    attr = resDoc.CreateAttribute( "name" );
                                    attr.Value = name;
                                    nodeData.Attributes.Append( attr );
                                    resDoc.SelectSingleNode( "//root" ).AppendChild( nodeData );

                                    node = nodeData.AppendChild( resDoc.CreateElement( "value" ) );
                                }
                                node.InnerXml = teContent.Text;
                            }
                            else if( node != null )
                            {
                                // remove item = default
                                resDoc.SelectSingleNode( "//root" ).RemoveChild( node.ParentNode );
                            }
                            if( resDoc.SelectNodes( "//root/data" ).Count > 0 )
                            {
                                // there's something to save
                                resDoc.Save( filename );
                            }
                            else
                            {
                                // nothing to be saved, if file exists delete
                                if( File.Exists( filename ) )
                                {
                                    File.Delete( filename );
                                }
                            }
                        }
                        break;

                    case "Portal":

                        if( IsNewFile )
                        {
                            if( teContent.Text != lblDefault.Text )
                            {
                                foreach( XmlNode n in resDoc.SelectNodes( "//root/data" ) )
                                {
                                    parent = n.ParentNode;
                                    parent.RemoveChild( n );
                                }
                                nodeData = resDoc.CreateElement( "data" );
                                attr = resDoc.CreateAttribute( "name" );
                                attr.Value = name;
                                nodeData.Attributes.Append( attr );
                                resDoc.SelectSingleNode( "//root" ).AppendChild( nodeData );

                                node = nodeData.AppendChild( resDoc.CreateElement( "value" ) );
                                node.InnerXml = teContent.Text;

                                resDoc.Save( filename );
                            }
                        }
                        else
                        {
                            node = resDoc.SelectSingleNode( "//root/data[@name=\'" + name + "\']/value" );
                            if( teContent.Text != lblDefault.Text )
                            {
                                if( node == null )
                                {
                                    // missing entry
                                    nodeData = resDoc.CreateElement( "data" );
                                    attr = resDoc.CreateAttribute( "name" );
                                    attr.Value = name;
                                    nodeData.Attributes.Append( attr );
                                    resDoc.SelectSingleNode( "//root" ).AppendChild( nodeData );

                                    node = nodeData.AppendChild( resDoc.CreateElement( "value" ) );
                                }
                                node.InnerXml = teContent.Text;
                            }
                            else if( node != null )
                            {
                                // remove item = default
                                resDoc.SelectSingleNode( "//root" ).RemoveChild( node.ParentNode );
                            }
                            if( resDoc.SelectNodes( "//root/data" ).Count > 0 )
                            {
                                // there's something to save
                                resDoc.Save( filename );
                            }
                            else
                            {
                                // nothing to be saved, if file exists delete
                                if( File.Exists( filename ) )
                                {
                                    File.Delete( filename );
                                }
                            }
                        }
                        break;
                }

                if( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId )
                {
                    Response.Redirect( EditUrl( "locale", locale, "Language", "resourcefile=" + Globals.QueryStringEncode( resfile ) ), true );
                }
                else
                {
                    Response.Redirect( Globals.NavigateURL( TabId, "", "locale=" + locale, "resourcefile=" + Globals.QueryStringEncode( resfile ), "mode=" + mode ), true );
                }
            }
            catch( Exception ) //Module failed to load
            {
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "Save.ErrorMessage", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.YellowWarning );
            }
        }
    }
}