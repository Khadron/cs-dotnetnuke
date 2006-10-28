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