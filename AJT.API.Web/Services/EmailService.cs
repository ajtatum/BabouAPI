using System.Net.Mail;
using System.Threading.Tasks;
using AJT.API.Web.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            await SendViaMailGun(email, subject, htmlMessage);
        }

        private async Task SendViaMailGun(string email, string subject, string htmlMessage)
        {
            var babouMail = new MailGun(_appSettings.EmailSender.Domain, _appSettings.EmailSender.ApiKey);

            var babouEmail = new BabouMail.Common.Email()
            {
                FromMailAddress = new MailAddress(_appSettings.EmailSender.FromUserName, _appSettings.EmailSender.FromSenderName),
                ToAddress = email,
                Subject = subject,
                Body = htmlMessage,
                IsHtml = true
            };

            var response = await babouMail.SendMailAsync(babouEmail);

            if (response.IsSuccessful)
            {
                _logger.LogInformation("EmailService: Email sent to {ToEmail} with the subject {Subject}", email, subject);
            }
            else
            {
                _logger.LogError("EmailService: Error sending email to {ToEmail} with the subject {Subject}. Here are the errors: {ErrorMessage}", email, subject, response.ErrorMessage);
            }
        }
    }
}
