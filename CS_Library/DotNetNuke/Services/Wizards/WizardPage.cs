using System.Web.UI;

namespace DotNetNuke.Services.Wizards
{
    /// <Summary>The WizardPage class defines a Wizard Page.</Summary>
    public class WizardPage
    {
        private Control _Control;
        private string _Help;
        private string _Icon;
        private WizardPageType _PageType;
        private string _Title;

        /// <Summary>
        /// Default Constructor Builds a default WizardPageInfo object.
        /// </Summary>
        public WizardPage() : this( DotNetNuke.Services.Localization.Localization.GetString( "NoPage" ), "", ( (System.Web.UI.Control)null ), WizardPageType.Content, "" )
        {
        }

        /// <Summary>Constructor Builds a custom WizardPageInfo object.</Summary>
        /// <Param name="title">The title of the Page</Param>
        /// <Param name="icon">The icon for the Page</Param>
        /// <Param name="ctl">The control associated with the Page</Param>
        public WizardPage( string title, string icon, System.Web.UI.Control ctl ) : this( title, icon, ctl, WizardPageType.Content, "" )
        {
        }

        /// <Summary>Constructor Builds a custom WizardPageInfo object.</Summary>
        /// <Param name="title">The title of the Page</Param>
        /// <Param name="icon">The icon for the Page</Param>
        /// <Param name="ctl">The control associated with the Page</Param>
        /// <Param name="help">The Help text for the  Page</Param>
        public WizardPage( string title, string icon, System.Web.UI.Control ctl, string help ) : this( title, icon, ctl, WizardPageType.Content, help )
        {
        }

        /// <Summary>Constructor Builds a custom WizardPageInfo object.</Summary>
        /// <Param name="title">The title of the Page</Param>
        /// <Param name="icon">The icon for the Page</Param>
        /// <Param name="ctl">The control associated with the Page</Param>
        /// <Param name="type">The type of the Page</Param>
        public WizardPage( string title, string icon, System.Web.UI.Control ctl, WizardPageType type ) : this( title, icon, ctl, type, "" )
        {
        }

        /// <Summary>Constructor Builds a custom WizardPageInfo object.</Summary>
        /// <Param name="title">The title of the Page</Param>
        /// <Param name="icon">The icon for the Page</Param>
        /// <Param name="ctl">The control associated with the Page</Param>
        /// <Param name="type">The type of the Page</Param>
        /// <Param name="help">The Help text for the  Page</Param>
        public WizardPage( string title, string icon, System.Web.UI.Control ctl, WizardPageType type, string help )
        {
            this._Control = ctl;
            this._Help = help;
            this._Icon = icon;
            this._PageType = type;
            this._Title = title;
        }

        /// <Summary>Gets And Sets the Panel associated with the Wizard Page.</Summary>
        public Control Control
        {
            get
            {
                return this._Control;
            }
            set
            {
                this._Control = value;
            }
        }

        /// <Summary>Gets And Sets the Help associated with the Wizard Page.</Summary>
        public string Help
        {
            get
            {
                return this._Help;
            }
            set
            {
                this._Help = value;
            }
        }

        /// <Summary>Gets And Sets the Icon associated with the Wizard Page.</Summary>
        public string Icon
        {
            get
            {
                return this._Icon;
            }
            set
            {
                this._Icon = value;
            }
        }

        /// <Summary>Gets And Sets the Type of the Wizard Page.</Summary>
        public WizardPageType PageType
        {
            get
            {
                return this._PageType;
            }
            set
            {
                this._PageType = value;
            }
        }

        /// <Summary>Gets And Sets the Title of the Wizard Page.</Summary>
        public string Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }
    }
}