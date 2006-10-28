using System;
using System.Diagnostics;
using DotNetNuke.Common;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.Skins.Controls
{
    /// <summary></summary>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class Privacy : SkinObjectBase
    {
        // private members
        private string _text;
        private string _cssClass;

        private const string MyFileName = "Privacy.ascx";

        // protected controls

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

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
            try
            {
                // public attributes
                if( CssClass != "" )
                {
                    hypPrivacy.CssClass = CssClass;
                }

                if( Text != "" )
                {
                    hypPrivacy.Text = Text;
                }
                else
                {
                    hypPrivacy.Text = Localization.GetString( "Privacy", Localization.GetResourceFile( this, MyFileName ) );
                }

                hypPrivacy.NavigateUrl = Globals.NavigateURL( "Privacy" );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}