using System;
using System.Collections;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Admin.Security
{
    /// <summary>
    /// The MemberServices UserModuleBase is used to manage a User's services
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	03/03/2006
    /// </history>
    public partial class MemberServices : UserModuleBase
    {
        private int Services = 0;
        private int RoleID = - 1;

        /// <summary>
        /// DataBind binds the data to the controls
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/13/2006  Created
        /// </history>
        public override void DataBind()
        {
            //Localize the Headers
            Localization.LocalizeDataGrid( ref grdServices, this.LocalResourceFile );

            if( Services == 1 )
            {
                RoleController objRoles = new RoleController();
                grdServices.DataSource = objRoles.GetUserRoles( PortalId );
                grdServices.DataBind();

                if( grdServices.Items.Count != 0 )
                {
                    lblServices.Text = string.Format( Localization.GetString( "PleaseRegister", this.LocalResourceFile ), Globals.GetPortalDomainName( PortalAlias.HTTPAlias, Request, true ) + "/" + Globals.glbDefaultPage, TabId );
                }
                else
                {
                    grdServices.Visible = false;
                    lblServices.Text = Localization.GetString( "MembershipNotOffered", this.LocalResourceFile );
                }
                lblServices.Visible = true;

                grdServices.Columns[0].Visible = false; // subscribe
                grdServices.Columns[9].Visible = false; // expiry date
            }
            else
            {
                if( Request.IsAuthenticated )
                {
                    RoleController objRoles = new RoleController();

                    grdServices.DataSource = objRoles.GetUserRoles( PortalId, UserInfo.UserID, false );
                    grdServices.DataBind();

                    // if no service available then hide options
                    //ServicesRow.Visible = (grdServices.Items.Count > 0)
                }
            }
        }

        /// <summary>
        /// FormatURL correctly formats a URL
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<returns>The correctly formatted url</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatURL()
        {
            string _FormatURL = Null.NullString;
            try
            {
                string strServerPath;

                strServerPath = Request.ApplicationPath;
                if( ! strServerPath.EndsWith( "/" ) )
                {
                    strServerPath += "/";
                }

                _FormatURL = strServerPath + "Register.aspx?tabid=" + TabId;
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return _FormatURL;
        }

        /// <summary>
        /// FormatPrice formats the Fee amount and filters out null-values
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="price">The price to format</param>
        ///	<returns>The correctly formatted price</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatPrice( float price )
        {
            string _FormatPrice = Null.NullString;
            try
            {
                if( price != Null.NullSingle )
                {
                    _FormatPrice = price.ToString( "##0.00" );
                }
                else
                {
                    _FormatPrice = "";
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return _FormatPrice;
        }

        /// <summary>
        /// FormatPeriod formats the Period and filters out null-values
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="period">The period to format</param>
        ///	<returns>The correctly formatted period</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatPeriod( int period )
        {
            string _FormatPeriod = Null.NullString;
            try
            {
                if( period != Null.NullInteger )
                {
                    _FormatPeriod = period.ToString();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return _FormatPeriod;
        }

        /// <summary>
        /// FormatExpiryDate formats the expiry date and filters out null-values
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="DateTime">The date to format</param>
        ///	<returns>The correctly formatted date</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatExpiryDate( DateTime DateTime )
        {
            string _FormatExpiryDate = Null.NullString;
            try
            {
                if( ! Null.IsNull( DateTime ) )
                {
                    _FormatExpiryDate = DateTime.ToShortDateString();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return _FormatExpiryDate;
        }

        /// <summary>
        /// ServiceText gets the Service Text (Cancel or Subscribe)
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="Subscribed">The service state</param>
        ///	<returns>The correctly formatted text</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string ServiceText( bool Subscribed )
        {
            string _ServiceText = Null.NullString;
            try
            {
                if( ! Subscribed )
                {
                    _ServiceText = Localization.GetString( "Subscribe", this.LocalResourceFile );
                }
                else
                {
                    _ServiceText = Localization.GetString( "Unsubscribe", this.LocalResourceFile );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return _ServiceText;
        }

        /// <summary>
        /// ServiceURL correctly formats the Subscription url
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="strKeyName">The key name for the service</param>
        ///	<param name="strKeyValue">The key value for the service</param>
        ///	<param name="objServiceFee">The service fee</param>
        ///	<param name="Subscribed">The service state</param>
        ///	<returns>The correctly formatted url</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string ServiceURL( string strKeyName, string strKeyValue, object objServiceFee, bool Subscribed )
        {
            string _ServiceURL = Null.NullString;
            try
            {
                double dblServiceFee = 0;
                string ctlRegister;

                if( objServiceFee != DBNull.Value )
                {
                    dblServiceFee = Convert.ToDouble( objServiceFee );
                }

                if( PortalSettings.UserTabId != - 1 )
                {
                    // user defined tab
                    ctlRegister = "";
                }
                else
                {
                    // admin tab
                    ctlRegister = "Register";
                }

                if( ! Subscribed )
                {
                    if( dblServiceFee > 0 )
                    {
                        _ServiceURL = "~/admin/Sales/PayPalSubscription.aspx?tabid=" + TabId + "&" + strKeyName + "=" + strKeyValue;
                    }
                    else
                    {
                        _ServiceURL = Globals.NavigateURL( PortalSettings.UserTabId, ctlRegister, strKeyName + "=" + strKeyValue );
                    }
                }
                else // cancel
                {
                    if( dblServiceFee > 0 )
                    {
                        _ServiceURL = "~/admin/Sales/PayPalSubscription.aspx?tabid=" + TabId + "&" + strKeyName + "=" + strKeyValue + "&cancel=1";
                    }
                    else
                    {
                        _ServiceURL = Globals.NavigateURL( PortalSettings.UserTabId, ctlRegister, strKeyName + "=" + strKeyValue, "cancel=1" );
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return _ServiceURL;
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/13/2006
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ( Request.QueryString["Services"] != null ) )
                {
                    Services = int.Parse( Request.QueryString["Services"] );
                }

                // free subscriptions
                if( ( Request.QueryString["RoleID"] != null ) )
                {
                    RoleID = int.Parse( Request.QueryString["RoleID"] );

                    RoleController objRoles = new RoleController();

                    RoleInfo objRole = objRoles.GetRole( RoleID, PortalSettings.PortalId );

                    if( objRole.IsPublic && objRole.ServiceFee == 0.0 )
                    {
                        objRoles.UpdateUserRole( PortalId, UserInfo.UserID, RoleID, Convert.ToBoolean( ( Request.QueryString["cancel"] != null ) ? true : false ) );

                        if( PortalSettings.UserTabId != - 1 )
                        {
                            // user defined tab
                            Response.Redirect( Globals.NavigateURL( PortalSettings.UserTabId ), true );
                        }
                        else
                        {
                            // admin tab
                            Response.Redirect( Globals.NavigateURL( TabId, "profile", "UserID=" + UserInfo.UserID.ToString() ), true );
                        }
                    }
                    else
                    {
                        // EVENTLOGGER
                    }
                }

                // If this is the first visit to the page, bind the role data to the datalist
                if( Page.IsPostBack == false )
                {
                    //DataBind()
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdRSVP_Click runs when the Subscribe to RSVP Code Roles Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	01/19/2006  created
        /// </history>
        protected void cmdRSVP_Click( object sender, EventArgs e )
        {
            //Get the RSVP code
            string code = txtRSVPCode.Text;

            if( code != "" )
            {
                //Get the roles from the Database
                RoleController objRoles = new RoleController();
                RoleInfo objRole;
                ArrayList arrRoles = objRoles.GetPortalRoles( PortalSettings.PortalId );

                //Parse the roles
                foreach( RoleInfo tempLoopVar_objRole in arrRoles )
                {
                    objRole = tempLoopVar_objRole;
                    if( objRole.RSVPCode == code )
                    {
                        //Subscribe User to Role
                        objRoles.UpdateUserRole( PortalId, UserInfo.UserID, objRole.RoleID );
                    }
                }
            }

            //Reset RSVP Code field
            txtRSVPCode.Text = "";

            DataBind();
        }
    }
}