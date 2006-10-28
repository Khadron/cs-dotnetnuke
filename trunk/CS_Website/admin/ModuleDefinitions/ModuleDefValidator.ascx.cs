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

        
        private void InitializeComponent()
        {
        }

        protected void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
    }
}