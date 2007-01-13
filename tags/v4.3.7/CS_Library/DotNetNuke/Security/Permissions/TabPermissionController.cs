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
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Security.Permissions
{
    public class TabPermissionController
    {
        public int AddTabPermission( TabPermissionInfo objTabPermission )
        {
            return DataProvider.Instance().AddTabPermission( objTabPermission.TabID, objTabPermission.PermissionID, objTabPermission.RoleID, objTabPermission.AllowAccess );
        }

        public ArrayList GetTabPermissionsByPortal( int PortalID )
        {
            TabPermissionController permissionController = new TabPermissionController();
            ArrayList arrTabPermissions = null;
            if( Globals.PerformanceSetting != Globals.PerformanceSettings.NoCaching )
            {
                arrTabPermissions = ( (ArrayList)DataCache.GetCache( ( "GetTabPermissionsByPortal" + PortalID.ToString() ) ) );
            }
            if( arrTabPermissions != null )
            {
                return arrTabPermissions;
            }
            arrTabPermissions = CBO.FillCollection( DataProvider.Instance().GetTabPermissionsByPortal( PortalID ), typeof( TabPermissionInfo ) );
            if( Globals.PerformanceSetting == Globals.PerformanceSettings.NoCaching )
            {
                return arrTabPermissions;
            }
            DataCache.SetCache( ( "GetTabPermissionsByPortal" + PortalID.ToString() ), arrTabPermissions );
            return arrTabPermissions;
        }

        public TabPermissionCollection GetTabPermissionsByTabID( ArrayList arrTabPermissions, int TabID )
        {
            TabPermissionCollection permissionCollection = new TabPermissionCollection();
            int total = ( arrTabPermissions.Count - 1 );
            for( int i = 0; ( i <= total ); i++ )
            {
                TabPermissionInfo info = ( (TabPermissionInfo)arrTabPermissions[i] );
                if( info.TabID == TabID )
                {
                    permissionCollection.Add( info );
                }
            }
            return permissionCollection;
        }

        public string GetTabPermissionsByTabID( ArrayList arrTabPermissions, int TabID, string PermissionKey )
        {
            string strRoles = ";";
            int total = ( arrTabPermissions.Count - 1 );
            for(int i = 0; ( i <= total ); i++ )
            {
                TabPermissionInfo objTabPermission = (TabPermissionInfo)arrTabPermissions[i];
                if (objTabPermission.TabID == TabID && objTabPermission.AllowAccess == true && objTabPermission.PermissionKey == PermissionKey)
                {
                    strRoles += objTabPermission.RoleName + ";";
                }
            }
            return strRoles;
        }

        public ArrayList GetTabPermissionsByTabID( int TabID )
        {
            return CBO.FillCollection( DataProvider.Instance().GetTabPermissionsByTabID( TabID, -1 ), typeof( TabPermissionInfo ) );
        }

        public TabPermissionCollection GetTabPermissionsCollectionByTabID( int TabID )
        {
            IList permissionCollection = new TabPermissionCollection();
            CBO.FillCollection( DataProvider.Instance().GetTabPermissionsByTabID( TabID, -1 ), typeof( TabPermissionInfo ), ref permissionCollection );
            return ( (TabPermissionCollection)permissionCollection );
        }

        public TabPermissionCollection GetTabPermissionsCollectionByTabID( ArrayList arrTabPermissions, int TabID )
        {
            return new TabPermissionCollection( arrTabPermissions, TabID );
        }

        public static bool HasTabPermission( TabPermissionCollection objTabPermissions, string PermissionKey )
        {
            
            PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();
            TabPermissionCollection tabPermissionCollection = objTabPermissions;
            int total = ( tabPermissionCollection.Count - 1 );
            for( int i = 0; ( i <= total ); i++ )
            {
                TabPermissionInfo tp = tabPermissionCollection[i];
                if (tp.PermissionKey == PermissionKey && PortalSecurity.IsInRoles(tp.RoleName))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasTabPermission( string PermissionKey )
        {
            PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();
            TabPermissionCollection t = portalSettings.ActiveTab.TabPermissions;
            int tCount = ( t.Count - 1 );
            for( int i = 0; ( i <= tCount ); i++ )
            {
                TabPermissionInfo tp = t[i];
                if (tp.PermissionKey == PermissionKey && PortalSecurity.IsInRoles(tp.RoleName))
                {
                    return true;
                }
            }
            return false;
        }

        public void DeleteTabPermission( int TabPermissionID )
        {
            DataProvider.Instance().DeleteTabPermission( TabPermissionID );
        }

        public void DeleteTabPermissionsByTabID( int TabID )
        {
            DataProvider.Instance().DeleteTabPermissionsByTabID( TabID );
        }

        public void UpdateTabPermission( TabPermissionInfo objTabPermission )
        {
            DataProvider.Instance().UpdateTabPermission( objTabPermission.TabPermissionID, objTabPermission.TabID, objTabPermission.PermissionID, objTabPermission.RoleID, objTabPermission.AllowAccess );
        }
    }
}