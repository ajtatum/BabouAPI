using System.Threading.Tasks;
using AJT.API.Models;

namespace AJT.API.Web.Services.Interfaces
{
    public interface ISlackService
    {
        void SendBotMessage();
        Task GetMarvel(SlackCommandRequest slackCommandRequest);
        Task GetDcComics(SlackCommandRequest slackCommandRequest);
    }
}
