using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FileStorage.Controllers
{
    public class Service
    {
        private readonly ILogger<Service> logger;
        public Service(ILogger<Service> logger)
        {
            this.logger = logger;
        }
        public void SendEmailDefault(string msg,string email)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.IsBodyHtml = true;
                message.From = new MailAddress("admin@company.com", "My company");
                message.To.Add(email);
                message.Subject = "File";
                message.Body = msg;
                using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Credentials = new NetworkCredential("merlin050809wwww@gmail.com", "050809Roma");
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Send(message);
                    logger.LogInformation("Сообщение отправлено успешно");
                }

            }
            catch (Exception e)
            {
                logger.LogError(e.GetBaseException().Message);
            }
        }
    }
}
