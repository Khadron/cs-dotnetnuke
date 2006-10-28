using System;
using System.Web.UI;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The TextEditControl control provides a standard UI component for editing
    /// string/text properties.
    /// </Summary>
    [ToolboxData( "<{0}:TextEditControl runat=server></{0}:TextEditControl>" )]
    public class TextEditControl : EditControl
    {

        /// <Summary>
        /// OldStringValue returns the Boolean representation of the OldValue
        /// </Summary>
        protected string OldStringValue
        {
            get
            {
                return System.Convert.ToString(OldValue);
            }
        }

        /// <Summary>
        /// StringValue is the value of the control expressed as a String
        /// </Summary>
        protected override string StringValue
        {
            get
            {
                return System.Convert.ToString(Value);
            }
            set
            {
                this.Value = value;
            }
        }
        /// <Summary>Constructs a TextEditControl</Summary>
        public TextEditControl()
        {
        }

        /// <Summary>Constructs a TextEditControl</Summary>
        /// <Param name="type">The type of the property</Param>
        public TextEditControl( string type )
        {
            this.SystemType = type;
        }

        /// <Summary>
        /// OnDataChanged runs when the PostbackData has changed.  It raises the ValueChanged
        /// Event
        /// </Summary>
        protected override void OnDataChanged( EventArgs e )
        {
            PropertyEditorEventArgs args = new PropertyEditorEventArgs(Name);
            args.Value = StringValue;
            args.OldValue = OldStringValue;
            args.StringValue = StringValue;
            base.OnValueChanged(args);
        }

        /// <Summary>RenderEditMode renders the Edit mode of the control</Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderEditMode( HtmlTextWriter writer )
        {
            int length = DotNetNuke.Common.Utilities.Null.NullInteger;
            if (CustomAttributes != null)
            {
                foreach (System.Attribute attribute in CustomAttributes)
                {
                    if (attribute is MaxLengthAttribute)
                    {
                        MaxLengthAttribute lengthAtt = (MaxLengthAttribute)attribute;
                        length = lengthAtt.Length;
                        
                    }
                }            
            }

            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            writer.AddAttribute(HtmlTextWriterAttribute.Value, StringValue);
            if (length > DotNetNuke.Common.Utilities.Null.NullInteger)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, length.ToString());
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }
    }
}