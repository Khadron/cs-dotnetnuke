using System;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.UI.WebControls
{
    [AttributeUsage( AttributeTargets.Property )]
    public sealed class MaxLengthAttribute : Attribute
    {
        private int _Length;

        /// <Summary>
        /// Initializes a new instance of the RequiredAttribute class.
        /// </Summary>
        public MaxLengthAttribute( int length )
        {
            this._Length = Null.NullInteger;
            this._Length = length;
        }

        public int Length
        {
            get
            {
                return this._Length;
            }
        }
    }
}