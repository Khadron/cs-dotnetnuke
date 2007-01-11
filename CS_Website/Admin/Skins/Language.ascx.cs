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
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.UI.Skins.Controls
{
    public partial class Language : SkinObjectBase
    {
        // private members
        private string _cssClass;

        // protected controls

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

        //*******************************************************
        //
        // The Page_Load server event handler on this page is used
        // to populate the role information for the page
        //
        //*******************************************************

        protected void Page_Load( Object sender, EventArgs e )
        {
            if( ! Page.IsPostBack )
            {
                // public attributes
                if( !String.IsNullOrEmpty(CssClass) )
                {
                    selectCulture.CssClass = CssClass;
                }

                Localization.LoadCultureDropDownList( selectCulture, CultureDropDownTypes.NativeName, ( (PageBase)Page ).PageCulture.Name );

                //only show language selector if more than one language
                if( selectCulture.Items.Count <= 1 )
                {
                    selectCulture.Visible = false;
                }
            }
        }

        protected void selectCulture_SelectedIndexChanged( object sender, EventArgs e )
        {
            // Store selected language in cookie
            Localization.SetLanguage( selectCulture.SelectedItem.Value );

            //Redirect to same page to update all controls for newly selected culture
            Response.Redirect( Request.RawUrl, true );
        }
    }
}