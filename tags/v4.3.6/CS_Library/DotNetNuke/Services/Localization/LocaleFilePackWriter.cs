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
using System.Web;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Modules.Admin.ResourceInstaller;
using DotNetNuke.Services.Log.EventLog;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Services.Localization
{
    public class LocaleFilePackWriter
    {
        private PaLogger _ProgressLog;

        public PaLogger ProgressLog
        {
            get
            {
                return this._ProgressLog;
            }
        }

        public LocaleFilePackWriter()
        {
            this._ProgressLog = new PaLogger();
        }

        private string CreateResourcePack( LocaleFilePack ResourcePack, string FileName, LanguagePackType packtype )
        {
            int CompressionLevel = 9;
            int BlockSize = 4096;
            string ResPackName;
            string ResPackShortName;
            ResPackShortName = "ResourcePack." + FileName + ".";
            if (packtype == LanguagePackType.Core || packtype == LanguagePackType.Full)
            {
                ResPackShortName += Globals.glbAppVersion + ".";
            }
            ResPackShortName += ResourcePack.LocalePackCulture.Code + ".zip";
            ResPackName = Globals.HostMapPath + ResPackShortName;

            FileStream strmZipFile = null;
            try
            {
                ProgressLog.AddInfo(string.Format(Localization.GetString("LOG.LangPack.CreateArchive"), ResPackShortName));
                strmZipFile = File.Create(ResPackName);
                ZipOutputStream strmZipStream = null;
                try
                {
                    strmZipStream = new ZipOutputStream(strmZipFile);

                    ZipEntry myZipEntry;
                    myZipEntry = new ZipEntry("Manifest.xml");

                    strmZipStream.PutNextEntry(myZipEntry);
                    strmZipStream.SetLevel(CompressionLevel);

                    byte[] FileData = GetLanguagePackManifest(ResourcePack);

                    strmZipStream.Write(FileData, 0, FileData.Length);
                    ProgressLog.AddInfo(string.Format(Localization.GetString("LOG.LangPack.SavedFile"), "Manifest.xml"));

                    foreach (LocaleFileInfo LocaleFile in ResourcePack.Files)
                    {
                        myZipEntry = new ZipEntry(LocaleFileUtil.GetFullFileName(LocaleFile));
                        strmZipStream.PutNextEntry(myZipEntry);
                        strmZipStream.Write(LocaleFile.Buffer, 0, LocaleFile.Buffer.Length);
                        ProgressLog.AddInfo(string.Format(Localization.GetString("LOG.LangPack.SavedFile"), LocaleFile.LocaleFileName));
                    }
                }
                catch (Exception ex)
                {
                    Exceptions.Exceptions.LogException(ex);
                    ProgressLog.AddFailure(string.Format(Localization.GetString("LOG.LangPack.ERROR.SavingFile"), ex));
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
                Exceptions.Exceptions.LogException(ex);
                ProgressLog.AddFailure(string.Format(Localization.GetString("LOG.LangPack.ERROR.SavingFile"), ex));
            }
            finally
            {
                if (strmZipFile != null)
                {
                    strmZipFile.Close();
                }
            }

            return ResPackName;
        }

        private byte[] GetFileAsByteArray( FileInfo FileObject )
        {
            
            byte[] buffer = new byte[( ( (int)( FileObject.Length - ( (long)1 ) ) ) + 1 )];
            FileStream strmFile = null;
            try
            {
                try
                {
                    strmFile = FileObject.OpenRead();
                    strmFile.Read( buffer, 0, buffer.Length );
                    
                }
                catch( Exception ex )
                {
                    
                    Exceptions.Exceptions.LogException( ex );
                    this.ProgressLog.AddFailure( string.Format( Localization.GetString( "Log.ERROR.CreatingByteArray" ), FileObject.Name, ex ) );
                    
                }
            }
            finally
            {
                if( strmFile != null )
                {
                    strmFile.Close();
                }
            }
            return buffer;
        }

        private byte[] GetLanguagePackManifest( LocaleFilePack ResourcePack )
        {
            
            byte[] manifest = null;
            XmlSerializer manifestSerializer = new XmlSerializer( typeof( LocaleFilePack ) );
            MemoryStream ms = new MemoryStream();
            try
            {
                manifestSerializer.Serialize(ms, ResourcePack);
                manifest = new byte[(((int)(ms.Length - 1)) + 1)];
                ms.Position = 0L;
                ms.Read(manifest, 0, ((int)ms.Length));
                this.ProgressLog.AddInfo(Localization.GetString("LOG.LangPack.SerializeManifest"));
                return manifest;
            }
            catch (Exception ex)
            {
                
                Exceptions.Exceptions.LogException(ex);
                this.ProgressLog.AddFailure(string.Format(Localization.GetString("LOG.LangPack.ERROR.ManifestFile"), ex));
                return manifest;
            }
            
            finally
            {
                ms.Close();
            }
            
        }

        private LocaleFileInfo GetLocaleFile( ref string RootPath, LocaleType ResType, FileInfo objFile )
        {
            LocaleFileInfo LocaleFile = new LocaleFileInfo();
            LocaleFile.LocaleFileName = objFile.Name;
            LocaleFile.LocalePath = StripCommonDirectory(RootPath, objFile.DirectoryName);
            LocaleFile.LocaleModule = GetModuleName(LocaleFile.LocalePath);
            LocaleFile.LocalePath = StripModuleName(LocaleFile.LocalePath, LocaleFile.LocaleModule);
            LocaleFile.LocaleFileType = ResType;
            LocaleFile.Buffer = GetFileAsByteArray(objFile);

            return LocaleFile;
        }

        public string GetModuleName( string FullPath )
        {
            string ModuleName = FullPath;
            if (ModuleName != null)
            {
                string[] Paths = ModuleName.Split('/');
                ModuleName = Paths[0];
            }
            return ModuleName;
        }

        private string GetServerPath( string RelativePath )
        {
            return HttpContext.Current.Server.MapPath( RelativePath );
        }

        public string SaveLanguagePack( Locale LocaleCulture, LanguagePackType packtype, ArrayList basefolders, string FileName )
        {
            string Result;
            LocaleFilePack ResPack = new LocaleFilePack();
            ResPack.Version = "3.0";
            ResPack.LocalePackCulture = LocaleCulture;

            switch (packtype)
            {
                case LanguagePackType.Core:

                    ProcessCoreFiles(ResPack);
                    break;

                case LanguagePackType.Module:

                    ProcessModuleFiles(ResPack, basefolders);
                    break;

                case LanguagePackType.Provider:

                    ProcessProviderFiles(ResPack, basefolders);
                    break;

                case LanguagePackType.Full:

                    ProcessCoreFiles(ResPack);
                    ProcessModuleFiles(ResPack);
                    ProcessProviderFiles(ResPack);
                    break;
            }

            ProgressLog.StartJob(Localization.GetString("LOG.LangPack.ArchiveFiles"));
            Result = CreateResourcePack(ResPack, FileName, packtype);
            ProgressLog.EndJob(Localization.GetString("LOG.LangPack.ArchiveFiles"));

            // log installation event
            try
            {
                LogInfo objEventLogInfo = new LogInfo();
                objEventLogInfo.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString();
                objEventLogInfo.LogProperties.Add(new LogDetailInfo("Install Language Pack:", FileName));
                PaLogEntry objLogEntry;
                foreach (PaLogEntry tempLoopVar_objLogEntry in ProgressLog.Logs)
                {
                    objLogEntry = tempLoopVar_objLogEntry;
                    objEventLogInfo.LogProperties.Add(new LogDetailInfo("Info:", objLogEntry.Description));
                }
                EventLogController objEventLog = new EventLogController();
                objEventLog.AddLog(objEventLogInfo);
            }
            catch (Exception)
            {
                // error
            }

            return Result;
        }

        public string StripCommonDirectory( string RootPath, string FullPath )
        {
            string NewPath = FullPath; //.Replace("/", "\")
            NewPath = NewPath.Replace(RootPath, "");
            NewPath = NewPath.Replace(Localization.LocalResourceDirectory, "");
            NewPath = NewPath.Trim('\\', '/');

            if (NewPath.Length == 0)
            {
                NewPath = null;
            }
            return NewPath;
        }

        public string StripModuleName( string BasePath, string ModuleName )
        {
            string NewPath;
            if (BasePath == null || ModuleName == null)
            {
                return null;
            }
            else
            {
                NewPath = BasePath.Replace(ModuleName, "").Trim('/');
            }

            if (NewPath.Length == 0)
            {
                NewPath = null;
            }
            return NewPath;
        }

        private void GetGlobalResourceFiles( LocaleFileCollection ResFileList, string LocaleCode )
        {
            FileInfo objFile;
            DirectoryInfo objFolder = new DirectoryInfo(GetServerPath(Localization.ApplicationResourceDirectory));

            FileInfo[] Files;
            FileInfo TimeZoneFile;
            if (LocaleCode == Localization.SystemLocale)
            {
                // This is the case for en-US which is the default locale
                Files = objFolder.GetFiles("*.resx");
                int LastIndex = Files.Length;
                Files = (FileInfo[])Utils.CopyArray((Array)Files, new FileInfo[LastIndex + 1]);
                TimeZoneFile = new FileInfo(Path.Combine(objFolder.FullName, "TimeZones.xml"));
                Files[LastIndex] = TimeZoneFile;
            }
            else
            {
                Files = objFolder.GetFiles("*." + LocaleCode + ".resx");
                int LastIndex = Files.Length;
                Files = (FileInfo[])Utils.CopyArray((Array)Files, new FileInfo[LastIndex + 1]);
                TimeZoneFile = new FileInfo(Path.Combine(objFolder.FullName, "TimeZones." + LocaleCode + ".xml"));
                Files[LastIndex] = TimeZoneFile;
            }

            foreach (FileInfo tempLoopVar_objFile in Files)
            {
                objFile = tempLoopVar_objFile;
                if ((!objFile.Name.StartsWith("Template")) && (LocaleCode != Localization.SystemLocale || (LocaleCode == Localization.SystemLocale && objFile.Name.IndexOf('.') == objFile.Name.LastIndexOf('.'))))
                {
                    LocaleFileInfo LocaleFile = new LocaleFileInfo();
                    LocaleFile.LocaleFileName = objFile.Name;
                    //Since paths are relative and all global resources exist in a known directory,
                    // we don't need a path.
                    LocaleFile.LocalePath = null;
                    LocaleFile.LocaleFileType = LocaleType.GlobalResource;
                    LocaleFile.Buffer = GetFileAsByteArray(objFile);

                    ResFileList.Add(LocaleFile);
                    ProgressLog.AddInfo(string.Format(Localization.GetString("LOG.LangPack.LoadFileName"), objFile.Name));
                }
            }
        }

        private void GetLocalResourceFiles( LocaleFileCollection ResFileList, string RootPath, string LocaleCode, LocaleType ResType, DirectoryInfo objFolder )
        {
            FileInfo objFile;
            FileInfo[] ascxFiles;
            FileInfo[] aspxFiles;

            if (LocaleCode == Localization.SystemLocale)
            {
                // This is the case for en-US which is the default locale
                ascxFiles = objFolder.GetFiles("*.ascx.resx");
                aspxFiles = objFolder.GetFiles("*.aspx.resx");
            }
            else
            {
                ascxFiles = objFolder.GetFiles("*.ascx." + LocaleCode + ".resx");
                aspxFiles = objFolder.GetFiles("*.aspx." + LocaleCode + ".resx");
            }

            foreach (FileInfo tempLoopVar_objFile in ascxFiles)
            {
                objFile = tempLoopVar_objFile;
                ResFileList.Add(GetLocaleFile(ref RootPath, ResType, objFile));
                ProgressLog.AddInfo(string.Format(Localization.GetString("LOG.LangPack.LoadFileName"), objFile.Name));
            }
            foreach (FileInfo tempLoopVar_objFile in aspxFiles)
            {
                objFile = tempLoopVar_objFile;
                ResFileList.Add(GetLocaleFile(ref RootPath, ResType, objFile));
                ProgressLog.AddInfo(string.Format(Localization.GetString("LOG.LangPack.LoadFileName"), objFile.Name));
            }
        }

        private void GetLocalSharedResourceFile( LocaleFileCollection ResFileList, string RootPath, string LocaleCode, LocaleType ResType, DirectoryInfo objFolder )
        {
            FileInfo SharedFile;
            string strFilePath;
            if (LocaleCode == Localization.SystemLocale)
            {
                // This is the case for en-US which is the default locale
                strFilePath = Path.Combine(objFolder.FullName, "SharedResources.resx");
            }
            else
            {
                strFilePath = Path.Combine(objFolder.FullName, "SharedResources." + LocaleCode + ".resx");
            }
            if (File.Exists(strFilePath))
            {
                SharedFile = new FileInfo(strFilePath);

                ResFileList.Add(GetLocaleFile(ref RootPath, ResType, SharedFile));
                ProgressLog.AddInfo(string.Format(Localization.GetString("LOG.LangPack.LoadFileName"), SharedFile.Name));
            }
        }

        private void GetResourceFiles( LocaleFileCollection ResFileList, string BasePath, string RootPath, string LocaleCode, LocaleType ResType )
        {
            string[] folders;
            try
            {
                folders = Directory.GetDirectories(BasePath);
            }
            catch
            {
                // in case a folder does not exist in DesktopModules (base admin modules)
                // just exit
                return;
            }

            string folder;
            DirectoryInfo objFolder;

            foreach (string tempLoopVar_folder in folders)
            {
                folder = tempLoopVar_folder;
                objFolder = new DirectoryInfo(folder);

                if (objFolder.Name == Localization.LocalResourceDirectory)
                {
                    // found local resource folder, add resources
                    GetLocalResourceFiles(ResFileList, RootPath, LocaleCode, ResType, objFolder);
                    GetLocalSharedResourceFile(ResFileList, RootPath, LocaleCode, ResType, objFolder);
                }
                else
                {
                    GetResourceFiles(ResFileList, folder, RootPath, LocaleCode, ResType);
                }
            }
        }

        private void ProcessCoreFiles( LocaleFilePack ResPack )
        {

            // Global files
            ProgressLog.StartJob(string.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Global")));
            GetGlobalResourceFiles(ResPack.Files, ResPack.LocalePackCulture.Code);
            ProgressLog.EndJob(string.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Global")));

            // Controls files
            ProgressLog.StartJob(string.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Control")));
            GetResourceFiles(ResPack.Files, GetServerPath("~/Controls"), GetServerPath("~/Controls"), ResPack.LocalePackCulture.Code, LocaleType.ControlResource);
            ProgressLog.EndJob(string.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Control")));

            // Admin files
            ProgressLog.StartJob(string.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Admin")));
            GetResourceFiles(ResPack.Files, GetServerPath("~/Admin"), GetServerPath("~/Admin"), ResPack.LocalePackCulture.Code, LocaleType.AdminResource);
            ProgressLog.EndJob(string.Format(Localization.GetString("LOG.LangPack.LoadFiles"), Localization.GetString("LOG.LangPack.Admin")));
        }

        private void ProcessModuleFiles( LocaleFilePack ResPack )
        {
            this.ProgressLog.StartJob( string.Format( Localization.GetString( "LOG.LangPack.LoadFiles" ), Localization.GetString( "LOG.LangPack.Module" ) ) );
            this.GetResourceFiles( ResPack.Files, this.GetServerPath( "~/desktopmodules" ), this.GetServerPath( "~/desktopmodules" ), ResPack.LocalePackCulture.Code, LocaleType.LocalResource );
            this.ProgressLog.EndJob( string.Format( Localization.GetString( "LOG.LangPack.LoadFiles" ), Localization.GetString( "LOG.LangPack.Module" ) ) );
        }

        private void ProcessModuleFiles( LocaleFilePack ResPack, ArrayList basefolders )
        {
            this.ProgressLog.StartJob( string.Format( Localization.GetString( "LOG.LangPack.LoadFiles" ), Localization.GetString( "LOG.LangPack.Module" ) ) );
            foreach( string f in basefolders )
            {
                this.GetResourceFiles( ResPack.Files, ( this.GetServerPath( "~/desktopmodules/" ) + f ), this.GetServerPath( "~/desktopmodules" ), ResPack.LocalePackCulture.Code, LocaleType.LocalResource );
            }
            this.ProgressLog.EndJob( string.Format( Localization.GetString( "LOG.LangPack.LoadFiles" ), Localization.GetString( "LOG.LangPack.Module" ) ) );
        }

        private void ProcessProviderFiles( LocaleFilePack ResPack )
        {
            this.ProgressLog.StartJob( string.Format( Localization.GetString( "LOG.LangPack.LoadFiles" ), Localization.GetString( "LOG.LangPack.Provider" ) ) );
            this.GetResourceFiles( ResPack.Files, this.GetServerPath( "~/providers" ), this.GetServerPath( "~/providers" ), ResPack.LocalePackCulture.Code, LocaleType.ProviderResource );
            this.ProgressLog.EndJob( string.Format( Localization.GetString( "LOG.LangPack.LoadFiles" ), Localization.GetString( "LOG.LangPack.Provider" ) ) );
        }

        private void ProcessProviderFiles( LocaleFilePack ResPack, ArrayList basefolders )
        {
            this.ProgressLog.StartJob( string.Format( Localization.GetString( "LOG.LangPack.LoadFiles" ), Localization.GetString( "LOG.LangPack.Provider" ) ) );
            foreach( string string1 in basefolders )
            {
                this.GetResourceFiles( ResPack.Files, ( this.GetServerPath( "~/providers/HtmlEditorProviders/" ) + string1 ), this.GetServerPath( "~/providers" ), ResPack.LocalePackCulture.Code, LocaleType.ProviderResource );
            }
            this.ProgressLog.EndJob( string.Format( Localization.GetString( "LOG.LangPack.LoadFiles" ), Localization.GetString( "LOG.LangPack.Provider" ) ) );
        }
    }
}