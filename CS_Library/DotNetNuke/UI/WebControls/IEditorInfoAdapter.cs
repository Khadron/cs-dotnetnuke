namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The IEditorInfoAdapter control provides an Adapter Interface for datasources
    /// </Summary>
    public interface IEditorInfoAdapter
    {
        EditorInfo CreateEditControl();
        bool UpdateValue( PropertyEditorEventArgs e );
        bool UpdateVisibility( PropertyEditorEventArgs e );
    }
}