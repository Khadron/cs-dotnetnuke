#region DotNetNuke License
// DotNetNuke� - http://www.dotnetnuke.com
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