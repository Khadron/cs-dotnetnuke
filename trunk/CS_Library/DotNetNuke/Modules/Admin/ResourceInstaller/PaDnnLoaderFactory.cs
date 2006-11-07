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