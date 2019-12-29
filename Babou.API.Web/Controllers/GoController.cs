using System;
using System.Threading.Tasks;
using Babou.API.Web.Data;
using Babou.API.Web.Helpers;
using Babou.API.Web.Models;
using Babou.API.Web.Models.Database;
using Babou.API.Web.Services.Interfaces;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Babou.API.Web.Controllers
{
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public class GoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GoController> _logger;
        private readonly AppSettings _appSettings;
        private readonly IIpService _ipService;

        public GoController(ApplicationDbContext context, ILogger<GoController> logger, IOptionsMonitor<AppSettings> appSettings, IIpService ipService)
        {
            _context = context;
            _logger = logger;
            _appSettings = appSettings.CurrentValue;
            _ipService = ipService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var shortenedUrl = await _context.ShortenedUrls.FirstOrDefaultAsync(x => x.Token == id && x.Domain == Constants.ShortDomainUrls.BabouIoGo);

            if (shortenedUrl != null)
            {
                var referer = Request.Headers["Referer"].ToString();

                try
                {
                    if (referer.IsNullOrEmpty())
                    {
                        var header = Request.GetTypedHeaders();
                        var uriReferer = header.Referer;

                        if (uriReferer != null)
                            referer = uriReferer.AbsoluteUri;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Attempt to retrieve header referer.");
                }

                if (referer.IsNullOrEmpty())
                    referer = null;

                referer = referer.WithMaxLength(500);

                var ipAddress = _ipService.GetRemoteIp();
#if DEBUG
                ipAddress = "44.21.199.18";
#endif
                var ipAddressDetails = _ipService.GetIpAddressDetails(ipAddress);

                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var coordinate = new Coordinate(ipAddressDetails.Longitude, ipAddressDetails.Latitude);
                var locationPoint = geometryFactory.CreatePoint(coordinate);

                var click = new ShortenedUrlClick()
                {
                    ShortenedUrlId = shortenedUrl.Id,
                    ClickDate = DateTime.Now,
                    Referrer = referer,
                    City = ipAddressDetails.City,
                    State = ipAddressDetails.RegionName,
                    Country = ipAddressDetails.CountryName,
                    Geography = locationPoint
                };

                _context.Add(click);
                await _context.SaveChangesAsync();

                return new RedirectResult(shortenedUrl.LongUrl, false);
            }
            else
                return new BadRequestObjectResult($"Unable to match ShortUrl.");
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new RedirectToPageResult("/Index");
        }
    }
}