using System;
using System.Threading.Tasks;
using Babou.API.Web.Data;
using Babou.API.Web.Helpers;
using Babou.API.Web.Models.Database;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Babou.API.Web.Controllers
{
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public class GoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GoController> _logger;

        public GoController(ApplicationDbContext context, ILogger<GoController> logger)
        {
            _context = context;
            _logger = logger;
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

                var click = new ShortenedUrlClick()
                {
                    ShortenedUrlId = shortenedUrl.Id,
                    ClickDate = DateTime.Now,
                    Referrer = referer
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