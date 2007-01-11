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
using System.Collections;

namespace DotNetNuke.Security.Permissions
{
    public class ModulePermissionCollection : CollectionBase
    {

        public ModulePermissionInfo this[int index]
        {
            get
            {
                return ((ModulePermissionInfo)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }
        public ModulePermissionCollection()
        {
        }

        public ModulePermissionCollection(ArrayList ModulePermissions)
        {
            int i;
            for (i = 0; i <= ModulePermissions.Count - 1; i++)
            {
                ModulePermissionInfo objModulePerm = (ModulePermissionInfo)ModulePermissions[i];
                Add(objModulePerm);
            }
        }

        public ModulePermissionCollection(ArrayList ModulePermissions, int ModuleID)
        {
            int i;
            for (i = 0; i <= ModulePermissions.Count - 1; i++)
            {
                ModulePermissionInfo objModulePerm = (ModulePermissionInfo)ModulePermissions[i];
                if (objModulePerm.ModuleID == ModuleID)
                {
                    Add(objModulePerm);
                }
            }
        }

        public int Add(ModulePermissionInfo value)
        {
            return List.Add(value);
        }

        public bool CompareTo(ModulePermissionCollection objModulePermissionCollection)
        {
            if (objModulePermissionCollection.Count != this.Count)
            {
                return false;
            }
            InnerList.Sort(new CompareModulePermissions());
            objModulePermissionCollection.InnerList.Sort(new CompareModulePermissions());

            ModulePermissionInfo objModulePermission;
            int i = 0;
            foreach (ModulePermissionInfo tempLoopVar_objModulePermission in objModulePermissionCollection)
            {
                objModulePermission = tempLoopVar_objModulePermission;
                if (objModulePermissionCollection[i].ModulePermissionID != this[i].ModulePermissionID || objModulePermissionCollection[i].AllowAccess != this[i].AllowAccess)
                {
                    return false;
                }
                i++;
            }
            return true;
        }

        public bool Contains(ModulePermissionInfo value)
        {
            return List.Contains(value);
        }

        public int IndexOf(ModulePermissionInfo value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, ModulePermissionInfo value)
        {
            List.Insert(index, value);
        }

        public void Remove(ModulePermissionInfo value)
        {
            List.Remove(value);
        }
    }
}