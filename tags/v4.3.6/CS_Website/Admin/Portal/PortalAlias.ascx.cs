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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Admin.Portals
{
    public partial class PortalAlias : PortalModuleBase, IActionable
    {
        private int intPortalID;

        private void BindData()
        {
            ArrayList arr;
            PortalAliasController p = new PortalAliasController();

            arr = p.GetPortalAliasArrayByPortalID( intPortalID );
            dgPortalAlias.DataSource = arr;
            dgPortalAlias.DataBind();
        }

        public bool IsNotCurrent( string Id )
        {
            int portalAliasId = int.Parse( Id );
            if( portalAliasId == this.PortalAlias.PortalAliasID )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                Localization.LocalizeDataGrid(ref dgPortalAlias, this.LocalResourceFile );
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
                if( ( Request.QueryString["pid"] != null ) && ( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId || UserInfo.IsSuperUser ) )
                {
                    intPortalID = int.Parse( Request.QueryString["pid"] );
                }
                else
                {
                    intPortalID = PortalId;
                }

                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl( "pid", intPortalID.ToString(), "Edit" ), false, SecurityAccessLevel.Admin, true, false );
                return actions;
            }
        }



    }
}