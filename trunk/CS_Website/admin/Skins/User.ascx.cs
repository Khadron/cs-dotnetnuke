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
                if (_text != null) return _text; else return String.Empty;
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
                if (_cssClass != null) return _cssClass; else return String.Empty;
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

    }
}