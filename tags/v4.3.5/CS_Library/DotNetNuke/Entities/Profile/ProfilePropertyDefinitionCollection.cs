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
using System.Collections;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.Entities.Profile
{
    /// <Summary>
    /// The ProfilePropertyDefinitionCollection class provides Business Layer methods for
    /// a collection of property Definitions
    /// </Summary>
    public class ProfilePropertyDefinitionCollection : CollectionBase
    {
        /// <Summary>Constructs a new default collection</Summary>
        public ProfilePropertyDefinitionCollection()
        {
        }

        /// <Summary>
        /// Constructs a new Collection from an ArrayList of ProfilePropertyDefinition objects
        /// </Summary>
        /// <Param name="definitionsList">
        /// An ArrayList of ProfilePropertyDefinition objects
        /// </Param>
        public ProfilePropertyDefinitionCollection( ArrayList definitionsList )
        {
            this.AddRange( definitionsList );
        }

        /// <Summary>
        /// Constructs a new Collection from a ProfilePropertyDefinitionCollection
        /// </Summary>
        /// <Param name="collection">A ProfilePropertyDefinitionCollection</Param>
        public ProfilePropertyDefinitionCollection( ProfilePropertyDefinitionCollection collection )
        {
            this.AddRange( collection );
        }

        public ProfilePropertyDefinition this[ string name ]
        {
            get
            {
                return this.GetByName( name );
            }
        }

        public ProfilePropertyDefinition this[ int index ]
        {
            get
            {
                return ( (ProfilePropertyDefinition)this.List[index] );
            }
            set
            {
                this.List[index] = value;
            }
        }

        /// <Summary>Adds a property Definition to the collectio.</Summary>
        /// <Param name="value">A ProfilePropertyDefinition object</Param>
        /// <Returns>The index of the property Definition in the collection</Returns>
        public int Add( ProfilePropertyDefinition value )
        {
            return this.List.Add( value );
        }

        /// <Summary>Add an ArrayList of ProfilePropertyDefinition objects</Summary>
        /// <Param name="definitionsList">
        /// An ArrayList of ProfilePropertyDefinition objects
        /// </Param>
        public void AddRange( ArrayList definitionsList )
        {
            foreach( ProfilePropertyDefinition propertyDefinition in definitionsList )
            {
                this.Add( propertyDefinition );
            }
        }

        /// <Summary>Add an existing ProfilePropertyDefinitionCollection</Summary>
        /// <Param name="collection">A ProfilePropertyDefinitionCollection</Param>
        public void AddRange( ProfilePropertyDefinitionCollection collection )
        {
            foreach( ProfilePropertyDefinition propertyDefinition in collection )
            {
                this.Add( propertyDefinition );
            }
        }

        /// <Summary>
        /// Determines whether the collection contains a property definition
        /// </Summary>
        /// <Param name="value">A ProfilePropertyDefinition object</Param>
        /// <Returns>A Boolean True/False</Returns>
        public bool Contains( ProfilePropertyDefinition value )
        {
            return this.List.Contains( value );
        }

        /// <Summary>
        /// Gets a sub-collection of items in the collection by category.
        /// </Summary>
        /// <Param name="category">The category to get</Param>
        /// <Returns>A ProfilePropertyDefinitionCollection object</Returns>
        public ProfilePropertyDefinitionCollection GetByCategory( string category )
        {
            ProfilePropertyDefinitionCollection collection = new ProfilePropertyDefinitionCollection();
            foreach( ProfilePropertyDefinition definition in this.InnerList )
            {
                if( Operators.CompareString( definition.PropertyCategory, category, false ) == 0 )
                {
                    collection.Add( definition );
                }
            }
            return collection;
        }

        /// <Summary>Gets an item in the collection by Id.</Summary>
        /// <Param name="id">The id of the Property to get</Param>
        /// <Returns>A ProfilePropertyDefinition object</Returns>
        public ProfilePropertyDefinition GetById( int id )
        {
            ProfilePropertyDefinition definition = ( (ProfilePropertyDefinition)null );
            foreach( ProfilePropertyDefinition propertyDefinition in this.InnerList )
            {
                if( propertyDefinition.PropertyDefinitionId == id )
                {
                    definition = propertyDefinition;
                }
            }
            return definition;
        }

        /// <Summary>Gets an item in the collection by name.</Summary>
        /// <Param name="name">The name of the Property to get</Param>
        /// <Returns>A ProfilePropertyDefinition object</Returns>
        public ProfilePropertyDefinition GetByName( string name )
        {
            ProfilePropertyDefinition definition = null;
            foreach( ProfilePropertyDefinition propertyDefinition in this.InnerList )
            {
                if( Operators.CompareString( propertyDefinition.PropertyName, name, false ) == 0 )
                {
                    definition = propertyDefinition;
                }
            }
            return definition;
        }

        /// <Summary>Gets the index of a property Definition</Summary>
        /// <Param name="value">A ProfilePropertyDefinition object</Param>
        /// <Returns>The index of the property Definition in the collection</Returns>
        public int IndexOf( ProfilePropertyDefinition value )
        {
            return this.List.IndexOf( value );
        }

        /// <Summary>Inserts a property Definition into the collectio.</Summary>
        /// <Param name="index">The index to insert the item at</Param>
        /// <Param name="value">A ProfilePropertyDefinition object</Param>
        public void Insert( int index, ProfilePropertyDefinition value )
        {
            this.List.Insert( index, value );
        }

        /// <Summary>Removes a property definition from the collection</Summary>
        /// <Param name="value">The ProfilePropertyDefinition object to remove</Param>
        public void Remove( ProfilePropertyDefinition value )
        {
            this.List.Remove( value );
        }

        /// <Summary>
        /// Sorts the collection using the ProfilePropertyDefinitionComparer (ie by ViewOrder)
        /// </Summary>
        public void Sort()
        {
            this.InnerList.Sort( new ProfilePropertyDefinitionComparer() );
        }
    }
}