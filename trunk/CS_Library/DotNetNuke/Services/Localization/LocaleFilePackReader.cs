using System;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Modules.Admin.ResourceInstaller;
using ICSharpCode.SharpZipLib.Zip;

namespace DotNetNuke.Services.Localization
{
    public class LocaleFilePackReader
    {
        private PaLogger _ProgressLog;
        protected string EXCEPTION_FileRead;
        protected string EXCEPTION_LangPack_FileMissing;
        protected string EXCEPTION_LangPack_Install;
        protected string EXCEPTION_LangPack_ManifestLoad;
        protected string EXCEPTION_LangPack_ManifestZip;
        protected string EXCEPTION_LangPack_ResourceLoad;
        protected string LOG_LangPack_CreateLocale;
        protected string LOG_LangPack_ImportFiles;
        protected string LOG_LangPack_Job_CreateLocale;
        protected string LOG_LangPack_Job_DeserializeManifest;
        protected string LOG_LangPack_Job_ImportFiles;
        protected string LOG_LangPack_Job_LoadFiles;
        protected string LOG_LangPack_Job_LoadManifest;
        protected string LOG_LangPack_LoadFiles;
        protected string LOG_LangPack_ModuleWarning;
        protected PortalSettings ResourcePortalSettings;

        public PaLogger ProgressLog
        {
            get
            {
                return this._ProgressLog;
            }
        }

        public LocaleFilePackReader()
        {
            this.ResourcePortalSettings = Globals.GetPortalSettings();
            this.EXCEPTION_FileRead = this.GetLocalizedString( "EXCEPTION_FileRead" );
            this.EXCEPTION_LangPack_Install = this.GetLocalizedString( "EXCEPTION.LangPack.Install" );
            this.EXCEPTION_LangPack_ManifestZip = this.GetLocalizedString( "EXCEPTION.LangPack.ManifestZip" );
            this.EXCEPTION_LangPack_ManifestLoad = this.GetLocalizedString( "EXCEPTION.LangPack.ManifestLoad" );
            this.EXCEPTION_LangPack_FileMissing = this.GetLocalizedString( "EXCEPTION.LangPack.FileMissing" );
            this.EXCEPTION_LangPack_ResourceLoad = this.GetLocalizedString( "EXCEPTION.LangPack.Install" );
            this.LOG_LangPack_Job_LoadManifest = this.GetLocalizedString( "LOG.LangPack.Job.LoadManifest" );
            this.LOG_LangPack_Job_DeserializeManifest = this.GetLocalizedString( "LOG.LangPack.Job.DeserializeManifest" );
            this.LOG_LangPack_Job_LoadFiles = this.GetLocalizedString( "LOG.LangPack.Job.LoadFiles" );
            this.LOG_LangPack_Job_ImportFiles = this.GetLocalizedString( "LOG.LangPack.Job.SaveFiles" );
            this.LOG_LangPack_Job_CreateLocale = this.GetLocalizedString( "LOG.LangPack.Job.CreateLocale" );
            this.LOG_LangPack_LoadFiles = this.GetLocalizedString( "LOG.LangPackReader.LoadFiles" );
            this.LOG_LangPack_ImportFiles = this.GetLocalizedString( "LOG.LangPackReader.UnzipFiles" );
            this.LOG_LangPack_CreateLocale = this.GetLocalizedString( "LOG.LangPackReader.CreateLocale" );
            this.LOG_LangPack_ModuleWarning = this.GetLocalizedString( "LOG.LangPackReader.ModuleWarning" );
            this._ProgressLog = new PaLogger();
        }

        protected string GetFullLocaleFileName(string RootPath, LocaleFileInfo LocaleFile)
        {
            string Result = RootPath + "/";
            if (LocaleFile.LocaleModule != null)
            {
                Result += LocaleFile.LocaleModule + "/";
            }
            if (LocaleFile.LocalePath != null)
            {
                Result += LocaleFile.LocalePath + "/";
            }
            Result += Localization.LocalResourceDirectory + "/" + LocaleFile.LocaleFileName;
            return Result;
        }

