using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AJT.API.Helpers;
using AJT.API.Models;
using AJT.API.Services;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AJT.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SlackController : ControllerBase
    {
        private readonly ILogger<SlackController> _logger;
        private readonly AppSettings _appSettings;
        private readonly SlackService _slackService;

        public SlackController(ILogger<SlackController> logger, IOptionsMonitor<AppSettings> appSettings, SlackService slackService)
        {
            _logger = logger;
            _appSettings = appSettings.CurrentValue;
            _slackService = slackService;
        }

        [HttpPost]
        public async Task<IActionResult> StanLeeBotCommandAsync()
        {
            Request.EnableBuffering();
            var requestBody = await Request.GetRawBodyStringAsyncWithOptions(null, null, true);
            var slackSignature = Request.Headers["X-Slack-Signature"].ToString();
            var slackRequestTimeStamp = Request.Headers["X-Slack-Request-Timestamp"].ToString();

            var fromSlack = IsRequestFromSlack(slackSignature, slackRequestTimeStamp, requestBody);

            if (!fromSlack)
                return new BadRequestObjectResult("Request is not from Slack");

            var requestForm = await Request.ReadFormAsync();
            var slackCommandRequest = new SlackCommandRequest()
            {
                Token = requestForm["token"],
                TeamId = requestForm["team_id"],
                TeamDomain = requestForm["team_domain"],
                ChannelId = requestForm["channel_id"],
                ChannelName = requestForm["channel_name"],
                UserId = requestForm["user_id"],
                UserName = requestForm["user_name"],
                Command = requestForm["command"],
                Text = requestForm["text"],
                ResponseUrl = requestForm["response_url"],
                TriggerId = requestForm["trigger_id"]
            };

            _logger.LogInformation("SlackCommandRequest: {@SlackCommandRequest}", slackCommandRequest);

            switch (slackCommandRequest.Command)
            {
                case "/marvel":
                    await _slackService.GetMarvel(slackCommandRequest);
                    return new OkResult();
                case "/dc":
                    await _slackService.GetDcComics(slackCommandRequest);
                    return new OkResult();
                default:
                    return new BadRequestResult();
            }
        }

        private bool IsRequestFromSlack(string slackSignature, string slackRequestTimeStamp, string requestBody)
        {
            var baseString = $"v0:{slackRequestTimeStamp}:{requestBody}";
            var signingSecret = _appSettings.Slack.SigningSecret;
            var computedHash = Utilities.GetHash(baseString, signingSecret);

            return $"v0={computedHash}" == slackSignature;
        }
    }
}