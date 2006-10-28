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

        /// <Summary>Constructs a DNNRichTextEditControl</Summary>
        public DNNRichTextEditControl()
        {
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