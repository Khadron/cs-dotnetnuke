using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Vendors;
using DotNetNuke.UI.Utilities;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Vendors
{
    /// <summary>
    /// The Vendors PortalModuleBase is used to manage the Vendors of a portal
    /// </summary>
    /// <history>
    /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class Vendors : PortalModuleBase, IActionable
    {
        protected Label lblMessage;

        protected int CurrentPage = - 1;
        protected int TotalPages = - 1;

        private string strFilter;

        /// <summary>
        /// BindData gets the vendors from the Database and binds them to the DataGrid
        /// </summary>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void BindData()
        {
            BindData( null, null );
        }

        private void BindData( string SearchText, string SearchField )
        {
            CreateLetterSearch();

            //Localize the Headers
            Localization.LocalizeDataGrid( ref grdVendors, this.LocalResourceFile );

            if( SearchText == Localization.GetString( "All" ) )
            {
                strFilter = "";
            }
            else if( SearchText == Localization.GetString( "Unauthorized" ) )
            {
                strFilter = "";
            }
            else
            {
                strFilter = SearchText;
            }

            // Get the list of vendors from the database
            int PageSize = Convert.ToInt32( ddlRecordsPerPage.SelectedItem.Value );
            int TotalRecords = 0;
            VendorController objVendors = new VendorController();
            int Portal;
            if( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId )
            {
                Portal = Null.NullInteger;
            }
            else
            {
                Portal = PortalId;
            }

            if( strFilter == "" )
            {
                if( SearchText == Localization.GetString( "Unauthorized" ) )
                {
                    grdVendors.DataSource = objVendors.GetVendors( Portal, true, CurrentPage - 1, PageSize, ref TotalRecords );
                }
                else
                {
                    grdVendors.DataSource = objVendors.GetVendors( Portal, false, CurrentPage - 1, PageSize, ref TotalRecords );
                }
            }
            else
            {
                if( SearchField == "email" )
                {
                    grdVendors.DataSource = objVendors.GetVendorsByEmail( strFilter, Portal, CurrentPage - 1, PageSize, ref TotalRecords );
                }
                else
                {
                    grdVendors.DataSource = objVendors.GetVendorsByName( strFilter, Portal, CurrentPage - 1, PageSize, ref TotalRecords );
                }
            }

            grdVendors.DataBind();

            ctlPagingControl.TotalRecords = TotalRecords;
            ctlPagingControl.PageSize = PageSize;
            ctlPagingControl.CurrentPage = CurrentPage;
            string strQuerystring = String.Empty;
            if( ddlRecordsPerPage.SelectedIndex != 0 )
            {
                strQuerystring = "PageRecords=" + ddlRecordsPerPage.SelectedValue;
            }
            if( strFilter != "" )
            {
                strQuerystring += "&filter=" + strFilter;
            }
            ctlPagingControl.QuerystringParams = strQuerystring;
            ctlPagingControl.TabID = TabId;
        }

        private void CreateLetterSearch()
        {
            string[] strAlphabet = new string[] {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", Localization.GetString( "All" ), Localization.GetString( "Unauthorized" )};
            rptLetterSearch.DataSource = strAlphabet;
            rptLetterSearch.DataBind();
        }

        /// <summary>
        /// DisplayAddress correctly formats an Address
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string DisplayAddress( object Unit, object Street, object City, object Region, object Country, object PostalCode )
        {
            string returnValue = String.Empty;
            try
            {
                returnValue = Globals.FormatAddress( Unit, Street, City, Region, Country, PostalCode );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return returnValue;
        }

        /// <summary>
        /// DisplayEmail correctly formats an Email Address
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string DisplayEmail( string Email )
        {
            string returnValue = String.Empty;
            try
            {
                returnValue = HtmlUtils.FormatEmail( Email );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return returnValue;
        }

        /// <summary>
        /// FormatURL correctly formats the Url for the Edit Vendor Link
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatURL( string strKeyName, string strKeyValue )
        {
            string returnValue = String.Empty;
            try
            {
                if( strFilter != "" )
                {
                    returnValue = EditUrl( strKeyName, strKeyValue, "", "filter=" + strFilter );
                }
                else
                {
                    returnValue = EditUrl( strKeyName, strKeyValue );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return returnValue;
        }

        /// <summary>
        /// FilterURL correctly formats the Url for filter by first letter and paging
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected string FilterURL( string filter, string currentPage )
        {
            try
            {
                if( filter != "" )
                {
                    if( currentPage != "" )
                    {
                        return Globals.NavigateURL( TabId, "", "filter=" + filter, "currentpage=" + currentPage, "PageRecords=" + ddlRecordsPerPage.SelectedValue );
                    }
                    else
                    {
                        return Globals.NavigateURL( TabId, "", "filter=" + filter, "PageRecords=" + ddlRecordsPerPage.SelectedValue );
                    }
                }
                else
                {
                    if( currentPage != "" )
                    {
                        return Globals.NavigateURL( TabId, "", "currentpage=" + currentPage, "PageRecords=" + ddlRecordsPerPage.SelectedValue );
                    }
                    else
                    {
                        return Globals.NavigateURL( TabId, "", "PageRecords=" + ddlRecordsPerPage.SelectedValue );
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }

            return String.Empty;
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
                if( Request.QueryString["CurrentPage"] != null )
                {
                    CurrentPage = Convert.ToInt32( Request.QueryString["CurrentPage"] );
                }
                else
                {
                    CurrentPage = 1;
                }

                if( Request.QueryString["filter"] != null )
                {
                    strFilter = Request.QueryString["filter"];
                }
                else
                {
                    strFilter = "";
                }

                if( ! Page.IsPostBack )
                {
                    ClientAPI.AddButtonConfirm( cmdDelete, Localization.GetString( "DeleteItems" ) );

                    if( Request.QueryString["PageRecords"] != null )
                    {
                        ddlRecordsPerPage.SelectedValue = Request.QueryString["PageRecords"];
                    }

                    BindData( strFilter, "username" );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// grdVendors_ItemCommand runs when a command button in the grid is clicked.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void grdVendors_ItemCommand( object source, DataGridCommandEventArgs e )
        {
            try
            {
                if( e.CommandName == "filter" )
                {
                    strFilter = e.CommandArgument.ToString();
                    CurrentPage = 1;
                    txtSearch.Text = "";
                    BindData( strFilter, "username" );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdDelete_Click runs when the Delete Unauthorized Vendors button is clicked.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdDelete_Click( object sender, EventArgs e )
        {
            try
            {
                VendorController objVendors = new VendorController();
                if( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId )
                {
                    objVendors.DeleteVendors();
                }
                else
                {
                    objVendors.DeleteVendors( PortalId );
                }
                BindData();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// ddlRecordsPerPage_SelectedIndexChanged runs when the user selects a new
        /// Records Per Page value from the dropdown.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[dancaron]	10/28/2004	Intial Version
        /// </history>
        protected void ddlRecordsPerPage_SelectedIndexChanged( Object sender, EventArgs e )
        {
            CurrentPage = 1;
            BindData();
        }

        /// <summary>
        /// btnSearch_Click runs when the user searches for accounts by username or email
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[dancaron]	10/28/2004	Intial Version
        /// </history>
        protected void btnSearch_Click( Object sender, ImageClickEventArgs e )
        {
            CurrentPage = 1;
            BindData( txtSearch.Text, ddlSearchType.SelectedItem.Value );
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection Actions = new ModuleActionCollection();
                Actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl(), false, SecurityAccessLevel.Admin, true, false );
                return Actions;
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