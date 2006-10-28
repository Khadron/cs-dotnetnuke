using System.Xml.Schema;
using DotNetNuke.Common;

namespace DotNetNuke.Entities.Portals
{
    /// <Summary>
    /// The PortalTemplateValidator Class is used to validate the Portal Template
    /// </Summary>
    public class PortalTemplateValidator : XmlValidatorBase
    {
        public bool Validate( string xmlFilename, string schemaFileName )
        {
            this.SchemaSet.Add( "", schemaFileName );
            return this.Validate( xmlFilename );
        }
    }
}