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
using System.Diagnostics;
using System.Text;
using System.Web.UI.WebControls;

namespace DotNetNuke.Services.Mail
{
    /// <Summary>
    /// The SendBulkEMail Class is a helper class tht manages the sending of Email
    /// </Summary>
    public class SendBulkEmail
    {
        public ArrayList Recipients;
        public MailPriority Priority;
        public string Subject;
        public MailFormat BodyFormat;
        public string Body;
        public string Attachment;
        public string SendMethod;
        public string SMTPServer;
        public string SMTPAuthentication;
        public string SMTPUsername;
        public string SMTPPassword;
        public string Administrator;
        public string Heading;

        public SendBulkEmail()
        {
        }

        public SendBulkEmail( ArrayList recipients, string priority, string format, string portalAlias )
        {
            this.Recipients = recipients;
            switch( priority )
            {
                case "1":

                    this.Priority = MailPriority.High;
                    break;
                case "2":

                    this.Priority = MailPriority.Normal;
                    break;
                case "3":

                    this.Priority = MailPriority.Low;
                    break;
            }
            if( format == "BASIC" )
            {
                this.BodyFormat = MailFormat.Text;
            }
            else
            {
                this.BodyFormat = MailFormat.Html;
                // Add Base Href for any images inserted in to the email.
                this.Body = "<Base Href='" + portalAlias + "'>";
            }
        }

        public void Send()
        {
            try
            {
                string endDelimit;
                //Use either vbCrLF or <br><br> depending on BodyFormat.
                if( BodyFormat == MailFormat.Html )
                {
                    endDelimit = "<br>";
                }
                else
                {
                    endDelimit = "\r\n";
                }

                string strConfirmation;
                strConfirmation = "Bulk Email Operation Started: " + DateTime.Now.ToString() + endDelimit + endDelimit;

                // send to recipients
                string strBody;
                int intRecipients = 0;
                int intMessages = 0;
                string strDistributionList = "";
                ListItem objRecipient;

                switch( SendMethod )
                {
                    case "TO":

                        foreach( ListItem tempLoopVar_objRecipient in Recipients )
                        {
                            objRecipient = tempLoopVar_objRecipient;
                            if( !String.IsNullOrEmpty(objRecipient.Text) )
                            {
                                intRecipients++;
                                strBody = Heading + objRecipient.Value + "," + endDelimit + endDelimit + Body;
                                Mail.SendMail( Administrator, objRecipient.Text, "", "", Priority, Subject, BodyFormat, Encoding.UTF8, strBody, Attachment, SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword );
                                intMessages++;
                            }
                        }
                        break;
                    case "BCC":

                        foreach( ListItem tempLoopVar_objRecipient in Recipients )
                        {
                            objRecipient = tempLoopVar_objRecipient;
                            if( !String.IsNullOrEmpty(objRecipient.Text) )
                            {
                                intRecipients++;
                                strDistributionList += "; " + objRecipient.Text;
                            }
                        }
                        intMessages = 1;
                        strBody = Body;
                        Mail.SendMail( Administrator, "", "", strDistributionList, Priority, Subject, BodyFormat, Encoding.UTF8, strBody, Attachment, SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword );
                        break;
                }

                // send confirmation
                strConfirmation += "Email Recipients: " + intRecipients.ToString() + endDelimit;
                strConfirmation += "Email Messages: " + intMessages.ToString() + endDelimit + endDelimit;
                strConfirmation += "Bulk Email Operation Completed: " + DateTime.Now.ToString() + endDelimit;
                Mail.SendMail( Administrator, Administrator, "", "", Priority, "Bulk Email Confirmation", BodyFormat, Encoding.UTF8, strConfirmation, "", SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword );
            }
            catch( Exception exc )
            {
                // send mail failure
                Debug.Write( exc.Message );
            }
        }
    }
}