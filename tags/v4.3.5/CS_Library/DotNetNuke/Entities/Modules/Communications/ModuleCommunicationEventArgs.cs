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
using System;

namespace DotNetNuke.Entities.Modules.Communications
{
    public class ModuleCommunicationEventArgs : EventArgs
    {
        private string _Type = null;
        private object _Value = null;
        private string _Sender = null;
        private string _Target = null;

        private string _Text = null;

        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
            }
        }

        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        public object Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        public string Sender
        {
            get
            {
                return _Sender;
            }
            set
            {
                _Sender = value;
            }
        }

        public string Target
        {
            get
            {
                return _Target;
            }
            set
            {
                _Target = value;
            }
        }

        public ModuleCommunicationEventArgs()
        {
        }

        public ModuleCommunicationEventArgs( string Type, object Value, string Sender, string Target )
        {
            _Type = Type;
            _Value = Value;
            _Sender = Sender;
            _Target = Target;
        }

        public ModuleCommunicationEventArgs( string Text )
        {
            _Text = Text;
        }
    }
}