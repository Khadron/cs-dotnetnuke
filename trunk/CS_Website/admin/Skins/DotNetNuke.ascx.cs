using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.UI.Skins.Controls
{
    /// <summary></summary>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class DotNetNuke : SkinObjectBase
    {
        // private members
        private string _cssClass;

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
                hypDotNetNuke.CssClass = CssClass;
            }

            // get Product Name and Legal Copyright from constants (Medium Trust)
            hypDotNetNuke.Text = Strings.Replace( Globals.glbLegalCopyright, "YYYY", DateAndTime.Year( DateTime.Now ).ToString(), 1, -1, 0 );
            hypDotNetNuke.NavigateUrl = Globals.glbAppUrl;

            // show copyright credits?
            if (Globals.GetHashValue(Globals.HostSettings["Copyright"], "Y") != "Y")
            {
                this.Visible = false;
            }
        }
    }
}