using System;
using System.Collections;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Personalization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.Modules.Admin.Log
{
    /// Project	 : DotNetNuke
    /// Class	 : LogViewer
    ///
    /// <summary>
    /// Supplies the functionality for viewing the Site Logs
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///   [cnurse] 17/9/2004  Updated for localization, Help and 508. Also
    ///                       consolidated Send Exceptions into one set of
    ///                       controls
    /// </history>
    public partial class LogViewer : PortalModuleBase, IActionable
    {
        private ArrayList arrLogTypeInfo;
        private Hashtable htLogTypeInfo;
        private bool ColorCodingOn = false;
        private bool ColorCodingLegendOn = true;
        private int PageIndex;
        private int intPortalID = - 1;
        private string strLogTypeKey;

        private void BindData()
        {
            btnEmail.Attributes.Add( "onclick", "return CheckExceptions();" );

            if( ColorCodingOn )
            {
                chkColorCoding.Checked = true;
            }
            else
            {
                chkColorCoding.Checked = false;
            }

            if( ColorCodingLegendOn )
            {
                pnlLegend.Visible = true;
            }
            else
            {
                pnlLegend.Visible = false;
            }

            if( UserInfo.IsSuperUser )
            {
                btnClear.Visible = true;
                btnDelete.Visible = true;
                if( Page.IsPostBack && Request.QueryString["PortalID"] != null )
                {
                    ddlPortalid.Items.FindByValue( Request.QueryString["PortalID"] ).Selected = true;
                }
                intPortalID = int.Parse( ddlPortalid.SelectedItem.Value );
            }
            else
            {
                btnClear.Visible = false;
                btnDelete.Visible = false;
                intPortalID = PortalId;
            }

            int TotalRecords = 0;
            int PageSize = Convert.ToInt32( ddlRecordsPerPage.SelectedValue );

            if( Page.IsPostBack && Request.QueryString["LogTypeKey"] != null )
            {
                ddlLogType.Items.FindByValue( Request.QueryString["LogTypeKey"] ).Selected = true;
            }

            strLogTypeKey = ddlLogType.SelectedItem.Value;

            LogInfoArray objLog;
            int CurrentPage = PageIndex;
            if( CurrentPage > 0 )
            {
                CurrentPage--;
            }
            LogController objLogController = new LogController();
            if( intPortalID == - 1 && strLogTypeKey == "*" )
            {
                objLog = objLogController.GetLog( PageSize, CurrentPage, ref TotalRecords );
            }
            else if( intPortalID == - 1 && strLogTypeKey != "*" )
            {
                objLog = objLogController.GetLog( strLogTypeKey, PageSize, CurrentPage, ref TotalRecords );
            }
            else if( intPortalID != - 1 && strLogTypeKey == "*" )
            {
                objLog = objLogController.GetLog( intPortalID, PageSize, CurrentPage, ref TotalRecords );
            }
            else if( intPortalID != - 1 && strLogTypeKey != "*" )
            {
                objLog = objLogController.GetLog( intPortalID, strLogTypeKey, PageSize, CurrentPage, ref TotalRecords );
            }
            else
            {
                objLog = objLogController.GetLog( strLogTypeKey, PageSize, CurrentPage, ref TotalRecords );
            }

            if( objLog.Count > 0 )
            {
                dlLog.Visible = true;
                pnlSendExceptions.Visible = true;
                if( UserInfo.IsSuperUser )
                {
                    btnDelete.Visible = true;
                    btnClear.Visible = true;
                }
                pnlOptions.Visible = true;
                tblInstructions.Visible = true;
                dlLog.DataSource = objLog;
                dlLog.DataBind();

                InitializePaging( ctlPagingControlBottom, TotalRecords, PageSize );
            }
            else
            {
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "NoEntries", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.YellowWarning );
                dlLog.Visible = false;
                pnlSendExceptions.Visible = false;
                btnDelete.Visible = false;
                btnClear.Visible = false;
                pnlLegend.Visible = false;
                tblInstructions.Visible = false;
            }
        }

        private void InitializePaging( PagingControl ctlPagingControl, int TotalRecords, int PageSize )
        {
            ctlPagingControl.TotalRecords = TotalRecords;
            ctlPagingControl.PageSize = PageSize;
            ctlPagingControl.CurrentPage = PageIndex;
            string strQuerystring = "";
            if( ddlRecordsPerPage.SelectedIndex != 0 )
            {
                strQuerystring += "&PageRecords=" + ddlRecordsPerPage.SelectedValue;
            }
            if( intPortalID >= 0 )
            {
                strQuerystring += "&PortalID=" + intPortalID.ToString();
            }
            if( strLogTypeKey != "*" && strLogTypeKey != "" )
            {
                strQuerystring += "&LogTypeKey=" + strLogTypeKey;
            }
            ctlPagingControl.QuerystringParams = strQuerystring;
            ctlPagingControl.TabID = TabId;
        }

        private void BindLogTypeDropDown()
        {
            LogController objLogController = new LogController();
            ArrayList arrLogTypes = objLogController.GetLogTypeInfo();
            arrLogTypes.Sort( new LogTypeSortFriendlyName() );
            ddlLogType.DataTextField = "LogTypeFriendlyName";
            ddlLogType.DataValueField = "LogTypeKey";
            ddlLogType.DataSource = arrLogTypes;
            ddlLogType.DataBind();
            ListItem ddlAllPortals = new ListItem( Localization.GetString( "All" ), "*" );
            ddlLogType.Items.Insert( 0, ddlAllPortals );
        }

        private void BindPortalDropDown()
        {
            
            int i;

            if( UserInfo.IsSuperUser )
            {
                PortalController objPortalController = new PortalController();
                ArrayList arrPortals = objPortalController.GetPortals();
                arrPortals.Sort( new PortalSortTitle() );
                ddlPortalid.DataTextField = "PortalName";
                ddlPortalid.DataValueField = "PortalID";
                ddlPortalid.DataSource = arrPortals;
                ddlPortalid.DataBind();
                ListItem ddlAllPortals = new ListItem( Localization.GetString( "All" ), "-1" );
                ddlPortalid.Items.Insert( 0, ddlAllPortals );
                //check to see if any portalname is empty, otherwise set it to portalid
                for( i = 0; i <= ddlPortalid.Items.Count - 1; i++ )
                {
                    if( ddlPortalid.Items[i].Text.Length == 0 )
                    {
                        ddlPortalid.Items[i].Text = "Portal: " + ddlPortalid.Items[i].Value;
                    }
                }
            }
            else
            {
                plPortalID.Visible = false;
                ddlPortalid.Visible = false;
            }
        }

        private void DeleteSelectedExceptions()
        {
            try
            {
                
                string s = Request.Form["Exception"];
                if( s != null )
                {
                    string[] arrExcPositions = new string[0];
                    if( s.LastIndexOf( "," ) > 0 )
                    {
                        arrExcPositions = s.Split( Convert.ToChar( "," ) );
                    }
                    else if( s.Length > 0 )
                    {
                        arrExcPositions = new string[1];
                        arrExcPositions[0] = s;
                    }

                    LogController objLoggingController = new LogController();

                    int i;
                    int j = arrExcPositions.Length;
                    for( i = 1; i <= arrExcPositions.Length; i++ )
                    {
                        j--;
                        string[] excKey;
                        excKey = arrExcPositions[j].Split( Convert.ToChar( "|" ) );
                        LogInfo objLogInfo = new LogInfo();
                        objLogInfo.LogGUID = excKey[0];
                        objLogInfo.LogFileID = excKey[1];
                        objLoggingController.DeleteLog( objLogInfo );
                    }
                    UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "DeleteSuccess", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.GreenSuccess );
                }
                BindPortalDropDown();
                BindData();
            }
            catch( Exception exc )
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        private XmlDocument GetSelectedExceptions()
        {
            XmlDocument objXML = new XmlDocument();
            try
            {
                
                string s = Request.Form["Exception"];
                if( s != null )
                {
                    string[] arrExcPositions = new string[0];
                    if( s.LastIndexOf( "," ) > 0 )
                    {
                        arrExcPositions = s.Split( Convert.ToChar( "," ) );
                    }
                    else if( s.Length > 0 )
                    {
                        arrExcPositions = new string[1];
                        arrExcPositions[0] = s;
                    }

                    LogController objLoggingController = new LogController();

                    objXML.LoadXml( "<LogEntries></LogEntries>" );

                    int i;
                    int j = arrExcPositions.Length;
                    for( i = 1; i <= arrExcPositions.Length; i++ )
                    {
                        j--;
                        string[] excKey;
                        excKey = arrExcPositions[j].Split( Convert.ToChar( "|" ) );
                        LogInfo objLogInfo = new LogInfo();
                        objLogInfo.LogGUID = excKey[0];
                        objLogInfo.LogFileID = excKey[1];
                        XmlNode objNode;
                        objNode = objXML.ImportNode( ( (XmlNode)objLoggingController.GetSingleLog( objLogInfo, LoggingProvider.ReturnType.XML ) ), true );
                        objXML.DocumentElement.AppendChild( objNode );
                    }
                }
            }
            catch( Exception exc )
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
            return objXML;
        }

        private void SendEmail()
        {
            string strEmail = GetSelectedExceptions().OuterXml;
            string strFromEmailAddress;
            
            if( UserInfo.Email != "" )
            {
                strFromEmailAddress = UserInfo.Email;
            }
            else
            {
                strFromEmailAddress = PortalSettings.Email;
            }
            string ReturnMsg = Mail.SendMail( strFromEmailAddress, txtEmailAddress.Text, "", PortalSettings.PortalName + " Exceptions", Server.HtmlEncode( txtMessage.Text + "\r\n" + strEmail.ToString() ), "", "html", "", "", "", "" );

            if( ReturnMsg == "" )
            {
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "EmailSuccess", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.GreenSuccess );
            }
            else
            {
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "EmailFailure", this.LocalResourceFile ) + ReturnMsg, ModuleMessage.ModuleMessageType.RedError );
            }
            BindData();
        }

        protected LogTypeInfo GetMyLogType( string LogTypeKey )
        {
            LogTypeInfo objLogTypeInfo;
            if( htLogTypeInfo[LogTypeKey] != null )
            {
                objLogTypeInfo = (LogTypeInfo)htLogTypeInfo[LogTypeKey];
                if( ! ColorCodingOn )
                {
                    objLogTypeInfo.LogTypeCSSClass = "Normal";
                }
                return objLogTypeInfo;
            }
            else
            {
                return new LogTypeInfo();
            }
        }

        public string GetPropertiesText( object obj )
        {
            LogInfo objLogInfo = (LogInfo)obj;

            LogProperties objLogProperties = objLogInfo.LogProperties;
            StringBuilder str = new StringBuilder();

            int i;
            for( i = 0; i <= objLogProperties.Count - 1; i++ )
            {
                // display the values in the Panel child controls.
                LogDetailInfo ldi = (LogDetailInfo)objLogProperties[i];
                str.Append( "<b>" + ldi.PropertyName + "</b>: " + Server.HtmlEncode( ldi.PropertyValue ) + "<br>" );
            }
            str.Append( Localization.GetString( "ServerName", this.LocalResourceFile ) + Server.HtmlEncode( objLogInfo.LogServerName ) + "<br>" );
            return str.ToString();
        }

        /// <summary>
        /// The Page_Load runs when the page loads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///   [cnurse] 17/9/2004  Updated for localization, Help and 508
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                

                // If this is the first visit to the page, populate the site data
                if( Page.IsPostBack == false )
                {
                    LogController objLC = new LogController();
                    objLC.PurgeLogBuffer();

                    if( Request.QueryString["CurrentPage"] != null )
                    {
                        PageIndex = Convert.ToInt32( Request.QueryString["CurrentPage"] );
                    }
                    else
                    {
                        PageIndex = 1;
                    }

                    if( Request.QueryString["PageRecords"] != null )
                    {
                        ddlRecordsPerPage.SelectedValue = Request.QueryString["PageRecords"];
                    }

                    BindPortalDropDown();
                    BindLogTypeDropDown();
                    BindData();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void btnClear_Click( Object sender, EventArgs e )
        {
            LogController objLoggingController = new LogController();
            objLoggingController.ClearLog();
            UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "LogCleared", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.GreenSuccess );
            BindPortalDropDown();
            dlLog.Visible = false;
            pnlSendExceptions.Visible = false;
            btnDelete.Visible = false;
            btnClear.Visible = false;
            pnlOptions.Visible = false;
            pnlLegend.Visible = false;
            tblInstructions.Visible = false;
        }

        protected void btnDelete_Click( Object sender, EventArgs e )
        {
            DeleteSelectedExceptions();
        }

        protected void btnEmail_Click( Object sender, EventArgs e )
        {
            switch( optEmailType.SelectedValue )
            {
                case "Email":

                    SendEmail();
                    break;
            }
        }

        protected void chkColorCoding_CheckedChanged( Object sender, EventArgs e )
        {
            int i;
            if( chkColorCoding.Checked )
            {
                i = 1;
                ColorCodingOn = true;
                ColorCodingLegendOn = true;
                Personalization.SetProfile( "LogViewer", "ColorCodingLegend", "1" );
            }
            else
            {
                i = 0;
                ColorCodingOn = false;
                ColorCodingLegendOn = false;
            }
            Personalization.SetProfile( "LogViewer", "ColorCoding", i );
            BindData();
        }

        protected void ddlLogType_SelectedIndexChanged( Object sender, EventArgs e )
        {
            PageIndex = 1;
            BindData();
        }

        protected void ddlPortalID_SelectedIndexChanged( Object sender, EventArgs e )
        {
            PageIndex = 1;
            BindData();
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                if( IsEditable )
                {
                    actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl(), false, SecurityAccessLevel.Host, true, false );
                }
                return actions;
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
            LogController l = new LogController();
            arrLogTypeInfo = l.GetLogTypeInfo();

            htLogTypeInfo = new Hashtable();

            int i;
            for( i = 0; i <= arrLogTypeInfo.Count - 1; i++ )
            {
                LogTypeInfo objLogTypeInfo = (LogTypeInfo)arrLogTypeInfo[i];
                htLogTypeInfo.Add( objLogTypeInfo.LogTypeKey, objLogTypeInfo );
            }

            string ColorCoding;
            ColorCoding = Convert.ToString( Personalization.GetProfile( "LogViewer", "ColorCoding" ) );
            if( ColorCoding == "0" )
            {
                ColorCodingOn = false;
            }
            else if( ColorCoding == "1" )
            {
                ColorCodingOn = true;
            }
            else
            {
                ColorCodingOn = true;
            }

            string ColorCodingLegend;
            ColorCodingLegend = Convert.ToString( Personalization.GetProfile( "LogViewer", "ColorCodingLegend" ) );
            if( ColorCodingLegend == "0" )
            {
                ColorCodingLegendOn = false;
            }
            else if( ColorCodingLegend == "1" )
            {
                ColorCodingLegendOn = true;
            }
            else
            {
                ColorCodingLegendOn = true;
            }
        }

        protected void ddlRecordsPerPage_SelectedIndexChanged( Object sender, EventArgs e )
        {
            PageIndex = 1;
            BindData();
        }
    }
}