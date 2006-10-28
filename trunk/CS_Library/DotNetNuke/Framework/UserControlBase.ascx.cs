using System.ComponentModel;
using System.Web.UI;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Framework
{
    /// <summary>
    /// The UserControlBase class defines a custom base class inherited by all
    /// user controls within the Portal.
    /// </summary>
    public class UserControlBase : UserControl
    {
        public bool IsAdminMenu
        {
            get
            {
                bool _IsAdmin = false;
                if (PortalSettings.ActiveTab.ParentId == PortalSettings.AdminTabId)
                {
                    _IsAdmin = true;
                }
                return _IsAdmin;
            }
        }

        public bool IsHostMenu
        {
            get
            {
                bool _IsHost = false;
                if (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId)
                {
                    _IsHost = true;
                }
                return _IsHost;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PortalSettings PortalSettings
        {
            get
            {
                PortalSettings returnValue;
                returnValue = PortalController.GetCurrentPortalSettings();
                return returnValue;
            }
        }
    }
}