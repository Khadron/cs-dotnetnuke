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
        private void AddLocalizedModuleMessage( string message, ModuleMessageType type, bool display )
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
        private void AddModuleMessage( string message, ModuleMessageType type, bool display )
        {
            AddLocalizedModuleMessage( Localization.GetString( message, LocalResourceFile ), type, display );
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            this.cmdSendPassword.Click +=new EventHandler(cmdSendPassword_Click);
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

                        UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessageType.GreenSuccess );
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

                        UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessageType.RedError );
                    }
                }
                else
                {
                    strMessage = Localization.GetString( "EnterUsername", this.LocalResourceFile );
                    UI.Skins.Skin.AddModuleMessage( this, strMessage, ModuleMessageType.RedError );
                }
            }
        }


    }
}