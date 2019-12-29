using System.Linq;
using Babou.API.Web.Helpers.Filters;
using Babou.API.Web.Models;
using Babou.API.Web.Services.Interfaces;
using MaxMind.GeoIP2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Babou.API.Web.Areas.API
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TaterController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<TaterController> _logger;
        private readonly IIpService _ipService;

        public TaterController(IOptionsMonitor<AppSettings> appSettings, ILogger<TaterController> logger, IIpService ipService)
        {
            _appSettings = appSettings.CurrentValue;
            _logger = logger;
            _ipService = ipService;
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

        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpGet]
        public IActionResult GetMaxMindIP(string ipAddress)
        {
            using var reader = new DatabaseReader(_appSettings.MaxMindDb);

            var city = reader.City(ipAddress);

            _logger.LogInformation("Country ISO Code: {CountryCode}", city.Country.IsoCode);
            _logger.LogInformation("Country Name: {CountyName}", city.Country.Name);

            _logger.LogInformation("Full State Name: {StateName}", city.MostSpecificSubdivision.Name);
            _logger.LogInformation("State Abbreviation: {StateAbbv}", city.MostSpecificSubdivision.IsoCode);

            _logger.LogInformation("City Name: {City}", city.City.Name);

            _logger.LogInformation("Postal Code: {PostalCode}", city.Postal.Code);

            _logger.LogInformation("Latitude: {Latitude}", city.Location.Latitude); 
            _logger.LogInformation("Longitude: {Longitude}", city.Location.Longitude);

            return new OkResult();
        }
    }
}