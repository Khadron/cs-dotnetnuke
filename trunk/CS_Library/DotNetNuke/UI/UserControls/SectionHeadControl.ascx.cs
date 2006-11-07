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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.UI.UserControls
{

    /// <Summary>
    /// SectionHeadControl is a user control that provides all the server code to allow a
    /// section to be collapsed/expanded, using user provided images for the button.
    /// </Summary>
    public class SectionHeadControl : UserControl
    {
        private bool _includeRule = false;
        private bool _isExpanded = true;
        private string _javaScript = "__dnn_SectionMaxMin";
        private string _maxImageUrl = "images/plus.gif";
        private string _minImageUrl = "images/minus.gif";
        private string _resourceKey;
        private string _section; //Associated Section for this Control
        protected ImageButton imgIcon;
        protected Label lblTitle;
        protected Panel pnlRule;

        /// <Summary>CssClass determines the Css Class used for the Title Text</Summary>
        public string CssClass
        {
            get
            {
                return this.lblTitle.CssClass;
            }
            set
            {
                this.lblTitle.CssClass = value;
            }
        }

        /// <Summary>
        /// IncludeRule determines whether there is a horizontal rule displayed under the
        /// header text
        /// </Summary>
        public bool IncludeRule
        {
            get
            {
                return this._includeRule;
            }
            set
            {
                this._includeRule = value;
            }
        }

        /// <Summary>
        /// IsExpanded determines whether the section is expanded or collapsed.
        /// </Summary>
        public bool IsExpanded
        {
            get
            {
                return DNNClientAPI.GetMinMaxContentVisibile( ( (Control)this.imgIcon ), ( ! this._isExpanded ), DNNClientAPI.MinMaxPersistanceType.Page );
            }
            set
            {
                this._isExpanded = value;
                DNNClientAPI.SetMinMaxContentVisibile( ( (Control)this.imgIcon ), ( ! this._isExpanded ), DNNClientAPI.MinMaxPersistanceType.Page, value );
            }
        }

        /// <Summary>
        /// JavaScript is the name of the javascript function implementation.
        /// </Summary>
        public string JavaScript
        {
            get
            {
                return this._javaScript;
            }
            set
            {
                this._javaScript = value;
            }
        }

       

        /// <Summary>
        /// The MaxImageUrl is the url of the image displayed when the contained panel is
        /// collapsed.
        /// </Summary>
        public string MaxImageUrl
        {
            get
            {
                return this._maxImageUrl;
            }
            set
            {
                this._maxImageUrl = value;
            }
        }

        /// <Summary>
        /// The MinImageUrl is the url of the image displayed when the contained panel is
        /// expanded.
        /// </Summary>
        public string MinImageUrl
        {
            get
            {
                return this._minImageUrl;
            }
            set
            {
                this._minImageUrl = value;
            }
        }

        

        /// <Summary>
        /// The ResourceKey is the key used to identify the Localization Resource for the
        /// title text.
        /// </Summary>
        public string ResourceKey
        {
            get
            {
                return this._resourceKey;
            }
            set
            {
                this._resourceKey = value;
            }
        }

        /// <Summary>
        /// The Section is the Id of the DHTML object  that contains the xection content
        /// title text.
        /// </Summary>
        public string Section
        {
            get
            {
                return this._section;
            }
            set
            {
                this._section = value;
            }
        }

        /// <Summary>The Text is the name or title of the section</Summary>
        public string Text
        {
            get
            {
                return this.lblTitle.Text;
            }
            set
            {
                this.lblTitle.Text = value;
            }
        }

        public SectionHeadControl()
        {
            base.Load += new EventHandler( this.Page_Load );
            base.PreRender += new EventHandler( this.Page_PreRender );
            this.imgIcon.Click += new ImageClickEventHandler(this.imgIcon_Click);
            this._includeRule = false;
            this._isExpanded = true;

        }

        private void imgIcon_Click( object sender, ImageClickEventArgs e )
        {
            HtmlControl ctl = (HtmlControl)this.Parent.FindControl(Section);
            if (ctl != null)
            {
                this.IsExpanded = !this.IsExpanded;
            }
        }

        /// <Summary>Assign resource key to label for localization</Summary>
        private void Page_Load( object sender, EventArgs e )
        {
            try
            {
                //set the resourcekey attribute to the label
                if (ResourceKey != "")
                {
                    lblTitle.Attributes["resourcekey"] = ResourceKey;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <Summary>Renders the SectionHeadControl</Summary>
        private void Page_PreRender( object sender, EventArgs e )
        {
            try
            {
                HtmlControl ctl = (HtmlControl)this.Parent.FindControl(Section);
                if (ctl != null)
                {
                    DNNClientAPI.EnableMinMax(imgIcon, ctl, !IsExpanded, Page.ResolveUrl(MinImageUrl), Page.ResolveUrl(MaxImageUrl), DNNClientAPI.MinMaxPersistanceType.Page);
                }

                //optionlly show hr
                pnlRule.Visible = _includeRule;
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}