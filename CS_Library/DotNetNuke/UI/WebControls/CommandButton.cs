using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The CommandButton Class provides an enhanced Button control for DotNetNuke
    /// </Summary>
    [ToolboxData( "<{0}:CommandButton runat=server></{0}:CommandButton>" )]
    public class CommandButton : WebControl
    {

        public event EventHandler Click
        {
            add
            {
                this.ClickEvent += value;
            }
            remove
            {
                this.ClickEvent -= value;
            }
        }
        private string _onClick;
        private string _resourceKey;
        private EventHandler ClickEvent;
        private ImageButton icon;
        private LinkButton link;

        /// <Summary>
        /// Gets or sets whether the control causes Validation to occur
        /// </Summary>
        public bool CausesValidation
        {
            get
            {
                this.EnsureChildControls();
                return this.link.CausesValidation;
            }
            set
            {
                this.EnsureChildControls();
                this.icon.CausesValidation = value;
                this.link.CausesValidation = value;
            }
        }

        public string CommandName
        {
            get
            {
                this.EnsureChildControls();
                return this.link.CommandName;
            }
            set
            {
                this.EnsureChildControls();
                this.icon.CommandName = value;
                this.link.CommandName = value;
            }
        }

        /// <Summary>Gets or sets whether the icon is displayed</Summary>
        public bool DisplayIcon
        {
            get
            {
                this.EnsureChildControls();
                return this.icon.Visible;
            }
            set
            {
                this.EnsureChildControls();
                this.icon.Visible = value;
            }
        }

        /// <Summary>Gets or sets whether the link is displayed</Summary>
        public bool DisplayLink
        {
            get
            {
                this.EnsureChildControls();
                return this.link.Visible;
            }
            set
            {
                this.EnsureChildControls();
                this.link.Visible = value;
            }
        }

        /// <Summary>Gets or sets the Image used for the Icon</Summary>
        public string ImageUrl
        {
            get
            {
                this.EnsureChildControls();
                return this.icon.ImageUrl;
            }
            set
            {
                this.EnsureChildControls();
                this.icon.ImageUrl = value;
            }
        }

        /// <Summary>Gets or sets the "onClick" Attribute</Summary>
        public string OnClick
        {
            get
            {
                this.EnsureChildControls();
                return this.link.Attributes["onclick"];
            }
            set
            {
                this.EnsureChildControls();
                if (value == "")
                {
                    icon.Attributes.Remove("onclick");
                    link.Attributes.Remove("onclick");
                }
                else
                {
                    icon.Attributes.Add("onclick", value);
                    link.Attributes.Add("onclick", value);
                }
            }
        }

        /// <Summary>Gets or sets the Resource Key used for the Control</Summary>
        public string ResourceKey
        {
            get
            {
                this.EnsureChildControls();
                return this.link.Attributes["resourcekey"];
            }
            set
            {
                this.EnsureChildControls();
                if (value == "")
                {
                    icon.Attributes.Remove("resourcekey");
                    link.Attributes.Remove("resourcekey");
                }
                else
                {
                    icon.Attributes.Add("resourcekey", value);
                    link.Attributes.Add("resourcekey", value);
                }
            }
        }

        /// <Summary>Gets or sets the Text used for the Control</Summary>
        public string Text
        {
            get
            {
                this.EnsureChildControls();
                return this.link.Text;
            }
            set
            {
                this.EnsureChildControls();
                this.icon.ToolTip = value;
                this.link.Text = value;
                this.link.ToolTip = value;
            }
        }

        public CommandButton()
        {
            this.link = new LinkButton();
            this.icon = new ImageButton();
        }

        /// <Summary>
        /// CreateChildControls overrides the Base class's method to correctly build the
        /// control based on the configuration
        /// </Summary>
        protected override void CreateChildControls()
        {
            if (CssClass == "")
            {
                CssClass = "CommandButton";
            }

            icon.Visible = true;
            icon.CausesValidation = true;
            icon.Click += new ImageClickEventHandler(RaiseImageClick);
            this.Controls.Add(icon);

            this.Controls.Add(new LiteralControl("&nbsp;"));

            link.Visible = true;
            link.CausesValidation = true;
            link.Click += new EventHandler(RaiseClick);
            this.Controls.Add(link);

            if (DisplayIcon == true && ImageUrl != "")
            {
                icon.EnableViewState = this.EnableViewState;
            }

            if (DisplayLink)
            {
                link.CssClass = CssClass;
                link.EnableViewState = this.EnableViewState;
            }
        }

        /// <Summary>
        /// RaiseClick runs when one of the contained Link buttons is clciked
        /// </Summary>
        /// <Param name="sender">The object that triggers the event</Param>
        /// <Param name="e">An EventArgs object</Param>
        private void RaiseClick( object sender, EventArgs e )
        {
            if (ClickEvent != null)
            {
                ClickEvent(this, e);
            }
        }

        /// <Summary>RaiseImageClick runs when the Image button is clicked</Summary>
        /// <Param name="sender">The object that triggers the event</Param>
        /// <Param name="e">An ImageClickEventArgs object</Param>
        private void RaiseImageClick( object sender, ImageClickEventArgs e )
        {
            if (ClickEvent != null)
            {
                ClickEvent(this, new EventArgs());
            }
        }
    }
}