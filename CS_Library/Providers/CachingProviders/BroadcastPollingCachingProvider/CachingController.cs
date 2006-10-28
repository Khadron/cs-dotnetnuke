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