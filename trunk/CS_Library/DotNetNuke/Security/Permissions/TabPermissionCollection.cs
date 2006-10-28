using System.Collections;

namespace DotNetNuke.Security.Permissions
{
    public class TabPermissionCollection : CollectionBase
    {

        public TabPermissionInfo this[int index]
        {
            get
            {
                return ((TabPermissionInfo)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }
        public TabPermissionCollection()
        {
        }

        public TabPermissionCollection(ArrayList TabPermissions)
        {
            int i;
            for (i = 0; i <= TabPermissions.Count - 1; i++)
            {
                TabPermissionInfo objTabPerm = (TabPermissionInfo)TabPermissions[i];
                Add(objTabPerm);
            }
        }

        public TabPermissionCollection(ArrayList TabPermissions, int TabId)
        {
            int i;
            for (i = 0; i <= TabPermissions.Count - 1; i++)
            {
                TabPermissionInfo objTabPerm = (TabPermissionInfo)TabPermissions[i];
                if (objTabPerm.TabID == TabId)
                {
                    Add(objTabPerm);
                }
            }
        }

        public int Add(TabPermissionInfo value)
        {
            return List.Add(value);
        }

        public bool CompareTo(TabPermissionCollection objTabPermissionCollection)
        {
            if (objTabPermissionCollection.Count != this.Count)
            {
                return false;
            }
            InnerList.Sort(new CompareTabPermissions());
            objTabPermissionCollection.InnerList.Sort(new CompareTabPermissions());

            TabPermissionInfo objTabPermission;
            int i = 0;
            foreach (TabPermissionInfo tempLoopVar_objTabPermission in objTabPermissionCollection)
            {
                objTabPermission = tempLoopVar_objTabPermission;
                if (objTabPermissionCollection[i].TabPermissionID != this[i].TabPermissionID || objTabPermissionCollection[i].AllowAccess != this[i].AllowAccess)
                {
                    return false;
                }
                i++;
            }
            return true;
        }

        public bool Contains(TabPermissionInfo value)
        {
            return List.Contains(value);
        }

        public int IndexOf(TabPermissionInfo value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, TabPermissionInfo value)
        {
            List.Insert(index, value);
        }

        public void Remove(TabPermissionInfo value)
        {
            List.Remove(value);
        }
    }
}