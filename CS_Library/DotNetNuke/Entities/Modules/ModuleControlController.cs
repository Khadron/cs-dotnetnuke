using System;
using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Entities.Modules
{
    public class ModuleControlController
    {
        public ModuleControlInfo GetModuleControl(int ModuleControlId)
        {
            return ((ModuleControlInfo)CBO.FillObject(DataProvider.Instance().GetModuleControl(ModuleControlId), typeof(ModuleControlInfo)));
        }

        public ArrayList GetModuleControls(int ModuleDefID)
        {
            return CBO.FillCollection(DataProvider.Instance().GetModuleControls(ModuleDefID), typeof(ModuleControlInfo));
        }

        public ArrayList GetModuleControlsByKey(string ControlKey, int ModuleDefId)
        {
            return CBO.FillCollection(DataProvider.Instance().GetModuleControlsByKey(ControlKey, ModuleDefId), typeof(ModuleControlInfo));
        }

        public ModuleControlInfo GetModuleControlByKeyAndSrc(int ModuleDefId, string ControlKey, string ControlSrc)
        {
            return ((ModuleControlInfo)CBO.FillObject(DataProvider.Instance().GetModuleControlByKeyAndSrc(ModuleDefId, ControlKey, ControlSrc), typeof(ModuleControlInfo)));
        }

        public void AddModuleControl(ModuleControlInfo objModuleControl)
        {
            DataProvider.Instance().AddModuleControl(objModuleControl.ModuleDefID, objModuleControl.ControlKey, objModuleControl.ControlTitle, objModuleControl.ControlSrc, objModuleControl.IconFile, Convert.ToInt32(objModuleControl.ControlType), objModuleControl.ViewOrder, objModuleControl.HelpURL);
        }

        public void UpdateModuleControl(ModuleControlInfo objModuleControl)
        {
            DataProvider.Instance().UpdateModuleControl(objModuleControl.ModuleControlID, objModuleControl.ModuleDefID, objModuleControl.ControlKey, objModuleControl.ControlTitle, objModuleControl.ControlSrc, objModuleControl.IconFile, Convert.ToInt32(objModuleControl.ControlType), objModuleControl.ViewOrder, objModuleControl.HelpURL);
        }

        public void DeleteModuleControl(int ModuleControlId)
        {
            DataProvider.Instance().DeleteModuleControl(ModuleControlId);
        }
    }
}