namespace DotNetNuke.Entities.Modules
{
    public interface IPortable
    {
        string ExportModule( int ModuleID );
        void ImportModule( int ModuleID, string Content, string Version, int UserID );
    }
}