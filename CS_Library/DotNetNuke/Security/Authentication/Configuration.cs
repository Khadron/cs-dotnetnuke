#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework.Providers;

namespace DotNetNuke.Security.Authentication
{
    public class Configuration
    {
        private const string AUTHENTICATION_CONFIG_CACHE_PREFIX = "Authentication.Configuration";
        public const string AUTHENTICATION_KEY = "authentication";
        public const string AUTHENTICATION_LOGOFF_PAGE = "Logoff.aspx";
        public const string AUTHENTICATION_LOGON_PAGE = "WindowsSignin.aspx";
        public const string AUTHENTICATION_STATUS_KEY = "authentication.status";
        public const string LOGON_USER_VARIABLE = "LOGON_USER";
        private string mAuthenticationType = DefaultAuthenticationType;
        private string mEmailDomain = DefaultEmailDomain;
        private string mPassword = "";

        private int mPortalId;
        private string mProviderTypeName = DefaultProviderTypeName;
        private string mRootDomain = "";
        private bool mSynchronizePassword = false; // reserve for future feature
        private bool mSynchronizeRole = false;
        private string mUserName = "";
        private bool mWindowsAuthentication = false;

        /// <summary>
        /// </summary>
        /// <remarks>
        /// It was configured in web.config, move to site settings is more flexible
        /// When configure first time, only default provider (ADs) available to provide list of type to select
        /// </remarks>
        public string AuthenticationType
        {
            get
            {
                return this.mAuthenticationType;
            }
        }

        public static string DefaultAuthenticationType
        {
            get
            {
                return "Delegation";
            }
        }

        public static string DefaultEmailDomain
        {
            get
            {
                PortalSettings portalSettings1 = PortalController.GetCurrentPortalSettings();
                string string1 = portalSettings1.Email;
                return string1.Substring( string1.IndexOf( "@" ) );
            }
        }

        public static string DefaultProviderTypeName
        {
            get
            {
                return "DotNetNuke.Security.Authentication.ADSIProvider, DotNetNuke.Authentication.ADSIProvider";
            }
        }

        public string EmailDomain
        {
            get
            {
                return this.mEmailDomain;
            }
        }

        public string Password
        {
            get
            {
                return this.mPassword;
            }
        }

        public int PortalId
        {
            get
            {
                return this.mPortalId;
            }
        }

        public string ProviderTypeName
        {
            get
            {
                return this.mProviderTypeName;
            }
        }

        public string RootDomain
        {
            get
            {
                return this.mRootDomain;
            }
        }

        /// <summary>
        /// Process checking DNN password against Windows password
        /// update DNN password if not match
        /// requires modified signin page for functionality
        /// </summary>
        /// <remarks>
        /// This process quite expensive in terms of performance
        /// Reserve for future implementation
        /// </remarks>
        public bool SynchronizePassword
        {
            get
            {
                return this.mSynchronizePassword;
            }
        }

        /// <Summary>
        /// Role membership to be synchronized (Authentication/DNN) when user logon
        /// </Summary>
        public bool SynchronizeRole
        {
            get
            {
                return this.mSynchronizeRole;
            }
        }

        public string UserName
        {
            get
            {
                return this.mUserName;
            }
        }

        public bool WindowsAuthentication
        {
            get
            {
                return this.mWindowsAuthentication;
            }
        }

        /// <summary>
        /// Obtain Authentication settings from database
        /// </summary>
        /// <remarks>
        ///  Setting records are stored in ModuleSettings table, separately for each portal,
        /// this method allows each portal could have different accessing method to Windows Active Directory
        /// </remarks>
        public Configuration()
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(AUTHENTICATION_KEY);

            mPortalId = _portalSettings.PortalId;

