using System.Xml.Serialization;

namespace DotNetNuke.Security.Permissions
{
    public class TabPermissionInfo : PermissionInfo
    {
        private bool _AllowAccess;
        private string _permissionKey;
        private int _roleID;
        private string _RoleName;
        private int _TabID;
        private int _TabPermissionID;

        [XmlElement( "allowaccess" )]
        public bool AllowAccess
        {
            get
            {
                return this._AllowAccess;
            }
            set
            {
                this._AllowAccess = value;
            }
        }

        [XmlIgnoreAttribute()]
        public int RoleID
        {
            get
            {
                return this._roleID;
            }
            set
            {
                this._roleID = value;
            }
        }

        [XmlElementAttribute( "rolename" )]
        public string RoleName
        {
            get
            {
                return this._RoleName;
            }
            set
            {
                this._RoleName = value;
            }
        }

        [XmlIgnoreAttribute()]
        public int TabID
        {
            get
            {
                return this._TabID;
            }
            set
            {
                this._TabID = value;
            }
        }

        [XmlIgnoreAttribute()]
        public int TabPermissionID
        {
            get
            {
                return this._TabPermissionID;
            }
            set
            {
                this._TabPermissionID = value;
            }
        }
    }
}