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
using System.Xml;
using DotNetNuke.UI.WebControls;
using System.Web.UI;
using System.Collections;
using DotNetNuke.UI.Utilities;
using System;

namespace DotNetNuke.UI.WebControls
{
	public class MenuNodeCollection : DNNNodeCollection
	{

		#region "Member Variables"
		private DNNMenu m_objDNNMenu;
		//Private m_strNamespace As String
		#endregion

		#region "Constructors"
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Constructor to call when creating a Root Node
		/// </summary>
		/// <param name="strNamespace">Namespace of node hierarchy</param>
		/// <param name="objControl">DnnMenu control associated to MenuNodeCollection</param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public MenuNodeCollection(string strNamespace, DNNMenu objControl) : base(strNamespace)
		{
			m_objDNNMenu = objControl;
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Loads node collection based off of XML string
		/// </summary>
		/// <param name="strXML">XML String</param>
		/// <param name="strXSLFile">XSL FileName.  Leave empty if no transform needed</param>
		/// <param name="objControl">DNN Menu control</param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public MenuNodeCollection(string strXML, string strXSLFile, DNNMenu objControl) : base(strXML, strXSLFile)
		{
			m_objDNNMenu = objControl;
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Constructor for all nodes that are not the root.  
		/// </summary>
		/// <param name="objXmlNode">Node whose children will be exposed by this class</param>
		/// <param name="objControl">DNN Menu control</param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[Jon Henning]	12/22/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public MenuNodeCollection(XmlNode objXmlNode, DNNMenu objControl) : base(objXmlNode)
		{
			//, ByVal objControl As Control)
			m_objDNNMenu = objControl;
		}

		#endregion

		//MenuNode
		public int Add()
		{
			return this.Add("");
		}

		// MenuNode
		public int Add(string strText)
		{
			MenuNode objNode = new MenuNode();
			int intIndex = this.Add(objNode);
			objNode.Text = strText;
			return intIndex;
		}

		// MenuNode
		public int Add(MenuNode objNode)
		{
			int intIndex = base.Add(objNode);
			objNode.SetDNNMenu(m_objDNNMenu);
			//ChildNodes.count is not guaranteed to be unique... GetHashCode should be a little safer (I assume it is medium trust safe...
			if (String.IsNullOrEmpty(objNode.ID)) objNode.ID = objNode.ParentNameSpace + "_" + objNode.GetHashCode(); 
			//Me.XMLNode.ChildNodes.Count
			return intIndex;
		}

		public int Add(string strID, string strKey, string strText, string strNavigateURL)
		{
			return Add(strID, strKey, strText, strNavigateURL, "", "", "", true, "", "", 
			"", false, eClickAction.Navigate, "", -1);
		}

		public int Add(string strID, string strKey, string strText, string strNavigateURL, string strJSFunction, string strTarget, string strToolTip, bool blnEnabled, string strCSSClass, string strCSSClassSelected, 
		// MenuNode
		string strCSSClassHover, bool blnSelected, eClickAction enumClickAction, string strCssClassOver, int intImageIndex)
		{

			int intIndex = this.Add();
			MenuNode objNode = this[intIndex];

			if (!String.IsNullOrEmpty(strID))
			{
				objNode.ID = strID;
			}
			else
			{
				objNode.ID = objNode.ParentNameSpace + "_" + this.XMLNode.ChildNodes.Count;
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
			objNode.Selected = blnSelected;
			objNode.ClickAction = enumClickAction;
			//objNode.CheckBox = blnCheckBox
			objNode.CssClassOver = strCssClassOver;
			objNode.ImageIndex = intImageIndex;

			return intIndex;

		}

		public new MenuNode this[int index] {
			get { return new MenuNode(this.XMLNode.ChildNodes[index], m_objDNNMenu); }
              set { throw new Exception("Cannot Assign Node Directly"); }
		}
         //public new MenuNode this[string index]
         //{
         //    get 
         //    {
         //        int idx;
         //        if (int.TryParse(index, out idx))
         //        {
         //            return this[idx];
         //        }
         //        return null;
         //    }
         //    set { throw new Exception("Cannot Assign Node Directly"); }
         //}

		public int IndexOf(MenuNode value)
		{
			int i;
			for (i = 0; i <= this.XMLNode.ChildNodes.Count; i++) {
				if (new MenuNode(this.XMLNode.ChildNodes[i], m_objDNNMenu).ID == value.ID)
				{
					return i;
				}
			}
               return -1;
		}

		public void Insert(int index, MenuNode value)
		{
			this.XMLNode.InsertAfter(this.XMLNode.ChildNodes[index], value.XmlNode);
		}

		public void Remove(MenuNode value)
		{
			this.XMLNode.RemoveChild(value.XmlNode);
		}

		//Public Sub Clear()
		//	Dim i As Integer
		//	For i = Me.XMLNode.ChildNodes.Count - 1 To 0 Step -1
		//		Me.XMLNode.RemoveChild(Me.XMLNode.ChildNodes[i])
		//	Next
		//End Sub

		public bool Contains(MenuNode value)
		{
			if (this.FindNode(value.ID) == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public new MenuNode FindNode(string ID)
		{
			XmlNode objNode = FindFast("id", ID, this.XMLNode, true);
			if ((objNode != null)) return new MenuNode(objNode, m_objDNNMenu); 
			return null;
		}

		public new MenuNode FindNodeByKey(string Key)
		{
			XmlNode objNode = FindFast("key", Key, this.XMLNode, true);
			if ((objNode != null)) return new MenuNode(objNode, m_objDNNMenu); 
			return null;
		}

		public override ArrayList FindSelectedNodes()
		{
              ArrayList colMenuNodes = new ArrayList();
			//MenuNodeCollection = New MenuNodeCollection(m_strNamespace, m_objDNNMenu)
			if ((this.XMLNode != null))
			{
				XmlNodeList objNodeList = this.XMLNode.SelectNodes("//n[@selected='1']");
				foreach (XmlNode objNode in objNodeList) {
					colMenuNodes.Add(new MenuNode(objNode, m_objDNNMenu));
				}
			}
			return colMenuNodes;
		}


		//Public Function ToXml() As String
		//	Return Me.XMLDoc.OuterXml
		//End Function

		#region "ICollection Implementation"
		public IEnumerator GetEnumerator()
		{
			return new MenuNodeEnumerator(this.XMLNode, m_objDNNMenu);
		}

		#endregion

	}

	class MenuNodeEnumerator : IEnumerator
	{

		private XmlNode m_objXMLNode;
		private DNNMenu m_objDNNMenu;
		private int m_intCursor;

		public MenuNodeEnumerator(XmlNode objRoot, DNNMenu objControl)
		{
			m_objXMLNode = objRoot;
			m_objDNNMenu = objControl;
			m_intCursor = -1;
		}

		public void Reset()
		{
			m_intCursor = -1;
		}

		public bool MoveNext()
		{
			if (m_intCursor < m_objXMLNode.ChildNodes.Count)
			{
				m_intCursor = m_intCursor + 1;
			}

			if ((m_intCursor == m_objXMLNode.ChildNodes.Count))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public object Current {
			get {
				if (((m_intCursor < 0) | (m_intCursor == m_objXMLNode.ChildNodes.Count)))
				{
					throw new InvalidOperationException();
				}
				else
				{
					return new MenuNode(m_objXMLNode.ChildNodes[m_intCursor], m_objDNNMenu);
				}
			}
		}
	}
}
