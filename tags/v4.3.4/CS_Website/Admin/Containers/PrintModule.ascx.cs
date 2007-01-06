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
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.UI.Containers
{
    /// <summary>
    /// Contains the attributes of an Icon.
    /// These are read into the PortalModuleBase collection as attributes for the icons within the module controls.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[sun1]	        2/1/2004	Created
    /// 	[Nik Kalyani]	10/15/2004	Replaced public members with properties and removed
    ///                                 brackets from property names
    /// </history>
    public partial class PrintModule : ActionBase
    {
        // private members
        private string _printIcon;

        public string PrintIcon
        {
            get
            {
                return _printIcon;
            }
            set
            {
                _printIcon = value;
            }
        }

        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                UserInfo _UserInfo = UserController.GetCurrentUserInfo();

                foreach( ModuleAction action in this.MenuActions )
                {
                    if( action.CommandName == ModuleActionType.PrintModule )
                    {
                        if( action.Visible && PortalSecurity.HasNecessaryPermission( action.Secure, PortalSettings, ModuleConfiguration, _UserInfo.UserID.ToString() ) )
                        {
                            bool blnPreview = false;
                            if( Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId] != null )
                            {
                                blnPreview = bool.Parse( Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId].Value );
                            }
                            if( blnPreview == false || ( action.Secure == SecurityAccessLevel.Anonymous || action.Secure == SecurityAccessLevel.View ) )
                            {
                                if( PortalModule.ModuleConfiguration.DisplayPrint )
                                {
                                    ImageButton ModuleActionIcon = new ImageButton();
                                    if( !String.IsNullOrEmpty(PrintIcon) )
                                    {
                                        ModuleActionIcon.ImageUrl = PortalModule.ModuleConfiguration.ContainerPath.Substring( 0, ModuleConfiguration.ContainerPath.LastIndexOf( "/" ) + 1 ) + PrintIcon;
                                    }
                                    else
                                    {
                                        ModuleActionIcon.ImageUrl = "~/images/" + action.Icon;
                                    }
                                    ModuleActionIcon.ToolTip = action.Title;
                                    ModuleActionIcon.ID = "ico" + action.ID;
                                    ModuleActionIcon.CausesValidation = false;

                                    ModuleActionIcon.Click += new ImageClickEventHandler( IconAction_Click );

                                    this.Controls.Add( ModuleActionIcon );
                                }
                            }
                        }
                    }
                }

                // set visibility
                if( this.Controls.Count > 0 )
                {
                    this.Visible = true;
                }
                else
                {
                    this.Visible = false;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void IconAction_Click( object sender, ImageClickEventArgs e )
        {
            try
            {
                ProcessAction( ( (ImageButton)sender ).ID.Substring( 3 ) );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}