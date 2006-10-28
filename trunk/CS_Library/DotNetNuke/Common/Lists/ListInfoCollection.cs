using System;
using System.Collections;

namespace DotNetNuke.Common.Lists
{
    public class ListInfoCollection : CollectionBase
    {
        private Hashtable mKeyIndexLookup = new Hashtable();



        public ArrayList GetChild( string ParentKey )
        {
            object child;
            ArrayList childList = new ArrayList();
            foreach( object tempLoopVar_child in List )
            {
                child = tempLoopVar_child;
                if( ( (ListInfo)child ).@Key.IndexOf( ParentKey.ToLower() ) > -1 )
                {
                    childList.Add( child );
                }
            }
            return childList;
        }

        public ListInfo GetChildren( string ParentName )
        {
            return ( (ListInfo)Item( ParentName ) );
        }

        public object Item( int index )
        {
            try
            {
                object obj;
                obj = base.List[index];
                return obj;
            }
            catch( Exception )
            {
                return null;
            }
        }

        public object Item( string key )
        {
            int index;
            object obj;

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
            obj = base.List[index];

            return obj;
        }

        // Another method, get Lists on demand
        public object Item( string key, bool Cache )
        {
            int index;
            object obj = null;
            bool itemExists = false;

            try // Do validation first
            {
                if( mKeyIndexLookup[key.ToLower()] != null )
                {
                    itemExists = true;
                }
            }
            catch( Exception )
            {
            }

            // key will be in format Country.US:Region
            if( !itemExists )
            {
                ListController ctlLists = new ListController();
                string listName = key.Substring( key.IndexOf( ":" ) + 1 );
                string parentKey = key.Replace( listName, "" ).TrimEnd( ':' );

                ListInfo listInfo = ctlLists.GetListInfo( listName, parentKey );
                // the collection has been cache, so add this entry list into it if specified
                if( Cache )
                {
                    this.Add( listInfo.@Key, listInfo );
                    return listInfo;
                }
            }
            else
            {
                index = Convert.ToInt32( mKeyIndexLookup[key.ToLower()] );
                obj = base.List[index];
            }

            return obj;
        }

        public void Add( string key, object value )
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

        internal new void Clear()
        {
            mKeyIndexLookup.Clear();
            base.Clear();
        }
    }
}