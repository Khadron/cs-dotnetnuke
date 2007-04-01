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
using System.Text;
using System.Xml;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.UI.WebControls
{
    public class DNNNode
    {
        private XmlDocument m_objXMLDoc;
        private XmlNode m_objXMLNode;
        private string m_strParentNS;
        private DNNNodeCollection m_objNodes;
        private Hashtable m_objHashAttributes;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks>
        /// When using this constructor your node will not have properties that are related
        /// to the xml node hierarchy until you associate the node with the hierarchy.  This is
        /// accomplished by adding it to the DNNNodeCollection or by calling Import
        /// directly
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public DNNNode()
            : this(new XmlDocument().CreateNode(XmlNodeType.Element, "n", ""))
        {
        }

        /// <summary>
        /// Constructor providing the defaulting of the Text property
        /// </summary>
        /// <param name="strText">Text for the Node</param>
        /// <remarks>
        /// When using this constructor your node will not have properties that are related
        /// to the xml node hierarchy until you associate the node with the hierarchy.  This is
        /// accomplished by adding it to the DNNNodeCollection or by calling Import
        /// directly
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public DNNNode(string strText)
            : this()
        {
            this.Text = strText;
        }

        public DNNNode(string NodeText, string navigateUrl)
        {
            if (NodeText == null || navigateUrl == null)
            {
                throw (new ArgumentNullException());
            }
            Text = NodeText;
            navigateUrl = navigateUrl;
        }

        /// <summary>
        /// Constructor to create a Node already associated to a hierarchy
        /// </summary>
        /// <param name="objXmlNode"></param>
        /// <remarks>
        /// Preferred method for creating a node.  Since the node wraps the XmlNode object
        /// there will be no need for an intermediate holder of the attributes, they can
        /// be directly written to the XmlNode object that belongs to the hierarchy.
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public DNNNode(XmlNode objXmlNode)
        {
            m_objXMLNode = objXmlNode;
            m_objXMLDoc = objXmlNode.OwnerDocument;
        }

        /// <summary>
        /// A node that is instantiated without knowledge of its parent is created "stand-alone",
        /// therefore, it does not belong to an xmldocument, until associated with a DNNNodeCollection
        /// Until this association takes place, the node will not be able to
        /// expose a property/method that requires the hierarchy.  This property indicates
        /// the node's membership to a hierarchy.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public bool IsInHierarchy
        {
            get
            {
                //Return Not XmlNode Is Nothing
                return XmlNode.ParentNode != null;
            }
        }

        /// <summary>
        /// Exposes the XmlDocument that the OwnerDocument of the XmlNode the class is wrapping
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        internal XmlDocument XMLDoc
        {
            get
            {
                return m_objXMLDoc;
            }
        }

        /// <summary>
        /// Exposes the XmlNode the node is wrapping
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        internal XmlNode XmlNode
        {
            get
            {
                return m_objXMLNode;
            }
        }

        /// <summary>
        /// Exposes the root XmlNode
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        private XmlNode RootNode
        {
            get
            {
                return XMLDoc.ChildNodes[0];
            }
        }

        /// <summary>
        /// Returns the parent DNNNode
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// Requires node to be associated or nothing will return
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public DNNNode ParentNode
        {
            get
            {
                if (this.XmlNode.ParentNode != null && this.XmlNode.ParentNode.NodeType != XmlNodeType.Document)
                {
                    return new DNNNode(this.XmlNode.ParentNode);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns a collection of the children DNNNodes
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public DNNNodeCollection DNNNodes
        {
            get
            {
                if (m_objNodes == null)
                {
                    m_objNodes = new DNNNodeCollection(this.XmlNode);
                }
                return m_objNodes;
            }
        }

        /// <summary>
        /// Returns a colleciton of the children XmlNodes
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public XmlNodeList XmlNodes
        {
            get
            {
                return this.XmlNode.ChildNodes;
            }
        }

        /// <summary>
        /// Returns a flag indicating whether the node has children
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public bool HasNodes
        {
            get
            {
                bool blnHas = Convert.ToBoolean(CustomAttribute("hasNodes", "false"));
                if (blnHas == false)
                {
                    return this.DNNNodes.Count > 0;
                }
                else
                {
                    return blnHas; //False
                }
            }
            set
            {
                //CustomAttribute("hasNodes", 0) = Value
                this.SetCustomAttribute("hasNodes", (value ? "true" : "false").ToString());
            }
        }

        /// <summary>
        /// Gets the node's parent namespace by walking the chain up until it reaches the
        /// root.  This name plus a unique identifier for the node will be unique.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string ParentNameSpace
        {
            get
            {

                if (m_strParentNS == null || m_strParentNS.Length == 0)
                {
                    m_strParentNS = "";
                    if (XmlNode.ParentNode != null && (XmlNode.ParentNode) is XmlElement)
                    {
                        m_strParentNS = XmlNode.ParentNode.Attributes.GetNamedItem("id").Value;
                    }
                }

                return m_strParentNS;
            }
        }

        /// <summary>
        /// Returns the level (depth) of the node within the hierarchy
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public int Level
        {
            get
            {
                //If IsInHierarchy AndAlso Not Me.ParentNode Is Nothing Then
                if (this.ParentNode != null)
                {
                    XmlNode objParent = this.XmlNode;
                    int intLevel = -1;
                    while (objParent != null && (objParent) is XmlElement)
                    {
                        intLevel++;
                        objParent = objParent.ParentNode;
                        if (objParent != null && objParent.Name == "root")
                        {
                            break;
                        }
                    }
                    return intLevel;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Property to access attribute collection for the node
        /// </summary>
        /// <param name="Key">Attribute Name</param>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string CustomAttribute(string Key)
        {
            if (!Convert.ToBoolean(XmlNode.Attributes.GetNamedItem(Key) == null))
            {
                return XmlNode.Attributes.GetNamedItem(Key).Value;
            }
            else
            {
                return null;
            }
        }

        public void SetCustomAttribute(string Key, string Value)
        {
            try
            {
                if (!Convert.ToBoolean(XmlNode.Attributes.GetNamedItem(Key) == null))
                {
                    if (Value == null)
                    {
                        XmlNode.Attributes.Remove((XmlAttribute)XmlNode.Attributes.GetNamedItem(Key));
                    }
                    else
                    {
                        XmlNode.Attributes.GetNamedItem(Key).Value = Value;
                    }
                }
                else if (Value != null)
                {
                    XmlAttribute objAttr = XMLDoc.CreateAttribute(Key);
                    objAttr.Value = Value;
                    XmlNode.Attributes.Append(objAttr);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        /// <summary>
        /// Property to access attribute collection for the node.  If property is not set,
        /// the passed in default value will be returned
        /// </summary>
        /// <param name="Key">Attribute Name</param>
        /// <param name="DefaultValue">Value to return when attribute not set</param>
        /// <value></value>
        /// <remarks>
        /// Until the node is in a hierarchy, thus allowing the creation of an XmlNode,
        /// the attributes will be stored in a hashtable.  Once the node is associated
        /// with a hierarchy, these attributes will be transferred to the XmlNode
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string CustomAttribute(string Key, string DefaultValue)
        {
            string value = CustomAttribute(Key);
            if ((value == null) || (value.Length == 0))
            {
                return DefaultValue;
            }
            else
            {
                return CustomAttribute(Key);
            }
        }

        public void SetCustomAttribute(string Key, string DefaultValue, string Value)
        {
            if (Value == DefaultValue)
            {
                Value = "";
            }
            SetCustomAttribute(Key, Value);
        }

        /// <summary>
        /// ID of Node
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string ID
        {
            get
            {
                return CustomAttribute("id");
            }
            set
            {
                SetCustomAttribute("id", value);
            }
        }

        /// <summary>
        /// ClientID of Node
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// Since the client browsers do not like :, we are replacing with _
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string ClientID
        {
            get
            {
                return this.ID.Replace(":", "_");
            }
        }

        /// <summary>
        /// Key to identify the node
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string Key
        {
            get
            {
                return CustomAttribute("key");
            }
            set
            {
                SetCustomAttribute("key", value);
            }
        }

        /// <summary>
        /// Text for the node to display
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string Text
        {
            get
            {
                return CustomAttribute("txt");
            }
            set
            {
                SetCustomAttribute("txt", value);
            }
        }

        /// <summary>
        /// URL to navigate to when node is selected
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        ///		[Jon Henning]	11/14/2005  When assigning a NavigateUrl, set clickaction to navigate to maintain backwards compat
        /// </history>
        public string NavigateURL
        {
            get
            {
                return CustomAttribute("url");
            }
            set
            {
                if (value.Length > 0)
                {
                    this.ClickAction = eClickAction.Navigate;
                    SetCustomAttribute("url", value);
                }
                else
                {
                    SetCustomAttribute("url", null); //don't render attribute
                }
            }
        }

        /// <summary>
        /// Function to execute when node is selected
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string JSFunction
        {
            get
            {
                return CustomAttribute("js");
            }
            set
            {
                SetCustomAttribute("js", value);
            }
        }

        /// <summary>
        /// Target frame to do the navigation
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string Target
        {
            get
            {
                return CustomAttribute("tar");
            }
            set
            {
                SetCustomAttribute("tar", value);
            }
        }

        /// <summary>
        /// ToolTip for the node to display
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string ToolTip
        {
            get
            {
                return CustomAttribute("tTip");
            }
            set
            {
                if (value.Length == 0)
                {
                    SetCustomAttribute("tTip", null); //don't render attribute
                }
                else
                {
                    SetCustomAttribute("tTip", value);
                }
            }
        }

        /// <summary>
        /// Flag to determine if node is enabled
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public bool Enabled
        {
            get
            {
                return Convert.ToBoolean(CustomAttribute("enabled", "true"));
            }
            set
            {
                SetCustomAttribute("enabled", (value ? "true" : "false").ToString());
            }
        }

        /// <summary>
        /// CSS Class Name of node
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string CSSClass
        {
            get
            {
                return CustomAttribute("css");
            }
            set
            {
                SetCustomAttribute("css", value);
            }
        }

        /// <summary>
        /// CSS Class Name of node when selected
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string CSSClassSelected
        {
            get
            {
                return CustomAttribute("cssSel");
            }
            set
            {
                SetCustomAttribute("cssSel", value);
            }
        }

        /// <summary>
        /// CSS Class Name of node when hovered
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string CSSClassHover
        {
            get
            {
                return CustomAttribute("cssHover");
            }
            set
            {
                SetCustomAttribute("cssHover", value);
            }
        }

        /// <summary>
        /// CSS Class Name of Icon
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string CSSIcon
        {
            get
            {
                return CustomAttribute("cssIcon");
            }
            set
            {
                SetCustomAttribute("cssIcon", value);
            }
        }

        /// <summary>
        /// image for the node to display
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	4/7/2005	Created
        /// </history>
        public string Image
        {
            get
            {
                return CustomAttribute("img");
            }
            set
            {
                SetCustomAttribute("img", value);
            }
        }

        /// <summary>
        /// Determines if node is selected
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	4/7/2005	Created
        /// </history>
        public bool Selected
        {
            get
            {
                return Convert.ToBoolean(this.CustomAttribute("selected", "false"));
            }
            set
            {
                this.SetCustomAttribute("selected", (value ? "true" : "false").ToString());
            }
        }

        /// <summary>
        /// Determines if node is part of the breadcrumb
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	4/7/2005	Created
        /// </history>
        public bool BreadCrumb
        {
            get
            {
                return Convert.ToBoolean(this.CustomAttribute("bcrumb", "false"));
            }
            set
            {
                this.SetCustomAttribute("bcrumb", (value ? "true" : "false").ToString());
            }
        }

        public eClickAction ClickAction
        {
            get
            {
                if (this.CustomAttribute("ca").Length > 0)
                {
                    eClickAction eAction = (eClickAction)Convert.ToInt32(this.CustomAttribute("ca"));
                    return (eAction);
                }
                else
                {
                    return eClickAction.PostBack;
                }
            }
            set
            {
                // HACK : The 'get' for this property was erroring because
                // it could not convert a string to an int as it was in 
                // the original code.  Cast the value to an int and then 
                // to a string to be saved so it will work on the way out.
                //this.SetCustomAttribute( "ca", value.ToString() );
                this.SetCustomAttribute("ca", ((int)value).ToString());
            }
        }

        public bool IsBreak
        {
            get
            {
                return Convert.ToBoolean(this.CustomAttribute("break", "false"));
            }
            set
            {
                this.SetCustomAttribute("break", (value ? "true" : "false").ToString());
            }
        }

        /// <summary>
        /// Retrieves Xml for the node
        /// </summary>
        /// <returns>Xml of current node and all its children</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        public string ToXML()
        {
            return XmlNode.OuterXml;
        }

        public string ToJSON()
        {
            return ToJSON(true);
        }

        public string ToJSON(bool blnDeep)
        {
            //blnDeep not supported yet...

            StringBuilder sb = new StringBuilder();
            XmlAttribute oAttr;
            foreach (XmlAttribute tempLoopVar_oAttr in this.XmlNode.Attributes)
            {
                oAttr = tempLoopVar_oAttr;
                if (sb.Length == 0)
                {
                    sb.Append("{");
                }
                else
                {
                    sb.Append(",");
                }
                //xml contains abbreviations... but when we are using this on the client we want to use it as if it was a real DNNNode (DNNTreeNode) object so we want the long names
                switch (oAttr.Name)
                {
                    case "txt":

                        sb.Append("text");
                        break;
                    case "tar":

                        sb.Append("target");
                        break;
                    case "tTip":

                        sb.Append("toolTip");
                        break;
                    default:

                        sb.Append(oAttr.Name);
                        break;
                }
                sb.Append(":");
                sb.Append("\"" + ClientAPI.GetSafeJSString(oAttr.Value) + "\"");
            }
            sb.Append("};");
            return sb.ToString();
        }

        //Friend Sub AssociateXmlNode(ByVal objXmlNode As XmlNode)
        //	m_objXMLNode = objXmlNode
        //	m_objXMLDoc = objXmlNode.OwnerDocument

        //	'copy all the values from the cache over
        //	Dim strKey As String
        //	If Not m_objHashAttributes Is Nothing Then
        //		For Each strKey In m_objHashAttributes.Keys
        //			CustomAttribute(strKey) = m_objHashAttributes(strKey)
        //		Next
        //	End If
        //End Sub

        /// <summary>
        /// Since an XmlNode cannot exist without a document, and since the node's
        /// interface allows the creation of a node without a document, we need
        /// to expose a way for the node to be added to a document.
        /// </summary>
        /// <param name="objXmlNode">XmlNode to associate with the DNNNode object</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	12/22/2004	Created
        /// </history>
        internal void AssociateXmlNode(XmlNode objXmlNode)
        {
            m_objXMLNode = objXmlNode;
            m_objXMLDoc = objXmlNode.OwnerDocument;
        }

        public DNNNode Clone()
        {
            return Clone(true);
        }

        public DNNNode Clone(bool blnDeep)
        {
            XmlNode objXmlNode = this.XmlNode.CloneNode(blnDeep);
            return new DNNNode(objXmlNode);
        }
    }
}