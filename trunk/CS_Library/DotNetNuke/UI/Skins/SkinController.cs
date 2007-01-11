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
using System.IO;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.UI.Skins
{
    /// <Summary>Handles the Business Control Layer for Skins</Summary>
    public class SkinController
    {
        public static string FormatMessage( string title, string body, int level, bool isError )
        {
            string Message = title;

            if (isError)
            {
                Message = "<font class=\"NormalRed\">" + title + "</font>";
            }

            switch (level)
            {
                case -1:

                    Message = "<hr><br><b>" + Message + "</b>";
                    break;
                case 0:

                    Message = "<br><br><b>" + Message + "</b>";
                    break;
                case 1:

                    Message = "<br><b>" + Message + "</b>";
                    break;
                default:

                    Message = "<br><li>" + Message;
                    break;
            }

            return Message + ": " + body + "\r\n";
        }

        public static string FormatSkinPath( string skinSrc )
        {
            string strSkinSrc = skinSrc;

            if (!String.IsNullOrEmpty(strSkinSrc))
            {
                strSkinSrc = strSkinSrc.Substring(0, Strings.InStrRev(strSkinSrc, "/", -1, 0));
                // AC could not get the following lines to work. no skin is displayed if used.
                //                int lastIndexOf = strSkinSrc.LastIndexOf( "/" );
                //                if(lastIndexOf >= 0)
                //                {
                //                    strSkinSrc = strSkinSrc.Substring(0, lastIndexOf);                    
                //                }
            }

            return strSkinSrc;
        }

        public static string FormatSkinSrc( string skinSrc, PortalSettings portalSettings )
        {
            string strSkinSrc = skinSrc;

            if (!String.IsNullOrEmpty(strSkinSrc))
            {
                switch (strSkinSrc.Substring(0, 3))
                {
                    case "[G]":

                        strSkinSrc = strSkinSrc.Replace("[G]", Globals.HostPath);
                        break;
                    case "[L]":

                        strSkinSrc = strSkinSrc.Replace("[L]", portalSettings.HomeDirectory);
                        break;
                }
            }

            return strSkinSrc;
        }

        public static SkinInfo GetSkin( string skinRoot, int portalId, SkinType skinType )
        {
            return ( (SkinInfo)CBO.FillObject( DataProvider.Instance().GetSkin( skinRoot, portalId, ( (int)skinType ) ), typeof( SkinInfo ) ) );
        }

        public static string UploadSkin( string rootPath, string skinRoot, string skinName, string path )
        {
            FileStream objFileStream;
            objFileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            string strMessage = UploadSkin(rootPath, skinRoot, skinName, objFileStream);

            objFileStream.Close();

            return strMessage;
        }

        public static string UploadSkin( string rootPath, string skinRoot, string skinName, Stream objInputStream )
        {
            ZipInputStream objZipInputStream = new ZipInputStream(objInputStream);

            ZipEntry objZipEntry;
            int intSize = 2049;
            byte[] arrData = new byte[intSize];
            string strMessage = "";
            ArrayList arrSkinFiles = new ArrayList();

            //Localized Strings
            PortalSettings ResourcePortalSettings = Globals.GetPortalSettings();
            string BEGIN_MESSAGE = Localization.GetString("BeginZip", ResourcePortalSettings);
            string CREATE_DIR = Localization.GetString("CreateDir", ResourcePortalSettings);
            string WRITE_FILE = Localization.GetString("WriteFile", ResourcePortalSettings);
            string FILE_ERROR = Localization.GetString("FileError", ResourcePortalSettings);
            string END_MESSAGE = Localization.GetString("EndZip", ResourcePortalSettings);
            string FILE_RESTICTED = Localization.GetString("FileRestricted", ResourcePortalSettings);

            strMessage += FormatMessage(BEGIN_MESSAGE, skinName, -1, false);

            objZipEntry = objZipInputStream.GetNextEntry();
            while (objZipEntry != null)
            {
                if (!objZipEntry.IsDirectory)
                {
                    // validate file extension
                    string strExtension = objZipEntry.Name.Substring(objZipEntry.Name.LastIndexOf(".") + 1);
                    if (Strings.InStr(1, ",ASCX,HTM,HTML,CSS,SWF,RESX," + Globals.HostSettings["FileExtensions"].ToString().ToUpper(), "," + strExtension.ToUpper(), 0) != 0)
                    {
                        // process embedded zip files
                        if (objZipEntry.Name.ToLower() == SkinInfo.RootSkin.ToLower() + ".zip")
                        {
                            MemoryStream objMemoryStream = new MemoryStream();
                            intSize = objZipInputStream.Read(arrData, 0, arrData.Length);
                            while (intSize > 0)
                            {
                                objMemoryStream.Write(arrData, 0, intSize);
                                intSize = objZipInputStream.Read(arrData, 0, arrData.Length);
                            }
                            objMemoryStream.Seek(0, SeekOrigin.Begin);
                            strMessage += UploadSkin(rootPath, SkinInfo.RootSkin, skinName, objMemoryStream);
                        }
                        else if (objZipEntry.Name.ToLower() == SkinInfo.RootContainer.ToLower() + ".zip")
                        {
                            MemoryStream objMemoryStream = new MemoryStream();
                            intSize = objZipInputStream.Read(arrData, 0, arrData.Length);
                            while (intSize > 0)
                            {
                                objMemoryStream.Write(arrData, 0, intSize);
                                intSize = objZipInputStream.Read(arrData, 0, arrData.Length);
                            }
                            objMemoryStream.Seek(0, SeekOrigin.Begin);
                            strMessage += UploadSkin(rootPath, SkinInfo.RootContainer, skinName, objMemoryStream);
                        }
                        else
                        {
                            string strFileName = rootPath + skinRoot + "\\" + skinName + "\\" + objZipEntry.Name;

                            // create the directory if it does not exist
                            if (!Directory.Exists(Path.GetDirectoryName(strFileName)))
                            {
                                strMessage += FormatMessage(CREATE_DIR, Path.GetDirectoryName(strFileName), 2, false);
                                Directory.CreateDirectory(Path.GetDirectoryName(strFileName));
                            }

                            // remove the old file
                            if (File.Exists(strFileName))
                            {
                                File.SetAttributes(strFileName, FileAttributes.Normal);
                                File.Delete(strFileName);
                            }
                            // create the new file
                            FileStream objFileStream = File.Create(strFileName);

                            // unzip the file
                            strMessage += FormatMessage(WRITE_FILE, Path.GetFileName(strFileName), 2, false);
                            intSize = objZipInputStream.Read(arrData, 0, arrData.Length);
                            while (intSize > 0)
                            {
                                objFileStream.Write(arrData, 0, intSize);
                                intSize = objZipInputStream.Read(arrData, 0, arrData.Length);
                            }
                            objFileStream.Close();

                            // save the skin file
                            switch (Path.GetExtension(strFileName))
                            {
                                case ".htm":
                                    if (strFileName.ToLower().IndexOf(Globals.glbAboutPage.ToLower()) < 0)
                                    {
                                        arrSkinFiles.Add(strFileName);
                                    }
                                    break;

                                case ".html":
                                    if (strFileName.ToLower().IndexOf(Globals.glbAboutPage.ToLower()) < 0)
                                    {
                                        arrSkinFiles.Add(strFileName);
                                    }
                                    break;

                                case ".ascx":
                                    if (strFileName.ToLower().IndexOf(Globals.glbAboutPage.ToLower()) < 0)
                                    {
                                        arrSkinFiles.Add(strFileName);
                                    }
                                    break;

                                case ".css":

                                    if (strFileName.ToLower().IndexOf(Globals.glbAboutPage.ToLower()) < 0)
                                    {
                                        arrSkinFiles.Add(strFileName);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        strMessage += FormatMessage(FILE_ERROR, string.Format(FILE_RESTICTED, objZipEntry.Name, Strings.Replace(Globals.HostSettings["FileExtensions"].ToString(), ",", ", *.", 1, -1, 0)), 2, true);
                    }
                }
                objZipEntry = objZipInputStream.GetNextEntry();
            }
            strMessage += FormatMessage(END_MESSAGE, skinName + ".zip", 1, false);
            objZipInputStream.Close();

            // process the list of skin files
            SkinFileProcessor NewSkin = new SkinFileProcessor(rootPath, skinRoot, skinName);
            strMessage += NewSkin.ProcessList(arrSkinFiles, SkinParser.Portable);

            // log installation event
            try
            {                
                LogInfo objEventLogInfo = new LogInfo();
                objEventLogInfo.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString();
                objEventLogInfo.LogProperties.Add(new LogDetailInfo("Install Skin:", skinName));
                Array arrMessage = strMessage.Split("<br>".ToCharArray()[0]);
                foreach (string tempLoopVar_strRow in arrMessage)
                {
                    string strRow = tempLoopVar_strRow;
                    objEventLogInfo.LogProperties.Add(new LogDetailInfo("Info:", HtmlUtils.StripTags(strRow, true)));
                }
                EventLogController objEventLog = new EventLogController();
                objEventLog.AddLog(objEventLogInfo);
            }
            catch (Exception)
            {
                // error
            }

            return strMessage;
        }

        public static void SetSkin( string skinRoot, int portalId, SkinType skinType, string skinSrc )
        {
            // remove skin assignment
            DataProvider.Instance().DeleteSkin(skinRoot, portalId, (int)skinType);

            // add new skin assignment if specified
            if (!String.IsNullOrEmpty(skinSrc))
            {
                DataProvider.Instance().AddSkin(skinRoot, portalId, (int)skinType, skinSrc);
            }
        }
    }
}