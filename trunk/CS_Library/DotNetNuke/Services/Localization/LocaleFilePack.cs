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