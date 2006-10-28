using System.Configuration;
using System.Xml;

namespace DotNetNuke.Framework.Providers
{
    internal class ProviderConfigurationHandler : IConfigurationSectionHandler
    {
        public virtual object Create( object parent, object context, XmlNode node )
        {
            ProviderConfiguration providerConfiguration = new ProviderConfiguration();
            providerConfiguration.LoadValuesFromConfigurationXml( node );
            return providerConfiguration;
        }
    }
}