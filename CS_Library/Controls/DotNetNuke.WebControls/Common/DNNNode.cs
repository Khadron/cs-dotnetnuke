//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2005
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
//

using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Collections;

namespace DotNetNuke.UI.WebControls
{
	public class DNNNode
	{

		#region "Member Variables"
		private XmlDocument m_objXMLDoc;
		private XmlNode m_objXMLNode;
		private string m_strParentNS;
		private DNNNodeCollection m_objNodes;
		private Hashtable m_objHashAttributes;
		#endregion

		#region "Constructors"
		/// -----------------------------------------------------------------------------
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
		/// -----------------------------------------------------------------------------
		public DNNNode() : this(new XmlDocument().CreateNode(XmlNodeType.Element, "n", ""))
		{
		}

		/// -----------------------------------------------------------------------------
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
		/// -----------------------------------------------------------------------------
		public DNNNode(string strText) : this()
		{
			this.Text = strText;
		}

		public DNNNode(string NodeText, string navigateUrl)
		{
			if (NodeText == null || navigateUrl == null)
			{
				throw new ArgumentNullException();
			}
			this.Text = NodeText;
			this.NavigateURL = navigateUrl;
		}

		/// -----------------------------------------------------------------------------
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
		/// -----------------------------------------------------------------------------
		public DNNNode(XmlNode objXmlNode)
		{
			m_objXMLNode = objXmlNode;
			m_objXMLDoc = objXmlNode.OwnerDocument;
		}

		#endregion

