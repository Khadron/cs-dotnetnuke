using System.Web.UI;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// IDNNTreeWriter interface declaration. All the objects which want to implement
    /// a writer class for the DNNTree should inherit from this interface.
    /// </Summary>
    internal interface IDNNTreeWriter
    {
        /// <Summary>When implemented renders the tree.</Summary>
        void RenderTree( HtmlTextWriter writer, DnnTree tree );
    }
}