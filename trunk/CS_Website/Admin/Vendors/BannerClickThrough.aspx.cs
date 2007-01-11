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
                                if( !String.IsNullOrEmpty(objVendor.Website) )
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