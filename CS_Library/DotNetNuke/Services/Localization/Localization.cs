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
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Globals=DotNetNuke.Common.Globals;
using TabInfo=DotNetNuke.Entities.Tabs.TabInfo;

namespace DotNetNuke.Services.Localization
{
    public class Localization
    {

        private enum CustomizedLocale
        {
            None = 0,
            Portal = 1,
            Host = 2,
        }
        public const string ApplicationResourceDirectory = "~/App_GlobalResources";
        public const string GlobalResourceFile = "~/App_GlobalResources/GlobalResources.resx";
        internal const string keyConst = "resourcekey";
        public const string LocalResourceDirectory = "App_LocalResources";
        public const string LocalSharedResourceFile = "SharedResources.resx";
        public const string SharedResourceFile = "~/App_GlobalResources/SharedResources.resx";
        public const string SupportedLocalesFile = "~/App_GlobalResources/Locales.xml";
        public const string SystemLocale = "en-US";
        public const int SystemTimeZoneOffset = -480;
        public const string TimezonesFile = "~/App_GlobalResources/TimeZones.xml";
        private static string _defaultKeyName;
        private static ListItem[] _timeZoneListItems;

        /// <Summary>
        /// The CurrentCulture returns the current Culture being used is 'key'.
        /// </Summary>
        public string CurrentCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.ToString();
            }
        }

        /// <Summary>
        /// The KeyName property returns and caches the name of the key attribute used to lookup resources.
        /// This can be configured by setting ResourceManagerKey property in the web.config file. The default value for this property
        /// is 'key'.
        /// </Summary>
        public static string KeyName
        {
            get
            {
                return _defaultKeyName;
            }
            set
            {
                _defaultKeyName = value;
                if (_defaultKeyName == null || _defaultKeyName == string.Empty)
                {
                    _defaultKeyName = keyConst;
                }
            }
        }

        static Localization()
        {
            _defaultKeyName = "resourcekey";
        }

        public string AddLocale( string Key, string Name )
        {
            XmlNode node;
            XmlDocument resDoc = new XmlDocument();
            XmlAttribute a;
            string result;

            resDoc.Load(HttpContext.Current.Server.MapPath(SupportedLocalesFile));
            node = resDoc.SelectSingleNode("//root/language[@key=\'" + Key + "\']");
            if (node != null)
            {
                result = "Duplicate.ErrorMessage";
            }
            else
            {
                node = resDoc.CreateElement("language");
                a = resDoc.CreateAttribute("name");
                a.Value = Name;
                node.Attributes.Append(a);
                a = resDoc.CreateAttribute("key");
                a.Value = Key;
                node.Attributes.Append(a);
                a = resDoc.CreateAttribute("fallback");
                a.Value = SystemLocale;
                node.Attributes.Append(a);
                resDoc.SelectSingleNode("//root").AppendChild(node);

                try
                {
                    resDoc.Save(HttpContext.Current.Server.MapPath(SupportedLocalesFile));
                    if (!File.Exists(HttpContext.Current.Server.MapPath(TimeZoneFile(TimezonesFile, Key))))
                    {
                        File.Copy(HttpContext.Current.Server.MapPath(TimezonesFile), HttpContext.Current.Server.MapPath(TimeZoneFile(TimezonesFile, Key)));
                        result = "NewLocale.ErrorMessage";
                    }
                    result = "";
                }
                catch
                {
                    result = "Save.ErrorMessage";
                }
            }
            return result;
        }

        public static LocaleCollection GetEnabledLocales()
        {
            string cacheKey = "";
            PortalSettings objPortalSettings = PortalController.GetCurrentPortalSettings();
            string FilePath = HttpContext.Current.Server.MapPath(ApplicationResourceDirectory + "/Locales.Portal-" + objPortalSettings.PortalId.ToString() + ".xml");

            if (File.Exists(FilePath))
            {
                //locales have been previously disabled
                cacheKey = "dotnetnuke-enabledlocales" + objPortalSettings.PortalId.ToString();
                LocaleCollection enabledLocales = (LocaleCollection)DataCache.GetCache(cacheKey);

                if (enabledLocales == null)
                {
                    enabledLocales = new LocaleCollection();
                    XmlDocument xmlInactiveLocales = new XmlDocument();
                    bool bXmlLoaded = false;

                    //load inactive locales xml file
                    try
                    {
                        xmlInactiveLocales.Load(FilePath);
                        bXmlLoaded = true;
                    }
                    catch
                    {
                    }

                    CacheDependency dp = new CacheDependency(FilePath);
                    XmlDocument d = new XmlDocument();
                    d.Load(FilePath);

                    LocaleCollection _SupportedLocales = GetSupportedLocales();
                    foreach (string _LocaleCode in _SupportedLocales.AllKeys)
                    {
                        if (!bXmlLoaded || xmlInactiveLocales.SelectSingleNode("//locales/inactive/locale[.=\'" + _LocaleCode + "\']") == null)
                        {
                            Locale objLocale = new Locale();
                            objLocale.Text = _SupportedLocales[_LocaleCode].Text;
                            objLocale.Code = _LocaleCode;
                            objLocale.Fallback = _SupportedLocales[_LocaleCode].Fallback;

                            enabledLocales.Add(_LocaleCode, objLocale);
                        }
                    }
                    if (Globals.PerformanceSetting != Globals.PerformanceSettings.NoCaching)
                    {
                        DataCache.SetCache(cacheKey, enabledLocales, dp);
                    }
                }

                return enabledLocales;
            }
            else
            {
                // if portal specific xml file does not exist, all locales have been enabled, so just return supportedlocales
                return GetSupportedLocales();
            }
        }

        public string GetFixedCurrency( decimal Expression, string Culture )
        {
            return GetFixedCurrency( Expression, Culture, -1, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault );
        }

        public string GetFixedCurrency( decimal Expression, string Culture, int NumDigitsAfterDecimal )
        {
            return GetFixedCurrency( Expression, Culture, NumDigitsAfterDecimal, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault );
        }

        public string GetFixedCurrency( decimal Expression, string Culture, int NumDigitsAfterDecimal, TriState IncludeLeadingDigit )
        {
            return GetFixedCurrency( Expression, Culture, NumDigitsAfterDecimal, IncludeLeadingDigit, TriState.UseDefault, TriState.UseDefault );
        }

        public string GetFixedCurrency( decimal Expression, string Culture, int NumDigitsAfterDecimal, TriState IncludeLeadingDigit, TriState UseParensForNegativeNumbers )
        {
            return GetFixedCurrency( Expression, Culture, NumDigitsAfterDecimal, IncludeLeadingDigit, UseParensForNegativeNumbers, TriState.UseDefault );
        }

        public string GetFixedCurrency( decimal Expression, string Culture, int NumDigitsAfterDecimal, TriState IncludeLeadingDigit, TriState UseParensForNegativeNumbers, TriState GroupDigits )
        {
            string oldCurrentCulture = CurrentCulture;
            CultureInfo newCulture = new CultureInfo(Culture);
            Thread.CurrentThread.CurrentUICulture = newCulture;
            Thread.CurrentThread.CurrentCulture = newCulture;
            string currencyStr = Strings.FormatCurrency(Expression, NumDigitsAfterDecimal, IncludeLeadingDigit, UseParensForNegativeNumbers, GroupDigits);
            CultureInfo oldCulture = new CultureInfo(oldCurrentCulture);
            Thread.CurrentThread.CurrentUICulture = oldCulture;
            Thread.CurrentThread.CurrentCulture = oldCulture;

            return currencyStr;
        }

        public string GetFixedDate( DateTime Expression, string Culture )
        {
            return GetFixedDate( Expression, Culture, DateFormat.GeneralDate, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault );
        }

        public string GetFixedDate( DateTime Expression, string Culture, DateFormat NamedFormat )
        {
            return GetFixedDate( Expression, Culture, NamedFormat, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault );
        }

        public string GetFixedDate( DateTime Expression, string Culture, DateFormat NamedFormat, TriState IncludeLeadingDigit )
        {
            return GetFixedDate( Expression, Culture, NamedFormat, IncludeLeadingDigit, TriState.UseDefault, TriState.UseDefault );
        }

        public string GetFixedDate( DateTime Expression, string Culture, DateFormat NamedFormat, TriState IncludeLeadingDigit, TriState UseParensForNegativeNumbers )
        {
            return GetFixedDate( Expression, Culture, NamedFormat, IncludeLeadingDigit, UseParensForNegativeNumbers, TriState.UseDefault );
        }

        public string GetFixedDate( DateTime Expression, string Culture, DateFormat NamedFormat, TriState IncludeLeadingDigit, TriState UseParensForNegativeNumbers, TriState GroupDigits )
        {
            string oldCurrentCulture = CurrentCulture;
            CultureInfo newCulture = new CultureInfo(Culture);
            Thread.CurrentThread.CurrentUICulture = newCulture;
            Thread.CurrentThread.CurrentCulture = newCulture;
            string dateStr = Strings.FormatDateTime(Expression, NamedFormat);
            CultureInfo oldCulture = new CultureInfo(oldCurrentCulture);
            Thread.CurrentThread.CurrentUICulture = oldCulture;
            Thread.CurrentThread.CurrentCulture = oldCulture;

            return dateStr;
        }

        /// <Summary>
        /// GetResource is used by GetString to load the resources Hashtable
        /// </Summary>
        /// <Param name="ResourceFileRoot">The Local Resource root</Param>
        /// <Param name="objPortalSettings">The current portals Portal Settings</Param>
        /// <Param name="strLanguage">
        /// A specific language used to get the resource
        /// </Param>
        /// <Returns>The resources Hashtable</Returns>
        private static Hashtable GetResource( string ResourceFileRoot, PortalSettings objPortalSettings, string strLanguage )
        {
            Hashtable resources;
            string defaultLanguage = objPortalSettings.DefaultLanguage.ToLower();
            string userLanguage;
            string fallbackLanguage = SystemLocale.ToLower();
            string userFile;
            string defaultFile;
            string fallbackFile;

            if (strLanguage == null)
            {
                userLanguage = Thread.CurrentThread.CurrentCulture.ToString().ToLower();
            }
            else
            {
                userLanguage = strLanguage;
            }

            // Ensure the user has a language set
            if (userLanguage == "")
            {
                userLanguage = defaultLanguage;
            }

            Locale userLocale = GetSupportedLocales()[userLanguage];
            if ((userLocale != null) && (userLocale.Fallback.ToLower() != ""))
            {
                fallbackLanguage = userLocale.Fallback.ToLower();
            }

            //Get the filename for the userlanguage version of the resource file
            userFile = GetResourceFileName(ResourceFileRoot, userLanguage);

            //Set the cachekey as the userFile + the PortalId
            string filePath = HttpContext.Current.Server.MapPath(userFile);
            string cacheKey = filePath.Replace(Globals.ApplicationMapPath, "").ToLower() + objPortalSettings.PortalId.ToString();

            //Attempt to get the resources from the cache
            resources = (Hashtable)DataCache.GetCache(cacheKey);

            if (resources == null)
            {
                // resources not in Cache so load from Files

                //Create resources Hashtable
                resources = new Hashtable();

                //First Load the Fallback Language ensuring that the keys are loaded
                fallbackFile = GetResourceFileName(ResourceFileRoot, fallbackLanguage);
                resources = LoadResource(resources, fallbackLanguage, cacheKey, fallbackFile, CustomizedLocale.None, objPortalSettings);
                // Add any host level customizations
                resources = LoadResource(resources, fallbackLanguage, cacheKey, fallbackFile, CustomizedLocale.Host, objPortalSettings);
                // Add any portal level customizations
                resources = LoadResource(resources, fallbackLanguage, cacheKey, fallbackFile, CustomizedLocale.Portal, objPortalSettings);

                //if the defaultLanguage is different, load it
                if (defaultLanguage != "" && defaultLanguage != fallbackLanguage && userLanguage != fallbackLanguage)
                {
                    defaultFile = GetResourceFileName(ResourceFileRoot, defaultLanguage);
                    resources = LoadResource(resources, defaultLanguage, cacheKey, defaultFile, CustomizedLocale.None, objPortalSettings);
                    // Add any host level customizations
                    resources = LoadResource(resources, defaultLanguage, cacheKey, defaultFile, CustomizedLocale.Host, objPortalSettings);
                    // Add any portal level customizations
                    resources = LoadResource(resources, defaultLanguage, cacheKey, defaultFile, CustomizedLocale.Portal, objPortalSettings);
                }

                // If the user language is different load it
                if (userLanguage != defaultLanguage && userLanguage != fallbackLanguage)
                {
                    resources = LoadResource(resources, userLanguage, cacheKey, userFile, CustomizedLocale.None, objPortalSettings);
                    // Add any host level customizations
                    resources = LoadResource(resources, userLanguage, cacheKey, userFile, CustomizedLocale.Host, objPortalSettings);
                    // Add any portal level customizations
                    resources = LoadResource(resources, userLanguage, cacheKey, userFile, CustomizedLocale.Portal, objPortalSettings);
                }
            }

            return resources;
        }

        /// <Summary>
        /// GetResource is used by GetString to load the resources Hashtable
        /// </Summary>
        /// <Param name="ResourceFileRoot">The Local Resource root</Param>
        /// <Param name="objPortalSettings">The current portals Portal Settings</Param>
        /// <Returns>The rsources Hashtable</Returns>
        private static Hashtable GetResource( string ResourceFileRoot, PortalSettings objPortalSettings )
        {
            return GetResource( ResourceFileRoot, objPortalSettings, ( (string)null ) );
        }

        public static string GetResourceFile( Control Ctrl, string FileName )
        {
            return ( Ctrl.TemplateSourceDirectory + "/App_LocalResources/" + FileName );
        }

        /// <Summary>
        /// GetResourceFileName is used to build the resource file name according to the
        /// language
        /// </Summary>
        /// <Param name="ResourceFileRoot">The resource file root</Param>
        /// <Param name="language">The language</Param>
        /// <Returns>The language specific resource file</Returns>
        private static string GetResourceFileName( string ResourceFileRoot, string language )
        {
            string ResourceFile;

            if (ResourceFileRoot != null)
            {
                if (language == SystemLocale.ToLower() || language == "")
                {
                    switch (ResourceFileRoot.Substring(ResourceFileRoot.Length - 5, 5).ToLower())
                    {
                        case ".resx":

                            ResourceFile = ResourceFileRoot;
                            break;
                        case ".ascx":

                            ResourceFile = ResourceFileRoot + ".resx";
                            break;
                        default:

                            ResourceFile = ResourceFileRoot + ".ascx.resx"; // a portal module
                            break;
                    }
                }
                else
                {
                    switch (ResourceFileRoot.Substring(ResourceFileRoot.Length - 5, 5).ToLower())
                    {
                        case ".resx":

                            ResourceFile = ResourceFileRoot.Replace(".resx", "." + language + ".resx");
                            break;
                        case ".ascx":

                            ResourceFile = ResourceFileRoot.Replace(".ascx", ".ascx." + language + ".resx");
                            break;
                        default:

                            ResourceFile = ResourceFileRoot + ".ascx." + language + ".resx"; // a portal module
                            break;
                    }
                }
            }
            else
            {
                if (language == SystemLocale.ToLower() || language == "")
                {
                    ResourceFile = SharedResourceFile;
                }
                else
                {
                    ResourceFile = SharedResourceFile.Replace(".resx", "." + language + ".resx");
                }
            }

            return ResourceFile;
        }

        /// <Summary>
        /// GetString gets the localized string corresponding to the resourcekey
        /// </Summary>
        /// <Param name="name">The resourcekey to find</Param>
        /// <Param name="ResourceFileRoot">The Local Resource root</Param>
        /// <Param name="objPortalSettings">The current portals Portal Settings</Param>
        /// <Param name="strLanguage">A specific language to lookup the string</Param>
        /// <Returns>The localized Text</Returns>
        public static string GetString( string name, string ResourceFileRoot, PortalSettings objPortalSettings, string strLanguage )
        {
            //Load the Local Resource Files resources
            Hashtable resources = GetResource(ResourceFileRoot, objPortalSettings, strLanguage);

            //make the default translation property ".Text"
            if (name.IndexOf(".") < 1)
            {
                name += ".Text";
            }

            //If the key can't be found try the Local Shared Resource File resources
            if (ResourceFileRoot != null && (resources == null || resources[name] == null))
            {
                //try to use a module specific shared resource file
                string localSharedFile = ResourceFileRoot.Substring(0, ResourceFileRoot.LastIndexOf("/") + 1) + LocalSharedResourceFile;
                resources = GetResource(localSharedFile, objPortalSettings);
            }

            //If the key can't be found try the Shared Resource Files resources
            if (resources == null || resources[name] == null)
            {
                resources = GetResource(SharedResourceFile, objPortalSettings);
            }

            //check for setting in web.config
            bool showMissingKeys;
            if (Config.GetSetting("ShowMissingKeys") == null)
            {
                showMissingKeys = false;
            }
            else
            {
                showMissingKeys = bool.Parse(Config.GetSetting("ShowMissingKeys"));
            }
            //If the key still can't be found then it doesn't exist in the Localization Resources
            if (showMissingKeys)
            {
                if (resources == null || resources[name] == null)
                {
                    return "RESX:" + name;
                }
                else
                {
                    return "[L]" + resources[name].ToString();
                }
            }

            return resources[name].ToString();
        }

        /// <Summary>
        /// GetString gets the localized string corresponding to the resourcekey
        /// </Summary>
        /// <Param name="name">The resourcekey to find</Param>
        /// <Param name="ResourceFileRoot">The Local Resource root</Param>
        /// <Param name="strLanguage">A specific language to lookup the string</Param>
        /// <Returns>The localized Text</Returns>
        public static string GetString( string name, string ResourceFileRoot, string strlanguage )
        {
            PortalSettings settings = ( (PortalSettings)HttpContext.Current.Items["PortalSettings"] );
            return GetString( name, ResourceFileRoot, settings, strlanguage );
        }

        /// <Summary>
        /// GetString gets the localized string corresponding to the resourcekey
        /// </Summary>
        /// <Param name="name">The resourcekey to find</Param>
        /// <Param name="ResourceFileRoot">The Local Resource root</Param>
        /// <Returns>The localized Text</Returns>
        public static string GetString( string name, string ResourceFileRoot )
        {
            PortalSettings settings = ( (PortalSettings)HttpContext.Current.Items["PortalSettings"] );
            return GetString( name, ResourceFileRoot, settings, ( (string)null ) );
        }

        /// <Summary>
        /// GetString gets the localized string corresponding to the resourcekey
        /// </Summary>
        /// <Param name="name">The resourcekey to find</Param>
        /// <Param name="objPortalSettings">The current portals Portal Settings</Param>
        /// <Returns>The localized Text</Returns>
        public static string GetString( string name, PortalSettings objPortalSettings )
        {
            return GetString( name, ( (string)null ), objPortalSettings, ( (string)null ) );
        }

        /// <Summary>
        /// GetString gets the localized string corresponding to the resourcekey
        /// </Summary>
        /// <Param name="name">The resourcekey to find</Param>
        /// <Returns>The localized Text</Returns>
        public static string GetString( string name )
        {
            PortalSettings settings = ( (PortalSettings)HttpContext.Current.Items["PortalSettings"] );
            return GetString( name, ( (string)null ), settings, ( (string)null ) );
        }

        /// <Summary>
        /// GetSupportedLocales returns the list of locales from the locales.xml file.
        /// </Summary>
        public static LocaleCollection GetSupportedLocales()
        {
            string cacheKey = "dotnetnuke-supportedlocales";
            LocaleCollection supportedLocales = (LocaleCollection)DataCache.GetCache(cacheKey);

            if (supportedLocales == null)
            {
                supportedLocales = new LocaleCollection();
                string filePath = Globals.ApplicationMapPath + SupportedLocalesFile.Substring(1).Replace("/", "\\");

                if (!File.Exists(filePath))
                {
                    try
                    {
                        // First access to locales.xml, create using template
                        File.Copy(Globals.ApplicationMapPath + ApplicationResourceDirectory.Substring(1).Replace("/", "\\") + "\\Locales.xml.config", Globals.ApplicationMapPath + SupportedLocalesFile.Substring(1).Replace("/", "\\"));
                    }
                    catch
                    {
                        Locale objLocale = new Locale();
                        objLocale.Text = "English";
                        objLocale.Code = "en-US";
                        objLocale.Fallback = "";
                        supportedLocales.Add("en-US", objLocale);
                        return supportedLocales; //Will be Empty and not cached
                    }
                }

                CacheDependency dp = new CacheDependency(filePath);

                XmlDocument d = new XmlDocument();
                d.Load(filePath);

                XmlNode n;
                foreach (XmlNode tempLoopVar_n in d.SelectNodes("root/language"))
                {
                    n = tempLoopVar_n;
                    if (n.NodeType != XmlNodeType.Comment)
                    {
                        Locale objLocale = new Locale();
                        objLocale.Text = n.Attributes["name"].Value;
                        objLocale.Code = n.Attributes["key"].Value;
                        objLocale.Fallback = n.Attributes["fallback"].Value;

                        supportedLocales.Add(n.Attributes["key"].Value, objLocale);
                    }
                }
                if (Globals.PerformanceSetting != Globals.PerformanceSettings.NoCaching)
                {
                    DataCache.SetCache(cacheKey, supportedLocales, dp);
                }
            }

            return supportedLocales;
        }

        /// <Summary>Gets a SystemMessage.</Summary>
        /// <Param name="objPortal">
        /// The portal settings for the portal to which the message will affect.
        /// </Param>
        /// <Param name="MessageName">
        /// The message tag which identifies the SystemMessage.
        /// </Param>
        /// <Returns>The message body with all tags replaced.</Returns>
        public static string GetSystemMessage( PortalSettings objPortal, string MessageName )
        {
            return GetSystemMessage( ( (string)null ), objPortal, MessageName, ( (UserInfo)null ), "~/App_GlobalResources/GlobalResources.resx", ( (ArrayList)null ) );
        }

        /// <Summary>Gets a SystemMessage.</Summary>
        /// <Param name="objPortal">
        /// The portal settings for the portal to which the message will affect.
        /// </Param>
        /// <Param name="MessageName">
        /// The message tag which identifies the SystemMessage.
        /// </Param>
        /// <Param name="objUser">
        /// Reference to the user used to personalize the message.
        /// </Param>
        /// <Param name="ResourceFile">
        /// The root name of the Resource File where the localized text can be found
        /// </Param>
        /// <Returns>The message body with all tags replaced.</Returns>
        public static string GetSystemMessage( PortalSettings objPortal, string MessageName, UserInfo objUser, string ResourceFile )
        {
            return GetSystemMessage( ( (string)null ), objPortal, MessageName, objUser, ResourceFile, ( (ArrayList)null ) );
        }

        /// <Summary>
        /// Gets a SystemMessage passing extra custom parameters to personalize.
        /// </Summary>
        /// <Param name="objPortal">
        /// The portal settings for the portal to which the message will affect.
        /// </Param>
        /// <Param name="MessageName">
        /// The message tag which identifies the SystemMessage.
        /// </Param>
        /// <Param name="ResourceFile">
        /// The root name of the Resource File where the localized text can be found
        /// </Param>
        /// <Param name="Custom">An ArrayList with replacements for custom tags.</Param>
        /// <Returns>The message body with all tags replaced.</Returns>
        public static string GetSystemMessage( PortalSettings objPortal, string MessageName, string ResourceFile, ArrayList Custom )
        {
            return GetSystemMessage( ( (string)null ), objPortal, MessageName, ( (UserInfo)null ), ResourceFile, Custom );
        }

        /// <Summary>''' Gets a SystemMessage.</Summary>
        /// <Param name="strLanguage">
        /// A specific language to get the SystemMessage for.
        /// </Param>
        /// <Param name="objPortal">
        /// The portal settings for the portal to which the message will affect.
        /// </Param>
        /// <Param name="MessageName">
        /// The message tag which identifies the SystemMessage.
        /// </Param>
        /// <Param name="objUser">
        /// Reference to the user used to personalize the message.
        /// </Param>
        /// <Returns>The message body with all tags replaced.</Returns>
        public static string GetSystemMessage( string strLanguage, PortalSettings objPortal, string MessageName, UserInfo objUser )
        {
            return GetSystemMessage( strLanguage, objPortal, MessageName, objUser, "~/App_GlobalResources/GlobalResources.resx", ( (ArrayList)null ) );
        }

        /// <Summary>Gets a SystemMessage.</Summary>
        /// <Param name="objPortal">
        /// The portal settings for the portal to which the message will affect.
        /// </Param>
        /// <Param name="MessageName">
        /// The message tag which identifies the SystemMessage.
        /// </Param>
        /// <Param name="ResourceFile">
        /// The root name of the Resource File where the localized text can be found
        /// </Param>
        /// <Returns>The message body with all tags replaced.</Returns>
        public static string GetSystemMessage( PortalSettings objPortal, string MessageName, string ResourceFile )
        {
            return GetSystemMessage( ( (string)null ), objPortal, MessageName, ( (UserInfo)null ), ResourceFile, ( (ArrayList)null ) );
        }

        /// <Summary>Gets a SystemMessage.</Summary>
        /// <Param name="objPortal">
        /// The portal settings for the portal to which the message will affect.
        /// </Param>
        /// <Param name="MessageName">
        /// The message tag which identifies the SystemMessage.
        /// </Param>
        /// <Param name="objUser">
        /// Reference to the user used to personalize the message.
        /// </Param>
        /// <Returns>The message body with all tags replaced.</Returns>
        public static string GetSystemMessage( PortalSettings objPortal, string MessageName, UserInfo objUser )
        {
            return GetSystemMessage( ( (string)null ), objPortal, MessageName, objUser, "~/App_GlobalResources/GlobalResources.resx", ( (ArrayList)null ) );
        }

        /// <Summary>
        /// Gets a SystemMessage passing extra custom parameters to personalize.
        /// </Summary>
        /// <Param name="strLanguage">
        /// A specific language to get the SystemMessage for.
        /// </Param>
        /// <Param name="objPortal">
        /// The portal settings for the portal to which the message will affect.
        /// </Param>
        /// <Param name="MessageName">
        /// The message tag which identifies the SystemMessage.
        /// </Param>
        /// <Param name="objUser">
        /// Reference to the user used to personalize the message.
        /// </Param>
        /// <Param name="ResourceFile">
        /// The root name of the Resource File where the localized text can be found
        /// </Param>
        /// <Param name="Custom">An ArrayList with replacements for custom tags.</Param>
        /// <Returns>The message body with all tags replaced.</Returns>
        public static string GetSystemMessage( string strLanguage, PortalSettings objPortal, string MessageName, UserInfo objUser, string ResourceFile, ArrayList Custom )
        {
            string strMessageValue;
            strMessageValue = GetString(MessageName, ResourceFile, strLanguage);

            if (strMessageValue != "")
            {
                string strKey;

                // host values
                if (Strings.InStr(1, strMessageValue, "Host:", CompareMethod.Text) != 0)
                {
                    Hashtable objHostSettings = HostSettings.GetSecureHostSettings();
                    foreach (string tempLoopVar_strKey in objHostSettings.Keys)
                    {
                        strKey = tempLoopVar_strKey;
                        if (Strings.InStr(1, strMessageValue, "[Host:" + strKey + "]", CompareMethod.Text) != 0)
                        {
                            strMessageValue = Strings.Replace(strMessageValue, "[Host:" + strKey + "]", objHostSettings[strKey].ToString(), 1, -1, CompareMethod.Text);
                        }
                    }
                }

                // get portal values
                if (Strings.InStr(1, strMessageValue, "Portal:", CompareMethod.Text) != 0)
                {
                    if (objPortal != null)
                    {
                        strMessageValue = PersonalizeSystemMessage(strMessageValue, "Portal:", objPortal, typeof(PortalSettings));
                    }
                    strMessageValue = Strings.Replace(strMessageValue, "[Portal:URL]", objPortal.PortalAlias.HTTPAlias, 1, -1, CompareMethod.Text);
                }

                // get user values
                if ((objUser != null) && (objPortal != null))
                {
                    if (Strings.InStr(1, strMessageValue, "User:", CompareMethod.Text) != 0)
                    {
                        strMessageValue = PersonalizeSystemMessage(strMessageValue, "User:", objUser, typeof(UserInfo));
                        strMessageValue = Strings.Replace(strMessageValue, "[User:VerificationCode]", objPortal.PortalId.ToString() + "-" + objUser.UserID.ToString(), 1, -1, CompareMethod.Text);
                    }
                    if (Strings.InStr(1, strMessageValue, "Membership:", CompareMethod.Text) != 0)
                    {
                        if (objUser.IsSuperUser)
                        {
                            objUser.Membership.Password = "xxxxxx";
                        }
                        strMessageValue = PersonalizeSystemMessage(strMessageValue, "Membership:", objUser.Membership, typeof(UserMembership));
                    }
                    if (Strings.InStr(1, strMessageValue, "Profile:", CompareMethod.Text) != 0)
                    {
                        strMessageValue = PersonalizeSystemMessage(strMessageValue, "Profile:", objUser.Profile, typeof(UserProfile));
                    }
                }

                // custom
                if (Strings.InStr(1, strMessageValue, "Custom:", CompareMethod.Text) != 0 && Custom != null)
                {

                    int intIndex;
                    for (intIndex = 0; intIndex <= Custom.Count - 1; intIndex++)
                    {
                        strMessageValue = Strings.Replace(strMessageValue, "[Custom:" + intIndex.ToString() + "]", (intIndex).ToString(), 1, -1, CompareMethod.Text);
                    }

                }

                // constants
                CultureInfo ci;
                if (strLanguage != null)
                {
                    ci = new CultureInfo(strLanguage);
                }
                else
                {
                    ci = new CultureInfo(Thread.CurrentThread.CurrentCulture.ToString().ToLower());
                }

                strMessageValue = Strings.Replace(strMessageValue, "[Date:Current]", DateTime.Now.ToString("D", ci), 1, -1, CompareMethod.Text);
            }

            return strMessageValue;
        }

        /// <Summary>
        /// Gets a SystemMessage passing extra custom parameters to personalize.
        /// </Summary>
        /// <Param name="objPortal">
        /// The portal settings for the portal to which the message will affect.
        /// </Param>
        /// <Param name="MessageName">
        /// The message tag which identifies the SystemMessage.
        /// </Param>
        /// <Param name="objUser">
        /// Reference to the user used to personalize the message.
        /// </Param>
        /// <Param name="ResourceFile">
        /// The root name of the Resource File where the localized text can be found
        /// </Param>
        /// <Param name="Custom">An ArrayList with replacements for custom tags.</Param>
        /// <Returns>The message body with all tags replaced.</Returns>
        public static string GetSystemMessage( PortalSettings objPortal, string MessageName, UserInfo objUser, string ResourceFile, ArrayList Custom )
        {
            return GetSystemMessage( ( (string)null ), objPortal, MessageName, objUser, ResourceFile, Custom );
        }

        public static NameValueCollection GetTimeZones( string language )
        {
            language = language.ToLower();
            string cacheKey = "dotnetnuke-" + language + "-timezones";

            string TranslationFile;

            if (language == SystemLocale.ToLower())
            {
                TranslationFile = TimezonesFile;
            }
            else
            {
                TranslationFile = TimezonesFile.Replace(".xml", "." + language + ".xml");
            }

            NameValueCollection timeZones = (NameValueCollection)DataCache.GetCache(cacheKey);

            if (timeZones == null)
            {
                string filePath = HttpContext.Current.Server.MapPath(TranslationFile);
                timeZones = new NameValueCollection();
                if (File.Exists(filePath) == false)
                {
                    return timeZones;
                }
                CacheDependency dp = new CacheDependency(filePath);
                try
                {
                    XmlDocument d = new XmlDocument();
                    d.Load(filePath);

                    XmlNode n;
                    foreach (XmlNode tempLoopVar_n in d.SelectSingleNode("root").ChildNodes)
                    {
                        n = tempLoopVar_n;
                        if (n.NodeType != XmlNodeType.Comment)
                        {
                            timeZones.Add(n.Attributes["name"].Value, n.Attributes["key"].Value);
                        }
                    }
                }
                catch (Exception)
                {
                }
                if (Globals.PerformanceSetting != Globals.PerformanceSettings.NoCaching)
                {
                    DataCache.SetCache(cacheKey, timeZones, dp);
                }
            }

            return timeZones;
        }

        private static Hashtable LoadResource( Hashtable target, string language, string cacheKey, string ResourceFile, CustomizedLocale CheckCustomCulture, PortalSettings objPortalSettings )
        {
            string filePath;
            string f = Null.NullString;

            //Are we looking for customised resources
            switch (CheckCustomCulture)
            {
                case CustomizedLocale.None:

                    f = ResourceFile;
                    break;
                case CustomizedLocale.Portal:

                    f = ResourceFile.Replace(".resx", ".Portal-" + objPortalSettings.PortalId.ToString() + ".resx");
                    break;
                case CustomizedLocale.Host:

                    f = ResourceFile.Replace(".resx", ".Host.resx");
                    break;
            }

            //If the filename is empty or the file does not exist return the Hashtable
            if (f == null || File.Exists(HttpContext.Current.Server.MapPath(f)) == false)
            {
                return target;
            }

            filePath = HttpContext.Current.Server.MapPath(f);

            CacheDependency dp = new CacheDependency(filePath);
            bool xmlLoaded = false;
            XmlDocument d = new XmlDocument();
            try
            {
                d.Load(filePath);
                xmlLoaded = true;
            }
            catch //exc As Exception
            {
                xmlLoaded = false;
            }
            if (xmlLoaded)
            {
                XmlNode n;
                foreach (XmlNode tempLoopVar_n in d.SelectNodes("root/data"))
                {
                    n = tempLoopVar_n;
                    if (n.NodeType != XmlNodeType.Comment)
                    {
                        string val = n.SelectSingleNode("value").InnerText;
                        if (target[n.Attributes["name"].Value] == null)
                        {
                            target.Add(n.Attributes["name"].Value, val);
                        }
                        else
                        {
                            target[n.Attributes["name"].Value] = val;
                        }
                    }
                }

                int CacheMins;
                CacheMins = 3 * Convert.ToInt32(Globals.PerformanceSetting);

                if (CacheMins > 0)
                {
                    DataCache.SetCache(cacheKey, target, dp, DateTime.MaxValue, new TimeSpan(0, CacheMins, 0));
                }
            }
            return target;
        }

        public static bool LocaleIsEnabled( Locale locale )
        {
            
            string string1 = locale.Code;
            return LocaleIsEnabled( ref string1 );            
        }

        public static bool LocaleIsEnabled( ref string localeCode )
        {
            try
            {
                bool isEnabled = false;
                LocaleCollection collEnabledLocales = GetEnabledLocales();
                if (collEnabledLocales[localeCode] == null)
                {
                    //if localecode is neutral (en, es,...) try to find a locale that has the same language
                    if (localeCode.IndexOf("-") == -1)
                    {
                        for (int i = 0; i <= collEnabledLocales.Count - 1; i++)
                        {
                            if (Convert.ToString(collEnabledLocales[i].Key).Split('-')[0] == localeCode)
                            {
                                //set the requested _localecode to the full locale
                                localeCode = Convert.ToString(collEnabledLocales[i].Key);
                                isEnabled = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    isEnabled = true;
                }
                return isEnabled;
            }
            catch (Exception)
            {
                //item could not be retrieved  or error
                return false;
            }
        }

        /// <Summary>Localizes ModuleControl Titles</Summary>
        /// <Param name="ControlTitle">Current control title</Param>
        /// <Param name="ControlSrc">Control Source</Param>
        /// <Param name="Key">Control Key</Param>
        /// <Returns>Localized control title if found</Returns>
        public static string LocalizeControlTitle( string ControlTitle, string ControlSrc, string Key )
        {
            string reskey;
            reskey = "ControlTitle_" + Key.ToLower() + ".Text";
            string ResFile = ControlSrc.Substring(0, ControlSrc.LastIndexOf("/") + 1) + LocalResourceDirectory + ControlSrc.Substring(ControlSrc.LastIndexOf("/"), ControlSrc.LastIndexOf(".") - ControlSrc.LastIndexOf("/"));
            string localizedvalue = GetString(reskey, ResFile);
            if (localizedvalue != null)
            {
                return localizedvalue;
            }
            else
            {
                return ControlTitle;
            }
        }

        /// <Summary>Localizes the "Built In" Roles</Summary>
        public static string LocalizeRole( string role )
        {
            string localRole;

            if (role == Globals.glbRoleAllUsersName)
            {
                string roleKey = role.Replace(" ", "");
                localRole = GetString(roleKey);
            }
            else if (role == Globals.glbRoleSuperUserName)
            {
                string roleKey = role.Replace(" ", "");
                localRole = GetString(roleKey);
            }
            else if (role == Globals.glbRoleUnauthUserName)
            {
                string roleKey = role.Replace(" ", "");
                localRole = GetString(roleKey);
            }
            else
            {
                localRole = role;
            }

            return localRole;
        }

        private static ArrayList LocalizeTabStripDetails( ArrayList arrTabStripDetails )
        {
            int i;
            for (i = 0; i <= arrTabStripDetails.Count - 1; i++)
            {
                TabInfo objTab = (TabInfo)arrTabStripDetails[i];
                if (objTab.IsAdminTab)
                {
                    string strLocalizedTabName = GetString(objTab.TabName + ".String", GlobalResourceFile);
                    if (strLocalizedTabName != "")
                    {
                        objTab.TabName = strLocalizedTabName;
                        objTab.Title = "";
                    }
                }
            }
            return arrTabStripDetails;
        }

        /// <Summary>Does the replacement of tags on a givem message.</Summary>
        /// <Param name="MessageValue">The message to be formatted.</Param>
        /// <Param name="Prefix">The prefix to look for supported tags.</Param>
        /// <Param name="objObject">
        /// An object used to look for replacement values.
        /// </Param>
        /// <Param name="objType">
        /// Object type for the object used to look for replacement values.
        /// </Param>
        /// <Returns>The message with the found tags replaced.</Returns>
        private static string PersonalizeSystemMessage( string MessageValue, string Prefix, object objObject, Type objType )
        {
            int intProperty;
            string strPropertyName = "";
            string strPropertyValue = "";

            ArrayList objProperties = CBO.GetPropertyInfo(objType);

            for (intProperty = 0; intProperty <= objProperties.Count - 1; intProperty++)
            {
                strPropertyName = ((PropertyInfo)objProperties[intProperty]).Name;
                if (Strings.InStr(1, MessageValue, "[" + Prefix + strPropertyName + "]", CompareMethod.Text) != 0)
                {
                    PropertyInfo propInfo = (PropertyInfo)objProperties[intProperty];
                    object propValue = propInfo.GetValue(objObject, null);

                    if (propValue != null)
                    {
                        strPropertyValue = propValue.ToString();
                    }

                    // special case for encrypted passwords
                    if ((Prefix + strPropertyName == "Membership:Password") && Convert.ToString(Globals.HostSettings["EncryptionKey"]) != "")
                    {
                        PortalSecurity objSecurity = new PortalSecurity();
                        strPropertyValue = objSecurity.Decrypt(Globals.HostSettings["EncryptionKey"].ToString(), strPropertyValue);
                    }

                    MessageValue = Strings.Replace(MessageValue, "[" + Prefix + strPropertyName + "]", strPropertyValue, 1, -1, CompareMethod.Text);
                }
            }

            return MessageValue;
        }

        /// <Summary>
        /// Returns the TimeZone file name for a given resource and language
        /// </Summary>
        /// <Param name="filename">Resource File</Param>
        /// <Param name="language">Language</Param>
        /// <Returns>Localized File Name</Returns>
        private string TimeZoneFile( string filename, string language )
        {
            if (language == SystemLocale)
            {
                return filename;
            }
            else
            {
                return filename.Substring(0, filename.Length - 4) + "." + language + ".xml";
            }
        }

        /// <Summary>
        /// LoadCultureDropDownList loads a DropDownList with the list of supported cultures
        /// based on the languages defined in the supported locales file
        /// </Summary>
        /// <Param name="list">DropDownList to load</Param>
        /// <Param name="displayType">
        /// Format of the culture to display. Must be one the CultureDropDownTypes values.
        /// for list of allowable values
        /// </Param>
        /// <Param name="selectedValue">Name of the default culture to select</Param>
        public static void LoadCultureDropDownList( DropDownList list, CultureDropDownTypes displayType, string selectedValue )
        {
            LocaleCollection supportedLanguages = GetSupportedLocales();
            ListItem[] _cultureListItems = new ListItem[supportedLanguages.Count - 1 + 1];
            CultureDropDownTypes _cultureListItemsType = displayType;
            PortalSettings objPortalSettings = PortalController.GetCurrentPortalSettings();
            XmlDocument xmlLocales = new XmlDocument();
            bool bXmlLoaded = false;
            int intAdded = 0;

            if (File.Exists(HttpContext.Current.Server.MapPath(ApplicationResourceDirectory + "/Locales.Portal-" + objPortalSettings.PortalId.ToString() + ".xml")))
            {
                try
                {
                    xmlLocales.Load(HttpContext.Current.Server.MapPath(ApplicationResourceDirectory + "/Locales.Portal-" + objPortalSettings.PortalId.ToString() + ".xml"));
                    bXmlLoaded = true;
                }
                catch
                {
                }
            }

            int i;
            for (i = 0; i <= supportedLanguages.Count - 1; i++)
            {
                // Only load active locales
                if (!bXmlLoaded || xmlLocales.SelectSingleNode("//locales/inactive/locale[.=\'" + ((Locale)supportedLanguages[i].Value).Code + "\']") == null)
                {
                    // Create a CultureInfo class based on culture
                    CultureInfo info = CultureInfo.CreateSpecificCulture(((Locale)supportedLanguages[i].Value).Code);

                    // Create and initialize a new ListItem
                    ListItem item = new ListItem();
                    item.Value = ((Locale)supportedLanguages[i].Value).Code;

                    // Based on the display type desired by the user, select the correct property
                    switch (displayType)
                    {
                        case CultureDropDownTypes.EnglishName:

                            item.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.EnglishName);
                            break;
                        case CultureDropDownTypes.Lcid:

                            item.Text = info.LCID.ToString();
                            break;
                        case CultureDropDownTypes.Name:

                            item.Text = info.Name;
                            break;
                        case CultureDropDownTypes.NativeName:

                            item.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.NativeName);
                            break;
                        case CultureDropDownTypes.TwoLetterIsoCode:

                            item.Text = info.TwoLetterISOLanguageName;
                            break;
                        case CultureDropDownTypes.ThreeLetterIsoCode:

                            item.Text = info.ThreeLetterISOLanguageName;
                            break;
                        default:

                            item.Text = info.DisplayName;
                            break;
                    }
                    _cultureListItems[intAdded] = item;
                    intAdded++;
                }
            }

            // If the drop down list already has items, clear the list
            if (list.Items.Count > 0)
            {
                list.Items.Clear();
            }

            _cultureListItems = (ListItem[])Utils.CopyArray((Array)_cultureListItems, new ListItem[intAdded - 1 + 1]);
            // add the items to the list
            list.Items.AddRange(_cultureListItems);

            // select the default item
            if (selectedValue != null)
            {
                ListItem item = list.Items.FindByValue(selectedValue);
                if (item != null)
                {
                    list.SelectedIndex = -1;
                    item.Selected = true;
                }
            }
        }

        /// <Summary>
        /// LoadTimeZoneDropDownList loads a drop down list with the Timezones
        /// </Summary>
        /// <Param name="list">The list to load</Param>
        /// <Param name="language">Language</Param>
        /// <Param name="selectedValue">The selected Time Zone</Param>
        public static void LoadTimeZoneDropDownList( DropDownList list, string language, string selectedValue )
        {
            NameValueCollection timeZones = GetTimeZones(language);
            //If no Timezones defined get the System Locale Time Zones
            if (timeZones.Count == 0)
            {
                timeZones = GetTimeZones(SystemLocale.ToLower());
            }
            int i;
            for (i = 0; i <= timeZones.Keys.Count - 1; i++)
            {
                list.Items.Add(new ListItem(timeZones.GetKey(i).ToString(), timeZones.Get(i).ToString()));
            }

            // select the default item
            if (selectedValue != null)
            {
                ListItem item = list.Items.FindByValue(selectedValue);
                if (item == null)
                {
                    //Try system default
                    item = list.Items.FindByValue(SystemTimeZoneOffset.ToString());
                }
                if (item != null)
                {
                    list.SelectedIndex = -1;
                    item.Selected = true;
                }
            }
        }
        /// <summary>
        /// LocalizeDataGrid creates localized Headers for a DataGrid
        /// </summary>
        /// <param name="grid">Grid t localize</param>
        /// <param name="ResourceFile">The root name of the Resource File where the localized
        ///   text can be found</param>
        public static void LocalizeDataGrid( ref DataGrid grid, string ResourceFile )
        {
            DataGridColumn col;
            string key;
            string localizedText;

            foreach (DataGridColumn tempLoopVar_col in grid.Columns)
            {
                col = tempLoopVar_col;
                //Localize Header Text
                key = col.HeaderText;
                if (key != "")
                {
                    localizedText = GetString(key + ".Header", ResourceFile);
                    if (localizedText != "")
                    {
                        col.HeaderText = localizedText;
                    }
                }

                //Localize Edit Text
                if (col is EditCommandColumn)
                {
                    EditCommandColumn editCol = (EditCommandColumn)col;
                    key = editCol.EditText;
                    localizedText = GetString(key + ".EditText", ResourceFile);
                    if (localizedText != "")
                    {
                        editCol.EditText = localizedText;
                    }
                }
            }
        }

        /// <Summary>Localizes PortalSettings</Summary>
        public static void LocalizePortalSettings()
        {
            PortalSettings objPortalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];

            objPortalSettings.DesktopTabs = LocalizeTabStripDetails(objPortalSettings.DesktopTabs);
            objPortalSettings.ActiveTab.BreadCrumbs = LocalizeTabStripDetails(objPortalSettings.ActiveTab.BreadCrumbs);
            if (objPortalSettings.ActiveTab.IsAdminTab)
            {
                string strLocalizedTabName = GetString(objPortalSettings.ActiveTab.TabName + ".String", GlobalResourceFile);
                if (strLocalizedTabName != "")
                {
                    objPortalSettings.ActiveTab.TabName = strLocalizedTabName;
                    objPortalSettings.ActiveTab.Title = "";
                }
            }

            HttpContext.Current.Items["PortalSettings"] = objPortalSettings;
        }

        public static void SetLanguage( string value )
        {
          try
            {
                HttpResponse Response = HttpContext.Current.Response;
                if( Response == null )
                {
                    return;
                }

                // save the pageculture as a cookie
                HttpCookie cookie = null;
                cookie = Response.Cookies.Get( "language" );
                if( cookie == null )
                {
                    if( value != "" )
                    {
                        cookie = new HttpCookie( "language", value );
                        Response.Cookies.Add( cookie );
                    }
                }
                else
                {
                    cookie.Value = value;
                    if( value != "" )
                    {
                        Response.Cookies.Set( cookie );
                    }
                    else
                    {
                        Response.Cookies.Remove( "language" );
                    }
                }
            }
            catch
            {
                return;
            }
        
        }
    }
}