#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
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
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The EnumEditControl control provides a standard UI component for editing
    /// enumerated properties.
    /// </Summary>
    [ToolboxData( "<{0}:EnumEditControl runat=server></{0}:EnumEditControl>" )]
    public class EnumEditControl : EditControl
    {
        private Type EnumType;

        /// <Summary>
        /// StringValue is the value of the control expressed as a String
        /// </Summary>
        protected override string StringValue
        {
            get
            {
                int retValue = Convert.ToInt32(Value);
                return retValue.ToString();
            }
            set
            {
                int result;
                if (int.TryParse( value, out result ))
                {
                    this.Value = result;
                }
                
            }
        }

        /// <Summary>Constructs an EnumEditControl</Summary>
        public EnumEditControl( string type )
        {
            this.SystemType = type;
            this.EnumType = Type.GetType( type );
        }

        /// <Summary>Constructs an EnumEditControl</Summary>
        public EnumEditControl()
        {
        }

        /// <Summary>
        /// OnDataChanged runs when the PostbackData has changed.  It raises the ValueChanged
        /// Event
        /// </Summary>
        protected override void OnDataChanged( EventArgs e )
        {
            int intValue = Convert.ToInt32(Value);
            int intOldValue = Convert.ToInt32(OldValue);

            PropertyEditorEventArgs args = new PropertyEditorEventArgs(Name);
            args.Value = Enum.ToObject(EnumType, intValue);
            args.OldValue = Enum.ToObject(EnumType, intOldValue);

            base.OnValueChanged(args);
        }

        /// <Summary>RenderEditMode renders the Edit mode of the control</Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderEditMode( HtmlTextWriter writer )
        {
            int propValue = Convert.ToInt32(Value);
            Array enumValues = Enum.GetValues(EnumType);

            //Render the Select Tag
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Select);

            for (int I = 0; I <= enumValues.Length - 1; I++)
            {
                int enumValue = Convert.ToInt32(enumValues.GetValue(I));
                string enumName = Enum.GetName(EnumType, enumValue);
                enumName = Localization.GetString(enumName, LocalResourceFile);

                //Add the Value Attribute
                writer.AddAttribute(HtmlTextWriterAttribute.Value, enumValue.ToString());

                if (enumValue == propValue)
                {
                    //Add the Selected Attribute
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                }

                //Render Option Tag
                writer.RenderBeginTag(HtmlTextWriterTag.Option);
                writer.Write(enumName);
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
            object propValue;
            try
            {
                propValue = Enum.Parse( EnumType, Value.ToString() );
            }
            catch( ArgumentException e )
            {
                propValue = Value;
            }

            string enumValue = Enum.Format(EnumType, propValue, "G");

            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(enumValue);
            writer.RenderEndTag();
        }
    }
}