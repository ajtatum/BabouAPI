﻿using System.Threading.Tasks;
using Babou.API.Web.Models;
using Babou.API.Web.Models.Database;
using Babou.API.Web.Services.Interfaces;
using BabouMail.Common;
using BabouMail.MailGun;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Babou.API.Web.Services
{
    public class EmailService : IEmailService
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
                .From(_appSettings.EmailSender.FromEmail, _appSettings.EmailSender.FromName)
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

        public async Task SendContactMessage(string fromEmail, string fromName, string subject, string htmlMessage)
        {
            var babouEmail = new BabouEmail()
                .From(_appSettings.EmailSender.FromEmail, fromName)
                .To(_appSettings.EmailSender.ToEmail)
                .ReplyTo(fromEmail, fromName)
                .Subject(subject)
                .Body(htmlMessage, true);

            babouEmail.Sender = new MailGunSender(_appSettings.EmailSender.Domain, _appSettings.EmailSender.ApiKey);

            var response = await babouEmail.SendAsync();

            if (response.Successful)
            {
                _logger.LogInformation("EmailService: Email sent to {ToEmail} from {FromEmail} with the subject {Subject}", _appSettings.EmailSender.ToEmail, fromEmail, subject);
            }
            else
            {
                _logger.LogError("EmailService: Error sending email to {ToEmail} from {FromEmail} with the subject {Subject}. Here are the errors: {@ErrorMessage}", _appSettings.EmailSender.ToEmail, fromEmail, subject, response.ErrorMessages);
            }
        }

        public async Task SendNewUserMessage(ApplicationUser applicationUser)
        {
            var to = _appSettings.EmailSender.ToEmail;
            var subject = $"New User Registered: {applicationUser.UserName}";

            var body = $"<html><body><b>New User Registered:</b> {applicationUser.Email}.<br /><br /><a href='https://babou.io' target='_blank'>Go to API.</a></body></html>";

            await SendEmailAsync(to, subject, body);
        }
    }
}
