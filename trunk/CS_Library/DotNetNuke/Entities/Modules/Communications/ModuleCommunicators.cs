using System.Collections;

namespace DotNetNuke.Entities.Modules.Communications
{
    public class ModuleCommunicators : CollectionBase
    {
        public IModuleCommunicator this[ int index ]
        {
            get
            {
                return ( (IModuleCommunicator)this.List[index] );
            }
            set
            {
                this.List[index] = value;
            }
        }

        public int Add( IModuleCommunicator item )
        {
            return this.List.Add( item );
        }
    }
}