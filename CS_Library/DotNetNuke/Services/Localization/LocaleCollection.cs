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
using System;
using System.Collections;
using System.Collections.Specialized;

namespace DotNetNuke.Services.Localization
{
    /// <Summary>
    /// The LocaleCollection class is a collection of Locale objects.  It stores the supported locales.
    /// </Summary>
    public class LocaleCollection : NameObjectCollectionBase
    {
        private DictionaryEntry _de;

        public LocaleCollection()
        {
            this._de = new DictionaryEntry();
        }
        /// <summary>
        /// Gets a String array that contains all the keys in the collection.
        /// </summary>
        public string[] AllKeys
        {
            get
            {
                return this.BaseGetAllKeys();
            }
        }
        /// <summary>
        /// Gets an Object array that contains all the values in the collection.
        /// </summary>
        public Array AllValues
        {
            get
            {
                return this.BaseGetAllValues();
            }
        }
        /// <summary>
        /// Gets a value indicating if the collection contains keys that are not null.
        /// </summary>
        public bool HasKeys
        {
            get
            {
                return this.BaseHasKeys();
            }
        }

        public DictionaryEntry this[ int index ]
        {
            get
            {
                this._de.Key = this.BaseGetKey( index );
                this._de.Value = this.BaseGet( index );
                return this._de;
            }
        }
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        public Locale this[ string key ]
        {
            get
            {
                return ( (Locale)this.BaseGet( key ) );
            }
            set
            {
                this.BaseSet( key, value );
            }
        }
        /// <summary>
        /// Adds an entry to the collection.
        /// </summary>
        public void Add( string key, object value )
        {
            this.BaseAdd( key, value );
        }
        /// <summary>
        /// Removes an entry with the specified key from the collection.
        /// </summary>
        public void Remove( string key )
        {
            this.BaseRemove( key );
        }
    }
}