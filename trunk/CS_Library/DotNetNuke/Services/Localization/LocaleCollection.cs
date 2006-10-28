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