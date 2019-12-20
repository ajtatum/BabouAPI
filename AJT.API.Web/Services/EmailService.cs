using System.Threading.Tasks;
using AJT.API.Web.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BabouMail.Common;
using BabouMail.MailGun;

namespace AJT.API.Web.Services
{
    public class EmailService : IEmailSender
    {
        private readonly ILogger<EmailService> _logger;
        private readonly AppSettings _appSettings;

        public EmailService(ILogger<EmailService> logger, IOptionsMonitor<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.CurrentValue;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var babouEmail = new BabouEmail()
                .From(_appSettings.EmailSender.FromUserName, _appSettings.EmailSender.FromSenderName)
                .To(email)
                .Subject(subject)
                .Body(htmlMessage, true);

            babouEmail.Sender = new MailGunSender(_appSettings.EmailSender.Domain, _appSettings.EmailSender.ApiKey);

            var response = await babouEmail.SendAsync();

            if (response.Successful)
            {
                _logger.LogInformation("EmailService: Email sent to {ToEmail} with the subject {Subject}", email, subject);
            }
            else
            {
                _logger.LogError("EmailService: Error sending email to {ToEmail} with the subject {Subject}. Here are the errors: {@ErrorMessage}", email, subject, response.ErrorMessages);
            }
        }
    }
}
