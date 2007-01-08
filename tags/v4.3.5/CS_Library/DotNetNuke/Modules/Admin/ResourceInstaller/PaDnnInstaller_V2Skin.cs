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