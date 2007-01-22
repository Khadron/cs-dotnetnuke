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
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Modules.Admin.ResourceInstaller;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using ICSharpCode.SharpZipLib.Zip;

namespace DotNetNuke.Entities.Modules
{
    /// <Summary>
    /// The PaWriter class packages a Module as a Private Assembly.
    /// </Summary>
    public class PaWriter
    {
        private string _AppCodeFolder;

        //List of Files to include in PA
        private ArrayList _Files = new ArrayList();

        //Source Folder of PA
        private string _Folder;
        private bool _IncludeSource = false;
        private bool _CreateManifest = true;
        private string _ResourceFileName;
        private bool _SupportsProbingPrivatePath = false;

        //Name of PA
        private string _Name;
        private PaLogger _ProgressLog = new PaLogger();
        private string _ZipFile;

        protected bool IncludeSource
        {
            get
            {
                return _IncludeSource;
            }           
        }

        public PaLogger ProgressLog
        {
            get
            {
                return _ProgressLog;
            }
        }

        public string ZipFile
        {
            get
            {
                return _ZipFile;
            }
            set
            {
                _ZipFile = value;
            }
        }

        protected string AppCodeFolder
        {
            get
            {
                return _AppCodeFolder;
            }
        }

        protected bool CreateManifest
        {
            get
            {
                return _CreateManifest;
            }
        }

        protected string Folder
        {
            get
            {
                return _Folder;
            }
        }

        protected string ResourceFileName
        {
            get
            {
                return _ResourceFileName;
            }
        }

        protected bool SupportsProbingPrivatePath
        {
            get
            {
                return _SupportsProbingPrivatePath;
            }
        }

        public PaWriter()
            : this(false, "")
        {
            _IncludeSource = false;
            _CreateManifest = true;
            _SupportsProbingPrivatePath = false;
        }

        public PaWriter(bool IncludeSource, string ZipFile)
        {
            _IncludeSource = IncludeSource;
            _ZipFile = ZipFile;            
        }

        public PaWriter(bool IncludeSource, bool CreateManifest, bool SupportsProbingPrivatePath, string ZipFile) : this()
        {
            _CreateManifest = CreateManifest;
            _IncludeSource = IncludeSource;
            _SupportsProbingPrivatePath = SupportsProbingPrivatePath;
            _ZipFile = ZipFile;
        }

        public string CreatePrivateAssembly(int DesktopModuleId)
        {

            string Result = "";

            //Get the Module Definition File for this Module
            DesktopModuleController objDesktopModuleController = new DesktopModuleController();
            DesktopModuleInfo objModule = objDesktopModuleController.GetDesktopModule(DesktopModuleId);
            _Folder = Globals.ApplicationMapPath + "\\DesktopModules\\" + objModule.FolderName;
            _AppCodeFolder = Globals.ApplicationMapPath + "\\App_Code\\" + objModule.FolderName;

            if (IncludeSource)
            {
                _ResourceFileName = objModule.ModuleName + "_Source.zip";
                CreateResourceFile();
            }

            //Create File List
            CreateFileList();

            if (CreateManifest)
            {
                ProgressLog.StartJob(string.Format(Localization.GetString("LOG.PAWriter.CreateManifest"), objModule.FriendlyName));
                CreateDnnManifest(objModule);
                ProgressLog.EndJob((string.Format(Localization.GetString("LOG.PAWriter.CreateManifest"), objModule.FriendlyName)));
            }

            //Always add Manifest file to file list
            AddFile(new PaFileInfo(objModule.ModuleName + ".dnn", "", Folder), true);

            ProgressLog.StartJob(string.Format(Localization.GetString("LOG.PAWriter.CreateZipFile"), objModule.FriendlyName));
            CreateZipFile();
            ProgressLog.EndJob((string.Format(Localization.GetString("LOG.PAWriter.CreateZipFile"), objModule.FriendlyName)));

            return Result;
        }

