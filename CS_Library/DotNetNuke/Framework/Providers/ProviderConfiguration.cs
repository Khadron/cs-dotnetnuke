using System.Collections;
using System.Xml;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Framework.Providers
{
    public class ProviderConfiguration
    {
        private string _DefaultProvider;
        private Hashtable _Providers = new Hashtable();

        public string DefaultProvider
        {
            get
            {
                return _DefaultProvider;
            }
        }

        public Hashtable Providers
        {
            get
            {
                return _Providers;
            }
        }

        public static ProviderConfiguration GetProviderConfiguration(string strProvider)
        {
            return ((ProviderConfiguration)Config.GetSection("dotnetnuke/" + strProvider));
        }

        internal void GetProviders(XmlNode node)
        {
            XmlNode Provider;
            foreach (XmlNode tempLoopVar_Provider in node.ChildNodes)
            {
                Provider = tempLoopVar_Provider;

                switch (Provider.Name)
                {
                    case "add":

                        Providers.Add(Provider.Attributes["name"].Value, new Provider(Provider.Attributes));
                        break;

                    case "remove":

                        Providers.Remove(Provider.Attributes["name"].Value);
                        break;

                    case "clear":

                        Providers.Clear();
                        break;
                }
            }
        }

        internal void LoadValuesFromConfigurationXml(XmlNode node)
        {
            XmlAttributeCollection attributeCollection = node.Attributes;

            // Get the default provider
            _DefaultProvider = attributeCollection["defaultProvider"].Value;

            // Read child nodes
            XmlNode child;
            foreach (XmlNode tempLoopVar_child in node.ChildNodes)
            {
                child = tempLoopVar_child;
                if (child.Name == "providers")
                {
                    GetProviders(child);
                }
            }
        }
    }
}