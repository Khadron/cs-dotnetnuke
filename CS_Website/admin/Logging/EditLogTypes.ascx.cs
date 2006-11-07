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
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.UserControls;

namespace DotNetNuke.Modules.Admin.Log
{
    /// Project	 : DotNetNuke
    /// Class	 : EditLogTypes
    ///
    /// <summary>
    /// Manage the Log Types for the portal
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///   [cnurse] 17/9/2004  Updated for localization, Help and 508.
    /// </history>
    public partial class EditLogTypes : PortalModuleBase, IActionable
    {
        //DataGrid List

        //Logging Settings

        //Email Notification Settings
        protected LabelControl plThresholdNotificationTime;
        protected LabelControl plThresholdNotificationTimeType;

        //tasks

        private void BindDetailData()
        {
            PortalController pc = new PortalController();
            ddlLogTypePortalID.DataTextField = "PortalName";
            ddlLogTypePortalID.DataValueField = "PortalID";
            ddlLogTypePortalID.DataSource = pc.GetPortals();
            ddlLogTypePortalID.DataBind();

            ListItem i = new ListItem();
            i.Text = Localization.GetString( "All" );
            i.Value = "*";
            ddlLogTypePortalID.Items.Insert( 0, i );

            pnlEditLogTypeConfigInfo.Visible = true;
            pnlLogTypeConfigInfo.Visible = false;
            LogController l = new LogController();

            ArrayList arrLogTypeInfo;
            arrLogTypeInfo = l.GetLogTypeInfo();

            arrLogTypeInfo.Sort( new LogTypeSortFriendlyName() );

            ddlLogTypeKey.DataTextField = "LogTypeFriendlyName";
            ddlLogTypeKey.DataValueField = "LogTypeKey";
            ddlLogTypeKey.DataSource = arrLogTypeInfo;
            ddlLogTypeKey.DataBind();

            int[] items = new int[] {1, 2, 3, 4, 5, 10, 25, 100, 250, 500};
            ddlKeepMostRecent.Items.Clear();
            ddlKeepMostRecent.Items.Add( new ListItem( Localization.GetString( "All" ), "*" ) );
            foreach( int item in items )
            {
                if( item == 1 )
                {
                    ddlKeepMostRecent.Items.Add( new ListItem( item.ToString() + Localization.GetString( "LogEntry", this.LocalResourceFile ), item.ToString() ) );
                }
                else
                {
                    ddlKeepMostRecent.Items.Add( new ListItem( item.ToString() + Localization.GetString( "LogEntries", this.LocalResourceFile ), item.ToString() ) );
                }
            }

            int[] items2 = new int[] {1, 2, 3, 4, 5, 10, 25, 100, 250, 500, 1000};
            ddlThreshold.Items.Clear();
            foreach( int item in items2 )
            {
                if( item == 1 )
                {
                    ddlThreshold.Items.Add( new ListItem( item.ToString() + Localization.GetString( "Occurance", this.LocalResourceFile ), item.ToString() ) );
                }
                else
                {
                    ddlThreshold.Items.Add( new ListItem( item.ToString() + Localization.GetString( "Occurances", this.LocalResourceFile ), item.ToString() ) );
                }
            }

            ListItem j = new ListItem();
            j.Text = Localization.GetString( "All" );
            j.Value = "*";
            ddlLogTypeKey.Items.Insert( 0, j );
        }

        private void BindSummaryData()
        {
            LogController objLogController = new LogController();
            ArrayList arrLogTypeConfigInfo = objLogController.GetLogTypeConfigInfo();

            //Localize the Headers
            Localization.LocalizeDataGrid( ref dgLogTypeConfigInfo, this.LocalResourceFile );

            dgLogTypeConfigInfo.DataSource = arrLogTypeConfigInfo;
            dgLogTypeConfigInfo.DataBind();
            pnlEditLogTypeConfigInfo.Visible = false;
            pnlLogTypeConfigInfo.Visible = true;
        }

