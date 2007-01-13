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
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Vendors;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using Calendar=DotNetNuke.Common.Utilities.Calendar;

namespace DotNetNuke.Modules.Admin.Vendors
{
    /// <summary>
    /// The EditBanner PortalModuleBase is used to add/edit a Banner
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/21/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class EditBanner : PortalModuleBase
    {
        private int VendorId;
        protected Label lblBannerGroup;
        private int BannerId;

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
            try
            {
                if( ( Request.QueryString["VendorId"] != null ) )
                {
                    VendorId = int.Parse( Request.QueryString["VendorId"] );
                }
                if( ( Request.QueryString["BannerId"] != null ) )
                {
                    BannerId = int.Parse( Request.QueryString["BannerId"] );
                }

                //this needs to execute always to the client script code is registred in InvokePopupCal
                cmdStartCalendar.NavigateUrl = Calendar.InvokePopupCal( txtStartDate );
                cmdEndCalendar.NavigateUrl = Calendar.InvokePopupCal( txtEndDate );

                if( Page.IsPostBack == false )
                {
                    ctlImage.FileFilter = Common.Globals.glbImageFileTypes;
                    ClientAPI.AddButtonConfirm( cmdDelete, Localization.GetString( "DeleteItem" ) );

                    BannerTypeController objBannerTypes = new BannerTypeController();
                    // Get the banner types from the database
                    cboBannerType.DataSource = objBannerTypes.GetBannerTypes();
                    cboBannerType.DataBind();

                    BannerController objBanners = new BannerController();
                    if( BannerId != Null.NullInteger )
                    {
                        // Obtain a single row of banner information
                        BannerInfo objBanner = objBanners.GetBanner( BannerId, VendorId, PortalId );

                        if( objBanner != null )
                        {
                            txtBannerName.Text = objBanner.BannerName;
                            cboBannerType.Items.FindByValue( objBanner.BannerTypeId.ToString() ).Selected = true;
                            txtBannerGroup.Text = objBanner.GroupName;
                            ctlImage.Url = objBanner.ImageFile;
                            if( objBanner.Width != 0 )
                            {
                                txtWidth.Text = objBanner.Width.ToString();
                            }
                            if( objBanner.Height != 0 )
                            {
                                txtHeight.Text = objBanner.Height.ToString();
                            }
                            txtDescription.Text = objBanner.Description;
                            if( objBanner.URL != null )
                            {
                                ctlURL.Url = objBanner.URL;
                            }
                            txtImpressions.Text = objBanner.Impressions.ToString();
                            txtCPM.Text = objBanner.CPM.ToString();
                            if( ! Null.IsNull( objBanner.StartDate ) )
                            {
                                txtStartDate.Text = objBanner.StartDate.ToShortDateString();
                            }
                            if( ! Null.IsNull( objBanner.EndDate ) )
                            {
                                txtEndDate.Text = objBanner.EndDate.ToShortDateString();
                            }
                            optCriteria.Items.FindByValue( objBanner.Criteria.ToString() ).Selected = true;

                            ctlAudit.CreatedByUser = objBanner.CreatedByUser;
                            ctlAudit.CreatedDate = objBanner.CreatedDate.ToString();

                            ArrayList arrBanners = new ArrayList();
                            arrBanners.Add( objBanner );
                            lstBanners.DataSource = arrBanners;
                            lstBanners.DataBind();
                        }
                        else // security violation attempt to access item not related to this Module
                        {
                            Response.Redirect( EditUrl( "VendorId", VendorId.ToString() ), true );
                        }
                    }
                    else
                    {
                        txtImpressions.Text = "0";
                        txtCPM.Text = "0";
                        optCriteria.Items.FindByValue( "1" ).Selected = true;

                        cmdDelete.Visible = false;
                        cmdCopy.Visible = false;
                        cmdEmail.Visible = false;
                        ctlAudit.Visible = false;
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
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
            try
            {
                // Redirect back to the portal home page
                Response.Redirect( EditUrl( "VendorId", VendorId.ToString() ), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
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
            try
            {
                if( BannerId != - 1 )
                {
                    BannerController objBanner = new BannerController();
                    objBanner.DeleteBanner( BannerId );

                    // Redirect back to the portal home page
                    Response.Redirect( EditUrl( "VendorId", VendorId.ToString() ), true );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
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
            try
            {
                // Only Update if the Entered Data is val
                if( Page.IsValid )
                {
                    if( ! cmdCopy.Visible )
                    {
                        BannerId = - 1;
                    }
                    DateTime StartDate = Null.NullDate;
                    if( !String.IsNullOrEmpty(txtStartDate.Text) )
                    {
                        StartDate = Convert.ToDateTime( txtStartDate.Text );
                    }

                    DateTime EndDate = Null.NullDate;
                    if( !String.IsNullOrEmpty(txtEndDate.Text) )
                    {
                        EndDate = Convert.ToDateTime( txtEndDate.Text );
                    }

                    // Create an instance of the Banner DB component
                    BannerInfo objBanner = new BannerInfo();
                    objBanner.BannerId = BannerId;
                    objBanner.VendorId = VendorId;
                    objBanner.BannerName = txtBannerName.Text;
                    objBanner.BannerTypeId = Convert.ToInt32( cboBannerType.SelectedItem.Value );
                    objBanner.GroupName = txtBannerGroup.Text;
                    objBanner.ImageFile = ctlImage.Url;
                    if( !String.IsNullOrEmpty(txtWidth.Text) )
                    {
                        objBanner.Width = int.Parse( txtWidth.Text );
                    }
                    else
                    {
                        objBanner.Width = 0;
                    }
                    if( !String.IsNullOrEmpty(txtHeight.Text) )
                    {
                        objBanner.Height = int.Parse( txtHeight.Text );
                    }
                    else
                    {
                        objBanner.Height = 0;
                    }
                    objBanner.Description = txtDescription.Text;
                    objBanner.URL = ctlURL.Url;
                    objBanner.Impressions = int.Parse( txtImpressions.Text );
                    objBanner.CPM = double.Parse( txtCPM.Text );
                    objBanner.StartDate = StartDate;
                    objBanner.EndDate = EndDate;
                    objBanner.Criteria = int.Parse( optCriteria.SelectedItem.Value );
                    objBanner.CreatedByUser = UserInfo.UserID.ToString();

                    BannerController objBanners = new BannerController();
                    if( BannerId == Null.NullInteger )
                    {
                        // Add the banner within the Banners table
                        objBanners.AddBanner( objBanner );
                    }
                    else
                    {
                        // Update the banner within the Banners table
                        objBanners.UpdateBanner( objBanner );
                    }

                    // Redirect back to the portal home page
                    Response.Redirect( EditUrl( "VendorId", VendorId.ToString() ), true );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdCopy_Click( object sender, EventArgs e )
        {
            try
            {
                txtStartDate.Text = "";
                txtEndDate.Text = "";
                cmdDelete.Visible = false;
                cmdCopy.Visible = false;
                cmdEmail.Visible = false;
                ctlAudit.Visible = false;
            }
            catch( Exception exc )
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdEmail_Click( object sender, EventArgs e )
        {
            // send email summary to vendor
            BannerController objBanners = new BannerController();
            BannerInfo objBanner = objBanners.GetBanner( BannerId, VendorId, PortalId );
            if( objBanner != null )
            {
                VendorController objVendors = new VendorController();
                VendorInfo objVendor = objVendors.GetVendor( objBanner.VendorId, PortalId );
                if( objVendor != null )
                {
                    if( ! Null.IsNull( objVendor.Email ) )
                    {
                        ArrayList custom = new ArrayList();
                        custom.Add( objBanner.BannerName );
                        custom.Add( objBanner.Description );
                        custom.Add( objBanner.ImageFile );
                        custom.Add( objBanner.CPM.ToString( "#0.#####" ) );
                        custom.Add( objBanner.Impressions.ToString() );
                        custom.Add( objBanner.StartDate.ToShortDateString() );
                        custom.Add( objBanner.EndDate.ToShortDateString() );
                        custom.Add( objBanner.Views.ToString() );
                        custom.Add( objBanner.ClickThroughs.ToString() );

                        string errorMsg = Mail.SendMail( PortalSettings.Email, objVendor.Email, "", Localization.GetSystemMessage( PortalSettings, "EMAIL_BANNER_NOTIFICATION_SUBJECT", Localization.GlobalResourceFile, custom ), Localization.GetSystemMessage( PortalSettings, "EMAIL_BANNER_NOTIFICATION_BODY", Localization.GlobalResourceFile,  custom), "", "", "", "", "", "" );

                        string strMessage;
                        if( errorMsg == "" )
                        {
                            //Success
                            strMessage = Localization.GetString( "EmailSuccess", this.LocalResourceFile );
                            UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessageType.GreenSuccess );
                        }
                        else
                        {
                            //Failed
                            strMessage = Localization.GetString( "EmailFailure", this.LocalResourceFile );
                            strMessage = string.Format( strMessage, errorMsg );
                            UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessageType.RedError );
                        }
                    }
                }
            }
        }

        public string FormatItem( int vendorId, int bannerId, int BannerTypeId, string BannerName, string ImageFile, string Description, string URL, int Width, int Height )
        {
            BannerController objBanners = new BannerController();            
            return objBanners.FormatBanner(VendorId, BannerId, BannerTypeId, BannerName, ImageFile, Description, URL, Width, Height, System.Convert.ToString(((PortalSettings.BannerAdvertising == 1) ? "L" : "G")), PortalSettings.HomeDirectory);
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