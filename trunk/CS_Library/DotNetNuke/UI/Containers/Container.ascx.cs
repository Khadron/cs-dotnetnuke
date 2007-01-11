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