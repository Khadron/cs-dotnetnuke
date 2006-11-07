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
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The CheckBoxColumnTemplate provides a Template for the CheckBoxColumn
    /// </Summary>
    public class CheckBoxColumnTemplate : ITemplate
    {

        public event DNNDataGridCheckedColumnEventHandler CheckedChanged
        {
            add
            {
                this.CheckedChangedEvent += value;
            }
            remove
            {
                this.CheckedChangedEvent -= value;
            }
        }
        private DNNDataGridCheckedColumnEventHandler CheckedChangedEvent;
        private bool mAutoPostBack;
        private bool mChecked;
        private string mDataField;
        private bool mDesignMode;
        private bool mEnabled;
        private string mEnabledField;
        private ListItemType mItemType;
        private string mText;

        /// <Summary>
        /// Gets and sets whether the column fires a postback when any check box is changed
        /// </Summary>
        public bool AutoPostBack
        {
            get
            {
                return this.mAutoPostBack;
            }
            set
            {
                this.mAutoPostBack = value;
            }
        }

        /// <Summary>
        /// Gets and sets whether the checkbox is checked (unless DataBound)
        /// </Summary>
        public bool Checked
        {
            get
            {
                return this.mChecked;
            }
            set
            {
                this.mChecked = value;
            }
        }

        /// <Summary>The Data Field that the column should bind to</Summary>
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

        /// <Summary>
        /// An flag that indicates whether the hcekboxes are enabled (this is overridden if
        /// the EnabledField is set changed
        /// </Summary>
        public bool Enabled
        {
            get
            {
                return this.mEnabled;
            }
            set
            {
                this.mEnabled = value;
            }
        }

        /// <Summary>
        /// The Data Field that determines whether the checkbox is Enabled changed
        /// </Summary>
        public string EnabledField
        {
            get
            {
                return this.mEnabledField;
            }
            set
            {
                this.mEnabledField = value;
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

        /// <Summary>The Text to display in a Header Template</Summary>
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

        public CheckBoxColumnTemplate( ListItemType itemType )
        {
            this.mAutoPostBack = false;
            this.mChecked = false;
            this.mDataField = Null.NullString;
            this.mEnabled = true;
            this.mEnabledField = Null.NullString;
            this.mItemType = ListItemType.Item;
            this.mText = "";
            this.ItemType = itemType;
        }

        public CheckBoxColumnTemplate() : this( ListItemType.Item )
        {
        }

        /// <Summary>
        /// InstantiateIn is called when the Template is instantiated by the parent control
        /// </Summary>
        /// <Param name="container">The container control</Param>
        public virtual void InstantiateIn( Control container )
        {
            CheckBox box = new CheckBox();
            box.AutoPostBack = AutoPostBack;

            box.DataBinding += new EventHandler(Item_DataBinding);
            box.CheckedChanged += new EventHandler(OnCheckChanged);

            if (Text != "")
            {
                container.Controls.Add(new LiteralControl(Text + "<br/>"));
            }
            container.Controls.Add(box);
        }

        /// <Summary>Called when the template item is Data Bound</Summary>
        private void Item_DataBinding( object sender, EventArgs e )
        {
            CheckBox box = (CheckBox)sender;
            DataGridItem container = (DataGridItem)box.NamingContainer;

            if (DataField != "" && ItemType != ListItemType.Header)
            {
                if (DesignMode)
                {
                    box.Checked = false;
                }
                else
                {
                    box.Checked = Convert.ToBoolean(DataBinder.Eval(container.DataItem, DataField));
                }
            }
            else
            {
                box.Checked = this.Checked;
            }

            if (EnabledField != "")
            {
                if (DesignMode)
                {
                    box.Enabled = false;
                }
                else
                {
                    box.Enabled = Convert.ToBoolean(DataBinder.Eval(container.DataItem, EnabledField));
                }
            }
            else
            {
                box.Enabled = this.Enabled;
            }
        }

        /// <Summary>
        /// Centralised Event that is raised whenever a check box's state is modified
        /// </Summary>
        private void OnCheckChanged( object sender, EventArgs e )
        {
            CheckBox box = (CheckBox)sender;
            DataGridItem container = (DataGridItem)box.NamingContainer;
            DNNDataGridCheckChangedEventArgs evntArgs;

            if (container.ItemIndex == Null.NullInteger)
            {
                evntArgs = new DNNDataGridCheckChangedEventArgs(container, box.Checked, DataField, true);
            }
            else
            {
                evntArgs = new DNNDataGridCheckChangedEventArgs(container, box.Checked, DataField, false);
            }

            if (CheckedChangedEvent != null)
            {
                CheckedChangedEvent(sender, evntArgs);
            }
        }
    }
}