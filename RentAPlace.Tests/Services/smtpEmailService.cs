using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace RentAPlace.Api.Services
{
    public interface ISmtpClient : IDisposable
    {
        Task SendMailAsync(MailMessage message);
    }

    public class SmtpClientWrapper : SmtpClient, ISmtpClient
    {
        public SmtpClientWrapper(string host, int port) : base(host, port) { }
    }

    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string bodyHtml);
    }

    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly Func<ISmtpClient> _smtpClientFactory;

        public SmtpEmailService(IConfiguration config, Func<ISmtpClient> smtpClientFactory = null!)
        {
            _config = config;
            _smtpClientFactory = smtpClientFactory ?? (() =>
                new SmtpClientWrapper(_config["Email:SmtpHost"]!, int.Parse(_config["Email:SmtpPort"]!))
                {
                    EnableSsl = bool.Parse(_config["Email:UseSsl"] ?? "true"),
                    Credentials = new NetworkCredential(_config["Email:User"]!, _config["Email:Password"]!)
                });
        }

        public async Task SendAsync(string to, string subject, string bodyHtml)
        {
            using var client = _smtpClientFactory();
            var msg = new MailMessage(_config["Email:From"]!, to, subject, bodyHtml) { IsBodyHtml = true };
            await client.SendMailAsync(msg);
        }
    }
}
