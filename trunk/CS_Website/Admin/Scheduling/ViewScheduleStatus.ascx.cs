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
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.UI.Skins.Controls;


namespace DotNetNuke.Modules.Admin.Scheduling
{
    /// <summary>
    /// The ViewScheduleStatus PortalModuleBase is used to view the schedule Status
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class ViewScheduleStatus : PortalModuleBase, IActionable
    {
        private ScheduleStatus Status;

        private void BindData()
        {
            lblFreeThreads.Text = SchedulingProvider.Instance().GetFreeThreadCount().ToString();
            lblActiveThreads.Text = SchedulingProvider.Instance().GetActiveThreadCount().ToString();
            lblMaxThreads.Text = SchedulingProvider.Instance().GetMaxThreadCount().ToString();

            ArrayList arrScheduleQueue = SchedulingProvider.Instance().GetScheduleQueue();
            if( arrScheduleQueue.Count == 0 )
            {
                pnlScheduleQueue.Visible = false;
            }
            else
            {
                //Localize Grid
                Localization.LocalizeDataGrid( ref dgScheduleQueue, this.LocalResourceFile );

                dgScheduleQueue.DataSource = arrScheduleQueue;
                dgScheduleQueue.DataBind();
            }

            ArrayList arrScheduleProcessing = SchedulingProvider.Instance().GetScheduleProcessing();
            if( arrScheduleProcessing.Count == 0 )
            {
                pnlScheduleProcessing.Visible = false;
            }
            else
            {
                //Localize Grid
                Localization.LocalizeDataGrid( ref dgScheduleProcessing, this.LocalResourceFile );

                dgScheduleProcessing.DataSource = arrScheduleProcessing;
                dgScheduleProcessing.DataBind();
            }
            if( arrScheduleProcessing.Count == 0 && arrScheduleQueue.Count == 0 )
            {
                string strMessage = Localization.GetString( "NoTasks", this.LocalResourceFile );
                UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessageType.YellowWarning );
            }
        }

        private void BindStatus()
        {
            Status = SchedulingProvider.Instance().GetScheduleStatus();
            lblStatus.Text = Status.ToString();
            if( Status == ScheduleStatus.STOPPED && SchedulingProvider.SchedulerMode != SchedulerMode.DISABLED )
            {
                cmdStart.Enabled = true;
                cmdStop.Enabled = false;
            }
            else if( Status == ScheduleStatus.WAITING_FOR_REQUEST || SchedulingProvider.SchedulerMode == SchedulerMode.DISABLED )
            {
                cmdStart.Enabled = false;
                cmdStop.Enabled = false;
            }
            else
            {
                cmdStart.Enabled = false;
                cmdStop.Enabled = true;
            }
        }

        protected string GetOverdueText( double OverdueBy )
        {
            if( OverdueBy > 0 )
            {
                return OverdueBy.ToString();
            }
            else
            {
                return "";
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
                if( SchedulingProvider.Enabled )
                {
                    if( ! Page.IsPostBack )
                    {
                        BindData();
                        BindStatus();
                    }
                }
                else
                {
                    UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "DisabledMessage", this.LocalResourceFile ), ModuleMessageType.RedError );
                    lblStatus.Text = Localization.GetString( "Disabled", this.LocalResourceFile );
                    cmdStart.Enabled = false;
                    cmdStop.Enabled = false;
                    lblFreeThreads.Text = "0";
                    lblActiveThreads.Text = "0";
                    lblMaxThreads.Text = "0";
                    pnlScheduleQueue.Visible = false;
                    pnlScheduleProcessing.Visible = false;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdStart_Click runs when the Start Button is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdStart_Click( Object sender, EventArgs e )
        {
            SchedulingProvider.Instance().StartAndWaitForResponse();
            BindData();
            BindStatus();
        }

        /// <summary>
        /// cmdStop_Click runs when the Stop Button is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdStop_Click( Object sender, EventArgs e )
        {
            SchedulingProvider.Instance().Halt( Localization.GetString( "ManuallyStopped", this.LocalResourceFile ) );
            BindData();
            BindStatus();
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
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl(), false, SecurityAccessLevel.Admin, true, false );
                return actions;
            }
        }



    }
}