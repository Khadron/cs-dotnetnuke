using System;

namespace DotNetNuke.UI.WebControls
{
    [AttributeUsage( AttributeTargets.Property )]
    public sealed class RegularExpressionValidatorAttribute : Attribute
    {
        private string _Expression;

        /// <Summary>
        /// Initializes a new instance of the RegularExpressionValidatorAttribute class.
        /// </Summary>
        public RegularExpressionValidatorAttribute( string expression )
        {
            this._Expression = expression;
        }

        public string Expression
        {
            get
            {
                return this._Expression;
            }
        }
    }
}