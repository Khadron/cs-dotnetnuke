namespace DotNetNuke.Entities.Modules.Definitions
{
    public class ModuleDefinitionInfo
    {
        private int _DefaultCacheTime;
        private int _DesktopModuleID;
        private string _FriendlyName;
        private int _ModuleDefID;
        private int _TempModuleID;

        public ModuleDefinitionInfo()
        {
            this._DefaultCacheTime = 0;
        }

        public int DefaultCacheTime
        {
            get
            {
                return this._DefaultCacheTime;
            }
            set
            {
                this._DefaultCacheTime = value;
            }
        }

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

        public int ModuleDefID
        {
            get
            {
                return this._ModuleDefID;
            }
            set
            {
                this._ModuleDefID = value;
            }
        }

        public int TempModuleID
        {
            get
            {
                return this._TempModuleID;
            }
            set
            {
                this._TempModuleID = value;
            }
        }
    }
}