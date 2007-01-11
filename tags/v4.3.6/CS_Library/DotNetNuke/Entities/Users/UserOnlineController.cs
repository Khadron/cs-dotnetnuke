#region DotNetNuke License
// DotNetNuke� - http://www.dotnetnuke.com
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
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Membership;

namespace DotNetNuke.Entities.Users
{
    /// <Summary>
    /// The UserOnlineController class provides Business Layer methods for Users Online
    /// </Summary>
    public class UserOnlineController
    {
        private static MembershipProvider memberProvider = MembershipProvider.Instance();

        /// <summary>
        /// Gets the Online time window
        /// </summary>
        public int GetOnlineTimeWindow()
        {
            if (Globals.HostSettings.Contains("UsersOnlineTime"))
            {
                // Try casting the setting
                try
                {
                    return Convert.ToInt32(Globals.HostSettings["UsersOnlineTime"].ToString());
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                // Return a default of 15
                return 15;
            }
        }

        /// <summary>
        /// Gets the cached Users Online Information
        /// </summary>
        public Hashtable GetUserList()
        {
            string key = "OnlineUserList";
            Hashtable userList = (Hashtable)DataCache.GetCache(key);

            //Do we have the Hashtable?
            if (userList == null)
            {
                userList = new Hashtable();
                DataCache.SetCache(key, userList);
            }

            return userList;
        }

        /// <summary>
        /// Gets whether the Users Online functionality is enabled
        /// </summary>
        public bool IsEnabled()
        {
            if (Globals.HostSettings.Contains("DisableUsersOnline"))
            {
                if (Globals.HostSettings["DisableUsersOnline"].ToString() == "N")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether a User is online
        /// </summary>
        public bool IsUserOnline(UserInfo user)
        {
            bool isOnline = false;
            if (IsEnabled())
            {
                isOnline = memberProvider.IsUserOnline(user);
            }
            return isOnline;
        }

        /// <summary>
        /// Clears the cached Users Online Information
        /// </summary>
        public void ClearUserList()
        {
            string key = "OnlineUserList";

            DataCache.RemoveCache(key);
        }

        /// <summary>
        /// Sets the cached Users Online Information
        /// </summary>
        public void SetUserList(Hashtable userList)
        {
            string key = "OnlineUserList";

            DataCache.SetCache(key, userList);
        }

        /// <summary>
        /// Tracks an Anonymous User
        /// </summary>
        /// <param name="context">An HttpContext Object</param>
        private void TrackAnonymousUser(HttpContext context)
        {
            string cookieName = "DotNetNukeAnonymous";

            PortalSettings portalSettings = (PortalSettings)context.Items["PortalSettings"];

            if (portalSettings == null)
            {
                return;
            }

            AnonymousUserInfo user;
            Hashtable userList = GetUserList();
            string userID;

            // Check if the Tracking cookie exists
            HttpCookie cookie = context.Request.Cookies[cookieName];

            // Track Anonymous User
            if (cookie == null)
            {
                // Create a temporary userId
                userID = Guid.NewGuid().ToString();

                // Create a new cookie
                cookie = new HttpCookie(cookieName);
                cookie.Value = userID;
                cookie.Expires = DateTime.Now.AddMinutes(20);
                context.Response.Cookies.Add(cookie);

                // Create a user
                user = new AnonymousUserInfo();
                user.UserID = userID;
                user.PortalID = portalSettings.PortalId;
                user.TabID = portalSettings.ActiveTab.TabID;
                user.CreationDate = DateTime.Now;
                user.LastActiveDate = DateTime.Now;

                // Add the user
                if (!(userList.Contains(userID)))
                {
                    userList[userID] = user;
                }
            }
            else
            {
                if (cookie.Value == null)
                {
                    // Expire the cookie, there is something wrong with it
                    context.Response.Cookies[cookieName].Expires = new DateTime(1999, 10, 12);

                    // No need to do anything else
                    return;
                }

                // Get userID out of cookie
                userID = cookie.Value;

                // Find the cookie in the user list
                if (userList[userID] == null)
                {
                    userList[userID] = new AnonymousUserInfo();
                    ((AnonymousUserInfo)userList[userID]).CreationDate = DateTime.Now;
                }

                user = (AnonymousUserInfo)userList[userID];
                user.UserID = userID;
                user.PortalID = portalSettings.PortalId;
                user.TabID = portalSettings.ActiveTab.TabID;
                user.LastActiveDate = DateTime.Now;

                // Reset the expiration on the cookie
                cookie = new HttpCookie(cookieName);
                cookie.Value = userID;
                cookie.Expires = DateTime.Now.AddMinutes(20);
                context.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// Tracks an Authenticated User
        /// </summary>
        /// <param name="context">An HttpContext Object</param>
        private void TrackAuthenticatedUser(HttpContext context)
        {
            // Retrieve Portal Settings
            PortalSettings portalSettings = (PortalSettings)context.Items["PortalSettings"];

            if (portalSettings == null)
            {
                return;
            }

            // Get the logged in User ID
            UserInfo objUserInfo = UserController.GetCurrentUserInfo();

            // Get user list
            Hashtable userList = GetUserList();

            OnlineUserInfo user = new OnlineUserInfo();
            if (objUserInfo.UserID >= 0)
            {
                // forms authentication
                user.UserID = objUserInfo.UserID;
            }
            user.PortalID = portalSettings.PortalId;
            user.TabID = portalSettings.ActiveTab.TabID;
            user.LastActiveDate = DateTime.Now;
            if (userList[objUserInfo.UserID.ToString()] == null)
            {
                user.CreationDate = user.LastActiveDate;
            }

            userList[objUserInfo.UserID.ToString()] = user;

            SetUserList(userList);
        }

        /// <summary>
        /// Tracks an online User
        /// </summary>
        public void TrackUsers()
        {
            HttpContext context = HttpContext.Current;

            // Have we already done the work for this request?
            if (!(context.Items["CheckedUsersOnlineCookie"] == null))
            {
                return;
            }
            else
            {
                context.Items["CheckedUsersOnlineCookie"] = "true";
            }

            if (context.Request.IsAuthenticated)
            {
                TrackAuthenticatedUser(context);
            }
            else
            {
                if (context.Request.Browser.Cookies)
                {
                    TrackAnonymousUser(context);
                }
            }
        }

        /// <summary>
        /// Update the Users Online information
        /// </summary>
        public void UpdateUsersOnline()
        {
            // Get a Current User List
            Hashtable userList = GetUserList();

            // Create a shallow copy of the list to Process
            Hashtable listToProcess = (Hashtable)userList.Clone();

            // Clear the list
            ClearUserList();

            // Persist the current User List
            try
            {
                memberProvider.UpdateUsersOnline(listToProcess);
            }
            catch (Exception)
            {
            }

            // Remove users that have expired
            memberProvider.DeleteUsersOnline(GetOnlineTimeWindow());
        }
    }
}