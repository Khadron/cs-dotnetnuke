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
                ListItem objRole;
                ListItem objListItem;
                UserInfo objUser;

                // load all user emails based on roles selected
                foreach( ListItem tempLoopVar_objRole in chkRoles.Items )
                {
                    objRole = tempLoopVar_objRole;
                    if( objRole.Selected )
                    {
                        foreach( UserInfo tempLoopVar_objUser in objRoles.GetUsersByRoleName( PortalId, objRole.Value ) )
                        {
                            objUser = tempLoopVar_objUser;
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
                if( txtEmail.Text != "" )
                {
                    string[] splitter = {";"};
                    Array arrEmail = txtEmail.Text.Split(splitter, StringSplitOptions.None);
                    string strEmail;
                    foreach( string tempLoopVar_strEmail in arrEmail )
                    {
                        strEmail = tempLoopVar_strEmail;
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
                UI.Skins.Skin.AddModuleMessage( this, Localization.GetString( "MessageSent", this.LocalResourceFile ), ModuleMessage.ModuleMessageType.GreenSuccess );
            }
            catch( Exception exc ) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException( this, exc );
            }
        }
    }
}