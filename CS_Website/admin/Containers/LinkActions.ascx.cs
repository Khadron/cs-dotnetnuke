using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.UI.Containers
{
    public partial class LinkActions : ActionBase
    {
        protected string _itemSeparator = "";

        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }

        protected void Page_PreRender( object sender, EventArgs e )
        {
            //Put user code to initialize the page here
            try
            {
                if( _itemSeparator.Length == 0 )
                {
                    _itemSeparator = "&nbsp;&nbsp;";
                }

                BindLinkList();
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        private void BindLinkList()
        {
            object with_1 = m_menuActionRoot;
            // Is Root Menu visible?
            if( ( (ModuleAction)with_1 ).Visible )
            {
                if( this.Controls.Count > 0 )
                {
                    this.Controls.Clear();
                }

                LiteralControl PreSpacer = new LiteralControl( ItemSeparator );
                this.Controls.Add( PreSpacer );

                // Add Menu Items
                ModuleAction action;
                foreach( ModuleAction tempLoopVar_action in m_menuActionRoot.Actions )
                {
                    action = tempLoopVar_action;
                    if( action.Title == "~" )
                    {
                        // not supported in this Action object
                    }
                    else
                    {
                        UserInfo _UserInfo = UserController.GetCurrentUserInfo();
                        if( action.Visible && PortalSecurity.HasNecessaryPermission( action.Secure, ( (PortalSettings)HttpContext.Current.Items["PortalSettings"] ), ModuleConfiguration, _UserInfo.UserID.ToString() ) )
                        {
                            if( ( EditMode == true && PortalModule.PortalSettings.ActiveTab.IsAdminTab == false && Globals.IsAdminControl() == false ) || ( action.Secure != SecurityAccessLevel.Anonymous && action.Secure != SecurityAccessLevel.View ) )
                            {
                                LinkButton ModuleActionLink = new LinkButton();
                                ModuleActionLink.Text = action.Title;
                                ModuleActionLink.CssClass = "CommandButton";
                                ModuleActionLink.ID = "lnk" + action.ID.ToString();

                                ModuleActionLink.Click += new EventHandler( LinkAction_Click );

                                this.Controls.Add( ModuleActionLink );
                                LiteralControl Spacer = new LiteralControl( ItemSeparator );
                                this.Controls.Add( Spacer );
                            }
                        }
                    }
                }
            }

            //Need to determine if this action list actually has any items.
            if( this.Controls.Count > 0 )
            {
                this.Visible = true;
            }
            else
            {
                this.Visible = false;
            }
        }

        private void LinkAction_Click( object sender, EventArgs e )
        {
            try
            {
                ProcessAction( ( (LinkButton)sender ).ID.Substring( 3 ) );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        public string ItemSeparator
        {
            get
            {
                return _itemSeparator;
            }
            set
            {
                _itemSeparator = value;
            }
        }
    }
}