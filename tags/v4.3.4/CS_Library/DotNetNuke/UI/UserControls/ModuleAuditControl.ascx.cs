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
            Load += new EventHandler( this.Page_Load );
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

        protected void Page_Load(object sender, EventArgs e)
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