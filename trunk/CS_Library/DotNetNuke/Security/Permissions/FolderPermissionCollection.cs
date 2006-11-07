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