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