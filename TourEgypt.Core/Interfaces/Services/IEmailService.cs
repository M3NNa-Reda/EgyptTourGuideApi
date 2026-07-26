using System;
using System.Collections.Generic;
using System.Text;

namespace TourEgypt.Core.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
