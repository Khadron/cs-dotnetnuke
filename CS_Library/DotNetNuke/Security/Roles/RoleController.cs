using System;
using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using Microsoft.VisualBasic;

namespace DotNetNuke.Security.Roles
{
    /// <summary>
    /// The RoleController class provides Business Layer methods for Roles
    /// </summary>
    public class RoleController
    {
        private static RoleProvider provider = RoleProvider.Instance();

        /// <summary>
        /// This overload adds a role and optionally adds the info to the AspNet Roles
        /// </summary>
        /// <param name="objRoleInfo">The Role to Add</param>
        /// <returns>The Id of the new role</returns>
        public int AddRole(RoleInfo objRoleInfo)
        {
            int roleId = -1;
            bool success = provider.CreateRole(objRoleInfo.PortalID, ref objRoleInfo);

            if (success)
            {
                AutoAssignUsers(objRoleInfo);
                roleId = objRoleInfo.RoleID;
            }

            return roleId;
        }

        [Obsolete("This function has been replaced by AddRole(objRoleInfo)")]
        public int AddRole(RoleInfo objRoleInfo, bool SynchronizationMode)
        {
            return AddRole(objRoleInfo);
        }

        /// <summary>
        /// Adds a Role Group
        /// </summary>
        /// <param name="objRoleGroupInfo">The RoleGroup to Add</param>
        /// <returns>The Id of the new role</returns>
        public static int AddRoleGroup(RoleGroupInfo objRoleGroupInfo)
        {
            return provider.CreateRoleGroup(objRoleGroupInfo);
        }

        /// <summary>
        /// Delete/Remove a User from a Role
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <param name="UserId">The Id of the User</param>
        /// <param name="RoleId">The Id of the Role</param>
        /// <returns></returns>
        public bool DeleteUserRole(int PortalId, int UserId, int RoleId)
        {
            UserInfo objUser = UserController.GetUser(PortalId, UserId, false);
            UserRoleInfo objUserRole = GetUserRole(PortalId, UserId, RoleId);

            PortalController objPortals = new PortalController();
            bool blnDelete = true;

            PortalInfo objPortal = objPortals.GetPortal(PortalId);
            if (objPortal != null)
            {
                if ((objPortal.AdministratorId != UserId || objPortal.AdministratorRoleId != RoleId) && objPortal.RegisteredRoleId != RoleId)
                {
                    provider.RemoveUserFromRole(PortalId, objUser, objUserRole);
                }
                else
                {
                    blnDelete = false;
                }
            }

            return blnDelete;
        }

        /// <summary>
        /// Get a list of roles for the Portal
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <returns>An ArrayList of RoleInfo objects</returns>
        public ArrayList GetPortalRoles(int PortalId)
        {
            return provider.GetRoles(PortalId);
        }

        [Obsolete("This function has been replaced by GetPortalRoles(PortalId)")]
        public ArrayList GetPortalRoles(int PortalId, bool SynchronizeRoles)
        {
            return GetPortalRoles(PortalId);
        }

        [Obsolete("This function has been replaced by GetRolesByUser")]
        public string[] GetPortalRolesByUser(int UserId, int PortalId)
        {
            return GetRolesByUser(UserId, PortalId);
        }

        /// <summary>
        /// Fetch a single Role
        /// </summary>
        /// <param name="RoleID">The Id of the Role</param>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public RoleInfo GetRole(int RoleID, int PortalID)
        {
            return provider.GetRole(PortalID, RoleID);
        }

        /// <summary>
        /// Obtains a role given the role name
        /// </summary>
        /// <param name="PortalId">Portal indentifier</param>
        /// <param name="RoleName">Name of the role to be found</param>
        /// <returns>A RoleInfo object is the role is found</returns>
        public RoleInfo GetRoleByName(int PortalId, string RoleName)
        {
            return provider.GetRole(PortalId, RoleName);
        }

        /// <summary>
        /// Fetch a single RoleGroup
        /// </summary>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static RoleGroupInfo GetRoleGroup(int PortalID, int RoleGroupID)
        {
            return provider.GetRoleGroup(PortalID, RoleGroupID);
        }

