using AJT.API.Helpers;
using AJT.API.Helpers.Filters;
using AJT.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AJT.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class TaterController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _accessor;

        public TaterController(IOptionsMonitor<AppSettings> appSettings, IHttpContextAccessor accessor)
        {
            _appSettings = appSettings.CurrentValue;
            _accessor = accessor;
        }

        [ServiceFilter(typeof(AuthKeyAuthorize))]
        [ServiceFilter(typeof(ClientIpCheckFilter))]
        [HttpGet]
        public IActionResult GetAppSettings()
        {
            return new OkObjectResult(_appSettings);
        }

        [ServiceFilter(typeof(AuthKeyAuthorize))]
        [HttpGet]
        public IActionResult RefreshKeyVault()
        {
            Startup.IConfigurationRoot.Reload();
            return new OkResult();
        }

        [HttpGet]
        public IActionResult WhatsMyIp()
        {
            return new OkObjectResult(_accessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
        }
    }
}