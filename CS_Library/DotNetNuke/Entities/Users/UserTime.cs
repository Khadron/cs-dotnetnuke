using System;
using System.Web;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Entities.Users
{
    public class UserTime
    {
        public DateTime ConvertToUserTime( DateTime dt, double ClientTimeZone )
        {
            // Obtain PortalSettings from Current Context
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return dt.AddMinutes( FromClientToServerFactor( ClientTimeZone, _portalSettings.TimeZoneOffset ) );
        }

        public DateTime ConvertToServerTime( DateTime dt, double ClientTimeZone )
        {
            // Obtain PortalSettings from Current Context
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return dt.AddMinutes( FromServerToClientFactor( ClientTimeZone, _portalSettings.TimeZoneOffset ) );
        }

        public DateTime CurrentUserTime
        {
            get
            {
                HttpContext context = HttpContext.Current;
                if( !( context.Request.IsAuthenticated ) )
                {
                    return DateTime.Now;
                }
                else
                {
                    return DateTime.Now.AddMinutes( ClientToServerTimeZoneFactor );
                }
            }
        }

        public double ClientToServerTimeZoneFactor
        {
            get
            {
                // Obtain PortalSettings from Current Context
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                return FromClientToServerFactor( objUserInfo.Profile.TimeZone, _portalSettings.TimeZoneOffset );
            }
        }

        public double ServerToClientTimeZoneFactor
        {
            get
            {
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                // Obtain PortalSettings from Current Context
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                return FromServerToClientFactor( objUserInfo.Profile.TimeZone, _portalSettings.TimeZoneOffset );
            }
        }

        private double FromClientToServerFactor( double Client, double Server )
        {
            return Client - Server;
        }

        private double FromServerToClientFactor( double Client, double Server )
        {
            return Server - Client;
        }
    }
}