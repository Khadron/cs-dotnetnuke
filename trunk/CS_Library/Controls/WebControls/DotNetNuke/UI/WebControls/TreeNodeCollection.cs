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
using System.Web.UI;
using System.Xml;

namespace DotNetNuke.UI.WebControls
{
    public class TreeNodeCollection : DNNNodeCollection, IStateManager
    {
        private DnnTree m_objDNNTree;

        /// <summary>
        /// Constructor to call when creating a Root Node
        /// </summary>
        /// <param name="strNamespace">Namespace of node hierarchy</param>
        /// <param name="objControl">DnnTree control associated to TreeNodeCollection</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public TreeNodeCollection( string strNamespace, DnnTree objControl ) : base( strNamespace, objControl )
        {
            m_objDNNTree = objControl;
        }

        /// <summary>
        /// Loads node collection based off of XML string
        /// </summary>
        /// <param name="strXML">XML String</param>
        /// <param name="strXSLFile">XSL FileName.  Leave empty if no transform needed</param>
        /// <param name="objControl">DNN Tree control</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public TreeNodeCollection( string strXML, string strXSLFile, DnnTree objControl ) : base( strXML, strXSLFile )
        {
            m_objDNNTree = objControl;
        }

        /// <summary>
        /// Constructor for all nodes that are not the root.
        /// </summary>
        /// <param name="objXmlNode">Node whose children will be exposed by this class</param>
        /// <param name="objControl">DNN Tree control</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public TreeNodeCollection( XmlNode objXmlNode, DnnTree objControl ) : base( objXmlNode, objControl ) //, ByVal objControl As Control)
        {
            m_objDNNTree = objControl;
        }

        public new int Add() //TreeNode
        {
            return this.Add( "" );
        }

        public int Add( string strText ) // TreeNode
        {
            TreeNode objNode = new TreeNode( m_objDNNTree );
            int intIndex = this.Add( objNode );
            objNode.Text = strText;
            return intIndex;
        }

        public int Add( TreeNode objNode ) // TreeNode
        {
            int intIndex = base.Add( objNode );
            objNode.SetDNNTree( m_objDNNTree );
            //ChildNodes.count is not guaranteed to be unique... GetHashCode should be a little safer (I assume it is medium trust safe...
            if( objNode.ID.Length == 0 )
            {
                objNode.ID = objNode.ParentNameSpace + "_" + objNode.GetHashCode(); //Me.XmlNode.ChildNodes.Count
            }
            return intIndex;
        }

        public int Add( string strID, string strKey, string strText, string strNavigateURL )
        {
            return Add( strID, strKey, strText, strNavigateURL, "", "", "", true, "", "", "", false, eClickAction.Navigate, "", -1 );
        }

        public int Add( string strID, string strKey, string strText, string strNavigateURL, string strJSFunction, string strTarget, string strToolTip, bool blnEnabled, string strCSSClass, string strCSSClassSelected, string strCSSClassHover, bool blnSelected, eClickAction enumClickAction, string strCssClassOver, int intImageIndex ) // TreeNode
        {
            int intIndex = this.Add();
            TreeNode objNode = this[intIndex];

            if( strID.Length > 0 )
            {
                objNode.ID = strID;
            }
            else
            {
                objNode.ID = objNode.ParentNameSpace + "_" + this.XMLNode.ChildNodes.Count;
            }
            objNode.Key = strKey;
            objNode.Text = strText;
            objNode.NavigateUrl = strNavigateURL;
            objNode.JSFunction = strJSFunction;
            objNode.Target = strTarget;
            objNode.ToolTip = strToolTip;
            objNode.Enabled = blnEnabled;
            objNode.CssClass = strCSSClass;
            objNode.CSSClassSelected = strCSSClassSelected;
            objNode.CSSClassHover = strCSSClassHover;
            objNode.Selected = blnSelected;
            objNode.ClickAction = enumClickAction;
            //objNode.CheckBox = blnCheckBox
            objNode.CssClassOver = strCssClassOver;
            objNode.ImageIndex = intImageIndex;

            return intIndex;
        }

        public new TreeNode this[ int index ]
        {
            get
            {
                return new TreeNode( this.XMLNode.ChildNodes[index], m_objDNNTree );
            }
            set
            {
                throw ( new Exception( "Cannot Assign Node Directly" ) );
            }
        }

        public int IndexOf( TreeNode value )
        {
            int i;
            for( i = 0; i <= this.XMLNode.ChildNodes.Count; i++ )
            {
                if( new TreeNode( this.XMLNode.ChildNodes[i], m_objDNNTree ).ID == value.ID )
                {
                    return i;
                }
            }
            return 0;
        }

        public void Insert( int index, TreeNode value )
        {
            this.XMLNode.InsertAfter( this.XMLNode.ChildNodes[index], value.XmlNode );
        }

        public void Remove( TreeNode value )
        {
            this.XMLNode.RemoveChild( value.XmlNode );
        }

        public new void Clear()
        {
            int i;
            for( i = this.XMLNode.ChildNodes.Count - 1; i >= 0; i-- )
            {
                this.XMLNode.RemoveChild( this.XMLNode.ChildNodes[i] );
            }
        }

        public bool Contains( TreeNode value )
        {
            if( this.FindNode( value.ID ) == null )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public new TreeNode FindNode( string ID )
        {
            XmlNode objNode = this.XMLNode.SelectSingleNode( "//n[@id=\'" + ID + "\']" );
            if( objNode != null )
            {
                return new TreeNode( objNode, m_objDNNTree );
            }
            else
            {
                return null;
            }
        }

        public new TreeNode FindNodeByKey( string Key )
        {
            XmlNode objNode = this.XMLNode.SelectSingleNode( "//n[@key=\'" + Key + "\']" );
            if( objNode != null )
            {
                return new TreeNode( objNode, m_objDNNTree );
            }
            else
            {
                return null;
            }
        }

        public override ArrayList FindSelectedNodes()
        {
            ArrayList colTreeNodes = new ArrayList(); //TreeNodeCollection = New TreeNodeCollection(m_strNamespace, m_objDNNTree)
            if( this.XMLNode != null )
            {
                XmlNodeList objNodeList = this.XMLNode.SelectNodes( "//n[@selected=\'1\']" );
                XmlNode objNode;
                foreach( XmlNode tempLoopVar_objNode in objNodeList )
                {
                    objNode = tempLoopVar_objNode;
                    colTreeNodes.Add( new TreeNode( objNode, m_objDNNTree ));
                }
            }
            return colTreeNodes;
        }

        //Public Overloads Function ToXml() As String
        //	Return Me.XMLDoc.OuterXml
        //End Function

        public new IEnumerator GetEnumerator()
        {
            return new TreeNodeEnumerator( this.XMLNode, m_objDNNTree );
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