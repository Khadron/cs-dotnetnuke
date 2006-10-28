using System;
using System.Web.UI.WebControls;

namespace DotNetNuke.UI.WebControls
{
    [AttributeUsage( AttributeTargets.Property )]
    public sealed class ControlStyleAttribute : Attribute
    {
        private string _CssClass;
        private Unit _Height;
        private Unit _Width;

        /// <Summary>Initializes a new instance of the StyleAttribute class.</Summary>
        /// <Param name="cssClass">
        /// The css class to apply to the associated property
        /// </Param>
        public ControlStyleAttribute( string cssClass )
        {
            this._CssClass = cssClass;
        }

        /// <Summary>Initializes a new instance of the StyleAttribute class.</Summary>
        /// <Param name="cssClass">
        /// The css class to apply to the associated property
        /// </Param>
        public ControlStyleAttribute( string cssClass, string width )
        {
            this._CssClass = cssClass;
            this._Width = Unit.Parse( width );
        }

        /// <Summary>Initializes a new instance of the StyleAttribute class.</Summary>
        /// <Param name="cssClass">
        /// The css class to apply to the associated property
        /// </Param>
        public ControlStyleAttribute( string cssClass, string width, string height )
        {
            this._CssClass = cssClass;
            this._Height = Unit.Parse( height );
            this._Width = Unit.Parse( width );
        }

        public string CssClass
        {
            get
            {
                return this._CssClass;
            }
        }

        public Unit Height
        {
            get
            {
                return this._Height;
            }
        }

        public Unit Width
        {
            get
            {
                return this._Width;
            }
        }
    }
}