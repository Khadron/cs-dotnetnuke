using DotNetNuke.Entities.Modules.Actions;

namespace DotNetNuke.Entities.Modules
{
    public interface IActionable
    {
        ModuleActionCollection ModuleActions { get; }
    }
}