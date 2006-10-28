using System;
using System.Collections;
using System.Diagnostics;
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