        private void DisableLoggingControls()
        {
            if( chkIsActive.Checked )
            {
                ddlLogTypeKey.Enabled = true;
                ddlLogTypePortalID.Enabled = true;
                ddlKeepMostRecent.Enabled = true;
                txtFileName.Enabled = true;
            }
            else
            {
                ddlLogTypeKey.Enabled = false;
                ddlLogTypePortalID.Enabled = false;
                ddlKeepMostRecent.Enabled = false;
                txtFileName.Enabled = false;
            }
        }

        private void DisableNotificationControls()
        {
            if( chkEmailNotificationStatus.Checked)
            {
                ddlThreshold.Enabled = true;
                ddlThresholdNotificationTime.Enabled = true;
                ddlThresholdNotificationTimeType.Enabled = true;
                txtMailFromAddress.Enabled = true;
                txtMailToAddress.Enabled = true;
            }
            else
            {
                ddlThreshold.Enabled = false;
                ddlThresholdNotificationTime.Enabled = false;
                ddlThresholdNotificationTimeType.Enabled = false;
                txtMailFromAddress.Enabled = false;
                txtMailToAddress.Enabled = false;
            }
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
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ! Page.IsPostBack )
                {
                    if( Request.QueryString["action"] == "add" )
                    {
                        BindDetailData();
                    }
                    else
                    {
                        BindSummaryData();
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdCancel_Click runs when the cancel Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdCancel_Click( Object sender, EventArgs e )
        {
            try
            {
                BindSummaryData();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdDelete_Click runs when the delete Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdDelete_Click( Object sender, EventArgs e )
        {
            LogTypeConfigInfo objLogTypeConfigInfo = new LogTypeConfigInfo();
            LogController l = new LogController();
            objLogTypeConfigInfo.ID = Convert.ToString( ViewState["LogID"] );
            try
            {
                l.DeleteLogTypeConfigInfo( objLogTypeConfigInfo );
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "ConfigDeleted", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.GreenSuccess );
                BindSummaryData();
            }
            catch
            {
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "DeleteError", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.RedError );
            }
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdUpdate_Click( Object sender, EventArgs e )
        {
            LogTypeConfigInfo objLogTypeConfigInfo = new LogTypeConfigInfo();
            objLogTypeConfigInfo.LoggingIsActive = chkIsActive.Checked;
            objLogTypeConfigInfo.LogTypeKey = ddlLogTypeKey.SelectedItem.Value;
            objLogTypeConfigInfo.LogTypePortalID = ddlLogTypePortalID.SelectedItem.Value;
            objLogTypeConfigInfo.KeepMostRecent = ddlKeepMostRecent.SelectedItem.Value;
            objLogTypeConfigInfo.LogFileName = txtFileName.Text;

            objLogTypeConfigInfo.EmailNotificationIsActive = chkEmailNotificationStatus.Checked;
            objLogTypeConfigInfo.NotificationThreshold = Convert.ToInt32( ddlThreshold.SelectedItem.Value );
            objLogTypeConfigInfo.NotificationThresholdTime = Convert.ToInt32( ddlThresholdNotificationTime.SelectedItem.Value );
            objLogTypeConfigInfo.NotificationThresholdTimeType = (LogTypeConfigInfo.NotificationThresholdTimeTypes)Enum.Parse(typeof(LogTypeConfigInfo.NotificationThresholdTimeTypes), ddlThresholdNotificationTimeType.SelectedItem.Value);
            objLogTypeConfigInfo.MailFromAddress = txtMailFromAddress.Text;
            objLogTypeConfigInfo.MailToAddress = txtMailToAddress.Text;

            LogController l = new LogController();

            if( ViewState["LogID"] != null )
            {
                objLogTypeConfigInfo.ID = Convert.ToString( ViewState["LogID"] );
                l.UpdateLogTypeConfigInfo( objLogTypeConfigInfo );
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "ConfigUpdated", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.GreenSuccess );
            }
            else
            {
                objLogTypeConfigInfo.ID = Guid.NewGuid().ToString();
                l.AddLogTypeConfigInfo( objLogTypeConfigInfo );
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "ConfigAdded", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.GreenSuccess );
            }

            BindSummaryData();
        }

        protected void cmdReturn_Click( Object sender, EventArgs e )
        {
            Response.Redirect( Globals.NavigateURL(), true );
        }

        protected void chkEmailNotificationStatus_CheckedChanged( Object sender, EventArgs e )
        {
            DisableNotificationControls();
        }

        protected void chkIsActive_CheckedChanged( Object sender, EventArgs e )
        {
            DisableLoggingControls();
        }

        public void dgLogTypeConfigInfo_EditCommand( object source, DataGridCommandEventArgs e )
        {
            string LogID = Convert.ToString( dgLogTypeConfigInfo.DataKeys[ e.Item.ItemIndex ] );
            ViewState["LogID"] = LogID;

            BindDetailData();

            LogController l = new LogController();

            LogTypeConfigInfo objLogTypeConfigInfo = l.GetLogTypeConfigInfoByID( LogID );

            txtFileName.Text = objLogTypeConfigInfo.LogFileName;
            chkIsActive.Checked = objLogTypeConfigInfo.LoggingIsActive;
            chkEmailNotificationStatus.Checked = objLogTypeConfigInfo.EmailNotificationIsActive;

            if( ddlLogTypeKey.Items.FindByValue( objLogTypeConfigInfo.LogTypeKey ) != null )
            {
                ddlLogTypeKey.ClearSelection();
                ddlLogTypeKey.Items.FindByValue( objLogTypeConfigInfo.LogTypeKey ).Selected = true;
            }
            if( ddlLogTypePortalID.Items.FindByValue( objLogTypeConfigInfo.LogTypePortalID ) != null )
            {
                ddlLogTypePortalID.ClearSelection();
                ddlLogTypePortalID.Items.FindByValue( objLogTypeConfigInfo.LogTypePortalID ).Selected = true;
            }
            if( ddlKeepMostRecent.Items.FindByValue( objLogTypeConfigInfo.KeepMostRecent ) != null )
            {
                ddlKeepMostRecent.ClearSelection();
                ddlKeepMostRecent.Items.FindByValue( objLogTypeConfigInfo.KeepMostRecent ).Selected = true;
            }
            if( ddlThreshold.Items.FindByValue( objLogTypeConfigInfo.NotificationThreshold.ToString() ) != null )
            {
                ddlThreshold.ClearSelection();
                ddlThreshold.Items.FindByValue( objLogTypeConfigInfo.NotificationThreshold.ToString() ).Selected = true;
            }
            if( ddlThresholdNotificationTime.Items.FindByValue( objLogTypeConfigInfo.NotificationThresholdTime.ToString() ) != null )
            {
                ddlThresholdNotificationTime.ClearSelection();
                ddlThresholdNotificationTime.Items.FindByValue( objLogTypeConfigInfo.NotificationThresholdTime.ToString() ).Selected = true;
            }
            if( ddlThresholdNotificationTimeType.Items.FindByText( objLogTypeConfigInfo.NotificationThresholdTimeType.ToString() ) != null )
            {
                ddlThresholdNotificationTimeType.ClearSelection();
                ddlThresholdNotificationTimeType.Items.FindByText( objLogTypeConfigInfo.NotificationThresholdTimeType.ToString() ).Selected = true;
            }
            txtMailFromAddress.Text = objLogTypeConfigInfo.MailFromAddress;
            txtMailToAddress.Text = objLogTypeConfigInfo.MailToAddress;

            DisableLoggingControls();
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl( "action", "add" ), false, SecurityAccessLevel.Admin, true, false );
                return actions;
            }
        }

        //This call is required by the Web Form Designer.        
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