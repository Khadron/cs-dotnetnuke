using System;
using System.Collections;
using System.DirectoryServices;
using System.Runtime.InteropServices;

namespace DotNetNuke.Security.Authentication.ADSI
{
    public class CrossReferenceCollection : CollectionBase
    {
        public struct CrossReference
        {
            internal string mDomainPath;
            internal string mCanonicalName;
            internal string mNetBIOSName;

            internal CrossReference( string Path, string NetBIOS, string Canonical )
            {
                mDomainPath = Path;
                mCanonicalName = Canonical;
                mNetBIOSName = NetBIOS;
            }

            public string DomainPath
            {
                get
                {
                    return mDomainPath;
                }
            }

            public string CanonicalName
            {
                get
                {
                    return mCanonicalName;
                }
            }

            public string NetBIOSName
            {
                get
                {
                    return mNetBIOSName;
                }
            }
        }

        private Hashtable mCanonicalLookup = new Hashtable();

        // Allows access to items by both NetBiosName or CanonicalName
        private Hashtable mNetBIOSLookup = new Hashtable();
        private string mProcessLog = "";

        public string ProcesssLog
        {
            get
            {
                return mProcessLog;
            }
        }

        public CrossReferenceCollection( string UserName, string Password, AuthenticationTypes AuthType )
        {
            try
            {
                // Obtain NETBIOS only if LDAP accessible to prevent error
                DirectoryEntry rootLDAP = new DirectoryEntry( "LDAP://rootDSE", UserName, Password, AuthType );
                string crossRefPath = "LDAP://CN=Partitions," + rootLDAP.Properties["configurationNamingContext"].Value.ToString();
                DirectoryEntry objCrossRefContainer;

                if( ( UserName.Length > 0 ) && ( Password.Length > 0 ) )
                {
                    objCrossRefContainer = new DirectoryEntry( crossRefPath, UserName, Password, AuthType );
                }
                else
                {
                    objCrossRefContainer = new DirectoryEntry( crossRefPath );
                }

                DirectoryEntry objCrossRef;
                foreach( DirectoryEntry tempLoopVar_objCrossRef in objCrossRefContainer.Children )
                {
                    objCrossRef = tempLoopVar_objCrossRef;
                    if( !Convert.ToBoolean( objCrossRef.Properties["nETBIOSName"].Value == null ) )
                    {
                        string netBIOSName = Convert.ToString( objCrossRef.Properties["nETBIOSName"].Value );
                        string canonicalName = Convert.ToString( objCrossRef.Properties["dnsRoot"].Value );
                        string domainPath = Convert.ToString( objCrossRef.Properties["nCName"].Value );
                        CrossReference crossRef = new CrossReference( domainPath, netBIOSName, canonicalName );
                        this.Add( crossRef );
                    }
                }
                //mProcessLog += "Accessing LDAP : OK"
            }
            catch( COMException ex )
            {
                //mProcessLog += "Accessing LDAP : FAIL" & "<br>"
                mProcessLog += ex.Message + "<br>";
            }
        }

        public CrossReference Item( int index )
        {
            try
            {
                object obj;
                obj = base.List[index];
                return ( (CrossReference)obj );
            }
            catch( Exception )
            {
                return new CrossReference();
            }
        }

        public CrossReference Item( string Name )
        {
            int index;
            object obj;

            // Do validation first
            try
            {
                if( mCanonicalLookup[Name] == null )
                {
                    return new CrossReference();
                }
            }
            catch( Exception )
            {
                return new CrossReference();
            }

            index = Convert.ToInt32( mCanonicalLookup[Name] );
            obj = base.List[index];

            return ( (CrossReference)obj );
        }

        public CrossReference ItemByNetBIOS( string Name )
        {
            int index;
            object obj;

            // Do validation first
            try
            {
                if( mNetBIOSLookup[Name] == null )
                {
                    return new CrossReference();
                }
            }
            catch( Exception )
            {
                return new CrossReference();
            }

            index = Convert.ToInt32( mNetBIOSLookup[Name] );
            obj = base.List[index];

            return ( (CrossReference)obj );
        }

        internal void Add( CrossReference RefObject )
        {
            int index;
            try
            {
                index = base.List.Add( RefObject );
                mCanonicalLookup.Add( RefObject.CanonicalName, index );
                mNetBIOSLookup.Add( RefObject.NetBIOSName, index );
            }
            catch( COMException ex )
            {
                mProcessLog += ex.Message;
            }
        }

        internal new void Clear()
        {
            mNetBIOSLookup.Clear();
            mCanonicalLookup.Clear();
            base.Clear();
        }
    }
}