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
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Search;

namespace DotNetNuke.Common.Utilities
{
    /// <summary>
    /// The LinkClick Page processes links
    /// </summary>
    public partial class RSS : PageBase
    {
        protected Label XML;

        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;

                int intPortalId = PortalSettings.PortalId;
                int intTabId = Null.NullInteger;
                int intModuleId = Null.NullInteger;

                if( Request.QueryString["tabid"] != null )
                {
                    intTabId = Convert.ToInt32( Request.QueryString["tabid"] );
                }
                if( Request.QueryString["moduleid"] != null )
                {
                    intModuleId = Convert.ToInt32( Request.QueryString["moduleid"] );
                }

                Response.Write( BuildRSS( intPortalId, intTabId, intModuleId ) );
            }
            catch( Exception )
            {
            }
        }

        /// <summary>
        /// Builds RSS Channel
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// ---Corrected link elment to use prefix 'http://' & current request context.
        /// </remarks>
        /// <history>Sat, 26 Feb 2005 Phil Guerra
        /// </history>
        private string BuildRSS( int PortalId, int TabId, int ModuleId )
        {
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule;

            StringBuilder sb = new StringBuilder( 1024 );

            // build header
            sb.Append( "<?xml version=\"1.0\" ?>" + "\r\n" );
            sb.Append( "<rss version=\"2.0\"" );
            sb.Append( " xmlns:dc=\"http://purl.org/dc/elements/1.1/\">" + "\r\n" );
            // build channel
            sb.Append( WriteElement( "channel", 1 ) );
            sb.Append( WriteElement( "title", PortalSettings.PortalName, 2 ) );
            // pmg - This line was updated to correct the link item.
            // We are making the assumption that the current context of the
            // request has the desired link to the Portal.  Even though there
            // may be more than 1 portalAlias, this should be correct. If not,
            // we'll have to revisit.
            // sb.Append(WriteElement("link", Request.Url.Host, 2))
            sb.Append(WriteElement("link", Globals.AddHTTP(Globals.GetDomainName(HttpContext.Current.Request)), 2));
            if( PortalSettings.Description != "" )
            {
                sb.Append( WriteElement( "description", PortalSettings.Description, 2 ) );
            }
            else
            {
                sb.Append( WriteElement( "description", PortalSettings.PortalName, 2 ) );
            }
            sb.Append( WriteElement( "language", PortalSettings.DefaultLanguage, 2 ) );
            sb.Append( WriteElement( "copyright", PortalSettings.FooterText, 2 ) );
            sb.Append( WriteElement( "webMaster", PortalSettings.Email, 2 ) );
            // build items
            SearchResultsInfoCollection objResults = SearchDataStoreProvider.Instance().GetSearchItems( PortalId, TabId, ModuleId );
            SearchResultsInfo objResult;
            foreach( SearchResultsInfo tempLoopVar_objResult in objResults )
            {
                objResult = tempLoopVar_objResult;
                if( PortalSecurity.IsInRoles( PortalSettings.ActiveTab.AuthorizedRoles ) )
                {
                    if( PortalSettings.ActiveTab.StartDate < DateTime.Now && PortalSettings.ActiveTab.EndDate > DateTime.Now )
                    {
                        objModule = objModules.GetModule( objResult.ModuleId, objResult.TabId );
                        if( objModule.DisplaySyndicate == true && objModule.IsDeleted == false )
                        {
                            if( PortalSecurity.IsInRoles( objModule.AuthorizedViewRoles ) == true )
                            {
                                if( Convert.ToDateTime( objModule.StartDate == Null.NullDate ? DateTime.MinValue : objModule.StartDate ) < DateTime.Now && Convert.ToDateTime( objModule.EndDate == Null.NullDate ? DateTime.MaxValue : objModule.EndDate ) > DateTime.Now )
                                {
                                    sb.Append( BuildItem( objResult, 2 ) );
                                }
                            }
                        }
                    }
                }
            }

            // close document
            sb.Append( WriteElement( "/channel", 1 ) );
            sb.Append( "</rss>" );

            return sb.ToString();
        }

        /// <summary>
        /// Builds RSS Item
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// ---Corrected pubDate elment name.
        /// ---Corrected formatting of pubDate value to RFC-822 spec.
        /// ---Correction for guid element '&' replaced with '?' to correct linkage issues.
        /// </remarks>
        /// <history>Sat, 29 Jan 2005 Phil Guerra
        /// </history>
        private string BuildItem( SearchResultsInfo objResult, int Indent )
        {
            StringBuilder sb = new StringBuilder( 1024 );

            string URL = Globals.NavigateURL(objResult.TabId);
            if( URL.IndexOf( Request.Url.Host ) == - 1 )
            {
                URL = Globals.AddHTTP(Request.Url.Host) + URL;
            }

            sb.Append( WriteElement( "item", Indent ) );
            sb.Append( WriteElement( "title", objResult.Title, Indent + 1 ) );
            sb.Append( WriteElement( "description", objResult.Description, Indent + 1 ) );
            sb.Append( WriteElement( "link", URL, Indent + 1 ) );
            sb.Append( WriteElement( "dc:creator", objResult.AuthorName, Indent + 1 ) );
            // Commented out <author> element - RSSv2.0 specs call for an email used here,
            // not just a simple name.
            // sb.Append(WriteElement("author", objResult.AuthorName, Indent + 1))
            sb.Append( WriteElement( "pubDate", objResult.PubDate.ToUniversalTime().ToString( "r" ), Indent + 1 ) );
            sb.Append( WriteElement( "guid", URL + Convert.ToString( objResult.Guid != "" ? "?" + objResult.Guid : "" ), Indent + 1 ) );
            sb.Append( WriteElement( "/item", Indent ) );

            return sb.ToString();
        }

        private string WriteElement( string Element, int Indent )
        {
            int InputLength = Element.Trim().Length + 20;
            StringBuilder sb = new StringBuilder( InputLength );
            sb.Append( "\r\n".PadRight( Indent + 2, '\t' ) );
            sb.Append( "<" ).Append( Element ).Append( ">" );
            return sb.ToString();
        }

        private string WriteElement( string Element, string ElementValue, int Indent )
        {
            int InputLength = Element.Trim().Length + ElementValue.Trim().Length + 20;
            StringBuilder sb = new StringBuilder( InputLength );
            sb.Append( "\r\n".PadRight( Indent + 2, '\t' ) );
            sb.Append( "<" ).Append( Element ).Append( ">" );
            sb.Append( CleanXmlString( ElementValue ) );
            sb.Append( "</" ).Append( Element ).Append( ">" );
            return sb.ToString();
        }

        private string CleanXmlString( string XmlString )
        {
            XmlString = XmlString.Replace( "&", "&amp;" );
            XmlString = XmlString.Replace( "<", "&lt;" );
            XmlString = XmlString.Replace( ">", "&gt;" );
            return XmlString;
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