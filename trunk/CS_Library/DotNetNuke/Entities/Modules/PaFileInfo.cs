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