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
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.WebControls;

namespace DotNetNuke.UI.Containers
{
    /// <summary></summary>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class Title : SkinObjectBase
    {
        // private members
        private string _cssClass;

        public string CssClass
        {
            get
            {
                return _cssClass;
            }
            set
            {
                _cssClass = value;
            }
        }

        private bool CanEditModule()
        {
            bool blnCanEdit = false;
            PortalModuleBase objModule = Container.GetPortalModuleBase( this );
            if( ( objModule != null ) && ( objModule.ModuleId > Null.NullInteger ) )
            {
                blnCanEdit = ( PortalSecurity.IsInRoles( PortalSettings.AdministratorRoleName ) || PortalSecurity.IsInRoles( PortalSettings.ActiveTab.AdministratorRoles.ToString() ) ) && Globals.IsAdminControl() == false && PortalSettings.ActiveTab.IsAdminTab == false;
            }
            return blnCanEdit;
        }

        /// <summary>
        /// Assign the CssClass and Text Attributes for the Title label.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[sun1]	2/1/2004	Created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                // public attributes
                if( CssClass != "" )
                {
                    lblTitle.CssClass = CssClass;
                }

                PortalModuleBase objPortalModule = Container.GetPortalModuleBase( this );

                string strTitle = Null.NullString;
                if( objPortalModule != null )
                {
                    strTitle = objPortalModule.ModuleConfiguration.ModuleTitle;
                }
                if( strTitle == Null.NullString )
                {
                    strTitle = "&nbsp;";
                }
                lblTitle.Text = strTitle;
                if( CanEditModule() == false )
                {
                    lblTitle.EditEnabled = false;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        protected void lblTitle_UpdateLabel( object source, DNNLabelEditEventArgs e )
        {
            if( CanEditModule() )
            {
                ModuleController objModule = new ModuleController();
                PortalModuleBase objPortalModule = Container.GetPortalModuleBase( this );
                ModuleInfo objModInfo = objModule.GetModule( objPortalModule.ModuleId, objPortalModule.TabId );

                objModInfo.ModuleTitle = e.Text;
                objModule.UpdateModule( objModInfo );
            }
        }
    }
}