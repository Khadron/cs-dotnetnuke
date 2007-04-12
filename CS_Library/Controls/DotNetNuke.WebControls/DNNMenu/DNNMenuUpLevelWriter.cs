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
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.UI.Utilities;
using System.Collections;

namespace DotNetNuke.UI.WebControls
{

	internal class DNNMenuUpLevelWriter : WebControl, IDNNMenuWriter
	{
		private DNNMenu m_objMenu;

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public DNNMenuUpLevelWriter()
		{
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="Menu"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public void RenderMenu(HtmlTextWriter writer, DNNMenu Menu)
		{

			m_objMenu = Menu;
			RenderControl(writer);
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		///		[jhenning] 2/22/2005	Added properties
		/// </history>
		/// -----------------------------------------------------------------------------
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_objMenu.CssClass);
			writer.AddAttribute(HtmlTextWriterAttribute.Name, m_objMenu.UniqueID);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, m_objMenu.ClientID);

			writer.AddAttribute("orient", ((int)m_objMenu.Orientation).ToString());
			if (m_objMenu.SubMenuOrientation != Orientation.Vertical)
			{
				writer.AddAttribute("suborient", ((int)m_objMenu.SubMenuOrientation).ToString());
			}
			writer.AddAttribute("sysimgpath", m_objMenu.SystemImagesPath);
			if (!String.IsNullOrEmpty(m_objMenu.Target)) writer.AddAttribute("target", m_objMenu.Target); 

			//--- imagelist logic ---
			if (m_objMenu.ImageList.Count > 0)
			{
				SortedList objImagePaths = new SortedList();
				string strList = "";
				string strImagePathList = "";
				foreach (NodeImage objNodeImage in m_objMenu.ImageList) {
					if (objNodeImage.ImageUrl.IndexOf("/") > -1)
					{
						string strPath = objNodeImage.ImageUrl.Substring(0, objNodeImage.ImageUrl.LastIndexOf("/") + 1);
						string strImage = objNodeImage.ImageUrl.Substring(objNodeImage.ImageUrl.LastIndexOf("/") + 1);
						if (objImagePaths.ContainsValue(strPath) == false)
						{
							objImagePaths.Add(objImagePaths.Count, strPath);
						}
						objNodeImage.ImageUrl = string.Format("[{0}]{1}", objImagePaths.IndexOfValue(strPath).ToString(), strImage);
					}
                        strList += (String.IsNullOrEmpty(strList) ? "" : ",") + objNodeImage.ImageUrl;
				}
				for (int intPaths = 0; intPaths <= objImagePaths.Count - 1; intPaths++) 
                    {
                        strImagePathList += (String.IsNullOrEmpty(strImagePathList) ? "" : ",") + objImagePaths.GetByIndex(intPaths).ToString();
				}
				writer.AddAttribute("imagelist", strList);
				writer.AddAttribute("imagepaths", strImagePathList);
			}

			//--- urllist logic ---'
			//Dim objUsedTokens As ArrayList = New ArrayList
			//Me.AssignUrlTokens(m_objMenu.MenuNodes, Nothing, objUsedTokens)
			//If objUsedTokens.Count > 0 Then
			//	writer.AddAttribute("urllist", Join(objUsedTokens.ToArray(), ","))				  'comma safe?!?!?!
			//End If

			if (!String.IsNullOrEmpty(m_objMenu.RootArrowImage)) writer.AddAttribute("rarrowimg", m_objMenu.RootArrowImage); 
			if (!String.IsNullOrEmpty(m_objMenu.ChildArrowImage)) writer.AddAttribute("carrowimg", m_objMenu.ChildArrowImage); 
			if (!String.IsNullOrEmpty(m_objMenu.WorkImage)) writer.AddAttribute("workimg", m_objMenu.WorkImage); 

			//css attributes
			if (!String.IsNullOrEmpty(m_objMenu.DefaultNodeCssClass)) writer.AddAttribute("css", m_objMenu.DefaultNodeCssClass); 
			if (!String.IsNullOrEmpty(m_objMenu.DefaultChildNodeCssClass)) writer.AddAttribute("csschild", m_objMenu.DefaultChildNodeCssClass); 
			if (!String.IsNullOrEmpty(m_objMenu.DefaultNodeCssClassOver)) writer.AddAttribute("csshover", m_objMenu.DefaultNodeCssClassOver); 
			if (!String.IsNullOrEmpty(m_objMenu.DefaultNodeCssClassSelected)) writer.AddAttribute("csssel", m_objMenu.DefaultNodeCssClassSelected); 
			if (!String.IsNullOrEmpty(m_objMenu.MenuBarCssClass)) writer.AddAttribute("mbcss", m_objMenu.MenuBarCssClass); 
			if (!String.IsNullOrEmpty(m_objMenu.MenuCssClass)) writer.AddAttribute("mcss", m_objMenu.MenuCssClass); 
			if (!String.IsNullOrEmpty(m_objMenu.DefaultIconCssClass)) writer.AddAttribute("cssicon", m_objMenu.DefaultIconCssClass); 

