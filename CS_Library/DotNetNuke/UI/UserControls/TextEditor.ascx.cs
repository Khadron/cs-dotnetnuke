#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
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
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.HTMLEditorProvider;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Personalization;

namespace DotNetNuke.UI.UserControls
{

    /// <Summary>
    /// TextEditor is a user control that provides a wrapper for the HtmlEditor providers
    /// </Summary>
    [ValidationProperty( "Text" )]
    public partial class TextEditor : UserControl
    {
        private bool _ChooseMode = true;
        private bool _ChooseRender = true;
        private Unit _Height;
        private bool _HtmlEncode = true;
        private Unit _Width;
        protected System.Web.UI.HtmlControls.HtmlTable tblTextEditor;
        protected System.Web.UI.HtmlControls.HtmlTableCell celTextEditor;
        private string MyFileName = "TextEditor.ascx";
        protected RadioButtonList optRender;
        protected RadioButtonList optView;
        protected PlaceHolder plcEditor;
        protected Panel pnlBasicRender;
        protected Panel pnlBasicTextBox;
        protected Panel pnlOption;
        protected Panel pnlRichTextBox;
        private HtmlEditorProvider RichTextEditor;
        protected TextBox txtDesktopHTML;

        //Enables/Disables the option to allow the user to select between Rich/Basic Mode
        //Default is true.
        public bool ChooseMode
        {
            get
            {
                return this._ChooseMode;
            }
            set
            {
                this._ChooseMode = value;
            }
        }

        //Determines wether or not the Text/Html button is rendered for Basic mode
        //Default is True
        public bool ChooseRender
        {
            get
            {
                return this._ChooseRender;
            }
            set
            {
                this._ChooseRender = value;
            }
        }
        
        //Gets/Sets the Default mode of the control, either "RICH" or "BASIC"
        //Defaults to Rich
        public string DefaultMode
        {
            get
            {
                if (Convert.ToString(this.ViewState["DefaultMode"]) == "")
                {
                    return "RICH"; // default to rich
                }
                else
                {
                    return Convert.ToString(this.ViewState["DefaultMode"]);
                }
            }
            set
            {
                if (value.ToUpper() != "BASIC")
                {
                    this.ViewState["DefaultMode"] = "RICH";
                }
                else
                {
                    this.ViewState["DefaultMode"] = "BASIC";
                }
            }
        }

        //Gets/Sets the Height of the control
        public Unit Height
        {
            get
            {
                return this._Height;
            }
            set
            {
                this._Height = value;
            }
        }

        //Turns on HtmlEncoding of text.  If this option is on the control will assume
        //it is being passed encoded text and will decode.
        public bool HtmlEncode
        {
            get
            {
                return this._HtmlEncode;
            }
            set
            {
                this._HtmlEncode = value;
            }
        }

        //The current mode of the control "RICH",  "BASIC"
        public string Mode
        {
            get
            {
                string strMode = "";
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();

                //Check if Personal Preference is set
                if (objUserInfo.UserID >= 0)
                {
                    if (Personalization.GetProfile("DotNetNuke.TextEditor", "PreferredTextEditor") != null)
                    {
                        strMode = Convert.ToString(Personalization.GetProfile("DotNetNuke.TextEditor", "PreferredTextEditor"));
                    }
                }

                //If no Preference Check if Viewstate has been saved
                if (strMode == null || strMode == "")
                {
                    if (Convert.ToString(this.ViewState["DesktopMode"]) != "")
                    {
                        strMode = Convert.ToString(this.ViewState["DesktopMode"]);
                    }
                }

                //Finally if still no value Use default
                if (strMode == null || strMode == "")
                {
                    strMode = DefaultMode;
                }

                return strMode;
            }
            set
            {
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();

                if (value.ToUpper() != "BASIC")
                {
                    this.ViewState["DesktopMode"] = "RICH";

                    if (objUserInfo.UserID >= 0)
                    {
                        Personalization.SetProfile("DotNetNuke.TextEditor", "PreferredTextEditor", "RICH");
                    }
                }
                else
                {
                    this.ViewState["DesktopMode"] = "BASIC";

                    if (objUserInfo.UserID >= 0)
                    {
                        Personalization.SetProfile("DotNetNuke.TextEditor", "PreferredTextEditor", "BASIC");
                    }
                }
            }
        }




