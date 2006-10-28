namespace DotNetNuke.Entities.Modules
{
    public class PortalDesktopModuleInfo
    {
        private int _DesktopModuleID;
        private string _FriendlyName;
        private int _PortalDesktopModuleID;
        private int _PortalID;
        private string _PortalName;

        public int DesktopModuleID
        {
            get
            {
                return this._DesktopModuleID;
            }
            set
            {
                this._DesktopModuleID = value;
            }
        }

        public string FriendlyName
        {
            get
            {
                return this._FriendlyName;
            }
            set
            {
                this._FriendlyName = value;
            }
        }

        public int PortalDesktopModuleID
        {
            get
            {
                return this._PortalDesktopModuleID;
            }
            set
            {
                this._PortalDesktopModuleID = value;
            }
        }

        public int PortalID
        {
            get
            {
                return this._PortalID;
            }
            set
            {
                this._PortalID = value;
            }
        }

        public string PortalName
        {
            get
            {
                return this._PortalName;
            }
            set
            {
                this._PortalName = value;
            }
        }
    }
}