using System;
using System.Text;
using System.Threading.Tasks;
using AJT.API.Helpers;
using AJT.API.Models;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PushBulletSharp.Core;
using PushBulletSharp.Core.Models.Requests;

namespace AJT.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class PushBulletController : ControllerBase
    {
        private readonly ILogger<PushBulletController> _logger;
        private readonly AppSettings _appSettings;

        public PushBulletController(ILogger<PushBulletController> logger, IOptionsMonitor<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.CurrentValue;
        }

        [ServiceFilter(typeof(AuthKeyAuthorize))]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> SendAppVeyorMessage()
        {
            try
            {
                var pushBulletApiKey = _appSettings.PushBullet.ApiKey;
                var pushBulletEncryptionKey = _appSettings.PushBullet.EncryptionKey;

                if (pushBulletApiKey.IsNullOrEmpty())
                    return new BadRequestObjectResult("PushBelletApiKey cannot be found");

                if (pushBulletEncryptionKey.IsNullOrEmpty())
                    return new BadRequestObjectResult("PushBulletEncryptionKey cannot be found");

                var requestBody = await Request.GetRawBodyStringAsync();

                var appVeyor = JsonConvert.DeserializeObject<AppVeyorCustom>(requestBody);

                _logger.LogInformation("AppVeyor Request Built: {@AppVeyor}", appVeyor);

                var client = new PushBulletClient(pushBulletApiKey, pushBulletEncryptionKey, TimeZoneInfo.Local);

                if (appVeyor.Channel.IsNullOrEmpty())
                    return new BadRequestObjectResult($"Unknown channel from project {appVeyor.ProjectName}");

                var pushLinkRequest = new PushLinkRequest
                {
                    ChannelTag = appVeyor.Channel,
                    Title = appVeyor.Title,
                    Body = appVeyor.Body,
                    Url = appVeyor.Url
                };

                var pushLinkResponse = await client.PushLink(pushLinkRequest);

                _logger.LogInformation("PushBullet Sent Link Message. {@PushLinkRequest} {@PushLinkResponse}", pushLinkRequest, pushLinkResponse);

                return new OkObjectResult(pushLinkResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending PushBullet message");
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}