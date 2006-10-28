using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Security;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The EditControl control provides a standard UI component for editing properties.
    /// </Summary>
    [ValidationProperty( "Value" )]
    public abstract class EditControl : WebControl, IPostBackDataHandler
    {

        public event PropertyChangedEventHandler ValueChanged
        {
            add
            {
                this.ValueChangedEvent += value;
            }
            remove
            {
                this.ValueChangedEvent -= value;
            }
        }
        private object[] _CustomAttributes;
        private PropertyEditorMode _EditMode;
        private string _LocalResourceFile;
        private string _Name;
        private object _OldValue;
        private bool _Required;
        private string _SystemType;
        private object _Value;
        private PropertyChangedEventHandler ValueChangedEvent;

        /// <Summary>Gets and sets the Custom Attributes for this Control</Summary>
        public object[] CustomAttributes
        {
            get
            {
                return this._CustomAttributes;
            }
            set
            {
                this._CustomAttributes = value;
                if ((_CustomAttributes != null) && _CustomAttributes.Length > 0)
                {
                    OnAttributesChanged();
                }
            }
        }

        /// <Summary>Gets and sets the Edit Mode of the Editor</Summary>
        public PropertyEditorMode EditMode
        {
            get
            {
                return this._EditMode;
            }
            set
            {
                this._EditMode = value;
            }
        }

        /// <Summary>Returns whether the</Summary>
        public virtual bool IsValid
        {
            get
            {
                return true;
            }
        }

        /// <Summary>Gets and sets the Local Resource File for the Control</Summary>
        public string LocalResourceFile
        {
            get
            {
                return this._LocalResourceFile;
            }
            set
            {
                this._LocalResourceFile = value;
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

        /// <Summary>OldValue is the initial value of the field</Summary>
        public object OldValue
        {
            get
            {
                return this._OldValue;
            }
            set
            {
                this._OldValue = value;
            }
        }

        /// <Summary>gets and sets whether the Property is required</Summary>
        public bool Required
        {
            get
            {
                return this._Required;
            }
            set
            {
                this._Required = value;
            }
        }

        /// <Summary>
        /// StringValue is the value of the control expressed as a String
        /// </Summary>
        protected abstract string StringValue { get; set; }

        /// <Summary>SystemType is the System Data Type for the property</Summary>
        public string SystemType
        {
            get
            {
                return this._SystemType;
            }
            set
            {
                this._SystemType = value;
            }
        }

        /// <Summary>Value is the value of the control</Summary>
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

        /// <Summary>Constructs an EditControl</Summary>
        public EditControl()
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
        /// OnAttributesChanged runs when the CustomAttributes property has changed.
        /// </Summary>
        protected virtual void OnAttributesChanged()
        {
        }

        /// <Summary>
        /// OnDataChanged runs when the PostbackData has changed.  It raises the ValueChanged
        /// Event
        /// </Summary>
        protected abstract void OnDataChanged( EventArgs e );

        /// <Summary>
        /// OnValueChanged runs when the Value has changed.  It raises the ValueChanged
        /// Event
        /// </Summary>
        protected virtual void OnValueChanged( PropertyEditorEventArgs e )
        {
            if (ValueChangedEvent != null)
            {
                ValueChangedEvent(this, e);
            }
        }

        /// <Summary>
        /// RaisePostDataChangedEvent runs when the PostBackData has changed.  It triggers
        /// a ValueChanged Event
        /// </Summary>
        public virtual void RaisePostDataChangedEvent()
        {
            this.OnDataChanged( EventArgs.Empty );
        }

        /// <Summary>
        /// Render is called by the .NET framework to render the control
        /// </Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void Render( HtmlTextWriter writer )
        {
            if (EditMode == PropertyEditorMode.Edit || (Required && OldValue.ToString() == ""))
            {
                RenderEditMode(writer);
            }
            else
            {
                RenderViewMode(writer);
            }
        }

        /// <Summary>RenderEditMode renders the Edit mode of the control</Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected virtual void RenderEditMode( HtmlTextWriter writer )
        {
            string propValue = Convert.ToString(this.Value);

            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            writer.AddAttribute(HtmlTextWriterAttribute.Value, propValue);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }

        /// <Summary>
        /// RenderViewMode renders the View (readonly) mode of the control
        /// </Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected virtual void RenderViewMode( HtmlTextWriter writer )
        {
            string propValue = Convert.ToString(this.Value);

            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            PortalSecurity security = new PortalSecurity();
            writer.Write(security.InputFilter(propValue, PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup | PortalSecurity.FilterFlag.NoScripting));
            writer.RenderEndTag();
        }
    }
}