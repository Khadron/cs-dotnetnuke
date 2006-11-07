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
using System.Xml.Serialization;

namespace DotNetNuke.Services.Localization
{
    /// <Summary>
    /// The Locale class is a custom business object that represents a locale, which is the language and country combination.
    /// </Summary>
    public class Locale
    {
        private string _Code;
        private string _Fallback;
        private string _Text;

        [XmlAttribute( "Code" )]
        public string Code
        {
            get
            {
                return this._Code;
            }
            set
            {
                this._Code = value;
            }
        }

        [XmlAttributeAttribute( "Fallback" )]
        public string Fallback
        {
            get
            {
                return this._Fallback;
            }
            set
            {
                this._Fallback = value;
            }
        }

        [XmlAttributeAttribute( "DisplayName" )]
        public string Text
        {
            get
            {
                return this._Text;
            }
            set
            {
                this._Text = value;
            }
        }
    }
}