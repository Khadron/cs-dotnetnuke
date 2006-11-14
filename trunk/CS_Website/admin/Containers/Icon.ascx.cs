#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Skins;

namespace DotNetNuke.UI.Containers
{
    /// Project	 : DotNetNuke
    /// Class	 : DotNetNuke.UI.Containers.Icon
    ///
    /// <summary>
    /// Contains the attributes of an Icon.
    /// These are read into the PortalModuleBase collection as attributes for the icons within the module controls.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[sun1]	    2/1/2004	Created
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    public partial class Icon : SkinObjectBase
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


        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                // public attributes
                if( BorderWidth != "" )
                {
                    imgIcon.BorderWidth = Unit.Parse( BorderWidth );
                }

                this.Visible = false;
                PortalModuleBase objPortalModule = Container.GetPortalModuleBase( this );
                if( ( objPortalModule != null ) && ( objPortalModule.ModuleConfiguration != null ) )
                {
                    if( objPortalModule.ModuleConfiguration.IconFile != "" )
                    {
                        if( objPortalModule.ModuleConfiguration.IconFile.StartsWith( "~/" ) )
                        {
                            imgIcon.ImageUrl = objPortalModule.ModuleConfiguration.IconFile;
                        }
                        else
                        {
                            if( Globals.IsAdminControl() )
                            {
                                imgIcon.ImageUrl = objPortalModule.TemplateSourceDirectory + "/" + objPortalModule.ModuleConfiguration.IconFile;
                            }
                            else
                            {
                                if( objPortalModule.PortalSettings.ActiveTab.IsAdminTab )
                                {
                                    imgIcon.ImageUrl = "~/images/" + objPortalModule.ModuleConfiguration.IconFile;
                                }
                                else
                                {
                                    imgIcon.ImageUrl = objPortalModule.PortalSettings.HomeDirectory + objPortalModule.ModuleConfiguration.IconFile;
                                }
                            }
                        }
                        imgIcon.AlternateText = objPortalModule.ModuleConfiguration.ModuleTitle;
                        this.Visible = true;
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}