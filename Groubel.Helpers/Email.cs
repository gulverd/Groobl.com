using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Groubel.Helpers
{
    public class Email
    {

        public string SendEmail(string mail)
        {

            var randomString = RandomString(10);

            string sendingToAddress = mail;
            MailAddress to = new MailAddress(sendingToAddress);

            string sendingFromAddress = "recovery@groubel.com";
            MailAddress from = new MailAddress(sendingFromAddress);

            MailMessage EamilSingnature = new MailMessage(from, to);
            EamilSingnature.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            EamilSingnature.Subject = "Password Recovery";
            EamilSingnature.Body = "Go to this page, to change password - " + "http://groubel.com/Account/ResetPassword/?code=" + randomString;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "groubel.com";
            smtp.Port = 25;
            smtp.Credentials = new NetworkCredential(sendingFromAddress, "Password888***");
            smtp.EnableSsl = false;
            smtp.Send(EamilSingnature);

            return randomString;

        }

        private Random _random = new Random(Environment.TickCount);

        public string RandomString(int length)
        {
            string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
            StringBuilder builder = new StringBuilder(length);

            for (int i = 0; i < length; ++i)
                builder.Append(chars[_random.Next(chars.Length)]);

            return builder.ToString();
        }

    }
}
