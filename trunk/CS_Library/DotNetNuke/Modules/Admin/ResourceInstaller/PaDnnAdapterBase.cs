namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    /// <Summary>
    /// The PaDnnAdapterBase is a base class for all PaDnnAdaptors
    /// </Summary>
    public abstract class PaDnnAdapterBase : ResourceInstallerBase
    {
        private PaInstallInfo _installerInfo;

        public PaDnnAdapterBase( PaInstallInfo InstallerInfo )
        {
            this._installerInfo = InstallerInfo;
        }

        public PaInstallInfo InstallerInfo
        {
            get
            {
                return this._installerInfo;
            }
        }

        public abstract PaFolderCollection ReadDnn();
    }
}