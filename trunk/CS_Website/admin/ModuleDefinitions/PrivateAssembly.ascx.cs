using System;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Admin.ModuleDefinitions
{
    /// <summary>
    /// The PrivateAssembly PortalModuleBase is used to create a Private Assembly Package
    /// from this module
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	1/14/2005	created
    /// </history>
    public partial class PrivateAssembly : PortalModuleBase
    {
        /// <summary>
        /// Page_Init runs when the control is initialised.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	1/14/2005	created
        /// </history>
        protected void Page_Init( Object sender, EventArgs e )
        {
        }

        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	1/14/2005	created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
        }

        /// <summary>
        /// cmdCancel_Click runs when the Cancel Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	1/14/2005	created
        /// </history>
        protected void cmdCancel_Click( Object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Globals.NavigateURL() );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdCreate_Click runs when the Create Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	1/14/2005	created
        /// </history>
        protected void cmdCreate_Click( Object sender, EventArgs e )
        {
            int mid = Null.NullInteger;
            if( ( Request.QueryString["desktopmoduleid"] != null ) )
            {
                mid = int.Parse( Request.QueryString["desktopmoduleid"] );
            }

            if( mid > 0 )
            {
                PaWriter PaWriter = new PaWriter( false, txtFileName.Text );

                string PaZipName = PaWriter.CreatePrivateAssembly( mid );

                if( PaWriter.ProgressLog.Valid )
                {
                    lblMessage.Text = string.Format( Localization.GetString( "LOG.MESSAGE.Success", LocalResourceFile ), PaZipName, null );
                    lblMessage.CssClass = "Head";
                }
                else
                {
                    lblMessage.Text = Localization.GetString( "LOG.MESSAGE.Error", LocalResourceFile );
                    lblMessage.CssClass = "NormalRed";
                }

                ModuleInfo FileManagerModule = ( new ModuleController() ).GetModuleByDefinition( Null.NullInteger, "File Manager" );

                if( FileManagerModule != null )
                {
                    lblLink.Text = string.Format( Localization.GetString( "lblLink", LocalResourceFile ), Globals.NavigateURL( FileManagerModule.TabID ) );
                }
                divLog.Controls.Add( PaWriter.ProgressLog.GetLogsTable() );
                pnlLogs.Visible = true;
            }
        }
    }
}