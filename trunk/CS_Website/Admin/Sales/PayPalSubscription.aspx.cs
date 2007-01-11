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
using System.Globalization;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Sales
{
    public partial class PayPalSubscription : PageBase
    {

        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                UserInfo objUserInfo = null;
                int intUserID = - 1;
                if( Request.IsAuthenticated )
                {
                    objUserInfo = UserController.GetCurrentUserInfo();
                    if( objUserInfo != null )
                    {
                        intUserID = objUserInfo.UserID;
                    }
                }

                int intRoleId = - 1;
                if( Request.QueryString["roleid"] != null )
                {
                    intRoleId = int.Parse( Request.QueryString["roleid"] );
                }

                string strProcessorUserId = "";
                PortalController objPortalController = new PortalController();
                PortalInfo objPortalInfo = objPortalController.GetPortal( PortalSettings.PortalId );
                if( objPortalInfo != null )
                {
                    strProcessorUserId = objPortalInfo.ProcessorUserId;
                }

                Hashtable settings = PortalSettings.GetSiteSettings( PortalSettings.PortalId );
                string strPayPalURL;

                if( intUserID != - 1 && intRoleId != - 1 && !String.IsNullOrEmpty(strProcessorUserId) )
                {
                    strPayPalURL = "https://www.paypal.com/cgi-bin/webscr?";

                    if( Request.QueryString["cancel"] != null )
                    {
                        // build the cancellation PayPal URL
                        strPayPalURL += "cmd=_subscr-find&alias=" + Globals.HTTPPOSTEncode( strProcessorUserId );
                    }
                    else
                    {
                        strPayPalURL += "cmd=_ext-enter";

                        RoleController objRoles = new RoleController();

                        RoleInfo objRole = objRoles.GetRole( intRoleId, PortalSettings.PortalId );
                        if( objRole.RoleID != -1 )
                        {
                            int intTrialPeriod = 1;
                            if( objRole.TrialPeriod != 0 )
                            {
                                intTrialPeriod = objRole.TrialPeriod;
                            }
                            int intBillingPeriod = 1;
                            if( objRole.BillingPeriod != 0 )
                            {
                                intBillingPeriod = objRole.BillingPeriod;
                            }
                            // explicitely format numbers using en-US so numbers are correctly built
                            CultureInfo enFormat = new CultureInfo( "en-US" );
                            string strService = string.Format( enFormat.NumberFormat, "{0:#####0.00}", objRole.ServiceFee );
                            string strTrial = string.Format( enFormat.NumberFormat, "{0:#####0.00}", objRole.TrialFee );

                            if( objRole.BillingFrequency == "O" || objRole.TrialFrequency == "O" ) //one-time payment
                            {
                                // build the payment PayPal URL
                                strPayPalURL += "&redirect_cmd=_xclick&business=" + Globals.HTTPPOSTEncode( strProcessorUserId );
                                strPayPalURL += "&item_name=" + Globals.HTTPPOSTEncode( PortalSettings.PortalName + " - " + objRole.RoleName + " ( " + Strings.Format( objRole.ServiceFee, "0.00" ) + " " + PortalSettings.Currency + " )" );
                                strPayPalURL += "&item_number=" + Globals.HTTPPOSTEncode( intRoleId.ToString() );
                                strPayPalURL += "&no_shipping=1&no_note=1";
                                strPayPalURL += "&quantity=1";
                                strPayPalURL += "&amount=" + Globals.HTTPPOSTEncode( strService );
                                strPayPalURL += "&currency_code=" + Globals.HTTPPOSTEncode( PortalSettings.Currency );
                            }
                            else //recurring payments
                            {
                                // build the subscription PayPal URL
                                strPayPalURL += "&redirect_cmd=_xclick-subscriptions&business=" + Globals.HTTPPOSTEncode( strProcessorUserId );
                                strPayPalURL += "&item_name=" + Globals.HTTPPOSTEncode( PortalSettings.PortalName + " - " + objRole.RoleName + " ( " + Strings.Format( objRole.ServiceFee, "0.00" ) + " " + PortalSettings.Currency + " every " + intBillingPeriod.ToString() + " " + GetBillingFrequencyCode( objRole.BillingFrequency ) + " )" );
                                strPayPalURL += "&item_number=" + Globals.HTTPPOSTEncode( intRoleId.ToString() );
                                strPayPalURL += "&no_shipping=1&no_note=1";
                                if( objRole.TrialFrequency != "N" )
                                {
                                    strPayPalURL += "&a1=" + Globals.HTTPPOSTEncode( strTrial );
                                    strPayPalURL += "&p1=" + Globals.HTTPPOSTEncode( intTrialPeriod.ToString() );
                                    strPayPalURL += "&t1=" + Globals.HTTPPOSTEncode( objRole.TrialFrequency );
                                }
                                strPayPalURL += "&a3=" + Globals.HTTPPOSTEncode( strService );
                                strPayPalURL += "&p3=" + Globals.HTTPPOSTEncode( intBillingPeriod.ToString() );
                                strPayPalURL += "&t3=" + Globals.HTTPPOSTEncode( objRole.BillingFrequency );
                                strPayPalURL += "&src=1";
                                strPayPalURL += "&currency_code=" + Globals.HTTPPOSTEncode( PortalSettings.Currency );
                            }
                        }

                        ListController ctlList = new ListController();

                        strPayPalURL += "&custom=" + Globals.HTTPPOSTEncode( intUserID.ToString() );
                        strPayPalURL += "&first_name=" + Globals.HTTPPOSTEncode( objUserInfo.Profile.FirstName );
                        strPayPalURL += "&last_name=" + Globals.HTTPPOSTEncode( objUserInfo.Profile.LastName );
                        try
                        {
                            if( objUserInfo.Profile.Country == "United States" )
                            {
                                ListEntryInfo colList = ctlList.GetListEntryInfo( "Region", objUserInfo.Profile.Region, "Country:US" );
                                strPayPalURL += "&address1=" + Globals.HTTPPOSTEncode( Convert.ToString( !String.IsNullOrEmpty(objUserInfo.Profile.Unit) ? objUserInfo.Profile.Unit + " " : "" ) + objUserInfo.Profile.Street );
                                strPayPalURL += "&city=" + Globals.HTTPPOSTEncode( objUserInfo.Profile.City );
                                strPayPalURL += "&state=" + Globals.HTTPPOSTEncode( colList.Value );
                                strPayPalURL += "&zip=" + Globals.HTTPPOSTEncode( objUserInfo.Profile.PostalCode );
                            }
                        }
                        catch
                        {
                            // issue getting user address
                        }

                        // Return URL
                        if( Convert.ToString( settings["paypalsubscriptionreturn"] ) != "" )
                        {
                            strPayPalURL += "&return=" + Globals.HTTPPOSTEncode( Convert.ToString( settings["paypalsubscriptionreturn"] ) );
                        }
                        else
                        {
                            strPayPalURL += "&return=" + Globals.HTTPPOSTEncode( Globals.AddHTTP( Globals.GetDomainName( Request ) ) );
                        }

                        // Cancellation URL
                        if( Convert.ToString( settings["paypalsubscriptioncancelreturn"] ) != "" )
                        {
                            strPayPalURL += "&cancel_return=" + Globals.HTTPPOSTEncode( Convert.ToString( settings["paypalsubscriptioncancelreturn"] ) );
                        }
                        else
                        {
                            strPayPalURL += "&cancel_return=" + Globals.HTTPPOSTEncode( Globals.AddHTTP( Globals.GetDomainName( Request ) ) );
                        }

                        // Instant Payment Notification URL
                        if( Convert.ToString( settings["paypalsubscriptionnotifyurl"] ) != "" )
                        {
                            strPayPalURL += "&notify_url=" + Globals.HTTPPOSTEncode( Convert.ToString( settings["paypalsubscriptionnotifyurl"] ) );
                        }
                        else
                        {
                            strPayPalURL += "&notify_url=" + Globals.HTTPPOSTEncode( Globals.AddHTTP( Globals.GetDomainName( Request ) ) + "/admin/Sales/PayPalIPN.aspx" );
                        }

                        strPayPalURL += "&sra=1"; // reattempt on failure
                    }

                    // redirect to PayPal
                    Response.Redirect( strPayPalURL, true );
                }
                else
                {
                    // Cancellation URL
                    if( Convert.ToString( settings["paypalsubscriptioncancelreturn"] ) != "" )
                    {
                        strPayPalURL = Convert.ToString( settings["paypalsubscriptioncancelreturn"] );
                    }
                    else
                    {
                        strPayPalURL = Globals.AddHTTP( Globals.GetDomainName( Request ) );
                    }

                    // redirect to PayPal
                    Response.Redirect( strPayPalURL, true );
                }
            }
            catch( Exception exc ) //Page failed to load
            {
                Exceptions.ProcessPageLoadException( exc );
            }
        }

        private string GetBillingFrequencyCode( string Value )
        {
            ListController ctlEntry = new ListController();
            ListEntryInfo entry = ctlEntry.GetListEntryInfo( "Frequency", Value );
            return entry.Value;
        }
    }
}