using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace MemoriesBack.Service
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPasswordResetEmailAsync(string to, string token)
        {
            var settings = _configuration.GetSection("EmailSettings");
            var host = settings["Host"];
            var port = int.Parse(settings["Port"]);
            var username = settings["Username"];
            var password = settings["Password"];
            var sender = settings["Sender"];
            var enableSsl = bool.Parse(settings["EnableSsl"]);

            string resetUrl = $"http://localhost:8080/reset-password.html?token={token}";
            string subject = "Resetowanie hasła";
            string body = $"Kliknij poniższy link, aby zresetować hasło:\n{resetUrl}";

            var message = new MailMessage
            {
                From = new MailAddress(sender),
                Subject = subject,
                Body = body
            };
            message.To.Add(to);

            using var smtp = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(username, password)
            };

            await smtp.SendMailAsync(message);
        }
    }
}