		#region "Node Properties"
		/// -----------------------------------------------------------------------------
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
		/// -----------------------------------------------------------------------------
		public bool IsInHierarchy {
          //Return Not XmlNode Is Nothing
			get { return (XmlNode.ParentNode != null); }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Exposes the XmlDocument that the OwnerDocument of the XmlNode the class is wrapping
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		internal XmlDocument XMLDoc {
			get { return m_objXMLDoc; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Exposes the XmlNode the node is wrapping
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		internal XmlNode XmlNode {
			get { return m_objXMLNode; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Exposes the root XmlNode
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		private XmlNode RootNode {
			get { return XMLDoc.ChildNodes[0]; }
		}

		/// -----------------------------------------------------------------------------
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
		/// -----------------------------------------------------------------------------
		public DNNNode ParentNode {
			get {
				if ((this.XmlNode.ParentNode != null) && (this.XmlNode.ParentNode.NodeType != System.Xml.XmlNodeType.Document))
				{
					return new DNNNode(this.XmlNode.ParentNode);
				}
				else
				{
					return null;
				}
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Returns a collection of the children DNNNodes
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public DNNNodeCollection DNNNodes {
			get {
				if (m_objNodes == null)
				{
					m_objNodes = new DNNNodeCollection(this.XmlNode);
				}
				return m_objNodes;
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Returns a colleciton of the children XmlNodes
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public XmlNodeList XmlNodes {
			get { return this.XmlNode.ChildNodes; }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Returns a flag indicating whether the node has children
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public bool HasNodes {
			get {
                    Boolean blnHas = this.CustomAttribute("hasNodes", "0") == "1" ? true : false;
				if (blnHas == false)
				{
					return this.DNNNodes.Count > 0;
				}
				else
				{
					return blnHas;
					//False
				}
			}              
			set 
               {
                   string strValue = value == true ? "1" : "0";
                   this.SetCustomAttribute("hasNodes", strValue); 
               }
		}

		/// -----------------------------------------------------------------------------
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
		/// -----------------------------------------------------------------------------
		public string ParentNameSpace {
			get {
				if (String.IsNullOrEmpty(m_strParentNS))
				{
					m_strParentNS = "";
					if ((XmlNode.ParentNode != null) && (XmlNode.ParentNode) is XmlElement)
					{
						m_strParentNS = XmlNode.ParentNode.Attributes.GetNamedItem("id").Value;
					}
				}
				return m_strParentNS;
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Returns the level (depth) of the node within the hierarchy
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public int Level {
			get {
				//If IsInHierarchy AndAlso Not Me.ParentNode Is Nothing Then
				if ((this.ParentNode != null))
				{
					XmlNode objParent = this.XmlNode;
					int intLevel = -1;
					while ((objParent != null) && (objParent) is XmlElement) {
						intLevel += 1;
						objParent = objParent.ParentNode;
						if ((objParent != null) && objParent.Name == "root") break; // TODO: might not be correct. Was : Exit While
 
					}
					return intLevel;
				}
				else
				{

					return -1;
				}
			}
		}

		/// -----------------------------------------------------------------------------
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
		/// -----------------------------------------------------------------------------
	    // HACK : Big time re-work.  The original VB was a property that took a prameter and
	    // could be set to a value, there is noting in C# that works that way.  All the 
         // code that call the VB property needs to be change also to call the new method.
         public string CustomAttribute(string key) 
          {
              if ((XmlNode.Attributes.GetNamedItem(key) != null))
			{
                   return XmlNode.Attributes.GetNamedItem(key).Value;
			}
			else
			{
				return null;
			}
		}
		public void SetCustomAttribute(string key, string value)
          {
			try {
                   if ((XmlNode.Attributes.GetNamedItem(key) != null))
				{
					if (value == null)
					{
                             XmlNode.Attributes.Remove((XmlAttribute)XmlNode.Attributes.GetNamedItem(key));
					}
					else
					{
                             XmlNode.Attributes.GetNamedItem(key).Value = value;
					}
				}
                     else if ((value != null)) 
                     {
                         System.Xml.XmlAttribute objAttr = XMLDoc.CreateAttribute(key);
					objAttr.Value = value;
					XmlNode.Attributes.Append(objAttr);
				}
			}
			catch (Exception ex) {
				throw ex;
			}
		}

		/// -----------------------------------------------------------------------------
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
		/// -----------------------------------------------------------------------------
		public string CustomAttribute(string key, string defaultValue)  
          {
              if (String.IsNullOrEmpty(CustomAttribute(key)))
			{
				return defaultValue;
			}
			else
			{
                   return CustomAttribute(key);
			}

		}
         public void SetCustomAttribute(string key, string defaultValue, string value)
         {
			if (value == defaultValue) value = ""; 
			SetCustomAttribute(key, value);
		}

		#endregion

		#region "Base Attributes"

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// ID of Node
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string ID {
			get { return (string)CustomAttribute("id"); }
			set { SetCustomAttribute("id", value); }
		}

		/// -----------------------------------------------------------------------------
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
		/// -----------------------------------------------------------------------------
		public string ClientID {
			get { return this.ID.Replace(":", "_"); }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Key to identify the node
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string Key {
			get { return CustomAttribute("key"); }
			set { SetCustomAttribute("key", value); }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Text for the node to display
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string Text {
			get { return CustomAttribute("txt"); }
			set { SetCustomAttribute("txt", value); }
		}

		/// -----------------------------------------------------------------------------
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
		/// -----------------------------------------------------------------------------
		public string NavigateURL {
			get { return CustomAttribute("url"); }
			set {
				if (!String.IsNullOrEmpty(value))
				{
					this.ClickAction = eClickAction.Navigate;
					this.SetCustomAttribute("url", value);
				}
				else
				{
					this.SetCustomAttribute("url", null);
					//don't render attribute
				}
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Function to execute when node is selected
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string JSFunction {
			get { return CustomAttribute("js"); }
			set { SetCustomAttribute("js", value); }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Target frame to do the navigation
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string Target {
			get { return CustomAttribute("tar"); }
			set { SetCustomAttribute("tar", value); }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// ToolTip for the node to display
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string ToolTip {
			get { return CustomAttribute("tTip"); }
			set {
				if (String.IsNullOrEmpty(value))
				{
					SetCustomAttribute("tTip", null);
					//don't render attribute
				}
				else
				{
					SetCustomAttribute("tTip", value);
				}
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Flag to determine if node is enabled
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public bool Enabled {
               get 
               { 
                   return this.CustomAttribute("enabled", "1") == "1" ? true : false; }
			set 
               {
                   string strValue = value == true ? "1" : "0";
                   this.SetCustomAttribute("enabled", strValue); 
               }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// CSS Class Name of node
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string CSSClass {
			get { return CustomAttribute("css"); }
			set { SetCustomAttribute("css", value); }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// CSS Class Name of node when selected
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string CSSClassSelected {
			get { return CustomAttribute("cssSel"); }
			set { SetCustomAttribute("cssSel", value); }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// CSS Class Name of node when hovered
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string CSSClassHover {
			get { return CustomAttribute("cssHover"); }
			set { SetCustomAttribute("cssHover", value); }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// CSS Class Name of Icon
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string CSSIcon {
			get { return CustomAttribute("cssIcon"); }
			set { SetCustomAttribute("cssIcon", value); }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// image for the node to display
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	4/7/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public string Image {
			get { return CustomAttribute("img"); }
			set { SetCustomAttribute("img", value); }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Determines if node is selected
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	4/7/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public bool Selected {
			get 
               { 
                   return this.CustomAttribute("selected", "0") == "1" ? true : false; }
			set 
               {
                   string strValue = value == true ? "1" : "0";
                   this.SetCustomAttribute("selected", strValue); 
               }
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Determines if node is part of the breadcrumb
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	4/7/2005	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public bool BreadCrumb {
              get 
               { 
                   return this.CustomAttribute("bcrumb", "0") == "1" ? true : false; }
			set 
               {
                   string strValue = value == true ? "1" : "0";
                   this.SetCustomAttribute("bcrumb", strValue); 
               }
		}

		public eClickAction ClickAction 
          {
			get 
               {
                   if (!String.IsNullOrEmpty(this.CustomAttribute("ca")))
				{
                        eClickAction eAction = (eClickAction)Convert.ToInt32(this.CustomAttribute("ca"));
				    return (eAction);
				}
				else
				{
				    return eClickAction.PostBack;
				}
			}
			set { this.SetCustomAttribute("ca", ((int)value).ToString()); }
		}

		public bool IsBreak {
              get 
               { 
                   return this.CustomAttribute("break", "0") == "1" ? true : false; }
			set 
               {
                   string strValue = value == true ? "1" : "0";
                   this.SetCustomAttribute("break", strValue); 
               }
		}

		#endregion

		#region "Methods"

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Retrieves Xml for the node
		/// </summary>
		/// <returns>Xml of current node and all its children</returns>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
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

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (XmlAttribute oAttr in this.XmlNode.Attributes) {
				if (sb.Length == 0)
				{
					sb.Append("{");
				}
				else
				{
					sb.Append(",");
				}
				//xml contains abbreviations... but when we are using this on the client we want to use it as if it was a real DNNNode (DNNTreeNode) object so we want the long names
				switch (oAttr.Name) {
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
				sb.Append("\"" + DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(oAttr.Value) + "\"");
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

		/// -----------------------------------------------------------------------------
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
		/// -----------------------------------------------------------------------------
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

		#endregion

	}
}
