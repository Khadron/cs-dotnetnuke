using System;

namespace DotNetNuke.UI.WebControls
{
    [AttributeUsage( AttributeTargets.Property )]
    public sealed class IsReadOnlyAttribute : Attribute
    {
        private bool _IsReadOnly;

        /// <Summary>
        /// Initializes a new instance of the ReadOnlyAttribute class.
        /// </Summary>
        /// <Param name="read">
        /// A boolean that indicates whether the property is ReadOnly
        /// </Param>
        public IsReadOnlyAttribute( bool read )
        {
            this._IsReadOnly = false;
            this._IsReadOnly = read;
        }

        public bool IsReadOnly
        {
            get
            {
                return this._IsReadOnly;
            }
        }
    }
}