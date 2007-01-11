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
using System.Web.UI;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The DNNListEditControl control provides a standard UI component for selecting
    /// from Lists
    /// </Summary>
    [ToolboxData( "<{0}:DNNListEditControl runat=server></{0}:DNNListEditControl>" )]
    public class DNNListEditControl : EditControl, IPostBackEventHandler
    {

        public event PropertyChangedEventHandler ItemChanged
        {
            add
            {
                this.ItemChangedEvent += value;
            }
            remove
            {
                this.ItemChangedEvent -= value;
            }
        }
        private bool _AutoPostBack;
        private ListEntryInfoCollection _IList;
        private string _ListName;
        private string _ParentKey;
        private ListBoundField _TextField;
        private ListBoundField _ValueField;
        private PropertyChangedEventHandler ItemChangedEvent;

        /// <Summary>Determines whether the List Auto Posts Back</Summary>
        protected bool AutoPostBack
        {
            get
            {
                return this._AutoPostBack;
            }
            set
            {
                this._AutoPostBack = value;
            }
        }

        /// <Summary>
        /// IntegerValue returns the Integer representation of the Value
        /// </Summary>
        protected int IntegerValue
        {
            get
            {
                int intValue = Null.NullInteger;
                try
                {
                    //Try and cast the value to an Integer
                    intValue = Convert.ToInt32(Value);
                }
                catch (Exception)
                {
                }
                return intValue;
            }
        }

        /// <Summary>List gets the List associated with the control</Summary>
        protected ListEntryInfoCollection List
        {
            get
            {
                if (_IList == null)
                {
                    ListController objListController = new ListController();
                    _IList = objListController.GetListEntryInfoCollection(ListName, "", ParentKey);
                }
                return _IList;
            }
        }

        /// <Summary>ListName is the name of the List to display</Summary>
        protected virtual string ListName
        {
            get
            {
                if (_ListName == Null.NullString)
                {
                    _ListName = this.Name;
                }
                return _ListName;
            }
            set
            {
                _ListName = value;
            }
        }


        /// <Summary>
        /// OldIntegerValue returns the Integer representation of the OldValue
        /// </Summary>
        protected int OldIntegerValue
        {
            get
            {
                int intValue = Null.NullInteger;
                try
                {
                    //Try and cast the value to an Integer
                    intValue = Convert.ToInt32(OldValue);
                }
                catch (Exception)
                {
                }
                return intValue;
            }
        }

        /// <Summary>
        /// OldStringValue returns the Boolean representation of the OldValue
        /// </Summary>
        protected string OldStringValue
        {
            get
            {
                return Convert.ToString(OldValue);
            }
        }

        /// <Summary>ListName is the name of the List to display</Summary>
        protected virtual string ParentKey
        {
            get
            {
                return this._ParentKey;
            }
            set
            {
                this._ParentKey = value;
            }
        }

        /// <Summary>
        /// StringValue is the value of the control expressed as a String
        /// </Summary>
        protected override string StringValue
        {
            get
            {
                return Convert.ToString(Value);
            }
            set
            {
                if (ValueField == ListBoundField.Id)
                {
                    //Integer type field
                    this.Value = int.Parse(value);
                }
                else
                {
                    //String type Field
                    this.Value = Value;
                }
            }
        }

        /// <Summary>TextField is the field to display in the combo</Summary>
        protected virtual ListBoundField TextField
        {
            get
            {
                return this._TextField;
            }
            set
            {
                this._TextField = value;
            }
        }

        /// <Summary>ValueField is the field to use as the combo item values</Summary>
        protected virtual ListBoundField ValueField
        {
            get
            {
                return this._ValueField;
            }
            set
            {
                this._ValueField = value;
            }
        }

        /// <Summary>Constructs a DNNListEditControl</Summary>
        public DNNListEditControl()
        {
            this._ListName = Null.NullString;
            this._ParentKey = Null.NullString;
            this._TextField = ListBoundField.Text;
            this._ValueField = ListBoundField.Value;
        }

        private PropertyEditorEventArgs GetEventArgs()
        {
            PropertyEditorEventArgs args = new PropertyEditorEventArgs(Name);
            if (ValueField == ListBoundField.Id)
            {
                //This is an Integer Value
                args.Value = IntegerValue;
                args.OldValue = OldIntegerValue;
            }
            else
            {
                //This is a String Value
                args.Value = StringValue;
                args.OldValue = OldStringValue;
            }
            args.StringValue = StringValue;
            return args;
        }

        /// <Summary>
        /// OnAttributesChanged runs when the CustomAttributes property has changed.
        /// </Summary>
        protected override void OnAttributesChanged()
        {
            //Get the List settings out of the "Attributes"
            if (CustomAttributes != null)
            {
                foreach (Attribute attribute in CustomAttributes)
                {
                    if (attribute is ListAttribute)
                    {
                        ListAttribute listAtt = (ListAttribute)attribute;
                        ListName = listAtt.ListName;
                        ParentKey = listAtt.ParentKey;
                        TextField = listAtt.TextField;
                        ValueField = listAtt.ValueField;
                        break;
                    }
                }
            }
        }

        /// <Summary>
        /// OnDataChanged runs when the PostbackData has changed.  It raises the ValueChanged
        /// Event
        /// </Summary>
        protected override void OnDataChanged( EventArgs e )
        {
            base.OnValueChanged( this.GetEventArgs() );
        }

        /// <Summary>OnItemChanged runs when the Item has changed</Summary>
        protected virtual void OnItemChanged( PropertyEditorEventArgs e )
        {
            if (ItemChangedEvent != null)
            {
                ItemChangedEvent(this, e);
            }
        }

        public virtual void RaisePostBackEvent( string eventArgument )
        {
            if (AutoPostBack)
            {
                OnItemChanged(GetEventArgs());
            }
        }

        /// <Summary>RenderEditMode renders the Edit mode of the control</Summary>
        /// <Param name="writer">A HtmlTextWriter.</Param>
        protected override void RenderEditMode( HtmlTextWriter writer )
        {
            //Render the Select Tag
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            if (AutoPostBack)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Onchange, Page.ClientScript.GetPostBackEventReference(this, this.ID.ToString()));
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Select);

            //Add the Not Specified Option
            if (!Required)
            {
                if (ValueField == ListBoundField.Text)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, Null.NullString);
                }
                else
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, Null.NullString);
                }
                if (StringValue == Null.NullString)
                {
                    //Add the Selected Attribute
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Option);
                writer.Write("<" + Localization.GetString("Not_Specified", Localization.SharedResourceFile) + ">");
                writer.RenderEndTag();
            }

            for (int I = 0; I <= List.Count - 1; I++)
            {
                ListEntryInfo item = List.Item(I);
                string itemValue = Null.NullString;

                //Add the Value Attribute
                switch (ValueField)
                {
                    case ListBoundField.Id:

                        itemValue = item.EntryID.ToString();
                        break;
                    case ListBoundField.Text:

                        itemValue = item.Text;
                        break;
                    case ListBoundField.Value:

                        itemValue = item.Value;
                        break;
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Value, itemValue);
                if (StringValue == itemValue)
                {
                    //Add the Selected Attribute
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                }
                //Render Option Tag
                writer.RenderBeginTag(HtmlTextWriterTag.Option);
                switch (TextField)
                {
                    case ListBoundField.Id:

                        writer.Write(item.EntryID.ToString());
                        break;
                    case ListBoundField.Text:

                        writer.Write(item.Text);
                        break;
                    case ListBoundField.Value:

                        writer.Write(item.Value);
                        break;
                }
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
            ListController objListController = new ListController();
            ListEntryInfo entry = null;
            string entryText = Null.NullString;

            switch (ValueField)
            {
                case ListBoundField.Id:

                    entry = objListController.GetListEntryInfo(Convert.ToInt32(Value));
                    break;
                case ListBoundField.Text:

                    entryText = StringValue;
                    break;
                case ListBoundField.Value:

                    entry = objListController.GetListEntryInfo(ListName, StringValue);
                    break;
            }

            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);

            if (entry != null)
            {
                switch (TextField)
                {
                    case ListBoundField.Id:

                        writer.Write(entry.EntryID.ToString());
                        break;
                    case ListBoundField.Text:

                        writer.Write(entry.Text);
                        break;
                    case ListBoundField.Value:

                        writer.Write(entry.Value);
                        break;
                }
            }
            else
            {
                writer.Write(entryText);
            }

            //Close Select Tag
            writer.RenderEndTag();
        }
    }
}