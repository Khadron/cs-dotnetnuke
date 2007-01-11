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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Modules.SearchResults
{
    /// <summary>
    /// The ResultsSettings ModuleSettingsBase is used to manage the
    /// settings for the Search Results Module
    /// </summary>
    /// <history>
    ///		[cnurse]	11/11/2004	created
    /// </history>
    public partial class ResultsSettings : ModuleSettingsBase
    {
        /// <summary>
        /// LoadSettings loads the settings from the Databas and displays them
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///		[cnurse]	11/11/2004	created
        /// </history>
        public override void LoadSettings()
        {
            try
            {
                if( Page.IsPostBack == false )
                {
                    if( Convert.ToString( TabModuleSettings["maxresults"] ) != "" )
                    {
                        txtresults.Text = Convert.ToString( TabModuleSettings["maxresults"] );
                    }
                    else
                    {
                        txtresults.Text = "";
                    }
                    if( Convert.ToString( TabModuleSettings["perpage"] ) != "" )
                    {
                        txtPage.Text = Convert.ToString( TabModuleSettings["perpage"] );
                    }
                    else
                    {
                        txtPage.Text = "";
                    }
                    if( Convert.ToString( TabModuleSettings["titlelength"] ) != "" )
                    {
                        txtTitle.Text = Convert.ToString( TabModuleSettings["titlelength"] );
                    }
                    else
                    {
                        txtTitle.Text = "";
                    }
                    if( Convert.ToString( TabModuleSettings["descriptionlength"] ) != "" )
                    {
                        txtdescription.Text = Convert.ToString( TabModuleSettings["descriptionlength"] );
                    }
                    else
                    {
                        txtdescription.Text = "";
                    }
                    chkDescription.Checked = false;
                    if( Convert.ToString( TabModuleSettings["showdescription"] ) != "" )
                    {
                        if( Convert.ToString( TabModuleSettings["showdescription"] ) == "Y" )
                        {
                            chkDescription.Checked = true;
                        }
                    }
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// UpdateSettings saves the modified settings to the Database
        /// </summary>
        /// <history>
        ///		[cnurse]	11/11/2004	created
        /// </history>
        public override void UpdateSettings()
        {
            try
            {
                if(Page.IsValid)
                {
                    ModuleController objModules = new ModuleController();

                    objModules.UpdateTabModuleSetting( TabModuleId, "maxresults", txtresults.Text );
                    objModules.UpdateTabModuleSetting( TabModuleId, "perpage", txtPage.Text );
                    objModules.UpdateTabModuleSetting( TabModuleId, "titlelength", txtTitle.Text );
                    objModules.UpdateTabModuleSetting( TabModuleId, "descriptionlength", txtdescription.Text );
                    objModules.UpdateTabModuleSetting( TabModuleId, "showdescription", Convert.ToString( chkDescription.Checked ? "Y" : "N" ) );
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }


    }
}