using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Interfaces.Services;

namespace TourEgypt.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            throw new NotImplementedException();
        }
    }
}
