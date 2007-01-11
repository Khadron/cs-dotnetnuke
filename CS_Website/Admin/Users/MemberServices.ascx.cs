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
        ///	<returns>The correctly formatted url</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatURL()
        {
            string formatURL = Null.NullString;
            try
            {
                string strServerPath;

                strServerPath = Request.ApplicationPath;
                if( ! strServerPath.EndsWith( "/" ) )
                {
                    strServerPath += "/";
                }

                formatURL = strServerPath + "Register.aspx?tabid=" + TabId;
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return formatURL;
        }

        /// <summary>
        /// FormatPrice formats the Fee amount and filters out null-values
        /// </summary>
        ///	<param name="price">The price to format</param>
        ///	<returns>The correctly formatted price</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatPrice( float price )
        {
            string formatPrice = Null.NullString;
            try
            {
                if( price != Null.NullSingle )
                {
                    formatPrice = price.ToString( "##0.00" );
                }
                else
                {
                    formatPrice = "";
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return formatPrice;
        }

        /// <summary>
        /// FormatPeriod formats the Period and filters out null-values
        /// </summary>
        ///	<param name="period">The period to format</param>
        ///	<returns>The correctly formatted period</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatPeriod( int period )
        {
            string formatPeriod = Null.NullString;
            try
            {
                if( period != Null.NullInteger )
                {
                    formatPeriod = period.ToString();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return formatPeriod;
        }

        /// <summary>
        /// FormatExpiryDate formats the expiry date and filters out null-values
        /// </summary>
        ///	<param name="DateTime">The date to format</param>
        ///	<returns>The correctly formatted date</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatExpiryDate( DateTime DateTime )
        {
            string formatExpiryDate = Null.NullString;
            try
            {
                if( ! Null.IsNull( DateTime ) )
                {
                    formatExpiryDate = DateTime.ToShortDateString();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return formatExpiryDate;
        }

        /// <summary>
        /// ServiceText gets the Service Text (Cancel or Subscribe)
        /// </summary>
        ///	<param name="Subscribed">The service state</param>
        ///	<returns>The correctly formatted text</returns>
        /// <history>
        /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string ServiceText( bool Subscribed )
        {
            string serviceText = Null.NullString;
            try
            {
                if( ! Subscribed )
                {
                    serviceText = Localization.GetString( "Subscribe", this.LocalResourceFile );
                }
                else
                {
                    serviceText = Localization.GetString( "Unsubscribe", this.LocalResourceFile );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return serviceText;
        }

        /// <summary>
        /// ServiceURL correctly formats the Subscription url
        /// </summary>
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
            string serviceURL = Null.NullString;
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
                        serviceURL = "~/admin/Sales/PayPalSubscription.aspx?tabid=" + TabId + "&" + strKeyName + "=" + strKeyValue;
                    }
                    else
                    {
                        serviceURL = Globals.NavigateURL( PortalSettings.UserTabId, ctlRegister, strKeyName + "=" + strKeyValue );
                    }
                }
                else // cancel
                {
                    if( dblServiceFee > 0 )
                    {
                        serviceURL = "~/admin/Sales/PayPalSubscription.aspx?tabid=" + TabId + "&" + strKeyName + "=" + strKeyValue + "&cancel=1";
                    }
                    else
                    {
                        serviceURL = Globals.NavigateURL( PortalSettings.UserTabId, ctlRegister, strKeyName + "=" + strKeyValue, "cancel=1" );
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return serviceURL;
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
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
                            Response.Redirect( Globals.NavigateURL( TabId, "profile", "UserID=" + UserInfo.UserID ), true );
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
                    //Localize the Headers
                    Localization.LocalizeDataGrid(ref grdServices, this.LocalResourceFile);
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
        /// <history>
        /// 	[cnurse]	01/19/2006  created
        /// </history>
        protected void cmdRSVP_Click( object sender, EventArgs e )
        {
            //Get the RSVP code
            string code = txtRSVPCode.Text;

            if( !String.IsNullOrEmpty(code) )
            {
                //Get the roles from the Database
                RoleController objRoles = new RoleController();
                ArrayList arrRoles = objRoles.GetPortalRoles( PortalSettings.PortalId );

                //Parse the roles
                foreach( RoleInfo objRole in arrRoles )
                {
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