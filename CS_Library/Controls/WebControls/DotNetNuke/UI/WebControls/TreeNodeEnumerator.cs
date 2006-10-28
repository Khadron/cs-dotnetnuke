using System;
using System.Collections;
using System.Xml;

namespace DotNetNuke.UI.WebControls
{
    public class TreeNodeEnumerator : IEnumerator
    {
        private XmlNode m_objXmlNode;
        private DnnTree m_objDNNTree;
        private int m_intCursor;

        public TreeNodeEnumerator( XmlNode objRoot, DnnTree objControl )
        {
            m_objXmlNode = objRoot;
            m_objDNNTree = objControl;
            m_intCursor = -1;
        }

        public void Reset()
        {
            m_intCursor = -1;
        }

        public bool MoveNext()
        {
            if( m_intCursor < m_objXmlNode.ChildNodes.Count )
            {
                m_intCursor++;
            }

            if( m_intCursor == m_objXmlNode.ChildNodes.Count )
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
                if( ( m_intCursor < 0 ) || ( m_intCursor == m_objXmlNode.ChildNodes.Count ) )
                {
                    throw ( new InvalidOperationException() );
                }
                else
                {
                    return new TreeNode( m_objXmlNode.ChildNodes[m_intCursor], m_objDNNTree );
                }
            }
        }
    }
}