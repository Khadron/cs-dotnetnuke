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
using System.IO;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.UserControls
{
    public abstract class Help : PortalModuleBase
    {
        private string _key;
        protected LinkButton cmdCancel;
        protected HyperLink cmdHelp;
        protected Label lblHelp;

        public Help()
        {
            this.cmdCancel.Click += new EventHandler( this.cmdCancel_Click );
            base.Load += new EventHandler( this.Page_Load );
        }

        /// <Summary>cmdCancel_Click runs when the cancel Button is clicked</Summary>
        private void cmdCancel_Click( object sender, EventArgs e )
        {
            try
            {
                Response.Redirect( Convert.ToString( ViewState["UrlReferrer"] ), true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <Summary>Page_Load runs when the control is loaded.</Summary>
        private void Page_Load( object sender, EventArgs e )
        {
            string FriendlyName = "";

            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModule( ModuleId, TabId );
            if( objModule != null )
            {
                FriendlyName = objModule.FriendlyName;
            }

            int ModuleControlId = Null.NullInteger;

            if( !( Request.QueryString["ctlid"] == null ) )
            {
                ModuleControlId = int.Parse( Request.QueryString["ctlid"] );
            }

            ModuleControlController objModuleControls = new ModuleControlController();
            ModuleControlInfo objModuleControl = objModuleControls.GetModuleControl( ModuleControlId );
            if( objModuleControl != null )
            {
                string FileName = Path.GetFileName( objModuleControl.ControlSrc );
                string LocalResourceFile = objModuleControl.ControlSrc.Replace( FileName, Localization.LocalResourceDirectory + "/" + FileName );
                if( Localization.GetString( ModuleActionType.HelpText, LocalResourceFile ) != "" )
                {
                    lblHelp.Text = Localization.GetString( ModuleActionType.HelpText, LocalResourceFile );
                }
                _key = objModuleControl.ControlKey;

                string helpUrl = Globals.GetOnLineHelp( objModuleControl.HelpURL, ModuleConfiguration );
                if( !Null.IsNull( helpUrl ) )
                {
                    cmdHelp.NavigateUrl = Globals.FormatHelpUrl( helpUrl, PortalSettings, FriendlyName );
                    cmdHelp.Visible = true;
                }
                else
                {
                    cmdHelp.Visible = false;
                }
            }

            if( Page.IsPostBack == false )
            {
                if( Request.UrlReferrer != null )
                {
                    ViewState["UrlReferrer"] = Convert.ToString( Request.UrlReferrer );
                }
                else
                {
                    ViewState["UrlReferrer"] = "";
                }
            }
        }
    }
}