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
using System;
using System.Collections;

namespace DotNetNuke.Common.Lists
{
    public class ListInfoCollection : CollectionBase
    {
        private Hashtable mKeyIndexLookup = new Hashtable();



        public ArrayList GetChild( string ParentKey )
        {
            ArrayList childList = new ArrayList();
            foreach( object child in List )
            {                
                if( ( (ListInfo)child ).Key.IndexOf( ParentKey.ToLower() ) > -1 )
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
                return List[index];                
            }
            catch( Exception )
            {
                return null;
            }
        }

        public object Item( string key )
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
            return List[index];
            
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