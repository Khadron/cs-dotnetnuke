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
using System.Data;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership.Data;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using AspNetSecurity = System.Web.Security;
using AspNetProfile = System.Web.Profile;

namespace DotNetNuke.Security.Membership
{
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Security.Membership
    /// Class:      AspNetMembershipProvider
    /// <summary>
    /// The AspNetMembershipProvider overrides the default MembershipProvider to provide
    /// an AspNet Membership Component (MemberRole) implementation
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	12/09/2005	created
    /// </history>
    public class AspNetMembershipProvider : MembershipProvider
    {
        private static DataProvider dataProvider = DataProvider.Instance();

        /// <summary>
        /// Gets whether the Provider Properties can be edited
        /// </summary>
        /// <returns>A Boolean</returns>
        /// <history>
        ///     [cnurse]	03/02/2006	created
        /// </history>
        public override bool CanEditProviderProperties
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets and sets the maximum number of invlaid attempts to login are allowed
        /// </summary>
        /// <returns>A Boolean.</returns>
        /// <history>
        ///     [cnurse]	03/02/2006	created
        /// </history>
        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return System.Web.Security.Membership.MaxInvalidPasswordAttempts;
            }
            set
            {
                throw ( new NotSupportedException( "Provider properties for AspNetMembershipProvider must be set in web.config" ) );
            }
        }

        /// <summary>
        /// Gets and sets the Mimimum no of Non AlphNumeric characters required
        /// </summary>
        /// <returns>An Integer.</returns>
        /// <history>
        ///     [cnurse]	02/07/2006	created
        /// </history>
        public override int MinNonAlphanumericCharacters
        {
            get
            {
                return System.Web.Security.Membership.MinRequiredNonAlphanumericCharacters;
            }
            set
            {
                throw ( new NotSupportedException( "Provider properties for AspNetMembershipProvider must be set in web.config" ) );
            }
        }

        /// <summary>
        /// Gets and sets the Mimimum Password Length
        /// </summary>
        /// <returns>An Integer.</returns>
        /// <history>
        ///     [cnurse]	02/07/2006	created
        /// </history>
        public override int MinPasswordLength
        {
            get
            {
                return System.Web.Security.Membership.MinRequiredPasswordLength;
            }
            set
            {
                throw ( new NotSupportedException( "Provider properties for AspNetMembershipProvider must be set in web.config" ) );
            }
        }

        /// <summary>
        /// Gets and sets the window in minutes that the maxium attempts are tracked for
        /// </summary>
        /// <returns>A Boolean.</returns>
        /// <history>
        ///     [cnurse]	03/02/2006	created
        /// </history>
        public override int PasswordAttemptWindow
        {
            get
            {
                return System.Web.Security.Membership.PasswordAttemptWindow;
            }
            set
            {
                throw ( new NotSupportedException( "Provider properties for AspNetMembershipProvider must be set in web.config" ) );
            }
        }

        /// <Summary>
        /// Gets and sets the Password Format as set in the web.config file
        /// </Summary>
        /// <Returns>A PasswordFormat enumeration.</Returns>
        public override PasswordFormat PasswordFormat
        {
            get
            {
                PasswordFormat passwordFormat1 = PasswordFormat.Clear;
                switch( System.Web.Security.Membership.Provider.PasswordFormat )
                {
                    case MembershipPasswordFormat.Clear:
                        {
                            return PasswordFormat.Clear;
                        }
                    case MembershipPasswordFormat.Hashed:
                        {
                            return PasswordFormat.Hashed;
                        }
                    case MembershipPasswordFormat.Encrypted:
                        {
                            return PasswordFormat.Encrypted;
                        }
                }
                return passwordFormat1;
            }
            set
            {
                throw new NotSupportedException( "Provider properties for AspNetMembershipProvider must be set in web.config" );
            }
        }

        /// <summary>
        /// Gets and sets whether the Users's Password can be reset
        /// </summary>
        /// <returns>A Boolean.</returns>
        /// <history>
        ///     [cnurse]	03/02/2006	created
        /// </history>
        public override bool PasswordResetEnabled
        {
            get
            {
                return System.Web.Security.Membership.EnablePasswordReset;
            }
            set
            {
                throw ( new NotSupportedException( "Provider properties for AspNetMembershipProvider must be set in web.config" ) );
            }
        }

        /// <summary>
        /// Gets and sets whether the Users's Password can be retrieved
        /// </summary>
        /// <returns>A Boolean.</returns>
        /// <history>
        ///     [cnurse]	03/02/2006	created
        /// </history>
        public override bool PasswordRetrievalEnabled
        {
            get
            {
                return System.Web.Security.Membership.EnablePasswordRetrieval;
            }
            set
            {
                throw ( new NotSupportedException( "Provider properties for AspNetMembershipProvider must be set in web.config" ) );
            }
        }

        /// <summary>
        /// Gets and sets a Regular Expression that deermines the strength of the password
        /// </summary>
        /// <returns>A String.</returns>
        /// <history>
        ///     [cnurse]	02/07/2006	created
        /// </history>
        public override string PasswordStrengthRegularExpression
        {
            get
            {
                return System.Web.Security.Membership.PasswordStrengthRegularExpression;
            }
            set
            {
                throw ( new NotSupportedException( "Provider properties for AspNetMembershipProvider must be set in web.config" ) );
            }
        }

        /// <summary>
        /// Gets and sets whether a Question/Answer is required for Password retrieval
        /// </summary>
        /// <returns>A Boolean.</returns>
        /// <history>
        ///     [cnurse]	02/07/2006	created
        /// </history>
        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return System.Web.Security.Membership.RequiresQuestionAndAnswer;
            }
            set
            {
                throw ( new NotSupportedException( "Provider properties for AspNetMembershipProvider must be set in web.config" ) );
            }
        }

        /// <summary>
        /// ChangePassword attempts to change the users password
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to update.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>A Boolean indicating success or failure.</returns>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        public override bool ChangePassword( UserInfo user, string oldPassword, string newPassword )
        {
            bool retValue = false;

            // Get AspNet MembershipUser
            MembershipUser aspnetUser = GetMembershipUser( user );

            if( oldPassword == Null.NullString )
            {
                oldPassword = aspnetUser.GetPassword();
            }

            aspnetUser.ChangePassword( oldPassword, newPassword );

            if( this.PasswordRetrievalEnabled )
            {
                string confirmPassword = aspnetUser.GetPassword();

                if( confirmPassword == newPassword )
                {
                    user.Membership.Password = confirmPassword;
                    retValue = true;
                }
            }
            else
            {
                retValue = true;
            }

            return retValue;
        }

        /// <summary>
        /// ChangePasswordQuestionAndAnswer attempts to change the users password Question
        /// and PasswordAnswer
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to update.</param>
        /// <param name="password">The password.</param>
        /// <param name="passwordQuestion">The new password question.</param>
        /// <param name="passwordAnswer">The new password answer.</param>
        /// <returns>A Boolean indicating success or failure.</returns>
        /// <history>
        ///     [cnurse]	02/08/2006	created
        /// </history>
        public override bool ChangePasswordQuestionAndAnswer( UserInfo user, string password, string passwordQuestion, string passwordAnswer )
        {
            bool retValue = false;

            // Get AspNet MembershipUser
            MembershipUser aspnetUser = GetMembershipUser( user );

            if( password == Null.NullString )
            {
                password = aspnetUser.GetPassword();
            }

            retValue = aspnetUser.ChangePasswordQuestionAndAnswer( password, passwordQuestion, passwordAnswer );

            return retValue;
        }

        /// <summary>
        /// CreateDNNUser persists the DNN User information to the Database
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to persist to the Data Store.</param>
        /// <returns>The UserId of the newly created user.</returns>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        private UserCreateStatus CreateDNNUser( ref UserInfo user )
        {
            PortalSecurity objSecurity = new PortalSecurity();
            string userName = objSecurity.InputFilter( user.Username, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup );
            string email = objSecurity.InputFilter( user.Email, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup );
            string lastName = objSecurity.InputFilter( user.LastName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup );
            string firstName = objSecurity.InputFilter( user.FirstName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup );
            UserCreateStatus createStatus = UserCreateStatus.Success;
            string displayName = objSecurity.InputFilter( user.DisplayName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup );
            bool updatePassword = user.Membership.UpdatePassword;
            bool isApproved = user.Membership.Approved;

            try
            {
                user.UserID = Convert.ToInt32( dataProvider.AddUser( user.PortalID, userName, firstName, lastName, user.AffiliateID, user.IsSuperUser, email, displayName, updatePassword, isApproved ) );
            }
            catch( Exception ex )
            {
                //Clear User (duplicate User information)
                user = null;
                createStatus = UserCreateStatus.ProviderError;
            }

            return createStatus;
        }

        /// <summary>
        /// CreateMemberhipUser persists the User as an AspNet MembershipUser to the AspNet
        /// Data Store
        /// </summary>
        /// <param name="user">The user to persist to the Data Store.</param>
        /// <returns>A UserCreateStatus enumeration indicating success or reason for failure.</returns>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        private UserCreateStatus CreateMemberhipUser( UserInfo user )
        {
            PortalSecurity objSecurity = new PortalSecurity();
            string userName = objSecurity.InputFilter( user.Username, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup );
            string email = objSecurity.InputFilter( user.Email, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup );
            MembershipCreateStatus objStatus;

            MembershipUser objMembershipUser = null;

            if( MembershipProviderConfig.RequiresQuestionAndAnswer )
            {
                objMembershipUser = System.Web.Security.Membership.CreateUser( userName, user.Membership.Password, email, user.Membership.PasswordQuestion, user.Membership.PasswordAnswer, true, out objStatus );
            }
            else
            {
                objMembershipUser = System.Web.Security.Membership.CreateUser( userName, user.Membership.Password, email, null, null, true, out objStatus );
            }

            UserCreateStatus createStatus = UserCreateStatus.Success;
            switch( objStatus )
            {
                case MembershipCreateStatus.DuplicateEmail:
                    createStatus = UserCreateStatus.DuplicateEmail;
                    break;
                case MembershipCreateStatus.DuplicateProviderUserKey:
                    createStatus = UserCreateStatus.DuplicateProviderUserKey;
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    createStatus = UserCreateStatus.DuplicateUserName;
                    break;
                case MembershipCreateStatus.InvalidAnswer:
                    createStatus = UserCreateStatus.InvalidAnswer;
                    break;
                case MembershipCreateStatus.InvalidEmail:
                    createStatus = UserCreateStatus.InvalidEmail;
                    break;
                case MembershipCreateStatus.InvalidPassword:
                    createStatus = UserCreateStatus.InvalidPassword;
                    break;
                case MembershipCreateStatus.InvalidProviderUserKey:
                    createStatus = UserCreateStatus.InvalidProviderUserKey;
                    break;
                case MembershipCreateStatus.InvalidQuestion:
                    createStatus = UserCreateStatus.InvalidQuestion;
                    break;
                case MembershipCreateStatus.InvalidUserName:
                    createStatus = UserCreateStatus.InvalidUserName;
                    break;
                case MembershipCreateStatus.ProviderError:
                    createStatus = UserCreateStatus.ProviderError;
                    break;
                case MembershipCreateStatus.UserRejected:
                    createStatus = UserCreateStatus.UserRejected;
                    break;
            }

            return createStatus;
        }

        /// <summary>
        /// CreateUser persists a User to the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to persist to the Data Store.</param>
        /// <returns>A UserCreateStatus enumeration indicating success or reason for failure.</returns>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        public override UserCreateStatus CreateUser( ref UserInfo user )
        {
            UserCreateStatus createStatus;

            try
            {
                // check if username exists in database for any portal
                UserInfo objVerifyUser = GetUserByUserName( Null.NullInteger, user.Username, false );
                if( objVerifyUser != null )
                {
                    if( objVerifyUser.IsSuperUser )
                    {
                        // the username belongs to an existing super user
                        createStatus = UserCreateStatus.UserAlreadyRegistered;
                    }
                    else
                    {
                        // the username exists so we should now verify the password
                        if( ValidateUser( objVerifyUser.PortalID, user.Username, user.Membership.Password ) )
                        {
                            // check if user exists for the portal specified
                            objVerifyUser = GetUserByUserName( user.PortalID, user.Username, false );
                            if( objVerifyUser != null )
                            {
                                // the user is already registered for this portal
                                createStatus = UserCreateStatus.UserAlreadyRegistered;
                            }
                            else
                            {
                                // the user does not exist in this portal - add them
                                createStatus = UserCreateStatus.AddUserToPortal;
                            }
                        }
                        else
                        {
                            // not the same person - prevent registration
                            createStatus = UserCreateStatus.UsernameAlreadyExists;
                        }
                    }
                }
                else
                {
                    // the user does not exist
                    createStatus = UserCreateStatus.AddUser;
                }

                //If new user - add to aspnet membership
                if( createStatus == UserCreateStatus.AddUser )
                {
                    createStatus = CreateMemberhipUser( user );
                }

                //If asp user has been successfully created or we are adding a existing user
                //to a new portal
                if( createStatus == UserCreateStatus.Success || createStatus == UserCreateStatus.AddUserToPortal )
                {
                    //Create the DNN User Record
                    createStatus = CreateDNNUser( ref user );

                    if( createStatus == UserCreateStatus.Success )
                    {
                        //Persist the Profile to the Data Store
                        ProfileController.UpdateUserProfile( user );
                    }
                }
            }
            catch( Exception ) // an unexpected error occurred
            {
                //LogException(exc)
                createStatus = UserCreateStatus.UnexpectedError;
            }

            return createStatus;
        }

        /// <summary>
        /// DeleteMembershipUser deletes the User as an AspNet MembershipUser from the AspNet
        /// Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to delete from the Data Store.</param>
        /// <returns>A Boolean indicating success or failure.</returns>
        /// <history>
        ///     [cnurse]	12/22/2005	created
        /// </history>
        private bool DeleteMembershipUser( UserInfo user )
        {
            bool retValue = true;
            try
            {
                System.Web.Security.Membership.DeleteUser( user.Username, true );
            }
            catch( Exception )
            {
                retValue = false;
            }
            return retValue;
        }

        /// <summary>
        /// DeleteUser deletes a single User from the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to delete from the Data Store.</param>
        /// <returns>A Boolean indicating success or failure.</returns>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        public override bool DeleteUser( UserInfo user )
        {
            bool retValue = true;
            IDataReader dr;

            //Delete AspNet MemrshipUser
            retValue = DeleteMembershipUser( user );

            try
            {
                dr = dataProvider.GetRolesByUser( user.UserID, user.PortalID );
                while( dr.Read() )
                {
                    dataProvider.DeleteUserRole( user.UserID, Convert.ToInt32( dr["RoleId"] ) );
                }
                dr.Close();

                //check if user exists in any other portal
                dr = dataProvider.GetUserByUsername( - 1, user.Username );
                dr.Read();
                if( ! dr.Read() )
                {
                    dataProvider.DeleteUser( user.UserID );
                }
                else
                {
                    dataProvider.DeleteUserPortal( user.UserID, user.PortalID );
                }
                dr.Close();
            }
            catch( Exception )
            {
                retValue = false;
            }

            return retValue;
        }

        /// <summary>
        /// FillUserCollection fills an ArrayList from a collection Asp.Net MembershipUsers
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="dr">The data reader corresponding to the User.</param>
        /// <param name="isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        /// <history>
        ///     [cnurse]	03/30/2006	created
        /// </history>
        private ArrayList FillUserCollection( int portalId, IDataReader dr, bool ishydrated, ref int totalRecords )
        {
            //Note:  the DataReader returned from this method should contain 2 result sets.  The first set
            //       contains the TotalRecords, that satisfy the filter, the second contains the page
            //       of data

            ArrayList arrUsers = new ArrayList();
            try
            {
                UserInfo obj;
                while( dr.Read() )
                {
                    // fill business object
                    obj = FillUserInfo( portalId, dr, ishydrated, false );
                    // add to collection
                    arrUsers.Add( obj );
                }

                //Get the next result (containing the total)
                bool nextResult = dr.NextResult();

                //Get the total no of records from the second result
                totalRecords = GetTotalRecords( dr );
            }
            catch( Exception exc )
            {
                Exceptions.LogException( exc );
            }
            finally
            {
                // close datareader
                if( dr != null )
                {
                    dr.Close();
                }
            }

            return arrUsers;
        }

        /// <summary>
        /// FillUserCollection fills an ArrayList from a collection Asp.Net MembershipUsers
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="dr">The data reader corresponding to the User.</param>
        /// <param name="isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        /// <history>
        ///     [cnurse]	06/15/2006	created
        /// </history>
        private ArrayList FillUserCollection( int portalId, IDataReader dr, bool ishydrated )
        {
            //Note:  the DataReader returned from this method should contain 2 result sets.  The first set
            //       contains the TotalRecords, that satisfy the filter, the second contains the page
            //       of data

            ArrayList arrUsers = new ArrayList();
            try
            {
                UserInfo obj;
                while( dr.Read() )
                {
                    // fill business object
                    obj = FillUserInfo( portalId, dr, ishydrated, false );
                    // add to collection
                    arrUsers.Add( obj );
                }
            }
            catch( Exception exc )
            {
                Exceptions.LogException( exc );
            }
            finally
            {
                // close datareader
                if( dr != null )
                {
                    dr.Close();
                }
            }

            return arrUsers;
        }

        /// <summary>
        /// FillUserInfo fills a User Info object from a data reader
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="dr">The data reader corresponding to the User.</param>
        /// <param name="isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <param name="CheckForOpenDataReader">Flag to determine whether to chcek if the datareader is open</param>
        /// <returns>The User as a UserInfo object</returns>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        private UserInfo FillUserInfo( int portalId, IDataReader dr, bool isHydrated, bool CheckForOpenDataReader )
        {
            UserInfo objUserInfo = null;
            string userName = Null.NullString;
            string email = Null.NullString;
            bool updatePassword = false;
            bool isApproved = false;

            try
            {
                // read datareader
                bool bContinue = true;

                if( CheckForOpenDataReader )
                {
                    bContinue = false;
                    if( dr.Read() )
                    {
                        bContinue = true;
                    }
                }
                if( bContinue )
                {
                    objUserInfo = new UserInfo();
                    objUserInfo.PortalID = portalId;
                    objUserInfo.UserID = Convert.ToInt32( dr["UserID"] );
                    objUserInfo.FirstName = Convert.ToString( dr["FirstName"] );
                    objUserInfo.LastName = Convert.ToString( dr["LastName"] );
                    try
                    {
                        objUserInfo.DisplayName = Convert.ToString( dr["DisplayName"] );
                    }
                    catch
                    {
                    }
                    objUserInfo.IsSuperUser = Convert.ToBoolean( dr["IsSuperUser"] );
                    try
                    {
                        objUserInfo.AffiliateID = Convert.ToInt32( Null.SetNull( dr["AffiliateID"], objUserInfo.AffiliateID ) );
                    }
                    catch
                    {
                    }

                    //store username and email in local variables for later use
                    //as assigning them now will trigger a GetUser membership call
                    userName = Convert.ToString( dr["Username"] );
                    try
                    {
                        email = Convert.ToString( dr["Email"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        updatePassword = Convert.ToBoolean( dr["UpdatePassword"] );
                    }
                    catch
                    {
                    }
                    if( ! objUserInfo.IsSuperUser )
                    {
                        //For Users the approved/authorised info is stored in UserPortals
                        try
                        {
                            isApproved = Convert.ToBoolean( dr["Authorised"] );
                        }
                        catch
                        {
                        }
                    }
                }
            }
            finally
            {
                if( CheckForOpenDataReader && dr != null )
                {
                    dr.Close();
                }
            }

            if( objUserInfo != null )
            {
                if( isHydrated )
                {
                    // Get AspNet MembershipUser
                    MembershipUser aspnetUser = GetMembershipUser( userName );

                    //Fill Membership Property
                    FillUserMembership( aspnetUser, objUserInfo );
                }

                objUserInfo.Username = userName;
                objUserInfo.Email = email;
                objUserInfo.Membership.UpdatePassword = updatePassword;
                if( ! objUserInfo.IsSuperUser )
                {
                    //SuperUser authorisation is managed in aspnet Membership
                    objUserInfo.Membership.Approved = isApproved;
                }
            }

            return objUserInfo;
        }

        /// <summary>
        /// Generates a new random password (Length = Minimum Length + 4)
        /// </summary>
        /// <returns>A String</returns>
        /// <history>
        ///     [cnurse]	03/08/2006	created
        /// </history>
        public override string GeneratePassword()
        {
            return GeneratePassword( MinPasswordLength + 4 );
        }

        /// <summary>
        /// Generates a new random password
        /// </summary>
        /// <param name="length">The length of password to generate.</param>
        /// <returns>A String</returns>
        /// <history>
        ///     [cnurse]	03/08/2006	created
        /// </history>
        public override string GeneratePassword( int length )
        {
            return System.Web.Security.Membership.GeneratePassword( length, MinNonAlphanumericCharacters );
        }

        /// <summary>
        /// GetLegacyUsers loads legacy Users into an ArayList
        /// </summary>
        /// <remarks>
        ///	Used in Upgrading from v2.1.2 to v3.0.x
        /// </remarks>
        ///	<param name="dr">DataReader containing the legacy Users</param>
        /// <history>
        /// 	[cnurse]	11/6/2004	documented
        ///     [cnurse]    12/15/2005  Moved to MembershipProvider
        /// </history>
        private ArrayList GetLegacyUsers( IDataReader dr )
        {
            ArrayList arrUsers = new ArrayList();
            int iCount = 0;
            int iMin = 1;
            try
            {
                while( dr.Read() )
                {
                    if( iCount%1000 == 0 )
                    {
                        HtmlUtils.WriteFeedback( HttpContext.Current.Response, 0, "Building User Array - Adding Users: " + iMin.ToString() + "<br>" );
                        iCount++;
                        iMin = iMin + 1000;
                    }
                    UserInfo objUserInfo = new UserInfo();
                    try
                    {
                        objUserInfo.UserID = Convert.ToInt32( dr["UserID"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.PortalID = Convert.ToInt32( dr["PortalID"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.IsSuperUser = Convert.ToBoolean( dr["IsSuperUser"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.Username = Convert.ToString( dr["Username"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.Membership.Approved = Convert.ToBoolean( dr["Authorized"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.Membership.CreatedDate = Convert.ToDateTime( dr["CreatedDate"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.Email = Convert.ToString( dr["Email"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.Profile.FirstName = Convert.ToString( dr["FirstName"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.Profile.LastName = Convert.ToString( dr["LastName"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.Membership.Password = Convert.ToString( dr["Password"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.Profile.City = Convert.ToString( dr["City"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.Profile.Country = Convert.ToString( dr["Country"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.Profile.PostalCode = Convert.ToString( dr["PostalCode"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.Profile.Region = Convert.ToString( dr["Region"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.Profile.Street = Convert.ToString( dr["Street"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.Profile.Telephone = Convert.ToString( dr["Telephone"] );
                    }
                    catch
                    {
                    }
                    try
                    {
                        objUserInfo.Profile.Unit = Convert.ToString( dr["Unit"] );
                    }
                    catch
                    {
                    }

                    arrUsers.Add( objUserInfo );
                }
                return arrUsers;
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
        }

        /// <summary>
        /// Gets an AspNet MembershipUser from the DataStore
        /// </summary>
        /// <param name="user">The user to get from the Data Store.</param>
        /// <returns>The User as a AspNet MembershipUser object</returns>
        /// <history>
        ///     [cnurse]	12/10/2005	created
        /// </history>
        private MembershipUser GetMembershipUser( UserInfo user )
        {
            return GetMembershipUser( user.Username );
        }

        /// <summary>
        /// Gets an AspNet MembershipUser from the DataStore
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <returns>The User as a AspNet MembershipUser object</returns>
        /// <history>
        ///     [cnurse]	04/25/2006	created
        /// </history>
        private MembershipUser GetMembershipUser( string userName )
        {
            return System.Web.Security.Membership.GetUser( userName );
        }

        /// <summary>
        /// Gets a collection of Online Users
        /// </summary>
        /// <param name="portalId">The Id of the Portal</param>
        /// <returns>An ArrayList of UserInfo objects</returns>
        /// <history>
        ///     [cnurse]	03/15/2006	created
        /// </history>
        public override ArrayList GetOnlineUsers( int PortalId )
        {
            int totalRecords = 0;
            return FillUserCollection( PortalId, dataProvider.GetOnlineUsers( PortalId ), false, ref totalRecords );
        }

        /// <summary>
        /// Gets the Current Password Information for the User
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to delete from the Data Store.</param>
        /// <param name="passwordAnswer">The answer to the Password Question, ues to confirm the user
        /// has the right to obtain the password.</param>
        /// <returns>A String</returns>
        /// <history>
        ///     [cnurse]	12/10/2005	created
        /// </history>
        public override string GetPassword( UserInfo user, string passwordAnswer )
        {
            string retValue = "";

            // Get AspNet MembershipUser
            MembershipUser aspnetUser = GetMembershipUser( user );

            if( RequiresQuestionAndAnswer )
            {
                retValue = aspnetUser.GetPassword( passwordAnswer );
            }
            else
            {
                retValue = aspnetUser.GetPassword();
            }

            return retValue;
        }

        /// <summary>
        /// The GetTotalRecords method gets the number of Records returned.
        /// </summary>
        /// <param name="dr">An <see cref="IDataReader"/> containing the Total no of records</param>
        /// <returns>An Integer</returns>
        /// <history>
        /// 	[cnurse]	03/30/2006	Created
        /// </history>
        private static int GetTotalRecords( IDataReader dr )
        {
            int total = 0;

            if( dr.Read() )
            {
                try
                {
                    total = Convert.ToInt32( dr["TotalRecords"] );
                }
                catch( Exception )
                {
                    total = - 1;
                }
            }

            return total;
        }

        public override ArrayList GetUnAuthorizedUsers( int portalId, bool isHydrated )
        {
            return FillUserCollection( portalId, dataProvider.GetUnAuthorizedUsers( portalId ), isHydrated );
        }

        /// <summary>
        /// GetUserByUserName retrieves a User from the DataStore
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="userId">The id of the user being retrieved from the Data Store.</param>
        /// <param name="isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <returns>The User as a UserInfo object</returns>
        /// <history>
        ///     [cnurse]	12/10/2005	created
        /// </history>
        public override UserInfo GetUser( int portalId, int userId, bool isHydrated )
        {
            IDataReader dr = dataProvider.GetUser( portalId, userId );
            UserInfo objUserInfo = FillUserInfo( portalId, dr, isHydrated, true );

            return objUserInfo;
        }

        /// <summary>
        /// GetUserByUserName retrieves a User from the DataStore
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="username">The username of the user being retrieved from the Data Store.</param>
        /// <param name="isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <returns>The User as a UserInfo object</returns>
        /// <history>
        ///     [cnurse]	12/10/2005	created
        /// </history>
        public override UserInfo GetUserByUserName( int portalId, string username, bool isHydrated )
        {
            IDataReader dr = dataProvider.GetUserByUsername( portalId, username );
            UserInfo objUserInfo = FillUserInfo( portalId, dr, isHydrated, true );

            return objUserInfo;
        }

        /// <summary>
        /// GetUserCountByPortal gets the number of users in the portal
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <returns>The no of users</returns>
        /// <history>
        ///     [cnurse]	05/01/2006	created
        /// </history>
        public override int GetUserCountByPortal( int portalId )
        {
            return dataProvider.GetUserCountByPortal( portalId );
        }

        /// <summary>
        /// GetUsers gets all the users of the portal
        /// </summary>
        /// <remarks>If all records are required, (ie no paging) set pageSize = -1</remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <param name="pageIndex">The page of records to return.</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        /// <history>
        ///     [cnurse]	12/14/2005	created
        /// </history>
        public override ArrayList GetUsers( int portalId, bool isHydrated, int pageIndex, int pageSize, ref int totalRecords )
        {
            if( pageIndex == - 1 )
            {
                pageIndex = 0;
                pageSize = int.MaxValue;
            }

            return FillUserCollection( portalId, dataProvider.GetAllUsers( portalId, pageIndex, pageSize ), isHydrated, ref totalRecords );
        }

        /// <summary>
        /// GetUsersByEmail gets all the users of the portal whose email matches a provided
        /// filter expression
        /// </summary>
        /// <remarks>If all records are required, (ie no paging) set pageSize = -1</remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <param name="emailToMatch">The email address to use to find a match.</param>
        /// <param name="pageIndex">The page of records to return.</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        /// <history>
        ///     [cnurse]	12/14/2005	created
        /// </history>
        public override ArrayList GetUsersByEmail( int portalId, bool isHydrated, string emailToMatch, int pageIndex, int pageSize, ref int totalRecords )
        {
            if( pageIndex == - 1 )
            {
                pageIndex = 0;
                pageSize = int.MaxValue;
            }

            return FillUserCollection( portalId, dataProvider.GetUsersByEmail( portalId, emailToMatch, pageIndex, pageSize ), isHydrated, ref totalRecords );
        }

        /// <summary>
        /// GetUsersByProfileProperty gets all the users of the portal whose profile matches
        /// the profile property pased as a parameter
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <param name="propertyName">The name of the property being matched.</param>
        /// <param name="propertyValue">The value of the property being matched.</param>
        /// <param name="pageIndex">The page of records to return.</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        /// <history>
        ///     [cnurse]	02/01/2006	created
        /// </history>
        public override ArrayList GetUsersByProfileProperty( int portalId, bool isHydrated, string propertyName, string propertyValue, int pageIndex, int pageSize, ref int totalRecords )
        {
            if( pageIndex == - 1 )
            {
                pageIndex = 0;
                pageSize = int.MaxValue;
            }

            return FillUserCollection( portalId, dataProvider.GetUsersByProfileProperty( portalId, propertyName, propertyValue, pageIndex, pageSize ), isHydrated, ref totalRecords );
        }

        /// <summary>
        /// GetUsersByUserName gets all the users of the portal whose username matches a provided
        /// filter expression
        /// </summary>
        /// <remarks>If all records are required, (ie no paging) set pageSize = -1</remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <param name="userNameToMatch">The username to use to find a match.</param>
        /// <param name="pageIndex">The page of records to return.</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        /// <history>
        ///     [cnurse]	12/14/2005	created
        /// </history>
        public override ArrayList GetUsersByUserName( int portalId, bool isHydrated, string userNameToMatch, int pageIndex, int pageSize, ref int totalRecords )
        {
            if( pageIndex == - 1 )
            {
                pageIndex = 0;
                pageSize = int.MaxValue;
            }

            return FillUserCollection( portalId, dataProvider.GetUsersByUsername( portalId, userNameToMatch, pageIndex, pageSize ), isHydrated, ref totalRecords );
        }

        /// <summary>
        /// Gets whether the user in question is online
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user.</param>
        /// <returns>A Boolean indicating whether the user is online.</returns>
        /// <history>
        ///     [cnurse]	03/14/2006	created
        /// </history>
        public override bool IsUserOnline( UserInfo user )
        {
            bool isOnline = false;
            UserOnlineController objUsersOnline = new UserOnlineController();

            if( objUsersOnline.IsEnabled() )
            {
                //First try the Cache
                Hashtable userList = objUsersOnline.GetUserList();
                OnlineUserInfo onlineUser = userList[user.UserID.ToString()] as OnlineUserInfo;

                if( onlineUser != null )
                {
                    isOnline = true;
                }
                else
                {
                    //Next try the Database
                    onlineUser = (OnlineUserInfo)CBO.FillObject( dataProvider.GetOnlineUser( user.UserID ), typeof( OnlineUserInfo ) );
                    if( onlineUser != null )
                    {
                        isOnline = true;
                    }
                }
            }

            return isOnline;
        }

        /// <summary>
        /// ResetPassword resets a user's password and returns the newly created password
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to update.</param>
        /// <param name="passwordAnswer">The answer to the user's password Question.</param>
        /// <returns>The new Password.</returns>
        /// <history>
        ///     [cnurse]	02/08/2006	created
        /// </history>
        public override string ResetPassword( UserInfo user, string passwordAnswer )
        {
            string retValue = "";

            // Get AspNet MembershipUser
            MembershipUser aspnetUser = GetMembershipUser( user );

            if( RequiresQuestionAndAnswer )
            {
                retValue = aspnetUser.ResetPassword( passwordAnswer );
            }
            else
            {
                retValue = aspnetUser.ResetPassword();
            }

            return retValue;
        }

        /// <summary>
        /// Unlocks the User's Account
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user whose account is being Unlocked.</param>
        /// <returns>True if successful, False if unsuccessful.</returns>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        public override bool UnLockUser( UserInfo user )
        {
            MembershipUser objMembershipUser;
            objMembershipUser = System.Web.Security.Membership.GetUser( user.Username );
            return objMembershipUser.UnlockUser();
        }

        /// <summary>
        /// UserLogin attempts to log the user in, and returns the User if successful
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal the user belongs to</param>
        /// <param name="username">The user name of the User attempting to log in</param>
        /// <param name="password">The password of the User attempting to log in</param>
        /// <param name="VerificationCode">The verification code of the User attempting to log in</param>
        /// <param name="loginStatus">An enumerated value indicating the login status.</param>
        /// <returns>The User as a UserInfo object</returns>
        /// <history>
        ///     [cnurse]	12/10/2005	created
        /// </history>
        public override UserInfo UserLogin( int portalId, string username, string password, string verificationCode, ref UserLoginStatus loginStatus )
        {
            //For now, we are going to ignore the possibility that the User may exist in the
            //Global Data Store but not in the Local DataStore ie. A shared Global Data Store

            //Initialise Login Status to Failure
            loginStatus = UserLoginStatus.LOGIN_FAILURE;

            //Get a light-weight (unhydrated) DNN User from the Database, we will hydrate it later if neccessary
            UserInfo user = null;
            user = GetUserByUserName( portalId, username, false );

            if( user != null )
            {
                //Get AspNet MembershipUser
                MembershipUser aspnetUser = null;
                aspnetUser = GetMembershipUser( user );

                //Fill Membership Property from AspNet MembershipUser
                FillUserMembership( aspnetUser, user );

                //Check if the User is Locked Out (and unlock if AutoUnlock has expired)
                if( aspnetUser.IsLockedOut )
                {
                    int intTimeout;
                    intTimeout = Convert.ToInt32( ( Globals.HostSettings["AutoAccountUnlockDuration"] != null ) ? ( Globals.HostSettings["AutoAccountUnlockDuration"] ) : -1 );
                    if( intTimeout != 0 )
                    {
                        if( intTimeout == - 1 )
                        {
                            intTimeout = 10;
                        }
                        if( aspnetUser.LastLockoutDate < DateTime.Now.AddMinutes( - 1*intTimeout ) )
                        {
                            //Unlock User
                            user.Membership.LockedOut = false;

                            //Persist to Data Store
                            aspnetUser.UnlockUser();
                        }
                        else
                        {
                            loginStatus = UserLoginStatus.LOGIN_USERLOCKEDOUT;
                        }
                    }
                }

                //Check in a verified situation whether the user is Approved
                if( user.Membership.Approved == false && user.IsSuperUser == false )
                {
                    //Check Verification code
                    if( verificationCode == ( portalId.ToString() + "-" + user.UserID ) )
                    {
                        //Approve User
                        user.Membership.Approved = true;

                        //Persist to Data Store
                        UpdateUser( user );
                    }
                    else
                    {
                        loginStatus = UserLoginStatus.LOGIN_USERNOTAPPROVED;
                    }
                }

                //Verify User Credentials
                bool bValid = false;
                if( loginStatus != UserLoginStatus.LOGIN_USERLOCKEDOUT && loginStatus != UserLoginStatus.LOGIN_USERNOTAPPROVED )
                {
                    if( user.IsSuperUser )
                    {
                        if( ValidateUser( Null.NullInteger, username, password ) )
                        {
                            loginStatus = UserLoginStatus.LOGIN_SUPERUSER;
                            bValid = true;
                        }
                    }
                    else
                    {
                        if( ValidateUser( portalId, username, password ) )
                        {
                            loginStatus = UserLoginStatus.LOGIN_SUCCESS;
                            bValid = true;
                        }
                    }
                }

                if( ! bValid )
                {
                    //Clear the user object
                    user = null;
                }
            }

            return user;
        }

        /// <summary>
        /// Validates the users credentials against the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal the user belongs to</param>
        /// <param name="username">The user name of the User attempting to log in</param>
        /// <param name="password">The password of the User attempting to log in</param>
        /// <returns>A Boolean result</returns>
        /// <history>
        ///     [cnurse]	12/12/2005	created
        /// </history>
        private bool ValidateUser( int portalId, string username, string password )
        {
            return System.Web.Security.Membership.ValidateUser( username, password );
        }

        /// <summary>
        /// Deletes all UserOnline inof from the database that has activity outside of the
        /// time window
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="TimeWindow">Time Window in Minutes</param>
        /// <history>
        ///     [cnurse]	03/15/2006	created
        /// </history>
        public override void DeleteUsersOnline( int TimeWindow )
        {
            dataProvider.DeleteUsersOnline( TimeWindow );
        }

        /// <summary>
        /// Builds a UserMembership object from an AspNet MembershipUser
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="aspNetUser">The MembershipUser object to use to fill the DNN UserMembership.</param>
        /// <history>
        /// 	[cnurse]	12/10/2005	created
        /// </history>
        private void FillUserMembership( MembershipUser aspNetUser, UserInfo user )
        {
            //Fill Membership Property
            if( aspNetUser != null )
            {
                user.Membership.CreatedDate = aspNetUser.CreationDate;
                user.Membership.LastActivityDate = aspNetUser.LastActivityDate;
                user.Membership.LastLockoutDate = aspNetUser.LastLockoutDate;
                user.Membership.LastLoginDate = aspNetUser.LastLoginDate;
                user.Membership.LastPasswordChangeDate = aspNetUser.LastPasswordChangedDate;
                user.Membership.LockedOut = aspNetUser.IsLockedOut;
                user.Membership.PasswordQuestion = aspNetUser.PasswordQuestion;
                user.Membership.ObjectHydrated = true;
                if( user.IsSuperUser )
                {
                    //For superusers the Approved info is stored in aspnet membership
                    user.Membership.Approved = aspNetUser.IsApproved;
                }
            }
        }

        /// <summary>
        /// GetUserMembership retrieves the UserMembership information from the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user whose Membership information we are retrieving.</param>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        public override void GetUserMembership( ref UserInfo user )
        {
            MembershipUser aspnetUser = null;

            //Get AspNet MembershipUser
            aspnetUser = GetMembershipUser( user );

            //Fill Membership Property
            FillUserMembership( aspnetUser, user );

            //Get Online Status
            user.Membership.IsOnLine = IsUserOnline( user );
        }

        /// <summary>
        /// TransferUsers transfers legacy users to the new ASP.NET MemberRole Architecture
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="PortalID">Id of the Portal</param>
        ///	<param name="arrUsers">An ArrayList of the Users</param>
        ///	<param name="SuperUsers">A flag indicating whether the users are SuperUsers</param>
        /// <history>
        /// 	[cnurse]	11/6/2004	documented
        ///     [cnurse]    12/15/2005  Moved to MembershipProvider
        /// </history>
        private void TransferUsers( int PortalID, ArrayList arrUsers, bool SuperUsers )
        {
            UserController objUserCont = new UserController();
            try
            {
                //Set the MemberRole API ApplicationName
                if( SuperUsers )
                {
                    HtmlUtils.WriteFeedback( HttpContext.Current.Response, 0, "Start Transferring SuperUsers to MemberRole:<br>" );
                }
                else
                {
                    HtmlUtils.WriteFeedback( HttpContext.Current.Response, 0, "Start Transferring Portal Users to MemberRole: PortalId= " + PortalID.ToString() + "<br>" );
                }

                IDataReader dr;
                string EncryptionKey = "";
                dr = DotNetNuke.Data.DataProvider.Instance().GetHostSetting( "EncryptionKey" );
                if( dr.Read() )
                {
                    EncryptionKey = dr["SettingValue"].ToString();
                }
                dr.Close();

                int i;
                int iMin = 1;
                int iMax = 100;
                for( i = 0; i <= arrUsers.Count - 1; i++ )
                {
                    if( i%100 == 0 )
                    {
                        if( iMin > arrUsers.Count )
                        {
                            iMin = arrUsers.Count;
                        }
                        if( iMax > arrUsers.Count )
                        {
                            iMax = arrUsers.Count;
                        }

                        HtmlUtils.WriteFeedback( HttpContext.Current.Response, 0, "Transferring Users:" + iMin.ToString() + " to " + iMax.ToString() + "<br>" );

                        iMin = iMin + 100;
                        iMax = iMax + 100;
                    }

                    UserInfo objUser;
                    objUser = (UserInfo)arrUsers[i];
                    MembershipCreateStatus objStatus;
                    string strPassword;
                    PortalSecurity objPortalSecurity = new PortalSecurity();
                    strPassword = objPortalSecurity.Decrypt( EncryptionKey, objUser.Membership.Password );
                    if( objUser.IsSuperUser )
                    {
                        objUser.Membership.Approved = true;
                    }
                    MembershipUser objMembershipUser;
                    objMembershipUser = System.Web.Security.Membership.CreateUser( objUser.Username, strPassword, objUser.Email, null, null, objUser.Membership.Approved, out objStatus );
                    if( objStatus != MembershipCreateStatus.Success )
                    {
                        Exceptions.LogException( new Exception( objStatus.ToString() ) );
                    }
                    else
                    {
                        try
                        {
                            ProfileBase objProfile;
                            objProfile = ProfileBase.Create( objUser.Username, true );
                            objProfile["FirstName"] = objUser.Profile.FirstName;
                            objProfile["LastName"] = objUser.Profile.LastName;
                            objProfile["Unit"] = objUser.Profile.Unit;
                            objProfile["Street"] = objUser.Profile.Street;
                            objProfile["City"] = objUser.Profile.City;
                            objProfile["Region"] = objUser.Profile.Region;
                            objProfile["PostalCode"] = objUser.Profile.PostalCode;
                            objProfile["Country"] = objUser.Profile.Country;
                            objProfile["Telephone"] = objUser.Profile.Telephone;
                            objProfile.Save();
                        }
                        catch( Exception exc )
                        {
                            Exceptions.LogException( exc );
                        }

                        RoleController objDNNRoles = new RoleController();
                        string[] arrUserRoles = objDNNRoles.GetRolesByUser( objUser.UserID, PortalID );
                        if( arrUserRoles != null )
                        {
                            try
                            {
                                System.Web.Security.Roles.AddUserToRoles( objUser.Username, arrUserRoles );
                            }
                            catch( Exception exc )
                            {
                                Exceptions.LogException( exc );
                            }
                        }
                    }
                }
            }
            finally
            {
            }

            if( SuperUsers )
            {
                HtmlUtils.WriteFeedback( HttpContext.Current.Response, 0, "Finish Transferring SuperUsers to MemberRole:<br>" );
            }
            else
            {
                HtmlUtils.WriteFeedback( HttpContext.Current.Response, 0, "Finish Transferring Portal Users to MemberRole: PortalId= " + PortalID.ToString() + "<br>" );
            }
        }

        /// <summary>
        /// TransferUsersToMembershipProvider transfers legacy users to the
        ///	new ASP.NET MemberRole Architecture
        /// </summary>
        /// <history>
        /// 	[cnurse]	11/6/2004	documented
        ///     [cnurse]    12/15/2005  Moved to MembershipProvider
        /// </history>
        public override void TransferUsersToMembershipProvider()
        {
            int j;
            ArrayList arrUsers = new ArrayList();
            UserController objUserController = new UserController();

            //Get the Super Users and Transfer them to the Provider
            arrUsers = GetLegacyUsers( dataProvider.GetSuperUsers() );
            TransferUsers( - 1, arrUsers, true );

            PortalController objPortalController = new PortalController();
            ArrayList arrPortals;
            arrPortals = objPortalController.GetPortals();

            for( j = 0; j <= arrPortals.Count - 1; j++ )
            {
                PortalInfo objPortal;
                objPortal = (PortalInfo)arrPortals[j];

                RoleController objRoles = new RoleController();
                ArrayList arrRoles = objRoles.GetPortalRoles( objPortal.PortalID );
                int q;
                for( q = 0; q <= arrRoles.Count - 1; q++ )
                {
                    try
                    {
                        System.Web.Security.Roles.CreateRole( ( (RoleInfo)arrRoles[q] ).RoleName );
                    }
                    catch( Exception exc )
                    {
                        Exceptions.LogException( exc );
                    }
                }

                //Get the Portal Users and Transfer them to the Provider
                arrUsers = GetLegacyUsers( dataProvider.GetUsers( objPortal.PortalID ) );
                TransferUsers( objPortal.PortalID, arrUsers, false );
            }
        }

        /// <summary>
        /// UpdateUser persists a user to the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to persist to the Data Store.</param>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        public override void UpdateUser( UserInfo user )
        {
            PortalSecurity objSecurity = new PortalSecurity();
            string firstName = objSecurity.InputFilter( user.FirstName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup );
            string lastName = objSecurity.InputFilter( user.LastName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup );
            string email = objSecurity.InputFilter( user.Email, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup );
            string displayName = objSecurity.InputFilter( user.DisplayName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup );
            bool updatePassword = user.Membership.UpdatePassword;
            bool isApproved = user.Membership.Approved;

            if( displayName == "" )
            {
                displayName = firstName + " " + lastName;
            }

            //Persist the DNN User to the Database
            dataProvider.UpdateUser( user.UserID, user.PortalID, firstName, lastName, email, displayName, updatePassword, isApproved );

            //Persist the Membership to the Data Store
            UpdateUserMembership( user );

            //Persist the Profile to the Data Store
            ProfileController.UpdateUserProfile( user );
        }

        /// <summary>
        /// UpdateUserMembership persists a user's Membership to the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to persist to the Data Store.</param>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        private void UpdateUserMembership( UserInfo user )
        {
            PortalSecurity objSecurity = new PortalSecurity();
            string email = objSecurity.InputFilter( user.Email, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup );

            //Persist the Membership Properties to the AspNet Data Store
            MembershipUser objMembershipUser;
            objMembershipUser = System.Web.Security.Membership.GetUser( user.Username );
            objMembershipUser.Email = email;
            objMembershipUser.LastActivityDate = DateTime.Now;
            objMembershipUser.IsApproved = user.Membership.Approved;
            System.Web.Security.Membership.UpdateUser( objMembershipUser );
        }

        /// <summary>
        /// Updates UserOnline info
        /// time window
        /// </summary>
        /// <param name="UserList">List of users to update</param>
        /// <history>
        ///     [cnurse]	03/15/2006	created
        /// </history>
        public override void UpdateUsersOnline( Hashtable UserList )
        {
            dataProvider.UpdateUsersOnline( UserList );
        }
    }
}