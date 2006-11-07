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
namespace DotNetNuke.Entities.Modules.Definitions
{
    public class ModuleDefinitionInfo
    {
        private int _DefaultCacheTime;
        private int _DesktopModuleID;
        private string _FriendlyName;
        private int _ModuleDefID;
        private int _TempModuleID;

        public ModuleDefinitionInfo()
        {
            this._DefaultCacheTime = 0;
        }

        public int DefaultCacheTime
        {
            get
            {
                return this._DefaultCacheTime;
            }
            set
            {
                this._DefaultCacheTime = value;
            }
        }

        public int DesktopModuleID
        {
            get
            {
                return this._DesktopModuleID;
            }
            set
            {
                this._DesktopModuleID = value;
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

        public int ModuleDefID
        {
            get
            {
                return this._ModuleDefID;
            }
            set
            {
                this._ModuleDefID = value;
            }
        }

        public int TempModuleID
        {
            get
            {
                return this._TempModuleID;
            }
            set
            {
                this._TempModuleID = value;
            }
        }
    }
}