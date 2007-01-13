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
using DotNetNuke.UI.Design.WebControls;
using DotNetNuke.UI.Utilities;
using Microsoft.VisualBasic;

namespace DotNetNuke.UI.WebControls
{
    [ControlBuilder( typeof( DNNTreeBuilder ) ), Designer( typeof( DNNTreeDesigner ) ), DefaultProperty( "Nodes" ), ToolboxData( "<{0}:DNNTree runat=server></{0}:DNNTree>" )]
    public class DnnTree : WebControl, IClientAPICallbackEventHandler
    {
        public delegate void DNNTreeEventHandler( object source, DNNTreeEventArgs e );

        public delegate void DNNTreeNodeClickHandler( object source, DNNTreeNodeClickEventArgs e );

        private DNNTreeEventHandler ExpandEvent;

        public event DNNTreeEventHandler Expand
        {
            add
            {
                ExpandEvent = (DNNTreeEventHandler)Delegate.Combine( ExpandEvent, value );
            }
            remove
            {
                ExpandEvent = (DNNTreeEventHandler)Delegate.Remove( ExpandEvent, value );
            }
        }

        private DNNTreeEventHandler CollapseEvent;

        public event DNNTreeEventHandler Collapse
        {
            add
            {
                CollapseEvent = (DNNTreeEventHandler)Delegate.Combine( CollapseEvent, value );
            }
            remove
            {
                CollapseEvent = (DNNTreeEventHandler)Delegate.Remove( CollapseEvent, value );
            }
        }

        private DNNTreeNodeClickHandler NodeClickEvent;

        public event DNNTreeNodeClickHandler NodeClick
        {
            add
            {
                NodeClickEvent = (DNNTreeNodeClickHandler)Delegate.Combine( NodeClickEvent, value );
            }
            remove
            {
                NodeClickEvent = (DNNTreeNodeClickHandler)Delegate.Remove( NodeClickEvent, value );
            }
        }

        private DNNTreeEventHandler PopulateOnDemandEvent;

        public event DNNTreeEventHandler PopulateOnDemand
        {
            add
            {
                PopulateOnDemandEvent = (DNNTreeEventHandler)Delegate.Combine( PopulateOnDemandEvent, value );
            }
            remove
            {
                PopulateOnDemandEvent = (DNNTreeEventHandler)Delegate.Remove( PopulateOnDemandEvent, value );
            }
        }

        private TreeNodeCollection m_objNodes;
        private NodeImageCollection m_objImages;

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        public DnnTree()
        {
        }

        /// <summary>
        /// Allows developer to force the rendering of the tree in DownLevel mode
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

        public bool IsCrawler
        {
            get
            {
                object isCrawler = ViewState["IsCrawler"];

                if (isCrawler == null /*&& ((string)isCrawler).Length == 0*/)
                {
                    return HttpContext.Current.Request.Browser.Crawler;
                }
                else
                {
                    return Convert.ToBoolean(ViewState["IsCrawler"]);
                }
                
            }
            set
            {
                ViewState["IsCrawler"] = value;
            }
        }

