using System;
using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Scheduling;

namespace DotNetNuke.Modules.Admin.Scheduling
{
    /// <summary>
    /// The ViewSchedule PortalModuleBase is used to manage the scheduled items.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class ViewSchedule : PortalModuleBase, IActionable
    {
        /// <summary>
        /// GetTimeLapse formats the time lapse as a string
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected string GetTimeLapse( int TimeLapse, string TimeLapseMeasurement )
        {
            if( TimeLapse != Null.NullInteger )
            {
                string str=String.Empty;
                string strPrefix = Localization.GetString( "TimeLapsePrefix", this.LocalResourceFile );
                string strSec = Localization.GetString( "Second", this.LocalResourceFile );
                string strMn = Localization.GetString( "Minute", this.LocalResourceFile );
                string strHour = Localization.GetString( "Hour", this.LocalResourceFile );
                string strDay = Localization.GetString( "Day", this.LocalResourceFile );
                string strSecs = Localization.GetString( "Seconds" );
                string strMns = Localization.GetString( "Minutes" );
                string strHours = Localization.GetString( "Hours" );
                string strDays = Localization.GetString( "Days" );

                switch( TimeLapseMeasurement )
                {
                    case "s":

                        str = strPrefix + " " + TimeLapse.ToString() + " " + ( TimeLapse > 1 ? strSecs : strSec ).ToString();
                        break;
                    case "m":

                        str = strPrefix + " " + TimeLapse.ToString() + " " + ( TimeLapse > 1 ? strMns : strMn ).ToString();
                        break;
                    case "h":

                        str = strPrefix + " " + TimeLapse.ToString() + " " + ( TimeLapse > 1 ? strHours : strHour ).ToString();
                        break;
                    case "d":

                        str = strPrefix + " " + TimeLapse.ToString() + " " + ( TimeLapse > 1 ? strDays : strDay ).ToString();
                        break;
                }

                return str.ToString();
            }
            else
            {
                return Localization.GetString( "n/a", this.LocalResourceFile );
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
                    ArrayList arrSchedule = SchedulingProvider.Instance().GetSchedule();

                    //Lcalize Grid
                    Localization.LocalizeDataGrid( ref dgSchedule, this.LocalResourceFile );

                    dgSchedule.DataSource = arrSchedule;
                    dgSchedule.DataBind();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl(), false, SecurityAccessLevel.Admin, true, false );
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.ContentOptions, LocalResourceFile ), ModuleActionType.ContentOptions, "", "", EditUrl( "", "", "Status" ), false, SecurityAccessLevel.Admin, true, false );
                actions.Add( GetNextActionID(), Localization.GetString( "ScheduleHistory.Action", LocalResourceFile ), "", "", "", EditUrl( "", "", "History" ), false, SecurityAccessLevel.Admin, true, false );
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