using System;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.UI.Skins.Controls
{
    public class ModuleMessage : SkinObjectBase
    {
        private string _heading;
        private string _iconImage;
        private ModuleMessageType _iconType;

        // private members
        private string _text;
        protected Image imgIcon;
        protected Image imgLogo;
        protected Label lblHeading;

        // protected controls
        protected Label lblMessage;

        public enum ModuleMessageType
        {
            GreenSuccess = 0,
            YellowWarning = 1,
            RedError = 2,
        }

        public ModuleMessage()
        {
            base.Init += new EventHandler( this.Page_Init );
            base.Load += new EventHandler( this.Page_Load );
        }

        public string Heading
        {
            get
            {
                return this._heading;
            }
            set
            {
                this._heading = value;
            }
        }

        public string IconImage
        {
            get
            {
                return this._iconImage;
            }
            set
            {
                this._iconImage = value;
            }
        }

        public ModuleMessageType IconType
        {
            get
            {
                return this._iconType;
            }
            set
            {
                this._iconType = value;
            }
        }

        

        public string Text
        {
            get
            {
                return this._text;
            }
            set
            {
                this._text = value;
            }
        }

        
        private void InitializeComponent()
        {
        }

        

        private void Page_Init( object sender, EventArgs e )
        {
            this.InitializeComponent();
        }

        private void Page_Load( object sender, EventArgs e )
        {
            try
            {
                string strMessage = "";

                //check to see if a url
                //was passed in for an icon
                if (IconImage != "")
                {
                    strMessage += this.Text;
                    lblHeading.CssClass = "SubHead";
                    lblMessage.CssClass = "Normal";
                    imgIcon.ImageUrl = IconImage;
                    imgIcon.Visible = true;
                }
                else
                {
                    switch (IconType)
                    {
                        case ModuleMessageType.GreenSuccess:

                            strMessage += this.Text;
                            lblHeading.CssClass = "SubHead";
                            lblMessage.CssClass = "Normal";
                            imgIcon.ImageUrl = "~/images/green-ok.gif";
                            imgIcon.Visible = true;
                            break;
                        case ModuleMessageType.YellowWarning:

                            strMessage += this.Text;
                            lblHeading.CssClass = "Normal";
                            lblMessage.CssClass = "Normal";
                            imgIcon.ImageUrl = "~/images/yellow-warning.gif";
                            imgIcon.Visible = true;
                            break;
                        case ModuleMessageType.RedError:

                            strMessage += this.Text;
                            lblHeading.CssClass = "NormalRed";
                            lblMessage.CssClass = "Normal";
                            imgIcon.ImageUrl = "~/images/red-error.gif";
                            imgIcon.Visible = true;
                            break;
                    }
                }
                lblMessage.Text = strMessage;
                if (Heading != "")
                {
                    lblHeading.Visible = true;
                    lblHeading.Text = Heading + "<br/>";
                }
            }
            catch (Exception exc) //Control failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc, false);
            }
        }
    }
}