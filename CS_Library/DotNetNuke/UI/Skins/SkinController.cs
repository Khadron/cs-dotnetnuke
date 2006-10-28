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
        public static string FormatMessage( string Title, string Body, int Level, bool IsError )
        {
            string Message = Title;

            if (IsError)
            {
                Message = "<font class=\"NormalRed\">" + Title + "</font>";
            }

            switch (Level)
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

            return Message + ": " + Body + "\r\n";
        }

        public static string FormatSkinPath( string SkinSrc )
        {
            string strSkinSrc = SkinSrc;

            if (strSkinSrc != "")
            {
                strSkinSrc = strSkinSrc.Substring(0, Strings.InStrRev(strSkinSrc, "/", -1, 0));
            }

            return strSkinSrc;
        }

        public static string FormatSkinSrc( string SkinSrc, PortalSettings PortalSettings )
        {
            string strSkinSrc = SkinSrc;

            if (strSkinSrc != "")
            {
                switch (strSkinSrc.Substring(0, 3))
                {
                    case "[G]":

                        strSkinSrc = strSkinSrc.Replace("[G]", Globals.HostPath);
                        break;
                    case "[L]":

                        strSkinSrc = strSkinSrc.Replace("[L]", PortalSettings.HomeDirectory);
                        break;
                }
            }

            return strSkinSrc;
        }

        public static SkinInfo GetSkin( string SkinRoot, int PortalId, SkinType SkinType )
        {
            return ( (SkinInfo)CBO.FillObject( DataProvider.Instance().GetSkin( SkinRoot, PortalId, ( (int)SkinType ) ), typeof( SkinInfo ) ) );
        }

        public static string UploadSkin( string RootPath, string SkinRoot, string SkinName, string Path )
        {
            string strMessage = "";

            FileStream objFileStream;
            objFileStream = new FileStream(Path, FileMode.Open, FileAccess.Read);

            strMessage = UploadSkin(RootPath, SkinRoot, SkinName, ((Stream)objFileStream));

            objFileStream.Close();

            return strMessage;
        }

        public static string UploadSkin( string RootPath, string SkinRoot, string SkinName, Stream objInputStream )
        {
            ZipInputStream objZipInputStream = new ZipInputStream(objInputStream);

            ZipEntry objZipEntry;
            string strExtension;
            string strFileName;
            FileStream objFileStream;
            int intSize = 2048;
            byte[] arrData = new byte[2049];
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

            strMessage += FormatMessage(BEGIN_MESSAGE, SkinName, -1, false);

            objZipEntry = objZipInputStream.GetNextEntry();
            while (objZipEntry != null)
            {
                if (!objZipEntry.IsDirectory)
                {
                    // validate file extension
                    strExtension = objZipEntry.Name.Substring(objZipEntry.Name.LastIndexOf(".") + 1);
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
                            strMessage += UploadSkin(RootPath, SkinInfo.RootSkin, SkinName, ((Stream)objMemoryStream));
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
                            strMessage += UploadSkin(RootPath, SkinInfo.RootContainer, SkinName, ((Stream)objMemoryStream));
                        }
                        else
                        {
                            strFileName = RootPath + SkinRoot + "\\" + SkinName + "\\" + objZipEntry.Name;

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
                            objFileStream = File.Create(strFileName);

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
            strMessage += FormatMessage(END_MESSAGE, SkinName + ".zip", 1, false);
            objZipInputStream.Close();

            // process the list of skin files
            SkinFileProcessor NewSkin = new SkinFileProcessor(RootPath, SkinRoot, SkinName);
            strMessage += NewSkin.ProcessList(arrSkinFiles, SkinParser.Portable);

            // log installation event
            try
            {
                LogInfo objEventLogInfo = new LogInfo();
                objEventLogInfo.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString();
                objEventLogInfo.LogProperties.Add(new LogDetailInfo("Install Skin:", SkinName));
                Array arrMessage = strMessage.Split("<br>".ToCharArray()[0]);
                string strRow;
                foreach (string tempLoopVar_strRow in arrMessage)
                {
                    strRow = tempLoopVar_strRow;
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

        public static void SetSkin( string SkinRoot, int PortalId, SkinType SkinType, string SkinSrc )
        {
            // remove skin assignment
            DataProvider.Instance().DeleteSkin(SkinRoot, PortalId, (int)SkinType);

            // add new skin assignment if specified
            if (SkinSrc != "")
            {
                DataProvider.Instance().AddSkin(SkinRoot, PortalId, (int)SkinType, SkinSrc);
            }
        }
    }
}