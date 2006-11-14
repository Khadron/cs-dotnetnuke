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
using System.Text;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Admin.Portals
{
    /// <summary>
    /// The Portals PortalModuleBase is used to manage the portlas.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class Portals : PortalModuleBase, IActionable
    {
        /// <summary>
        /// BindData fetches the data from the database and updates the controls
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void BindData()
        {
            PortalController objPortalController = new PortalController();

            //Localize the Grid
            Localization.LocalizeDataGrid( ref grdPortals, this.LocalResourceFile );

            grdPortals.DataSource = objPortalController.GetPortals();
            grdPortals.DataBind();
        }

        /// <summary>
        /// FormatExpiryDate formats the expiry date and filter out null-dates
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatExpiryDate( DateTime DateTime )
        {
            string returnValue=String.Empty;
            try
            {
                if( ! Null.IsNull( DateTime ) )
                {
                    returnValue = DateTime.ToShortDateString();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return returnValue;
        }

        /// <summary>
        /// FormatExpiryDate formats the format name as an <a> tag
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatPortalAliases( int PortalID )
        {
            try
            {
                PortalAliasController objPortalAliasController = new PortalAliasController();
                ArrayList arr = objPortalAliasController.GetPortalAliasArrayByPortalID( PortalID );
                PortalAliasInfo objPortalAliasInfo;
                StringBuilder str = new StringBuilder();
                int i;
                for( i = 0; i <= arr.Count - 1; i++ )
                {
                    objPortalAliasInfo = (PortalAliasInfo)arr[i];
                    str.Append("<a href=\"" + Globals.AddHTTP(objPortalAliasInfo.HTTPAlias) + "\">" + objPortalAliasInfo.HTTPAlias + "</a>" + "<BR>");
                }
                return str.ToString();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return null;
        }

        public string GetEditURL( int PortalID )
        {
            TabController objTabs = new TabController();
            TabInfo objTab = objTabs.GetTabByName( "Site Settings", PortalSettings.PortalId, PortalSettings.AdminTabId );
            return Globals.NavigateURL( objTab.TabID, "", "pid=" + PortalID.ToString() );
        }

        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        ///     [VMasanas]  9/28/2004   Changed redirect to Access Denied
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                // Verify that the current user has access to access this page
                if( ! UserInfo.IsSuperUser )
                {
                    Response.Redirect( Globals.NavigateURL( "Access Denied" ), true );
                }

                BindData();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl( "Signup" ), false, SecurityAccessLevel.Host, true, false );
                return actions;
            }
        }

        

    }
}