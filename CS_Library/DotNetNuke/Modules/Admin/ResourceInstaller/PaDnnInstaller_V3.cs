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
using System.IO;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Services.EventQueue;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    /// <summary>
    /// The PaDnnInstaller_V3 extends PaDnnInstallerBase to support V3 Modules
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class PaDnnInstaller_V3 : PaDnnInstallerBase
    {
        public PaDnnInstaller_V3(PaInstallInfo InstallerInfo)
            : base(InstallerInfo)
        {
        }

        protected bool DeleteFiles(string FolderName, string Version)
        {
            bool WasSuccessful = true;

            string strListFile = Path.Combine(Path.Combine(InstallerInfo.SitePath, Path.Combine("DesktopModules", FolderName)), Version + ".txt");

            if (File.Exists(strListFile))
            {
                // read list file
                StreamReader objStreamReader;
                objStreamReader = File.OpenText(strListFile);
                Array arrPaths = objStreamReader.ReadToEnd().Split(ControlChars.CrLf.ToCharArray());
                objStreamReader.Close();

                // loop through path list
                string strPath;

                foreach (string tempLoopVar_strPath in arrPaths)
                {
                    strPath = tempLoopVar_strPath;
                    if (strPath.Trim() != "")
                    {
                        strPath = HttpContext.Current.Server.MapPath("..\\" + strPath);
                        if (strPath.EndsWith("\\"))
                        {
                            // folder
                            if (Directory.Exists(strPath))
                            {
                                try // delete the folder
                                {
                                    Globals.DeleteFolderRecursive(strPath);
                                }
                                catch (Exception)
                                {
                                    // supress
                                }
                            }
                        }
                        else
                        {
                            // file
                            if (File.Exists(strPath))
                            {
                                try // delete the file
                                {
                                    File.SetAttributes(strPath, FileAttributes.Normal);
                                    File.Delete(strPath);
                                }
                                catch (Exception)
                                {
                                    // suppress
                                }
                            }
                        }
                    }
                }
            }

            return WasSuccessful;
        }

        protected override DesktopModuleInfo GetDesktopModule(PaFolder Folder)
        {
            DesktopModuleController objDesktopModules = new DesktopModuleController();
            DesktopModuleInfo objDesktopModule = objDesktopModules.GetDesktopModuleByModuleName(Folder.ModuleName);

            return objDesktopModule;
        }

        protected override DesktopModuleInfo GetDesktopModuleSettings(DesktopModuleInfo objDesktopModule, PaFolder Folder)
        {
            // call the V2 implementation to load the values
            objDesktopModule = base.GetDesktopModuleSettings(objDesktopModule, Folder);

            // V3 .dnn file format adds the optional businesscontrollerclass node to the folder node element
            objDesktopModule.BusinessControllerClass = Folder.BusinessControllerClass;

            //V3.1 adds the IsSearchable/IsPortable properties - set them to false
            objDesktopModule.IsSearchable = false;
            objDesktopModule.IsPortable = false;

            //Create an instance of the business controller and determine the values of IsSearchable and
            //IsPortable by Reflection
            try
            {
                if (!String.IsNullOrEmpty(objDesktopModule.BusinessControllerClass))
                {
                    object objController = Reflection.CreateObject(objDesktopModule.BusinessControllerClass, objDesktopModule.BusinessControllerClass);
                    if (objController is ISearchable)
                    {
                        objDesktopModule.IsSearchable = true;
                    }
                    if (objController is IPortable)
                    {
                        objDesktopModule.IsPortable = true;
                    }
                }
            }
            catch
            {
                //this code may not work because the module may have just been upgraded and did not have
                //the BusinessControllerClass in the version that is currently in the Application Domain
                //if this is the case, then the updating of thos features will be handled after the application is restarted
            }

            return objDesktopModule;
        }

        protected override string UpgradeModule(DesktopModuleInfo ModuleInfo)
        {
            if (!String.IsNullOrEmpty(ModuleInfo.BusinessControllerClass))
            {
                string UpgradeVersionsList = "";

                if (UpgradeVersions.Count > 0)
                {
                    foreach (string Version in UpgradeVersions)
                    {
                        UpgradeVersionsList = UpgradeVersionsList + Version + ",";
                        DeleteFiles(ModuleInfo.FolderName, Version);
                    }
                    if (UpgradeVersionsList.EndsWith(","))
                    {
                        UpgradeVersionsList = UpgradeVersionsList.Remove(UpgradeVersionsList.Length - 1, 1);
                    }
                }
                else
                {
                    UpgradeVersionsList = ModuleInfo.Version;
                }

                //this cannot be done directly at this time because
                //the module may not be loaded into the app domain yet
                //So send an EventMessage that will process the update
                //after the App recycles
                EventMessage oAppStartMessage = new EventMessage();
                oAppStartMessage.ProcessorType = "DotNetNuke.Entities.Modules.EventMessageProcessor, DotNetNuke";
                oAppStartMessage.Attributes.Add("ProcessCommand", "UpgradeModule");
                oAppStartMessage.Attributes.Add("BusinessControllerClass", ModuleInfo.BusinessControllerClass);
                oAppStartMessage.Attributes.Add("DesktopModuleId", ModuleInfo.DesktopModuleID.ToString());
                oAppStartMessage.Attributes.Add("UpgradeVersionsList", UpgradeVersionsList);
                oAppStartMessage.Priority = MessagePriority.High;
                oAppStartMessage.SentDate = DateTime.Now;
                //make it expire as soon as it's processed
                oAppStartMessage.ExpirationDate = DateTime.Now.AddYears(-1);
                //send it
                EventQueueController oEventQueueController = new EventQueueController();
                oEventQueueController.SendMessage(oAppStartMessage, "Application_Start");

                //force an app restart
                Config.Touch();
            }
            //TODO: Need to implement a feedback loop to display the results of the upgrade.

            return "";
        }
    }
}