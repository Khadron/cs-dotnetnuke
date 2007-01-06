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
                return Convert.ToString( OldValue );
            }
        }

        /// <Summary>
        /// StringValue is the value of the control expressed as a String
        /// </Summary>
        protected override string StringValue
        {
            get
            {
                return Convert.ToString( Value );
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
            PropertyEditorEventArgs args = new PropertyEditorEventArgs( Name );
            args.Value = StringValue;
            args.OldValue = OldStringValue;
            args.StringValue = StringValue;
            base.OnValueChanged( args );
        }

        /// <Summary>RenderEditMode renders the Edit mode of the control</Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderEditMode( HtmlTextWriter writer )
        {
            int length = Null.NullInteger;
            if( CustomAttributes != null )
            {
                foreach( Attribute attribute in CustomAttributes )
                {
                    if( attribute is MaxLengthAttribute )
                    {
                        MaxLengthAttribute lengthAtt = (MaxLengthAttribute)attribute;
                        length = lengthAtt.Length;
                    }
                }
            }

            ControlStyle.AddAttributesToRender( writer );
            writer.AddAttribute( HtmlTextWriterAttribute.Type, "text" );
            writer.AddAttribute( HtmlTextWriterAttribute.Value, StringValue );
            if( length > Null.NullInteger )
            {
                writer.AddAttribute( HtmlTextWriterAttribute.Maxlength, length.ToString() );
            }
            writer.AddAttribute( HtmlTextWriterAttribute.Name, this.UniqueID );
            writer.RenderBeginTag( HtmlTextWriterTag.Input );
            writer.RenderEndTag();
        }
    }
}