using System;
using System.Web.UI;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The TrueFalseEditControl control provides a standard UI component for editing
    /// true/false (boolean) properties.
    /// </Summary>
    [ToolboxData( "<{0}:TrueFalseEditControl runat=server></{0}:TrueFalseEditControl>" )]
    public class TrueFalseEditControl : EditControl
    {

        /// <Summary>
        /// BooleanValue returns the Boolean representation of the Value
        /// </Summary>
        protected bool BooleanValue
        {
            get
            {
                bool boolValue = DotNetNuke.Common.Utilities.Null.NullBoolean;
                try
                {
                    //Try and cast the value to an Boolean
                    boolValue = System.Convert.ToBoolean(Value);
                }
                catch (Exception)
                {
                }
                return boolValue;
            }
        }

        /// <Summary>
        /// OldBooleanValue returns the Boolean representation of the OldValue
        /// </Summary>
        protected bool OldBooleanValue
        {
            get
            {
                bool boolValue = DotNetNuke.Common.Utilities.Null.NullBoolean;
                try
                {
                    //Try and cast the value to an Boolean
                    boolValue = System.Convert.ToBoolean(OldValue);
                }
                catch (Exception)
                {
                }
                return boolValue;
            }
        }

        /// <Summary>
        /// StringValue is the value of the control expressed as a String
        /// </Summary>
        protected override string StringValue
        {
            get
            {
                return this.BooleanValue.ToString();
            }
            set
            {
                bool b1 = bool.Parse( value );
                this.Value = b1;
            }
        }
        /// <Summary>Constructs a TrueFalseEditControl</Summary>
        public TrueFalseEditControl()
        {
            this.SystemType = "System.Boolean";
        }

        /// <Summary>
        /// OnDataChanged runs when the PostbackData has changed.  It raises the ValueChanged
        /// Event
        /// </Summary>
        protected override void OnDataChanged( EventArgs e )
        {
            PropertyEditorEventArgs args = new PropertyEditorEventArgs(Name);
            args.Value = BooleanValue;
            args.OldValue = OldBooleanValue;
            args.StringValue = StringValue;
            base.OnValueChanged(args);
        }

        /// <Summary>RenderEditMode renders the Edit mode of the control</Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderEditMode( HtmlTextWriter writer )
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
            if (BooleanValue)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "True");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();

            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write("True");
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
            if (!BooleanValue)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "False");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();

            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write("False");
            writer.RenderEndTag();
        }
    }
}