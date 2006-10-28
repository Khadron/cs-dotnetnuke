using System;
using System.Collections;
using System.Drawing;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Vendors;

namespace DotNetNuke.UI.Skins.Controls
{
    /// <summary></summary>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class Banner : SkinObjectBase
    {
        // private members
        private string _groupName;
        private string _bannerTypeId;
        private string _bannerCount;
        private string _width;
        private string _orientation;
        private string _borderWidth;
        private string _borderColor;
        private string _rowHeight;
        private string _colWidth;

        private const string MyFileName = "Banner.ascx";

        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                _groupName = value;
            }
        }

        public string BannerTypeId
        {
            get
            {
                return _bannerTypeId;
            }
            set
            {
                _bannerTypeId = value;
            }
        }

        public string BannerCount
        {
            get
            {
                return _bannerCount;
            }
            set
            {
                _bannerCount = value;
            }
        }

        public string Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        public string Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                _orientation = value;
            }
        }

        public string BorderWidth
        {
            get
            {
                return _borderWidth;
            }
            set
            {
                _borderWidth = value;
            }
        }

        public string BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value;
            }
        }

        public string RowHeight
        {
            get
            {
                return _rowHeight;
            }
            set
            {
                _rowHeight = value;
            }
        }

        public string ColWidth
        {
            get
            {
                return _colWidth;
            }
            set
            {
                _colWidth = value;
            }
        }

        //*******************************************************
        //
        // The Page_Load server event handler on this page is used
        // to populate the role information for the page
        //
        //*******************************************************

        protected void Page_Load( Object sender, EventArgs e )
        {
            // public attributes
            if( BannerTypeId == "" )
            {
                BannerTypeId = "1"; // banner
            }
            if( BannerCount == "" )
            {
                BannerCount = "1";
            }

            if( PortalSettings.BannerAdvertising != 0 )
            {
                int intPortalId;
                if( PortalSettings.BannerAdvertising == 1 )
                {
                    intPortalId = PortalSettings.PortalId; // portal
                }
                else
                {
                    intPortalId = Null.NullInteger; // host
                }

                // load banners
                BannerController objBanners = new BannerController();
                ArrayList arrBanners = objBanners.LoadBanners( intPortalId, Null.NullInteger, int.Parse( BannerTypeId ), GroupName, int.Parse( BannerCount ) );

                if( arrBanners.Count == 0 )
                {
                    if( BannerTypeId == "1" )
                    {
                        // add default banner if none found
                        BannerInfo objBanner = new BannerInfo();
                        objBanner.BannerId = - 1;
                        objBanner.ImageFile = Globals.ApplicationPath + "/images/banner.gif";
                        objBanner.URL = Globals.GetDomainName(Request);
                        objBanner.BannerName = Localization.GetString( "Banner", Localization.GetResourceFile( this, MyFileName ) );
                        arrBanners.Add( objBanner );
                    }
                }
                
                // bind to datalist
                lstBanners.DataSource = arrBanners;
                lstBanners.DataBind();

                // set banner display characteristics
                if( lstBanners.Items.Count != 0 )
                {
                    // container attributes
                    lstBanners.RepeatLayout = RepeatLayout.Table;
                    if( Width != "" )
                    {
                        lstBanners.Width = Unit.Parse( Width + "px" );
                    }
                    if( lstBanners.Items.Count == 1 )
                    {
                        lstBanners.CellPadding = 0;
                        lstBanners.CellSpacing = 0;
                    }
                    else
                    {
                        lstBanners.CellPadding = 4;
                        lstBanners.CellSpacing = 0;
                    }

                    if( Orientation != "" )
                    {
                        switch( Orientation )
                        {
                            case "H":

                                lstBanners.RepeatDirection = RepeatDirection.Horizontal;
                                break;
                            case "V":

                                lstBanners.RepeatDirection = RepeatDirection.Vertical;
                                break;
                        }
                    }
                    else
                    {
                        lstBanners.RepeatDirection = RepeatDirection.Vertical;
                    }

                    if( BorderWidth != "" )
                    {
                        lstBanners.ItemStyle.BorderWidth = Unit.Parse( BorderWidth + "px" );
                    }
                    if( BorderColor != "" )
                    {
                        ColorConverter objColorConverter = new ColorConverter();
                        lstBanners.ItemStyle.BorderColor = (Color)objColorConverter.ConvertFrom( BorderColor );
                    }

                    // item attributes
                    lstBanners.ItemStyle.VerticalAlign = VerticalAlign.Middle;
                    if( RowHeight != "" )
                    {
                        lstBanners.ItemStyle.Height = Unit.Parse( RowHeight + "px" );
                    }
                    if( ColWidth != "" )
                    {
                        lstBanners.ItemStyle.Width = Unit.Parse( ColWidth + "px" );
                    }
                }
                else
                {
                    lstBanners.Visible = false;
                }
            }
            else
            {
                lstBanners.Visible = false;
            }
        }

        public string FormatItem( int VendorId, int BannerId, int BannerTypeId, string BannerName, string ImageFile, string Description, string URL, int Width, int Height )
        {
            BannerController objBanners = new BannerController();
            return objBanners.FormatBanner( VendorId, BannerId, BannerTypeId, BannerName, ImageFile, Description, URL, Width, Height, Convert.ToString( PortalSettings.BannerAdvertising == 1 ? "L" : "G" ), PortalSettings.HomeDirectory );
        }
    }
}