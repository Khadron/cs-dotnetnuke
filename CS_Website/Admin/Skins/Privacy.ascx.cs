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
using DotNetNuke.Common;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.Skins.Controls
{
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed brackets from property names
    /// </history>
    public partial class Privacy : SkinObjectBase
    {
        // private members
        private string _text;
        private string _cssClass;

        private const string MyFileName = "Privacy.ascx";

        // protected controls

        public string Text
        {
            get
            {
                if( _text != null )
                {
                    return _text;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                _text = value;
            }
        }

        public string CssClass
        {
            get
            {
                if( _cssClass != null )
                {
                    return _cssClass;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                _cssClass = value;
            }
        }

        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                // public attributes
                if( !String.IsNullOrEmpty(CssClass) )
                {
                    hypPrivacy.CssClass = CssClass;
                }

                if( !String.IsNullOrEmpty(Text) )
                {
                    hypPrivacy.Text = Text;
                }
                else
                {
                    hypPrivacy.Text = Localization.GetString( "Privacy", Localization.GetResourceFile( this, MyFileName ) );
                }

                hypPrivacy.NavigateUrl = Globals.NavigateURL( "Privacy" );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}