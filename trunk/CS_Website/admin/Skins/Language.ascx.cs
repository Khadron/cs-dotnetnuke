using System;
using System.Diagnostics;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.Skins.Controls
{
    public partial class Language : SkinObjectBase
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
            if( ! Page.IsPostBack )
            {
                // public attributes
                if( CssClass != "" )
                {
                    selectCulture.CssClass = CssClass;
                }

                Localization.LoadCultureDropDownList( selectCulture, CultureDropDownTypes.NativeName, ( (PageBase)Page ).PageCulture.Name );

                //only show language selector if more than one language
                if( selectCulture.Items.Count <= 1 )
                {
                    selectCulture.Visible = false;
                }
            }
        }

        protected void selectCulture_SelectedIndexChanged( object sender, EventArgs e )
        {
            // Store selected language in cookie
            Localization.SetLanguage( selectCulture.SelectedItem.Value );

            //Redirect to same page to update all controls for newly selected culture
            Response.Redirect( Request.RawUrl, true );
        }
    }
}