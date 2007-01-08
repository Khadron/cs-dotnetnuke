#region DotNetNuke License
// DotNetNuke� - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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