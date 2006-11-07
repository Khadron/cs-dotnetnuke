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