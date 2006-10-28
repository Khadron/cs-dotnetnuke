using System;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.Schema;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Entities.Modules.Definitions
{
    public class ModuleDefinitionValidator : XmlValidatorBase
    {
        private string GetDnnSchemaPath(Stream xmlStream)
        {
            ModuleDefinitionVersion Version = GetModuleDefinitionVersion(xmlStream);
            string schemaPath = "";

            switch (Version)
            {
                case ModuleDefinitionVersion.V2:

                    schemaPath = "components\\ResourceInstaller\\ModuleDef_V2.xsd";
                    break;
                case ModuleDefinitionVersion.V3:

                    schemaPath = "components\\ResourceInstaller\\ModuleDef_V3.xsd";
                    break;
                case ModuleDefinitionVersion.V2_Skin:

                    schemaPath = "components\\ResourceInstaller\\ModuleDef_V2Skin.xsd";
                    break;
                case ModuleDefinitionVersion.V2_Provider:

                    schemaPath = "components\\ResourceInstaller\\ModuleDef_V2Provider.xsd";
                    break;
                case ModuleDefinitionVersion.VUnknown:

                    throw (new Exception(GetLocalizedString("EXCEPTION_LoadFailed")));
                    break;
            }
            return Path.Combine(Globals.ApplicationMapPath, schemaPath);
        }

        private string GetLocalizedString(string key)
        {
            PortalSettings objPortalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];

            if (objPortalSettings == null)
            {
                return key;
            }
            else
            {
                return Localization.GetString(key, objPortalSettings);
            }
        }

        public ModuleDefinitionVersion GetModuleDefinitionVersion(Stream xmlStream)
        {
            xmlStream.Seek(0, SeekOrigin.Begin);
            XmlTextReader xmlReader = new XmlTextReader(xmlStream);
            xmlReader.MoveToContent();

            //This test assumes provides a simple validation
            if (xmlReader.LocalName.ToLower() == "module")
            {
                return ModuleDefinitionVersion.V1;
            }
            if (xmlReader.LocalName.ToLower() == "dotnetnuke")
            {
                return ModuleDefinitionVersion.VUnknown;
            }

            if (xmlReader.GetAttribute("type") == "Module")
            {
                if (xmlReader.GetAttribute("version") == "2.0")
                {
                    return ModuleDefinitionVersion.V2;
                }
                else if (xmlReader.GetAttribute("version") == "3.0")
                {
                    return ModuleDefinitionVersion.V3;
                }
            }
            if (xmlReader.GetAttribute("type") == "SkinObject")
            {
                return ModuleDefinitionVersion.V2_Skin;
            }
            if (xmlReader.GetAttribute("type") == "Provider")
            {
                return ModuleDefinitionVersion.V2_Provider;
            }
            else
            {
                return ModuleDefinitionVersion.VUnknown;
            }
        }

        public override bool Validate(Stream XmlStream)
        {
            SchemaSet.Add("", GetDnnSchemaPath(XmlStream));
            return base.Validate(XmlStream);
        }
    }
}