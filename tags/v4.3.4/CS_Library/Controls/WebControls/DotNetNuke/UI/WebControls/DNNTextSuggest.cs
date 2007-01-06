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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.UI.Design.WebControls;
using DotNetNuke.UI.Utilities;
using Microsoft.VisualBasic;

namespace DotNetNuke.UI.WebControls
{
    [Designer( typeof( DNNTextSuggestDesigner ) ), ToolboxData( "<{0}:DNNTextSuggest runat=server></{0}:DNNTextSuggest>" )]
    public class DNNTextSuggest : TextBox, IClientAPICallbackEventHandler
    {
        public enum eIDTokenChar
        {
            None,
            Brackets,
            Paranthesis
        }

        public delegate void DNNTextSuggestEventHandler( object source, DNNTextSuggestEventArgs e );

        public delegate void DNNDNNNodeClickHandler( object source, DNNTextSuggestEventArgs e );

        private DNNDNNNodeClickHandler NodeClickEvent;

        public event DNNDNNNodeClickHandler NodeClick
        {
            add
            {
                NodeClickEvent = (DNNDNNNodeClickHandler)Delegate.Combine( NodeClickEvent, value );
            }
            remove
            {
                NodeClickEvent = (DNNDNNNodeClickHandler)Delegate.Remove( NodeClickEvent, value );
            }
        }

        private DNNTextSuggestEventHandler PopulateOnDemandEvent;

        public event DNNTextSuggestEventHandler PopulateOnDemand
        {
            add
            {
                PopulateOnDemandEvent = (DNNTextSuggestEventHandler)Delegate.Combine( PopulateOnDemandEvent, value );
            }
            remove
            {
                PopulateOnDemandEvent = (DNNTextSuggestEventHandler)Delegate.Remove( PopulateOnDemandEvent, value );
            }
        }

        private DNNNodeCollection m_objNodes;

        /// <summary>
        /// Allows developer to force the rendering of the Menu in DownLevel mode
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/22/2005	Commented
        /// </history>
        [Category( "Behavior" ), Description( "Allows developer to force the rendering of the Menu in DownLevel mode" ), DefaultValue( false )]
        public bool ForceDownLevel
        {
            get
            {
                return Convert.ToBoolean( ViewState["ForceDownLevel"] );
            }
            set
            {
                ViewState["ForceDownLevel"] = value;
            }
        }

