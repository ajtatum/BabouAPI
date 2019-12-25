using System.Threading.Tasks;
using Babou.API.Web.Models.Database;

namespace Babou.API.Web.Services.Interfaces
{
    public interface ISendNewUserNotificationService
    {
        Task SendMessage(ApplicationUser applicationUser);
    }
}
