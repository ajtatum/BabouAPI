using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AJT.API.Web.Data;
using AJT.API.Web.Helpers;
using AJT.API.Web.Helpers.Filters;
using AJT.API.Web.Models;
using AJT.API.Web.Services.Interfaces;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AJT.API.Web.Areas.API
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [AllowAnonymous]
    public class UrlShortenerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUrlShortenerService _urlShortenerService;
        private readonly ILogger<UrlShortenerController> _logger;
        private readonly AppSettings _appSettings;

        public UrlShortenerController(ApplicationDbContext context, IUrlShortenerService urlShortenerService,
            ILogger<UrlShortenerController> logger, IOptionsMonitor<AppSettings> appSettings)
        {
            _context = context;
            _urlShortenerService = urlShortenerService;
            _logger = logger;
            _appSettings = appSettings.CurrentValue;
        }

        /// <summary>
        /// Converts a long URL to a short URL
        /// </summary>
        /// <param name="longUrl">The URL you wish to shorten</param>
        /// <param name="domain">The domain to use. Options are https://babou.io/ and https://mrvl.co/. Default is https://babou.io/ </param>
        /// <param name="token">You can specify the token you want to use if available or let the app generate a token for you.</param>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost]
        [Produces("text/plain")]
        public async Task<IActionResult> Post([Required][FromQuery] string longUrl, [Optional][FromQuery] string domain, [Optional][FromQuery] string token)
        {
            var userAuthKey = Request.Headers["AuthKey"].ToString();
            var applicationUser = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.ApiAuthKey == userAuthKey);

            if (domain.IsNullOrWhiteSpace())
            {
                domain = Constants.ShortDomainUrls.BabouIo;
            }

            var allowedDomains = _appSettings.BaseShortenedUrls.ToList();
            if (!allowedDomains.Contains(domain))
            {
                return new BadRequestObjectResult($"Domain not available. Please choose from {string.Join(", ", allowedDomains)}.");
            }

            var tokenTaken = true;

            if (!token.IsNullOrWhiteSpace())
            {
                try
                {
                    tokenTaken = await _urlShortenerService.CheckIfTokenIsAvailable(token, domain);
                }
                catch (DuplicateNameException dne)
                {
                    _logger.LogError(dne, "UrlShortenerController: Token {Token} is already being used by domain {Domain}", token, domain);
                    return new BadRequestObjectResult(dne.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "UrlShortenerController: Error while trying to create custom token {Token} for domain {Domain}.", token, domain);
                    return new BadRequestObjectResult(ex.Message);
                }
            }

            var shortenedUrl = tokenTaken
                ? await _urlShortenerService.CreateByUserId(applicationUser.Id, longUrl, domain)
                : await _urlShortenerService.CreateByUserId(applicationUser.Id, longUrl, domain, token);

            _logger.LogInformation("UrlShortenerController: New Shortened Url Created for {LongUrl} as {ShortUrl}", shortenedUrl.LongUrl, shortenedUrl.ShortUrl);

            return new OkObjectResult(shortenedUrl.ShortUrl);
        }
    }
}
