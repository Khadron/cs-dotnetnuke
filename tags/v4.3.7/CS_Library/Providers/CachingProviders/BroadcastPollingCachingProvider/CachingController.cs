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
using System.IO;
using System.Text;
using System.Xml.Serialization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Cache.BroadcastPollingCachingProvider.Data;

namespace DotNetNuke.Services.Cache.BroadcastPollingCachingProvider
{
    public class Controller
    {
        public object GetCachedObject( string Key, Type objType )
        {
            IDataReader dr = DataProvider.Instance().GetCachedObject( Key );
            try
            {
                object obj = null;
                if( dr != null )
                {
                    while( dr.Read() )
                    {
                        string str = Convert.ToString( dr["CachedObjectValue"] );
                        obj = Deserialize( str, objType );
                    }
                }
                return obj;
            }
            finally
            {
                if( dr != null )
                {
                    dr.Close();
                }
            }
        }

        public void AddCachedObject( string Key, object obj, string ServerName )
        {
            if( obj != null )
            {
                string str = Serialize( obj );
                DataProvider.Instance().AddCachedObject( Key, str, ServerName );
            }
        }

        public void DeleteCachedObject( string Key )
        {
            DataProvider.Instance().DeleteCachedObject( Key );
        }

        public int AddBroadcast( string BroadcastType, string BroadcastMessage, string ServerName )
        {
            return DataProvider.Instance().AddBroadcast( BroadcastType, BroadcastMessage, ServerName );
        }

        public ArrayList GetBroadcasts( string ServerName )
        {
            return CBO.FillCollection( DataProvider.Instance().GetBroadcasts( ServerName ), typeof( BroadcastInfo ) );
        }

        private string Serialize( object obj )
        {
            string str = XmlUtils.Serialize( obj );
            return str;
        }

        private object Deserialize( string str, Type objType )
        {
            MemoryStream objStream;
            objStream = new MemoryStream( ASCIIEncoding.Default.GetBytes( str ) );
            XmlSerializer serializer = new XmlSerializer( objType );
            TextReader tr = new StreamReader( objStream );
            object obj = serializer.Deserialize( tr );
            return obj;
        }
    }
}