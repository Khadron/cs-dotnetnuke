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