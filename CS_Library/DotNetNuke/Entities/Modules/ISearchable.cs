using DotNetNuke.Services.Search;

namespace DotNetNuke.Entities.Modules
{
    public interface ISearchable
    {
        SearchItemInfoCollection GetSearchItems( ModuleInfo ModInfo );
    }
}