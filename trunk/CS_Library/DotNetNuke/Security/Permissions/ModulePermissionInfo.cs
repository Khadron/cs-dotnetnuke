#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
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