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
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.UI.Skins
{
    /// <Summary>
    /// Handles processing of a list of uploaded skin files into a working skin.
    /// </Summary>
    public class SkinFileProcessor
    {
        /// <summary>
        /// Parsing functionality for token replacement in new skin files.
        /// </summary>
        /// <remarks>
        /// This class encapsulates the data and methods necessary to appropriately
        /// handle all the token parsing needs for new skin files (which is appropriate
        /// only for HTML files).  The parser accomodates some ill formatting of tokens
        /// (ignoring whitespace and casing) and allows for naming of token instances
        /// if more than one instance of a particular control is desired on a skin.  The
        /// proper syntax for an instance is: "[TOKEN_INSTANCE]" where the instance can
        /// be any alphanumeric string.  Generated control ID's all take the
        /// form of "TOKENINSTANCE".
        /// </remarks>
        private class ControlParser
        {
            private XmlDocument m_Attributes;
            private readonly Hashtable m_ControlList;
            private readonly string m_InitMessages;
            private string m_ParseMessages;
            private ArrayList m_RegisterList;

            private XmlDocument Attributes
            {
                get
                {
                    return this.m_Attributes;
                }
                set
                {
                    this.m_Attributes = value;
                }
            }

            private Hashtable ControlList
            {
                get
                {
                    return this.m_ControlList;
                }
            }

            private MatchEvaluator Handler
            {
                get
                {
                    return new MatchEvaluator( this.TokenMatchHandler );
                }
            }

            private string Messages
            {
                get
                {
                    return this.m_ParseMessages;
                }
                set
                {
                    this.m_ParseMessages = value;
                }
            }

            private ArrayList RegisterList
            {
                get
                {
                    return this.m_RegisterList;
                }
                set
                {
                    this.m_RegisterList = value;
                }
            }

            /// <summary>
            /// Registration directives generated as a result of the Parse method.
            /// </summary>
            /// <returns>ArrayList of formatted registration directives.</returns>
            /// <remarks>
            /// In addition to the updated file contents, the Parse method also
            /// creates this list of formatted registration directives which can
            /// be processed later.  They are not performed in place during the
            /// Parse method in order to preserve the formatting of the input file
            /// in case additional parsing might not anticipate the formatting of
            /// those directives.  Since they are properly formatted, it is better
            /// to exclude them from being subject to parsing.
            /// </remarks>
            public ArrayList Registrations
            {
                get
                {
                    return this.m_RegisterList;
                }
            }

            /// <summary>
            /// ControlParser class constructor.
            /// </summary>
            /// <remarks>
            /// The constructor processes accepts a hashtable of skin objects to process against.
            /// </remarks>
            public ControlParser( Hashtable ControlList )
            {
                this.m_ControlList = new Hashtable();
                this.m_InitMessages = "";
                this.m_RegisterList = new ArrayList();
                this.m_Attributes = new XmlDocument();
                this.m_ParseMessages = "";
                this.m_ControlList = ( (Hashtable)ControlList.Clone() );
            }

            /// <summary>
            /// Perform parsing on the specified source file using the specified attributes.
            /// </summary>
            /// <param name="Source">Pointer to Source string to be parsed.</param>
            /// <param name="Attributes">XML document containing token attribute information (can be empty).</param>
            /// <remarks>
            /// This procedure invokes a handler for each match of a formatted token.
            /// The attributes are first set because they will be referenced by the
            /// match handler.
            /// </remarks>
            public string Parse( ref string Source, XmlDocument Attributes )
            {
                this.Messages = m_InitMessages;

                // set the token attributes
                this.Attributes = Attributes;
                // clear register list
                this.RegisterList.Clear();

                // define the regular expression to match tokens
                Regex FindTokenInstance = new Regex( "\\[\\s*(?<token>\\w*)\\s*:?\\s*(?<instance>\\w*)\\s*]", RegexOptions.IgnoreCase );

                // parse the file
                Source = FindTokenInstance.Replace( Source, this.Handler );

                return Messages;
            }

            /// <summary>
            /// Process regular expression matches.
            /// </summary>
            /// <param name="m">Regular expression match for token which requires processing.</param>
            /// <returns>Properly formatted token.</returns>
            /// <remarks>
            /// The handler is invoked by the Regex.Replace method once for each match that
            /// it encounters.  The returned value of the handler is substituted for the
            /// original match.  So the handler properly formats the replacement for the
            /// token and returns it instead.  If an unknown token is encountered, the token
            /// is unmodified.  This can happen if a token is used for a skin object which
            /// has not yet been installed.
            /// </remarks>
            private string TokenMatchHandler( Match m )
            {
                string TOKEN_PROC = Localization.GetString( "ProcessToken", Globals.GetPortalSettings() );
                string TOKEN_SKIN = Localization.GetString( "SkinToken", Globals.GetPortalSettings() );
                string TOKEN_PANE = Localization.GetString( "PaneToken", Globals.GetPortalSettings() );
                string TOKEN_FOUND = Localization.GetString( "TokenFound", Globals.GetPortalSettings() );
                string TOKEN_FORMAT = Localization.GetString( "TokenFormat", Globals.GetPortalSettings() );
                string TOKEN_NOTFOUND_INFILE = Localization.GetString( "TokenNotFoundInFile", Globals.GetPortalSettings() );
                string CONTROL_FORMAT = Localization.GetString( "ControlFormat", Globals.GetPortalSettings() );
                string TOKEN_NOTFOUND = Localization.GetString( "TokenNotFound", Globals.GetPortalSettings() );

                string Token = m.Groups["token"].Value.ToUpper();
                string ControlName = Token + m.Groups["instance"].Value;

                // if the token has an instance name, use it to look for the corresponding attributes
                string AttributeNode = Token + Convert.ToString( ( m.Groups["instance"].Value == "" ) ? "" : ( ":" + m.Groups["instance"].Value ) );

                this.Messages += SkinController.FormatMessage( TOKEN_PROC, "[" + AttributeNode + "]", 2, false );

                // if the token is a recognized skin control
                if( this.ControlList.ContainsKey( Token ) == true || Token.IndexOf( "CONTENTPANE" ) != -1 )
                {
                    string SkinControl = "";

                    if( this.ControlList.ContainsKey( Token ) )
                    {
                        this.Messages += SkinController.FormatMessage( TOKEN_SKIN, ( (string)this.ControlList[Token] ), 2, false );
                    }
                    else
                    {
                        this.Messages += SkinController.FormatMessage( TOKEN_PANE, Token, 2, false );
                    }

                    // if there is an attribute file
                    if( this.Attributes.DocumentElement != null )
                    {
                        // look for the the node of this instance of the token
                        XmlNode xmlSkinAttributeRoot = this.Attributes.DocumentElement.SelectSingleNode( "descendant::Object[Token='[" + AttributeNode + "]']" );
                        // if the token is found
                        if( xmlSkinAttributeRoot != null )
                        {
                            this.Messages += SkinController.FormatMessage( TOKEN_FOUND, "[" + AttributeNode + "]", 2, false );
                            // process each token attribute
                            XmlNode xmlSkinAttribute;
                            foreach( XmlNode tempLoopVar_xmlSkinAttribute in xmlSkinAttributeRoot.SelectNodes( ".//Settings/Setting" ) )
                            {
                                xmlSkinAttribute = tempLoopVar_xmlSkinAttribute;
                                if( xmlSkinAttribute.SelectSingleNode( "Value" ).InnerText != "" )
                                {
                                    // append the formatted attribute to the inner contents of the control statement
                                    this.Messages += SkinController.FormatMessage( TOKEN_FORMAT, xmlSkinAttribute.SelectSingleNode( "Name" ).InnerText + "=\"" + xmlSkinAttribute.SelectSingleNode( "Value" ).InnerText + "\"", 2, false );
                                    SkinControl += " " + xmlSkinAttribute.SelectSingleNode( "Name" ).InnerText + "=\"" + xmlSkinAttribute.SelectSingleNode( "Value" ).InnerText.Replace( "\"", "&quot;" ) + "\"";
                                }
                            }
                        }
                        else
                        {
                            this.Messages += SkinController.FormatMessage( TOKEN_NOTFOUND_INFILE, "[" + AttributeNode + "]", 2, false );
                        }
                    }

                    if( this.ControlList.ContainsKey( Token ) )
                    {
                        // create the skin object user control tag
                        SkinControl = "dnn:" + ControlName + " runat=\"server\" id=\"dnn" + ControlName + "\"" + SkinControl;

                        // Save control registration statement
                        RegisterList.Add( "<%@ Register TagPrefix=\"dnn\" TagName=\"" + ControlName + "\" Src=\"~/" + ( (string)this.ControlList[Token] ) + "\" %>" + "\r\n" );

                        // return the control statement
                        this.Messages += SkinController.FormatMessage( CONTROL_FORMAT, "&lt;" + SkinControl + " /&gt;", 2, false );

                        SkinControl = "<" + SkinControl + " />";
                    }
                    else // CONTENTPANE
                    {
                        if( SkinControl.ToLower().IndexOf( "id=" ) == -1 )
                        {
                            SkinControl = " id=\"ContentPane\"";
                        }
                        SkinControl = "div runat=\"server\"" + SkinControl + "></div";

                        // return the control statement
                        this.Messages += SkinController.FormatMessage( CONTROL_FORMAT, "&lt;" + SkinControl + "&gt;", 2, false );

                        SkinControl = "<" + SkinControl + ">";
                    }

                    return SkinControl;
                }
                else
                {
                    // return the unmodified token
                    // note that this is currently protecting array syntax in embedded javascript
                    // should be fixed in the regular expressions but is not, currently.
                    this.Messages += SkinController.FormatMessage( TOKEN_NOTFOUND, "[" + m.Groups["token"].Value + "]", 2, false );
                    return "[" + m.Groups["token"].Value + "]";
                }
            }
        }

        /// <summary>
        /// Parsing functionality for path replacement in new skin files.
        /// </summary>
        /// <remarks>
        /// This class encapsulates the data and methods necessary to appropriately
        /// handle all the path replacement parsing needs for new skin files. Parsing
        /// supported for CSS syntax and HTML syntax (which covers ASCX files also).
        /// </remarks>
        private class PathParser
        {
            private ArrayList m_CSSPatterns;
            private ArrayList m_HTMLPatterns;
            private string m_Messages;
            private SkinParser m_ParseOption;
            private string m_SkinPath;
            private string SUBST;
            private string SUBST_DETAIL;

            /// <summary>
            /// List of regular expressions for processing CSS syntax.
            /// </summary>
            /// <returns>ArrayList of Regex objects formatted for the Parser method.</returns>
            /// <remarks>
            /// Additional patterns can be added to this list (if necessary) if properly
            /// formatted to return <tag/>, <content/> and <endtag/> groups.  For future
            /// consideration, this list could be imported from a configuration file to
            /// provide for greater flexibility.
            /// </remarks>
            public ArrayList CSSList
            {
                get
                {
                    // if the arraylist in uninitialized
                    if( m_CSSPatterns.Count == 0 )
                    {
                        // retrieve the patterns
                        string[] arrPattern = new string[] {"(?<tag>\\surl\\u0028)(?<content>[^\\u0029]*)(?<endtag>\\u0029.*;)"};

                        // for each pattern, create a regex object
                        int i;
                        for( i = 0; i <= arrPattern.GetLength( 0 ) - 1; i++ )
                        {
                            Regex re = new Regex( arrPattern[i], RegexOptions.Multiline | RegexOptions.IgnoreCase );
                            // add the Regex object to the pattern array list
                            m_CSSPatterns.Add( re );
                        }
                        // optimize the arraylist size since it will not change
                        m_CSSPatterns.TrimToSize();
                    }

                    return m_CSSPatterns;
                }
            }

            private MatchEvaluator Handler
            {
                get
                {
                    return new MatchEvaluator( this.MatchHandler );
                }
            }

            /// <summary>
            /// List of regular expressions for processing HTML syntax.
            /// </summary>
            /// <returns>ArrayList of Regex objects formatted for the Parser method.</returns>
            /// <remarks>
            /// Additional patterns can be added to this list (if necessary) if properly
            /// formatted to return <tag/>, <content/> and <endtag/> groups.  For future
            /// consideration, this list could be imported from a configuration file to
            /// provide for greater flexibility.
            /// </remarks>
            public ArrayList HTMLList
            {
                get
                {
                    // if the arraylist in uninitialized
                    if( m_HTMLPatterns.Count == 0 )
                    {
                        // retrieve the patterns
                        string[] arrPattern = new string[] {"(?<tag><head[^>]*?\\sprofile\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)", "(?<tag><object[^>]*?\\s(?:codebase|data|usemap)\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)", "(?<tag><img[^>]*?\\s(?:src|longdesc|usemap)\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)", "(?<tag><input[^>]*?\\s(?:src|usemap)\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)", "(?<tag><iframe[^>]*?\\s(?:src|longdesc)\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)", "(?<tag><(?:td|th|table|body)[^>]*?\\sbackground\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)", "(?<tag><(?:script|bgsound|embed|xml|frame)[^>]*?\\ssrc\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)", "(?<tag><(?:base|link|a|area)[^>]*?\\shref\\s*=\\s*\")(?!https://|http://|\\\\|[~/]|javascript:|mailto:)(?<content>[^\"]*)(?<endtag>\"[^>]*>)", "(?<tag><(?:blockquote|ins|del|q)[^>]*?\\scite\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)", "(?<tag><(?:param\\s+name\\s*=\\s*\"(?:movie|src|base)\")[^>]*?\\svalue\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)", "(?<tag><embed[^>]*?\\s(?:src)\\s*=\\s*\")(?!https://|http://|\\\\|[~/])(?<content>[^\"]*)(?<endtag>\"[^>]*>)"};

                        // for each pattern, create a regex object
                        int i;
                        for( i = 0; i <= arrPattern.GetLength( 0 ) - 1; i++ )
                        {
                            Regex re = new Regex( arrPattern[i], RegexOptions.Multiline | RegexOptions.IgnoreCase );
                            // add the Regex object to the pattern array list
                            m_HTMLPatterns.Add( re );
                        }
                        // optimize the arraylist size since it will not change
                        m_HTMLPatterns.TrimToSize();
                    }

                    return m_HTMLPatterns;
                }
            }

            private SkinParser ParseOption
            {
                get
                {
                    return this.m_ParseOption;
                }
                set
                {
                    this.m_ParseOption = value;
                }
            }

            private string SkinPath
            {
                get
                {
                    return this.m_SkinPath;
                }
                set
                {
                    this.m_SkinPath = value;
                }
            }

            public PathParser()
            {
                this.m_HTMLPatterns = new ArrayList();
                this.m_CSSPatterns = new ArrayList();
                this.m_SkinPath = "";
                this.m_Messages = "";
                this.SUBST = Localization.GetString( "Substituting", Globals.GetPortalSettings() );
                this.SUBST_DETAIL = Localization.GetString( "Substituting.Detail", Globals.GetPortalSettings() );
            }

            /// <summary>
            /// Process regular expression matches.
            /// </summary>
            /// <param name="m">Regular expression match for path information which requires processing.</param>
            /// <returns>Properly formatted path information.</returns>
            /// <remarks>
            /// The handler is invoked by the Regex.Replace method once for each match that
            /// it encounters.  The returned value of the handler is substituted for the
            /// original match.  So the handler properly formats the path information and
            /// returns it in favor of the improperly formatted match.
            /// </remarks>
            private string MatchHandler( Match m )
            {
                string strOldTag = m.Groups["tag"].Value + m.Groups["content"].Value + m.Groups["endtag"].Value;
                string strNewTag = strOldTag;

                switch( this.ParseOption )
                {
                    case SkinParser.Localized:

                        // if the tag does not contain the localized path
                        if( strNewTag.IndexOf( this.SkinPath ) == -1 )
                        {
                            // insert the localized path
                            strNewTag = m.Groups["tag"].Value + this.SkinPath + m.Groups["content"].Value + m.Groups["endtag"].Value;
                        }
                        break;
                    case SkinParser.Portable:

                        // if the tag does not contain a reference to the skinpath
                        if( strNewTag.ToLower().IndexOf( "<%= skinpath %>" ) == -1 )
                        {
                            // insert the skinpath
                            strNewTag = m.Groups["tag"].Value + "<%= SkinPath %>" + m.Groups["content"].Value + m.Groups["endtag"].Value;
                        }
                        // if the tag contains the localized path
                        if( strNewTag.IndexOf( this.SkinPath ) != -1 )
                        {
                            // remove the localized path
                            strNewTag = strNewTag.Replace( this.SkinPath, "" );
                        }
                        break;
                }

                m_Messages += SkinController.FormatMessage( SUBST, string.Format( SUBST_DETAIL, HttpUtility.HtmlEncode( strOldTag ), HttpUtility.HtmlEncode( strNewTag ) ), 2, false );
                return strNewTag;
            }

            /// <summary>
            /// Perform parsing on the specified source file.
            /// </summary>
            /// <param name="Source">Pointer to Source string to be parsed.</param>
            /// <param name="RegexList">ArrayList of properly formatted regular expression objects.</param>
            /// <param name="SkinPath">Path to use in replacement operation.</param>
            /// <remarks>
            /// This procedure iterates through the list of regular expression objects
            /// and invokes a handler for each match which uses the specified path.
            /// </remarks>
            public string Parse( ref string Source, ref ArrayList RegexList, string SkinPath, SkinParser ParseOption )
            {
                m_Messages = "";

                // set path propery which is file specific
                this.SkinPath = SkinPath;
                // set parse option
                this.ParseOption = ParseOption;

                // process each regular expression
                int i;
                for( i = 0; i <= RegexList.Count - 1; i++ )
                {
                    Source = ( (Regex)RegexList[i] ).Replace( Source, this.Handler );
                }

                return m_Messages;
            }
        }

        /// <summary>
        /// Utility class for processing of skin files.
        /// </summary>
        private class SkinFile
        {
            private string CONTROL_DIR = Localization.GetString( "ControlDirective", Globals.GetPortalSettings() );
            private string CONTROL_REG = Localization.GetString( "ControlRegister", Globals.GetPortalSettings() );
            private string FILE_FORMAT_DETAIL = Localization.GetString( "FileFormat.Detail", Globals.GetPortalSettings() );

            //Localized Strings
            private string FILE_FORMAT_ERROR = Localization.GetString( "FileFormat.Error", Globals.GetPortalSettings() );
            private string FILE_LOAD = Localization.GetString( "SkinFileLoad", Globals.GetPortalSettings() );
            private string FILE_LOAD_ERROR = Localization.GetString( "SkinFileLoad.Error", Globals.GetPortalSettings() );
            private string FILE_WRITE = Localization.GetString( "FileWrite", Globals.GetPortalSettings() );
            private readonly XmlDocument m_FileAttributes;
            private string m_FileContents;
            private readonly string m_FileExtension;
            private readonly string m_FileName;
            private string m_Messages = "";
            private readonly string m_SkinRoot;
            private readonly string m_SkinRootPath;
            private readonly string m_WriteFileName;

            public XmlDocument Attributes
            {
                get
                {
                    return this.m_FileAttributes;
                }
            }

            public string Contents
            {
                get
                {
                    return this.m_FileContents;
                }
                set
                {
                    this.m_FileContents = value;
                }
            }

            public string FileExtension
            {
                get
                {
                    return this.m_FileExtension;
                }
            }

            public string FileName
            {
                get
                {
                    return this.m_FileName;
                }
            }

            public string Messages
            {
                get
                {
                    return this.m_Messages;
                }
            }

            public string SkinRoot
            {
                get
                {
                    return this.m_SkinRoot;
                }
            }

            public string SkinRootPath
            {
                get
                {
                    return this.m_SkinRootPath;
                }
            }

            public string WriteFileName
            {
                get
                {
                    return this.m_WriteFileName;
                }
            }

            public SkinFile( string SkinContents, XmlDocument SkinAttributes )
            {
                // set attributes
                m_FileAttributes = SkinAttributes;

                // set file contents
                this.Contents = SkinContents;
            }

            /// <summary>
            /// SkinFile class constructor.
            /// </summary>
            /// <param name="SkinRoot"></param>
            /// <param name="FileName"></param>
            /// <param name="SkinAttributes"></param>
            /// <remarks>
            /// The constructor primes the utility class with basic file information.
            /// It also checks for the existentce of a skinfile level attribute file
            /// and read it in, if found.
            /// </remarks>
            public SkinFile( string SkinRoot, string FileName, XmlDocument SkinAttributes )
            {
                // capture file information
                m_FileName = FileName;
                m_FileExtension = Path.GetExtension( FileName );
                m_SkinRoot = SkinRoot;
                m_FileAttributes = SkinAttributes;

                // determine and store path to portals skin root folder
                string strTemp = FileName.Replace( Path.GetFileName( FileName ), "" );
                strTemp = strTemp.Replace( "\\", "/" );
                m_SkinRootPath = Globals.ApplicationPath + strTemp.Substring( Strings.InStr( 1, strTemp.ToUpper(), "/PORTALS", 0 ) - 1 );

                // read file contents
                this.Contents = Read( FileName );

                // setup some attributes based on file extension
                switch( this.FileExtension )
                {
                    case ".htm":
                        // set output file name to <filename>.ASCX
                        m_WriteFileName = FileName.Replace( Path.GetExtension( FileName ), ".ascx" );
                        // capture warning if file does not contain a id="ContentPane" or [CONTENTPANE]
                        Regex PaneCheck1 = new Regex( "\\s*id\\s*=\\s*\"" + Globals.glbDefaultPane + "\"", RegexOptions.IgnoreCase );
                        Regex PaneCheck2 = new Regex( "\\s*[" + Globals.glbDefaultPane + "]", RegexOptions.IgnoreCase );
                        if( PaneCheck1.IsMatch( this.Contents ) == false && PaneCheck2.IsMatch( this.Contents ) == false )
                        {
                            m_Messages += SkinController.FormatMessage( FILE_FORMAT_ERROR, string.Format( FILE_FORMAT_ERROR, FileName ), 2, true );
                        }

                        // Check for existence of and load skin file level attribute information
                        if( File.Exists( FileName.Replace( this.FileExtension, ".xml" ) ) )
                        {
                            try
                            {
                                m_FileAttributes.Load( FileName.Replace( this.FileExtension, ".xml" ) );
                                m_Messages += SkinController.FormatMessage( FILE_LOAD, FileName, 2, false );
                            }
                            catch( Exception ) // could not load XML file
                            {
                                m_FileAttributes = SkinAttributes;
                                m_Messages += SkinController.FormatMessage( FILE_LOAD_ERROR, FileName, 2, true );
                            }
                        }
                        break;

                    case ".html":

                        // set output file name to <filename>.ASCX
                        m_WriteFileName = FileName.Replace( Path.GetExtension( FileName ), ".ascx" );
                        // capture warning if file does not contain a id="ContentPane" or [CONTENTPANE]
                        PaneCheck1 = new Regex( "\\s*id\\s*=\\s*\"" + Globals.glbDefaultPane + "\"", RegexOptions.IgnoreCase );
                        PaneCheck2 = new Regex( "\\s*[" + Globals.glbDefaultPane + "]", RegexOptions.IgnoreCase );
                        if( PaneCheck1.IsMatch( this.Contents ) == false && PaneCheck2.IsMatch( this.Contents ) == false )
                        {
                            m_Messages += SkinController.FormatMessage( FILE_FORMAT_ERROR, string.Format( FILE_FORMAT_ERROR, FileName ), 2, true );
                        }

                        // Check for existence of and load skin file level attribute information
                        if( File.Exists( FileName.Replace( this.FileExtension, ".xml" ) ) )
                        {
                            try
                            {
                                m_FileAttributes.Load( FileName.Replace( this.FileExtension, ".xml" ) );
                                m_Messages += SkinController.FormatMessage( FILE_LOAD, FileName, 2, false );
                            }
                            catch( Exception ) // could not load XML file
                            {
                                m_FileAttributes = SkinAttributes;
                                m_Messages += SkinController.FormatMessage( FILE_LOAD_ERROR, FileName, 2, true );
                            }
                        }
                        break;

                    default:

                        // output file name is same as input file name
                        m_WriteFileName = FileName;
                        break;
                }
            }

            /// <summary>
            /// Prepend ascx control directives to file contents.
            /// </summary>
            /// <param name="Registrations">ArrayList of registration directives.</param>
            /// <remarks>
            /// This procedure formats the @Control directive and prepends it and all
            /// registration directives to the file contents.
            /// </remarks>
            public string PrependASCXDirectives(ArrayList Registrations)
            {
                string Messages = "";
                string Prefix = "";

                // if the skin source is an HTML document, extract the content within the <body> tags
                string strPattern = "<\\s*body[^>]*>(?<skin>.*)<\\s*/\\s*body\\s*>";
                Match objMatch = Regex.Match(this.Contents, strPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (objMatch.Groups[1].Value != "")
                {
                    this.Contents = objMatch.Groups[1].Value;
                }

                // format and save @Control directive
                if (this.SkinRoot == SkinInfo.RootSkin)
                {
                        //Prefix += "<%@ Control language=\"vb\" CodeBehind=\"~/admin/" + SkinRoot + "/skin.vb\" AutoEventWireup=\"false\" Explicit=\"True\" Inherits=\"DotNetNuke.UI.Skins.Skin\" %>" + Environment.NewLine;
                        Prefix += "<%@ Control language=\"c#\" CodeBehind=\"~/Admin/" + SkinRoot + "/Skin.cs\" AutoEventWireup=\"true\" Inherits=\"DotNetNuke.UI.Skins.Skin\" %>" + Environment.NewLine;
                }
                else if (this.SkinRoot == SkinInfo.RootContainer)
                {
                        //Prefix += "<%@ Control language=\"vb\" CodeBehind=\"~/admin/" + SkinRoot + "/container.vb\" AutoEventWireup=\"false\" Explicit=\"True\" Inherits=\"DotNetNuke.UI.Containers.Container\" %>" + Environment.NewLine;
                        Prefix += "<%@ Control language=\"c#\" CodeBehind=\"~/Admin/" + SkinRoot + "/Container.cs\" AutoEventWireup=\"true\" Inherits=\"DotNetNuke.UI.Containers.Container\" %>" + Environment.NewLine;
                }

                Messages += SkinController.FormatMessage(CONTROL_DIR, HttpUtility.HtmlEncode(Prefix), 2, false);

                // add preformatted Control Registrations
                foreach (string Item in Registrations)
                {
                    Messages += SkinController.FormatMessage(CONTROL_REG, HttpUtility.HtmlEncode(Item), 2, false);
                    Prefix += Item;
                }

                // update file contents to include ascx header information
                this.Contents = Prefix + this.Contents;

                return Messages;

            }

        

            private string Read( string FileName )
            {
                StreamReader objStreamReader = new StreamReader( FileName );
                string strFileContents = objStreamReader.ReadToEnd();
                objStreamReader.Close();

                return strFileContents;
            }

            public void Write()
            {
                // delete the file before attempting to write
                if( File.Exists( this.WriteFileName ) )
                {
                    File.Delete( this.WriteFileName );
                }

                m_Messages += SkinController.FormatMessage( FILE_WRITE, Path.GetFileName( this.WriteFileName ), 2, false );
                StreamWriter objStreamWriter = new StreamWriter( this.WriteFileName );
                objStreamWriter.WriteLine( this.Contents );
                objStreamWriter.Flush();
                objStreamWriter.Close();
            }
        }

        private string DUPLICATE_DETAIL = Localization.GetString( "DuplicateSkinObject.Detail", Globals.GetPortalSettings() );
        private string DUPLICATE_ERROR = Localization.GetString( "DuplicateSkinObject.Error", Globals.GetPortalSettings() );
        private string FILE_BEGIN = Localization.GetString( "BeginSkinFile", Globals.GetPortalSettings() );
        private string FILE_END = Localization.GetString( "EndSkinFile", Globals.GetPortalSettings() );
        private string FILES_END = Localization.GetString( "EndSkinFiles", Globals.GetPortalSettings() );

        //Localized Strings
        private string INITIALIZE_PROCESSOR = Localization.GetString( "StartProcessor", Globals.GetPortalSettings() );
        private string LOAD_SKIN_TOKEN = Localization.GetString( "LoadingSkinToken", Globals.GetPortalSettings() );
        private readonly ControlParser m_ControlFactory;
        private readonly Hashtable m_ControlList = new Hashtable();
        private string m_Message = "";
        private readonly PathParser m_PathFactory = new PathParser();
        private readonly XmlDocument m_SkinAttributes = new XmlDocument();
        private readonly string m_SkinName;
        private readonly string m_SkinPath;
        private readonly string m_SkinRoot;
        private string PACKAGE_LOAD = Localization.GetString( "PackageLoad", Globals.GetPortalSettings() );
        private string PACKAGE_LOAD_ERROR = Localization.GetString( "PackageLoad.Error", Globals.GetPortalSettings() );

        private ControlParser ControlFactory
        {
            get
            {
                return this.m_ControlFactory;
            }
        }

        private string Message
        {
            get
            {
                return this.m_Message;
            }
            set
            {
                this.m_Message = value;
            }
        }

        private PathParser PathFactory
        {
            get
            {
                return this.m_PathFactory;
            }
        }

        private XmlDocument SkinAttributes
        {
            get
            {
                return this.m_SkinAttributes;
            }
        }

        public string SkinName
        {
            get
            {
                return this.m_SkinName;
            }
        }

        public string SkinPath
        {
            get
            {
                return this.m_SkinPath;
            }
        }

        public string SkinRoot
        {
            get
            {
                return this.m_SkinRoot;
            }
        }

        /// <summary>
        /// SkinFileProcessor class constructor.
        /// </summary>
        /// <param name="SkinPath">File path to the portals upload directory.</param>
        /// <param name="SkinRoot">Specifies type of skin (Skins or Containers)</param>
        /// <param name="SkinName">Name of folder in which skin will reside (Zip file name)</param>
        /// <remarks>
        /// The constructor primes the file processor with path information and
        /// control data that should only be retrieved once.  It checks for the
        /// existentce of a skin level attribute file and read it in, if found.
        /// It also sorts through the complete list of controls and creates
        /// a hashtable which contains only the skin objects and their source paths.
        /// These are recognized by their ControlKey's which are formatted like
        /// tokens ("[TOKEN]").  The hashtable is required for speed as it will be
        /// processed for each token found in the source file by the Control Parser.
        /// </remarks>
        public SkinFileProcessor( string SkinPath, string SkinRoot, string SkinName )
        {
            this.Message += SkinController.FormatMessage( INITIALIZE_PROCESSOR, SkinRoot + " :: " + SkinName, 0, false );

            // Save path information for future use
            m_SkinRoot = SkinRoot;
            m_SkinPath = SkinPath;
            m_SkinName = SkinName;

            // Check for and read skin package level attribute information file
            string FileName = this.SkinPath + this.SkinRoot + "\\" + this.SkinName + "\\" + SkinRoot.Substring( 0, SkinRoot.Length - 1 ) + ".xml";
            if( File.Exists( FileName ) )
            {
                try
                {
                    this.SkinAttributes.Load( FileName );
                    this.Message += SkinController.FormatMessage( PACKAGE_LOAD, Path.GetFileName( FileName ), 2, false );
                }
                catch( Exception ex )
                {
                    // could not load XML file
                    this.Message += SkinController.FormatMessage( string.Format( PACKAGE_LOAD_ERROR, ex.Message ), Path.GetFileName( FileName ), 2, true );
                }
            }

            // Retrieve a list of available controls
            ModuleControlController objModuleControls = new ModuleControlController();
            ArrayList arrModuleControls = objModuleControls.GetModuleControls( Null.NullInteger );

            // Look at every control
            string Token;
            int i;
            ModuleControlInfo objModuleControl;
            for( i = 0; i <= arrModuleControls.Count - 1; i++ )
            {
                objModuleControl = (ModuleControlInfo)arrModuleControls[i];
                // If the control is a skin object, save the key and source in the hash table
                if( objModuleControl.ControlType == SecurityAccessLevel.SkinObject )
                {
                    Token = objModuleControl.ControlKey.ToUpper();

                    // If the control is already in the hash table
                    if( m_ControlList.ContainsKey( Token ) )
                    {
                        // Record an error message and skip it
                        this.Message += SkinController.FormatMessage( string.Format( DUPLICATE_ERROR, objModuleControl.ControlKey.ToString().ToUpper() ), string.Format( DUPLICATE_DETAIL, ( (string)m_ControlList[Token] ), objModuleControl.ControlSrc.ToString() ), 2, true );
                    }
                    else
                    {
                        // Add it
                        this.Message += SkinController.FormatMessage( string.Format( LOAD_SKIN_TOKEN, objModuleControl.ControlKey.ToString().ToUpper() ), objModuleControl.ControlSrc.ToString(), 2, false );
                        m_ControlList.Add( Token, objModuleControl.ControlSrc );
                    }
                }
            }

            // Instantiate the control parser with the list of skin objects
            m_ControlFactory = new ControlParser( m_ControlList );
        }

        /// <summary>
        /// SkinFileProcessor class constructor.
        /// </summary>
        /// <remarks>
        /// This constructor parses a memory based skin
        /// </remarks>
        public SkinFileProcessor( string ControlKey, string ControlSrc )
        {
            m_ControlList.Add( ControlKey, ControlSrc );

            // Instantiate the control parser with the list of skin objects
            m_ControlFactory = new ControlParser( m_ControlList );
        }

        /// <Summary>Perform processing on list of files to generate skin.</Summary>
        /// <Param name="FileList">ArrayList of files to be processed.</Param>
        /// <Returns>HTML formatted string of informational messages.</Returns>
        public string ProcessList( ArrayList FileList )
        {
            return this.ProcessList( FileList, SkinParser.Localized );
        }

        public string ProcessList( ArrayList FileList, SkinParser ParseOption )
        {
            string FileName;

            // process each file in the list
            foreach( string tempLoopVar_FileName in FileList )
            {
                FileName = tempLoopVar_FileName;

                this.Message += SkinController.FormatMessage( FILE_BEGIN, Path.GetFileName( FileName ), 0, false );

                // create a skin file object to aid in processing
                //TODO: Uncomment this:
                SkinFile objSkinFile = new SkinFile( this.SkinRoot, FileName, this.SkinAttributes );

                // choose processing based on type of file
                if( objSkinFile.FileExtension == ".htm" )
                {
                    string skinFileContents = objSkinFile.Contents;
                    ArrayList pathList = this.PathFactory.HTMLList;
                    // replace paths, process control tokens and convert html to ascx format
                    this.Message += this.PathFactory.Parse( ref skinFileContents, ref pathList, objSkinFile.SkinRootPath, ParseOption );
                    this.Message += this.ControlFactory.Parse( ref skinFileContents, objSkinFile.Attributes );
                    this.Message += objSkinFile.PrependASCXDirectives( this.ControlFactory.Registrations );
                }
                else if( objSkinFile.FileExtension == ".html" )
                {
                    string skinFileContents = objSkinFile.Contents;
                    ArrayList pathList = this.PathFactory.HTMLList;
                    // replace paths, process control tokens and convert html to ascx format
                    this.Message += this.PathFactory.Parse( ref skinFileContents, ref pathList, objSkinFile.SkinRootPath, ParseOption );
                    this.Message += this.ControlFactory.Parse( ref skinFileContents, objSkinFile.Attributes );
                    this.Message += objSkinFile.PrependASCXDirectives( this.ControlFactory.Registrations );
                }

                objSkinFile.Write();
                this.Message += objSkinFile.Messages;

                this.Message += SkinController.FormatMessage( FILE_END, Path.GetFileName( FileName ), 1, false );
            }

            this.Message += SkinController.FormatMessage( FILES_END, this.SkinRoot + " :: " + this.SkinName, 0, false );

            return this.Message;
        }

        public string ProcessSkin( string SkinSource, XmlDocument SkinAttributes, SkinParser ParseOption )
        {
            // create a skin file object to aid in processing
            SkinFile objSkinFile = new SkinFile( SkinSource, SkinAttributes );

            string skinFileContents = objSkinFile.Contents;
            // process control tokens and convert html to ascx format
            this.Message += this.ControlFactory.Parse( ref skinFileContents, objSkinFile.Attributes );
            this.Message += objSkinFile.PrependASCXDirectives( this.ControlFactory.Registrations );

            return objSkinFile.Contents;
        }
    }
}