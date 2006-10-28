using System;
using System.Collections;

namespace DotNetNuke.Common.Lists
{
    [Serializable()]
    public class ListEntryInfoCollection : CollectionBase
    {
        private Hashtable mKeyIndexLookup = new Hashtable();

        public ListEntryInfo GetChildren( string ParentName )
        {
            return Item( ParentName );
        }

        internal new void Clear()
        {
            mKeyIndexLookup.Clear();
            base.Clear();
        }

        public void Add( string key, ListEntryInfo value )
        {
            int index;
            // <tam:note key to be lowercase for appropiated seeking>
            try
            {
                index = base.List.Add( value );
                mKeyIndexLookup.Add( key.ToLower(), index );
            }
            catch( Exception )
            {
                //Throw ex
            }
        }

        public ListEntryInfo Item( int index )
        {
            try
            {
                return ( (ListEntryInfo)base.List[index] );
            }
            catch( Exception )
            {
                return null;
            }
        }

        public ListEntryInfo Item( string key )
        {
            int index;

            try // Do validation first
            {
                if( mKeyIndexLookup[key.ToLower()] == null )
                {
                    return null;
                }
            }
            catch( Exception )
            {
                return null;
            }

            index = Convert.ToInt32( mKeyIndexLookup[key.ToLower()] );

            return ( (ListEntryInfo)base.List[index] );
        }
    }
}