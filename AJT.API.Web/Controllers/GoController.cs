using System;
using System.Threading.Tasks;
using Babou.API.Web.Data;
using Babou.API.Web.Helpers;
using Babou.API.Web.Models.Database;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace Babou.API.Web.Controllers
{
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public class GoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var shortenedUrl = await _context.ShortenedUrls.FirstOrDefaultAsync(x => x.Token == id && x.Domain == Constants.ShortDomainUrls.ApiAjtGo);

            if (shortenedUrl != null)
            {
                var click = new ShortenedUrlClick()
                {
                    ShortenedUrlId = shortenedUrl.Id,
                    ClickDate = DateTime.Now,
                    Referrer = HttpContext.Request.Headers[HeaderNames.Referer].ToString().WithMaxLength(500)
                };
                _context.Add(click);
                await _context.SaveChangesAsync();

                return new RedirectResult(shortenedUrl.LongUrl, false);
            }
            else
                return new BadRequestObjectResult($"Unable to match ShortUrl.");
        }
    }
}