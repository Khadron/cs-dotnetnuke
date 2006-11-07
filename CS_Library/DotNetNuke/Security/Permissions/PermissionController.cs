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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Security.Permissions
{
    public class PermissionController
    {
        public int AddPermission( PermissionInfo objPermission )
        {
            return DataProvider.Instance().AddPermission( objPermission.PermissionCode, objPermission.ModuleDefID, objPermission.PermissionKey, objPermission.PermissionName );
        }

        public PermissionInfo GetPermission( int permissionID )
        {
            return ( (PermissionInfo)CBO.FillObject( DataProvider.Instance().GetPermission( permissionID ), typeof( PermissionInfo ) ) );
        }

        public ArrayList GetPermissionByCodeAndKey( string PermissionCode, string PermissionKey )
        {
            return CBO.FillCollection( DataProvider.Instance().GetPermissionByCodeAndKey( PermissionCode, PermissionKey ), typeof( PermissionInfo ) );
        }

        public ArrayList GetPermissionsByFolder( int PortalID, string Folder )
        {
            return CBO.FillCollection( DataProvider.Instance().GetPermissionsByFolderPath( PortalID, Folder ), typeof( PermissionInfo ) );
        }

        public ArrayList GetPermissionsByModuleID( int ModuleID )
        {
            return CBO.FillCollection( DataProvider.Instance().GetPermissionsByModuleID( ModuleID ), typeof( PermissionInfo ) );
        }

        public ArrayList GetPermissionsByTabID( int TabID )
        {
            return CBO.FillCollection( DataProvider.Instance().GetPermissionsByTabID( TabID ), typeof( PermissionInfo ) );
        }

        public void DeletePermission( int permissionID )
        {
            DataProvider.Instance().DeletePermission( permissionID );
        }

        public void UpdatePermission( PermissionInfo objPermission )
        {
            DataProvider.Instance().UpdatePermission( objPermission.PermissionID, objPermission.PermissionCode, objPermission.ModuleDefID, objPermission.PermissionKey, objPermission.PermissionName );
        }
    }
}