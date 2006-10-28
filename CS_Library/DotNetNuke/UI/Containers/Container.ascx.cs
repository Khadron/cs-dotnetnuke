using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.Skins;

namespace DotNetNuke.UI.Containers
{
    /// <Summary>Container is the base for the Containers</Summary>
    public class Container : UserControl
    {
        public string SkinPath
        {
            get
            {
                return ( this.TemplateSourceDirectory + "/" );
            }
        }

        /// <Summary>
        /// GetPortalModuleBase gets the parent PortalModuleBase Control
        /// </Summary>
        public static PortalModuleBase GetPortalModuleBase( UserControl objControl )
        {
            PortalModuleBase objPortalModuleBase = null;

            Panel ctlPanel;

            if (objControl is SkinObjectBase)
            {
                ctlPanel = (Panel)objControl.Parent.FindControl("ModuleContent");
            }
            else
            {
                ctlPanel = (Panel)objControl.FindControl("ModuleContent");
            }

            if (ctlPanel != null)
            {
                try
                {
                    objPortalModuleBase = (PortalModuleBase)ctlPanel.Controls[0];
                }
                catch
                {
                    // module was not loaded correctly
                }
            }

            if (objPortalModuleBase == null)
            {
                objPortalModuleBase = new PortalModuleBase();
                objPortalModuleBase.ModuleConfiguration = new ModuleInfo();
            }

            return objPortalModuleBase;
        }
    }
}