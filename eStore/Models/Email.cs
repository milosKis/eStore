using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace eStore.Models
{
    public static class Email
    {
        public static void Send(string mailTo, string subject, string body)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(mailTo));
            message.From = new MailAddress("estoreapp53@gmail.com");
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "estoreapp53@gmail.com",
                    Password = "eStore123"
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(message);
            }
        }
    }
}