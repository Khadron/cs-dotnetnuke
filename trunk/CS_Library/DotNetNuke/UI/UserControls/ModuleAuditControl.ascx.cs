using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.UserControls
{

    public abstract class ModuleAuditControl : UserControl
    {
        private string _CreatedByUser;
        private string _CreatedDate;
        protected Label lblCreatedBy;
        private string MyFileName = "ModuleAuditControl.ascx";

        public ModuleAuditControl()
        {
            base.Load += new EventHandler( this.Page_Load );
            this._CreatedDate = "";
            this._CreatedByUser = "";
            
        }

        public string CreatedByUser
        {
            set
            {
                this._CreatedByUser = value;
            }
        }

        public string CreatedDate
        {
            set
            {
                this._CreatedDate = value;
            }
        }

        private void Page_Load( object sender, EventArgs e )
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    //if (Information.IsNumeric(_CreatedByUser))
                    Int32.Parse( _CreatedByUser );
                    {
                        // contains a UserID
                        UserController objUsers = new UserController();
                        UserInfo objUser = UserController.GetUser(PortalController.GetCurrentPortalSettings().PortalId, int.Parse(_CreatedByUser), false);
                        if (objUser != null)
                        {
                            _CreatedByUser = objUser.DisplayName;
                        }
                    }

                    string str = Localization.GetString("UpdatedBy", Localization.GetResourceFile(this, MyFileName));
                    lblCreatedBy.Text = string.Format(str, _CreatedByUser, _CreatedDate);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}