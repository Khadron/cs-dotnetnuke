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
using System.ComponentModel;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.Security.Membership
{
    /// <summary>
    /// The PasswordConfig class provides a wrapper any Portal wide Password Settings
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class PasswordConfig
    {
        /// <summary>
        /// Gets and sets the Password Expiry time in days
        /// </summary>
        /// <returns>An integer.</returns>
        [SortOrder(0), Category("Password")]
        public static int PasswordExpiry
        {
            get
            {
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                int _PasswordExpiry = 0;
                if (_portalSettings.HostSettings["PasswordExpiry"] != null)
                {
                    _PasswordExpiry = Convert.ToInt32(_portalSettings.HostSettings["PasswordExpiry"]);
                }
                return _PasswordExpiry;
            }
            set
            {
                HostSettingsController objHostSettings = new HostSettingsController();
                objHostSettings.UpdateHostSetting("PasswordExpiry", value.ToString());
            }
        }

        /// <summary>
        /// Gets and sets the a Reminder time in days (to remind the user that theire password
        /// is about to expire
        /// </summary>
        /// <returns>An integer.</returns>
        [SortOrder(1), Category("Password")]
        public static int PasswordExpiryReminder
        {
            get
            {
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                int _PasswordExpiry = 7;
                if (_portalSettings.HostSettings["PasswordExpiryReminder "] != null)
                {
                    _PasswordExpiry = Convert.ToInt32(_portalSettings.HostSettings["PasswordExpiryReminder "]);
                }
                return _PasswordExpiry;
            }
            set
            {
                HostSettingsController objHostSettings = new HostSettingsController();
                objHostSettings.UpdateHostSetting("PasswordExpiryReminder ", value.ToString());
            }
        }
    }
}