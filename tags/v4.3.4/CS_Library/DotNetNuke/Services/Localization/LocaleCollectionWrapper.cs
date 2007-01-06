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

namespace DotNetNuke.Services.Localization
{
    /// <Summary>
    /// The LocaleCollectionWrapper class provides a simple wrapper around a .
    /// </Summary>
    public class LocaleCollectionWrapper : IEnumerator, IEnumerable
    {
        private int _index;
        private LocaleCollection _locales;

        /// <Summary>
        /// Initializes a new instance of the
        /// class containing the specified collection  objects.
        /// </Summary>
        /// <Param name="Locales">A  object which is wrapped by the collection.</Param>
        public LocaleCollectionWrapper( LocaleCollection Locales )
        {
            this._index = 0;
            this._locales = Locales;
        }

        public virtual object Current
        {
            get
            {
                return this._locales[this._locales.Keys[this._index]];
            }
        }

        public virtual IEnumerator GetEnumerator()
        {
            return this;
        }

        public virtual bool MoveNext()
        {
            this._index++;
            return ( this._index < this._locales.Keys.Count );
        }

        public virtual void Reset()
        {
            this._index = 0;
        }
    }
}