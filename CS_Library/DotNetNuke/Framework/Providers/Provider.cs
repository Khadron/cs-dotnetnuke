using System.Collections.Specialized;
using System.Xml;

namespace DotNetNuke.Framework.Providers
{
    public class Provider
    {
        private string _ProviderName;
        private string _ProviderType;
        private NameValueCollection _ProviderAttributes = new NameValueCollection();

        public Provider(XmlAttributeCollection Attributes)
        {
            // Set the name of the provider
            //
            _ProviderName = Attributes["name"].Value;

            // Set the type of the provider
            //
            _ProviderType = Attributes["type"].Value;

            // Store all the attributes in the attributes bucket
            //
            XmlAttribute Attribute;
            foreach (XmlAttribute tempLoopVar_Attribute in Attributes)
            {
                Attribute = tempLoopVar_Attribute;

                if (Attribute.Name != "name" && Attribute.Name != "type")
                {
                    _ProviderAttributes.Add(Attribute.Name, Attribute.Value);
                }
            }
        }

        public string Name
        {
            get
            {
                return _ProviderName;
            }
        }

        public string Type
        {
            get
            {
                return _ProviderType;
            }
        }

        public NameValueCollection Attributes
        {
            get
            {
                return _ProviderAttributes;
            }
        }
    }
}