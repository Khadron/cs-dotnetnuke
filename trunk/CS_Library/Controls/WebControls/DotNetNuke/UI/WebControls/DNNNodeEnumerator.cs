using System;
using System.Collections;
using System.Xml;

namespace DotNetNuke.UI.WebControls
{
    public class DNNNodeEnumerator : IEnumerator
    {
        private XmlNode m_objXMLNode;
        private int m_intCursor;

        public DNNNodeEnumerator( XmlNode objRoot )
        {
            m_objXMLNode = objRoot;
            m_intCursor = -1;
        }

        public void Reset()
        {
            m_intCursor = -1;
        }

        public bool MoveNext()
        {
            if( m_intCursor < m_objXMLNode.ChildNodes.Count )
            {
                m_intCursor++;
            }

            if( m_intCursor == m_objXMLNode.ChildNodes.Count )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object Current
        {
            get
            {
                if( ( m_intCursor < 0 ) || ( m_intCursor == m_objXMLNode.ChildNodes.Count ) )
                {
                    throw ( new InvalidOperationException() );
                }
                else
                {
                    return new DNNNode( m_objXMLNode.ChildNodes[m_intCursor] );
                }
            }
        }
    }
}