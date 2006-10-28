using System;

namespace DotNetNuke.Entities.Modules.Actions
{
    public class ActionEventArgs : EventArgs
    {
        private ModuleAction _action;
        private ModuleInfo _moduleConfiguration;

        public ActionEventArgs( ModuleAction Action, ModuleInfo ModuleConfiguration )
        {
            this._action = Action;
            this._moduleConfiguration = ModuleConfiguration;
        }

        public ModuleAction Action
        {
            get
            {
                return this._action;
            }
        }

        public ModuleInfo ModuleConfiguration
        {
            get
            {
                return this._moduleConfiguration;
            }
        }
    }
}