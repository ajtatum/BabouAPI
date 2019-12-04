using System.ComponentModel.DataAnnotations;
using AJT.API.Web.Helpers.Filters;
using AJT.API.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AJT.API.Web.Areas.API
{
    [Route("api/[controller]/[action]")]
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
        public IActionResult WhatsMyIp()
        {
            return new OkObjectResult(_ipService.GetRemoteIp());
        }

        [HttpGet]
        [ServiceFilter(typeof(AuthKeyFilter))]
        public IActionResult GetIpAddressDetails()
        {
            var ipAddress = _ipService.GetRemoteIp();
            var ipAddressDetails = _ipService.GetIpAddressDetails(ipAddress);

            return new OkObjectResult(ipAddressDetails);
        }

        [HttpGet]
        [ServiceFilter(typeof(AuthKeyFilter))]
        public IActionResult GetIpAddressDetails([Required] string ipAddress)
        {
            var ipAddressDetails = _ipService.GetIpAddressDetails(ipAddress);

            return new OkObjectResult(ipAddressDetails);
        }
    }
}