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
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.SearchInput
{
    /// Namespace:  DotNetNuke.Modules.SearchInput
    /// Project:    DotNetNuke.SearchInput
    /// Class:      InputSettings
    /// <summary>
    /// The InputSettings ModuleSettingsBase is used to manage the
    /// settings for the Search Input Module
    /// </summary>
    /// <history>
    ///		[cnurse]	11/30/2004	converted to SettingsBase
    /// </history>
    public partial class InputSettings : ModuleSettingsBase
    {
        protected LinkButton cmdUpdate;
        protected LinkButton cmdCancel;

        /// <summary>
        /// BindSearchResults gets the Search Results Modules available and binds them to the
        /// drop-down combo
        /// </summary>
        /// <history>
        ///		[cnurse]	11/30/2004	converted to SettingsBase
        /// </history>
        private void BindSearchResults()
        {
            SearchInputController objSearch = new SearchInputController();

            cboModule.DataSource = objSearch.GetSearchResultModules( PortalId );
            cboModule.DataTextField = "SearchTabName";
            cboModule.DataValueField = "TabID";
            cboModule.DataBind();
            if( cboModule.Items.Count < 2 )
            {
                cboModule.Visible = false;
                txtModule.Visible = true;
            }
            else
            {
                cboModule.Visible = true;
                txtModule.Visible = false;
            }
        }

        /// <summary>
        /// LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// <history>
        ///		[cnurse]	11/30/2004	converted to SettingsBase
        /// </history>
        public override void LoadSettings()
        {
            try
            {
                if( Page.IsPostBack == false )
                {
                    BindSearchResults();

                    string SearchTabID = Convert.ToString( ModuleSettings["SearchResultsModule"] );
                    string ShowGoImage = Convert.ToString( ModuleSettings["ShowGoImage"] );
                    string ShowSearchImage = Convert.ToString( ModuleSettings["ShowSearchImage"] );

                    if( cboModule.Items.FindByValue( SearchTabID ) != null )
                    {
                        cboModule.Items.FindByValue( SearchTabID ).Selected = true;
                    }
                    if( cboModule.Items.Count > 0 )
                    {
                        txtModule.Text = cboModule.SelectedItem.Text;
                    }
                    else
                    {
                        txtModule.Text = Localization.GetString( "NoSearchModule", LocalResourceFile );
                    }

                    if( ShowGoImage != null )
                    {
                        chkGo.Checked = Convert.ToBoolean( ShowGoImage );
                    }

                    if( ShowSearchImage != null )
                    {
                        chkSearchImage.Checked = Convert.ToBoolean( ShowSearchImage );
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// UpdateSettings saves the modified settings to the Database
        /// </summary>
        /// <history>
        ///		[cnurse]	11/30/2004	converted to SettingsBase
        /// </history>
        public override void UpdateSettings()
        {
            try
            {
                ModuleController objModules = new ModuleController();

                if( cboModule.SelectedIndex != -1 )
                {
                    objModules.UpdateModuleSetting( this.ModuleId, "SearchResultsModule", cboModule.SelectedItem.Value );
                }

                objModules.UpdateModuleSetting( this.ModuleId, "ShowGoImage", chkGo.Checked.ToString() );
                objModules.UpdateModuleSetting( this.ModuleId, "ShowSearchImage", chkSearchImage.Checked.ToString() );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}