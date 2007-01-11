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
using System.Web.UI;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.UI.Skins.Controls;

namespace DotNetNuke.Services.Exceptions
{
    public class ErrorContainer : Control
    {
        private ModuleMessage _Container;

        public ErrorContainer( string strError )
        {            
            this.Container = this.FormatException( strError );
        }

        public ErrorContainer( string strError, Exception exc )
        {
            this.Container = this.FormatException( strError, exc );
        }

        public ErrorContainer( PortalSettings portalSettings, string strError, Exception exc )
        {
            UserInfo userInfo = UserController.GetCurrentUserInfo();
            if( userInfo.IsSuperUser )
            {
                this.Container = this.FormatException( strError, exc );
                return;
            }            
            //this.Container = this.FormatException( strError );
            this.Container = this.FormatException( strError, exc );
        }

        public ModuleMessage Container
        {
            get
            {
                return this._Container;
            }
            set
            {
                this._Container = value;
            }
        }

        private ModuleMessage FormatException( string strError )
        {            
            return UI.Skins.Skin.GetModuleMessageControl( DotNetNuke.Services.Localization.Localization.GetString( "ErrorOccurred" ), strError, ModuleMessageType.RedError );
        }

        private ModuleMessage FormatException( string strError, Exception exc )
        {
            if( exc != null )
            {                
                return UI.Skins.Skin.GetModuleMessageControl( strError, exc.ToString(), ModuleMessageType.RedError );
            }
            else
            {                
                return UI.Skins.Skin.GetModuleMessageControl( DotNetNuke.Services.Localization.Localization.GetString( "ErrorOccurred" ), strError, ModuleMessageType.RedError );
            }
        }
    }
}