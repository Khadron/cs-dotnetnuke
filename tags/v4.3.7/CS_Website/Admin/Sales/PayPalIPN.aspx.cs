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
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;

namespace DotNetNuke.Modules.Admin.Sales
{
    public partial class PayPalIPN : PageBase
    {
        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }

        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                string strName;
                StreamWriter objStream;
                bool blnValid = true;
                string strTransactionID;
                string strTransactionType;
                int intRoleID = 0;
                int intPortalID = PortalSettings.PortalId;
                int intUserID = 0;
                string strDescription;
                double dblAmount = 0;
                string strEmail;
                string strBody;
                bool blnCancel = false;
                string strPayPalID = String.Empty;

                RoleController objRoles = new RoleController();
                PortalController objPortalController = new PortalController();

                string strPost = "cmd=_notify-validate";
                foreach( string tempLoopVar_strName in Request.Form )
                {
                    strName = tempLoopVar_strName;
                    string strValue = Request.Form[strName];
                    switch( strName )
                    {
                        case "txn_type": // get the transaction type

                            strTransactionType = strValue;
                            switch( strTransactionType )
                            {
                                case "subscr_signup":
                                    break;

                                case "subscr_payment":
                                    break;

                                case "web_accept":

                                    break;
                                case "subscr_cancel":

                                    blnCancel = true;
                                    break;
                                default:

                                    blnValid = false;
                                    break;
                            }
                            break;
                        case "payment_status": // verify the status

                            if( strValue != "Completed" )
                            {
                                blnValid = false;
                            }
                            break;
                        case "txn_id": // verify the transaction id for duplicates

                            strTransactionID = strValue;
                            break;
                        case "receiver_email": // verify the PayPalId

                            strPayPalID = strValue;
                            break;
                        case "mc_gross": // verify the price

                            dblAmount = double.Parse( strValue );
                            break;
                        case "item_number": // get the RoleID

                            intRoleID = int.Parse( strValue );
                            RoleInfo objRole = objRoles.GetRole( intRoleID, intPortalID );
                            break;
                        case "item_name": // get the product description

                            strDescription = strValue;
                            break;
                        case "custom": // get the UserID

                            intUserID = int.Parse( strValue );
                            break;
                        case "email": // get the email

                            strEmail = strValue;
                            break;
                    }
                    // reconstruct post for postback validation
                    strPost += string.Format( "&{0}={1}", strName, Globals.HTTPPOSTEncode( strValue ) );
                }
                // postback to verify the source
                if( blnValid )
                {
                    HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create( "https://www.paypal.com/cgi-bin/webscr" );
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

                    switch( strResponse )
                    {
                        case "VERIFIED":

                            break;
                        default:

                            // possible fraud
                            blnValid = false;
                            break;
                    }
                }

                if( blnValid )
                {
                    int intAdministratorRoleId = 0;
                    string strProcessorID = String.Empty;
                    PortalInfo objPortalInfo = objPortalController.GetPortal( intPortalID );
                    if( objPortalInfo != null )
                    {
                        intAdministratorRoleId = objPortalInfo.AdministratorRoleId;
                        strProcessorID = objPortalInfo.ProcessorUserId.ToLower();
                    }
                    if( intRoleID == intAdministratorRoleId )
                    {
                        // admin portal renewal
                        strProcessorID = Convert.ToString( PortalSettings.HostSettings["ProcessorUserId"] ).ToLower();
                        float portalPrice = objPortalInfo.HostFee;
                        if( ( portalPrice.ToString() == dblAmount.ToString() ) && ( HttpUtility.UrlDecode( strPayPalID.ToLower() ) == strProcessorID ) )
                        {
                            objPortalController.UpdatePortalExpiry( intPortalID );
                        }
                        else
                        {
                            try
                            {
                                EventLogController objEventLog = new EventLogController();
                                LogInfo objEventLogInfo = new LogInfo();
                                objEventLogInfo.LogPortalID = intPortalID;
                                objEventLogInfo.LogPortalName = PortalSettings.PortalName;
                                objEventLogInfo.LogUserID = intUserID;
                                objEventLogInfo.LogTypeKey = "POTENTIAL PAYPAL PAYMENT FRAUD";
                                objEventLog.AddLog( objEventLogInfo );
                            }
                            catch( Exception )
                            {
                            }
                        }
                    }
                    else
                    {
                        // user subscription
                        RoleInfo objRoleInfo = objRoles.GetRole( intRoleID, intPortalID );
                        double rolePrice = objRoleInfo.ServiceFee;
                        if( ( rolePrice.ToString() == dblAmount.ToString() ) && ( HttpUtility.UrlDecode( strPayPalID.ToLower() ) == strProcessorID ) )
                        {
                            objRoles.UpdateUserRole( intPortalID, intUserID, intRoleID, blnCancel );
                        }
                        else
                        {
                            try
                            {
                                //let's use the new logging provider.
                            }
                            catch( Exception )
                            {
                            }
                        }
                    }
                }
            }
            catch( Exception exc ) //Page failed to load
            {
                Exceptions.ProcessPageLoadException( exc );
            }
        }
    }
}