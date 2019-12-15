using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AJT.API.Web.Data;
using AJT.API.Web.Helpers.Filters;
using AJT.API.Web.Models.Database;
using AJT.API.Web.Services.Interfaces;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        public UrlShortenerController(ApplicationDbContext context, IUrlShortenerService urlShortenerService, ILogger<UrlShortenerController> logger)
        {
            _context = context;
            _urlShortenerService = urlShortenerService;
            _logger = logger;
        }

        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost]
        public async Task<IActionResult> Post([Required][FromQuery] string longUrl, [Optional][FromQuery] string token)
        {
            var userAuthKey = Request.Headers["AuthKey"].ToString();
            var applicationUser = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.ApiAuthKey == userAuthKey);

            if (token.IsNullOrWhiteSpace())
                token = null;

            if (token != null)
            {
                var tokenTaken = await _context.ShortenedUrls.FirstOrDefaultAsync(x => x.Id == token);
                if (tokenTaken != null)
                    return new BadRequestObjectResult($"The token {token} has already been reserved.");
            }

            var shortenedUrl = token != null 
                ? await _urlShortenerService.CreateByUserId(applicationUser.Id, longUrl, token) 
                : await _urlShortenerService.CreateByUserId(applicationUser.Id, longUrl);

            _logger.LogInformation("UrlShortenerController: New Shortened Url Created for {LongUrl} as {ShortUrl}", shortenedUrl.LongUrl, shortenedUrl.ShortUrl);

            return new OkObjectResult(shortenedUrl.ShortUrl);
        }
    }
}
