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
                
                if( _text != null )
                {
                    return _text;
                    
                }
                else
                {
                    return String.Empty;
                }
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
                
                if( _cssClass != null )
                {
                    return _cssClass;
                }
                else
                {
                    return String.Empty;
                }
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
                if( _logoffText != null )
                {
                    return _logoffText;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                _logoffText = value;
            }
        }

        protected void Page_Load( Object sender, EventArgs e )
        {
            // public attributes
            if( !String.IsNullOrEmpty(CssClass) )
            {
                hypLogin.CssClass = CssClass;
            }

            if( Request.IsAuthenticated )
            {
                if( !String.IsNullOrEmpty(LogoffText) )
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
                    hypLogin.NavigateUrl = Globals.FriendlyUrl(PortalSettings.ActiveTab, Globals.ApplicationURL(PortalSettings.ActiveTab.TabID) + "&portalid=" + PortalSettings.PortalId, "Logoff.aspx");
                }
                else
                {
                    hypLogin.NavigateUrl = ResolveUrl( "~/Admin/Security/Logoff.aspx?tabid=" + PortalSettings.ActiveTab.TabID + "&portalid=" + PortalSettings.PortalId );
                }
            }
            else
            {
                if( !String.IsNullOrEmpty(Text) )
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