        /// <summary>
        /// Gets an ArrayList of RoleGroups
        /// </summary>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <returns>An ArrayList of RoleGroups</returns>
        public static ArrayList GetRoleGroups(int PortalID)
        {
            return provider.GetRoleGroups(PortalID);
        }

        /// <summary>
        /// Returns an array of rolenames for a Portal
        /// </summary>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <returns>A String Array of Role Names</returns>
        public string[] GetRoleNames(int PortalID)
        {
            return provider.GetRoleNames(PortalID);
        }

        /// <summary>
        /// Gets an ArrayList of Roles
        /// </summary>
        /// <returns>An ArrayList of Roles</returns>
        public ArrayList GetRoles()
        {
            return provider.GetRoles(Null.NullInteger);
        }

        /// <summary>
        /// Get the roles for a Role Group
        /// </summary>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="roleGroupId">Id of the Role Group (If -1 all roles for the portal are
        /// retrieved).</param>
        /// <returns>An ArrayList of RoleInfo objects</returns>
        public ArrayList GetRolesByGroup(int portalId, int roleGroupId)
        {
            return provider.GetRolesByGroup(portalId, roleGroupId);
        }

        /// <summary>
        /// Gets a List of Roles for a given User
        /// </summary>
        /// <param name="UserId">The Id of the User</param>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <returns>A String Array of Role Names</returns>
        public string[] GetRolesByUser(int UserId, int PortalId)
        {
            return provider.GetRoleNames(PortalId, UserId);
        }

        [Obsolete("This function has been replaced by GetUserRoles")]
        public ArrayList GetServices(int PortalId)
        {
            return GetUserRoles(PortalId, -1, false);
        }

        [Obsolete("This function has been replaced by GetUserRoles")]
        public ArrayList GetServices(int PortalId, int UserId)
        {
            return GetUserRoles(PortalId, UserId, false);
        }

        /// <summary>
        /// Gets a User/Role
        /// </summary>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <param name="UserId">The Id of the user</param>
        /// <param name="RoleId">The Id of the Role</param>
        /// <returns>A UserRoleInfo object</returns>
        public UserRoleInfo GetUserRole(int PortalID, int UserId, int RoleId)
        {
            return provider.GetUserRole(PortalID, UserId, RoleId);
        }

        /// <summary>
        /// Gets a list of UserRoles for a Portal
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <returns>An ArrayList of UserRoleInfo objects</returns>
        public ArrayList GetUserRoles(int PortalId)
        {
            return GetUserRoles(PortalId, -1);
        }

        /// <summary>
        /// Gets a list of UserRoles for a User
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <param name="UserId">The Id of the User</param>
        /// <returns>An ArrayList of UserRoleInfo objects</returns>
        public ArrayList GetUserRoles(int PortalId, int UserId)
        {
            return provider.GetUserRoles(PortalId, UserId, true);
        }

        /// <summary>
        /// Gets a list of UserRoles for a User
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <param name="UserId">The Id of the User</param>
        /// <returns>An ArrayList of UserRoleInfo objects</returns>
        public ArrayList GetUserRoles(int PortalId, int UserId, bool includePrivate)
        {
            return provider.GetUserRoles(PortalId, UserId, includePrivate);
        }

        /// <summary>
        /// Get the users in a role (as UserRole objects)
        /// </summary>
        /// <param name="portalId">Id of the portal (If -1 all roles for all portals are
        /// retrieved.</param>
        /// <param name="roleName">The role to fetch users for</param>
        /// <returns>An ArrayList of UserRoleInfo objects</returns>
        public ArrayList GetUserRolesByRoleName(int portalId, string roleName)
        {
            return provider.GetUserRolesByRoleName(portalId, roleName);
        }

        /// <summary>
        /// Gets a List of UserRoles by UserName and RoleName
        /// </summary>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <param name="Username">The username of the user</param>
        /// <param name="Rolename">The role name</param>
        /// <returns>An ArrayList of UserRoleInfo objects</returns>
        public ArrayList GetUserRolesByUsername(int PortalID, string Username, string Rolename)
        {
            return provider.GetUserRoles(PortalID, Username, Rolename);
        }

