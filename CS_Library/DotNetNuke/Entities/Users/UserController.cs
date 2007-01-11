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
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using MembershipProvider=DotNetNuke.Security.Membership.MembershipProvider;

namespace DotNetNuke.Entities.Users
{
    /// <Summary>
    /// The UserController class provides Business Layer methods for Users
    /// </Summary>
    public class UserController
    {
        private static MembershipProvider memberProvider = MembershipProvider.Instance();

        [Obsolete("This function has been replaced by UserController.CreateUser")]
        public int AddUser(UserInfo objUser)
        {
            UserCreateStatus createStatus = CreateUser(ref objUser);
            return objUser.UserID;
        }

        [Obsolete("This function has been replaced by UserController.CreateUser")]
        public int AddUser(UserInfo objUser, bool AddToMembershipProvider)
        {
            UserCreateStatus createStatus = CreateUser(ref objUser);
            return objUser.UserID;
        }

        /// <summary>
        /// CacheKey builds the Cache key for the portalId/Username
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="username">The username of the user being retrieved.</param>
        /// <returns>The User as a UserInfo object</returns>
        public static string CacheKey(int portalId, string username)
        {
            return "UserInfo|" + portalId + "|" + username;
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
        public static bool ChangePassword(UserInfo user, string oldPassword, string newPassword)
        {
            bool retValue;

            //Although we would hope that the caller has already validated the password,
            //Validate the new Password
            if (ValidatePassword(newPassword))
            {
                retValue = memberProvider.ChangePassword(user, oldPassword, newPassword);

                //Update User
                user.Membership.UpdatePassword = false;
                UpdateUser(user.PortalID, user);
            }
            else
            {
                throw (new Exception("Invalid Password"));
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
        public static bool ChangePasswordQuestionAndAnswer(UserInfo user, string password, string passwordQuestion, string passwordAnswer)
        {
            return memberProvider.ChangePasswordQuestionAndAnswer(user, password, passwordQuestion, passwordAnswer);
        }

        /// <summary>
        /// Creates a new User in the Data Store
        /// </summary>
        /// <remarks></remarks>
        /// <param name="objUser">The userInfo object to persist to the Database</param>
        /// <returns>The Created status ot the User</returns>
        public static UserCreateStatus CreateUser(ref UserInfo objUser)
        {
            //Create the User
            UserCreateStatus createStatus = memberProvider.CreateUser(ref objUser);

            if (createStatus == UserCreateStatus.Success && !objUser.IsSuperUser)
            {
                RoleController objRoles = new RoleController();

                // autoassign user to portal roles
                ArrayList arrRoles = objRoles.GetPortalRoles(objUser.PortalID);
                
                for (int i = 0; i < arrRoles.Count; i++)
                {
                    RoleInfo objRole = (RoleInfo)arrRoles[i];
                    if (objRole.AutoAssignment)
                    {
                        objRoles.AddUserRole(objUser.PortalID, objUser.UserID, objRole.RoleID, Null.NullDate, Null.NullDate);
                    }
                }
            }

            return createStatus;
        }

        /// <summary>
        /// Deletes an existing User from the Data Store
        /// </summary>
        /// <remarks></remarks>
        /// <param name="objUser">The userInfo object to delete from the Database</param>
        /// <param name="notify">A flag that indicates whether an email notification should be sent</param>
        /// <param name="deleteAdmin">A flag that indicates whether the Portal Administrator should be deleted</param>
        /// <returns>A Boolean value that indicates whether the User was successfully deleted</returns>
        public static bool DeleteUser(ref UserInfo objUser, bool notify, bool deleteAdmin)
        {
            bool CanDelete = true;

            try
            {
                //Determine if the User is the Portal Administrator
                IDataReader dr;
                dr = DataProvider.Instance().GetPortal(objUser.PortalID);
                if (dr.Read())
                {
                    if (objUser.UserID == Convert.ToInt32(dr["AdministratorId"]))
                    {
                        CanDelete = deleteAdmin;
                    }
                }
                if (dr != null)
                {
                    dr.Close();
                }

                if (CanDelete)
                {
                    if (notify)
                    {
                        // Obtain PortalSettings from Current Context
                        PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

                        // send email notification to portal administrator that the user was removed from the portal
                        Mail.SendMail(_portalSettings.Email, _portalSettings.Email, "", Localization.GetSystemMessage(_portalSettings, "EMAIL_USER_UNREGISTER_SUBJECT", objUser), Localization.GetSystemMessage(_portalSettings, "EMAIL_USER_UNREGISTER_BODY", objUser), "", "", "", "", "", "");
                    }

                    CanDelete = memberProvider.DeleteUser(objUser);
                }

                if (CanDelete)
                {
                    string UserInfoCacheKey = CacheKey(objUser.PortalID, objUser.Username);
                    DataCache.RemoveCache(UserInfoCacheKey);
                }
            }
            catch (Exception)
            {
                CanDelete = false;
            }

            return CanDelete;
        }

        [Obsolete("This function has been replaced by UserController.DeleteUser")]
        public bool DeleteUser(int PortalId, int UserId)
        {
            UserInfo objUser = GetUser(PortalId, UserId, false);

            //Call Shared method with notify=true, deleteAdmin=false
            return DeleteUser(ref objUser, true, false);
        }

        [Obsolete("This function has been replaced by UserController.GetUserByName")]
        public UserInfo FillUserInfo(int PortalID, string Username)
        {
            return GetUserByName(PortalID, Username, false);
        }

        /// <summary>
        /// Generates a new random password (Length = Minimum Length + 4)
        /// </summary>
        /// <returns>A String</returns>
        public static string GeneratePassword()
        {
            return GeneratePassword(MembershipProviderConfig.MinPasswordLength + 4);
        }

        /// <summary>
        /// Generates a new random password
        /// </summary>
        /// <param name="length">The length of password to generate.</param>
        /// <returns>A String</returns>
        public static string GeneratePassword(int length)
        {
            return memberProvider.GeneratePassword(length);
        }

        /// <summary>
        /// GetCachedUser retrieves the User from the Cache, or fetches a fresh copy if
        /// not in cache or if Cache settings not set to HeavyCaching
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="username">The username of the user being retrieved.</param>
        /// <returns>The User as a UserInfo object</returns>
        public static UserInfo GetCachedUser(int portalId, string username)
        {
            string UserInfoCacheKey = CacheKey(portalId, username);
            UserInfo objUserInfo = (UserInfo)DataCache.GetCache(UserInfoCacheKey);
            if (objUserInfo == null)
            {
                objUserInfo = GetUserByName(portalId, username, false);
                if (objUserInfo != null)
                {
                    DataCache.SetCache(UserInfoCacheKey, objUserInfo, TimeSpan.FromMinutes((double)Globals.PerformanceSetting));
                }
            }

            return objUserInfo;
        }

        [Obsolete("This function has been replaced by UserController.CacheKey")]
        public string GetCacheKey(int PortalID, string Username)
        {
            return CacheKey(PortalID, Username);
        }

        /// <summary>
        /// Get the current UserInfo object
        /// </summary>
        /// <returns>The current UserInfo if authenticated, oherwise an empty user</returns>
        public static UserInfo GetCurrentUserInfo()
        {
            if (HttpContext.Current == null)
            {
                if (!(Thread.CurrentPrincipal.Identity.IsAuthenticated))
                {
                    return new UserInfo();
                }
                else
                {
                    PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                    UserInfo objUser = GetUserByName(_portalSettings.PortalId, Thread.CurrentPrincipal.Identity.Name, false);
                    if (objUser != null)
                    {
                        return objUser;
                    }
                    else
                    {
                        return new UserInfo();
                    }
                }
            }
            else
            {
                UserInfo objUser = (UserInfo)HttpContext.Current.Items["UserInfo"];
                if (objUser != null)
                {
                    return objUser;
                }
                else
                {
                    return new UserInfo();
                }
            }
        }

        /// <summary>
        /// Gets a collection of Online Users
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <returns>An ArrayList of UserInfo objects</returns>
        public static ArrayList GetOnlineUsers(int PortalId)
        {
            return memberProvider.GetOnlineUsers(PortalId);
        }

        /// <summary>
        /// Gets the Current Password Information for the User
        /// </summary>
        /// <remarks>This method will only return the password if the memberProvider supports
        /// and is using a password encryption method that supports decryption.</remarks>
        /// <param name="user">The user whose Password information we are retrieving.</param>
        /// <param name="passwordAnswer">The answer to the "user's" password Question.</param>
        public static string GetPassword(ref UserInfo user, string passwordAnswer)
        {
            if (MembershipProviderConfig.PasswordRetrievalEnabled)
            {
                user.Membership.Password = memberProvider.GetPassword(user, passwordAnswer);
            }
            else
            {
                //Throw a configuration exception as password retrieval is not enabled
                throw (new ConfigurationErrorsException("Password Retrieval is not enabled"));
            }

            return user.Membership.Password;
        }

        [Obsolete("This function has been replaced by UserController.GetUsers")]
        public ArrayList GetSuperUsers()
        {
            return GetUsers(Null.NullInteger, false);
        }

        /// <summary>
        /// GetUnAuthorizedUsers gets all the users of the portal, that are not authorized
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        public static ArrayList GetUnAuthorizedUsers(int portalId)
        {
            return memberProvider.GetUnAuthorizedUsers(portalId, false);
        }

        /// <summary>
        /// GetUnAuthorizedUsers gets all the users of the portal, that are not authorized
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        public static ArrayList GetUnAuthorizedUsers(int portalId, bool isHydrated)
        {
            return memberProvider.GetUnAuthorizedUsers(portalId, isHydrated);
        }

        /// <summary>
        /// GetUser retrieves a User from the DataStore
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="userId">The Id of the user being retrieved from the Data Store.</param>
        /// <param name="isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <returns>The User as a UserInfo object</returns>
        public static UserInfo GetUser(int portalId, int userId, bool isHydrated)
        {
            return memberProvider.GetUser(portalId, userId, isHydrated);
        }

        /// <summary>
        /// GetUser retrieves a User from the DataStore
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="userId">The Id of the user being retrieved from the Data Store.</param>
        /// <returns>The User as a UserInfo object</returns>
        public UserInfo GetUser(int portalId, int userId)
        {
            return memberProvider.GetUser(portalId, userId, false);
        }

        /// <summary>
        /// GetUserByUserName retrieves a User from the DataStore
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="username">The username of the user being retrieved from the Data Store.</param>
        /// <returns>The User as a UserInfo object</returns>
        public static UserInfo GetUserByName(int portalId, string username)
        {
            return memberProvider.GetUserByUserName(portalId, username, false);
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
        public static UserInfo GetUserByName(int portalId, string username, bool isHydrated)
        {
            return memberProvider.GetUserByUserName(portalId, username, isHydrated);
        }

        [Obsolete("This function has been replaced by UserController.GetUserByName")]
        public UserInfo GetUserByUsername(int PortalID, string Username)
        {
            return GetUserByName(PortalID, Username, false);
        }

        [Obsolete("This function has been replaced by UserController.GetUserByName")]
        public UserInfo GetUserByUsername(int PortalID, string Username, bool SynchronizeUsers)
        {
            return GetUserByName(PortalID, Username, false);
        }

        /// <summary>
        /// GetUserCountByPortal gets the number of users in the portal
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <returns>The no of users</returns>
        public static int GetUserCountByPortal(int portalId)
        {
            return memberProvider.GetUserCountByPortal(portalId);
        }

        /// <summary>
        /// Retruns a String corresponding to the Registration Status of the User
        /// </summary>
        /// <param name="UserRegistrationStatus">The AUserCreateStatus</param>
        /// <returns>A String</returns>
        public static string GetUserCreateStatus(UserCreateStatus UserRegistrationStatus)
        {
            if (UserRegistrationStatus == UserCreateStatus.DuplicateEmail)
            {
                return Localization.GetString("UserEmailExists");
            }
            else if (UserRegistrationStatus == UserCreateStatus.InvalidAnswer)
            {
                return Localization.GetString("InvalidAnswer");
            }
            else if (UserRegistrationStatus == UserCreateStatus.InvalidEmail)
            {
                return Localization.GetString("InvalidEmail");
            }
            else if (UserRegistrationStatus == UserCreateStatus.InvalidQuestion)
            {
                return Localization.GetString("InvalidQuestion");
            }
            else if (UserRegistrationStatus == UserCreateStatus.InvalidUserName)
            {
                return Localization.GetString("InvalidUserName");
            }
            else if (UserRegistrationStatus == UserCreateStatus.UserRejected)
            {
                return Localization.GetString("UserRejected");
            }
            else if (UserRegistrationStatus == UserCreateStatus.PasswordMismatch)
            {
                return Localization.GetString("PasswordMismatch");
            }
            else if (UserRegistrationStatus == UserCreateStatus.UsernameAlreadyExists || UserRegistrationStatus == UserCreateStatus.UserAlreadyRegistered || UserRegistrationStatus == UserCreateStatus.DuplicateUserName)
            {
                return Localization.GetString("UserNameExists");
            }
            else if (UserRegistrationStatus == UserCreateStatus.DuplicateProviderUserKey || UserRegistrationStatus == UserCreateStatus.InvalidProviderUserKey || UserRegistrationStatus == UserCreateStatus.ProviderError)
            {
                return Localization.GetString("RegError");
            }
            else if (UserRegistrationStatus == UserCreateStatus.Success || UserRegistrationStatus == UserCreateStatus.UnexpectedError)
            {
                return null;
            }
            else if (UserRegistrationStatus == UserCreateStatus.InvalidPassword)
            {
                string replace = Localization.GetString("InvalidPassword");
                replace = replace.Replace("[PasswordLength]", MembershipProviderConfig.MinPasswordLength.ToString());
                return replace.Replace("[NoneAlphabet]", MembershipProviderConfig.MinNonAlphanumericCharacters.ToString());
            }
            return null;
        }

        /// <summary>
        /// GetUsers gets all the users of the portal
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        public static ArrayList GetUsers(int portalId)
        {
            int ii = -1;
            return GetUsers(portalId, false, -1, -1, ref ii);
        }

        /// <summary>
        /// GetUsers gets all the users of the portal
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        public static ArrayList GetUsers(int portalId, bool isHydrated)
        {
            int ii = -1;
            return GetUsers(portalId, isHydrated, -1, -1, ref ii);
        }

        /// <summary>
        /// GetUsers gets all the users of the portal, by page
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="pageIndex">The page of records to return.</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        public static ArrayList GetUsers(int portalId, int pageIndex, int pageSize, ref int totalRecords)
        {
            return memberProvider.GetUsers(portalId, false, pageIndex, pageSize, ref totalRecords);
        }

        /// <summary>
        /// GetUsers gets all the users of the portal, by page
        /// </summary>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="isHydrated"></param>
        /// <param name="pageIndex">The page of records to return.</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        public static ArrayList GetUsers(int portalId, bool isHydrated, int pageIndex, int pageSize, ref int totalRecords)
        {
            return memberProvider.GetUsers(portalId, isHydrated, pageIndex, pageSize, ref totalRecords);
        }

        [Obsolete("This function has been replaced by UserController.GetUsers")]
        public ArrayList GetUsers(bool SynchronizeUsers, bool ProgressiveHydration)
        {
            return GetUsers(Null.NullInteger, ProgressiveHydration);
        }

        [Obsolete("This function has been replaced by UserController.GetUsers")]
        public ArrayList GetUsers(int PortalId, bool SynchronizeUsers, bool ProgressiveHydration)
        {
            return GetUsers(PortalId, ProgressiveHydration);
        }

        /// <summary>
        /// GetUsersByEmail gets all the users of the portal whose email matches a provided
        /// filter expression
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="emailToMatch">The email address to use to find a match.</param>
        /// <param name="pageIndex">The page of records to return.</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        public static ArrayList GetUsersByEmail(int portalId, string emailToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            return memberProvider.GetUsersByEmail(portalId, false, emailToMatch, pageIndex, pageSize, ref totalRecords);
        }

        /// <summary>
        /// GetUsersByEmail gets all the users of the portal whose email matches a provided
        /// filter expression
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <param name="emailToMatch">The email address to use to find a match.</param>
        /// <param name="pageIndex">The page of records to return.</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        public static ArrayList GetUsersByEmail(int portalId, bool isHydrated, string emailToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            return memberProvider.GetUsersByEmail(portalId, isHydrated, emailToMatch, pageIndex, pageSize, ref totalRecords);
        }

        /// <summary>
        /// GetUsersByProfileProperty gets all the users of the portal whose profile matches
        /// the profile property pased as a parameter
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="propertyName">The name of the property being matched.</param>
        /// <param name="propertyValue">The value of the property being matched.</param>
        /// <param name="pageIndex">The page of records to return.</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        public static ArrayList GetUsersByProfileProperty(int portalId, string propertyName, string propertyValue, int pageIndex, int pageSize, ref int totalRecords)
        {
            return memberProvider.GetUsersByProfileProperty(portalId, false, propertyName, propertyValue, pageIndex, pageSize, ref totalRecords);
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
        public static ArrayList GetUsersByProfileProperty(int portalId, bool isHydrated, string propertyName, string propertyValue, int pageIndex, int pageSize, ref int totalRecords)
        {
            return memberProvider.GetUsersByProfileProperty(portalId, isHydrated, propertyName, propertyValue, pageIndex, pageSize, ref totalRecords);
        }

        /// <summary>
        /// GetUsersByUserName gets all the users of the portal whose username matches a provided
        /// filter expression
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="userNameToMatch">The username to use to find a match.</param>
        /// <param name="pageIndex">The page of records to return.</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        public static ArrayList GetUsersByUserName(int portalId, string userNameToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            return memberProvider.GetUsersByUserName(portalId, false, userNameToMatch, pageIndex, pageSize, ref totalRecords);
        }

        /// <summary>
        /// GetUsersByUserName gets all the users of the portal whose username matches a provided
        /// filter expression
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="isHydrated">A flag that determines whether the user is hydrated.</param>
        /// <param name="userNameToMatch">The username to use to find a match.</param>
        /// <param name="pageIndex">The page of records to return.</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        /// <returns>An ArrayList of UserInfo objects.</returns>
        public static ArrayList GetUsersByUserName(int portalId, bool isHydrated, string userNameToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            return memberProvider.GetUsersByUserName(portalId, isHydrated, userNameToMatch, pageIndex, pageSize, ref totalRecords);
        }

        /// <summary>
        /// GetUserSettings retrieves the UserSettings from the User
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <returns>The Settings Hashtable</returns>
        public static Hashtable GetUserSettings(int portalId)
{

            string SettingsCacheKey = SettingsKey(portalId);
            Hashtable settings = (Hashtable)(DataCache.GetCache(SettingsCacheKey));
            if (settings == null)
            {
                ModuleController objModules = new ModuleController();
                ModuleInfo objModule = objModules.GetModuleByDefinition(portalId, "User Accounts");
                if (objModule != null)
                {
                    settings = objModules.GetModuleSettings(objModule.ModuleID);
                    DataCache.SetCache(SettingsCacheKey, settings, TimeSpan.FromMinutes((double)Globals.PerformanceSetting));
                }
            }

            return settings;

        }

        /// <summary>
        /// Resets the password for the specified user
        /// </summary>
        /// <remarks>Resets the user's password</remarks>
        /// <param name="user">The user whose Password information we are resetting.</param>
        /// <param name="passwordAnswer">The answer to the "user's" password Question.</param>
        public static string ResetPassword(UserInfo user, string passwordAnswer)
        {
            if (MembershipProviderConfig.PasswordResetEnabled)
            {
                user.Membership.Password = memberProvider.ResetPassword(user, passwordAnswer);
            }
            else
            {
                //Throw a configuration exception as password reset is not enabled
                throw (new ConfigurationErrorsException("Password Reset is not enabled"));
            }

            return user.Membership.Password;
        }

        [Obsolete("This function has been replaced by UserController.ChangePassword")]
        public bool SetPassword(UserInfo objUser, string newPassword)
        {
            return ChangePassword(objUser, Null.NullString, newPassword);
        }

        [Obsolete("This function has been replaced by UserController.ChangePassword")]
        public bool SetPassword(UserInfo objUser, string oldPassword, string newPassword)
        {
            return ChangePassword(objUser, oldPassword, newPassword);
        }

        public static string SettingsKey(int portalId)
        {
            return "UserSettings|" + portalId;
        }

        /// <summary>
        /// Unlocks the User's Account
        /// </summary>
        /// <remarks></remarks>
        /// <param name="user">The user whose account is being Unlocked.</param>
        public static bool UnLockUser(UserInfo user)
        {
            string UserInfoCacheKey = CacheKey(user.PortalID, user.Username);
            bool retValue;

            //Unlock the User
            retValue = memberProvider.UnLockUser(user);

            DataCache.RemoveCache(UserInfoCacheKey);

            return retValue;
        }

        /// <summary>
        /// Validates a User's credentials against the Data Store, and sets the Forms Authentication
        /// Ticket
        /// </summary>
        /// <param name="portalId">The Id of the Portal the user belongs to</param>
        /// <param name="Username">The user name of the User attempting to log in</param>
        /// <param name="Password">The password of the User attempting to log in</param>
        /// <param name="VerificationCode">The verification code of the User attempting to log in</param>
        /// <param name="PortalName">The name of the Portal</param>
        /// <param name="IP">The IP Address of the user attempting to log in</param>
        /// <param name="loginStatus">A UserLoginStatus enumeration that indicates the status of the
        /// Login attempt.  This value is returned by reference.</param>
        /// <param name="CreatePersistentCookie">A flag that indicates whether the login credentials
        /// should be persisted.</param>
        /// <returns>The UserInfo object representing a successful login</returns>
        public static UserInfo UserLogin(int portalId, string Username, string Password, string VerificationCode, string PortalName, string IP, ref UserLoginStatus loginStatus, bool CreatePersistentCookie)
        {
            loginStatus = UserLoginStatus.LOGIN_FAILURE;

            //Validate the user
            UserInfo objUser = ValidateUser(portalId, Username, Password, VerificationCode, PortalName, IP, ref loginStatus);

            if (objUser != null)
            {
                //Call UserLogin overload
                UserLogin(portalId, objUser, PortalName, IP, CreatePersistentCookie);
            }
            else
            {
                AddEventLog(portalId, Username, Null.NullInteger, PortalName, IP, loginStatus);
            }

            // return the User object
            return objUser;
        }

        /// <summary>
        /// Validates a Password
        /// </summary>
        /// <param name="password">The password to Validate</param>
        /// <returns>A boolean</returns>
        public static bool ValidatePassword(string password)
        {
            bool isValid = true;
            Regex rx;

            //Valid Length
            if (password.Length < MembershipProviderConfig.MinPasswordLength)
            {
                isValid = false;
            }

            //Validate NonAlphaChars
            rx = new Regex("[^0-9a-zA-Z]");
            if (rx.Matches(password).Count < MembershipProviderConfig.MinNonAlphanumericCharacters)
            {
                isValid = false;
            }

            //Validate Regex
            if (!String.IsNullOrEmpty(MembershipProviderConfig.PasswordStrengthRegularExpression))
            {
                rx = new Regex(MembershipProviderConfig.PasswordStrengthRegularExpression);
                isValid = rx.IsMatch(password);
            }

            return isValid;
        }

        /// <summary>
        /// Validates a User's credentials against the Data Store
        /// </summary>
        /// <param name="portalId">The Id of the Portal the user belongs to</param>
        /// <param name="Username">The user name of the User attempting to log in</param>
        /// <param name="Password">The password of the User attempting to log in</param>
        /// <param name="VerificationCode">The verification code of the User attempting to log in</param>
        /// <param name="PortalName">The name of the Portal</param>
        /// <param name="IP">The IP Address of the user attempting to log in</param>
        /// <param name="loginStatus">A UserLoginStatus enumeration that indicates the status of the
        /// Login attempt.  This value is returned by reference.</param>
        /// <returns>The UserInfo object representing a valid user</returns>
        public static UserInfo ValidateUser(int portalId, string Username, string Password, string VerificationCode, string PortalName, string IP, ref UserLoginStatus loginStatus)
        {
            loginStatus = UserLoginStatus.LOGIN_FAILURE;

            //Try and Log the user in
            UserInfo objUser = memberProvider.UserLogin(portalId, Username, Password, VerificationCode, ref loginStatus);

            if (loginStatus == UserLoginStatus.LOGIN_USERLOCKEDOUT || loginStatus == UserLoginStatus.LOGIN_FAILURE)
            {
                //User Locked Out so log to event log
                AddEventLog(portalId, Username, Null.NullInteger, PortalName, IP, loginStatus);
            }

            // return the User object
            return objUser;
        }

        private static void AddEventLog(int portalId, string username, int userId, string portalName, string Ip, UserLoginStatus loginStatus)
        {
            EventLogController objEventLog = new EventLogController();

            // initialize log record
            LogInfo objEventLogInfo = new LogInfo();
            PortalSecurity objSecurity = new PortalSecurity();
            objEventLogInfo.AddProperty("IP", Ip);
            objEventLogInfo.LogPortalID = portalId;
            objEventLogInfo.LogPortalName = portalName;
            objEventLogInfo.LogUserName = objSecurity.InputFilter(username, (PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup));
            objEventLogInfo.LogUserID = userId;

            // create log record
            objEventLogInfo.LogTypeKey = loginStatus.ToString();
            objEventLog.AddLog(objEventLogInfo);
        }

        [Obsolete("This function has been replaced by UserController.DeleteUsers")]
        public void DeleteAllUsers(int PortalId)
        {
            DeleteUsers(PortalId, false, true);
        }

        /// <summary>
        /// Deletes all Unauthorized Users for a Portal
        /// </summary>
        /// <remarks></remarks>
        /// <param name="portalId">The Id of the Portal</param>
        public static void DeleteUnauthorizedUsers(int portalId)
        {
            ArrayList arrUsers = GetUsers(portalId, false);

            foreach (UserInfo objUser in arrUsers)
            {
                if (objUser.Membership.Approved == false || objUser.Membership.LastLoginDate == Null.NullDate)
                {
                    UserInfo userInfo = objUser;
                    DeleteUser(ref userInfo, true, false);
                }
            }
        }

        /// <summary>
        /// Deletes all Users for a Portal
        /// </summary>
        /// <remarks></remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="notify">A flag that indicates whether an email notification should be sent</param>
        /// <param name="deleteAdmin">A flag that indicates whether the Portal Administrator should be deleted</param>
        public static void DeleteUsers(int portalId, bool notify, bool deleteAdmin)
        {
            ArrayList arrUsers = GetUsers(portalId, false);

            foreach (UserInfo objUser in arrUsers)
            {
                UserInfo userInfo = objUser;
                DeleteUser(ref userInfo, notify, deleteAdmin);
            }
        }

        [Obsolete("This function has been replaced by UserController.DeleteUsers")]
        public void DeleteUsers(int PortalId)
        {
            DeleteUsers(PortalId, true, false);
        }

        /// <summary>
        /// Gets the Membership Information for the User
        /// </summary>
        /// <remarks></remarks>
        /// <param name="objUser">The user whose Membership information we are retrieving.</param>
        public static void GetUserMembership(ref UserInfo objUser)
        {
            memberProvider.GetUserMembership(ref objUser);
        }

        public static void SetAuthCookie(string username, bool CreatePersistentCookie)
        {
        }

        [Obsolete("This function has been replaced by UserController.UnlockUserAccount")]
        public void UnlockUserAccount(UserInfo objUser)
        {
            UnLockUser(objUser);
        }

        /// <summary>
        /// Updates a User
        /// </summary>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="objUser">The use to update</param>
        /// <remarks>
        /// </remarks>
        public static void UpdateUser(int portalId, UserInfo objUser)
        {
            string UserInfoCacheKey = CacheKey(portalId, objUser.Username);

            //Update the User
            memberProvider.UpdateUser(objUser);

            //Remove the UserInfo from the Cache, as it has been modified
            DataCache.RemoveCache(UserInfoCacheKey);
        }

        [Obsolete("This function has been replaced by UserController.UpdateUser")]
        public void UpdateUser(UserInfo objUser)
        {
            UpdateUser(objUser.PortalID, objUser);
        }

        /// <summary>
        /// Logs a Validated User in
        /// </summary>
        /// <param name="portalId">The Id of the Portal the user belongs to</param>
        /// <param name="user">The validated User</param>
        /// <param name="PortalName">The name of the Portal</param>
        /// <param name="IP">The IP Address of the user attempting to log in</param>
        /// <param name="CreatePersistentCookie">A flag that indicates whether the login credentials
        /// should be persisted.</param>
        public static void UserLogin(int portalId, UserInfo user, string PortalName, string IP, bool CreatePersistentCookie)
        {
            if (user.IsSuperUser)
            {
                AddEventLog(portalId, user.Username, user.UserID, PortalName, IP, UserLoginStatus.LOGIN_SUPERUSER);
            }
            else
            {
                AddEventLog(portalId, user.Username, user.UserID, PortalName, IP, UserLoginStatus.LOGIN_SUCCESS);
            }

            // set the forms authentication cookie ( log the user in )
            FormsAuthentication.SetAuthCookie(user.Username, CreatePersistentCookie);

            //check if cookie is persistent, and user has supplied custom value for expiration
            if (CreatePersistentCookie)
            {
                if (Config.GetSetting("PersistentCookieTimeout") != null)
                {
                    int PersistentCookieTimeout = int.Parse(Config.GetSetting("PersistentCookieTimeout"));
                    //only use if non-zero, otherwise leave as asp.net value
                    if (PersistentCookieTimeout != 0)
                    {
                        //locate and update cookie
                        string authCookie = FormsAuthentication.FormsCookieName;
                        foreach (string cookie in HttpContext.Current.Response.Cookies)
                        {
                            if (cookie.Equals(authCookie))
                            {
                                HttpContext.Current.Response.Cookies[cookie].Expires = DateTime.Now.AddMinutes(PersistentCookieTimeout);
                            }
                        }
                    }
                }
            }

        }
    }
}