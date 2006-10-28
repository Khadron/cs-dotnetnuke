using System.Xml.Serialization;

namespace DotNetNuke.Security.Permissions
{
    public class FolderPermissionInfo : PermissionInfo
    {
        private bool _AllowAccess;
        private int _folderID;
        private string _folderPath;
        private int _folderPermissionID;
        private string _permissionKey;
        private int _portalID;
        private int _roleID;
        private string _RoleName;

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
        public int FolderID
        {
            get
            {
                return this._folderID;
            }
            set
            {
                this._folderID = value;
            }
        }

        [XmlElementAttribute( "folderpath" )]
        public string FolderPath
        {
            get
            {
                return this._folderPath;
            }
            set
            {
                this._folderPath = value;
            }
        }

        [XmlIgnoreAttribute()]
        public int FolderPermissionID
        {
            get
            {
                return this._folderPermissionID;
            }
            set
            {
                this._folderPermissionID = value;
            }
        }

        [XmlIgnoreAttribute()]
        public int PortalID
        {
            get
            {
                return this._portalID;
            }
            set
            {
                this._portalID = value;
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
    }
}