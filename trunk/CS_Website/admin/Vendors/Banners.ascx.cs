#region DotNetNuke License
// DotNetNuke� - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Vendors;

namespace DotNetNuke.Modules.Admin.Vendors
{
    /// <summary>
    /// The Banners PortalModuleBase is used to manage a Vendor's Banners
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class Banners : PortalModuleBase, IActionable
    {
        public int VendorID;

        /// <summary>
        /// BindData gets the banners from the Database and binds them to the DataGrid
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void BindData()
        {
            BannerController objBanners = new BannerController();

            //Localize the Grid
            Localization.LocalizeDataGrid( ref grdBanners, this.LocalResourceFile );

            grdBanners.DataSource = objBanners.GetBanners( VendorID );
            grdBanners.DataBind();

            cmdAdd.NavigateUrl = FormatURL( "BannerId", "-1" );
        }

        /// <summary>
        /// DisplayDate formats a Date
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="DateValue">The Date to format</param>
        /// <returns>The correctly formatted date</returns>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string DisplayDate( DateTime DateValue )
        {
            string _DisplayDate = Null.NullString;
            try
            {
                if( Null.IsNull( DateValue ) )
                {
                    _DisplayDate = "";
                }
                else
                {
                    _DisplayDate = DateValue.ToShortDateString();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return _DisplayDate;
        }

        /// <summary>
        /// DisplayDate formats a Date
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>The correctly formatted date</returns>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string DisplayType( int bannerTypeId )
        {
            string _DisplayType = Null.NullString;
            try
            {
                BannerType bannerType = (BannerType)Enum.ToObject( typeof( BannerType ), bannerTypeId );
                if( bannerType == BannerType.Banner )
                {
                    _DisplayType = Localization.GetString( "BannerType.Banner.String", Localization.GlobalResourceFile );
                }
                else if( bannerType == BannerType.MicroButton )
                {
                    _DisplayType = Localization.GetString( "BannerType.MicroButton.String", Localization.GlobalResourceFile );
                }
                else if( bannerType == BannerType.Button )
                {
                    _DisplayType = Localization.GetString( "BannerType.Button.String", Localization.GlobalResourceFile );
                }
                else if( bannerType == BannerType.Block )
                {
                    _DisplayType = Localization.GetString( "BannerType.Block.String", Localization.GlobalResourceFile );
                }
                else if( bannerType == BannerType.Skyscraper )
                {
                    _DisplayType = Localization.GetString( "BannerType.Skyscraper.String", Localization.GlobalResourceFile );
                }
                else if( bannerType == BannerType.Text )
                {
                    _DisplayType = Localization.GetString( "BannerType.Text.String", Localization.GlobalResourceFile );
                }
                else if( bannerType == BannerType.Script )
                {
                    _DisplayType = Localization.GetString( "BannerType.Script.String", Localization.GlobalResourceFile );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return _DisplayType;
        }

        /// <summary>
        /// FormatURL correctly formats the Url (adding a key/Value pair)
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="strKeyName">The name of the key to add</param>
        /// <param name="strKeyValue">The value to add</param>
        /// <returns>The correctly formatted url</returns>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public string FormatURL( string strKeyName, string strKeyValue )
        {
            return EditUrl( strKeyName, strKeyValue, "Banner", "VendorId=" + VendorID.ToString() );
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        internal void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ! Null.IsNull( VendorID ) )
                {
                    BindData();
                }
                else
                {
                    this.Visible = false;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                if( ( Request.QueryString["VendorID"] != null ) )
                {
                    VendorID = int.Parse( Request.QueryString["VendorID"] );
                }
                else
                {
                    VendorID = Null.NullInteger;
                }
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl( "VendorID", VendorID.ToString(), "Banner" ), false, SecurityAccessLevel.Admin, Null.IsNull( VendorID ) == false, false );
                return actions;
            }
        }
    }
}