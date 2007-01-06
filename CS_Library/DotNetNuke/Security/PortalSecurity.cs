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
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Permissions;

namespace DotNetNuke.Security
{
    public class PortalSecurity
    {
        /// <summary>
        /// The FilterFlag enum determines which filters are applied by the InputFilter
        /// function.  The Flags attribute allows the user to include multiple
        /// enumerated values in a single variable by OR'ing the individual values
        /// together.
        /// </summary>
        [Flags()]
        public enum FilterFlag
        {
            MultiLine = 1,
            NoMarkup = 2,
            NoScripting = 4,
            NoSQL = 8,
            NoAngleBrackets = 16
        }

        /// <summary>
        /// This function converts a byte array to a hex string
        /// </summary>
        /// <param name="bytes">An array of bytes</param>
        /// <returns>A string representing the hex converted value</returns>
        /// <remarks>
        /// This is a private function that is used internally by the CreateKey function
        /// </remarks>
        private string BytesToHexString( byte[] bytes )
        {
            StringBuilder hexString = new StringBuilder( 64 );

            int counter;
            for( counter = 0; counter <= bytes.Length - 1; counter++ )
            {
                hexString.Append( string.Format( "{0:X2}", bytes[counter] ) );
            }

            return hexString.ToString();
        }

        /// <summary>
        /// This function creates a random key
        /// </summary>
        /// <param name="numBytes">This is the number of bytes for the key</param>
        /// <returns>A random string</returns>
        /// <remarks>
        /// This is a public function used for generating SHA1 keys
        /// </remarks>
        public string CreateKey( int numBytes )
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[numBytes - 1 + 1];

            rng.GetBytes( buff );

            return BytesToHexString( buff );
        }

        public string Decrypt( string strKey, string strData )
        {
            string strValue = "";

            if( !String.IsNullOrEmpty(strKey) )
            {
                // convert key to 16 characters for simplicity
                int ii = strKey.Length;
                if( ii < 16 )
                {
                    strKey = strKey + "XXXXXXXXXXXXXXXX".Substring( 0, 16 - strKey.Length );
                }
                else if( ii > 16 )
                {
                    strKey = strKey.Substring( 0, 16 );
                }

                // create encryption keys
                byte[] byteKey = Encoding.UTF8.GetBytes( strKey.Substring( 0, 8 ) );
                byte[] byteVector = Encoding.UTF8.GetBytes( strKey.Substring( strKey.Length - 8, 8 ) );

                // convert data to byte array and Base64 decode
                byte[] byteData = new byte[strData.Length + 1];
                try
                {
                    byteData = Convert.FromBase64String( strData );
                }
                catch // invalid length
                {
                    strValue = strData;
                }

                if( strValue == "" )
                {
                    try
                    {
                        // decrypt
                        DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
                        MemoryStream objMemoryStream = new MemoryStream();
                        CryptoStream objCryptoStream = new CryptoStream( objMemoryStream, objDES.CreateDecryptor( byteKey, byteVector ), CryptoStreamMode.Write );
                        objCryptoStream.Write( byteData, 0, byteData.Length );
                        objCryptoStream.FlushFinalBlock();

                        // convert to string
                        Encoding objEncoding = Encoding.UTF8;
                        strValue = objEncoding.GetString( objMemoryStream.ToArray() );
                    }
                    catch // decryption error
                    {
                        strValue = "";
                    }
                }
            }
            else
            {
                strValue = strData;
            }

            return strValue;
        }

        public string Encrypt( string strKey, string strData )
        {
            string strValue;

            if( !String.IsNullOrEmpty(strKey) )
            {
                // convert key to 16 characters for simplicity
                int ii = strKey.Length;
                if( ii < 16 )
                {
                    strKey = strKey + "XXXXXXXXXXXXXXXX".Substring( 0, 16 - strKey.Length );
                }
                else if( ii > 16 )
                {
                    strKey = strKey.Substring( 0, 16 );
                }

                // create encryption keys
                byte[] byteKey = Encoding.UTF8.GetBytes( strKey.Substring( 0, 8 ) );
                byte[] byteVector = Encoding.UTF8.GetBytes( strKey.Substring( strKey.Length - 8, 8 ) );

                // convert data to byte array
                byte[] byteData = Encoding.UTF8.GetBytes( strData );

                // encrypt
                DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
                MemoryStream objMemoryStream = new MemoryStream();
                CryptoStream objCryptoStream = new CryptoStream( objMemoryStream, objDES.CreateEncryptor( byteKey, byteVector ), CryptoStreamMode.Write );
                objCryptoStream.Write( byteData, 0, byteData.Length );
                objCryptoStream.FlushFinalBlock();

                // convert to string and Base64 encode
                strValue = Convert.ToBase64String( objMemoryStream.ToArray() );
            }
            else
            {
                strValue = strData;
            }

            return strValue;
        }

