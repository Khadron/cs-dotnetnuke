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
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Modules.Admin.ResourceInstaller
{
    /// <Summary>Represents a collection of PaFolder objects.</Summary>
    // Warning: Custom Attribute Disabled --> [DefaultMemberAttribute("Item")]
    public class PaFolderCollection : CollectionBase
    {

        public PaFolder this[ int index ]
        {
            get
            {
                return ( (PaFolder)this.List[index] );
            }
            set
            {
                this.List[index] = value;
            }
        }
        /// <Summary>
        /// Initializes a new instance of the PaFolderCollection class.
        /// </Summary>
        public PaFolderCollection()
        {
        }

        /// <Summary>
        /// Initializes a new instance of the PaFolderCollection class containing the elements of the specified source collection.
        /// </Summary>
        /// <Param name="value">
        /// A PaFolderCollection with which to initialize the collection.
        /// </Param>
        public PaFolderCollection( PaFolderCollection value )
        {
            this.AddRange( value );
        }

        /// <Summary>
        /// Initializes a new instance of the PaFolderCollection class containing the specified array of PaFolder objects.
        /// </Summary>
        /// <Param name="value">
        /// An array of PaFolder objects with which to initialize the collection.
        /// </Param>
        public PaFolderCollection( PaFolder[] value )
        {
            this.AddRange( value );
        }

        /// <Summary>
        /// Add an element of the specified PaFolder to the end of the collection.
        /// </Summary>
        /// <Param name="value">
        /// An object of type PaFolder to add to the collection.
        /// </Param>
        public int Add( PaFolder value )
        {
            return this.List.Add( value );
        }

        /// <Summary>
        /// Gets a value indicating whether the collection contains the specified PaFolderCollection.
        /// </Summary>
        /// <Param name="value">
        /// The PaFolderCollection to search for in the collection.
        /// </Param>
        /// <Returns>
        /// true if the collection contains the specified object; otherwise, false.
        /// </Returns>
        public bool Contains( PaFolder value )
        {
            return this.List.Contains( value );
        }

        /// <Summary>
        /// Gets the index in the collection of the specified PaFolderCollection, if it exists in the collection.
        /// </Summary>
        /// <Param name="value">
        /// The PaFolderCollection to locate in the collection.
        /// </Param>
        /// <Returns>
        /// The index in the collection of the specified object, if found; otherwise, -1.
        /// </Returns>
        public int IndexOf( PaFolder value )
        {
            return this.List.IndexOf( value );
        }

        /// <Summary>
        /// Creates a one-dimensional Array instance containing the collection items.
        /// </Summary>
        /// <Returns>Array of type PaFolder</Returns>
        public PaFolder[] ToArray()
        {
            PaFolder[] paFolders = new PaFolder[( ( this.Count - 1 ) + 1 )];
            paFolders = ( (PaFolder[])Utils.CopyArray( paFolders, new PaFolder[( ( this.Count - 1 ) + 1 )] ) );
            this.CopyTo( paFolders, 0 );
            return paFolders;
        }

        /// <Summary>
        /// Adds the contents of another PaFolderCollection to the end of the collection.
        /// </Summary>
        /// <Param name="value">
        /// A PaFolderCollection containing the objects to add to the collection.
        /// </Param>
        public void AddRange( PaFolderCollection value )
        {
            int total = ( value.Count - 1 );
            for( int i = 0; ( i <= total ); i++ )
            {
                this.Add( ( (PaFolder)value.List[i] ) );
            }
        }

        /// <Summary>
        /// Copies the elements of the specified PaFolder array to the end of the collection.
        /// </Summary>
        /// <Param name="value">
        /// An array of type PaFolder containing the objects to add to the collection.
        /// </Param>
        public void AddRange( PaFolder[] value )
        {
            int total = ( value.Length - 1 );
            for( int i = 0; ( i <= total ); i++ )
            {
                this.Add( value[i] );
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
        public void CopyTo( PaFolder[] array, int index )
        {
            this.List.CopyTo( ( (Array)array ), index );
        }

        /// <Summary>
        /// Add an element of the specified PaFolder to the collection at the designated index.
        /// </Summary>
        /// <Param name="index">
        /// An Integer to indicate the location to add the object to the collection.
        /// </Param>
        /// <Param name="value">
        /// An object of type PaFolder to add to the collection.
        /// </Param>
        public void Insert( int index, PaFolder value )
        {
            this.List.Insert( index, value );
        }

        /// <Summary>
        /// Remove the specified object of type PaFolder from the collection.
        /// </Summary>
        /// <Param name="value">
        /// An object of type PaFolder to remove to the collection.
        /// </Param>
        public void Remove( PaFolder value )
        {
            this.List.Remove( value );
        }
    }
}