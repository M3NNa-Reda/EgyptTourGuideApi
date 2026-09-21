using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IConfiguration configuration,
            IHostEnvironment environment,
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _environment = environment;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var section = _configuration.GetSection("Smtp");

            var host = section["Host"];
            var fromEmail = section["FromEmail"];
            var fromName = section["FromName"] ?? "Tour Egypt";
            var userName = section["UserName"];
            var password = section["Password"];

            var port = int.TryParse(section["Port"], out var parsedPort) ? parsedPort : 587;
            var enableSsl = !bool.TryParse(section["EnableSsl"], out var parsedSsl) || parsedSsl;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(fromEmail))
            {
               if (_environment.IsDevelopment())
                {
                    _logger.LogWarning(
                        "SMTP not configured. Email NOT sent.\nTo: {To}\nSubject: {Subject}\n{Body}",
                        to, subject, body);
                    return;
                }

                throw new ApplicationException(
                    "SMTP is not configured. Set Smtp:Host and Smtp:FromEmail.");
            }

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8
            };

            message.To.Add(new MailAddress(to));

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 15000
            };

            if (!string.IsNullOrWhiteSpace(userName))
            {
                client.Credentials = new NetworkCredential(userName, password);
            }

            await client.SendMailAsync(message);
        }
    }
}