        /// <summary>
        /// Get the users in a role (as User objects)
        /// </summary>
        /// <param name="PortalID">Id of the portal (If -1 all roles for all portals are
        /// retrieved.</param>
        /// <param name="RoleName">The role to fetch users for</param>
        /// <returns>An ArrayList of UserInfo objects</returns>
        public ArrayList GetUsersByRoleName(int PortalID, string RoleName)
        {
            return provider.GetUsersByRoleName(PortalID, RoleName);
        }

        [Obsolete("This function has been replaced by GetUserRolesByRoleName")]
        public ArrayList GetUsersInRole(int PortalID, string RoleName)
        {
            return provider.GetUserRolesByRoleName(PortalID, RoleName);
        }

        /// <summary>
        /// Adds a User to a Role
        /// </summary>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <param name="UserId">The Id of the User</param>
        /// <param name="RoleId">The Id of the Role</param>
        /// <param name="ExpiryDate">The expiry Date of the Role membership</param>
        public void AddUserRole(int PortalID, int UserId, int RoleId, DateTime ExpiryDate)
        {
            AddUserRole(PortalID, UserId, RoleId, DateTime.Now, ExpiryDate);
        }

        /// <summary>
        /// Adds a User to a Role
        /// </summary>
        /// <remarks>Overload adds Effective Date</remarks>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <param name="UserId">The Id of the User</param>
        /// <param name="RoleId">The Id of the Role</param>
        /// <param name="EffectiveDate">The expiry Date of the Role membership</param>
        /// <param name="ExpiryDate">The expiry Date of the Role membership</param>
        public void AddUserRole(int PortalID, int UserId, int RoleId, DateTime EffectiveDate, DateTime ExpiryDate)
        {
            UserInfo objUser = UserController.GetUser(PortalID, UserId, false);
            UserRoleInfo objUserRole = GetUserRole(PortalID, UserId, RoleId);

            if (objUserRole == null)
            {
                //Create new UserRole
                objUserRole = new UserRoleInfo();
                objUserRole.UserID = UserId;
                objUserRole.RoleID = RoleId;
                objUserRole.PortalID = PortalID;
                objUserRole.EffectiveDate = EffectiveDate;
                objUserRole.ExpiryDate = ExpiryDate;
                provider.AddUserToRole(PortalID, objUser, objUserRole);
            }
            else
            {
                objUserRole.EffectiveDate = EffectiveDate;
                objUserRole.ExpiryDate = ExpiryDate;
                provider.UpdateUserRole(objUserRole);
            }
        }

        /// <summary>
        /// Auto Assign existing users to a role
        /// </summary>
        /// <param name="objRoleInfo">The Role to Auto assign</param>
        private void AutoAssignUsers(RoleInfo objRoleInfo)
        {
            if (objRoleInfo.AutoAssignment)
            {
                // loop through users for portal and add to role
                ArrayList arrUsers = UserController.GetUsers(objRoleInfo.PortalID, false);
                foreach (UserInfo objUser in arrUsers)
                {
                    try
                    {
                        AddUserRole(objRoleInfo.PortalID, objUser.UserID, objRoleInfo.RoleID, Null.NullDate, Null.NullDate);
                    }
                    catch (Exception)
                    {
                        // user already belongs to role
                    }
                }
            }
        }

        /// <summary>
        /// Delete a Role
        /// </summary>
        /// <param name="RoleId">The Id of the Role to delete</param>
        /// <param name="PortalId">The Id of the Portal</param>
        public void DeleteRole(int RoleId, int PortalId)
        {
            RoleInfo objRole = GetRole(RoleId, PortalId);

            if (objRole != null)
            {
                provider.DeleteRole(PortalId, ref objRole);
            }
        }

        /// <summary>
        /// Deletes a Role Group
        /// </summary>
        public static void DeleteRoleGroup(int PortalID, int RoleGroupId)
        {
            DeleteRoleGroup(GetRoleGroup(PortalID, RoleGroupId));
        }

        /// <summary>
        /// Deletes a Role Group
        /// </summary>
        /// <param name="objRoleGroupInfo">The RoleGroup to Delete</param>
        public static void DeleteRoleGroup(RoleGroupInfo objRoleGroupInfo)
        {
            provider.DeleteRoleGroup(objRoleGroupInfo);
        }

        /// <summary>
        /// Persists a role to the Data Store
        /// </summary>
        /// <param name="objRoleInfo">The role to persist</param>
        public void UpdateRole(RoleInfo objRoleInfo)
        {
            provider.UpdateRole(objRoleInfo);
            AutoAssignUsers(objRoleInfo);
        }

