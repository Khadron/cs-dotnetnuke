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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;

namespace DotNetNuke.Services.Log.EventLog
{
    public class EventLogController : LogController
    {
        public enum EventLogType
        {
            USER_CREATED,
            USER_DELETED,
            LOGIN_SUPERUSER,
            LOGIN_SUCCESS,
            LOGIN_FAILURE,
            LOGIN_USERLOCKEDOUT,
            LOGIN_USERNOTAPPROVED,
            CACHE_REFRESHED,
            PASSWORD_SENT_SUCCESS,
            PASSWORD_SENT_FAILURE,
            LOG_NOTIFICATION_FAILURE,
            PORTAL_CREATED,
            PORTAL_DELETED,
            TAB_CREATED,
            TAB_UPDATED,
            TAB_DELETED,
            TAB_SENT_TO_RECYCLE_BIN,
            TAB_RESTORED,
            USER_ROLE_CREATED,
            USER_ROLE_DELETED,
            ROLE_CREATED,
            ROLE_UPDATED,
            ROLE_DELETED,
            MODULE_CREATED,
            MODULE_UPDATED,
            MODULE_DELETED,
            MODULE_SENT_TO_RECYCLE_BIN,
            MODULE_RESTORED,
            SCHEDULER_EVENT_STARTED,
            SCHEDULER_EVENT_PROGRESSING,
            SCHEDULER_EVENT_COMPLETED,
            APPLICATION_START,
            APPLICATION_END,
            APPLICATION_SHUTTING_DOWN,
            SCHEDULER_STARTED,
            SCHEDULER_SHUTTING_DOWN,
            SCHEDULER_STOPPED,
            ADMIN_ALERT,
            HOST_ALERT
        }

        public void AddLog(LogInfo objEventLogInfo)
        {
            LogController objLogController = new LogController();
            objLogController.AddLog(objEventLogInfo);
        }

        public void AddLog(object objCBO, PortalSettings _PortalSettings, int UserID, string UserName, EventLogType objLogType)
        {
            LogController objLogController = new LogController();
            LogInfo objLogInfo = new LogInfo();
            objLogInfo.LogUserID = UserID;
            objLogInfo.LogPortalID = _PortalSettings.PortalId;
            objLogInfo.LogTypeKey = objLogType.ToString();
            objLogInfo.LogPortalName = _PortalSettings.PortalName;
            switch (objCBO.GetType().FullName)
            {
                case "DotNetNuke.Entities.Portals.PortalInfo":

                    PortalInfo objPortal = (PortalInfo)objCBO;
                    objLogInfo.LogProperties.Add(new LogDetailInfo("PortalID", objPortal.PortalID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("PortalName", objPortal.PortalName));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("Description", objPortal.Description));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("KeyWords", objPortal.KeyWords));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("LogoFile", objPortal.LogoFile));
                    break;
                case "DotNetNuke.Entities.Tabs.TabInfo":

                    TabInfo objTab = (TabInfo)objCBO;
                    objLogInfo.LogProperties.Add(new LogDetailInfo("TabID", objTab.TabID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("PortalID", objTab.PortalID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("TabName", objTab.TabName));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("Title", objTab.Title));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("Description", objTab.Description));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("KeyWords", objTab.KeyWords));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("Url", objTab.Url));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("ParentId", objTab.ParentId.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("IconFile", objTab.IconFile));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("IsVisible", objTab.IsVisible.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("SkinSrc", objTab.SkinSrc));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("ContainerSrc", objTab.ContainerSrc));
                    break;
                case "DotNetNuke.Entities.Modules.ModuleInfo":

                    ModuleInfo objModule = (ModuleInfo)objCBO;
                    objLogInfo.LogProperties.Add(new LogDetailInfo("ModuleId", objModule.ModuleID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("ModuleTitle", objModule.ModuleTitle));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("TabModuleID", objModule.TabModuleID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("TabID", objModule.TabID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("PortalID", objModule.PortalID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("ModuleDefId", objModule.ModuleDefID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("FriendlyName", objModule.FriendlyName));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("IconFile", objModule.IconFile));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("Visibility", objModule.Visibility.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("ContainerSrc", objModule.ContainerSrc));
                    break;
                case "DotNetNuke.Entities.Users.UserInfo":

                    UserInfo objUser = (UserInfo)objCBO;
                    objLogInfo.LogProperties.Add(new LogDetailInfo("UserID", objUser.UserID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("FirstName", objUser.Profile.FirstName));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("LastName", objUser.Profile.LastName));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("UserName", objUser.Username));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("Email", objUser.Email));
                    break;
                case "DotNetNuke.Security.Roles.RoleInfo":

                    RoleInfo objRole = (RoleInfo)objCBO;
                    objLogInfo.LogProperties.Add(new LogDetailInfo("RoleID", objRole.RoleID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("RoleName", objRole.RoleName));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("PortalID", objRole.PortalID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("Description", objRole.Description));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("IsPublic", objRole.IsPublic.ToString()));
                    break;
                default: //Serialise using XmlSerializer

                    objLogInfo.LogProperties.Add(new LogDetailInfo("logdetail", XmlUtils.Serialize(objCBO)));
                    break;
            }
            objLogController.AddLog(objLogInfo);
        }

        public void AddLog(PortalSettings _PortalSettings, int UserID, EventLogType objLogType)
        {
            //Used for DotNetNuke native  log types
            LogProperties objProperties = new LogProperties();
            AddLog(objProperties, _PortalSettings, UserID, objLogType.ToString(), false);
        }

        public void AddLog(string PropertyName, string PropertyValue, PortalSettings _PortalSettings, int UserID, EventLogType objLogType)
        {
            //Used for DotNetNuke native  log types
            LogProperties objProperties = new LogProperties();
            LogDetailInfo objLogDetailInfo = new LogDetailInfo();
            objLogDetailInfo.PropertyName = PropertyName;
            objLogDetailInfo.PropertyValue = PropertyValue;
            objProperties.Add(objLogDetailInfo);
            AddLog(objProperties, _PortalSettings, UserID, objLogType.ToString(), false);
        }

        public void AddLog(string PropertyName, string PropertyValue, PortalSettings _PortalSettings, int UserID, string LogType)
        {
            //Used for custom/on-the-fly  log types
            LogProperties objProperties = new LogProperties();
            LogDetailInfo objLogDetailInfo = new LogDetailInfo();
            objLogDetailInfo.PropertyName = PropertyName;
            objLogDetailInfo.PropertyValue = PropertyValue;
            objProperties.Add(objLogDetailInfo);
            AddLog(objProperties, _PortalSettings, UserID, LogType, false);
        }

        public void AddLog(LogProperties objProperties, PortalSettings _PortalSettings, int UserID, string LogTypeKey, bool BypassBuffering)
        {
            //Used for custom/on-the-fly  log types

            LogController objLogController = new LogController();
            LogInfo objLogInfo = new LogInfo();
            objLogInfo.LogUserID = UserID;
            objLogInfo.LogPortalID = _PortalSettings.PortalId;
            objLogInfo.LogTypeKey = LogTypeKey;
            objLogInfo.LogProperties = objProperties;
            objLogInfo.LogPortalName = _PortalSettings.PortalName;
            objLogInfo.BypassBuffering = BypassBuffering;
            objLogController.AddLog(objLogInfo);
        }
    }
}