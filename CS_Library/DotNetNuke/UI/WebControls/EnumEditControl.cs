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
                int i1 = int.Parse( value );
                this.Value = i1;
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
            int propValue = Convert.ToInt32(Value);
            string enumValue = Enum.Format(EnumType, propValue, "G");

            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(enumValue);
            writer.RenderEndTag();
        }
    }
}