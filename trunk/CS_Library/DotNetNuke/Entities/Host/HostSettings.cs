using System;
using System.Collections;
using System.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Entities.Host
{
    public class HostSettings
    {
        public static string GetHostSetting( string Key )
        {
            if( GetHostSettings().ContainsKey( Key ) )
            {
                return Convert.ToString( GetHostSettings()[Key] );
            }
            else
            {
                return "";
            }
        }

        public static Hashtable GetHostSettings()
        {
            Hashtable h;
            h = (Hashtable)DataCache.GetCache( "GetHostSettings" );
            if( h == null )
            {
                h = new Hashtable();
                IDataReader dr = DataProvider.Instance().GetHostSettings();
                while( dr.Read() )
                {
                    if( !dr.IsDBNull( 1 ) )
                    {
                        h.Add( dr.GetString( 0 ), dr.GetString( 1 ) );
                    }
                    else
                    {
                        h.Add( dr.GetString( 0 ), "" );
                    }
                }
                dr.Close();
                DataCache.SetCache( "GetHostSettings", h );
            }
            return h;
        }

        public static Hashtable GetSecureHostSettings()
        {
            Hashtable h;
            h = (Hashtable)DataCache.GetCache( "GetSecureHostSettings" );
            if( h == null )
            {
                h = new Hashtable();
                string SettingName;
                IDataReader dr = DataProvider.Instance().GetHostSettings();
                while( dr.Read() )
                {
                    if( !Convert.ToBoolean( dr[2] ) )
                    {
                        SettingName = dr.GetString( 0 );
                        if( SettingName.ToLower().IndexOf( "password" ) == -1 )
                        {
                            if( !dr.IsDBNull( 1 ) )
                            {
                                h.Add( SettingName, dr.GetString( 1 ) );
                            }
                            else
                            {
                                h.Add( SettingName, "" );
                            }
                        }
                    }
                }
                dr.Close();
                DataCache.SetCache( "GetSecureHostSettings", h );
            }
            return h;
        }
    }
}