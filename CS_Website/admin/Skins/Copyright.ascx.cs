using System;
using System.Diagnostics;
using DotNetNuke.Services.Localization;
using Microsoft.VisualBasic;

namespace DotNetNuke.UI.Skins.Controls
{
    /// <summary></summary>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class Copyright : SkinObjectBase
    {
        // private members
        private string _cssClass;

        private const string MyFileName = "Copyright.ascx";

        // protected controls

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
            // public attributes
            if( CssClass != "" )
            {
                lblCopyright.CssClass = CssClass;
            }

            if( PortalSettings.FooterText != "" )
            {
                lblCopyright.Text = PortalSettings.FooterText;
            }
            else
            {
                lblCopyright.Text = string.Format( Localization.GetString( "Copyright", Localization.GetResourceFile( this, MyFileName ) ), DateAndTime.Year( DateTime.Now ), PortalSettings.PortalName );
            }
        }
    }
}