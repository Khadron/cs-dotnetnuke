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