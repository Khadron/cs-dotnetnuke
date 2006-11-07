#region DotNetNuke License
// DotNetNuke� - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
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