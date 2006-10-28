using System.Collections;

namespace DotNetNuke.Entities.Modules.Communications
{
    public class ModuleListeners : CollectionBase
    {
        public IModuleListener this[ int index ]
        {
            get
            {
                return ( (IModuleListener)this.List[index] );
            }
            set
            {
                this.List[index] = value;
            }
        }

        

        public int Add( IModuleListener item )
        {
            return this.List.Add( item );
        } 
    }
}