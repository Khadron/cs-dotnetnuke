#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
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