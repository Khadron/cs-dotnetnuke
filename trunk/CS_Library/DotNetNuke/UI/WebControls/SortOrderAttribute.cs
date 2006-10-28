using System;

namespace DotNetNuke.UI.WebControls
{
    [AttributeUsage( AttributeTargets.Property )]
    public sealed class SortOrderAttribute : Attribute
    {
        private int _order;

        /// <Summary>
        /// Initializes a new instance of the SortOrderAttribute class.
        /// </Summary>
        public SortOrderAttribute( int order )
        {
            this._order = order;
        }

        public static int DefaultOrder
        {
            get
            {
                return int.MaxValue;
            }
        }

        public int Order
        {
            get
            {
                return this._order;
            }
            set
            {
                this._order = value;
            }
        }
    }
}