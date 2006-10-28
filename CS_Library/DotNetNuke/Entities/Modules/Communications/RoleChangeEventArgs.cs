namespace DotNetNuke.Entities.Modules.Communications
{
    public class RoleChangeEventArgs : ModuleCommunicationEventArgs
    {
        private string _RoleId = null;
        private string _PortalId = null;

        public string PortalId
        {
            get
            {
                return _PortalId;
            }
            set
            {
                _PortalId = value;
            }
        }

        public string RoleId
        {
            get
            {
                return _RoleId;
            }
            set
            {
                _RoleId = value;
            }
        }
    } 
}