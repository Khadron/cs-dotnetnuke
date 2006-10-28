using System;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Vendors;

namespace DotNetNuke.Modules.Admin.Vendors
{
    /// <summary>
    /// The Affiliates PortalModuleBase is used to manage a Vendor's Affiliates
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class Affiliates : PortalModuleBase, IActionable
    {
        public int VendorID;

        /// <summary>
        /// BindData gets the affiliates from the Database and binds them to the DataGrid
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void BindData()
        {
            AffiliateController objAffiliates = new AffiliateController();

            //Localize the Grid
            Localization.LocalizeDataGrid( ref grdAffiliates, this.LocalResourceFile );

            grdAffiliates.DataSource = objAffiliates.GetAffiliates( VendorID );
            grdAffiliates.DataBind();

            cmdAdd.NavigateUrl = FormatURL( "AffilId", "-1" );
        }

        /// <summary>
        /// DisplayDate formats a Date
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <paam name="DateValue">The Date to format</param>
        /// <returns>The correctly formatted date</returns>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string DisplayDate( DateTime DateValue )
        {
            try
            {
                if( Null.IsNull( DateValue ) )
                {
                    return "";
                }
                else
                {
                    return DateValue.ToShortDateString();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return String.Empty;
        }

        /// <summary>
        /// FormatURL correctly formats the Url (adding a key/Value pair)
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <paam name="strKeyName">The name of the key to add</param>
        /// <paam name="strKeyValue">The value to add</param>
        /// <returns>The correctly formatted url</returns>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatURL( string strKeyName, string strKeyValue )
        {
            return EditUrl( strKeyName, strKeyValue, "Affiliate", "VendorId=" + VendorID.ToString() );
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ! Null.IsNull( VendorID ) )
                {
                    BindData();
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

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                if( ( Request.QueryString["VendorID"] != null ) )
                {
                    VendorID = int.Parse( Request.QueryString["VendorID"] );
                }
                else
                {
                    VendorID = Null.NullInteger;
                }
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl( "VendorId", VendorID.ToString(), "Affiliate" ), false, SecurityAccessLevel.Admin, Null.IsNull( VendorID ) == false, false );
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