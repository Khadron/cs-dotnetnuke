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
                this.Body = "<Base Href=\'" + portalAlias + "\'>";
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
                            if( objRecipient.Text != "" )
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
                            if( objRecipient.Text != "" )
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