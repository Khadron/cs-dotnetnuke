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

            
            int i = 0;
            foreach (TabPermissionInfo objTabPermission in objTabPermissionCollection)
            {
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