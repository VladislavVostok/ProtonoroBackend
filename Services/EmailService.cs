using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using ProtonoroBackend.Services.Interfaces;
using ProtonoroBackend.Configurations;

namespace ProtonoroBackend.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;


        public EmailService(
            IOptions<EmailSettings> emailSettings
        ) {
            _emailSettings = emailSettings.Value;

        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(_emailSettings.SMTPServer, _emailSettings.Port, _emailSettings.EnableSSL);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(emailMessage);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}