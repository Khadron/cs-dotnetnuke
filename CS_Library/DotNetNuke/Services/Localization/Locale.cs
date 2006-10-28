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