#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
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
    [XmlRoot( "LanguagePack" )]
    public class LocaleFilePack
    {
        private LocaleFileCollection _Files;
        private Locale _LocalePackCulture;
        private string _Version;

        public LocaleFilePack()
        {
            this._LocalePackCulture = new Locale();
            this._Files = new LocaleFileCollection();
        }

        [XmlArrayItemAttribute( "File", typeof( LocaleFileInfo ) )]
        public LocaleFileCollection Files
        {
            get
            {
                return this._Files;
            }
            set
            {
                this._Files = value;
            }
        }

        [XmlElementAttribute( "Culture" )]
        public Locale LocalePackCulture
        {
            get
            {
                return this._LocalePackCulture;
            }
            set
            {
                this._LocalePackCulture = value;
            }
        }

        [XmlAttributeAttribute()]
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