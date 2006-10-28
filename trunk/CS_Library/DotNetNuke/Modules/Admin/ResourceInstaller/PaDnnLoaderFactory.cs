using System;
using System.IO;
using DotNetNuke.Entities.Modules.Definitions;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    public class PaDnnLoaderFactory : ResourceInstallerBase
    {
        private PaInstallInfo _installerInfo;

        public PaInstallInfo InstallerInfo
        {
            get
            {
                return _installerInfo;
            }
        }

        public PaDnnLoaderFactory(PaInstallInfo InstallerInfo)
        {
            _installerInfo = InstallerInfo;
        }

        public PaDnnAdapterBase GetDnnAdapter()
        {
            ModuleDefinitionVersion Version = GetModuleVersion();
            PaDnnAdapterBase retValue = null;

            switch (Version)
            {
                case ModuleDefinitionVersion.V2:

                    retValue = new PaDnnAdapter_V2(InstallerInfo);
                    break;
                case ModuleDefinitionVersion.V3:

                    retValue = new PaDnnAdapter_V3(InstallerInfo);
                    break;
                case ModuleDefinitionVersion.V2_Skin:

                    retValue = new PaDnnAdapter_V2Skin(InstallerInfo);
                    break;
                case ModuleDefinitionVersion.V2_Provider:

                    retValue = new PaDnnAdapter_V2Provider(InstallerInfo);
                    break;
                case ModuleDefinitionVersion.VUnknown:

                    throw (new Exception(EXCEPTION_Format));
                    break;
            }

            return retValue;
        }

        public PaDnnInstallerBase GetDnnInstaller()
        {
            ModuleDefinitionVersion Version = GetModuleVersion();
            PaDnnInstallerBase retValue = null;

            switch (Version)
            {
                case ModuleDefinitionVersion.V2:

                    retValue = new PaDnnInstallerBase(InstallerInfo);
                    break;
                case ModuleDefinitionVersion.V3:

                    retValue = new PaDnnInstaller_V3(InstallerInfo);
                    break;
                case ModuleDefinitionVersion.V2_Skin:

                    retValue = new PaDnnInstaller_V2Skin(InstallerInfo);
                    break;
                case ModuleDefinitionVersion.V2_Provider:

                    retValue = new PaDnnInstaller_V2Provider(InstallerInfo);
                    break;
                case ModuleDefinitionVersion.VUnknown:

                    throw (new Exception(EXCEPTION_Format));
                    break;
            }

            return retValue;
        }

        private ModuleDefinitionVersion GetModuleVersion()
        {
            if (InstallerInfo.DnnFile != null)
            {
                MemoryStream buffer = new MemoryStream(InstallerInfo.DnnFile.Buffer, false);
                ModuleDefinitionValidator xval = new ModuleDefinitionValidator();
                return xval.GetModuleDefinitionVersion(buffer);
            }
            else
            {
                return ModuleDefinitionVersion.VUnknown;
            }
        }
    }
}