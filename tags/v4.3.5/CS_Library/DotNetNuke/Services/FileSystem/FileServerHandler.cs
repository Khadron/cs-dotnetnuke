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
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Services.FileSystem
{
    public class FileServerHandler : IHttpHandler
    {
        public virtual bool IsReusable
        {
            get
            {
                return true;
            }
        }

        /// <Summary>
        /// This handler handles requests for LinkClick.aspx, but only those specifc
        /// to file serving
        /// </Summary>
        /// <Param name="context">System.Web.HttpContext)</Param>
        public virtual void ProcessRequest( HttpContext context )
        {
            PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();

            // get TabId
            int tabId = -1;
            if (context.Request.QueryString["tabid"] != null)
            {
                tabId = int.Parse(context.Request.QueryString["tabid"]);
            }

            // get ModuleId
            int moduleId = -1;
            if (context.Request.QueryString["mid"] != null)
            {
                moduleId = int.Parse(context.Request.QueryString["mid"]);
            }

            // get UserId
            int UserId = -1;
            UserInfo objUserInfo = UserController.GetCurrentUserInfo();
            if (context.Request.IsAuthenticated)
            {
                UserId = objUserInfo.UserID;
            }

            // get the URL
            string URL = "";
            bool blnClientCache = true;
            bool blnForceDownload = false;

            if (context.Request.QueryString["fileticket"] != null)
            {
                URL = "FileID=" + UrlUtils.DecryptParameter(context.Request.QueryString["fileticket"]);
            }
            if (context.Request.QueryString["userticket"] != null)
            {
                URL = "UserId=" + UrlUtils.DecryptParameter(context.Request.QueryString["userticket"]);
            }
            if (context.Request.QueryString["link"] != null)
            {
                URL = context.Request.QueryString["link"];
                if (URL.ToLower().StartsWith("fileid="))
                {
                    URL = ""; // restrict direct access by FileID
                }
            }

            if (!String.IsNullOrEmpty(URL))
            {
                TabType UrlType = Globals.GetURLType(URL);

                if (UrlType != TabType.File)
                {
                    URL = Globals.LinkClick(URL, tabId, moduleId, false);
                }

                if (UrlType == TabType.File && URL.ToLower().StartsWith("fileid=") == false)
                {
                    // to handle legacy scenarios before the introduction of the FileServerHandler
                    FileController objFiles = new FileController();
                    URL = "FileID=" + objFiles.ConvertFilePathToFileId(URL, portalSettings.PortalId);
                }

                // get optional parameters
                if (context.Request.QueryString["clientcache"] != null)
                {
                    blnClientCache = bool.Parse(context.Request.QueryString["clientcache"]);
                }

                if ((context.Request.QueryString["forcedownload"] != null) || (context.Request.QueryString["contenttype"] != null))
                {
                    blnForceDownload = bool.Parse(context.Request.QueryString["forcedownload"]);
                }

                // update clicks
                UrlController objUrls = new UrlController();
                objUrls.UpdateUrlTracking(portalSettings.PortalId, URL, moduleId, UserId);

                // clear the current response
                context.Response.Clear();

                if (UrlType == TabType.File)
                {
                    // serve the file
                    if (!FileSystemUtils.DownloadFile(portalSettings, int.Parse(UrlUtils.GetParameterValue(URL)), blnClientCache, blnForceDownload))
                    {
                        context.Response.Write(Localization.Localization.GetString("FilePermission.Error"));
                    }
                }
                else
                {
                    // redirect to URL
                    context.Response.Redirect(URL, true);
                }
            }
        }
    }
}