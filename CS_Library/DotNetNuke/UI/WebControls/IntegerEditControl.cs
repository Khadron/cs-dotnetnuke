using System;
using System.Web.UI;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The IntegerEditControl control provides a standard UI component for editing
    /// integer properties.
    /// </Summary>
    [ToolboxData( "<{0}:IntegerEditControl runat=server></{0}:IntegerEditControl>" )]
    public class IntegerEditControl : EditControl
    {

        /// <Summary>
        /// IntegerValue returns the Integer representation of the Value
        /// </Summary>
        protected int IntegerValue
        {
            get
            {
                int intValue = Null.NullInteger;
                try
                {
                    //Try and cast the value to an Integer
                    intValue = Convert.ToInt32(Value);
                }
                catch (Exception)
                {
                }
                return intValue;
            }
        }

        /// <Summary>
        /// OldIntegerValue returns the Integer representation of the OldValue
        /// </Summary>
        protected int OldIntegerValue
        {
            get
            {
                int intValue = Null.NullInteger;
                try
                {
                    //Try and cast the value to an Integer
                    intValue = Convert.ToInt32(OldValue);
                }
                catch (Exception)
                {
                }
                return intValue;
            }
        }

        /// <Summary>
        /// StringValue is the value of the control expressed as a String
        /// </Summary>
        protected override string StringValue
        {
            get
            {
                return this.IntegerValue.ToString();
            }
            set
            {
                int setValue = int.Parse(value);
                this.Value = setValue;
            }
        }
        /// <Summary>Constructs an IntegerEditControl</Summary>
        public IntegerEditControl()
        {
            this.SystemType = "System.Int32";
        }

        /// <Summary>
        /// OnDataChanged runs when the PostbackData has changed.  It raises the ValueChanged
        /// Event
        /// </Summary>
        protected override void OnDataChanged( EventArgs e )
        {
            PropertyEditorEventArgs args = new PropertyEditorEventArgs(Name);
            args.Value = IntegerValue;
            args.OldValue = OldIntegerValue;
            args.StringValue = StringValue;
            base.OnValueChanged(args);
        }

        /// <Summary>RenderEditMode renders the Edit mode of the control</Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderEditMode( HtmlTextWriter writer )
        {
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            writer.AddAttribute(HtmlTextWriterAttribute.Size, "5");
            writer.AddAttribute(HtmlTextWriterAttribute.Value, StringValue);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }
    }
}