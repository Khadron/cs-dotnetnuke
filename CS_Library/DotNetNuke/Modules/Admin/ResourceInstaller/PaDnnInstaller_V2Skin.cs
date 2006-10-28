using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    /// <summary>
    /// The PaDnnInstaller_V2Skin extends PaDnnInstallerBase to support V2 Skin Objects
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class PaDnnInstaller_V2Skin : PaDnnInstallerBase
    {
        public PaDnnInstaller_V2Skin(PaInstallInfo InstallerInfo)
            : base(InstallerInfo)
        {
        }

        protected override void RegisterModules(PaFolder Folder, ArrayList Modules, ArrayList Controls)
        {
            InstallerInfo.Log.AddInfo(REGISTER_Controls);

            ModuleControlController objModuleControls = new ModuleControlController();

            ModuleControlInfo objModuleControl;
            foreach (ModuleControlInfo tempLoopVar_objModuleControl in Controls)
            {
                objModuleControl = tempLoopVar_objModuleControl;
                // Skins Objects have a null ModuleDefID
                objModuleControl.ModuleDefID = Null.NullInteger;

                // check if control exists
                ModuleControlInfo objModuleControl2 = objModuleControls.GetModuleControlByKeyAndSrc(Null.NullInteger, objModuleControl.ControlKey, objModuleControl.ControlSrc);
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
        }
    }
}