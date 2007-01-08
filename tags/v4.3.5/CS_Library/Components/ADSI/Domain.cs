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
using System.Collections;
using System.DirectoryServices;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Security.Authentication.ADSI
{
    public class Domain : DirectoryEntry
    {
        private ArrayList mChildDomains = new ArrayList(); //One level child
        private ArrayList mAllChildDomains = new ArrayList(); //All level child
        private Domain mParentDomain;
        private string mDistinguishedName = "";
        private string mNetBIOSName = "";
        private string mCanonicalName = "";
        private int mLevel;
        private bool mChildPopulate = false;

        public Domain()
        {
        }

        public Domain( string Path, string UserName, string Password, AuthenticationTypes AuthenticationType ) : base( Path, UserName, Password, AuthenticationType )
        {
            PopulateInfo();
            PopulateChild( this );
            mChildPopulate = true;
        }

        public Domain( string Path ) : base( Path )
        {
            PopulateInfo();
            PopulateChild( this );
            mChildPopulate = true;
        }

        private void PopulateInfo()
        {
            Configuration config = Configuration.GetConfig();

            mDistinguishedName = Convert.ToString( base.Properties[Configuration.ADSI_DISTINGUISHEDNAME].Value );
            mCanonicalName = Utilities.ConvertToCanonical( mDistinguishedName, false );

            // Note that this property will be null string if LDAP is unaccessible
            mNetBIOSName = Utilities.CanonicalToNetBIOS( mCanonicalName );
        }

        private void PopulateChild( Domain Domain )
        {
            Search objSearch = new Search( Domain );

            objSearch.SearchScope = SearchScope.OneLevel;
            objSearch.AddFilter( Configuration.ADSI_CLASS, CompareOperator.Is, ToString().ToString() );

            ArrayList resDomains = objSearch.GetEntries();
            DirectoryEntry entry;

            foreach( DirectoryEntry tempLoopVar_entry in resDomains )
            {
                entry = tempLoopVar_entry;
                Domain child;
                //If (Not Domain.Username Is Nothing) AndAlso (Not Domain.Password Is Nothing) Then
                //    child = ADSI.Domain.GetDomain(entry.Path, Domain.Username, Domain.Password, Domain.AuthenticationType)
                //Else
                child = GetDomain( entry.Path );
                //End If

                if( child != null )
                {
                    child.ParentDomain = Domain;
                    child.Level = Domain.Level + 1;
                    // Add this child into childDomains collection
                    Domain.ChildDomains.Add( child );
                    // add this child and all it's child into allchilddomains collection
                    Domain.AllChildDomains.Add( child );
                    Domain.AllChildDomains.AddRange( child.AllChildDomains );
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// Accessing ADs costs lots of resource so we better put ADs object into app cache
        /// </remarks>
        public static Domain GetDomain( string Path )
        {
            return GetDomain( Path, "", "", AuthenticationTypes.Delegation );
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// Accessing ADs costs lots of resource so we better put ADs object into app cache
        /// </remarks>
        public static Domain GetDomain( string Path, string UserName, string Password, AuthenticationTypes AuthenticationType )
        {
            Domain Domain = (Domain)DataCache.GetCache( Path );
            if( Domain == null )
            {
                if( ( UserName.Length > 0 ) && ( Password.Length > 0 ) )
                {
                    Domain = new Domain( Path, UserName, Password, AuthenticationType );
                }
                else
                {
                    Domain = new Domain( Path );
                }

                DataCache.SetCache( Path, Domain );
            }

            return Domain;
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// Clear domain object in cache
        /// </remarks>
        public static void ResetDomain( string Path )
        {
            DataCache.RemoveCache( Path );
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// Return one level child domains
        /// </remarks>
        public ArrayList ChildDomains
        {
            get
            {
                return mChildDomains;
            }
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// Return child all level child domains
        /// </remarks>
        /// <history>
        ///     [tamttt]	08/01/2004	Created
        /// </history>
        public ArrayList AllChildDomains
        {
            get
            {
                return mAllChildDomains;
            }
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// Return parent domain of this domain
        /// </remarks>
        public Domain ParentDomain
        {
            get
            {
                return mParentDomain;
            }
            set
            {
                mParentDomain = value;
            }
        }

        public int Level
        {
            get
            {
                return mLevel;
            }
            set
            {
                mLevel = value;
            }
        }

        public string DistinguishedName
        {
            get
            {
                return mDistinguishedName;
            }
            set
            {
                mDistinguishedName = value;
            }
        }

        public bool ChildPopulate
        {
            get
            {
                return mChildPopulate;
            }
            set
            {
                mChildPopulate = value;
            }
        }
    }
}