using System.Xml.Serialization;
using DotNetNuke.Common.Utilities;

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
                string _folderName = DotNetNuke.Common.Utilities.FileSystemUtils.RemoveTrailingSlash(_folderPath);
                if (_folderName.Length > 0 && _folderName.LastIndexOf("/") > -1)
                {
                    _folderName = _folderName.Substring(_folderName.LastIndexOf("/") + 1);
                }
                return _folderName;
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