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
using System;
using System.Collections;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Security.Permissions
{
    public class FolderPermissionController
    {
        public int AddFolderPermission(FolderPermissionInfo objFolderPermission)
        {
            return Convert.ToInt32(DataProvider.Instance().AddFolderPermission(objFolderPermission.FolderID, objFolderPermission.PermissionID, objFolderPermission.RoleID, objFolderPermission.AllowAccess));
        }

        public FolderPermissionInfo GetFolderPermission(int FolderPermissionID)
        {
            return ((FolderPermissionInfo)CBO.FillObject(DataProvider.Instance().GetFolderPermission(FolderPermissionID), typeof(FolderPermissionInfo)));
        }

        public ArrayList GetFolderPermissionsByFolder(int PortalID, string Folder)
        {
            return CBO.FillCollection(DataProvider.Instance().GetFolderPermissionsByFolderPath(PortalID, Folder, -1), typeof(FolderPermissionInfo));
        }

        public FolderPermissionCollection GetFolderPermissionsByFolder(ArrayList arrFolderPermissions, string FolderPath)
        {
            FolderPermissionCollection p = new FolderPermissionCollection();

            int i;
            for (i = 0; i <= arrFolderPermissions.Count - 1; i++)
            {
                FolderPermissionInfo objFolderPermission = (FolderPermissionInfo)arrFolderPermissions[i];
                if (objFolderPermission.FolderPath == FolderPath)
                {
                    p.Add(objFolderPermission);
                }
            }
            return p;
        }

        public string GetFolderPermissionsByFolderPath(ArrayList arrFolderPermissions, string FolderPath, string PermissionKey)
        {
            string strRoles = ";";
            int i;
            for (i = 0; i <= arrFolderPermissions.Count - 1; i++)
            {
                FolderPermissionInfo objFolderPermission = (FolderPermissionInfo)arrFolderPermissions[i];
                if (objFolderPermission.FolderPath == FolderPath && objFolderPermission.AllowAccess == true && objFolderPermission.PermissionKey == PermissionKey)
                {
                    strRoles += Globals.GetRoleName(objFolderPermission.RoleID) + ";";
                }
            }
            return strRoles;
        }

        public FolderPermissionCollection GetFolderPermissionsCollectionByFolderPath(int PortalID, string FolderPath)
        {
            IList objFolderPermissionCollection = new FolderPermissionCollection();
            CBO.FillCollection(DataProvider.Instance().GetFolderPermissionsByFolderPath(PortalID, FolderPath, -1), typeof(FolderPermissionInfo), ref objFolderPermissionCollection);
            return ((FolderPermissionCollection)objFolderPermissionCollection);
        }

        public FolderPermissionCollection GetFolderPermissionsCollectionByFolderPath(ArrayList arrFolderPermissions, string FolderPath)
        {
            FolderPermissionCollection objFolderPermissionCollection = new FolderPermissionCollection(arrFolderPermissions, FolderPath);
            return objFolderPermissionCollection;
        }

        public static bool HasFolderPermission(FolderPermissionCollection objFolderPermissions, string PermissionKey)
        {
            FolderPermissionCollection m = objFolderPermissions;
            int i;
            for (i = 0; i <= m.Count - 1; i++)
            {
                FolderPermissionInfo mp;
                mp = m[i];
                if (mp.PermissionKey == PermissionKey && PortalSecurity.IsInRoles(mp.RoleName))
                {
                    return true;
                }
            }
            return false;
        }

        public void DeleteFolderPermission(int FolderPermissionID)
        {
            DataProvider.Instance().DeleteFolderPermission(FolderPermissionID);
        }

        public void DeleteFolderPermissionsByFolder(int PortalID, string FolderPath)
        {
            DataProvider.Instance().DeleteFolderPermissionsByFolderPath(PortalID, FolderPath);
        }

        public void UpdateFolderPermission(FolderPermissionInfo objFolderPermission)
        {
            DataProvider.Instance().UpdateFolderPermission(objFolderPermission.FolderPermissionID, objFolderPermission.FolderID, objFolderPermission.PermissionID, objFolderPermission.RoleID, objFolderPermission.AllowAccess);
        }
    }
}