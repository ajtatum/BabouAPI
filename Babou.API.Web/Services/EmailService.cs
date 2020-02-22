using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Babou.API.Web.Models;
using Babou.API.Web.Models.Database;
using Babou.API.Web.Services.Interfaces;
using BabouMail.Common;
using BabouMail.MailGun;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Babou.API.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppSettings _appSettings;

        public EmailService(ILogger<EmailService> logger, UserManager<ApplicationUser> userManager, IOptionsMonitor<AppSettings> appSettings)
        {
            _logger = logger;
            _userManager = userManager;
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

        public async Task SendQuickWelcomeMessage(ApplicationUser applicationUser)
        {
            var to = applicationUser.Email;
            var subject = "Welcome to Babou.io";

            var code = await _userManager.GeneratePasswordResetTokenAsync(applicationUser);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = $"https://babou.io/Identity/Account/ResetPassword?code={code}";

            var body = $"Welcome to babou.io! To get started, you'll need to set your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>. " +
                       $"Once logged in, you'll be able to view your Short Urls and do other nerdy stuff by checking out the API Docs. If you have any questions, please contact us.";

            await SendEmailAsync(to, subject, body);
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
