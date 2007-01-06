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
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The PropertyLabelControl control provides a standard UI component for displaying
    /// a label for a property. It contains a Label and Help Text and can be Data Bound.
    /// </Summary>
    [ToolboxData( "<{0}:PropertyLabelControl runat=server></{0}:PropertyLabelControl>" )]
    public class PropertyLabelControl : WebControl
    {
        private string _DataField;

        //Data
        private object _DataSource;

        //Edit Control
        private Control _EditControl;

        //Styles
        private Style _HelpStyle = new Style();
        private Style _LabelStyle = new Style();

        //Localization
        private string _ResourceKey;

        //Label Help icon
        protected LinkButton cmdHelp;
        protected Image imgHelp;
        //Label container <label>
        protected HtmlGenericControl label;
        protected Label lblHelp;

        //Label Text
        protected Label lblLabel;

        //Help
        protected Panel pnlHelp;

        /// <Summary>
        /// Gets and Sets the Caption Text if no ResourceKey is provided
        /// </Summary>
        [Description( "Enter Caption for the control." ), DefaultValueAttribute( "Property" ), BrowsableAttribute( true ), CategoryAttribute( "Appearance" )]
        public string Caption
        {
            get
            {
                this.EnsureChildControls();
                return this.lblLabel.Text;
            }
            set
            {
                this.EnsureChildControls();
                this.lblLabel.Text = value;
                this.cmdHelp.ToolTip = value;
            }
        }

       

        /// <Summary>
        /// Gets and sets the value of the Field that is bound to the Label
        /// </Summary>
        [BrowsableAttribute( true ), CategoryAttribute( "Data" ), DescriptionAttribute( "Enter the name of the field that is data bound to the Label's Text property." ), DefaultValueAttribute( "" )]
        public string DataField
        {
            get
            {
                return this._DataField;
            }
            set
            {
                this._DataField = value;
            }
        }

        /// <Summary>
        /// Gets and sets the DataSource that is bound to this control
        /// </Summary>
        [BrowsableAttribute( false )]
        public object DataSource
        {
            get
            {
                return this._DataSource;
            }
            set
            {
                this._DataSource = value;
            }
        }

        /// <Summary>Gets and Sets the related Edit Control</Summary>
        [BrowsableAttribute( false )]
        public Control EditControl
        {
            get
            {
                return this._EditControl;
            }
            set
            {
                this._EditControl = value;
            }
        }

        /// <Summary>Gets and sets the value of the Label Style</Summary>
        [BrowsableAttribute( true ), DescriptionAttribute( "Set the Style for the Help Text." ), TypeConverterAttribute( typeof( ExpandableObjectConverter ) ), DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content ), CategoryAttribute( "Styles" )]
        public Style HelpStyle
        {
            get
            {
                this.EnsureChildControls();
                return this.pnlHelp.ControlStyle;
            }
        }

        /// <Summary>
        /// Text is value of the Label Text if no ResourceKey is provided
        /// </Summary>
        [DescriptionAttribute( "Enter Help Text for the control." ), DefaultValueAttribute( "" ), BrowsableAttribute( true ), CategoryAttribute( "Appearance" )]
        public string HelpText
        {
            get
            {
                this.EnsureChildControls();
                return this.lblHelp.Text;
            }
            set
            {
                this.EnsureChildControls();
                this.lblHelp.Text = value;
            }
        }

        

        /// <Summary>Gets and sets the value of the Label Style</Summary>
        [DescriptionAttribute( "Set the Style for the Label Text" ), CategoryAttribute( "Styles" ), TypeConverterAttribute( typeof( ExpandableObjectConverter ) ), DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content ), BrowsableAttribute( true )]
        public Style LabelStyle
        {
            get
            {
                this.EnsureChildControls();
                return this.lblLabel.ControlStyle;
            }
        }

        

        /// <Summary>ResourceKey is the root localization key for this control</Summary>
        [DescriptionAttribute( "Enter the Resource key for the control." ), CategoryAttribute( "Localization" ), BrowsableAttribute( true ), DefaultValueAttribute( "" )]
        public string ResourceKey
        {
            get
            {
                return this._ResourceKey;
            }
            set
            {
                this._ResourceKey = value;
                this.EnsureChildControls();
                this.lblHelp.Attributes["resourcekey"] = ( this._ResourceKey + ".Help" );
                this.lblLabel.Attributes["resourcekey"] = ( this._ResourceKey + ".Text" );
            }
        }

        [CategoryAttribute( "Behavior" ), BrowsableAttribute( true ), DescriptionAttribute( "Set whether the Help icon is displayed." ), DefaultValueAttribute( false )]
        public bool ShowHelp
        {
            get
            {
                this.EnsureChildControls();
                return this.cmdHelp.Visible;
            }
            set
            {
                this.EnsureChildControls();
                this.cmdHelp.Visible = value;
            }
        }

        public PropertyLabelControl()
        {
            this._HelpStyle = new Style();
            this._LabelStyle = new Style();
        }

        /// <Summary>CreateChildControls creates the control collection.</Summary>
        protected override void CreateChildControls()
        {
            //Initialise the Label container
            label = new HtmlGenericControl();
            label.TagName = "label";

            if (!DesignMode)
            {
                //Initialise Help LinkButton
                cmdHelp = new LinkButton();
                cmdHelp.ID = this.ID + "_cmdHelp";
                cmdHelp.CausesValidation = false;
                cmdHelp.EnableViewState = false;
                cmdHelp.TabIndex = -1;

                //Initialise Help Image and add to Help LinkButton
                imgHelp = new Image();
                imgHelp.ID = this.ID + "_imgHelp";
                imgHelp.EnableViewState = false;
                imgHelp.ImageUrl = "~/images/help.gif";
                imgHelp.TabIndex = -1;
                cmdHelp.Controls.Add(imgHelp);

                //Add Help LinkButton to Label container
                label.Controls.Add(cmdHelp);

                label.Controls.Add(new LiteralControl("&nbsp;"));
            }

            //Initialise Label
            lblLabel = new Label();
            lblLabel.ID = this.ID + "_label";
            lblLabel.EnableViewState = false;
            label.Controls.Add(lblLabel);

            //Initialise Help Panel
            pnlHelp = new Panel();
            pnlHelp.ID = this.ID + "_pnlHelp";
            pnlHelp.EnableViewState = false;

            //Initialise Help Label
            lblHelp = new Label();
            lblHelp.ID = this.ID + "_lblHelp";
            lblHelp.EnableViewState = false;
            pnlHelp.Controls.Add(lblHelp);

            this.Controls.Add(label);
            this.Controls.Add(pnlHelp);
        }

        /// <Summary>
        /// OnDataBinding runs when the Control is being Data Bound (It is triggered by
        /// a call to Control.DataBind()
        /// </Summary>
        protected override void OnDataBinding( EventArgs e )
        {
            //If there is a DataSource bind the relevent Properties
            if (DataSource != null)
            {
                //Make sure the Child Controls are created before assigning any properties
                this.EnsureChildControls();

                if (!String.IsNullOrEmpty(DataField))
                {
                    //DataBind the Label (via the Resource Key)
                    DataRowView dataRow = (DataRowView)DataSource;
                    if (ResourceKey == string.Empty)
                    {
                        ResourceKey = Convert.ToString(dataRow[DataField]);
                    }
                    if (DesignMode)
                    {
                        label.InnerText = Convert.ToString(dataRow[DataField]);
                    }
                }
            }
        }

        /// <Summary>
        /// OnLoad runs just before the Control is rendered, and makes sure that any
        /// properties are set properly before the control is rendered
        /// </Summary>
        protected override void OnPreRender( EventArgs e )
        {
            //Make sure the Child Controls are created before assigning any properties
            this.EnsureChildControls();

            //Set up client-side script
            DNNClientAPI.EnableMinMax(cmdHelp, pnlHelp, true, DNNClientAPI.MinMaxPersistanceType.None);

            if (EditControl != null)
            {
                label.Attributes.Add("for", EditControl.ClientID);
            }
        }

        /// <Summary>
        /// Render is called by the .NET framework to render the control
        /// </Summary>
        protected override void Render( HtmlTextWriter writer )
        {
            base.Render( writer );
        }
    }
}