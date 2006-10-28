using System.Web.UI;

namespace DotNetNuke.Entities.Modules.Communications
{
    public class ModuleCommunicate
    {
        private ModuleCommunicators _ModuleCommunicators = new ModuleCommunicators();
        private ModuleListeners _ModuleListeners = new ModuleListeners();

        public ModuleCommunicators ModuleCommunicators
        {
            get
            {
                return _ModuleCommunicators;
            }
        }

        public ModuleListeners ModuleListeners
        {
            get
            {
                return _ModuleListeners;
            }
        }


        public void LoadCommunicator( Control ctrl )
        {
            // Check and see if the module implements IModuleCommunicator
            if( ctrl is IModuleCommunicator )
            {
                this.Add( (IModuleCommunicator)ctrl );
            }

            // Check and see if the module implements IModuleListener
            if( ctrl is IModuleListener )
            {
                this.Add( (IModuleListener)ctrl );
            }
        }

        private int Add( IModuleCommunicator item )
        {
            int returnData = _ModuleCommunicators.Add( item );

            int i;
            for( i = 0; i <= _ModuleListeners.Count - 1; i++ )
            {
                item.ModuleCommunication += new ModuleCommunicationEventHandler( this._ModuleListeners[i].OnModuleCommunication );
            }

            return returnData;
        } 

        private int Add( IModuleListener item )
        {
            int returnData = _ModuleListeners.Add( item );

            int i;
            for( i = 0; i <= _ModuleCommunicators.Count - 1; i++ )
            {
                _ModuleCommunicators[i].ModuleCommunication += new ModuleCommunicationEventHandler( item.OnModuleCommunication );
            }

            return returnData;
        } 
    }
}