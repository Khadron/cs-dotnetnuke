using System;
using System.Diagnostics;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Modules.SearchInput
{
    /// Namespace:  DotNetNuke.Modules.SearchInput
    /// Project:    DotNetNuke.SearchInput
    /// Class:      InputSettings
    /// <summary>
    /// The InputSettings ModuleSettingsBase is used to manage the
    /// settings for the Search Input Module
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
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
        /// <remarks>
        /// </remarks>
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
        /// <remarks>
        /// </remarks>
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
                    else
                    {
                        //Select first one
                        cboModule.SelectedIndex = 0;
                    }
                    txtModule.Text = cboModule.SelectedItem.Text;

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
        /// <remarks>
        /// </remarks>
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

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
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