        /// <summary>
        /// Returns whether the TextSuggest will render DownLevel or not
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/22/2005	Commented
        ///		[Jon Henning]	3/9/2005	Requiring XML support on client for uplevel
        /// </history>
        [Browsable( false )]
        public bool IsDownLevel
        {
            get
            {
                if( ForceDownLevel || ClientAPI.BrowserSupportsFunctionality( ClientAPI.ClientFunctionality.DHTML ) == false || ClientAPI.BrowserSupportsFunctionality( ClientAPI.ClientFunctionality.XML ) == false )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Location of ClientAPI js files
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/22/2005	Created
        /// </history>
        [Category( "Paths" ), Description( "Location of ClientAPI js files" ), DefaultValue( "" )]
        public string ClientAPIScriptPath
        {
            get
            {
                return ClientAPI.ScriptPath;
            }
            set
            {
                ClientAPI.ScriptPath = value;
            }
        }

        /// <summary>
        /// Location of dnn.controls.DNNTextSuggest.js file
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// Since 1.1 this path will be the same as the ClientAPI path.
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/22/2005	Created
        /// </history>
        [Category( "Paths" ), Description( "Location of dnn.controls.DNNTextSuggest.js file" ), DefaultValue( "" )]
        public string TextSuggestScriptPath
        {
            get
            {
                if( Strings.Len( ViewState["TextSuggestScriptPath"] ) == 0 )
                {
                    return ClientAPIScriptPath;
                }
                else
                {
                    return ViewState["TextSuggestScriptPath"].ToString();
                }
            }
            set
            {
                ViewState["TextSuggestScriptPath"] = value;
            }
        }

        /// <summary>
        /// Location of system images (i.e. spacer.gif)
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/22/2005	Created
        /// </history>
        [Category( "Paths" ), Description( "Directory to find the images for the TextSuggest." ), DefaultValue( "images/" )]
        public string SystemImagesPath
        {
            get
            {
                if( Strings.Len( ViewState["SystemImagesPath"] ) == 0 )
                {
                    return "images/";
                }
                else
                {
                    return ViewState["SystemImagesPath"].ToString();
                }
            }
            set
            {
                ViewState["SystemImagesPath"] = value;
            }
        }

        /// <summary>
        /// This property really makes no sense for this control and should not be here,
        /// but due to concerns for backwards compatibility I am keeping.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        public DNNNodeCollection DNNNodes
        {
            get
            {
                if( m_objNodes == null )
                {
                    m_objNodes = new DNNNodeCollection( this.ClientID );
                }
                return m_objNodes;
            }
        }

        /// <summary>
        /// Holds collection of selected node objects
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	4/6/2005	Commented
        /// </history>
        [Browsable( false )]
        public DNNNodeCollection SelectedNodes
        {
            get
            {
                DNNNodeCollection objNodes = new DNNNodeCollection( "" );
                DNNNode objNode;
                if( Page.IsPostBack )
                {
                    string[] aryEntries = this.Text.Split( Convert.ToChar( this.Delimiter ) );
                    string strText;
                    string strKey;
                    foreach( string strEntry in aryEntries )
                    {
                        //wish I knew regular expressions better...
                        if( strEntry.Length > 0 )
                        {
                            strText = strEntry;
                            strKey = "";
                            switch( this.IDToken )
                            {
                                case eIDTokenChar.None:

                                    break;
                                case eIDTokenChar.Brackets:

                                    //text [key]
                                    int intTextEnd = strEntry.LastIndexOf( " [" );
                                    int intKeyBegin = intTextEnd + " [".Length;
                                    int intKeyEnd = strEntry.LastIndexOf( "]" );
                                    if( intTextEnd > -1 && intKeyEnd > intKeyBegin )
                                    {
                                        strText = strEntry.Substring( 0, intTextEnd );
                                        strKey = strEntry.Substring( intKeyBegin, intKeyEnd - intKeyBegin );
                                    }
                                    break;
                                case eIDTokenChar.Paranthesis:

                                    break;
                            }
                            objNode = new DNNNode( strText );
                            objNode.Key = strKey;
                            objNodes.Add( objNode );
                        }
                    }
                }
                return objNodes;
            }
        }

        /// <summary>
        /// If ClickAction for a node is set to navigate this is the target frame that
        ///	will do the navigating.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Category( "Behavior" ), Description( "If ClickAction for a node is set to navigate this is the target frame that will do the navigating." ), DefaultValue( "" )]
        public string Target
        {
            get
            {
                return ( Convert.ToString( ViewState["Target"] ) );
            }
            set
            {
                ViewState["Target"] = value;
            }
        }

        /// <summary>
        /// Default Classname for node.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Category( "Appearance" ), Description( "Default Classname for node." ), DefaultValue( "" )]
        public string DefaultNodeCssClass
        {
            get
            {
                return this.CssClass;
            }
            set
            {
                this.CssClass = value;
            }
        }

        /// <summary>
        /// Default Classname for child node.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Category( "Appearance" ), Description( "Default Classname for child node." ), DefaultValue( "" )]
        public string DefaultChildNodeCssClass
        {
            get
            {
                return ( Convert.ToString( ViewState["DefaultChildNodeCssClass"] ) );
            }
            set
            {
                ViewState["DefaultChildNodeCssClass"] = value;
            }
        }

        /// <summary>
        /// Default Classname for node when hovered.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Category( "Appearance" ), Description( "Default Classname for node when hovered." ), DefaultValue( "" )]
        public string DefaultNodeCssClassOver
        {
            get
            {
                return ( Convert.ToString( ViewState["DefaultNodeCssClassOver"] ) );
            }
            set
            {
                ViewState["DefaultNodeCssClassOver"] = value;
            }
        }

        /// <summary>
        /// Default Classname for node when selected.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Category( "Appearance" ), Description( "Default Classname for node when selected." ), DefaultValue( "" )]
        public string DefaultNodeCssClassSelected
        {
            get
            {
                return ( Convert.ToString( ViewState["DefaultNodeCssClassSelected"] ) );
            }
            set
            {
                ViewState["DefaultNodeCssClassSelected"] = value;
            }
        }

        /// <summary>
        /// Default Classname container holding all of suggestion nodes.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Category( "Appearance" ), Description( "Default Classname container holding all of suggestion nodes." ), DefaultValue( "" )]
        public string TextSuggestCssClass
        {
            get
            {
                return ( Convert.ToString( ViewState["TextSuggestCssClass"] ) );
            }
            set
            {
                ViewState["TextSuggestCssClass"] = value;
            }
        }

        /// <summary>
        /// Allows you to have a common JS function be invoked for all nodes, unless
        /// a different JS function is provided on the node level.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// If the client-side function returns false, the action associated to the node
        /// selection will be canceled
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	4/6/2005	Commented
        /// </history>
        [Category( "Behavior" ), Description( "Allows you to have a common JS function be invoked for all nodes, unless a different JS function is provided on the node level." ), DefaultValue( "" )]
        public string JSFunction
        {
            get
            {
                return ( Convert.ToString( ViewState["JSFunction"] ) );
            }
            set
            {
                ViewState["JSFunction"] = value;
            }
        }

        /// <summary>
        /// If callbacks are supported/enabled, this javascript function will be invoked
        /// with each status change of the xmlhttp request.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// The Javascript transport does not raise any events.
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	4/6/2005	Created
        /// </history>
        [Category( "Behavior" ), Description( "If callbacks are supported/enabled, this javascript function will be invoked with each status change of the xmlhttp request." ), DefaultValue( "" )]
        public string CallbackStatusFunction
        {
            get
            {
                return ( Convert.ToString( ViewState["CallbackStatusFunction"] ) );
            }
            set
            {
                ViewState["CallbackStatusFunction"] = value;
            }
        }

        /// <summary>
        /// Specifies a delimiter to be used to allow for multiple entries to be added.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// This should be a safe character that is not going to be used as an entry itself.
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/24/2006	Created
        /// </history>
        [Category( "Behavior" ), Description( "Specifies a delimiter to be used to allow for multiple entries to be added." ), DefaultValue( "" )]
        public char Delimiter
        {
            get
            {
                return ( Convert.ToChar( ViewState["Delimiter"] ) );
            }
            set
            {
                ViewState["Delimiter"] = value;
            }
        }

        /// <summary>
        /// Specifies a type of character to be used to surround/delimit the underlying
        ///	value/id of the selected item
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// An example of the text generated with this set to Brackets would be Smith [123]
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/24/2006	Created
        /// </history>
        [Category( "Behavior" ), Description( "Specifies a type of character to be used to surround/delimit the underlying value/id of the selected item." ), DefaultValue( eIDTokenChar.None )]
        public eIDTokenChar IDToken
        {
            get
            {
                return ( (eIDTokenChar)ViewState["IDToken"] );
            }
            set
            {
                ViewState["IDToken"] = Convert.ToInt32( value );
            }
        }

        /// <summary>
        /// Number of characters typed before a lookup is performed
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/24/2006	Created
        /// </history>
        [Category( "Behavior" ), Description( "Minimum number of characters typed before a lookup will be invoked" ), DefaultValue( 1 )]
        public int MinCharacterLookup
        {
            get
            {
                if( Strings.Len( ViewState["MinCharacterLookup"] ) == 0 )
                {
                    return 1;
                }
                else
                {
                    return ( Convert.ToInt32( ViewState["MinCharacterLookup"] ) );
                }
            }
            set
            {
                ViewState["MinCharacterLookup"] = value;
            }
        }

        /// <summary>
        /// Maximum number of rows to display.
        /// </summary>
        /// <value></value>
        /// <remarks>
        ///	This is important since it will allow the client side code to determine when
        ///	a new lookup is needed.  As a developer you need to return MaxSuggestRows + 1
        ///	results down to the client in order for it to determine when a lookup is required.
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/24/2006	Created
        /// </history>
        [Category( "Behavior" ), Description( "Maximum number of rows to display." ), DefaultValue( 10 )]
        public int MaxSuggestRows
        {
            get
            {
                if( Strings.Len( ViewState["MaxSuggestRows"] ) == 0 )
                {
                    return 10;
                }
                else
                {
                    return ( Convert.ToInt32( ViewState["MaxSuggestRows"] ) );
                }
            }
            set
            {
                ViewState["MaxSuggestRows"] = value;
            }
        }

        /// <summary>
        /// Number of milliseconds to wait after keypress before a lookup takes place
        /// </summary>
        /// <value></value>
        /// <remarks>
        ///	Defaults to half a second (500)
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/24/2006	Created
        /// </history>
        [Category( "Behavior" ), Description( "Number of milliseconds to wait after keypress before a lookup takes place." ), DefaultValue( 500 )]
        public int LookupDelay
        {
            get
            {
                if( Strings.Len( ViewState["LookupDelay"] ) == 0 )
                {
                    return 500;
                }
                else
                {
                    return ( Convert.ToInt32( ViewState["LookupDelay"] ) );
                }
            }
            set
            {
                ViewState["LookupDelay"] = value;
            }
        }

        /// <summary>
        /// Determines if lookup uses a case sensitve match
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/24/2006	Created
        /// </history>
        [Category( "Behavior" ), Description( "Determines if lookup uses a case sensitve match." ), DefaultValue( false )]
        public bool CaseSensitive
        {
            get
            {
                if( Strings.Len( ViewState["CaseSensitive"] ) == 0 )
                {
                    return false;
                }
                else
                {
                    return ( Convert.ToBoolean( ViewState["CaseSensitive"] ) );
                }
            }
            set
            {
                ViewState["CaseSensitive"] = value;
            }
        }

        protected override void AddAttributesToRender( HtmlTextWriter writer )
        {
            base.AddAttributesToRender( writer );
            //writer.AddAttribute(HtmlTextWriterAttribute.Class, Me.CssClass)

            writer.AddAttribute( "sysimgpath", this.SystemImagesPath );
            if( this.Target.Length > 0 )
            {
                writer.AddAttribute( "target", this.Target );
            }

            //css attributes
            if( this.TextSuggestCssClass.Length > 0 )
            {
                writer.AddAttribute( "tscss", this.TextSuggestCssClass );
            }
            if( this.DefaultNodeCssClass.Length > 0 )
            {
                writer.AddAttribute( "css", this.DefaultNodeCssClass );
            }
            if( this.DefaultChildNodeCssClass.Length > 0 )
            {
                writer.AddAttribute( "csschild", this.DefaultChildNodeCssClass );
            }
            if( this.DefaultNodeCssClassOver.Length > 0 )
            {
                writer.AddAttribute( "csshover", this.DefaultNodeCssClassOver );
            }
            if( this.DefaultNodeCssClassSelected.Length > 0 )
            {
                writer.AddAttribute( "csssel", this.DefaultNodeCssClassSelected );
            }

            if( this.JSFunction.Length > 0 )
            {
                writer.AddAttribute( "js", this.JSFunction );
            }
            if( Strings.Len( this.Delimiter ) > 0 )
            {
                writer.AddAttribute( "del", this.Delimiter.ToString() );
            }
            switch( this.IDToken )
            {
                case eIDTokenChar.None:

                    break;
                case eIDTokenChar.Brackets:

                    writer.AddAttribute( "idtok", "[~]" );
                    break;
                case eIDTokenChar.Paranthesis:

                    writer.AddAttribute( "idtok", "(~)" );
                    break;
            }

            if( this.MinCharacterLookup > 1 )
            {
                writer.AddAttribute( "minchar", this.MinCharacterLookup.ToString() );
            }
            if( this.MaxSuggestRows != 10 )
            {
                writer.AddAttribute( "maxrows", this.MaxSuggestRows.ToString() );
            }
            if( this.LookupDelay != 500 )
            {
                writer.AddAttribute( "ludelay", this.LookupDelay.ToString() );
            }
            if( this.CaseSensitive )
            {
                writer.AddAttribute( "casesens", "1" );
            }

            writer.AddAttribute( "postback", ClientAPI.GetPostBackEventReference( this, "[TEXT]" + ClientAPI.COLUMN_DELIMITER + "Click" ) );

            if( ClientAPI.BrowserSupportsFunctionality( ClientAPI.ClientFunctionality.XMLHTTP ) )
            {
                writer.AddAttribute( "callback", ClientAPI.GetCallbackEventReference( this, "this.getText()", "this.callBackSuccess", "this", "this.callBackFail", "this.callBackStatus" ) );
            }

            if( this.CallbackStatusFunction.Length > 0 )
            {
                writer.AddAttribute( "callbackSF", this.CallbackStatusFunction );
            }

            if( this.JSFunction.Length > 0 )
            {
                writer.AddAttribute( "js", this.JSFunction );
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="eventArgument"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        ///		[Jon Henning]	2/21/2006	Fixed arg to not pass Click text
        /// </history>
        public virtual void RaisePostBackEvent( string eventArgument )
        {
            string[] args = eventArgument.Split( ClientAPI.COLUMN_DELIMITER.ToCharArray()[0] );

            if( args.Length > 1 )
            {
                switch( args[1] )
                {
                    case "Click":

                        DNNTextSuggestEventArgs oArg = new DNNTextSuggestEventArgs( this.DNNNodes, args[0] );
                        OnNodeClick( oArg );
                        break;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	10/6/2004	Created
        /// </history>
        public virtual void OnNodeClick( DNNTextSuggestEventArgs e )
        {
            if( NodeClickEvent != null )
            {
                NodeClickEvent( this, e );
            }
        }

        private void DNNTextSuggest_PreRender( object sender, EventArgs e )
        {
            RegisterClientScript();
            Page.RegisterRequiresPostBack( this );
        }

        public string RaiseClientAPICallbackEvent( string eventArgument )
        {
            if( PopulateOnDemandEvent != null )
            {
                PopulateOnDemandEvent( this, new DNNTextSuggestEventArgs( this.DNNNodes, eventArgument ) );
            }
            return this.DNNNodes.ToXml();
        }

        /// <summary>
        /// Determines if client supports an uplevel rendering, and if so it registers
        /// the appropriate scripts.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	4/6/2005	Commented
        /// </history>
        public void RegisterClientScript()
        {
            if( IsDownLevel == false )
            {
                ClientAPI.RegisterClientReference( this.Page, ClientAPI.ClientNamespaceReferences.dnn_dom );
                ClientAPI.RegisterClientReference( this.Page, ClientAPI.ClientNamespaceReferences.dnn_xml );
                ClientAPI.RegisterClientReference( this.Page, ClientAPI.ClientNamespaceReferences.dnn_dom_positioning );
                ClientAPI.RegisterClientReference( this.Page, ClientAPI.ClientNamespaceReferences.dnn_xmlhttp );

                if( !ClientAPI.IsClientScriptBlockRegistered( this.Page, "dnn.controls.DNNTextSuggest.js" ) )
                {
                    ClientAPI.RegisterClientScriptBlock( this.Page, "dnn.controls.dnnTextSuggest.js", "<script src=\"" + TextSuggestScriptPath + "dnn.controls.dnnTextSuggest.js\"></script>" );
                }
                ClientAPI.RegisterStartUpScript( Page, this.ClientID + "_startup", "<script>dnn.controls.initTextSuggest($('" + this.ClientID + "'));</script>" );
            }
        }

        /// <summary>
        /// Finds Node by passed in Key.
        /// </summary>
        /// <param name="strID">ID of node</param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	11/17/2004	Created
        /// </history>
        public DNNNode FindNode( string strID )
        {
            return this.DNNNodes.FindNode( strID );
        }

        /// <summary>
        /// Finds Node by passed in Key.
        /// </summary>
        /// <param name="strKey">Key of node</param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	11/17/2004	Created
        /// </history>
        public DNNNode FindNodeByKey( string strKey )
        {
            return this.DNNNodes.FindNodeByKey( strKey );
        }

        public void LoadXml( string strXml )
        {
            m_objNodes = new DNNNodeCollection( strXml, "" );
        }
    }
}