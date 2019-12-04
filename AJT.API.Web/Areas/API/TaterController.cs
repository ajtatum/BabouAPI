using AJT.API.Web.Helpers.Filters;
using AJT.API.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AJT.API.Web.Areas.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TaterController : ControllerBase
    {
        private readonly AppSettings _appSettings;

        public TaterController(IOptionsMonitor<AppSettings> appSettings)
        {
            _appSettings = appSettings.CurrentValue;
        }

        [ServiceFilter(typeof(AuthKeyFilter))]
        [ServiceFilter(typeof(ClientIpFilter))]
        [HttpGet]
        public IActionResult GetAppSettings()
        {
            return new OkObjectResult(_appSettings);
        }

        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpGet]
        public IActionResult RefreshKeyVault()
        {
            Startup.ConfigurationRoot.Reload();
            return new OkResult();
        }
    }
}