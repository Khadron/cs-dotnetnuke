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

        public static void OpenNewWindow(string Url)
        {
            HttpContext.Current.Response.Write("<script>window.open('" + Url + "', 'new');</script>");
        }
    }
}