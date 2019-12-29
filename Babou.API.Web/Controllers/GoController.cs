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
                #region GetReferrer
                var referrerUrl = string.Empty;

                try
                {
                    if (!Request.Query["src"].ToString().IsNullOrWhiteSpace())
                    {
                        var srcReferrer = Request.Query["src"].ToString();
                        if (srcReferrer.TryGetUrl(out var url))
                        {
                            referrerUrl = url;
                        }
                    }
                }
                catch (ArgumentNullException argumentNullException)
                {
                    _logger.LogError(argumentNullException, "Error trying to recieve query string src.");
                }

                if (referrerUrl.IsNullOrWhiteSpace())
                    referrerUrl = Request.Headers["Referer"].ToString();

                try
                {
                    if (referrerUrl.IsNullOrEmpty())
                    {
                        var header = Request.GetTypedHeaders();
                        var uriReferer = header.Referer;

                        if (uriReferer != null)
                            referrerUrl = uriReferer.AbsoluteUri;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Attempt to retrieve header referer.");
                }

                if (referrerUrl.IsNullOrEmpty())
                    referrerUrl = null;

                referrerUrl = referrerUrl.WithMaxLength(500);
                #endregion

                #region GetIPAddress
                var ipAddress = _ipService.GetRemoteIp();
#if DEBUG
                ipAddress = "44.21.199.18";
#endif
                string city = null;
                string state = null;
                string country = null;
                Point locationPoint = null;

                try
                {
                    var ipAddressDetails = _ipService.GetIpAddressDetails(ipAddress);
                    city = ipAddressDetails.City;
                    state = ipAddressDetails.RegionCode;
                    country = ipAddressDetails.CountryCode;

                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                    var coordinate = new Coordinate(ipAddressDetails.Longitude, ipAddressDetails.Latitude);
                    locationPoint = geometryFactory.CreatePoint(coordinate);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error receiving IP Address Details from IP Stack.");
                }
                #endregion

                var click = new ShortenedUrlClick()
                {
                    ShortenedUrlId = shortenedUrl.Id,
                    ClickDate = DateTime.Now,
                    Referrer = referrerUrl,
                    City = city,
                    State = state,
                    Country = country,
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