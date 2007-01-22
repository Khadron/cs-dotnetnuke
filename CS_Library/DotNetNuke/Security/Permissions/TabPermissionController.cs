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
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Security.Permissions
{
    public class TabPermissionController
    {
        public static bool HasTabPermission( string PermissionKey )
        {
            PortalSettings _PortalSettings = PortalController.GetCurrentPortalSettings();
            return HasTabPermission( _PortalSettings.ActiveTab.TabPermissions, PermissionKey );
        }

        public static bool HasTabPermission( TabPermissionCollection objTabPermissions, string PermissionKey )
        {
            PortalSettings _PortalSettings = PortalController.GetCurrentPortalSettings();
            foreach( TabPermissionInfo objTabPermission in objTabPermissions )
            {
                if( objTabPermission.PermissionKey == PermissionKey && PortalSecurity.IsInRoles( objTabPermission.RoleName ) )
                {
                    return true;
                }
            }
            return false;
        }

        private void ClearPermissionCache( int tabId )
        {
            TabController objTabs = new TabController();
            TabInfo objTab = objTabs.GetTab( tabId, Null.NullInteger, false );
            DataCache.ClearTabPermissionsCache( objTab.PortalID );
        }

        private TabPermissionCollection FillTabPermissionCollection( IDataReader dr )
        {
            TabPermissionCollection arr = new TabPermissionCollection();
            try
            {
                TabPermissionInfo obj = null;
                while( dr.Read() )
                {
                    // fill business object
                    obj = FillTabPermissionInfo( dr, false );
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

        private Dictionary<int, TabPermissionCollection> FillTabPermissionDictionary( IDataReader dr )
        {
            Dictionary<int, TabPermissionCollection> dic = new Dictionary<int, TabPermissionCollection>();
            try
            {
                TabPermissionInfo obj = null;
                while( dr.Read() )
                {
                    // fill business object
                    obj = FillTabPermissionInfo( dr, false );

                    // add Tab Permission to dictionary
                    if( dic.ContainsKey( obj.TabID ) )
                    {
                        //Add TabPermission to TabPermission Collection already in dictionary for TabId
                        dic[obj.TabID].Add( obj );
                    }
                    else
                    {
                        //Create new TabPermission Collection for TabId
                        TabPermissionCollection collection = new TabPermissionCollection();

                        //Add Permission to Collection
                        collection.Add( obj );

                        //Add Collection to Dictionary
                        dic.Add( obj.TabID, collection );
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

        private TabPermissionInfo FillTabPermissionInfo( IDataReader dr )
        {
            return FillTabPermissionInfo( dr, true );
        }

        private TabPermissionInfo FillTabPermissionInfo( IDataReader dr, bool CheckForOpenDataReader )
        {
            TabPermissionInfo permissionInfo = null;

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
                permissionInfo = new TabPermissionInfo();
                permissionInfo.TabPermissionID = (int)( Null.SetNull( dr["TabPermissionID"], permissionInfo.TabPermissionID ) );
                permissionInfo.TabID = (int)( Null.SetNull( dr["TabID"], permissionInfo.TabID ) );
                permissionInfo.PermissionID = (int)( Null.SetNull( dr["PermissionID"], permissionInfo.PermissionID ) );
                permissionInfo.RoleID = (int)( Null.SetNull( dr["RoleID"], permissionInfo.RoleID ) );
                permissionInfo.RoleName = (string)( Null.SetNull( dr["RoleName"], permissionInfo.RoleName ) );
                permissionInfo.AllowAccess = (bool)( Null.SetNull( dr["AllowAccess"], permissionInfo.AllowAccess ) );
                permissionInfo.PermissionCode = (string)( Null.SetNull( dr["PermissionCode"], permissionInfo.PermissionCode ) );
                permissionInfo.PermissionKey = (string)( Null.SetNull( dr["PermissionKey"], permissionInfo.PermissionKey ) );
                permissionInfo.PermissionName = (string)( Null.SetNull( dr["PermissionName"], permissionInfo.PermissionName ) );
            }
            else
            {
                permissionInfo = null;
            }

            return permissionInfo;
        }

        private Dictionary<int, TabPermissionCollection> GetTabPermissionsDictionary( int PortalId )
        {
            //Get the Cache Key
            string key = string.Format( DataCache.TabPermissionCacheKey, PortalId );

            //Try fetching the Dictionary from the Cache
            Dictionary<int, TabPermissionCollection> dicTabPermissions = DataCache.GetCache( key ) as Dictionary<int, TabPermissionCollection>;

            if( dicTabPermissions == null )
            {
                //tabPermission caching settings
                Int32 timeOut = DataCache.TabPermissionCacheTimeOut*Convert.ToInt32( Globals.PerformanceSetting );

                //Get the Dictionary from the database
                dicTabPermissions = FillTabPermissionDictionary( DataProvider.Instance().GetTabPermissionsByPortal( PortalId ) );

                //Cache the Dictionary
                if( timeOut > 0 )
                {
                    DataCache.SetCache( key, dicTabPermissions, TimeSpan.FromMinutes( timeOut ) );
                }
            }

            //Return the Dictionary
            return dicTabPermissions;
        }

        public int AddTabPermission( TabPermissionInfo objTabPermission )
        {
            int Id = Convert.ToInt32( DataProvider.Instance().AddTabPermission( objTabPermission.TabID, objTabPermission.PermissionID, objTabPermission.RoleID, objTabPermission.AllowAccess ) );
            ClearPermissionCache( objTabPermission.TabID );
            return Id;
        }

        public void DeleteTabPermission( int TabPermissionID )
        {
            DataProvider.Instance().DeleteTabPermission( TabPermissionID );
        }

        public void DeleteTabPermissionsByTabID( int TabID )
        {
            DataProvider.Instance().DeleteTabPermissionsByTabID( TabID );
            ClearPermissionCache( TabID );
        }

        public string GetTabPermissions( TabPermissionCollection tabPermissions, string PermissionKey )
        {
            string strRoles = ";";
            foreach( TabPermissionInfo objTabPermission in tabPermissions )
            {
                if( objTabPermission.AllowAccess == true && objTabPermission.PermissionKey == PermissionKey )
                {
                    strRoles += objTabPermission.RoleName + ";";
                }
            }
            return strRoles;
        }

        public TabPermissionCollection GetTabPermissionsCollectionByTabID( int TabID, int PortalId )
        {
            bool bFound = false;

            //Get the Portal TabPermission Dictionary
            Dictionary<int, TabPermissionCollection> dicTabPermissions = GetTabPermissionsDictionary( PortalId );

            //Get the Collection from the Dictionary
            TabPermissionCollection tabPermissions = null;
            bFound = dicTabPermissions.TryGetValue( TabID, out tabPermissions );

            if( !bFound )
            {
                //try the database
                tabPermissions = FillTabPermissionCollection( DataProvider.Instance().GetTabPermissionsByTabID( TabID, -1 ) );
            }

            return tabPermissions;
        }

        public void UpdateTabPermission( TabPermissionInfo objTabPermission )
        {
            DataProvider.Instance().UpdateTabPermission( objTabPermission.TabPermissionID, objTabPermission.TabID, objTabPermission.PermissionID, objTabPermission.RoleID, objTabPermission.AllowAccess );
            ClearPermissionCache( objTabPermission.TabID );
        }

        private ArrayList FillTabPermissionInfoList( IDataReader dr )
        {
            ArrayList arr = new ArrayList();
            try
            {
                TabPermissionInfo obj = null;
                while( dr.Read() )
                {
                    // fill business object
                    obj = FillTabPermissionInfo( dr, false );
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
        public ArrayList GetTabPermissionsByPortal( int PortalID )
        {
            string key = string.Format( DataCache.TabPermissionCacheKey, PortalID );

            ArrayList arrTabPermissions = (ArrayList)( DataCache.GetCache( key ) );

            if( arrTabPermissions == null )
            {
                //tabPermission caching settings
                Int32 timeOut = DataCache.TabPermissionCacheTimeOut*Convert.ToInt32( Globals.PerformanceSetting );

                arrTabPermissions = FillTabPermissionInfoList( DataProvider.Instance().GetTabPermissionsByPortal( PortalID ) );

                //Cache tabs
                if( timeOut > 0 )
                {
                    DataCache.SetCache( key, arrTabPermissions, TimeSpan.FromMinutes( timeOut ) );
                }
            }
            return arrTabPermissions;
        }

        [Obsolete( "This method has been deprecated.  Please use GetTabPermissionsCollectionByTabID(TabId, PortalId)" )]
        public ArrayList GetTabPermissionsByTabID( int TabID )
        {
            return FillTabPermissionInfoList( DataProvider.Instance().GetTabPermissionsByTabID( TabID, -1 ) );
        }

        [Obsolete( "This method has been deprecated.  GetTabPermissions(TabPermissionCollection, String) " )]
        public string GetTabPermissionsByTabID( ArrayList arrTabPermissions, int TabID, string PermissionKey )
        {
            string strRoles = ";";
            int i = 0;
            for( i = 0; i < arrTabPermissions.Count; i++ )
            {
                TabPermissionInfo objTabPermission = (TabPermissionInfo)( arrTabPermissions[i] );
                if( objTabPermission.TabID == TabID && objTabPermission.AllowAccess == true && objTabPermission.PermissionKey == PermissionKey )
                {
                    strRoles += objTabPermission.RoleName + ";";
                }
            }
            return strRoles;
        }

        [Obsolete( "This method has been deprecated.  Please use GetTabPermissionsCollectionByTabID(TabId, PortalId)" )]
        public TabPermissionCollection GetTabPermissionsByTabID( ArrayList arrTabPermissions, int TabID )
        {
            TabPermissionCollection p = new TabPermissionCollection();
            int i = 0;
            for( i = 0; i < arrTabPermissions.Count; i++ )
            {
                TabPermissionInfo objTabPermission = (TabPermissionInfo)( arrTabPermissions[i] );
                if( objTabPermission.TabID == TabID )
                {
                    p.Add( objTabPermission );
                }
            }
            return p;
        }

        [Obsolete( "This method has been deprecated.  Please use GetTabPermissionsCollectionByTabID(TabId, PortalId)" )]
        public TabPermissionCollection GetTabPermissionsCollectionByTabID( int TabID )
        {
            return FillTabPermissionCollection( DataProvider.Instance().GetTabPermissionsByTabID( TabID, -1 ) );
        }

        [Obsolete( "This method has been deprecated.  Please use GetTabPermissionsCollectionByTabID(TabId, PortalId)" )]
        public TabPermissionCollection GetTabPermissionsCollectionByTabID( ArrayList arrTabPermissions, int TabID )
        {
            TabPermissionCollection objTabPermissionCollection = new TabPermissionCollection( arrTabPermissions, TabID );
            return objTabPermissionCollection;
        }
    }
}