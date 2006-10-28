using System;
using System.Collections;
using System.Xml;

namespace DotNetNuke.UI.WebControls
{
    public abstract class XmlCollectionBase : ICollection, IList, IEnumerable
    {
        private ArrayList m_arr; //THIS IS NOT USED BUT IS NECESSARY FOR BACKWARDS COMPATIBILITY!!!! DO NOT REMOVE!!!!
        private XmlNode m_objXMLNode;
        private XmlDocument m_objXMLDoc;
        //Private m_objNode As DNNNode

        protected XmlCollectionBase()
        {
        }

        /// <summary>
        /// Constructor to call when creating a Root Node
        /// </summary>
        /// <param name="strNamespace">Namespace of node hierarchy</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public XmlCollectionBase( string strNamespace )
        {
            InnerXMLDoc = new XmlDocument();
            InnerXMLNode = InnerXMLDoc.CreateNode( XmlNodeType.Element, "root", "" );

            XmlAttribute objAttr = InnerXMLDoc.CreateAttribute( "id" );
            objAttr.Value = strNamespace;
            InnerXMLNode.Attributes.Append( objAttr );

            InnerXMLDoc.AppendChild( InnerXMLNode );
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
        public XmlCollectionBase( string strXML, string strXSLFile )
        {
            InnerXMLDoc = new XmlDocument();
            if( strXSLFile.Length > 0 )
            {
                //InnerXMLDoc.LoadXml(DoTransform(strXML, strXSLFile))
            }
            else
            {
                InnerXMLDoc.LoadXml( strXML );
            }
            InnerXMLNode = InnerXMLDoc.SelectSingleNode( "//root" );
            //m_objNode = New DNNNode(InnerXMLNode)
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
        public XmlCollectionBase( XmlNode objXmlNode )
        {
            InnerXMLNode = objXmlNode;
            InnerXMLDoc = InnerXMLNode.OwnerDocument;
            //m_objNode = New DNNNode(InnerXMLNode)
        }

        private DnnTree m_objTree;
        //In order to maintain backwards compatibility with the tree from versions before 3.2 we need to allow the
        //baseclass to return the treenodeenumerator, thus the need to pass the tree to the new base collection class
        //Yes, this is a hack!
        protected XmlCollectionBase( DnnTree objTreeControl )
        {
            m_objTree = objTreeControl;
        }

        public XmlCollectionBase( XmlNode objXmlNode, DnnTree objTreeControl )
        {
            m_objTree = objTreeControl;
            InnerXMLNode = objXmlNode;
            InnerXMLDoc = InnerXMLNode.OwnerDocument;
        }

        public XmlCollectionBase( string strNamespace, DnnTree objTreeControl )
        {
            m_objTree = objTreeControl;
            InnerXMLDoc = new XmlDocument();
            InnerXMLNode = InnerXMLDoc.CreateNode( XmlNodeType.Element, "root", "" );

            XmlAttribute objAttr = InnerXMLDoc.CreateAttribute( "id" );
            objAttr.Value = strNamespace;
            InnerXMLNode.Attributes.Append( objAttr );

            InnerXMLDoc.AppendChild( InnerXMLNode );
        }

        public void Clear()
        {
            int i;
            for( i = this.InnerXMLNode.ChildNodes.Count - 1; i >= 0; i-- )
            {
                this.InnerXMLNode.RemoveChild( this.InnerXMLNode.ChildNodes[i] );
            }
        }

        public IEnumerator GetEnumerator()
        {
            //In order to maintain backwards compatibility with the tree from versions before 3.2 we need to allow the
            //baseclass to return the treenodeenumerator, thus the need to pass the tree to the new base collection class
            //Yes, this is a hack!
            if( m_objTree == null )
            {
                return m_objXMLNode.ChildNodes.GetEnumerator();
            }
            else
            {
                return new TreeNodeEnumerator( m_objXMLNode, m_objTree );
            }
        }

        public void RemoveAt( int index )
        {
            this.InnerXMLNode.RemoveChild( this.InnerXMLNode.ChildNodes[index] );
        }

        public void CopyTo( Array array, int index )
        {
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return null;
            }
        }

        public int Add( object value )
        {
            return 0;
        }

        public bool Contains( object value )
        {
            return false;
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public object this[ int index ]
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public int IndexOf( object value )
        {
            return 0;
        }

        public void Insert( int index, object value )
        {
        }

        public void Remove( object value )
        {
        }

        public int Count
        {
            get
            {
                return this.InnerXMLNode.ChildNodes.Count;
            }
        }

        //These are not necessary, but were in original CollectionBase class so keeping them
        protected ArrayList InnerList
        {
            get
            {
                return null;
            }
        }

        //These are not necessary, but were in original CollectionBase class so keeping them
        protected IList List
        {
            get
            {
                return null;
            }
        }

        protected XmlNode InnerXMLNode
        {
            get
            {
                return m_objXMLNode;
            }
            set
            {
                m_objXMLNode = value;
            }
        }

        protected XmlDocument InnerXMLDoc
        {
            get
            {
                return m_objXMLDoc;
            }
            set
            {
                m_objXMLDoc = value;
            }
        }
    }
}