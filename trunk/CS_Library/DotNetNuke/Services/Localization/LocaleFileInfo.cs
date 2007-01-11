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

namespace DotNetNuke.Services.Localization
{
    public class LocaleFileInfo
    {
        private byte[] _Buffer;
        private string _LocaleFileName;
        private LocaleType _LocaleFileType;
        private string _LocaleModule;
        private string _LocalePath;

        public LocaleFileInfo()
        {
        }

        public LocaleFileInfo( string FileName, LocaleType FileType, string ModuleName, string Path )
        {
            this._LocaleFileName = FileName;
            this._LocaleFileType = FileType;
            this._LocaleModule = ModuleName;
            this._LocalePath = Path;
        }

        [XmlIgnore()]
        public byte[] Buffer
        {
            get
            {
                return this._Buffer;
            }
            set
            {
                this._Buffer = value;
            }
        }

        [XmlAttributeAttribute( "FileName" )]
        public string LocaleFileName
        {
            get
            {
                return this._LocaleFileName;
            }
            set
            {
                this._LocaleFileName = value;
            }
        }

        [XmlAttributeAttribute( "FileType" )]
        public LocaleType LocaleFileType
        {
            get
            {
                return this._LocaleFileType;
            }
            set
            {
                this._LocaleFileType = value;
            }
        }

        [XmlAttributeAttribute( "ModuleName" )]
        public string LocaleModule
        {
            get
            {
                return this._LocaleModule;
            }
            set
            {
                this._LocaleModule = value;
            }
        }

        [XmlAttributeAttribute( "FilePath" )]
        public string LocalePath
        {
            get
            {
                return this._LocalePath;
            }
            set
            {
                this._LocalePath = value;
            }
        }
    }
}