using System;
using System.Diagnostics;
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
    public partial class HostName : SkinObjectBase
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
            try
            {
                // public attributes
                if( CssClass != "" )
                {
                    hypHostName.CssClass = CssClass;
                }

                hypHostName.Text = Convert.ToString( Globals.HostSettings["HostTitle"] );
                hypHostName.NavigateUrl = Globals.AddHTTP(Convert.ToString(Globals.HostSettings["HostURL"]));
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}