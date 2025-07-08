using Microsoft.AspNetCore.Identity.UI.Services;

namespace Clinic.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Here can be logic
            return Task.CompletedTask;
        }
    }
}
