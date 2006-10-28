namespace DotNetNuke.Security.Authentication
{
    public interface IAuthenticationObjectBase
    {
        string GUID { get; set; }

        string Location { get; set; }

        string Name { get; }

        ObjectClass ObjectClass { get; }
    }
}