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
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.UI.UserControls
{

    /// <Summary>
    /// LabelControl is a user control that provides all the server code to manage a
    /// label, including localization, 508 support and help.
    /// </Summary>
    public abstract class LabelControl : UserControl
    {
        private string _ControlName; //Associated Edit Control for this Label
        private string _HelpKey; //Resource Key for the Help Text
        private string _ResourceKey; //Resource Key for the Label Text
        private string _Suffix; //Optional Text that appears after the Localized Label Text
        protected LinkButton cmdHelp;
        protected Image imgHelp;
        protected HtmlGenericControl label;
        protected Label lblHelp;
        protected Label lblLabel;
        protected Panel pnlHelp;


        /// <Summary>
        /// ControlName is the Id of the control that is associated with the label
        /// </Summary>
        public string ControlName
        {
            get
            {
                return this._ControlName;
            }
            set
            {
                this._ControlName = value;
            }
        }

        /// <Summary>HelpKey is the Resource Key for the Help Text</Summary>
        public string HelpKey
        {
            get
            {
                return this._HelpKey;
            }
            set
            {
                this._HelpKey = value;
            }
        }

        /// <Summary>
        /// HelpText is value of the Help Text if no ResourceKey is provided
        /// </Summary>
        public string HelpText
        {
            get
            {
                return this.lblHelp.Text;
            }
            set
            {
                this.lblHelp.Text = value;
                this.imgHelp.AlternateText = HtmlUtils.Clean( value, false );
                //hide the help icon if the help text is ""
                if (value == "")
                {
                    imgHelp.Visible = false;
                }
            }
        }

        

        /// <Summary>ResourceKey is the Resource Key for the Label Text</Summary>
        public string ResourceKey
        {
            get
            {
                return this._ResourceKey;
            }
            set
            {
                this._ResourceKey = value;
            }
        }

        /// <Summary>
        /// Suffix is Optional Text that appears after the Localized Label Text
        /// </Summary>
        public string Suffix
        {
            get
            {
                return this._Suffix;
            }
            set
            {
                this._Suffix = value;
            }
        }

        /// <Summary>
        /// Text is value of the Label Text if no ResourceKey is provided
        /// </Summary>
        public string Text
        {
            get
            {
                return this.lblLabel.Text;
            }
            set
            {
                this.lblLabel.Text = value;
            }
        }

        public LabelControl()
        {
            base.Load += new EventHandler( this.Page_Load );
            this.cmdHelp.Click += new EventHandler(this.imageClick);
        }

        /// <Summary>
        /// GetLocalizedText gets the localized text for the provided key
        /// </Summary>
        /// <Param name="key">The resource key</Param>
        /// <Param name="ctl">The current control</Param>
        private string GetLocalizedText( string key, Control ctl )
        {
            //We need to find the parent module
            Control parentControl = ctl.Parent;
            string localizedText;

            if (parentControl is PortalModuleBase)
            {
                //We are at the Module Level so return key
                //Get Resource File Root from Parents LocalResourceFile Property
                PortalModuleBase ctrl;
                ctrl = (PortalModuleBase)parentControl;
                localizedText = Localization.GetString(key, ctrl.LocalResourceFile);
            }
            else
            {
                PropertyInfo pi = parentControl.GetType().GetProperty("LocalResourceFile");
                if (pi != null)
                {
                    //If control has a LocalResourceFile property use this
                    localizedText = Localization.GetString(key, pi.GetValue(parentControl, null).ToString());
                }
                else
                {
                    //Drill up to the next level
                    localizedText = GetLocalizedText(key, parentControl);
                }
            }

            return localizedText;
        }

        private void imageClick( object sender, EventArgs e )
        {
            this.pnlHelp.Visible = true;
        }

        /// <Summary>Page_Load runs when the control is loaded</Summary>
        private void Page_Load( object sender, EventArgs e )
        {
            try
            {
                DNNClientAPI.EnableMinMax(cmdHelp, pnlHelp, true, DNNClientAPI.MinMaxPersistanceType.None);

                //get the localised text
                if (_ResourceKey == "")
                {
                    //Set Resource Key to the ID of the control
                    _ResourceKey = this.ID;
                }
                string localText = GetLocalizedText(_ResourceKey, this);
                if (localText != "")
                {
                    this.Text = localText + _Suffix;
                }

                if (_HelpKey == "")
                {
                    //Set Help Key to the Resource Key plus ".Help"
                    _HelpKey = _ResourceKey + ".Help";
                }
                string helpText = GetLocalizedText(_HelpKey, this);
                if (helpText != "")
                {
                    this.HelpText = helpText;
                }

                //find the reference control in the parents Controls collection
                if (ControlName != "")
                {
                    Control c = this.Parent.FindControl(ControlName);
                    if (c != null)
                    {
                        label.Attributes["for"] = c.ClientID;
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}