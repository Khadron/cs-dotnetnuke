using System;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Scheduling
{
    /// <summary>
    /// The EditSchedule PortalModuleBase is used to edit the scheduled items.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class EditSchedule : PortalModuleBase, IActionable
    {
        private void BindData()
        {
            ScheduleItem objScheduleItem;

            if( Request.QueryString["ScheduleID"] != null )
            {
                ViewState["ScheduleID"] = Request.QueryString["ScheduleID"];
                objScheduleItem = SchedulingProvider.Instance().GetSchedule( Convert.ToInt32( Request.QueryString["ScheduleID"] ) );

                txtType.Enabled = false;
                txtType.Text = objScheduleItem.TypeFullName;
                chkEnabled.Checked = objScheduleItem.Enabled;
                if( objScheduleItem.TimeLapse == Null.NullInteger )
                {
                    txtTimeLapse.Text = "";
                }
                else
                {
                    txtTimeLapse.Text = Convert.ToString( objScheduleItem.TimeLapse );
                }

                if( ddlTimeLapseMeasurement.Items.FindByValue( objScheduleItem.TimeLapseMeasurement ) != null )
                {
                    ddlTimeLapseMeasurement.Items.FindByValue( objScheduleItem.TimeLapseMeasurement ).Selected = true;
                }
                if( objScheduleItem.RetryTimeLapse == Null.NullInteger )
                {
                    txtRetryTimeLapse.Text = "";
                }
                else
                {
                    txtRetryTimeLapse.Text = Convert.ToString( objScheduleItem.RetryTimeLapse );
                }
                if( ddlRetryTimeLapseMeasurement.Items.FindByValue( objScheduleItem.RetryTimeLapseMeasurement ) != null )
                {
                    ddlRetryTimeLapseMeasurement.Items.FindByValue( objScheduleItem.RetryTimeLapseMeasurement ).Selected = true;
                }
                if( ddlRetainHistoryNum.Items.FindByValue( Convert.ToString( objScheduleItem.RetainHistoryNum ) ) != null )
                {
                    ddlRetainHistoryNum.Items.FindByValue( Convert.ToString( objScheduleItem.RetainHistoryNum ) ).Selected = true;
                }
                else
                {
                    ddlRetainHistoryNum.Items.Add( objScheduleItem.RetainHistoryNum.ToString() );
                    ddlRetainHistoryNum.Items.FindByValue( Convert.ToString( objScheduleItem.RetainHistoryNum ) ).Selected = true;
                }
                if( ddlAttachToEvent.Items.FindByValue( objScheduleItem.AttachToEvent ) != null )
                {
                    ddlAttachToEvent.Items.FindByValue( objScheduleItem.AttachToEvent ).Selected = true;
                }
                chkCatchUpEnabled.Checked = objScheduleItem.CatchUpEnabled;
                txtObjectDependencies.Text = objScheduleItem.ObjectDependencies;
                txtServers.Text = objScheduleItem.Servers;
            }
            else
            {
                cmdDelete.Visible = false;
                txtType.Enabled = true;
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ! Page.IsPostBack )
                {
                    ClientAPI.AddButtonConfirm( cmdDelete, Localization.GetString( "DeleteItem" ) );
                    BindData();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdDelete_Click runs when the Delete Button is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdDelete_Click( Object sender, EventArgs e )
        {
            ScheduleItem objScheduleItem = new ScheduleItem();
            objScheduleItem.ScheduleID = Convert.ToInt32( ViewState["ScheduleID"] );
            SchedulingProvider.Instance().DeleteSchedule( objScheduleItem );
            string strMessage;
            strMessage = Localization.GetString( "DeleteSuccess", this.LocalResourceFile );
            UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessage.ModuleMessageType.GreenSuccess );
            pnlScheduleItem.Visible = false;
        }

        /// <summary>
        /// cmdUpdate_Click runs when the Update Button is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdUpdate_Click( Object sender, EventArgs e )
        {
            ScheduleItem objScheduleItem = new ScheduleItem();
            string strMessage;
            objScheduleItem.TypeFullName = txtType.Text;
            if( txtTimeLapse.Text == "" || txtTimeLapse.Text == "0" || txtTimeLapse.Text == "-1" )
            {
                objScheduleItem.TimeLapse = Null.NullInteger;
            }
            else
            {
                objScheduleItem.TimeLapse = Convert.ToInt32( txtTimeLapse.Text );
            }

            objScheduleItem.TimeLapseMeasurement = ddlTimeLapseMeasurement.SelectedItem.Value;

            if( txtRetryTimeLapse.Text == "" || txtRetryTimeLapse.Text == "0" || txtRetryTimeLapse.Text == "-1" )
            {
                objScheduleItem.RetryTimeLapse = Null.NullInteger;
            }
            else
            {
                objScheduleItem.RetryTimeLapse = Convert.ToInt32( txtRetryTimeLapse.Text );
            }

            objScheduleItem.RetryTimeLapseMeasurement = ddlRetryTimeLapseMeasurement.SelectedItem.Value;
            objScheduleItem.RetainHistoryNum = Convert.ToInt32( ddlRetainHistoryNum.SelectedItem.Value );
            objScheduleItem.AttachToEvent = ddlAttachToEvent.SelectedItem.Value;
            objScheduleItem.CatchUpEnabled = chkCatchUpEnabled.Checked;
            objScheduleItem.Enabled = chkEnabled.Checked;
            objScheduleItem.ObjectDependencies = txtObjectDependencies.Text;
            objScheduleItem.Servers = txtServers.Text;

            if( ViewState["ScheduleID"] != null )
            {
                objScheduleItem.ScheduleID = Convert.ToInt32( ViewState["ScheduleID"] );
                SchedulingProvider.Instance().UpdateSchedule( objScheduleItem );
            }
            else
            {
                SchedulingProvider.Instance().AddSchedule( objScheduleItem );
            }
            strMessage = Localization.GetString( "UpdateSuccess", this.LocalResourceFile );
            UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessage.ModuleMessageType.GreenSuccess );
            if( SchedulingProvider.SchedulerMode == SchedulerMode.TIMER_METHOD )
            {
                SchedulingProvider.Instance().ReStart( "Change made to schedule." );
            }
            BindData();
            cmdDelete.Visible = true;
        }

        protected void cmdCancel_Click( Object sender, EventArgs e )
        {
            Response.Redirect( Globals.NavigateURL(), true );
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                if( Request.QueryString["ScheduleID"] != null )
                {
                    int ScheduleID = Convert.ToInt32( Request.QueryString["ScheduleID"] );
                    actions.Add( GetNextActionID(), Localization.GetString( "ScheduleHistory.Action", LocalResourceFile ), "", "", "", EditUrl( "ScheduleID", ScheduleID.ToString(), "History" ), false, SecurityAccessLevel.Admin, true, false );
                }
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.ContentOptions, LocalResourceFile ), ModuleActionType.ContentOptions, "", "", EditUrl( "", "", "Status" ), false, SecurityAccessLevel.Admin, true, false );
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
        }
    }
}