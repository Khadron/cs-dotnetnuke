using System.Web;
using System.Web.UI;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.UI.ControlPanels
{
    /// <Summary>
    /// The ControlPanel class defines a custom base class inherited by all
    /// ControlPanel controls.
    /// </Summary>
    public class ControlPanelBase : UserControl
    {
        private string _localResourceFile;

        public string LocalResourceFile
        {
            get
            {
                if( _localResourceFile == "" )
                {
                    return ( this.TemplateSourceDirectory + "/App_LocalResources/" + this.ID );
                }
                else
                {
                    return this._localResourceFile;
                }
            }
            set
            {
                this._localResourceFile = value;
            }
        }

        public PortalSettings PortalSettings
        {
            get
            {
                return ( (PortalSettings)HttpContext.Current.Items["PortalSettings"] );
            }
        }
    }
}