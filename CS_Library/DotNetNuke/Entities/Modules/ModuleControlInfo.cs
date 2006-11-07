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
using DotNetNuke.Security;

namespace DotNetNuke.Entities.Modules
{
    public class ModuleControlInfo
    {
        private string _ControlKey;
        private string _ControlSrc;
        private string _ControlTitle;
        private int _ControlType;
        private string _HelpURL;
        private string _IconFile;
        private int _ModuleControlID;
        private int _ModuleDefID;
        private int _ViewOrder;

        public string ControlKey
        {
            get
            {
                return this._ControlKey;
            }
            set
            {
                this._ControlKey = value;
            }
        }

        public string ControlSrc
        {
            get
            {
                return this._ControlSrc;
            }
            set
            {
                this._ControlSrc = value;
            }
        }

        public string ControlTitle
        {
            get
            {
                return this._ControlTitle;
            }
            set
            {
                this._ControlTitle = value;
            }
        }

        public SecurityAccessLevel ControlType
        {
            get
            {
                return ( (SecurityAccessLevel)this._ControlType );
            }
            set
            {
                this._ControlType = ( (int)value );
            }
        }

        public string HelpURL
        {
            get
            {
                return this._HelpURL;
            }
            set
            {
                this._HelpURL = value;
            }
        }

        public string IconFile
        {
            get
            {
                return this._IconFile;
            }
            set
            {
                this._IconFile = value;
            }
        }

        public int ModuleControlID
        {
            get
            {
                return this._ModuleControlID;
            }
            set
            {
                this._ModuleControlID = value;
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

        public int ViewOrder
        {
            get
            {
                return this._ViewOrder;
            }
            set
            {
                this._ViewOrder = value;
            }
        }
    }
}