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
using System.Collections;
using System.Web.UI.WebControls;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.UserControls
{

    public abstract class DualListControl : UserControlBase
    {
        private ArrayList _Assigned;
        private ArrayList _Available;
        private string _DataTextField = "";
        private string _DataValueField = "";
        private bool _Enabled = true;
        private string _ListBoxHeight = "";
        private string _ListBoxWidth = "";
        protected LinkButton cmdAdd;
        protected LinkButton cmdAddAll;
        protected LinkButton cmdRemove;
        protected LinkButton cmdRemoveAll;
        protected Label Label1;
        protected Label Label2;
        protected ListBox lstAssigned;

        protected ListBox lstAvailable;

        private string MyFileName = "DualListControl.ascx";

        public ArrayList Assigned
        {
            get
            {
                ArrayList returnValue;
                ListItem objListItem;

                ArrayList objList = new ArrayList();

                foreach (ListItem tempLoopVar_objListItem in lstAssigned.Items)
                {
                    objListItem = tempLoopVar_objListItem;
                    objList.Add(objListItem);
                }

                returnValue = objList;
                return returnValue;
            }
            set
            {
                this._Assigned = value;
            }
        }

        public ArrayList Available
        {
            get
            {
                ArrayList returnValue;
                ListItem objListItem;

                ArrayList objList = new ArrayList();

                foreach (ListItem tempLoopVar_objListItem in lstAvailable.Items)
                {
                    objListItem = tempLoopVar_objListItem;
                    objList.Add(objListItem);
                }

                returnValue = objList;
                return returnValue;
            }
            set
            {
                this._Available = value;
            }
        }



        public string DataTextField
        {
            set
            {
                this._DataTextField = value;
            }
        }

        public string DataValueField
        {
            set
            {
                this._DataValueField = value;
            }
        }

        public bool Enabled
        {
            set
            {
                this._Enabled = value;
            }
        }

        public string ListBoxHeight
        {
            get
            {
                return Convert.ToString( this.ViewState[( this.ClientID + "_ListBoxHeight" )] );
            }
            set
            {
                this._ListBoxHeight = value;
            }
        }

        public string ListBoxWidth
        {
            get
            {
                return Convert.ToString( this.ViewState[( this.ClientID + "_ListBoxWidth" )] );
            }
            set
            {
                this._ListBoxWidth = value;
            }
        }

      

        public DualListControl()
        {
            base.Load += new EventHandler( this.Page_Load );
            this._ListBoxWidth = "";
            this._ListBoxHeight = "";
            this._DataTextField = "";
            this._DataValueField = "";
            this.cmdRemove.Click += new EventHandler(this.cmdRemove_Click);
            this.cmdRemoveAll.Click += new EventHandler(this.cmdRemoveAll_Click);
            this.cmdAddAll.Click += new EventHandler(this.cmdAddAll_Click);
            this.cmdAdd.Click += new EventHandler(this.cmdAdd_Click);
            this._Enabled = true;
        }

        private void cmdAdd_Click( object sender, EventArgs e )
        {
            ListItem objListItem;

            ArrayList objList = new ArrayList();

            foreach (ListItem tempLoopVar_objListItem in lstAvailable.Items)
            {
                objListItem = tempLoopVar_objListItem;
                objList.Add(objListItem);
            }

            foreach (ListItem tempLoopVar_objListItem in objList)
            {
                objListItem = tempLoopVar_objListItem;
                if (objListItem.Selected)
                {
                    lstAvailable.Items.Remove(objListItem);
                    lstAssigned.Items.Add(objListItem);
                }
            }

            lstAvailable.ClearSelection();
            lstAssigned.ClearSelection();

            Sort(lstAssigned);
        }

        private void cmdAddAll_Click( object sender, EventArgs e )
        {
            ListItem objListItem;

            foreach (ListItem tempLoopVar_objListItem in lstAvailable.Items)
            {
                objListItem = tempLoopVar_objListItem;
                lstAssigned.Items.Add(objListItem);
            }

            lstAvailable.Items.Clear();

            lstAvailable.ClearSelection();
            lstAssigned.ClearSelection();

            Sort(lstAssigned);
        }

        private void cmdRemove_Click( object sender, EventArgs e )
        {
            ListItem objListItem;

            ArrayList objList = new ArrayList();

            foreach (ListItem tempLoopVar_objListItem in lstAssigned.Items)
            {
                objListItem = tempLoopVar_objListItem;
                objList.Add(objListItem);
            }

            foreach (ListItem tempLoopVar_objListItem in objList)
            {
                objListItem = tempLoopVar_objListItem;
                if (objListItem.Selected)
                {
                    lstAssigned.Items.Remove(objListItem);
                    lstAvailable.Items.Add(objListItem);
                }
            }

            lstAvailable.ClearSelection();
            lstAssigned.ClearSelection();

            Sort(lstAvailable);
        }

        private void cmdRemoveAll_Click( object sender, EventArgs e )
        {
            ListItem objListItem;

            foreach (ListItem tempLoopVar_objListItem in lstAssigned.Items)
            {
                objListItem = tempLoopVar_objListItem;
                lstAvailable.Items.Add(objListItem);
            }

            lstAssigned.Items.Clear();

            lstAvailable.ClearSelection();
            lstAssigned.ClearSelection();

            Sort(lstAvailable);
        }

        private void Page_Load( object sender, EventArgs e )
        {
            try
            {
                //Localization
                Label1.Text = Localization.GetString("Available", Localization.GetResourceFile(this, MyFileName));
                Label2.Text = Localization.GetString("Assigned", Localization.GetResourceFile(this, MyFileName));
                cmdAdd.ToolTip = Localization.GetString("Add", Localization.GetResourceFile(this, MyFileName));
                cmdAddAll.ToolTip = Localization.GetString("AddAll", Localization.GetResourceFile(this, MyFileName));
                cmdRemove.ToolTip = Localization.GetString("Remove", Localization.GetResourceFile(this, MyFileName));
                cmdRemoveAll.ToolTip = Localization.GetString("RemoveAll", Localization.GetResourceFile(this, MyFileName));

                if (!Page.IsPostBack)
                {
                    // set dimensions of control
                    if (_ListBoxWidth != "")
                    {
                        lstAvailable.Width = Unit.Parse(_ListBoxWidth);
                        lstAssigned.Width = Unit.Parse(_ListBoxWidth);
                    }
                    if (_ListBoxHeight != "")
                    {
                        lstAvailable.Height = Unit.Parse(_ListBoxHeight);
                        lstAssigned.Height = Unit.Parse(_ListBoxHeight);
                    }

                    // load available
                    lstAvailable.DataTextField = _DataTextField;
                    lstAvailable.DataValueField = _DataValueField;
                    lstAvailable.DataSource = _Available;
                    lstAvailable.DataBind();
                    Sort(lstAvailable);

                    // load selected
                    lstAssigned.DataTextField = _DataTextField;
                    lstAssigned.DataValueField = _DataValueField;
                    lstAssigned.DataSource = _Assigned;
                    lstAssigned.DataBind();
                    Sort(lstAssigned);

                    // set enabled
                    lstAvailable.Enabled = _Enabled;
                    lstAssigned.Enabled = _Enabled;

                    // save persistent values
                    ViewState[this.ClientID + "_ListBoxWidth"] = _ListBoxWidth;
                    ViewState[this.ClientID + "_ListBoxHeight"] = _ListBoxHeight;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void Sort( ListBox ctlListBox )
        {
            ArrayList arrListItems = new ArrayList();
            ListItem objListItem;

            // store listitems in temp arraylist
            foreach (ListItem tempLoopVar_objListItem in ctlListBox.Items)
            {
                objListItem = tempLoopVar_objListItem;
                arrListItems.Add(objListItem);
            }

            // sort arraylist based on text value
            arrListItems.Sort(new ListItemComparer());

            // clear control
            ctlListBox.Items.Clear();

            // add listitems to control
            foreach (ListItem tempLoopVar_objListItem in arrListItems)
            {
                objListItem = tempLoopVar_objListItem;
                ctlListBox.Items.Add(objListItem);
            }
        }
    }
}