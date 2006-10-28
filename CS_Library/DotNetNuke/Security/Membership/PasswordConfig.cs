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