        //Allows public access ot the HtmlEditorProvider
        public HtmlEditorProvider RichText
        {
            get
            {
                return this.RichTextEditor;
            }
        }
        //Gets/Sets the Text of the control
        public string Text
        {
            get
            {
                string returnValue;
                if (optView.SelectedItem.Value == "BASIC")
                {
                    switch (optRender.SelectedItem.Value)
                    {
                        case "T":

                            returnValue = Encode(FormatHtml(txtDesktopHTML.Text));
                            break;
                        case "R":

                            returnValue = txtDesktopHTML.Text;
                            break;
                        default:

                            returnValue = Encode(txtDesktopHTML.Text);
                            break;
                    }
                }
                else
                {
                    returnValue = Encode(RichTextEditor.Text);
                }
                return returnValue;
            }
            set
            {

                if (!String.IsNullOrEmpty(value))
                {
                    txtDesktopHTML.Text = Decode(FormatText(value));
                    RichTextEditor.Text = Decode(value);
                }
            }
        }
        //Sets the render mode for Basic mode.  {Raw | HTML | Text}
        public string TextRenderMode
        {
            get
            {
                return Convert.ToString(this.ViewState["textrender"]);
            }
            set
            {
                string strMode;

                strMode = value.ToUpper().Substring(0, 1);
                if (strMode != "R" && strMode != "H" && strMode != "T")
                {
                    strMode = "H";
                }

                this.ViewState["textrender"] = strMode;
            }
        }


        //Gets/Sets the Width of the control
        public Unit Width
        {
            get
            {
                return this._Width;
            }
            set
            {
                this._Width = value;
            }
        }

        public TextEditor()
        {
            Init += new EventHandler(this.Page_Init);
            Load += new EventHandler( this.Page_Load );
            
            this._ChooseMode = true;
            this._ChooseRender = true;
            this._HtmlEncode = true;
        }

        /// <Summary>Decodes the html</Summary>
        /// <Param name="strHtml">Html to decode</Param>
        /// <Returns>The decoded html</Returns>
        private string Decode( string strHtml )
        {
            if( this.HtmlEncode )
            {
                return this.Server.HtmlDecode( strHtml );
            }
            else
            {
                return strHtml;
            }
        }

        /// <Summary>Encodes the html</Summary>
        /// <Param name="strHtml">Html to encode</Param>
        /// <Returns>The encoded html</Returns>
        private string Encode( string strHtml )
        {
            if( this.HtmlEncode )
            {
                return this.Server.HtmlEncode( strHtml );
            }
            else
            {
                return strHtml;
            }
        }

