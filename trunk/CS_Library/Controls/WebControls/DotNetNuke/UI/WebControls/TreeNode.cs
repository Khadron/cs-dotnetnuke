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
using System.Xml;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.UI.WebControls
{
    public class TreeNode : DNNNode, IStateManager
    {
        internal static string _separator = ":";
        internal static readonly string _checkboxIDSufix = "checkbox";

        private TreeNodeCollection m_objNodes;
        private DnnTree m_objDNNTree;

        public TreeNode()
        {
        }

        public TreeNode( string strText ) : base( strText )
        {
        }

        internal TreeNode( XmlNode objXmlNode, Control ctlOwner ) : base( objXmlNode )
        {
            m_objDNNTree = (DnnTree)ctlOwner;
        }

        internal TreeNode( Control ctlOwner )
        {
            m_objDNNTree = (DnnTree)ctlOwner;
        }

        //This is here for backwards compatibility
        public string CssClass
        {
            get
            {
                return CSSClass;
            }
            set
            {
                CSSClass = value;
            }
        }

        //This is here for backwards compatibility
        public string NavigateUrl
        {
            get
            {
                return NavigateURL;
            }
            set
            {
                NavigateURL = value;
            }
        }

        [Browsable( true ), PersistenceMode( PersistenceMode.InnerProperty )]
        public TreeNodeCollection TreeNodes
        {
            get
            {
                if( m_objNodes == null )
                {
                    m_objNodes = new TreeNodeCollection( this.XmlNode, this.DNNTree );
                }
                return m_objNodes;
            }
        }

        [Browsable( false )]
        public TreeNode Parent
        {
            get
            {
                return this.ParentNode;
            }
        }

        [Browsable( false )]
        public DnnTree DNNTree
        {
            get
            {
                return m_objDNNTree;
            }
        }

        [DefaultValue( false ), Browsable( false )]
        public bool IsExpanded
        {
            get
            {
                object _expanded;
                if( DNNTree.IsDownLevel == false )
                {
                    string sExpanded = ClientAPI.GetClientVariable( m_objDNNTree.Page, m_objDNNTree.ClientID + "_" + this.ClientID + ":expanded" );
                    if( sExpanded.Length > 0 )
                    {
                        _expanded = Convert.ToBoolean( sExpanded );
                    }
                    else
                    {
                        _expanded = Convert.ToBoolean( this.CustomAttribute( "expanded", "0" ) );
                    }
                }
                else
                {
                    _expanded = Convert.ToBoolean( this.CustomAttribute( "expanded", "0" ) );
                }
                return Convert.ToBoolean( _expanded );
            }
        }

        //<Bindable(True), DefaultValue(False), PersistenceMode(PersistenceMode.Attribute)> _
        //  Public Property CheckBox() As Boolean
        //	Get
        //		Dim _checkBox As Object = Me.CustomAttribute("checkBox", 0)
        //		Return CType(_checkBox, Boolean)
        //	End Get
        //	Set(ByVal Value As Boolean)
        //		Me.CustomAttribute("checkBox", 0) = Value
        //	End Set
        //End Property

        [Bindable( true ), DefaultValue( "" ), PersistenceMode( PersistenceMode.Attribute )]
        public string CssClassOver
        {
            get
            {
                return this.CSSClassHover;
            }
            set
            {
                this.CSSClassHover = value;
            }
        }

        [Bindable( true ), DefaultValue( -1 ), PersistenceMode( PersistenceMode.Attribute )]
        public int ImageIndex
        {
            get
            {
                if( this.CustomAttribute( "imgIdx" ).Length > 0 )
                {
                    return Convert.ToInt32( this.CustomAttribute( "imgIdx" ) );
                }
                else
                {
                    if( this.DNNTree.ImageList.Count > 0 )
                    {
                        return 0; //BACKWARDS COMPATIBILITY!!!! SHOULD BE -1
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            set
            {
                this.SetCustomAttribute( "imgIdx", value.ToString() );
            }
        }

        //Public Property PopulateOnDemand() As Boolean
        //	Get
        //		Return CBool(Me.CustomAttribute("pond", False))
        //	End Get
        //	Set(ByVal Value As Boolean)
        //		Me.CustomAttribute("pond") = Value
        //	End Set
        //End Property

        public string LeftHTML
        {
            get
            {
                return this.CustomAttribute( "lhtml", "" );
            }
            set
            {
                this.SetCustomAttribute( "lhtml", value );
            }
        }

        public string RightHTML
        {
            get
            {
                return this.CustomAttribute( "rhtml", "" );
            }
            set
            {
                this.SetCustomAttribute( "rhtml", value );
            }
        }

        public new TreeNode ParentNode
        {
            get
            {
                if( this.XmlNode.ParentNode != null && this.XmlNode.ParentNode.NodeType != XmlNodeType.Document )
                {
                    return new TreeNode( this.XmlNode.ParentNode, m_objDNNTree );
                }
                else
                {
                    return null;
                }
            }
        }

        public void MakeNodeVisible()
        {
            if( this.Parent != null )
            {
                this.Parent.Expand();
                this.Parent.MakeNodeVisible();
            }
        }

        private ITreeNodeWriter NodeWriter
        {
            get
            {
                if( m_objDNNTree.IsDownLevel )
                {
                    return new TreeNodeWriter();
                }
                else
                {
                    return null; // New TreeNodeUpLevelWriter
                }
            }
        }

        public void Expand()
        {
            if( HasNodes )
            {
                this.SetCustomAttribute( "expanded", "1" );
                DNNTree.OnExpand( new DNNTreeEventArgs( this ) );
            }
        }

        public void Collapse()
        {
            if( HasNodes )
            {
                this.SetCustomAttribute( "expanded", "0" );
                DNNTree.OnCollapse( new DNNTreeEventArgs( this ) );
            }
        }

        public void Click()
        {
            this.Selected = true;
            DNNTree.OnNodeClick( new DNNTreeNodeClickEventArgs( this ) );
        }

        public virtual void Render( HtmlTextWriter writer )
        {
            NodeWriter.RenderNode( writer, this );
        }

        internal void SetDNNTree( DnnTree objTree )
        {
            m_objDNNTree = objTree;
        }

        //BACKWARDS COMPATIBILITY ONLY

        public bool IsTrackingViewState
        {
            get
            {
                return false;
            }
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
        public void LoadViewState( object state )
        {
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
        public object SaveViewState()
        {
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        public void TrackViewState()
        {
        }
    }
}