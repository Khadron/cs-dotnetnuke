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
    public partial class ResetPassword : UserModuleBase
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
                object setting = GetSetting(PortalId, "Security_CaptchaLogin");
                return Convert.ToBoolean(setting);
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
        private void AddLocalizedModuleMessage(string message, ModuleMessageType type, bool display)
        {
            if (display)
            {
                UI.Skins.Skin.AddModuleMessage(this, message, type);
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
        private void AddModuleMessage(string message, ModuleMessageType type, bool display)
        {
            AddLocalizedModuleMessage(Localization.GetString(message, LocalResourceFile), type, display);
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            this.cmdResetPassword.Click += new EventHandler(cmdResetPassword_Click);
        }

        /// <summary>
        /// Resets the Control to the first load state.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[jt]	04/14/2007  Created
        /// </history>
        private void ResetControl()
        {            
            UserId = -1;
            User = null;
            lblQuestion.Text = String.Empty;            
            txtAnswer.Text = String.Empty;
            tblQA.Visible = false;
        }


        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        protected void Page_Load(Object sender, EventArgs e)
        {
            if (Request.UserHostAddress != null)
            {
                ipAddress = Request.UserHostAddress;
            }

            trCaptcha1.Visible = UseCaptcha;
            trCaptcha2.Visible = UseCaptcha;

            if (UseCaptcha)
            {
                ctlCaptcha.ErrorMessage = Localization.GetString("InvalidCaptcha", this.LocalResourceFile);
                ctlCaptcha.Text = Localization.GetString("CaptchaText", this.LocalResourceFile);
            }
            //Display Question and Answer area if a valid user has been entered.
            if (!Page.IsPostBack)
            {
                tblQA.Visible = false;
            }
        }

        /// <summary>
        /// cmdResetPassword_Click runs when the password reset button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[JT]	04/13/2006  Created
        /// </history>
        protected void cmdResetPassword_Click(Object sender, EventArgs e)
        {
            string strMessage = Null.NullString;
            ModuleMessageType moduleMessageType = ModuleMessageType.GreenSuccess;
            bool canReset = true;
            string answer = String.Empty;

            if ((UseCaptcha && ctlCaptcha.IsValid) || (!UseCaptcha))
            {
                // No point in continuing if the user has not entered a username.
                if (!String.IsNullOrEmpty(txtUsername.Text.Trim()))
                {
                    PortalSecurity objSecurity = new PortalSecurity();

                    UserInfo objUser = UserController.GetUserByName(PortalSettings.PortalId, txtUsername.Text.Trim(), false);          
                    
                    if (objUser != null)
                    {
                        if (MembershipProviderConfig.RequiresQuestionAndAnswer)
                        {
                            // This is a simple check to see if this is our first or second pass through this event method.
                            if (User.UserID != objUser.UserID)
                            {
                                User = objUser;
                                canReset = false;

                                // Check to see if the user had enter an email and password question.
                                if (!String.IsNullOrEmpty(User.Membership.Email.Trim()) && !String.IsNullOrEmpty(User.Membership.PasswordQuestion.Trim()))
                                {
                                    tblQA.Visible = true;
                                    lblQuestion.Text = User.Membership.PasswordQuestion;
                                    txtAnswer.Text = String.Empty;                                    
                                    strMessage = Localization.GetString("RequiresQAndAEnabled", this.LocalResourceFile);
                                    moduleMessageType = ModuleMessageType.YellowWarning;
                                }
                                else
                                {
                                    strMessage = Localization.GetString("MissingEmailOrQuestion", this.LocalResourceFile);
                                    moduleMessageType = ModuleMessageType.RedError;
                                }                                
                            }
                            else
                            {
                                answer = txtAnswer.Text.Trim();
                                if (String.IsNullOrEmpty(answer))
                                {
                                    canReset = false;
                                    strMessage = Localization.GetString("EnterAnswer", this.LocalResourceFile);
                                    moduleMessageType = ModuleMessageType.RedError;
                                }
                            }
                        }                        
                    }
                    else
                    {   
                        canReset = false;
                        ResetControl();
                        strMessage = Localization.GetString("UsernameError", this.LocalResourceFile);
                        moduleMessageType = ModuleMessageType.YellowWarning;
                    }                   

                    if (canReset)
                    {
                        try
                        {
                            //UserController.ResetPassword(objUser, answer);
                            //Mail.SendMail(User, MessageType.PasswordReminder, PortalSettings);
                            strMessage = Localization.GetString("PasswordSent", this.LocalResourceFile);
                            moduleMessageType = ModuleMessageType.GreenSuccess;

                            EventLogController objEventLog = new EventLogController();
                            LogInfo objEventLogInfo = new LogInfo();
                            objEventLogInfo.AddProperty("IP", ipAddress);
                            objEventLogInfo.LogPortalID = PortalSettings.PortalId;
                            objEventLogInfo.LogPortalName = PortalSettings.PortalName;
                            objEventLogInfo.LogUserID = UserId;
                            objEventLogInfo.LogUserName = objSecurity.InputFilter(txtUsername.Text, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
                            objEventLogInfo.LogTypeKey = "PASSWORD_RESET_SUCCESS";
                            objEventLog.AddLog(objEventLogInfo);                            
                        }
                        catch (Exception)
                        {
                            strMessage = Localization.GetString("PasswordResetError", this.LocalResourceFile);
                            moduleMessageType = ModuleMessageType.RedError; 

                            EventLogController objEventLog = new EventLogController();
                            LogInfo objEventLogInfo = new LogInfo();
                            objEventLogInfo.AddProperty("IP", ipAddress);
                            objEventLogInfo.LogPortalID = PortalSettings.PortalId;
                            objEventLogInfo.LogPortalName = PortalSettings.PortalName;
                            objEventLogInfo.LogUserID = UserId;
                            objEventLogInfo.LogUserName = objSecurity.InputFilter(txtUsername.Text, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
                            objEventLogInfo.LogTypeKey = "PASSWORD_RESET_FAILURE";
                            objEventLog.AddLog(objEventLogInfo);                           
                        }  
                    }
                }
                else
                {
                    ResetControl();
                    strMessage = Localization.GetString("EnterUsername", this.LocalResourceFile);
                    moduleMessageType = ModuleMessageType.RedError;
                }

                UI.Skins.Skin.AddModuleMessage(this, strMessage, moduleMessageType);
            }
        }
    }
}