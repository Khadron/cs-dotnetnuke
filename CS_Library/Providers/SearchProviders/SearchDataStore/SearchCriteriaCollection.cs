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
using Microsoft.VisualBasic;

namespace DotNetNuke.Services.Search
{
    /// <summary>
    /// Represents a collection of <see cref="SearchCriteria">SearchCriteria</see> objects.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	11/15/2004	documented
    /// </history>
    public class SearchCriteriaCollection : CollectionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> class.
        /// </summary>
        public SearchCriteriaCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> class containing the elements of the specified source collection.
        /// </summary>
        /// <param name="value">A <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> with which to initialize the collection.</param>
        public SearchCriteriaCollection( SearchCriteriaCollection value )
        {
            AddRange( value );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> class containing the specified array of <see cref="SearchCriteria">SearchCriteria</see> objects.
        /// </summary>
        /// <param name="value">An array of <see cref="SearchCriteria">SearchCriteria</see> objects with which to initialize the collection. </param>
        public SearchCriteriaCollection( SearchCriteria[] value )
        {
            AddRange( value );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> class containing the elements of the specified source collection.
        /// </summary>
        /// <param name="value">A criteria string with which to initialize the collection</param>
        public SearchCriteriaCollection( string value )
        {
            // split search criteria into words
            string[] Words = Strings.Split( value, " ", -1, 0 );
            string word;
            // Add all criteria without modifiers
            foreach( string tempLoopVar_word in Words )
            {
                word = tempLoopVar_word;
                SearchCriteria criterion = new SearchCriteria();

                if( ( ! word.StartsWith( "+" ) ) && ( ! word.StartsWith( "-" ) ) )
                {
                    criterion.MustInclude = false;
                    criterion.MustExclude = false;
                    criterion.Criteria = word;
                    Add( criterion );
                }
            }
            // Add all mandatory criteria
            foreach( string tempLoopVar_word in Words )
            {
                word = tempLoopVar_word;
                SearchCriteria criterion = new SearchCriteria();

                if( word.StartsWith( "+" ) )
                {
                    criterion.MustInclude = true;
                    criterion.MustExclude = false;
                    criterion.Criteria = word.Remove( 0, 1 );
                    Add( criterion );
                }
            }
            // Add all excluded criteria
            foreach( string tempLoopVar_word in Words )
            {
                word = tempLoopVar_word;
                SearchCriteria criterion = new SearchCriteria();

                if( word.StartsWith( "-" ) )
                {
                    criterion.MustInclude = false;
                    criterion.MustExclude = true;
                    criterion.Criteria = word.Remove( 0, 1 );
                    Add( criterion );
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> at the specified index in the collection.
        /// <para>
        /// In VB.Net, this property is the indexer for the <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> class.
        /// </para>
        /// </summary>
        public SearchCriteria this[ int index ]
        {
            get
            {
                return ( (SearchCriteria)List[index] );
            }
            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        /// Add an element of the specified <see cref="SearchCriteria">SearchCriteria</see> to the end of the collection.
        /// </summary>
        /// <param name="value">An object of type <see cref="SearchCriteria">SearchCriteria</see> to add to the collection.</param>
        public int Add( SearchCriteria value )
        {
            return List.Add( value );
        } //Add

        /// <summary>
        /// Gets the index in the collection of the specified <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see>, if it exists in the collection.
        /// </summary>
        /// <param name="value">The <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> to locate in the collection.</param>
        /// <returns>The index in the collection of the specified object, if found; otherwise, -1.</returns>
        public int IndexOf( SearchCriteria value )
        {
            return List.IndexOf( value );
        } //IndexOf

        /// <summary>
        /// Add an element of the specified <see cref="SearchCriteria">SearchCriteria</see> to the collection at the designated index.
        /// </summary>
        /// <param name="index">An Integer to indicate the location to add the object to the collection.</param>
        /// <param name="value">An object of type <see cref="SearchCriteria">SearchCriteria</see> to add to the collection.</param>
        public void Insert( int index, SearchCriteria value )
        {
            List.Insert( index, value );
        } //Insert

        /// <summary>
        /// Remove the specified object of type <see cref="SearchCriteria">SearchCriteria</see> from the collection.
        /// </summary>
        /// <param name="value">An object of type <see cref="SearchCriteria">SearchCriteria</see> to remove to the collection.</param>
        public void Remove( SearchCriteria value )
        {
            List.Remove( value );
        } //Remove

        /// <summary>
        /// Gets a value indicating whether the collection contains the specified <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see>.
        /// </summary>
        /// <param name="value">The <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> to search for in the collection.</param>
        /// <returns><b>true</b> if the collection contains the specified object; otherwise, <b>false</b>.</returns>
        public bool Contains( SearchCriteria value )
        {
            // If value is not of type SearchCriteria, this will return false.
            return List.Contains( value );
        } //Contains

        /// <summary>
        /// Copies the elements of the specified <see cref="SearchCriteria">SearchCriteria</see> array to the end of the collection.
        /// </summary>
        /// <param name="value">An array of type <see cref="SearchCriteria">SearchCriteria</see> containing the objects to add to the collection.</param>
        public void AddRange( SearchCriteria[] value )
        {
            for( int i = 0; i <= value.Length - 1; i++ )
            {
                Add( value[ i ] );
            }
        }

        /// <summary>
        /// Adds the contents of another <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> to the end of the collection.
        /// </summary>
        /// <param name="value">A <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> containing the objects to add to the collection. </param>
        public void AddRange( SearchCriteriaCollection value )
        {
            for( int i = 0; i <= value.Count - 1; i++ )
            {
                Add( (SearchCriteria)value.List[ i ] );
            }
        }

        /// <summary>
        /// Copies the collection objects to a one-dimensional <see cref="T:System.Array">Array</see> instance beginning at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array">Array</see> that is the destination of the values copied from the collection.</param>
        /// <param name="index">The index of the array at which to begin inserting.</param>
        public void CopyTo( SearchCriteria[] array, int index )
        {
            List.CopyTo( array, index );
        }

        /// <summary>
        /// Creates a one-dimensional <see cref="T:System.Array">Array</see> instance containing the collection items.
        /// </summary>
        /// <returns>Array of type SearchCriteria</returns>
        public SearchCriteria[] ToArray()
        {
            SearchCriteria[] arr = null;
            arr = (SearchCriteria[])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray( arr, new SearchCriteria[Count - 1 + 1] );
            CopyTo( arr, 0 );

            return arr;
        }
    }
}