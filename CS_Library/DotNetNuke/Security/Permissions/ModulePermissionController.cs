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
using System.Collections.Generic;
using System.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using Microsoft.VisualBasic;
using Globals=DotNetNuke.Common.Globals;

namespace DotNetNuke.Security.Permissions
{
    public class ModulePermissionController
    {
        public static bool HasModulePermission( ModulePermissionCollection objModulePermissions, string PermissionKey )
        {
            foreach( ModulePermissionInfo objPermission in objModulePermissions )
            {
                if( objPermission.PermissionKey == PermissionKey && PortalSecurity.IsInRoles( objPermission.RoleName ) )
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasModulePermission( int moduleID, int TabId, string PermissionKey )
        {
            ModulePermissionController objModulePermissionController = new ModulePermissionController();
            ModulePermissionCollection objModulePermissions = objModulePermissionController.GetModulePermissionsCollectionByModuleID( moduleID, TabId );
            return HasModulePermission( objModulePermissions, PermissionKey );
        }

        private static void ClearPermissionCache( int moduleId )
        {
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModule( moduleId, Null.NullInteger );
            DataCache.ClearModulePermissionsCache( objModule.TabID );
        }

        private static ModulePermissionCollection FillModulePermissionCollection( IDataReader dr )
        {
            ModulePermissionCollection arr = new ModulePermissionCollection();
            try
            {
                while( dr.Read() )
                {
                    // fill business object
                    ModulePermissionInfo obj = FillModulePermissionInfo( dr, false );
                    // add to collection
                    arr.Add( obj );
                }
            }
            catch( Exception exc )
            {
                Exceptions.LogException( exc );
            }
            finally
            {
                // close datareader
                if( dr != null )
                {
                    dr.Close();
                }
            }
            return arr;
        }

        private static Dictionary<int, ModulePermissionCollection> FillModulePermissionDictionary( IDataReader dr )
        {
            Dictionary<int, ModulePermissionCollection> dic = new Dictionary<int, ModulePermissionCollection>();
            try
            {
                while( dr.Read() )
                {
                    // fill business object
                    ModulePermissionInfo obj = FillModulePermissionInfo( dr, false );

                    // add Module Permission to dictionary
                    if( dic.ContainsKey( obj.ModuleID ) )
                    {
                        //Add ModulePermission to ModulePermission Collection already in dictionary for TabId
                        dic[obj.ModuleID].Add( obj );
                    }
                    else
                    {
                        //Create new ModulePermission Collection for ModuleId
                        ModulePermissionCollection collection = new ModulePermissionCollection();

                        //Add Permission to Collection
                        collection.Add( obj );

                        //Add Collection to Dictionary
                        dic.Add( obj.ModuleID, collection );
                    }
                }
            }
            catch( Exception exc )
            {
                Exceptions.LogException( exc );
            }
            finally
            {
                // close datareader
                if( dr != null )
                {
                    dr.Close();
                }
            }
            return dic;
        }

        private static ModulePermissionInfo FillModulePermissionInfo( IDataReader dr )
        {
            return FillModulePermissionInfo( dr, true );
        }

        private static ModulePermissionInfo FillModulePermissionInfo( IDataReader dr, bool CheckForOpenDataReader )
        {
            ModulePermissionInfo permissionInfo;

            // read datareader
            bool canContinue = true;
            if( CheckForOpenDataReader )
            {
                canContinue = false;
                if( dr.Read() )
                {
                    canContinue = true;
                }
            }

            if( canContinue )
            {
                permissionInfo = new ModulePermissionInfo();
                permissionInfo.ModulePermissionID = Convert.ToInt32( Null.SetNull( dr["ModulePermissionID"], permissionInfo.ModulePermissionID ) );
                permissionInfo.ModuleID = Convert.ToInt32( Null.SetNull( dr["ModuleID"], permissionInfo.ModuleID ) );
                permissionInfo.ModuleDefID = Convert.ToInt32( Null.SetNull( dr["ModuleDefID"], permissionInfo.ModuleDefID ) );
                permissionInfo.PermissionID = Convert.ToInt32( Null.SetNull( dr["PermissionID"], permissionInfo.PermissionID ) );
                permissionInfo.RoleID = Convert.ToInt32( Null.SetNull( dr["RoleID"], permissionInfo.RoleID ) );
                permissionInfo.RoleName = Convert.ToString( Null.SetNull( dr["RoleName"], permissionInfo.RoleName ) );
                permissionInfo.AllowAccess = Convert.ToBoolean( Null.SetNull( dr["AllowAccess"], permissionInfo.AllowAccess ) );
                permissionInfo.PermissionCode = Convert.ToString( Null.SetNull( dr["PermissionCode"], permissionInfo.PermissionCode ) );
                permissionInfo.PermissionKey = Convert.ToString( Null.SetNull( dr["PermissionKey"], permissionInfo.PermissionKey ) );
                permissionInfo.PermissionName = Convert.ToString( Null.SetNull( dr["PermissionName"], permissionInfo.PermissionName ) );
            }
            else
            {
                permissionInfo = null;
            }

            return permissionInfo;
        }

        private static Dictionary<int, ModulePermissionCollection> GetModulePermissionsDictionary( int TabId )
        {
            //Get the Cache Key
            string key = string.Format( DataCache.ModulePermissionCacheKey, TabId );

            //Try fetching the Dictionary from the Cache
            Dictionary<int, ModulePermissionCollection> dicModulePermissions = DataCache.GetCache( key ) as Dictionary<int, ModulePermissionCollection>;

            if( dicModulePermissions == null )
            {
                //tabPermission caching settings
                Int32 timeOut = DataCache.TabPermissionCacheTimeOut*Convert.ToInt32( Globals.PerformanceSetting );

                //Get the Dictionary from the database
                dicModulePermissions = FillModulePermissionDictionary( DataProvider.Instance().GetModulePermissionsByTabID( TabId ) );

                //Cache the Dictionary
                if( timeOut > 0 )
                {
                    DataCache.SetCache( key, dicModulePermissions, TimeSpan.FromMinutes( timeOut ) );
                }
            }

            //Return the Dictionary
            return dicModulePermissions;
        }

        public int AddModulePermission( ModulePermissionInfo objModulePermission )
        {
            int Id = Convert.ToInt32( DataProvider.Instance().AddModulePermission( objModulePermission.ModuleID, objModulePermission.PermissionID, objModulePermission.RoleID, objModulePermission.AllowAccess ) );
            ClearPermissionCache( objModulePermission.ModuleID );
            return Id;
        }

        public int AddModulePermission( ModulePermissionInfo objModulePermission, int tabId )
        {
            int Id = Convert.ToInt32( DataProvider.Instance().AddModulePermission( objModulePermission.ModuleID, objModulePermission.PermissionID, objModulePermission.RoleID, objModulePermission.AllowAccess ) );
            DataCache.ClearModulePermissionsCache( tabId );
            return Id;
        }

        public void DeleteModulePermission( int modulePermissionID )
        {
            DataProvider.Instance().DeleteModulePermission( modulePermissionID );
        }
        
        public void DeleteModulePermissionsByModuleID( int ModuleID )
        {
            DataProvider.Instance().DeleteModulePermissionsByModuleID( ModuleID );
            ClearPermissionCache( ModuleID );
        }

        public ModulePermissionInfo GetModulePermission( int modulePermissionID )
        {
            ModulePermissionInfo permission;

            //Get permission from Database
            IDataReader dr = DataProvider.Instance().GetModulePermission( modulePermissionID );
            try
            {
                permission = FillModulePermissionInfo( dr );
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }

            return permission;
        }

        public string GetModulePermissions( ModulePermissionCollection modulePermissions, string PermissionKey )
        {
            string strRoles = ";";
            foreach( ModulePermissionInfo objModulePermission in modulePermissions )
            {
                if( objModulePermission.AllowAccess && objModulePermission.PermissionKey == PermissionKey )
                {
                    strRoles += objModulePermission.RoleName + ";";
                }
            }
            return strRoles;
        }

        public ModulePermissionCollection GetModulePermissionsCollectionByModuleID( int ModuleId, int TabId )
        {
            bool bFound;

            //Get the Tab ModulePermission Dictionary
            Dictionary<int, ModulePermissionCollection> dicModulePermissions = GetModulePermissionsDictionary( TabId );

            //Get the Collection from the Dictionary
            ModulePermissionCollection modulePermissions;
            bFound = dicModulePermissions.TryGetValue( ModuleId, out modulePermissions );

            if( !bFound )
            {
                //try the database
                modulePermissions = FillModulePermissionCollection( DataProvider.Instance().GetModulePermissionsByModuleID( ModuleId, -1 ) );
            }

            return modulePermissions;
        }

        public void UpdateModulePermission( ModulePermissionInfo objModulePermission )
        {
            DataProvider.Instance().UpdateModulePermission( objModulePermission.ModulePermissionID, objModulePermission.ModuleID, objModulePermission.PermissionID, objModulePermission.RoleID, objModulePermission.AllowAccess );
            ClearPermissionCache( objModulePermission.ModuleID );
        }

        private static ArrayList FillModulePermissionInfoList( IDataReader dr )
        {
            ArrayList arr = new ArrayList();
            try
            {
                while( dr.Read() )
                {
                    // fill business object
                    ModulePermissionInfo obj = FillModulePermissionInfo( dr, false );
                    // add to collection
                    arr.Add( obj );
                }
            }
            catch( Exception exc )
            {
                Exceptions.LogException( exc );
            }
            finally
            {
                // close datareader
                if( dr != null )
                {
                    dr.Close();
                }
            }
            return arr;
        }

        [Obsolete( "This method has been deprecated.  This should have been declared as Friend as it was never meant to be used outside of the core." )]
        public ArrayList GetModulePermissionsByPortal( int PortalID )
        {
            return FillModulePermissionInfoList( DataProvider.Instance().GetModulePermissionsByPortal( PortalID ) );
        }

        [Obsolete( "This method has been deprecated.  This should have been declared as Friend as it was never meant to be used outside of the core." )]
        public ArrayList GetModulePermissionsByTabID( int TabID )
        {
            string key = string.Format( DataCache.ModulePermissionCacheKey, TabID );
            ArrayList arrModulePermissions;

            arrModulePermissions = (ArrayList)( DataCache.GetCache( key ) );
            if( arrModulePermissions == null )
            {
                //modulePermission caching settings
                Int32 timeOut = DataCache.ModulePermissionCacheTimeOut*Convert.ToInt32( Globals.PerformanceSetting );

                arrModulePermissions = FillModulePermissionInfoList( DataProvider.Instance().GetModulePermissionsByTabID( TabID ) );

                if( timeOut != 0 )
                {
                    DataCache.SetCache( key, arrModulePermissions, TimeSpan.FromMinutes( timeOut ), true );
                }
            }
            return arrModulePermissions;
        }

        [Obsolete( "This method has been deprecated.  GetModulePermissions(ModulePermissionCollection, String) " )]
        public string GetModulePermissionsByModuleID( ModuleInfo objModule, string PermissionKey )
        {
            ModulePermissionCollection objModPerms = objModule.ModulePermissions;
            string strRoles = ";";
            foreach( ModulePermissionInfo objModulePermission in objModPerms )
            {
                if( objModulePermission.AllowAccess && objModulePermission.PermissionKey == PermissionKey )
                {
                    strRoles += objModulePermission.RoleName + ";";
                }
            }
            return strRoles;
        }

        [Obsolete( "This method has been deprecated.  Please use GetModulePermissionsCollectionByModuleID(ModuleID,TabId)" )]
        public ModulePermissionCollection GetModulePermissionsCollectionByModuleID( int moduleID )
        {
            return FillModulePermissionCollection( DataProvider.Instance().GetModulePermissionsByModuleID( moduleID, -1 ) );
        }

        [Obsolete( "This method has been deprecated.  Please use GetModulePermissionsCollectionByModuleID(ModuleID,TabId)" )]
        public ModulePermissionCollection GetModulePermissionsCollectionByModuleID( ArrayList arrModulePermissions, int moduleID )
        {
            ModulePermissionCollection objModulePermissionCollection = new ModulePermissionCollection( arrModulePermissions, moduleID );
            return objModulePermissionCollection;
        }

        [Obsolete( "This method is obsoleted.  It was used to replace lists of RoleIds by lists of RoleNames." )]
        public string GetRoleNamesFromRoleIDs( string Roles )
        {
            string strRoles = "";
            if( Roles.IndexOf( ";" ) > 0 )
            {
                string[] arrRoles = Roles.Split( ';' );
                for( int i = 0; i < arrRoles.Length; i++ )
                {
                    if( Information.IsNumeric( arrRoles[i] ) )
                    {
                        strRoles += Globals.GetRoleName( Convert.ToInt32( arrRoles[i] ) ) + ";";
                    }
                }
            }
            else if( Roles.Trim().Length > 0 )
            {
                strRoles = Globals.GetRoleName( Convert.ToInt32( Roles.Trim() ) );
            }
            if( !( strRoles.StartsWith( ";" ) ) )
            {
                strRoles += ";";
            }
            return strRoles;
        }

        [Obsolete( "This method has been deprecated.  Please use HasModulePermission(ModuleID,TabId, PermissionKey)" )]
        public static bool HasModulePermission( int moduleID, string PermissionKey )
        {            
            ModulePermissionCollection objModulePermissions = FillModulePermissionCollection( DataProvider.Instance().GetModulePermissionsByModuleID( moduleID, -1 ) );
            return HasModulePermission( objModulePermissions, PermissionKey );
        }
    }
}