using System;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using Microsoft.VisualBasic;
using Calendar=DotNetNuke.Common.Utilities.Calendar;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.UI.UserControls
{
    public abstract class URLTrackingControl : UserControlBase
    {
        protected Label lblURL;
        protected Label lblCreatedDate;

        protected Panel pnlTrack;
        protected Label lblTrackingURL;
        protected Label lblClicks;
        protected Label lblLastClick;

        protected Panel pnlLog;
        protected TextBox txtStartDate;
        protected TextBox txtEndDate;
        protected HyperLink cmdStartCalendar;
        protected HyperLink cmdEndCalendar;
        protected CompareValidator valStartDate;
        protected CompareValidator valEndDate;
        protected LinkButton cmdDisplay;
        protected DataGrid grdLog;
        protected Label lblLogURL;

        protected Label Label1;
        protected Label Label2;
        protected Label Label3;
        protected Label Label4;
        protected Label Label5;
        protected Label Label6;
        protected Label Label7;

        private string _URL = "";
        private string _FormattedURL = "";
        private string _TrackingURL = "";
        private int _ModuleID = -2;
        private string _localResourceFile;

        public URLTrackingControl()
        {
            base.Load += new EventHandler( this.Page_Load );
            this.cmdDisplay.Click += new EventHandler(cmdDisplay_Click);
            this._URL = "";
            this._FormattedURL = "";
            this._TrackingURL = "";
            this._ModuleID = -2;
        }

        public string FormattedURL
        {
            get
            {
                return this._FormattedURL;
            }
            set
            {
                this._FormattedURL = value;
            }
        }

        public string LocalResourceFile
        {
            get
            {
                string fileRoot;

                if( _localResourceFile == "" )
                {
                    fileRoot = this.TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/URLTrackingControl.ascx";
                }
                else
                {
                    fileRoot = _localResourceFile;
                }
                return fileRoot;
            }
            set
            {
                this._localResourceFile = value;
            }
        }

        public int ModuleID
        {
            get
            {
                int returnValue;
                returnValue = _ModuleID;
                if( returnValue == -2 )
                {
                    if( Request.QueryString["mid"] != null )
                    {
                        returnValue = int.Parse( Request.QueryString["mid"] );
                    }
                }
                return returnValue;
            }
            set
            {
                this._ModuleID = value;
            }
        }

        public string TrackingURL
        {
            get
            {
                return this._TrackingURL;
            }
            set
            {
                this._TrackingURL = value;
            }
        }

        public string URL
        {
            get
            {
                return this._URL;
            }
            set
            {
                this._URL = value;
            }
        }

        //TODO move into aspx page.
        protected void cmdDisplay_Click( object sender, EventArgs e )
        {
            try
            {
                string strStartDate = txtStartDate.Text;
                if( strStartDate != "" )
                {
                    strStartDate = strStartDate + " 00:00";
                }

                string strEndDate = txtEndDate.Text;
                if( strEndDate != "" )
                {
                    strEndDate = strEndDate + " 23:59";
                }

                UrlController objUrls = new UrlController();
                //localize datagrid
                Localization.LocalizeDataGrid( ref grdLog, this.LocalResourceFile );
                grdLog.DataSource = objUrls.GetUrlLog( PortalSettings.PortalId, lblLogURL.Text, ModuleID, Convert.ToDateTime( strStartDate ), Convert.ToDateTime( strEndDate ) );
                grdLog.DataBind();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// The Page_Load server event handler on this page is used to populate the role information for the page
        /// </summary>
        private void Page_Load( object sender, EventArgs e )
        {
            try
            {
                //this needs to execute always to the client script code is registred in InvokePopupCal
                cmdStartCalendar.NavigateUrl = Calendar.InvokePopupCal( txtStartDate );
                cmdEndCalendar.NavigateUrl = Calendar.InvokePopupCal( txtEndDate );

                if( !Page.IsPostBack )
                {
                    if( _URL != "" )
                    {
                        lblLogURL.Text = URL; // saved for loading Log grid

                        TabType URLType = Globals.GetURLType( _URL );
                        if( URLType == TabType.File && _URL.ToLower().StartsWith( "fileid=" ) == false )
                        {
                            // to handle legacy scenarios before the introduction of the FileServerHandler
                            FileController objFiles = new FileController();
                            lblLogURL.Text = "FileID=" + objFiles.ConvertFilePathToFileId( _URL, PortalSettings.PortalId ).ToString();
                        }

                        UrlController objUrls = new UrlController();
                        UrlTrackingInfo objUrlTracking = objUrls.GetUrlTracking( PortalSettings.PortalId, lblLogURL.Text, ModuleID );
                        if( objUrlTracking != null )
                        {
                            if( _FormattedURL == "" )
                            {
                                if( !URL.StartsWith( "http" ) && !URL.StartsWith( "mailto" ) )
                                {
                                    lblURL.Text = Globals.AddHTTP( Request.Url.Host );
                                }
                                lblURL.Text += Globals.LinkClick( URL, PortalSettings.ActiveTab.TabID, ModuleID, false );
                            }
                            else
                            {
                                lblURL.Text = _FormattedURL;
                            }
                            lblCreatedDate.Text = objUrlTracking.CreatedDate.ToString();

                            if( objUrlTracking.TrackClicks )
                            {
                                pnlTrack.Visible = true;
                                if( _TrackingURL == "" )
                                {
                                    if( !URL.StartsWith( "http" ) )
                                    {
                                        lblTrackingURL.Text = Globals.AddHTTP( Request.Url.Host );
                                    }
                                    lblTrackingURL.Text += Globals.LinkClick( URL, PortalSettings.ActiveTab.TabID, ModuleID, objUrlTracking.TrackClicks );
                                }
                                else
                                {
                                    lblTrackingURL.Text = _TrackingURL;
                                }
                                lblClicks.Text = objUrlTracking.Clicks.ToString();
                                if( !Null.IsNull( objUrlTracking.LastClick ) )
                                {
                                    lblLastClick.Text = objUrlTracking.LastClick.ToString();
                                }
                            }

                            if( objUrlTracking.LogActivity )
                            {
                                pnlLog.Visible = true;

                                txtStartDate.Text = DateAndTime.DateAdd( DateInterval.Day, -6, DateTime.Today ).ToShortDateString();
                                txtEndDate.Text = DateAndTime.DateAdd( DateInterval.Day, 1, DateTime.Today ).ToShortDateString();
                            }
                        }
                    }
                    else
                    {
                        this.Visible = false;
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}