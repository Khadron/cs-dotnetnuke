using System;
using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Security.Permissions
{
    public class ModulePermissionController
    {

        public int AddModulePermission(ModulePermissionInfo objModulePermission)
        {
            return Convert.ToInt32(DataProvider.Instance().AddModulePermission(objModulePermission.ModuleID, objModulePermission.PermissionID, objModulePermission.RoleID, objModulePermission.AllowAccess));
        }

        public ModulePermissionInfo GetModulePermission(int modulePermissionID)
        {
            return ((ModulePermissionInfo)CBO.FillObject(DataProvider.Instance().GetModulePermission(modulePermissionID), typeof(ModulePermissionInfo)));
        }

        public string GetModulePermissionsByModuleID(ModuleInfo objModule, string PermissionKey)
        {
            ModulePermissionCollection objModPerms = objModule.ModulePermissions;

            string strRoles = ";";
            ModulePermissionInfo objModulePermission;
            foreach (ModulePermissionInfo tempLoopVar_objModulePermission in objModPerms)
            {
                objModulePermission = tempLoopVar_objModulePermission;
                if (objModulePermission.AllowAccess == true && objModulePermission.PermissionKey == PermissionKey)
                {
                    strRoles += Globals.GetRoleName(objModulePermission.RoleID) + ";";
                }
            }
            return strRoles;
        }

        public ArrayList GetModulePermissionsByPortal(int PortalID)
        {
            return CBO.FillCollection(DataProvider.Instance().GetModulePermissionsByPortal(PortalID), typeof(ModulePermissionInfo));
        }

        public ModulePermissionCollection GetModulePermissionsCollectionByModuleID(int moduleID)
        {
            IList objModulePermissionCollection = new ModulePermissionCollection();
            CBO.FillCollection(DataProvider.Instance().GetModulePermissionsByModuleID(moduleID, -1), typeof(ModulePermissionInfo), ref objModulePermissionCollection);
            return ((ModulePermissionCollection)objModulePermissionCollection);
        }

        public string GetRoleNamesFromRoleIDs(string Roles)
        {
            string strRoles = "";
            if (Roles.IndexOf(";") > 0)
            {
                string[] arrRoles = Roles.Split(';');
                int i;
                for (i = 0; i <= arrRoles.Length - 1; i++)
                {
                    if (Information.IsNumeric(arrRoles[i]))
                    {
                        strRoles += Globals.GetRoleName(Convert.ToInt32(arrRoles[i])) + ";";
                    }
                }
            }
            else if (Roles.Trim().Length > 0)
            {
                strRoles = Globals.GetRoleName(Convert.ToInt32(Roles.Trim()));
            }
            if (!strRoles.StartsWith(";"))
            {
                strRoles += ";";
            }
            return strRoles;
        }
        public static bool HasModulePermission(ModulePermissionCollection objModulePermissions, string PermissionKey)
        {
            ModulePermissionCollection m = objModulePermissions;
            int i;
            for (i = 0; i <= m.Count - 1; i++)
            {
                ModulePermissionInfo mp;
                mp = m[i];
                if (mp.PermissionKey == PermissionKey && PortalSecurity.IsInRoles(mp.RoleName))
                {
                    return true;
                }
            }
            return false;
        }

        public void DeleteModulePermission(int modulePermissionID)
        {
            DataProvider.Instance().DeleteModulePermission(modulePermissionID);
        }

        public void DeleteModulePermissionsByModuleID(int ModuleID)
        {
            DataProvider.Instance().DeleteModulePermissionsByModuleID(ModuleID);
        }

        public void UpdateModulePermission(ModulePermissionInfo objModulePermission)
        {
            DataProvider.Instance().UpdateModulePermission(objModulePermission.ModulePermissionID, objModulePermission.ModuleID, objModulePermission.PermissionID, objModulePermission.RoleID, objModulePermission.AllowAccess);
        }
    }
}