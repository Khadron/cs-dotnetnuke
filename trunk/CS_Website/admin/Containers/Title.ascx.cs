using System;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.UI.Containers
{
    /// <summary></summary>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class Title : SkinObjectBase
    {
        // private members
        private string _cssClass;

        public string CssClass
        {
            get
            {
                return _cssClass;
            }
            set
            {
                _cssClass = value;
            }
        }

        private bool CanEditModule()
        {
            bool blnCanEdit = false;
            PortalModuleBase objModule = Container.GetPortalModuleBase( this );
            if( ( objModule != null ) && ( objModule.ModuleId > Null.NullInteger ) )
            {
                blnCanEdit = ( PortalSecurity.IsInRoles( PortalSettings.AdministratorRoleName ) || PortalSecurity.IsInRoles( PortalSettings.ActiveTab.AdministratorRoles.ToString() ) ) && Globals.IsAdminControl() == false && PortalSettings.ActiveTab.IsAdminTab == false;
            }
            return blnCanEdit;
        }

        /// <summary>
        /// Assign the CssClass and Text Attributes for the Title label.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[sun1]	2/1/2004	Created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                // public attributes
                if( CssClass != "" )
                {
                    lblTitle.CssClass = CssClass;
                }

                PortalModuleBase objPortalModule = Container.GetPortalModuleBase( this );

                string strTitle = Null.NullString;
                if( objPortalModule != null )
                {
                    strTitle = objPortalModule.ModuleConfiguration.ModuleTitle;
                }
                if( strTitle == Null.NullString )
                {
                    strTitle = "&nbsp;";
                }
                lblTitle.Text = strTitle;
                if( CanEditModule() == false )
                {
                    lblTitle.EditEnabled = false;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void lblTitle_UpdateLabel( object source, DNNLabelEditEventArgs e )
        {
            if( CanEditModule() )
            {
                ModuleController objModule = new ModuleController();
                PortalModuleBase objPortalModule = Container.GetPortalModuleBase( this );
                ModuleInfo objModInfo = objModule.GetModule( objPortalModule.ModuleId, objPortalModule.TabId );

                objModInfo.ModuleTitle = e.Text;
                objModule.UpdateModule( objModInfo );
            }
        }
    }
}