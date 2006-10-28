using System.Xml.Serialization;

namespace DotNetNuke.Security.Permissions
{
    public class ModulePermissionInfo : PermissionInfo
    {
        private bool _AllowAccess;
        private int _moduleID;
        private int _modulePermissionID;
        private string _permissionKey;
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
        public int ModuleID
        {
            get
            {
                return this._moduleID;
            }
            set
            {
                this._moduleID = value;
            }
        }

        [XmlIgnoreAttribute()]
        public int ModulePermissionID
        {
            get
            {
                return this._modulePermissionID;
            }
            set
            {
                this._modulePermissionID = value;
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

        /// <Summary>
        /// Compares if two ModulePermissionInfo objects are equivalent/equal
        /// </Summary>
        /// <Param name="obj">a ModulePermissionObject</Param>
        /// <Returns>
        /// true if the permissions being passed represents the same permission
        /// in the current object
        /// </Returns>
        public override bool Equals( object obj )
        {
            if( ( obj == null ) | ( this.GetType() != obj.GetType() ) )
            {
                return false;
            }
            ModulePermissionInfo modulePermissionInfo1 = ( (ModulePermissionInfo)obj );
            return ( ( ( ( this.AllowAccess == modulePermissionInfo1.AllowAccess ) & ( this.ModuleID == modulePermissionInfo1.ModuleID ) ) & ( this.RoleID == modulePermissionInfo1.RoleID ) ) & ( this.PermissionID == modulePermissionInfo1.PermissionID ) );
        }
    }
}