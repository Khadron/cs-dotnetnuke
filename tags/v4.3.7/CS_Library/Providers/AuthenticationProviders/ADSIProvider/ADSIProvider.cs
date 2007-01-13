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
using System.Collections;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Text;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Authentication.ADSI;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Security.Authentication
{
    public class ADSIProvider : AuthenticationProvider
    {
        private ADSI.Configuration _adsiConfig;
        private PortalSettings _portalSettings;

        public ADSIProvider()
        {
            _portalSettings = PortalController.GetCurrentPortalSettings();
            _adsiConfig = ADSI.Configuration.GetConfig();
        }

        public override Array GetAuthenticationTypes()
        {
            return Enum.GetValues( typeof( AuthenticationTypes ) );
        }

        public override ArrayList GetGroups()
        {
            //Dim adsiConfig As Authentication.ADSI.Configuration = Authentication.ADSI.Configuration.GetConfig(_portalSettings.PortalId)
            // Normally number of roles in DNN less than groups in Authentication,
            // so start from DNN roles to get better performance
            try
            {
                // Obtain search object
                //Dim rootDomain As DirectoryEntry = GetRootDomain()
                //Dim objSearch As New ADSI.Search(rootDomain)

                ArrayList colGroup = new ArrayList();
                RoleController objRoleController = new RoleController();
                ArrayList lstRoles = objRoleController.GetPortalRoles( _portalSettings.PortalId );
                RoleInfo objRole;

                foreach( RoleInfo tempLoopVar_objRole in lstRoles )
                {
                    objRole = tempLoopVar_objRole;
                    // Auto assignment roles have been added by DNN, so don't need to get them
                    if( ! objRole.AutoAssignment )
                    {
                        // It's possible in multiple domains network that search result return more than one group with the same name (i.e Administrators)
                        // We better check them all
                        DirectoryEntry entry;
                        foreach( DirectoryEntry tempLoopVar_entry in Utilities.GetGroupEntriesByName( objRole.RoleName ) )
                        {
                            entry = tempLoopVar_entry;
                            GroupInfo group = new GroupInfo();

                            group.PortalID = objRole.PortalID;
                            group.RoleID = objRole.RoleID;
                            group.GUID = entry.NativeGuid;
                            group.Location = Utilities.GetEntryLocation( entry );
                            group.RoleName = objRole.RoleName;
                            group.Description = objRole.Description;
                            group.ServiceFee = objRole.ServiceFee;
                            group.BillingFrequency = objRole.BillingFrequency;
                            group.TrialPeriod = objRole.TrialPeriod;
                            group.TrialFrequency = objRole.TrialFrequency;
                            group.BillingPeriod = objRole.BillingPeriod;
                            group.TrialFee = objRole.TrialFee;
                            group.IsPublic = objRole.IsPublic;
                            group.AutoAssignment = objRole.AutoAssignment;
                            // Populate member with distingushed name
                            PopulateMembership( group, entry );

                            colGroup.Add( group );
                        }
                    }
                }

                return colGroup;
            }
            catch( COMException exc )
            {
                Exceptions.LogException( exc );
                return null;
            }
        }

        //'Obtain group objects from ADSI, to be used in custom module for importing role into DNN
        //Public Overloads Overrides Function GetGroups(ByVal Filter As String) As ArrayList
        //    Return SearchGroups("", Filter)
        //End Function

        //Public Overrides Sub AddRoleMembership(ByVal Role As DotNetNuke.Security.Roles.RoleInfo, ByVal UserDistinguishedName As String)
        //    Dim adsiConfig As Authentication.ADSI.Configuration = Authentication.ADSI.Configuration.GetConfig(_portalSettings.PortalId)
        //    Dim strDomain As String = Right(UserDistinguishedName, UserDistinguishedName.Length - UserDistinguishedName.IndexOf("DC="))
        //    Dim strUserName As String = UserDistinguishedName.Substring(3, UserDistinguishedName.IndexOf(",") - 3)
        //    Try
        //        Dim objCrossReference As ADSI.CrossReferenceCollection.CrossReference = _adsiConfig.RefCollection.Item(ConvertToCanonical(strDomain, False))
        //        If (Not objCrossReference.NetBIOSName Is Nothing) AndAlso (objCrossReference.NetBIOSName.Length > 0) Then
        //            strUserName = objCrossReference.NetBIOSName & "\" & strUserName
        //        End If

        //        ' Get DNN UserInfo from database
        //        Dim objUserController As New DotNetNuke.Entities.Users.UserController
        //        Dim objRoleController As New DotNetNuke.Security.Roles.RoleController
        //        Dim objUserInfo As DotNetNuke.Entities.Users.UserInfo = objUserController.GetUserByUsername(_portalSettings.PortalId, strUserName)
        //        ' Add user role
        //        If Not objUserInfo Is Nothing Then
        //            objRoleController.AddUserRole(_portalSettings.PortalId, objUserInfo.UserID, Role.RoleID, DateTime.MaxValue)
        //        End If
        //    Catch Exc As System.Runtime.InteropServices.COMException
        //        LogException(Exc)
        //    End Try

        //End Sub

        //Public Overrides Sub AddGroupMembership(ByVal Role As DotNetNuke.Security.Roles.RoleInfo, ByVal User As DotNetNuke.Entities.Users.UserInfo)
        //    Dim adsiConfig As Authentication.ADSI.Configuration = Authentication.ADSI.Configuration.GetConfig(_portalSettings.PortalId)
        //    Try
        //        Dim authGroup As Authentication.GroupInfo = GetGroup(Role.RoleName)
        //        Dim authUser As Authentication.UserInfo = GetUser(User.Username)

        //        If (Not authUser Is Nothing) Then
        //            ' Create new group, if not exists in AD
        //            If authGroup Is Nothing Then
        //                authGroup = CreateGroup(Role)
        //            End If

        //            If (Not authGroup Is Nothing) AndAlso (Not IsAuthenticationMember(authGroup, authUser)) Then
        //                'Get object in ADSI
        //                'Dim rootDomain As ADSI.Domain = _adsiConfig.RootDomain(ADSIPath.LDAP)
        //                Dim rootDomain As ADSI.Domain = _adsiConfig.RootDomain()
        //                Dim userEntry As DirectoryEntry = GetUserEntryByLoggedOnName(User.Username, rootDomain)
        //                Dim tempGroupEntry As DirectoryEntry '= GetGroupEntryByName(Role.RoleName, rootDomain)

        //                ' With a new group, it might not be available due to replication
        //                ' Return to avoid error or
        //                Do Until (Not tempGroupEntry Is Nothing)
        //                    tempGroupEntry = GetGroupEntryByName(Role.RoleName, rootDomain)
        //                Loop
        //                'If tempGroupEntry Is Nothing Then
        //                '    Return
        //                'End If
        //                Dim groupEntry As DirectoryEntry = GetLDAPEntry(tempGroupEntry, _adsiConfig)
        //                If (Not groupEntry Is Nothing) AndAlso (Not userEntry Is Nothing) Then
        //                    Dim strDisName As String = CheckNullString(userEntry.Properties(ADSI_DISTINGUISHEDNAME).Value)
        //                    groupEntry.Properties(ADSI_MEMBER).Add(strDisName)
        //                    groupEntry.CommitChanges()
        //                End If

        //            End If
        //        End If

        //    Catch Exc As System.Runtime.InteropServices.COMException
        //        LogException(Exc)
        //    End Try
        //End Sub

        //Public Overrides Sub RemoveGroupMembership(ByVal Role As DotNetNuke.Security.Roles.RoleInfo, ByVal User As DotNetNuke.Entities.Users.UserInfo)
        //    Dim adsiConfig As Authentication.ADSI.Configuration = Authentication.ADSI.Configuration.GetConfig(_portalSettings.PortalId)
        //    Try
        //        Dim authGroup As Authentication.GroupInfo = GetGroup(Role.RoleName)
        //        Dim authUser As Authentication.UserInfo = GetUser(User.Username)

        //        If (Not authGroup Is Nothing) AndAlso (Not authUser Is Nothing) Then
        //            If IsAuthenticationMember(authGroup, authUser) Then
        //                'Get object in ADSI
        //                'Dim rootDomain As ADSI.Domain = _adsiConfig.RootDomain(ADSIPath.LDAP)
        //                Dim rootDomain As ADSI.Domain = _adsiConfig.RootDomain()
        //                Dim userEntry As DirectoryEntry = GetUserEntryByLoggedOnName(User.Username, rootDomain)
        //                Dim tempGroupEntry As DirectoryEntry = GetGroupEntryByName(Role.RoleName, rootDomain)
        //                Dim groupEntry As DirectoryEntry = GetLDAPEntry(tempGroupEntry, _adsiConfig)

        //                If (Not groupEntry Is Nothing) AndAlso (Not userEntry Is Nothing) Then
        //                    Dim strDisName As String = CheckNullString(userEntry.Properties(ADSI_DISTINGUISHEDNAME).Value)
        //                    groupEntry.Properties(ADSI_MEMBER).Remove(strDisName)
        //                    groupEntry.CommitChanges()
        //                End If

        //            End If
        //        End If

        //    Catch Exc As System.Runtime.InteropServices.COMException
        //        LogException(Exc)
        //    End Try
        //End Sub

        public override string GetNetworkStatus()
        {
            StringBuilder sb = new StringBuilder();
            // Refresh settings cache first
            ADSI.Configuration.ResetConfig();
            _adsiConfig = ADSI.Configuration.GetConfig();

            sb.Append( "<b>[Global Catalog Status]</b>" + "<br>" );
            try
            {
                if( _adsiConfig.ADSINetwork )
                {
                    sb.Append( "OK<br>" );
                }
                else
                {
                    sb.Append( "FAIL<br>" );
                }
            }
            catch( COMException ex )
            {
                sb.Append( "FAIL<br>" );
                sb.Append( ex.Message + "<br>" );
            }

            sb.Append( "<b>[Root Domain Status]</b><br>" );
            try
            {
                if( Utilities.GetRootEntry() != null )
                {
                    sb.Append( "OK<br>" );
                }
                else
                {
                    sb.Append( "FAIL<br>" );
                }
            }
            catch( COMException ex )
            {
                sb.Append( "FAIL<br>" );
                sb.Append( ex.Message + "<br>" );
            }

            sb.Append( "<b>[LDAP Status]</b><br>" );
            try
            {
                if( _adsiConfig.LDAPAccesible )
                {
                    sb.Append( "OK<br>" );
                }
                else
                {
                    sb.Append( "FAIL<br>" );
                }
            }
            catch( COMException ex )
            {
                sb.Append( "FAIL<br>" );
                sb.Append( ex.Message + "<br>" );
            }

            sb.Append( "<b>[Network Domains Status]</b><br>" );
            try
            {
                if( _adsiConfig.RefCollection != null && _adsiConfig.RefCollection.Count > 0 )
                {
                    sb.Append( _adsiConfig.RefCollection.Count.ToString() );
                    sb.Append( " Domain(s):<br>" );
                    CrossReferenceCollection.CrossReference crossRef;
                    foreach( CrossReferenceCollection.CrossReference tempLoopVar_crossRef in _adsiConfig.RefCollection )
                    {
                        crossRef = tempLoopVar_crossRef;
                        sb.Append( crossRef.CanonicalName );
                        sb.Append( " (" );
                        sb.Append( crossRef.NetBIOSName );
                        sb.Append( ")<br>" );
                    }

                    if( _adsiConfig.RefCollection.ProcesssLog.Length > 0 )
                    {
                        sb.Append( _adsiConfig.RefCollection.ProcesssLog + "<br>" );
                    }
                }
                else
                {
                    sb.Append( "[LDAP Error Message]<br>" );
                }
            }
            catch( COMException ex )
            {
                sb.Append( "[LDAP Error Message]<br>" );
                sb.Append( ex.Message + "<br>" );
            }

            if( _adsiConfig.ProcessLog.Length > 0 )
            {
                sb.Append( _adsiConfig.ProcessLog + "<br>" );
            }

            return sb.ToString();
        }

        private UserInfo GetSimplyUser( string UserName )
        {
            UserInfo objAuthUser = new UserInfo();

            objAuthUser.PortalID = _portalSettings.PortalId;
            objAuthUser.GUID = "";
            objAuthUser.Username = UserName;
            objAuthUser.FirstName = Utilities.TrimUserDomainName( UserName );
            objAuthUser.LastName = Utilities.GetUserDomainName( UserName );
            objAuthUser.IsSuperUser = false;
            objAuthUser.Location = _adsiConfig.ConfigDomainPath;
            objAuthUser.PrincipalName = Utilities.TrimUserDomainName( UserName ) + "@" + objAuthUser.Location;
            objAuthUser.DistinguishedName = Utilities.ConvertToDistinguished( UserName, Path.GC );

            string strEmail = _adsiConfig.DefaultEmailDomain;
            if( strEmail.Length != 0 )
            {
                if( strEmail.IndexOf( "@" ) == - 1 )
                {
                    strEmail = "@" + strEmail;
                }
                strEmail = objAuthUser.FirstName + strEmail;
            }
            else
            {
                strEmail = objAuthUser.FirstName + "@" + objAuthUser.LastName + ".com"; // confusing?
            }
            // Membership properties
            objAuthUser.Username = UserName;
            objAuthUser.Email = strEmail;
            objAuthUser.Membership.Approved = true;
            objAuthUser.Membership.LastLoginDate = DateTime.Today;
            objAuthUser.Membership.Password = Utilities.GetRandomPassword(); //Membership.GeneratePassword(6)
            objAuthUser.AuthenticationExists = false;

            return objAuthUser;
        }

        public override UserInfo GetUser( string LoggedOnUserName, string LoggedOnPassword )
        {
            UserInfo objAuthUser;

            if( ! _adsiConfig.ADSINetwork )
            {
                return null;
            }

            try
            {
                DirectoryEntry entry = Utilities.GetUserEntryByName( LoggedOnUserName );

                //Check authenticated
                if( ! IsAuthenticated( entry.Path, LoggedOnUserName, LoggedOnPassword ) )
                {
                    return null;
                }

                // Return authenticated if no error
                objAuthUser = new UserInfo();

                string location = Utilities.GetEntryLocation( entry );
                if( location.Length == 0 )
                {
                    location = _adsiConfig.ConfigDomainPath;
                }

                objAuthUser.PortalID = _portalSettings.PortalId;
                objAuthUser.GUID = entry.NativeGuid;
                objAuthUser.Username = LoggedOnUserName;
                objAuthUser.Location = location;
                objAuthUser.PrincipalName = Utilities.TrimUserDomainName( LoggedOnUserName ) + "@" + location;
                objAuthUser.Username = LoggedOnUserName;
                objAuthUser.Membership.Password = LoggedOnPassword;

                FillUserInfo( entry, objAuthUser );

                return objAuthUser;
            }
            catch( Exception exc )
            {
                Exceptions.LogException( exc );
                return null;
            }
        }

        public override UserInfo GetUser( string LoggedOnUserName )
        {
            //Dim adsiConfig As Authentication.ADSI.Configuration = Authentication.ADSI.Configuration.GetConfig(_portalSettings.PortalId)
            UserInfo objAuthUser;
            try
            {
                if( _adsiConfig.ADSINetwork )
                {
                    DirectoryEntry entry;

                    entry = Utilities.GetUserEntryByName( LoggedOnUserName );

                    if( entry != null )
                    {
                        objAuthUser = new UserInfo();

                        string location = Utilities.GetEntryLocation( entry );
                        if( location.Length == 0 )
                        {
                            location = _adsiConfig.ConfigDomainPath;
                        }

                        objAuthUser.PortalID = _portalSettings.PortalId;
                        objAuthUser.GUID = entry.NativeGuid;
                        objAuthUser.Location = location;
                        objAuthUser.Username = LoggedOnUserName;
                        objAuthUser.PrincipalName = Utilities.TrimUserDomainName( LoggedOnUserName ) + "@" + location;
                        objAuthUser.Username = LoggedOnUserName;
                        objAuthUser.Membership.Password = Utilities.GetRandomPassword();

                        FillUserInfo( entry, objAuthUser );
                    }
                    else
                    {
                        objAuthUser = GetSimplyUser( LoggedOnUserName );
                    }
                }
                else // could not find it in AD, so populate user object with minumum info
                {
                    objAuthUser = GetSimplyUser( LoggedOnUserName );
                }

                return objAuthUser;
            }
            catch( COMException exc )
            {
                Exceptions.LogException( exc );
                return null;
            }
        }

        private bool IsAuthenticated( string Path, string UserName, string Password )
        {
            try
            {
                DirectoryEntry userEntry = new DirectoryEntry( Path, UserName, Password, AuthenticationTypes.Signing );
                // Bind to the native AdsObject to force authentication.
                object obj = userEntry.NativeObject;
            }
            catch( COMException exc )
            {
                Exceptions.LogException( exc );
                return false;
            }

            return true;
        }

        public override bool IsAuthenticationMember( GroupInfo AuthenticationGroup, UserInfo AuthenticationUser )
        {
            if( ! AuthenticationGroup.IsPopulated )
            {
                PopulateMembership( AuthenticationGroup );
            }

            return AuthenticationGroup.AuthenticationMember.Contains( AuthenticationUser.DistinguishedName );
        }

        private void FillUserInfo( DirectoryEntry UserEntry, UserInfo userInfo )
        {

            userInfo.IsSuperUser = false;
            userInfo.Username = userInfo.Username;
            userInfo.Membership.Approved = true;
            userInfo.Membership.LastLoginDate = DateTime.Today;
            userInfo.Email = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_EMAIL].Value );
            userInfo.CName = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_CNAME].Value.ToString() );
            userInfo.DisplayName = Utilities.CheckNullString(UserEntry.Properties[ADSI.Configuration.ADSI_DISPLAYNAME].Value);
            if (userInfo.DisplayName == "")
            {
                userInfo.DisplayName = userInfo.CName;
            }
            userInfo.DistinguishedName = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_DISTINGUISHEDNAME].Value.ToString() );
            userInfo.sAMAccountName = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_ACCOUNTNAME].Value.ToString() );
            userInfo.Profile.FirstName = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_FIRSTNAME].Value );
            userInfo.Profile.LastName = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_LASTNAME].Value );
            userInfo.Profile.Street = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_STREET].Value );
            userInfo.Profile.City = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_CITY].Value );
            userInfo.Profile.Region = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_REGION].Value );
            userInfo.Profile.PostalCode = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_POSTALCODE].Value );
            userInfo.Profile.Country = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_COUNTRY].Value );
            userInfo.Profile.Telephone = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_TELEPHONE].Value );
            userInfo.Profile.Fax = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_FAX].Value );
            userInfo.Profile.Cell = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_CELL].Value );
            userInfo.Profile.Website = Utilities.CheckNullString( UserEntry.Properties[ADSI.Configuration.ADSI_WEBSITE].Value );
            userInfo.AuthenticationExists = true;
            // obtain firstname from username if admin has not enter enough user info
            if( userInfo.Profile.FirstName.Length == 0 )
            {
                userInfo.Profile.FirstName = Utilities.TrimUserDomainName( userInfo.Username );
            }
        }

        private void PopulateMembership( GroupInfo GroupInfo )
        {
            //Dim adsiConfig As Authentication.ADSI.Configuration = Authentication.ADSI.Configuration.GetConfig(GroupInfo.PortalID)
            Domain rootDomain = Utilities.GetRootDomain( Path.GC );
            DirectoryEntry groupEntry = Utilities.GetGroupEntryByName( GroupInfo.RoleName );

            PopulateMembership( GroupInfo, groupEntry );
        }

        private void PopulateMembership( GroupInfo GroupInfo, DirectoryEntry GroupEntry )
        {
            if( ! GroupInfo.IsPopulated )
            {
                // Populate membership with distinguished name
                foreach( string strMember in GroupEntry.Properties[ADSI.Configuration.ADSI_MEMBER] )
                {
                    //Store DistinguishedName, this method is more accurated
                    GroupInfo.AuthenticationMember.Add( strMember );
                }
                GroupInfo.IsPopulated = true;
            }
        }
    }
}