namespace DotNetNuke.Entities.Modules
{
    public class PaFileInfo
    {
        private byte[] _Buffer;
        private string _FullPath;
        private string _Name;
        private string _Path;

        public PaFileInfo()
        {
        }

        public PaFileInfo( string Name, string Path, string FullPath )
        {
            this._Name = Name;
            this._Path = Path;
            this._FullPath = FullPath;
        }

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

        public string FullName
        {
            get
            {
                return ( this._FullPath + @"\" + this._Name );
            }
        }

        public string FullPath
        {
            get
            {
                return this._FullPath;
            }
            set
            {
                this._FullPath = value;
            }
        }

        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        public string Path
        {
            get
            {
                return this._Path;
            }
            set
            {
                this._Path = value;
            }
        }
    }
}