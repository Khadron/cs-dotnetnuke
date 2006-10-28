using System;

namespace DotNetNuke.UI.WebControls
{
    [AttributeUsage( AttributeTargets.Property )]
    public sealed class RequiredAttribute : Attribute
    {
        private bool _Required;

        /// <Summary>
        /// Initializes a new instance of the RequiredAttribute class.
        /// </Summary>
        public RequiredAttribute( bool required )
        {
            this._Required = false;
            this._Required = required;
        }

        public bool Required
        {
            get
            {
                return this._Required;
            }
        }
    }
}