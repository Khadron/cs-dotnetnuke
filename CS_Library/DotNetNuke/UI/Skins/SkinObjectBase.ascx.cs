using System.ComponentModel;
using System.Web.UI;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;

namespace DotNetNuke.UI.Skins
{
    /// <Summary>
    /// The SkinObject class defines a custom base class inherited by all
    /// skin and container objects within the Portal.
    /// </Summary>
    public class SkinObjectBase : UserControl
    {
        public bool AdminMode
        {
            get
            {
                return PortalSecurity.IsInRoles(PortalSettings.AdministratorRoleName) || PortalSecurity.IsInRoles(PortalSettings.ActiveTab.AdministratorRoles.ToString());
            }
        }

        [Browsable( false ), DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Hidden )]
        public PortalSettings PortalSettings
        {
            get
            {
                return PortalController.GetCurrentPortalSettings();
            }
        }
    }
}