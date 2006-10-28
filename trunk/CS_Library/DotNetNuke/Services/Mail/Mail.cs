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

            MailMessage objMail = new MailMessage(MailFrom, MailTo);
            if (Cc != "")
            {
                objMail.CC.Add(Cc);
            }
            if (Bcc != "")
            {
                objMail.Bcc.Add(Bcc);
            }
            objMail.Priority = (System.Net.Mail.MailPriority)Priority;
            objMail.IsBodyHtml = Convert.ToBoolean(BodyFormat == MailFormat.Html ? true : false);

            if (Attachment != "")
            {
                objMail.Attachments.Add(new Attachment(Attachment));
            }

            // message
            objMail.SubjectEncoding = BodyEncoding;
            objMail.Subject = HtmlUtils.StripWhiteSpace(Subject, true);
            objMail.BodyEncoding = BodyEncoding;
            objMail.Body = Body;

            // external SMTP server
            int SmtpPort = Null.NullInteger;
            int portPos = SMTPServer.IndexOf(":");
            if (portPos > -1)
            {
                SmtpPort = int.Parse(SMTPServer.Substring(portPos + 1, SMTPServer.Length - portPos - 1));
            }

            SmtpClient smtpClient = new SmtpClient();

            if (SMTPServer != "")
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

                        if (SMTPUsername != "" && SMTPPassword != "")
                        {
                            smtpClient.Credentials = new NetworkCredential(SMTPUsername, SMTPPassword);
                        }
                        break;
                    case "2": // NTLM

                        smtpClient.UseDefaultCredentials = true;
                        break;
                }
            }

            try
            {
                smtpClient.Send(objMail);
                returnValue = "";
            }
            catch (Exception objException)
            {
                // mail configuration problem
                returnValue = objException.Message;
                Exceptions.Exceptions.LogException(objException);
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
            if( BodyType != "" )
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
                        int i1 = custom.Add( HttpContext.Current.Server.UrlEncode( user.Username ) );
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