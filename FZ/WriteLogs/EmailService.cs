using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FZ.FZSite
{
    public static class EmailService
    {
        public static async Task SendEmailAsync(string recipient, string subject, string message)
        {
            var fromAddress = new MailAddress("vessolair@gmail.com", "Site Monitoring");
            var toAddress = new MailAddress(recipient);
            const string fromPassword = "Vasequnen0801";
            const string host = "smtp.gmail.com";
            const int port = 587;

            var smtpClient = new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            var mailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = message
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
