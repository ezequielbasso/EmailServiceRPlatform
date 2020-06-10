using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace IMAP
{
    public class Monitoring
    {
        public string _ServerSmtp { get; set; }
        public string _User { get; set; }
        public string _Password { get; set; }
        public string _From { get; set; }
        public List<string> _To{ get; set; }

        public Monitoring(string server, string user, string password, string from, string to)
        {
            _ServerSmtp = server;
            _User = user;
            _Password = password;
            _From = from;
            _To = to.Split(',').ToList();
        }
        public System.Net.Mail.MailMessage SendMail( string subject, string text)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new System.Net.Mail.MailAddress("g7basso@gmail.com");
                foreach (var myMail in _To)
                {
                    mail.To.Add(new System.Net.Mail.MailAddress(myMail));
                }
                mail.Subject = subject;
                mail.Body = text;

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential("g7basso", "eBasso34854944eE");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);

                return mail;
            }
            catch (Exception e)
            {
                throw new Exception("Mail.Send: " + e.Message);
            }
        }


    }
}
