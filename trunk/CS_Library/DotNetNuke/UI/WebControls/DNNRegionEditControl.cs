using System;
using System.Web.UI;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The DNNRegionEditControl control provides a standard UI component for editing
    /// Regions
    /// </Summary>
    [ToolboxData( "<{0}:DNNRegionEditControl runat=server></{0}:DNNRegionEditControl>" )]
    public class DNNRegionEditControl : DNNListEditControl
    {
        /// <Summary>Constructs a DNNRegionEditControl</Summary>
        public DNNRegionEditControl()
        {
            this.AutoPostBack = false;
            this.TextField = ListBoundField.Text;
            this.ValueField = ListBoundField.Text;
        }

        /// <Summary>RenderEditMode renders the Edit mode of the control</Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderEditMode( HtmlTextWriter writer )
        {
            if ((List == null) || List.Count == 0)
            {
                //No List so use a Text Box
                string propValue = Convert.ToString(this.Value);

                ControlStyle.AddAttributesToRender(writer);
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, propValue);
                writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();
            }
            else
            {
                //Render the standard List
                base.RenderEditMode(writer);
            }
        }
    }
}