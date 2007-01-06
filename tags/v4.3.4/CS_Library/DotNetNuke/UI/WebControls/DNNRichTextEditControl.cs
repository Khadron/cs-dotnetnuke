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
using System.Collections.Specialized;
using System.Web.UI;
using DotNetNuke.Modules.HTMLEditorProvider;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The DNNRichTextEditControl control provides a standard UI component for editing
    /// RichText
    /// </Summary>
    [ToolboxData( "<{0}:DNNRichTextEditControl runat=server></{0}:DNNRichTextEditControl>" )]
    public class DNNRichTextEditControl : TextEditControl
    {
        private HtmlEditorProvider RichTextEditor;

        /// <Summary>
        /// LoadPostData loads the Post Back Data and determines whether the value has change
        /// </Summary>
        /// <Param name="postDataKey">A key to the PostBack Data to load</Param>
        /// <Param name="postCollection">
        /// A name value collection of postback data
        /// </Param>
        public override bool LoadPostData( string postDataKey, NameValueCollection postCollection )
        {
            this.EnsureChildControls();
            return base.LoadPostData( ( this.ClientID + "edit" ), postCollection );
        }

        /// <Summary>CreateChildControls creates the controls collection</Summary>
        protected override void CreateChildControls()
        {
            RichTextEditor = HtmlEditorProvider.Instance();
            RichTextEditor.ControlID = this.ID + "edit";
            RichTextEditor.Initialize();
            RichTextEditor.Height = this.ControlStyle.Height;
            RichTextEditor.Width = this.ControlStyle.Width;

            Controls.Clear();
            Controls.Add(RichTextEditor.HtmlEditorControl);

            base.CreateChildControls();
        }

        /// <Summary>
        /// OnDataChanged runs when the PostbackData has changed.  It raises the ValueChanged
        /// Event
        /// </Summary>
        protected override void OnDataChanged( EventArgs e )
        {
            string strValue = Convert.ToString(Value);
            string strOldValue = Convert.ToString(OldValue);

            PropertyEditorEventArgs args = new PropertyEditorEventArgs(Name);
            args.Value = this.Page.Server.HtmlEncode(strValue);
            args.OldValue = this.Page.Server.HtmlEncode(strOldValue);
            args.StringValue = this.Page.Server.HtmlEncode(StringValue);

            base.OnValueChanged(args);
        }

        /// <Summary>
        /// OnPreRender runs just before the control is rendered.  It forces a postback to the
        /// Control.
        /// </Summary>
        protected override void OnPreRender( EventArgs e )
        {
            base.OnPreRender(e);

            RichTextEditor.Text = this.Page.Server.HtmlDecode(Convert.ToString(this.Value));

            if (Page != null && this.EditMode == PropertyEditorMode.Edit)
            {
                this.Page.RegisterRequiresPostBack(this);
            }
        }

        /// <Summary>
        /// Render is called by the .NET framework to render the control
        /// </Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderEditMode( HtmlTextWriter writer )
        {
            this.RenderChildren( writer );
        }

        /// <Summary>
        /// RenderViewMode renders the View (readonly) mode of the control
        /// </Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderViewMode( HtmlTextWriter writer )
        {
            string propValue = this.Page.Server.HtmlDecode(Convert.ToString(this.Value));

            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(propValue);
            writer.RenderEndTag();
        }
    }
}