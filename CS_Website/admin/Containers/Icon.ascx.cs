using System;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Skins;

namespace DotNetNuke.UI.Containers
{
    /// Project	 : DotNetNuke
    /// Class	 : DotNetNuke.UI.Containers.Icon
    ///
    /// <summary>
    /// Contains the attributes of an Icon.
    /// These are read into the PortalModuleBase collection as attributes for the icons within the module controls.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[sun1]	    2/1/2004	Created
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class Icon : SkinObjectBase
    {
        // private members
        private string _borderWidth;

        // protected controls

        public string BorderWidth
        {
            get
            {
                return _borderWidth;
            }
            set
            {
                _borderWidth = value;
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

        //*******************************************************
        //
        // The Page_Load server event handler on this page is used
        // to populate the control information
        //
        //*******************************************************

        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                // public attributes
                if( BorderWidth != "" )
                {
                    imgIcon.BorderWidth = Unit.Parse( BorderWidth );
                }

                this.Visible = false;
                PortalModuleBase objPortalModule = Container.GetPortalModuleBase( this );
                if( ( objPortalModule != null ) && ( objPortalModule.ModuleConfiguration != null ) )
                {
                    if( objPortalModule.ModuleConfiguration.IconFile != "" )
                    {
                        if( objPortalModule.ModuleConfiguration.IconFile.StartsWith( "~/" ) )
                        {
                            imgIcon.ImageUrl = objPortalModule.ModuleConfiguration.IconFile;
                        }
                        else
                        {
                            if( Globals.IsAdminControl() )
                            {
                                imgIcon.ImageUrl = objPortalModule.TemplateSourceDirectory + "/" + objPortalModule.ModuleConfiguration.IconFile;
                            }
                            else
                            {
                                if( objPortalModule.PortalSettings.ActiveTab.IsAdminTab )
                                {
                                    imgIcon.ImageUrl = "~/images/" + objPortalModule.ModuleConfiguration.IconFile;
                                }
                                else
                                {
                                    imgIcon.ImageUrl = objPortalModule.PortalSettings.HomeDirectory + objPortalModule.ModuleConfiguration.IconFile;
                                }
                            }
                        }
                        imgIcon.AlternateText = objPortalModule.ModuleConfiguration.ModuleTitle;
                        this.Visible = true;
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}