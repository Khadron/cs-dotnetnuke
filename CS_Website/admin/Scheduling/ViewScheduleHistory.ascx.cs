using System;
using System.Collections;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Scheduling;

namespace DotNetNuke.Modules.Admin.Scheduling
{
    /// <summary>
    /// The ViewScheduleHistory PortalModuleBase is used to view the schedule History
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class ViewScheduleHistory : PortalModuleBase, IActionable
    {
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
                    int ScheduleID;
                    if( Request.QueryString["ScheduleID"] != null )
                    {
                        //get history for specific scheduleid
                        ScheduleID = Convert.ToInt32( Request.QueryString["ScheduleID"] );
                    }
                    else
                    {
                        //get history for all schedules
                        ScheduleID = - 1;
                    }
                    ArrayList arrSchedule = SchedulingProvider.Instance().GetScheduleHistory( ScheduleID );
                    arrSchedule.Sort( new ScheduleHistorySortStartDate() );

                    //Localize Grid
                    Localization.LocalizeDataGrid( ref dgScheduleHistory, this.LocalResourceFile );

                    dgScheduleHistory.DataSource = arrSchedule;
                    dgScheduleHistory.DataBind();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
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
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.ContentOptions, LocalResourceFile ), ModuleActionType.ContentOptions, "", "", EditUrl( "", "", "Status" ), false, SecurityAccessLevel.Admin, true, false );
                return actions;
            }
        }

        protected string GetNotesText( string Notes )
        {
            if( Notes != "" )
            {
                Notes = "<textarea rows=\"5\" cols=\"65\">" + Notes + "</textarea>";
                return Notes;
            }
            else
            {
                return "";
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