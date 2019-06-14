using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.App_Code
{
    class Mail
    {
        public static void Send(string strMailTitle, string strMailContent)
        {
            string MailServer = System.Configuration.ConfigurationSettings.AppSettings["MailServer"].ToString();
            string Reciever = System.Configuration.ConfigurationSettings.AppSettings["Reciever"].ToString();
            using (SmtpClient client = new SmtpClient(MailServer))
            {
                string to = Reciever;
                string from = string.Format("系統通知信- 請勿回覆<{0}>", "service@eslite.com");
                MailMessage message = new MailMessage(from, to);
                message.Subject = strMailTitle;
                message.Body = strMailContent;
                client.UseDefaultCredentials = true;
                client.Send(message);
            }
        }
    }
}
