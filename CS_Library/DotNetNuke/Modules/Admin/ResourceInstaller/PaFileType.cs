using System.ComponentModel;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    [TypeConverter( typeof( EnumConverter ) )]
    public enum PaFileType
    {
        Dll = 0,
        Sql = 1,
        Ascx = 2,
        Dnn = 3,
        DataProvider = 4,
        Other = 5,
    }
}