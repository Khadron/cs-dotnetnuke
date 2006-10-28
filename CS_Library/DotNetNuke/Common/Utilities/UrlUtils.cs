using System;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;

namespace DotNetNuke.Common.Utilities
{
    public class UrlUtils
    {
        public static string EncryptParameter( string Value )
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            string strKey = _portalSettings.GUID.ToString(); // restrict the key to 6 characters to conserve space
            PortalSecurity objSecurity = new PortalSecurity();
            return HttpUtility.UrlEncode( objSecurity.Encrypt( strKey, Value ) );
        }

        public static string DecryptParameter( string Value )
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            string strKey = _portalSettings.GUID.ToString(); // restrict the key to 6 characters to conserve space
            PortalSecurity objSecurity = new PortalSecurity();
            return objSecurity.Decrypt( strKey, Value );
        }

        public static string GetParameterName( string Pair )
        {
            string[] arrNameValue = Pair.Split( Convert.ToChar( "=" ) );
            return arrNameValue[0];
        }

        public static string GetParameterValue( string Pair )
        {
            string[] arrNameValue = Pair.Split( Convert.ToChar( "=" ) );
            if( arrNameValue.Length > 1 )
            {
                return arrNameValue[1];
            }
            else
            {
                return "";
            }
        }
    }
}