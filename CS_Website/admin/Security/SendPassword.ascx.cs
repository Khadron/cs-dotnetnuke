using System;
using System.Diagnostics;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using DotNetNuke.UI.Skins.Controls;

namespace DotNetNuke.Modules.Admin.Security
{
    /// <summary>
    /// The SendPassword UserModuleBase is used to allow a user to retrieve their password
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	03/21/2006  Created
    /// </history>
    public partial class SendPassword : UserModuleBase
    {
        private string ipAddress;

        /// <summary>
        /// Gets whether the Captcha control is used to validate the login
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        protected bool UseCaptcha
        {
            get
            {
                object setting = GetSetting( PortalId, "Security_CaptchaLogin" );
                return Convert.ToBoolean( setting );
            }
        }

        /// <summary>
        /// AddLocalizedModuleMessage adds a localized module message
        /// </summary>
        /// <param name="message">The localized message</param>
        /// <param name="type">The type of message</param>
        /// <param name="display">A flag that determines whether the message should be displayed</param>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        private void AddLocalizedModuleMessage( string message, ModuleMessage.ModuleMessageType type, bool display )
        {
            if( display )
            {
                UI.Skins.Skin.AddModuleMessage( this, message, type );
            }
        }

        /// <summary>
        /// AddModuleMessage adds a module message
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="type">The type of message</param>
        /// <param name="display">A flag that determines whether the message should be displayed</param>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        private void AddModuleMessage( string message, ModuleMessage.ModuleMessageType type, bool display )
        {
            AddLocalizedModuleMessage( Localization.GetString( message, LocalResourceFile ), type, display );
        }

        /// <summary>
        /// Page_Init runs when the control is initialised
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        private void Page_Init( Object sender, EventArgs e )
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        protected void Page_Load( Object sender, EventArgs e )
        {
            if( Request.UserHostAddress != null )
            {
                ipAddress = Request.UserHostAddress;
            }

            trCaptcha1.Visible = UseCaptcha;
            trCaptcha2.Visible = UseCaptcha;

            if( UseCaptcha )
            {
                ctlCaptcha.ErrorMessage = Localization.GetString( "InvalidCaptcha", this.LocalResourceFile );
                ctlCaptcha.Text = Localization.GetString( "CaptchaText", this.LocalResourceFile );
            }

            tblQA.Visible = MembershipProviderConfig.RequiresQuestionAndAnswer;
        }

        /// <summary>
        /// cmdSendPassword_Click runs when the Password Reminder button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        protected void cmdSendPassword_Click( Object sender, EventArgs e )
        {
            string strMessage = Null.NullString;
            bool canSend = true;

            if( ( UseCaptcha && ctlCaptcha.IsValid ) || ( ! UseCaptcha ) )
            {
                if( txtUsername.Text.Trim() != "" )
                {
                    PortalSecurity objSecurity = new PortalSecurity();

                    UserInfo objUser = UserController.GetUserByName( PortalSettings.PortalId, txtUsername.Text, false );
                    if( objUser != null )
                    {
                        if( MembershipProviderConfig.PasswordRetrievalEnabled )
                        {
                            try
                            {
                                objUser.Membership.Password = UserController.GetPassword( ref objUser, txtAnswer.Text );
                            }
                            catch( Exception )
                            {
                                canSend = false;
                                strMessage = Localization.GetString( "PasswordRetrievalError", this.LocalResourceFile );
                            }
                        }
                        else
                        {
                            canSend = false;
                            strMessage = Localization.GetString( "PasswordRetrievalDisabled", this.LocalResourceFile );
                        }
                        if( canSend )
                        {
                            try
                            {
                                Mail.SendMail( objUser, MessageType.PasswordReminder, PortalSettings );
                                strMessage = Localization.GetString( "PasswordSent", this.LocalResourceFile );
                            }
                            catch( Exception )
                            {
                                canSend = false;
                            }
                        }
                    }
                    else
                    {
                        strMessage = Localization.GetString( "UsernameError", this.LocalResourceFile );
                        canSend = false;
                    }

                    if( canSend )
                    {
                        EventLogController objEventLog = new EventLogController();
                        LogInfo objEventLogInfo = new LogInfo();
                        objEventLogInfo.AddProperty( "IP", ipAddress );
                        objEventLogInfo.LogPortalID = PortalSettings.PortalId;
                        objEventLogInfo.LogPortalName = PortalSettings.PortalName;
                        objEventLogInfo.LogUserID = UserId;
                        objEventLogInfo.LogUserName = objSecurity.InputFilter( txtUsername.Text, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup );
                        objEventLogInfo.LogTypeKey = "PASSWORD_SENT_SUCCESS";
                        objEventLog.AddLog( objEventLogInfo );

                        UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessage.ModuleMessageType.GreenSuccess );
                    }
                    else
                    {
                        EventLogController objEventLog = new EventLogController();
                        LogInfo objEventLogInfo = new LogInfo();
                        objEventLogInfo.AddProperty( "IP", ipAddress );
                        objEventLogInfo.LogPortalID = PortalSettings.PortalId;
                        objEventLogInfo.LogPortalName = PortalSettings.PortalName;
                        objEventLogInfo.LogUserID = UserId;
                        objEventLogInfo.LogUserName = objSecurity.InputFilter( txtUsername.Text, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup );
                        objEventLogInfo.LogTypeKey = "PASSWORD_SENT_FAILURE";
                        objEventLog.AddLog( objEventLogInfo );

                        UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessage.ModuleMessageType.RedError );
                    }
                }
                else
                {
                    strMessage = Localization.GetString( "EnterUsername", this.LocalResourceFile );
                    UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessage.ModuleMessageType.RedError );
                }
            }
        }

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }
    }
}