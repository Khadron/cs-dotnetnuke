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
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Services.Search
{
    /// <Summary>Represents a collection of SearchItemInfo objects.</Summary>
    // Warning: Custom Attribute Disabled --> [DefaultMemberAttribute("Item")]
    public class SearchItemInfoCollection : CollectionBase
    {

        public SearchItemInfo this[ int index ]
        {
            get
            {
                return ( (SearchItemInfo)this.List[index] );
            }
            set
            {
                this.List[index] = value;
            }
        }
        /// <Summary>
        /// Initializes a new instance of the SearchItemInfoCollection class.
        /// </Summary>
        public SearchItemInfoCollection()
        {
        }

        /// <Summary>
        /// Initializes a new instance of the SearchItemInfoCollection class containing the elements of the specified source collection.
        /// </Summary>
        /// <Param name="value">
        /// A SearchItemInfoCollection with which to initialize the collection.
        /// </Param>
        public SearchItemInfoCollection( SearchItemInfoCollection value )
        {
            this.AddRange( value );
        }

        /// <Summary>
        /// Initializes a new instance of the SearchItemInfoCollection class containing the specified array of SearchItemInfo objects.
        /// </Summary>
        /// <Param name="value">
        /// An array of SearchItemInfo objects with which to initialize the collection.
        /// </Param>
        public SearchItemInfoCollection( SearchItemInfo[] value )
        {
            this.AddRange( value );
        }

        /// <Summary>
        /// Initializes a new instance of the SearchItemInfoCollectionSearchItemInfoCollection class containing the specified array of SearchItemInfo objects.
        /// </Summary>
        /// <Param name="value">
        /// An arraylist of SearchItemInfo objects with which to initialize the collection.
        /// </Param>
        public SearchItemInfoCollection( ArrayList value )
        {
            this.AddRange( value );
        }

        /// <Summary>
        /// Add an element of the specified SearchItemInfo to the end of the collection.
        /// </Summary>
        /// <Param name="value">
        /// An object of type SearchItemInfo to add to the collection.
        /// </Param>
        public int Add( SearchItemInfo value )
        {
            return this.List.Add( value );
        }

        /// <Summary>
        /// Gets a value indicating whether the collection contains the specified SearchItemInfoCollection.
        /// </Summary>
        /// <Param name="value">
        /// The SearchItemInfoCollection to search for in the collection.
        /// </Param>
        /// <Returns>
        /// true if the collection contains the specified object; otherwise, false.
        /// </Returns>
        public bool Contains( SearchItemInfo value )
        {
            return this.List.Contains( value );
        }

        /// <Summary>
        /// Gets the index in the collection of the specified SearchItemInfoCollection, if it exists in the collection.
        /// </Summary>
        /// <Param name="value">
        /// The SearchItemInfoCollection to locate in the collection.
        /// </Param>
        /// <Returns>
        /// The index in the collection of the specified object, if found; otherwise, -1.
        /// </Returns>
        public int IndexOf( SearchItemInfo value )
        {
            return this.List.IndexOf( value );
        }

        public SearchItemInfoCollection ModuleItems( int ModuleId )
        {
            SearchItemInfoCollection retValue = new SearchItemInfoCollection();
            foreach (SearchItemInfo info in this)
            {
                if (info.ModuleId == ModuleId)
                {
                    retValue.Add(info);
                }
            }
            return retValue;
        }

        /// <Summary>
        /// Creates a one-dimensional Array instance containing the collection items.
        /// </Summary>
        /// <Returns>Array of type SearchItemInfo</Returns>
        public SearchItemInfo[] ToArray()
        {
            SearchItemInfo[] arr = new SearchItemInfo[( ( this.Count - 1 ) + 1 )];
            arr = ( (SearchItemInfo[])Utils.CopyArray( ( (Array)arr ), ( (Array)new SearchItemInfo[( ( this.Count - 1 ) + 1 )] ) ) );
            this.CopyTo( arr, 0 );
            return arr;
        }

        /// <Summary>
        /// Copies the elements of the specified arraylist to the end of the collection.
        /// </Summary>
        /// <Param name="value">
        /// An arraylist of type SearchItemInfo containing the objects to add to the collection.
        /// </Param>
        public void AddRange( ArrayList value )
        {
            foreach( object obj in value )
            {
                if( obj is SearchItemInfo )
                {
                    this.Add( ( (SearchItemInfo)obj ) );
                }
            }
        }

        /// <Summary>
        /// Copies the elements of the specified SearchItemInfo array to the end of the collection.
        /// </Summary>
        /// <Param name="value">
        /// An array of type SearchItemInfo containing the objects to add to the collection.
        /// </Param>
        public void AddRange( SearchItemInfo[] value )
        {
            for (int i = 0; i <= value.Length - 1; i++)
            {
                Add(value[i]);
            }
        }

        /// <Summary>
        /// Adds the contents of another SearchItemInfoCollection to the end of the collection.
        /// </Summary>
        /// <Param name="value">
        /// A SearchItemInfoCollection containing the objects to add to the collection.
        /// </Param>
        public void AddRange( SearchItemInfoCollection value )
        {
            for (int i = 0; i <= value.Count - 1; i++)
            {
                Add((SearchItemInfo)value.List[i]);
            }
        }

        /// <Summary>
        /// Copies the collection objects to a one-dimensional Array instance beginning at the specified index.
        /// </Summary>
        /// <Param name="array">
        /// The one-dimensional Array that is the destination of the values copied from the collection.
        /// </Param>
        /// <Param name="index">
        /// The index of the array at which to begin inserting.
        /// </Param>
        public void CopyTo( SearchItemInfo[] array, int index )
        {
            this.List.CopyTo( array, index );
        }

        /// <Summary>
        /// Add an element of the specified SearchItemInfo to the collection at the designated index.
        /// </Summary>
        /// <Param name="index">
        /// An Integer to indicate the location to add the object to the collection.
        /// </Param>
        /// <Param name="value">
        /// An object of type SearchItemInfo to add to the collection.
        /// </Param>
        public void Insert( int index, SearchItemInfo value )
        {
            this.List.Insert( index, value );
        }

        /// <Summary>
        /// Remove the specified object of type SearchItemInfo from the collection.
        /// </Summary>
        /// <Param name="value">
        /// An object of type SearchItemInfo to remove to the collection.
        /// </Param>
        public void Remove( SearchItemInfo value )
        {
            this.List.Remove( value );
        }
    }
}