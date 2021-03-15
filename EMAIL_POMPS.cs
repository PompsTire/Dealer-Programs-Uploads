using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Dealer_Programs_Uploads
{
    class EMAIL_POMPS
    {
        public string EMailAddress { get; set; }
        public string EMailSubjectLine { get; set; } = "";
        public string EMailMessageBody { get; set; } = "";
        public string EMailFrom { get; set; } = "";
        public string EMail_CC { get; set; } = "";
        public string EMail_BCC { get; set; } = "";
        public bool EMail_SendAsHTML { get; set; } = false;
        public string EMail_Attachment { get; set; } = "";
        public bool IsError { get; set; } = false;
        public string ErrorMessage { get; set; } = "";

        public bool SendEmail()
        {
            ClearError();

            try
            {
                MailMessage notification = new MailMessage();
                SmtpClient client = new SmtpClient();
                client.Port = 25;
                client.Host = "mail.pompstire.com";
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("anonymous", "");
                notification.To.Add(new MailAddress(EMailAddress));
                notification.From = new MailAddress(EMailFrom);
                if (EMail_CC.Length > 0)
                    notification.CC.Add(new MailAddress(EMail_CC));

                if (EMail_BCC.Length > 0)
                    notification.Bcc.Add(new MailAddress(EMail_BCC));

                notification.Subject = EMailSubjectLine;
                notification.IsBodyHtml = EMail_SendAsHTML;
                notification.Body = EMailMessageBody;
                notification.Attachments.Add(new Attachment(EMail_Attachment));

                client.Send(notification);

            }
            catch(Exception ex)
            { SetError(ex.Message); }
            return !IsError;
        }

        private void ClearError()
        {
            IsError = false;
            ErrorMessage = "";
        }
        private void SetError(string msg)
        {
            IsError = true;
            ErrorMessage = msg;
        }

    }



}
