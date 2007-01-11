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
using System.IO;
using System.Net;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Sales
{
    public partial class Purchase : PortalModuleBase
    {
        private int RoleID = - 1;


        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                double dblTotal;
                string strCurrency;

                if( ( Request.QueryString["RoleID"] != null ) )
                {
                    RoleID = int.Parse( Request.QueryString["RoleID"] );
                }

                if( Page.IsPostBack == false )
                {
                    RoleController objRoles = new RoleController();

                    if( RoleID != - 1 )
                    {
                        RoleInfo objRole = objRoles.GetRole( RoleID, PortalSettings.PortalId );

                        if( objRole.RoleID != -1 )
                        {
                            lblServiceName.Text = objRole.RoleName;
                            if( ! Null.IsNull( objRole.Description ) )
                            {
                                lblDescription.Text = objRole.Description;
                            }
                            if( RoleID == PortalSettings.AdministratorRoleId )
                            {
                                if( ! Null.IsNull( PortalSettings.HostFee ) )
                                {
                                    lblFee.Text = Strings.Format( PortalSettings.HostFee, "#,##0.00" );
                                }
                            }
                            else
                            {
                                if( ! Null.IsNull( objRole.ServiceFee ) )
                                {
                                    lblFee.Text = Strings.Format( objRole.ServiceFee, "#,##0.00" );
                                }
                            }
                            if( ! Null.IsNull( objRole.BillingFrequency ) )
                            {
                                ListController ctlEntry = new ListController();
                                ListEntryInfo entry = ctlEntry.GetListEntryInfo( "Frequency", objRole.BillingFrequency );
                                lblFrequency.Text = entry.Text;
                            }
                            txtUnits.Text = "1";
                            if( objRole.BillingFrequency == "1" ) // one-time fee
                            {
                                txtUnits.Enabled = false;
                            }
                        }
                        else // security violation attempt to access item not related to this Module
                        {
                            Response.Redirect( Globals.NavigateURL(), true );
                        }
                    }

                    // Store URL Referrer to return to portal
                    if( Request.UrlReferrer != null )
                    {
                        ViewState["UrlReferrer"] = Convert.ToString( Request.UrlReferrer );
                    }
                    else
                    {
                        ViewState["UrlReferrer"] = "";
                    }
                }

                if( RoleID == PortalSettings.AdministratorRoleId )
                {
                    strCurrency = Convert.ToString( Globals.HostSettings["HostCurrency"] );
                }
                else
                {
                    strCurrency = PortalSettings.Currency;
                }

                dblTotal = Conversion.Val( lblFee.Text )*Conversion.Val( txtUnits.Text );
                lblTotal.Text = Strings.Format( dblTotal, "#,##0.00" );

                lblFeeCurrency.Text = strCurrency;
                lblTotalCurrency.Text = strCurrency;
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void PurchaseBtn_Click( Object sender, EventArgs e )
        {
            try
            {
                string strPaymentProcessor = "";
                string strProcessorUserId = "";
                string strProcessorPassword = "";

                if( Page.IsValid )
                {
                    PortalController objPortalController = new PortalController();
                    PortalInfo objPortalInfo = objPortalController.GetPortal( PortalSettings.PortalId );
                    if( objPortalInfo != null )
                    {
                        strPaymentProcessor = objPortalInfo.PaymentProcessor;
                        strProcessorUserId = objPortalInfo.ProcessorUserId;
                        strProcessorPassword = objPortalInfo.ProcessorPassword;
                    }

                    if( strPaymentProcessor == "PayPal" )
                    {
                        // build secure PayPal URL
                        string strPayPalURL = "";
                        strPayPalURL = "https://www.paypal.com/xclick/business=" + Globals.HTTPPOSTEncode( strProcessorUserId );
                        strPayPalURL = strPayPalURL + "&item_name=" + Globals.HTTPPOSTEncode(PortalSettings.PortalName + " - " + lblDescription.Text + " ( " + txtUnits.Text + " units @ " + lblFee.Text + " " + lblFeeCurrency.Text + " per " + lblFrequency.Text + " )");
                        strPayPalURL = strPayPalURL + "&item_number=" + Globals.HTTPPOSTEncode(Convert.ToString(RoleID));
                        strPayPalURL = strPayPalURL + "&quantity=1";
                        strPayPalURL = strPayPalURL + "&custom=" + Globals.HTTPPOSTEncode(UserInfo.UserID.ToString());
                        strPayPalURL = strPayPalURL + "&amount=" + Globals.HTTPPOSTEncode(lblTotal.Text);
                        strPayPalURL = strPayPalURL + "&currency_code=" + Globals.HTTPPOSTEncode(lblTotalCurrency.Text);
                        strPayPalURL = strPayPalURL + "&return=" + Globals.HTTPPOSTEncode("http://" + Globals.GetDomainName(Request));
                        strPayPalURL = strPayPalURL + "&cancel_return=" + Globals.HTTPPOSTEncode("http://" + Globals.GetDomainName(Request));
                        strPayPalURL = strPayPalURL + "&notify_url=" + Globals.HTTPPOSTEncode("http://" + Globals.GetDomainName(Request) + "/admin/Sales/PayPalIPN.aspx");
                        strPayPalURL = strPayPalURL + "&undefined_quantity=&no_note=1&no_shipping=1";

                        // redirect to PayPal
                        Response.Redirect( strPayPalURL, true );
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void cmdCancel_Click( Object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Convert.ToString( ViewState["UrlReferrer"] ), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        public double ConvertCurrency( string Amount, string FromCurrency, string ToCurrency )
        {
            double returnValue;
            string strPost = "Amount=" + Amount + "&From=" + FromCurrency + "&To=" + ToCurrency;
            StreamWriter objStream;

            returnValue = 0;

            try
            {
                HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create( "http://www.xe.com/ucc/convert.cgi" );
                objRequest.Method = "POST";
                objRequest.ContentLength = strPost.Length;
                objRequest.ContentType = "application/x-www-form-urlencoded";

                objStream = new StreamWriter( objRequest.GetRequestStream() );
                objStream.Write( strPost );
                objStream.Close();

                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                StreamReader sr;
                sr = new StreamReader( objResponse.GetResponseStream() );
                string strResponse = sr.ReadToEnd();
                sr.Close();

                int intPos1 = Strings.InStr( 1, strResponse, ToCurrency + "</B>", 0 );
                int intPos2 = Strings.InStrRev( strResponse, "<B>", intPos1, 0 );

                returnValue = Conversion.Val( strResponse.Substring( intPos2 + 3 - 1, ( intPos1 - intPos2 ) - 4 ) );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }

            return returnValue;
        }
    }
}