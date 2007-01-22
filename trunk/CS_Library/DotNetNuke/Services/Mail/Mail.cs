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
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Services.Mail
{

    public class Mail
    {
        /// <Summary>Send a simple email.</Summary>
        public static string SendMail( string MailFrom, string MailTo, string Cc, string Bcc, MailPriority Priority, string Subject, MailFormat BodyFormat, Encoding BodyEncoding, string Body, string Attachment, string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword )
        {
            string returnValue;

            // translate semi-colon delimiters to commas as ASP.NET 2.0 does not support semi-colons
            MailTo = MailTo.Replace(";", ",");
            Cc = Cc.Replace(";", ",");
            Bcc = Bcc.Replace(";", ",");

            MailMessage objMail = new MailMessage(MailFrom, MailTo);
            if (!String.IsNullOrEmpty(Cc))
            {
                objMail.CC.Add(Cc);
            }
            if (!String.IsNullOrEmpty(Bcc))
            {
                objMail.Bcc.Add(Bcc);
            }
            objMail.Priority = (System.Net.Mail.MailPriority)Priority;
            objMail.IsBodyHtml = Convert.ToBoolean(BodyFormat == MailFormat.Html ? true : false);

            if (!String.IsNullOrEmpty(Attachment))
            {
                objMail.Attachments.Add(new Attachment(Attachment));
            }

            // message
            objMail.SubjectEncoding = BodyEncoding;
            objMail.Subject = HtmlUtils.StripWhiteSpace(Subject, true);
            objMail.BodyEncoding = BodyEncoding;
            objMail.Body = Body;

            // external SMTP server alternate port
            int SmtpPort = Null.NullInteger;
            int portPos = SMTPServer.IndexOf(":");
            if (portPos > -1)
            {
                SmtpPort = int.Parse(SMTPServer.Substring(portPos + 1, SMTPServer.Length - portPos - 1));
                SMTPServer = SMTPServer.Substring(0, portPos);
            }

            SmtpClient smtpClient = new SmtpClient();

            if (!String.IsNullOrEmpty(SMTPServer))
            {
                smtpClient.Host = SMTPServer;
                if (SmtpPort > Null.NullInteger)
                {
                    smtpClient.Port = SmtpPort;
                }
                switch (SMTPAuthentication)
                {
                    case "": // anonymous
                        break;

                    case "0":

                        break;
                    case "1": // basic

                        if (!String.IsNullOrEmpty(SMTPUsername) && !String.IsNullOrEmpty(SMTPPassword))
                        {
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = new NetworkCredential(SMTPUsername, SMTPPassword);
                        }
                        break;
                    case "2": // NTLM

                        smtpClient.UseDefaultCredentials = true;
                        break;
                }
            }

            if (Convert.ToString(Globals.HostSettings["SMTPEnableSSL"]) == "Y")
            {
                smtpClient.EnableSsl = true;
            }
            try
            {
                smtpClient.Send(objMail);
                returnValue = "";
            }
            catch (Exception objException)
            {
                // mail configuration problem
                if (objException.InnerException != null)
                {
                    returnValue = string.Concat(objException.Message, Environment.NewLine, objException.InnerException.Message);
                    Exceptions.Exceptions.LogException(objException.InnerException);
                }
                else
                {
                    returnValue = objException.Message;
                    Exceptions.Exceptions.LogException(objException);
                }
            }

            return returnValue;
        }

        /// <Summary>Send a simple email.</Summary>
        public static string SendMail( string MailFrom, string MailTo, string Bcc, string Subject, string Body, string Attachment, string BodyType, string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword )
        {
           // SMTP server configuration
            if( SMTPServer == "" )
            {
                if( Convert.ToString( Globals.HostSettings["SMTPServer"] ) != "" )
                {
                    SMTPServer = Convert.ToString( Globals.HostSettings["SMTPServer"] );
                }
            }
            if( SMTPAuthentication == "" )
            {
                if( Convert.ToString( Globals.HostSettings["SMTPAuthentication"] ) != "" )
                {
                    SMTPAuthentication = Convert.ToString( Globals.HostSettings["SMTPAuthentication"] );
                }
            }
            if( SMTPUsername == "" )
            {
                if( Convert.ToString( Globals.HostSettings["SMTPUsername"] ) != "" )
                {
                    SMTPUsername = Convert.ToString( Globals.HostSettings["SMTPUsername"] );
                }
            }
            if( SMTPPassword == "" )
            {
                if( Convert.ToString( Globals.HostSettings["SMTPPassword"] ) != "" )
                {
                    SMTPPassword = Convert.ToString( Globals.HostSettings["SMTPPassword"] );
                }
            }

            // here we check if we want to format the email as html or plain text.
            MailFormat objBodyFormat = MailFormat.Html;
            if( !String.IsNullOrEmpty(BodyType) )
            {
                switch( BodyType.ToLower() )
                {
                    case "html":

                        objBodyFormat = MailFormat.Html;
                        break;
                    case "text":

                        objBodyFormat = MailFormat.Text;
                        break;
                }
            }

            return SendMail( MailFrom, MailTo, "", Bcc, MailPriority.Normal, Subject, objBodyFormat, Encoding.UTF8, Body, Attachment, SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword );
       }

        /// <Summary>Send an email notification</Summary>
        /// <Param name="user">The user to whom the message is being sent</Param>
        /// <Param name="msgType">The type of message being sent</Param>
        /// <Param name="settings">Portal Settings</Param>
        public static string SendMail( UserInfo user, MessageType msgType, PortalSettings settings )
        {
            string userEmail = user.Email;
            string locale = user.Profile.PreferredLocale;
            string subject = "";
            string body = "";
            ArrayList custom = null;
            switch( msgType )
            {
                case MessageType.UserRegistrationAdmin:
                    {
                        subject = "EMAIL_USER_REGISTRATION_ADMINISTRATOR_SUBJECT";
                        body = "EMAIL_USER_REGISTRATION_ADMINISTRATOR_BODY";
                        userEmail = settings.Email;
                        return SendMail( settings.Email, userEmail, "", Localization.Localization.GetSystemMessage( locale, settings, subject, user ), Localization.Localization.GetSystemMessage( locale, settings, body, user, "~/App_GlobalResources/GlobalResources.resx", custom ), "", "", "", "", "", "" );
                    }
                case MessageType.UserRegistrationPrivate:
                    {
                        subject = "EMAIL_USER_REGISTRATION_PRIVATE_SUBJECT";
                        body = "EMAIL_USER_REGISTRATION_PRIVATE_BODY";
                        return SendMail( settings.Email, userEmail, "", Localization.Localization.GetSystemMessage( locale, settings, subject, user ), Localization.Localization.GetSystemMessage( locale, settings, body, user, "~/App_GlobalResources/GlobalResources.resx", custom ), "", "", "", "", "", "" );
                    }
                case MessageType.UserRegistrationPublic:
                    {
                        subject = "EMAIL_USER_REGISTRATION_PUBLIC_SUBJECT";
                        body = "EMAIL_USER_REGISTRATION_PUBLIC_BODY";
                        return SendMail( settings.Email, userEmail, "", Localization.Localization.GetSystemMessage( locale, settings, subject, user ), Localization.Localization.GetSystemMessage( locale, settings, body, user, "~/App_GlobalResources/GlobalResources.resx", custom ), "", "", "", "", "", "" );
                    }
                case MessageType.UserRegistrationVerified:
                    {
                        subject = "EMAIL_USER_REGISTRATION_VERIFIED_SUBJECT";
                        body = "EMAIL_USER_REGISTRATION_VERIFIED_BODY";
                        if( HttpContext.Current == null )
                        {
                            return SendMail( settings.Email, userEmail, "", Localization.Localization.GetSystemMessage( locale, settings, subject, user ), Localization.Localization.GetSystemMessage( locale, settings, body, user, "~/App_GlobalResources/GlobalResources.resx", custom ), "", "", "", "", "", "" );
                        }
                        custom = new ArrayList();
                        custom.Add( HttpContext.Current.Server.UrlEncode( user.Username ) );
                        return SendMail( settings.Email, userEmail, "", Localization.Localization.GetSystemMessage( locale, settings, subject, user ), Localization.Localization.GetSystemMessage( locale, settings, body, user, "~/App_GlobalResources/GlobalResources.resx", custom ), "", "", "", "", "", "" );
                    }
                case MessageType.PasswordReminder:
                    {
                        subject = "EMAIL_PASSWORD_REMINDER_SUBJECT";
                        body = "EMAIL_PASSWORD_REMINDER_BODY";
                        return SendMail( settings.Email, userEmail, "", Localization.Localization.GetSystemMessage( locale, settings, subject, user ), Localization.Localization.GetSystemMessage( locale, settings, body, user, "~/App_GlobalResources/GlobalResources.resx", custom ), "", "", "", "", "", "" );
                    }
                case MessageType.ProfileUpdated:
                    {
                        subject = "EMAIL_PROFILE_UPDATED_SUBJECT";
                        body = "EMAIL_PROFILE_UPDATED_BODY";
                        return SendMail( settings.Email, userEmail, "", Localization.Localization.GetSystemMessage( locale, settings, subject, user ), Localization.Localization.GetSystemMessage( locale, settings, body, user, "~/App_GlobalResources/GlobalResources.resx", custom ), "", "", "", "", "", "" );
                    }
            }
            return SendMail( settings.Email, userEmail, "", Localization.Localization.GetSystemMessage( locale, settings, subject, user ), Localization.Localization.GetSystemMessage( locale, settings, body, user, "~/App_GlobalResources/GlobalResources.resx", custom ), "", "", "", "", "", "" );
        }
    }
}