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
using System.Text;
using System.Text.RegularExpressions;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework.Providers;
using ICSharpCode.SharpZipLib.Zip;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    /// <summary>
    /// The PaDnnInstallerBase is a base class for all PaDnnInstallers
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class PaDnnInstallerBase : ResourceInstallerBase
    {
        private PaInstallInfo _installerInfo;
        private ArrayList _upgradeversions;

        public PaInstallInfo InstallerInfo
        {
            get
            {
                return _installerInfo;
            }
        }

        public ArrayList UpgradeVersions
        {
            get
            {
                if (_upgradeversions == null)
                {
                    _upgradeversions = new ArrayList();
                }
                return _upgradeversions;
            }
            set
            {
                _upgradeversions = value;
            }
        }

        public PaDnnInstallerBase(PaInstallInfo InstallerInfo)
        {
            _installerInfo = InstallerInfo;
        }

        protected virtual bool BatchSql(PaFile sqlFile)
        {
            bool WasSuccessful = true;

            InstallerInfo.Log.StartJob(string.Format(SQL_BeginFile, sqlFile.Name));

            string strScript = "";
            switch (sqlFile.Encoding)
            {
                case PaTextEncoding.UTF16LittleEndian:

                    strScript = GetAsciiString(sqlFile.Buffer, Encoding.Unicode); //System.Text.Encoding.Unicode.GetString(sqlFile.Buffer)
                    break;
                case PaTextEncoding.UTF16BigEndian:

                    strScript = GetAsciiString(sqlFile.Buffer, Encoding.BigEndianUnicode); //System.Text.Encoding.BigEndianUnicode.GetString(sqlFile.Buffer)
                    break;
                case PaTextEncoding.UTF8:

                    strScript = GetAsciiString(sqlFile.Buffer, Encoding.UTF8); //System.Text.Encoding.UTF8.GetString(sqlFile.Buffer)
                    break;
                case PaTextEncoding.UTF7:

                    strScript = GetAsciiString(sqlFile.Buffer, Encoding.UTF7); //System.Text.Encoding.UTF7.GetString(sqlFile.Buffer)
                    break;
                case PaTextEncoding.Unknown:

                    throw (new Exception(string.Format(SQL_UnknownFile, sqlFile.Name)));
            }

            //This check needs to be included because the unicode Byte Order mark results in an extra character at the start of the file
            //The extra character - '?' - causes an error with the database.
            if (strScript.StartsWith("?"))
            {
                strScript = strScript.Substring(1);
            }

            // execute SQL installation script
            //TODO: Transactions are removed temporarily.
            string strSQLExceptions = PortalSettings.ExecuteScript(strScript, false);

            if (!String.IsNullOrEmpty(strSQLExceptions))
            {
                InstallerInfo.Log.AddFailure(string.Format(SQL_Exceptions, "\r\n", strSQLExceptions));
                WasSuccessful = false;
            }

            InstallerInfo.Log.EndJob(string.Format(SQL_EndFile, sqlFile.Name));

            return WasSuccessful;
        }

        protected string GetAsciiString(byte[] Buffer, Encoding SourceEncoding)
        {
            // Create two different encodings.
            Encoding TargetEncoding = Encoding.ASCII;

            // Perform the conversion from one encoding to the other.
            byte[] asciiBytes = Encoding.Convert(SourceEncoding, TargetEncoding, Buffer);

            // Convert the new byte[] into an ascii string.
            string asciiString = Encoding.ASCII.GetString(asciiBytes);

            return asciiString;
        }

        protected virtual DesktopModuleInfo GetDesktopModule(PaFolder Folder)
        {
            DesktopModuleController objDesktopModules = new DesktopModuleController();
            DesktopModuleInfo objDesktopModule = objDesktopModules.GetDesktopModuleByFriendlyName(Folder.FriendlyName);

            return objDesktopModule;
        }

        protected virtual DesktopModuleInfo GetDesktopModuleSettings(DesktopModuleInfo objDesktopModule, PaFolder Folder)
        {
            objDesktopModule.FriendlyName = Folder.FriendlyName;
            objDesktopModule.FolderName = Folder.FolderName;
            objDesktopModule.ModuleName = Folder.ModuleName;
            objDesktopModule.Description = Folder.Description;
            objDesktopModule.Version = Folder.Version;

            return objDesktopModule;
        }

        protected int GetModDefID(int TempModDefID, ArrayList Modules)
        {
            int ModDefID = -1;

            foreach (ModuleDefinitionInfo MI in Modules)
            {
                if (MI.TempModuleID == TempModDefID)
                {
                    ModDefID = MI.ModuleDefID;
                    break;
                }
            }        
            return ModDefID;
        }

        protected virtual string UpgradeModule(DesktopModuleInfo ModuleInfo)
        {
            return null;
        }

        protected bool ValidateVersion(PaFolder Folder)
        {
            // check if desktop module exists
            DesktopModuleInfo objDesktopModule = GetDesktopModule(Folder);
            if (objDesktopModule != null)
            {
                if (String.Compare(objDesktopModule.Version, Folder.Version, false) > 0)
                {
                    return false;
                }
            }
            return true;
        }

        protected bool ValidateCompatibility(PaFolder Folder)
        {

            // check core framework compatibility
            if( !String.IsNullOrEmpty( Folder.CompatibleVersions))
            {
                try
                {
                    Match objMatch = Regex.Match(Globals.glbAppVersion, Folder.CompatibleVersions, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (objMatch.Groups[1].Value == "")
                    {
                        return false;
                    }
                }
                catch // RegExp expression is not valid
                {
                    return false;
                }
            }

            return true;

        }

        protected virtual void CreateBinFile(PaFile File)
        {
            string binFolder = Path.Combine(InstallerInfo.SitePath, "bin");
            string binFullFileName = Path.Combine(binFolder, File.Name);
            CreateFile(binFullFileName, File.Buffer);
            InstallerInfo.Log.AddInfo(FILE_Created + binFullFileName);
        }

        protected virtual void CreateDataProviderFile(PaFile file, PaFolder Folder)
        {
            string rootFolder = Path.Combine(InstallerInfo.SitePath, Path.Combine("DesktopModules", Folder.FolderName));
            string ProviderTypeFolder = Path.Combine("Providers\\DataProviders", file.Extension);
            string ProviderFolder = Path.Combine(rootFolder, ProviderTypeFolder);

            if (!Directory.Exists(ProviderFolder))
            {
                Directory.CreateDirectory(ProviderFolder);
            }

            // save file
            string FullFileName = Path.Combine(ProviderFolder, file.Name);
            CreateFile(FullFileName, file.Buffer);
            InstallerInfo.Log.AddInfo(FILE_Created + FullFileName);
        }

        protected void CreateFile(string FullFileName, byte[] Buffer)
        {
            if (File.Exists(FullFileName))
            {
                File.SetAttributes(FullFileName, FileAttributes.Normal);
            }
            FileStream fs = new FileStream(FullFileName, FileMode.Create, FileAccess.Write);
            fs.Write(Buffer, 0, Buffer.Length);
            fs.Close();
        }

        protected virtual void CreateFiles(PaFolder Folder)
        {
            InstallerInfo.Log.StartJob(FILES_Creating);

            // create the files
            foreach (PaFile file in Folder.Files)
            {
                switch (file.Type)
                {
                    case PaFileType.DataProvider:

                        // We need to store uninstall files in the main module directory because
                        // that is where the uninstaller expects to find them
                        if (file.Name.ToLower().IndexOf("uninstall") != -1)
                        {
                            CreateModuleFile(file, Folder);
                        }
                        else
                        {
                            CreateDataProviderFile(file, Folder);
                        }
                        break;
                    case PaFileType.Dll:

                        CreateBinFile(file);
                        break;
                    case PaFileType.Dnn:
                        CreateModuleFile(file, Folder);
                        break;

                    case PaFileType.Ascx:
                        CreateModuleFile(file, Folder);
                        break;

                    case PaFileType.Sql:
                        CreateModuleFile(file, Folder);
                        break;

                    case PaFileType.Other:

                        CreateModuleFile(file, Folder);
                        break;
                }
            }

            InstallerInfo.Log.EndJob(FILES_Created);
        }

        protected virtual void CreateModuleFile(PaFile File, PaFolder Folder)
        {
            // account for skinobject names which include [ ]
            string FolderName = Folder.FolderName.Trim('[').Trim(']');

            string rootFolder = "";

            // check for the [app_code] token ( dynamic modules )
            if (File.Path.ToLower().StartsWith("[app_code]"))
            {
                rootFolder = Path.Combine(InstallerInfo.SitePath, Path.Combine("App_Code", FolderName));
                // remove the [app_code] token
                if (File.Path.ToLower() == "[app_code]")
                {
                    File.Path = "";
                }
                else // there is extra path info to retain
                {
                    File.Path = File.Path.Substring(10);
                }
                Config.AddCodeSubDirectory(FolderName);
            }
            else
            {
                rootFolder = Path.Combine(InstallerInfo.SitePath, Path.Combine("DesktopModules", FolderName));
            }

            // create the root folder
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }

            //if this is a Resource file, then we need to expand the resourcefile
            if (Folder.ResourceFile != null && File.Name.ToLower().Equals(Folder.ResourceFile.ToLower()))
            {
                CreateResourceFile(File, rootFolder);
            }
            else
            {
                // create the actual file folder which includes any relative filepath info
                string fileFolder = Path.Combine(rootFolder, File.Path);
                if (!Directory.Exists(fileFolder))
                {
                    Directory.CreateDirectory(fileFolder);
                }

                // save file
                string FullFileName = Path.Combine(fileFolder, File.Name);
                if (File.Type == PaFileType.Dnn)
                {
                    FullFileName += ".config"; // add a forbidden extension so that it is not browsable
                }
                CreateFile(FullFileName, File.Buffer);
                InstallerInfo.Log.AddInfo(FILE_Created + FullFileName);
            }
        }

        protected virtual void CreateResourceFile(PaFile ResourceFile, string RootFolder)
        {
            InstallerInfo.Log.StartJob(FILES_Expanding);

            try
            {
                ZipInputStream objZipInputStream = new ZipInputStream(new MemoryStream(ResourceFile.Buffer));
                FileSystemUtils.UnzipResources(objZipInputStream, RootFolder);
            }
            catch (Exception)
            {
            }

            InstallerInfo.Log.EndJob(FILES_CreatedResources);
        }

        protected virtual void ExecuteSql(PaFolder Folder)
        {
            InstallerInfo.Log.StartJob(SQL_Begin);
            // get list of script files
            ArrayList arrScriptFiles = new ArrayList();
            PaFile InstallScript = null;

            // executing all the sql files
            foreach (PaFile file in Folder.Files)
            {
                // DataProvider files may be either: the SQL to execute, uninstall, or XML stored procs.
                // We only want to execute the first type of DataProvider files.
                if (file.Type == PaFileType.Sql || (file.Type == PaFileType.DataProvider && file.Name.ToLower().IndexOf("uninstall") == -1 && file.Name.ToLower().IndexOf(".xml") == -1))
                {
                    ProviderConfiguration objProviderConfiguration = ProviderConfiguration.GetProviderConfiguration("data");

                    if (objProviderConfiguration.DefaultProvider.ToLower() == Path.GetExtension(file.Name.ToLower()).Substring(1))
                    {
                        if (file.Name.ToLower().StartsWith("install."))
                        {
                            InstallScript = file;
                        }
                        else
                        {
                            arrScriptFiles.Add(file);
                        }
                    }
                }
            }

            // get current module version
            string strModuleVersion = "000000";
            DesktopModuleInfo objDesktopModule = GetDesktopModule(Folder);
            if (objDesktopModule != null)
            {
                strModuleVersion = objDesktopModule.Version.Replace(".", "");
            }

            if (InstallScript != null && objDesktopModule == null)
            {
                // new install
                InstallerInfo.Log.AddInfo(SQL_Executing + InstallScript.Name);
                BatchSql(InstallScript);

                string strInstallVersion = Path.GetFileNameWithoutExtension(InstallScript.Name).Replace(".", "");
                strInstallVersion = strInstallVersion.ToLower().Replace("install", "");

                // if install script includes version number will be used a base version for upgrades
                // otherwise it is assigned an initial version of 000000
                if (!String.IsNullOrEmpty(strInstallVersion))
                {
                    strModuleVersion = strInstallVersion;
                }
                UpgradeVersions.Add(Regex.Replace(strModuleVersion, "^(?<a>\\d{2})(?<b>\\d{2})(?<c>\\d{2})$", "${a}.${b}.${c}"));
            }

            // iterate through scripts
            PaDataProviderComparer Comparer = new PaDataProviderComparer();
            arrScriptFiles.Sort(Comparer);
            foreach (PaFile scriptFile in arrScriptFiles)
            {
                string strScriptVersion = Path.GetFileNameWithoutExtension(scriptFile.Name).Replace(".", "");
                if (String.Compare(strScriptVersion, strModuleVersion, false) > 0)
                {
                    UpgradeVersions.Add(Path.GetFileNameWithoutExtension(scriptFile.Name));
                    InstallerInfo.Log.AddInfo(SQL_Executing + scriptFile.Name);
                    BatchSql(scriptFile);
                }
            }

            InstallerInfo.Log.EndJob(SQL_End);
        }

        public virtual void Install(PaFolderCollection folders)
        {
            foreach (PaFolder folder in folders)
            {
                if (ValidateCompatibility(folder))
                {
                    if (ValidateVersion(folder))
                    {
                        ExecuteSql(folder);
                        CreateFiles(folder);
                        if (folder.Modules.Count > 0)
                        {
                            RegisterModules(folder, folder.Modules, folder.Controls);
                        }
                    }
                    else
                    {
                        InstallerInfo.Log.AddWarning(INSTALL_Aborted);
                    }
                }
                else
                {
                    InstallerInfo.Log.AddWarning(INSTALL_Compatibility);
                }
            }
        }

        protected virtual void RegisterModules(PaFolder Folder, ArrayList Modules, ArrayList Controls)
        {
            InstallerInfo.Log.StartJob(REGISTER_Module);

            DesktopModuleController objDesktopModules = new DesktopModuleController();

            // check if desktop module exists
            DesktopModuleInfo objDesktopModule = GetDesktopModule(Folder);
            if (objDesktopModule == null)
            {
                objDesktopModule = new DesktopModuleInfo();
                objDesktopModule.DesktopModuleID = Null.NullInteger;
            }

            objDesktopModule = GetDesktopModuleSettings(objDesktopModule, Folder);

            if (Null.IsNull(objDesktopModule.DesktopModuleID))
            {
                // new desktop module
                objDesktopModule.DesktopModuleID = objDesktopModules.AddDesktopModule(objDesktopModule);
            }
            else
            {
                // existing desktop module
                objDesktopModules.UpdateDesktopModule(objDesktopModule);
            }

            InstallerInfo.Log.AddInfo(REGISTER_Definition);

            ModuleDefinitionController objModuleDefinitons = new ModuleDefinitionController();

            foreach (ModuleDefinitionInfo objModuleDefinition in Modules)
            {
                // check if definition exists
                ModuleDefinitionInfo objModuleDefinition2 = objModuleDefinitons.GetModuleDefinitionByName(objDesktopModule.DesktopModuleID, objModuleDefinition.FriendlyName);
                if (objModuleDefinition2 == null)
                {
                    // add new definition
                    objModuleDefinition.DesktopModuleID = objDesktopModule.DesktopModuleID;
                    objModuleDefinition.ModuleDefID = objModuleDefinitons.AddModuleDefinition(objModuleDefinition);
                }
                else
                {
                    // update existing definition
                    objModuleDefinition.ModuleDefID = objModuleDefinition2.ModuleDefID;
                    objModuleDefinitons.UpdateModuleDefinition(objModuleDefinition);
                }
            }

            InstallerInfo.Log.AddInfo(REGISTER_Controls);

            ModuleControlController objModuleControls = new ModuleControlController();

            foreach (ModuleControlInfo objModuleControl in Controls)
            {
                // get the real ModuleDefID from the associated Module
                objModuleControl.ModuleDefID = GetModDefID(objModuleControl.ModuleDefID, Modules);

                // check if control exists
                ModuleControlInfo objModuleControl2 = objModuleControls.GetModuleControlByKeyAndSrc(objModuleControl.ModuleDefID, objModuleControl.ControlKey, objModuleControl.ControlSrc);
                if (objModuleControl2 == null)
                {
                    // add new control
                    objModuleControls.AddModuleControl(objModuleControl);
                }
                else
                {
                    // update existing control
                    objModuleControl.ModuleControlID = objModuleControl2.ModuleControlID;
                    objModuleControls.UpdateModuleControl(objModuleControl);
                }
            }

            InstallerInfo.Log.EndJob(REGISTER_End);

            UpgradeModule(objDesktopModule);
        }
    }
}