        protected LocaleFilePack GetLanguagePack(string Manifest)
        {
            XmlSerializer ManifestSerializer = new XmlSerializer(typeof(LocaleFilePack));
            LocaleFilePack LanguagePack = null;
            try
            {
                StringReader ManifestXML = new StringReader(Manifest);
                XmlTextReader XMLText = new XmlTextReader(ManifestXML);
                LanguagePack = ((LocaleFilePack)ManifestSerializer.Deserialize(XMLText));
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
                ProgressLog.AddFailure(string.Format(EXCEPTION_LangPack_ManifestLoad, ex.Message));
            }
            finally
            {
            }

            return LanguagePack;
        }

        protected byte[] GetLanguagePackManifest(Stream LangPackStream)
        {
            ZipInputStream unzip = new ZipInputStream(LangPackStream);
            byte[] Buffer = null;
            try
            {
                ZipEntry entry = unzip.GetNextEntry();

                while (!(entry == null))
                {
                    if (!entry.IsDirectory && Path.GetFileName(entry.Name).ToLower() == "manifest.xml")
                    {
                        break;
                    }
                    entry = unzip.GetNextEntry();
                }

                int size = 0;
                if (entry != null)
                {
                    Buffer = new byte[Convert.ToInt32(entry.Size) - 1 + 1];
                    while (size < Buffer.Length)
                    {
                        size += unzip.Read(Buffer, size, Buffer.Length - size);
                    }
                    if (size != Buffer.Length)
                    {
                        throw (new Exception(EXCEPTION_FileRead + Buffer.Length + "/" + size));
                    }
                }
                else
                {
                    throw (new Exception(EXCEPTION_FileRead + Buffer.Length + "/" + size));
                }
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
                ProgressLog.AddFailure(string.Format(EXCEPTION_LangPack_ManifestZip, ex.Message));
            }
            finally
            {
            }

            return Buffer;
        }

        private string GetLocalizedString(string key)
        {
            return Localization.GetString(key, ResourcePortalSettings);
        }

        public PaLogger Install(string FileName)
        {
            FileStream strm = new FileStream(FileName, FileMode.Open);
            try
            {
                Install(strm);
            }
            finally
            {
                strm.Close();
            }

            return ProgressLog;
        }

        public PaLogger Install(Stream FileStrm)
        {
            string Manifest;
            try
            {
                FileStrm.Position = 0;

                ProgressLog.StartJob(LOG_LangPack_Job_LoadManifest);
                Manifest = Encoding.UTF8.GetString(GetLanguagePackManifest(FileStrm));
                ProgressLog.EndJob(LOG_LangPack_Job_LoadManifest);

                ProgressLog.StartJob(LOG_LangPack_Job_DeserializeManifest);
                LocaleFilePack LanguagePack = GetLanguagePack(Manifest);
                ProgressLog.EndJob(LOG_LangPack_Job_DeserializeManifest);

                ProgressLog.StartJob(LOG_LangPack_Job_LoadFiles);
                LoadLocaleFilesFromZip(LanguagePack, FileStrm);
                ProgressLog.EndJob(LOG_LangPack_Job_LoadFiles);

                ProgressLog.StartJob(LOG_LangPack_Job_ImportFiles);
                SaveLocaleFiles(LanguagePack);
                ProgressLog.EndJob(LOG_LangPack_Job_ImportFiles);

                ProgressLog.StartJob(LOG_LangPack_Job_CreateLocale);
                CreateLocale(LanguagePack.LocalePackCulture);
                ProgressLog.EndJob(LOG_LangPack_Job_CreateLocale);
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
                ProgressLog.AddFailure(string.Format(EXCEPTION_LangPack_Install, ex.Message));
            }

            return ProgressLog;
        }

        protected void CreateLocale(Locale LocaleCulture)
        {
            ProgressLog.AddInfo(string.Format(LOG_LangPack_CreateLocale, LocaleCulture.Text));

            switch ((new Localization()).AddLocale(LocaleCulture.Code, LocaleCulture.Text))
            {
                case "EXCEPTION.DuplicateLocale.Text":

                    ProgressLog.AddWarning(Localization.GetString("Duplicate.ErrorMessage.Text"));
                    break;
                case "EXCEPTION.SaveLocale.Text":

                    ProgressLog.AddWarning(Localization.GetString("Save.ErrorMessage.Text"));
                    break;
            }
        }

