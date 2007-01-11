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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Vendors;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Vendors
{
    /// <summary>
    /// The EditAffiliate PortalModuleBase is used to add/edit an Affiliate
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/21/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class EditAffiliate : PortalModuleBase
    {
        private int VendorId = - 1;
        private int AffiliateId = - 1;

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/21/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            if( ( Request.QueryString["VendorId"] != null ) )
            {
                VendorId = int.Parse( Request.QueryString["VendorId"] );
            }

            if( ( Request.QueryString["AffilId"] != null ) )
            {
                AffiliateId = int.Parse( Request.QueryString["AffilId"] );
            }

            //this needs to execute always to the client script code is registred in InvokePopupCal
            cmdStartCalendar.NavigateUrl = Calendar.InvokePopupCal( txtStartDate );
            cmdEndCalendar.NavigateUrl = Calendar.InvokePopupCal( txtEndDate );

            if( Page.IsPostBack == false )
            {
                ClientAPI.AddButtonConfirm( cmdDelete, Localization.GetString( "DeleteItem" ) );

                AffiliateController objAffiliates = new AffiliateController();
                if( AffiliateId != Null.NullInteger )
                {
                    // Obtain a single row of banner information
                    AffiliateInfo objAffiliate = objAffiliates.GetAffiliate( AffiliateId, VendorId, PortalId );

                    if( objAffiliate != null )
                    {
                        if( ! Null.IsNull( objAffiliate.StartDate ) )
                        {
                            txtStartDate.Text = objAffiliate.StartDate.ToShortDateString();
                        }
                        if( ! Null.IsNull( objAffiliate.EndDate ) )
                        {
                            txtEndDate.Text = objAffiliate.EndDate.ToShortDateString();
                        }
                        txtCPC.Text = objAffiliate.CPC.ToString( "#0.#####" );
                        txtCPA.Text = objAffiliate.CPA.ToString( "#0.#####" );
                    }
                    else // security violation attempt to access item not related to this Module
                    {
                        Response.Redirect( EditUrl( "VendorId", VendorId.ToString() ), true );
                    }
                }
                else
                {
                    txtCPC.Text = "0.00";
                    txtCPA.Text = "0.00";

                    cmdDelete.Visible = false;
                }
            }
        }

        /// <summary>
        /// cmdCancel_Click runs when the Cancel Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/21/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdCancel_Click( object sender, EventArgs e )
        {
            // Redirect back to the portal home page
            Response.Redirect( EditUrl( "VendorId", VendorId.ToString() ), true );
        }

        /// <summary>
        /// cmdDelete_Click runs when the Delete Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/21/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdDelete_Click( object sender, EventArgs e )
        {
            if( AffiliateId != - 1 )
            {
                AffiliateController objAffiliates = new AffiliateController();
                objAffiliates.DeleteAffiliate( AffiliateId );

                // Redirect back to the portal home page
                Response.Redirect( EditUrl( "VendorId", VendorId.ToString() ), true );
            }
        }

        /// <summary>
        /// cmdSend_Click runs when the Send Notification Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/21/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdSend_Click( object sender, EventArgs e )
        {
            VendorController objVendors = new VendorController();
            VendorInfo objVendor;

            objVendor = objVendors.GetVendor( VendorId, PortalId );
            if( objVendor != null )
            {
                if( ! Null.IsNull( objVendor.Email ) )
                {
                    ArrayList custom = new ArrayList();
                    custom.Add( objVendor.VendorName );
                    custom.Add( Globals.GetPortalDomainName( PortalSettings.PortalAlias.HTTPAlias, Request, true ) + "/" + Globals.glbDefaultPage + "?AffiliateId=" + VendorId.ToString() );

                    string errorMsg = Mail.SendMail( PortalSettings.Email, objVendor.Email, "", Localization.GetSystemMessage( PortalSettings, "EMAIL_AFFILIATE_NOTIFICATION_SUBJECT" ), Localization.GetSystemMessage( PortalSettings, "EMAIL_AFFILIATE_NOTIFICATION_BODY", Localization.GlobalResourceFile,  custom), "", "", "", "", "", "" );
                    string strMessage;
                    if( errorMsg == "" )
                    {
                        //Success
                        strMessage = Localization.GetString( "NotificationSuccess", this.LocalResourceFile );
                        UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessageType.GreenSuccess );
                    }
                    else
                    {
                        //Failed
                        strMessage = Localization.GetString( "NotificationFailure", this.LocalResourceFile );
                        strMessage = string.Format( strMessage, errorMsg );
                        UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessageType.RedError );
                    }
                }
            }
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/21/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdUpdate_Click( object sender, EventArgs e )
        {
            if( Page.IsValid )
            {
                AffiliateInfo objAffiliate = new AffiliateInfo();

                objAffiliate.AffiliateId = AffiliateId;
                objAffiliate.VendorId = VendorId;
                if( !String.IsNullOrEmpty(txtStartDate.Text) )
                {
                    objAffiliate.StartDate = DateTime.Parse( txtStartDate.Text );
                }
                else
                {
                    objAffiliate.StartDate = Null.NullDate;
                }
                if( !String.IsNullOrEmpty(txtEndDate.Text) )
                {
                    objAffiliate.EndDate = DateTime.Parse( txtEndDate.Text );
                }
                else
                {
                    objAffiliate.EndDate = Null.NullDate;
                }
                objAffiliate.CPC = double.Parse( txtCPC.Text );
                objAffiliate.CPA = double.Parse( txtCPA.Text );

                AffiliateController objAffiliates = new AffiliateController();

                if( AffiliateId == - 1 )
                {
                    objAffiliates.AddAffiliate( objAffiliate );
                }
                else
                {
                    objAffiliates.UpdateAffiliate( objAffiliate );
                }

                // Redirect back to the portal home page
                Response.Redirect( EditUrl( "VendorId", VendorId.ToString() ), true );
            }
        }

        public new int PortalId
        {
            get
            {
                if( PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId )
                {
                    return - 1;
                }
                else
                {
                    return PortalSettings.PortalId;
                }
            }
        }

        

    }
}