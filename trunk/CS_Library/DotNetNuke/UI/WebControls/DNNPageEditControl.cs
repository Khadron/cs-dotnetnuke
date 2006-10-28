using System.Collections;
using System.Web.UI;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The DNNPageEditControl control provides a standard UI component for selecting
    /// a DNN Page
    /// </Summary>
    [ToolboxData( "<{0}:DNNPageEditControl runat=server></{0}:DNNPageEditControl>" )]
    public class DNNPageEditControl : IntegerEditControl
    {
        /// <Summary>Constructs a DNNPageEditControl</Summary>
        public DNNPageEditControl()
        {
        }

        /// <Summary>RenderEditMode renders the Edit mode of the control</Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderEditMode( HtmlTextWriter writer )
        {
            PortalSettings _portalSettings = Globals.GetPortalSettings();

            //Get the Pages
            ArrayList tabs = Globals.GetPortalTabs(_portalSettings.PortalId, true, true, false, false, false);

            //Render the Select Tag
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Select);

            for (int tabIndex = 0; tabIndex <= tabs.Count - 1; tabIndex++)
            {
                TabInfo tab = (TabInfo)tabs[tabIndex];

                //Add the Value Attribute
                writer.AddAttribute(HtmlTextWriterAttribute.Value, tab.TabID.ToString());

                if (tab.TabID == IntegerValue)
                {
                    //Add the Selected Attribute
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                }

                //Render Option Tag
                writer.RenderBeginTag(HtmlTextWriterTag.Option);
                writer.Write(tab.TabName);
                writer.RenderEndTag();
            }

            //Close Select Tag
            writer.RenderEndTag();
        }
    }
}