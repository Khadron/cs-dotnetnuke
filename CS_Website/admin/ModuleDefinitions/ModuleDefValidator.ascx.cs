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
using System.Collections;
using System.IO;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Admin.ModuleDefinitions
{
    /// <summary>
    /// The ModuleDefValidator PortalModuleBase is used to validate user PAs before
    /// the are uploaded
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    public partial class ModuleDefValidator : PortalModuleBase
    {
        /// <summary>
        /// Page_Load runs when the control is loaded.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            //Put user code to initialize the page here
        }

        /// <summary>
        /// lnkValidate_Click runs when the Validate button is clicked
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void lnkValidate_Click( Object sender, EventArgs e )
        {
            if( Page.IsPostBack )
            {
                if( cmdBrowse.PostedFile.FileName != "" )
                {
                    string strExtension = Path.GetExtension( cmdBrowse.PostedFile.FileName );
                    ArrayList Messages = new ArrayList();
                    

                    string postedFile = Path.GetFileName( cmdBrowse.PostedFile.FileName );
                    if( strExtension.ToLower() == ".dnn" )
                    {
                        ModuleDefinitionValidator xval = new ModuleDefinitionValidator();
                        xval.Validate( cmdBrowse.PostedFile.InputStream );
                        if( xval.Errors.Count > 0 )
                        {
                            Messages.AddRange( xval.Errors );
                        }
                        else
                        {
                            Messages.Add( string.Format( Localization.GetString( "Valid", this.LocalResourceFile ), postedFile, null ) );
                        }
                    }
                    else
                    {
                        Messages.Add( string.Format( Localization.GetString( "Invalid", this.LocalResourceFile ), postedFile, null ) );
                    }
                    lstResults.Visible = true;
                    lstResults.DataSource = Messages;
                    lstResults.DataBind();
                }
            }
        }

        

    }
}