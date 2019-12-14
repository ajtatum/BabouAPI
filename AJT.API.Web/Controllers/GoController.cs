using System;
using System.Threading.Tasks;
using AJT.API.Web.Data;
using AJT.API.Web.Models.Database;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace AJT.API.Web.Controllers
{
    [Route("[controller]")]
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
            var shortenedUrl = await _context.ShortenedUrls.FirstOrDefaultAsync(x => x.Id == id);

            if (shortenedUrl != null)
            {
                var click = new ShortenedUrlClick()
                {
                    ShortenedUrlId = id,
                    ClickDate = DateTime.Now,
                    Referrer = HttpContext.Request.Headers[HeaderNames.Referer].ToString().Truncate(500, false)
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