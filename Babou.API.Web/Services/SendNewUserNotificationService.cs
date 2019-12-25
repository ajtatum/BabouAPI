using System.Threading.Tasks;
using Babou.API.Web.Models;
using Babou.API.Web.Models.Database;
using Babou.API.Web.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Babou.API.Web.Services
{
    public class SendNewUserNotificationService : ISendNewUserNotificationService
    {
        private readonly IEmailService _emailSender;
        private readonly AppSettings _appSettings;

        public SendNewUserNotificationService(IEmailService emailSender, IOptionsMonitor<AppSettings> appSettings)
        {
            _emailSender = emailSender;
            _appSettings = appSettings.CurrentValue;
        }

        public async Task SendMessage(ApplicationUser applicationUser)
        {
            var to = _appSettings.EmailSender.ToEmail;
            var subject = $"New User Registered: {applicationUser.UserName}";

            var body = $"<html><body><b>New User Registered:</b> {applicationUser.Email}.<br /><br /><a href='https://api.ajt.io' target='_blank'>Go to API.</a></body></html>";

           await _emailSender.SendEmailAsync(to, subject, body);
        }
    }
}
