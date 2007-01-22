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
using System.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Security.Permissions
{
    public class FolderPermissionController
    {
        private static FolderPermissionCollection FillFolderPermissionCollection(IDataReader dr)
        {
            FolderPermissionCollection arr = new FolderPermissionCollection();
            try
            {
                while (dr.Read())
                {
                    // fill business object
                    FolderPermissionInfo obj = FillFolderPermissionInfo(dr, false);
                    // add to collection
                    arr.Add(obj);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                // close datareader
                if (dr != null)
                {
                    dr.Close();
                }
            }
            return arr;
        }

        private static ArrayList FillFolderPermissionInfoList(IDataReader dr)
        {
            ArrayList arr = new ArrayList();
            try
            {
                while (dr.Read())
                {
                    // fill business object
                    FolderPermissionInfo obj = FillFolderPermissionInfo(dr, false);
                    // add to collection
                    arr.Add(obj);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                // close datareader
                if (dr != null)
                {
                    dr.Close();
                }
            }
            return arr;
        }

        private static FolderPermissionInfo FillFolderPermissionInfo(IDataReader dr)
        {
            return FillFolderPermissionInfo(dr, true);
        }

        private static FolderPermissionInfo FillFolderPermissionInfo(IDataReader dr, bool CheckForOpenDataReader)
        {
            FolderPermissionInfo permissionInfo;

            // read datareader
            bool canContinue = true;
            if (CheckForOpenDataReader)
            {
                canContinue = false;
                if (dr.Read())
                {
                    canContinue = true;
                }
            }

            if (canContinue)
            {
                permissionInfo = new FolderPermissionInfo();
                permissionInfo.FolderPermissionID = Convert.ToInt32(Null.SetNull(dr["FolderPermissionID"], permissionInfo.FolderPermissionID));
                permissionInfo.FolderID = Convert.ToInt32(Null.SetNull(dr["FolderID"], permissionInfo.FolderID));
                permissionInfo.FolderPath = Convert.ToString(Null.SetNull(dr["FolderPath"], permissionInfo.FolderPath));
                permissionInfo.PermissionID = Convert.ToInt32(Null.SetNull(dr["PermissionID"], permissionInfo.PermissionID));
                permissionInfo.RoleID = Convert.ToInt32(Null.SetNull(dr["RoleID"], permissionInfo.RoleID));
                permissionInfo.RoleName = Convert.ToString(Null.SetNull(dr["RoleName"], permissionInfo.RoleName));
                permissionInfo.AllowAccess = Convert.ToBoolean(Null.SetNull(dr["AllowAccess"], permissionInfo.AllowAccess));
                permissionInfo.PermissionCode = Convert.ToString(Null.SetNull(dr["PermissionCode"], permissionInfo.PermissionCode));
                permissionInfo.PermissionKey = Convert.ToString(Null.SetNull(dr["PermissionKey"], permissionInfo.PermissionKey));
                permissionInfo.PermissionName = Convert.ToString(Null.SetNull(dr["PermissionName"], permissionInfo.PermissionName));
            }
            else
            {
                permissionInfo = null;
            }

            return permissionInfo;

        }

        public int AddFolderPermission(FolderPermissionInfo objFolderPermission)
        {
            return Convert.ToInt32(DataProvider.Instance().AddFolderPermission(objFolderPermission.FolderID, objFolderPermission.PermissionID, objFolderPermission.RoleID, objFolderPermission.AllowAccess));
        }

        public FolderPermissionInfo GetFolderPermission(int FolderPermissionID)
        {
            FolderPermissionInfo permission;

            //Get permission from Database
            IDataReader dr = DataProvider.Instance().GetFolderPermission(FolderPermissionID);
            try
            {
                permission = FillFolderPermissionInfo(dr);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }

            return permission;
        }

        public ArrayList GetFolderPermissionsByFolder(int PortalID, string Folder)
        {
            return FillFolderPermissionInfoList(DataProvider.Instance().GetFolderPermissionsByFolderPath(PortalID, Folder, -1));
        }

        public FolderPermissionCollection GetFolderPermissionsByFolder(ArrayList arrFolderPermissions, string FolderPath)
        {
            FolderPermissionCollection p = new FolderPermissionCollection();

            for (int i = 0; i < arrFolderPermissions.Count; i++)
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
            for (int i = 0; i < arrFolderPermissions.Count; i++)
            {
                FolderPermissionInfo objFolderPermission = (FolderPermissionInfo)(arrFolderPermissions[i]);
                if (objFolderPermission.FolderPath == FolderPath && objFolderPermission.AllowAccess && objFolderPermission.PermissionKey == PermissionKey)
                {
                    strRoles += objFolderPermission.RoleName + ";";
                }
            }
            return strRoles;
        }

        public FolderPermissionCollection GetFolderPermissionsCollectionByFolderPath(int PortalID, string FolderPath)
        {
            return FillFolderPermissionCollection(DataProvider.Instance().GetFolderPermissionsByFolderPath(PortalID, FolderPath, -1));
        }

        public FolderPermissionCollection GetFolderPermissionsCollectionByFolderPath(ArrayList arrFolderPermissions, string FolderPath)
        {
            FolderPermissionCollection objFolderPermissionCollection = new FolderPermissionCollection(arrFolderPermissions, FolderPath);
            return objFolderPermissionCollection;
        }

        public static bool HasFolderPermission(FolderPermissionCollection objFolderPermissions, string PermissionKey)
        {
            FolderPermissionCollection m = objFolderPermissions;
            for (int i = 0; i < m.Count; i++)
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