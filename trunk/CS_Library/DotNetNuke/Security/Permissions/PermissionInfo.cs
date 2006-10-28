using System.Xml.Serialization;

namespace DotNetNuke.Security.Permissions
{
    public class PermissionInfo
    {
        private int _moduleDefID;
        private string _permissionCode;
        private int _permissionID;
        private string _permissionKey;
        private string _PermissionName;

        [XmlIgnore()]
        public int ModuleDefID
        {
            get
            {
                return this._moduleDefID;
            }
            set
            {
                this._moduleDefID = value;
            }
        }

        [XmlElementAttribute( "permissioncode" )]
        public string PermissionCode
        {
            get
            {
                return this._permissionCode;
            }
            set
            {
                this._permissionCode = value;
            }
        }

        [XmlIgnoreAttribute()]
        public int PermissionID
        {
            get
            {
                return this._permissionID;
            }
            set
            {
                this._permissionID = value;
            }
        }

        [XmlElementAttribute( "permissionkey" )]
        public string PermissionKey
        {
            get
            {
                return this._permissionKey;
            }
            set
            {
                this._permissionKey = value;
            }
        }

        [XmlIgnoreAttribute()]
        public string PermissionName
        {
            get
            {
                return this._PermissionName;
            }
            set
            {
                this._PermissionName = value;
            }
        }
    }
}