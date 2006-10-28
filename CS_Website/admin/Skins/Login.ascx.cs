using System;
using DotNetNuke.Common;
using DotNetNuke.Entities.Host;
using DotNetNuke.Services.Localization;
namespace DotNetNuke.UI.Skins.Controls
{
    /// <summary></summary>
    /// <remarks></remarks>
    /// <history>
    /// 	[smcculloch]10/15/2004	Fixed Logoff Link for FriendlyUrls
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class Login : SkinObjectBase
    {
        // public attributes
        private string _text;
        private string _cssClass;
        private string _logoffText;

        private const string MyFileName = "Login.ascx";

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

        public string LogoffText
        {
            get
            {
                return _logoffText;
            }
            set
            {
                _logoffText = value;
            }
        }

        //*******************************************************
        //
        // The Page_Load server event handler on this page is used
        // to populate the role information for the page
        //
        //*******************************************************
        protected void Page_Load( Object sender, EventArgs e )
        {
            // public attributes
            if( CssClass != "" )
            {
                hypLogin.CssClass = CssClass;
            }

            if( Request.IsAuthenticated == true )
            {
                if( LogoffText != "" )
                {
                    if( LogoffText.IndexOf( "src=" ) != - 1 )
                    {
                        LogoffText = LogoffText.Replace( "src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath );
                    }
                    hypLogin.Text = LogoffText;
                }
                else
                {
                    hypLogin.Text = Localization.GetString( "Logout", Localization.GetResourceFile( this, MyFileName ) );
                }

                if( HostSettings.GetHostSetting( "UseFriendlyUrls" ) == "Y" )
                {
                    hypLogin.NavigateUrl = Globals.FriendlyUrl(PortalSettings.ActiveTab, Globals.ApplicationURL(PortalSettings.ActiveTab.TabID) + "&portalid=" + PortalSettings.PortalId.ToString(), "Logoff.aspx");
                }
                else
                {
                    hypLogin.NavigateUrl = ResolveUrl( "~/Admin/Security/Logoff.aspx?tabid=" + PortalSettings.ActiveTab.TabID + "&portalid=" + PortalSettings.PortalId.ToString() );
                }
            }
            else
            {
                if( Text != "" )
                {
                    if( Text.IndexOf( "src=" ) != - 1 )
                    {
                        Text = Text.Replace( "src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath );
                    }
                    hypLogin.Text = Text;
                }
                else
                {
                    hypLogin.Text = Localization.GetString( "Login", Localization.GetResourceFile( this, MyFileName ) );
                }

                if( PortalSettings.LoginTabId != - 1 && Request.QueryString["override"] == null )
                {
                    // user defined tab
                    hypLogin.NavigateUrl = Globals.NavigateURL( PortalSettings.LoginTabId );
                }
                else
                {
                    // admin tab
                    hypLogin.NavigateUrl = Globals.NavigateURL( "Login" );
                }
            }
        }
    }
}