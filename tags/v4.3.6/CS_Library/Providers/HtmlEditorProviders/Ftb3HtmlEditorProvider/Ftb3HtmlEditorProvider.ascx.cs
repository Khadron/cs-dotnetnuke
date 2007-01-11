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
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Modules.HTMLEditorProvider;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using FreeTextBoxControls;
using FreeTextBoxControls.Support;
using BulletedList=FreeTextBoxControls.BulletedList;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.HtmlEditor
{
    /// Class:  TextEditor
    /// Project: Provider.FtbHtmlEditorProvider
    /// <summary>
    /// Ftb3HtmlEditorProvider implements an Html Editor Provider for FTB (FreeTextBox) 3
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	12/13/2004	Documented
    /// </history>
    public class Ftb3HtmlEditorProvider : HtmlEditorProvider
    {
        private FreeTextBox cntlFtb = new FreeTextBox();

        private const string ProviderType = "htmlEditor";

        private ArrayList _AdditionalToolbars = new ArrayList();
        private string _ControlID;
        private bool enableProFeatures;
        private ProviderConfiguration providerConfiguration = ProviderConfiguration.GetProviderConfiguration( ProviderType );
        private string providerPath;
        private string _RootImageDirectory;
        private string spellCheck;
        private SortedList styles;
        private string toolbarStyle;

        /// <summary>
        /// Creates the Provider
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/30/2005	documented
        /// </history>
        public Ftb3HtmlEditorProvider()
        {
            // Read the configuration specific information for this provider
            Provider objProvider = (Provider)providerConfiguration.Providers[providerConfiguration.DefaultProvider];

            providerPath = objProvider.Attributes["providerPath"];
            spellCheck = objProvider.Attributes["spellCheck"];
            enableProFeatures = bool.Parse( objProvider.Attributes["enableProFeatures"] );
            toolbarStyle = objProvider.Attributes["toolbarStyle"];
        }

        public override ArrayList AdditionalToolbars
        {
            get
            {
                return _AdditionalToolbars;
            }
            set
            {
                _AdditionalToolbars = value;
            }
        }

        public override string ControlID
        {
            get
            {
                return _ControlID;
            }
            set
            {
                _ControlID = value;
            }
        }

        public override Unit Height
        {
            get
            {
                return cntlFtb.Height;
            }
            set
            {
                cntlFtb.Height = value;
            }
        }

        public override Control HtmlEditorControl
        {
            get
            {
                return cntlFtb;
            }
        }

        public override string RootImageDirectory
        {
            get
            {
                if( String.IsNullOrEmpty( _RootImageDirectory ) )
                {
                    PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

                    //Remove the Application Path from the Home Directory
                    if( !String.IsNullOrEmpty(Globals.ApplicationPath) )
                    {
                        //Remove the Application Path from the Home Directory                        
                        return _portalSettings.HomeDirectory.Substring( Common.Globals.ApplicationPath.Length );
                    }
                    else
                    {
                        return _portalSettings.HomeDirectory;
                    }
                }
                else
                {
                    return _RootImageDirectory;
                }
            }
            set
            {
                _RootImageDirectory = value;
            }
        }

        public override string Text
        {
            get
            {
                return cntlFtb.Text;
            }
            set
            {
                cntlFtb.Text = value;
            }
        }

        public override Unit Width
        {
            get
            {
                return cntlFtb.Width;
            }
            set
            {
                cntlFtb.Width = value;
            }
        }

        /// <summary>
        /// Returns the provider path
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/30/2005	documented
        /// </history>
        public string ProviderPath
        {
            get
            {
                return providerPath;
            }
        }

        /// <summary>
        /// Creates the Color ToolBar
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>A FreeTextBox ToolBar</returns>
        /// <history>
        /// 	[cnurse]	12/13/2004	Documented
        /// </history>
        private Toolbar AddColorToolBar()
        {
            Toolbar tb = new Toolbar();
            tb.Items.Add( new FontForeColorsMenu() );
            tb.Items.Add( new FontForeColorPicker() );
            tb.Items.Add( new FontBackColorsMenu() );
            tb.Items.Add( new FontBackColorPicker() );

            return tb;
        }

        /// <summary>
        /// Creates the Edit ToolBar
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>A FreeTextBox ToolBar</returns>
        /// <history>
        /// 	[cnurse]	12/13/2004	Documented
        /// </history>
        private Toolbar AddEditToolBar()
        {
            Toolbar tb = new Toolbar();
            tb.Items.Add( new Cut() );
            tb.Items.Add( new Copy() );
            tb.Items.Add( new Paste() );
            tb.Items.Add( new Delete() );
            tb.Items.Add( new ToolbarSeparator() );
            tb.Items.Add( new Undo() );
            tb.Items.Add( new Redo() );

            return tb;
        }

        /// <summary>
        /// Creates the Form ToolBar in FreeTextBox
        /// (Pro edition only)
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>A FreeTextBox ToolBar</returns>
        /// <history>
        /// 	[JWhite]	2/27/2005	Added/Documented
        /// </history>
        private Toolbar AddFormToolBar()
        {
            Toolbar tb = new Toolbar();
            tb.Items.Add( new InsertForm() ); //pro only
            tb.Items.Add( new InsertTextBox() ); //pro only
            tb.Items.Add( new InsertTextArea() ); //pro only
            tb.Items.Add( new InsertRadioButton() ); //pro only
            tb.Items.Add( new InsertCheckBox() ); //pro only
            tb.Items.Add( new InsertDropDownList() ); //pro only
            tb.Items.Add( new InsertButton() ); //pro only

            return tb;
        }

        /// <summary>
        /// Creates the Format ToolBar
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>A FreeTextBox ToolBar</returns>
        /// <history>
        /// 	[cnurse]	12/13/2004	Documented
        /// </history>
        private Toolbar AddFormatToolBar()
        {
            Toolbar tb = new Toolbar();
            tb.Items.Add( new JustifyLeft() );
            tb.Items.Add( new JustifyCenter() );
            tb.Items.Add( new JustifyRight() );
            tb.Items.Add( new JustifyFull() );
            tb.Items.Add( new ToolbarSeparator() );
            tb.Items.Add( new BulletedList() );
            tb.Items.Add( new NumberedList() );
            tb.Items.Add( new Indent() );
            tb.Items.Add( new Outdent() );

            return tb;
        }

        /// <summary>
        /// Creates the Insert ToolBar
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>A FreeTextBox ToolBar</returns>
        /// <history>
        /// 	[cnurse]	12/13/2004	Documented
        /// </history>
        private Toolbar AddInsertToolBar()
        {
            Toolbar tb = new Toolbar();
            tb.Items.Add( new SymbolsMenu() );
            tb.Items.Add( new InsertRule() );
            tb.Items.Add( new InsertDate() );
            tb.Items.Add( new InsertTime() );
            tb.Items.Add( new ToolbarSeparator() );
            tb.Items.Add( new CreateLink() );
            tb.Items.Add( new Unlink() );
            tb.Items.Add( InsertSmiley() );
            tb.Items.Add( new InsertImageFromGallery() );

            return tb;
        }

        /// <summary>
        /// Creates the Insert ToolBar
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>A FreeTextBox ToolBar</returns>
        /// <history>
        /// 	[cnurse]	12/13/2004	Documented
        /// 	[JWhite]	2/25/2005	Added SpellCheck
        /// </history>
        private Toolbar AddSpecialToolBar()
        {
            Toolbar tb = new Toolbar();
            tb.Items.Add( new Preview() );
            tb.Items.Add( new SelectAll() );
            switch( spellCheck )
            {
                case "NetSpell":

                    tb.Items.Add( new NetSpell() ); //requires NetSpell library
                    break;
                case "IeSpellCheck":

                    tb.Items.Add( new IeSpellCheck() );
                    break;
            }
            if( enableProFeatures )
            {
                tb.Items.Add( new WordClean() ); //pro only
            }
            else
            {
                tb.Items.Add( WordCleanDnn() ); // dnn version for non-pro users
            }

            return tb;
        }

        /// <summary>
        /// Creates the Styles Menu
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>A FreeTextBox StylesMenu</returns>
        /// <history>
        /// 	[cnurse]	12/13/2004	Documented
        /// </history>
        private StylesMenu AddStylesMenu()
        {
            StylesMenu styleMenu = new StylesMenu();            

            styles = new SortedList();

            //Parse default css
            ParseStyleSheet( Globals.HostPath + "default.css" );

            //Parse skin stylesheet(s)
            ParseStyleSheet( PortalSettings.ActiveTab.SkinPath + "skin.css" );
            ParseStyleSheet( PortalSettings.ActiveTab.SkinSrc.Replace( ".ascx", ".css") );

            //Parse portal stylesheet
            ParseStyleSheet( PortalSettings.HomeDirectory + "portal.css" );

            for( int i = 0; i <= styles.Count - 1; i++ )
            {
                ToolbarListItem myListItem = new ToolbarListItem();
                myListItem.Text = Convert.ToString( styles.GetByIndex( i ) );
                myListItem.Value = Convert.ToString( styles.GetKey( i ) );
                styleMenu.Items.Add( myListItem );
            }

            return styleMenu;
        }

        /// <summary>
        /// Creates the Style ToolBar
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>A FreeTextBox ToolBar</returns>
        /// <history>
        /// 	[cnurse]	12/13/2004	Documented
        /// </history>
        private Toolbar AddStyleToolBar()
        {
            Toolbar tb = new Toolbar();
            tb.Items.Add( AddStylesMenu() );
            tb.Items.Add( new ParagraphMenu() );
            tb.Items.Add( new FontFacesMenu() );
            tb.Items.Add( new FontSizesMenu() );

            return tb;
        }

        /// <summary>
        /// Creates the Table ToolBar in FreeTextBox
        /// (Pro edition only)
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>A FreeTextBox ToolBar</returns>
        /// <history>
        /// 	[JWhite]	2/25/2005	Added/Documented
        /// </history>
        private Toolbar AddTableToolBar()
        {
            Toolbar tb = new Toolbar();
            tb.Items.Add( new InsertTable() );
            tb.Items.Add( new EditTable() );
            if( enableProFeatures )
            {
                tb.Items.Add( new InsertTableColumnAfter() ); //pro only
                tb.Items.Add( new InsertTableColumnBefore() ); //pro only
                tb.Items.Add( new InsertTableRowBefore() ); //pro only
                tb.Items.Add( new InsertTableRowAfter() ); //pro only
                tb.Items.Add( new DeleteTableColumn() ); //pro only
                tb.Items.Add( new DeleteTableRow() ); //pro only
            }
            return tb;
        }

        /// <summary>
        /// Creates the Text ToolBar
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>A FreeTextBox ToolBar</returns>
        /// <history>
        /// 	[cnurse]	12/13/2004	Documented
        /// </history>
        private Toolbar AddTextToolBar()
        {
            Toolbar tb = new Toolbar();
            tb.Items.Add( new Bold() );
            tb.Items.Add( new Italic() );
            tb.Items.Add( new Underline() );
            tb.Items.Add( new StrikeThrough() );
            tb.Items.Add( new SuperScript() );
            tb.Items.Add( new SubScript() );
            tb.Items.Add( new RemoveFormat() );
            return tb;
        }

        /// <summary>
        /// Provides an insert Smiley Popup
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	01/27/2006  created
        /// </history>
        private ToolbarButton InsertSmiley()
        {
            ToolbarButton dnnInsertSmiley = new ToolbarButton();
            dnnInsertSmiley.Title = Localization.GetString( "InsertSmileyButton" );
            dnnInsertSmiley.ButtonImage = "insertsmiley";
            dnnInsertSmiley.ScriptBlock = "this.ftb.SelectSmiley();";

            return dnnInsertSmiley;
        }

        /// <summary>
        /// Parses the Stylesheet looking for styles
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="strCssFile">Stylesheet to parse</param>
        /// <history>
        /// 	[cnurse]	12/13/2004	Documented
        /// </history>
        private void ParseStyleSheet( string strCssFile )
        {
            if( File.Exists( HttpContext.Current.Server.MapPath( strCssFile ) ) )
            {
                //Use the StyleSheetParser provided by the FreeTextBox control to obtain the styles
                // this returns all styles including styles with a :hover, :link etc
                if( !String.IsNullOrEmpty(Globals.ApplicationPath) )
                {
                    strCssFile = strCssFile.Replace( Globals.ApplicationPath, "" );
                }
                string[] styleDefinitions = StyleSheetParser.ParseStyleSheet( strCssFile );

                foreach( string styleDefinition in styleDefinitions )
                {
                    //First split single lines that contain multiple styles (separated by ",")
                    string[] styleDefs = styleDefinition.Split( ',' );

                    for( int i = 0; i < styleDefs.Length; i++ )
                    {
                        string style = styleDefs[i];
                        
                        //First strip any white space
                        style = HtmlUtils.StripWhiteSpace( style, false );

                        //Next strip any :active, :hover, :link, :visited modifiers
                        style = style.Replace( ":active", "" ).Replace( ":hover", "" ).Replace( ":link", "" ).Replace( ":visited", "" );

                        //Next strip any leading "."
                        if( style.StartsWith( "." ) )
                        {
                            style = style.Substring( 1 );
                        }

                        //Only add styles that can apply to any content (ie do not add A.CommandButton etc)
                        if( style.IndexOf( "." ) == - 1 )
                        {
                            if( ! this.styles.ContainsKey( style.ToLower() ) )
                            {
                                this.styles.Add( style.ToLower(), style );
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the Design Mode Css Property, to enable WYSIWYG behaviour for the editor
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/29/2005	Created
        /// </history>
        private void SetDesignModeCss()
        {
            //Note the javascript expects there to be one and only once Css StyleSheet
            // eg "<link rel='stylesheet' href='" + this.designModeCss + "' type='text/css' />"

            //We typically have more than one stylesheet
            //  default.css
            //  skin.css
            //  portal.css

            string DesignModeCss = Globals.HostPath + "default.css";

            //Add skin css
            DesignModeCss += "' type='text/css' /><link rel='stylesheet' href='" + PortalSettings.ActiveTab.SkinPath + "skin.css";
            DesignModeCss += "' type='text/css' /><link rel='stylesheet' href='" + PortalSettings.ActiveTab.SkinSrc.Replace(".ascx", ".css" );

            //Add portal css
            DesignModeCss += "' type='text/css' /><link rel='stylesheet' href='" + PortalSettings.HomeDirectory + "portal.css";

            cntlFtb.DesignModeCss = DesignModeCss;
        }

        /// <summary>
        /// Provides an alternative Word Clean script if Pro Features aren't available
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/29/2005	Moved to a separate Method
        /// </history>
        private ToolbarButton WordCleanDnn()
        {
            ToolbarButton dnnWordClean = new ToolbarButton();
            dnnWordClean.Title = Localization.GetString( "WordCleanButton" );
            dnnWordClean.ButtonImage = "wordclean";
            dnnWordClean.ScriptBlock = "this.ftb.WordClean();";

            return dnnWordClean;
        }

        /// <summary>
        /// Adds additional toolbar(s) to the FTB control
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	01/12/2005	created
        /// </history>
        public override void AddToolbar()
        {
            foreach( Toolbar tb in AdditionalToolbars )
            {
                cntlFtb.Toolbars.Add( tb );
            }
        }

        /// <summary>
        /// Initialises the control
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	12/13/2004	Documented
        /// </history>
        public override void Initialize()
        {
            //initialize the control
            cntlFtb = new FreeTextBox();
            cntlFtb.PreRender += new EventHandler( cntlFtb_PreRender );
            cntlFtb.Language = Thread.CurrentThread.CurrentUICulture.Name;
            cntlFtb.ButtonImagesLocation = ResourceLocation.ExternalFile;
            cntlFtb.JavaScriptLocation = ResourceLocation.ExternalFile;

            //Set Design Mode Css for WYSIWYG
// TODO AC When the style sheets are added, we get js errors resulting in nothing working, add this back at a later time.
//            SetDesignModeCss();

            cntlFtb.ToolbarImagesLocation = ResourceLocation.ExternalFile;
            cntlFtb.ID = ControlID;
            cntlFtb.AutoGenerateToolbarsFromString = false;

            //Build the Standard ToolBar Collection
            cntlFtb.Toolbars.Clear();
            cntlFtb.Toolbars.Add( AddStyleToolBar() );
            cntlFtb.Toolbars.Add( AddColorToolBar() );

            Toolbar tb = AddTextToolBar();
            if( tb.Items.Count > 0 )
            {
                cntlFtb.Toolbars.Add( AddTextToolBar() );
            }

            cntlFtb.Toolbars.Add( AddFormatToolBar() );
            cntlFtb.Toolbars.Add( AddEditToolBar() );
            cntlFtb.Toolbars.Add( AddInsertToolBar() );
            cntlFtb.Toolbars.Add( AddTableToolBar() );
            if( enableProFeatures )
            {
                cntlFtb.Toolbars.Add( AddFormToolBar() );
            }
            cntlFtb.Toolbars.Add( AddSpecialToolBar() );

            cntlFtb.ImageGalleryUrl = Globals.ResolveUrl( "~/Providers/HtmlEditorProviders/Ftb3HtmlEditorProvider/ftb3/ftb.imagegallery.aspx?cif=~" + RootImageDirectory + "&rif=~" + RootImageDirectory + "&portalid=" + PortalSettings.PortalId );
            cntlFtb.SupportFolder = Globals.ResolveUrl( "~/Providers/HtmlEditorProviders/Ftb3HtmlEditorProvider/ftb3/" );
            switch( toolbarStyle )
            {
                case "OfficeMac":

                    cntlFtb.ToolbarStyleConfiguration = ToolbarStyleConfiguration.OfficeMac;
                    break;
                case "Office2000":

                    cntlFtb.ToolbarStyleConfiguration = ToolbarStyleConfiguration.Office2000;
                    break;
                case "OfficeXP":

                    cntlFtb.ToolbarStyleConfiguration = ToolbarStyleConfiguration.OfficeXP;
                    break;
                case "Office2003":

                    cntlFtb.ToolbarStyleConfiguration = ToolbarStyleConfiguration.Office2003;
                    break;
                default:

                    cntlFtb.ToolbarStyleConfiguration = ToolbarStyleConfiguration.NotSet;
                    break;
            }
        }

        /// <summary>
        /// cntlFtb_Load runs when the FTB control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	12/02/2005	Created
        /// </history>
        protected void cntlFtb_PreRender( object sender, EventArgs e )
        {
            string jsFileName = "FTB-DotNetNuke.js";
            string jsPath = cntlFtb.SupportFolder;

            //Register the FTB-DotNetNuke.js script file that implements the DNN enhancements
            if( ! ClientAPI.IsClientScriptBlockRegistered( cntlFtb.Page, jsFileName ) )
            {
                ClientAPI.RegisterClientScriptBlock( cntlFtb.Page, jsFileName, "<script type='text/javascript' src='" + jsPath + jsFileName + "'></script>" );
            }
        }
    }
}