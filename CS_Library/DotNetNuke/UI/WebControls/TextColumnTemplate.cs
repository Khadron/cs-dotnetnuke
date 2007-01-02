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
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The TextColumnTemplate provides a Template for the TextColumn
    /// </Summary>
    public class TextColumnTemplate : ITemplate
    {
        private string mDataField;
        private bool mDesignMode;
        private ListItemType mItemType;
        private string mText;
        private Unit mWidth;

        /// <Summary>The Data Field is the field that binds to the Text Column</Summary>
        public string DataField
        {
            get
            {
                return this.mDataField;
            }
            set
            {
                this.mDataField = value;
            }
        }

        /// <Summary>Gets or sets the Design Mode of the Column</Summary>
        public bool DesignMode
        {
            get
            {
                return this.mDesignMode;
            }
            set
            {
                this.mDesignMode = value;
            }
        }

        /// <Summary>The type of Template to Create</Summary>
        public ListItemType ItemType
        {
            get
            {
                return this.mItemType;
            }
            set
            {
                this.mItemType = value;
            }
        }

        /// <Summary>Gets or sets the Text (for Header/Footer Templates)</Summary>
        public string Text
        {
            get
            {
                return this.mText;
            }
            set
            {
                this.mText = value;
            }
        }

        /// <Summary>Gets or sets the Width of the Column</Summary>
        public Unit Width
        {
            get
            {
                return this.mWidth;
            }
            set
            {
                this.mWidth = value;
            }
        }

        public TextColumnTemplate( ListItemType itemType )
        {
            this.mItemType = ListItemType.Item;
            this.ItemType = itemType;
        }

        public TextColumnTemplate() : this( ListItemType.Item )
        {
        }

        /// <Summary>Gets the value of the Data Field</Summary>
        /// <Param name="container">The parent container (DataGridItem)</Param>
        private string GetValue( DataGridItem container )
        {
            string itemValue = DotNetNuke.Common.Utilities.Null.NullString;
            if (!String.IsNullOrEmpty(DataField))
            {
                if (DesignMode)
                {
                    itemValue = "DataBound to " + DataField;
                }
                else
                {
                    itemValue = DataBinder.Eval(container.DataItem, DataField).ToString();
                }
            }

            return itemValue;
        }

        /// <Summary>
        /// InstantiateIn instantiates the template (implementation of ITemplate)
        /// </Summary>
        /// <Param name="container">The parent container (DataGridItem)</Param>
        public virtual void InstantiateIn( Control container )
        {
            Label lblText;
            TextBox txtText;
            switch (ItemType)
            {
                case ListItemType.Item:
                    //Add a Text Label
                    lblText = new Label();
                    lblText.Width = Width;
                    lblText.DataBinding += new System.EventHandler(Item_DataBinding);
                    container.Controls.Add(lblText);
                    break;

                case ListItemType.AlternatingItem:
                    //Add a Text Label
                    lblText = new Label();
                    lblText.Width = Width;
                    lblText.DataBinding += new System.EventHandler(Item_DataBinding);
                    container.Controls.Add(lblText);
                    break;

                case ListItemType.SelectedItem:

                    //Add a Text Label
                    lblText = new Label();
                    lblText.Width = Width;
                    lblText.DataBinding += new System.EventHandler(Item_DataBinding);
                    container.Controls.Add(lblText);
                    break;
                case ListItemType.EditItem:

                    //Add a Text Box
                    txtText = new TextBox();
                    txtText.Width = Width;
                    txtText.DataBinding += new System.EventHandler(Item_DataBinding);
                    container.Controls.Add(txtText);
                    break;
                case ListItemType.Footer:
                    container.Controls.Add(new LiteralControl(Text));
                    break;

                case ListItemType.Header:

                    container.Controls.Add(new LiteralControl(Text));
                    break;
            }
        }

        /// <Summary>
        /// Item_DataBinding runs when an Item of type ListItemType.Item is being data-bound
        /// </Summary>
        /// <Param name="sender">The object that triggers the event</Param>
        /// <Param name="e">An EventArgs object</Param>
        private void Item_DataBinding( object sender, EventArgs e )
        {
            DataGridItem container;
            int keyValue = DotNetNuke.Common.Utilities.Null.NullInteger;
            Label lblText;
            TextBox txtText;
            switch (ItemType)
            {
                case ListItemType.Item:
                    lblText = (Label)sender;
                    container = (DataGridItem)lblText.NamingContainer;
                    lblText.Text = GetValue(container);
                    break;

                case ListItemType.AlternatingItem:
                    lblText = (Label)sender;
                    container = (DataGridItem)lblText.NamingContainer;
                    lblText.Text = GetValue(container);
                    break;

                case ListItemType.SelectedItem:

                    lblText = (Label)sender;
                    container = (DataGridItem)lblText.NamingContainer;
                    lblText.Text = GetValue(container);
                    break;
                case ListItemType.EditItem:

                    txtText = (TextBox)sender;
                    container = (DataGridItem)txtText.NamingContainer;
                    txtText.Text = GetValue(container);
                    break;
            }
        }
    }
}