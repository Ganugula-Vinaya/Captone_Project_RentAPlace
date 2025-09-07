
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace RentAPlace.Api.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string bodyHtml);
    }

    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public SmtpEmailService(IConfiguration config) => _config = config;

        public async Task SendAsync(string to, string subject, string bodyHtml)
        {
            var host = _config["Email:SmtpHost"]!;
            var port = int.Parse(_config["Email:SmtpPort"]!);
            var user = _config["Email:User"]!;
            var pass = _config["Email:Password"]!;
            var from = _config["Email:From"]!;
            var enableSsl = bool.Parse(_config["Email:UseSsl"] ?? "true");

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(user, pass)
            };

            var msg = new MailMessage(from, to, subject, bodyHtml) { IsBodyHtml = true };
            await client.SendMailAsync(msg);
        }
    }
}

