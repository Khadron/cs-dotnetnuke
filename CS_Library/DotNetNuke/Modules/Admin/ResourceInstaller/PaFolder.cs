using System.Collections;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    public class PaFolder
    {
        private string _BusinessControllerClass;
        private ArrayList _Controls;
        private string _Description;
        private ArrayList _Files;
        private string _FolderName;
        private string _FriendlyName;
        private string _ModuleName;
        private ArrayList _Modules;
        private string _Name;
        private string _ProviderType;
        private string _ResourceFile;
        private string _Version;

        public PaFolder()
        {
            this._Name = Null.NullString;
            this._FolderName = Null.NullString;
            this._FriendlyName = Null.NullString;
            this._ModuleName = Null.NullString;
            this._Description = Null.NullString;
            this._Version = Null.NullString;
            this._ResourceFile = Null.NullString;
            this._ProviderType = Null.NullString;
            this._BusinessControllerClass = Null.NullString;
            this._Modules = new ArrayList();
            this._Controls = new ArrayList();
            this._Files = new ArrayList();
        }

        public string BusinessControllerClass
        {
            get
            {
                return this._BusinessControllerClass;
            }
            set
            {
                this._BusinessControllerClass = value;
            }
        }

        public ArrayList Controls
        {
            get
            {
                return this._Controls;
            }
        }

        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                this._Description = value;
            }
        }

        public ArrayList Files
        {
            get
            {
                return this._Files;
            }
        }

        public string FolderName
        {
            get
            {
                return this._FolderName;
            }
            set
            {
                this._FolderName = value;
            }
        }

        public string FriendlyName
        {
            get
            {
                return this._FriendlyName;
            }
            set
            {
                this._FriendlyName = value;
            }
        }

        public string ModuleName
        {
            get
            {
                return this._ModuleName;
            }
            set
            {
                this._ModuleName = value;
            }
        }

        public ArrayList Modules
        {
            get
            {
                return this._Modules;
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

        public string ProviderType
        {
            get
            {
                return this._ProviderType;
            }
            set
            {
                this._ProviderType = value;
            }
        }

        public string ResourceFile
        {
            get
            {
                return this._ResourceFile;
            }
            set
            {
                this._ResourceFile = value;
            }
        }

        public string Version
        {
            get
            {
                return this._Version;
            }
            set
            {
                this._Version = value;
            }
        }
    }
}