        /// <summary>
        /// Returns whether the tree will render DownLevel or not
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/22/2005	Commented
        ///		[Jon Henning]	3/9/2005	Requiring XML support on client for uplevel
        ///		[Jon Henning]	11/28/2005	rendering downlevel if it is a crawler
        /// </history>
        [Browsable( false )]
        public bool IsDownLevel
        {
            get
            {
                if( ForceDownLevel || IsCrawler || ClientAPI.BrowserSupportsFunctionality( ClientAPI.ClientFunctionality.DHTML ) == false || ClientAPI.BrowserSupportsFunctionality( ClientAPI.ClientFunctionality.XML ) == false )
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
        /// Location of dnn.controls.dnntree.js file
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// Since 1.1 this path will be the same as the ClientAPI path.
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	2/22/2005	Created
        /// </history>
        public string TreeScriptPath
        {
            get
            {
                if( Strings.Len( ViewState["TreeScriptPath"] ) == 0 )
                {
                    return ClientAPIScriptPath;
                }
                else
                {
                    return ViewState["TreeScriptPath"].ToString();
                }
            }
            set
            {
                ViewState["TreeScriptPath"] = value;
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
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        [Browsable( true ), PersistenceMode( PersistenceMode.InnerProperty )]
        public TreeNodeCollection TreeNodes
        {
            get
            {
                if( m_objNodes == null )
                {
                    m_objNodes = new TreeNodeCollection( this.ClientID, this );
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
        public ArrayList SelectedTreeNodes
        {
            get
            {
                return this.TreeNodes.FindSelectedNodes();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        [Bindable( true ), DefaultValue( 10 ), PersistenceMode( PersistenceMode.Attribute )]
        public int IndentWidth
        {
            get
            {
                return Convert.ToInt32( ViewState["IndentWidth"] );
            }
            set
            {
                ViewState["IndentWidth"] = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
        public string CollapsedNodeImage
        {
            get
            {
                return Convert.ToString( ViewState["CollapsedNodeImage"] );
            }
            set
            {
                ViewState["CollapsedNodeImage"] = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
        public string ExpandedNodeImage
        {
            get
            {
                return Convert.ToString( ViewState["ExpandedNodeImage"] );
            }
            set
            {
                ViewState["ExpandedNodeImage"] = value;
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

        [Bindable( true ), DefaultValue( 5 ), PersistenceMode( PersistenceMode.Attribute ), Category( "Behavior" )]
        public int AnimationFrames
        {
            get
            {
                if( Strings.Len( ViewState["AnimationFrames"] ) > 0 )
                {
                    return Convert.ToInt32( ViewState["AnimationFrames"] );
                }
                else
                {
                    return 5;
                }
            }
            set
            {
                ViewState["AnimationFrames"] = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        [Bindable( true ), DefaultValue( false ), PersistenceMode( PersistenceMode.Attribute )]
        public bool CheckBoxes
        {
            get
            {
                return Convert.ToBoolean( ViewState["CheckBoxes"] );
            }
            set
            {
                ViewState["CheckBoxes"] = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
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
        /// 	[jbrinkman]	5/6/2004	Created
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
        /// 	[jbrinkman]	5/6/2004	Created
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
        /// 	[jbrinkman]	5/6/2004	Created
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
        /// 	[jbrinkman]	5/6/2004	Created
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

        [Bindable( true ), DefaultValue( 12 ), PersistenceMode( PersistenceMode.Attribute )]
        public int ExpandCollapseImageWidth
        {
            get
            {
                if( Strings.Len( ViewState["ExpColImgWidth"] ) > 0 )
                {
                    return Convert.ToInt32( ViewState["ExpColImgWidth"] );
                }
                else
                {
                    return 12;
                }
            }
            set
            {
                ViewState["ExpColImgWidth"] = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        private IDNNTreeWriter TreeWriter
        {
            get
            {
                if( this.IsDownLevel )
                {
                    return new DNNTreeWriter();
                }
                else
                {
                    return new DNNTreeUpLevelWriter();
                }
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
                //If DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.XMLHTTP) Then
                if( ViewState["PopNodesFromClient"] == null )
                {
                    return true;
                }
                else
                {
                    return ( Convert.ToBoolean( ViewState["PopNodesFromClient"] ) );
                }
                //Else
                //Return False
                //End If
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

        /// <summary>
        /// Responsible for rendering the DNNTree and in an uplevel rendering of the tree, sending down the xml for the child nodes in a ClientAPI variable.
        /// </summary>
        /// <param name="writer"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        ///		[Jon Henning 4/6/2005	Commented
        /// </history>
        protected override void Render( HtmlTextWriter writer )
        {
            //If Me.IsDownLevel = False Then DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Me.Page, Me.ClientID & "_xml", Me.TreeNodes.ToXml, True)
            TreeWriter.RenderTree( writer, this );
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
        /// 	[jbrinkman]	5/6/2004	Created
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
                LoadPostedXml();
            }
        }

        protected void ParentOnLoad( object Sender, EventArgs e )
        {
            ClientAPI.RegisterDNNVariableControl( this );
            LoadPostedXml();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
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
        /// 	[jbrinkman]	5/6/2004	Created
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
            if( IsDownLevel && Strings.Len( ViewState["xml"] ) > 0 )
            {
                m_objNodes = new TreeNodeCollection( ViewState["xml"].ToString(), "", this );
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
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
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        public virtual void RaisePostBackEvent( string eventArgument )
        {
            string[] args = eventArgument.Split( ClientAPI.COLUMN_DELIMITER.ToCharArray()[0] );

            TreeNode Node = TreeNodes.FindNode( args[0] );

            if( !( Node == null ) )
            {
                if( args.Length > 1 )
                {
                    switch( args[1] )
                    {
                        case "Click":

                            if( this.CheckBoxes == false )
                            {
                                TreeNode objNode;
                                foreach( TreeNode tempLoopVar_objNode in TreeNodes.FindSelectedNodes() )
                                {
                                    objNode = tempLoopVar_objNode;
                                    objNode.Selected = false;
                                }
                            }
                            if( Node.ClickAction == eClickAction.Expand )
                            {
                                if( Node.DNNNodes.Count == 0 && this.PopulateNodesFromClient ) //Node.DNNNodes.Count = 0 NEW!!!
                                {
                                    if( PopulateOnDemandEvent != null )
                                    {
                                        PopulateOnDemandEvent( this, new DNNTreeEventArgs( Node ) );
                                    }
                                }
                                if( Node.IsExpanded )
                                {
                                    Node.Collapse();
                                }
                                else
                                {
                                    Node.Expand();
                                }
                            }
                            Node.Click();
                            break;
                        case "Checked":

                            Node.Selected = !Node.Selected;
                            break;
                        case "OnDemand":

                            if( PopulateOnDemandEvent != null )
                            {
                                PopulateOnDemandEvent( this, new DNNTreeEventArgs( Node ) );
                            }
                            break;
                    }
                }
                else
                {
                    if( Node.IsExpanded )
                    {
                        Node.Collapse();
                    }
                    else
                    {
                        Node.Expand();
                    }
                    if( Node.DNNNodes.Count == 0 && this.PopulateNodesFromClient ) //Node.DNNNodes.Count = 0 NEW!!!
                    {
                        if( PopulateOnDemandEvent != null )
                        {
                            PopulateOnDemandEvent( this, new DNNTreeEventArgs( Node ) );
                        }
                    }
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
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        public virtual void OnExpand( DNNTreeEventArgs e )
        {
            if( ExpandEvent != null )
            {
                ExpandEvent( this, e );
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        public virtual void OnCollapse( DNNTreeEventArgs e )
        {
            if( CollapseEvent != null )
            {
                CollapseEvent( this, e );
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	10/6/2004	Created
        /// </history>
        public virtual void OnNodeClick( DNNTreeNodeClickEventArgs e )
        {
            if( NodeClickEvent != null )
            {
                NodeClickEvent( this, e );
            }
        }

        private void DnnTree_PreRender( object sender, EventArgs e )
        {
            RegisterClientScript();
            Page.RegisterRequiresPostBack( this );
            UpdateNodes( this.TreeNodes ); //update all imageindex properties

            if( this.IsDownLevel == false )
            {
                ClientAPI.RegisterClientVariable( this.Page, this.ClientID + "_xml", this.TreeNodes.ToXml(), true );
            }
            else
            {
                ViewState["xml"] = this.TreeNodes.ToXml();
                if( ClientAPI.BrowserSupportsFunctionality( ClientAPI.ClientFunctionality.DHTML ) )
                {
                    ClientAPI.RegisterClientReference( this.Page, ClientAPI.ClientNamespaceReferences.dnn );
                    if( this.SelectedTreeNodes.Count > 0 )
                    {
                        ClientAPI.RegisterClientVariable( this.Page, this.ClientID + "_selNode", ( (TreeNode)this.SelectedTreeNodes[1] ).ToJSON( false ), true );
                    }
                }
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
            TreeNodeCollection objNodes = new TreeNodeCollection( strNodeXml, "", this );

            TreeNode objNode = objNodes[0];
            if( objNode != null )
            {
                if( PopulateOnDemandEvent != null )
                {
                    PopulateOnDemandEvent( this, new DNNTreeEventArgs( objNode ) );
                }
                TreeNode objTNode = this.FindNode( objNode.ID ); //if whole tree was populated (i.e. LoadXml, then use the node from the tree

                if( objTNode != null )
                {
                    return objTNode.XmlNode.InnerXml; //objNode.ToXML
                }
                else //if only passed in node object was updated then use that xml.
                {
                    return objNode.XmlNode.InnerXml; //objNode.ToXML
                }
            }
            else
            {
                return null;
            }
        }

        //Private Sub SelectNodes(ByVal node As TreeNode, ByVal selectedNodes As System.Collections.Specialized.NameValueCollection)
        //	Dim objTreeNodes As TreeNodeCollection
        //	If Not node Is Nothing Then
        //		If selectedNodes(node.ID & ":checkbox") Is Nothing Then
        //			'We need to perform a check to see if data has changed
        //			node.Selected = False
        //		Else
        //			node.Selected = True
        //		End If
        //		objTreeNodes = node.TreeNodes
        //	Else
        //		objTreeNodes = Me.TreeNodes
        //	End If

        //	For Each childNode As TreeNode In objTreeNodes
        //		SelectNodes(childNode, selectedNodes)
        //	Next
        //End Sub

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
                if( this.PopulateNodesFromClient ) //AndAlso DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.XMLHTTP) Then
                {
                    ClientAPI.RegisterClientReference( this.Page, ClientAPI.ClientNamespaceReferences.dnn_xmlhttp );
                }
                if( !ClientAPI.IsClientScriptBlockRegistered( this.Page, "dnn.controls.dnntree.js" ) )
                {
                    ClientAPI.RegisterClientScriptBlock( this.Page, "dnn.controls.dnntree.js", "<script src=\"" + TreeScriptPath + "dnn.controls.dnntree.js\"></script>" );
                }
                ClientAPI.RegisterStartUpScript( Page, this.ClientID + "_startup", "<script>dnn.controls.initTree($('" + this.ClientID + "'));</script>" ); //wrong place
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
        public TreeNode FindNode( string strID )
        {
            return this.TreeNodes.FindNode( strID );
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
        public TreeNode FindNodeByKey( string strKey )
        {
            return this.TreeNodes.FindNodeByKey( strKey );
        }

        /// <summary>
        /// Finds Node by passed in ID and selects it.  Additionally it will expand
        /// all parent nodes.
        /// </summary>
        /// <param name="strID">ID of node</param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	11/17/2004	Created
        /// </history>
        public TreeNode SelectNode( string strID )
        {
            TreeNode objNode = null;

            if( this.TreeNodes.Count > 0 )
            {
                objNode = this.FindNode( strID );
                if( objNode != null )
                {
                    objNode.Selected = true;
                    TreeNode objParent;
                    objParent = objNode.Parent;

                    while( objParent != null )
                    {
                        objParent.Expand();
                        objParent = objParent.Parent;
                    }
                }
            }

            return objNode;
        }

        /// <summary>
        /// Finds Node by passed in Key and selects it.  Additionally it will expand
        /// all parent nodes.
        /// </summary>
        /// <param name="strKey">Key of node</param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	11/17/2004	Created
        /// </history>
        public TreeNode SelectNodeByKey( string strKey )
        {
            TreeNode objNode = null;

            if( this.TreeNodes.Count > 0 )
            {
                objNode = this.FindNodeByKey( strKey );
                if( objNode != null )
                {
                    objNode.Selected = true;
                    TreeNode objParent;
                    objParent = objNode;

                    while( objParent != null )
                    {
                        objParent.Expand();
                        objParent = objParent.Parent;
                    }
                }
            }

            return objNode;
        }

        private void LoadPostedXml()
        {
            string strXML = "";
            if( this.IsDownLevel == false )
            {
                strXML = ClientAPI.GetClientVariable( this.Page, this.ClientID + "_xml" );
            }
            if( strXML.Length > 0 )
            {
                LoadXml( strXML );
            }
        }

        public void LoadXml( string strXml )
        {
            m_objNodes = new TreeNodeCollection( strXml, "", this );
        }

        private void UpdateNodes( TreeNodeCollection objNodes )
        {
            TreeNode objNode;
            //todo: xpath lookup for img attribute so we don't waste time looping.
            foreach( TreeNode tempLoopVar_objNode in objNodes )
            {
                objNode = tempLoopVar_objNode;
                if( objNode.Image.Length > 0 )
                {
                    if( this.ImageList.Contains( objNode.Image ) == false ) //THIS CHECK FOR CONTAINS DOES NOT WORK!!!
                    {
                        objNode.ImageIndex = this.ImageList.Add( objNode.Image );
                    }
                    else
                    {
                        objNode.ImageIndex = this.ImageList.IndexOf( objNode.Image );
                    }
                    objNode.Image = null;
                }
                if( objNode.DNNNodes.Count > 0 )
                {
                    UpdateNodes( objNode.TreeNodes );
                }
            }
        }
    }
}