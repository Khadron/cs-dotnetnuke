using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
    internal class DNNTreeWriter : WebControl, IDNNTreeWriter
    {
        private DnnTree _tree;

        public void RenderTree( HtmlTextWriter writer, DnnTree tree )
        {
            _tree = tree;
            RenderControl( writer );
        }

        protected override void RenderContents( HtmlTextWriter writer )
        {
            writer.AddAttribute( HtmlTextWriterAttribute.Width, "100%" );
            writer.AddAttribute( HtmlTextWriterAttribute.Class, "DNNTree" );
            writer.AddAttribute( HtmlTextWriterAttribute.Name, _tree.UniqueID );
            writer.AddAttribute( HtmlTextWriterAttribute.Id, _tree.ClientID );

            writer.RenderBeginTag( HtmlTextWriterTag.Div );
            RenderChildren( writer );
            writer.RenderEndTag();
        }

        protected override void RenderChildren( HtmlTextWriter writer )
        {
            TreeNode TempNode;
            foreach( TreeNode tempLoopVar_TempNode in _tree.TreeNodes )
            {
                TempNode = tempLoopVar_TempNode;
                TempNode.Render( writer );
            }
        }
    }
}