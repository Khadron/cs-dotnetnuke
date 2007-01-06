#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using System.Xml;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Security;

namespace DotNetNuke.Common.Utilities
{
    /// <Summary>The Config class provides access to the web.config file</Summary>
    public class Config
    {

        public static XmlDocument AddAppSetting(XmlDocument xmlDoc, string Key, string Value)
        {
            XmlElement xmlElement;

            // retrieve the appSettings node
            XmlNode xmlAppSettings = xmlDoc.SelectSingleNode("//appSettings");

            if (xmlAppSettings != null)
            {
                // get the node based on key
                XmlNode xmlNode = xmlAppSettings.SelectSingleNode("//add[@key='" + Key + "']");

                if (xmlNode != null)
                {
                    // update the existing element
                    xmlElement = (XmlElement)xmlNode;
                    xmlElement.SetAttribute("value", Value);
                }
                else
                {
                    // create a new element
                    xmlElement = xmlDoc.CreateElement("add");
                    xmlElement.SetAttribute("key", Key);
                    xmlElement.SetAttribute("value", Value);
                    xmlAppSettings.AppendChild(xmlElement);
                }
            }

            // return the xml doc
            return xmlDoc;
        }

        /// <summary>
        /// Gets the default connection String as specified in the provider.
        /// </summary>
        /// <returns>The connection String</returns>
        /// <remarks></remarks>
        public static string GetConnectionString()
        {
            ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration("data");

            // Read the configuration specific information for this provider
            Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            return GetConnectionString(objProvider.Attributes["connectionStringName"]);
        }

        /// <summary>
        /// Gets the specified connection String
        /// </summary>
        /// <param name="name">Name of Connection String to return</param>
        /// <returns>The connection String</returns>
        /// <remarks></remarks>
        public static string GetConnectionString(string name)
        {
            string connectionString = "";

            //First check if connection string is specified in <connectionstrings> (ASP.NET 2.0 / DNN v4.x)
            if (!String.IsNullOrEmpty(name))
            {
                //ASP.NET 2 version connection string (in <connectionstrings>)
                //This will be for new v4.x installs or upgrades from v4.x
                connectionString = WebConfigurationManager.ConnectionStrings[name].ConnectionString;
            }

            if (connectionString == "")
            {
                //Next check if connection string is specified in <appsettings> (ASP.NET 1.1 / DNN v3.x)
                //This will accomodate upgrades from v3.x
                if (!String.IsNullOrEmpty(name))
                {
                    connectionString = GetSetting(name);
                }
            }

            return connectionString;
        }

        public static object GetSection(string section)
        {
            return WebConfigurationManager.GetSection(section);
        }

        public static string GetSetting(string setting)
        {
            return WebConfigurationManager.AppSettings[setting];
        }

        public static XmlDocument Load()
        {
            // open the web.config file
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Globals.ApplicationMapPath + "\\web.config");
            return xmlDoc;
        }

        public static XmlDocument Load(string filename)
        {
            // open the config file
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Globals.ApplicationMapPath + "\\" + filename);
            return xmlDoc;
        }

        public static string Save(XmlDocument xmlDoc)
        {
            try
            {
                // save the config file
                XmlTextWriter writer = new XmlTextWriter(Globals.ApplicationMapPath + "\\web.config", null);
                writer.Formatting = Formatting.Indented;
                xmlDoc.WriteTo(writer);
                writer.Flush();
                writer.Close();
                return "";
            }
            catch (Exception exc)
            {
                // the file may be read-only or the file permissions may not be set properly
                return exc.Message;
            }
        }

        public static string Save(XmlDocument xmlDoc, string filename)
        {
            try
            {
                // save the config file
                XmlTextWriter writer = new XmlTextWriter(Globals.ApplicationMapPath + "\\" + filename, null);
                writer.Formatting = Formatting.Indented;
                xmlDoc.WriteTo(writer);
                writer.Flush();
                writer.Close();
                return "";
            }
            catch (Exception exc)
            {
                // the file may be read-only or the file permissions may not be set properly
                return exc.Message;
            }
        }

        public static bool Touch()
        {
            File.SetLastWriteTime(Globals.ApplicationMapPath + "\\web.config", DateTime.Now);
            return true;
        }

        public static XmlDocument UpdateMachineKey(XmlDocument xmlConfig)
        {
            PortalSecurity objSecurity = new PortalSecurity();
            string validationKey = objSecurity.CreateKey(20);
            string decryptionKey = objSecurity.CreateKey(24);

            XmlNode xmlMachineKey = xmlConfig.SelectSingleNode("configuration/system.web/machineKey");

            xmlMachineKey.Attributes["validationKey"].InnerText = validationKey;
            xmlMachineKey.Attributes["decryptionKey"].InnerText = decryptionKey;

            xmlConfig = AddAppSetting(xmlConfig, "InstallationDate", DateTime.Today.ToShortDateString());

            return xmlConfig;
        }
        public static void AddCodeSubDirectory(string name)
        {
            Configuration webConfig = WebConfigurationManager.OpenWebConfiguration(Globals.ApplicationPath);

            CompilationSection configSection = (CompilationSection)webConfig.GetSection("system.web/compilation");
            bool directoryFound = false;
            foreach (CodeSubDirectory codeSubDirectory in configSection.CodeSubDirectories)
            {
                if (codeSubDirectory.DirectoryName == name)
                {
                    directoryFound = true;
                    break;
                }
            }
        

            if (!directoryFound)
            {
                configSection.CodeSubDirectories.Add(new CodeSubDirectory(name));
                webConfig.Save();
            }
        }

        public static void RemoveCodeSubDirectory(string name)
        {
            Configuration webConfig = WebConfigurationManager.OpenWebConfiguration(Globals.ApplicationPath);

            CompilationSection configSection = (CompilationSection)webConfig.GetSection("system.web/compilation");
            bool directoryFound = false;
            foreach (CodeSubDirectory codeSubDirectory in configSection.CodeSubDirectories)
            {
                if (codeSubDirectory.DirectoryName == name)
                {
                    directoryFound = true;
                    break;
                }
            }
        

            if (directoryFound)
            {
                configSection.CodeSubDirectories.Remove(name);
                webConfig.Save();
            }
        }
    }
}