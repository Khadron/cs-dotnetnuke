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