        /// <summary>
        /// This filter removes angle brackets i.e.
        /// </summary>
        /// <param name="strInput">This is the string to be filtered</param>
        /// <returns>Filtered UserInput</returns>
        /// <remarks>
        /// This is a private function that is used internally by the InputFilter function
        /// </remarks>        
        private string FormatAngleBrackets( string strInput )
        {
            string TempInput = strInput.Replace( "<", "" );
            TempInput = TempInput.Replace( ">", "" );
            return TempInput;
        }

        /// <summary>
        /// This function uses Regex search strings to remove HTML tags which are
        /// targeted in Cross-site scripting (XSS) attacks.  This function will evolve
        /// to provide more robust checking as additional holes are found.
        /// </summary>
        /// <param name="strInput">This is the string to be filtered</param>
        /// <returns>Filtered UserInput</returns>
        /// <remarks>
        /// This is a private function that is used internally by the InputFilter function
        /// </remarks>
        private string FormatDisableScripting( string strInput )
        {
            string TempInput = strInput;

            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            string strReplacement = " ";

            TempInput = Regex.Replace( TempInput, "<script[^>]*>.*?</script[^><]*>", strReplacement, options );
            TempInput = Regex.Replace( TempInput, "<input[^>]*>.*?</input[^><]*>", strReplacement, options );
            TempInput = Regex.Replace( TempInput, "<object[^>]*>.*?</object[^><]*>", strReplacement, options );
            TempInput = Regex.Replace( TempInput, "<embed[^>]*>.*?</embed[^><]*>", strReplacement, options );
            TempInput = Regex.Replace( TempInput, "<applet[^>]*>.*?</applet[^><]*>", strReplacement, options );
            TempInput = Regex.Replace( TempInput, "<form[^>]*>.*?</form[^><]*>", strReplacement, options );
            TempInput = Regex.Replace( TempInput, "<option[^>]*>.*?</option[^><]*>", strReplacement, options );
            TempInput = Regex.Replace( TempInput, "<select[^>]*>.*?</select[^><]*>", strReplacement, options );
            TempInput = Regex.Replace( TempInput, "<iframe[^>]*>.*?</iframe[^><]*>", strReplacement, options );
            TempInput = Regex.Replace( TempInput, "<ilayer[^>]*>.*?</ilayer[^><]*>", strReplacement, options );
            TempInput = Regex.Replace( TempInput, "<form[^>]*>", strReplacement, options );
            TempInput = Regex.Replace( TempInput, "</form[^><]*>", strReplacement, options );
            TempInput = Regex.Replace( TempInput, "javascript:", strReplacement, options );
            TempInput = Regex.Replace( TempInput, "vbscript:", strReplacement, options );

            return TempInput;
        }

        /// <summary>
        /// This filter removes CrLf characters and inserts br
        /// </summary>
        /// <param name="strInput">This is the string to be filtered</param>
        /// <returns>Filtered UserInput</returns>
        /// <remarks>
        /// This is a private function that is used internally by the InputFilter function
        /// </remarks>
        private string FormatMultiLine( string strInput )
        {
            string TempInput = strInput.Replace( "\r\n", "<br>" );
            return TempInput.Replace( "\r", "<br>" );
        }

        /// <summary>
        /// This function verifies raw SQL statements to prevent SQL injection attacks
        /// and replaces a similar function (PreventSQLInjection) from the Common.Globals.vb module
        /// </summary>
        /// <param name="strSQL">This is the string to be filtered</param>
        /// <returns>Filtered UserInput</returns>
        /// <remarks>
        /// This is a private function that is used internally by the InputFilter function
        /// </remarks>
        private string FormatRemoveSQL( string strSQL )
        {
            string strCleanSQL = strSQL;

            if( strSQL != null )
            {
                Array BadCommands = ";,--,create,drop,select,insert,delete,update,union,sp_,xp_".Split( ',' );

                // strip any dangerous SQL commands
                int intCommand;
                for( intCommand = 0; intCommand <= BadCommands.Length - 1; intCommand++ )
                {
                    strCleanSQL = Regex.Replace( strCleanSQL, Convert.ToString( BadCommands.GetValue( intCommand ) ), " ", RegexOptions.IgnoreCase );
                }

                // convert any single quotes
                strCleanSQL = strCleanSQL.Replace( "'", "''" );
            }

            return strCleanSQL;
        }

