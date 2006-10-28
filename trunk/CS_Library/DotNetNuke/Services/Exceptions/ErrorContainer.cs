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
            this.Container = this.FormatException( strError );
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
            return UI.Skins.Skin.GetModuleMessageControl( DotNetNuke.Services.Localization.Localization.GetString( "ErrorOccurred" ), strError, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError );
        }

        private ModuleMessage FormatException( string strError, Exception exc )
        {
            if( exc != null )
            {
                return UI.Skins.Skin.GetModuleMessageControl( strError, exc.ToString(), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError );
            }
            else
            {
                return UI.Skins.Skin.GetModuleMessageControl( DotNetNuke.Services.Localization.Localization.GetString( "ErrorOccurred" ), strError, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError );
            }
        }
    }
}