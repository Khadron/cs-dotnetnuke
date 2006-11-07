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
using System.Diagnostics;
using System.Web.Security;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Common.Controls
{
    public partial class Logoff : PageBase
    {
        /// <summary>
        /// Gets the Redirect URL after the user logs out
        /// </summary>
        /// <history>
        /// </history>
        protected string RedirectURL
        {
            get
            {
                string _RedirectURL;

                // check if anonymous users have access to the current page
                if( PortalSettings.ActiveTab.AuthorizedRoles.IndexOf( ";" + Globals.glbRoleAllUsersName + ";" ) != - 1 )
                {
                    // redirect to current page
                    _RedirectURL = Globals.NavigateURL( PortalSettings.ActiveTab.TabID );
                }
                else // redirect to a different page
                {
                    object setting = UserModuleBase.GetSetting( PortalSettings.PortalId, "Redirect_AfterLogout" );

                    if( Convert.ToInt32( setting ) == Null.NullInteger )
                    {
                        if( PortalSettings.HomeTabId != - 1 )
                        {
                            // redirect to portal home page specified
                            _RedirectURL = Globals.NavigateURL( PortalSettings.HomeTabId );
                        }
                        else // redirect to default portal root
                        {
                            _RedirectURL = Globals.GetPortalDomainName(PortalSettings.PortalAlias.HTTPAlias, Request, true);
                        }
                    }
                    else // redirect to after logout page
                    {
                        _RedirectURL = Globals.NavigateURL( Convert.ToInt32( setting ) );
                    }
                }

                return _RedirectURL;
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/23/2006  Documented
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                // Log User Off from Cookie Authentication System
                FormsAuthentication.SignOut();

                //remove language cookie
                Response.Cookies["language"].Value = "";

                // expire cookies
                if( PortalSecurity.IsInRoles( PortalSettings.AdministratorRoleId.ToString() ) && Request.Cookies["_Tab_Admin_Content" + PortalSettings.PortalId.ToString()] != null )
                {
                    Response.Cookies["_Tab_Admin_Content" + PortalSettings.PortalId.ToString()].Value = null;
                    Response.Cookies["_Tab_Admin_Content" + PortalSettings.PortalId.ToString()].Path = "/";
                    Response.Cookies["_Tab_Admin_Content" + PortalSettings.PortalId.ToString()].Expires = DateTime.Now.AddYears( - 30 );
                }

                Response.Cookies["portalaliasid"].Value = null;
                Response.Cookies["portalaliasid"].Path = "/";
                Response.Cookies["portalaliasid"].Expires = DateTime.Now.AddYears( - 30 );

                Response.Cookies["portalroles"].Value = null;
                Response.Cookies["portalroles"].Path = "/";
                Response.Cookies["portalroles"].Expires = DateTime.Now.AddYears( - 30 );

                if( Request.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId.ToString()] != null )
                {
                    Response.Cookies["_Tab_Admin_Preview" + PortalSettings.PortalId.ToString()].Value = "False";
                }

                // Redirect browser back to portal
                Response.Redirect( RedirectURL, true );
            }
            catch( Exception exc ) //Page failed to load
            {
                Exceptions.ProcessPageLoadException( exc );
            }
        }

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
    }
}