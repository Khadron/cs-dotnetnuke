using System;

namespace DotNetNuke.UI.WebControls
{
    [AttributeUsage( AttributeTargets.Property )]
    public sealed class LabelModeAttribute : Attribute
    {
        private LabelMode _Mode;

        /// <Summary>
        /// Initializes a new instance of the LabelModeAttribute class.
        /// </Summary>
        /// <Param name="mode">
        /// The label mode to apply to the associated property
        /// </Param>
        public LabelModeAttribute( LabelMode mode )
        {
            this._Mode = mode;
        }

        public LabelMode Mode
        {
            get
            {
                return this._Mode;
            }
        }
    }
}