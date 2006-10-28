using System;
using System.Collections;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Admin.ModuleDefinitions
{
    /// <summary>
    /// The ModuleDefinitions PortalModuleBase is used to manage the modules
    /// attached to this portal
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class ModuleDefinitions : PortalModuleBase, IActionable
    {
        /// <summary>
        /// BindData fetches the data from the database and updates the controls
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        public void BindData()
        {
            // Get the portal's defs from the database
            DesktopModuleController objDesktopModules = new DesktopModuleController();

            ArrayList arr = objDesktopModules.GetDesktopModules();

            DesktopModuleInfo objDesktopModule = new DesktopModuleInfo();

            objDesktopModule.DesktopModuleID = - 2;
            objDesktopModule.FriendlyName = Localization.GetString( "SkinObjects" );
            objDesktopModule.Description = Localization.GetString( "SkinObjectsDescription" );
            objDesktopModule.IsPremium = false;

            arr.Insert( 0, objDesktopModule );

            //Localize Grid
            Localization.LocalizeDataGrid(ref grdDefinitions, this.LocalResourceFile );

            grdDefinitions.DataSource = arr;
            grdDefinitions.DataBind();
        }

        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
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
                BindData();
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
                actions.Add( GetNextActionID(), Localization.GetString( ModuleActionType.AddContent, LocalResourceFile ), ModuleActionType.AddContent, "", "", EditUrl(), false, SecurityAccessLevel.Host, true, false );

                ModuleInfo FileManagerModule = ( new ModuleController() ).GetModuleByDefinition( Null.NullInteger, "File Manager" );

                string[] @params = new string[3];

                @params[0] = "mid=" + FileManagerModule.ModuleID;
                @params[1] = "ftype=" + UploadType.Module.ToString();
                @params[2] = "rtab=" + this.TabId;
                actions.Add( GetNextActionID(), Localization.GetString( "ModuleUpload.Action", LocalResourceFile ), ModuleActionType.AddContent, "", "", Globals.NavigateURL( FileManagerModule.TabID, "Edit", @params ), false, SecurityAccessLevel.Host, true, false );
                return actions;
            }
        }
    }
}