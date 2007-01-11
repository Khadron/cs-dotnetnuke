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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.UI.Containers
{
    public partial class LinkActions : ActionBase
    {
        protected string _itemSeparator = "";


        protected void Page_PreRender( object sender, EventArgs e )
        {
            //Put user code to initialize the page here
            try
            {
                if( _itemSeparator.Length == 0 )
                {
                    _itemSeparator = "&nbsp;&nbsp;";
                }

                BindLinkList();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        private void BindLinkList()
        {
            object with_1 = m_menuActionRoot;
            // Is Root Menu visible?
            if( ( (ModuleAction)with_1 ).Visible )
            {
                if( this.Controls.Count > 0 )
                {
                    this.Controls.Clear();
                }

                LiteralControl PreSpacer = new LiteralControl( ItemSeparator );
                this.Controls.Add( PreSpacer );

                // Add Menu Items
                foreach( ModuleAction action in m_menuActionRoot.Actions )
                {
                    if( action.Title == "~" )
                    {
                        // not supported in this Action object
                    }
                    else
                    {
                        UserInfo userInfo = UserController.GetCurrentUserInfo();
                        if( action.Visible && PortalSecurity.HasNecessaryPermission( action.Secure, ( (PortalSettings)HttpContext.Current.Items["PortalSettings"] ), ModuleConfiguration, userInfo.UserID.ToString() ) )
                        {
                            if( ( EditMode && PortalModule.PortalSettings.ActiveTab.IsAdminTab == false && Globals.IsAdminControl() == false ) || ( action.Secure != SecurityAccessLevel.Anonymous && action.Secure != SecurityAccessLevel.View ) )
                            {
                                LinkButton ModuleActionLink = new LinkButton();
                                ModuleActionLink.Text = action.Title;
                                ModuleActionLink.CssClass = "CommandButton";
                                ModuleActionLink.ID = "lnk" + action.ID;

                                ModuleActionLink.Click += new EventHandler( LinkAction_Click );

                                this.Controls.Add( ModuleActionLink );
                                LiteralControl Spacer = new LiteralControl( ItemSeparator );
                                this.Controls.Add( Spacer );
                            }
                        }
                    }
                }
            }

            //Need to determine if this action list actually has any items.
            if( this.Controls.Count > 0 )
            {
                this.Visible = true;
            }
            else
            {
                this.Visible = false;
            }
        }

        protected void LinkAction_Click( object sender, EventArgs e )
        {
            try
            {
                ProcessAction( ( (LinkButton)sender ).ID.Substring( 3 ) );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        public string ItemSeparator
        {
            get
            {
                return _itemSeparator;
            }
            set
            {
                _itemSeparator = value;
            }
        }
    }
}