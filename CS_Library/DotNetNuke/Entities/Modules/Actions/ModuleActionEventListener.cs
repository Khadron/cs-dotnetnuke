namespace DotNetNuke.Entities.Modules.Actions
{
    public class ModuleActionEventListener
    {
        private ActionEventHandler _actionEvent;
        private int _moduleID;

        public ModuleActionEventListener( int ModID, ActionEventHandler e )
        {
            this._moduleID = ModID;
            this._actionEvent = e;
        }

        public ActionEventHandler ActionEvent
        {
            get
            {
                return this._actionEvent;
            }
        }

        public int ModuleID
        {
            get
            {
                return this._moduleID;
            }
        }
    }
}