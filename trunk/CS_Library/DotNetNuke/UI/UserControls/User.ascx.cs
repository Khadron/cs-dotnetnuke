using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.UserControls
{
    /// <Summary>The User UserControl is used to manage User Details</Summary>
    public abstract class User : UserControlBase
    {
        private string _Confirm;
        private string _ControlColumnWidth = "";
        private string _Email;
        private string _FirstName;
        private string _IM;
        private string _LabelColumnWidth = "";
        private string _LastName;

        private int _ModuleId;
        private string _Password;
        private bool _ShowPassword;
        private int _StartTabIndex = 1;
        private string _UserName;
        private string _Website;
        protected HtmlTableRow ConfirmPasswordRow;
        protected Label lblUsername;
        protected Label lblUsernameAsterisk;

        private string MyFileName = "User.ascx";

        protected HtmlTableRow PasswordRow;
        protected LabelControl plConfirm;
        protected LabelControl plEmail;
        protected LabelControl plFirstName;
        protected LabelControl plIM;
        protected LabelControl plLastName;
        protected LabelControl plPassword;
        protected LabelControl plUserName;
        protected LabelControl plWebsite;
        protected TextBox txtConfirm;
        protected TextBox txtEmail;
        protected TextBox txtFirstName;
        protected TextBox txtIM;
        protected TextBox txtLastName;
        protected TextBox txtPassword;
        protected TextBox txtUsername;
        protected TextBox txtWebsite;
        protected RequiredFieldValidator valConfirm1;
        protected CompareValidator valConfirm2;
        protected RequiredFieldValidator valEmail1;
        protected RegularExpressionValidator valEmail2;
        protected RequiredFieldValidator valFirstName;
        protected RequiredFieldValidator valLastName;
        protected RequiredFieldValidator valPassword;
        protected RequiredFieldValidator valUsername;

        public string Confirm
        {
            get
            {
                return this.txtConfirm.Text;
            }
            set
            {
                this._Confirm = value;
            }
        }

        public string ControlColumnWidth
        {
            get
            {
                return Convert.ToString( this.ViewState["ControlColumnWidth"] );
            }
            set
            {
                this._ControlColumnWidth = value;
            }
        }

        public string Email
        {
            get
            {
                return this.txtEmail.Text;
            }
            set
            {
                this._Email = value;
            }
        }

        public string FirstName
        {
            get
            {
                return this.txtFirstName.Text;
            }
            set
            {
                this._FirstName = value;
            }
        }

        public string IM
        {
            get
            {
                return this.txtIM.Text;
            }
            set
            {
                this._IM = value;
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

        public string LastName
        {
            get
            {
                return this.txtLastName.Text;
            }
            set
            {
                this._LastName = value;
            }
        }

        public string LocalResourceFile
        {
            get
            {
                return Localization.GetResourceFile( ( (Control)this ), this.MyFileName );
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

        public string Password
        {
            get
            {
                return this.txtPassword.Text;
            }
            set
            {
                this._Password = value;
            }
        }

        public bool ShowPassword
        {
            set
            {
                this._ShowPassword = value;
            }
        }

        public int StartTabIndex
        {
            set
            {
                this._StartTabIndex = value;
            }
        }

        public string Website
        {
            get
            {
                return this.txtWebsite.Text;
            }
            set
            {
                this._Website = value;
            }
        }

        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
            }
        }

        public User()
        {
            Load += new EventHandler( this.Page_Load );
            this._LabelColumnWidth = "";
            this._ControlColumnWidth = "";
            this._StartTabIndex = 1;
            this.MyFileName = "User.ascx";
        }

        /// <Summary>Page_Load runs when the control is loaded</Summary>
        protected void Page_Load( object sender, EventArgs e )
        {
            try
            {
                if( Page.IsPostBack == false )
                {
                    txtFirstName.TabIndex = Convert.ToInt16( _StartTabIndex );
                    txtLastName.TabIndex = Convert.ToInt16( _StartTabIndex + 1 );
                    txtUsername.TabIndex = Convert.ToInt16( _StartTabIndex + 2 );
                    txtPassword.TabIndex = Convert.ToInt16( _StartTabIndex + 3 );
                    txtConfirm.TabIndex = Convert.ToInt16( _StartTabIndex + 4 );
                    txtEmail.TabIndex = Convert.ToInt16( _StartTabIndex + 5 );
                    txtWebsite.TabIndex = Convert.ToInt16( _StartTabIndex + 6 );
                    txtIM.TabIndex = Convert.ToInt16( _StartTabIndex + 7 );

                    txtFirstName.Text = _FirstName;
                    txtLastName.Text = _LastName;
                    txtUsername.Text = UserName;
                    lblUsername.Text = UserName;
                    txtPassword.Text = _Password;
                    txtConfirm.Text = _Confirm;
                    txtEmail.Text = _Email;
                    txtWebsite.Text = _Website;
                    txtIM.Text = _IM;

                    if( _ControlColumnWidth != "" )
                    {
                        txtFirstName.Width = Unit.Parse( _ControlColumnWidth );
                        txtLastName.Width = Unit.Parse( _ControlColumnWidth );
                        txtUsername.Width = Unit.Parse( _ControlColumnWidth );
                        txtPassword.Width = Unit.Parse( _ControlColumnWidth );
                        txtConfirm.Width = Unit.Parse( _ControlColumnWidth );
                        txtEmail.Width = Unit.Parse( _ControlColumnWidth );
                        txtWebsite.Width = Unit.Parse( _ControlColumnWidth );
                        txtIM.Width = Unit.Parse( _ControlColumnWidth );
                    }

                    if( !_ShowPassword )
                    {
                        valPassword.Enabled = false;
                        valConfirm1.Enabled = false;
                        valConfirm2.Enabled = false;
                        PasswordRow.Visible = false;
                        ConfirmPasswordRow.Visible = false;
                        txtUsername.Visible = false;
                        valUsername.Enabled = false;
                        lblUsername.Visible = true;
                        lblUsernameAsterisk.Visible = false;
                    }
                    else
                    {
                        txtUsername.Visible = true;
                        valUsername.Enabled = true;
                        lblUsername.Visible = false;
                        lblUsernameAsterisk.Visible = true;
                        valPassword.Enabled = true;
                        valConfirm1.Enabled = true;
                        valConfirm2.Enabled = true;
                        PasswordRow.Visible = true;
                        ConfirmPasswordRow.Visible = true;
                    }

                    ViewState["ModuleId"] = Convert.ToString( _ModuleId );
                    ViewState["LabelColumnWidth"] = _LabelColumnWidth;
                    ViewState["ControlColumnWidth"] = _ControlColumnWidth;
                }

                txtPassword.Attributes.Add( "value", txtPassword.Text );
                txtConfirm.Attributes.Add( "value", txtConfirm.Text );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}