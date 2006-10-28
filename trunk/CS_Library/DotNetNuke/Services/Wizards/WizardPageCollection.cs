using System.Collections;
using System.Web.UI;

namespace DotNetNuke.Services.Wizards
{
    /// <Summary>Represents a collection of  objects.</Summary>
    public class WizardPageCollection : CollectionBase
    {
        /// <Summary>Initializes a new, empty instance of the  class.</Summary>
        public WizardPageCollection()
        {
        }

        /// <Summary>
        /// Initializes a new instance of the
        /// class containing the specified array of  objects.
        /// </Summary>
        /// <Param name="value">
        /// An array of  objects with which to initialize the collection.
        /// </Param>
        public WizardPageCollection( WizardPage[] value )
        {
            this.AddRange( value );
        }

        /// <Summary>
        /// Add an element of the specified  to the end of the collection.
        /// </Summary>
        /// <Param name="value">An object of type  to add to the collection.</Param>
        /// <Returns>The index of the newly added</Returns>
        public int Add( WizardPage value )
        {
            return this.List.Add( value );
        }

        /// <Summary>
        /// Add an element of the specified  to the end of the collection.
        /// </Summary>
        /// <Param name="title">
        /// This is the title that will be displayed for this action
        /// </Param>
        /// <Param name="icon">This is the identifier to use for this action.</Param>
        /// <Param name="ctl">The Control Assocaited with the Page</Param>
        /// <Param name="help">The Help text for the  Page</Param>
        /// <Returns>The index of the newly added</Returns>
        public WizardPage Add( string title, string icon, Control ctl, string help )
        {
            WizardPage page = new WizardPage(title, icon, ctl, help);
            this.Add(page);
            return page;
        }

        /// <Summary>
        /// Gets a value indicating whether the collection contains the specified .
        /// </Summary>
        /// <Param name="value">The  to search for in the collection.</Param>
        /// <Returns>
        /// true if the collection contains the specified object; otherwise, false.
        /// </Returns>
        public bool Contains( WizardPage value )
        {
            // If value is not of type WizardPage, this will return false.
            return List.Contains(value);
        }

        /// <Summary>
        /// Gets the index in the collection of the specified ,
        /// if it exists in the collection.
        /// </Summary>
        /// <Param name="value">The  to locate in the collection.</Param>
        /// <Returns>
        /// The index in the collection of the specified object, if found; otherwise, -1.
        /// </Returns>
        public int IndexOf( WizardPage value )
        {
            return this.List.IndexOf( value );
        }

        /// <Summary>
        /// Copies the elements of the specified array to the end of the collection.
        /// </Summary>
        /// <Param name="value">
        /// An array of type containing the objects to add to the collection.
        /// </Param>
        public void AddRange( WizardPage[] value )
        {
            int i;
            for (i = 0; i <= value.Length - 1; i++)
            {
                Add(value[i]);
            }
        }

        /// <Summary>
        /// Add an element of the specified  to the collection at the designated index.
        /// </Summary>
        /// <Param name="index">
        /// An Integer to indicate the location to add the object to the collection.
        /// </Param>
        /// <Param name="value">An object of type  to add to the collection.</Param>
        public void Insert( int index, WizardPage value )
        {
            this.List.Insert( index, value );
        }

        /// <Summary>Remove the specified object of type  from the collection.</Summary>
        /// <Param name="value">
        /// An object of type  to remove from the collection.
        /// </Param>
        public void Remove( WizardPage value )
        {
            this.List.Remove( value );
        }

        public WizardPage this[ int index ]
        {
            get
            {
                return ( (WizardPage)this.List[index] );
            }
            set
            {
                this.List[index] = value;
            }
        }
    }
}