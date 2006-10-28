namespace DotNetNuke.UI.Utilities
{
    public interface IClientAPICallbackEventHandler
    {
        string RaiseClientAPICallbackEvent( string eventArgument );
    }
}