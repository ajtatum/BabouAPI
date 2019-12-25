using System.ComponentModel.DataAnnotations;
using Babou.API.Web.Helpers;
using Babou.API.Web.Helpers.Filters;
using Babou.API.Web.Services.Interfaces;
using IpStack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Babou.API.Web.Areas.API
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class IpAddressController : ControllerBase
    {
        private readonly IIpService _ipService;

        public IpAddressController(IIpService ipService)
        {
            _ipService = ipService;
        }

        [HttpGet]
        [Produces(Constants.ContentTypes.TextPlain)]
        [SwaggerOperation(
            Summary = "Gets your IP Address.",
            OperationId = "WhatsMyIP")]
        public IActionResult WhatsMyIp()
        {
            return new OkObjectResult(_ipService.GetRemoteIp());
        }

        [HttpGet]
        [ServiceFilter(typeof(AuthKeyFilter))]
        [Produces(Constants.ContentTypes.ApplicationJson, Type = typeof(IpAddressDetails))]
        [SwaggerOperation(
            Summary = "Returns your IP Address details",
            Description = "Requires user to enter their ApiKey and EncryptionKey in their user profile.",
            OperationId = "GetMyIpAddressDetails")]
        public IActionResult GetMyIpAddressDetails()
        {
            var ipAddress = _ipService.GetRemoteIp();
            var ipAddressDetails = _ipService.GetIpAddressDetails(ipAddress);

            return new OkObjectResult(ipAddressDetails);
        }

        [HttpGet]
        [ServiceFilter(typeof(AuthKeyFilter))]
        [Produces(Constants.ContentTypes.ApplicationJson, Type = typeof(IpAddressDetails))]
        [SwaggerOperation(
            Summary = "Returns an IP Address details.",
            Description = "Requires user to enter their ApiKey and EncryptionKey in their user profile.",
            OperationId = "GetIpAddressDetails")]
        public IActionResult GetIpAddressDetails([Required] string ipAddress)
        {
            var ipAddressDetails = _ipService.GetIpAddressDetails(ipAddress);

            return new OkObjectResult(ipAddressDetails);
        }
    }
}