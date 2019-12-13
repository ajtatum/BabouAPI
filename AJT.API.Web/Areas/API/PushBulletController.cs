using System;
using System.Threading.Tasks;
using AJT.API.Web.Helpers.ExtensionMethods;
using AJT.API.Web.Helpers.Filters;
using AJT.API.Web.Models;
using AJT.API.Web.Models.Database;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PushBulletSharp.Core;
using PushBulletSharp.Core.Models.Requests;

namespace AJT.API.Web.Areas.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class PushBulletController : ControllerBase
    {
        private readonly ILogger<PushBulletController> _logger;
        private readonly AppSettings _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public PushBulletController(ILogger<PushBulletController> logger, IOptionsMonitor<AppSettings> appSettings, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _appSettings = appSettings.CurrentValue;
            _userManager = userManager;
        }

        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> SendAppVeyorMessage()
        {
            Request.EnableBuffering();
            var requestBody = await Request.GetRawBodyStringAsyncWithOptions(null, null, true);
            var userAuthKey = Request.Headers["AuthKey"].ToString();

            var user = _userManager.FindByApiAuthKeyAsync(userAuthKey);
            _logger.LogInformation("Found user {User}", user.UserName);

            try
            {
                var pushBulletApiKey = _appSettings.PushBullet.ApiKey;
                var pushBulletEncryptionKey = _appSettings.PushBullet.EncryptionKey;

                if (pushBulletApiKey.IsNullOrEmpty())
                    return new BadRequestObjectResult("PushBelletApiKey cannot be found");

                if (pushBulletEncryptionKey.IsNullOrEmpty())
                    return new BadRequestObjectResult("PushBulletEncryptionKey cannot be found");

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
                _logger.LogError(ex, "Error sending PushBullet message. Request Body: {RequestBody}", requestBody);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}