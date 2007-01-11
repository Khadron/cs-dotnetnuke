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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
    internal class DNNMenuWriter : WebControl, IDNNMenuWriter
    {
        private DNNMenu m_objMenu;
        private bool m_blnForceFullMenu;

        public bool ForceFullMenu
        {
            get
            {
                return m_blnForceFullMenu;
            }
            set
            {
                m_blnForceFullMenu = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jhenning]	9/21/2005	Created
        /// </history>
        public DNNMenuWriter()
        {
        }

        public DNNMenuWriter( bool blnForceFullMenu )
        {
            this.ForceFullMenu = blnForceFullMenu;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="Menu"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jhenning]	9/21/2005	Created
        /// </history>
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
        /// 	[jhenning]	9/21/2005	Created
        /// </history>
        protected override void RenderContents( HtmlTextWriter writer )
        {
            string strClass = "DNNMenu ";
            if( m_objMenu.SelectedMenuNodes.Count == 0 )
            {
                if( m_objMenu.MenuBarCssClass.Length > 0 )
                {
                    strClass += m_objMenu.MenuBarCssClass;
                }
            }
            else
            {
                if( m_objMenu.MenuCssClass.Length > 0 )
                {
                    strClass += m_objMenu.MenuCssClass;
                }
            }
            writer.AddAttribute( HtmlTextWriterAttribute.Width, "100%" );
            writer.AddAttribute( HtmlTextWriterAttribute.Class, strClass );
            writer.AddAttribute( HtmlTextWriterAttribute.Name, m_objMenu.UniqueID );
            writer.AddAttribute( HtmlTextWriterAttribute.Id, m_objMenu.ClientID );

            writer.RenderBeginTag( HtmlTextWriterTag.Div );
            RenderChildren( writer );
            writer.RenderEndTag();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="writer"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jhenning]	9/21/2005	Created
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
    }
}