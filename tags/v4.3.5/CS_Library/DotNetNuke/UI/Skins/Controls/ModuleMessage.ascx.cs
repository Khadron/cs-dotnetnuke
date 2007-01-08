#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
using System;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.UI.Skins.Controls
{
    public enum ModuleMessageType
    {
        GreenSuccess = 0,
        YellowWarning = 1,
        RedError = 2,
    }

    public partial class ModuleMessage : SkinObjectBase
    {
        protected Image imgIcon;
        protected Image imgLogo;
        protected Label lblHeading;
        protected Label lblMessage;

        private string _heading;
        private string _iconImage;
        private ModuleMessageType _iconType;

        // private members
        private string _text;      

        

        public ModuleMessage()
        {
           this._text = String.Empty;
           Load += new EventHandler( this.Page_Load );
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

        protected void Page_Load( object sender, EventArgs e )
        {
            try
            {
                string strMessage = String.Empty;

                //check to see if a url was passed in for an icon
                
                if (!String.IsNullOrEmpty(IconImage) )
                {
                    strMessage = this.Text;
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

                            strMessage = this.Text;
                            lblHeading.CssClass = "SubHead";
                            lblMessage.CssClass = "Normal";
                            imgIcon.ImageUrl = "~/images/green-ok.gif";
                            imgIcon.Visible = true;
                            break;
                        case ModuleMessageType.YellowWarning:

                            strMessage = this.Text;
                            lblHeading.CssClass = "Normal";
                            lblMessage.CssClass = "Normal";
                            imgIcon.ImageUrl = "~/images/yellow-warning.gif";
                            imgIcon.Visible = true;
                            break;
                        case ModuleMessageType.RedError:

                            strMessage = this.Text;
                            lblHeading.CssClass = "NormalRed";
                            lblMessage.CssClass = "Normal";
                            imgIcon.ImageUrl = "~/images/red-error.gif";
                            imgIcon.Visible = true;
                            break;
                    }
                }
                lblMessage.Text = strMessage;
                if (!String.IsNullOrEmpty(Heading))
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