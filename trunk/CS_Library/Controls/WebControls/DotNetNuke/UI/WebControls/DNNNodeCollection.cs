using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace DotNetNuke.UI.WebControls
{
    // Warning: Custom Attribute Disabled --> [DefaultMemberAttribute("Item")]
    public class DNNNodeCollection : XmlCollectionBase
    {
        /// <summary>
        /// Constructor to call when creating a Root Node
        /// </summary>
        /// <param name="strNamespace">Namespace of node hierarchy</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public DNNNodeCollection( string strNamespace ) : base( strNamespace )
        {
        }

        /// <summary>
        /// Loads node collection based off of XML string
        /// </summary>
        /// <param name="strXML">XML string</param>
        /// <param name="strXSLFile">XSL FileName.  Leave empty if no transform needed</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public DNNNodeCollection( string strXML, string strXSLFile ) : base( strXML, strXSLFile )
        {
        }

        /// <summary>
        /// Constructor for all nodes that are not the root.
        /// </summary>
        /// <param name="objXmlNode">Node whose children will be exposed by this class</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public DNNNodeCollection( XmlNode objXmlNode ) : base( objXmlNode )
        {
        }

        //In order to maintain backwards compatibility with the tree from versions before 3.2 we need to allow the
        //baseclass to return the treenodeenumerator, thus the need to pass the tree to the new base collection class
        //Yes, this is a hack!
        public DNNNodeCollection( string strNamespace, DnnTree objTreeControl ) : base( strNamespace, objTreeControl )
        {
        }

        public DNNNodeCollection( XmlNode objXmlNode, DnnTree objTreeControl ) : base( objXmlNode, objTreeControl )
        {
        }

        public XmlNode XMLNode
        {
            get
            {
                return InnerXMLNode;
            }
        }

        public XmlDocument XMLDoc
        {
            get
            {
                return InnerXMLDoc;
            }
        }

        public int Add() // DNNNode
        {
            DNNNode objNode = new DNNNode();
            this.XMLNode.AppendChild( this.XMLDoc.ImportNode( objNode.XmlNode, false ) );
            objNode.ID = objNode.ParentNameSpace + "_" + Count;
            return this.XMLNode.ChildNodes.Count - 1; //Return objNode
        }

        public int Add( DNNNode objNode ) // DNNNode
        {
            XmlNode objXmlNode = this.XMLDoc.ImportNode( objNode.XmlNode, true );
            this.XMLNode.AppendChild( objXmlNode );
            objNode.AssociateXmlNode( objXmlNode );
            return this.XMLNode.ChildNodes.Count - 1;
        }

        public int AddBreak() // DNNNode
        {
            int intIndex = this.Add();
            DNNNode objNode = this[intIndex];
            objNode.IsBreak = true;
            return intIndex;
        }

        public int Add( string strID, string strKey, string strText, string strNavigateURL, string strJSFunction, string strTarget, string strToolTip, bool blnEnabled, string strCSSClass, string strCSSClassSelected, string strCSSClassHover ) //DNNNode
        {
            int intIndex = Add();
            DNNNode objNode = this[intIndex];

            if( strID.Length > 0 )
            {
                objNode.ID = strID;
            }

            objNode.Key = strKey;
            objNode.Text = strText;
            objNode.NavigateURL = strNavigateURL;
            objNode.JSFunction = strJSFunction;
            objNode.Target = strTarget;
            objNode.ToolTip = strToolTip;
            objNode.Enabled = blnEnabled;
            objNode.CSSClass = strCSSClass;
            objNode.CSSClassSelected = strCSSClassSelected;
            objNode.CSSClassHover = strCSSClassHover;

            return intIndex; //objNode
        }

        public int Import( DNNNode objNode )
        {
            return Import( objNode, true );
        }

        public int Import( DNNNode objNode, bool blnDeep ) // DNNNode
        {
            XmlNode objXmlNode = this.XMLDoc.ImportNode( objNode.XmlNode, blnDeep );
            this.XMLNode.AppendChild( objXmlNode );

            return this.XMLNode.ChildNodes.Count - 1;
        }

        public new DNNNode this[ int index ]
        {
            get
            {
                return new DNNNode( InnerXMLNode.ChildNodes[index] );
            }
            set
            {
                //MyBase.InnerXMLNode.ChildNodes(index) = Value.XmlNode
                throw ( new Exception( "Cannot Assign Node Directly" ) );
            }
        }

        public int IndexOf( DNNNode value )
        {
            int i;
            for( i = 0; i <= this.XMLNode.ChildNodes.Count - 1; i++ )
            {
                if( new DNNNode( this.XMLNode.ChildNodes[i] ).ID == value.ID )
                {
                    return i;
                }
            }
            return 0;
        }

        public void InsertAfter( int index, DNNNode value )
        {
            XmlNode objXmlNode = this.XMLDoc.ImportNode( value.XmlNode, true );
            this.XMLNode.InsertAfter( objXmlNode, this.XMLNode.ChildNodes[index] );
        }

        public void InsertBefore( int index, DNNNode value )
        {
            XmlNode objXmlNode = this.XMLDoc.ImportNode( value.XmlNode, true );
            this.XMLNode.InsertBefore( objXmlNode, this.XMLNode.ChildNodes[index] );
        }

        public void Remove( DNNNode value )
        {
            this.XMLNode.RemoveChild( value.XmlNode );
        }

        public void Remove( int index )
        {
            this.XMLNode.RemoveChild( this.XMLNode.ChildNodes[index] );
        }

        //Protected Overrides Sub OnInsert(ByVal index As Integer, ByVal value As Object)
        //	If m_bNodeCollectionInterfaceCall = False Then Me.InsertAfter(index, CType(value, DNNNode))
        //End Sub

        //Protected Overrides Sub OnRemove(ByVal index As Integer, ByVal value As Object)
        //	If m_bNodeCollectionInterfaceCall = False Then Me.Remove(CType(value, DNNNode))
        //End Sub

        //Protected Overrides Sub OnSet(ByVal index As Integer, ByVal oldValue As Object, ByVal newValue As Object)
        //	Me.Item(index) = CType(newValue, DNNNode)
        //End Sub

        //Protected Overrides Sub OnClear()
        //	Me.Clear()
        //End Sub

        //Public Shadows Sub Clear()
        //	Dim i As Integer
        //	For i = Me.XMLNode.ChildNodes.Count - 1 To 0 Step -1
        //		Me.XMLNode.RemoveChild(Me.XMLNode.ChildNodes(i))
        //	Next
        //End Sub

        public bool Contains( DNNNode value )
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

        public DNNNode FindNode( string ID )
        {
            XmlNode objNode = this.XMLNode.SelectSingleNode( ".//n[@id=\'" + ID + "\']" );
            if( objNode != null )
            {
                return new DNNNode( objNode );
            }
            else
            {
                return null;
            }
        }

        public DNNNode FindNodeByKey( string Key )
        {
            XmlNode objNode = this.XMLNode.SelectSingleNode( ".//n[@key=\'" + Key + "\']" );
            if( objNode != null )
            {
                return new DNNNode( objNode );
            }
            else
            {
                return null;
            }
        }

        public virtual ArrayList FindSelectedNodes()
        {
            ArrayList colNodes = new ArrayList();
            if( this.XMLNode != null )
            {
                XmlNodeList objNodeList = this.XMLNode.SelectNodes( "//n[@selected=\'1\']" );
                XmlNode objNode;
                foreach( XmlNode tempLoopVar_objNode in objNodeList )
                {
                    objNode = tempLoopVar_objNode;
                    colNodes.Add( new DNNNode( objNode ) );
                }
            }
            return colNodes;
        }

        public string ToXml()
        {
            return XMLDoc.OuterXml;
        }

        public new void CopyTo( Array myArr, int index ) //Implements ICollection.CopyTo
        {
            XmlNode objNode;
            foreach( XmlNode tempLoopVar_objNode in InnerXMLNode.ChildNodes )
            {
                objNode = tempLoopVar_objNode;
                //myArr(index) = objNode
                myArr.SetValue( objNode, index );
                index++;
            }
        }

        public new void RemoveAt( int index )
        {
            this.Remove( index );
            //			DNNNodeCollection o;
        }

        public new IEnumerator GetEnumerator() //Implements ICollection.GetEnumerator
        {
            return new DNNNodeEnumerator( InnerXMLNode );
        }

        //The IsSynchronized Boolean property returns True if the
        //collection is designed to be thread safe; otherwise, it returns False.
        //ReadOnly Property IsSynchronized() As Boolean		  'Implements ICollection.IsSynchronized
        //	Get
        //		Return False
        //	End Get
        //End Property

        //The SyncRoot property returns an object, which is used to synchronize
        //the collection. This should return the instance of the object or return the
        //SyncRoot of another collection if the collection contains other collections.
        //ReadOnly Property SyncRoot() As Object		  'Implements ICollection.SyncRoot
        //	Get
        //		Return Me
        //	End Get
        //End Property

        //The ReadOnly property Count returns the number
        //of items in the custom collection.
        public new int Count //Implements ICollection.Count
        {
            get
            {
                return InnerXMLNode.ChildNodes.Count;
            }
        }

        private string DoTransform( string XML, string XSL )
        {
            return DoTransform( XML, XSL, null );
        }

        private string DoTransform( string XML, string XSL, XsltArgumentList Params )
        {
            try
            {
                XslTransform oXsl = new XslTransform();
                oXsl.Load( XSL );
                XmlTextReader oXR = new XmlTextReader( XML, XmlNodeType.Document, null );

                XPathDocument oXml = new XPathDocument( oXR );
                XmlUrlResolver oResolver = new XmlUrlResolver();

                StringBuilder oSB = new StringBuilder();
                StringWriter oWriter = new StringWriter( oSB, null );
                oXsl.Transform( oXml, Params, oWriter, oResolver );
                oWriter.Close();
                return oSB.ToString();
            }
            catch( Exception ex )
            {
                throw ( ex );
            }
        }
    }
}