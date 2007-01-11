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
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.UI.UserControls
{

    /// <Summary>The Address UserControl is used to manage User Addresses</Summary>
    public abstract class Address : UserControlBase
    {
        private string _Cell;
        private string _City;
        private string _ControlColumnWidth = "";
        private string _Country;

        private string _CountryData = "Text";
        private string _Fax;
        private string _LabelColumnWidth = "";

        private int _ModuleId;
        private string _Postal;
        private string _Region;
        private string _RegionData = "Text";
        private bool _ShowCell = true;
        private bool _ShowCity = true;
        private bool _ShowCountry = true;
        private bool _ShowFax = true;
        private bool _ShowPostal = true;
        private bool _ShowRegion = true;

        private bool _ShowStreet = true;
        private bool _ShowTelephone = true;
        private bool _ShowUnit = true;
        private int _StartTabIndex = 1;

        private string _Street;
        private string _Telephone;
        private string _Unit;
        protected CountryListBox cboCountry;
        protected DropDownList cboRegion;
        protected CheckBox chkCell;
        protected CheckBox chkCity;
        protected CheckBox chkCountry;
        protected CheckBox chkFax;
        protected CheckBox chkPostal;
        protected CheckBox chkRegion;
        protected CheckBox chkStreet;
        protected CheckBox chkTelephone;
        protected Label lblCellRequired;
        protected Label lblCityRequired;
        protected Label lblCountryRequired;
        protected Label lblFaxRequired;
        protected Label lblPostalRequired;
        protected Label lblRegion;
        protected Label lblRegionRequired;
        protected Label lblStreetRequired;
        protected Label lblTelephoneRequired;

        private string MyFileName = "Address.ascx";
        protected LabelControl plCell;
        protected LabelControl plCity;
        protected LabelControl plCountry;
        protected LabelControl plFax;
        protected LabelControl plPostal;
        protected LabelControl plRegion;
        protected LabelControl plStreet;
        protected LabelControl plTelephone;
        protected LabelControl plUnit;

        protected HtmlTableRow rowCell;

        protected HtmlTableRow rowCity;

        protected HtmlTableRow rowCountry;

        protected HtmlTableRow rowFax;

        protected HtmlTableRow rowPostal;

        protected HtmlTableRow rowRegion;
        protected HtmlTableRow rowStreet;

        protected HtmlTableRow rowTelephone;

        protected HtmlTableRow rowUnit;
        protected TextBox txtCell;
        protected TextBox txtCity;
        protected TextBox txtFax;
        protected TextBox txtPostal;
        protected TextBox txtRegion;
        protected TextBox txtStreet;
        protected TextBox txtTelephone;
        protected TextBox txtUnit;
        protected RequiredFieldValidator valCell;
        protected RequiredFieldValidator valCity;
        protected RequiredFieldValidator valCountry;
        protected RequiredFieldValidator valFax;
        protected RequiredFieldValidator valPostal;
        protected RequiredFieldValidator valRegion1;
        protected RequiredFieldValidator valRegion2;
        protected RequiredFieldValidator valStreet;
        protected RequiredFieldValidator valTelephone;



        

        public string Cell
        {
            get
            {
                return this.txtCell.Text;
            }
            set
            {
                this._Cell = value;
            }
        }



        public string City
        {
            get
            {
                return this.txtCity.Text;
            }
            set
            {
                this._City = value;
            }
        }

        public string ControlColumnWidth
        {
            get
            {
                return Convert.ToString(ViewState["ControlColumnWidth"]);
            }
            set
            {
                this._ControlColumnWidth = value;
            }
        }

        public string Country
        {
            get
            {
                string retValue = "";
                if (cboCountry.SelectedItem != null)
                {
                    switch (_CountryData.ToLower())
                    {
                        case "text":

                            if (cboCountry.SelectedIndex == 0) //Return blank if 'Not_Specified' selected
                            {
                                retValue = "";
                            }
                            else
                            {
                                retValue = cboCountry.SelectedItem.Text;
                            }
                            break;
                        case "value":

                            retValue = cboCountry.SelectedItem.Value;
                            break;
                    }
                }
                return retValue;
            }
            set
            {
                this._Country = value;
            }
        }

        public string CountryData
        {
            set
            {
                this._CountryData = value;
            }
        }

        public string Fax
        {
            get
            {
                return this.txtFax.Text;
            }
            set
            {
                this._Fax = value;
            }
        }

        public string LabelColumnWidth
        {
            get
            {
                return Convert.ToString( this.ViewState["LabelColumnWidth"] );
            }
            set
            {
                this._LabelColumnWidth = value;
            }
        }

        public string LocalResourceFile
        {
            get
            {
                return Localization.GetResourceFile( this, this.MyFileName );
            }
        }

        public int ModuleId
        {
            get
            {
                return Convert.ToInt32( this.ViewState["ModuleId"] );
            }
            set
            {
                this._ModuleId = value;
            }
        }

        public string Postal
        {
            get
            {
                return this.txtPostal.Text;
            }
            set
            {
                this._Postal = value;
            }
        }

        public string Region
        {
            get
            {
                string retValue = "";
                if (cboRegion.Visible)
                {
                    if (cboRegion.SelectedItem != null)
                    {
                        switch (_RegionData.ToLower())
                        {
                            case "text":

                                if (cboRegion.SelectedIndex > 0)
                                {
                                    retValue = cboRegion.SelectedItem.Text;
                                }
                                break;
                            case "value":

                                retValue = cboRegion.SelectedItem.Value;
                                break;
                        }
                    }
                }
                else
                {
                    retValue = txtRegion.Text;
                }
                return retValue;
            }
            set
            {
                this._Region = value;
            }
        }

        public string RegionData
        {
            set
            {
                this._RegionData = value;
            }
        }

        public bool ShowCell
        {
            set
            {
                this._ShowCell = value;
            }
        }

        public bool ShowCity
        {
            set
            {
                this._ShowCity = value;
            }
        }

        public bool ShowCountry
        {
            set
            {
                this._ShowCountry = value;
            }
        }

        public bool ShowFax
        {
            set
            {
                this._ShowFax = value;
            }
        }

        public bool ShowPostal
        {
            set
            {
                this._ShowPostal = value;
            }
        }

        public bool ShowRegion
        {
            set
            {
                this._ShowRegion = value;
            }
        }

        public bool ShowStreet
        {
            set
            {
                this._ShowStreet = value;
            }
        }

        public bool ShowTelephone
        {
            set
            {
                this._ShowTelephone = value;
            }
        }

        public bool ShowUnit
        {
            set
            {
                this._ShowUnit = value;
            }
        }

        public int StartTabIndex
        {
            set
            {
                this._StartTabIndex = value;
            }
        }

        public string Street
        {
            get
            {
                return this.txtStreet.Text;
            }
            set
            {
                this._Street = value;
            }
        }

        public string Telephone
        {
            get
            {
                return this.txtTelephone.Text;
            }
            set
            {
                this._Telephone = value;
            }
        }

        public string Unit
        {
            get
            {
                return this.txtUnit.Text;
            }
            set
            {
                this._Unit = value;
            }
        }

        public Address()
        {
            Load += new EventHandler( this.Page_Load );
            Init += new EventHandler(Address_Init);
            this._LabelColumnWidth = "";
            this._ControlColumnWidth = "";
            this._StartTabIndex = 1;
            this._ShowStreet = true;
            this._ShowUnit = true;
            this._ShowCity = true;
            this._ShowCountry = true;
            this._ShowRegion = true;
            this._ShowPostal = true;
            this._ShowTelephone = true;
            this._ShowCell = true;
            this._ShowFax = true;
            this._CountryData = "Text";
            this._RegionData = "Text";
            this.MyFileName = "Address.ascx";


        }

        void Address_Init(object sender, EventArgs e)
        {
            this.cboCountry.SelectedIndexChanged += new EventHandler(this.cboCountry_SelectedIndexChanged);
            this.chkCell.CheckedChanged += new EventHandler(this.chkCell_CheckedChanged);
            this.chkCity.CheckedChanged += new EventHandler(this.chkCity_CheckedChanged);
            this.chkCountry.CheckedChanged += new EventHandler(this.chkCountry_CheckedChanged);

            this.chkFax.CheckedChanged += new EventHandler(this.chkFax_CheckedChanged);
            this.chkPostal.CheckedChanged += new EventHandler(this.chkPostal_CheckedChanged);
            this.chkRegion.CheckedChanged += new EventHandler(this.chkRegion_CheckedChanged);
            this.chkStreet.CheckedChanged += new EventHandler(this.chkStreet_CheckedChanged);
            this.chkTelephone.CheckedChanged += new EventHandler(this.chkTelephone_CheckedChanged);

        }

        protected void cboCountry_SelectedIndexChanged( object sender, EventArgs e )
        {
            try
            {
                Localize();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkCell_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkCity_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkCountry_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkFax_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkPostal_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkRegion_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkStreet_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkTelephone_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <Summary>
        /// Localize correctly sets up the control for US/Canada/Other Countries
        /// </Summary>
        private void Localize()
        {
            string countryCode = cboCountry.SelectedItem.Value;
            ListController ctlEntry = new ListController();
            // listKey in format "Country.US:Region"
            string listKey = "Country." + countryCode;
            ListEntryInfoCollection entryCollection = ctlEntry.GetListEntryInfoCollection("Region", "", listKey);

            if (entryCollection.Count != 0)
            {
                cboRegion.Visible = true;
                txtRegion.Visible = false;
                cboRegion.Items.Clear();
                cboRegion.DataSource = entryCollection;
                cboRegion.DataBind();
                cboRegion.Items.Insert(0, new ListItem("<" + Localization.GetString("Not_Specified", Localization.SharedResourceFile) + ">", ""));
                if (countryCode.ToLower() == "us")
                {
                    valRegion1.Enabled = true;
                    valRegion2.Enabled = false;
                    valRegion1.ErrorMessage = Localization.GetString("StateRequired", Localization.GetResourceFile(this, MyFileName));
                    plRegion.Text = Localization.GetString("plState", Localization.GetResourceFile(this, MyFileName));
                    plRegion.HelpText = Localization.GetString("plState.Help", Localization.GetResourceFile(this, MyFileName));
                    plPostal.Text = Localization.GetString("plZip", Localization.GetResourceFile(this, MyFileName));
                    plPostal.HelpText = Localization.GetString("plZip.Help", Localization.GetResourceFile(this, MyFileName));
                }
                else
                {
                    valRegion1.ErrorMessage = Localization.GetString("ProvinceRequired", Localization.GetResourceFile(this, MyFileName));
                    plRegion.Text = Localization.GetString("plProvince", Localization.GetResourceFile(this, MyFileName));
                    plRegion.HelpText = Localization.GetString("plProvince.Help", Localization.GetResourceFile(this, MyFileName));
                    plPostal.Text = Localization.GetString("plPostal", Localization.GetResourceFile(this, MyFileName));
                    plPostal.HelpText = Localization.GetString("plPostal.Help", Localization.GetResourceFile(this, MyFileName));
                }
                valRegion1.Enabled = true;
                valRegion2.Enabled = false;
            }
            else
            {
                cboRegion.ClearSelection();
                cboRegion.Visible = false;
                txtRegion.Visible = true;
                valRegion1.Enabled = false;
                valRegion2.Enabled = true;
                valRegion2.ErrorMessage = Localization.GetString("RegionRequired", Localization.GetResourceFile(this, MyFileName));
                plRegion.Text = Localization.GetString("plRegion", Localization.GetResourceFile(this, MyFileName));
                plRegion.HelpText = Localization.GetString("plRegion.Help", Localization.GetResourceFile(this, MyFileName));
                plPostal.Text = Localization.GetString("plPostal", Localization.GetResourceFile(this, MyFileName));
                plPostal.HelpText = Localization.GetString("plPostal.Help", Localization.GetResourceFile(this, MyFileName));
            }

            if (lblRegionRequired.Text == "")
            {
                valRegion1.Enabled = false;
                valRegion2.Enabled = false;
            }
        }

        /// <Summary>Page_Load runs when the control is loaded</Summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                valStreet.ErrorMessage = Localization.GetString("StreetRequired", Localization.GetResourceFile(this, MyFileName));
                valCity.ErrorMessage = Localization.GetString("CityRequired", Localization.GetResourceFile(this, MyFileName));
                valCountry.ErrorMessage = Localization.GetString("CountryRequired", Localization.GetResourceFile(this, MyFileName));
                valPostal.ErrorMessage = Localization.GetString("PostalRequired", Localization.GetResourceFile(this, MyFileName));
                valTelephone.ErrorMessage = Localization.GetString("TelephoneRequired", Localization.GetResourceFile(this, MyFileName));
                valCell.ErrorMessage = Localization.GetString("CellRequired", Localization.GetResourceFile(this, MyFileName));
                valFax.ErrorMessage = Localization.GetString("FaxRequired", Localization.GetResourceFile(this, MyFileName));

                if (!Page.IsPostBack)
                {
                    if (!String.IsNullOrEmpty(_LabelColumnWidth))
                    {
                        //lblCountry.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        //lblRegion.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        //lblCity.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        //lblStreet.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        //lblUnit.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        //lblPostal.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        //lblTelephone.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        //lblCell.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                        //lblFax.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth)
                    }

                    if (!String.IsNullOrEmpty(_ControlColumnWidth))
                    {
                        cboCountry.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        cboRegion.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtRegion.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtCity.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtStreet.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtUnit.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtPostal.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtTelephone.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtCell.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtFax.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                    }

                    txtStreet.TabIndex = Convert.ToInt16(_StartTabIndex);
                    txtUnit.TabIndex = Convert.ToInt16(_StartTabIndex + 1);
                    txtCity.TabIndex = Convert.ToInt16(_StartTabIndex + 2);
                    cboCountry.TabIndex = Convert.ToInt16(_StartTabIndex + 3);
                    cboRegion.TabIndex = Convert.ToInt16(_StartTabIndex + 4);
                    txtRegion.TabIndex = Convert.ToInt16(_StartTabIndex + 5);
                    txtPostal.TabIndex = Convert.ToInt16(_StartTabIndex + 6);
                    txtTelephone.TabIndex = Convert.ToInt16(_StartTabIndex + 7);
                    txtCell.TabIndex = Convert.ToInt16(_StartTabIndex + 8);
                    txtFax.TabIndex = Convert.ToInt16(_StartTabIndex + 9);

                    // <tam:note modified to test Lists
                    //Dim objRegionalController As New RegionalController
                    //cboCountry.DataSource = objRegionalController.GetCountries
                    // <this test using method 2: get empty collection then get each entry list on demand & store into cache

                    ListController ctlEntry = new ListController();
                    ListEntryInfoCollection entryCollection = ctlEntry.GetListEntryInfoCollection("Country");

                    cboCountry.DataSource = entryCollection;
                    cboCountry.DataBind();
                    cboCountry.Items.Insert(0, new ListItem("<" + Localization.GetString("Not_Specified", Localization.SharedResourceFile) + ">", ""));

                    switch (_CountryData.ToLower())
                    {
                        case "text":

                            if (_Country == "")
                            {
                                cboCountry.SelectedIndex = 0;
                            }
                            else
                            {
                                if (cboCountry.Items.FindByText(_Country) != null)
                                {
                                    cboCountry.ClearSelection();
                                    cboCountry.Items.FindByText(_Country).Selected = true;
                                }
                            }
                            break;
                        case "value":

                            if (cboCountry.Items.FindByValue(_Country) != null)
                            {
                                cboCountry.ClearSelection();
                                cboCountry.Items.FindByValue(_Country).Selected = true;
                            }
                            break;
                    }

                    Localize();

                    if (cboRegion.Visible)
                    {
                        switch (_RegionData.ToLower())
                        {
                            case "text":

                                if (_Region == "")
                                {
                                    cboRegion.SelectedIndex = 0;
                                }
                                else
                                {
                                    if (cboRegion.Items.FindByText(_Region) != null)
                                    {
                                        cboRegion.Items.FindByText(_Region).Selected = true;
                                    }
                                }
                                break;
                            case "value":

                                if (cboRegion.Items.FindByValue(_Region) != null)
                                {
                                    cboRegion.Items.FindByValue(_Region).Selected = true;
                                }
                                break;
                        }
                    }
                    else
                    {
                        txtRegion.Text = _Region;
                    }

                    txtStreet.Text = _Street;
                    txtUnit.Text = _Unit;
                    txtCity.Text = _City;
                    txtPostal.Text = _Postal;
                    txtTelephone.Text = _Telephone;
                    txtCell.Text = _Cell;
                    txtFax.Text = _Fax;

                    rowStreet.Visible = _ShowStreet;
                    rowUnit.Visible = _ShowUnit;
                    rowCity.Visible = _ShowCity;
                    rowCountry.Visible = _ShowCountry;
                    rowRegion.Visible = _ShowRegion;
                    rowPostal.Visible = _ShowPostal;
                    rowTelephone.Visible = _ShowTelephone;
                    rowCell.Visible = _ShowCell;
                    rowFax.Visible = _ShowFax;

                    if (PortalSecurity.IsInRoles(PortalSettings.AdministratorRoleName))
                    {
                        chkStreet.Visible = true;
                        chkCity.Visible = true;
                        chkCountry.Visible = true;
                        chkRegion.Visible = true;
                        chkPostal.Visible = true;
                        chkTelephone.Visible = true;
                        chkCell.Visible = true;
                        chkFax.Visible = true;
                    }

                    ViewState["ModuleId"] = Convert.ToString(_ModuleId);
                    ViewState["LabelColumnWidth"] = _LabelColumnWidth;
                    ViewState["ControlColumnWidth"] = _ControlColumnWidth;

                    ShowRequiredFields();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <Summary>
        /// ShowRequiredFields sets up displaying which fields are required
        /// </Summary>
        private void ShowRequiredFields()
        {
            Hashtable settings = PortalSettings.GetSiteSettings(PortalSettings.PortalId);

            lblStreetRequired.Text = ((Convert.ToString(settings["addressstreet"]) == "N") ? "" : "*").ToString();
            lblCityRequired.Text = ((Convert.ToString(settings["addresscity"]) == "N") ? "" : "*").ToString();
            lblCountryRequired.Text = ((Convert.ToString(settings["addresscountry"]) == "N") ? "" : "*").ToString();
            lblRegionRequired.Text = ((Convert.ToString(settings["addressregion"]) == "N") ? "" : "*").ToString();
            lblPostalRequired.Text = ((Convert.ToString(settings["addresspostal"]) == "N") ? "" : "*").ToString();
            lblTelephoneRequired.Text = ((Convert.ToString(settings["addresstelephone"]) == "N") ? "" : "*").ToString();
            lblCellRequired.Text = ((Convert.ToString(settings["addresscell"]) == "N") ? "" : "*").ToString();
            lblFaxRequired.Text = ((Convert.ToString(settings["addressfax"]) == "N") ? "" : "*").ToString();

            if (PortalSecurity.IsInRoles(PortalSettings.AdministratorRoleName))
            {
                if (lblCountryRequired.Text == "*")
                {
                    chkCountry.Checked = true;
                    valCountry.Enabled = true;
                }
                if (lblRegionRequired.Text == "*")
                {
                    chkRegion.Checked = true;
                    if (cboRegion.Visible == true)
                    {
                        valRegion1.Enabled = true;
                        valRegion2.Enabled = false;
                    }
                    else
                    {
                        valRegion1.Enabled = false;
                        valRegion2.Enabled = true;
                    }
                }
                if (lblCityRequired.Text == "*")
                {
                    chkCity.Checked = true;
                    valCity.Enabled = true;
                }
                if (lblStreetRequired.Text == "*")
                {
                    chkStreet.Checked = true;
                    valStreet.Enabled = true;
                }
                if (lblPostalRequired.Text == "*")
                {
                    chkPostal.Checked = true;
                    valPostal.Enabled = true;
                }
                if (lblTelephoneRequired.Text == "*")
                {
                    chkTelephone.Checked = true;
                    valTelephone.Enabled = true;
                }
                if (lblCellRequired.Text == "*")
                {
                    chkCell.Checked = true;
                    valCell.Enabled = true;
                }
                if (lblFaxRequired.Text == "*")
                {
                    chkFax.Checked = true;
                    valFax.Enabled = true;
                }
            }

            if (lblCountryRequired.Text == "")
            {
                valCountry.Enabled = false;
            }
            if (lblRegionRequired.Text == "")
            {
                valRegion1.Enabled = false;
                valRegion2.Enabled = false;
            }
            if (lblCityRequired.Text == "")
            {
                valCity.Enabled = false;
            }
            if (lblStreetRequired.Text == "")
            {
                valStreet.Enabled = false;
            }
            if (lblPostalRequired.Text == "")
            {
                valPostal.Enabled = false;
            }
            if (lblTelephoneRequired.Text == "")
            {
                valTelephone.Enabled = false;
            }
            if (lblCellRequired.Text == "")
            {
                valCell.Enabled = false;
            }
            if (lblFaxRequired.Text == "")
            {
                valFax.Enabled = false;
            }
        }

        /// <Summary>UpdateRequiredFields updates which fields are required</Summary>
        private void UpdateRequiredFields()
        {
            if (chkCountry.Checked == false)
            {
                chkRegion.Checked = false;
            }

            PortalSettings.UpdatePortalSetting(PortalSettings.PortalId, "addressstreet", (chkStreet.Checked ? "" : "N").ToString());
            PortalSettings.UpdatePortalSetting(PortalSettings.PortalId, "addresscity", (chkCity.Checked ? "" : "N").ToString());
            PortalSettings.UpdatePortalSetting(PortalSettings.PortalId, "addresscountry", (chkCountry.Checked ? "" : "N").ToString());
            PortalSettings.UpdatePortalSetting(PortalSettings.PortalId, "addressregion", (chkRegion.Checked ? "" : "N").ToString());
            PortalSettings.UpdatePortalSetting(PortalSettings.PortalId, "addresspostal", (chkPostal.Checked ? "" : "N").ToString());
            PortalSettings.UpdatePortalSetting(PortalSettings.PortalId, "addresstelephone", (chkTelephone.Checked ? "" : "N").ToString());
            PortalSettings.UpdatePortalSetting(PortalSettings.PortalId, "addresscell", (chkCell.Checked ? "" : "N").ToString());
            PortalSettings.UpdatePortalSetting(PortalSettings.PortalId, "addressfax", (chkFax.Checked ? "" : "N").ToString());

            ShowRequiredFields();
        }
    }
}