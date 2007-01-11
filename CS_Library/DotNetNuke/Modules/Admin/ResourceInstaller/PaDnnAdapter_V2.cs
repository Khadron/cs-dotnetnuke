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
using System.ComponentModel;
using System.IO;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Security;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    /// <summary>
    /// The PaDnnAdapter_V2 extends PaDnnAdapterBase to support V2 Modules
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class PaDnnAdapter_V2 : PaDnnAdapterBase
    {
        public PaDnnAdapter_V2(PaInstallInfo InstallerInfo)
            : base(InstallerInfo)
        {
        }

        protected virtual PaFolder GetFolderAttributesFromNode(XmlElement FolderElement)
        {
            PaFolder Folder = new PaFolder();

            //The folder/name node is a required node.  If empty, this will throw an exception.
            try
            {
                Folder.Name = FolderElement.SelectSingleNode("name").InnerText.Trim();
            }
            catch (Exception)
            {
                throw (new Exception(EXCEPTION_FolderName));
            }

            XmlElement descriptionElement = (XmlElement)FolderElement.SelectSingleNode("description");
            if (descriptionElement != null)
            {
                Folder.Description = descriptionElement.InnerText.Trim();
            }

            //The folder/version node is a required node.  If empty, this will throw an exception.
            try
            {
                Folder.Version = FolderElement.SelectSingleNode("version").InnerText.Trim();
            }
            catch (Exception)
            {
                throw (new Exception(EXCEPTION_FolderVersion));
            }

            // in V2 the FriendlyName/FolderName/ModuleName attributes were not supported in the *.dnn file,
            // so set them to the Folder Name
            Folder.FriendlyName = Folder.Name;
            Folder.FolderName = Folder.Name;
            Folder.ModuleName = Folder.Name;

            return Folder;
        }

        protected virtual PaFolder GetFolderFromNode(ref int TempModuleDefinitionID, XmlElement FolderElement)
        {
            PaFolder Folder = GetFolderAttributesFromNode(FolderElement);

            XmlElement resourcefileElement = (XmlElement)FolderElement.SelectSingleNode("resourcefile");
            if (resourcefileElement != null)
            {
                if (resourcefileElement.InnerText.Trim() != "")
                {
                    Folder.ResourceFile = resourcefileElement.InnerText.Trim();
                    PaFile paRF = (PaFile)InstallerInfo.FileTable[Folder.ResourceFile.ToLower()];
                    if (paRF == null)
                    {
                        InstallerInfo.Log.AddFailure(EXCEPTION_MissingResource + Folder.ResourceFile);
                    }
                    else
                    {
                        //need to add this file to the list of files to be installed
                        Folder.Files.Add(paRF);
                    }
                }
            }

            // loading files
            InstallerInfo.Log.AddInfo(FILES_Loading);
            XmlElement file;
            foreach (XmlElement tempLoopVar_file in FolderElement.SelectNodes("files/file"))
            {
                file = tempLoopVar_file;
                //The file/name node is a required node.  If empty, this will throw
                //an exception.
                string name;
                try
                {
                    name = file.SelectSingleNode("name").InnerText.Trim();
                }
                catch (Exception)
                {
                    throw (new Exception(EXCEPTION_FolderName));
                }
                PaFile paf = (PaFile)InstallerInfo.FileTable[name.ToLower()];
                if (paf == null)
                {
                    InstallerInfo.Log.AddFailure(FILE_NotFound + name);
                }
                else
                {
                    //A file path may or may not exist
                    XmlElement pathElement = (XmlElement)file.SelectSingleNode("path");
                    if (!(pathElement == null))
                    {
                        string filepath = pathElement.InnerText.Trim();
                        InstallerInfo.Log.AddInfo(string.Format(FILE_Found, filepath, name));
                        paf.Path = filepath;
                    }
                    Folder.Files.Add(paf);
                }
            }

            // add dnn file to collection ( if it does not exist already )
            if (Folder.Files.Contains(InstallerInfo.DnnFile) == false)
            {
                Folder.Files.Add(InstallerInfo.DnnFile);
            }

            InstallerInfo.Log.AddInfo(MODULES_Loading);
            XmlElement DNNModule;
            foreach (XmlElement tempLoopVar_DNNModule in FolderElement.SelectNodes("modules/module"))
            {
                DNNModule = tempLoopVar_DNNModule;

                Folder.Modules.Add(GetModuleFromNode(TempModuleDefinitionID, Folder, DNNModule));

                TempModuleDefinitionID++;
            }

            return Folder;
        }

        protected virtual ModuleControlInfo GetModuleControlFromNode(string Foldername, int TempModuleID, XmlElement ModuleControl)
        {
            ModuleControlInfo ModuleControlDef = new ModuleControlInfo();

            XmlElement keyElement = (XmlElement)ModuleControl.SelectSingleNode("key");
            if (keyElement != null)
            {
                ModuleControlDef.ControlKey = keyElement.InnerText.Trim();
            }

            XmlElement titleElement = (XmlElement)ModuleControl.SelectSingleNode("title");
            if (titleElement != null)
            {
                ModuleControlDef.ControlTitle = titleElement.InnerText.Trim();
            }

            try
            {
                string ControlSrc = ModuleControl.SelectSingleNode("src").InnerText.Trim();
                if (ControlSrc.ToLower().StartsWith("desktopmodules"))
                {
                    // this code allows a developer to reference an ASCX file in a different folder than the module folder ( good for ASCX files shared between modules where you want only a single copy )
                    ModuleControlDef.ControlSrc = ControlSrc;
                }
                else
                {
                    ModuleControlDef.ControlSrc = Path.Combine("DesktopModules", Path.Combine(Foldername, ControlSrc));
                }
                ModuleControlDef.ControlSrc = ModuleControlDef.ControlSrc.Replace(@"\", "/");
            }
            catch (Exception)
            {
                throw (new Exception(EXCEPTION_Src));
            }

            XmlElement iconFileElement = (XmlElement)ModuleControl.SelectSingleNode("iconfile");
            if (iconFileElement != null)
            {
                ModuleControlDef.IconFile = iconFileElement.InnerText.Trim();
            }

            try
            {
                ModuleControlDef.ControlType = (SecurityAccessLevel)TypeDescriptor.GetConverter(typeof(SecurityAccessLevel)).ConvertFromString(ModuleControl.SelectSingleNode("type").InnerText.Trim());
            }
            catch (Exception)
            {
                throw (new Exception(EXCEPTION_Type));
            }

            XmlElement orderElement = (XmlElement)ModuleControl.SelectSingleNode("vieworder");
            if (orderElement != null)
            {
                ModuleControlDef.ViewOrder = Convert.ToInt32(orderElement.InnerText.Trim());
            }
            else
            {
                ModuleControlDef.ViewOrder = Null.NullInteger;
            }

            //This is a temporary relationship since the ModuleDef has not been saved to the db
            //it does not have a valid ModuleDefID.  Once it is saved then we can update the
            //ModuleControlDef with the correct value.
            ModuleControlDef.ModuleDefID = TempModuleID;

            return ModuleControlDef;
        }

        protected virtual ModuleDefinitionInfo GetModuleFromNode(int TempModuleDefinitionID, PaFolder Folder, XmlElement DNNModule)
        {
            ModuleDefinitionInfo ModuleDef = new ModuleDefinitionInfo();

            XmlElement friendlyNameElement = (XmlElement)DNNModule.SelectSingleNode("friendlyname");
            if (friendlyNameElement != null)
            {
                ModuleDef.FriendlyName = friendlyNameElement.InnerText.Trim();
            }

            ModuleDef.TempModuleID = TempModuleDefinitionID;
            //We need to ensure that admin order is null for "User" modules

            InstallerInfo.Log.AddInfo(string.Format(MODULES_ControlInfo, ModuleDef.FriendlyName));

            XmlElement ModuleControl;
            foreach (XmlElement tempLoopVar_ModuleControl in DNNModule.SelectNodes("controls/control"))
            {
                ModuleControl = tempLoopVar_ModuleControl;

                Folder.Controls.Add(GetModuleControlFromNode(Folder.Name, ModuleDef.TempModuleID, ModuleControl));
            }

            return ModuleDef;
        }

        public override PaFolderCollection ReadDnn()
        {
            //This is a very long subroutine and should probably be broken down
            //into a couple of smaller routines.  That would make it easier to
            //maintain.
            InstallerInfo.Log.StartJob(DNN_Reading);

            //Determine if XML conforms to designated schema
            ArrayList DnnErrors = ValidateDnn();
            if (DnnErrors.Count == 0)
            {
                LogValidFormat();

                PaFolderCollection Folders = new PaFolderCollection();

                XmlDocument doc = new XmlDocument();
                MemoryStream buffer = new MemoryStream(InstallerInfo.DnnFile.Buffer, false);
                doc.Load(buffer);
                InstallerInfo.Log.AddInfo(XML_Loaded);

                XmlNode dnnRoot = doc.DocumentElement;

                int TempModuleDefinitionID = 0;

                XmlElement FolderElement;
                foreach (XmlElement tempLoopVar_FolderElement in dnnRoot.SelectNodes("folders/folder"))
                {
                    FolderElement = tempLoopVar_FolderElement;
                    //We will process each folder individually.  So we don't need to keep
                    //as much in memory.

                    Folders.Add(GetFolderFromNode(ref TempModuleDefinitionID, FolderElement));
                }

                if (!InstallerInfo.Log.Valid)
                {
                    throw (new Exception(EXCEPTION_LoadFailed));
                }
                InstallerInfo.Log.EndJob(DNN_Success);

                return Folders;
            }
            else
            {
                string err;
                foreach (string tempLoopVar_err in DnnErrors)
                {
                    err = tempLoopVar_err;
                    InstallerInfo.Log.AddFailure(err);
                }
                throw (new Exception(EXCEPTION_Format));
            }
        }

        private ArrayList ValidateDnn()
        {
            //Create New Validator
            ModuleDefinitionValidator ModuleValidator = new ModuleDefinitionValidator();

            //Tell Validator what XML to use
            MemoryStream xmlStream = new MemoryStream(InstallerInfo.DnnFile.Buffer);
            ModuleValidator.Validate(xmlStream);
            return ModuleValidator.Errors;
        }

        protected virtual void LogValidFormat()
        {
            InstallerInfo.Log.AddInfo(string.Format(DNN_Valid, "2.0"));
        }
    }
}