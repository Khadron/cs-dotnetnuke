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

using System;
using System.Xml.Serialization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Services.FileSystem
{
    [XmlRoot( "folder", IsNullable = false )]
    public class FolderInfo
    {
        private int _folderID;
        private string _folderPath;
        private bool _isCached;
        private bool _isProtected;
        private int _portalID;
        private int _storageLocation;
        private DateTime _lastUpdated;

        public FolderInfo()
        {
            this._isCached = false;
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

        [XmlIgnoreAttribute()]
        public string FolderName
        {
            get
            {
                string folderName = FileSystemUtils.RemoveTrailingSlash(_folderPath);
                if (!String.IsNullOrEmpty( folderName) && folderName.LastIndexOf("/") > -1)
                {
                    folderName = folderName.Substring(folderName.LastIndexOf("/") + 1);
                }
                return folderName;
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
        public bool IsCached
        {
            get
            {
                return this._isCached;
            }
            set
            {
                this._isCached = value;
            }
        }

        [XmlIgnoreAttribute()]
        public bool IsProtected
        {
            get
            {
                return this._isProtected;
            }
            set
            {
                this._isProtected = value;
            }
        }

        [XmlIgnore()]
        public DateTime LastUpdated
        {
            get
            {
                return _lastUpdated;
            }
            set
            {
                _lastUpdated = value;
            }
        }

        [XmlIgnore()]
        public string PhysicalPath
        {
            get
            {
                PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();
                string physicalPath;

                if (PortalID == Null.NullInteger)
                {
                    physicalPath = DotNetNuke.Common.Globals.HostMapPath + FolderPath;
                }
                else
                {
                    physicalPath = portalSettings.HomeDirectoryMapPath + FolderPath;
                }

                return physicalPath.Replace("/", "\\");
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

        [XmlElementAttribute( "storagelocation" )]
        public int StorageLocation
        {
            get
            {
                return this._storageLocation;
            }
            set
            {
                this._storageLocation = value;
            }
        }
    }
}