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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.UI.Utilities;
using Microsoft.VisualBasic;

namespace DotNetNuke.UI.WebControls
{
    [Designer( typeof( DNNMenuDesigner ) )]
    public class DNNMenu : WebControl, IClientAPICallbackEventHandler
    {
        public delegate void DNNMenuEventHandler( object source, DNNMenuEventArgs e );

        public delegate void DNNMenuNodeClickHandler( object source, DNNMenuNodeClickEventArgs e );

        private DNNMenuNodeClickHandler NodeClickEvent;

        public event DNNMenuNodeClickHandler NodeClick
        {
            add
            {
                NodeClickEvent = (DNNMenuNodeClickHandler)Delegate.Combine( NodeClickEvent, value );
            }
            remove
            {
                NodeClickEvent = (DNNMenuNodeClickHandler)Delegate.Remove( NodeClickEvent, value );
            }
        }

        private DNNMenuEventHandler PopulateOnDemandEvent;

        public event DNNMenuEventHandler PopulateOnDemand
        {
            add
            {
                PopulateOnDemandEvent = (DNNMenuEventHandler)Delegate.Combine( PopulateOnDemandEvent, value );
            }
            remove
            {
                PopulateOnDemandEvent = (DNNMenuEventHandler)Delegate.Remove( PopulateOnDemandEvent, value );
            }
        }

        private MenuNodeCollection m_objNodes;
        private NodeImageCollection m_objImages;

