using System.Collections;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    public class PaDataProviderComparer : IComparer
    {
        public virtual int Compare( object x, object y )
        {
            return new CaseInsensitiveComparer().Compare( ( (PaFile)x ).Name, ( (PaFile)y ).Name );
        }
    }
}