using System.Threading.Tasks;
using AJT.API.Web.Models;
using AJT.API.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace AJT.API.Web.Services
{
    public class SendNewUserNotificationService : ISendNewUserNotificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly AppSettings _appSettings;

        public SendNewUserNotificationService(IEmailSender emailSender, IOptionsMonitor<AppSettings> appSettings)
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