        /// <summary>
        /// Allows developer to force the rendering of the Menu in DownLevel mode
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/22/2005	Commented
        /// </history>
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
        /// Returns whether the Menu will render DownLevel or not
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
                if( ForceDownLevel || this.IsCrawler || ClientAPI.BrowserSupportsFunctionality( ClientAPI.ClientFunctionality.DHTML ) == false || ClientAPI.BrowserSupportsFunctionality( ClientAPI.ClientFunctionality.XML ) == false )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsCrawler
        {
            get
            {
                if( ( (string)( ViewState["IsCrawler"] ) ).Length == 0 )
                {
                    return HttpContext.Current.Request.Browser.Crawler;
                }
                else
                {
                    return Convert.ToBoolean( ViewState["IsCrawler"] );
                }
            }
            set
            {
                ViewState["IsCrawler"] = value;
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
        public string ClientAPIScriptPath
        {
            get
            {
                return ClientAPI.ScriptPath;
            }
            set
            {
                // Commented out because it was breaking the build.. (SM 26 Oct)
                // uncommented out, don't understand how this could have broke build - JH 22 Feb 2005
                ClientAPI.ScriptPath = value;
            }
        }

        /// <summary>
        /// Location of dnn.controls.DNNMenu.js file
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// Since 1.1 this path will be the same as the ClientAPI path.
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/22/2005	Created
        /// </history>
        public string MenuScriptPath
        {
            get
            {
                if( ( (string)( ViewState["MenuScriptPath"] ) ).Length == 0 )
                {
                    return ClientAPIScriptPath;
                }
                else
                {
                    return ViewState["MenuScriptPath"].ToString();
                }
            }
            set
            {
                ViewState["MenuScriptPath"] = value;
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
        [Description( "Directory to find the images for the menu.  Need to have spacer.gif here!" ), DefaultValue( "images/" )]
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
        ///
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Browsable( true ), PersistenceMode( PersistenceMode.InnerProperty )]
        public MenuNodeCollection MenuNodes
        {
            get
            {
                if( m_objNodes == null )
                {
                    m_objNodes = new MenuNodeCollection( this.ClientID, this );
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
        public ArrayList SelectedMenuNodes
        {
            get
            {
                return this.MenuNodes.FindSelectedNodes();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
        public string RootArrowImage
        {
            get
            {
                return Convert.ToString( ViewState["RootArrowImage"] );
            }
            set
            {
                ViewState["RootArrowImage"] = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
        public string ChildArrowImage
        {
            get
            {
                return Convert.ToString( ViewState["ChildArrowImage"] );
            }
            set
            {
                ViewState["ChildArrowImage"] = value;
            }
        }

        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
        public string WorkImage
        {
            get
            {
                return Convert.ToString( ViewState["WorkImage"] );
            }
            set
            {
                ViewState["WorkImage"] = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Bindable( true ), DefaultValue( "" )]
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
        ///
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Browsable( true ), PersistenceMode( PersistenceMode.InnerProperty )]
        public NodeImageCollection ImageList
        {
            get
            {
                if( m_objImages == null )
                {
                    m_objImages = new NodeImageCollection();
                    if( IsTrackingViewState )
                    {
                        ( (IStateManager)m_objImages ).TrackViewState();
                    }
                }
                return m_objImages;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
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
        ///
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
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
        ///
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
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

        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
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

        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
        public string DefaultIconCssClass
        {
            get
            {
                return ( Convert.ToString( ViewState["DefaultIconCssClass"] ) );
            }
            set
            {
                ViewState["DefaultIconCssClass"] = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
        public string MenuBarCssClass
        {
            get
            {
                return ( Convert.ToString( ViewState["MenuBarCssClass"] ) );
            }
            set
            {
                ViewState["MenuBarCssClass"] = value;
            }
        }

        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
        public string MenuCssClass
        {
            get
            {
                return ( Convert.ToString( ViewState["MenuCssClass"] ) );
            }
            set
            {
                ViewState["MenuCssClass"] = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        private IDNNMenuWriter MenuWriter
        {
            get
            {
                IDNNMenuWriter oWriter;
                if( this.IsDownLevel )
                {
                    oWriter = new DNNMenuWriter( this.IsCrawler );
                }
                else
                {
                    oWriter = new DNNMenuUpLevelWriter();
                }
                return oWriter;
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
        /// Allows the nodes to be populated on the client
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	4/6/2005	Created
        /// </history>
        [Bindable( false ), DefaultValue( true ), PersistenceMode( PersistenceMode.Attribute )]
        public bool PopulateNodesFromClient
        {
            get
            {
                if( ViewState["PopNodesFromClient"] == null )
                {
                    return true;
                }
                else
                {
                    return ( Convert.ToBoolean( ViewState["PopNodesFromClient"] ) );
                }
            }
            set
            {
                ViewState["PopNodesFromClient"] = value;
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
        [Bindable( true ), PersistenceMode( PersistenceMode.Attribute )]
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

        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
        public Orientation Orientation
        {
            get
            {
                return ( (Orientation)ViewState["Orientation"] ); //?
            }
            set
            {
                ViewState["Orientation"] = value;
            }
        }

        [PersistenceMode( PersistenceMode.Attribute ), DefaultValue( true )]
        public bool UseTables
        {
            get
            {
                if( ViewState["UseTables"].ToString().Length == 0 )
                {
                    return true;
                }
                else
                {
                    return Convert.ToBoolean( ViewState["UseTables"] );
                }
            }
            set
            {
                ViewState["UseTables"] = value;
            }
        }

        [Description( "Number of milliseconds to wait befor hiding sub-menu on mouse out" ), Category( "Behavior" ), PersistenceMode( PersistenceMode.Attribute ), DefaultValue( 250 )]
        public int MouseOutDelay
        {
            get
            {
                if( ViewState["MouseOutDelay"].ToString().Length == 0 )
                {
                    return 250;
                }
                else
                {
                    return Convert.ToInt32( ViewState["MouseOutDelay"] );
                }
            }
            set
            {
                ViewState["MouseOutDelay"] = value;
            }
        }

        [Description( "Number of milliseconds to wait befor displaying sub-menu on mouse over" ), Category( "Behavior" ), PersistenceMode( PersistenceMode.Attribute ), DefaultValue( 250 )]
        public int MouseInDelay
        {
            get
            {
                if( ViewState["MouseInDelay"].ToString().Length == 0 )
                {
                    return 250;
                }
                else
                {
                    return Convert.ToInt32( ViewState["MouseInDelay"] );
                }
            }
            set
            {
                ViewState["MouseInDelay"] = value;
            }
        }

        [PersistenceMode( PersistenceMode.Attribute ), DefaultValue( false )]
        public bool EnablePostbackState
        {
            get
            {
                if( ViewState["EnablePostbackState"].ToString().Length == 0 )
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean( ViewState["EnablePostbackState"] );
                }
            }
            set
            {
                ViewState["EnablePostbackState"] = value;
            }
        }

        /// <summary>
        /// Responsible for rendering the DNNMenu and in an uplevel rendering of the Menu, sending down the xml for the child nodes in a ClientAPI variable.
        /// </summary>
        /// <param name="writer"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        ///		[Jon Henning 4/6/2005	Commented
        /// </history>
        protected override void Render( HtmlTextWriter writer )
        {
            MenuWriter.RenderMenu( writer, this );
            //added back...??? not sure if this is ok... - urllist needs it
            //If Me.IsDownLevel = False Then DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Me.Page, Me.ClientID & "_xml", Me.MenuNodes.ToXml, True)
            Control oCtl;
            foreach( Control tempLoopVar_oCtl in this.Controls )
            {
                oCtl = tempLoopVar_oCtl;
                oCtl.RenderControl( writer );
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
            if( ClientAPI.NeedsDNNVariable( this ) )
            {
                //This is to allow us to add a control to our parent's control collection...  kindof a hack
                this.Page.Load += new EventHandler( ParentOnLoad );
            }
            else
            {
                LoadPostedXML();
            }
        }

        protected void ParentOnLoad( object Sender, EventArgs e )
        {
            ClientAPI.RegisterDNNVariableControl( this );

            LoadPostedXML();
        }

        protected override void OnLoad( EventArgs e )
        {
            if( IsDownLevel && Strings.Len( ViewState["xml"] ) > 0 )
            {
                m_objNodes = new MenuNodeCollection( ViewState["xml"].ToString(), "", this );
            }

            //RegisterClientScript()
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        protected override object SaveViewState()
        {
            object _baseState = base.SaveViewState();
            object _imagesState = ( (IStateManager)ImageList ).SaveViewState();
            object[] _newState = new object[2];
            _newState[0] = _baseState;
            _newState[1] = _imagesState;
            return _newState;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="state"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        protected override void LoadViewState( object state )
        {
            if( !( state == null ) )
            {
                object[] _newState = (object[])state;
                if( !( _newState[0] == null ) )
                {
                    base.LoadViewState( _newState[0] );
                }
                if( !( _newState[1] == null ) )
                {
                    ( (IStateManager)ImageList ).LoadViewState( _newState[1] );
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        protected override void TrackViewState()
        {
            base.TrackViewState();
            //CType(Root, IStateManager).TrackViewState()
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="eventArgument"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	5/6/2005	Created
        /// </history>
        public virtual void RaisePostBackEvent( string eventArgument )
        {
            string[] args = eventArgument.Split( ClientAPI.COLUMN_DELIMITER.ToCharArray()[0] );

            MenuNode Node = MenuNodes.FindNode( args[0] );

            if( !( Node == null ) )
            {
                if( args.Length > 1 )
                {
                    switch( args[1] )
                    {
                        case "Click":

                            Node.Click();
                            break;
                        case "Checked":

                            Node.Selected = !Node.Selected;
                            break;
                        case "OnDemand":

                            if( PopulateOnDemandEvent != null )
                            {
                                PopulateOnDemandEvent( this, new DNNMenuEventArgs( Node ) );
                            }
                            break;
                    }
                }
                else
                {
                    //assume an event with no specific argument to be a click
                    Node.Click();
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
        public virtual void OnNodeClick( DNNMenuNodeClickEventArgs e )
        {
            if( NodeClickEvent != null )
            {
                NodeClickEvent( this, e );
            }
        }

        private void DNNMenu_PreRender( object sender, EventArgs e )
        {
            RegisterClientScript();
            Page.RegisterRequiresPostBack( this );
            UpdateNodes( this.MenuNodes ); //update all imageindex properties

            if( this.IsDownLevel == false )
            {
                ClientAPI.RegisterClientVariable( this.Page, this.ClientID + "_xml", this.MenuNodes.ToXml(), true );
            }
            else //If Me.EnablePostbackState Then
            {
                ViewState["xml"] = this.MenuNodes.ToXml();
            }
        }

        private void LoadPostedXML()
        {
            string strXML = "";
            //If Me.EnablePostbackState AndAlso Me.IsDownLevel = False Then strXML = DotNetNuke.UI.Utilities.ClientAPI.GetClientVariable(Me.Page, Me.ClientID & "_xml")
            if( this.IsDownLevel == false )
            {
                strXML = ClientAPI.GetClientVariable( this.Page, this.ClientID + "_xml" );
            }
            if( strXML.Length > 0 )
            {
                LoadXml( strXML );
            }
        }

        public bool LoadPostData( string postDataKey, NameValueCollection postCollection )
        {
            //We need to process the individual checkboxes
            //If Me.CheckBoxes Then SelectNodes(Nothing, postCollection)

            return false;
        }

        public void RaisePostDataChangedEvent()
        {
        }

        public string RaiseClientAPICallbackEvent( string eventArgument )
        {
            string[] aryArgs = eventArgument.Split( ClientAPI.COLUMN_DELIMITER.ToCharArray()[0] );
            string strNodeXml = "<root>" + aryArgs[0] + "</root>";
            MenuNodeCollection objNodes = new MenuNodeCollection( strNodeXml, "", this );

            MenuNode objNode = objNodes[0];
            if( objNode != null )
            {
                if( PopulateOnDemandEvent != null )
                {
                    PopulateOnDemandEvent( this, new DNNMenuEventArgs( objNode ) );
                }
                MenuNode objTNode = this.FindNode( objNode.ID ); //if whole Menu was populated (i.e. LoadXml, then use the node from the Menu

                if( objTNode != null )
                {
                    return objTNode.XmlNode.InnerXml;
                }
                else //if only passed in node object was updated then use that xml.
                {
                    return objNode.XmlNode.InnerXml;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Responsible for inputting the hidden field necessary for the ClientAPI to pass variables back in forth
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	4/6/2005	Commented
        /// </history>
        [Obsolete( "RegisterDNNVariableControl on control level is now obsolete.  Use RegisterDNNVariableControl.WebControls" )]
        public void RegisterDNNVariableControl()
        {
            ClientAPI.RegisterDNNVariableControl( this );
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
                if( this.PopulateNodesFromClient ) //AndAlso DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.XMLHTTP) Then
                {
                    ClientAPI.RegisterClientReference( this.Page, ClientAPI.ClientNamespaceReferences.dnn_xmlhttp );
                }
                if( !ClientAPI.IsClientScriptBlockRegistered( this.Page, "dnn.controls.DNNMenu.js" ) )
                {
                    ClientAPI.RegisterClientScriptBlock( this.Page, "dnn.controls.dnnmenu.js", "<script src=\"" + MenuScriptPath + "dnn.controls.dnnmenu.js\"></script>" );
                }
                if( this.Visible )
                {
                    ClientAPI.RegisterStartUpScript( this.Page, this.ClientID + "_startup", "<script>dnn.controls.initMenu($('" + this.ClientID + "'));</script>" ); //wrong place
                }
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
        public MenuNode FindNode( string strID )
        {
            return this.MenuNodes.FindNode( strID );
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
        public MenuNode FindNodeByKey( string strKey )
        {
            return this.MenuNodes.FindNodeByKey( strKey );
        }

        public void LoadXml( string strXml )
        {
            m_objNodes = new MenuNodeCollection( strXml, "", this );
        }

        private void UpdateNodes( MenuNodeCollection objNodes )
        {
            MenuNode objNode;
            //todo: xpath lookup for img attribute so we don't waste time looping.
            foreach( MenuNode tempLoopVar_objNode in objNodes )
            {
                objNode = tempLoopVar_objNode;
                if( objNode.Image.Length > 0 )
                {
                    if( this.ImageList.Contains( new NodeImage( objNode.Image ) ) == false )
                    {
                        objNode.ImageIndex = this.ImageList.Add( objNode.Image );
                    }
                    else
                    {
                        objNode.ImageIndex = this.ImageList.IndexOf( new NodeImage( objNode.Image ) );
                    }
                    objNode.Image = null;
                }
                if( objNode.DNNNodes.Count > 0 )
                {
                    UpdateNodes( objNode.MenuNodes );
                }
            }
        }
    }
}