        /// <summary>
        /// Updates a Role Group
        /// </summary>
        /// <param name="objRoleGroupInfo">The RoleGroup to Update</param>
        public static void UpdateRoleGroup(RoleGroupInfo objRoleGroupInfo)
        {
            provider.UpdateRoleGroup(objRoleGroupInfo);
        }

        [Obsolete("This function has been replaced by UpdateUserRole")]
        public void UpdateService(int PortalId, int UserId, int RoleId)
        {
            UpdateUserRole(PortalId, UserId, RoleId, false);
        }

        [Obsolete("This function has been replaced by UpdateUserRole")]
        public void UpdateService(int PortalId, int UserId, int RoleId, bool Cancel)
        {
            UpdateUserRole(PortalId, UserId, RoleId, Cancel);
        }

        /// <summary>
        /// Updates a Service (UserRole)
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <param name="UserId">The Id of the User</param>
        /// <param name="RoleId">The Id of the Role</param>
        public void UpdateUserRole(int PortalId, int UserId, int RoleId)
        {
            UpdateUserRole(PortalId, UserId, RoleId, false);
        }

        /// <summary>
        /// Updates a Service (UserRole)
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <param name="UserId">The Id of the User</param>
        /// <param name="RoleId">The Id of the Role</param>
        /// <param name="Cancel">A flag that indicates whether to cancel (delete) the userrole</param>
        public void UpdateUserRole(int PortalId, int UserId, int RoleId, bool Cancel)
        {
            if (Cancel)
            {
                DeleteUserRole(PortalId, UserId, RoleId);
            }
            else
            {
                int UserRoleId = -1;
                UserRoleInfo userRole;
                RoleInfo role;
                DateTime ExpiryDate = DateTime.Now;
                DateTime EffectiveDate = Null.NullDate;
                bool IsTrialUsed = false;
                int Period = 0;
                string Frequency = "";

                userRole = GetUserRole(PortalId, UserId, RoleId);

                if (userRole != null)
                {
                    UserRoleId = userRole.UserRoleID;
                    EffectiveDate = userRole.EffectiveDate;
                    ExpiryDate = userRole.ExpiryDate;
                    IsTrialUsed = userRole.IsTrialUsed;
                }

                role = GetRole(RoleId, PortalId);

                if (role != null)
                {
                    if (IsTrialUsed == false && role.TrialFrequency.ToString() != "N")
                    {
                        Period = role.TrialPeriod;
                        Frequency = role.TrialFrequency;
                    }
                    else
                    {
                        Period = role.BillingPeriod;
                        Frequency = role.BillingFrequency;
                    }
                }

                if (EffectiveDate < DateTime.Now)
                {
                    EffectiveDate = Null.NullDate;
                }
                if (ExpiryDate < DateTime.Now)
                {
                    ExpiryDate = DateTime.Now;
                }

                if (Frequency == "N")
                {
                    ExpiryDate = Null.NullDate;
                }
                else if (Frequency == "O")
                {
                    ExpiryDate = new DateTime(9999, 12, 31);
                }
                else if (Frequency == "D")
                {
                    ExpiryDate = DateAndTime.DateAdd(DateInterval.Day, Period, Convert.ToDateTime(ExpiryDate));
                }
                else if (Frequency == "W")
                {
                    ExpiryDate = DateAndTime.DateAdd(DateInterval.Day, (Period * 7), Convert.ToDateTime(ExpiryDate));
                }
                else if (Frequency == "M")
                {
                    ExpiryDate = DateAndTime.DateAdd(DateInterval.Month, Period, Convert.ToDateTime(ExpiryDate));
                }
                else if (Frequency == "Y")
                {
                    ExpiryDate = DateAndTime.DateAdd(DateInterval.Year, Period, Convert.ToDateTime(ExpiryDate));
                }

                if (UserRoleId != -1)
                {
                    userRole.ExpiryDate = ExpiryDate;
                    provider.UpdateUserRole(userRole);
                }
                else
                {
                    AddUserRole(PortalId, UserId, RoleId, EffectiveDate, ExpiryDate);
                }
            }
        }
    }
}