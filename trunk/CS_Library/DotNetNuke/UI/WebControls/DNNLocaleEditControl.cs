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
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The DNNLocaleEditControl control provides a standard UI component for selecting
    /// a Locale
    /// </Summary>
    [ToolboxData( "<{0}:DNNLocaleEditControl runat=server></{0}:DNNLocaleEditControl>" )]
    public class DNNLocaleEditControl : TextEditControl
    {
        /// <Summary>Constructs a DNNLocaleEditControl</Summary>
        public DNNLocaleEditControl()
        {
        }

        /// <Summary>RenderEditMode renders the Edit mode of the control</Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderEditMode( HtmlTextWriter writer )
        {
            //For convenience create a DropDownList to use
            DropDownList cboLocale = new DropDownList();

            //Load the List with Locales
            Localization.LoadCultureDropDownList(cboLocale, CultureDropDownTypes.NativeName, ((PageBase)Page).PageCulture.Name);

            //Select the relevant item
            if (cboLocale.Items.FindByValue(StringValue) != null)
            {
                cboLocale.ClearSelection();
                cboLocale.Items.FindByValue(StringValue).Selected = true;
            }

            //Render the Select Tag
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Select);

            for (int I = 0; I <= cboLocale.Items.Count - 1; I++)
            {
                string localeValue = cboLocale.Items[I].Value;
                string localeName = cboLocale.Items[I].Text;
                bool isSelected = cboLocale.Items[I].Selected;

                //Add the Value Attribute
                writer.AddAttribute(HtmlTextWriterAttribute.Value, localeValue);

                if (isSelected)
                {
                    //Add the Selected Attribute
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                }

                //Render Option Tag
                writer.RenderBeginTag(HtmlTextWriterTag.Option);
                writer.Write(localeName);
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
            //For convenience create a DropDownList to use
            DropDownList cboLocale = new DropDownList();

            //Load the List with Locales
            Localization.LoadCultureDropDownList(cboLocale, CultureDropDownTypes.NativeName, ((PageBase)Page).PageCulture.Name);

            //Select the relevant item
            if (cboLocale.Items.FindByValue(StringValue) != null)
            {
                cboLocale.ClearSelection();
                cboLocale.Items.FindByValue(StringValue).Selected = true;
            }

            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(cboLocale.SelectedItem.Text);
            writer.RenderEndTag();
        }
    }
}