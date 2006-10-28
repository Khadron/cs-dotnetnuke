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