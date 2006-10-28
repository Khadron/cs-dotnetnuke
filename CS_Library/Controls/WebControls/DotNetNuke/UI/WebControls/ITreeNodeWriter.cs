using System.Web.UI;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// ITreeNodeWriter interface declaration. All the objects which want to implement
    /// a writer class for the TreeNode should inherit from this interface.
    /// </Summary>
    internal interface ITreeNodeWriter
    {
        /// <Summary>When implemented renders an Node inside the tree.</Summary>
        void RenderNode( HtmlTextWriter writer, TreeNode Node );
    }
}