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
using System.Collections;
using System.Threading;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.UI.Skins.Controls;

namespace DotNetNuke.Modules.Admin.Users
{
    /// <summary>
    /// The BulkEmail PortalModuleBase is used to manage a Bulk Email mesages
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    ///     [lpointer]  03-Feb-06   Added 'From' email address support.
    /// </history>
    public partial class BulkEmail : PortalModuleBase
    {
        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            try
            {
                if( ! Page.IsPostBack )
                {
                    RoleController objRoleController = new RoleController();
                    chkRoles.DataSource = objRoleController.GetPortalRoles( PortalId );
                    chkRoles.DataBind();

                    //Dim FileList As ArrayList = GetFileList(PortalId)
                    //cboAttachment.DataSource = FileList
                    //cboAttachment.DataBind()
                    //cmdUpload.NavigateUrl =Common.Globals.NavigateURL("File Manager")

                    txtFrom.Text = UserInfo.Email;
                }
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }

        /// <summary>
        /// cmdSend_Click runs when the cmdSend Button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        protected void cmdSend_Click( Object sender, EventArgs e )
        {
            try
            {
                ArrayList objRecipients = new ArrayList();
                RoleController objRoles = new RoleController();
                ListItem objListItem;

                // load all user emails based on roles selected
                foreach( ListItem objRole in chkRoles.Items )
                {
                    if( objRole.Selected )
                    {
                        foreach( UserInfo objUser in objRoles.GetUsersByRoleName( PortalId, objRole.Value ) )
                        {
                            if( objUser.Membership.Approved )
                            {
                                objListItem = new ListItem( objUser.Email, objUser.DisplayName );
                                if( ! objRecipients.Contains( objListItem ) )
                                {
                                    objRecipients.Add( objListItem );
                                }
                            }
                        }
                    }
                }

                // load emails specified in email distribution list
                if( !String.IsNullOrEmpty(txtEmail.Text) )
                {
                    string[] splitter = {";"};
                    Array arrEmail = txtEmail.Text.Split(splitter, StringSplitOptions.None);
                    foreach( string strEmail in arrEmail )
                    {
                        objListItem = new ListItem( strEmail, strEmail );
                        if( ! objRecipients.Contains( objListItem ) )
                        {
                            objRecipients.Add( objListItem );
                        }
                    }
                }

                // create object
                SendBulkEmail objSendBulkEMail = new SendBulkEmail( objRecipients, cboPriority.SelectedItem.Value, teMessage.Mode, PortalSettings.PortalAlias.HTTPAlias );
                objSendBulkEMail.Subject = txtSubject.Text;
                objSendBulkEMail.Body += teMessage.Text;
                if( ctlAttachment.Url.StartsWith( "FileID=" ) )
                {
                    int fileId = int.Parse( ctlAttachment.Url.Substring( 7 ) );
                    FileController objFileController = new FileController();
                    FileInfo objFileInfo = objFileController.GetFileById( fileId, PortalId );

                    objSendBulkEMail.Attachment = PortalSettings.HomeDirectoryMapPath + objFileInfo.Folder + objFileInfo.FileName;
                }
                objSendBulkEMail.SendMethod = cboSendMethod.SelectedItem.Value;
                objSendBulkEMail.SMTPServer = Convert.ToString( PortalSettings.HostSettings["SMTPServer"] );
                objSendBulkEMail.SMTPAuthentication = Convert.ToString( PortalSettings.HostSettings["SMTPAuthentication"] );
                objSendBulkEMail.SMTPUsername = Convert.ToString( PortalSettings.HostSettings["SMTPUsername"] );
                objSendBulkEMail.SMTPPassword = Convert.ToString( PortalSettings.HostSettings["SMTPPassword"] );
                objSendBulkEMail.Administrator = txtFrom.Text;
                objSendBulkEMail.Heading = Localization.GetString( "Heading", this.LocalResourceFile );

                // send mail
                if( optSendAction.SelectedItem.Value == "S" )
                {
                    objSendBulkEMail.Send();
                }
                else // ansynchronous uses threading
                {
                    Thread objThread = new Thread( new ThreadStart( objSendBulkEMail.Send ) );
                    objThread.Start();
                }

                // completed
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "MessageSent", this.LocalResourceFile ), ModuleMessageType.GreenSuccess );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}