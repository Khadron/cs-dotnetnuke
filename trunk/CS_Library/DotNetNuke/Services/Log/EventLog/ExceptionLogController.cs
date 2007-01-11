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
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Services.Log.EventLog
{
    public class ExceptionLogController : LogController
    {
        public enum ExceptionLogType
        {
            GENERAL_EXCEPTION,
            MODULE_LOAD_EXCEPTION,
            PAGE_LOAD_EXCEPTION,
            SCHEDULER_EXCEPTION,
            SECURITY_EXCEPTION
        }

        public void AddLog(Exception objException)
        {
            AddLog(objException, ExceptionLogType.GENERAL_EXCEPTION);
        }

        public void AddLog(BasePortalException objBasePortalException)
        {
            if (objBasePortalException.GetType().Name == "ModuleLoadException")
            {
                AddLog(objBasePortalException, ExceptionLogType.MODULE_LOAD_EXCEPTION);
            }
            else if (objBasePortalException.GetType().Name == "PageLoadException")
            {
                AddLog(objBasePortalException, ExceptionLogType.PAGE_LOAD_EXCEPTION);
            }
            else if (objBasePortalException.GetType().Name == "SchedulerException")
            {
                AddLog(objBasePortalException, ExceptionLogType.SCHEDULER_EXCEPTION);
            }
            else if (objBasePortalException.GetType().Name == "SecurityException")
            {
                AddLog(objBasePortalException, ExceptionLogType.SECURITY_EXCEPTION);
            }
            else
            {
                AddLog(objBasePortalException, ExceptionLogType.GENERAL_EXCEPTION);
            }
        }

        public void AddLog(Exception objException, ExceptionLogType LogType)
        {
            LogController objLogController = new LogController();
            LogInfo objLogInfo = new LogInfo();
            objLogInfo.LogTypeKey = LogType.ToString();
            if (LogType == ExceptionLogType.MODULE_LOAD_EXCEPTION)
            {
                //Add ModuleLoadException Properties
                ModuleLoadException objModuleLoadException = (ModuleLoadException)objException;
                objLogInfo.LogProperties.Add(new LogDetailInfo("ModuleId", objModuleLoadException.ModuleId.ToString()));
                objLogInfo.LogProperties.Add(new LogDetailInfo("ModuleDefId", objModuleLoadException.ModuleDefId.ToString()));
                objLogInfo.LogProperties.Add(new LogDetailInfo("FriendlyName", objModuleLoadException.FriendlyName));
                objLogInfo.LogProperties.Add(new LogDetailInfo("ModuleControlSource", objModuleLoadException.ModuleControlSource));
            }

            if (LogType == ExceptionLogType.SECURITY_EXCEPTION)
            {
                //Add ModuleLoadException Properties
                ModuleLoadException objModuleLoadException = (ModuleLoadException)objException;
                objLogInfo.LogProperties.Add(new LogDetailInfo("ModuleId", objModuleLoadException.ModuleId.ToString()));
                objLogInfo.LogProperties.Add(new LogDetailInfo("ModuleDefId", objModuleLoadException.ModuleDefId.ToString()));
                objLogInfo.LogProperties.Add(new LogDetailInfo("FriendlyName", objModuleLoadException.FriendlyName));
                objLogInfo.LogProperties.Add(new LogDetailInfo("ModuleControlSource", objModuleLoadException.ModuleControlSource));
            }

            //Add BasePortalException Properties
            BasePortalException objBasePortalException = new BasePortalException(objException.ToString(), objException);
            objLogInfo.LogProperties.Add(new LogDetailInfo("AssemblyVersion", objBasePortalException.AssemblyVersion));
            objLogInfo.LogProperties.Add(new LogDetailInfo("PortalID", objBasePortalException.PortalID.ToString()));
            objLogInfo.LogProperties.Add(new LogDetailInfo("PortalName", objBasePortalException.PortalName));
            objLogInfo.LogProperties.Add(new LogDetailInfo("UserID", objBasePortalException.UserID.ToString()));
            objLogInfo.LogProperties.Add(new LogDetailInfo("UserName", objBasePortalException.UserName));
            objLogInfo.LogProperties.Add(new LogDetailInfo("ActiveTabID", objBasePortalException.ActiveTabID.ToString()));
            objLogInfo.LogProperties.Add(new LogDetailInfo("ActiveTabName", objBasePortalException.ActiveTabName));
            objLogInfo.LogProperties.Add(new LogDetailInfo("RawURL", objBasePortalException.RawURL));
            objLogInfo.LogProperties.Add(new LogDetailInfo("AbsoluteURL", objBasePortalException.AbsoluteURL));
            objLogInfo.LogProperties.Add(new LogDetailInfo("AbsoluteURLReferrer", objBasePortalException.AbsoluteURLReferrer));
            objLogInfo.LogProperties.Add(new LogDetailInfo("UserAgent", objBasePortalException.UserAgent));
            objLogInfo.LogProperties.Add(new LogDetailInfo("DefaultDataProvider", objBasePortalException.DefaultDataProvider));
            objLogInfo.LogProperties.Add(new LogDetailInfo("ExceptionGUID", objBasePortalException.ExceptionGUID));
            objLogInfo.LogProperties.Add(new LogDetailInfo("InnerException", objBasePortalException.InnerException.Message));
            objLogInfo.LogProperties.Add(new LogDetailInfo("FileName", objBasePortalException.FileName));
            objLogInfo.LogProperties.Add(new LogDetailInfo("FileLineNumber", objBasePortalException.FileLineNumber.ToString()));
            objLogInfo.LogProperties.Add(new LogDetailInfo("FileColumnNumber", objBasePortalException.FileColumnNumber.ToString()));
            objLogInfo.LogProperties.Add(new LogDetailInfo("Method", objBasePortalException.Method));
            objLogInfo.LogProperties.Add(new LogDetailInfo("StackTrace", objBasePortalException.StackTrace));
            objLogInfo.LogProperties.Add(new LogDetailInfo("Message", objBasePortalException.Message));
            objLogInfo.LogProperties.Add(new LogDetailInfo("Source", objBasePortalException.Source));

            objLogInfo.LogPortalID = objBasePortalException.PortalID;

            objLogController.AddLog(objLogInfo);
        }
    }
}