        /// <summary>
        /// Verifies edit permissions on a module
        /// </summary>
        /// <param name="objModulePermissions"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public static bool HasEditPermissions( ModulePermissionCollection objModulePermissions )
        {
            return ModulePermissionController.HasModulePermission( objModulePermissions, "EDIT" );
        }

        public static bool HasEditPermissions( int ModuleId )
        {
            ModulePermissionController objModulePermissionController = new ModulePermissionController();
            ModulePermissionCollection objModulePermissions = objModulePermissionController.GetModulePermissionsCollectionByModuleID( ModuleId );
            return HasEditPermissions( objModulePermissions );
        }

        /// <summary>
        /// Determines is user has the necessary permissions to access the an item with the
        /// designated AccessLevel.
        /// </summary>
        /// <param name="AccessLevel">The SecurityAccessLevel required to access a portal module or module action.</param>
        /// <param name="PortalSettings">The PortalSettings for the current portal.</param>
        /// <param name="ModuleConfiguration">The ModuleInfo object for the associated module.</param>
        /// <param name="UserName">The Context.User.Identity.Name of the currently logged in user.</param>
        /// <returns>A boolean value indicating if the user has the necessary permissions</returns>
        /// <remarks>Every module control and module action has an associated permission level.  This
        /// function determines whether the user represented by UserName has sufficient permissions, as
        /// determined by the PortalSettings and ModuleSettings, to access a resource with the
        /// designated AccessLevel.</remarks>
        public static bool HasNecessaryPermission( SecurityAccessLevel AccessLevel, PortalSettings PortalSettings, ModuleInfo ModuleConfiguration, string UserName )
        {
            bool blnAuthorized = true;
            switch( AccessLevel )
            {
                case SecurityAccessLevel.Anonymous:

                    blnAuthorized = true;
                    break;
                case SecurityAccessLevel.View: // view

                    if( IsInRole( PortalSettings.AdministratorRoleName.ToString() ) == false && IsInRoles( PortalSettings.ActiveTab.AdministratorRoles.ToString() ) == false )
                    {
                        if( !IsInRoles( ModuleConfiguration.AuthorizedViewRoles ) )
                        {
                            blnAuthorized = false;
                        }
                    }
                    break;
                case SecurityAccessLevel.Edit: // edit

                    if( IsInRole( PortalSettings.AdministratorRoleName.ToString() ) == false && IsInRoles( PortalSettings.ActiveTab.AdministratorRoles.ToString() ) == false )
                    {
                        if( !IsInRoles( ModuleConfiguration.AuthorizedViewRoles ) )
                        {
                            blnAuthorized = false;
                        }
                        else
                        {
                            if( !HasEditPermissions( ModuleConfiguration.ModulePermissions ) )
                            {
                                blnAuthorized = false;
                            }
                        }
                    }
                    break;
                case SecurityAccessLevel.Admin: // admin

                    if( IsInRole( PortalSettings.AdministratorRoleName.ToString() ) == false && IsInRoles( PortalSettings.ActiveTab.AdministratorRoles.ToString() ) == false )
                    {
                        blnAuthorized = false;
                    }
                    break;
                case SecurityAccessLevel.Host: // host

                    if( UserName.Length > 0 )
                    {
                        UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                        if( !objUserInfo.IsSuperUser )
                        {
                            blnAuthorized = false;
                        }
                    }
                    else // no longer logged in
                    {
                        blnAuthorized = false;
                    }
                    break;
            }
            return blnAuthorized;
        }

        /// <summary>
        /// This function determines if the Input string contains any markup.
        /// </summary>
        /// <param name="strInput">This is the string to be checked</param>
        /// <returns>True if string contains Markup tag(s)</returns>
        /// <remarks>
        /// This is a private function that is used internally by the InputFilter function
        /// </remarks>
        private bool IncludesMarkup( string strInput )
        {
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            string strPattern = "<[^<>]*>";
            return Regex.IsMatch( strInput, strPattern, options );
        }

