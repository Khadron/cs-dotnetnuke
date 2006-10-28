using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The VisibilityControl control provides a base control for defining visibility
    /// options
    /// </Summary>
    [ToolboxData( "<{0}:VisibilityControl runat=server></{0}:VisibilityControl>" )]
    public class VisibilityControl : WebControl, IPostBackDataHandler, INamingContainer
    {

        public event PropertyChangedEventHandler VisibilityChanged
        {
            add
            {
                this.VisibilityChangedEvent = ( (PropertyChangedEventHandler)Delegate.Combine( ( (Delegate)this.VisibilityChangedEvent ), ( (Delegate)value ) ) );
            }
            remove
            {
                this.VisibilityChangedEvent = ( (PropertyChangedEventHandler)Delegate.Remove( ( (Delegate)this.VisibilityChangedEvent ), ( (Delegate)value ) ) );
            }
        }
        private string _Caption;
        private string _Name;
        private object _Value;
        private PropertyChangedEventHandler VisibilityChangedEvent;

        /// <Summary>Caption</Summary>
        public string Caption
        {
            get
            {
                return this._Caption;
            }
            set
            {
                this._Caption = value;
            }
        }

        /// <Summary>Name is the name of the field as a string</Summary>
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        /// <Summary>
        /// StringValue is the value of the control expressed as a String
        /// </Summary>
        public object Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
            }
        }

        /// <Summary>Constructs a VisibilityControl</Summary>
        public VisibilityControl()
        {
        }

        /// <Summary>
        /// LoadPostData loads the Post Back Data and determines whether the value has change
        /// </Summary>
        /// <Param name="postDataKey">A key to the PostBack Data to load</Param>
        /// <Param name="postCollection">
        /// A name value collection of postback data
        /// </Param>
        public virtual bool LoadPostData( string postDataKey, NameValueCollection postCollection )
        {
            bool dataChanged = false;
            string presentValue = Value.ToString();
            string postedValue = postCollection[postDataKey];
            if (!presentValue.Equals(postedValue))
            {
                Value = postedValue;
                dataChanged = true;
            }

            return dataChanged;
        }

        /// <Summary>
        /// OnVisibilityChanged runs when the Visibility has changed.  It raises the VisibilityChanged
        /// Event
        /// </Summary>
        protected virtual void OnVisibilityChanged( PropertyEditorEventArgs e )
        {
            if (VisibilityChangedEvent != null)
            {
                VisibilityChangedEvent(this, e);
            }
        }

        /// <Summary>
        /// RaisePostDataChangedEvent runs when the PostBackData has changed.  It triggers
        /// a ValueChanged Event
        /// </Summary>
        public virtual void RaisePostDataChangedEvent()
        {
            //Raise the VisibilityChanged Event
            int intValue = System.Convert.ToInt32(Value);
            PropertyEditorEventArgs args = new PropertyEditorEventArgs(Name);
            args.Value = System.Enum.ToObject(typeof(DotNetNuke.Entities.Users.UserVisibilityMode), intValue);
            OnVisibilityChanged(args);
        }

        /// <Summary>Render renders the control</Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void Render( HtmlTextWriter writer )
        {
            DotNetNuke.Entities.Users.UserVisibilityMode propValue = (DotNetNuke.Entities.Users.UserVisibilityMode)Value;

            //Render Outer Div
            ControlStyle.AddAttributesToRender(writer);
            AddAttributesToRender(writer);

            //Render Caption
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(Caption);

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
            if (propValue == DotNetNuke.Entities.Users.UserVisibilityMode.AllUsers)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
            writer.Write(DotNetNuke.Services.Localization.Localization.GetString("Public"));

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
            if (propValue == DotNetNuke.Entities.Users.UserVisibilityMode.MembersOnly)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "1");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
            writer.Write(DotNetNuke.Services.Localization.Localization.GetString("MemberOnly"));

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
            if (propValue == DotNetNuke.Entities.Users.UserVisibilityMode.AdminOnly)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "2");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
            writer.Write(DotNetNuke.Services.Localization.Localization.GetString("AdminOnly"));

            //End render outer div
            writer.RenderEndTag();
        }
    }
}