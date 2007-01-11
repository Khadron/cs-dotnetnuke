#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by DotNetNuke Corporation
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