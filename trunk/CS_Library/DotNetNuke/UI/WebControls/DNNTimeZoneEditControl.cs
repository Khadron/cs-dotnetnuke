using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The DNNTimeZoneEditControl control provides a standard UI component for selecting
    /// a Time Zone
    /// </Summary>
    [ToolboxData( "<{0}:DNNTimeZoneEditControl runat=server></{0}:DNNTimeZoneEditControl>" )]
    public class DNNTimeZoneEditControl : IntegerEditControl
    {
        /// <Summary>Constructs a DNNTimeZoneEditControl</Summary>
        public DNNTimeZoneEditControl()
        {
        }

        /// <Summary>RenderEditMode renders the Edit mode of the control</Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderEditMode( HtmlTextWriter writer )
        {
            PortalSettings _portalSettings = Globals.GetPortalSettings();

            //For convenience create a DropDownList to use
            DropDownList cboTimeZones = new DropDownList();

            //Load the List with Time Zones
            Localization.LoadTimeZoneDropDownList(cboTimeZones, ((PageBase)Page).PageCulture.Name, Convert.ToString(_portalSettings.TimeZoneOffset));

            //Select the relevant item
            if (cboTimeZones.Items.FindByValue(StringValue) != null)
            {
                cboTimeZones.ClearSelection();
                cboTimeZones.Items.FindByValue(StringValue).Selected = true;
            }

            //Render the Select Tag
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Select);

            for (int I = 0; I <= cboTimeZones.Items.Count - 1; I++)
            {
                string timeZoneValue = cboTimeZones.Items[I].Value;
                string timeZoneName = cboTimeZones.Items[I].Text;
                bool isSelected = cboTimeZones.Items[I].Selected;

                //Add the Value Attribute
                writer.AddAttribute(HtmlTextWriterAttribute.Value, timeZoneValue);

                if (isSelected)
                {
                    //Add the Selected Attribute
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                }

                //Render Option Tag
                writer.RenderBeginTag(HtmlTextWriterTag.Option);
                writer.Write(timeZoneName.PadRight(100).Substring(0, 50));
                writer.RenderEndTag();
            }

            //Close Select Tag
            writer.RenderEndTag();
        }

        /// <Summary>
        /// RenderViewMode renders the View (readonly) mode of the control
        /// </Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderViewMode( HtmlTextWriter writer )
        {
            PortalSettings _portalSettings = Globals.GetPortalSettings();

            //For convenience create a DropDownList to use
            DropDownList cboTimeZones = new DropDownList();

            //Load the List with Time Zones
            Localization.LoadTimeZoneDropDownList(cboTimeZones, ((PageBase)Page).PageCulture.Name, Convert.ToString(_portalSettings.TimeZoneOffset));

            //Select the relevant item
            if (cboTimeZones.Items.FindByValue(StringValue) != null)
            {
                cboTimeZones.ClearSelection();
                cboTimeZones.Items.FindByValue(StringValue).Selected = true;
            }

            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(cboTimeZones.SelectedItem.Text);
            writer.RenderEndTag();
        }
    }
}