        /// <summary>
        /// This function applies security filtering to the UserInput string.
        /// </summary>
        /// <param name="UserInput">This is the string to be filtered</param>
        /// <param name="FilterType">Flags which designate the filters to be applied</param>
        /// <returns>Filtered UserInput</returns>
        public string InputFilter( string UserInput, FilterFlag FilterType )
        {
            if( UserInput == null )
            {
                return "";
            }

            string TempInput = UserInput;

            if( ( FilterType & FilterFlag.NoAngleBrackets ) == FilterFlag.NoAngleBrackets )
            {
                bool RemoveAngleBrackets;
                if( Config.GetSetting( "RemoveAngleBrackets" ) == null )
                {
                    RemoveAngleBrackets = false;
                }
                else
                {
                    RemoveAngleBrackets = bool.Parse( Config.GetSetting( "RemoveAngleBrackets" ) );
                }
                if( RemoveAngleBrackets  )
                {
                    TempInput = FormatAngleBrackets( TempInput );
                }
            }

            if( ( FilterType & FilterFlag.NoSQL ) == FilterFlag.NoSQL )
            {
                TempInput = FormatRemoveSQL( TempInput );
            }
            else
            {
                if( ( FilterType & FilterFlag.NoMarkup ) == FilterFlag.NoMarkup )
                {
                    if( IncludesMarkup( TempInput ) )
                    {
                        TempInput = HttpUtility.HtmlEncode( TempInput );
                    }
                }
                else if( ( FilterType & FilterFlag.NoScripting ) == FilterFlag.NoScripting )
                {
                    TempInput = FormatDisableScripting( TempInput );
                }

                if( ( FilterType & FilterFlag.MultiLine ) == FilterFlag.MultiLine )
                {
                    TempInput = FormatMultiLine( TempInput );
                }
            }

            return TempInput;
        }

        public static bool IsInRole( string role )
        {
            UserInfo objUserInfo = UserController.GetCurrentUserInfo();
            HttpContext context = HttpContext.Current;

            if( !String.IsNullOrEmpty(role) &&  ( context.Request.IsAuthenticated == false && role == Globals.glbRoleUnauthUserName ) )
            {
                return true;
            }
            else
            {
                return objUserInfo.IsInRole( role );
            }
        }

        public static bool IsInRoles( string roles )
        {
            if( roles != null )
            {
                HttpContext context = HttpContext.Current;
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();

                string[] availableRoles = roles.Split( new char[] {';'} );
                
                for( int i = 0; i < availableRoles.Length; i++ )
                {
                    string role = availableRoles[i];
                    if( objUserInfo.IsSuperUser || ( !String.IsNullOrEmpty( role ) && ( ( context.Request.IsAuthenticated == false && role == Globals.glbRoleUnauthUserName ) || role == Globals.glbRoleAllUsersName || objUserInfo.IsInRole( role )  ) ) )
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [Obsolete( "This function has been replaced by UserController.UserLogin" )]
        public int UserLogin( string Username, string Password, int PortalID, string PortalName, string IP, bool CreatePersistentCookie )
        {
            UserLoginStatus loginStatus = UserLoginStatus.LOGIN_USERNOTAPPROVED;
            int UserId = -1;
            UserInfo objUser = UserController.UserLogin( PortalID, Username, Password, "", PortalName, IP, ref loginStatus, CreatePersistentCookie );

            if( loginStatus == UserLoginStatus.LOGIN_SUCCESS || loginStatus == UserLoginStatus.LOGIN_SUPERUSER )
            {
                UserId = objUser.UserID;
            }

            // return the UserID
            return UserId;
        }

        public void SignOut()
        {
            // Log User Off from Cookie Authentication System
            FormsAuthentication.SignOut();

            // expire cookies
            HttpContext.Current.Response.Cookies["portalaliasid"].Value = null;
            HttpContext.Current.Response.Cookies["portalaliasid"].Path = "/";
            HttpContext.Current.Response.Cookies["portalaliasid"].Expires = DateTime.Now.AddYears( -30 );

            HttpContext.Current.Response.Cookies["portalroles"].Value = null;
            HttpContext.Current.Response.Cookies["portalroles"].Path = "/";
            HttpContext.Current.Response.Cookies["portalroles"].Expires = DateTime.Now.AddYears( -30 );
        }
    }
}