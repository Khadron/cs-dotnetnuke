using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.SiteLog;
using DotNetNuke.Services.Vendors;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Utilities;
using DataCache=DotNetNuke.Common.Utilities.DataCache;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Framework
{
    /// <summary>
    ///
    /// </summary>
    /// <history>
    /// 	[sun1]	1/19/2004	Created
    /// </history>
    public partial class DefaultPage : CDefault
    {
        /// <summary>
        /// Property to allow the programmatic assigning of ScrollTop position
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Jon Henning]	3/23/2005	Created
        /// </history>
        public int PageScrollTop
        {
            get
            {
                int result;
                if( ScrollTop.Value.Length > 0 && Int32.TryParse( ScrollTop.Value, out result ) )
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                ScrollTop.Value = value.ToString();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// - Obtain PortalSettings from Current Context
        /// - redirect to a specific tab based on name
        /// - if first time loading this page then reload to avoid caching
        /// - set page title and stylesheet
        /// - check to see if we should show the Assembly Version in Page Title
        /// - set the background image if there is one selected
        /// - set META tags, copyright, keywords and description
        /// </remarks>
        /// <history>
        /// 	[sun1]	1/19/2004	Created
        /// </history>
        private void InitializePage()
        {
            TabController objTabs = new TabController();
            TabInfo objTab;

            // redirect to a specific tab based on name
            if( Request.QueryString["tabname"] != "" )
            {
                string strURL = "";

                objTab = objTabs.GetTabByName( Request.QueryString["TabName"], ( (PortalSettings)HttpContext.Current.Items["PortalSettings"] ).PortalId );
                if( objTab != null )
                {
                    int actualParamCount = 0;
                    string[] elements = new string[Request.QueryString.Count - 1 + 1]; //maximum number of elements
                    for( int intParam = 0; intParam <= Request.QueryString.Count - 1; intParam++ )
                    {
                        switch( Request.QueryString.Keys[intParam].ToLower() )
                        {
                            case "tabid":
                                break;

                            case "tabname":

                                break;
                            default:

                                elements[actualParamCount] = Request.QueryString.Keys[intParam] + "=" + Request.QueryString[intParam];
                                actualParamCount++;
                                break;
                        }
                    }
                    string[] copiedTo = new string[actualParamCount - 1 + 1];
                    elements.CopyTo(copiedTo, 0); //redim to remove blank elements
                    elements = copiedTo;
                    //elements = (string[])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray( (Array)elements, new string[actualParamCount - 1 + 1] ); //redim to remove blank elements

                    Response.Redirect( Globals.NavigateURL( objTab.TabID, Null.NullString, elements ), true );
                }
            }

            if( Request.IsAuthenticated )
            {
                // set client side page caching for authenticated users
                if( Convert.ToString( PortalSettings.HostSettings["AuthenticatedCacheability"] ) != "" )
                {
                    switch( Convert.ToString( PortalSettings.HostSettings["AuthenticatedCacheability"] ) )
                    {
                        case "0":

                            Response.Cache.SetCacheability( HttpCacheability.NoCache );
                            break;
                        case "1":

                            Response.Cache.SetCacheability( HttpCacheability.Private );
                            break;
                        case "2":

                            Response.Cache.SetCacheability( HttpCacheability.Public );
                            break;
                        case "3":

                            Response.Cache.SetCacheability( HttpCacheability.Server );
                            break;
                        case "4":

                            Response.Cache.SetCacheability( HttpCacheability.ServerAndNoCache );
                            break;
                        case "5":

                            Response.Cache.SetCacheability( HttpCacheability.ServerAndPrivate );
                            break;
                    }
                }
                else
                {
                    Response.Cache.SetCacheability( HttpCacheability.ServerAndNoCache );
                }
            }

            // page comment
            if( Globals.GetHashValue( Globals.HostSettings["Copyright"], "Y" ) == "Y" )
            {
                Comment += "\r\n" + "<!--**********************************************************************************-->" + "\r\n" + "<!-- DotNetNuke® - http://www.dotnetnuke.com                                          -->" + "\r\n" + "<!-- Copyright (c) 2002-2006                                                          -->" + "\r\n" + "<!-- by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )   -->" + "\r\n" + "<!--**********************************************************************************-->" + "\r\n";
            }
            Page.Header.Controls.AddAt( 0, new LiteralControl( Comment ) );

            if( PortalSettings.ActiveTab.PageHeadText != Null.NullString )
            {
                Page.Header.Controls.Add( new LiteralControl( PortalSettings.ActiveTab.PageHeadText ) );
            }

            // set page title
            string strTitle = PortalSettings.PortalName;
            foreach( TabInfo tempLoopVar_objTab in PortalSettings.ActiveTab.BreadCrumbs )
            {
                objTab = tempLoopVar_objTab;
                strTitle += " > " + objTab.TabName;
            }
            // show copyright credits?
            if( Globals.GetHashValue( Globals.HostSettings["Copyright"], "Y" ) == "Y" )
            {
                strTitle += " ( DNN " + PortalSettings.Version + " )";
            }
            // tab title override
            if( PortalSettings.ActiveTab.Title != "" )
            {
                strTitle = PortalSettings.ActiveTab.Title;
            }
            this.Title = strTitle;

            //set the background image if there is one selected
            if( this.FindControl( "Body" ) != null )
            {
                if( PortalSettings.BackgroundFile != "" )
                {
                    ( (HtmlGenericControl)this.FindControl( "Body" ) ).Attributes["background"] = PortalSettings.HomeDirectory + PortalSettings.BackgroundFile;
                }
            }

            // META Refresh
            if( PortalSettings.ActiveTab.RefreshInterval > 0 && Request.QueryString["ctl"] == null )
            {
                MetaRefresh.Content = PortalSettings.ActiveTab.RefreshInterval.ToString();
            }
            else
            {
                MetaRefresh.Visible = false;
            }

            // META description
            if( PortalSettings.ActiveTab.Description != "" )
            {
                Description = PortalSettings.ActiveTab.Description;
            }
            else
            {
                Description = PortalSettings.Description;
            }

            // META keywords
            if( PortalSettings.ActiveTab.KeyWords != "" )
            {
                KeyWords = PortalSettings.ActiveTab.KeyWords;
            }
            else
            {
                KeyWords = PortalSettings.KeyWords;
            }
            if( Globals.GetHashValue( Globals.HostSettings["Copyright"], "Y" ) == "Y" )
            {
                KeyWords += ",DotNetNuke,DNN";
            }

            // META copyright
            if( PortalSettings.FooterText != "" )
            {
                
                Copyright = PortalSettings.FooterText;
            }
            else
            {
                Copyright = "Copyright (c) " + DateTime.Now.ToString("yyyy") +" by " + PortalSettings.PortalName;
            }

            // META generator
            if( Globals.GetHashValue( Globals.HostSettings["Copyright"], "Y" ) == "Y" )
            {
                Generator = "DotNetNuke " + PortalSettings.Version;
            }
            else
            {
                Generator = "";
            }

            Page.ClientScript.RegisterClientScriptInclude( "dnncore", ResolveUrl( "~/js/dnncore.js" ) );
        }

        private UserControl LoadSkin( string SkinPath )
        {
            UserControl ctlSkin = null;

            try
            {
                if( SkinPath.ToLower().IndexOf( Globals.ApplicationPath.ToLower() ) != - 1 )
                {
                    SkinPath = SkinPath.Remove( 0, Globals.ApplicationPath.Length );
                }
                ctlSkin = (UserControl)LoadControl( "~" + SkinPath );
                // call databind so that any server logic in the skin is executed
                ctlSkin.DataBind();
            }
            catch( Exception exc )
            {
                // could not load user control
                PageLoadException lex = new PageLoadException( "Unhandled error loading page.", exc );
                if( PortalSecurity.IsInRoles( PortalSettings.AdministratorRoleName ) == true || PortalSecurity.IsInRoles( PortalSettings.ActiveTab.AdministratorRoles.ToString() ) == true )
                {
                    // only display the error to administrators
                    SkinError.Text += "<center>Could Not Load Skin: " + SkinPath + " Error: " + Server.HtmlEncode( exc.Message ) + "</center><br>";
                    SkinError.Visible = true;
                }
                Exceptions.LogException( lex );
            }

            return ctlSkin;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// - manage affiliates
        /// - log visit to site
        /// </remarks>
        /// <history>
        /// 	[sun1]	1/19/2004	Created
        /// </history>
        private void ManageRequest()
        {
            // affiliate processing
            int AffiliateId = - 1;
            if( Request.QueryString["AffiliateId"] != null )
            {                
                if( Int32.TryParse(Request.QueryString["AffiliateId"], out AffiliateId ) )
                {
                    AffiliateId = int.Parse( Request.QueryString["AffiliateId"] );
                    AffiliateController objAffiliates = new AffiliateController();
                    objAffiliates.UpdateAffiliateStats( AffiliateId, 1, 0 );

                    // save the affiliateid for acquisitions
                    if( Request.Cookies["AffiliateId"] == null ) // do not overwrite
                    {
                        HttpCookie objCookie = new HttpCookie( "AffiliateId" );
                        objCookie.Value = AffiliateId.ToString();
                        objCookie.Expires = DateTime.Now.AddYears( 1 ); // persist cookie for one year
                        Response.Cookies.Add( objCookie );
                    }
                }
            }

            // site logging
            if( PortalSettings.SiteLogHistory != 0 )
            {
                // get User ID

                // URL Referrer
                string URLReferrer = "";
                if( Request.UrlReferrer != null )
                {
                    URLReferrer = Request.UrlReferrer.ToString();
                }

                string strSiteLogStorage = "D";
                if( Convert.ToString( Globals.HostSettings["SiteLogStorage"] ) != "" )
                {
                    strSiteLogStorage = Convert.ToString( Globals.HostSettings["SiteLogStorage"] );
                }
                int intSiteLogBuffer = 1;
                if( Convert.ToString( Globals.HostSettings["SiteLogBuffer"] ) != "" )
                {
                    intSiteLogBuffer = int.Parse( Convert.ToString( Globals.HostSettings["SiteLogBuffer"] ) );
                }

                // log visit
                SiteLogController objSiteLogs = new SiteLogController();

                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                objSiteLogs.AddSiteLog( PortalSettings.PortalId, objUserInfo.UserID, URLReferrer, Request.Url.ToString(), Request.UserAgent, Request.UserHostAddress, Request.UserHostName, PortalSettings.ActiveTab.TabID, AffiliateId, intSiteLogBuffer, strSiteLogStorage );
            }
        }

        private void ManageStyleSheets( bool PortalCSS )
        {
            // initialize reference paths to load the cascading style sheets
            string ID;

            Hashtable objCSSCache = (Hashtable)DataCache.GetCache( "CSS" );
            if( objCSSCache == null )
            {
                objCSSCache = new Hashtable();
            }

            if( PortalCSS == false )
            {
                // default style sheet ( required )
                ID = Globals.CreateValidID( Globals.HostPath );
                AddStyleSheet( ID, Globals.HostPath + "default.css" );

                // skin package style sheet
                ID = Globals.CreateValidID( PortalSettings.ActiveTab.SkinPath );
                if( objCSSCache.ContainsKey( ID ) == false )
                {
                    if( File.Exists( Server.MapPath( PortalSettings.ActiveTab.SkinPath ) + "skin.css" ) )
                    {
                        objCSSCache[ID] = PortalSettings.ActiveTab.SkinPath + "skin.css";
                    }
                    else
                    {
                        objCSSCache[ID] = "";
                    }
                    if( Globals.PerformanceSetting != Globals.PerformanceSettings.NoCaching )
                    {
                        DataCache.SetCache( "CSS", objCSSCache );
                    }
                }
                if( objCSSCache[ID].ToString() != "" )
                {
                    AddStyleSheet( ID, objCSSCache[ID].ToString() );
                }

                // skin file style sheet
                ID = Globals.CreateValidID( PortalSettings.ActiveTab.SkinSrc.Replace(".ascx", ".css") );
                if( objCSSCache.ContainsKey( ID ) == false )
                {
                    if( File.Exists( Server.MapPath( PortalSettings.ActiveTab.SkinSrc.Replace(".ascx", ".css")) ))
                    {
                        objCSSCache[ID] = PortalSettings.ActiveTab.SkinSrc.Replace(".ascx", ".css" );
                    }
                    else
                    {
                        objCSSCache[ID] = "";
                    }
                    if( Globals.PerformanceSetting != Globals.PerformanceSettings.NoCaching )
                    {
                        DataCache.SetCache( "CSS", objCSSCache );
                    }
                }
                if( objCSSCache[ID].ToString() != "" )
                {
                    AddStyleSheet( ID, objCSSCache[ID].ToString() );
                }
            }
            else
            {
                // portal style sheet
                ID = Globals.CreateValidID( PortalSettings.HomeDirectory );
                AddStyleSheet( ID, PortalSettings.HomeDirectory + "portal.css" );
            }
        }

        private void ManageFavicon()
        {
            string strFavicon = Convert.ToString( DataCache.GetCache( "FAVICON" + PortalSettings.PortalId.ToString() ) );
            if( strFavicon == "" )
            {
                if( File.Exists( PortalSettings.HomeDirectoryMapPath + "favicon.ico" ) )
                {
                    strFavicon = PortalSettings.HomeDirectory + "favicon.ico";
                    if( Globals.PerformanceSetting != Globals.PerformanceSettings.NoCaching )
                    {
                        DataCache.SetCache( "FAVICON" + PortalSettings.PortalId.ToString(), strFavicon );
                    }
                }
            }
            if( strFavicon != "" )
            {
                HtmlLink objLink = new HtmlLink();
                objLink.Attributes["rel"] = "SHORTCUT ICON";
                objLink.Attributes["href"] = strFavicon;

                Page.Header.Controls.Add( objLink );
            }
        }

        /// <summary>
        /// Contains the functionality to populate the Root aspx page with controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// - obtain PortalSettings from Current Context
        /// - set global page settings.
        /// - initialise reference paths to load the cascading style sheets
        /// - add skin control placeholder.  This holds all the modules and content of the page.
        /// </remarks>
        /// <history>
        /// 	[sun1]	1/19/2004	Created
        ///		[jhenning] 8/24/2005 Added logic to look for post originating from a ClientCallback
        /// </history>
        protected void Page_Init( Object sender, EventArgs e )
        {
            // set global page settings
            InitializePage();

            // load skin control
            UserControl ctlSkin = null;

            // skin preview
            if( Request.QueryString["SkinSrc"] != null )
            {
                PortalSettings.ActiveTab.SkinSrc = SkinController.FormatSkinSrc( Globals.QueryStringDecode( Request.QueryString["SkinSrc"] ) + ".ascx", PortalSettings );
                ctlSkin = LoadSkin( PortalSettings.ActiveTab.SkinSrc );
            }

            // load user skin ( based on cookie )
            if( ctlSkin == null )
            {
                if( Request.Cookies["_SkinSrc" + PortalSettings.PortalId.ToString()] != null )
                {
                    if( Request.Cookies["_SkinSrc" + PortalSettings.PortalId.ToString()].Value != "" )
                    {
                        PortalSettings.ActiveTab.SkinSrc = SkinController.FormatSkinSrc( Request.Cookies["_SkinSrc" + PortalSettings.PortalId.ToString()].Value + ".ascx", PortalSettings );
                        ctlSkin = LoadSkin( PortalSettings.ActiveTab.SkinSrc );
                    }
                }
            }

            // load assigned skin
            if( ctlSkin == null )
            {
                if( Globals.IsAdminSkin( PortalSettings.ActiveTab.IsAdminTab ) )
                {
                    SkinInfo objSkin;
                    objSkin = SkinController.GetSkin( SkinInfo.RootSkin, PortalSettings.PortalId, SkinType.Admin );
                    if( objSkin != null )
                    {
                        PortalSettings.ActiveTab.SkinSrc = objSkin.SkinSrc;
                    }
                    else
                    {
                        PortalSettings.ActiveTab.SkinSrc = "";
                    }
                }

                if( PortalSettings.ActiveTab.SkinSrc != "" )
                {
                    PortalSettings.ActiveTab.SkinSrc = SkinController.FormatSkinSrc( PortalSettings.ActiveTab.SkinSrc, PortalSettings );
                    ctlSkin = LoadSkin( PortalSettings.ActiveTab.SkinSrc );
                }
            }

            // error loading skin - load default
            if( ctlSkin == null )
            {
                // could not load skin control - load default skin
                if( Globals.IsAdminSkin( PortalSettings.ActiveTab.IsAdminTab ) )
                {
                    PortalSettings.ActiveTab.SkinSrc = Globals.HostPath + SkinInfo.RootSkin + Globals.glbDefaultSkinFolder + Globals.glbDefaultAdminSkin;
                }
                else
                {
                    PortalSettings.ActiveTab.SkinSrc = Globals.HostPath + SkinInfo.RootSkin + Globals.glbDefaultSkinFolder + Globals.glbDefaultSkin;
                }
                ctlSkin = LoadSkin( PortalSettings.ActiveTab.SkinSrc );
            }

            // set skin path
            PortalSettings.ActiveTab.SkinPath = SkinController.FormatSkinPath( PortalSettings.ActiveTab.SkinSrc );

            // set skin id to an explicit short name to reduce page payload and make it standards compliant
            ctlSkin.ID = "dnn";

            // add CSS links
            ManageStyleSheets( false );

            // add skin to page
            SkinPlaceHolder.Controls.Add( ctlSkin );

            // add CSS links
            ManageStyleSheets( true );

            // add Favicon
            ManageFavicon();

            // ClientCallback Logic
            ClientAPI.HandleClientAPICallbackEvent( this );
        }

        /// <summary>
        /// Initialize the Scrolltop html control which controls the open / closed nature of each module
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[sun1]	1/19/2004	Created
        ///		[jhenning] 3/23/2005 No longer passing in parameter to __dnn_setScrollTop, instead pulling value from textbox on client
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            HtmlInputHidden Scrolltop = (HtmlInputHidden)Page.FindControl( "ScrollTop" );
            if( Scrolltop.Value != "" )
            {
                DNNClientAPI.AddBodyOnloadEventHandler( Page, "__dnn_setScrollTop();" );
                Scrolltop.Value = Scrolltop.Value;
            }
        }

        protected void Page_PreRender( object sender, EventArgs e )
        {
            // process the current request
            if( !Globals.IsAdminControl() )
            {
                ManageRequest();
            }

            //Set the Head tags
            Page.Header.Title = Title;
            
            MetaGenerator.Content = Generator;
            MetaAuthor.Content = PortalSettings.PortalName;
            MetaCopyright.Content = Copyright;
            MetaKeywords.Content = KeyWords;
            MetaDescription.Content = Description;
        }
    }
}