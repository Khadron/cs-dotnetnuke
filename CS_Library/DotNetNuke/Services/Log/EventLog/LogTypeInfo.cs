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
namespace DotNetNuke.Services.Log.EventLog
{
    public class LogTypeInfo
    {
        private string _LogTypeCSSClass;
        private string _LogTypeDescription;
        private string _LogTypeFriendlyName;
        private string _LogTypeKey;
        private string _LogTypeOwner;

        public string LogTypeCSSClass
        {
            get
            {
                return this._LogTypeCSSClass;
            }
            set
            {
                this._LogTypeCSSClass = value;
            }
        }

        public string LogTypeDescription
        {
            get
            {
                return this._LogTypeDescription;
            }
            set
            {
                this._LogTypeDescription = value;
            }
        }

        public string LogTypeFriendlyName
        {
            get
            {
                return this._LogTypeFriendlyName;
            }
            set
            {
                this._LogTypeFriendlyName = value;
            }
        }

        public string LogTypeKey
        {
            get
            {
                return this._LogTypeKey;
            }
            set
            {
                this._LogTypeKey = value;
            }
        }

        public string LogTypeOwner
        {
            get
            {
                return this._LogTypeOwner;
            }
            set
            {
                this._LogTypeOwner = value;
            }
        }
    }
}