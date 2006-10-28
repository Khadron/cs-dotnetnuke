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