			if (!String.IsNullOrEmpty(m_objMenu.JSFunction)) writer.AddAttribute("js", m_objMenu.JSFunction); 
			if (m_objMenu.UseTables == false) writer.AddAttribute("usetables", "0"); 
			if (m_objMenu.EnablePostbackState) writer.AddAttribute("enablepbstate", "1"); 
			if (m_objMenu.MouseOutDelay != 500) writer.AddAttribute("moutdelay", m_objMenu.MouseOutDelay.ToString()); 
			if (m_objMenu.MouseInDelay != 250) writer.AddAttribute("mindelay", m_objMenu.MouseInDelay.ToString()); 

			writer.AddAttribute("postback", ClientAPI.GetPostBackEventReference(m_objMenu, "[NODEID]" + ClientAPI.COLUMN_DELIMITER + "Click"));

			if (m_objMenu.PopulateNodesFromClient)
			{
				if (DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(Utilities.ClientAPI.ClientFunctionality.XMLHTTP))
				{
					writer.AddAttribute("callback", DotNetNuke.UI.Utilities.ClientAPI.GetCallbackEventReference(m_objMenu, "'[NODEXML]'", "this.callBackSuccess", "oMNode", "this.callBackFail", "this.callBackStatus"));
				}
				else
				{
					writer.AddAttribute("callback", ClientAPI.GetPostBackClientHyperlink(m_objMenu, "[NODEID]" + ClientAPI.COLUMN_DELIMITER + "OnDemand"));
				}
				if (!String.IsNullOrEmpty(m_objMenu.CallbackStatusFunction))
				{
					writer.AddAttribute("callbacksf", m_objMenu.CallbackStatusFunction);
				}

			}

			if (!String.IsNullOrEmpty(m_objMenu.JSFunction))
			{
				writer.AddAttribute("js", m_objMenu.JSFunction);
			}
			//writer.RenderBeginTag(HtmlTextWriterTag.P)			 '//SAFARI DOES NOT LIKE DIV TAG!!!
			writer.RenderBeginTag(HtmlTextWriterTag.Span);
			//TODO: TEST SAFARI!
			//RenderChildren(writer)	'no longer rendering children for uplevel, only sending down xml and client is responsible
			writer.RenderEndTag();
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[jbrinkman]	5/6/2004	Created
		/// </history>
		/// -----------------------------------------------------------------------------
		protected override void RenderChildren(HtmlTextWriter writer)
		{
			foreach (MenuNode TempNode in m_objMenu.MenuNodes) {
				TempNode.Render(writer);
			}
		}

		private void AssignUrlTokens(MenuNodeCollection objNodes, ref Hashtable objTokens, ref ArrayList objUsedTokens)
		{
			string strLastToken;
			if (objTokens == null)
			{
				GetUrlTokens(objNodes, ref objTokens);
			}
			foreach (MenuNode objNode in objNodes) {
				//look all nodes
				strLastToken = "";
				foreach (string strToken in ((Hashtable)objTokens.Clone()).Keys) {
					//loop all tokens (have to clone so we can modify real collection
					if (!String.IsNullOrEmpty(objNode.NavigateURL) && objNode.NavigateURL.IndexOf(strToken) > -1)
					{
						//if url contains token
						objTokens[strToken] = (int)objTokens[strToken] - 1;
						//remove token from count
						if (strToken.Length > strLastToken.Length && ((int)objTokens[strToken] > 0 || objUsedTokens.Contains(strToken)))
						{
							//if token is better and not only one with match
							strLastToken = strToken;
							//use it
						}
					}
				}
				if (!String.IsNullOrEmpty(strLastToken))
				{
					if (objUsedTokens.Contains(strLastToken) == false)
					{
						objUsedTokens.Add(strLastToken);
					}
					objNode.UrlIndex = objUsedTokens.IndexOf(strLastToken);
					objNode.NavigateURL = objNode.NavigateURL.Substring(strLastToken.Length);
				}
				AssignUrlTokens(objNode.MenuNodes, ref objTokens, ref objUsedTokens);
			}
		}

		private void GetUrlTokens(MenuNodeCollection objNodes, ref Hashtable objTokens)
		{
			if (objTokens == null) objTokens = new Hashtable(); 
			foreach (MenuNode objNode in objNodes) {
				if (!String.IsNullOrEmpty(objNode.NavigateURL)) AddUrlTokens(objNode.NavigateURL, ref objTokens); 
				GetUrlTokens(objNode.MenuNodes, ref objTokens);
			}
		}

		private void AddUrlTokens(string strUrl, ref Hashtable objTokens)
		{
			string strToken = "";
			foreach (string strPart in strUrl.Split('/')) {
				if (strToken.Length + 1 + strPart.Length < strUrl.Length)
				{
					//determine if we can append /
                        strToken += strPart + "/";
				}
				else
				{
                        strToken += strPart;
				}
				if (objTokens.ContainsKey(strToken) == false)
				{
					objTokens.Add(strToken, 1);
				}
				else
				{
					objTokens[strToken] = (int)objTokens[strToken] + 1;
				}
			}
		}

	}
}
