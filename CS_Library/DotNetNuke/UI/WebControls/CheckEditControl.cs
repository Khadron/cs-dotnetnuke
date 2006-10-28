using System;
using System.Collections.Specialized;
using System.Web.UI;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The TrueFalseEditControl control provides a standard UI component for editing
    /// true/false (boolean) properties.
    /// </Summary>
    [ToolboxData( "<{0}:CheckEditControl runat=server></{0}:CheckEditControl>" )]
    public class CheckEditControl : TrueFalseEditControl
    {
        /// <Summary>Constructs a TrueFalseEditControl</Summary>
        public CheckEditControl()
        {
            this.SystemType = "System.Boolean";
        }

        /// <Summary>
        /// LoadPostData loads the Post Back Data and determines whether the value has change
        /// </Summary>
        /// <Param name="postDataKey">A key to the PostBack Data to load</Param>
        /// <Param name="postCollection">
        /// A name value collection of postback data
        /// </Param>
        public override bool LoadPostData( string postDataKey, NameValueCollection postCollection )
        {
            string postedValue = postCollection[postDataKey];
            bool boolValue = false;
            if (!(postedValue == null || postedValue == string.Empty))
            {
                boolValue = true;
            }
            if (!BooleanValue.Equals(boolValue))
            {
                Value = boolValue;
                return true;
            }
            return false;
        }

        /// <Summary>
        /// OnPreRender runs just before the control is rendered.  It forces a postback to the
        /// Control.
        /// </Summary>
        protected override void OnPreRender( EventArgs e )
        {
            base.OnPreRender(e);
            if (Page != null && this.EditMode == PropertyEditorMode.Edit)
            {
                this.Page.RegisterRequiresPostBack(this);
            }
        }

        /// <Summary>RenderEditMode renders the Edit mode of the control</Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderEditMode( HtmlTextWriter writer )
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
            if (BooleanValue)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
            }
            else
            {
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }

        /// <Summary>
        /// RenderViewMode renders the View (readonly) mode of the control
        /// </Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderViewMode( HtmlTextWriter writer )
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
            if (BooleanValue)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
            }
            else
            {
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }
    }
}