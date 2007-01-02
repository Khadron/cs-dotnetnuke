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
using System.Web.UI.WebControls;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.UI.WebControls
{
    internal class DNNMenuUpLevelWriter : WebControl, IDNNMenuWriter
    {
        private DNNMenu m_objMenu;

        public void RenderMenu( HtmlTextWriter writer, DNNMenu Menu )
        {
            m_objMenu = Menu;
            RenderControl( writer );
        }

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
        protected override void RenderContents( HtmlTextWriter writer )
        {
            writer.AddAttribute( HtmlTextWriterAttribute.Width, "100%" );
            writer.AddAttribute( HtmlTextWriterAttribute.Class, m_objMenu.CssClass );
            writer.AddAttribute( HtmlTextWriterAttribute.Name, m_objMenu.UniqueID );
            writer.AddAttribute( HtmlTextWriterAttribute.Id, m_objMenu.ClientID );

            writer.AddAttribute( "orient", Convert.ToInt32( m_objMenu.Orientation ).ToString() );
            writer.AddAttribute( "sysimgpath", m_objMenu.SystemImagesPath );
            if( m_objMenu.Target.Length > 0 )
            {
                writer.AddAttribute( "target", m_objMenu.Target );
            }

            //--- imagelist logic ---
            if( m_objMenu.ImageList.Count > 0 )
            {
                SortedList objImagePaths = new SortedList();
                string strList = "";
                string strImagePathList = "";
                NodeImage objNodeImage;
                foreach( NodeImage tempLoopVar_objNodeImage in m_objMenu.ImageList )
                {
                    objNodeImage = tempLoopVar_objNodeImage;
                    if( objNodeImage.ImageUrl.IndexOf( "/" ) > -1 )
                    {
                        string strPath = objNodeImage.ImageUrl.Substring( 0, objNodeImage.ImageUrl.LastIndexOf( "/" ) + 1 );
                        string strImage = objNodeImage.ImageUrl.Substring( objNodeImage.ImageUrl.LastIndexOf( "/" ) + 1 );
                        if( objImagePaths.ContainsValue( strPath ) == false )
                        {
                            objImagePaths.Add( objImagePaths.Count, strPath );
                        }
                        objNodeImage.ImageUrl = string.Format( "[{0}]{1}", objImagePaths.IndexOfValue( strPath ).ToString(), strImage );
                    }
                    strList += ( ( strList.Length > 0 ) ? "," : "" ).ToString() + objNodeImage.ImageUrl;
                }
                for( int intPaths = 0; intPaths <= objImagePaths.Count - 1; intPaths++ )
                {
                    strImagePathList += ( ( strImagePathList.Length > 0 ) ? "," : "" ).ToString() + objImagePaths.GetByIndex( intPaths ).ToString();
                }
                writer.AddAttribute( "imagelist", strList );
                writer.AddAttribute( "imagepaths", strImagePathList );
            }

            //--- urllist logic ---'
            //Dim objUsedTokens As ArrayList = New ArrayList
            //Me.AssignUrlTokens(m_objMenu.MenuNodes, Nothing, objUsedTokens)
            //If objUsedTokens.Count > 0 Then
            //	writer.AddAttribute("urllist", Join(objUsedTokens.ToArray(), ","))				  'comma safe?!?!?!
            //End If

            if( m_objMenu.RootArrowImage.Length > 0 )
            {
                writer.AddAttribute( "rarrowimg", m_objMenu.RootArrowImage );
            }
            if( m_objMenu.ChildArrowImage.Length > 0 )
            {
                writer.AddAttribute( "carrowimg", m_objMenu.ChildArrowImage );
            }
            if( m_objMenu.WorkImage.Length > 0 )
            {
                writer.AddAttribute( "workimg", m_objMenu.WorkImage );
            }

            //css attributes
            if( m_objMenu.DefaultNodeCssClass.Length > 0 )
            {
                writer.AddAttribute( "css", m_objMenu.DefaultNodeCssClass );
            }
            if( m_objMenu.DefaultChildNodeCssClass.Length > 0 )
            {
                writer.AddAttribute( "csschild", m_objMenu.DefaultChildNodeCssClass );
            }
            if( m_objMenu.DefaultNodeCssClassOver.Length > 0 )
            {
                writer.AddAttribute( "csshover", m_objMenu.DefaultNodeCssClassOver );
            }
            if( m_objMenu.DefaultNodeCssClassSelected.Length > 0 )
            {
                writer.AddAttribute( "csssel", m_objMenu.DefaultNodeCssClassSelected );
            }
            if( m_objMenu.MenuBarCssClass.Length > 0 )
            {
                writer.AddAttribute( "mbcss", m_objMenu.MenuBarCssClass );
            }
            if( m_objMenu.MenuCssClass.Length > 0 )
            {
                writer.AddAttribute( "mcss", m_objMenu.MenuCssClass );
            }
            if( m_objMenu.DefaultIconCssClass.Length > 0 )
            {
                writer.AddAttribute( "cssicon", m_objMenu.DefaultIconCssClass );
            }

            if( m_objMenu.JSFunction.Length > 0 )
            {
                writer.AddAttribute( "js", m_objMenu.JSFunction );
            }
            if( m_objMenu.UseTables == false )
            {
                writer.AddAttribute( "usetables", "0" );
            }
            if( m_objMenu.EnablePostbackState )
            {
                writer.AddAttribute( "enablepbstate", "1" );
            }
            if( m_objMenu.MouseOutDelay != 1000 )
            {
                writer.AddAttribute( "moutdelay", m_objMenu.MouseOutDelay.ToString() );
            }
            if( m_objMenu.MouseInDelay != 250 )
            {
                writer.AddAttribute( "mindelay", m_objMenu.MouseInDelay.ToString() );
            }

            writer.AddAttribute( "postback", ClientAPI.GetPostBackEventReference( m_objMenu, "[NODEID]" + ClientAPI.COLUMN_DELIMITER + "Click" ) );

            if( m_objMenu.PopulateNodesFromClient )
            {
                if( ClientAPI.BrowserSupportsFunctionality( ClientAPI.ClientFunctionality.XMLHTTP ) )
                {
                    writer.AddAttribute( "callback", ClientAPI.GetCallbackEventReference( m_objMenu, "'[NODEXML]'", "this.callBackSuccess", "oMNode", "this.callBackFail", "this.callBackStatus" ) );
                }
                else
                {
                    writer.AddAttribute( "callback", ClientAPI.GetPostBackClientHyperlink( m_objMenu, "[NODEID]" + ClientAPI.COLUMN_DELIMITER + "OnDemand" ) );
                }
                if( m_objMenu.CallbackStatusFunction.Length > 0 )
                {
                    writer.AddAttribute( "callbacksf", m_objMenu.CallbackStatusFunction );
                }
            }

            if( m_objMenu.JSFunction.Length > 0 )
            {
                writer.AddAttribute( "js", m_objMenu.JSFunction );
            }
            //writer.RenderBeginTag(HtmlTextWriterTag.P)			 '//SAFARI DOES NOT LIKE DIV TAG!!!
            writer.RenderBeginTag( HtmlTextWriterTag.Span ); //TODO: TEST SAFARI!
            //RenderChildren(writer)	'no longer rendering children for uplevel, only sending down xml and client is responsible
            writer.RenderEndTag();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="writer"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jbrinkman]	5/6/2004	Created
        /// </history>
        protected override void RenderChildren( HtmlTextWriter writer )
        {
            MenuNode TempNode;
            foreach( MenuNode tempLoopVar_TempNode in m_objMenu.MenuNodes )
            {
                TempNode = tempLoopVar_TempNode;
                TempNode.Render( writer );
            }
        }

        private void AssignUrlTokens( MenuNodeCollection objNodes, ref Hashtable objTokens, ref ArrayList objUsedTokens )
        {
            string strLastToken;
            if( objTokens == null )
            {
                GetUrlTokens( objNodes, ref objTokens );
            }
            foreach( MenuNode objNode in objNodes ) //look all nodes
            {
                strLastToken = "";
                foreach( string strToken in ( (Hashtable)objTokens.Clone() ).Keys ) //loop all tokens (have to clone so we can modify real collection
                {
                    if( objNode.NavigateURL.Length > 0 && objNode.NavigateURL.IndexOf( strToken ) > -1 ) //if url contains token
                    {
                        objTokens[strToken] = Convert.ToInt32( objTokens[strToken] ) - 1; //remove token from count
                        if( strToken.Length > strLastToken.Length && ( Convert.ToInt32( objTokens[strToken] ) > 0 || objUsedTokens.Contains( strToken ) ) ) //if token is better and not only one with match
                        {
                            strLastToken = strToken; //use it
                        }
                    }
                }
                if( strLastToken.Length > 0 )
                {
                    if( objUsedTokens.Contains( strLastToken ) == false )
                    {
                        objUsedTokens.Add( strLastToken );
                    }
                    objNode.UrlIndex = objUsedTokens.IndexOf( strLastToken );
                    objNode.NavigateURL = objNode.NavigateURL.Substring( strLastToken.Length );
                }
                AssignUrlTokens( objNode.MenuNodes, ref objTokens, ref objUsedTokens );
            }
        }

        private void GetUrlTokens( MenuNodeCollection objNodes, ref Hashtable objTokens )
        {
            if( objTokens == null )
            {
                objTokens = new Hashtable();
            }
            foreach( MenuNode objNode in objNodes )
            {
                if( objNode.NavigateURL.Length > 0 )
                {
                    AddUrlTokens( objNode.NavigateURL, ref objTokens );
                }
                GetUrlTokens( objNode.MenuNodes, ref objTokens );
            }
        }

        private void AddUrlTokens( string strUrl, ref Hashtable objTokens )
        {
            string strToken = "";
            foreach( string strPart in strUrl.Split( '/' ) )
            {
                if( strToken.Length + 1 + strPart.Length < strUrl.Length ) //determine if we can append /
                {
                    strToken += strPart + "/";
                }
                else
                {
                    strToken += strPart;
                }
                if( objTokens.ContainsKey( strToken ) == false )
                {
                    objTokens.Add( strToken, 1 );
                }
                else
                {
                    objTokens[strToken] = Convert.ToInt32( objTokens[strToken] ) + 1;
                }
            }
        }
    }
}