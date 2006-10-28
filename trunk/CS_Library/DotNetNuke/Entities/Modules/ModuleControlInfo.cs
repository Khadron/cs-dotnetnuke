using DotNetNuke.Security;

namespace DotNetNuke.Entities.Modules
{
    public class ModuleControlInfo
    {
        private string _ControlKey;
        private string _ControlSrc;
        private string _ControlTitle;
        private int _ControlType;
        private string _HelpURL;
        private string _IconFile;
        private int _ModuleControlID;
        private int _ModuleDefID;
        private int _ViewOrder;

        public string ControlKey
        {
            get
            {
                return this._ControlKey;
            }
            set
            {
                this._ControlKey = value;
            }
        }

        public string ControlSrc
        {
            get
            {
                return this._ControlSrc;
            }
            set
            {
                this._ControlSrc = value;
            }
        }

        public string ControlTitle
        {
            get
            {
                return this._ControlTitle;
            }
            set
            {
                this._ControlTitle = value;
            }
        }

        public SecurityAccessLevel ControlType
        {
            get
            {
                return ( (SecurityAccessLevel)this._ControlType );
            }
            set
            {
                this._ControlType = ( (int)value );
            }
        }

        public string HelpURL
        {
            get
            {
                return this._HelpURL;
            }
            set
            {
                this._HelpURL = value;
            }
        }

        public string IconFile
        {
            get
            {
                return this._IconFile;
            }
            set
            {
                this._IconFile = value;
            }
        }

        public int ModuleControlID
        {
            get
            {
                return this._ModuleControlID;
            }
            set
            {
                this._ModuleControlID = value;
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

        public int ViewOrder
        {
            get
            {
                return this._ViewOrder;
            }
            set
            {
                this._ViewOrder = value;
            }
        }
    }
}