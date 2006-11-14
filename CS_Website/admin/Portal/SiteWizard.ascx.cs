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
using System.IO;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Wizards;
using DotNetNuke.UI.Skins;

namespace DotNetNuke.Modules.Admin.PortalManagement
{
    /// <summary>
    /// The SiteWizard Wizard is a user-friendly Wizard that leads the user through the
    ///	process of setting up a new site
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	10/8/2004	created
    /// </history>
    public partial class SiteWizard : Wizard
    {
        public enum ContainerType
        {
            Host = 0,
            Portal = 1,
            Folder = 2,
            All = 3
        }

        //Wizard Framework Controls

        //Page 1 Controls

        //Page 2 Controls

        //Page 3 Controls

        //Page 4 Controls

        //Page 5 Controls

        /// <summary>
        /// BindContainers manages the containers
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	12/15/2004	created
        /// </history>
        private void BindContainers()
        {
            ctlPortalContainer.Clear();

            if( chkIncludeAll.Checked )
            {
                GetContainers( ContainerType.All, "", "" );
            }
            else
            {
                if( ctlPortalSkin.SkinSrc != "" )
                {
                    string strFolder;
                    string strContainerFolder = ctlPortalSkin.SkinSrc.Substring( 0, ctlPortalSkin.SkinSrc.LastIndexOf( "/" ) );
                    if( strContainerFolder.StartsWith( "[G]" ) )
                    {
                        strContainerFolder = strContainerFolder.Replace( "[G]Skins/", "Containers\\" );
                        strFolder = Globals.HostMapPath + strContainerFolder;
                        GetContainers( ContainerType.Folder, "[G]", strFolder );
                    }
                    else
                    {
                        strContainerFolder = strContainerFolder.Replace( "[L]Skins/", "Containers\\" );
                        strFolder = PortalSettings.HomeDirectoryMapPath + strContainerFolder;
                        GetContainers( ContainerType.Folder, "[L]", strFolder );
                    }
                }
                else
                {
                    GetContainers( ContainerType.Portal, "", "" );
                }
            }
        }

        /// <summary>
        /// GetContainers gets the containers and binds the lists to the controls
        ///	the buttons
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="type">An enum indicating what type of containers to load</param>
        /// <param name="skinType">A string that identifies whether the skin is Host "[G]" or Site "[L]"</param>
        /// <param name="strFolder">The folder to search for skins</param>
        /// <history>
        /// 	[cnurse]	12/14/2004	created
        /// </history>
        private void GetContainers( ContainerType type, string skinType, string strFolder )
        {
            SkinController objSkins = new SkinController();
            SkinInfo objSkin;

            //Configure SkinControl
            ctlPortalContainer.Width = "500px";
            ctlPortalContainer.Height = "250px";
            ctlPortalContainer.Border = "black 1px solid";
            ctlPortalContainer.Columns = 3;
            ctlPortalContainer.SkinRoot = SkinInfo.RootContainer;
            switch( type )
            {
                case ContainerType.Folder:

                    ctlPortalContainer.LoadSkins( strFolder, skinType, false );
                    break;
                case ContainerType.Portal:

                    ctlPortalContainer.LoadPortalSkins( false );
                    break;
                case ContainerType.Host:

                    ctlPortalContainer.LoadHostSkins( false );
                    break;
                case ContainerType.All:

                    ctlPortalContainer.LoadAllSkins( false );
                    break;
            }

            //Get current container and set selected skin
            objSkin = SkinController.GetSkin( SkinInfo.RootContainer, PortalId, SkinType.Portal );
            if( objSkin != null )
            {
                if( objSkin.PortalId == PortalId )
                {
                    ctlPortalContainer.SkinSrc = objSkin.SkinSrc;
                }
            }
        }

        /// <summary>
        /// GetSkins gets the skins and containers and binds the lists to the controls
        ///	the buttons
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/04/2004	created
        /// </history>
        private void GetSkins()
        {
            SkinController objSkins = new SkinController();
            SkinInfo objSkin;

            //Configure SkinControl
            ctlPortalSkin.Width = "500px";
            ctlPortalSkin.Height = "250px";
            ctlPortalSkin.Border = "black 1px solid";
            ctlPortalSkin.Columns = 3;
            ctlPortalSkin.SkinRoot = SkinInfo.RootSkin;
            ctlPortalSkin.LoadAllSkins( false );

            //Get current skin and set selected skin
            objSkin = SkinController.GetSkin( SkinInfo.RootSkin, PortalId, SkinType.Portal );
            if( objSkin != null )
            {
                if( objSkin.PortalId == PortalId )
                {
                    ctlPortalSkin.SkinSrc = objSkin.SkinSrc;
                }
            }
        }

