using System.Threading.Tasks;
using Babou.API.Web.Models.Database;

namespace Babou.API.Web.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
        Task SendContactMessage(string fromEmail, string fromName, string subject, string htmlMessage);
        Task SendNewUserMessage(ApplicationUser applicationUser);
        Task SendQuickWelcomeMessage(ApplicationUser applicationUser);
    }
}
