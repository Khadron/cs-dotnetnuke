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

namespace DotNetNuke.Services.Localization
{
    /// <Summary>Represents a collection of LocaleFileInfo objects.</Summary>
    // Warning: Custom Attribute Disabled --> [DefaultMemberAttribute("Item")]
    public class LocaleFileCollection : CollectionBase
    {

        public LocaleFileInfo this[ int index ]
        {
            get
            {
                return ( (LocaleFileInfo)this.List[index] );
            }
            set
            {
                this.List[index] = value;
            }
        }
        /// <Summary>
        /// Initializes a new instance of the LocaleFileCollection class.
        /// </Summary>
        public LocaleFileCollection()
        {
        }

        /// <Summary>
        /// Initializes a new instance of the LocaleFileCollection class containing the elements of the specified source collection.
        /// </Summary>
        /// <Param name="value">
        /// A LocaleFileCollection with which to initialize the collection.
        /// </Param>
        public LocaleFileCollection( LocaleFileCollection value )
        {
            this.AddRange( value );
        }

        /// <Summary>
        /// Initializes a new instance of the LocaleFileCollection class containing the specified array of LocaleFileInfo objects.
        /// </Summary>
        /// <Param name="value">
        /// An array of LocaleFileInfo objects with which to initialize the collection.
        /// </Param>
        public LocaleFileCollection( LocaleFileInfo[] value )
        {
            this.AddRange( value );
        }

        /// <Summary>
        /// Add an element of the specified LocaleFileInfo to the end of the collection.
        /// </Summary>
        /// <Param name="value">
        /// An object of type LocaleFileInfo to add to the collection.
        /// </Param>
        public int Add( LocaleFileInfo value )
        {
            return this.List.Add( value );
        }

        /// <Summary>
        /// Gets a value indicating whether the collection contains the specified LocaleFileCollection.
        /// </Summary>
        /// <Param name="value">
        /// The LocaleFileCollection to search for in the collection.
        /// </Param>
        /// <Returns>
        /// true if the collection contains the specified object; otherwise, false.
        /// </Returns>
        public bool Contains( LocaleFileInfo value )
        {
            return this.List.Contains( value );
        }

        private LocaleFileInfo GetLocaleFile( string FullFileName )
        {
            foreach (LocaleFileInfo LocaleFile in List)
            {
                if (LocaleFileUtil.GetFullFileName(LocaleFile).Replace("\\", "/") == FullFileName.Replace("\\", "/"))
                {
                    return LocaleFile;
                }
            }
            return null;
        }

        /// <Summary>
        /// Gets the index in the collection of the specified LocaleFileCollection, if it exists in the collection.
        /// </Summary>
        /// <Param name="value">
        /// The LocaleFileCollection to locate in the collection.
        /// </Param>
        /// <Returns>
        /// The index in the collection of the specified object, if found; otherwise, -1.
        /// </Returns>
        public int IndexOf( LocaleFileInfo value )
        {
            return this.List.IndexOf( value );
        }

        public LocaleFileInfo LocaleFile( string FullFileName )
        {
            return this.GetLocaleFile( FullFileName );
        }

        /// <Summary>
        /// Creates a one-dimensional Array instance containing the collection items.
        /// </Summary>
        /// <Returns>Array of type LocaleFileInfo</Returns>
        public LocaleFileInfo[] ToArray()
        {
            LocaleFileInfo[] arr = new LocaleFileInfo[( ( this.Count - 1 ) + 1 )];
            arr = ( (LocaleFileInfo[])Utils.CopyArray( ( (Array)arr ), ( (Array)new LocaleFileInfo[( ( this.Count - 1 ) + 1 )] ) ) );
            this.CopyTo( arr, 0 );
            return arr;
        }

        /// <Summary>
        /// Copies the elements of the specified LocaleFileInfo array to the end of the collection.
        /// </Summary>
        /// <Param name="value">
        /// An array of type LocaleFileInfo containing the objects to add to the collection.
        /// </Param>
        public void AddRange( LocaleFileInfo[] value )
        {                        
            for (int i = 0; i <value.Length - 1; i++)
            {
                this.Add( value[i] );
            }
        }

        /// <Summary>
        /// Adds the contents of another LocaleFileCollection to the end of the collection.
        /// </Summary>
        /// <Param name="value">
        /// A LocaleFileCollection containing the objects to add to the collection.
        /// </Param>
        public void AddRange( LocaleFileCollection value )
        {
            for (int i = 0; i <= value.Count - 1; i++)
            {
                Add((LocaleFileInfo)value.List[i]);
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
        public void CopyTo( LocaleFileInfo[] array, int index )
        {
            this.List.CopyTo( ( (Array)array ), index );
        }

        /// <Summary>
        /// Add an element of the specified LocaleFileInfo to the collection at the designated index.
        /// </Summary>
        /// <Param name="index">
        /// An Integer to indicate the location to add the object to the collection.
        /// </Param>
        /// <Param name="value">
        /// An object of type LocaleFileInfo to add to the collection.
        /// </Param>
        public void Insert( int index, LocaleFileInfo value )
        {
            this.List.Insert( index, value );
        }

        /// <Summary>
        /// Remove the specified object of type LocaleFileInfo from the collection.
        /// </Summary>
        /// <Param name="value">
        /// An object of type LocaleFileInfo to remove to the collection.
        /// </Param>
        public void Remove( LocaleFileInfo value )
        {
            this.List.Remove( value );
        }

        public void SetLocaleFile( string FullFileName, LocaleFileInfo value )
        {
            LocaleFileInfo LocaleFile = GetLocaleFile(FullFileName);
            if (LocaleFile != null)
            {
                LocaleFile = value;
            }
            else
            {
                throw (new ArgumentOutOfRangeException(FullFileName, FullFileName + " does not exist in this collection."));
            }
        }
    }
}