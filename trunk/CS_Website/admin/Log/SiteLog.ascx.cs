#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
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
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.SiteLog;
using DotNetNuke.UI.Skins.Controls;
using Microsoft.VisualBasic;
using Calendar=DotNetNuke.Common.Utilities.Calendar;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.SiteLog
{
    /// <summary>
    /// The SiteLog PortalModuleBase is used to display Logs for the Site
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/15/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class SiteLog : PortalModuleBase
    {
        //Log Properties
        protected CompareValidator valStartDate;
        protected CompareValidator valEndDate;

        //Log Display

        //Tasks

        /// <summary>
        /// BindData binds the controls to the Data
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/15/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        private void BindData()
        {
            string strPortalAlias;

            strPortalAlias = Globals.GetPortalDomainName( PortalAlias.HTTPAlias, Request, false );
            if( strPortalAlias.IndexOf("/") != 0 ) // child portal
            {
                strPortalAlias = strPortalAlias.Substring( 0, strPortalAlias.LastIndexOf( "/" ) - 1 );
            }

            string strStartDate = txtStartDate.Text;
            DateTime dtStart = DateTime.Parse( strStartDate );
            if( strStartDate != "" )
            {
                strStartDate = strStartDate + " 00:00";
            }

            string strEndDate = txtEndDate.Text;
            DateTime dtEnd = DateTime.Parse( strEndDate );
            if( strEndDate != "" )
            {
                strEndDate = strEndDate + " 23:59";
            }

            UserController objUsers = new UserController();
            UserInfo objUser;
            if( cboReportType.SelectedItem.Value == "10" )
            {
                //User Registrations By Date

                ArrayList arrUsers = UserController.GetUsers( PortalId, false );
                DataTable dt = new DataTable();
                DataRow dr;

                dt.Columns.Add( new DataColumn( "Full Name", typeof( string ) ) );
                dt.Columns.Add( new DataColumn( "User Name", typeof( string ) ) );
                dt.Columns.Add( new DataColumn( "Date Registered", typeof( DateTime ) ) );

                foreach( UserInfo tempLoopVar_objUser in arrUsers )
                {
                    objUser = tempLoopVar_objUser;
                    if( objUser.Membership.CreatedDate >= dtStart && objUser.Membership.CreatedDate <= dtEnd && objUser.IsSuperUser == false )
                    {
                        dr = dt.NewRow();

                        dr["Date Registered"] = objUser.Membership.CreatedDate;
                        dr["Full Name"] = objUser.Profile.FullName;
                        dr["User Name"] = objUser.Username;

                        dt.Rows.Add( dr );
                    }
                }

                DataView dv = new DataView( dt );
                dv.Sort = "Date Registered DESC";
                grdLog.DataSource = dv;
                grdLog.DataBind();
            }
            else if( cboReportType.SelectedItem.Value == "11" )
            {
                //User Registrations By Country

                ArrayList arrUsers = UserController.GetUsers( PortalId, false );
                DataTable dt = new DataTable();
                DataRow dr;

                dt.Columns.Add( new DataColumn( "Full Name", typeof( string ) ) );
                dt.Columns.Add( new DataColumn( "User Name", typeof( string ) ) );
                dt.Columns.Add( new DataColumn( "Country", typeof( string ) ) );

                foreach( UserInfo tempLoopVar_objUser in arrUsers )
                {
                    objUser = tempLoopVar_objUser;
                    if( objUser.Membership.CreatedDate >= dtStart && objUser.Membership.CreatedDate <= dtEnd && objUser.IsSuperUser == false )
                    {
                        dr = dt.NewRow();

                        dr["Country"] = objUser.Profile.Country;
                        dr["Full Name"] = objUser.Profile.FullName;
                        dr["User Name"] = objUser.Username;

                        dt.Rows.Add( dr );
                    }
                }

                DataView dv = new DataView( dt );
                dv.Sort = "Country";
                grdLog.DataSource = dv;
                grdLog.DataBind();
            }
            else
            {
                SiteLogController objSiteLog = new SiteLogController();
                IDataReader dr = objSiteLog.GetSiteLog( PortalId, strPortalAlias, Convert.ToInt32( cboReportType.SelectedItem.Value ), Convert.ToDateTime( strStartDate ), Convert.ToDateTime( strEndDate ) );
                grdLog.DataSource = dr; // we are using a DataReader here because the resultset returned by GetSiteLog varies based on the report type selected and therefore does not conform to a static business object
                grdLog.DataBind();
                dr.Close();
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/15/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                //this needs to execute always to the client script code is registred in InvokePopupCal
                cmdStartCalendar.NavigateUrl = Calendar.InvokePopupCal( txtStartDate );
                cmdEndCalendar.NavigateUrl = Calendar.InvokePopupCal( txtEndDate );

                // If this is the first visit to the page, bind the role data to the datalist
                if( Page.IsPostBack == false )
                {
                    string strSiteLogStorage = "D";
                    if( Convert.ToString( Globals.HostSettings["SiteLogStorage"] ) != "" )
                    {
                        strSiteLogStorage = Convert.ToString( Globals.HostSettings["SiteLogStorage"] );
                    }
                    if( strSiteLogStorage == "F" )
                    {
                        UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "LogDisabled", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.YellowWarning );
                        cmdDisplay.Visible = false;
                    }
                    else
                    {
                        switch( PortalSettings.SiteLogHistory )
                        {
                            case - 1: // unlimited

                                break;
                            case 0:

                                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "LogDisabled", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.YellowWarning );
                                break;
                            default:

                                UI.Skins.Skin.AddModuleMessage( this, string.Format( Localization.GetString( "LogHistory", this.LocalResourceFile ), PortalSettings.SiteLogHistory.ToString() ), ModuleMessage.ModuleMessageType.YellowWarning );
                                break;
                        }
                        cmdDisplay.Visible = true;
                    }

                    ListController ctlList = new ListController();
                    ListEntryInfoCollection colSiteLogReports = ctlList.GetListEntryInfoCollection( "Site Log Reports" );

                    cboReportType.DataSource = colSiteLogReports;
                    cboReportType.DataBind();
                    cboReportType.SelectedIndex = 0;

                    txtStartDate.Text = DateAndTime.DateAdd( DateInterval.Day, - 6, DateTime.Today ).ToShortDateString();
                    txtEndDate.Text = DateAndTime.DateAdd( DateInterval.Day, 1, DateTime.Today ).ToShortDateString();

                    // Store URL Referrer to return to portal
                    if( Request.UrlReferrer != null )
                    {
                        if( Request.UrlReferrer.AbsoluteUri == Request.Url.AbsoluteUri )
                        {
                            ViewState["UrlReferrer"] = "";
                        }
                        else
                        {
                            ViewState["UrlReferrer"] = Convert.ToString( Request.UrlReferrer );
                        }
                    }
                    else
                    {
                        ViewState["UrlReferrer"] = "";
                    }
                }

                if( Convert.ToString( ViewState["UrlReferrer"] ) == "" )
                {
                    cmdCancel.Visible = false;
                }
                else
                {
                    cmdCancel.Visible = true;
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
        /// 	[cnurse]	9/15/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdCancel_Click( object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Convert.ToString( ViewState["UrlReferrer"] ) );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdDisplay_Click runs when the Display Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/15/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdDisplay_Click( object sender, EventArgs e )
        {
            try
            {
                BindData();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        
        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
    }
}