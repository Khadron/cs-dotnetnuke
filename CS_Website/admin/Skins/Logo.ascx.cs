using System;
using System.Diagnostics;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.UI.Skins.Controls
{
    /// <summary></summary>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class Logo : SkinObjectBase
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

        //*******************************************************
        //
        // The Page_Load server event handler on this page is used
        // to populate the role information for the page
        //
        //*******************************************************

        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                // public attributes
                if( BorderWidth != "" )
                {
                    imgLogo.BorderWidth = Unit.Parse( BorderWidth );
                }

                if( PortalSettings.LogoFile != "" )
                {
                    imgLogo.ImageUrl = PortalSettings.HomeDirectory + PortalSettings.LogoFile;
                }
                else
                {
                    imgLogo.Visible = false;
                }
                imgLogo.AlternateText = PortalSettings.PortalName;

                hypLogo.ToolTip = PortalSettings.PortalName;
                hypLogo.NavigateUrl = Globals.GetPortalDomainName( PortalSettings.PortalAlias.HTTPAlias, Request, true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}