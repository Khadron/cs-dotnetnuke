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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.WebControls
{
    [ToolboxData( "<{0}:PagingControl runat=server></{0}:PagingControl>" )]
    public class PagingControl : WebControl
    {

        public class PageNumberLinkTemplate : ITemplate
        {
            private PagingControl _PagingControl;
            private static int itemcount;

            static PageNumberLinkTemplate()
            {
                itemcount = 0;
            }

            public PageNumberLinkTemplate( PagingControl ctlPagingControl )
            {
                this._PagingControl = ctlPagingControl;
            }

            private void BindData( object sender, EventArgs e )
            {
                Literal literal = ( (Literal)sender );
                RepeaterItem repeaterItem = ( (RepeaterItem)literal.NamingContainer );
                literal.Text = ( this._PagingControl.GetLink( Convert.ToInt32( DataBinder.Eval( repeaterItem.DataItem, "PageNum" ) ) ) + "&nbsp;&nbsp;" );
            }

            public virtual void InstantiateIn( Control container )
            {
                Literal literal = new Literal();
                literal.DataBinding += new EventHandler( this.BindData );
                container.Controls.Add( literal );
            }
        }


        private string _CSSClassLinkActive;
        private string _CSSClassLinkInactive;
        private string _CSSClassPagingStatus;
        private int _CurrentPage;
        private int _PageSize;
        private string _QuerystringParams;
        private int _TabID;

        private int _TotalRecords;
        protected TableCell cellDisplayLinks;
        protected TableCell cellDisplayStatus;
        protected Repeater PageNumbers;
        protected Table tablePageNumbers;

        private int TotalPages = -1;

        [Bindable( true ), DefaultValueAttribute( "Normal" ), CategoryAttribute( "Behavior" )]
        public string CSSClassLinkActive
        {
            get
            {
                if (_CSSClassLinkActive == "")
                {
                    return "CommandButton";
                }
                else
                {
                    return _CSSClassLinkActive;
                }
            }
            set
            {
                this._CSSClassLinkActive = value;
            }
        }

        [BindableAttribute( true ), DefaultValueAttribute( "CommandButton" ), CategoryAttribute( "Behavior" )]
        public string CSSClassLinkInactive
        {
            get
            {
                if (_CSSClassLinkInactive == "")
                {
                    return "NormalDisabled";
                }
                else
                {
                    return _CSSClassLinkInactive;
                }
            }
            set
            {
                this._CSSClassLinkInactive = value;
            }
        }

        [CategoryAttribute( "Behavior" ), BindableAttribute( true ), DefaultValueAttribute( "Normal" )]
        public string CSSClassPagingStatus
        {
            get
            {
                if (_CSSClassPagingStatus == "")
                {
                    return "Normal";
                }
                else
                {
                    return _CSSClassPagingStatus;
                }
            }
            set
            {
                this._CSSClassPagingStatus = value;
            }
        }

        [BindableAttribute( true ), CategoryAttribute( "Behavior" ), DefaultValueAttribute( 0 )]
        public int CurrentPage
        {
            get
            {
                return this._CurrentPage;
            }
            set
            {
                this._CurrentPage = value;
            }
        }

        [BindableAttribute( true ), DefaultValueAttribute( 0 ), CategoryAttribute( "Behavior" )]
        public int PageSize
        {
            get
            {
                return this._PageSize;
            }
            set
            {
                this._PageSize = value;
            }
        }

        [CategoryAttribute( "Behavior" ), DefaultValueAttribute( "" ), BindableAttribute( true )]
        public string QuerystringParams
        {
            get
            {
                return this._QuerystringParams;
            }
            set
            {
                this._QuerystringParams = value;
            }
        }

        [CategoryAttribute( "Behavior" ), BindableAttribute( true ), DefaultValueAttribute( 0 )]
        public int TabID
        {
            get
            {
                return this._TabID;
            }
            set
            {
                this._TabID = value;
            }
        }

        [CategoryAttribute( "Behavior" ), BindableAttribute( true ), DefaultValueAttribute( 0 )]
        public int TotalRecords
        {
            get
            {
                return this._TotalRecords;
            }
            set
            {
                this._TotalRecords = value;
            }
        }

        public PagingControl()
        {
            this.TotalPages = -1;
        }

        private string CreateURL( string currentPage )
        {
            if (!String.IsNullOrEmpty(QuerystringParams))
            {
                if (!String.IsNullOrEmpty(currentPage))
                {
                    return Globals.NavigateURL(TabID, "", QuerystringParams, "currentpage=" + currentPage);
                }
                else
                {
                    return Globals.NavigateURL(TabID, "", QuerystringParams);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(currentPage))
                {
                    return Globals.NavigateURL(TabID, "", "currentpage=" + currentPage);
                }
                else
                {
                    return Globals.NavigateURL(TabID);
                }
            }
        }

        /// <Summary>GetFirstLink returns the First Page link for paging.</Summary>
        private string GetFirstLink()
        {
            if (CurrentPage > 1 && TotalPages > 0)
            {
                return "<a href=\"" + CreateURL("1") + "\" class=\"" + CSSClassLinkActive + "\">" + Localization.GetString("First", Localization.SharedResourceFile) + "</a>";
            }
            else
            {
                return "<span class=\"" + CSSClassLinkInactive + "\">" + Localization.GetString("First", Localization.SharedResourceFile) + "</span>";
            }
        }

        /// <Summary>GetLastLink returns the Last Page link for paging.</Summary>
        private string GetLastLink()
        {
            if (CurrentPage != TotalPages && TotalPages > 0)
            {
                return "<a href=\"" + CreateURL(TotalPages.ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + Localization.GetString("Last", Localization.SharedResourceFile) + "</a>";
            }
            else
            {
                return "<span class=\"" + CSSClassLinkInactive + "\">" + Localization.GetString("Last", Localization.SharedResourceFile) + "</span>";
            }
        }

        /// <Summary>GetLink returns the page number links for paging.</Summary>
        private string GetLink( int PageNum )
        {
            if (PageNum == CurrentPage)
            {
                return "<span class=\"" + CSSClassLinkInactive + "\">[" + PageNum + "]</span>";
            }
            else
            {
                return "<a href=\"" + CreateURL(PageNum.ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + PageNum + "</a>";
            }
        }

        /// <Summary>
        /// GetNextLink returns the link for the Next Page for paging.
        /// </Summary>
        private string GetNextLink()
        {
            if (CurrentPage != TotalPages && TotalPages > 0)
            {
                return "<a href=\"" + CreateURL((CurrentPage + 1).ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + Localization.GetString("Next", Localization.SharedResourceFile) + "</a>";
            }
            else
            {
                return "<span class=\"" + CSSClassLinkInactive + "\">" + Localization.GetString("Next", Localization.SharedResourceFile) + "</span>";
            }

        }

        /// <Summary>
        /// GetPreviousLink returns the link for the Previous page for paging.
        /// </Summary>
        private string GetPreviousLink()
        {
            if (CurrentPage > 1 && TotalPages > 0)
            {
                return "<a href=\"" + CreateURL((CurrentPage - 1).ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + Localization.GetString("Previous", Localization.SharedResourceFile) + "</a>";
            }
            else
            {
                return "<span class=\"" + CSSClassLinkInactive + "\">" + Localization.GetString("Previous", Localization.SharedResourceFile) + "</span>";
            }
        }

        private void BindPageNumbers( int totalRecords, int recordsPerPage )
        {
            int PageLinksPerPage = 10;
            
            if ( recordsPerPage != 0 && (totalRecords / recordsPerPage) >= 1)
            {
                TotalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalRecords / recordsPerPage)));
            }
            else
            {
                TotalPages = 0;
            }

            if (TotalPages > 0)
            {
                DataTable ht = new DataTable();
                ht.Columns.Add("PageNum");

                int LowNum = 1;
                int HighNum;

                double tmpNum;
                tmpNum = CurrentPage - PageLinksPerPage / 2;
                if (tmpNum < 1)
                {
                    tmpNum = 1;
                }

                if (CurrentPage > (PageLinksPerPage / 2))
                {
                    LowNum = Convert.ToInt32(Math.Floor(tmpNum));
                }

                if (Convert.ToInt32(TotalPages) <= PageLinksPerPage)
                {
                    HighNum = Convert.ToInt32(TotalPages);
                }
                else
                {
                    HighNum = LowNum + PageLinksPerPage - 1;
                }

                if (HighNum > Convert.ToInt32(TotalPages))
                {
                    HighNum = Convert.ToInt32(TotalPages);
                    if (HighNum - LowNum < PageLinksPerPage)
                    {
                        LowNum = HighNum - PageLinksPerPage + 1;
                    }
                }

                if (HighNum > Convert.ToInt32(TotalPages))
                {
                    HighNum = Convert.ToInt32(TotalPages);
                }
                if (LowNum < 1)
                {
                    LowNum = 1;
                }

                int i;
                for (i = LowNum; i <= HighNum; i++)
                {
                    DataRow tmpRow = ht.NewRow();
                    tmpRow["PageNum"] = i;
                    ht.Rows.Add(tmpRow);
                }

                PageNumbers.DataSource = ht;
                PageNumbers.DataBind();
            }
        }

        protected override void CreateChildControls()
        {
            tablePageNumbers = new Table();
            cellDisplayStatus = new TableCell();
            cellDisplayLinks = new TableCell();
            cellDisplayStatus.CssClass = "Normal";
            cellDisplayLinks.CssClass = "Normal";

            if (String.IsNullOrEmpty( CssClass ))
            {
                tablePageNumbers.CssClass = "PagingTable";
                //add some defaults in case the site
                //has a custom css without the PagingTable css
                tablePageNumbers.Width = new Unit("100%");
                tablePageNumbers.BorderStyle = BorderStyle.Solid;
                tablePageNumbers.BorderWidth = new Unit("1px");
                tablePageNumbers.BorderColor = Color.Gray;
            }
            else
            {
                tablePageNumbers.CssClass = this.CssClass;
            }

            int intRowIndex = tablePageNumbers.Rows.Add(new TableRow());

            PageNumbers = new Repeater();
            PageNumberLinkTemplate I = new PageNumberLinkTemplate(this);
            PageNumbers.ItemTemplate = I;
            BindPageNumbers(TotalRecords, PageSize);

            cellDisplayStatus.HorizontalAlign = HorizontalAlign.Left;
            cellDisplayStatus.Width = new Unit("50%");
            cellDisplayLinks.HorizontalAlign = HorizontalAlign.Right;
            cellDisplayLinks.Width = new Unit("50%");
            int intTotalPages = TotalPages;
            if (intTotalPages == 0)
            {
                intTotalPages = 1;
            }

            string str = string.Format(Localization.GetString("Pages"), CurrentPage, intTotalPages);
            LiteralControl lit = new LiteralControl(str);
            cellDisplayStatus.Controls.Add(lit);

            tablePageNumbers.Rows[intRowIndex].Cells.Add(cellDisplayStatus);
            tablePageNumbers.Rows[intRowIndex].Cells.Add(cellDisplayLinks);
        }

        protected override void Render( HtmlTextWriter output )
        {
            if (PageNumbers == null)
            {
                CreateChildControls();
            }

            StringBuilder str = new StringBuilder();

            str.Append(GetFirstLink() + "&nbsp;&nbsp;&nbsp;");
            str.Append(GetPreviousLink() + "&nbsp;&nbsp;&nbsp;");
            StringBuilder result = new StringBuilder(1024);
            PageNumbers.RenderControl(new HtmlTextWriter(new StringWriter(result)));
            str.Append(result.ToString());
            str.Append(GetNextLink() + "&nbsp;&nbsp;&nbsp;");
            str.Append(GetLastLink() + "&nbsp;&nbsp;&nbsp;");
            cellDisplayLinks.Controls.Add(new LiteralControl(str.ToString()));

            tablePageNumbers.RenderControl(output);
        }
    }
}