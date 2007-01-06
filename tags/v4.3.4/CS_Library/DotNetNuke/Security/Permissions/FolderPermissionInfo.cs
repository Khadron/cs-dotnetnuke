#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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