        /// <Summary>Formats String as Html by replacing linefeeds by</Summary>
        /// <Param name="strText">Text to format</Param>
        /// <Returns>The formatted html</Returns>
        private string FormatHtml( string strText )
        {
            
            string strHtml = strText;
            try
            {
                if (!String.IsNullOrEmpty(strHtml))
                {
                    strHtml = strHtml.Replace("\r", "");
                    strHtml = strHtml.Replace("\n", "<br />");
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return strHtml;
        }

        /// <Summary>
        /// Formats Html as text by removing  tags and replacing by linefeeds
        /// </Summary>
        /// <Param name="strHtml">Html to format</Param>
        /// <Returns>The formatted text</Returns>
        private string FormatText( string strHtml )
        {
            string strText = strHtml;
            try
            {
                if (!String.IsNullOrEmpty(strText))
                {
                    //First remove white space (html does not render white space anyway and it screws up the conversion to text)
                    //Replace it by a single space
                    strText = HtmlUtils.StripWhiteSpace(strText, true);

                    //Replace all variants of <br> by Linefeeds
                    strText = HtmlUtils.FormatText(strText, false);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            return strText;
        }

        /// <Summary>
        /// optRender_SelectedIndexChanged runs when Basic Text Box mode is changed
        /// </Summary>
        protected void optRender_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (optRender.SelectedIndex != -1)
            {
                TextRenderMode = optRender.SelectedItem.Value;
            }

            if (Mode == "BASIC")
            {
                if (TextRenderMode == "H")
                {
                    txtDesktopHTML.Text = FormatHtml(txtDesktopHTML.Text);
                }
                else
                {
                    txtDesktopHTML.Text = FormatText(txtDesktopHTML.Text);
                }
            }
            SetPanels();
        }

        /// <Summary>
        /// optView_SelectedIndexChanged runs when Editor Mode is changed
        /// </Summary>
        protected void optView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (optView.SelectedIndex != -1)
            {
                Mode = optView.SelectedItem.Value;
            }

            if (Mode == "BASIC")
            {
                if (TextRenderMode == "T")
                {
                    txtDesktopHTML.Text = FormatText(RichTextEditor.Text);
                }
                else
                {
                    txtDesktopHTML.Text = RichTextEditor.Text;
                }
            }
            else
            {
                if (TextRenderMode == "T")
                {
                    RichTextEditor.Text = FormatHtml(txtDesktopHTML.Text);
                }
                else
                {
                    RichTextEditor.Text = txtDesktopHTML.Text;
                }
            }
            SetPanels();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            this.RichTextEditor = HtmlEditorProvider.Instance();
            this.RichTextEditor.ControlID = this.ID;
            this.RichTextEditor.Initialize();

            this.optRender.SelectedIndexChanged += new EventHandler(this.optRender_SelectedIndexChanged);
            this.optView.SelectedIndexChanged += new EventHandler(this.optView_SelectedIndexChanged);            
        }

        /// <Summary>Page_Load runs when the control is loaded</Summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Populate Radio Button Lists
                PopulateLists();

                //Get the current user
//                UserInfo objUserInfo = UserController.GetCurrentUserInfo();

                //Set the width and height of the controls
                RichTextEditor.Width = Width;
                RichTextEditor.Height = Height;
                txtDesktopHTML.Height = Height;
                txtDesktopHTML.Width = Width;
                tblTextEditor.Width = Width.ToString();
                celTextEditor.Width = Width.ToString();

                //Optionally display the radio button lists
                if (!ChooseMode)
                {
                    pnlOption.Visible = false;
                }
                if (!ChooseRender)
                {
                    pnlBasicRender.Visible = false;
                }

                //Load the editor
                plcEditor.Controls.Add(RichTextEditor.HtmlEditorControl);

                SetPanels();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <Summary>Builds the radio button lists</Summary>
        private void PopulateLists()
        {
            if (optRender.Items.Count == 0)
            {
                optRender.Items.Add(new ListItem(Localization.GetString("Text", Localization.GetResourceFile(this, MyFileName)), "T"));
                optRender.Items.Add(new ListItem(Localization.GetString("Html", Localization.GetResourceFile(this, MyFileName)), "H"));
                optRender.Items.Add(new ListItem(Localization.GetString("Raw", Localization.GetResourceFile(this, MyFileName)), "R"));
            }
            if (optView.Items.Count == 0)
            {
                optView.Items.Add(new ListItem(Localization.GetString("BasicTextBox", Localization.GetResourceFile(this, MyFileName)), "BASIC"));
                optView.Items.Add(new ListItem(Localization.GetString("RichTextBox", Localization.GetResourceFile(this, MyFileName)), "RICH"));
            }
        }

        /// <Summary>Sets the Mode displayed</Summary>
        private void SetPanels()
        {
            if (optView.SelectedIndex != -1)
            {
                Mode = optView.SelectedItem.Value;
            }
            if (!String.IsNullOrEmpty(Mode))
            {
                optView.Items.FindByValue(Mode).Selected = true;
            }
            else
            {
                optView.SelectedIndex = 0;
            }

            //Set the text render mode for basic mode
            if (optRender.SelectedIndex != -1)
            {
                TextRenderMode = optRender.SelectedItem.Value;
            }
            if (!String.IsNullOrEmpty(TextRenderMode))
            {
                optRender.Items.FindByValue(TextRenderMode).Selected = true;
            }
            else
            {
                optRender.SelectedIndex = 0;
            }

            if (optView.SelectedItem.Value == "BASIC")
            {
                pnlBasicTextBox.Visible = true;
                pnlRichTextBox.Visible = false;
            }
            else
            {
                pnlBasicTextBox.Visible = false;
                pnlRichTextBox.Visible = true;
            }
        }
    }
}