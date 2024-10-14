using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IVSource.Controllers.Email
{
    public class EMail
    {
        SmtpClient client;
        MailMessage mailMessage;
        string adminEmail;

        public EMail()
        {
            client = new SmtpClient("172.18.80.5");
            mailMessage = new MailMessage();
            //mailMessage.From = new MailAddress("Donotreply@galileo.co.in");
            mailMessage.From = new MailAddress("Donotreply@itq.in");
            adminEmail = "ivsource@itq.in"; // "tillotma@itq.in";
        }
        public bool SendEmail(string body, string subject, string toEmail, string ccEmail, bool isAdmin)
        {
            bool isSent = false;

            try {
                if (isAdmin)
                    mailMessage.To.Add(adminEmail);
                else
                {
                    foreach (string emailId in toEmail.Split(","))
                        mailMessage.To.Add(emailId.Trim());
                }

                if (!string.IsNullOrWhiteSpace(ccEmail))
                {
                    foreach (string ccEmailId in ccEmail.Split(","))
                        mailMessage.CC.Add(ccEmailId.Trim());
                }

                mailMessage.Body = body;
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                client.Send(mailMessage);
                isSent = true;
            }
            catch (Exception ex)
            {
                isSent = false;
            }

            return isSent;
        }
    }

    public class EMailr
    {
        SmtpClient client;
        MailMessage mailMessage;
        string adminEmail;

        public EMailr(string senderEmail)
        {
            client = new SmtpClient("172.18.80.5");
            mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(senderEmail);
            adminEmail = "ivsource@itq.in"; // "tillotma@itq.in";
        }
        public bool SendEmail(string body, string subject, string toEmail, string ccEmail, bool isAdmin)
        {
            bool isSent = false;

            try
            {
                if (isAdmin)
                    mailMessage.To.Add(adminEmail);
                else
                {
                    foreach (string emailId in toEmail.Split(","))
                        mailMessage.To.Add(emailId.Trim());
                }

                if (!string.IsNullOrWhiteSpace(ccEmail))
                {
                    foreach (string ccEmailId in ccEmail.Split(","))
                        mailMessage.CC.Add(ccEmailId.Trim());
                }

                mailMessage.Body = body;
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                client.Send(mailMessage);
                isSent = true;
            }
            catch (Exception ex)
            {
                isSent = false;
            }

            return isSent;
        }
    }
}
