using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RentAPlace.Api.Services;
using Xunit;

namespace RentAPlace.Tests
{
    public class FakeSmtpClient : ISmtpClient
    {
        public MailMessage? SentMessage { get; private set; }

        public Task SendMailAsync(MailMessage message)
        {
            SentMessage = message;
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }

    public class SmtpEmailServiceTests
    {
        private readonly IConfiguration _config;

        public SmtpEmailServiceTests()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Email:SmtpHost", "smtp.test.com"},
                {"Email:SmtpPort", "587"},
                {"Email:User", "testuser"},
                {"Email:Password", "testpass"},
                {"Email:From", "noreply@test.com"},
                {"Email:UseSsl", "true"}
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public async Task SendAsync_ShouldBuildCorrectMailMessage()
        {
            // Arrange
            var fakeClient = new FakeSmtpClient();
            var service = new SmtpEmailService(_config, () => fakeClient);

            // Act
            await service.SendAsync("receiver@test.com", "Hello", "<b>Test Body</b>");

            // Assert
            Assert.NotNull(fakeClient.SentMessage);
            Assert.Equal("noreply@test.com", fakeClient.SentMessage!.From.Address);
            Assert.Equal("receiver@test.com", fakeClient.SentMessage.To[0].Address);
            Assert.Equal("Hello", fakeClient.SentMessage.Subject);
            Assert.Equal("<b>Test Body</b>", fakeClient.SentMessage.Body);
            Assert.True(fakeClient.SentMessage.IsBodyHtml);
        }
    }
}
