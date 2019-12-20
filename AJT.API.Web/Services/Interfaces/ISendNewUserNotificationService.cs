using System.Threading.Tasks;
using AJT.API.Web.Models.Database;

namespace AJT.API.Web.Services.Interfaces
{
    public interface ISendNewUserNotificationService
    {
        Task SendMessage(ApplicationUser applicationUser);
    }
}
