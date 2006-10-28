using System.IO;
using System.Web;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Upgrade;
using DotNetNuke.UI.Skins;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    public class ResourceInstaller
    {

        private void DeleteFile(string strFile)
        {
            // delete the file
            try
            {
                File.SetAttributes(strFile, FileAttributes.Normal);
                File.Delete(strFile);
            }
            catch
            {
                // error removing the file
            }
        }
        public void Install()
        {
            Install(false, 0, "");
        }

        public void Install(bool status, int indent)
        {
            Install(status, indent, "");
        }

        public void Install(bool status, int indent, string type)
        {
            string[] arrFolders;
            string strFolder;
            string[] arrFiles;
            string strFile;

            string InstallPath = Globals.ApplicationMapPath + "\\Install";

            if (Directory.Exists(InstallPath))
            {
                arrFolders = Directory.GetDirectories(InstallPath);
                foreach (string tempLoopVar_strFolder in arrFolders)
                {
                    strFolder = tempLoopVar_strFolder;
                    arrFiles = Directory.GetFiles(strFolder);
                    foreach (string tempLoopVar_strFile in arrFiles)
                    {
                        strFile = tempLoopVar_strFile;

                        switch (type.ToLower())
                        {
                            case "modules":

                                // install custom module
                                InstallModules(strFile, status, indent);
                                break;

                            default:

                                // install custom module
                                InstallModules(strFile, status, indent);

                                // install skin
                                if (strFile.ToLower().IndexOf("\\skin\\") != -1)
                                {
                                    // check if valid skin
                                    if (Path.GetExtension(strFile.ToLower()) == ".zip")
                                    {
                                        if (status)
                                        {
                                            HtmlUtils.WriteFeedback(HttpContext.Current.Response, indent, "Installing Skin File " + strFile + ":<br>");
                                        }
                                        SkinController.UploadSkin(Globals.HostMapPath, SkinInfo.RootSkin, Path.GetFileNameWithoutExtension(strFile), strFile);
                                        // delete file
                                        DeleteFile(strFile);
                                    }
                                }

                                // install container
                                if (strFile.ToLower().IndexOf("\\container\\") != -1)
                                {
                                    // check if valid container
                                    if (Path.GetExtension(strFile.ToLower()) == ".zip")
                                    {
                                        if (status)
                                        {
                                            HtmlUtils.WriteFeedback(HttpContext.Current.Response, indent, "Installing Container File " + strFile + ":<br>");
                                        }
                                        SkinController.UploadSkin(Globals.HostMapPath, SkinInfo.RootContainer, Path.GetFileNameWithoutExtension(strFile), strFile);
                                        // delete file
                                        DeleteFile(strFile);
                                    }
                                }

                                // install language pack
                                if (strFile.ToLower().IndexOf("\\language\\") != -1)
                                {
                                    // check if valid language pack
                                    if (Path.GetExtension(strFile.ToLower()) == ".zip")
                                    {
                                        if (status)
                                        {
                                            HtmlUtils.WriteFeedback(HttpContext.Current.Response, indent, "Installing Language File " + strFile + ":<br>");
                                        }
                                        LocaleFilePackReader objLocaleFilePackReader = new LocaleFilePackReader();
                                        objLocaleFilePackReader.Install(strFile);
                                        // delete file
                                        DeleteFile(strFile);
                                    }
                                }

                                // install template
                                if (strFile.ToLower().IndexOf("\\template\\") != -1)
                                {
                                    // check if valid template file ( .template or .template.resources )
                                    if (strFile.ToLower().IndexOf(".template") != -1)
                                    {
                                        if (status)
                                        {
                                            HtmlUtils.WriteFeedback(HttpContext.Current.Response, indent, "Installing Template " + strFile + ":<br>");
                                        }
                                        string strNewFile = Globals.HostMapPath + "\\" + Path.GetFileName(strFile);
                                        if (File.Exists(strNewFile))
                                        {
                                            File.Delete(strNewFile);
                                        }
                                        File.Move(strFile, strNewFile);
                                    }
                                }

                                //Install Portal(s)
                                if (strFile.ToLower().IndexOf("\\portal\\") != -1)
                                {
                                    //Check if valid portals file
                                    if (strFile.ToLower().IndexOf(".resources") != -1)
                                    {
                                        XmlDocument xmlDoc = new XmlDocument();
                                        XmlNode node;
                                        XmlNodeList nodes;
                                        int intPortalId;
                                        xmlDoc.Load(strFile);

                                        // parse portal(s) if available
                                        nodes = xmlDoc.SelectNodes("//dotnetnuke/portals/portal");
                                        foreach (XmlNode tempLoopVar_node in nodes)
                                        {
                                            node = tempLoopVar_node;
                                            if (node != null)
                                            {
                                                if (status)
                                                {
                                                    HtmlUtils.WriteFeedback(HttpContext.Current.Response, indent, "Installing Portals:<br>");
                                                }
                                                intPortalId = Upgrade.AddPortal(node, true, indent);
                                                if (intPortalId > -1)
                                                {
                                                    HtmlUtils.WriteFeedback(HttpContext.Current.Response, indent + 2, "Successfully Installed Portal " + intPortalId + ":<br>");
                                                }
                                                else
                                                {
                                                    HtmlUtils.WriteFeedback(HttpContext.Current.Response, indent + 2, "Portal failed to install:<br>");
                                                }
                                            }
                                        }
                                        // delete file
                                        DeleteFile(strFile);
                                    }
                                }
                                break;
                        } 
                    } 
                } 
            } 
        }

        private void InstallModules(string strFile, bool status, int indent)
        {
            // install custom module
            if (strFile.ToLower().IndexOf("\\module\\") != -1)
            {
                // check if valid custom module
                if (Path.GetExtension(strFile.ToLower()) == ".zip")
                {
                    if (status)
                    {
                        HtmlUtils.WriteFeedback(HttpContext.Current.Response, indent, "Installing Module File " + strFile + ":<br>");
                    }
                    PaInstaller objPaInstaller = new PaInstaller(strFile, Globals.ApplicationMapPath);
                    objPaInstaller.Install();
                    // delete file
                    DeleteFile(strFile);
                }
            }
        }
    }
}