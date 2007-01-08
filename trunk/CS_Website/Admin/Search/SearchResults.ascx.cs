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
using System.Data;
using System.Diagnostics;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Search;

namespace DotNetNuke.Modules.SearchResults
{
    /// Namespace:  DotNetNuke.Modules.SearchResults
    /// Project:    DotNetNuke.SearchResults
    /// Class:      SearchResults
    /// <summary>
    /// The SearchResults Class provides the UI for displaying the Search Results
    /// </summary>
    /// <history>
    ///		[cnurse]	11/11/2004	Improved Formatting of results, and moved Search Options
    ///                             to Settings
    ///     [cnurse]    12/13/2004  Switched to using a DataGrid for Search Results
    ///     [cnurse]    01/04/2005  Modified so "Nos" stay in order
    /// </history>
    public partial class SearchResults : PortalModuleBase
    {
        private string _searchQuery;

        /// <summary>
        /// BindData binds the Search Results to the Grid
        /// </summary>
        /// <history>
        /// 	[cnurse]	12/13/2004	created
        /// </history>
        private void BindData()
        {
            SearchResultsInfoCollection Results = SearchDataStoreProvider.Instance().GetSearchResults( PortalId, _searchQuery );

            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn( "TabId" );
            dt.Columns.Add( new DataColumn( "TabId", typeof( Int32 ) ) );
            dt.Columns.Add( new DataColumn( "Guid", typeof( String ) ) );
            dt.Columns.Add( new DataColumn( "Title", typeof( String ) ) );
            dt.Columns.Add( new DataColumn( "Relevance", typeof( Int32 ) ) );
            dt.Columns.Add( new DataColumn( "Description", typeof( String ) ) );
            dt.Columns.Add( new DataColumn( "PubDate", typeof( DateTime ) ) );

            //Get the maximum items to display
            int maxItems = 0;
            if( Convert.ToString( Settings["maxresults"] ) != "" )
            {
                maxItems = int.Parse( Convert.ToString( Settings["maxresults"] ) );
            }
            else
            {
                maxItems = Results.Count;
            }
            if( Results.Count < maxItems || maxItems < 1 )
            {
                maxItems = Results.Count;
            }

            //Get the items/page to display
            int itemsPage = 10;
            if( Convert.ToString( Settings["perpage"] ) != "" )
            {
                itemsPage = int.Parse( Convert.ToString( Settings["perpage"] ) );
            }

            //Get the titlelength/descriptionlength
            int titleLength = 0;
            if( Convert.ToString( Settings["titlelength"] ) != "" )
            {
                titleLength = int.Parse( Convert.ToString( Settings["titlelength"] ) );
            }
            int descLength = 0;
            if( Convert.ToString( Settings["descriptionlength"] ) != "" )
            {
                descLength = int.Parse( Convert.ToString( Settings["descriptionlength"] ) );
            }

            int i = 0;
            SearchResultsInfo ResultItem;
            for( i = 0; i <= maxItems - 1; i++ )
            {
                ResultItem = Results[i];
                DataRow dr = dt.NewRow();
                dr["TabId"] = ResultItem.TabId;
                dr["Guid"] = ResultItem.Guid;
                if( titleLength > 0 && titleLength < ResultItem.Title.Length )
                {
                    dr["Title"] = ResultItem.Title.Substring( 0, titleLength );
                }
                else
                {
                    dr["Title"] = ResultItem.Title;
                }
                dr["Relevance"] = ResultItem.Relevance;
                if( descLength > 0 && descLength < ResultItem.Description.Length )
                {
                    dr["Description"] = ResultItem.Description.Substring( 0, descLength );
                }
                else
                {
                    dr["Description"] = ResultItem.Description;
                }
                dr["PubDate"] = ResultItem.PubDate;
                dt.Rows.Add( dr );
            }

            //Bind Search Results Grid
            DataView dv = new DataView( dt );
            dv.Sort = "Relevance DESC";
            dgResults.PageSize = itemsPage;
            dgResults.DataSource = dv;
            dgResults.DataBind();

            if( Results.Count == 0 )
            {
                dgResults.Visible = false;
            }
            if( Results.Count <= dgResults.PageSize )
            {
                dgResults.PagerStyle.Visible = false;
            }
            else
            {
                dgResults.PagerStyle.Visible = true;
            }
        }

        /// <summary>
        /// FormatDate displays the publication Date
        /// </summary>
        /// <param name="pubDate">The publication Date</param>
        /// <returns>The formatted date</returns>
        /// <history>
        /// 	[cnurse]	11/11/2004	created
        /// </history>
        public string FormatDate( DateTime pubDate )
        {
            return pubDate.ToString();
        }

        /// <summary>
        /// FormatRelevance displays the relevance value
        /// </summary>
        /// <returns>The formatted date</returns>
        /// <history>
        /// 	[cnurse]	11/12/2004	created
        /// </history>
        public string FormatRelevance( int relevance )
        {
            return Localization.GetString( "Relevance", this.LocalResourceFile ) + relevance.ToString();
        }

        /// <summary>
        /// FormatURL the correctly formatted url to the Search Result
        /// </summary>
        /// <param name="TabID">The Id of the Tab where the content is located</param>
        /// <param name="Link">The module provided querystring to access the correct content</param>
        /// <returns>The formatted url</returns>
        /// <history>
        /// 	[cnurse]	11/11/2004	created
        /// </history>
        public string FormatURL( int TabID, string Link )
        {
            string strURL;

            if( Link == "" )
            {
                strURL = Globals.NavigateURL( TabID );
            }
            else
            {
                strURL = Globals.NavigateURL( TabID, "", Link );
            }

            return strURL;
        }

        /// <summary>
        /// ShowDescription determines whether the description should be shown
        /// </summary>
        /// <returns>True or False string</returns>
        /// <history>
        /// 	[cnurse]	12/13/2004	created
        /// </history>
        public string ShowDescription()
        {
            string strShow;

            if( Convert.ToString( Settings["showdescription"] ) != "" )
            {
                if( Convert.ToString( Settings["showdescription"] ) == "Y" )
                {
                    strShow = "True";
                }
                else
                {
                    strShow = "False";
                }
            }
            else
            {
                strShow = "True";
            }

            return strShow;
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <history>
        /// 	[cnurse]	11/11/2004	documented
        ///     [cnurse]    12/13/2004  Switched to using a DataGrid for Search Results
        /// </history>
        protected void Page_Load( object sender, EventArgs e )
        {
            if( Request.Params["Search"] != null )
            {
                _searchQuery = Request.Params["Search"].ToString();
            }
            else
            {
                _searchQuery = "";
            }

            if( _searchQuery.Length > 0 )
            {
                if( ! Page.IsPostBack )
                {
                    BindData();
                }
            }
        }

        /// <summary>
        /// dgResults_PageIndexChanged runs when one of the Page buttons is clicked
        /// </summary>
        /// <history>
        ///     [cnurse]    12/13/2004  created
        /// </history>
        protected void dgResults_PageIndexChanged( object source, DataGridPageChangedEventArgs e )
        {
            dgResults.CurrentPageIndex = e.NewPageIndex;
            BindData();
        }


    }
}