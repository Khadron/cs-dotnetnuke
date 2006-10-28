using System;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Vendors;

namespace DotNetNuke.Modules.Admin.Vendors
{
    public partial class BannerClickThrough : PageBase
    {
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ( Request.QueryString["vendorid"] != null ) && ( Request.QueryString["bannerid"] != null ) )
                {
                    int intVendorId = int.Parse( Request.QueryString["vendorid"] );
                    int intBannerId = int.Parse( Request.QueryString["bannerid"] );

                    string strURL = "~/" + Globals.glbDefaultPage;

                    BannerController objBanners = new BannerController();
                    BannerInfo objBanner = objBanners.GetBanner( intBannerId, intVendorId, PortalSettings.PortalId );
                    if( objBanner == null )
                    {
                        //Try a Host Banner
                        objBanner = objBanners.GetBanner( intBannerId, intVendorId, Null.NullInteger );
                    }
                    if( objBanner != null )
                    {
                        if( ! Null.IsNull( objBanner.URL ) )
                        {
                            strURL = Globals.LinkClick( objBanner.URL, - 1, - 1, false );
                        }
                        else
                        {
                            VendorController objVendors = new VendorController();
                            VendorInfo objVendor = objVendors.GetVendor( objBanner.VendorId, PortalSettings.PortalId );
                            if( objVendor == null )
                            {
                                //Try a Host Vendor
                                objVendor = objVendors.GetVendor( objBanner.VendorId, Null.NullInteger );
                            }
                            if( objVendor != null )
                            {
                                if( objVendor.Website != "" )
                                {
                                    strURL = Globals.AddHTTP( objVendor.Website );
                                }
                            }
                        }
                    }
                    else
                    {
                        if( Request.UrlReferrer != null )
                        {
                            strURL = Request.UrlReferrer.ToString();
                        }
                    }

                    objBanners.UpdateBannerClickThrough( intBannerId, intVendorId );

                    Response.Redirect( strURL, true );
                }
            }
            catch( Exception exc ) //Page failed to load
            {
                Exceptions.ProcessPageLoadException( exc );
            }
        }
    }
}