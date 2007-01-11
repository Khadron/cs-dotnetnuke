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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Services.FileSystem
{
    /// <Summary>
    /// Represents the File object and holds the Properties of that object
    /// </Summary>
    [XmlRoot( "file", IsNullable = false )]
    public class FileInfo
    {
        /// <Summary>
        /// The Primary Key ID of the current File, as represented within the Database table named "Files"
        /// </Summary>
        /// <remarks>
        /// This Integer Property is passed to the FileInfo Collection
        /// </remarks>
        private int _FileId;
        private string _FileName;
        private string _Folder;
        private int _FolderId;
        private int _Height;
        private bool _IsCached;
        private int _PortalId;
        private int _Size;
        private int _StorageLocation;
        private int _Width;
        private string _ContentType;
        private string _Extension;

        public FileInfo()
        {
            this._IsCached = false;
        }

        [XmlElement( "contenttype" )]
        public string ContentType
        {
            get
            {
                return this._ContentType;
            }
            set
            {
                this._ContentType = value;
            }
        }

        [XmlElement( "extension" )]
        public string Extension
        {
            get
            {
                return this._Extension;
            }
            set
            {
                this._Extension = value;
            }
        }

        [XmlIgnore()]
        public int FileId
        {
            get
            {
                return this._FileId;
            }
            set
            {
                this._FileId = value;
            }
        }

        [XmlElement( "filename" )]
        public string FileName
        {
            get
            {
                return this._FileName;
            }
            set
            {
                this._FileName = value;
            }
        }

        [XmlIgnore()]
        public string Folder
        {
            get
            {
                return this._Folder;
            }
            set
            {
                this._Folder = value;
            }
        }

        [XmlIgnore()]
        public int FolderId
        {
            get
            {
                return this._FolderId;
            }
            set
            {
                this._FolderId = value;
            }
        }

        [XmlElement( "height" )]
        public int Height
        {
            get
            {
                return this._Height;
            }
            set
            {
                this._Height = value;
            }
        }

        [XmlIgnore()]
        public bool IsCached
        {
            get
            {
                return this._IsCached;
            }
            set
            {
                this._IsCached = value;
            }
        }

        [XmlIgnore()]
        public string PhysicalPath
        {
            get
            {
                PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();
                string physicalPath;

                if (PortalId == Null.NullInteger)
                {
                    physicalPath = DotNetNuke.Common.Globals.HostMapPath + Folder + FileName;
                }
                else
                {
                    physicalPath = portalSettings.HomeDirectoryMapPath + Folder + FileName;
                }

                return physicalPath.Replace("/", "\\");
            }
        }


        [XmlIgnore()]
        public int PortalId
        {
            get
            {
                return this._PortalId;
            }
            set
            {
                this._PortalId = value;
            }
        }

        [XmlElement( "size" )]
        public int Size
        {
            get
            {
                return this._Size;
            }
            set
            {
                this._Size = value;
            }
        }

        [XmlIgnore()]
        public int StorageLocation
        {
            get
            {
                return this._StorageLocation;
            }
            set
            {
                this._StorageLocation = value;
            }
        }

        [XmlElement( "width" )]
        public int Width
        {
            get
            {
                return this._Width;
            }
            set
            {
                this._Width = value;
            }
        }
    }
}