            PortalSecurity objSecurity = new PortalSecurity();
            try
            {
                if (_providerConfiguration.DefaultProvider == null)
                {
                    // No provider specified, so disable authentication feature
                    return;
                }
                else
                {
                    ModuleController objModules = new ModuleController();
                    ModuleInfo objModuleInfo = objModules.GetModuleByDefinition(mPortalId, "Site Settings");
                    Hashtable settings = PortalSettings.GetModuleSettings(objModuleInfo.ModuleID);

                    mWindowsAuthentication = Convert.ToBoolean(Null.GetNull(settings["WindowsAuthentication"], mWindowsAuthentication));
                    mSynchronizeRole = Convert.ToBoolean(Null.GetNull(settings["SynchronizeRole"], mSynchronizeRole));
                    mSynchronizePassword = Convert.ToBoolean(Null.GetNull(settings["SynchronizePassword"], mSynchronizePassword));
                    mRootDomain = Convert.ToString(Null.GetNull(settings["RootDomain"], mRootDomain));
                    mEmailDomain = Convert.ToString(Null.GetNull(settings["EmailDomain"], mEmailDomain));
                    mUserName = Convert.ToString(Null.GetNull(settings["UserName"], mUserName));
                    mProviderTypeName = Convert.ToString(Null.GetNull(settings["ProviderTypeName"], mProviderTypeName));
                    mAuthenticationType = Convert.ToString(Null.GetNull(settings["AuthenticationType"], mAuthenticationType));
                    // Since DNN 3.0, HostSettings("EncryptionKey") is empty string, so we handle by AUTHENTICATION_KEY
                    mPassword = objSecurity.Decrypt(AUTHENTICATION_KEY, Convert.ToString(Null.GetNull(settings["AuthenticationPassword"], mPassword.ToString())));
                    //mPassword = objSecurity.Decrypt(CStr(_portalSettings.HostSettings("EncryptionKey")), CType(GetValue(settings("AuthenticationPassword"), mPassword.ToString), String))
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Obtain Authentication Configuration
        /// </summary>
        /// <remarks>
        /// Accessing Active Directory also costs resource,
        /// so we only do it once then save into cache for later use
        /// </remarks>
        public static Configuration GetConfig()
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            string strKey = AUTHENTICATION_CONFIG_CACHE_PREFIX + "." + _portalSettings.PortalId.ToString();
            Configuration config = (Configuration)DataCache.GetCache(strKey);

            if (config == null)
            {
                config = new Configuration();
                DataCache.SetCache(strKey, config);
            }

            return config;
        }

        /// <summary>
        /// Used to determine if a valid input is provided, if not, return default value
        /// </summary>
        /// <remarks>
        /// </remarks>
        private string GetValue(object Input, string DefaultValue)
        {
            if (Input == null)
            {
                return DefaultValue;
            }
            else
            {
                return Input.ToString();
            }
        }

        public static void ResetConfig()
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            string strKey = AUTHENTICATION_CONFIG_CACHE_PREFIX + "." + _portalSettings.PortalId.ToString();
            DataCache.RemoveCache(strKey);
        }

        public static void UpdateConfig(int PortalID, bool WindowsAuthentication, string RootDomain, string EmailDomain, string AuthenticationUserName, string AuthenticationPassword, bool SynchronizeRole, bool SynchronizePassword, string ProviderTypeName, string AuthenticationType)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            ModuleController objModules = new ModuleController();
            PortalSecurity objSecurity = new PortalSecurity();
            ModuleInfo objModuleInfo = objModules.GetModuleByDefinition(PortalID, "Site Settings");
            int intModuleId = objModuleInfo.ModuleID;

            objModules.UpdateModuleSetting(intModuleId, "WindowsAuthentication", WindowsAuthentication.ToString());
            objModules.UpdateModuleSetting(intModuleId, "SynchronizeRole", SynchronizeRole.ToString());
            objModules.UpdateModuleSetting(intModuleId, "SynchronizePassword", SynchronizePassword.ToString());
            objModules.UpdateModuleSetting(intModuleId, "RootDomain", RootDomain);
            objModules.UpdateModuleSetting(intModuleId, "EmailDomain", EmailDomain);
            objModules.UpdateModuleSetting(intModuleId, "UserName", AuthenticationUserName);
            objModules.UpdateModuleSetting(intModuleId, "ProviderTypeName", ProviderTypeName);
            objModules.UpdateModuleSetting(intModuleId, "AuthenticationType", AuthenticationType);

            //Only update password if it has been changed
            if (AuthenticationPassword.Length > 0)
            {
                objModules.UpdateModuleSetting(intModuleId, "AuthenticationPassword", Convert.ToString(objSecurity.Encrypt(AUTHENTICATION_KEY, AuthenticationPassword)));
            }
        }
    }
}