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