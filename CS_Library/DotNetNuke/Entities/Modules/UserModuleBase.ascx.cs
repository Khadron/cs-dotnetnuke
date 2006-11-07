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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Entities.Modules
{
    /// <Summary>
    /// The UserModuleBase class defines a custom base class inherited by all
    /// desktop portal modules within the Portal that manage Users.
    /// </Summary>
    public class UserModuleBase : PortalModuleBase
    {
        private UserInfo _User;

        /// <summary>
        /// Gets whether we are in Add User mode
        /// </summary>
        protected bool AddUser
        {
            get
            {
                return (UserId == Null.NullInteger);
            }
        }

        /// <summary>
        /// Gets whether the current user is an Administrator (or SuperUser)
        /// </summary>
        protected bool IsAdmin
        {
            get
            {
                return (UserInfo.IsInRole(this.PortalSettings.AdministratorRoleName) || UserInfo.IsSuperUser);
            }
        }

        /// <summary>
        /// Gets whether this control is in the Admin menu
        /// </summary>
        protected bool IsAdminTab
        {
            get
            {
                return (PortalSettings.ActiveTab.ParentId == PortalSettings.AdminTabId);
            }
        }

        /// <summary>
        /// Gets whether the control is being called form the User Accounts module
        /// </summary>
        protected bool IsEdit
        {
            get
            {
                bool _IsEdit = false;
                if (!(Request.QueryString["ctl"] == null))
                {
                    string ctl = Request.QueryString["ctl"];
                    if (ctl == "Edit")
                    {
                        _IsEdit = true;
                    }
                }
                return _IsEdit;
            }
        }

        /// <summary>
        /// Gets whether this control is in the Host menu
        /// </summary>
        protected bool IsHostTab
        {
            get
            {
                return (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId);
            }
        }

        /// <summary>
        /// Gets whether the current user is modifying their profile
        /// </summary>
        protected bool IsProfile
        {
            get
            {
                bool _IsProfile = false;
                if (IsUser)
                {
                    if (PortalSettings.UserTabId != -1)
                    {
                        // user defined tab
                        if (PortalSettings.ActiveTab.TabID == PortalSettings.UserTabId)
                        {
                            _IsProfile = true;
                        }
                    }
                    else
                    {
                        // admin tab
                        if (!(Request.QueryString["ctl"] == null))
                        {
                            string ctl = Request.QueryString["ctl"];
                            if (ctl == "Profile")
                            {
                                _IsProfile = true;
                            }
                        }
                    }
                }
                return _IsProfile;
            }
        }

        /// <summary>
        /// Gets whether an anonymous user is trying to register
        /// </summary>
        protected bool IsRegister
        {
            get
            {
                bool _IsRegister = false;
                if (!IsAdmin && !IsUser)
                {
                    if (PortalSettings.UserTabId != -1)
                    {
                        // user defined tab
                        if (PortalSettings.ActiveTab.TabID == PortalSettings.UserTabId)
                        {
                            _IsRegister = true;
                        }
                    }
                    else
                    {
                        // admin tab
                        if (!(Request.QueryString["ctl"] == null))
                        {
                            string ctl = Request.QueryString["ctl"];
                            if (ctl == "Register")
                            {
                                _IsRegister = true;
                            }
                        }
                    }
                }
                return _IsRegister;
            }
        }

        /// <summary>
        /// Gets whether the User is editing their own information
        /// </summary>
        protected bool IsUser
        {
            get
            {
                bool _IsUser = false;
                if (Request.IsAuthenticated)
                {
                    _IsUser = User.UserID == UserInfo.UserID;
                }
                return _IsUser;
            }
        }

        /// <summary>
        /// Gets and sets the User associated with this control
        /// </summary>
        public UserInfo User
        {
            get
            {
                if (_User == null)
                {
                    if (AddUser)
                    {
                        _User = InitialiseUser();
                    }
                    else
                    {
                        _User = UserController.GetUser(PortalId, UserId, false);
                    }
                }
                return _User;
            }
            set
            {
                _User = value;
                if (_User != null)
                {
                    UserId = _User.UserID;
                }
            }
        }

        /// <summary>
        /// Gets and sets the UserId associated with this control
        /// </summary>
        public new int UserId
        {
            get
            {
                int _UserId = Null.NullInteger;
                if (ViewState["UserId"] == null)
                {
                    if (!(Request.QueryString["userid"] == null))
                    {
                        _UserId = int.Parse(Request.QueryString["userid"]);
                        ViewState["UserId"] = _UserId;
                    }
                }
                else
                {
                    _UserId = Convert.ToInt32(ViewState["UserId"]);
                }
                return _UserId;
            }
            set
            {
                ViewState["UserId"] = value;
            }
        }

        /// <summary>
        /// Gets the Default Settings for the Module
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static Hashtable GetDefaultSettings()
        {
            return GetSettings(new Hashtable());
        }

        /// <summary>
        /// Gets a Setting for the Module
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static object GetSetting(int portalId, string settingKey)
        {
            Hashtable settings = UserController.GetUserSettings(portalId);
            object setting = settings[settingKey];
            if (setting == null)
            {
                //Get Default Setting (and save them to the Database, as we obviously have no settings defined yet)
                settings = GetDefaultSettings();
                UpdateSettings(portalId, settings);

                setting = settings[settingKey];
            }

            return setting;
        }

        /// <summary>
        /// Gets the Settings for the Module
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static Hashtable GetSettings(Hashtable settings)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            if (settings["Column_FirstName"] == null)
            {
                settings["Column_FirstName"] = false;
            }
            if (settings["Column_LastName"] == null)
            {
                settings["Column_LastName"] = false;
            }
            if (settings["Column_DisplayName"] == null)
            {
                settings["Column_DisplayName"] = true;
            }
            if (settings["Column_Address"] == null)
            {
                settings["Column_Address"] = true;
            }
            if (settings["Column_Telephone"] == null)
            {
                settings["Column_Telephone"] = true;
            }
            if (settings["Column_Email"] == null)
            {
                settings["Column_Email"] = false;
            }
            if (settings["Column_CreatedDate"] == null)
            {
                settings["Column_CreatedDate"] = true;
            }
            if (settings["Column_LastLogin"] == null)
            {
                settings["Column_LastLogin"] = false;
            }
            if (settings["Column_Authorized"] == null)
            {
                settings["Column_Authorized"] = true;
            }
            if (settings["Display_Mode"] == null)
            {
                settings["Display_Mode"] = DisplayMode.None;
            }
            else
            {
                settings["Display_Mode"] = (DisplayMode)settings["Display_Mode"];
            }
            if (settings["Records_PerPage"] == null)
            {
                settings["Records_PerPage"] = 10;
            }
            if (settings["Redirect_AfterLogin"] == null)
            {
                settings["Redirect_AfterLogin"] = -1;
            }
            if (settings["Redirect_AfterRegistration"] == null)
            {
                settings["Redirect_AfterRegistration"] = -1;
            }
            if (settings["Redirect_AfterLogout"] == null)
            {
                settings["Redirect_AfterLogout"] = -1;
            }
            if (settings["Security_CaptchaLogin"] == null)
            {
                settings["Security_CaptchaLogin"] = false;
            }
            if (settings["Security_CaptchaRegister"] == null)
            {
                settings["Security_CaptchaRegister"] = false;
            }
            if (settings["Security_RequireValidProfile"] == null)
            {
                settings["Security_RequireValidProfile"] = false;
            }
            if (settings["Security_UsersControl"] == null)
            {
                if (UserController.GetUserCountByPortal(_portalSettings.PortalId) > 1000)
                {
                    settings["Security_UsersControl"] = UsersControl.TextBox;
                }
                else
                {
                    settings["Security_UsersControl"] = UsersControl.Combo;
                }
            }
            else
            {
                settings["Security_UsersControl"] = (UsersControl)settings["Security_UsersControl"];
            }

            return settings;
        }

        /// <summary>
        /// InitialiseUser initialises a "new" user
        /// </summary>
        private UserInfo InitialiseUser()
        {
            UserInfo newUser = new UserInfo();

            if (IsHostMenu)
            {
                newUser.IsSuperUser = true;
            }
            else
            {
                newUser.PortalID = PortalId;
            }

            //'Initialise the ProfileProperties Collection
            newUser.Profile.InitialiseProfile(PortalId);
            newUser.Profile.PreferredLocale = this.PortalSettings.DefaultLanguage;
            newUser.Profile.TimeZone = this.PortalSettings.TimeZoneOffset;

            //'Set AffiliateId
            int AffiliateId = Null.NullInteger;
            if (Request.Cookies["AffiliateId"] != null)
            {
                AffiliateId = int.Parse(Request.Cookies["AffiliateId"].Value);
            }
            newUser.AffiliateID = AffiliateId;

            return newUser;
        }

        /// <summary>
        /// Updates the Settings for the Module
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void UpdateSettings(int portalId, Hashtable settings)
        {
            ModuleController objModuleController = new ModuleController();
            string key;
            string setting;

            //Now save the values
            IDictionaryEnumerator settingsEnumerator = settings.GetEnumerator();
            while (settingsEnumerator.MoveNext())
            {
                key = Convert.ToString(settingsEnumerator.Key);
                setting = Convert.ToString(settingsEnumerator.Value);

                //This settings page is loaded from two locations - so make sure we use the correct ModuleId
                ModuleInfo objModule = objModuleController.GetModuleByDefinition(portalId, "User Accounts");
                objModuleController.UpdateModuleSetting(objModule.ModuleID, key, setting);
            }

            //Clear the UserSettings Cache
            DataCache.RemoveCache(UserController.SettingsKey(portalId));
        }
    }
}