using System;
using System.Diagnostics;
using DotNetNuke.Common;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.Skins.Controls
{
    /// <summary></summary>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class User : SkinObjectBase
    {
        private const string MyFileName = "User.ascx";

        private string _text;
        private string _cssClass;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        public string CssClass
        {
            get
            {
                return _cssClass;
            }
            set
            {
                _cssClass = value;
            }
        }

        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                // public attributes
                if( CssClass != "" )
                {
                    hypRegister.CssClass = CssClass;
                }

                if( Request.IsAuthenticated == false )
                {
                    if( PortalSettings.UserRegistration != (int)Globals.PortalRegistrationType.NoRegistration )
                    {
                        if( Text != "" )
                        {
                            if( Text.IndexOf( "src=" ) != - 1 )
                            {
                                Text = Text.Replace( "src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath );
                            }
                            hypRegister.Text = Text;
                        }
                        else
                        {
                            hypRegister.Text = Localization.GetString( "Register", Localization.GetResourceFile( this, MyFileName ) );
                        }
                        if( PortalSettings.UserTabId != - 1 )
                        {
                            // user defined tab
                            hypRegister.NavigateUrl = Globals.NavigateURL( PortalSettings.UserTabId );
                        }
                        else
                        {
                            // admin tab
                            hypRegister.NavigateUrl = Globals.NavigateURL( "Register" );
                        }
                        hypRegister.Visible = true;
                    }
                    else
                    {
                        hypRegister.Visible = false;
                    }
                }
                else
                {
                    UserInfo objUserInfo = UserController.GetCurrentUserInfo();

                    if( objUserInfo.UserID != - 1 )
                    {
                        hypRegister.Text = objUserInfo.DisplayName;
                        hypRegister.ToolTip = Localization.GetString( "ToolTip", Localization.GetResourceFile( this, MyFileName ) );

                        if( PortalSettings.UserTabId != - 1 )
                        {
                            hypRegister.NavigateUrl = Globals.NavigateURL( PortalSettings.UserTabId );
                        }
                        else
                        {
                            hypRegister.NavigateUrl = Globals.NavigateURL( PortalSettings.ActiveTab.TabID, "Profile", "UserID=" + objUserInfo.UserID.ToString() );
                        }
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
    }
}