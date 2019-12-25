using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Babou.API.Web.Helpers;
using Babou.API.Web.Helpers.ExtensionMethods;
using Babou.API.Web.Helpers.Filters;
using Babou.API.Web.Models;
using Babou.API.Web.Models.Database;
using Babou.API.Web.Services.Interfaces;
using Babou.API.Web.SwaggerExamples.Responses;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PushBulletSharp.Core;
using PushBulletSharp.Core.Models.Requests;
using PushBulletSharp.Core.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Babou.API.Web.Areas.API
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PushBulletController : ControllerBase
    {
        private readonly ILogger<PushBulletController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPushBulletAppService _pushBulletAppService;

        public PushBulletController(ILogger<PushBulletController> logger, UserManager<ApplicationUser> userManager, IPushBulletAppService pushBulletAppService)
        {
            _logger = logger;
            _userManager = userManager;
            _pushBulletAppService = pushBulletAppService;
        }

        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost]
        [Consumes(Constants.ContentTypes.ApplicationJson)]
        [Produces(Constants.ContentTypes.ApplicationJson)]
        [SwaggerOperation(
            Summary = "Send messages via PushBullet.",
            Description = "Requires user to enter their ApiKey and EncryptionKey in their user profile.",
            OperationId = "SendPushBulletMessage")]
        [SwaggerResponse(StatusCodes.Status200OK, Type=typeof(List<PushResponse>), Description = "Successfully sent message to device(s) or channel.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "You cannot send to both devices and channel at the same time.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status409Conflict, typeof(ConflictErrorResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Either the request body is malformed or an exception was thrown.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestStringResponseExample))]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable, "AuthKey not found.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(AuthKeyNotFoundResponseExample))]
        public async Task<IActionResult> Post([FromBody] PushBullet pushBulletModel)
        {
            var userAuthKey = Request.Headers["AuthKey"].ToString();

            var user = _userManager.FindByApiAuthKeyAsync(userAuthKey);

            try
            {
                var pushBulletUserSettings = await _pushBulletAppService.RetrieveDecryptedPushBulletSettingsByUserId(user.Id);

                var pushBulletApiKey = pushBulletUserSettings.ApiKey;
                var pushBulletEncryptionKey = pushBulletUserSettings.EncryptionKey;

                if (pushBulletApiKey.IsNullOrEmpty())
                    return new BadRequestObjectResult("PushBullet Api Key cannot be found");

                if (pushBulletEncryptionKey.IsNullOrEmpty())
                    return new BadRequestObjectResult("PushBullet Encryption Key cannot be found");

                var client = new PushBulletClient(pushBulletApiKey, pushBulletEncryptionKey, TimeZoneInfo.Local);

                if (!string.IsNullOrEmpty(pushBulletModel.Channel) && (pushBulletModel.DeviceNickNames == null || !pushBulletModel.DeviceNickNames.Any()))
                {
                    var sendToChannel = await SendToChannel(client, pushBulletModel, user.Id);

                    return new OkObjectResult(sendToChannel);
                }

                if (string.IsNullOrEmpty(pushBulletModel.Channel) && pushBulletModel.DeviceNickNames != null && pushBulletModel.DeviceNickNames.Any())
                {
                    var sendToDevices = await SendToDevices(client, pushBulletModel, user.Id);

                    return new OkObjectResult(sendToDevices);
                }

                return new ConflictObjectResult("Sending messages to devices and channels on the same request is not supported.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to send PushBullet messages for {UserId} using the PushBullet model {@PushBullet}", user.Id, pushBulletModel);
                return new BadRequestObjectResult(ex);
            }
        }

        private async Task<List<PushResponse>> SendToChannel(PushBulletClient client, PushBullet pushBulletModel, string userId)
        {
            var responses = new List<PushResponse>();

            if (string.IsNullOrEmpty(pushBulletModel.Url))
            {
                var pushNoteRequest = new PushNoteRequest
                {
                    ChannelTag = pushBulletModel.Channel,
                    Title = pushBulletModel.Title,
                    Body = pushBulletModel.Body
                };

                var pushNoteResponse = await client.PushNote(pushNoteRequest);

                _logger.LogInformation("PushBullet Sent Message by {UserId}.", userId);

                responses.Add(pushNoteResponse);
            }
            else
            {
                var pushLinkRequest = new PushLinkRequest
                {
                    ChannelTag = pushBulletModel.Channel,
                    Title = pushBulletModel.Title,
                    Url = pushBulletModel.Url,
                    Body = pushBulletModel.Body
                };

                var pushLinkResponse = await client.PushLink(pushLinkRequest);

                _logger.LogInformation("PushBullet Sent Link Message by {UserId}.", userId);

                responses.Add(pushLinkResponse);
            }

            return responses;
        }

        private async Task<List<PushResponse>> SendToDevices(PushBulletClient client, PushBullet pushBulletModel, string userId)
        {
            var devices = await client.CurrentUsersDevices();
            var deviceToSend = devices.Devices.Where(x => pushBulletModel.DeviceNickNames.Contains(x.Nickname)).ToList();

            if (!deviceToSend.Any())
                throw new NullReferenceException($"No device nicknames matched {string.Join(", ", pushBulletModel.DeviceNickNames)}");

            var responses = new List<PushResponse>();

            deviceToSend.ForEach(async device =>
            {
                if (string.IsNullOrEmpty(pushBulletModel.Url))
                {
                    var pushNoteRequest = new PushNoteRequest
                    {
                        DeviceIden = device.Iden,
                        Title = pushBulletModel.Title,
                        Body = pushBulletModel.Body
                    };

                    var pushNoteResponse = await client.PushNote(pushNoteRequest);

                    _logger.LogInformation("PushBullet Sent Message by {UserId}.", userId);

                    responses.Add(pushNoteResponse);
                }
                else
                {
                    var pushLinkRequest = new PushLinkRequest
                    {
                        DeviceIden = device.Iden,
                        Title = pushBulletModel.Title,
                        Url = pushBulletModel.Url,
                        Body = pushBulletModel.Body
                    };

                    var pushLinkResponse = await client.PushLink(pushLinkRequest);

                    _logger.LogInformation("PushBullet Sent Link Message by {UserId}.", userId);

                    responses.Add(pushLinkResponse);
                }
            });

            return responses;
        }
    }
}