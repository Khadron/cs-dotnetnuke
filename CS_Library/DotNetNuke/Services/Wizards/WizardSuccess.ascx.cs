using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Services.Wizards
{
    /// <Summary>
    /// The WizardSuccess UserControl provides a default success page for the Wizard
    /// Framework
    /// </Summary>
    public class WizardSuccess : UserControl
    {
        private Object designerPlaceholderDeclaration;
        protected Label lblDetail;
        protected Label lblMessage;
        protected Label lblTitle;
        private string m_Message = "";
        private bool m_Type = true; //True=Success,  False=Failure

        private string MyFileName = "Success.ascx";

        public WizardSuccess()
        {
            PreRender += new EventHandler( this.Page_PreRender );
            Init += new EventHandler( this.Page_Init );
            this.m_Message = "";
            this.m_Type = true;
        }

        
        private void InitializeComponent()
        {
        }

        protected void Page_Init( object sender, EventArgs e )
        {
            this.InitializeComponent();
        }

        /// <Summary>Page_PreRender runs just before the control is rendered</Summary>
        protected void Page_PreRender( object sender, EventArgs e )
        {
            if( this.m_Type )
            {
                this.lblTitle.Text = Localization.Localization.GetString( "Wizard.Success.SuccessTitle" );
                this.lblDetail.Text = Localization.Localization.GetString( "Wizard.Success.SuccessDetail" );
                this.lblMessage.CssClass = "WizardText";
            }
            else
            {
                this.lblTitle.Text = Localization.Localization.GetString( "Wizard.Success.FailureTitle" );
                this.lblDetail.Text = Localization.Localization.GetString( "Wizard.Success.FailureDetail" );
                this.lblMessage.CssClass = "NormalRed";
            }
            this.lblMessage.Text = this.m_Message;
        }

        

        public string Message
        {
            get
            {
                return this.m_Message;
            }
            set
            {
                this.m_Message = value;
            }
        }

        public bool Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }
    }
}