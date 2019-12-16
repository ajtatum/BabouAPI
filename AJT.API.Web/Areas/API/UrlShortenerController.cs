using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AJT.API.Web.Data;
using AJT.API.Web.Helpers.Filters;
using AJT.API.Web.Models;
using AJT.API.Web.Models.Database;
using AJT.API.Web.Services.Interfaces;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AJT.API.Web.Areas.API
{
    [Route("api/[controller]")]
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

        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost]
        public async Task<IActionResult> Post([Required][FromQuery] string longUrl, [Required][FromQuery] string domain, [Optional][FromQuery] string token)
        {
            var userAuthKey = Request.Headers["AuthKey"].ToString();
            var applicationUser = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.ApiAuthKey == userAuthKey);

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
                catch (Exception ex)
                {
                    _logger.LogError(ex, "UrlShortenerController: Error while trying to create custom token.");
                    return new BadRequestObjectResult(ex.Message);
                }
            }

            var shortenedUrl = tokenTaken
                ? await _urlShortenerService.CreateByUserId(applicationUser.Id, longUrl, domain)
                : await _urlShortenerService.CreateByUserId(applicationUser.Id, longUrl, token, domain);

            _logger.LogInformation("UrlShortenerController: New Shortened Url Created for {LongUrl} as {ShortUrl}", shortenedUrl.LongUrl, shortenedUrl.ShortUrl);

            return new OkObjectResult(shortenedUrl.ShortUrl);
        }
    }
}
