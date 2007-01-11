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
using System.Diagnostics;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.UI.Skins.Controls
{
    /// <summary></summary>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class Logo : SkinObjectBase
    {
        // private members
        private string _borderWidth;

        // protected controls

        public string BorderWidth
        {
            get
            {
                return _borderWidth;
            }
            set
            {
                _borderWidth = value;
            }
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            
        }

        //*******************************************************
        //
        // The Page_Load server event handler on this page is used
        // to populate the role information for the page
        //
        //*******************************************************

        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                // public attributes
                if( BorderWidth != null ) if( !String.IsNullOrEmpty(BorderWidth) )
                {
                    imgLogo.BorderWidth = Unit.Parse( BorderWidth );
                }

                if( PortalSettings.LogoFile != null ) if( !String.IsNullOrEmpty(PortalSettings.LogoFile) )
                {
                    imgLogo.ImageUrl = PortalSettings.HomeDirectory + PortalSettings.LogoFile;
                }
                else
                {
                    imgLogo.Visible = false;
                }
                imgLogo.AlternateText = PortalSettings.PortalName;

                hypLogo.ToolTip = PortalSettings.PortalName;
                hypLogo.NavigateUrl = Globals.GetPortalDomainName( PortalSettings.PortalAlias.HTTPAlias, Request, true );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}