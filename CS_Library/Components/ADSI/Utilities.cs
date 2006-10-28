using System;
using System.Collections;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Security.Authentication.ADSI
{
    public class Utilities
    {
        public static string AddADSIPath( string Path, Path ADSIPath )
        {
            if( Path.IndexOf( "://" ) != - 1 )
            {
                //Clean existing ADs path first
                Path = Path.Substring( Path.Length - Path.Length - ( Path.IndexOf( "://" ) + 3 ), Path.Length - ( Path.IndexOf( "://" ) + 3 ) );
            }
            return ADSIPath.ToString() + "://" + Path;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// This function's reserved for simple network which have single domain and logon username in simple format
        /// </remarks>
        /// <history>
        ///     [tamttt]	08/01/2004	Created
        /// </history>
        public static string CanonicalToNetBIOS( string CanonicalName )
        {
            Configuration config = Configuration.GetConfig();

            // Only access CrossRefCollection if LDAP is accessible
            if( config.RefCollection != null && config.RefCollection.Count > 0 )
            {
                CrossReferenceCollection.CrossReference refObject = config.RefCollection.Item( CanonicalName );
                return refObject.mNetBIOSName;
            }
            else
            {
                return "";
            }
        }

        public static string CheckNullString( object value )
        {
            if( value == null )
            {
                return "";
            }
            else
            {
                return value.ToString();
            }
        }

        public static string ConvertToCanonical( string Distinguished, bool IncludeADSIPath )
        {
            string strCanonical = Distinguished;

            if( ! IncludeADSIPath && Distinguished.IndexOf( "://" ) != - 1 )
            {
                strCanonical = Distinguished.Substring( Distinguished.Length - Distinguished.Length - ( Distinguished.IndexOf( "://" ) + 3 ), Distinguished.Length - ( Distinguished.IndexOf( "://" ) + 3 ) );
            }

            strCanonical = strCanonical.Replace( "DC=", "" );
            strCanonical = strCanonical.Replace( "dc=", "" );
            strCanonical = strCanonical.Replace( "CN=", "" );
            strCanonical = strCanonical.Replace( "cn=", "" );
            strCanonical = strCanonical.Replace( ",", "." );

            return strCanonical;
        }

        public static string ConvertToDistinguished( string Canonical, Path ADSIPath )
        {
            string strDistinguished;

            // Clean up ADSI.Path to make sure we get a proper path
            if( Canonical.IndexOf( "://" ) != - 1 )
            {
                strDistinguished = Canonical.Substring( Canonical.Length - Canonical.Length - ( Canonical.IndexOf( "://" ) + 3 ), Canonical.Length - ( Canonical.IndexOf( "://" ) + 3 ) );
            }
            else
            {
                strDistinguished = Canonical;
            }

            strDistinguished = strDistinguished.Replace( ".", ",DC=" );
            strDistinguished = "DC=" + strDistinguished;

            if( Canonical.IndexOf( "://" ) != - 1 )
            {
                strDistinguished = AddADSIPath( strDistinguished, ADSIPath );
            }

            return strDistinguished;
        }

        public static string GetCaseInsensitiveSearch( string search )
        {
            string result = string.Empty;

            int index;

            for( index = 0; index <= search.Length - 1; index++ )
            {
                char character = search[index];
                char characterLower = char.ToLower( character );
                char characterUpper = char.ToUpper( character );

                if( characterUpper == characterLower )
                {
                    result = result + character;
                }
                else
                {
                    result = result + "[" + characterLower + characterUpper + "]";
                }
            }
            return result;
        }

        /// <summary>
        ///     Depends on how User/Password specified, 2 different method to obtain directory entry
        /// </summary>
        /// <remarks>
        ///     Admin might not enter User/Password to access AD in web.config
        /// </remarks>
        /// <history>
        ///     [tamttt]	08/01/2004	Created
        /// </history>
        public static DirectoryEntry GetDirectoryEntry( string Path )
        {
            Configuration adsiConfig = Configuration.GetConfig();
            DirectoryEntry returnEntry;

            if( ( adsiConfig.UserName.Length > 0 ) && ( adsiConfig.Password.Length > 0 ) )
            {
                returnEntry = new DirectoryEntry( Path, adsiConfig.UserName, adsiConfig.Password, AuthenticationTypes.Delegation );
            }
            else
            {
                returnEntry = new DirectoryEntry( Path );
            }

            return returnEntry;
        }

        public static Domain GetDomainByBIOSName( string Name )
        {
            Configuration adsiConfig = Configuration.GetConfig();

            // Only access CrossRefCollection if LDAP is accessible
            if( adsiConfig.RefCollection != null && adsiConfig.RefCollection.Count > 0 )
            {
                CrossReferenceCollection.CrossReference refObject = adsiConfig.RefCollection.ItemByNetBIOS( Name );
                string path = AddADSIPath( refObject.DomainPath, Path.GC );
                Domain domain = Domain.GetDomain( path, adsiConfig.UserName, adsiConfig.Password, adsiConfig.AuthenticationType );

                return domain;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Obtain location of the domain contains this entry,
        /// </summary>
        /// <remarks>
        ///     Return string is in canonical format (ttt.com.vn)
        /// </remarks>
        /// <history>
        ///     [tamttt]	08/01/2004	Created
        /// </history>
        public static string GetEntryLocation( DirectoryEntry Entry )
        {
            string strReturn = "";
            if( Entry != null )
            {
                string entryPath = CheckNullString( Entry.Path );

                if( entryPath.Length > 0 )
                {
                    strReturn = entryPath.Substring( entryPath.Length - entryPath.Length - entryPath.IndexOf( "DC=" ), entryPath.Length - entryPath.IndexOf( "DC=" ) );
                    strReturn = ConvertToCanonical( strReturn, false );
                }
            }

            return strReturn;
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        ///     in multiple domains network that search result return more than one group with the same name (i.e Administrators)
        /// </remarks>
        /// <history>
        ///     [tamttt]	08/01/2004	Created
        /// </history>
        public static ArrayList GetGroupEntriesByName( string GroupName )
        {
            Domain RootDomain = GetRootDomain();
            Search objSearch = new Search( RootDomain );

            objSearch.AddFilter( Configuration.ADSI_CLASS, CompareOperator.Is, ObjectClass.group.ToString() );
            objSearch.AddFilter( Configuration.ADSI_ACCOUNTNAME, CompareOperator.Is, GroupName );

            ArrayList groupEntries = objSearch.GetEntries();

            if( groupEntries != null )
            {
                return groupEntries;
            }
            else
            {
                return null;
            }
        }

        public static DirectoryEntry GetGroupEntryByName( string GroupName )
        {
            Domain RootDomain = GetRootDomain();
            Search objSearch = new Search( RootDomain );

            objSearch.AddFilter( Configuration.ADSI_CLASS, CompareOperator.Is, ObjectClass.group.ToString() );
            objSearch.AddFilter( Configuration.ADSI_ACCOUNTNAME, CompareOperator.Is, GroupName );

            DirectoryEntry groupEntry = objSearch.GetEntry();

            if( groupEntry != null )
            {
                return groupEntry;
            }
            else
            {
                return null;
            }
        }

        public static string GetRandomPassword()
        {
            Random rd = new Random();
            return Convert.ToString( rd.Next() );
        }

        public static Domain GetRootDomain( Path ADSIPath )
        {
            try
            {
                Configuration adsiConfig = Configuration.GetConfig();

                string rootDomainFullPath = AddADSIPath( adsiConfig.RootDomainPath, ADSIPath );
                Domain rootDomainEntry = Domain.GetDomain( rootDomainFullPath, adsiConfig.UserName, adsiConfig.Password, adsiConfig.AuthenticationType );
                return rootDomainEntry;
            }
            catch( COMException exc )
            {
                Exceptions.LogException( exc );
                return null;
            }
        }

        public static Domain GetRootDomain()
        {
            try
            {
                Configuration adsiConfig = Configuration.GetConfig();

                string rootDomainFullPath = AddADSIPath( adsiConfig.RootDomainPath, Path.GC );
                Domain rootDomainEntry = Domain.GetDomain( rootDomainFullPath, adsiConfig.UserName, adsiConfig.Password, adsiConfig.AuthenticationType );
                return rootDomainEntry;
            }
            catch( COMException exc )
            {
                Exceptions.LogException( exc );
                return null;
            }
        }

        public static DirectoryEntry GetRootEntry()
        {
            return GetRootEntry( Path.GC );
        }

        public static DirectoryEntry GetRootEntry( Path ADSIPath )
        {
            try
            {
                Configuration adsiConfig = Configuration.GetConfig();
                DirectoryEntry entry = null;
                if( adsiConfig != null )
                {
                    string rootDomainFullPath = AddADSIPath( adsiConfig.RootDomainPath, ADSIPath );
                    if( rootDomainFullPath != null )
                    {
                        entry = GetDirectoryEntry( rootDomainFullPath );
                    }
                }
                if( entry != null && entry.Name.Length > 0 )
                {
                    return entry;
                }
                else
                {
                    return null;
                }
            }
            catch( COMException exc )
            {
                Exceptions.LogException( exc );
                return null;
            }
        }

        ///<summary>
        ///    Obtain the path to access top level domain entry in Windows Active Directory
        ///</summary>
        ///<remarks>For better performance and avoid error, Global Catalog is preferer accessing method
        ///</remarks>
        ///<history>
        ///    [tamttt]	08/01/2004	Created
        ///</history>
        public static string GetRootForestPath( Path ADSIPath )
        {
            try
            {
                string strADSIPath = ADSIPath.ToString() + "://";
                DirectoryEntry ADsRoot = new DirectoryEntry( strADSIPath + "rootDSE" );
                string strRootDomain = strADSIPath + Convert.ToString( ADsRoot.Properties[Configuration.ADSI_ROOTDOMAINNAMIMGCONTEXT].Value );

                return strRootDomain;
            }
            catch( COMException ex )
            {
                Exceptions.LogException( ex );
                return null;
            }
        }

        /// <summary>
        ///     Obtain user from Windows Active Directory using simple Username format
        /// </summary>
        /// <remarks>
        ///     Reserved for simple network which have single domain and logon username in simple format
        /// </remarks>
        /// <history>
        ///     [tamttt]	08/01/2004	Created
        /// </history>
        public static DirectoryEntry GetUser0( string Name )
        {
            // Create search object then assign required params to get user entry in Active Directory
            Search objSearch = new Search( GetRootDomain() );
            DirectoryEntry userEntry;

            objSearch.AddFilter( Configuration.ADSI_CLASS, CompareOperator.Is, ObjectClass.person.ToString() );
            objSearch.AddFilter( Configuration.ADSI_ACCOUNTNAME, CompareOperator.Is, Name );
            userEntry = objSearch.GetEntry();

            return userEntry;
        }

        /// <summary>
        ///     Obtain user from Windows Active Directory using UPN format - USERNAME@DOMAIN
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [tamttt]	08/01/2004	Created
        /// </history>
        public static DirectoryEntry GetUserByUPN0( string Name, Domain RootDomain )
        {
            // Create search object then assign required params to get user entry in Active Directory
            Search objSearch = new Search( RootDomain );
            DirectoryEntry userEntry;

            // UPN is unique in entire Windows network
            objSearch.AddFilter( Configuration.ADSI_CLASS, CompareOperator.Is, ObjectClass.person.ToString() );
            objSearch.AddFilter( Configuration.ADSI_UPN, CompareOperator.Is, Name );
            userEntry = objSearch.GetEntry();

            return userEntry;
        }

        /// <summary>
        /// Get domain name (NETBIOS) from user logon name
        /// </summary>
        /// <remarks>
        /// Input string must be LogonName format (NETBIOSNAME\USERNAME)
        /// </remarks>
        /// <history>
        ///     [tamttt]	08/01/2004	Created
        /// </history>
        public static string GetUserDomainName( string UserName )
        {
            string strReturn = "";
            if( UserName.IndexOf( "\\" ) > 0 )
            {
                strReturn = UserName.Substring( 0, ( UserName.IndexOf( "\\" ) ) );
            }
            return strReturn;
        }

        /// <summary>
        ///     Obtain user from Windows Active Directory using LogonName format - NETBIOSNAME\USERNAME
        /// </summary>
        /// <remarks>
        ///     -In multiple domains network, search result might return more than one user with the same name
        ///     -Additional steps to check by domain name to get correct user
        /// </remarks>       
        public static DirectoryEntry GetUserEntryByCName0( string CName, Domain RootDomain )
        {
            Search objSearch = new Search( RootDomain );

            objSearch.AddFilter( "objectClass", CompareOperator.Is, ObjectClass.person.ToString() );
            objSearch.AddFilter( "cn", CompareOperator.Is, TrimUserDomainName( CName ) );
            ArrayList userEntries = objSearch.GetEntries();
            switch( userEntries.Count )
            {
                case 0:
                    {
                        return null;
                    }
                case 1:
                    {
                        return ( (DirectoryEntry)userEntries[0] );
                    }
            }
            Domain userDomain = GetDomainByBIOSName( GetUserDomainName( CName ) );
            if( userDomain == null )
            {
                return ( (DirectoryEntry)userEntries[0] );
            }
            foreach( DirectoryEntry userEntry in userEntries )
            {
                string entryPath = userEntry.Path;
                string entryLocation = entryPath.Substring( entryPath.Length - entryPath.Length - entryPath.IndexOf( "DC=" ), entryPath.Length - entryPath.IndexOf( "DC=" ) );
                if( entryLocation.ToLower() == userDomain.DistinguishedName.ToLower() )
                {
                    return userEntry;
                }
            }
            objSearch = ( (Search)null );
            return null;
        }

        /// <summary>
        ///     Obtain user from Windows Active Directory using LogonName format - NETBIOSNAME\USERNAME
        /// </summary>
        /// <remarks>
        ///     -In multiple domains network, search result might return more than one user with the same name
        ///     -Additional steps to check by domain name to get correct user
        /// </remarks>               
        public static DirectoryEntry GetUserEntryByName( string Name )
        {
            Search objSearch = new Search( GetRootDomain() );

            objSearch.AddFilter( "objectClass", CompareOperator.Is, ObjectClass.person.ToString() );
            objSearch.AddFilter( "sAMAccountName", CompareOperator.Is, TrimUserDomainName( Name ) );
            ArrayList userEntries = objSearch.GetEntries();
            switch( userEntries.Count )
            {
                case 0:
                    {
                        return null;
                    }
                case 1:
                    {
                        return ( (DirectoryEntry)userEntries[0] );
                    }
            }
            Domain userDomain = GetDomainByBIOSName( GetUserDomainName( Name ) );
            if( userDomain == null )
            {
                return ( (DirectoryEntry)userEntries[0] );
            }
            foreach( DirectoryEntry userEntry in userEntries )
            {
                string entryPath = userEntry.Path;
                string entryLocation = entryPath.Substring( entryPath.Length - entryPath.Length - entryPath.IndexOf( "DC=" ), entryPath.Length - entryPath.IndexOf( "DC=" ) );
                if( entryLocation.ToLower() == userDomain.DistinguishedName.ToLower() )
                {
                    return userEntry;
                }
            }
            objSearch = null;
            return null;
        }

        // See http://www.aspalliance.com/bbilbro/viewarticle.aspx?paged_article_id=4
        public static string ReplaceCaseInsensitive( string text, string oldValue, string newValue )
        {
            oldValue = GetCaseInsensitiveSearch( oldValue );

            return Regex.Replace( @text, oldValue, newValue );
        }

        /// <summary>
        /// Trim user logon string to get simple user name
        /// </summary>
        /// <remarks>
        /// Accept 3 different formats :
        /// - LogonName format (NETBIOSNAME\USERNAME)
        /// - UPN format (USERNAME@DOMAINNAME)
        /// - Simple format (USERNAME only)
        /// </remarks>
        /// <history>
        ///     [tamttt]	08/01/2004	Created
        /// </history>
        public static string TrimUserDomainName( string UserName )
        {
            string strReturn;
            if( UserName.IndexOf( "\\" ) > - 1 )
            {
                strReturn = UserName.Substring( UserName.Length - UserName.Length - ( UserName.IndexOf( "\\" ) + 1 ), UserName.Length - ( UserName.IndexOf( "\\" ) + 1 ) );
            }
            else if( UserName.IndexOf( "@" ) > - 1 )
            {
                strReturn = UserName.Substring( 0, UserName.IndexOf( "@" ) );
            }
            else
            {
                strReturn = UserName;
            }

            return strReturn;
        }

        /// <summary>
        ///    Convert input string USERNAME@DOMAIN into NETBIOSNAME\USERNAME
        /// </summary>
        /// <remarks>
        ///    - We could do it only if LDAP is accessible to obtain NetBIOSName
        ///    - If LDAP is unaccessible, return original user name (UPN format)
        /// </remarks>
        /// <history>
        ///     [tamttt]	08/01/2004	Created
        /// </history>
        public static string UPNToLogonName0( string UserPrincipalName )
        {
            Configuration config = Configuration.GetConfig();
            string userName = UserPrincipalName;

            if( config.LDAPAccesible )
            {
                string userDomain = UserPrincipalName.Substring( UserPrincipalName.Length - UserPrincipalName.Length - ( UserPrincipalName.IndexOf( "@" ) + 1 ), UserPrincipalName.Length - ( UserPrincipalName.IndexOf( "@" ) + 1 ) );
                string userNetBIOS = CanonicalToNetBIOS( userDomain );
                if( userNetBIOS.Length != 0 )
                {
                    userName = userNetBIOS + "\\" + TrimUserDomainName( UserPrincipalName );
                }
            }

            return userName;
        }

        public static string ValidateDomainPath( string Path, Path ADSIPath )
        {
            // If root domain is not specified in site settings, we start from top root forest
            if( Path.Length == 0 )
            {
                return GetRootForestPath( ADSI.Path.GC );
            }
            else if( ( Path.IndexOf( "DC=" ) != - 1 ) && ( Path.IndexOf( "://" ) != - 1 ) )
            {
                return Path;
            }
            else if( Path.IndexOf( "." ) != - 1 )
            {
                // "ttt.com.vn" format,  it's possible for "LDAP://ttt.com.vn" format to access Authentication, however GC:// gives better performance
                return ConvertToDistinguished( Path, ADSI.Path.GC );
            }
            else
            {
                // Invalid path, so we get root path from Active Directory
                return GetRootForestPath( ADSI.Path.GC );
            }
        }
    }
}