        protected void LoadLocaleFilesFromZip(LocaleFilePack LangPack, Stream LangPackStream)
        {
            LangPackStream.Position = 0;
            ZipInputStream unzip = new ZipInputStream(LangPackStream);
            try
            {
                ZipEntry entry = unzip.GetNextEntry();

                while (!(entry == null))
                {
                    if (entry.Name.ToLower() != "manifest.xml" && (!entry.IsDirectory))
                    {
                        LocaleFileInfo LocaleFile = LangPack.Files.LocaleFile(entry.Name);
                        if (LocaleFile != null)
                        {
                            ProgressLog.AddInfo(string.Format(LOG_LangPack_LoadFiles, LocaleFile.LocaleFileName));
                            LocaleFile.Buffer = new byte[Convert.ToInt32(entry.Size) - 1 + 1];
                            int size = 0;
                            while (size < LocaleFile.Buffer.Length)
                            {
                                size += unzip.Read(LocaleFile.Buffer, size, LocaleFile.Buffer.Length - size);
                            }
                            if (size != LocaleFile.Buffer.Length)
                            {
                                throw (new Exception(EXCEPTION_FileRead + LocaleFile.Buffer.Length + "/" + size));
                            }
                        }
                        else
                        {
                            //TODO: Localize -- jmb 12/15/2004
                            ProgressLog.AddInfo(string.Format(EXCEPTION_LangPack_FileMissing, entry.Name));
                        }
                    }
                    entry = unzip.GetNextEntry();
                }
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
                ProgressLog.AddFailure(string.Format(EXCEPTION_LangPack_ResourceLoad, ex.Message));
            }
        }

        protected void SaveLocaleFiles(LocaleFilePack LangPack)
        {
            string GlobalResourceDirectory = HttpContext.Current.Server.MapPath(Localization.ApplicationResourceDirectory);
            string ControlResourceDirectory = HttpContext.Current.Server.MapPath("~/controls/" + Localization.LocalResourceDirectory);
            string ProviderDirectory = HttpContext.Current.Server.MapPath("~/providers/");

            string AdminResourceRootDirectory = HttpContext.Current.Server.MapPath("~/Admin");
            string LocalResourceRootDirectory = HttpContext.Current.Server.MapPath("~/DesktopModules");

            foreach (LocaleFileInfo LocaleFile in LangPack.Files)
            {
                ProgressLog.AddInfo(string.Format(LOG_LangPack_ImportFiles, LocaleFile.LocaleFileName));
                switch (LocaleFile.LocaleFileType)
                {
                    case LocaleType.ControlResource:

                        FileSystemUtils.SaveFile(Path.Combine(ControlResourceDirectory, LocaleFile.LocaleFileName), LocaleFile.Buffer);
                        break;

                    case LocaleType.GlobalResource:

                        FileSystemUtils.SaveFile(Path.Combine(GlobalResourceDirectory, LocaleFile.LocaleFileName), LocaleFile.Buffer);
                        break;

                    case LocaleType.AdminResource:

                        FileSystemUtils.SaveFile(GetFullLocaleFileName(AdminResourceRootDirectory, LocaleFile), LocaleFile.Buffer);
                        break;

                    case LocaleType.LocalResource:

                        try
                        {
                            FileSystemUtils.SaveFile(GetFullLocaleFileName(LocalResourceRootDirectory, LocaleFile), LocaleFile.Buffer);
                        }
                        catch (Exception)
                        {
                            ProgressLog.AddInfo(string.Format(LOG_LangPack_ModuleWarning, LocaleFile.LocaleFileName, LocaleFile.LocaleModule));
                        }
                        break;
                    case LocaleType.ProviderResource:

                        try
                        {
                            FileSystemUtils.SaveFile(GetFullLocaleFileName(ProviderDirectory, LocaleFile), LocaleFile.Buffer);
                        }
                        catch (Exception)
                        {
                            ProgressLog.AddInfo(string.Format(LOG_LangPack_ModuleWarning, LocaleFile.LocaleFileName, LocaleFile.LocaleModule));
                        }
                        break;
                }
            }
        }
    }
}