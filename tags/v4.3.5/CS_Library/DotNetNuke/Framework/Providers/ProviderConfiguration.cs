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