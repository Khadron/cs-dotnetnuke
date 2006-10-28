using System;
using System.Data;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Admin.SQL
{
    /// <summary>
    /// The SQL PortalModuleBase is used run SQL Scripts on the Database
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class SQL : PortalModuleBase
    {
        protected Label lblRunAsScript;

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
                if( ! Page.IsPostBack )
                {
                    cmdExecute.ToolTip = Localization.GetString( "cmdExecute.ToolTip", this.LocalResourceFile );
                    chkRunAsScript.ToolTip = Localization.GetString( "chkRunAsScript.ToolTip", this.LocalResourceFile );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdExecute_Click runs when the Execute button is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdExecute_Click( object sender, EventArgs e )
        {
            try
            {
                if( txtQuery.Text != "" )
                {
                    if( chkRunAsScript.Checked )
                    {
                        lblMessage.Text = PortalSettings.ExecuteScript( txtQuery.Text );
                    }
                    else
                    {
                        IDataReader dr = DataProvider.Instance().ExecuteSQL( txtQuery.Text );
                        if( dr != null )
                        {
                            grdResults.DataSource = dr;
                            grdResults.DataBind();
                            dr.Close();
                        }
                        else
                        {
                            lblMessage.Text = Localization.GetString( "QueryError", this.LocalResourceFile );
                        }
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
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