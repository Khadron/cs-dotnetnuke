using System.Collections;

namespace DotNetNuke.Security.Permissions
{
    public class FolderPermissionCollection : CollectionBase
    {

        public FolderPermissionInfo this[int index]
        {
            get
            {
                return ((FolderPermissionInfo)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }
        public FolderPermissionCollection()
        {
        }

        public FolderPermissionCollection(ArrayList FolderPermissions)
        {
            int i;
            for (i = 0; i <= FolderPermissions.Count - 1; i++)
            {
                FolderPermissionInfo objFolderPerm = (FolderPermissionInfo)FolderPermissions[i];
                Add(objFolderPerm);
            }
        }

        public FolderPermissionCollection(ArrayList FolderPermissions, string FolderPath)
        {
            int i;
            for (i = 0; i <= FolderPermissions.Count - 1; i++)
            {
                FolderPermissionInfo objFolderPerm = (FolderPermissionInfo)FolderPermissions[i];
                if (objFolderPerm.FolderPath == FolderPath)
                {
                    Add(objFolderPerm);
                }
            }
        }

        public int Add(FolderPermissionInfo value)
        {
            return List.Add(value);
        }

        public bool CompareTo(FolderPermissionCollection objFolderPermissionCollection)
        {
            if (objFolderPermissionCollection.Count != this.Count)
            {
                return false;
            }
            InnerList.Sort(new CompareFolderPermissions());
            objFolderPermissionCollection.InnerList.Sort(new CompareFolderPermissions());

            FolderPermissionInfo objFolderPermission;
            int i = 0;
            foreach (FolderPermissionInfo tempLoopVar_objFolderPermission in objFolderPermissionCollection)
            {
                objFolderPermission = tempLoopVar_objFolderPermission;
                if (objFolderPermissionCollection[i].FolderPermissionID != this[i].FolderPermissionID || objFolderPermissionCollection[i].AllowAccess != this[i].AllowAccess)
                {
                    return false;
                }
                i++;
            }
            return true;
        }

        public bool Contains(FolderPermissionInfo value)
        {
            return List.Contains(value);
        }

        public int IndexOf(FolderPermissionInfo value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, FolderPermissionInfo value)
        {
            List.Insert(index, value);
        }

        public void Remove(FolderPermissionInfo value)
        {
            List.Remove(value);
        }
    }
}