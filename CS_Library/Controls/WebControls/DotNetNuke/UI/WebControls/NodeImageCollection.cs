using System;
using System.Collections;
using System.Web.UI;
using Microsoft.VisualBasic.CompilerServices;

namespace DotNetNuke.UI.WebControls
{
    /// <summary>
    /// Represents a collection of <see cref="NodeImage">NodeImage</see> objects.
    /// </summary>
    public class NodeImageCollection : CollectionBase, IStateManager
    {
        protected bool m_isTrackingViewState;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeImageCollection">NodeImageCollection</see> class.
        /// </summary>
        public NodeImageCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeImageCollection">NodeImageCollection</see> class containing the elements of the specified source collection.
        /// </summary>
        /// <param name="value">A <see cref="NodeImageCollection">NodeImageCollection</see> with which to initialize the collection.</param>
        public NodeImageCollection( NodeImageCollection value )
        {
            AddRange( value );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeImageCollection">NodeImageCollection</see> class containing the specified array of <see cref="NodeImage">NodeImage</see> objects.
        /// </summary>
        /// <param name="value">An array of <see cref="NodeImage">NodeImage</see> objects with which to initialize the collection. </param>
        public NodeImageCollection( NodeImage[] value )
        {
            AddRange( value );
        }

        /// <summary>
        /// Gets the <see cref="NodeImageCollection">NodeImageCollection</see> at the specified index in the collection.
        /// <para>
        /// In VB.Net, this property is the indexer for the <see cref="NodeImageCollection">NodeImageCollection</see> class.
        /// </para>
        /// </summary>
        public NodeImage this[ int index ]
        {
            get
            {
                return ( (NodeImage)List[index] );
            }
            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        /// Add an element of the specified <see cref="NodeImage">NodeImage</see> to the end of the collection.
        /// </summary>
        /// <param name="value">An object of type <see cref="NodeImage">NodeImage</see> to add to the collection.</param>
        public int Add( NodeImage value )
        {
            int index = List.Add( value );
            if( m_isTrackingViewState )
            {
                value.TrackViewState();
                value.SetDirty();
            }
            return index;
        } //Add

        /// <summary>
        /// Add an element of the specified <see cref="NodeImage">NodeImage</see> to the end of the collection.
        /// </summary>
        /// <param name="ImageUrl">An object of type <see cref="NodeImage">NodeImage</see> to add to the collection.</param>
        public int Add( string ImageUrl )
        {
            return Add( new NodeImage( ImageUrl ) );
        } //Add

        /// <summary>
        /// Copies the elements of the specified <see cref="NodeImage">NodeImage</see> array to the end of the collection.
        /// </summary>
        /// <param name="value">An array of type <see cref="NodeImage">NodeImage</see> containing the objects to add to the collection.</param>
        public void AddRange( NodeImage[] value )
        {
            for( int i = 0; i <= value.Length - 1; i++ )
            {
                Add( value[i] );
            }
        }

        /// <summary>
        /// Adds the contents of another <see cref="NodeImageCollection">NodeImageCollection</see> to the end of the collection.
        /// </summary>
        /// <param name="value">A <see cref="NodeImageCollection">NodeImageCollection</see> containing the objects to add to the collection. </param>
        public void AddRange( NodeImageCollection value )
        {
            for( int i = 0; i <= value.Count - 1; i++ )
            {
                Add( (NodeImage)value.List[i] );
            }
        }

        /// <summary>
        /// Gets the index in the collection of the specified <see cref="NodeImageCollection">NodeImageCollection</see>, if it exists in the collection.
        /// </summary>
        /// <param name="value">The <see cref="NodeImageCollection">NodeImageCollection</see> to locate in the collection.</param>
        /// <returns>The index in the collection of the specified object, if found; otherwise, -1.</returns>
        public int IndexOf( NodeImage value )
        {
            return GetImageIndex( value.ImageUrl );
            //Return List.IndexOf(value)
        } //IndexOf

        public int IndexOf( string strImageUrl )
        {
            return GetImageIndex( strImageUrl );
            //Return List.IndexOf(value)
        } //IndexOf

        /// <summary>
        /// Gets a value indicating whether the collection contains the specified <see cref="NodeImageCollection">NodeImageCollection</see>.
        /// </summary>
        /// <param name="value">The <see cref="NodeImageCollection">NodeImageCollection</see> to search for in the collection.</param>
        /// <returns><b>true</b> if the collection contains the specified object; otherwise, <b>false</b>.</returns>
        public bool Contains( NodeImage value )
        {
            // If value is not of type NodeImage, this will return false.
            return !Convert.ToBoolean( GetImage( value.ImageUrl ) == null );
            //Return List.Contains(value)
        } //Contains

        public bool Contains( string strImageUrl )
        {
            // If value is not of type NodeImage, this will return false.
            return !Convert.ToBoolean( GetImage( strImageUrl ) == null );
        } //Contains

        private NodeImage GetImage( string strUrl )
        {
            NodeImage oImg;
            foreach( NodeImage tempLoopVar_oImg in List )
            {
                oImg = tempLoopVar_oImg;
                if( strUrl == oImg.ImageUrl )
                {
                    return oImg;
                }
            }
            return null;
        }

        private int GetImageIndex( string strUrl )
        {
            int i;
            for( i = 0; i <= List.Count - 1; i++ )
            {
                if( strUrl == ( (NodeImage)List[i] ).ImageUrl )
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Add an element of the specified <see cref="NodeImage">NodeImage</see> to the collection at the designated index.
        /// </summary>
        /// <param name="index">An <see cref="system.int32">Integer</see> to indicate the location to add the object to the collection.</param>
        /// <param name="value">An object of type <see cref="NodeImage">NodeImage</see> to add to the collection.</param>
        public void Insert( int index, NodeImage value )
        {
            List.Insert( index, value );
        } //Insert

        /// <summary>
        /// Remove the specified object of type <see cref="NodeImage">NodeImage</see> from the collection.
        /// </summary>
        /// <param name="value">An object of type <see cref="NodeImage">NodeImage</see> to remove to the collection.</param>
        public void Remove( NodeImage value )
        {
            List.Remove( value );
        } //Remove

        /// <summary>
        /// Copies the collection objects to a one-dimensional <see cref="T:System.Array">Array</see> instance beginning at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array">Array</see> that is the destination of the values copied from the collection.</param>
        /// <param name="index">The index of the array at which to begin inserting.</param>
        public void CopyTo( NodeImage[] array, int index )
        {
            List.CopyTo( array, index );
        }

        /// <summary>
        /// Creates a one-dimensional <see cref="T:System.Array">Array</see> instance containing the collection items.
        /// </summary>
        /// <returns>Array of type NodeImage</returns>
        public NodeImage[] ToArray()
        {
            NodeImage[] arr = null;
            arr = (NodeImage[])Utils.CopyArray( arr, new NodeImage[Count - 1 + 1] );
            CopyTo( arr, 0 );

            return arr;
        }

        /// <summary>
        /// (IStateManager.IsTrackingViewState)
        /// Gets a value indicating whether the NodeImageCollection is tracking its view state changes.
        /// </summary>
        public bool IsTrackingViewState
        {
            get
            {
                return m_isTrackingViewState;
            }
        }

        /// <summary>
        /// (IStateManager.TrackViewState)
        /// Instructs the NodeImageCollection to track changes to its view state.
        /// </summary>
        public void TrackViewState()
        {
            m_isTrackingViewState = true;
        }

        /// <summary>
        /// (IStateManager.SaveViewState)
        /// Saves the changes of NodeImageCollection's view state to an Object.
        /// </summary>
        public object SaveViewState()
        {
            if( Count == 0 )
            {
                return null;
            }
            object[] nodeState = new object[Count + 1];

            int index;
            for( index = 0; index <= Count - 1; index++ )
            {
                nodeState[index] = this[index].SaveViewState();
            }
            return nodeState;
        }

        /// <summary>
        /// (IStateManager.LoadViewState)
        /// Loads the NodeImageCollection's previously saved view state.
        /// </summary>
        public void LoadViewState( object state )
        {
            if( !( state == null ) )
            {
                object[] nodeState = (object[])state;
                int index;
                for( index = 0; index <= nodeState.Length - 1; index++ )
                {
                    NodeImage _nodeImage = new NodeImage();
                    Add( _nodeImage );

                    _nodeImage.TrackViewState();
                    _nodeImage.LoadViewState( nodeState[index] );
                }
            }
        }
    }
}