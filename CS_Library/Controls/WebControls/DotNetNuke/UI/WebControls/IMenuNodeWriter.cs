using System.Web.UI;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// IMenuNodeWriter interface declaration. All the objects which want to implement
    /// a writer class for the MenuNode should inherit from this interface.
    /// </Summary>
    internal interface IMenuNodeWriter
    {
        /// <Summary>When implemented renders an Node inside the Menu.</Summary>
        void RenderNode( HtmlTextWriter writer, MenuNode Node );
    }
}