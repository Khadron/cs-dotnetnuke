using System.Collections;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    public class PaInstallInfo
    {
        private PaFile _dnn;
        private Hashtable _FileTable;
        private PaLogger _Log;
        private string _Path;

        public PaInstallInfo()
        {
            this._Log = new PaLogger();
            this._Path = "";
            this._FileTable = new Hashtable();
        }

        public PaFile DnnFile
        {
            get
            {
                return this._dnn;
            }
            set
            {
                this._dnn = value;
            }
        }

        public Hashtable FileTable
        {
            get
            {
                return this._FileTable;
            }
            set
            {
                this._FileTable = value;
            }
        }

        public PaLogger Log
        {
            get
            {
                return this._Log;
            }
            set
            {
                this._Log = value;
            }
        }

        public string SitePath
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