        private string CreateZipFile()
        {
            int CompressionLevel = 9;
            string ZipFileShortName = _Name;
            string ZipFileName = _ZipFile;
            if (ZipFileName == "")
            {
                ZipFileName = ZipFileShortName + ".zip";
            }
            ZipFileName = Globals.HostMapPath + ZipFileName;

            FileStream strmZipFile = null;
            try
            {
                ProgressLog.AddInfo(string.Format(Localization.GetString("LOG.PAWriter.CreateArchive"), ZipFileShortName));
                strmZipFile = File.Create(ZipFileName);
                ZipOutputStream strmZipStream = null;
                try
                {
                    strmZipStream = new ZipOutputStream(strmZipFile);
                    strmZipStream.SetLevel(CompressionLevel);
                    foreach (PaFileInfo PaFile in _Files)
                    {
                        if (File.Exists(PaFile.FullName))
                        {
                            FileSystemUtils.AddToZip( ref strmZipStream, PaFile.FullName, PaFile.Name, "" );
                            ProgressLog.AddInfo( string.Format( Localization.GetString( "LOG.PAWriter.SavedFile" ), PaFile.Name ) );
                        }
                    }
                }
                catch (Exception ex)
                {
                    Exceptions.LogException(ex);
                    ProgressLog.AddFailure(string.Format(Localization.GetString("LOG.PAWriter.ERROR.SavingFile"), ex));
                }
                finally
                {
                    if (strmZipStream != null)
                    {
                        strmZipStream.Finish();
                        strmZipStream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                ProgressLog.AddFailure(string.Format(Localization.GetString("LOG.PAWriter.ERROR.SavingFile"), ex));
            }
            finally
            {
                if (strmZipFile != null)
                {
                    strmZipFile.Close();
                }
            }

            return ZipFileName;
        }

        private void AddFile(PaFileInfo File, bool AllowUnsafeExtensions)
        {
            bool blnAdd = true;
            foreach (PaFileInfo objPaFileInfo in _Files)
            {
                if (objPaFileInfo.FullName == File.FullName)
                {
                    blnAdd = false;
                    break;
                }
            }
            if (!AllowUnsafeExtensions)
            {
                if (File.FullName.Substring(File.FullName.Length - 3).ToLower() == "dnn")
                {
                    blnAdd = false;
                }
            }

            if (blnAdd)
            {
                _Files.Add(File);
            }

        }

        private void AddFile(PaFileInfo File)
        {
            AddFile(File, false);
        }

        /// <summary>
        /// Creates the DNN manifest.
        /// </summary>
        /// <param name="objDesktopModule">The obj desktop module.</param>
        private void CreateDnnManifest(DesktopModuleInfo objDesktopModule)
        {
            string filename = "";
            _Name = objDesktopModule.ModuleName;

            //Create Manifest Document
            XmlDocument xmlManifest = new XmlDocument();

            //Root Element
            XmlNode nodeRoot = xmlManifest.CreateElement("dotnetnuke");
            nodeRoot.Attributes.Append(XmlUtils.CreateAttribute(xmlManifest, "version", "3.0"));
            nodeRoot.Attributes.Append(XmlUtils.CreateAttribute(xmlManifest, "type", "Module"));

            //Folders Element
            XmlNode nodeFolders = xmlManifest.CreateElement("folders");
            nodeRoot.AppendChild(nodeFolders);

            //Folder Element
            XmlNode nodeFolder = xmlManifest.CreateElement("folder");
            nodeFolders.AppendChild(nodeFolder);

            //Desktop Module Info
            nodeFolder.AppendChild(XmlUtils.CreateElement(xmlManifest, "name", _Name));
            nodeFolder.AppendChild(XmlUtils.CreateElement(xmlManifest, "friendlyname", objDesktopModule.FriendlyName));
            nodeFolder.AppendChild(XmlUtils.CreateElement(xmlManifest, "foldername", objDesktopModule.FolderName));
            nodeFolder.AppendChild(XmlUtils.CreateElement(xmlManifest, "modulename", _Name));
            nodeFolder.AppendChild(XmlUtils.CreateElement(xmlManifest, "description", objDesktopModule.Description));
            if (objDesktopModule.Version == Null.NullString)
            {
                objDesktopModule.Version = "01.00.00";
            }
            nodeFolder.AppendChild(XmlUtils.CreateElement(xmlManifest, "version", objDesktopModule.Version));
            nodeFolder.AppendChild(XmlUtils.CreateElement(xmlManifest, "businesscontrollerclass", objDesktopModule.BusinessControllerClass));

            nodeFolder.AppendChild(XmlUtils.CreateElement(xmlManifest, "version", objDesktopModule.Version));
            nodeFolder.AppendChild(XmlUtils.CreateElement(xmlManifest, "businesscontrollerclass", objDesktopModule.BusinessControllerClass));
            if (objDesktopModule.CompatibleVersions != "")
            {
                nodeFolder.AppendChild(XmlUtils.CreateElement(xmlManifest, "compatibleversions", objDesktopModule.CompatibleVersions));
            }

            if (SupportsProbingPrivatePath)
            {
                nodeFolder.AppendChild(XmlUtils.CreateElement(xmlManifest, "supportsprobingprivatepath", SupportsProbingPrivatePath.ToString()));
            }

            //Add Source files
            if (IncludeSource)
            {
                nodeFolder.AppendChild(XmlUtils.CreateElement(xmlManifest, "resourcefile", ResourceFileName));
            }          

            //Modules Element
            XmlNode nodeModules = xmlManifest.CreateElement("modules");
            nodeFolder.AppendChild(nodeModules);

            //Get the Module Definitions for this Module
            ModuleDefinitionController objModuleDefinitionController = new ModuleDefinitionController();
            ArrayList arrModuleDefinitions = objModuleDefinitionController.GetModuleDefinitions(objDesktopModule.DesktopModuleID);

            //Iterate through Module Definitions
            foreach (ModuleDefinitionInfo objModuleInfo in arrModuleDefinitions)
            {
                XmlNode nodeModule = xmlManifest.CreateElement("module");

                //Add module definition properties
                nodeModule.AppendChild(XmlUtils.CreateElement(xmlManifest, "friendlyname", objModuleInfo.FriendlyName));

                //Add Cache properties
                nodeModule.AppendChild(XmlUtils.CreateElement(xmlManifest, "cachetime", objModuleInfo.DefaultCacheTime.ToString()));

                //Get the Module Controls for this Module Definition
                ModuleControlController objModuleControlController = new ModuleControlController();
                ArrayList arrModuleControls = objModuleControlController.GetModuleControls(objModuleInfo.ModuleDefID);

                //Controls Element
                XmlNode nodeControls = xmlManifest.CreateElement("controls");
                nodeModule.AppendChild(nodeControls);

                //Iterate through Module Controls
                foreach (ModuleControlInfo objModuleControl in arrModuleControls)
                {
                    XmlNode nodeControl = xmlManifest.CreateElement("control");

                    //Add module control properties
                    XmlUtils.AppendElement(ref xmlManifest, nodeControl, "key", objModuleControl.ControlKey, false);
                    XmlUtils.AppendElement(ref xmlManifest, nodeControl, "title", objModuleControl.ControlTitle, false);

                    XmlUtils.AppendElement(ref xmlManifest, nodeControl, "src", objModuleControl.ControlSrc, true);
                    XmlUtils.AppendElement(ref xmlManifest, nodeControl, "iconfile", objModuleControl.IconFile, false);
                    XmlUtils.AppendElement(ref xmlManifest, nodeControl, "type", objModuleControl.ControlType.ToString(), true);
                    XmlUtils.AppendElement(ref xmlManifest, nodeControl, "helpurl", objModuleControl.HelpURL, false);

                    //Add control Node to controls
                    nodeControls.AppendChild(nodeControl);

                    //Determine the filename for the Manifest file (It should be saved with the other Module files)
                    if (filename == "")
                    {
                        filename = Folder + "\\" + objDesktopModule.ModuleName + ".dnn";
                    }
                }

                //Add module Node to modules
                nodeModules.AppendChild(nodeModule);
            }

            //Files Element
            XmlNode nodeFiles = xmlManifest.CreateElement("files");
            nodeFolder.AppendChild(nodeFiles);

            //Add the files
            foreach (PaFileInfo file in _Files)
            {
                XmlNode nodeFile = xmlManifest.CreateElement("file");

                //Add file properties
                XmlUtils.AppendElement(ref xmlManifest, nodeFile, "path", file.Path, false);
                XmlUtils.AppendElement(ref xmlManifest, nodeFile, "name", file.Name, false);

                //Add file Node to files
                nodeFiles.AppendChild(nodeFile);
            }

            //Add Root element to document
            xmlManifest.AppendChild(nodeRoot);

            //Save Manifest file
            xmlManifest.Save(filename);

        }

        private void CreateFileList()
        {

            //Create the DirectoryInfo object
            DirectoryInfo folderInfo = new DirectoryInfo(Folder);

            //Get the Project File in the folder
            FileInfo[] files = folderInfo.GetFiles("*.??proj");

            if (files.Length == 0) //Assume Dynamic (App_Code based) Module
            {

                //Add the files in the DesktopModules Folder
                ParseFolder(Folder, Folder);

                //Add the files in the AppCode Folder
                ParseFolder(AppCodeFolder, AppCodeFolder);

            }
            else //WAP Project File is present
            {

                //Parse the Project files (probably only one)
                foreach (FileInfo projFile in files)
                {
                    ParseProject(projFile);
                }

            }
        }

        private void AddFile(XmlNode xmlFile)
        {
            string relPath = xmlFile.Attributes["Include"].Value.Replace("/", "\\");
            string path = "";
            string name = relPath;
            string fullPath = Folder;
            if (relPath.LastIndexOf("\\") > -1)
            {
                path = relPath.Substring(0, relPath.LastIndexOf("\\"));
                name = relPath.Replace(path + "\\", "");
                fullPath = fullPath + "\\" + path;
            }

            AddFile(new PaFileInfo(name, path, fullPath));
        }

        private void AddSourceFiles(DirectoryInfo folder, string fileType, ref ZipOutputStream resourcesFile)
        {
            //Get the Source Files in the folder
            FileInfo[] sourceFiles = folder.GetFiles(fileType);

            foreach (FileInfo sourceFile in sourceFiles)
            {
                string filePath = sourceFile.FullName;
                string fileName = sourceFile.Name;
                string folderName = folder.FullName.Replace(Folder, "");
                if (folderName != "")
                {
                    folderName += "\\";
                }
                if (folderName.StartsWith("\\"))
                {
                    folderName = folderName.Substring(1);
                }

                FileSystemUtils.AddToZip(ref resourcesFile, filePath, fileName, folderName);
            }
        }

        private void CreateResourceFile()
        {

            //Create the DirectoryInfo object
            DirectoryInfo folderInfo = new DirectoryInfo(Folder);
            string filename = Folder + "\\" + ResourceFileName;

            //Create Zip File to hold files
            ZipOutputStream resourcesFile = new ZipOutputStream(File.Create(filename));
            resourcesFile.SetLevel(6);

            ParseFolder(folderInfo, ref resourcesFile);

            //Add Resources File to File List
            AddFile(new PaFileInfo(filename.Replace(Folder + "\\", ""), "", Folder));

            //Finish and Close Zip file
            resourcesFile.Finish();
            resourcesFile.Close();

        }

        private void ParseFolder(DirectoryInfo folder, ref ZipOutputStream resourcesFile)
        {
            //Add the resource files
            AddSourceFiles(folder, "*.sln", ref resourcesFile);
            AddSourceFiles(folder, "*.??proj", ref resourcesFile);
            AddSourceFiles(folder, "*.cs", ref resourcesFile);
            AddSourceFiles(folder, "*.vb", ref resourcesFile);
            AddSourceFiles(folder, "*.resx", ref resourcesFile);

            //Check for Provider scripts
            ProviderConfiguration objProviderConfiguration = ProviderConfiguration.GetProviderConfiguration("data");
            foreach (DictionaryEntry entry in objProviderConfiguration.Providers)
            {
                Provider objProvider = (Provider)entry.Value;
                string providerName = objProvider.Name;
                AddSourceFiles(folder, "*." + providerName, ref resourcesFile);
            }

            //Get the sub-folders in the folder
            DirectoryInfo[] folders = folder.GetDirectories();

            //Recursively call ParseFolder to add files from sub-folder tree
            foreach (DirectoryInfo subfolder in folders)
            {
                ParseFolder(subfolder, ref resourcesFile);
            }
        }

        private void ParseProject(FileInfo projFile)
        {
            string assemblyFolder = Globals.ApplicationMapPath + "/bin";

            //Create and load the Project file xml
            XmlDocument xmlProject = new XmlDocument();
            xmlProject.Load(projFile.FullName);

            // Create an XmlNamespaceManager to resolve the default namespace.
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlProject.NameTable);
            nsmgr.AddNamespace("proj", "http://schemas.microsoft.com/developer/msbuild/2003");

            //Get the Assembly Name and add to File List
            XmlNode xmlSettings = xmlProject.DocumentElement.SelectSingleNode("proj:PropertyGroup/proj:AssemblyName", nsmgr);
            string assemblyName = xmlSettings.InnerText;
            AddFile(new PaFileInfo(assemblyName + ".dll", "", assemblyFolder));

            //Add all the files that are classified as None
            foreach (XmlNode xmlFile in xmlProject.DocumentElement.SelectNodes("proj:ItemGroup/proj:None", nsmgr))
            {
                AddFile(xmlFile);
            }

            //Add all the files that are classified as Content
            foreach (XmlNode xmlFile in xmlProject.DocumentElement.SelectNodes("proj:ItemGroup/proj:Content", nsmgr))
            {
                AddFile(xmlFile);
            }
        }

        private void ParseFolder(string folderName, string rootPath)
        {
            if (Directory.Exists(folderName))
            {
                DirectoryInfo folder = new DirectoryInfo( folderName );

                //Recursively parse the subFolders
                DirectoryInfo[] subFolders = folder.GetDirectories();
                foreach( DirectoryInfo subFolder in subFolders )
                {
                    ParseFolder( subFolder.FullName, rootPath );
                }

                //Add the Files in the Folder
                FileInfo[] files = folder.GetFiles();
                foreach( FileInfo file in files )
                {
                    string path = folder.FullName.Replace( rootPath, "" );
                    if( path.StartsWith( "\\" ) )
                    {
                        path = path.Substring( 1 );
                    }
                    if( folder.FullName.ToLower().Contains( "app_code" ) )
                    {
                        path = "[app_code]" + path;
                    }
                    AddFile( new PaFileInfo( file.Name, path, folder.FullName ) );
                }
            }
        }
    }
}