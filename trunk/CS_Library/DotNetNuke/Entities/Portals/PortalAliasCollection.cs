using System.Collections;

namespace DotNetNuke.Entities.Portals
{
    public class PortalAliasCollection : DictionaryBase
    {
        // Gets or sets the value associated with the specified key.
        public PortalAliasInfo this[string key]
        {
            get
            {
                return ((PortalAliasInfo)this.Dictionary[key]);
            }
            set
            {
                this.Dictionary[key] = value;
            }
        }

        public bool Contains(string key)
        {
            return Dictionary.Contains(key);
        } //Contains

        // Gets a value indicating if the collection contains keys that are not null.
        public bool HasKeys
        {
            get
            {
                return this.Dictionary.Keys.Count > 0;
            }
        }

        public ICollection Keys
        {
            get
            {
                return this.Dictionary.Keys;
            }
        }

        // Adds an entry to the collection.
        public void Add(string key, PortalAliasInfo value)
        {
            this.Dictionary.Add(key, value);
        } //Add
    }
}