        /// <summary>
        /// GetTemplates gets the skins and containers and binds the lists to the control
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/04/2004	created
        /// </history>
        private void GetTemplates()
        {
            string strFolder;
            string strFileName;

            strFolder = Globals.HostMapPath;
            if( Directory.Exists( strFolder ) )
            {
                // admin.template and a portal template are required at minimum
                string[] fileEntries = Directory.GetFiles( strFolder, "*.template" );
                this.EnableCommand( WizardCommand.NextPage, false );
                this.EnableCommand( WizardCommand.Finish, false );

                foreach( string tempLoopVar_strFileName in fileEntries )
                {
                    strFileName = tempLoopVar_strFileName;
                    if( Path.GetFileNameWithoutExtension( strFileName ) == "admin" )
                    {
                        this.EnableCommand( WizardCommand.NextPage, true );
                        this.EnableCommand( WizardCommand.Finish, true );
                    }
                    else
                    {
                        lstTemplate.Items.Add( Path.GetFileNameWithoutExtension( strFileName ) );
                    }
                }

                if( lstTemplate.Items.Count == 0 )
                {
                    this.EnableCommand( WizardCommand.NextPage, false );
                    this.EnableCommand( WizardCommand.Finish, false );
                }
            }
        }

        /// <summary>
        /// UseTemplate sets the page ready to select a Template
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/04/2004	created
        /// </history>
        private void UseTemplate()
        {
            lstTemplate.Enabled = chkTemplate.Checked;
            optMerge.Enabled = chkTemplate.Checked;
            lblMergeTitle.Enabled = chkTemplate.Checked;
            lblMergeWarning.Enabled = chkTemplate.Checked;
            lblTemplateMessage.Text = "";
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/11/2004	created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                this.AddPage( Localization.GetString( "ChooseTemplate.Title", this.LocalResourceFile ), "~/images/icon_portals_40px.gif", pnlTemplate, Localization.GetString( "Template.Help", this.LocalResourceFile ) );
                this.AddPage( Localization.GetString( "ChooseSkin.Title", this.LocalResourceFile ), "~/images/icon_skins_36px.gif", pnlSkin, Localization.GetString( "Skin.Help", this.LocalResourceFile ) );
                this.AddPage( Localization.GetString( "ChooseContainer.Title", this.LocalResourceFile ), "~/images/icon_skins_36px.gif", pnlContainer, Localization.GetString( "Container.Help", this.LocalResourceFile ) );
                this.AddPage( Localization.GetString( "Details.Title", this.LocalResourceFile ), "~/images/icon_sitesettings_36px.gif", pnlDetails, Localization.GetString( "Details.Help", this.LocalResourceFile ) );
                this.AddPage( Localization.GetString( "Logo.Title", this.LocalResourceFile ), "~/images/icon_sitesettings_36px.gif", pnlLogo, Localization.GetString( "Logo.Help", this.LocalResourceFile ) );

                if( ! Page.IsPostBack )
                {
                    //Get Templates for Page 1
                    GetTemplates();
                    chkTemplate.Checked = false;
                    lstTemplate.Enabled = false;

                    //Get Skins for Pages 2
                    GetSkins();

                    //Get Details for Page 4
                    PortalController objPortalController = new PortalController();
                    PortalInfo objPortal = objPortalController.GetPortal( PortalId );
                    txtPortalName.Text = objPortal.PortalName;
                    txtDescription.Text = objPortal.Description;
                    txtKeyWords.Text = objPortal.KeyWords;

                    //Get Details for Page 5
                    urlLogo.Url = objPortal.LogoFile;
                    urlLogo.FileFilter = Globals.glbImageFileTypes;

                    this.CurrentPage = 0;
                    this.FinishPage = 2;

                    this.DisplayCurrentPage();

                    UseTemplate();
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// Page_AfterPageChanged runs when a Wizard page has already been changed
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/12/2004	created
        /// </history>
        protected void Page_AfterPageChanged( object sender, WizardEventArgs we )
        {
            switch( we.PageNo )
            {
                case 2: //Page 3 (Containers)

                    BindContainers();
                    break;
            }
        }

        /// <summary>
        /// Page_BeforePageChanged runs when a Wizard page is about to be changed.  It provides
        ///	a mechanism for cancelling the page change if certain conditions aren't met.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/12/2004	created
        /// </history>
        protected void Page_BeforePageChanged( object sender, WizardCancelEventArgs we )
        {
            string strMessage;

            switch( we.PageNo )
            {
                case 0: //Page 1

                    //Before we leave Page 1, the user must have selected a Portal
                    if( lstTemplate.SelectedIndex == - 1 )
                    {
                        if( chkTemplate.Checked )
                        {
                            we.Cancel = true;
                            lblTemplateMessage.Text = Localization.GetString( "TemplateRequired", this.LocalResourceFile );
                        }
                    }
                    else
                    {
                        //Check Template Validity before proceeding
                        string schemaFilename = Server.MapPath( "admin/Portal/portal.template.xsd" );
                        string xmlFilename = Globals.HostMapPath + lstTemplate.SelectedItem.Text + ".template";
                        PortalTemplateValidator xval = new PortalTemplateValidator();
                        if( ! xval.Validate( xmlFilename, schemaFilename ) )
                        {
                            strMessage = Localization.GetString( "InvalidTemplate", this.LocalResourceFile );
                            lblTemplateMessage.Text = string.Format( strMessage, lstTemplate.SelectedItem.Text + ".template" );
                            //Cancel Page move if invalid template
                            we.Cancel = true;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Page_FinishWizard runs when the Finish Button on the Wizard is clicked.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/12/2004	created
        /// </history>
        protected void Page_FinishWizard( object sender, WizardEventArgs we )
        {
            PortalController objPortalController = new PortalController();

            // use Portal Template to update portal content pages
            if( lstTemplate.SelectedIndex != - 1 )
            {
                string strTemplateFile = lstTemplate.SelectedItem.Text + ".template";

                // process zip resource file if present
                objPortalController.ProcessResourceFile( PortalSettings.HomeDirectoryMapPath, Globals.HostMapPath + strTemplateFile );

                //Process Template
                switch( optMerge.SelectedValue )
                {
                    case "Ignore":

                        objPortalController.ParseTemplate( PortalId, Globals.HostMapPath, strTemplateFile, PortalSettings.AdministratorId, PortalTemplateModuleAction.Ignore, false );
                        break;
                    case "Replace":

                        objPortalController.ParseTemplate( PortalId, Globals.HostMapPath, strTemplateFile, PortalSettings.AdministratorId, PortalTemplateModuleAction.Replace, false );
                        break;
                    case "Merge":

                        objPortalController.ParseTemplate( PortalId, Globals.HostMapPath, strTemplateFile, PortalSettings.AdministratorId, PortalTemplateModuleAction.Merge, false );
                        break;
                }
            }

            // update Portal info in the database
            PortalInfo objPortal = objPortalController.GetPortal( PortalId );
            objPortal.Description = txtDescription.Text;
            objPortal.KeyWords = txtKeyWords.Text;
            objPortal.PortalName = txtPortalName.Text;
            objPortal.LogoFile = urlLogo.Url;
            objPortalController.UpdatePortalInfo( objPortal );

            SkinController objSkins = new SkinController();

            //Set Portal Skin
            SkinController.SetSkin(SkinInfo.RootSkin, PortalId, SkinType.Portal, ctlPortalSkin.SkinSrc);
            SkinController.SetSkin(SkinInfo.RootSkin, PortalId, SkinType.Admin, ctlPortalSkin.SkinSrc);

            //Set Portal Container
            SkinController.SetSkin(SkinInfo.RootContainer, PortalId, SkinType.Portal, ctlPortalContainer.SkinSrc);
            SkinController.SetSkin(SkinInfo.RootContainer, PortalId, SkinType.Admin, ctlPortalContainer.SkinSrc);

            this.DisplaySuccessPage( Localization.GetString( "SuccessMessage", this.LocalResourceFile ) );
        }

        /// <summary>
        /// chkIncludeAll_CheckedChanged runs when include all containers checkbox status is changed
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	12/15/2004	created
        /// </history>
        protected void chkIncludeAll_CheckedChanged( object sender, EventArgs e )
        {
            BindContainers();
        }

        /// <summary>
        /// chkTemplate_CheckedChanged runs when use template checkbox status is changed
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/13/2004	created
        /// </history>
        protected void chkTemplate_CheckedChanged( object sender, EventArgs e )
        {
            if( chkTemplate.Checked )
            {
                lstTemplate.SelectedIndex = - 1;
            }

            UseTemplate();
        }

        /// <summary>
        /// lstTemplate_SelectedIndexChanged runs when the selected template is changed
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/04/2004	created
        /// </history>
        protected void lstTemplate_SelectedIndexChanged( object sender, EventArgs e )
        {
            if( lstTemplate.SelectedIndex > - 1 )
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlNode node;
                string strTemplatePath = Globals.HostMapPath;
                string strTemplateFile = lstTemplate.SelectedItem.Text + ".template";

                // open the XML file
                try
                {
                    xmlDoc.Load( strTemplatePath + strTemplateFile );
                    node = xmlDoc.SelectSingleNode( "//portal/description" );
                    if( node != null )
                    {
                        lblTemplateMessage.Text = node.InnerText;
                    }
                    else
                    {
                        lblTemplateMessage.Text = "";
                    }
                }
                catch // error
                {
                    lblTemplateMessage.Text = "Error Loading Template description";
                }
            }
            else
            {
                lblTemplateMessage.Text = "";
            }
        }


    }
}