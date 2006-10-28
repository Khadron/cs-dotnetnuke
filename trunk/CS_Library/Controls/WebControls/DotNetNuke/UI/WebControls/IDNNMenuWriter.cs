using System.Web.UI;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// IDNNMenuWriter interface declaration. All the objects which want to implement
    /// a writer class for the DNNMenu should inherit from this interface.
    /// </Summary>
    internal interface IDNNMenuWriter
    {
        /// <Summary>When implemented renders the Menu.</Summary>
        void RenderMenu( HtmlTextWriter writer, DNNMenu menu );
    }
}